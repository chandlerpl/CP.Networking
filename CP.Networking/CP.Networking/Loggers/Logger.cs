using System;
using System.Collections.Generic;
using System.Text;

namespace CP.Networking.Loggers
{
    public enum LogLevel { INFO, WARNING, ERROR, CRITICAL }

    public class Logger
    {
        private static ILogger defaultlogger;
        public static ILogger DefaultLogger
        {
            get => defaultlogger ?? (defaultlogger = new ConsoleLogger());
            set => defaultlogger = value;
        }

        public static void Log(string message)
        {
            DefaultLogger.Log(LogLevel.INFO, message);
        }

        public static void Log(Exception exception)
        {
            DefaultLogger.Log(LogLevel.ERROR, exception);
        }

        public static void Log(string message, Exception exception)
        {
            DefaultLogger.Log(LogLevel.ERROR, message, exception);
        }

        public static void Log(LogLevel level, string message)
        {
            DefaultLogger.Log(level, message);
        }

        public static void Log(LogLevel level, Exception exception)
        {
            DefaultLogger.Log(level, exception);
        }

        public static void Log(LogLevel level, string message, Exception exception)
        {
            DefaultLogger.Log(level, message, exception);
        }
        public static string GetCurrentTime()
        {
            return DateTime.Now.ToString("dd/MM/yy HH:mm:ss");
        }
    }
}
