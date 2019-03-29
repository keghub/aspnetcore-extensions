using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using EMG.Extensions.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Tests
{
    public class BasicAutoDataAttribute : AutoDataAttribute
    {
        public BasicAutoDataAttribute() : base(CreateFixture)
        {

        }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true,
                GenerateDelegates = true
            });

            fixture.Customize<ExceptionContext>(e => e
                .With(p => p.ActionDescriptor, (ControllerActionDescriptor descriptor) => descriptor));

            return fixture;
        }
    }

    public class AutoCustomDataAttribute : AutoDataAttribute
    {
        public AutoCustomDataAttribute() : base(CreateFixture)
        {
            
        }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true,
                GenerateDelegates = true
            });

            fixture.Customize<Exception>(e => e.FromFactory((ExceptionInfo info) =>
            {
                var ex = new Exception();
                ex.Data.Add(ExceptionExtensions.ExceptionInfoKey, info);

                return ex;
            }));

            fixture.Customize<ExceptionContext>(e => e
                .With(p => p.ActionDescriptor, (ControllerActionDescriptor descriptor) => descriptor));

            return fixture;
        }
    }
}