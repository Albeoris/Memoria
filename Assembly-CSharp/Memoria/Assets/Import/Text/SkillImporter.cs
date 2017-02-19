using System;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria.Assets
{
    public class SkillImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(SkillImporter);
        protected override String ImportPath => ModTextResources.Import.Skills;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] skillNames, skillHelps;
            AbilityFormatter.Parse(entreis, out skillNames, out skillHelps);

            FF9TextTool.SetActionAbilityName(skillNames);
            FF9TextTool.SetActionAbilityHelpDesc(skillHelps);
        }

        protected override Boolean LoadInternal()
        {
            String[] skillNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.SkillNames);
            String[] skillHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.SkillHelps);

            FF9TextTool.SetActionAbilityName(skillNames);
            FF9TextTool.SetActionAbilityHelpDesc(skillHelps);
            return true;
        }
    }
}