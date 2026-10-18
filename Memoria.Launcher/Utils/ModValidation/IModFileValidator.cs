using System;
using System.Threading;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal interface IModFileValidator
    {
        String Name { get; }
        Boolean CanValidate(ModValidationFile file);
        ModValidationResult Validate(ModValidationFile file, CancellationToken cancellationToken);
    }
}