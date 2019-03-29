using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Extensions.AspNetCore.Extensions;
using EMG.Extensions.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NUnit.Framework;

namespace Tests.Filters
{
    [TestFixture]
    public class ExceptionHandlerFilterTests
    {
        [Test, AutoCustomData]
        public void Constructor_is_guarded_against_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ExceptionHandlerFilter).GetConstructors());
        }

        [Test, AutoCustomData]
        public void OnException_adds_an_error_model_if_exceptionInfo_is_provided(ExceptionHandlerFilter sut, [Frozen] ExceptionInfo exceptionInfo, ExceptionContext context, string formattedMessage)
        {
            Assume.That(context.Exception.Data[ExceptionExtensions.ExceptionInfoKey], Is.SameAs(exceptionInfo));

            Mock.Get(exceptionInfo.Formatter).Setup(p => p(exceptionInfo.State, context.Exception)).Returns(formattedMessage);

            sut.OnException(context);

            var result = context.Result as ObjectResult;

            Assert.That(result, Is.Not.Null);

            var error = result.Value as ErrorModel;

            Assert.That(error, Is.Not.Null);
            Assert.That(error.ErrorId, Is.EqualTo(exceptionInfo.EventId.Id));
            Assert.That(error.Error, Is.EqualTo(exceptionInfo.EventId.Name));
            Assert.That(error.Message, Is.EqualTo(formattedMessage));
            Assert.That(error.Data, Is.SameAs(exceptionInfo.State));
            Assert.That(error.AdditionalData, Is.Null.Or.Not.ContainKey(ExceptionExtensions.ExceptionInfoKey));
        }

        [Test, BasicAutoData]
        public void OnException_adds_simple_error_model_if_no_exceptionInfo_is_provided(ExceptionHandlerFilter sut, ExceptionContext context)
        {
            Assume.That(context.Exception.Data.Contains(ExceptionExtensions.ExceptionInfoKey), Is.False);

            sut.OnException(context);

            var result = context.Result as ObjectResult;

            Assert.That(result, Is.Not.Null);

            var error = result.Value as ErrorModel;

            Assert.That(error, Is.Not.Null);
            Assert.That(error.ErrorId, Is.EqualTo(0));
            Assert.That(error.Error, Is.Null);
            Assert.That(error.Message, Is.Not.Null);
            Assert.That(error.Data, Is.Null );
            Assert.That(error.AdditionalData, Is.Null.Or.Not.ContainKey(ExceptionExtensions.ExceptionInfoKey));

        }
    }
}
