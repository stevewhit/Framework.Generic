using System;
using System.Diagnostics;

namespace Framework.Generic.Utility
{
    public static class SystemTime
    {
        /// <summary>
        /// Returns either the current time, or the overridden value.
        /// </summary>
        public static Func<DateTime> Now = () => DateTime.Now;

        /// <summary>
        /// Sets the time to return when SystemTime.Now() is called.
        /// </summary>
        /// <param name="dateTimeNow">The time to return when SystemTime.Now() is called.</param>
        public static void SetDateTime(DateTime dateTimeNow)
        {
            Now = () => dateTimeNow;
        }

        /// <summary>
        /// Resets SystemTime.Now() to return DateTime.Now.
        /// </summary>
        public static void ResetDateTime()
        {
            Now = () => DateTime.Now;
        }

        /// <summary>
        /// A system-wide StopWatch
        /// </summary>
        public static Stopwatch Stopwatch { get; set; }
    }
}
