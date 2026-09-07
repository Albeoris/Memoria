using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Memoria.Launcher.Utils;

namespace Memoria.Launcher
{
    public sealed class UiLauncherPlayButton : UiLauncherButton
    {
        public SettingsGrid_Vanilla GameSettings { get; set; }
        public SettingsGrid_VanillaDisplay GameSettingsDisplay { get; set; }

        public UiLauncherPlayButton()
        {
            SetResourceReference(LabelProperty, "Launcher.Launch");
        }

        protected override Task DoAction()
        {
            SetResourceReference(LabelProperty, "Launcher.Launching");
            ApplyDebugSettingsSafe();

            Int32 monitor = GetActiveMonitorIndex();
            if (!DisplayService.Current.TryGetMonitor(monitor, out _))
            {
                MessageBox.Show((Window)this.GetRootElement(), $"Selected monitor ({monitor}) does not appear available.\nDisplaying to monitor 0.", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                monitor = 0;
            }

            GetScreenResolution(out Int32 width, out Int32 height, monitor);
            String gameArch = "x64";
            String workingDirectory = Path.GetFullPath(".\\" + gameArch);
            String executablePath = PrepareExecutableAndData(workingDirectory);
            String arguments = $"-runbylauncher -single-instance -monitor {monitor.ToString(CultureInfo.InvariantCulture)} -screen-width {width.ToString(CultureInfo.InvariantCulture)} -screen-height {height.ToString(CultureInfo.InvariantCulture)} -screen-fullscreen {(GameSettingsDisplay.WindowMode == 1 ? "1" : "0")} {(GameSettingsDisplay.WindowMode >= 2 ? "-popupwindow" : "")}";
            String debugInjectorDestPath = Path.Combine(workingDirectory, "version.dll");
            String appidPath = Path.Combine(".\\", "steam_appid.txt");

            if (!File.Exists(appidPath))
                File.WriteAllText(appidPath, "377840");

            if (GameSettings.IsDebugMode)
                File.Copy(Path.Combine(".\\Debugger", gameArch, "Memoria.Injection.dll"), debugInjectorDestPath, true);
            else if (File.Exists(debugInjectorDestPath))
                File.Delete(debugInjectorDestPath);

            SetResourceReference(LabelProperty, "Launcher.Launch");
            StartGameProcess(executablePath, arguments);
            Application.Current.Shutdown();
            return Task.CompletedTask;
        }

        private void ApplyDebugSettingsSafe()
        {
            try
            {
                IniFile iniFile = IniFile.MemoriaIni;
                if (LaunchModelViewer)
                {
                    iniFile.SetSetting("Debug", "Enabled", "1");
                    iniFile.SetSetting("Debug", "StartModelViewer", "1");
                }
                else
                {
                    iniFile.SetSetting("Debug", "StartModelViewer", "0");
                }
                iniFile.Save();
            }
            catch
            {
            }
        }

        private Int32 GetActiveMonitorIndex()
        {
            if (!String.IsNullOrEmpty(GameSettingsDisplay?.ActiveMonitor))
            {
                Int32 spaceIndex = GameSettingsDisplay.ActiveMonitor.IndexOf(' ');
                if (spaceIndex > 0)
                {
                    String number = GameSettingsDisplay.ActiveMonitor.Substring(0, spaceIndex);
                    if (Int32.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out Int32 result))
                        return result;
                }
            }
            return -1;
        }

        private void GetScreenResolution(out Int32 width, out Int32 height, Int32 monitor)
        {
            String configuredValue = IniFile.SettingsIni.GetSetting("Settings", "ScreenResolution", GameSettingsDisplay.ScreenResolution);
            Boolean hasConfiguredResolution = DisplayResolution.TryParse(configuredValue, out DisplayResolution configuredResolution);
            DisplayResolution monitorResolution = DisplayService.Current.TryGetMonitor(monitor, out DisplayMonitor selectedMonitor) ? selectedMonitor.CurrentResolution : DisplayService.Current.PrimaryResolution;
            DisplayResolution launchResolution;

            if (GameSettingsDisplay.WindowMode == 2 || !hasConfiguredResolution)
                launchResolution = monitorResolution;
            else if (monitorResolution.IsValid)
                launchResolution = new DisplayResolution(Math.Min(configuredResolution.Width, monitorResolution.Width), Math.Min(configuredResolution.Height, monitorResolution.Height));
            else
                launchResolution = configuredResolution;

            if (!launchResolution.IsValid)
                launchResolution = new DisplayResolution(1920, 1080);

            width = launchResolution.Width;
            height = launchResolution.Height;
        }

        private String PrepareExecutableAndData(String workingDirectory)
        {
            String executablePath = Path.Combine(workingDirectory, "FF9.exe");
            if (!GameSettings.IsDebugMode)
                return executablePath;

            String unityPath = Path.Combine(workingDirectory, "Unity.exe");
            if (!File.Exists(unityPath) || !IsFileIdentical(unityPath, executablePath))
            {
                File.Copy(executablePath, unityPath, true);
                File.SetLastWriteTimeUtc(unityPath, File.GetLastWriteTimeUtc(executablePath));
            }

            String ff9DataPath = Path.Combine(workingDirectory, "FF9_Data");
            String unityDataPath = Path.Combine(workingDirectory, "Unity_Data");
            if (!Directory.Exists(unityDataPath))
            {
                JunctionPoint.Create(unityDataPath, ff9DataPath, true);
            }
            else
            {
                try
                {
                    foreach (String item in Directory.EnumerateFileSystemEntries(unityDataPath))
                        break;
                }
                catch
                {
                    JunctionPoint.Delete(unityDataPath);
                    JunctionPoint.Create(unityDataPath, ff9DataPath, true);
                }
            }

            return unityPath;
        }

        private static Boolean IsFileIdentical(String firstPath, String secondPath)
        {
            FileInfo first = new FileInfo(firstPath);
            FileInfo second = new FileInfo(secondPath);
            return first.Length == second.Length && first.LastWriteTimeUtc == second.LastWriteTimeUtc;
        }

        private static void StartGameProcess(String executablePath, String arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(executablePath, arguments) { UseShellExecute = false };
            using Process process = new Process { StartInfo = startInfo };
            process.Start();
        }
    }
}
