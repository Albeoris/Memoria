using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using System;

namespace Memoria.Assets
{
	public sealed class ItemImporter : SingleFileImporter
	{
		protected override String TypeName => nameof(ItemImporter);
		protected override String ImportPath => ModTextResources.Import.Items;

		protected override void ProcessEntries(TxtEntry[] entreis)
		{
			String[] itemNames, itemHelps, itemBattle;
			ItemFormatter.Parse(entreis, out itemNames, out itemHelps, out itemBattle);

			FF9TextTool.ImportArrayToDictionary<RegularItem>(itemNames, FF9TextTool.SetItemName);
			FF9TextTool.ImportArrayToDictionary<RegularItem>(itemHelps, FF9TextTool.SetItemHelpDesc);
			FF9TextTool.ImportArrayToDictionary<RegularItem>(itemBattle, FF9TextTool.SetItemBattleDesc);
		}

		protected override Boolean LoadInternal()
		{
			FF9TextTool.ImportWithCumulativeModFiles<RegularItem>(EmbadedTextResources.ItemNames, FF9TextTool.SetItemName);
			FF9TextTool.ImportWithCumulativeModFiles<RegularItem>(EmbadedTextResources.ItemHelps, FF9TextTool.SetItemHelpDesc);
			FF9TextTool.ImportWithCumulativeModFiles<RegularItem>(EmbadedTextResources.ItemBattle, FF9TextTool.SetItemBattleDesc);
			return true;
		}
	}
}
