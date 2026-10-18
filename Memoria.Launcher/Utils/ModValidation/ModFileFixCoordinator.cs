#nullable disable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class ModFileFixCoordinator
    {
        private readonly ModValidationService _validationService;
        private readonly ModFixService _fixService;
        private readonly ModFixChangelogStore _changelogStore;

        public ModFileFixCoordinator(ModValidationService validationService, ModFixService fixService, ModFixChangelogStore changelogStore)
        {
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _fixService = fixService ?? throw new ArgumentNullException(nameof(fixService));
            _changelogStore = changelogStore ?? throw new ArgumentNullException(nameof(changelogStore));
        }

        public async Task<ModFileFixOutcome> FixAsync(ModValidationResult initialValidationResult, CancellationToken cancellationToken)
        {
            if (initialValidationResult == null)
                throw new ArgumentNullException(nameof(initialValidationResult));

            ModValidationResult validationResult = initialValidationResult;
            HashSet<String> attemptedFixerNames = new(StringComparer.Ordinal);
            List<String> appliedFixerNames = new();
            List<ModFixChange> accumulatedChanges = new();
            ModFixChangelogEntry latestChangelogEntry = null;

            while (validationResult.HasProblem)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ModFileFixResult fixResult = await Task.Run(() => _fixService.FixNext(validationResult, attemptedFixerNames, cancellationToken), cancellationToken);
                if (fixResult == null)
                    break;

                attemptedFixerNames.Add(fixResult.FixerName);
                if (!fixResult.Changed)
                    continue;

                appliedFixerNames.Add(fixResult.FixerName);
                accumulatedChanges.AddRange(fixResult.Changes);
                ModFileFixResult accumulatedResult = new ModFileFixResult(fixResult.File, String.Join(" + ", appliedFixerNames), accumulatedChanges);
                latestChangelogEntry = _changelogStore.Append(accumulatedResult);
                validationResult = await _validationService.ValidateFileAsync(fixResult.File, cancellationToken);
            }

            if (validationResult.Status == ModValidationStatus.Valid && latestChangelogEntry != null)
                validationResult = validationResult.WithFixChangelog(latestChangelogEntry);
            return new ModFileFixOutcome(validationResult, appliedFixerNames.Count);
        }
    }
}
