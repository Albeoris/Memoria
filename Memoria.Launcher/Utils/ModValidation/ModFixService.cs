#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class ModFixService
    {
        private readonly IReadOnlyList<IModFileFixer> _fixers;

        public ModFixService(IEnumerable<IModFileFixer> fixers)
        {
            _fixers = (fixers ?? throw new ArgumentNullException(nameof(fixers))).ToList();
        }

        public Boolean CanFix(ModValidationResult validationResult) => validationResult != null && _fixers.Any(fixer => fixer.CanFix(validationResult));

        public ModFileFixResult FixNext(ModValidationResult validationResult, ISet<String> excludedFixerNames, CancellationToken cancellationToken)
        {
            if (excludedFixerNames == null)
                throw new ArgumentNullException(nameof(excludedFixerNames));
            IModFileFixer fixer = _fixers.FirstOrDefault(candidate => !excludedFixerNames.Contains(candidate.Name) && candidate.CanFix(validationResult));
            return fixer?.Fix(validationResult, cancellationToken);
        }
    }
}
