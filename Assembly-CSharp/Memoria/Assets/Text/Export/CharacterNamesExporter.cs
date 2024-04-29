using System;
using System.Collections.Generic;
using Memoria.Data;

namespace Memoria.Assets
{
    public sealed class CharacterNamesExporter : SingleFileExporter
    {
        private const String Prefix = "$name";

        protected override String TypeName => nameof(CharacterNamesExporter);
        protected override TextResourcePath ExportPath => ModTextResources.Export.CharacterNames;

        protected override TxtEntry[] PrepareEntries()
        {
            Dictionary<CharacterId, String> characterNames = CharacterNamesFormatter.CharacterDefaultNames();
            return CharacterNamesFormatter.Build(Prefix, characterNames);
        }
    }
}