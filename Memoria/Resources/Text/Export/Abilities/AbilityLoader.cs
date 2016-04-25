using System;
using System.IO;

namespace Memoria
{
    public class AbilityLoader : SingleFileExporter
    {
        private const String Prefix = "$ability";

        protected override string TypeName => nameof(AbilityLoader);
        protected override string ExportPath => ModTextResources.Export.Abilities;

        protected override TxtEntry[] PrepareEntries()
        {
            String[] abilityNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.AbilityNames);
            String[] abilityHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.AbilityHelps);

            return AbilityFormatter.Build(Prefix, abilityNames, abilityHelps);
        }
    }
}