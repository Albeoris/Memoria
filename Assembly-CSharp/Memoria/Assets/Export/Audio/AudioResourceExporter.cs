using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memoria.Prime;
using UnityEngine;

namespace Memoria.Assets
{
    public static class AudioResourceExporter
    {
        public static void ExportSafe()
        {
            try
            {
                if (!Configuration.Export.Audio)
                {
                    Log.Message("[AudioResourceExporter] Pass through {Configuration.Export.Audio = 0}.");
                    return;
                }

                foreach (String relativePath in EnumerateUniqueSoundResources())
                {
                    String path = AudioResources.Embaded.GetSoundPath(relativePath);
                    TextAsset textAsset = AssetManager.Load<TextAsset>(path, true);
                    if (textAsset != null)
                    {
                        path = AudioResources.Export.GetSoundPath(relativePath);
                        ExportSoundSafe(path, textAsset);
                    }
                    else
                    {
                        Log.Warning($"[AudioResourceExporter] Sound file not found: [{relativePath}]");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GraphicResourceExporter] Failed to export graphic resources.");
            }
        }

        private static IEnumerable<String> EnumerateUniqueSoundResources()
        {
            HashSet<String> set = new HashSet<String>();

            foreach (String relativePath in SoundMetaData.SoundEffectIndex.Values.Where(set.Add))
                yield return relativePath;

            foreach (String relativePath in SoundMetaData.SoundEffectExtendedIndex.Values.Where(set.Add))
                yield return relativePath;

            foreach (String relativePath in SoundMetaData.MusicIndex.Values.Where(set.Add))
                yield return relativePath;

            foreach (String relativePath in SoundMetaData.MovieAudioIndex.Values.Where(set.Add))
                yield return relativePath;

            foreach (String relativePath in SoundMetaData.SongIndex.Values.Where(set.Add))
                yield return relativePath;

            foreach (String relativePath in SoundMetaData.SfxSoundIndex.Values.SelectMany(l => l).Where(set.Add))
                yield return relativePath;

            foreach (String relativePath in SoundMetaData.ResidentSfxSoundIndex.Values.SelectMany(l => l).Where(set.Add))
                yield return relativePath;
        }

        private static void ExportSoundSafe(String outputPath, TextAsset textAsset)
        {
            try
            {
                if (File.Exists(outputPath))
                {
                    Log.Warning("[AudioResourceExporter] Export was skipped bacause a file already exists: [{0}].", outputPath);
                    return;
                }

                FileCommander.PrepareFileDirectory(outputPath);

                using (Stream akb = File.Create(outputPath))
                using (Stream ogg = File.Create(outputPath + ".ogg"))
                {
                    akb.Write(textAsset.bytes, 0, textAsset.bytes.Length);
                    ogg.Write(textAsset.bytes, 304, textAsset.bytes.Length - 304);
                }

                File.SetLastWriteTimeUtc(outputPath, File.GetLastWriteTimeUtc(outputPath + ".ogg"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[AudioResourceExporter] Failed to export sound [{0}] to the [{1}].", textAsset.name, outputPath);
            }
        }
    }
}