using System;
using System.Text;

namespace Memoria.Prime.Text
{
    public static class ExtensionMethodsStringBuilder
    {
        public static void Clear(this StringBuilder self)
        {
            self.Length = 0;
        }
        
        public static StringBuilder AppendFormatLine(this StringBuilder self, String format, params Object[] args)
        {
            Exceptions.Exceptions.CheckArgumentNull(self, "self");

            self.AppendFormat(format, args);
            self.AppendLine();

            return self;
        }

        public static Int32 IndexOf(this StringBuilder self, Int32 offset, Char ch)
        {
            for (Int32 i = offset; i < self.Length; i++)
            {
                if (self[i] == ch)
                    return i;
            }

            return -1;
        }

        public static Int32 IndexFromEnd(this StringBuilder self, Int32 offset, Char ch)
        {
            for (Int32 i = offset; i >= 0; i--)
            {
                if (self[i] == ch)
                    return i;
            }

            return -1;
        }

        public static String Evict(this StringBuilder self)
        {
            String result = self.ToString();
            self.Length = 0;
            return result;
        }
    }
}