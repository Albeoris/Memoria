using System;
using System.IO;

internal class WMReadFile
{
    public static Byte[] ReadFully(Stream stream, Int32 initialLength)
    {
        if (initialLength < 1)
        {
            initialLength = 32768;
        }
        Byte[] array = new Byte[initialLength];
        Int32 num = 0;
        Int32 num2;
        while ((num2 = stream.Read(array, num, (Int32)array.Length - num)) > 0)
        {
            num += num2;
            if (num == (Int32)array.Length)
            {
                Int32 num3 = stream.ReadByte();
                if (num3 == -1)
                {
                    return array;
                }
                Byte[] array2 = new Byte[(Int32)array.Length * 2];
                Array.Copy(array, array2, (Int32)array.Length);
                array2[num] = (Byte)num3;
                array = array2;
                num++;
            }
        }
        Byte[] array3 = new Byte[num];
        Array.Copy(array, array3, num);
        return array3;
    }
}
