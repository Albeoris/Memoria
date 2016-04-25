using System;

namespace Memoria
{
    public sealed class ItemLoader : SingleFileExporter
    {
        private const String Prefix = "$item";

        protected override string TypeName => nameof(ItemLoader);
        protected override string ExportPath => ModTextResources.Export.Items;

        protected override TxtEntry[] PrepareEntries()
        {
            String[] itemNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.ItemNames);
            String[] itemHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.ItemHelps);
            String[] itemBattle = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.ItemBattle);

            return ItemFormatter.Build(Prefix, itemNames, itemHelps, itemBattle);
        }
    }
}