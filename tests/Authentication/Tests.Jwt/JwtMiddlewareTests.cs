using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Extensions.AspNetCore;
using EMG.Extensions.AspNetCore.Authenticators;
using EMG.Extensions.AspNetCore.UserExtractors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class JwtMiddlewareTests
    {
        [Test, BasicAutoData]
        public void Constructor_is_guarded_against_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(JwtMiddleware).GetConstructors());
        }

        [Test, BasicAutoData]
        public async Task Invoke_returns_401_if_no_match([Frozen] IUserExtractor extractor, JwtMiddleware sut, [Frozen] IServiceProvider serviceProvider, HttpContext context, JwtOptions options, IActionResultExecutor<ObjectResult> executor)
        {
            User user = null;

            Mock.Get(extractor).Setup(p => p.TryExtractUser(It.IsAny<HttpContext>(), out user)).Returns(false);

            Mock.Get(serviceProvider).Setup(p => p.GetService(typeof(IActionResultExecutor<ObjectResult>))).Returns(executor);

            await sut.Invoke(context, options);

            Mock.Get(executor).Verify(p => p.ExecuteAsync(It.IsAny<ActionContext>(), It.Is<ObjectResult>(or => or.StatusCode == 401)));
        }

        [Test, BasicAutoData]
        public async Task Invoke_returns_200_with_token_if_match([Frozen] IUserAuthenticator authenticator, [Frozen] IUserExtractor extractor, JwtMiddleware sut, [Frozen] IServiceProvider serviceProvider, HttpContext context, JwtOptions options, IActionResultExecutor<ObjectResult> executor, User user, ClaimsIdentity identity)
        {
            Mock.Get(extractor).Setup(p => p.TryExtractUser(It.IsAny<HttpContext>(), out user)).Returns(true);

            Mock.Get(authenticator).Setup(p => p.TryAuthenticateUserAsync(It.IsAny<User>(), out identity)).ReturnsAsync(true);

            Mock.Get(serviceProvider).Setup(p => p.GetService(typeof(IActionResultExecutor<ObjectResult>))).Returns(executor);

            await sut.Invoke(context, options);

            Mock.Get(executor).Verify(p => p.ExecuteAsync(It.IsAny<ActionContext>(), It.Is<ObjectResult>(or => or.StatusCode == 200 && or.Value is TokenModel)));
        }
    }
}