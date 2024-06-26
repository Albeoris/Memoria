using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    public class CharacterNamesImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(CharacterNamesImporter);
        protected override TextResourceReference ImportPath => ModTextResources.Import.CharacterNames;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] characterNamesArray = CharacterNamesFormatter.Parse(entreis);
            Dictionary<CharacterId, String> characterNames = new Dictionary<CharacterId, String>();
            for (Int32 i = 0; i < characterNamesArray.Length; i++)
                characterNames[(CharacterId)i] = characterNamesArray[i];
            FF9TextTool.SetCharacterNames(characterNames);
        }

        protected override Boolean LoadInternal()
        {
            Dictionary<CharacterId, String> characterNames = CharacterNamesFormatter.CharacterDefaultNames();
            FF9TextTool.SetCharacterNames(characterNames);
            return true;
        }
    }
}
