using System.Collections;

namespace EMG.Extensions.AspNetCore.Extensions
{
    public static class DictionaryExtensions
    {
        public static IDictionary PrepareForOutput(this IDictionary dictionary)
        {
            var result = new Hashtable(dictionary);
            result.Remove(ExceptionExtensions.ExceptionInfoKey);

            if (result.Count == 0)
            {
                return null;
            }

            return result;
        }
    }
}
