using System;
using System.IO;

namespace Memoria
{
    public sealed class KeyItemImporter : SingleFileImporter
    {
        protected override string TypeName => nameof(KeyItemImporter);
        protected override String ImportPath => ModTextResources.Import.KeyItems;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] itemNames, itemHelps, itemDescs;
            KeyItemFormatter.Parse(entreis, out itemNames, out itemHelps, out itemDescs);

            FF9TextToolAccessor.SetImportantItemName(itemNames);
            FF9TextToolAccessor.SetImportantItemHelpDesc(itemHelps);
            FF9TextToolAccessor.SetImportantSkinDesc(itemDescs);
        }

        protected override Boolean LoadInternal()
        {
            String[] itemNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.KeyItemNames);
            String[] itemHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.KeyItemHelps);
            String[] itemDescs = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.KeyItemDescriptions);

            FF9TextToolAccessor.SetImportantItemName(itemNames);
            FF9TextToolAccessor.SetImportantItemHelpDesc(itemHelps);
            FF9TextToolAccessor.SetImportantSkinDesc(itemDescs);
            return true;
        }
    }
}