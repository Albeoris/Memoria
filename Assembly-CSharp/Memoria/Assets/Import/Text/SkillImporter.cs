using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using System;

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

			FF9TextTool.ImportArrayToDictionary<BattleAbilityId>(skillNames, FF9TextTool.SetActionAbilityName);
			FF9TextTool.ImportArrayToDictionary<BattleAbilityId>(skillHelps, FF9TextTool.SetActionAbilityHelpDesc);
		}

		protected override Boolean LoadInternal()
		{
			FF9TextTool.ImportWithCumulativeModFiles<BattleAbilityId>(EmbadedTextResources.SkillNames, FF9TextTool.SetActionAbilityName);
			FF9TextTool.ImportWithCumulativeModFiles<BattleAbilityId>(EmbadedTextResources.SkillHelps, FF9TextTool.SetActionAbilityHelpDesc);
			return true;
		}
	}
}
