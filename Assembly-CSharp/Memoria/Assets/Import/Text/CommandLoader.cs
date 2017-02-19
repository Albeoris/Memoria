using System;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria.Assets
{
    public class CommandImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(CommandImporter);
        protected override String ImportPath => ModTextResources.Import.Commands;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] commandNames, commandHelps;
            AbilityFormatter.Parse(entreis, out commandNames, out commandHelps);

            FF9TextTool.SetCommandName(commandNames);
            FF9TextTool.SetCommandHelpDesc(commandHelps);
        }

        protected override Boolean LoadInternal()
        {
            String[] commandNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.CommandNames);
            String[] commandHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.CommandHelps);

            FF9TextTool.SetCommandName(commandNames);
            FF9TextTool.SetCommandHelpDesc(commandHelps);
            return true;
        }
    }
}