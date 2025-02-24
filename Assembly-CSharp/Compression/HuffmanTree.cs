namespace Compression;

internal class HuffmanTree
{
    internal const int MaxLiteralTreeElements = 288;
    internal const int MaxDistTreeElements = 32;
    internal const int EndOfBlockCode = 256;
    internal const int NumberOfCodeLengthTreeElements = 19;

    private int tableBits;
    private short[] table;
    private short[] left;
    private short[] right;
    private byte[] codeLengthArray;
    private int tableMask;

    private static HuffmanTree staticLiteralLengthTree;
    private static HuffmanTree staticDistanceTree;

    public static HuffmanTree StaticLiteralLengthTree => staticLiteralLengthTree;
    public static HuffmanTree StaticDistanceTree => staticDistanceTree;

    static HuffmanTree()
    {
        staticLiteralLengthTree = new HuffmanTree(GetStaticLiteralTreeLength());
        staticDistanceTree = new HuffmanTree(GetStaticDistanceTreeLength());
    }

    public HuffmanTree(byte[] codeLengths)
    {
        codeLengthArray = codeLengths;
        tableBits = codeLengthArray.Length == HuffmanTree.MaxLiteralTreeElements ? 9 : 7;
        tableMask = (1 << tableBits) - 1;
        CreateTable();
    }

    private static byte[] GetStaticLiteralTreeLength()
    {
        byte[] array = new byte[HuffmanTree.MaxLiteralTreeElements];
        for (int i = 0; i <= 143; i++)
            array[i] = 8;
        for (int i = 144; i <= 255; i++)
            array[i] = 9;
        for (int i = 256; i <= 279; i++)
            array[i] = 7;
        for (int i = 280; i <= 287; i++)
            array[i] = 8;
        return array;
    }

    private static byte[] GetStaticDistanceTreeLength()
    {
        byte[] array = new byte[HuffmanTree.MaxDistTreeElements];
        for (int i = 0; i < HuffmanTree.MaxDistTreeElements; i++)
            array[i] = 5;
        return array;
    }

    private uint[] CalculateHuffmanCode()
    {
        uint[] array = new uint[17];
        byte[] array2 = codeLengthArray;
        foreach (int num in array2)
            array[num]++;
        array[0] = 0u;
        uint[] array3 = new uint[17];
        uint num2 = 0u;
        for (int i = 1; i <= 16; i++)
            num2 = array3[i] = num2 + array[i - 1] << 1;
        uint[] array4 = new uint[HuffmanTree.MaxLiteralTreeElements];
        for (int i = 0; i < codeLengthArray.Length; i++)
        {
            int num3 = codeLengthArray[i];
            if (num3 > 0)
            {
                array4[i] = DecodeHelper.BitReverse(array3[num3], num3);
                array3[num3]++;
            }
        }
        return array4;
    }

    private void CreateTable()
    {
        uint[] array = CalculateHuffmanCode();
        table = new short[1 << tableBits];
        left = new short[2 * codeLengthArray.Length];
        right = new short[2 * codeLengthArray.Length];
        short num = (short)codeLengthArray.Length;
        for (int i = 0; i < codeLengthArray.Length; i++)
        {
            int num2 = codeLengthArray[i];
            if (num2 <= 0)
                continue;
            int num3 = (int)array[i];
            if (num2 <= tableBits)
            {
                int num4 = 1 << num2;
                if (num3 >= num4)
                    throw new InvalidDataException("InvalidHuffmanData");
                int num5 = 1 << tableBits - num2;
                for (int j = 0; j < num5; j++)
                {
                    table[num3] = (short)i;
                    num3 += num4;
                }
                continue;
            }
            int num6 = num2 - tableBits;
            int num7 = 1 << tableBits;
            int num8 = num3 & ((1 << tableBits) - 1);
            short[] array2 = table;
            do
            {
                short num9 = array2[num8];
                if (num9 == 0)
                {
                    array2[num8] = (short)-num;
                    num9 = (short)-num;
                    num++;
                }
                array2 = ((num3 & num7) != 0) ? right : left;
                num8 = -num9;
                num7 <<= 1;
                num6--;
            }
            while (num6 != 0);
            array2[num8] = (short)i;
        }
    }

    public int GetNextSymbol(InputBuffer input)
    {
        uint num = input.TryLoad16Bits();
        if (input.AvailableBits == 0)
            return -1;
        int num2 = table[num & tableMask];
        if (num2 < 0)
        {
            uint num3 = (uint)(1 << tableBits);
            do
            {
                num2 = -num2;
                num2 = ((num & num3) != 0) ? right[num2] : left[num2];
                num3 <<= 1;
            }
            while (num2 < 0);
        }
        if (codeLengthArray[num2] > input.AvailableBits)
            return -1;
        input.SkipBits(codeLengthArray[num2]);
        return num2;
    }
}
