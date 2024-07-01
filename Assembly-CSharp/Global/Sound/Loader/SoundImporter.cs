using Memoria.Assets;
using Memoria.Prime;
using Memoria.Prime.AKB2;
using Memoria.Prime.NVorbis;
using Memoria.Prime.WinAPI;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Memoria
{
    internal sealed class SoundImporter : ISoundLoader
    {
        const Int32 AkbHeaderSize = 304;

        public static readonly SoundImporter Instance = new SoundImporter();

        private readonly SoundLoaderResources _resourceLoader;

        public SoundImporter()
        {
            _resourceLoader = new SoundLoaderResources();

            this.Initial();
        }

        public override void Initial()
        {
        }

        public override void Load(SoundProfile profile, ResultCallback callback, SoundDatabase soundDatabase)
        {
            try
            {
                IntPtr akbData = ReadAkbDataToUnmanagedMemory(profile);
                if (akbData == IntPtr.Zero)
                {
                    _resourceLoader.Load(profile, callback, soundDatabase);
                    return;
                }

                try
                {
                    Int32 bankID = ISdLibAPIProxy.Instance.SdSoundSystem_AddData(akbData, profile);
                    profile.AkbBin = akbData;
                    profile.BankID = bankID;
                }
                catch
                {
                    Marshal.FreeHGlobal(akbData);
                    throw;
                }

                callback(profile, soundDatabase);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to import sound: " + profile.ResourceID);
                _resourceLoader.Load(profile, callback, soundDatabase);
            }
        }

        private static IntPtr ReadAkbDataToUnmanagedMemory(SoundProfile profile)
        {
            String akbPath = AudioResources.Import.GetSoundPath(profile.ResourceID);
            String oggPath = akbPath + ".ogg";

            String fileName;
            String directoryPath;
            String alternativeOggPath;
            if (AudioResources.TryAppendDisplayName(akbPath, out directoryPath, out fileName, out alternativeOggPath))
            {
                alternativeOggPath += ".ogg";

                if (!File.Exists(alternativeOggPath) && File.Exists(oggPath))
                {
                    Log.Message("[SoundImporter] The file [{0}] will be renamed to [{1}].", akbPath, alternativeOggPath);
                    File.Move(oggPath, alternativeOggPath);
                }
            }

            if (!Directory.Exists(directoryPath))
                return IntPtr.Zero;

            String[] oggFiles = Directory.GetFiles(directoryPath, fileName + "*.ogg");
            if (oggFiles.Length == 1)
            {
                oggPath = oggFiles[0];
                return ReadAkbDataFromOggViaCache(profile, oggPath, akbPath);
            }
            if (oggFiles.Length > 1)
            {
                oggPath = oggFiles.OrderByDescending(File.GetLastWriteTimeUtc).First();
                Log.Warning("[SoundImporter] There is several files with the same internal name. The last modified will be used: {0}", oggPath);
                return ReadAkbDataFromOggViaCache(profile, oggPath, akbPath);
            }

            if (File.Exists(akbPath))
                return ReadFileToUnmanagedMemory(akbPath);

            return IntPtr.Zero;
        }

        private static IntPtr ReadAkbDataFromOggViaCache(SoundProfile profile, String oggPath, String akbPath)
        {
            DateTime oggTime = File.GetLastWriteTimeUtc(oggPath);
            if (File.Exists(akbPath))
            {
                if (oggTime == File.GetLastWriteTimeUtc(akbPath))
                    return ReadFileToUnmanagedMemory(akbPath);
            }

            return ReadAkbDataFromOggAndCache(profile, oggPath, akbPath, oggTime);
        }

        private static IntPtr ReadAkbDataFromOggAndCache(SoundProfile profile, String oggPath, String akbPath, DateTime oggTime)
        {
            IntPtr result = IntPtr.Zero;
            try
            {
                UInt32 resultSize;
                ReadAkbDataFromOgg(profile, oggPath, out result, out resultSize);
                WriteAkbDataToCache(akbPath, oggTime, result, resultSize);
                return result;
            }
            catch
            {
                if (result != IntPtr.Zero)
                    Marshal.FreeHGlobal(result);

                throw;
            }
        }

        private static unsafe void WriteAkbDataToCache(String akbPath, DateTime oggTime, IntPtr result, UInt32 resultSize)
        {
            try
            {
                using (UnmanagedMemoryStream input = new UnmanagedMemoryStream((Byte*)result, resultSize, resultSize, FileAccess.Read))
                using (FileStream output = File.Create(akbPath))
                    input.CopyTo(output, Math.Min(resultSize, 80000));

                File.SetLastWriteTimeUtc(akbPath, oggTime);
                Log.Message("AKB2-cache refreshed: " + akbPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to resfresh AKB2-cache: " + akbPath);
            }
        }

        private static unsafe void ReadAkbDataFromOgg(SoundProfile profile, String oggPath, out IntPtr result, out UInt32 resultSize)
        {
            using (FileStream input = File.OpenRead(oggPath))
            {
                // Get size of content
                UInt32 contentSize = (UInt32)input.Length;
                UInt32 tailSize = contentSize % 16;
                UInt32 dataSize = AkbHeaderSize + contentSize;
                resultSize = dataSize + tailSize;
                if (tailSize > 0)
                    tailSize = 16 - tailSize;

                // Make an unmanaged array
                CreateUnmanagedArray(out result, resultSize, tailSize, AkbHeaderSize);

                // Read file to unmanaged memory
                using (UnmanagedMemoryStream output = new UnmanagedMemoryStream((Byte*)result, resultSize, resultSize, FileAccess.Write))
                {
                    output.Seek(AkbHeaderSize, SeekOrigin.Begin);
                    input.CopyTo(output, Math.Min(resultSize, 80000));
                }

                // Prepare header
                AKB2Header* header = (AKB2Header*)result;
                AKB2Header.Initialize(header);

                // Initialize header
                header->FileSize = resultSize;
                header->ContentSize = contentSize;
                header->Unknown09 = 0x000004002;
                header->Unknown33 = 0x0002;

                ReadMetadataFromVorbis(profile, input, header);
            }
        }

        private static unsafe void ReadMetadataFromVorbis(SoundProfile profile, FileStream input, AKB2Header* header)
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

        private static void CreateUnmanagedArray(out IntPtr result, UInt32 resultSize, UInt32 tailSize, Int32 headerSize)
        {
            result = Marshal.AllocHGlobal(new IntPtr(resultSize + tailSize));

            // Zero header
            Kernel32.FillMemory(result, AkbHeaderSize, 0);

            // Zero tail
            if (tailSize > 0)
                Kernel32.FillMemory(new IntPtr((Int64)result + resultSize - tailSize), tailSize, 0);
        }

        private static unsafe IntPtr ReadFileToUnmanagedMemory(String akbPath)
        {
            using (FileStream input = File.OpenRead(akbPath))
            {
                Int64 fileSize = input.Length;
                IntPtr resultPtr = Marshal.AllocHGlobal(new IntPtr(fileSize));
                try
                {
                    using (UnmanagedMemoryStream output = new UnmanagedMemoryStream((Byte*)resultPtr, fileSize, fileSize, FileAccess.Write))
                        input.CopyTo(output, bufferSize: (Int32)Math.Min(fileSize, 80000));
                }
                catch
                {
                    Marshal.FreeHGlobal(resultPtr);
                    throw;
                }
                return resultPtr;
            }
        }
    }
}
