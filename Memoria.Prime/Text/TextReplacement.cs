using System;
using System.Text;

namespace Memoria.Prime.Text
{
    public sealed class TextReplacement
    {
        private readonly String _replacement;

        public readonly ReplaceTextDelegate Replace;

        private TextReplacement(String replacement)
        {
            _replacement = replacement;
            Replace = GetReplacement;
        }

        private TextReplacement(ReplaceTextDelegate replacement)
        {
            Replace = replacement;
        }

        private String GetReplacement(String str, StringBuilder word, ref Int32 index, ref Int32 length)
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
