using System;
using System.Globalization;
using System.Linq;

namespace Memoria.Prime.CSV
{
    public static class CsvParser
    {
        public static Boolean Boolean(String raw)
        {
            switch (raw[0])
            {
                case '1':
                    return true;
                case '0':
                    return false;
                default:
                    throw new NotSupportedException(raw);
            }
        }

        private const NumberStyles NumberStyle = NumberStyles.Integer | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

        public static Byte Byte(String raw)
        {
            return System.Byte.Parse(raw, NumberStyle, CultureInfo.InvariantCulture);
        }

        public static Int16 Int16(String raw)
        {
            return System.Int16.Parse(raw, NumberStyle, CultureInfo.InvariantCulture);
        }

        public static UInt16 UInt16(String raw)
        {
            return System.UInt16.Parse(raw, NumberStyle, CultureInfo.InvariantCulture);
        }

        public static UInt32 UInt32(String raw)
        {
            return System.UInt32.Parse(raw, NumberStyle, CultureInfo.InvariantCulture);
        }

        public static Byte[] ByteArray(String raw)
        {
            if (String.IsNullOrEmpty(raw))
                return new Byte[0];

            return raw.Split(',').Select(Byte).ToArray();
        }
    }
}