using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Timer = System.Threading.Timer;

namespace Memoria.Launcher
{
    public sealed class UiLauncherPlayButton : UiLauncherButton
    {
        public GameSettingsControl GameSettings { get; set; }
        private ManualResetEvent CancelEvent { get; } = new ManualResetEvent(false);

        public UiLauncherPlayButton()
        {
            Label = Lang.Button.Launch;
        }

        protected override async Task DoAction()
        {
            Label = Lang.Button.Launching;
            try
            {
                if (GameSettings.CheckUpdates)
                {
                    String applicationPath = Path.GetFullPath(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
                    String applicationDirectory = Path.GetDirectoryName(applicationPath);
                    HttpFileInfo updateInfo = await FindFirstUpdateInfo(applicationDirectory);
                    if (updateInfo != null)
                    {
                        DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(applicationPath);
                        if (lastWriteTimeUtc < updateInfo.LastModified && MessageBox.Show((Window)this.GetRootElement(), "A new version is available, download now?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            String filePath = updateInfo.TargetPath;

                            using (UiProgressWindow progress = new UiProgressWindow("Downloading..."))
                            {
                                progress.SetTotal(updateInfo.ContentLength);
                                progress.Show();

                                Downloader downloader = new Downloader(CancelEvent);
                                downloader.DownloadProgress += progress.Incremented;
                                await downloader.Download(updateInfo.Url, filePath);
                                File.SetLastWriteTime(filePath, updateInfo.LastModified);
                            }

                            Process.Start(filePath, $@"-update ""{Process.GetCurrentProcess().Id}"" ""{applicationPath}""");
                            Environment.Exit(2);
                            return;
                        }
                    }
                }

                if (GameSettings.ScreenResolution == null)
                {
                    MessageBox.Show((Window)this.GetRootElement(), "Please select an available resolution.", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }

                Int32 activeMonitor = -1;
                if (!String.IsNullOrEmpty(GameSettings.ActiveMonitor))
                {
                    Int32 spaceIndex = GameSettings.ActiveMonitor.IndexOf(' ');
                    if (spaceIndex > 0)
                    {
                        String activeMonitorNumber = GameSettings.ActiveMonitor.Substring(0, spaceIndex);
                        Int32.TryParse(activeMonitorNumber, NumberStyles.Integer, CultureInfo.InvariantCulture, out activeMonitor);
                    }
                }

                if (activeMonitor < 0)
                {
                    MessageBox.Show((Window)this.GetRootElement(), "Please select an available monitor.", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk);
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

                String arguments = $"-runbylauncher -single-instance -monitor {activeMonitor.ToString(CultureInfo.InvariantCulture)} -screen-width {screenWidth.ToString(CultureInfo.InvariantCulture)} -screen-height {screenHeight.ToString(CultureInfo.InvariantCulture)} -screen-fullscreen {(GameSettings.Windowed ? "0" : "1")}";
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
                Label = Lang.Button.Launch;
            }
        }

        private async Task<HttpFileInfo> FindFirstUpdateInfo(String applicationDirectory)
        {
            Downloader downloader = new Downloader(CancelEvent);
            String[] urls = GameSettings.DownloadMirrors;

            foreach (String url in urls)
            {
                try
                {
                    HttpFileInfo fileInfo = await downloader.GetRemoteFileInfo(url);
                    if (fileInfo == null)
                        continue;

                    Int32 separatorIndex = url.LastIndexOf('/');
                    String remoteFileName = url.Substring(separatorIndex + 1);
                    fileInfo.TargetPath = Path.Combine(applicationDirectory, remoteFileName);

                    if (!File.Exists(fileInfo.TargetPath))
                        return fileInfo;

                    if (File.GetLastWriteTime(fileInfo.TargetPath) != fileInfo.LastModified)
                        return fileInfo;
                }
                catch
                {
                    // Do nothing
                }
            }

            return null;;
        }
    }

    public sealed class HttpFileInfo
    {
        public string Url;
        public long ContentLength = -1;
        public DateTime LastModified;
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

            UiTextBlock titleTextBlock = UiTextBlockFactory.Create(title);
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
        private readonly UiTextBlock _progressTextBlock;
        private readonly UiTextBlock _elapsedTextBlock;
        private readonly UiTextBlock _processedTextBlock;
        private readonly UiTextBlock _remainingTextBlock;

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

        public String FormatValue(Int64 value)
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