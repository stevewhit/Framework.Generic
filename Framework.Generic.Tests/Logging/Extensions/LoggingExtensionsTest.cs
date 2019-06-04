using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Generic.Logging;
using Framework.Generic.Tests.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Framework.Generic.Tests.Logging
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class LoggingExceptionsTest
    {
        private MockLog _mockLog;
        private SystemException _exception;
        private InvalidOperationException _innerException;
        private LogLevel _level;
        private const string _exceptionMessage = "ExceptionMessage: Test message to store as the main exception's message.";
        private const string _innerExceptionMessage = "InnerExceptionMessage: Test message to indicate that an operation was invalid.";
        
        [TestInitialize]
        public void Initialize()
        {
            _mockLog = new MockLog();

            _innerException = new InvalidOperationException(_innerExceptionMessage);
            _exception = new SystemException(_exceptionMessage, _innerException);
            _level = LogLevel.Warn;
        }

        #region Testing Log(this Exception exception, ILog log, LogLevel level)

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Log_WithNullException_ThrowsException()
        {
            // Arrange
            _exception = null;

            // Act
            _exception.Log(_mockLog.Object, LogLevel.Error);
        }
        
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Log_WithNullLog_ThrowsException()
        {
            // Act
            _exception.Log(null, _level);
        }

        [TestMethod]
        public void Log_WithoutNestedException_LogsOnlyException()
        {
            // Arrange
            _exception = new SystemException(_exceptionMessage);

            // Act
            _exception.Log(_mockLog.Object, _level);

            var loggedItems = _mockLog.LoggedItems;
            var logItem = loggedItems.FirstOrDefault();

            // Assert
            Assert.IsTrue(logItem.Message.TrimEnd() == _exceptionMessage, "The two messages should be identical after the stringbuilder \r\n sequences are removed.");      
        }

        [TestMethod]
        public void Log_WithMultipleNestedExceptions_LogsAll()
        {
            // Arrange
            var _innerInnerExceptionMessage = "InnerInnerExceptionMessage: Test ArgumentNullException Message.";
            _innerException = new InvalidOperationException(_innerExceptionMessage, new ArgumentNullException("TestArgumentException", _innerInnerExceptionMessage));
            _exception = new SystemException(_exceptionMessage, _innerException);

            // Act
            _exception.Log(_mockLog.Object, _level);

            var loggedItems = _mockLog.LoggedItems;
            var logItem = loggedItems.FirstOrDefault();

            // Assert
            Assert.IsTrue(logItem.Message.Contains(_exceptionMessage));
            Assert.IsTrue(logItem.Message.Contains(_innerExceptionMessage));
            Assert.IsTrue(logItem.Message.Contains(_innerInnerExceptionMessage));
        }

        [TestMethod]
        public void Log_WithDebugLevel_LogsDebugMessage()
        {
            // Act
            _exception.Log(_mockLog.Object, LogLevel.Debug);

            var loggedItems = _mockLog.LoggedItems;
            var Log = loggedItems.FirstOrDefault();

            // Assert
            Assert.IsTrue(loggedItems.Count == 1);
            Assert.IsNotNull(Log);
            Assert.IsTrue(Log.Level == LogLevel.Debug);
            Assert.IsTrue(Log.Message.Contains(_innerExceptionMessage));
        }

        [TestMethod]
        public void Log_WithErrorLevel_LogsErrorMessage()
        {
            // Act
            _exception.Log(_mockLog.Object, LogLevel.Error);

            var loggedItems = _mockLog.LoggedItems;
            var Log = loggedItems.FirstOrDefault();

            // Assert
            Assert.IsTrue(loggedItems.Count == 1);
            Assert.IsNotNull(Log);
            Assert.IsTrue(Log.Level == LogLevel.Error);
            Assert.IsTrue(Log.Message.Contains(_innerExceptionMessage));
        }

        [TestMethod]
        public void Log_WithFatalLevel_LogsFatalMessage()
        {
            // Act
            _exception.Log(_mockLog.Object, LogLevel.Fatal);

            var loggedItems = _mockLog.LoggedItems;
            var Log = loggedItems.FirstOrDefault();

            // Assert
            Assert.IsTrue(loggedItems.Count == 1);
            Assert.IsNotNull(Log);
            Assert.IsTrue(Log.Level == LogLevel.Fatal);
            Assert.IsTrue(Log.Message.Contains(_innerExceptionMessage));
        }

        [TestMethod]
        public void Log_WithInfoLevel_LogsInfoMessage()
        {
            // Act
            _exception.Log(_mockLog.Object, LogLevel.Info);

            var loggedItems = _mockLog.LoggedItems;
            var Log = loggedItems.FirstOrDefault();

            // Assert
            Assert.IsTrue(loggedItems.Count == 1);
            Assert.IsNotNull(Log);
            Assert.IsTrue(Log.Level == LogLevel.Info);
            Assert.IsTrue(Log.Message.Contains(_innerExceptionMessage));
        }

        [TestMethod]
        public void Log_WithWarnLevel_LogsWarnMessage()
        {
            // Act
            _exception.Log(_mockLog.Object, LogLevel.Warn);

            var loggedItems = _mockLog.LoggedItems;
            var Log = loggedItems.FirstOrDefault();

            // Assert
            Assert.IsTrue(loggedItems.Count == 1);
            Assert.IsNotNull(Log);
            Assert.IsTrue(Log.Level == LogLevel.Warn);
            Assert.IsTrue(Log.Message.Contains(_innerExceptionMessage));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Log_WithInvalidLevel_ThrowsException()
        {
            // Act
            _exception.Log(_mockLog.Object, (LogLevel)(-1));
        }

        #endregion
    }
}
