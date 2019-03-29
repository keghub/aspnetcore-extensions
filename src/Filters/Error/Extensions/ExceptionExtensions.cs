using System;
using System.Collections;
using Microsoft.Extensions.Logging;

namespace EMG.Extensions.AspNetCore.Extensions
{
    public static class ExceptionExtensions
    {
        public const string ExceptionInfoKey = "ExceptionInfo";

        public static TException DescribeError<TState, TException>(this TException exception, EventId eventId, TState state, Func<TState, TException, string> formatter)
            where TState : class
            where TException : Exception
        {
            exception.Data[ExceptionInfoKey] = new ExceptionInfo
            {
                State = state,
                EventId = eventId,
                Formatter = (o, e) => formatter(o as TState, e as TException),
                Logger = (l, e, s, ex, f) => l.LogError(e, s as TState, ex as TException, (st, exc) => f(st, exc))
            };

            return exception;
        }

        public static bool TryExtractExceptionInfo(this Exception exception, out ExceptionInfo exceptionInfo)
        {
            exceptionInfo = null;

            if (exception.Data.Contains(ExceptionInfoKey) && exception.Data[ExceptionInfoKey] is ExceptionInfo ei)
            {
                exceptionInfo = ei;
                return true;
            }

            return false;
        }
    }

    public class ExceptionInfo
    {
        public object State { get; set; }

        public EventId EventId { get; set; }

        public Func<object, Exception, string> Formatter { get; set; }

        public Action<ILogger, EventId, object, Exception, Func<object, Exception, string>> Logger { get; set; }
    }
}
