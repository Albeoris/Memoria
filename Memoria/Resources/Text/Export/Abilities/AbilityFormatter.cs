using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Memoria
{
    public static class AbilityFormatter
    {
        private static readonly KeyValuePair<String, TextReplacement>[] Tags = new Dictionary<String, TextReplacement>
        {
            {"[A85038][HSHD]", "{b}"},
            {"[383838][HSHD]", "{/b}"}
        }.ToArray();

        public static TxtEntry[] Build(String prefix, String[] abilityNames, String[] abilityHelps)
        {
            TxtEntry[] abilities = new TxtEntry[Math.Max(abilityNames.Length, abilityHelps.Length)];
            for (int i = 0; i < abilities.Length; i++)
            {
                String name = i < abilityNames.Length ? abilityNames[i] : String.Empty;
                String help = i < abilityHelps.Length ? abilityHelps[i] : String.Empty;
                abilities[i] = Build(prefix, name, help);
            }
            return abilities;
        }

        private static TxtEntry Build(String prefix, String abilityName, String abilityHelp)
        {
            return new TxtEntry
            {
                Prefix = prefix,
                Value = FormatValue(abilityName, abilityHelp)
            };
        }

        private static String FormatValue(String abilityName, String abilityHelp)
        {
            StringBuilder sb = new StringBuilder(abilityName.Length + abilityHelp.Length);
            sb.AppendLine(abilityName);
            sb.Append(abilityHelp.ReplaceAll(Tags));
            return sb.ToString();
        }
    }
}