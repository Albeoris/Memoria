using System;
using System.Threading;
using System.Threading.Tasks;

namespace Memoria.Launcher.Utils.Downloads
{
    public sealed class DownloadFileOperation : IDisposable, IProgress<DownloadProgress>
    {
        private readonly Object _lock = new Object();
        private readonly FileDownloader _downloader = new FileDownloader();
        private CancellationTokenSource _cancellation;
        private DownloadOperationState _state = DownloadOperationState.Created;

        public event EventHandler<FileDownloadProgressEventArgs> ProgressChanged;
        public event EventHandler<DownloadCompletedEventArgs> Completed;

        public DownloadOperationState State
        {
            get
            {
                lock (_lock)
                    return _state;
            }
        }

        public Boolean IsRunning
        {
            get
            {
                return State == DownloadOperationState.Running;
            }
        }

        public void Start(Uri source, String destinationPath)
        {
            FileDownloader.ValidateSource(source);
            StartCore(source, DownloadDestination.ForFile(destinationPath));
        }

        public void StartInDirectory(Uri source, String destinationDirectory)
        {
            FileDownloader.ValidateSource(source);
            StartCore(source, DownloadDestination.InDirectory(destinationDirectory));
        }

        private void StartCore(Uri source, DownloadDestination destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            CancellationTokenSource cancellation;
            lock (_lock)
            {
                if (_state == DownloadOperationState.Disposed)
                    throw new ObjectDisposedException(nameof(DownloadFileOperation));
                if (_state != DownloadOperationState.Created)
                    throw new InvalidOperationException("A download operation can only be started once.");

                _cancellation = new CancellationTokenSource();
                cancellation = _cancellation;
                _state = DownloadOperationState.Running;
            }

            _ = RunAsync(source, destination, cancellation);
        }

        public void Cancel()
        {
            lock (_lock)
            {
                if (_state == DownloadOperationState.Disposed)
                    throw new ObjectDisposedException(nameof(DownloadFileOperation));

                _cancellation?.Cancel();
            }
        }

        void IProgress<DownloadProgress>.Report(DownloadProgress value)
        {
            ProgressChanged?.Invoke(this, new FileDownloadProgressEventArgs(value));
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_state == DownloadOperationState.Disposed)
                    return;

                _cancellation?.Cancel();
                if (_state != DownloadOperationState.Running)
                {
                    _downloader.Dispose();
                    _cancellation?.Dispose();
                    _cancellation = null;
                }
                _state = DownloadOperationState.Disposed;
            }
        }

        private async Task RunAsync(Uri source, DownloadDestination destination, CancellationTokenSource cancellation)
        {
            DownloadCompletedEventArgs result;
            try
            {
                DownloadedFile downloadedFile = await _downloader
                    .DownloadAsync(source, destination, this, cancellation.Token)
                    .ConfigureAwait(false);
                result = DownloadCompletedEventArgs.Completed(downloadedFile);
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
                result = DownloadCompletedEventArgs.Cancelled();
            }
            catch (Exception exception)
            {
                result = DownloadCompletedEventArgs.Failed(exception);
            }

            lock (_lock)
            {
                Boolean wasDisposed = _state == DownloadOperationState.Disposed;
                if (!wasDisposed)
                    _state = result.State;
                if (ReferenceEquals(_cancellation, cancellation))
                    _cancellation = null;

                cancellation.Dispose();
                _downloader.Dispose();
                if (wasDisposed)
                    return;
            }

            Completed?.Invoke(this, result);
        }
    }
}
