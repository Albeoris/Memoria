using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Memoria
{
    public static class ItemFormatter
    {
        private const String Separator = "------";

        private static readonly KeyValuePair<String, TextReplacement>[] Tags = new Dictionary<String, TextReplacement>
        {
            {"[A85038][HSHD]", "{b}"},
            {"[383838][HSHD]", "{/b}"}
        }.ToArray();

        public static TxtEntry[] Build(String prefix, String[] itemNames, String[] itemHelps, String[] itemBattle)
        {
            TxtEntry[] abilities = new TxtEntry[Math.Max(itemNames.Length, itemHelps.Length)];
            for (int i = 0; i < abilities.Length; i++)
            {
                String name = i < itemNames.Length ? itemNames[i] : String.Empty;
                String help = i < itemHelps.Length ? itemHelps[i] : String.Empty;
                String bttl = i < itemBattle.Length ? itemBattle[i] : String.Empty;
                abilities[i] = Build(prefix, name, help, bttl);
            }
            return abilities;
        }

        private static TxtEntry Build(String prefix, String itemName, String itemHelp, String itemBattle)
        {
            return new TxtEntry
            {
                Prefix = prefix,
                Value = FormatValue(itemName, itemHelp, itemBattle)
            };
        }

        private static string FormatValue(String itemName, String itemHelp, String itemBattle)
        {
            StringBuilder sb = new StringBuilder(itemName.Length + itemHelp.Length + itemBattle.Length + 8);
            sb.AppendLine(itemName);
            sb.AppendLine(itemHelp.ReplaceAll(Tags));
            sb.AppendLine(Separator);
            sb.Append(itemBattle.ReplaceAll(Tags));
            return sb.ToString();
        }
    }
}