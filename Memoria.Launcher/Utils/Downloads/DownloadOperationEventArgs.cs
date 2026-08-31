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

    public abstract class DownloadCompletedEventArgs : EventArgs
    {
        private DownloadCompletedEventArgs(DownloadOperationState state)
        {
            State = state;
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
            return new SuccessfulDownload(downloadedFile);
        }

        public static DownloadCompletedEventArgs Cancelled()
        {
            return new CancelledDownload();
        }

        public static DownloadCompletedEventArgs Failed(Exception error)
        {
            return new FailedDownload(error);
        }

        public virtual Exception GetError()
        {
            throw new InvalidOperationException("Only a failed download has an error.");
        }

        public virtual DownloadedFile GetDownloadedFile()
        {
            throw new InvalidOperationException("Only a completed download has a downloaded file.");
        }

        private sealed class SuccessfulDownload : DownloadCompletedEventArgs
        {
            private readonly DownloadedFile _downloadedFile;

            public SuccessfulDownload(DownloadedFile downloadedFile)
                : base(DownloadOperationState.Completed)
            {
                _downloadedFile = downloadedFile ?? throw new ArgumentNullException(nameof(downloadedFile));
            }

            public override DownloadedFile GetDownloadedFile() => _downloadedFile;
        }

        private sealed class FailedDownload : DownloadCompletedEventArgs
        {
            private readonly Exception _error;

            public FailedDownload(Exception error)
                : base(DownloadOperationState.Failed)
            {
                _error = error ?? throw new ArgumentNullException(nameof(error));
            }

            public override Exception GetError() => _error;
        }

        private sealed class CancelledDownload : DownloadCompletedEventArgs
        {
            public CancelledDownload()
                : base(DownloadOperationState.Cancelled)
            {
            }
        }
    }
}
