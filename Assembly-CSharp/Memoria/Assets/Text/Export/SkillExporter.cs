using System;

namespace Memoria.Assets
{
    public sealed class SkillExporter : SingleFileExporter
    {
        private const String Prefix = "$skill";

        protected override String TypeName => nameof(SkillExporter);
        protected override String ExportPath => ModTextResources.Export.Skills;

        protected override TxtEntry[] PrepareEntries()
        {
            String[] skillNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.SkillNames);
            String[] skillHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.SkillHelps);

            return AbilityFormatter.Build(Prefix, skillNames, skillHelps);
        }
    }
}