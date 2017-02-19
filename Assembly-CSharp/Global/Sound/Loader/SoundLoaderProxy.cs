using System;
using System.Collections.Generic;

public class SoundLoaderProxy
{
	public static ISoundLoader Instance
	{
		get
		{
			if (SoundLoaderProxy.instance == null)
			{
				SoundLoaderProxy.instance = SoundLoaderProxy.soundLoaderDict[SoundLibResourceLocation.ResourcesRecursive]();
			}
			return SoundLoaderProxy.instance;
		}
	}

	private static ISoundLoader CreateSoundLoaderFromResources()
	{
		SoundLoaderProxy.instance = new SoundLoaderResources();
		return SoundLoaderProxy.instance;
	}

	private static ISoundLoader CreateSoundLoaderForDocumentDirectory()
	{
		SoundLoaderProxy.instance = new SoundLoaderDocumentDirectory();
		return SoundLoaderProxy.instance;
	}

	private static ISoundLoader CreateSoundLoaderFromStreamingAssets()
	{
		SoundLoaderProxy.instance = new SoundLoaderStreamingAssets();
		return SoundLoaderProxy.instance;
	}

	private static ISoundLoader instance = (ISoundLoader)null;

	private static Dictionary<SoundLibResourceLocation, SoundLoaderProxy.CreateSoundLoader> soundLoaderDict = new Dictionary<SoundLibResourceLocation, SoundLoaderProxy.CreateSoundLoader>
	{
		{
			SoundLibResourceLocation.Resources,
			new SoundLoaderProxy.CreateSoundLoader(SoundLoaderProxy.CreateSoundLoaderFromResources)
		},
		{
			SoundLibResourceLocation.ResourcesRecursive,
			new SoundLoaderProxy.CreateSoundLoader(SoundLoaderProxy.CreateSoundLoaderFromResources)
		},
		{
			SoundLibResourceLocation.DocumentDirectory,
			new SoundLoaderProxy.CreateSoundLoader(SoundLoaderProxy.CreateSoundLoaderForDocumentDirectory)
		},
		{
			SoundLibResourceLocation.StreamingAsset,
			new SoundLoaderProxy.CreateSoundLoader(SoundLoaderProxy.CreateSoundLoaderFromStreamingAssets)
		}
	};

	private delegate ISoundLoader CreateSoundLoader();
}
