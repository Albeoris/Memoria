using System;
using UnityEngine;

public abstract class ISharedDataEncryption : MonoBehaviour
{
	public abstract Byte[] Encrypt(Byte[] bytes);

	public abstract Byte[] Decrypt(Byte[] bytes);

	public abstract Int32 GetCipherSize(Int32 plainTextSize);
}
