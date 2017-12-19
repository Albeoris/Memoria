using System;

namespace Memoria.Assets
{
    public sealed class CharacterNamesExporter : SingleFileExporter
    {
        private const String Prefix = "$name";

        protected override String TypeName => nameof(CharacterNamesExporter);
        protected override String ExportPath => ModTextResources.Export.CharacterNames;

        protected override TxtEntry[] PrepareEntries()
        {
            String[] characterNames = CharacterNamesFormatter.CharacterDefaultNames();
            return CharacterNamesFormatter.Build(Prefix, characterNames);
        }
    }
}