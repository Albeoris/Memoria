using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Memoria.Launcher.Utils.Updates
{
    internal sealed class LauncherUpdateInstaller
    {
        private readonly Logger _log = AppLogger.GetLogger(nameof(LauncherUpdateInstaller));

        public void Start(String patcherPath)
        {
            if (String.IsNullOrWhiteSpace(patcherPath))
                throw new ArgumentException("The patcher path cannot be empty.", nameof(patcherPath));

            String applicationDirectory = Path.GetFullPath("./");
            String applicationPath = Path.Combine(applicationDirectory, Path.GetFileName(Assembly.GetExecutingAssembly().Location));
            String arguments = $@"-update ""{applicationPath}"" ""{Process.GetCurrentProcess().Id}""";
            _log.Info("Starting launcher update installer. Patcher: {PatcherPath}, Application: {ApplicationPath}", patcherPath, applicationPath);
            Process.Start(new ProcessStartInfo(patcherPath, arguments) { UseShellExecute = false });
        }
    }
}
