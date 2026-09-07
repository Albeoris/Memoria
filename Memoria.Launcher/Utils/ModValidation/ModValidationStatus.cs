namespace Memoria.Launcher.Utils.ModValidation
{
    internal enum ModValidationStatus
    {
        Pending,
        Validating,
        Valid,
        Invalid,
        Skipped,
        Error,
        Cancelled
    }
}