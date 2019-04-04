using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture.NUnit3;
using EMG.Extensions.AspNetCore;
using EMG.Extensions.AspNetCore.UserExtractors;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace Tests.UserExtractors
{
    [TestFixture]
    public class FormUserExtractorTests
    {
        [Test, CustomAutoData]
        public void TryExtractUser_returns_false_if_method_is_not_post(FormUserExtractor sut, HttpContext httpContext)
        {
            Assume.That(httpContext.Request.Method, Is.Not.EqualTo("post").IgnoreCase);

            var result = sut.TryExtractUser(httpContext, out _);

            Assert.That(result, Is.False);
        }

        [Test, CustomAutoData]
        public void TryExtractUser_returns_false_if_body_does_not_contain_data(FormUserExtractor sut, HttpContext httpContext)
        {
            Mock.Get(httpContext.Request).SetupGet(p => p.Method).Returns(HttpMethods.Post);
            Mock.Get(httpContext.Request).SetupGet(p => p.HasFormContentType).Returns(false);

            var result = sut.TryExtractUser(httpContext, out _);

            Assert.That(result, Is.False);
        }

        [Test, CustomAutoData]
        public void TryExtractUser_returns_true_if_body_contain_data([Frozen] IFormCollection form, [Frozen] HttpRequest request, FormUserExtractor sut, HttpContext httpContext, User user)
        {
            Mock.Get(form).SetupGet(p => p["username"]).Returns(user.UserName);
            Mock.Get(form).SetupGet(p => p["password"]).Returns(user.Password);

            Mock.Get(request).SetupGet(p => p.Method).Returns(HttpMethods.Post);
            Mock.Get(request).SetupGet(p => p.HasFormContentType).Returns(true);
            Mock.Get(request).SetupGet(p => p.Form).Returns(form);

            var result = sut.TryExtractUser(httpContext, out _);

            Assert.That(result, Is.True);
        }

        [Test, CustomAutoData]
        public void TryExtractUser_outputs_user_if_body_contain_data([Frozen] IFormCollection form, [Frozen] HttpRequest request, FormUserExtractor sut, HttpContext httpContext, User user)
        {
            Mock.Get(form).SetupGet(p => p["username"]).Returns(user.UserName);
            Mock.Get(form).SetupGet(p => p["password"]).Returns(user.Password);

            Mock.Get(request).SetupGet(p => p.Method).Returns(HttpMethods.Post);
            Mock.Get(request).SetupGet(p => p.HasFormContentType).Returns(true);
            Mock.Get(request).SetupGet(p => p.Form).Returns(form);

            sut.TryExtractUser(httpContext, out var newUser);

            Assert.That(newUser.UserName, Is.EqualTo(user.UserName));
            Assert.That(newUser.Password, Is.EqualTo(user.Password));
        }
    }
}
