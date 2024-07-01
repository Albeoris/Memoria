using System;

public class SharedDataNoEncryption : ISharedDataEncryption
{
    public override Byte[] Encrypt(Byte[] bytes)
    {
        return bytes;
    }

    public override Byte[] Decrypt(Byte[] bytes)
    {
        return bytes;
    }

    public override Int32 GetCipherSize(Int32 plainTextSize)
    {
        return plainTextSize;
    }
}
