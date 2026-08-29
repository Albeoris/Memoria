using System;
using System.Collections.Generic;
using Memoria.Launcher.Utils.Downloads;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
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

        protected override async Task DoAction()
        {
            SetResourceReference(LabelProperty, "Launcher.Launching");

            ApplyDebugSettingsSafe();

            int monitor = GetActiveMonitorIndex();
            if (!DisplayService.Current.TryGetMonitor(monitor, out _))
            {
                MessageBox.Show((Window)this.GetRootElement(), $"Selected monitor ({monitor}) does not appear available.\nDisplaying to monitor 0.", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                monitor = 0;
            }

            GetScreenResolution(out int width, out int height, monitor);

            String gameArch = "x64";
            String workingDirectory = Path.GetFullPath(".\\" + gameArch);
            String executablePath = PrepareExecutableAndData(workingDirectory);
            String arguments = $"-runbylauncher -single-instance -monitor {monitor.ToString(CultureInfo.InvariantCulture)} -screen-width {width.ToString(CultureInfo.InvariantCulture)} -screen-height {height.ToString(CultureInfo.InvariantCulture)} -screen-fullscreen {(GameSettingsDisplay.WindowMode == 1 ? "1" : "0")} {(GameSettingsDisplay.WindowMode >= 2 ? "-popupwindow" : "")}";
            String debugInjectorDestPath = Path.Combine(workingDirectory, "version.dll");

            var appidPath = Path.Combine(".\\", "steam_appid.txt");
            if (!File.Exists(appidPath)) File.WriteAllText(appidPath, "377840");

            if (GameSettings.IsDebugMode)
                File.Copy(
                    Path.Combine(".\\Debugger", gameArch, "Memoria.Injection.dll"),
                    debugInjectorDestPath,
                    true
                );
            else if (File.Exists(debugInjectorDestPath))
                File.Delete(debugInjectorDestPath);

            SetResourceReference(LabelProperty, "Launcher.Launch");
            StartGameProcess(executablePath, arguments);

            Application.Current.Shutdown();
        }

        // Try to update debug ini settings. Ignore exceptions.
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
            catch { }
        }

        // Parse the selected monitor index from settings.
        private int GetActiveMonitorIndex()
        {
            if (!string.IsNullOrEmpty(GameSettingsDisplay?.ActiveMonitor))
            {
                int spaceIndex = GameSettingsDisplay.ActiveMonitor.IndexOf(' ');
                if (spaceIndex > 0)
                {
                    string num = GameSettingsDisplay.ActiveMonitor.Substring(0, spaceIndex);
                    if (int.TryParse(num, NumberStyles.Integer, CultureInfo.InvariantCulture, out int res))
                        return res;
                }
            }
            return -1;
        }

        // Get the screen resolution
        private void GetScreenResolution(out int width, out int height, int monitor)
        {
            String configuredValue = IniFile.SettingsIni.GetSetting(
                "Settings",
                "ScreenResolution",
                GameSettingsDisplay.ScreenResolution);
            Boolean hasConfiguredResolution = DisplayResolution.TryParse(configuredValue, out DisplayResolution configuredResolution);

            DisplayResolution monitorResolution = DisplayService.Current.TryGetMonitor(monitor, out DisplayMonitor selectedMonitor)
                ? selectedMonitor.CurrentResolution
                : DisplayService.Current.PrimaryResolution;

            // EnumDisplaySettings supplies monitorResolution in physical pixels and does not
            // participate in DPI virtualization. This distinction is essential under Wine:
            // a 3840x2160 monitor at 200% desktop scaling may have a 1920x1080 logical
            // monitor rectangle, while the game must still receive 3840x2160.
            DisplayResolution launchResolution;
            if (GameSettingsDisplay.WindowMode == 2 || !hasConfiguredResolution)
            {
                launchResolution = monitorResolution;
            }
            else if (monitorResolution.IsValid)
            {
                launchResolution = new DisplayResolution(
                    Math.Min(configuredResolution.Width, monitorResolution.Width),
                    Math.Min(configuredResolution.Height, monitorResolution.Height));
            }
            else
            {
                launchResolution = configuredResolution;
            }

            if (!launchResolution.IsValid)
                launchResolution = new DisplayResolution(1920, 1080);

            width = launchResolution.Width;
            height = launchResolution.Height;
        }

        // Handles Unity/Debug shenanigans, returns the executable path to run.
        private string PrepareExecutableAndData(string workingDirectory)
        {
            string executablePath = Path.Combine(workingDirectory, "FF9.exe");
            if (GameSettings.IsDebugMode)
            {
                string unityPath = Path.Combine(workingDirectory, "Unity.exe");

                // Copy Unity.exe if missing or outdated.
                if (!File.Exists(unityPath) || !IsFileIdentical(unityPath, executablePath))
                {
                    File.Copy(executablePath, unityPath, true);
                    File.SetLastWriteTimeUtc(unityPath, File.GetLastWriteTimeUtc(executablePath));
                }
                executablePath = unityPath;

                string ff9DataPath = Path.Combine(workingDirectory, "FF9_Data");
                string unityDataPath = Path.Combine(workingDirectory, "Unity_Data");

                if (!Directory.Exists(unityDataPath))
                {
                    JunctionPoint.Create(unityDataPath, ff9DataPath, true);
                }
                else
                {
                    try
                    {
                        // Check directory accessibility.
                        foreach (string item in Directory.EnumerateFileSystemEntries(unityDataPath))
                            break;
                    }
                    catch
                    {
                        JunctionPoint.Delete(unityDataPath);
                        JunctionPoint.Create(unityDataPath, ff9DataPath, true);
                    }
                }
            }
            return executablePath;
        }

        // Compare files by length and last write time.
        private bool IsFileIdentical(string path1, string path2)
        {
            FileInfo f1 = new FileInfo(path1);
            FileInfo f2 = new FileInfo(path2);
            return f1.Length == f2.Length && f1.LastWriteTimeUtc == f2.LastWriteTimeUtc;
        }

        // Launch the game process with given args.
        private async void StartGameProcess(string exePath, string args)
        {
            ProcessStartInfo gameStartInfo = new ProcessStartInfo(exePath, args) { UseShellExecute = false };
            Process gameProcess = new Process { StartInfo = gameStartInfo };
            gameProcess.Start();
        }

        internal static async Task<Boolean> CheckUpdates(Window rootElement, SettingsGrid_Vanilla gameSettings, CancellationToken cancellationToken = default)
        {
            String applicationDirectory = Path.GetFullPath("./");
            String applicationPath = Path.Combine(applicationDirectory, Path.GetFileName(Assembly.GetExecutingAssembly().Location));
            LinkedList<RemoteFileInfo> updateInfo = await FindUpdatesInfo(applicationDirectory, gameSettings, cancellationToken);
            if (updateInfo.Count == 0)
                return false;

            StringBuilder messageSb = new StringBuilder(256);
            messageSb.AppendLine((String)Lang.Res["Launcher.NewVersionIsAvailable"]);
            Int64 size = 0;
            foreach (RemoteFileInfo info in updateInfo)
            {
                size += info.ContentLength;
                messageSb.AppendLine($"{info.TargetName} - {info.LastModified} ({UiProgressWindow.FormatValue(info.ContentLength)})");
            }

            if (MessageBox.Show(rootElement, messageSb.ToString(), (String)Lang.Res["Launcher.QuestionTitle"], MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<String> success = new List<String>(updateInfo.Count);
                List<String> failed = new List<String>();

                using (UiProgressWindow progress = new UiProgressWindow("Downloading...")) // TODO language?
                {
                    progress.SetTotal(size);
                    progress.Show();

                    using (LauncherUpdateDownloader downloader = new LauncherUpdateDownloader(cancellationToken))
                    {
                        IProgress<Int64> downloadProgress = new Progress<Int64>(progress.Incremented);

                        foreach (RemoteFileInfo info in updateInfo)
                        {
                            String filePath = info.TargetPath;

                            try
                            {
                                await downloader.DownloadAsync(info.Source, filePath, downloadProgress);
                                File.SetLastWriteTime(filePath, info.LastModified);

                                success.Add(filePath);
                            }
                            catch (OperationCanceledException) when (downloader.IsCancellationRequested)
                            {
                                progress.Close();
                                return false;
                            }
                            catch (Exception exception)
                            {
                                failed.Add($"{filePath}: {exception.Message}");
                            }
                        }

                        progress.Close();
                    }
                }

                Boolean runPatcher = false;
                if (failed.Count > 0)
                {
                    MessageBox.Show(rootElement,
                        "Failed to download:" + Environment.NewLine + String.Join(Environment.NewLine, failed),
                        (String)Lang.Res["Launcher.ErrorTitle"],
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }

                if (success.Count > 0)
                {
                    runPatcher = MessageBox.Show(rootElement,
                        "Download successful!\nRun the patcher?",
                        (String)Lang.Res["Launcher.QuestionTitle"],
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes;
                }

                if (runPatcher)
                {
                    String main = success.First();
                    if (success.Count > 1)
                    {
                        StringBuilder sb = new StringBuilder(256);
                        foreach (String path in success.Skip(1))
                        {
                            sb.Append('"');
                            sb.Append(path);
                            sb.Append('"');
                        }

                        Process.Start(main, $@"-update ""{applicationPath}"" ""{Process.GetCurrentProcess().Id}"" {sb}");
                    }
                    else
                    {
                        Process.Start(main, $@"-update ""{applicationPath}"" ""{Process.GetCurrentProcess().Id}""");
                    }

                    Environment.Exit(2);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private static String GetPatcherDownloadUrl(String updateChannel)
        {
            if (String.Equals(updateChannel, "Canary", StringComparison.OrdinalIgnoreCase))
                return "https://github.com/Albeoris/Memoria/releases/download/canary/Memoria.Patcher.exe";

            return "https://github.com/Albeoris/Memoria/releases/latest/download/Memoria.Patcher.exe";
        }

        private static async Task<LinkedList<RemoteFileInfo>> FindUpdatesInfo(String applicationDirectory, SettingsGrid_Vanilla gameSettings, CancellationToken cancellationToken)
        {
            using (LauncherUpdateDownloader downloader = new LauncherUpdateDownloader(cancellationToken))
            {
                String[] urls = [GetPatcherDownloadUrl(gameSettings.UpdateChannel)];

                LinkedList<RemoteFileInfo> list = new LinkedList<RemoteFileInfo>();
                Dictionary<String, LinkedListNode<RemoteFileInfo>> dic = new Dictionary<String, LinkedListNode<RemoteFileInfo>>(urls.Length);

                foreach (String url in urls)
                {
                    try
                    {
                        RemoteFileInfo fileInfo = await downloader.GetRemoteFileInfoAsync(new Uri(url, UriKind.Absolute));

                        Int32 separatorIndex = url.LastIndexOf('/');
                        String remoteFileName = url.Substring(separatorIndex + 1);
                        fileInfo.TargetName = remoteFileName;
                        fileInfo.TargetPath = Path.Combine(applicationDirectory, remoteFileName);

                        LinkedListNode<RemoteFileInfo> node;
                        if (!dic.TryGetValue(fileInfo.TargetPath, out node) && File.Exists(fileInfo.TargetPath) && File.GetLastWriteTime(fileInfo.TargetPath) >= fileInfo.LastModified)
                            continue;

                        if (node != null)
                        {
                            if (node.Value.LastModified >= fileInfo.LastModified)
                                continue;

                            LinkedListNode<RemoteFileInfo> newNode = list.AddBefore(node, fileInfo);
                            list.Remove(node);
                            dic[fileInfo.TargetPath] = newNode;
                        }
                        else
                        {
                            LinkedListNode<RemoteFileInfo> newNode = list.AddLast(fileInfo);
                            dic.Add(fileInfo.TargetPath, newNode);
                        }
                    }
                    catch (DownloadException)
                    {
                        // Update checks are optional. The downloader logs actionable diagnostics.
                    }
                }

                return list;
                ;
            }
        }
    }

    public sealed class UiProgressWindow : UiWindow, IDisposable
    {
        public UiProgressWindow(string title)
        {
            #region Construct

            Height = 72;
            Width = 320;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.None;

            UiGrid root = UiGridFactory.Create(3, 1);
            root.SetRowsHeight(GridLength.Auto);
            root.Margin = new Thickness(5);

            TextBlock titleTextBlock = UiTextBlockFactory.Create(title);
            {
                titleTextBlock.VerticalAlignment = VerticalAlignment.Center;
                titleTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                root.AddUiElement(titleTextBlock, 0, 0);
            }

            _progressBar = UiProgressBarFactory.Create();
            {
                root.AddUiElement(_progressBar, 1, 0);
            }

            _progressTextBlock = UiTextBlockFactory.Create("100%");
            {
                _progressTextBlock.VerticalAlignment = VerticalAlignment.Center;
                _progressTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                root.AddUiElement(_progressTextBlock, 1, 0);
            }

            _elapsedTextBlock = UiTextBlockFactory.Create((String)Lang.Res["Measurement.Elapsed"] + ": 00:00");
            {
                _elapsedTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                root.AddUiElement(_elapsedTextBlock, 2, 0);
            }

            _processedTextBlock = UiTextBlockFactory.Create("0 / 0");
            {
                _processedTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                root.AddUiElement(_processedTextBlock, 2, 0);
            }

            _remainingTextBlock = UiTextBlockFactory.Create((String)Lang.Res["Measurement.Remaining"] + ": 00:00");
            {
                _remainingTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
                root.AddUiElement(_remainingTextBlock, 2, 0);
            }

            Content = root;

            #endregion

            Loaded += OnLoaded;
            Closing += OnClosing;

            _timer = new System.Timers.Timer(500);
            _timer.Elapsed += OnTimer;
        }

        private readonly UiProgressBar _progressBar;
        private readonly TextBlock _progressTextBlock;
        private readonly TextBlock _elapsedTextBlock;
        private readonly TextBlock _processedTextBlock;
        private readonly TextBlock _remainingTextBlock;

        private readonly System.Timers.Timer _timer;

        private long _processedCount, _totalCount;
        private DateTime _begin;

        public void Dispose()
        {
            _timer.Dispose();
        }

        public void SetTotal(long totalCount)
        {
            Interlocked.Exchange(ref _totalCount, totalCount);
        }

        public void Incremented(long processedCount)
        {
            if (Interlocked.Add(ref _processedCount, processedCount) < 0)
                throw new ArgumentOutOfRangeException(nameof(processedCount));
        }

        #region Internal Logic

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _begin = DateTime.Now;
            _timer.Start();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _timer.Stop();
            _timer.Elapsed -= OnTimer;
        }

        private void OnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(UpdateProgress));
        }

        private void UpdateProgress()
        {
            _timer.Elapsed -= OnTimer;

            _progressBar.Maximum = _totalCount;
            _progressBar.Value = _processedCount;

            double percents = (_totalCount == 0) ? 0.0 : 100 * _processedCount / (double)_totalCount;
            TimeSpan elapsed = DateTime.Now - _begin;
            double speed = _processedCount / Math.Max(elapsed.TotalSeconds, 1);
            if (speed < 1) speed = 1;
            TimeSpan left = TimeSpan.FromSeconds((_totalCount - _processedCount) / speed);

            _progressTextBlock.Text = $"{percents:F2}%";
            _elapsedTextBlock.Text = String.Format("{1}: {0:mm\\:ss}", elapsed, (String)Lang.Res["Measurement.Elapsed"]);
            _processedTextBlock.Text = $"{FormatValue(_processedCount)} / {FormatValue(_totalCount)}";
            _remainingTextBlock.Text = String.Format("{1}: {0:mm\\:ss}", left, (String)Lang.Res["Measurement.Remaining"]);

            _timer.Elapsed += OnTimer;
        }

        public static String FormatValue(Int64 value)
        {
            Int32 i = 0;
            Decimal dec = value;
            while ((dec > 1024))
            {
                dec /= 1024;
                i++;
            }

            switch (i)
            {
                case 0:
                    return String.Format("{0:F2} " + (String)Lang.Res["Measurement.ByteAbbr"], dec);
                case 1:
                    return String.Format("{0:F2} " + (String)Lang.Res["Measurement.KByteAbbr"], dec);
                case 2:
                    return String.Format("{0:F2} " + (String)Lang.Res["Measurement.MByteAbbr"], dec);
                case 3:
                    return String.Format("{0:F2} " + (String)Lang.Res["Measurement.GByteAbbr"], dec);
                case 4:
                    return String.Format("{0:F2} " + (String)Lang.Res["Measurement.TByteAbbr"], dec);
                case 5:
                    return String.Format("{0:F2} " + (String)Lang.Res["Measurement.PByteAbbr"], dec);
                case 6:
                    return String.Format("{0:F2} " + (String)Lang.Res["Measurement.EByteAbbr"], dec);
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        #endregion

        public static void Execute(string title, IProgressSender progressSender, Action action)
        {
            using (UiProgressWindow window = new UiProgressWindow(title))
            {
                progressSender.ProgressTotalChanged += window.SetTotal;
                progressSender.ProgressIncremented += window.Incremented;
                Task.Run(() => ExecuteAction(window, action));
                window.ShowDialog();
            }
        }

        public static T Execute<T>(string title, IProgressSender progressSender, Func<T> func)
        {
            using (UiProgressWindow window = new UiProgressWindow(title))
            {
                progressSender.ProgressTotalChanged += window.SetTotal;
                progressSender.ProgressIncremented += window.Incremented;
                Task<T> task = Task.Run(() => ExecuteFunction(window, func));
                window.ShowDialog();
                return task.Result;
            }
        }

        public static void Execute(string title, Action<Action<long>, Action<long>> action)
        {
            using (UiProgressWindow window = new UiProgressWindow(title))
            {
                Task.Run(() => ExecuteAction(window, action));
                window.ShowDialog();
            }
        }

        public static T Execute<T>(string title, Func<Action<long>, Action<long>, T> action)
        {
            using (UiProgressWindow window = new UiProgressWindow(title))
            {
                Task<T> task = Task.Run(() => ExecuteFunction(window, action));
                window.ShowDialog();
                return task.Result;
            }
        }

        #region Internal Static Logic

        private static void ExecuteAction(UiProgressWindow window, Action action)
        {
            try
            {
                action();
            }
            finally
            {
                window.Dispatcher.Invoke(window.Close);
            }
        }

        private static void ExecuteAction(UiProgressWindow window, Action<Action<long>, Action<long>> action)
        {
            try
            {
                action(window.SetTotal, window.Incremented);
            }
            finally
            {
                window.Dispatcher.Invoke(window.Close);
            }
        }

        private static T ExecuteFunction<T>(UiProgressWindow window, Func<T> func)
        {
            try
            {
                return func();
            }
            finally
            {
                window.Dispatcher.Invoke(window.Close);
            }
        }

        private static T ExecuteFunction<T>(UiProgressWindow window, Func<Action<long>, Action<long>, T> action)
        {
            try
            {
                return action(window.SetTotal, window.Incremented);
            }
            finally
            {
                window.Dispatcher.Invoke(window.Close);
            }
        }

        #endregion
    }

    public class UiProgressBar : ProgressBar
    {
    }

    public class UiWindow : Window
    {
    }

    public static class UiGridFactory
    {
        public static UiGrid Create(int rows, int cols)
        {
            UiGrid grid = new UiGrid();

            if (rows > 1) while (rows-- > 0) grid.RowDefinitions.Add(new RowDefinition());
            if (cols > 1) while (cols-- > 0) grid.ColumnDefinitions.Add(new ColumnDefinition());

            return grid;
        }
    }

    public interface IProgressSender
    {
        event Action<long> ProgressTotalChanged;
        event Action<long> ProgressIncremented;
    }

    public static class UiProgressBarFactory
    {
        public static UiProgressBar Create()
        {
            return new UiProgressBar();
        }
    }
}
