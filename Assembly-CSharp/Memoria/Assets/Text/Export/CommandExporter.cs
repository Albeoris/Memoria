using System;

namespace Memoria.Assets
{
    public sealed class CommandExporter : SingleFileExporter
    {
        private const String Prefix = "$command";

        protected override String TypeName => nameof(CommandExporter);
        protected override TextResourcePath ExportPath => ModTextResources.Export.Commands;

        protected override TxtEntry[] PrepareEntries()
        {
            String[] commandNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.CommandNames);
            String[] commandHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.CommandHelps);

            return AbilityFormatter.Build(Prefix, commandNames, commandHelps);
        }
    }
}
