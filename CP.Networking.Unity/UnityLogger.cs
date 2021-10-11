using CP.Networking.Loggers;
using System;
using UnityEngine;
using ILogger = CP.Networking.Loggers.ILogger;

namespace CP.Networking.Unity
{
    class UnityLogger : ILogger
    {
        public void Log(LogLevel level, string message)
        {
            Debug.Log(Loggers.Logger.GetCurrentTime() + " " + level.ToString() + ": " + message);
        }

        public void Log(LogLevel level, Exception exception)
        {
            Debug.Log(Loggers.Logger.GetCurrentTime() + " " + level.ToString() + ": " + exception);
        }

        public void Log(LogLevel level, string message, Exception exception)
        {
            Debug.Log(Loggers.Logger.GetCurrentTime() + " " + level.ToString() + ": " + message + "\n" + exception);
        }

        public static void SetupUnityLogger()
        {
            Loggers.Logger.DefaultLogger = new UnityLogger();
        }
    }
}
