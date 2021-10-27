using System;
using System.Collections.Generic;
using System.Text;

namespace CP.Networking.Loggers
{
    public class ConsoleLogger : ILogger
    {
        public void Log(LogLevel level, string message)
        {
            Console.WriteLine(Logger.GetCurrentTime() + " " + level.ToString() + ": " + message);
        }

        public void Log(LogLevel level, Exception exception)
        {
            Console.WriteLine(Logger.GetCurrentTime() + " " + level.ToString() + ": " + exception);
        }

        public void Log(LogLevel level, string message, Exception exception)
        {
            Console.WriteLine(Logger.GetCurrentTime() + " " + level.ToString() + ": " + message + "\n" + exception);
        }
    }
}
