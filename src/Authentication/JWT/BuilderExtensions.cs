using System;
using System.ComponentModel.DataAnnotations;
using EMG.Extensions.AspNetCore.Authenticators;
using EMG.Extensions.AspNetCore.UserExtractors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EMG.Extensions.AspNetCore
{
    public static class BuilderExtensions
    {
        public static IJwtBuilder AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration, string sectionKey = "JWT")
        {
            var section = configuration.GetSection(sectionKey);

            var options = section.Get<JwtOptions>() ?? new JwtOptions();

            Validator.ValidateObject(options, new ValidationContext(options));

            services.AddSingleton(options);

            var authenticationBuilder = services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            authenticationBuilder.AddJwtBearer(opt =>
            {
                opt.IncludeErrorDetails = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = options.ValidateIssuer,
                    ValidIssuer = options.IssuerOptions.Issuer,

                    ValidateAudience = options.ValidateAudience,
                    ValidAudience = options.IssuerOptions.Audience,

                    ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
                    IssuerSigningKey = options.SecurityKey,

                    RequireExpirationTime = options.RequireExpirationTime,
                    ValidateLifetime = options.ValidateLifetime,

                    ClockSkew = options.AllowedClockSkew
                };
            });

            return new JwtBuilder(services, configuration);
        }

        public static void UseJwtAuthentication(this IApplicationBuilder app)
        {
            app.UseAuthentication();

            var options = app.ApplicationServices.GetRequiredService<JwtOptions>();

            app.Map(options.AuthenticationEndpoint, a => a.UseMiddleware<JwtMiddleware>());
        }

        public static IJwtBuilder AddJwtBuilderAction(this IJwtBuilder builder, Action<IServiceCollection, IConfiguration> action)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action(builder.Services, builder.Configuration);

            return builder;
        }

        public static IJwtBuilder AddBasicUserAuthenticator(this IJwtBuilder builder, string sectionKey = "JWT:Client")
        {
            return builder.AddJwtBuilderAction((services, configuration) =>
            {
                var credentials = configuration.GetSection(sectionKey).Get<BasicCredentials>();

                Validator.ValidateObject(credentials, new ValidationContext(credentials));

                services.AddSingleton(credentials);

                services.AddSingleton<IUserAuthenticator, BasicUserAuthenticator>();
            });

        }

        public static IJwtBuilder AddUserExtractor<TExtractor>(this IJwtBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TExtractor : class, IUserExtractor
        {
            return builder.AddJwtBuilderAction((services, configuration) => 
            {
                services.Add(new ServiceDescriptor(typeof(IUserExtractor), typeof(TExtractor), lifetime));
            });
        }

        public static IJwtBuilder AddFormUserExtractor(this IJwtBuilder builder)
        {
            return builder.AddUserExtractor<FormUserExtractor>();
        }

        public static IJwtBuilder RequireAuthentication(this IJwtBuilder builder, Func<bool> predicate = null)
        {
            if (predicate?.Invoke() ?? true)
            {
                builder.Services.Configure<MvcOptions>(options =>
                {
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                });
            }

            return builder;
        }
    }
}