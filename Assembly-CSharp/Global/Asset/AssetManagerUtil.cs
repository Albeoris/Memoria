using System;
using System.Collections.Generic;
using UnityEngine;

public static class AssetManagerUtil
{
    /// <summary>Destroy a texture that was loaded from disc. Warning: make sure it is not in use anymore when doing that.</summary>
    public static void FlushCustomTexture(String assetPath)
    {
        if (!AssetManagerUtil.CustomTextures.TryGetValue(assetPath, out Texture2D texture))
            return;
        AssetManagerUtil.CustomTextures.Remove(assetPath);
        UnityEngine.Object.Destroy(texture);
    }

    /// <summary>Destroy the textures loaded from disc. Warning: make sure all the textures are not in use anymore when doing that.</summary>
    public static void FlushAllCustomTextures()
    {
        foreach (Texture2D texture in AssetManagerUtil.CustomTextures.Values)
            UnityEngine.Object.Destroy(texture);
        AssetManagerUtil.CustomTextures.Clear();
    }

    public static String GetStreamingAssetsPath()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
            return "StreamingAssets";
        return Application.streamingAssetsPath;
    }

    public static String GetResourcesAssetsPath(Boolean shortVersion)
    {
        return shortVersion ? "FF9_Data" : "x64/FF9_Data";
    }

    public static string GetPersistentDataPath()
    {
        if (FF9StateSystem.PCEStorePlatform)
            return string.Empty;
        return Application.persistentDataPath;
    }

    public static String GetBundleExtension()
    {
        return ".bin";
    }

    public static String GetResourcesBasePath()
    {
        return "Assets/Resources/";
    }

    public static String GetPlatformPrefix(AssetManagerUtil.AvailablePlatform platform)
    {
        if (platform == AssetManagerUtil.AvailablePlatform.StandaloneWindows)
            return "p";
        if (platform == AssetManagerUtil.AvailablePlatform.iOS)
            return "i";
        if (platform == AssetManagerUtil.AvailablePlatform.Android)
            return "a";
        if (platform == AssetManagerUtil.AvailablePlatform.aaaa)
            return "v";
        global::Debug.LogWarning("AssetManagerUtil::GetPlatformPrefix::Unknown platform.");
        return String.Empty;
    }

    public static String GetPlatformPrefix(RuntimePlatform platform)
    {
        if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
            return "p";
        if (platform == RuntimePlatform.IPhonePlayer)
            return "i";
        if (platform == RuntimePlatform.Android)
            return "a";
        global::Debug.LogWarning("AssetManagerUtil::GetPlatformPrefix::Unknown platform.");
        return String.Empty;
    }

    public static String GetCompressionPrefix(Boolean isCompressed)
    {
        return isCompressed ? "1" : "0";
    }

    public static String GetModuleStartPath(AssetManagerUtil.ModuleBundle moduleBundle)
    {
        switch (moduleBundle)
        {
            case AssetManagerUtil.ModuleBundle.FieldMaps:
                return "FieldMaps/";
            case AssetManagerUtil.ModuleBundle.BattleMaps:
                return "BattleMap/";
            case AssetManagerUtil.ModuleBundle.WorldMaps:
                return "WorldMap/";
            case AssetManagerUtil.ModuleBundle.Models:
                return "Models/";
            case AssetManagerUtil.ModuleBundle.Animations:
                return "Animations/";
            case AssetManagerUtil.ModuleBundle.Sounds:
                return "Sounds/";
            case AssetManagerUtil.ModuleBundle.CommonAssets:
                return "CommonAsset/";
        }
        global::Debug.LogWarning("AssetManagerUtil::GetModuleBasePath::Unknown module bundle.");
        return String.Empty;
    }

    public static String GetModuleBasePath(AssetManagerUtil.ModuleBundle moduleBundle)
    {
        switch (moduleBundle)
        {
            case AssetManagerUtil.ModuleBundle.FieldMaps:
                return "Assets/Resources/FieldMaps/";
            case AssetManagerUtil.ModuleBundle.BattleMaps:
                return "Assets/Resources/BattleMap/";
            case AssetManagerUtil.ModuleBundle.WorldMaps:
                return "Assets/Resources/WorldMap/";
            case AssetManagerUtil.ModuleBundle.Models:
                return "Assets/Resources/Models/";
            case AssetManagerUtil.ModuleBundle.Animations:
                return "Assets/Resources/Animations/";
            case AssetManagerUtil.ModuleBundle.Sounds:
                return "Assets/Resources/Sounds/";
            case AssetManagerUtil.ModuleBundle.CommonAssets:
                return "Assets/Resources/CommonAsset/";
        }
        global::Debug.LogWarning("AssetManagerUtil::GetModuleBasePath::Unknown module bundle.");
        return String.Empty;
    }

    public static String GetBundledResourcesBasePath(AssetManagerUtil.ModuleBundle moduleBundle)
    {
        switch (moduleBundle)
        {
            case AssetManagerUtil.ModuleBundle.FieldMaps:
                return "BundledResources/FieldMaps/";
            case AssetManagerUtil.ModuleBundle.BattleMaps:
                return "BundledResources/BattleMap/";
            case AssetManagerUtil.ModuleBundle.WorldMaps:
                return "BundledResources/WorldMap/";
            case AssetManagerUtil.ModuleBundle.Models:
                return "BundledResources/Models/";
            case AssetManagerUtil.ModuleBundle.Animations:
                return "BundledResources/Animations/";
            case AssetManagerUtil.ModuleBundle.Sounds:
                return "BundledResources/Sounds/";
            case AssetManagerUtil.ModuleBundle.CommonAssets:
                return "BundledResources/CommonAsset/";
        }
        global::Debug.LogWarning("AssetManagerUtil::GetBundledResourcesBasePath::Unknown module bundle.");
        return String.Empty;
    }

    public static String GetModuleBundleName(AssetManagerUtil.ModuleBundle moduleBundle)
    {
        if (moduleBundle == AssetManagerUtil.ModuleBundle.FieldMaps)
            return "data1";
        if (moduleBundle == AssetManagerUtil.ModuleBundle.BattleMaps)
            return "data2";
        if (moduleBundle == AssetManagerUtil.ModuleBundle.WorldMaps)
            return "data3";
        if (moduleBundle == AssetManagerUtil.ModuleBundle.Models)
            return "data4";
        if (moduleBundle == AssetManagerUtil.ModuleBundle.Animations)
            return "data5";
        if (moduleBundle == AssetManagerUtil.ModuleBundle.Sounds)
            return "data6";
        if (moduleBundle == AssetManagerUtil.ModuleBundle.CommonAssets)
            return "data7";
        global::Debug.LogWarning("AssetManagerUtil::GetModuleBundleName::Unknown module!");
        return String.Empty;
    }

    public static Int32 GetFieldMapAreaCount()
    {
        return 56;
    }

    public static Int32 GetFieldMapAreaId(String fieldMapName)
    {
        Int32 fbgpos = fieldMapName.IndexOf("FBG_N", StringComparison.OrdinalIgnoreCase);
        if (fbgpos != -1)
        {
            String s = fieldMapName.Substring(fbgpos + "FBG_N".Length, 2);
            if (Int32.TryParse(s, out Int32 result))
                return result;
        }
        global::Debug.LogWarning("AssetManagerUtil::GetFieldMapAreaId::Unknown field map name " + fieldMapName);
        return -1;
    }

    public static Int32 GetFieldMapBundleId(String fieldMapName)
    {
        Int32 fieldMapAreaId = AssetManagerUtil.GetFieldMapAreaId(fieldMapName);
        if (fieldMapAreaId >= 44)
            return 9;
        if (fieldMapAreaId >= 39)
            return 8;
        if (fieldMapAreaId >= 32)
            return 7;
        if (fieldMapAreaId >= 27)
            return 6;
        if (fieldMapAreaId >= 19)
            return 5;
        if (fieldMapAreaId >= 13)
            return 4;
        if (fieldMapAreaId >= 10)
            return 3;
        if (fieldMapAreaId >= 3)
            return 2;
        if (fieldMapAreaId >= 0)
            return 1;
        global::Debug.LogWarning("AssetManagerUtil::GetFieldMapBundleId::Unknown field map name " + fieldMapName);
        return 1;
    }

    public static Int32 GetSoundCategoryId(String soundName)
    {
        Int32 sndpos = soundName.IndexOf("Sounds/Sounds");
        if (sndpos != -1)
        {
            String cat = soundName.Substring(sndpos + "Sounds/Sounds".Length, 2);
            if (Int32.TryParse(cat, out Int32 result))
                return result;
        }
        global::Debug.LogWarning("AssetManagerUtil::GetSoundCategoryId::Unknown sound name " + soundName);
        return -1;
    }

    public static Int32 GetSoundCategoryBundleId(String soundName)
    {
        Int32 soundCategoryId = AssetManagerUtil.GetSoundCategoryId(soundName);
        if (soundCategoryId == 1)
            return 1;
        if (soundCategoryId == 2)
            return 2;
        if (soundCategoryId == 3)
            return 3;
        global::Debug.LogWarning("AssetManagerUtil::GetSoundCategoryBundleId::Unknown sound name " + soundName);
        return -1;
    }

    public static String GetFieldMapBundleName(AssetManagerUtil.FieldMapBundleId bundleId)
    {
        return AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.FieldMaps) + (Int32)bundleId;
    }

    public static String GetSoundBundleName(AssetManagerUtil.SoundBundleId bundleId)
    {
        return AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.Sounds) + (Int32)bundleId;
    }

    public static String CreateFieldMapBundleFilename(RuntimePlatform buildTarget, Boolean isCompressed, AssetManagerUtil.FieldMapBundleId bundleId)
    {
        return AssetManagerUtil.GetPlatformPrefix(buildTarget)
             + AssetManagerUtil.GetCompressionPrefix(isCompressed)
             + AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.FieldMaps)
             + (Int32)bundleId
             + AssetManagerUtil.GetBundleExtension();
    }

    public static String CreateSoundBundleFilename(RuntimePlatform buildTarget, Boolean isCompressed, AssetManagerUtil.SoundBundleId bundleId)
    {
        return AssetManagerUtil.GetPlatformPrefix(buildTarget)
             + AssetManagerUtil.GetCompressionPrefix(isCompressed)
             + AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.Sounds)
             + (Int32)bundleId
             + AssetManagerUtil.GetBundleExtension();
    }

    public static String CreateModuleBundleFilename(RuntimePlatform buildTarget, Boolean isCompressed, AssetManagerUtil.ModuleBundle moduleBundle)
    {
        return AssetManagerUtil.GetPlatformPrefix(buildTarget)
             + AssetManagerUtil.GetCompressionPrefix(isCompressed)
             + AssetManagerUtil.GetModuleBundleName(moduleBundle)
             + AssetManagerUtil.GetBundleExtension();
    }

    public static String CreateObbBundleFilename(RuntimePlatform buildTarget)
    {
        return AssetManagerUtil.GetPlatformPrefix(buildTarget) + "OBB" + AssetManagerUtil.GetBundleExtension();
    }

    public static Boolean CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle moduleBundle, String name)
    {
        String moduleStartPath = AssetManagerUtil.GetModuleStartPath(moduleBundle);
        return String.Compare(name, 0, moduleStartPath, 0, moduleStartPath.Length, StringComparison.OrdinalIgnoreCase) == 0;
    }

    public static String GetBelongingBundleFilename(String assetName)
    {
        if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.FieldMaps, assetName))
        {
            Int32 fieldMapBundleId = AssetManagerUtil.GetFieldMapBundleId(assetName);
            return AssetManagerUtil.GetFieldMapBundleName((AssetManagerUtil.FieldMapBundleId)fieldMapBundleId);
        }
        if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.BattleMaps, assetName))
            return AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.BattleMaps);
        if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.WorldMaps, assetName))
            return AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.WorldMaps);
        if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.Models, assetName))
            return AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.Models);
        if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.Animations, assetName))
            return AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.Animations);
        if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.Sounds, assetName))
        {
            Int32 soundCategoryBundleId = AssetManagerUtil.GetSoundCategoryBundleId(assetName);
            if (soundCategoryBundleId != -1)
                return AssetManagerUtil.GetSoundBundleName((AssetManagerUtil.SoundBundleId)soundCategoryBundleId);
            else
                return AssetManagerUtil.GetSoundBundleName(AssetManagerUtil.SoundBundleId.Bundle_1);
        }
        if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.CommonAssets, assetName))
            return AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.CommonAssets);
        return String.Empty;
    }

    public static Boolean IsEmbededAssets(String name)
    {
        const String text = "EmbeddedAsset/";
        return String.Compare(name, 0, text, 0, text.Length, StringComparison.OrdinalIgnoreCase) == 0;
    }

    public static Boolean IsMemoriaAssets(String name)
    {
        const String text = "Data/";
        return String.Compare(name, 0, text, 0, text.Length, StringComparison.OrdinalIgnoreCase) == 0;
    }

    public static String GetAssetExtension<T>(String name) where T : UnityEngine.Object
    {
        if (typeof(T) == typeof(TextAsset))
            return ".bytes";
        if (typeof(T) == typeof(Texture2D) || typeof(T) == typeof(Texture))
        {
            if (name.IndexOf("atlas_a") != -1)
                return ".jpg";
            else
                return ".png";
        }
        if (typeof(T) == typeof(RenderTexture))
            return ".renderTexture";
        if (typeof(T) == typeof(Material))
            return ".mat";
        if (typeof(T) == typeof(AnimationClip))
            return ".anim";
        if (typeof(T) == typeof(GameObject))
        {
            if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.BattleMaps, name) || AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.Models, name))
                return ".fbx";
            else
                return ".prefab";
        }
        return String.Empty;
    }

    public enum AvailablePlatform // Not very interesting... use RuntimePlatform (Application.platform) instead
    {
        StandaloneWindows = 5,
        iOS = 9,
        Android = 13,
        aaaa = 30
    }

    public enum ModuleBundle
    {
        FieldMaps,
        BattleMaps,
        WorldMaps,
        Models,
        Animations,
        Sounds,
        CommonAssets
    }

    public enum FieldMapBundleId
    {
        Bundle_1 = 1,
        Bundle_2,
        Bundle_3,
        Bundle_4,
        Bundle_5,
        Bundle_6,
        Bundle_7,
        Bundle_8,
        Bundle_9
    }

    public enum SoundBundleId
    {
        Bundle_1 = 1,
        Bundle_2,
        Bundle_3
    }

    public static Dictionary<String, Texture2D> CustomTextures = new Dictionary<String, Texture2D>();
}
