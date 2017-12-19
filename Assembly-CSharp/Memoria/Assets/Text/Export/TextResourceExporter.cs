using System;
using System.Collections.Generic;
using Memoria.Prime;

namespace Memoria.Assets
{
    public static class TextResourceExporter
    {
        public static void ExportSafe()
        {
            try
            {
                if (!Configuration.Export.Text)
                {
                    Log.Message("[TextResourceExporter] Pass through {Configuration.Export.Text = 0}.");
                    return;
                }

                CreditsExporter credits = new CreditsExporter();

                foreach (String symbol in Configuration.Export.Languages)
                {
                    EmbadedTextResources.CurrentSymbol = symbol;
                    ModTextResources.Export.CurrentSymbol = symbol;

                    credits.Export();

                    foreach (IExporter exporter in EnumerateExporters())
                        exporter.Export();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to export text resources.");
            }
            finally
            {
                EmbadedTextResources.CurrentSymbol = null;
                ModTextResources.Export.CurrentSymbol = null;
            }
        }

        private static IEnumerable<IExporter> EnumerateExporters()
        {
            yield return new LocalizationExporter();

            foreach (EtcTextResource value in Enum.GetValues(typeof(EtcTextResource)))
                yield return new EtcExporter(value);

            yield return new CommandExporter();
            yield return new AbilityExporter();
            yield return new SkillExporter();
            yield return new ItemExporter();
            yield return new KeyItemExporter();
            yield return new BattleExporter();
            yield return new LocationNameExporter();
            yield return new FieldExporter();
            yield return new CharacterNamesExporter();
        }
    }

    public interface IExporter
    {
        void Export();
    }
}