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
        public DownloadCompletedEventArgs(DownloadOperationState state, Exception error)
        {
            if (state != DownloadOperationState.Completed
                && state != DownloadOperationState.Cancelled
                && state != DownloadOperationState.Failed)
            {
                throw new ArgumentOutOfRangeException(nameof(state), state, "The state must be terminal.");
            }
            if (state == DownloadOperationState.Failed && error == null)
                throw new ArgumentNullException(nameof(error));
            if (state != DownloadOperationState.Failed && error != null)
                throw new ArgumentException("Only a failed operation can contain an error.", nameof(error));

            State = state;
            Error = error;
        }

        public DownloadOperationState State { get; }
        public Exception Error { get; }
        public Boolean IsCancelled => State == DownloadOperationState.Cancelled;
    }
}
