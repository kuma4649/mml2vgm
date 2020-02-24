using System;
using System.Collections.Generic;
using System.Text;

namespace mucomDotNET.Interface
{
    public static class Log
    {
        public static Action<LogLevel, string> writeLine = null;
        public static LogLevel level = LogLevel.INFO;

        public static void WriteLine(LogLevel level, string msg)
        {

            if (level <= Log.level)
            {
                writeLine?.Invoke(level, msg);
            }
        }
    }
}
