using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using System;

namespace Memoria.Assets
{
    public class CommandImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(CommandImporter);
        protected override TextResourceReference ImportPath => ModTextResources.Import.Commands;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] commandNames, commandHelps;
            AbilityFormatter.Parse(entreis, out commandNames, out commandHelps);

            FF9TextTool.ImportArrayToDictionary<BattleCommandId>(commandNames, FF9TextTool.SetCommandName);
            FF9TextTool.ImportArrayToDictionary<BattleCommandId>(commandHelps, FF9TextTool.SetCommandHelpDesc);
        }

        protected override Boolean LoadInternal()
        {
            FF9TextTool.ImportWithCumulativeModFiles<BattleCommandId>(EmbadedTextResources.CommandNames, FF9TextTool.SetCommandName);
            FF9TextTool.ImportWithCumulativeModFiles<BattleCommandId>(EmbadedTextResources.CommandHelps, FF9TextTool.SetCommandHelpDesc);
            return true;
        }
    }
}
