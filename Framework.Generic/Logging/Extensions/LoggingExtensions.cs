using System;
using System.Text;

namespace Framework.Generic.Logging
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Logs a given exception and all inner exceptions to the provided log at the appropriate log level.
        /// </summary>
        public static void Log(this Exception exception, ILog log, LogLevel level)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            var message = GetNestedExceptionMessage(exception);

            switch (level)
            {
                case LogLevel.Debug:
                    log.Debug(message);
                    break;
                case LogLevel.Error:
                    log.Error(message);
                    break;
                case LogLevel.Fatal:
                    log.Fatal(message);
                    break;
                case LogLevel.Info:
                    log.Info(message);
                    break;
                case LogLevel.Warn:
                    log.Warn(message);
                    break;
                default:
                    throw new NotSupportedException($"LogLevel {level} is not supported.");
            }
        }

        /// <summary>
        /// Returns the parent level exception message concatinated with all children exceptions.
        /// </summary>
        private static string GetNestedExceptionMessage(Exception exception)
        {
            if (exception == null)
                return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine(exception.Message);
            sb.AppendLine(GetNestedExceptionMessage(exception.InnerException));

            return sb.ToString();
        }
    }
}