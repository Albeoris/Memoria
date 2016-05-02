using System;

namespace Memoria
{
    public sealed class ItemImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(ItemImporter);
        protected override String ImportPath => ModTextResources.Import.Items;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] itemNames, itemHelps, itemBattle;
            ItemFormatter.Parse(entreis, out itemNames, out itemHelps, out itemBattle);

            FF9TextToolAccessor.SetItemName(itemNames);
            FF9TextToolAccessor.SetItemHelpDesc(itemHelps);
            FF9TextToolAccessor.SetItemBattleDesc(itemBattle);
        }

        protected override Boolean LoadInternal()
        {
            String[] itemNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.ItemNames);
            String[] itemHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.ItemHelps);
            String[] itemBattle = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.ItemBattle);

            FF9TextToolAccessor.SetItemName(itemNames);
            FF9TextToolAccessor.SetItemHelpDesc(itemHelps);
            FF9TextToolAccessor.SetItemBattleDesc(itemBattle);
            return true;
        }
    }
}