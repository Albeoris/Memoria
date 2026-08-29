namespace Memoria.Launcher.Utils.Downloads
{
    public enum DownloadFailureKind
    {
        Network = 1,
        HttpResponse,
        AccessDenied,
        Storage,
        IncompleteContent
    }
}
