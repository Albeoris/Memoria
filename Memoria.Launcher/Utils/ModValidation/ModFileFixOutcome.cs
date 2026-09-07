#nullable disable
using System;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class ModFileFixOutcome
    {
        public ModFileFixOutcome(ModValidationResult validationResult, Int32 appliedFixerCount)
        {
            ValidationResult = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
            AppliedFixerCount = appliedFixerCount;
        }

        public ModValidationResult ValidationResult { get; }
        public Int32 AppliedFixerCount { get; }
        public Boolean Changed => AppliedFixerCount > 0;
    }
}
