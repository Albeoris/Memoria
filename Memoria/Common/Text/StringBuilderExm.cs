using System;
using System.Text;

namespace Memoria
{
    public static class StringBuilderExm
    {
        public static StringBuilder AppendFormatLine(this StringBuilder self, string format, params object[] args)
        {
            Exceptions.CheckArgumentNull(self, "self");

            self.AppendFormat(format, args);
            self.AppendLine();

            return self;
        }

        public static Int32 IndexOf(this StringBuilder self, Int32 offset, Char ch)
        {
            for (int i = offset; i < self.Length; i++)
            {
                if (self[i] == ch)
                    return i;
            }

            return -1;
        }

        public static Int32 IndexFromEnd(this StringBuilder self, Int32 offset, Char ch)
        {
            for (int i = offset; i >= 0; i--)
            {
                if (self[i] == ch)
                    return i;
            }

            return -1;
        }
    }
}