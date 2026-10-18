#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class ModFileFixResult
    {
        public ModFileFixResult(ModValidationFile file, String fixerName, IEnumerable<ModFixChange> changes)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            FixerName = fixerName ?? throw new ArgumentNullException(nameof(fixerName));
            Changes = new ReadOnlyCollection<ModFixChange>((changes ?? throw new ArgumentNullException(nameof(changes))).ToList());
        }

        public ModValidationFile File { get; }
        public String FixerName { get; }
        public IReadOnlyList<ModFixChange> Changes { get; }
        public Boolean Changed => Changes.Count > 0;
    }
}
