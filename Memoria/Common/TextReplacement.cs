using System;
using System.Text;

namespace Memoria
{
    public sealed class TextReplacement
    {
        private readonly String _replacement;

        public readonly ReplaceTextDelegate Replace;

        private TextReplacement(string replacement)
        {
            _replacement = replacement;
            Replace = GetReplacement;
        }

        private TextReplacement(ReplaceTextDelegate replacement)
        {
            Replace = replacement;
        }

        private String GetReplacement(String str, StringBuilder word, ref int index, ref int length)
        {
            return _replacement;
        }

        public static implicit operator TextReplacement(String replacement)
        {
            return new TextReplacement(replacement);
        }

        public static implicit operator TextReplacement(ReplaceTextDelegate replacement)
        {
            return new TextReplacement(replacement);
        }
    }
}