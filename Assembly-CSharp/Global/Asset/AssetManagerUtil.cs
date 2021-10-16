using System;
using UnityEngine;
using Object = System.Object;

public static class AssetManagerUtil
{
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
        {
            return string.Empty;
        }
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
		{
			return "p";
		}
		if (platform == AssetManagerUtil.AvailablePlatform.iOS)
		{
			return "i";
		}
		if (platform == AssetManagerUtil.AvailablePlatform.Android)
		{
			return "a";
		}
		if (platform == AssetManagerUtil.AvailablePlatform.aaaa)
		{
			return "v";
		}
		global::Debug.LogWarning("AssetManagerUtil::GetPlatformPrefix::Unknown platform.");
		return String.Empty;
	}

	public static String GetPlatformPrefix(RuntimePlatform platform)
	{
		if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
		{
			return "p";
		}
		if (platform == RuntimePlatform.IPhonePlayer)
		{
			return "i";
		}
		if (platform == RuntimePlatform.Android)
		{
			return "a";
		}
		global::Debug.LogWarning("AssetManagerUtil::GetPlatformPrefix::Unknown platform.");
		return String.Empty;
	}

	public static String GetCompressionPrefix(Boolean isCompressed)
	{
		if (isCompressed)
		{
			return "1";
		}
		return "0";
	}

	public static String GetModuleStartPath(AssetManagerUtil.ModuleBundle moduleBundle)
	{
		String result = String.Empty;
		switch (moduleBundle)
		{
		case AssetManagerUtil.ModuleBundle.FieldMaps:
			result = "FieldMaps/";
			break;
		case AssetManagerUtil.ModuleBundle.BattleMaps:
			result = "BattleMap/";
			break;
		case AssetManagerUtil.ModuleBundle.WorldMaps:
			result = "WorldMap/";
			break;
		case AssetManagerUtil.ModuleBundle.Models:
			result = "Models/";
			break;
		case AssetManagerUtil.ModuleBundle.Animations:
			result = "Animations/";
			break;
		case AssetManagerUtil.ModuleBundle.Sounds:
			result = "Sounds/";
			break;
		case AssetManagerUtil.ModuleBundle.CommonAssets:
			result = "CommonAsset/";
			break;
		default:
			global::Debug.LogWarning("AssetManagerUtil::GetModuleBasePath::Unknown module bundle.");
			break;
		}
		return result;
	}

	public static String GetModuleBasePath(AssetManagerUtil.ModuleBundle moduleBundle)
	{
		String result = String.Empty;
		switch (moduleBundle)
		{
		case AssetManagerUtil.ModuleBundle.FieldMaps:
			result = "Assets/Resources/FieldMaps/";
			break;
		case AssetManagerUtil.ModuleBundle.BattleMaps:
			result = "Assets/Resources/BattleMap/";
			break;
		case AssetManagerUtil.ModuleBundle.WorldMaps:
			result = "Assets/Resources/WorldMap/";
			break;
		case AssetManagerUtil.ModuleBundle.Models:
			result = "Assets/Resources/Models/";
			break;
		case AssetManagerUtil.ModuleBundle.Animations:
			result = "Assets/Resources/Animations/";
			break;
		case AssetManagerUtil.ModuleBundle.Sounds:
			result = "Assets/Resources/Sounds/";
			break;
		case AssetManagerUtil.ModuleBundle.CommonAssets:
			result = "Assets/Resources/CommonAsset/";
			break;
		default:
			global::Debug.LogWarning("AssetManagerUtil::GetModuleBasePath::Unknown module bundle.");
			break;
		}
		return result;
	}

	public static String GetBundledResourcesBasePath(AssetManagerUtil.ModuleBundle moduleBundle)
	{
		String result = String.Empty;
		switch (moduleBundle)
		{
		case AssetManagerUtil.ModuleBundle.FieldMaps:
			result = "BundledResources/FieldMaps/";
			break;
		case AssetManagerUtil.ModuleBundle.BattleMaps:
			result = "BundledResources/BattleMap/";
			break;
		case AssetManagerUtil.ModuleBundle.WorldMaps:
			result = "BundledResources/WorldMap/";
			break;
		case AssetManagerUtil.ModuleBundle.Models:
			result = "BundledResources/Models/";
			break;
		case AssetManagerUtil.ModuleBundle.Animations:
			result = "BundledResources/Animations/";
			break;
		case AssetManagerUtil.ModuleBundle.Sounds:
			result = "BundledResources/Sounds/";
			break;
		case AssetManagerUtil.ModuleBundle.CommonAssets:
			result = "BundledResources/CommonAsset/";
			break;
		default:
			global::Debug.LogWarning("AssetManagerUtil::GetBundledResourcesBasePath::Unknown module bundle.");
			break;
		}
		return result;
	}

	public static String GetModuleBundleName(AssetManagerUtil.ModuleBundle moduleBundle)
	{
		if (moduleBundle == AssetManagerUtil.ModuleBundle.FieldMaps)
		{
			return "data1";
		}
		if (moduleBundle == AssetManagerUtil.ModuleBundle.BattleMaps)
		{
			return "data2";
		}
		if (moduleBundle == AssetManagerUtil.ModuleBundle.WorldMaps)
		{
			return "data3";
		}
		if (moduleBundle == AssetManagerUtil.ModuleBundle.Models)
		{
			return "data4";
		}
		if (moduleBundle == AssetManagerUtil.ModuleBundle.Animations)
		{
			return "data5";
		}
		if (moduleBundle == AssetManagerUtil.ModuleBundle.Sounds)
		{
			return "data6";
		}
		if (moduleBundle == AssetManagerUtil.ModuleBundle.CommonAssets)
		{
			return "data7";
		}
		global::Debug.LogWarning("AssetManagerUtil::GetModuleBundleName::Unknown module!");
		return String.Empty;
	}

	public static Int32 GetFieldMapAreaCount()
	{
		return 56;
	}

	public static Int32 GetFieldMapAreaId(String fieldMapName)
	{
		Int32 num = fieldMapName.IndexOf("FBG_N");
		if (num != -1)
		{
			String s = fieldMapName.Substring(num + "FBG_N".Length, 2);
			Int32 result = 0;
			if (Int32.TryParse(s, out result))
			{
				return result;
			}
		}
		global::Debug.LogWarning("AssetManagerUtil::GetFieldMapAreaId::Unknown field map name " + fieldMapName);
		return -1;
	}

	public static Int32 GetFieldMapBundleId(String fieldMapName)
	{
		Int32 fieldMapAreaId = AssetManagerUtil.GetFieldMapAreaId(fieldMapName);
		if (fieldMapAreaId >= 44)
		{
			return 9;
		}
		if (fieldMapAreaId >= 39)
		{
			return 8;
		}
		if (fieldMapAreaId >= 32)
		{
			return 7;
		}
		if (fieldMapAreaId >= 27)
		{
			return 6;
		}
		if (fieldMapAreaId >= 19)
		{
			return 5;
		}
		if (fieldMapAreaId >= 13)
		{
			return 4;
		}
		if (fieldMapAreaId >= 10)
		{
			return 3;
		}
		if (fieldMapAreaId >= 3)
		{
			return 2;
		}
		if (fieldMapAreaId >= 0)
		{
			return 1;
		}
		global::Debug.LogWarning("AssetManagerUtil::GetFieldMapBundleId::Unknown field map name " + fieldMapName);
		return 1;
	}

	public static Int32 GetSoundCategoryId(String soundName)
	{
		Int32 num = soundName.IndexOf("Sounds/Sounds");
		if (num != -1)
		{
			String s = soundName.Substring(num + "Sounds/Sounds".Length, 2);
			Int32 result = -1;
			if (Int32.TryParse(s, out result))
			{
				return result;
			}
		}
		global::Debug.LogWarning("AssetManagerUtil::GetSoundCategoryId::Unknown sound name " + soundName);
		return -1;
	}

	public static Int32 GetSoundCategoryBundleId(String soundName)
	{
		Int32 soundCategoryId = AssetManagerUtil.GetSoundCategoryId(soundName);
		if (soundCategoryId == 1)
		{
			return 1;
		}
		if (soundCategoryId == 2)
		{
			return 2;
		}
		if (soundCategoryId == 3)
		{
			return 3;
		}
		global::Debug.LogWarning("AssetManagerUtil::GetSoundCategoryBundleId::Unknown sound name " + soundName);
		return -1;
	}

	public static String GetFieldMapBundleName(AssetManagerUtil.FieldMapBundleId bundleId)
	{
		return String.Format("{0}{1}", AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.FieldMaps), (Int32)bundleId);
	}

	public static String GetSoundBundleName(AssetManagerUtil.SoundBundleId bundleId)
	{
		return String.Format("{0}{1}", AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.Sounds), (Int32)bundleId);
	}

	public static String CreateFieldMapBundleFilename(RuntimePlatform buildTarget, Boolean isCompressed, AssetManagerUtil.FieldMapBundleId bundleId)
	{
		return String.Format("{0}{1}{2}{3}{4}", new Object[]
		{
			AssetManagerUtil.GetPlatformPrefix(buildTarget),
			AssetManagerUtil.GetCompressionPrefix(isCompressed),
			AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.FieldMaps),
			(Int32)bundleId,
			AssetManagerUtil.GetBundleExtension()
		});
	}

	public static String CreateSoundBundleFilename(RuntimePlatform buildTarget, Boolean isCompressed, AssetManagerUtil.SoundBundleId bundleId)
	{
		return String.Format("{0}{1}{2}{3}{4}", new Object[]
		{
			AssetManagerUtil.GetPlatformPrefix(buildTarget),
			AssetManagerUtil.GetCompressionPrefix(isCompressed),
			AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.Sounds),
			(Int32)bundleId,
			AssetManagerUtil.GetBundleExtension()
		});
	}

	public static String CreateModuleBundleFilename(RuntimePlatform buildTarget, Boolean isCompressed, AssetManagerUtil.ModuleBundle moduleBundle)
	{
		return String.Format("{0}{1}{2}{3}", new Object[]
		{
			AssetManagerUtil.GetPlatformPrefix(buildTarget),
			AssetManagerUtil.GetCompressionPrefix(isCompressed),
			AssetManagerUtil.GetModuleBundleName(moduleBundle),
			AssetManagerUtil.GetBundleExtension()
		});
	}

	public static String CreateObbBundleFilename(RuntimePlatform buildTarget)
	{
		return String.Format("{0}{1}{2}", AssetManagerUtil.GetPlatformPrefix(buildTarget), "OBB", AssetManagerUtil.GetBundleExtension());
	}

	public static Boolean CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle moduleBundle, String name)
	{
		String moduleStartPath = AssetManagerUtil.GetModuleStartPath(moduleBundle);
		return String.Compare(name, 0, moduleStartPath, 0, moduleStartPath.Length, StringComparison.OrdinalIgnoreCase) == 0;
	}

	public static String GetBelongingBundleFilename(String assetName)
	{
		String result = String.Empty;
		if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.FieldMaps, assetName))
		{
			Int32 fieldMapBundleId = AssetManagerUtil.GetFieldMapBundleId(assetName);
			result = AssetManagerUtil.GetFieldMapBundleName((AssetManagerUtil.FieldMapBundleId)fieldMapBundleId);
		}
		else if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.BattleMaps, assetName))
		{
			result = AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.BattleMaps);
		}
		else if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.WorldMaps, assetName))
		{
			result = AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.WorldMaps);
		}
		else if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.Models, assetName))
		{
			result = AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.Models);
		}
		else if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.Animations, assetName))
		{
			result = AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.Animations);
		}
		else if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.Sounds, assetName))
		{
			Int32 soundCategoryBundleId = AssetManagerUtil.GetSoundCategoryBundleId(assetName);
			if (soundCategoryBundleId != -1)
				result = AssetManagerUtil.GetSoundBundleName((AssetManagerUtil.SoundBundleId)soundCategoryBundleId);
			else
				result = AssetManagerUtil.GetSoundBundleName(AssetManagerUtil.SoundBundleId.Bundle_1);
		}
		else if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.CommonAssets, assetName))
		{
			result = AssetManagerUtil.GetModuleBundleName(AssetManagerUtil.ModuleBundle.CommonAssets);
		}
		return result;
	}

	public static Boolean IsEmbededAssets(String name)
	{
		String text = "EmbeddedAsset/";
		return String.Compare(name, 0, text, 0, text.Length, StringComparison.OrdinalIgnoreCase) == 0;
	}

	public static Boolean IsMemoriaAssets(String name)
	{
		String text = "StreamingAssets/Data/";
		return String.Compare(name, 0, text, 0, text.Length, StringComparison.OrdinalIgnoreCase) == 0;
	}

	public static String GetMemoriaAssetsPath()
	{
		return "StreamingAssets/Data/";
	}

	public static String GetAssetExtension<T>(String name) where T : UnityEngine.Object
	{
		String result = String.Empty;
		if (typeof(T) == typeof(TextAsset))
		{
			result = ".bytes";
		}
		else if (typeof(T) == typeof(Texture2D) || typeof(T) == typeof(Texture))
		{
			if (name.IndexOf("atlas_a") != -1)
			{
				result = ".jpg";
			}
			else
			{
				result = ".png";
			}
		}
		else if (typeof(T) == typeof(RenderTexture))
		{
			result = ".renderTexture";
		}
		else if (typeof(T) == typeof(Material))
		{
			result = ".mat";
		}
		else if (typeof(T) == typeof(AnimationClip))
		{
			result = ".anim";
		}
		else if (typeof(T) == typeof(GameObject))
		{
			if (AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.BattleMaps, name) || AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.Models, name))
			{
				result = ".fbx";
			}
			else
			{
				result = ".prefab";
			}
		}
		return result;
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
}
