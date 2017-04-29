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

        private static void ExportSoundSafe(String akbOutputPath, TextAsset textAsset)
        {
            try
            {
                String oggOutputPath = akbOutputPath + ".ogg";

                String fileName;
                String directoryPath;
                String alternativePath;
                if (AudioResources.TryAppendDisplayName(akbOutputPath, out directoryPath, out fileName, out alternativePath))
                {
                    alternativePath += ".ogg";

                    if (File.Exists(alternativePath))
                    {
                        Log.Warning("[AudioResourceExporter] Export was skipped bacause a file already exists: [{0}].", alternativePath);
                        return;
                    }

                    if (File.Exists(oggOutputPath))
                    {
                        Log.Message("[AudioResourceExporter] The file [{0}] will be renamed to [{1}].", oggOutputPath, alternativePath);
                        File.Move(oggOutputPath, alternativePath);
                        Log.Warning("[AudioResourceExporter] Export was skipped bacause a file already exists: [{0}].", alternativePath);
                        return;
                    }

                    oggOutputPath = alternativePath;
                }
                else if (File.Exists(akbOutputPath))
                {
                    Log.Warning("[AudioResourceExporter] Export was skipped bacause a file already exists: [{0}].", akbOutputPath);
                    return;
                }
                else
                {
                    oggOutputPath = akbOutputPath + ".ogg";
                }

                FileCommander.PrepareFileDirectory(akbOutputPath);

                using (Stream akb = File.Create(akbOutputPath))
                using (Stream ogg = File.Create(oggOutputPath))
                {
                    akb.Write(textAsset.bytes, 0, textAsset.bytes.Length);
                    ogg.Write(textAsset.bytes, 304, textAsset.bytes.Length - 304);
                }

                File.SetLastWriteTimeUtc(akbOutputPath, File.GetLastWriteTimeUtc(oggOutputPath));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[AudioResourceExporter] Failed to export sound [{0}] to the [{1}].", textAsset.name, akbOutputPath);
            }
        }
    }
}