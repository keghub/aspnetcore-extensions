using System;
using EMG.Extensions.AspNetCore.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod

namespace Tests.Extensions
{
    [TestFixture]
    public class ExceptionExtensionsTests
    {
        [Test, AutoCustomData]
        public void DescribeError_returns_same_exception(EventId eventId, object state, Func<object, Exception, string> formatter)
        {
            var exception = new Exception();

            var result = ExceptionExtensions.DescribeError(exception, eventId, state, formatter);

            Assert.That(result, Is.SameAs(exception));
        }

        [Test, AutoCustomData]
        public void DescribeError_adds_state_to_exceptionInfo(EventId eventId, object state, Func<object, Exception, string> formatter)
        {
            var exception = new Exception();

            ExceptionExtensions.DescribeError(exception, eventId, state, formatter);

            Assume.That(exception.Data.Contains(ExceptionExtensions.ExceptionInfoKey));
            
            var exceptionInfo = exception.Data[ExceptionExtensions.ExceptionInfoKey] as ExceptionInfo;

            Assert.That(exceptionInfo, Is.Not.Null);
            Assert.That(exceptionInfo.State, Is.SameAs(state));
        }

        [Test, AutoCustomData]
        public void DescribeError_adds_eventId_to_exceptionInfo(EventId eventId, object state, Func<object, Exception, string> formatter)
        {
            var exception = new Exception();

            ExceptionExtensions.DescribeError(exception, eventId, state, formatter);

            Assume.That(exception.Data.Contains(ExceptionExtensions.ExceptionInfoKey));
            
            var exceptionInfo = exception.Data[ExceptionExtensions.ExceptionInfoKey] as ExceptionInfo;

            Assert.That(exceptionInfo, Is.Not.Null);
            Assert.That(exceptionInfo.EventId, Is.EqualTo(eventId));
        }

        [Test, AutoCustomData]
        public void DescribeError_adds_valid_logger_delegate_to_exceptionInfo(EventId eventId, object state, Func<object, Exception, string> formatter, ILogger logger)
        {
            var exception = new Exception();

            ExceptionExtensions.DescribeError(exception, eventId, state, formatter);

            Assume.That(exception.Data.Contains(ExceptionExtensions.ExceptionInfoKey));

            var exceptionInfo = exception.Data[ExceptionExtensions.ExceptionInfoKey] as ExceptionInfo;

            Assert.That(exceptionInfo, Is.Not.Null);
            Assert.That(exceptionInfo.Logger, Is.Not.Null);

            exceptionInfo.Logger(logger, eventId, state, exception, exceptionInfo.Formatter);

            Mock.Get(logger).Verify(o => o.Log(LogLevel.Error, eventId, state, exception, It.IsAny<Func<object, Exception, string>>()));
        }

        [Test, AutoCustomData]
        public void DescribeError_adds_valid_formatter_to_exceptionInfo(EventId eventId, object state, Func<object, Exception, string> formatter)
        {
            var exception = new Exception();

            ExceptionExtensions.DescribeError(exception, eventId, state, formatter);

            Assume.That(exception.Data.Contains(ExceptionExtensions.ExceptionInfoKey));
            
            var exceptionInfo = exception.Data[ExceptionExtensions.ExceptionInfoKey] as ExceptionInfo;

            Assert.That(exceptionInfo, Is.Not.Null);
            Assert.That(exceptionInfo.Formatter, Is.Not.Null);

            var formattedMessage = exceptionInfo.Formatter(state, exception);

            Mock.Get(formatter).Verify(p => p(state, exception));
        }

        [Test, AutoCustomData]
        public void TryExtractExceptionInfo_can_extract_ExceptionInfo_when_available(ExceptionInfo exceptionInfo)
        {
            var exception = new Exception();
            exception.Data.Add(ExceptionExtensions.ExceptionInfoKey, exceptionInfo);

            var result = ExceptionExtensions.TryExtractExceptionInfo(exception, out var info);

            Assert.That(result, Is.True);
            Assert.That(info, Is.SameAs(exceptionInfo));
        }

        [Test, AutoCustomData]
        public void TryExtractExceptionInfo_returns_false_if_ExceptionInfo_not_available()
        {
            var exception = new Exception();

            var result = ExceptionExtensions.TryExtractExceptionInfo(exception, out var info);

            Assert.That(result, Is.False);
            Assert.That(info, Is.Null);
        }
    }
}
