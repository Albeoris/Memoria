using System;

public class ByteEncryption
{
    public static void Decryption(Byte[] src, ref Byte[] dst)
    {
        Int64 blockSize = 1024L;
        Int64 srcLength = (Int64)src.Length;
        Int64 dstLength = srcLength - blockSize;
        Int64 currentBlock = 0L;
        Int32 dstIndex = 0;
        while ((Int64)dstIndex < dstLength)
        {
            if (currentBlock < blockSize)
            {
                dst[dstIndex] = src[(Int32)(checked((IntPtr)(unchecked(srcLength - blockSize + currentBlock))))];
                currentBlock += 1L;
            }
            else if ((Int64)dstIndex < srcLength)
            {
                dst[dstIndex] = src[dstIndex];
            }
            dstIndex++;
        }
    }

    public static Byte[] Decryption(Byte[] bytes)
    {
        Int64 blockSize = 1024L;
        Int64 bytesLength = (Int64)bytes.Length;
        Int64 decryptedLength = bytesLength - blockSize;
        Byte[] decryptedArray = new Byte[decryptedLength];
        Int64 currentBlock = 0L;
        Int32 decryptedIndex = 0;
        while ((Int64)decryptedIndex < decryptedLength)
        {
            if (currentBlock < blockSize)
            {
                decryptedArray[decryptedIndex] = bytes[(Int32)(checked((IntPtr)(unchecked(bytesLength - blockSize + currentBlock))))];
                currentBlock += 1L;
            }
            else if ((Int64)decryptedIndex < bytesLength)
            {
                decryptedArray[decryptedIndex] = bytes[decryptedIndex];
            }
            decryptedIndex++;
        }
        return decryptedArray;
    }

    public static Byte[] Encryption(Byte[] bytes)
    {
        Int64 blockSize = 1024L;
        Int64 bytesLength = (Int64)bytes.Length;
        Int64 encryptedLength = bytesLength + blockSize;
        Byte[] encryptedArray = new Byte[encryptedLength];
        Int64 currentBlock = 0L;
        Int64 remainingIndex = 0L;
        Int32 encryptedIndex = 0;
        while ((Int64)encryptedIndex < encryptedLength)
        {
            if (currentBlock < blockSize)
            {
                encryptedArray[encryptedIndex] = bytes[(Int32)(checked((IntPtr)(unchecked(currentBlock + blockSize))))];
                currentBlock += 1L;
            }
            else if ((Int64)encryptedIndex < bytesLength)
            {
                encryptedArray[encryptedIndex] = bytes[encryptedIndex];
            }
            else
            {
                encryptedArray[encryptedIndex] = bytes[(Int32)(checked((IntPtr)remainingIndex))];
                remainingIndex += 1L;
            }
            encryptedIndex++;
        }
        return encryptedArray;
    }
}
