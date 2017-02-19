using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Memoria.Prime.Text;

namespace Memoria.Assets
{
    public sealed class ImportFieldTags : FieldTags
    {
        public readonly IList<KeyValuePair<String, TextReplacement>> ComplexTags;

        public ImportFieldTags()
        {
            ComplexTags = GetComplexTags();
        }

        private IList<KeyValuePair<String, TextReplacement>> GetComplexTags()
        {
            return new Dictionary<String, TextReplacement>
            {
                {"{W", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Variable ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Icon ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Icon+ ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Speed ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Text ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Widths ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Time ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Wait ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Center ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Item ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{PreChoose ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{PreChooseMask ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Position ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Offset ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Mobile ", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{y", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{y-", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{f", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{x", (ReplaceTextDelegate)ReplaceComplexTag},
                {"{Table ", (ReplaceTextDelegate)ReplaceComplexTag}
            }.ToArray();
        }

        private static String ReplaceComplexTag(String str, StringBuilder word, ref Int32 index, ref Int32 length)
        {
            Int32 offset = length;
            String incompleteTag;
            if (BattleFormatter.IsIncompleteTag(str, '}', word, ref index, ref length, out incompleteTag))
                return incompleteTag;

            String result;
            switch (word.ToString(0, offset))
            {
                case "{W":
                    result = "[STRT=" + String.Join(",", (word.ToString(offset, length - offset - 1) + ']').Split('H'));
                    return result;
                case "{Variable ":
                    result = "[NUMB=";
                    break;
                case "{Icon ":
                    result = "[ICON=";
                    break;
                case "{Icon+ ":
                    result = "[PNEW=";
                    break;
                case "{Speed ":
                    result = "[SPED=";
                    break;
                case "{Text ":
                    result = "[TEXT=";
                    break;
                case "{Widths ":
                    result = "[WDTH=";
                    break;
                case "{Time ":
                    result = "[TIME=";
                    break;
                case "{Wait ":
                    result = "[WAIT=";
                    break;
                case "{Center ":
                    result = "[CENT=";
                    break;
                case "{Item ":
                    result = "[ITEM=";
                    break;
                case "{PreChoose ":
                    result = "[PCHC=";
                    break;
                case "{PreChooseMask ":
                    result = "[PCHM=";
                    break;
                case "{Position ":
                    result = "[MPOS=";
                    break;
                case "{Offset ":
                    result = "[OFFT=";
                    break;
                case "{Mobile ":
                    result = "[MOBI=";
                    break;
                case "{y":
                    result = "[YADD=";
                    break;
                case "{y-":
                    result = "[YSUB=";
                    break;
                case "{f":
                    result = "[FEED=";
                    break;
                case "{x":
                    result = "[XTAB=";
                    break;
                case "{Table ":
                    result = "[TBLE=";
                    break;
                default:
                    return word.ToString();
            }

            result += word.ToString(offset, length - offset - 1) + ']';
            return result;
        }
    }
}