using System;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria.Assets
{
    public sealed class KeyItemImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(KeyItemImporter);
        protected override String ImportPath => ModTextResources.Import.KeyItems;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] itemNames, itemHelps, itemDescs;
            KeyItemFormatter.Parse(entreis, out itemNames, out itemHelps, out itemDescs);

            FF9TextTool.SetImportantItemName(itemNames);
            FF9TextTool.SetImportantItemHelpDesc(itemHelps);
            FF9TextTool.SetImportantSkinDesc(itemDescs);
        }

        protected override Boolean LoadInternal()
        {
            String[] itemNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.KeyItemNames);
            String[] itemHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.KeyItemHelps);
            String[] itemDescs = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.KeyItemDescriptions);

            FF9TextTool.SetImportantItemName(itemNames);
            FF9TextTool.SetImportantItemHelpDesc(itemHelps);
            FF9TextTool.SetImportantSkinDesc(itemDescs);
            return true;
        }
    }
}