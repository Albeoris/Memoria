using System;

namespace Memoria.Assets
{
    public sealed class EtcExporter : SingleFileExporter
    {
        protected override String TypeName { get; }
        protected override TextResourcePath ExportPath { get; }

        private readonly String _prefix;
        private readonly String _embadedPath;

        public EtcExporter(EtcTextResource resource)
        {
            switch (resource)
            {
                case EtcTextResource.BattleCommands:
                    _prefix = "$btlcommand";
                    _embadedPath = EmbadedTextResources.BattleCommands;
                    ExportPath = ModTextResources.Export.BattleCommands;
                    break;
                case EtcTextResource.BattleMessages:
                    _prefix = "$btlmessage";
                    _embadedPath = EmbadedTextResources.BattleMessages;
                    ExportPath = ModTextResources.Export.BattleMessages;
                    break;
                case EtcTextResource.CardLevels:
                    _prefix = "$cardlevel";
                    _embadedPath = EmbadedTextResources.CardLevels;
                    ExportPath = ModTextResources.Export.CardLevels;
                    break;
                case EtcTextResource.CardTitles:
                    _prefix = "$cardtitle";
                    _embadedPath = EmbadedTextResources.CardTitles;
                    ExportPath = ModTextResources.Export.CardTitles;
                    break;
                case EtcTextResource.Chocobo:
                    _prefix = "$chocobo";
                    _embadedPath = EmbadedTextResources.Chocobo;
                    ExportPath = ModTextResources.Export.Chocobo;
                    break;
                case EtcTextResource.Libra:
                    _prefix = "$libra";
                    _embadedPath = EmbadedTextResources.Libra;
                    ExportPath = ModTextResources.Export.Libra;
                    break;
                case EtcTextResource.WorldLocations:
                    _prefix = "$world";
                    _embadedPath = EmbadedTextResources.WorldLocations;
                    ExportPath = ModTextResources.Export.WorldLocations;
                    break;
                default:
                    throw new NotImplementedException(resource.ToString());
            }

            TypeName = $"{nameof(EtcExporter)}({resource})";
        }

        protected override TxtEntry[] PrepareEntries()
        {
            String[] itemNames = EmbadedSentenseLoader.LoadSentense(_embadedPath);
            return EtcFormatter.Build(_prefix, itemNames);
        }
    }
}