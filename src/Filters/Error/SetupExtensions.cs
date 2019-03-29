using System;
using EMG.Extensions.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EMG.Extensions.AspNetCore
{
    public static class SetupExtensions
    {
        public static MvcOptions AddExceptionHandlerFilter(this MvcOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Filters.Add<ExceptionHandlerFilter>();

            return options;
        }
    }
}
