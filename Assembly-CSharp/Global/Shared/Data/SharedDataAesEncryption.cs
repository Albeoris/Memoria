using System;

public class SharedDataAesEncryption : ISharedDataEncryption
{
	public override Byte[] Encrypt(Byte[] bytes)
	{
		return AESCryptography.Encrypt(bytes);
	}

	public override Byte[] Decrypt(Byte[] bytes)
	{
		return AESCryptography.Decrypt(bytes);
	}

	public override Int32 GetCipherSize(Int32 plainTextSize)
	{
		Int32 num = 16;
		return plainTextSize + num - plainTextSize % num;
	}
}
