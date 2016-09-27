using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Memoria
{
    public sealed class TextReplacements : IEnumerable<String>
    {
        private readonly List<KeyValuePair<String, TextReplacement>> _forward ;
        private readonly List<KeyValuePair<String, TextReplacement>> _backward;

        public TextReplacements()
        {
            _forward = new List<KeyValuePair<String, TextReplacement>>(2);
            _backward = new List<KeyValuePair<String, TextReplacement>>(2);
        }

        public TextReplacements(Int32 capacity)
        {
            _forward = new List<KeyValuePair<String, TextReplacement>>(capacity);
            _backward = new List<KeyValuePair<String, TextReplacement>>(capacity);
        }

        public void Add(String from, String to)
        {
            _forward.Add(new KeyValuePair<String, TextReplacement>(from, to));
            _backward.Add(new KeyValuePair<String, TextReplacement>(to, from));
        }

        public void AddForward(String from, String to)
        {
            _forward.Add(new KeyValuePair<String, TextReplacement>(from, to));
        }

        public void AddBackward(String from, String to)
        {
            _backward.Add(new KeyValuePair<String, TextReplacement>(from, to));
        }

        public TextReplacements Commit()
        {
            _forward.TrimExcess();
            _backward.TrimExcess();
            return this;
        }

        public IList<KeyValuePair<String, TextReplacement>> Forward => _forward;
        public IList<KeyValuePair<String, TextReplacement>> Backward => _backward;

        #region IEnumerable<String>

        public IEnumerator<String> GetEnumerator()
        {
            return _forward.Select(f => f.Key).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    public static class AbilityFormatter
    {
        private static readonly TextReplacements Replacements = new TextReplacements(2)
        {
            {"[A85038][HSHD]", "{b}"},
            {"[383838][HSHD]", "{/b}"}
        }.Commit();

        public static TxtEntry[] Build(String prefix, String[] abilityNames, String[] abilityHelps)
        {
            TxtEntry[] abilities = new TxtEntry[Math.Max(abilityNames.Length, abilityHelps.Length)];
            for (Int32 i = 0; i < abilities.Length; i++)
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
            sb.Append(abilityHelp.ReplaceAll(Replacements.Forward));
            return sb.ToString();
        }

        public static void Parse(TxtEntry[] entreis, out String[] skillNames, out String[] skillHelps)
        {
            skillNames = new String[entreis.Length];
            skillHelps = new String[entreis.Length];

            for (Int32 i = 0; i < entreis.Length; i++)
            {
                String value = entreis[i].Value;
                Int32 helpIndex = value.IndexOf('\n');
                if (helpIndex < 0)
                {
                    skillNames[i] = value;
                    skillHelps[i] = String.Empty;
                }
                else if (helpIndex == 0)
                {
                    skillNames[i] = String.Empty;
                    skillHelps[i] = value;
                }
                else
                {
                    skillNames[i] = value.Substring(0, helpIndex);
                    helpIndex += 1;
                    skillHelps[i] = helpIndex < value.Length
                        ? value.Substring(helpIndex).ReplaceAll(Replacements.Backward)
                        : String.Empty;
                }
            }
        }
    }
}