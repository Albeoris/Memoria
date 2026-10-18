#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class ModValidationResult
    {
        public ModValidationResult(ModValidationFile file, ModValidationStatus status, String validatorName, IEnumerable<ModValidationDiagnostic> diagnostics, ModFixChangelogEntry fixChangelog = null)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            Status = status;
            ValidatorName = validatorName ?? String.Empty;
            Diagnostics = new ReadOnlyCollection<ModValidationDiagnostic>((diagnostics ?? Enumerable.Empty<ModValidationDiagnostic>()).ToList());
            FixChangelog = fixChangelog;
        }

        public ModValidationFile File { get; }
        public ModValidationStatus Status { get; }
        public String ValidatorName { get; }
        public IReadOnlyList<ModValidationDiagnostic> Diagnostics { get; }
        public ModFixChangelogEntry FixChangelog { get; }
        public Boolean HasProblem => Status == ModValidationStatus.Invalid || Status == ModValidationStatus.Error;

        public ModValidationResult WithFixChangelog(ModFixChangelogEntry fixChangelog) => new(File, ModValidationStatus.Fixed, ValidatorName, Diagnostics, fixChangelog ?? throw new ArgumentNullException(nameof(fixChangelog)));
    }
}
