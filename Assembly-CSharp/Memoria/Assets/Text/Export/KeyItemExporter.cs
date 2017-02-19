using System;

namespace Memoria.Assets
{
    public sealed class KeyItemExporter : SingleFileExporter
    {
        private const String Prefix = "$key";

        protected override String TypeName => nameof(KeyItemExporter);
        protected override String ExportPath => ModTextResources.Export.KeyItems;

        protected override TxtEntry[] PrepareEntries()
        {
            String[] itemNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.KeyItemNames);
            String[] itemHelps = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.KeyItemHelps);
            String[] itemDescs = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.KeyItemDescriptions);

            return KeyItemFormatter.Build(Prefix, itemNames, itemHelps, itemDescs);
        }
    }
}