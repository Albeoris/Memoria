using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

public class AESCryptography
{
    public static Byte[] Encrypt(Byte[] bytesToBeEncrypted)
    {
        Byte[] array = AESCryptography.GetPassword();
        Byte[] result = null;
        Byte[] salt = AESCryptography.GetSalt();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.KeySize = 256;
                rijndaelManaged.BlockSize = 128;
                Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(array, salt, 1000);
                rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
                rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
                rijndaelManaged.Mode = CipherMode.CBC;
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytesToBeEncrypted, 0, (Int32)bytesToBeEncrypted.Length);
                    cryptoStream.Close();
                }
                result = memoryStream.ToArray();
            }
        }
        return result;
    }

    public static Byte[] Decrypt(Byte[] bytesToBeDecrypted)
    {
        Byte[] array = AESCryptography.GetPassword();
        Byte[] result = null;
        Byte[] salt = AESCryptography.GetSalt();
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.KeySize = 256;
                rijndaelManaged.BlockSize = 128;
                Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(array, salt, 1000);
                rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
                rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
                rijndaelManaged.Mode = CipherMode.CBC;
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytesToBeDecrypted, 0, (Int32)bytesToBeDecrypted.Length);
                    cryptoStream.Close();
                }
                result = memoryStream.ToArray();
            }
        }
        return result;
    }

    private static Byte[] GetPassword()
    {
        String[] array = new String[]
        {
            "67434cd0-1ca3-11e5-9a21-1697f925ec7b",
            "7a5313a0-1ca3-11e5-b939-0800200c9a66"
        };
        String[] array2 = array;
        for (Int32 i = 0; i < (Int32)array2.Length; i++)
        {
            String text = array2[i];
            for (Int32 j = 0; j < text.Length; j++)
            {
                AESCryptography.password.AppendChar(text[j]);
            }
        }
        Byte[] bytes = Encoding.UTF8.GetBytes(AESCryptography.password.ToString());
        AESCryptography.password.Clear();
        return bytes;
    }

    private static Byte[] GetSalt()
    {
        return new Byte[]
        {
            3,
            3,
            1,
            4,
            7,
            0,
            9,
            7
        };
    }

    public const Int32 KeySize = 256;

    public const Int32 BlockSize = 128;

    private static SecureString password = new SecureString();
}
