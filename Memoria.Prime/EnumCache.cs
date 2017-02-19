using System;
using System.Globalization;
using System.Linq;

namespace Memoria.Prime
{
    public static class EnumCache<T>
    {
        public static readonly String[] Names = Enum.GetNames(TypeCache<T>.Type);
        public static readonly T[] Values = (T[])Enum.GetValues(TypeCache<T>.Type);
        public static readonly UInt64[] Integers = Values.Select(ToUInt64).ToArray();

        public static UInt64 ToUInt64(T value)
        {
            UInt64 result;

            switch (TypeCache<T>.TypeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    result = (UInt64)Convert.ToInt64(value, CultureInfo.InvariantCulture);
                    break;

                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    result = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
                    break;

                default:
                    throw new InvalidOperationException("Unknown enum type: " + TypeCache<T>.TypeCode);
            }

            return result;
        }
    }
}