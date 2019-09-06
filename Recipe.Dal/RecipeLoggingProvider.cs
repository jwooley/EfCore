﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Dal
{
    public class RecipeLoggingProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new RecipeLogger();

        public void Dispose() { }

        private class RecipeLogger : ILogger
        {
            public IDisposable BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Error;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) => Console.WriteLine(formatter(state, exception));
        }
    }
}
