using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace Memoria
{
    public class JsonEncoder
    {
        public static String Encode(String value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return String.Empty;
            }

            StringBuilder b = null;
            int startIndex = 0;
            int count = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if (CharRequiresJavaScriptEncoding(c))
                {
                    if (b == null)
                    {
                        b = new StringBuilder(value.Length + 5);
                    }

                    if (count > 0)
                    {
                        b.Append(value, startIndex, count);
                    }

                    startIndex = i + 1;
                    count = 0;

                    switch (c)
                    {
                        case '\r':
                            b.Append("\\r");
                            break;
                        case '\t':
                            b.Append("\\t");
                            break;
                        case '\"':
                            b.Append("\\\"");
                            break;
                        case '\\':
                            b.Append("\\\\");
                            break;
                        case '\n':
                            b.Append("\\n");
                            break;
                        case '\b':
                            b.Append("\\b");
                            break;
                        case '\f':
                            b.Append("\\f");
                            break;
                        default:
                            AppendCharAsUnicodeJavaScript(b, c);
                            break;
                    }
                }
                else
                {
                    count++;
                }
            }

            if (b == null)
                return value;

            if (count > 0)
                b.Append(value, startIndex, count);

            return b.ToString();
        }

        private static void AppendCharAsUnicodeJavaScript(StringBuilder builder, char c)
        {
            builder.Append("\\u");
            builder.Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
        }

        private static bool CharRequiresJavaScriptEncoding(char c)
        {
            return c < 0x20 // control chars always have to be encoded
                   || c == '\"' // chars which must be encoded per JSON spec
                   || c == '\\'
                   || c == '\'' // HTML-sensitive chars encoded for safety
                   || c == '<'
                   || c == '>'
                   || c == '\u0085' // newline chars (see Unicode 6.2, Table 5-1 [http://www.unicode.org/versions/Unicode6.2.0/ch05.pdf]) have to be encoded (DevDiv #663531)
                   || c == '\u2028'
                   || c == '\u2029';
        }
    }
}