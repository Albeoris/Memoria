using System;
using System.IO;

namespace Memoria
{
    public class AbilityExporter : SingleFileExporter
    {
        private const String Prefix = "$ability";

        protected override String TypeName => nameof(AbilityExporter);
        protected override String ExportPath => ModTextResources.Export.Abilities;

        protected override TxtEntry[] PrepareEntries()
        {
            String[] abilityNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.AbilityNames);
            String[] abilityHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.AbilityHelps);

            return AbilityFormatter.Build(Prefix, abilityNames, abilityHelps);
        }
    }
}