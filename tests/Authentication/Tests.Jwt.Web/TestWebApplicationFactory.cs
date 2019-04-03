using System.Collections.Generic;
using EMG.Extensions.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    public class TestWebApplicationFactory : WebApplicationFactory<TestStartup>
    {
        private string _authenticationEndpoint;
        public string Username { get; set; }

        public string Password { get; set; }

        public string AuthenticationEndpoint
        {
            get => _authenticationEndpoint;
            set => _authenticationEndpoint = value.StartsWith("/") ? value : $"/{value}";
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder<TestStartup>(new string[0]).ConfigureAppConfiguration(ConfigureConfiguration);
        }

        private void ConfigureConfiguration(WebHostBuilderContext context, IConfigurationBuilder builder)
        {
            var settings = new Dictionary<string, string>
            {
                ["JWT:SecretKey"] = "test-key-test-key-test-key",
                ["JWT:IssuerOptions:Issuer"] = "Test API",
                ["JWT:IssuerOptions:Audience"] = "https://localhost:5001",
                ["JWT:Client:Username"] = Username,
                ["JWT:Client:Password"] = Password,
                ["JWT:AuthenticationEndpoint"] = AuthenticationEndpoint
            };

            builder.AddInMemoryCollection(settings);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(".");
            base.ConfigureWebHost(builder);
        }
    }

    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddJwtAuthentication(Configuration)
                    .RequireAuthentication()
                    .AddBasicUserAuthenticator()
                    .AddFormUserExtractor();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }


            app.UseJwtAuthentication();


            app.UseHttpsRedirection();

            app.UseMvc();
        }

    }

    [ApiController]
    public class TestController
    {
        [HttpGet("{text}")]
        public ActionResult<string> Get(string text) => text;
    }
}