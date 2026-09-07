using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Memoria.Launcher.Utils.Downloads;

namespace Memoria.Launcher.Utils.Updates
{
    internal sealed class UpdateDownloadWindow : Window, IDisposable
    {
        private readonly CancellationTokenSource _cancellation;
        private readonly ProgressBar _progressBar;
        private readonly TextBlock _status;
        private Boolean _allowClose;
        private Boolean _completionRequested;

        public UpdateDownloadWindow(String title, CancellationTokenSource cancellation)
        {
            _cancellation = cancellation ?? throw new ArgumentNullException(nameof(cancellation));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Width = 430;
            Height = 145;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            Grid root = new Grid { Margin = new Thickness(18) };
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(16) });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            _status = new TextBlock { HorizontalAlignment = HorizontalAlignment.Center, Text = title, TextWrapping = TextWrapping.Wrap };
            _progressBar = new ProgressBar { Height = 20, Minimum = 0, Maximum = 100, IsIndeterminate = true };
            Grid.SetRow(_progressBar, 2);
            root.Children.Add(_status);
            root.Children.Add(_progressBar);
            Content = root;
            Loaded += OnLoaded;
            Closing += OnClosing;
        }

        public Boolean UserCancellationRequested { get; private set; }

        public void Report(DownloadProgress progress)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() => Report(progress)));
                return;
            }

            if (progress.TotalBytes > 0)
            {
                _progressBar.IsIndeterminate = false;
                _progressBar.Value = progress.Percentage;
                _status.Text = $"{progress.Percentage}% — {FormatBytes(progress.BytesReceived)} / {FormatBytes(progress.TotalBytes)}";
            }
            else
            {
                _progressBar.IsIndeterminate = true;
                _status.Text = FormatBytes(progress.BytesReceived);
            }
        }

        public void CompleteAndClose()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(CompleteAndClose));
                return;
            }

            _completionRequested = true;
            if (!IsVisible)
                return;
            _allowClose = true;
            Close();
        }

        public void Dispose()
        {
            Closing -= OnClosing;
            Loaded -= OnLoaded;
        }

        private void OnLoaded(Object sender, RoutedEventArgs e)
        {
            if (_completionRequested)
                Dispatcher.BeginInvoke(new Action(CompleteAndClose));
        }

        private void OnClosing(Object sender, CancelEventArgs e)
        {
            if (_allowClose)
                return;

            MessageBoxResult answer = MessageBox.Show(this, GetText("Updater.CancelDownloadQuestion"), GetText("Updater.CancelDownloadTitle"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (answer != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            UserCancellationRequested = true;
            _allowClose = true;
            _cancellation.Cancel();
        }

        private static String FormatBytes(Int64 bytes)
        {
            String[] units = { GetText("Measurement.ByteAbbr"), GetText("Measurement.KByteAbbr"), GetText("Measurement.MByteAbbr"), GetText("Measurement.GByteAbbr"), GetText("Measurement.TByteAbbr") };
            Double value = Math.Max(0, bytes);
            Int32 index = 0;
            while (value >= 1024 && index < units.Length - 1)
            {
                value /= 1024;
                index++;
            }
            return $"{value:0.##} {units[index]}";
        }

        private static String GetText(String key)
        {
            return Lang.Res[key] as String ?? key;
        }
    }
}
