using System;

namespace Memoria.Assets
{
    public class AbilityExporter : SingleFileExporter
    {
        private const String Prefix = "$ability";

        protected override String TypeName => nameof(AbilityExporter);
        protected override TextResourcePath ExportPath => ModTextResources.Export.Abilities;

        protected override TxtEntry[] PrepareEntries()
        {
            String[] abilityNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.AbilityNames);
            String[] abilityHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.AbilityHelps);

            return AbilityFormatter.Build(Prefix, abilityNames, abilityHelps);
        }
    }
}
