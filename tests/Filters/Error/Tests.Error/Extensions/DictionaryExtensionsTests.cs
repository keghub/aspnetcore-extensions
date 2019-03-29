using System.Collections;
using EMG.Extensions.AspNetCore.Extensions;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod

namespace Tests.Extensions
{
    [TestFixture]
    public class DictionaryExtensionsTests
    {
        [Test]
        public void PrepareForOutput_returns_null_if_empty()
        {
            var dictionary = new Hashtable();

            var result = DictionaryExtensions.PrepareForOutput(dictionary);

            Assert.That(result, Is.Null);
        }

        [Test, AutoCustomData]
        public void PrepareForOutput_removes_ExceptionInfo_item(Hashtable dictionary, ExceptionInfo exceptionInfo, string key, object value)
        {
            dictionary.Add(ExceptionExtensions.ExceptionInfoKey, exceptionInfo);
            dictionary.Add(key, value);

            Assume.That(dictionary.Count, Is.Not.EqualTo(1));
            
            var result = DictionaryExtensions.PrepareForOutput(dictionary);

            Assert.That(result.Contains(ExceptionExtensions.ExceptionInfoKey), Is.False);
        }

        [Test, AutoCustomData]
        public void PrepareForOutput_returns_null_if_empty_after_removing_ExceptionInfo_item(Hashtable dictionary, ExceptionInfo exceptionInfo, string key, object value)
        {
            dictionary.Add(ExceptionExtensions.ExceptionInfoKey, exceptionInfo);

            Assume.That(dictionary.Count, Is.EqualTo(1));

            var result = DictionaryExtensions.PrepareForOutput(dictionary);

            Assert.That(result, Is.Null);
        }

    }
}