using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Memoria.Launcher
{
    public sealed class UiLauncherPlayButton : UiLauncherButton
    {
        private const String PlayLabel = "Play";
        private const String PlayingLabel = "Playing...";

        public GameSettingsControl GameSettings { get; set; }

        public UiLauncherPlayButton()
        {
            Label = PlayLabel;
        }

        protected override async Task DoAction()
        {
            Label = PlayingLabel;
            try
            {
                if (GameSettings.ScreenResolution == null)
                {
                    MessageBox.Show((Window)this.GetRootElement(), "Please select an available resolution.", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }

                String[] strArray = GameSettings.ScreenResolution.Split('x');
                Int32 screenWidth = Int32.Parse(strArray[0], CultureInfo.InvariantCulture);
                Int32 screenHeight = Int32.Parse(strArray[1], CultureInfo.InvariantCulture);

                String directoyPath = ".\\" + (GameSettings.IsX64 ? "x64" : "x86");

                String executablePath = directoyPath + "\\FF9.exe";
                if (GameSettings.IsDebugMode)
                {
                    String unityPath = directoyPath + "\\Unity.exe";

                    if (!File.Exists(unityPath))
                    {
                        File.Copy(executablePath, unityPath);
                        File.SetLastWriteTimeUtc(unityPath, File.GetLastWriteTimeUtc(executablePath));
                    }
                    else
                    {
                        FileInfo fi1 = new FileInfo(executablePath);
                        FileInfo fi2 = new FileInfo(unityPath);
                        if (fi1.Length != fi2.Length || fi1.LastWriteTimeUtc != fi2.LastWriteTimeUtc)
                        {
                            File.Copy(executablePath, unityPath, true);
                            File.SetLastWriteTimeUtc(unityPath, fi1.LastWriteTimeUtc);
                        }
                    }

                    executablePath = unityPath;

                    String ff9DataPath = Path.GetFullPath(directoyPath + "\\FF9_Data");
                    String unityDataPath = Path.GetFullPath(directoyPath + "\\Unity_Data");

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
                }

                String arguments = $"-runbylauncher -single-instance -screen-width {screenWidth.ToString(CultureInfo.InvariantCulture)} -screen-height {screenHeight.ToString(CultureInfo.InvariantCulture)} -screen-fullscreen {(GameSettings.Windowed ? "0" : "1")}";
                await Task.Factory.StartNew(
                    () =>
                    {
                        ProcessStartInfo gameStartInfo = new ProcessStartInfo(executablePath, arguments) {UseShellExecute = false};
                        if (GameSettings.IsDebugMode)
                            gameStartInfo.EnvironmentVariables["UNITY_GIVE_CHANCE_TO_ATTACH_DEBUGGER"] = "1";

                        Process gameProcess = new Process {StartInfo = gameStartInfo};
                        gameProcess.Start();

                        if (GameSettings.IsDebugMode)
                        {
                            Process debuggerProcess = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.StartsWith("Memoria.Debugger"));
                            if (debuggerProcess == null)
                            {
                                String debuggerDirectory = Path.Combine(Path.GetFullPath("Debugger"), (GameSettings.IsX64 ? "x64" : "x86"));
                                String debuggerPath = Path.Combine(debuggerDirectory, "Memoria.Debugger.exe");
                                String debuggerArgs = "10000"; // Timeout: 10 seconds
                                ProcessStartInfo debuggerStartInfo = new ProcessStartInfo(debuggerPath, debuggerArgs) {WorkingDirectory = debuggerDirectory};
                                debuggerProcess = new Process {StartInfo = debuggerStartInfo};
                                debuggerProcess.Start();
                            }
                        }
                    }
                );
                Application.Current.Shutdown();
            }
            finally
            {
                Label = PlayLabel;
            }
        }
    }
}