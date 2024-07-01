using Memoria.Assets;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class SoundMetaData
{
    public static String SoundEffectMetaData
    {
        get
        {
            return SoundMetaData.soundEffectMetaData;
        }
        set
        {
            SoundMetaData.IndexingSoundData(SoundMetaData.soundEffectExtendedMetaData, SoundMetaData.SoundEffectExtendedIndex);
            SoundMetaData.soundEffectMetaData = value;
            SoundMetaData.IndexingSoundData(value, SoundMetaData.SoundEffectIndex);
            foreach (KeyValuePair<Int32, String> keyValuePair in SoundMetaData.SoundEffectExtendedIndex)
            {
                if (!SoundMetaData.SoundEffectIndex.ContainsKey(keyValuePair.Key))
                {
                    SoundMetaData.SoundEffectIndex.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }
    }

    public static String MusicMetaData
    {
        get
        {
            return SoundMetaData.musicMetaData;
        }
        set
        {
            SoundMetaData.musicMetaData = value;
            SoundMetaData.IndexingSoundData(value, SoundMetaData.MusicIndex);
        }
    }

    public static String MovieAudioMetaData
    {
        get
        {
            return SoundMetaData.movieAudioMetaData;
        }
        set
        {
            SoundMetaData.movieAudioMetaData = value;
            SoundMetaData.IndexingSoundData(value, SoundMetaData.MovieAudioStringIndex);
            SoundMetaData.IndexingSoundData(value, SoundMetaData.MovieAudioIndex);
        }
    }

    public static String SongMetaData
    {
        get
        {
            return SoundMetaData.songMetaData;
        }
        set
        {
            SoundMetaData.songMetaData = value;
            SoundMetaData.IndexingSoundData(value, SoundMetaData.SongIndex);
        }
    }

    public static String SfxSoundMetaData
    {
        get
        {
            return SoundMetaData.sfxSoundMetaData;
        }
        set
        {
            SoundMetaData.sfxSoundMetaData = value;
            SoundMetaData.IndexingSoundData(value, SoundMetaData.SfxSoundIndex);
        }
    }

    public static String ResidentSfxSoundMetaData
    {
        get
        {
            return SoundMetaData.sfxSoundMetaData;
        }
        set
        {
            SoundMetaData.residentSfxSoundMetaData = value;
            SoundMetaData.IndexingSoundData(value, SoundMetaData.ResidentSfxSoundIndex);
        }
    }

    public static void LoadMetaData()
    {
        SoundMetaData.loadMetaDataDelegateDict[SoundLibResourceLocation.ResourcesRecursive]();
    }

    private static void LoadMetaDataFromResources()
    {
        List<SoundMetaDataProfile> list = new List<SoundMetaDataProfile>();
        list.Add(new SoundMetaDataProfile("SE_BT_AB01_00", 0, "SoundEffect"));
        list.Add(new SoundMetaDataProfile("SE_BT_AB01_01", 1, "SoundEffect"));
        List<SoundMetaDataProfile> list2 = new List<SoundMetaDataProfile>();
        list2.Add(new SoundMetaDataProfile("FF4Bat1", 2, "Music"));
        list2.Add(new SoundMetaDataProfile("FF4Damushian", 3, "Music"));
        list2.Add(new SoundMetaDataProfile("FF5Dungeon", 4, "Music"));
        list2.Add(new SoundMetaDataProfile("music102", 5, "Music"));
        SoundMetaData.SoundEffectMetaData = SoundMetaData.ComposeMetaDataJsonString(list);
        SoundMetaData.MusicMetaData = SoundMetaData.ComposeMetaDataJsonString(list2);
    }

    private static void LoadMetaDataFromResourcesRecursively()
    {
        String textAsset = AssetManager.LoadString("EmbeddedAsset/Manifest/Sounds/SoundEffectMetaData.txt");
        String textAsset2 = AssetManager.LoadString("EmbeddedAsset/Manifest/Sounds/SoundEffectExtendedMetaData.txt");
        String textAsset3 = AssetManager.LoadString("EmbeddedAsset/Manifest/Sounds/MusicMetaData.txt");
        String textAsset4 = AssetManager.LoadString("EmbeddedAsset/Manifest/Sounds/MovieAudioMetaData.txt");
        String textAsset5 = AssetManager.LoadString("EmbeddedAsset/Manifest/Sounds/SongMetaData.txt");
        String textAsset6 = AssetManager.LoadString("EmbeddedAsset/Manifest/Sounds/SpecialEffectMetaData.txt");
        String textAsset7 = AssetManager.LoadString("EmbeddedAsset/Manifest/Sounds/ResidentSpecialEffectMetaData.txt");


        Exception exception = null;

        Thread thread = new Thread(() =>
        {
            try
            {
                SoundMetaData.soundEffectExtendedMetaData = textAsset2;
                SoundMetaData.SoundEffectMetaData = textAsset;
                SoundMetaData.MusicMetaData = textAsset3;
                SoundMetaData.MovieAudioMetaData = textAsset4;
                SoundMetaData.SongMetaData = textAsset5;
                SoundMetaData.SfxSoundMetaData = textAsset6;
                SoundMetaData.ResidentSfxSoundMetaData = textAsset7;
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }, 30 * 1024 * 1024); // Workaround for StackOverflowException: The requested operation caused a stack overflow

        thread.Start();
        thread.Join();

        if (exception != null)
            throw new Exception("Failed to recursively load a meta data from resources.", exception);

        AudioResourceExporter.ExportSafe();
    }

    private static void LoadMetaDataFromDocumentDirectory()
    {
        SoundLoaderProxy.Instance.Initial();
        String path = Application.persistentDataPath + "/SoundEffect";
        String path2 = Application.persistentDataPath + "/Music";
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        FileInfo[] files = directoryInfo.GetFiles();
        DirectoryInfo directoryInfo2 = new DirectoryInfo(path2);
        FileInfo[] files2 = directoryInfo2.GetFiles();
        Int32 num = 0;
        JSONClass jsonclass = new JSONClass();
        JSONArray jsonarray = new JSONArray();
        jsonclass.Add("data", jsonarray);
        FileInfo[] array = files;
        for (Int32 i = 0; i < (Int32)array.Length; i++)
        {
            FileInfo fileInfo = array[i];
            jsonarray.Add(new JSONClass
            {
                {
                    "name",
                    fileInfo.Name
                },
                {
                    "soundIndex",
                    num.ToString()
                },
                {
                    "type",
                    "SoundEffect"
                }
            });
            num++;
        }
        JSONClass jsonclass2 = new JSONClass();
        JSONArray jsonarray2 = new JSONArray();
        jsonclass2.Add("data", jsonarray2);
        FileInfo[] array2 = files2;
        for (Int32 j = 0; j < (Int32)array2.Length; j++)
        {
            FileInfo fileInfo2 = array2[j];
            jsonarray2.Add(new JSONClass
            {
                {
                    "name",
                    fileInfo2.Name
                },
                {
                    "soundIndex",
                    num.ToString()
                },
                {
                    "type",
                    "Music"
                }
            });
            num++;
        }
        SoundMetaData.SoundEffectMetaData = jsonclass.ToString();
        SoundMetaData.MusicMetaData = jsonclass2.ToString();
    }

    private static void LoadMetaDataFromStreamingAssets()
    {
        Int32 num = 0;
        List<SoundMetaDataProfile> list = new List<SoundMetaDataProfile>();
        List<SoundMetaDataProfile> list2 = new List<SoundMetaDataProfile>();
        String item = AssetManagerUtil.GetStreamingAssetsPath() + "/Sounds";
        Queue<String> queue = new Queue<String>();
        queue.Enqueue(item);
        while (queue.Count > 0)
        {
            String text = queue.Peek();
            String[] directories = Directory.GetDirectories(text);
            String[] array = directories;
            for (Int32 i = 0; i < (Int32)array.Length; i++)
            {
                String item2 = array[i];
                queue.Enqueue(item2);
            }
            String[] files = Directory.GetFiles(text);
            String[] array2 = files;
            for (Int32 j = 0; j < (Int32)array2.Length; j++)
            {
                String text2 = array2[j];
                String[] array3 = text2.Split(new Char[]
                {
                    '.'
                });
                String a = array3[(Int32)array3.Length - 1];
                if (!String.Equals(a, "meta"))
                {
                    String[] array4 = text.Split(new Char[]
                    {
                        '/'
                    });
                    String[] array5 = array4[(Int32)array4.Length - 1].Split(new Char[]
                    {
                        '\\'
                    });
                    String text3 = array5[(Int32)array5.Length - 1];
                    String fileName = Path.GetFileName(text2);
                    if (String.Equals(text3, "BGM_") || String.Equals(text3, "Movie_") || String.Equals(text3, "song_") || String.Equals(text3, "OldBGM"))
                    {
                        list2.Add(new SoundMetaDataProfile(text3 + "/" + fileName, num++, "Music"));
                    }
                    else
                    {
                        list.Add(new SoundMetaDataProfile(text3 + "/" + fileName, num++, "SoundEffect"));
                    }
                }
            }
            queue.Dequeue();
        }
        SoundMetaData.SoundEffectMetaData = SoundMetaData.ComposeMetaDataJsonString(list);
        SoundMetaData.MusicMetaData = SoundMetaData.ComposeMetaDataJsonString(list2);
    }

    private static void IndexingSoundData(String metaData, Dictionary<String, Int32> index)
    {
        index.Clear();
        JSONNode jsonnode = JSONNode.Parse(metaData);
        JSONArray asArray = jsonnode["data"].AsArray;
        for (Int32 i = 0; i < asArray.Count; i++)
        {
            JSONClass asObject = asArray[i].AsObject;
            String key = asObject["name"];
            Int32 asInt = asObject["soundIndex"].AsInt;
            index.Add(key, asInt);
        }
    }

    private static void IndexingSoundData(String metaData, Dictionary<Int32, String> index)
    {
        index.Clear();
        JSONNode jsonnode = JSONNode.Parse(metaData);
        JSONArray asArray = jsonnode["data"].AsArray;
        for (Int32 i = 0; i < asArray.Count; i++)
        {
            JSONClass asObject = asArray[i].AsObject;
            String value = asObject["name"];
            Int32 asInt = asObject["soundIndex"].AsInt;
            index.Add(asInt, value);
        }
    }

    private static void IndexingSoundData(String metaData, Dictionary<Int32, List<String>> index)
    {
        index.Clear();
        JSONNode jsonnode = JSONNode.Parse(metaData);
        JSONClass asObject = jsonnode["data"].AsObject;
        foreach (String text in asObject.Dict.Keys)
        {
            String s = text;
            JSONArray asArray = asObject[text].AsArray;
            List<String> list = new List<String>();
            for (Int32 i = 0; i < asArray.Count; i++)
            {
                String item = asArray[i];
                list.Add(item);
            }
            Int32 key = Int32.Parse(s);
            index.Add(key, list);
        }
    }

    public static Int32 GetSoundIndex(String soundName, SoundProfileType type)
    {
        if (type != SoundProfileType.MovieAudio)
        {
            SoundLib.Log("No implementation");
            return -1;
        }
        if (SoundMetaData.MovieAudioStringIndex.ContainsKey(soundName))
        {
            return SoundMetaData.MovieAudioStringIndex[soundName];
        }
        SoundLib.Log("Movie audio, Name: " + soundName + " not found!");
        return -1;
    }

    public static SoundProfile GetSoundProfile(Int32 soundIndex, SoundProfileType type)
    {
        String text = String.Empty;
        Dictionary<Int32, String> musicDictionary = null;
        switch (type)
        {
            case SoundProfileType.Music:
                musicDictionary = SoundMetaData.MusicIndex;
                break;
            case SoundProfileType.SoundEffect:
                musicDictionary = SoundMetaData.SoundEffectIndex;
                break;
            case SoundProfileType.MovieAudio:
                musicDictionary = SoundMetaData.MovieAudioIndex;
                break;
            case SoundProfileType.Song:
                musicDictionary = SoundMetaData.SongIndex;
                break;
            case SoundProfileType.Sfx:
                SoundLib.Log("GetSoundProfile does not support type SoundProfileType.Sfx");
                return null;
        }
        if (musicDictionary == null || !musicDictionary.TryGetValue(soundIndex, out text))
        {
            // Log for Memoria.Prime.Log as well: a missing sound may indicate something needs to be fixed (possibly a missing asset or a missing entry in metadata as it was the case for enemy death sounds)
            SoundLib.Log($"Could not find the {type} with Id {soundIndex}");
            Memoria.Prime.Log.Warning($"Could not find the {type} with Id {soundIndex}");
            return null;
        }
        return new SoundProfile
        {
            Code = soundIndex.ToString(),
            Name = text,
            SoundIndex = soundIndex,
            ResourceID = text,
            SoundProfileType = type
        };
    }

    public static String ComposeMetaDataJsonString(List<SoundMetaDataProfile> list)
    {
        JSONClass jsonclass = new JSONClass();
        JSONArray jsonarray = new JSONArray();
        jsonclass.Add("data", jsonarray);
        foreach (SoundMetaDataProfile soundMetaDataProfile in list)
        {
            jsonarray.Add(new JSONClass
            {
                {
                    "name",
                    soundMetaDataProfile.Name
                },
                {
                    "soundIndex",
                    soundMetaDataProfile.SoundIndex.ToString()
                },
                {
                    "type",
                    soundMetaDataProfile.Type
                }
            });
        }
        return jsonclass.ToString();
    }

    private static String soundEffectMetaData = "{data:[]}";

    private static String soundEffectExtendedMetaData = "{data:[]}";

    private static String musicMetaData = "{data:[]}";

    private static String movieAudioMetaData = "{data:[]}";

    private static String songMetaData = "{data:[]}";

    private static String sfxSoundMetaData = "{data:[]}";

    private static String residentSfxSoundMetaData = "{data:[]}";

    public static Dictionary<Int32, String> SoundEffectIndex = new Dictionary<Int32, String>();

    public static Dictionary<Int32, String> SoundEffectExtendedIndex = new Dictionary<Int32, String>();

    public static Dictionary<Int32, String> MusicIndex = new Dictionary<Int32, String>();

    public static Dictionary<Int32, String> MovieAudioIndex = new Dictionary<Int32, String>();

    public static Dictionary<Int32, String> SongIndex = new Dictionary<Int32, String>();

    public static Dictionary<Int32, List<String>> SfxSoundIndex = new Dictionary<Int32, List<String>>();

    public static Dictionary<Int32, List<String>> ResidentSfxSoundIndex = new Dictionary<Int32, List<String>>();

    public static Dictionary<String, Int32> MovieAudioStringIndex = new Dictionary<String, Int32>();

    private static Dictionary<SoundLibResourceLocation, SoundMetaData.LoadMetaDataDelegate> loadMetaDataDelegateDict = new Dictionary<SoundLibResourceLocation, SoundMetaData.LoadMetaDataDelegate>
    {
        {
            SoundLibResourceLocation.Resources,
            new SoundMetaData.LoadMetaDataDelegate(SoundMetaData.LoadMetaDataFromResources)
        },
        {
            SoundLibResourceLocation.ResourcesRecursive,
            new SoundMetaData.LoadMetaDataDelegate(SoundMetaData.LoadMetaDataFromResourcesRecursively)
        },
        {
            SoundLibResourceLocation.DocumentDirectory,
            new SoundMetaData.LoadMetaDataDelegate(SoundMetaData.LoadMetaDataFromDocumentDirectory)
        },
        {
            SoundLibResourceLocation.StreamingAsset,
            new SoundMetaData.LoadMetaDataDelegate(SoundMetaData.LoadMetaDataFromStreamingAssets)
        }
    };

    private delegate void LoadMetaDataDelegate();
}
