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
            for (Int32 i = 0; i < characterNamesArray.Length; i++)
            {
                foreach (PLAYER player in FF9StateSystem.Common.FF9.player.Values)
                {
                    // Note: "CharacterNames.strings" sort its entries according to preset IDs
                    if (player.PresetId == (CharacterPresetId)i)
                    {
                        FF9TextTool.ChangeCharacterName(player.Index, characterNamesArray[i]);
                        break;
                    }
                }
            }
        }

        protected override Boolean LoadInternal()
        {
            Dictionary<CharacterId, String> characterNames = CharacterNamesFormatter.CharacterDefaultNames();
            FF9TextTool.SetCharacterNames(characterNames);
            return true;
        }
    }
}
