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

        public Boolean IsRunning => State == DownloadOperationState.Running;

        public void Start(Uri source, String destinationPath)
        {
            FileDownloader.ValidateArguments(source, destinationPath);

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

            _ = RunAsync(source, destinationPath, cancellation);
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

        private async Task RunAsync(Uri source, String destinationPath, CancellationTokenSource cancellation)
        {
            DownloadOperationState terminalState;
            Exception error = null;
            try
            {
                await _downloader.DownloadAsync(source, destinationPath, this, cancellation.Token).ConfigureAwait(false);
                terminalState = DownloadOperationState.Completed;
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
                terminalState = DownloadOperationState.Cancelled;
            }
            catch (Exception exception)
            {
                terminalState = DownloadOperationState.Failed;
                error = exception;
            }

            lock (_lock)
            {
                Boolean wasDisposed = _state == DownloadOperationState.Disposed;
                if (!wasDisposed)
                    _state = terminalState;
                if (ReferenceEquals(_cancellation, cancellation))
                    _cancellation = null;

                cancellation.Dispose();
                _downloader.Dispose();
                if (wasDisposed)
                    return;
            }

            Completed?.Invoke(this, new DownloadCompletedEventArgs(terminalState, error));
        }
    }
}
