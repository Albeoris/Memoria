using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public static class AssetManager
{
	static AssetManager()
	{
		Array values = Enum.GetValues(typeof(AssetManagerUtil.ModuleBundle));
		foreach (Object obj in values)
		{
			AssetManagerUtil.ModuleBundle moduleBundle = (AssetManagerUtil.ModuleBundle)((Int32)obj);
			if (moduleBundle == AssetManagerUtil.ModuleBundle.FieldMaps)
			{
				Array values2 = Enum.GetValues(typeof(AssetManagerUtil.FieldMapBundleId));
				foreach (Object obj2 in values2)
				{
					AssetManagerUtil.FieldMapBundleId bundleId = (AssetManagerUtil.FieldMapBundleId)((Int32)obj2);
					AssetManager._AddAssetBundleEntry(AssetManagerUtil.GetFieldMapBundleName(bundleId), 0);
				}
			}
			else if (moduleBundle == AssetManagerUtil.ModuleBundle.Sounds)
			{
				Array values3 = Enum.GetValues(typeof(AssetManagerUtil.SoundBundleId));
				foreach (Object obj3 in values3)
				{
					AssetManagerUtil.SoundBundleId bundleId2 = (AssetManagerUtil.SoundBundleId)((Int32)obj3);
					AssetManager._AddAssetBundleEntry(AssetManagerUtil.GetSoundBundleName(bundleId2), 0);
				}
			}
			else
			{
				AssetManager._AddAssetBundleEntry(AssetManagerUtil.GetModuleBundleName(moduleBundle), 0);
			}
		}
		AssetManager._LoadAnimationFolderMapping();
	}

	private static void _AddAssetBundleEntry(String url, Int32 version)
	{
		AssetManager.DictAssetBundleRefs.Add(url, new AssetManager.AssetBundleRef(url, version));
	}

	private static void _LoadAnimationFolderMapping()
	{
		AssetManager._animationInFolder = new Dictionary<String, List<String>>();
		TextAsset textAsset = AssetManager.Load<TextAsset>("EmbeddedAsset/Manifest/Animations/AnimationFolderMapping.txt", false);
		if (textAsset == (UnityEngine.Object)null)
		{
			return;
		}
		String[] array = textAsset.text.Split(new Char[]
		{
			'\n'
		});
		String[] array2 = array;
		for (Int32 i = 0; i < (Int32)array2.Length; i++)
		{
			String text = array2[i];
			String[] array3 = text.Split(new Char[]
			{
				':'
			});
			String text2 = array3[0];
			String[] array4 = array3[1].Split(new Char[]
			{
				','
			});
			List<String> list = new List<String>();
			String[] array5 = array4;
			for (Int32 j = 0; j < (Int32)array5.Length; j++)
			{
				String str = array5[j];
				String text3 = text2 + "/" + str;
				text3 = text3.Trim();
				list.Add(text3);
			}
			AssetManager._animationInFolder.Add(text2, list);
		}
	}

	public static void ClearCache()
	{
		Caching.CleanCache();
		foreach (KeyValuePair<String, AssetManager.AssetBundleRef> keyValuePair in AssetManager.DictAssetBundleRefs)
		{
			AssetManager.AssetBundleRef value = keyValuePair.Value;
			if (value.assetBundle != (UnityEngine.Object)null)
			{
				value.assetBundle.Unload(true);
				value.assetBundle = (AssetBundle)null;
			}
		}
	}

	public static T Load<T>(String name, Boolean suppressError = false) where T : UnityEngine.Object
	{
		if (AssetManagerForObb.IsUseOBB)
		{
			return AssetManagerForObb.Load<T>(name, suppressError);
		}
		if (!AssetManager.UseBundles || AssetManagerUtil.IsEmbededAssets(name))
		{
			return Resources.Load<T>(name);
		}
		String belongingBundleFilename = AssetManagerUtil.GetBelongingBundleFilename(name);
		Boolean flag = AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.CommonAssets, name);
		if (!String.IsNullOrEmpty(belongingBundleFilename) && AssetManager.DictAssetBundleRefs.ContainsKey(belongingBundleFilename))
		{
			AssetManager.AssetBundleRef assetBundleRef = AssetManager.DictAssetBundleRefs[belongingBundleFilename];
			if (assetBundleRef.assetBundle != (UnityEngine.Object)null)
			{
				String text = AssetManagerUtil.GetResourcesBasePath() + name;
				text += AssetManagerUtil.GetAssetExtension<T>(name);
				Boolean flag2 = name.IndexOf("atlas_a") != -1;
				T t = assetBundleRef.assetBundle.LoadAsset<T>(text);
				if (t != (UnityEngine.Object)null)
				{
					return t;
				}
				if (Application.platform != RuntimePlatform.Android && flag2)
				{
					return (T)((Object)null);
				}
				if (!flag && AssetManager.ForceUseBundles)
				{
					return (T)((Object)null);
				}
			}
		}
		if (AssetManager.ForceUseBundles)
		{
			return (T)((Object)null);
		}
		T t2 = Resources.Load<T>(name);
		if (t2 != (UnityEngine.Object)null)
		{
			return t2;
		}
		return (T)((Object)null);
	}

	public static AssetManagerRequest LoadAsync<T>(String name) where T : UnityEngine.Object
	{
		if (AssetManagerForObb.IsUseOBB)
		{
			return AssetManagerForObb.LoadAsync<T>(name);
		}
		if (!AssetManager.UseBundles || AssetManagerUtil.IsEmbededAssets(name))
		{
			ResourceRequest resourceRequest = Resources.LoadAsync<T>(name);
			if (resourceRequest != null)
			{
				return new AssetManagerRequest(resourceRequest, (AssetBundleRequest)null);
			}
		}
		String belongingBundleFilename = AssetManagerUtil.GetBelongingBundleFilename(name);
		Boolean flag = AssetManagerUtil.CheckModuleBundleFromName(AssetManagerUtil.ModuleBundle.CommonAssets, name);
		if (!String.IsNullOrEmpty(belongingBundleFilename) && AssetManager.DictAssetBundleRefs.ContainsKey(belongingBundleFilename))
		{
			AssetManager.AssetBundleRef assetBundleRef = AssetManager.DictAssetBundleRefs[belongingBundleFilename];
			if (assetBundleRef.assetBundle != (UnityEngine.Object)null)
			{
				String text = AssetManagerUtil.GetResourcesBasePath() + name;
				text += AssetManagerUtil.GetAssetExtension<T>(name);
				Boolean flag2 = name.IndexOf("atlas_a") != -1;
				AssetBundleRequest assetBundleRequest = assetBundleRef.assetBundle.LoadAssetAsync(text);
				if (assetBundleRequest != null)
				{
					return new AssetManagerRequest((ResourceRequest)null, assetBundleRequest);
				}
				if (!flag && AssetManager.ForceUseBundles)
				{
					if (Application.platform != RuntimePlatform.Android && flag2)
					{
						return (AssetManagerRequest)null;
					}
					return (AssetManagerRequest)null;
				}
			}
		}
		ResourceRequest resourceRequest2 = Resources.LoadAsync<T>(name);
		if (resourceRequest2 != null)
		{
			return new AssetManagerRequest(resourceRequest2, (AssetBundleRequest)null);
		}
		return (AssetManagerRequest)null;
	}

	public static T[] LoadAll<T>(String name) where T : UnityEngine.Object
	{
		if (AssetManagerForObb.IsUseOBB)
		{
			return AssetManagerForObb.LoadAll<T>(name);
		}
		if (typeof(T) != typeof(AnimationClip))
		{
			return null;
		}
		if (!AssetManager.UseBundles)
		{
			name = AnimationFactory.GetRenameAnimationDirectory(name);
			return Resources.LoadAll<T>(name);
		}
		if (AssetManager._animationInFolder.ContainsKey(name))
		{
			List<String> list = AssetManager._animationInFolder[name];
			T[] array = new T[list.Count];
			for (Int32 i = 0; i < list.Count; i++)
			{
				String renameAnimationPath = AnimationFactory.GetRenameAnimationPath(list[i]);
				array[i] = AssetManager.Load<T>(renameAnimationPath, false);
			}
			return array;
		}
		return null;
	}

	public static Boolean UseBundles;

	public static Boolean ForceUseBundles;

	public static Dictionary<String, AssetManager.AssetBundleRef> DictAssetBundleRefs = new Dictionary<String, AssetManager.AssetBundleRef>();

	private static Dictionary<String, List<String>> _animationInFolder;

	public class AssetBundleRef
	{
		public AssetBundleRef(String strUrlIn, Int32 intVersionIn)
		{
			this.url = strUrlIn;
			this.version = intVersionIn;
		}

		public AssetBundle assetBundle;

		public Int32 version;

		public String url;
	}
}
