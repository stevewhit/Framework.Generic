using Moq;
using Framework.Generic.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Framework.Generic.Tests.Builders
{
    [ExcludeFromCodeCoverage]
    public class LogItem
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

        public LogItem(LogLevel level, string message, Exception exception)
        {
            Level = level;
            Message = message;
            Exception = exception;
        }
    }

    [ExcludeFromCodeCoverage]
    public class MockLog : Mock<ILog>
    {
        public ICollection<LogItem> LoggedItems { get; set; }

        public MockLog()
        {
            LoggedItems = new Collection<LogItem>();

            SetupDebug().SetupError().SetupFatal().SetupInfo().SetupWarn();
        }

        public MockLog SetupDebug()
        {
            Setup(l => l.Debug(It.IsAny<object>()))
                .Callback((object message) => { LoggedItems.Add(new LogItem(LogLevel.Debug, message.ToString(), null)); });

            Setup(l => l.Debug(It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback((object message, Exception exception) => { LoggedItems.Add(new LogItem(LogLevel.Debug, message.ToString(), exception)); });

            return this;
        }

        public MockLog SetupError()
        {
            Setup(l => l.Error(It.IsAny<object>()))
                .Callback((object message) => { LoggedItems.Add(new LogItem(LogLevel.Error, message.ToString(), null)); });

            Setup(l => l.Error(It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback((object message, Exception exception) => { LoggedItems.Add(new LogItem(LogLevel.Error, message.ToString(), exception)); });

            return this;
        }

        public MockLog SetupFatal()
        {
            Setup(l => l.Fatal(It.IsAny<object>()))
                .Callback((object message) => { LoggedItems.Add(new LogItem(LogLevel.Fatal, message.ToString(), null)); });

            Setup(l => l.Fatal(It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback((object message, Exception exception) => { LoggedItems.Add(new LogItem(LogLevel.Fatal, message.ToString(), exception)); });

            return this;
        }

        public MockLog SetupInfo()
        {
            Setup(l => l.Info(It.IsAny<object>()))
                .Callback((object message) => { LoggedItems.Add(new LogItem(LogLevel.Info, message.ToString(), null)); });

            Setup(l => l.Info(It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback((object message, Exception exception) => { LoggedItems.Add(new LogItem(LogLevel.Info, message.ToString(), exception)); });

            return this;
        }

        public MockLog SetupWarn()
        {
            Setup(l => l.Warn(It.IsAny<object>()))
                .Callback((object message) => { LoggedItems.Add(new LogItem(LogLevel.Warn, message.ToString(), null)); });

            Setup(l => l.Warn(It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback((object message, Exception exception) => { LoggedItems.Add(new LogItem(LogLevel.Warn, message.ToString(), exception)); });

            return this;
        }
    }
}
