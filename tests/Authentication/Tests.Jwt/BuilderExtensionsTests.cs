using AutoFixture.Idioms;
using EMG.Extensions.AspNetCore;
using EMG.Extensions.AspNetCore.Authenticators;
using EMG.Extensions.AspNetCore.UserExtractors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestFixture]
    public class BuilderExtensionsTests
    {
        [Test, BasicAutoData]
        public void AddJwtAuthentication_returns_JwtBuilder(IServiceCollection services, JwtOptions options)
        {
            var values = new Dictionary<string, string>
            {
                ["JWT:SecretKey"] = options.SecretKey,
                ["JWT:IssuerOptions:Issuer"] = options.IssuerOptions.Issuer,
                ["JWT:IssuerOptions:Audience"] = options.IssuerOptions.Audience
            };

            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(values);
            var configuration = configurationBuilder.Build();

            var result = BuilderExtensions.AddJwtAuthentication(services, configuration);

            Assert.That(result, Is.InstanceOf<JwtBuilder>());
            Assert.That(result.Configuration, Is.SameAs(configuration));
            Assert.That(result.Services, Is.SameAs(services));
        }

        [Test, BasicAutoData]
        public void AddJwtAuthentication_registers_options(IServiceCollection services, JwtOptions options)
        {
            var values = new Dictionary<string, string>
            {
                ["JWT:SecretKey"] = options.SecretKey,
                ["JWT:IssuerOptions:Issuer"] = options.IssuerOptions.Issuer,
                ["JWT:IssuerOptions:Audience"] = options.IssuerOptions.Audience
            };

            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(values);
            var configuration = configurationBuilder.Build();

            var result = BuilderExtensions.AddJwtAuthentication(services, configuration);

            Mock.Get(services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(JwtOptions) && sd.ImplementationInstance != null)));
        }

        [Test, BasicAutoData]
        public void AddJwtAuthentication_adds_support_for_authentication(IServiceCollection services, JwtOptions options)
        {
            var values = new Dictionary<string, string>
            {
                ["JWT:SecretKey"] = options.SecretKey,
                ["JWT:IssuerOptions:Issuer"] = options.IssuerOptions.Issuer,
                ["JWT:IssuerOptions:Audience"] = options.IssuerOptions.Audience
            };

            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(values);
            var configuration = configurationBuilder.Build();

            var result = BuilderExtensions.AddJwtAuthentication(services, configuration);

            Mock.Get(services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(IAuthenticationService) && sd.ImplementationType == typeof(AuthenticationService))));
        }

        [Test, BasicAutoData]
        public void AddJwtAuthentication_adds_support_for_JWT_bearer(IServiceCollection services, JwtOptions options)
        {
            var values = new Dictionary<string, string>
            {
                ["JWT:SecretKey"] = options.SecretKey,
                ["JWT:IssuerOptions:Issuer"] = options.IssuerOptions.Issuer,
                ["JWT:IssuerOptions:Audience"] = options.IssuerOptions.Audience
            };

            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(values);
            var configuration = configurationBuilder.Build();

            var result = BuilderExtensions.AddJwtAuthentication(services, configuration);

            Mock.Get(services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ImplementationType == typeof(JwtBearerHandler))));
        }

        [Test, BasicAutoData]
        public void AddJwtBuilderAction_is_guarded_against_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(BuilderExtensions).GetMethod(nameof(BuilderExtensions.AddJwtBuilderAction)));
        }

        [Test, BasicAutoData]
        public void AddJwtBuilderAction_invokes_given_delegate(IJwtBuilder builder, Action<IServiceCollection, IConfiguration> action)
        {
            BuilderExtensions.AddJwtBuilderAction(builder, action);

            Mock.Get(action).Verify(p => p(builder.Services, builder.Configuration));
        }

        [Test, BasicAutoData]
        public void AddJwtBuilderAction_returns_builder(IJwtBuilder builder, Action<IServiceCollection, IConfiguration> action)
        {
            var result = BuilderExtensions.AddJwtBuilderAction(builder, action);

            Assert.That(result, Is.SameAs(builder));
        }

        [Test, BasicAutoData]
        public void AddBasicUserAuthenticator_registers_options(IJwtBuilder builder, BasicCredentials credentials)
        {
            var values = new Dictionary<string, string>
            {
                ["JWT:Client:Username"] = credentials.Username,
                ["JWT:Client:Password"] = credentials.Password
            };

            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(values);
            var configuration = configurationBuilder.Build();

            Mock.Get(builder).SetupGet(p => p.Configuration).Returns(configuration);

            BuilderExtensions.AddBasicUserAuthenticator(builder);

            Mock.Get(builder.Services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ImplementationInstance is BasicCredentials)));
        }

        [Test, BasicAutoData]
        public void AddBasicUserAuthenticator_registers_BasicUserAuthenticator(IJwtBuilder builder, BasicCredentials credentials)
        {
            var values = new Dictionary<string, string>
            {
                ["JWT:Client:Username"] = credentials.Username,
                ["JWT:Client:Password"] = credentials.Password
            };

            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(values);
            var configuration = configurationBuilder.Build();

            Mock.Get(builder).SetupGet(p => p.Configuration).Returns(configuration);

            BuilderExtensions.AddBasicUserAuthenticator(builder);

            Mock.Get(builder.Services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(IUserAuthenticator) && sd.ImplementationType == typeof(BasicUserAuthenticator))));
        }

        [Test, BasicAutoData]
        public void AddFormUserExtractor_registers_FormUserExtractor(IJwtBuilder builder)
        {
            BuilderExtensions.AddFormUserExtractor(builder);

            Mock.Get(builder.Services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(IUserExtractor) && sd.ImplementationType == typeof(FormUserExtractor))));
        }

        [Test, BasicAutoData]
        public void RequireAuthentication_configures_mvc_if_no_delegate_is_provided(IJwtBuilder builder)
        {
            BuilderExtensions.RequireAuthentication(builder);

            Mock.Get(builder.Services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(IConfigureOptions<MvcOptions>))));
        }

        [Test, BasicAutoData]
        public void RequireAuthentication_configures_mvc_if_delegate_returns_true(IJwtBuilder builder, Func<bool> test)
        {
            Mock.Get(test).Setup(p => p()).Returns(true);

            BuilderExtensions.RequireAuthentication(builder, test);

            Mock.Get(builder.Services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(IConfigureOptions<MvcOptions>))));
        }

        [Test, BasicAutoData]
        public void RequireAuthentication_does_not_configure_mvc_if_delegate_returns_false(IJwtBuilder builder, Func<bool> test)
        {
            Mock.Get(test).Setup(p => p()).Returns(false);

            BuilderExtensions.RequireAuthentication(builder, test);

            Mock.Get(builder.Services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(IConfigureOptions<MvcOptions>))), Times.Never);
        }
    }
}
