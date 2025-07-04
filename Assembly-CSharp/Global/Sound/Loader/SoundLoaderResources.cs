using Memoria;
using Memoria.Prime;
using Memoria.Prime.AKB2;
using Memoria.Prime.NVorbis;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

public class SoundLoaderResources : ISoundLoader
{
    public override void Initial()
    {
    }

    public override void Load(SoundProfile profile, ISoundLoader.ResultCallback callback, SoundDatabase soundDatabase)
    {
        String akbPath = "Sounds/" + profile.ResourceID + ".akb";
        Byte[] binAsset;
        SoundLib.Log("Load: " + akbPath);
        if (Configuration.Audio.PriorityToOGG)
        {
            binAsset = TryOpeningOgg(profile, callback);
            if (binAsset == null)
                binAsset = AssetManager.LoadBytes(akbPath);
        }
        else
        {
            binAsset = AssetManager.LoadBytes(akbPath);
            if (binAsset == null)
                binAsset = TryOpeningOgg(profile, callback);
        }
        if (binAsset == null)
        {
            SoundLib.LogError("File not found AT path: " + akbPath);
            callback(null, null);
            return;
        }
        // if (((binAsset[0] << 24) | (binAsset[1] << 16) | (binAsset[2] << 8) | binAsset[3]) == 0x4F676753)
        IntPtr intPtr = Marshal.AllocHGlobal(binAsset.Length);
        Marshal.Copy(binAsset, 0, intPtr, binAsset.Length);
        String[] akbInfo = null;
        String akbInfoPath = AssetManager.SearchAssetOnDisc(Path.ChangeExtension(akbPath, AssetManager.MemoriaInfoExtension), true, false);
        if (!String.IsNullOrEmpty(akbInfoPath))
            akbInfo = File.ReadAllLines(akbInfoPath);
        if (akbInfo != null && akbInfo.Length > 0)
        {
            Byte[] akbBin = new byte[304];
            Marshal.Copy(intPtr, akbBin, 0, 304);
            AKB2Header akbHeader = new AKB2Header();
            UInt32 headerSize = akbHeader.ReadFromBytes(akbBin);
            foreach (String s in akbInfo)
            {
                String[] akbCode = s.Split(' ');
                if (akbCode.Length >= 2 && String.Compare(akbCode[0], "LoopStart") == 0)
                    UInt32.TryParse(akbCode[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out akbHeader.LoopStart);
                else if (akbCode.Length >= 2 && String.Compare(akbCode[0], "LoopEnd") == 0)
                    UInt32.TryParse(akbCode[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out akbHeader.LoopEnd);
                else if (akbCode.Length >= 2 && String.Compare(akbCode[0], "LoopStart2") == 0)
                    UInt32.TryParse(akbCode[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out akbHeader.LoopStartAlternate);
                else if (akbCode.Length >= 2 && String.Compare(akbCode[0], "LoopEnd2") == 0)
                    UInt32.TryParse(akbCode[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out akbHeader.LoopEndAlternate);
                else if (akbCode.Length >= 2 && String.Compare(akbCode[0], "SampleRate") == 0)
                    UInt16.TryParse(akbCode[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out akbHeader.SampleRate);
            }
            Marshal.Copy(akbHeader.WriteToBytes(), 0, intPtr, (Int32)headerSize);
        }
        Int32 bankID = ISdLibAPIProxy.Instance.SdSoundSystem_AddData(intPtr, profile);
        profile.AkbBin = intPtr;
        profile.BankID = bankID;
        callback(profile, soundDatabase);
    }

    private Byte[] TryOpeningOgg(SoundProfile profile, ISoundLoader.ResultCallback callback)
    {
        String oggPath = AssetManager.SearchAssetOnDisc("Sounds/" + profile.ResourceID + ".ogg", true, false);
        if (!String.IsNullOrEmpty(oggPath))
        {
            try
            {
                Byte[] binOgg = File.ReadAllBytes(oggPath);
                Byte[] binAsset = ReadAkbDataFromOgg(profile, binOgg);
                File.WriteAllBytes(Path.ChangeExtension(oggPath, ".akb.bytes"), binAsset);
                return binAsset;
            }
            catch (Exception err)
            {
                Log.Error(err, $"[{nameof(SoundLoaderResources)}] Load {oggPath} failed.");
                callback(null, null);
                return null;
            }
        }
        return null;
    }

    // Duplicate of SoundImporter.ReadAkbDataFromOgg
    public static Byte[] ReadAkbDataFromOgg(SoundProfile profile, Byte[] oggBinary)
    {
        using (BinaryReader input = new BinaryReader(new MemoryStream(oggBinary)))
        {
            // Get size of content
            UInt32 contentSize = (UInt32)oggBinary.Length;
            UInt32 tailSize = contentSize % 16;
            if (tailSize > 0)
                tailSize = 16 - tailSize;
            UInt32 dataSize = AkbHeaderSize + contentSize;
            UInt32 resultSize = dataSize + tailSize;
            Byte[] akbBinary = new Byte[resultSize];

            // Prepare header
            unsafe
            {
                fixed (Byte* fixedBinary = akbBinary)
                {
                    AKB2Header* header = (AKB2Header*)fixedBinary;
                    AKB2Header.Initialize(header);

                    // Initialize header
                    header->FileSize = resultSize;
                    header->ContentSize = contentSize;
                    header->Unknown09 = 0x000004002;
                    header->Unknown33 = 0x0002;
                    ReadMetadataFromVorbis(profile, (MemoryStream)input.BaseStream, header);
                }
            }

            // Copy OGG
            Array.Copy(oggBinary, 0, akbBinary, AkbHeaderSize, contentSize);
            return akbBinary;
        }
    }

    private static unsafe void ReadMetadataFromVorbis(SoundProfile profile, MemoryStream input, AKB2Header* header)
    {
        input.Position = 0;

        using (VorbisReader vorbis = new VorbisReader(input, false))
        {
            //header->SampleCount = checked((UInt32)vorbis.TotalSamples);
            header->SampleRate = checked((UInt16)vorbis.SampleRate);

            foreach (String comment in vorbis.Comments)
            {
                TryParseTag(comment, "LoopStart", ref header->LoopStart);
                TryParseTag(comment, "LoopEnd", ref header->LoopEnd);
                TryParseTag(comment, "LoopStart2", ref header->LoopStartAlternate);
                TryParseTag(comment, "LoopEnd2", ref header->LoopEndAlternate);
            }

            if (profile.SoundProfileType == SoundProfileType.Music)
            {
                if (header->LoopEnd == 0)
                    header->LoopEnd = checked((UInt32)(vorbis.TotalSamples - 1));
            }
        }
    }

    private static Boolean TryParseTag(String comment, String tagName, ref UInt32 tagVariable)
    {
        String tagWithEq = tagName + "=";
        if (comment.Length > tagWithEq.Length && comment.StartsWith(tagWithEq, StringComparison.InvariantCultureIgnoreCase))
        {
            tagVariable = UInt32.Parse(comment.Substring(tagWithEq.Length), CultureInfo.InvariantCulture);
            return true;
        }
        return false;
    }

    private const Int32 AkbHeaderSize = 304;
}
