#nullable disable
using System;
using System.Threading;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal interface IModFileFixer
    {
        String Name { get; }
        Boolean CanFix(ModValidationResult validationResult);
        ModFileFixResult Fix(ModValidationResult validationResult, CancellationToken cancellationToken);
    }
}
