namespace Memoria.Launcher.Utils.Archives
{
    public enum SevenZipExitCode
    {
        Success = 0,
        Warning = 1,
        FatalError = 2,
        CommandLineError = 7,
        NotEnoughMemory = 8,
        Cancelled = 255
    }
}
