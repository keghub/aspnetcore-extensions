using System.Linq;
using AutoFixture.Idioms;
using EMG.Extensions.AspNetCore;
using EMG.Extensions.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod

namespace Tests
{
    [TestFixture]
    public class SetupExtensionsTests
    {
        [Test, AutoCustomData]
        public void AddExceptionHandlerFilter_is_guarded_against_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(SetupExtensions).GetMethod(nameof(SetupExtensions.AddExceptionHandlerFilter)));
        }

        [Test, AutoCustomData]
        public void AddExceptionHandlerFilter_adds_filter_to_MvcOptions(MvcOptions options)
        {
            SetupExtensions.AddExceptionHandlerFilter(options);

            Assert.That(options.Filters.Any(i => i is TypeFilterAttribute typeFilter && typeFilter.ImplementationType == typeof(ExceptionHandlerFilter)));
        }
    }
}