using System;
using Assets.Sources.Scripts.UI.Common;

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

            FF9TextTool.SetItemName(itemNames);
            FF9TextTool.SetItemHelpDesc(itemHelps);
            FF9TextTool.SetItemBattleDesc(itemBattle);
        }

        protected override Boolean LoadInternal()
        {
            String[] itemNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.ItemNames);
            String[] itemHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.ItemHelps);
            String[] itemBattle = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.ItemBattle);

            FF9TextTool.SetItemName(itemNames);
            FF9TextTool.SetItemHelpDesc(itemHelps);
            FF9TextTool.SetItemBattleDesc(itemBattle);
            return true;
        }
    }
}