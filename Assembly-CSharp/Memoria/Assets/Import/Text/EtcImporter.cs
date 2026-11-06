using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using Memoria.Data;

namespace Memoria.Assets
{
    public sealed class EtcImporter : SingleFileImporter
    {
        protected override String TypeName { get; }
        protected override TextResourceReference ImportPath { get; }

        private readonly Action<Int32, String> _setter;
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
            FF9TextTool.ImportWithCumulativeModFiles<Int32>(_embadedPath, _setter);
            if (_setter == FF9TextTool.SetFollowText)
            {
                for (Int32 i = 0; i < 8; i++)
                    FF9TextTool.SetElementName((ElementToken)i, FF9TextTool.BattleFollowText(i));
                FF9TextTool.ImportWithCumulativeModFiles<ElementToken>(EmbadedTextResources.ElementNames, FF9TextTool.SetElementName);
            }
            else if (_setter == FF9TextTool.SetLibraText)
            {
                for (Int32 i = 0; i < 8; i++)
                    FF9TextTool.SetEnemyCategoryName((EnemyCategoryToken)i, FF9TextTool.BattleLibraText(i));
                FF9TextTool.ImportWithCumulativeModFiles<EnemyCategoryToken>(EmbadedTextResources.EnemyCategoryNames, FF9TextTool.SetEnemyCategoryName);
            }
            return true;
        }

        protected override void ProcessEntries(TxtEntry[] entreis)
        {
            EtcFormatter.Parse(entreis, out String[] strings);
            FF9TextTool.ImportArrayToDictionary(strings, _setter);
        }
    }
}
