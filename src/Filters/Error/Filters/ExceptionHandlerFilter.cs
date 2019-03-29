using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using EMG.Extensions.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EMG.Extensions.AspNetCore.Filters
{
    public class ExceptionHandlerFilter : IExceptionFilter
    {
        private readonly ILoggerFactory _loggerFactory;

        public ExceptionHandlerFilter(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public void OnException(ExceptionContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                var logger = _loggerFactory.CreateLogger(descriptor.ControllerTypeInfo.FullName);

                ErrorModel error = null;

                if (context.Exception.TryExtractExceptionInfo(out var exceptionInfo))
                {
                    exceptionInfo.Logger(logger, exceptionInfo.EventId, exceptionInfo.State, context.Exception, exceptionInfo.Formatter);

                    error = new ErrorModel
                    {
                        ErrorId = exceptionInfo.EventId.Id,
                        Error = exceptionInfo.EventId.Name,
                        Data = exceptionInfo.State,
                        Message = exceptionInfo.Formatter(exceptionInfo.State, context.Exception),
                        RouteValues = descriptor.RouteValues,
                        AdditionalData = context.Exception.Data.PrepareForOutput(),
                    };
                }
                else
                {
                    logger.LogError(context.Exception, $"An error has occurred in '{descriptor.DisplayName}'");

                    error = new ErrorModel
                    {
                        Message = context.Exception.Message,
                        RouteValues = descriptor.RouteValues,
                        AdditionalData = context.Exception.Data.PrepareForOutput(),
                    };
                }

                context.Result = new ObjectResult(error)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };

                context.ExceptionHandled = true;
            }
        }
    }

    public class ErrorModel
    {
        [JsonProperty("errorId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int ErrorId { get; set; }

        [JsonProperty("error", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Error { get; set; }

        [JsonProperty("data", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object Data { get; set; }

        [JsonProperty("message", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty("routeValues", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, string> RouteValues { get; set; }

        [JsonProperty("additionalData", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary AdditionalData { get; set; }
    }
}
