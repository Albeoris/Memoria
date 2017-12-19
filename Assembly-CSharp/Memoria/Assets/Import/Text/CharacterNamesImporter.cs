using System;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria.Assets
{
    public class CharacterNamesImporter : SingleFileImporter
    {
        protected override String TypeName => nameof(CharacterNamesImporter);
        protected override String ImportPath => ModTextResources.Import.CharacterNames;

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] characterNames = CharacterNamesFormatter.Parse(entreis);
            FF9TextTool.SetCharacterNames(characterNames);
        }

        protected override Boolean LoadInternal()
        {
            String[] characterNames = CharacterNamesFormatter.CharacterDefaultNames();
            FF9TextTool.SetCharacterNames(characterNames);
            return true;
        }
    }
}