using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributor.Common.Debug
{

    public enum LogType
    {
        Server,
        Client
    }

    public enum LogLevel
    {
        Default,
        Information,
        Warning,
        Exception,
        Fatal
    }

    public class Log
    {

        public Log instance;

        private static string _logPath = "LogFiles";
        private static string _logFile = "Distributor";
        private static LogType _logType;
        private static StreamWriter _log;

        public Log(LogType logType = 0)
        {

            // Creates an instance - not used currently
            instance = this;

            // Sets the LogType to be that of whats been passed, or default to 0 (Normal)
            _logType = logType;
            _logFile = $"{_logFile}.{_logType.ToString()}";

            SetupLogger();

        }

        public static void SetupLogger(LogType logType = 0)
        {

            try
            {
                // Sets the LogType to be that of whats been passed, or default to 0 (Normal)
                _logType = logType;
                _logFile = $"{_logFile}.{_logType.ToString()}";

                _logFile = $"{_logFile}_{TimeStamp(true)}.txt";
                var path = Path.Combine(_logPath, _logFile);
                Directory.CreateDirectory(_logPath);

                File.WriteAllText(path, string.Empty);

                _log = new StreamWriter(path, true)
                {
                    AutoFlush = true
                };

                WriteLine($"Log File Created for {_logType.ToString()}", LogLevel.Information);

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error Creating Log : {ex.Message}");
            }

        }

        private static string TimeStamp(bool isFile = false)
        {

            if (isFile)
                return DateTime.Now.ToString("ddMM_HHmm");
            else
                return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        }

        public static void WriteLine(string logMessage, LogLevel logLevel = 0)
        {

            try
            {
                string logLevelStr = $"[{logLevel.ToString()}]";

                if (logLevel == LogLevel.Default)
                    logLevelStr = string.Empty;

                if (logLevel == LogLevel.Fatal)
                {
                    _log.WriteLine($"[{TimeStamp()}]{logLevelStr.ToUpper()} {logMessage} : Fatal Error Occured. Application will now Exit");
                    Environment.Exit(0);
                }

                _log.WriteLine($"[{TimeStamp()}]{logLevelStr} {logMessage}", _log.NewLine);




            }
            catch (Exception ex)
            {
                _log.WriteLine($"[{TimeStamp()}][{LogLevel.Exception.ToString().ToUpper()}] {ex.Message}");
            }

        }



    }
}
