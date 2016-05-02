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

        private static readonly TextReplacements Replacements = new TextReplacements
        {
            {"[A85038][HSHD]", "{b}"},
            {"[383838][HSHD]", "{/b}"},
            {"[383838]", "{Color}"}
        }.Commit();

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
            sb.AppendLine(itemHelp.ReplaceAll(Replacements.Forward));
            sb.AppendLine(Separator);

            itemDescription = itemDescription.ReplaceAll(Replacements.Forward);
            itemDescription = DescriptionFilter.Replace(itemDescription, String.Empty);

            sb.Append(itemDescription);
            return sb.ToString();
        }

        public static void Parse(TxtEntry[] entreis, out String[] itemNames, out String[] itemHelps, out String[] itemDescs)
        {
            itemNames = new String[entreis.Length];
            itemHelps = new String[entreis.Length];
            itemDescs = new String[entreis.Length];

            for (int i = 0; i < entreis.Length; i++)
            {
                String value = entreis[i].Value;
                Int32 newLineIndex = value.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                Int32 helpIndex = newLineIndex + Environment.NewLine.Length;
                if (newLineIndex < 0 || helpIndex >= value.Length)
                {
                    itemNames[i] = value;
                    itemHelps[i] = String.Empty;
                    itemDescs[i] = String.Empty;
                    continue;
                }

                if (newLineIndex == 0)
                    itemNames[i] = String.Empty;
                else
                    itemNames[i] = value.Substring(0, newLineIndex);

                Int32 separatorIndex = value.IndexOf(Separator, helpIndex, StringComparison.Ordinal) - Environment.NewLine.Length;
                if (separatorIndex < 0)
                {
                    itemHelps[i] = value.Substring(helpIndex).ReplaceAll(Replacements.Backward);
                    itemDescs[i] = String.Empty;
                    continue;
                }

                if (separatorIndex == 0)
                    itemHelps[i] = String.Empty;
                else
                    itemHelps[i] = value.Substring(helpIndex, separatorIndex - helpIndex).ReplaceAll(Replacements.Backward);

                Int32 itemDesc = separatorIndex + Separator.Length + Environment.NewLine.Length * 2;
                if (itemDesc >= value.Length)
                    itemDescs[i] = String.Empty;
                else
                {
                    itemDescs[i] = value.Substring(itemDesc).ReplaceAll(Replacements.Backward); // TODO: bring back tags
                }
            }
        }
    }
}