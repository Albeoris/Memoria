using System;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;

namespace Memoria.Assets
{
    public class AbilityImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(AbilityImporter);
        protected override TextResourceReference ImportPath => ModTextResources.Import.Abilities;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] abilityNames, abilityHelps;
            AbilityFormatter.Parse(entreis, out abilityNames, out abilityHelps);

            FF9TextTool.ImportArrayToDictionary<SupportAbility>(abilityNames, FF9TextTool.SetSupportAbilityName);
            FF9TextTool.ImportArrayToDictionary<SupportAbility>(abilityHelps, FF9TextTool.SetSupportAbilityHelpDesc);
        }

        protected override Boolean LoadInternal()
        {
            FF9TextTool.ImportWithCumulativeModFiles<SupportAbility>(EmbadedTextResources.AbilityNames, FF9TextTool.SetSupportAbilityName);
            FF9TextTool.ImportWithCumulativeModFiles<SupportAbility>(EmbadedTextResources.AbilityHelps, FF9TextTool.SetSupportAbilityHelpDesc);
            return true;
        }
    }
}
