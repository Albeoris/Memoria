using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Memoria
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
            {"[FRYA]", "{Fraya}"},
            {"[QUIN]", "{Quina}"},
            {"[EIKO]", "{Eiko}"},
            {"[AMRT]", "{Amarant}"}
        }.ToArray();

        private static readonly KeyValuePair<String, TextReplacement>[] ValueTags = new Dictionary<String, TextReplacement>
        {
            {"[ZDNE]", "{Zidane}"},
            {"[VIVI]", "{Vivi}"},
            {"[DGGR]", "{Dagger}"},
            {"[STNR]", "{Steiner}"},
            {"[FRYA]", "{Fraya}"},
            {"[QUIN]", "{Quina}"},
            {"[EIKO]", "{Eiko}"},
            {"[AMRT]", "{Amarant}"},
            {"[FLIM]", "{Flash}"},
            {"[MOVE=18,0]", "{Tab}"},
            {"[STRT=", (ReplaceTextDelegate)ConvertStartSentenseTag},
            {"[NUMB=", (ReplaceTextDelegate)ConvertNumberVariableTag},
            {"[SPED=", (ReplaceTextDelegate)ConvertMessageSpeedTag},
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
        }.ToArray();

        private static String ConvertStartSentenseTag(String str, StringBuilder word, ref Int32 index, ref Int32 length)
        {
            Int32 offset = length;
            String incompleteTag;
            if (TryGetResultFromIncompleteTag(str, word, ref index, ref length, out incompleteTag))
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
            if (TryGetResultFromIncompleteTag(str, word, ref index, ref length, out incompleteTag))
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
            if (TryGetResultFromIncompleteTag(str, word, ref index, ref length, out incompleteTag))
                return incompleteTag;

            String speedStr = word.ToString(offset, length - 1 - offset);
            StringBuilder sb = new StringBuilder(32);
            sb.Append("{Speed ");
            sb.Append(speedStr);
            sb.Append("}");

            return sb.ToString();
        }

        private static Boolean TryGetResultFromIncompleteTag(String str, StringBuilder word, ref Int32 index, ref Int32 length, out String incompleteTag)
        {
            Int32 bracetIndex = word.IndexOf(length, ']');
            if (bracetIndex < 0)
            {
                for (Int32 i = index + 1; i < str.Length; i++)
                {
                    Char ch = str[i];
                    word.Append(ch);
                    if (ch == ']')
                    {
                        bracetIndex = word.Length - 1;
                        index = i;
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

        internal static String GetKey(String line)
        {
            String key = line.ReplaceAll(KeyTags);
            key = TagsFilter.Replace(key, String.Empty);
            key = TagsSpaceFilter.Replace(key, " ");
            return key;
        }

        public static String GetValue(String line)
        {
            return line.ReplaceAll(ValueTags);
        }
    }
}