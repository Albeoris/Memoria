using System;
using System.IO;
using System.Linq;

namespace Memoria.Patcher
{
    public static class Base256
    {
        private static readonly Char[] Alphabet = "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[]^_`abcdefghijklmnopqrstuvwxyz{|}~¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃń".ToArray();
        private static readonly Byte[] Reversed = Reverse(Alphabet);

        public static Char[] Encode(Byte[] array, long offset, long length)
        {
            if (array == null)
                return null;

            if (offset + length > array.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            Char[] result = new Char[length];
            if (result.Length == 0)
                return result;

            unsafe
            {
                fixed (Byte* source = &array[offset])
                fixed (Char* map = &Alphabet[0])
                fixed (Char* target = &result[0])
                {
                    for (int i = 0; i < result.Length; i++)
                        target[i] = map[source[i]];
                }
            }

            return result;
        }

        public static Byte[] Decode(String str)
        {
            if (str == null)
                return null;

            Byte[] result = new Byte[str.Length];
            if (result.Length == 0)
                return result;

            unsafe
            {
                fixed (Char* source = str)
                fixed (Byte* target = &result[0])
                    Decode(result.Length, source, target);
            }

            return result;
        }

        public static Byte[] Decode(Char[] str)
        {
            if (str == null)
                return null;

            Byte[] result = new Byte[str.Length];
            if (result.Length == 0)
                return result;

            unsafe
            {
                fixed (Char* source = str)
                fixed (Byte* target = &result[0])
                    Decode(result.Length, source, target);
            }

            return result;
        }

        private static unsafe void Decode(Int32 length, Char* source, Byte* target)
        {
            fixed (Byte* map = &Reversed[0])
            {
                for (int i = 0; i < length; i++)
                    target[i] = map[source[i] - '!'];
            }
        }

        private static Byte[] Reverse(Char[] alphabet)
        {
            Byte[] result = new Byte['ń' - '!' + 1];
            for (int i = 0; i < 256; i++)
                result[alphabet[i] - '!'] = (byte)i;
            return result;
        }
    }
}