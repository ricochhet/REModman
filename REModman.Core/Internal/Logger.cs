using System;
using REModman.Configuration;
using REModman.Configuration.Enums;

namespace REModman.Internal
{
    public class Logger
    {
        public static void Log(LogType type, string[] lines)
        {
            string logType = type switch
            {
                LogType.Info => Constants.LOG_INFO,
                LogType.Warn => Constants.LOG_WARN,
                LogType.Error => Constants.LOG_ERROR,
                _ => throw new NotImplementedException(),
            };

            foreach (string line in lines)
            {
                Console.WriteLine($"[{logType}] {line}");
            }
        }

        public static void Log(LogOpType type, string[] lines)
        {
            string logType = type switch
            {
                LogOpType.Write => Constants.LOG_WRITE,
                LogOpType.Copy => Constants.LOG_COPY,
                LogOpType.Read => Constants.LOG_READ,
                LogOpType.Delete => Constants.LOG_DELETE,
                _ => throw new NotImplementedException(),
            };

            foreach (string line in lines)
            {
                Console.WriteLine($"[{logType}] {line}");
            }
        }
    }
}