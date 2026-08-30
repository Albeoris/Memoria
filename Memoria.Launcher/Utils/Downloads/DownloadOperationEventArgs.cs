#nullable enable

using System;

namespace Memoria.Launcher.Utils.Downloads
{
    public sealed class FileDownloadProgressEventArgs : EventArgs
    {
        public FileDownloadProgressEventArgs(DownloadProgress progress)
        {
            Progress = progress;
        }

        public DownloadProgress Progress { get; }
    }

    public sealed class DownloadCompletedEventArgs : EventArgs
    {
        private readonly Exception? _error;
        private readonly DownloadedFile? _downloadedFile;

        private DownloadCompletedEventArgs(DownloadOperationState state, Exception? error, DownloadedFile? downloadedFile)
        {
            State = state;
            _error = error;
            _downloadedFile = downloadedFile;
        }

        public DownloadOperationState State { get; }
        public Boolean IsCancelled
        {
            get
            {
                return State == DownloadOperationState.Cancelled;
            }
        }

        public Boolean IsFailed
        {
            get
            {
                return State == DownloadOperationState.Failed;
            }
        }

        public static DownloadCompletedEventArgs Completed(DownloadedFile downloadedFile)
        {
            return new DownloadCompletedEventArgs(
                DownloadOperationState.Completed,
                null,
                downloadedFile ?? throw new ArgumentNullException(nameof(downloadedFile)));
        }

        public static DownloadCompletedEventArgs Cancelled()
        {
            return new DownloadCompletedEventArgs(DownloadOperationState.Cancelled, null, null);
        }

        public static DownloadCompletedEventArgs Failed(Exception error)
        {
            return new DownloadCompletedEventArgs(
                DownloadOperationState.Failed,
                error ?? throw new ArgumentNullException(nameof(error)),
                null);
        }

        public Exception GetError()
        {
            return _error
                   ?? throw new InvalidOperationException("Only a failed download has an error.");
        }

        public DownloadedFile GetDownloadedFile()
        {
            return _downloadedFile
                   ?? throw new InvalidOperationException("Only a completed download has a downloaded file.");
        }
    }
}
