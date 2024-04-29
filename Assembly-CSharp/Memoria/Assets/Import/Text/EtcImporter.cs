using System;
using System.Collections.Generic;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria.Assets
{
    public sealed class EtcImporter : SingleFileImporter
    {
        protected override String TypeName { get; }
        protected override TextResourceReference ImportPath { get; }

        private readonly Action<String[]> _setter;
        private readonly String _embadedPath;

        public static IEnumerable<EtcImporter> EnumerateImporters()
        {
            foreach (EtcTextResource value in Enum.GetValues(typeof(EtcTextResource)))
                yield return new EtcImporter(value);
        }

        public EtcImporter(EtcTextResource resource)
        {
            switch (resource)
            {
                case EtcTextResource.BattleCommands:
                    _setter = FF9TextTool.SetCmdTitleText;
                    _embadedPath = EmbadedTextResources.BattleCommands;
                    ImportPath = ModTextResources.Import.BattleCommands;
                    break;
                case EtcTextResource.BattleMessages:
                    _setter = FF9TextTool.SetFollowText;
                    _embadedPath = EmbadedTextResources.BattleMessages;
                    ImportPath = ModTextResources.Import.BattleMessages;
                    break;
                case EtcTextResource.CardLevels:
                    _setter = FF9TextTool.SetCardLvName;
                    _embadedPath = EmbadedTextResources.CardLevels;
                    ImportPath = ModTextResources.Import.CardLevels;
                    break;
                case EtcTextResource.CardTitles:
                    _setter = FF9TextTool.SetCardName;
                    _embadedPath = EmbadedTextResources.CardTitles;
                    ImportPath = ModTextResources.Import.CardTitles;
                    break;
                case EtcTextResource.Chocobo:
                    _setter = FF9TextTool.SetChocoUiText;
                    _embadedPath = EmbadedTextResources.Chocobo;
                    ImportPath = ModTextResources.Import.Chocobo;
                    break;
                case EtcTextResource.Libra:
                    _setter = FF9TextTool.SetLibraText;
                    _embadedPath = EmbadedTextResources.Libra;
                    ImportPath = ModTextResources.Import.Libra;
                    break;
                case EtcTextResource.WorldLocations:
                    _setter = FF9TextTool.SetWorldLocationText;
                    _embadedPath = EmbadedTextResources.WorldLocations;
                    ImportPath = ModTextResources.Import.WorldLocations;
                    break;
                default:
                    throw new NotImplementedException(resource.ToString());
            }

            TypeName = $"{nameof(EtcImporter)}({resource})";
        }

        protected override Boolean LoadInternal()
        {
            String[] strings = EmbadedSentenseLoader.LoadSentense(_embadedPath);

            _setter(strings);
            return true;
        }

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            String[] strings;
            EtcFormatter.Parse(entreis, out strings);

            _setter(strings);
        }
    }
}