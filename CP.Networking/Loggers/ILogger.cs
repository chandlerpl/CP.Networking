using System;
using System.Collections.Generic;
using System.Text;

namespace CP.Networking.Loggers
{
    public interface ILogger
    {
        void Log(LogLevel level, string message);
        void Log(LogLevel level, Exception exception);
        void Log(LogLevel level, string message, Exception exception);

    }
}
