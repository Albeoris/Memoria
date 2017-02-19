using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class AssetManagerForObb
{
	public static void Initialize()
	{
		ExpansionVerifier.printLog("AssetManagerForObb : createfromfile fullPath = " + AssetManagerForObb.fullPath);
		AssetManagerForObb.obbAssetBundle = AssetBundle.CreateFromFile(AssetManagerForObb.fullPath);
		ExpansionVerifier.printLog("AssetManagerForObb : loadAnimationFolderMapping");
		AssetManagerForObb._LoadAnimationFolderMapping();
		ExpansionVerifier.printLog("AssetManagerForObb : isuseobb=true");
		AssetManagerForObb.IsUseOBB = true;
	}

	private static void _LoadAnimationFolderMapping()
	{
		AssetManagerForObb._animationInFolder = new Dictionary<String, List<String>>();
		TextAsset textAsset = AssetManager.Load<TextAsset>("EmbeddedAsset/Manifest/Animations/AnimationFolderMapping.txt", false);
		if (textAsset == (UnityEngine.Object)null)
		{
			global::Debug.LogWarning("Cannot load resource [AnimationFolderMapping]");
			ExpansionVerifier.printLog("AssetManagerForObb : Cannot load resource [AnimationFolderMapping]");
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
			AssetManagerForObb._animationInFolder.Add(text2, list);
		}
	}

	public static T Load<T>(String name, Boolean suppressError = false) where T : UnityEngine.Object
	{
		T t = Resources.Load<T>(name);
		if (t != (UnityEngine.Object)null)
		{
			return t;
		}
		String text = AssetManagerUtil.GetResourcesBasePath() + name;
		text += AssetManagerUtil.GetAssetExtension<T>(name);
		Boolean flag = name.IndexOf("atlas_a") != -1;
		if (AssetManagerForObb.obbAssetBundle != (UnityEngine.Object)null)
		{
			T t2 = AssetManagerForObb.obbAssetBundle.LoadAsset<T>(text);
			if (t2 != (UnityEngine.Object)null)
			{
				return t2;
			}
		}
		if (Application.platform != RuntimePlatform.Android && flag)
		{
			return (T)((Object)null);
		}
		if (!suppressError)
		{
			global::Debug.LogWarning("Cannot find " + name + " in bundles!!!");
		}
		return (T)((Object)null);
	}

	public static AssetManagerRequest LoadAsync<T>(String name) where T : UnityEngine.Object
	{
		if (AssetManagerUtil.IsEmbededAssets(name))
		{
			ResourceRequest resourceRequest = Resources.LoadAsync<T>(name);
			if (resourceRequest != null)
			{
				return new AssetManagerRequest(resourceRequest, (AssetBundleRequest)null);
			}
		}
		String text = AssetManagerUtil.GetResourcesBasePath() + name;
		text += AssetManagerUtil.GetAssetExtension<T>(name);
		Boolean flag = name.IndexOf("atlas_a") != -1;
		if (AssetManagerForObb.obbAssetBundle != (UnityEngine.Object)null)
		{
			AssetBundleRequest assetBundleRequest = AssetManagerForObb.obbAssetBundle.LoadAssetAsync(text);
			if (assetBundleRequest != null && assetBundleRequest.asset != (UnityEngine.Object)null)
			{
				return new AssetManagerRequest((ResourceRequest)null, assetBundleRequest);
			}
		}
		ResourceRequest resourceRequest2 = Resources.LoadAsync<T>(name);
		if (resourceRequest2 != null)
		{
			return new AssetManagerRequest(resourceRequest2, (AssetBundleRequest)null);
		}
		global::Debug.LogError("Cannot find " + name + " in bundles!!!");
		return (AssetManagerRequest)null;
	}

	public static T[] LoadAll<T>(String name) where T : UnityEngine.Object
	{
		if (typeof(T) != typeof(AnimationClip))
		{
			global::Debug.LogWarning("AssetManager::LoadAll<T>::Currently support only AnimationClip.");
			return null;
		}
		name = AnimationFactory.GetRenameAnimationDirectory(name);
		T[] array = Resources.LoadAll<T>(name);
		if (array != null)
		{
			return array;
		}
		if (AssetManagerForObb._animationInFolder.ContainsKey(name))
		{
			List<String> list = AssetManagerForObb._animationInFolder[name];
			T[] array2 = new T[list.Count];
			for (Int32 i = 0; i < list.Count; i++)
			{
				String renameAnimationPath = AnimationFactory.GetRenameAnimationPath(list[i]);
				array2[i] = AssetManagerForObb.Load<T>(renameAnimationPath, false);
			}
			return array2;
		}
		global::Debug.LogError("Cannot find " + name + " in bundles!!!");
		return null;
	}

	public static Boolean IsUseOBB;

	private static AssetBundle obbAssetBundle;

	private static String fullPath = String.Empty;

	private static Dictionary<String, List<String>> _animationInFolder;
}
