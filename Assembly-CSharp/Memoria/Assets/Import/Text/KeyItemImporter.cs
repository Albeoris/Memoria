using System;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria.Assets
{
    public sealed class KeyItemImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(KeyItemImporter);
        protected override TextResourceReference ImportPath => ModTextResources.Import.KeyItems;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] itemNames, itemHelps, itemDescs;
            KeyItemFormatter.Parse(entreis, out itemNames, out itemHelps, out itemDescs);

            FF9TextTool.ImportArrayToDictionary<Int32>(itemNames, FF9TextTool.SetImportantItemName);
            FF9TextTool.ImportArrayToDictionary<Int32>(itemHelps, FF9TextTool.SetImportantItemHelpDesc);
            FF9TextTool.ImportArrayToDictionary<Int32>(itemDescs, FF9TextTool.SetImportantSkinDesc);
        }

        protected override Boolean LoadInternal()
        {
            FF9TextTool.ImportWithCumulativeModFiles<Int32>(EmbadedTextResources.KeyItemNames, FF9TextTool.SetImportantItemName);
            FF9TextTool.ImportWithCumulativeModFiles<Int32>(EmbadedTextResources.KeyItemHelps, FF9TextTool.SetImportantItemHelpDesc);
            FF9TextTool.ImportWithCumulativeModFiles<Int32>(EmbadedTextResources.KeyItemDescriptions, FF9TextTool.SetImportantSkinDesc);
            return true;
        }
    }
}