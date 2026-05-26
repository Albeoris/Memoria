using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Text;

namespace Memoria.Launcher
{
    /// <summary>
    /// Central logging accessor for Memoria.Launcher.
    /// Configures NLog programmatically so logging always works regardless of
    /// whether NLog.config is present. NLog.config is still loaded when found
    /// and takes precedence, allowing users to customise behaviour.
    /// Any class in the project can obtain a named logger with:
    ///   private static readonly Logger Log = AppLogger.GetLogger();
    /// </summary>
    public static class AppLogger
    {
        static AppLogger()
        {
            // If NLog.config was found and loaded automatically, respect it.
            // Otherwise fall back to a hardcoded file target so logging always works.
            if (LogManager.Configuration == null || LogManager.Configuration.AllTargets.Count == 0)
            {
                var config = new LoggingConfiguration();

                var fileTarget = new FileTarget("file")
                {
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MemoriaLauncher.log"),
                    Layout = "${longdate} [${level:uppercase=true}] ${logger} - ${message}${onexception:inner=${newline}${exception:format=tostring}}",
                    KeepFileOpen = true,
                    Encoding = Encoding.UTF8,
                    DeleteOldFileOnStartup = true,
                };

                config.AddTarget(fileTarget);
                config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
                LogManager.Configuration = config;
            }
        }

        /// <summary>Returns a logger named after the calling class.</summary>
        public static Logger GetLogger() => LogManager.GetCurrentClassLogger();

        /// <summary>Returns a logger with the specified name.</summary>
        public static Logger GetLogger(string name) => LogManager.GetLogger(name);
    }
}
