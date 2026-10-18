namespace Memoria.Launcher.Utils.Downloads
{
    public enum DownloadOperationState
    {
        Created = 1,
        Running,
        Completed,
        Cancelled,
        Failed,
        Disposed
    }
}
