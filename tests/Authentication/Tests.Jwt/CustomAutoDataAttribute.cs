using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Moq;

namespace Tests
{
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute() : base(CreateFixture)
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

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            fixture.Customize<HttpResponse>(r => r.Without(p => p.Body));

            fixture.Register(() => Mock.Of<HttpRequest>());

            fixture.Register(() => Mock.Of<HttpResponse>());

            return fixture;
        }
    }
}