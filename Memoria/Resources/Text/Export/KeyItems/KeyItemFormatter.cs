using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Memoria
{
    public static class KeyItemFormatter
    {
        private const String Separator = "------";
        private static readonly Regex DescriptionFilter = new Regex(@"\[(?!EIKO)[^]]+\]+");

        private static readonly KeyValuePair<String, TextReplacement>[] Tags = new Dictionary<String, TextReplacement>
        {
            {"[A85038][HSHD]", "{b}"},
            {"[383838][HSHD]", "{/b}"},
            {"[383838]", "{Color}"}
        }.ToArray();

        public static TxtEntry[] Build(String prefix, String[] itemNames, String[] itemHelps, String[] itemDescriptions)
        {
            List<TxtEntry> abilities = new List<TxtEntry>(80);
            for (int i = 0; i < itemNames.Length; i++)
            {
                String name = i < itemNames.Length ? itemNames[i] : String.Empty;
                if (String.IsNullOrEmpty(name))
                    continue;

                String help = i < itemHelps.Length ? itemHelps[i] : String.Empty;
                String desc = i < itemDescriptions.Length ? itemDescriptions[i] : String.Empty;
                TxtEntry entry = Build(prefix, name, help, desc);
                entry.Index = i;
                abilities.Add(entry);
            }
            return abilities.ToArray();
        }

        private static TxtEntry Build(String prefix, String itemName, String itemHelp, String itemDescription)
        {
            return new TxtEntry
            {
                Prefix = prefix,
                Value = FormatValue(itemName, itemHelp, itemDescription)
            };
        }

        private static String FormatValue(String itemName, String itemHelp, String itemDescription)
        {
            if (String.IsNullOrEmpty(itemName))
                return String.Empty;

            StringBuilder sb = new StringBuilder(itemName.Length + itemHelp.Length + itemDescription.Length);
            sb.AppendLine(itemName);
            sb.AppendLine(itemHelp.ReplaceAll(Tags));
            sb.AppendLine(Separator);

            itemDescription = itemDescription.ReplaceAll(Tags);
            itemDescription = DescriptionFilter.Replace(itemDescription, String.Empty);

            sb.Append(itemDescription);
            return sb.ToString();
        }
    }
}