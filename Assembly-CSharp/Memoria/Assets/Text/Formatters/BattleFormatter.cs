using Memoria.Prime.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Memoria.Assets
{
    public static class BattleFormatter
    {
        private static readonly Regex TagsFilter = new Regex(@"\[(?!NUMB)[^]]+\]+");
        private static readonly Regex TagsSpaceFilter = new Regex(@"\s\s+");

        private static readonly KeyValuePair<String, TextReplacement>[] KeyTags = new Dictionary<String, TextReplacement>
        {
            {"[ZDNE]", "{Zidane}"},
            {"[VIVI]", "{Vivi}"},
            {"[DGGR]", "{Dagger}"},
            {"[STNR]", "{Steiner}"},
            {"[FRYA]", "{Freya}"},
            {"[QUIN]", "{Quina}"},
            {"[EIKO]", "{Eiko}"},
            {"[AMRT]", "{Amarant}"}
        }.ToArray();

        private static readonly TextReplacements ValueTextTags = new TextReplacements
        {
            {"[ZDNE]", "{Zidane}"},
            {"[VIVI]", "{Vivi}"},
            {"[DGGR]", "{Dagger}"},
            {"[STNR]", "{Steiner}"},
            {"[FRYA]", "{Freya}"},
            {"[QUIN]", "{Quina}"},
            {"[EIKO]", "{Eiko}"},
            {"[AMRT]", "{Amarant}"},
            {"[FLIM]", "{Flash}"},
            {"[MOVE=18,0]", "{Tab}"},
            {"[C8C8C8][HSHD]", "{White}"},
            {"[B880E0][HSHD]", "{Pink}"},
            {"[68C0D8][HSHD]", "{Cyan}"},
            {"[D06050][HSHD]", "{Brown}"},
            {"[TAIL=LOR]", "{LowerRight}"},
            {"[TAIL=LOL]", "{LowerLeft}"},
            {"[TAIL=UPR]", "{UpperRight}"},
            {"[TAIL=UPL]", "{UpperLeft}"},
            {"[TAIL=LOC]", "{LowerCenter}"},
            {"[TAIL=UPC]", "{UpperCenter}"},
            {"[TAIL=LORF]", "{LowerRightForce}"},
            {"[TAIL=LOLF]", "{LowerLeftForce}"},
            {"[TAIL=UPRF]", "{UpperRightForce}"},
            {"[TAIL=UPLF]", "{UpperLeftForce}"},
            {"[TAIL=DEFT]", "{DialogPosition}"}
        }.Commit();

        private static readonly KeyValuePair<String, TextReplacement>[] ValueSmartTagsForward = new Dictionary<String, TextReplacement>
        {
            {"[STRT=", (ReplaceTextDelegate)ConvertStartSentenseTag},
            {"[NUMB=", (ReplaceTextDelegate)ConvertNumberVariableTag},
            {"[SPED=", (ReplaceTextDelegate)ConvertMessageSpeedTag}
        }.ToArray();

        private static readonly KeyValuePair<String, TextReplacement>[] ValueSmartTagsBackward = new Dictionary<String, TextReplacement>
        {
            {"{Width ", (ReplaceTextDelegate)ReturnStartSentenseTag},
            {"{Variable ", (ReplaceTextDelegate)ReturnNumberVariableTag},
            {"{Speed ", (ReplaceTextDelegate)ReturnMessageSpeedTag}
        }.ToArray();

        private static String ConvertStartSentenseTag(String str, StringBuilder word, ref Int32 index, ref Int32 length)
        {
            Int32 offset = length;
            String incompleteTag;
            if (IsIncompleteTag(str, ']', word, ref index, ref length, out incompleteTag))
                return incompleteTag;

            Int32 comaIndex = word.IndexOf(offset, ',');

            String widthStr = word.ToString(offset, comaIndex - offset);
            String linesStr = word.ToString(comaIndex + 1, length - comaIndex - 2);
            StringBuilder sb = new StringBuilder(32);
            sb.Append("{Width ");
            sb.Append(widthStr);
            sb.Append("}");

            if (linesStr != "1")
            {
                sb.Append("{Lines ");
                sb.Append(linesStr);
                sb.Append("}");
            }

            return sb.ToString();
        }

        private static String ConvertNumberVariableTag(String str, StringBuilder word, ref Int32 index, ref Int32 length)
        {
            Int32 offset = length;
            String incompleteTag;
            if (IsIncompleteTag(str, ']', word, ref index, ref length, out incompleteTag))
                return incompleteTag;

            String numberStr = word.ToString(offset, length - 1 - offset);
            StringBuilder sb = new StringBuilder(32);
            sb.Append("{Variable ");
            sb.Append(numberStr);
            sb.Append("}");

            return sb.ToString();
        }

        private static String ConvertMessageSpeedTag(String str, StringBuilder word, ref Int32 index, ref Int32 length)
        {
            Int32 offset = length;
            String incompleteTag;
            if (IsIncompleteTag(str, ']', word, ref index, ref length, out incompleteTag))
                return incompleteTag;

            String speedStr = word.ToString(offset, length - 1 - offset);
            StringBuilder sb = new StringBuilder(32);
            sb.Append("{Speed ");
            sb.Append(speedStr);
            sb.Append("}");

            return sb.ToString();
        }

        public static String GetKey(String line)
        {
            String key = line.ReplaceAll(KeyTags);
            key = TagsFilter.Replace(key, String.Empty);
            key = TagsSpaceFilter.Replace(key, " ");
            return key;
        }

        public static String GetValue(String line)
        {
            return line.ReplaceAll(ValueTextTags.Forward, ValueSmartTagsForward);
        }

        public static void Parse(TxtEntry[] entries, out Dictionary<String, String> dic)
        {
            dic = new Dictionary<String, String>(entries.Length);
            foreach (TxtEntry entry in entries)
            {
                String value = ParseValue(entry.Value);
                dic.Add(entry.Prefix, value);
            }
        }

        private static String ParseValue(String line)
        {
            return line.ReplaceAll(ValueTextTags.Backward, ValueSmartTagsBackward);
        }

        private static String ReturnStartSentenseTag(String str, StringBuilder word, ref Int32 index, ref Int32 length)
        {
            Int32 offset = length;
            String incompleteTag;
            if (IsIncompleteTag(str, '}', word, ref index, ref length, out incompleteTag))
                return incompleteTag;

            Int32 bracetIndex = word.IndexOf(offset, '}');

            String widthStr = word.ToString(offset, bracetIndex - offset);
            StringBuilder sb = new StringBuilder(12);
            sb.Append("[STRT=");
            sb.Append(widthStr);
            sb.Append(',');

            if (TryGetNextTag(str, "{Lines ", word, ref index, ref length))
            {
                Int32 spaceIndex = word.IndexFromEnd(length - 1, ' ');
                String lineStr = word.ToString(spaceIndex + 1, length - spaceIndex - 2);
                sb.Append(lineStr);
            }
            else
            {
                sb.Append('1');
            }

            sb.Append("]");

            return sb.ToString();
        }

        private static String ReturnNumberVariableTag(String str, StringBuilder word, ref Int32 index, ref Int32 length)
        {
            Int32 offset = length;
            String incompleteTag;
            if (IsIncompleteTag(str, '}', word, ref index, ref length, out incompleteTag))
                return incompleteTag;

            String numberStr = word.ToString(offset, length - 1 - offset);
            StringBuilder sb = new StringBuilder(8);
            sb.Append("[NUMB=");
            sb.Append(numberStr);
            sb.Append("]");

            return sb.ToString();
        }

        private static String ReturnMessageSpeedTag(String str, StringBuilder word, ref Int32 index, ref Int32 length)
        {
            Int32 offset = length;
            String incompleteTag;
            if (IsIncompleteTag(str, '}', word, ref index, ref length, out incompleteTag))
                return incompleteTag;

            String speedStr = word.ToString(offset, length - 1 - offset);
            StringBuilder sb = new StringBuilder(8);
            sb.Append("[SPED=");
            sb.Append(speedStr);
            sb.Append("]");

            return sb.ToString();
        }

        public static Boolean IsIncompleteTag(String str, Char ending, StringBuilder word, ref Int32 index, ref Int32 length, out String incompleteTag)
        {
            Int32 bracetIndex = word.IndexOf(length, ending);
            if (bracetIndex < 0)
            {
                for (Int32 i = index + 1; i < str.Length; i++)
                {
                    Char ch = str[i];
                    word.Append(ch);
                    index++;
                    if (ch == ending)
                    {
                        bracetIndex = word.Length - 1;
                        length = word.Length;
                        break;
                    }
                }
            }

            if (bracetIndex < 0)
            {
                String result = word.ToString(0, length);
                incompleteTag = result;
                return true;
            }

            length = bracetIndex + 1;
            incompleteTag = null;
            return false;
        }

        private static Boolean TryGetNextTag(String str, String starting, StringBuilder word, ref Int32 index, ref Int32 length)
        {
            Int32 startingIndex = 0;
            Int32 currentInddex = length;
            while (startingIndex < starting.Length && currentInddex < word.Length)
            {
                if (word[currentInddex++] != starting[startingIndex++])
                    return false;
            }

            Int32 diff = starting.Length - (word.Length - length);
            if (diff > 0)
            {
                if (index + 1 + diff > str.Length)
                    return false;

                for (Int32 i = index + 1; diff > 0 && i < str.Length; i++)
                {
                    Char ch = str[i];
                    word.Append(ch);
                    index++;
                }

                while (startingIndex < starting.Length && currentInddex < word.Length)
                {
                    if (word[currentInddex++] != starting[startingIndex++])
                        return false;
                }
            }

            Int32 bracetIndex = word.IndexOf(length, '}');
            if (bracetIndex < 0)
            {
                for (Int32 i = index + 1; i < str.Length; i++)
                {
                    Char ch = str[i];
                    word.Append(ch);
                    index++;
                    if (ch == '}')
                    {
                        bracetIndex = word.Length - 1;
                        length = word.Length;
                        break;
                    }
                }
            }

            if (bracetIndex < 0)
                return false;

            length = bracetIndex + 1;
            return true;
        }
    }
}
