using Memoria;
using System;
using System.Collections.Generic;

public class SoundLoaderProxy
{
    public static ISoundLoader Instance
    {
        get
        {
            if (s_instance == null)
            {
                if (Configuration.Import.Audio)
                    s_instance = CreateSoundLoaderForImport();
                else
                    s_instance = SoundLoaderDict[SoundLibResourceLocation.ResourcesRecursive]();
            }
            return s_instance;
        }
    }

    private static ISoundLoader CreateSoundLoaderFromResources()
    {
        s_instance = new SoundLoaderResources();
        return s_instance;
    }

    private static ISoundLoader CreateSoundLoaderForDocumentDirectory()
    {
        s_instance = new SoundLoaderDocumentDirectory();
        return s_instance;
    }

    private static ISoundLoader CreateSoundLoaderFromStreamingAssets()
    {
        s_instance = new SoundLoaderStreamingAssets();
        return s_instance;
    }

    private static ISoundLoader CreateSoundLoaderForImport()
    {
        return SoundImporter.Instance;
    }

    private static ISoundLoader s_instance;

    private delegate ISoundLoader CreateSoundLoader();

    private static readonly Dictionary<SoundLibResourceLocation, CreateSoundLoader> SoundLoaderDict = new Dictionary<SoundLibResourceLocation, CreateSoundLoader>
    {
        {SoundLibResourceLocation.Resources, CreateSoundLoaderFromResources},
        {SoundLibResourceLocation.ResourcesRecursive, CreateSoundLoaderFromResources},
        {SoundLibResourceLocation.DocumentDirectory, CreateSoundLoaderForDocumentDirectory},
        {SoundLibResourceLocation.StreamingAsset, CreateSoundLoaderFromStreamingAssets}
    };
}
