using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Global.Sound.SaXAudio
{
    public class AudioEffectManager
    {
        private const String FILENAME = "AudioEffects.txt";

        public struct EffectPreset
        {
            public enum Flag : Byte
            {
                None = 0,
                Reverb = 1,
                Eq = 1 << 1,
                Echo = 1 << 2
            }
            public String Name = "";
            public Flag Flags;
            public SaXAudio.ReverbParameters Reverb;
            public SaXAudio.EqParameters Eq;
            public SaXAudio.EchoParameters Echo;

            public EffectPreset() { }
            public EffectPreset(String str)
            {
                String[] tokens = str.Split(';');
                Int32 i = 0;
                Name = tokens[i++];
                Flags = (Flag)Byte.Parse(tokens[i++]);

                FieldInfo[] fields = typeof(SaXAudio.ReverbParameters).GetFields();
                object reverb = Reverb;
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(reverb, Convert.ChangeType(tokens[i++], field.FieldType, CultureInfo.InvariantCulture));
                }
                Reverb = (SaXAudio.ReverbParameters)reverb;

                fields = typeof(SaXAudio.EqParameters).GetFields();
                object eq = Eq;
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(eq, Convert.ChangeType(tokens[i++], field.FieldType, CultureInfo.InvariantCulture));
                }
                Eq = (SaXAudio.EqParameters)eq;

                fields = typeof(SaXAudio.EchoParameters).GetFields();
                object echo = Echo;
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(echo, Convert.ChangeType(tokens[i++], field.FieldType, CultureInfo.InvariantCulture));
                }
                Echo = (SaXAudio.EchoParameters)echo;
            }

            public override String ToString()
            {
                StringBuilder builder = new StringBuilder();
                const Char separator = ';';
                builder.Append(Name);
                builder.Append(separator);
                builder.Append((Byte)Flags);

                FieldInfo[] fields = typeof(SaXAudio.ReverbParameters).GetFields();
                foreach (FieldInfo field in fields)
                {
                    builder.Append(separator);
                    builder.Append((String)Convert.ChangeType(field.GetValue(Reverb), typeof(String), CultureInfo.InvariantCulture));
                }

                fields = typeof(SaXAudio.EqParameters).GetFields();
                foreach (FieldInfo field in fields)
                {
                    builder.Append(separator);
                    builder.Append((String)Convert.ChangeType(field.GetValue(Eq), typeof(String), CultureInfo.InvariantCulture));
                }

                fields = typeof(SaXAudio.EchoParameters).GetFields();
                foreach (FieldInfo field in fields)
                {
                    builder.Append(separator);
                    builder.Append((String)Convert.ChangeType(field.GetValue(Echo), typeof(String), CultureInfo.InvariantCulture));
                }

                return builder.ToString();
            }
        }

        public static SortedDictionary<String, EffectPreset> LoadPresets(String modLocation, Boolean backup = false)
        {
            SortedDictionary<String, EffectPreset> presets = new SortedDictionary<String, EffectPreset>();
            String path = Path.Combine(modLocation, $"{FILENAME}{(backup ? ".bak" : "")}");
            if (File.Exists(path))
            {
                try
                {
                    String[] lines = File.ReadAllLines(path);
                    foreach (String line in lines)
                    {
                        String decryptedLine = Decrypt(line, modLocation);
                        Log.Message(decryptedLine);
                        if (decryptedLine.Length == 0 || decryptedLine.StartsWith("#"))
                            continue;
                        EffectPreset preset = new EffectPreset(decryptedLine);
                        presets.Add(preset.Name, preset);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            return presets;
        }

        public static void SavePresets(SortedDictionary<String, EffectPreset> presets, String modLocation, Boolean backup = false)
        {
            try
            {
                List<String> lines = new List<String>();
                foreach (EffectPreset effectPreset in presets.Values)
                {
                    lines.Add(Encrypt(effectPreset.ToString(), modLocation));
                }
                String path = Path.Combine(modLocation, $"{FILENAME}{(backup ? ".bak" : "")}");
                File.WriteAllLines(path, lines.ToArray());
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static string Encrypt(string plainText, string password)
        {
            using Aes aes = Aes.Create();
            var key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("AlexandriaCastle"), 10000);
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);
            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                cs.Write(plainBytes, 0, plainBytes.Length);
                cs.FlushFinalBlock(); // This is crucial!
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText, string password)
        {
            using Aes aes = Aes.Create();
            var key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("AlexandriaCastle"), 10000);
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs);

            return reader.ReadToEnd();
        }
    }

}
