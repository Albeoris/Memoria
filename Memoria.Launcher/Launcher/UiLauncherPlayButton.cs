using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Memoria.Launcher
{
    public sealed class UiLauncherPlayButton : UiLauncherButton
    {
        public SettingsGrid_Vanilla GameSettings { get; set; }
        public SettingsGrid_VanillaDisplay GameSettingsDisplay { get; set; }
        private ManualResetEvent CancelEvent { get; } = new ManualResetEvent(false);

        public UiLauncherPlayButton()
        {
            Label = Lang.Launcher.Launch;
        }

        protected override async Task DoAction()
        {
            Label = Lang.Launcher.Launching;
            try
            {
                try
                {
                    IniFile iniFile = new(IniFile.IniPath);
                    if (LaunchModelViewer)
                    {
                        iniFile.WriteValue("Debug", "Enabled", "1");
                        iniFile.WriteValue("Debug", "StartModelViewer", "1");
                    }
                    else
                    {
                        iniFile.WriteValue("Debug", "StartModelViewer", "0");
                    }
                }
                catch (Exception) { }

                if (GameSettings.CheckUpdates)
                {
                    if (await CheckUpdates((Window)this.GetRootElement(), CancelEvent, GameSettings))
                        return;
                }

                if (GameSettingsDisplay.ScreenResolution == null)
                {
                    MessageBox.Show((Window)this.GetRootElement(), "Please select an available resolution.", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }

                Int32 activeMonitor = -1;
                if (!String.IsNullOrEmpty(GameSettingsDisplay.ActiveMonitor))
                {
                    Int32 spaceIndex = GameSettingsDisplay.ActiveMonitor.IndexOf(' ');
                    if (spaceIndex > 0)
                    {
                        String activeMonitorNumber = GameSettingsDisplay.ActiveMonitor.Substring(0, spaceIndex);
                        Int32.TryParse(activeMonitorNumber, NumberStyles.Integer, CultureInfo.InvariantCulture, out activeMonitor);
                    }
                }

                if (activeMonitor < 0)
                {
                    MessageBox.Show((Window)this.GetRootElement(), "Please select an available monitor.", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }

                String[] strArray = GameSettingsDisplay.ScreenResolution.Split(' ')[0].Split('x');
                Int32 screenWidth;
                Int32 screenHeight;
                if (strArray.Length < 2 || !Int32.TryParse(strArray[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out screenWidth) || !Int32.TryParse(strArray[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out screenHeight))
                {
                    MessageBox.Show((Window)this.GetRootElement(), "Please select an available resolution.", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }

                String directoyPath = Path.GetFullPath(".\\" + (GameSettings.IsX64 ? "x64" : "x86"));

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

                    String ff9DataPath = directoyPath + "\\FF9_Data";
                    String unityDataPath = directoyPath + "\\Unity_Data";

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

                String arguments = $"-runbylauncher -single-instance -monitor {activeMonitor.ToString(CultureInfo.InvariantCulture)} -screen-width {screenWidth.ToString(CultureInfo.InvariantCulture)} -screen-height {screenHeight.ToString(CultureInfo.InvariantCulture)} -screen-fullscreen {((GameSettingsDisplay.WindowMode == 0 ^ GameSettingsDisplay.WindowMode == 2) ? "0" : "1")} {(GameSettingsDisplay.WindowMode == 2 ? "-popupwindow" : "")}";
                await Task.Factory.StartNew(
                    () =>
                    {
                        ProcessStartInfo gameStartInfo = new ProcessStartInfo(executablePath, arguments) { UseShellExecute = false };
                        if (GameSettings.IsDebugMode)
                            gameStartInfo.EnvironmentVariables["UNITY_GIVE_CHANCE_TO_ATTACH_DEBUGGER"] = "1";

                        Process gameProcess = new Process { StartInfo = gameStartInfo };
                        gameProcess.Start();

                        if (GameSettings.IsDebugMode)
                        {
                            Process debuggerProcess = Process.GetProcesses().FirstOrDefault(p => p.ProcessName.StartsWith("Memoria.Debugger"));
                            if (debuggerProcess == null)
                            {
                                try
                                {
                                    String debuggerDirectory = Path.Combine(Path.GetFullPath("Debugger"), (GameSettings.IsX64 ? "x64" : "x86"));
                                    String debuggerPath = Path.Combine(debuggerDirectory, "Memoria.Debugger.exe");
                                    String debuggerArgs = "10000"; // Timeout: 10 seconds
                                    if (Directory.Exists(debuggerDirectory) && File.Exists(debuggerPath))
                                    {
                                        ProcessStartInfo debuggerStartInfo = new ProcessStartInfo(debuggerPath, debuggerArgs) { WorkingDirectory = debuggerDirectory };
                                        debuggerProcess = new Process { StartInfo = debuggerStartInfo };
                                        debuggerProcess.Start();
                                    }
                                }
                                catch (Exception)
                                {
                                }

                            }
                        }
                    }
                );
                Application.Current.Shutdown();
            }
            finally
            {
                Label = Lang.Launcher.Launch;
            }
        }

        internal static async Task<Boolean> CheckUpdates(Window rootElement, ManualResetEvent cancelEvent, SettingsGrid_Vanilla gameSettings)
        {
            String applicationPath = Path.GetFullPath(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            String applicationDirectory = Path.GetDirectoryName(applicationPath);
            LinkedList<HttpFileInfo> updateInfo = await FindUpdatesInfo(applicationDirectory, cancelEvent, gameSettings);
            if (updateInfo.Count == 0)
                return false;

            StringBuilder messageSb = new StringBuilder(256);
            messageSb.AppendLine(Lang.Launcher.NewVersionIsAvailable);
            Int64 size = 0;
            foreach (HttpFileInfo info in updateInfo)
            {
                size += info.ContentLength;
                messageSb.AppendLine($"{info.TargetName} - {info.LastModified} ({UiProgressWindow.FormatValue(info.ContentLength)})");
            }

            if (MessageBox.Show(rootElement, messageSb.ToString(), Lang.Launcher.QuestionTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<String> success = new List<String>(updateInfo.Count);
                List<String> failed = new List<String>();

                using (UiProgressWindow progress = new UiProgressWindow("Downloading..."))
                {
                    progress.SetTotal(size);
                    progress.Show();

                    Downloader downloader = new Downloader(cancelEvent);
                    downloader.DownloadProgress += progress.Incremented;

                    foreach (HttpFileInfo info in updateInfo)
                    {
                        String filePath = info.TargetPath;

                        try
                        {
                            await downloader.Download(info.Url, filePath);
                            File.SetLastWriteTime(filePath, info.LastModified);

                            success.Add(filePath);
                        }
                        catch
                        {
                            failed.Add(filePath);
                        }
                    }
                    progress.Close();
                }

                Boolean runPatcher = false;
                if (failed.Count > 0)
                {
                    MessageBox.Show(rootElement,
                        "Failed to download:" + Environment.NewLine + String.Join(Environment.NewLine, failed),
                        Lang.Launcher.ErrorTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }

                if (success.Count > 0)
                {
                    runPatcher = MessageBox.Show(rootElement,
                        "Download successful!\nRun the patcher?",
                        Lang.Launcher.QuestionTitle,
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



        private static async Task<LinkedList<HttpFileInfo>> FindUpdatesInfo(String applicationDirectory, ManualResetEvent cancelEvent, SettingsGrid_Vanilla gameSettings)
        {
            Downloader downloader = new Downloader(cancelEvent);
            String[] urls = gameSettings.DownloadMirrors;

            LinkedList<HttpFileInfo> list = new LinkedList<HttpFileInfo>();
            Dictionary<String, LinkedListNode<HttpFileInfo>> dic = new Dictionary<String, LinkedListNode<HttpFileInfo>>(urls.Length);

            foreach (String url in urls)
            {
                try
                {
                    HttpFileInfo fileInfo = await downloader.GetRemoteFileInfo(url);
                    if (fileInfo == null)
                        continue;

                    Int32 separatorIndex = url.LastIndexOf('/');
                    String remoteFileName = url.Substring(separatorIndex + 1);
                    fileInfo.TargetName = remoteFileName;
                    fileInfo.TargetPath = Path.Combine(applicationDirectory, remoteFileName);

                    LinkedListNode<HttpFileInfo> node;
                    if (!dic.TryGetValue(fileInfo.TargetPath, out node) && File.Exists(fileInfo.TargetPath) && File.GetLastWriteTime(fileInfo.TargetPath) >= fileInfo.LastModified)
                        continue;

                    if (node != null)
                    {
                        if (node.Value.LastModified >= fileInfo.LastModified)
                            continue;

                        LinkedListNode<HttpFileInfo> newNode = list.AddBefore(node, fileInfo);
                        list.Remove(node);
                        dic[fileInfo.TargetPath] = newNode;
                    }
                    else
                    {
                        LinkedListNode<HttpFileInfo> newNode = list.AddLast(fileInfo);
                        dic.Add(fileInfo.TargetPath, newNode);
                    }
                }
                catch
                {
                    // Do nothing
                }
            }

            return list; ;
        }
    }

    public sealed class HttpFileInfo
    {
        public string Url;
        public long ContentLength = -1;
        public DateTime LastModified;
        public String TargetName;
        public String TargetPath;

        public void ReadFromResponse(string url, WebResponse response)
        {
            Url = url;
            ContentLength = response.ContentLength;
            LastModified = ((HttpWebResponse)response).LastModified;
        }
    }

    public sealed class Downloader
    {
        private readonly ManualResetEvent _cancelEvent;

        public event Action<long> DownloadProgress;

        public Downloader(ManualResetEvent cancelEvent)
        {
            _cancelEvent = cancelEvent;
        }

        public async Task<HttpFileInfo> GetRemoteFileInfo(string url)
        {
            HttpFileInfo result = new HttpFileInfo();

            if (_cancelEvent.WaitOne(0))
                return result;

            WebRequest request = WebRequest.Create(url);
            request.Method = "HEAD";

            using (WebResponse resp = await request.GetResponseAsync())
            {
                if (_cancelEvent.WaitOne(0))
                    return result;

                result.ReadFromResponse(url, resp);
                return result;
            }
        }

        public async Task Download(string url, string fileName)
        {
            if (_cancelEvent.WaitOne(0))
                return;

            using (Stream output = File.Create(fileName))
                await Download(url, output);
        }

        private async Task Download(String url, Stream output)
        {
            if (_cancelEvent.WaitOne(0))
                return;

            using (HttpClient client = new HttpClient())
            using (Stream input = await client.GetStreamAsync(url))
                await CopyAsync(input, output, _cancelEvent, DownloadProgress);
        }

        private static async Task CopyAsync(Stream input, Stream output, ManualResetEvent cancelEvent, Action<long> progress)
        {
            byte[] buff = new byte[32 * 1024];

            int read;
            while ((read = await input.ReadAsync(buff, 0, buff.Length)) != 0)
            {
                if (cancelEvent.WaitOne(0))
                    return;

                await output.WriteAsync(buff, 0, read);
                progress?.Invoke(read);
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

            _elapsedTextBlock = UiTextBlockFactory.Create(Lang.Measurement.Elapsed + ": 00:00");
            {
                _elapsedTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                root.AddUiElement(_elapsedTextBlock, 2, 0);
            }

            _processedTextBlock = UiTextBlockFactory.Create("0 / 0");
            {
                _processedTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                root.AddUiElement(_processedTextBlock, 2, 0);
            }

            _remainingTextBlock = UiTextBlockFactory.Create(Lang.Measurement.Remaining + ": 00:00");
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
            _elapsedTextBlock.Text = String.Format("{1}: {0:mm\\:ss}", elapsed, Lang.Measurement.Elapsed);
            _processedTextBlock.Text = $"{FormatValue(_processedCount)} / {FormatValue(_totalCount)}";
            _remainingTextBlock.Text = String.Format("{1}: {0:mm\\:ss}", left, Lang.Measurement.Remaining);

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
                    return String.Format("{0:F2} " + Lang.Measurement.ByteAbbr, dec);
                case 1:
                    return String.Format("{0:F2} " + Lang.Measurement.KByteAbbr, dec);
                case 2:
                    return String.Format("{0:F2} " + Lang.Measurement.MByteAbbr, dec);
                case 3:
                    return String.Format("{0:F2} " + Lang.Measurement.GByteAbbr, dec);
                case 4:
                    return String.Format("{0:F2} " + Lang.Measurement.TByteAbbr, dec);
                case 5:
                    return String.Format("{0:F2} " + Lang.Measurement.PByteAbbr, dec);
                case 6:
                    return String.Format("{0:F2} " + Lang.Measurement.EByteAbbr, dec);
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
