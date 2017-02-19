using System;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria.Assets
{
    public class AbilityImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(AbilityImporter);
        protected override String ImportPath => ModTextResources.Import.Abilities;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] abilityNames, abilityHelps;
            AbilityFormatter.Parse(entreis, out abilityNames, out abilityHelps);

            FF9TextTool.SetSupportAbilityName(abilityNames);
            FF9TextTool.SetSupportAbilityHelpDesc(abilityHelps);
        }

        protected override Boolean LoadInternal()
        {
            String[] abilityNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.AbilityNames);
            String[] abilityHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.AbilityHelps);

            FF9TextTool.SetSupportAbilityName(abilityNames);
            FF9TextTool.SetSupportAbilityHelpDesc(abilityHelps);
            return true;
        }
    }
}