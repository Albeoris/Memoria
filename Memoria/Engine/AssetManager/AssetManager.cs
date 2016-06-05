using System;
using System.Collections.Generic;
using Memoria;
using UnityEngine;

// ReSharper disable UnassignedField.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedField.Global

[ExportedType("×čĖìńńńń&!!!rQmÐÅı&êJ·čĢá#rº)!!!Þòð¦Ôúéİã²ďħ¡¬Ċě¯ě%ÑÓ¨ĜSòÝM¶#!!!È2fÉńńńń%!!!ńÄ^ħþÑġºjðuČ#!!!è¥T-ńńńń")]
public static class AssetManager
{
    public static Boolean UseBundles;
    public static Boolean ForceUseBundles;
    public static Dictionary<String, AssetBundleRef> DictAssetBundleRefs;
    private static Dictionary<String, List<String>> _animationInFolder;

    static AssetManager()
    {
        DictAssetBundleRefs = new Dictionary<String, AssetBundleRef>();
        foreach (AssetManagerUtil.ModuleBundle moduleBundle in Enum.GetValues(typeof(AssetManagerUtil.ModuleBundle)))
        {
            switch (moduleBundle)
            {
                case AssetManagerUtil.ModuleBundle.FieldMaps:
                    CreateFieldMapsBundleEntries();
                    break;
                case AssetManagerUtil.ModuleBundle.Sounds:
                    CreateSoundBundleEntries();
                    break;
                default:
                    AddAssetBundleEntry(AssetManagerUtil.GetModuleBundleName(moduleBundle), 0);
                    break;
            }
        }

        LoadAnimationFolderMapping();
    }

    private static void CreateFieldMapsBundleEntries()
    {
        foreach (AssetManagerUtil.FieldMapBundleId id in Enum.GetValues(typeof(AssetManagerUtil.FieldMapBundleId)))
        {
            String name = AssetManagerUtil.GetFieldMapBundleName(id);
            AddAssetBundleEntry(name, 0);
        }
    }

    private static void CreateSoundBundleEntries()
    {
        foreach (AssetManagerUtil.SoundBundleId id in Enum.GetValues(typeof(AssetManagerUtil.SoundBundleId)))
        {
            String name = AssetManagerUtil.GetSoundBundleName(id);
            AddAssetBundleEntry(name, 0);
        }
    }

    private static void AddAssetBundleEntry(String url, Int32 version)
    {
        DictAssetBundleRefs.Add(url, new AssetBundleRef(url, version));
    }

    private static void LoadAnimationFolderMapping()
    {
        _animationInFolder = new Dictionary<String, List<String>>();
        TextAsset textAsset = Load<TextAsset>("EmbeddedAsset/Manifest/Animations/AnimationFolderMapping.txt", false);
        if (textAsset == null)
            return;

        String text = textAsset.text;
        foreach (String str1 in text.Split('\n'))
        {
            String[] strArray1 = str1.Split(':');
            String key = strArray1[0];
            String[] strArray2 = strArray1[1].Split(',');
            List<String> stringList = new List<String>();
            foreach (String str2 in strArray2)
            {
                String str3 = (key + "/" + str2).Trim();
                stringList.Add(str3);
            }
            _animationInFolder.Add(key, stringList);
        }
    }

    public static void ClearCache()
    {
        Caching.CleanCache();
        using (Dictionary<string, AssetBundleRef>.Enumerator enumerator = DictAssetBundleRefs.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                AssetBundleRef assetBundleRef = enumerator.Current.Value;
                if (assetBundleRef.assetBundle != null)
                {
                    assetBundleRef.assetBundle.Unload(true);
                    assetBundleRef.assetBundle = null;
                }
            }
        }
    }

    public static T Load<T>(String name, Boolean suppressError = false) where T : UnityEngine.Object
    {
        //if (!name.EndsWith(".mes"))
        //{
        //    Log.Message(typeof(T).Name + ": " + name);
        //    Log.Message(Environment.StackTrace);
        //}

        if (AssetManagerForObb.IsUseOBB)
            return AssetManagerForObb.Load<T>(name, suppressError);

        if (!UseBundles || AssetManagerUtil.IsEmbededAssets(name))
            return Resources.Load<T>(name);

        String belongingBundleFilename = AssetManagerUtil.GetBelongingBundleFilename(name);
        Boolean flag1 = AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.CommonAssets, name);
        if (!String.IsNullOrEmpty(belongingBundleFilename) && DictAssetBundleRefs.ContainsKey(belongingBundleFilename))
        {
            AssetBundleRef assetBundleRef = DictAssetBundleRefs[belongingBundleFilename];
            if (assetBundleRef.assetBundle != null)
            {
                String name1 = AssetManagerUtil.GetResourcesBasePath() + name + AssetManagerUtil.GetAssetExtension<T>(name);
                Boolean flag2 = name.IndexOf("atlas_a", StringComparison.Ordinal) != -1;
                T obj = assetBundleRef.assetBundle.LoadAsset<T>(name1);
                if (obj != null)
                    return obj;

                if (Application.platform != RuntimePlatform.Android && flag2)
                    return null;

                if (!flag1 && ForceUseBundles)
                    return null;
            }
        }

        if (ForceUseBundles)
            return null;

        T obj1 = Resources.Load<T>(name);
        return obj1;
    }

    public static AssetManagerRequest LoadAsync<T>(String name) where T : UnityEngine.Object
    {
        //if (!name.EndsWith(".mes"))
        //{
        //    Log.Message(typeof(T).Name + ": " + name);
        //    Log.Message(Environment.StackTrace);
        //}

        if (AssetManagerForObb.IsUseOBB)
            return AssetManagerForObb.LoadAsync<T>(name);

        if (!UseBundles || AssetManagerUtil.IsEmbededAssets(name))
        {
            ResourceRequest resReq = Resources.LoadAsync<T>(name);
            if (resReq != null)
                return new AssetManagerRequest(resReq, null);
        }

        String belongingBundleFilename = AssetManagerUtil.GetBelongingBundleFilename(name);
        Boolean flag1 = AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.CommonAssets, name);
        if (!String.IsNullOrEmpty(belongingBundleFilename) && DictAssetBundleRefs.ContainsKey(belongingBundleFilename))
        {
            AssetBundleRef assetBundleRef = DictAssetBundleRefs[belongingBundleFilename];
            if (assetBundleRef.assetBundle != null)
            {
                String name1 = AssetManagerUtil.GetResourcesBasePath() + name + AssetManagerUtil.GetAssetExtension<T>(name);
                //Boolean flag2 = name.IndexOf("atlas_a", StringComparison.Ordinal) != -1;
                AssetBundleRequest assReq = assetBundleRef.assetBundle.LoadAssetAsync(name1);
                if (assReq != null)
                    return new AssetManagerRequest(null, assReq);

                if (!flag1 && ForceUseBundles)
                {
                    //if (Application.platform != RuntimePlatform.Android && flag2)
                    //    return null;

                    return null;
                }
            }
        }

        ResourceRequest resReq1 = Resources.LoadAsync<T>(name);
        if (resReq1 != null)
            return new AssetManagerRequest(resReq1, null);

        return null;
    }

    public static T[] LoadAll<T>(String name) where T : UnityEngine.Object
    {
        //if (!name.EndsWith(".mes"))
        //{
        //    Log.Message(typeof(T).Name + ": " + name);
        //    Log.Message(Environment.StackTrace);
        //}

        if (AssetManagerForObb.IsUseOBB)
            return AssetManagerForObb.LoadAll<T>(name);

        if (typeof(T) != typeof(AnimationClip))
            return null;

        if (!UseBundles)
        {
            name = AnimationFactory.GetRenameAnimationDirectory(name);
            return Resources.LoadAll<T>(name);
        }

        if (!_animationInFolder.ContainsKey(name))
            return null;

        List<String> stringList = _animationInFolder[name];
        T[] objArray = new T[stringList.Count];
        for (Int32 index = 0; index < stringList.Count; ++index)
        {
            String renameAnimationPath = AnimationFactory.GetRenameAnimationPath(stringList[index]);
            objArray[index] = Load<T>(renameAnimationPath, false);
        }

        return objArray;
    }

    public class AssetBundleRef
    {
        public AssetBundle assetBundle;
        public Int32 version;
        public String url;

        public AssetBundleRef(String strUrlIn, Int32 intVersionIn)
        {
            url = strUrlIn;
            version = intVersionIn;
        }
    }
}