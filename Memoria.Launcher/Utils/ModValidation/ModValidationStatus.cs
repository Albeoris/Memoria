namespace Memoria.Launcher.Utils.ModValidation
{
    internal enum ModValidationStatus
    {
        Pending,
        Validating,
        Valid,
        Fixed,
        Invalid,
        Skipped,
        Error,
        Cancelled
    }
}
