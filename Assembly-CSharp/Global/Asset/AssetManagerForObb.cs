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
		String filestr = AssetManager.LoadString("EmbeddedAsset/Manifest/Animations/AnimationFolderMapping.txt");
		if (filestr == null)
		{
			global::Debug.LogWarning("Cannot load resource [AnimationFolderMapping]");
			ExpansionVerifier.printLog("AssetManagerForObb : Cannot load resource [AnimationFolderMapping]");
			return;
		}
		String[] array = filestr.Split(new Char[]
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

	public static T Load<T>(String name) where T : UnityEngine.Object
	{
		T asset = Resources.Load<T>(name);
		if (asset != null)
			return asset;
		String path = AssetManagerUtil.GetResourcesBasePath() + name;
		path += AssetManagerUtil.GetAssetExtension<T>(name);
		if (AssetManagerForObb.obbAssetBundle != null)
		{
			asset = AssetManagerForObb.obbAssetBundle.LoadAsset<T>(path);
			if (asset != null)
				return asset;
		}
		Boolean isAtlas = name.IndexOf("atlas_a") != -1;
		if (Application.platform != RuntimePlatform.Android && isAtlas)
			return null;
		return null;
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
		T[] assets = Resources.LoadAll<T>(name);
		if (assets != null)
			return assets;
		if (AssetManagerForObb._animationInFolder.ContainsKey(name))
		{
			List<String> animList = AssetManagerForObb._animationInFolder[name];
			assets = new T[animList.Count];
			for (Int32 i = 0; i < animList.Count; i++)
			{
				String renameAnimationPath = AnimationFactory.GetRenameAnimationPath(animList[i]);
				assets[i] = AssetManagerForObb.Load<T>(renameAnimationPath);
			}
			return assets;
		}
		global::Debug.LogError("Cannot find " + name + " in bundles!!!");
		return null;
	}

	public static Boolean IsUseOBB;

	private static AssetBundle obbAssetBundle;

	private static String fullPath = String.Empty;

	private static Dictionary<String, List<String>> _animationInFolder;
}
