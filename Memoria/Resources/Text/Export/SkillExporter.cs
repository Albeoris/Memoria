using System;

namespace Memoria
{
    public sealed class SkillExporter : SingleFileExporter
    {
        private const String Prefix = "$skill";

        protected override string TypeName => nameof(SkillExporter);
        protected override string ExportPath => ModTextResources.Export.Skills;

        protected override TxtEntry[] PrepareEntries()
        {
            String[] skillNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.SkillNames);
            String[] skillHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.SkillHelps);

            return AbilityFormatter.Build(Prefix, skillNames, skillHelps);
        }
    }
}