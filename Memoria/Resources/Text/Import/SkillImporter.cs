using System;

namespace Memoria
{
    public class SkillImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(SkillImporter);
        protected override String ImportPath => ModTextResources.Import.Skills;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] skillNames, skillHelps;
            AbilityFormatter.Parse(entreis, out skillNames, out skillHelps);

            FF9TextToolAccessor.SetActionAbilityName(skillNames);
            FF9TextToolAccessor.SetActionAbilityHelpDesc(skillHelps);
        }

        protected override Boolean LoadInternal()
        {
            String[] skillNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.SkillNames);
            String[] skillHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.SkillHelps);

            FF9TextToolAccessor.SetActionAbilityName(skillNames);
            FF9TextToolAccessor.SetActionAbilityHelpDesc(skillHelps);
            return true;
        }
    }
}