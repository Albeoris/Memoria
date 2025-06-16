using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            public enum Effect : Byte
            {
                None = 0,
                Reverb = 1,
                Eq = 1 << 1,
                Echo = 1 << 2,
                Volume = 1 << 3,
                All = Reverb | Eq | Echo | Volume
            }

            public enum Layer : Byte
            {
                None = 0,
                Music = 1,
                Ambient = 1 << 1,
                SoundEffect = 1 << 2,
                Voice = 1 << 3,
                All = Music | Ambient | SoundEffect | Voice
            }

            public String Name = "";
            public Effect Effects = Effect.None;
            public SaXAudio.ReverbParameters Reverb = new SaXAudio.ReverbParameters();
            public SaXAudio.EqParameters Eq = new SaXAudio.EqParameters();
            public SaXAudio.EchoParameters Echo = new SaXAudio.EchoParameters();
            public Single Volume = 1f;

            public Layer Layers = Layer.None;
            public HashSet<Int32> FieldIDs = new HashSet<Int32>();
            public HashSet<Int32> BattleIDs = new HashSet<Int32>();
            public HashSet<Int32> BattleBgIDs = new HashSet<Int32>();

            public String Condition = "";

            public EffectPreset() { }
            public EffectPreset(String str)
            {
                String[] tokens = str.Split(';');
                Int32 i = 0;
                Name = tokens[i++];
                Effects = (Effect)Byte.Parse(tokens[i++]);

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

                Volume = Single.Parse(tokens[i++]);

                if (i == tokens.Length) return;

                Layers = (Layer)Byte.Parse(tokens[i++]);

                String[] ids = tokens[i++].Split('|');
                if (ids.Length > 0)
                {
                    FieldIDs.Clear();
                    foreach (String id in ids)
                    {
                        if (id.Length > 0)
                            FieldIDs.Add(Int32.Parse(id));
                    }
                }

                ids = tokens[i++].Split('|');
                if (ids.Length > 0)
                {
                    BattleIDs.Clear();
                    foreach (String id in ids)
                    {
                        if (id.Length > 0)
                            BattleIDs.Add(Int32.Parse(id));
                    }
                }

                ids = tokens[i++].Split('|');
                if (ids.Length > 0)
                {
                    BattleBgIDs.Clear();
                    foreach (String id in ids)
                    {
                        if (id.Length > 0)
                            BattleBgIDs.Add(Int32.Parse(id));
                    }
                }

                Condition = tokens[i++].Replace('\x0A', ';');
            }

            public override readonly String ToString()
            {
                StringBuilder builder = new StringBuilder();
                const Char separator = ';';
                builder.Append(Name);
                builder.Append(separator);
                builder.Append((Byte)Effects);

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

                builder.Append(separator);
                builder.Append(Volume);

                builder.Append(separator);
                builder.Append((Byte)Layers);

                builder.Append(separator);
                builder.Append(String.Join("|", FieldIDs.Select(x => x.ToString()).ToArray()));

                builder.Append(separator);
                builder.Append(String.Join("|", BattleIDs.Select(x => x.ToString()).ToArray()));

                builder.Append(separator);
                builder.Append(String.Join("|", BattleBgIDs.Select(x => x.ToString()).ToArray()));

                builder.Append(separator);
                builder.Append(Condition.Replace(';', '\x0A'));

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
                cs.FlushFinalBlock();
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
