using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.Common;
using UnityEngine;
using Object = System.Object;

public class BundleScene : MonoBehaviour
{
	public static Int32 BundleVersionInt
	{
		get
		{
			if (BundleScene.bundleVersionInt == -999)
			{
				Int32 length = BundleScene.BundleVersion.IndexOf("_") - 3;
				String s = BundleScene.BundleVersion.Substring(3, length);
				BundleScene.bundleVersionInt = Int32.Parse(s);
			}
			return BundleScene.bundleVersionInt;
		}
	}

	private void Awake()
	{
		GameInitializer.Initial();
		this._logOutput = new StringBuilder();
		this._scrollPosition = new Vector2(0f, 0f);
		this._baseUrl = "http://10.1.0.144:8080/";
		this._isDownloading = false;
		this._isCompressedBundles = true;
		this._SetStatusText("Ready...");
		this._AddLogOutput("---------- LOG ----------");
		this._skipBundleScene = false;
		if (Application.platform == RuntimePlatform.Android && (AssetManagerForObb.IsUseOBB || FF9StateSystem.AndroidAmazonPlatform))
		{
			this._skipBundleScene = true;
		}
		this._skipBundleScene = true;
		ExpansionVerifier.printLog("BundleScene : awake _skipBundleScene = " + this._skipBundleScene);
		this._UsingUncompressedAssetBundlesFromLocal();
	}

	private void LateUpdate()
	{
		if (this._skipBundleScene && !this._isDownloading)
		{
			if (Application.platform == RuntimePlatform.Android && FF9StateSystem.AndroidAmazonPlatform)
			{
				UnityEngine.Debug.Log("calling _UsingAssetBundlesFromLocal() for Android Amazon");
				this._UsingAssetBundlesFromLocal();
				return;
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				this._OnDownloadFinished();
			}
			else
			{
				this._UsingUncompressedAssetBundlesFromLocal();
			}
			// Reverse order for first having priority; might move the patcher somewhere else; might use the default folder (AssetManager.Folder.Length-1) as well
			Memoria.Prime.Log.Message("[BundleScene] Patch dictionaries");
			for (Int32 i = AssetManager.Folder.Length-2; i >= 0; --i)
            {
				if (System.IO.File.Exists(AssetManager.Folder[i].FolderPath + AssetManager.MemoriaDictionaryPatcherPath))
                {
					String[] patch = System.IO.File.ReadAllLines(AssetManager.Folder[i].FolderPath + AssetManager.MemoriaDictionaryPatcherPath);
					AssetManager.PatchDictionaries(patch);
				}
            }
		}
	}

	private void OnGUI()
	{
		if (this._skipBundleScene)
		{
			return;
		}
		Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
		DebugGuiSkin.ApplySkin();
		TextAnchor alignment = GUI.skin.label.alignment;
		GUILayout.BeginArea(fullscreenRect);
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical("Box", new GUILayoutOption[0]);
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUILayout.Label("Asset Bundle Version : " + BundleScene.BundleVersion, new GUILayoutOption[0]);
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		Single width = fullscreenRect.width * 3f / 4f;
		Single height = fullscreenRect.height * 3f / 4f;
		GUIContent content = new GUIContent("SSKK Server");
		GUIStyle guistyle = new GUIStyle(GUI.skin.button);
		Single num = guistyle.CalcHeight(content, width);
		this._scrollPosition = GUILayout.BeginScrollView(this._scrollPosition, new GUILayoutOption[]
		{
			GUILayout.Width(width),
			GUILayout.Height(height)
		});
		if (!this._isDownloading)
		{
			if (GUILayout.Button("SSKK Server", new GUILayoutOption[]
			{
				GUILayout.Height(num * 2f)
			}))
			{
				this._UsingAssetBundlesFromSSKK();
			}
			if (GUILayout.Button("Clear Cache", new GUILayoutOption[]
			{
				GUILayout.Height(num * 2f)
			}))
			{
				this._ClearCache();
			}
			GUILayout.Label(" ", new GUILayoutOption[0]);
			if (Application.isEditor && GUILayout.Button("Disable Bundles", new GUILayoutOption[]
			{
				GUILayout.Height(num * 2.5f)
			}))
			{
				Int32 num2 = BundleScene.BundleVersionInt;
				UnityEngine.Debug.Log("version = " + num2);
				this._DisableAssetBundles();
			}
			if (GUILayout.Button("SST Server", new GUILayoutOption[]
			{
				GUILayout.Height(num * 2f)
			}))
			{
				this._UsingAssetBundlesFromSST();
			}
			if (GUILayout.Button("Local Bundles", new GUILayoutOption[0]))
			{
				this._UsingAssetBundlesFromLocal();
			}
			if (GUILayout.Button("Local UNC Bundles", new GUILayoutOption[0]))
			{
				this._UsingUncompressedAssetBundlesFromLocal();
			}
		}
		if (Application.isEditor)
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			AssetManager.ForceUseBundles = GUILayout.Toggle(AssetManager.ForceUseBundles, "[Forced to use Bundles]", new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
		GUILayout.Label(this._statusText, new GUILayoutOption[0]);
		GUILayout.Label("    =========================", new GUILayoutOption[0]);
		GUILayout.Label(this._logOutput.ToString(), new GUILayoutOption[0]);
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
		GUI.skin.label.alignment = alignment;
	}

	private void _SetStatusText(String msg)
	{
		this._statusText = "    " + msg;
	}

	private void _ClearLogOutput()
	{
		this._logOutput.Length = 0;
	}

	private void _AddLogOutput(String msg)
	{
		this._logOutput.AppendLine("    " + msg);
	}

	private void _DisableAssetBundles()
	{
		this._AddLogOutput("Run without AssetBundles.");
		AssetManager.UseBundles = false;
		AssetManager.ForceUseBundles = false;
		SceneDirector.ReplaceNow("MainMenu");
	}

	private void _UsingAssetBundlesFromLocal()
	{
		this._AddLogOutput("Downloading from Local StreamingAssets.");
		AssetManager.UseBundles = true;
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		{
			this._baseUrl = "file://";
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			this._baseUrl = "file://";
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			this._baseUrl = "";
		}
		this._isDownloading = true;
		this._isCompressedBundles = true;
		base.StartCoroutine(this.DownloadAssetBundles());
	}

	private void _UsingUncompressedAssetBundlesFromLocal()
	{
		this._AddLogOutput("Downloading from Local Uncompressed StreamingAssets.");
		ExpansionVerifier.printLog("BundleScene : Downloading from Local Uncompressed StreamingAssets.");
		AssetManager.UseBundles = true;
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		{
			this._baseUrl = "";
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			this._baseUrl = "";
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			this._baseUrl = "";
		}
		this._isDownloading = true;
		this._isCompressedBundles = false;
		ExpansionVerifier.printLog("BundleScene : _baseUrl = " + this._baseUrl);
		base.StartCoroutine(this.DownloadAssetBundles());
	}

	private void _UsingAssetBundlesFromSST()
	{
		this._AddLogOutput("Downloading from SST Server.");
		AssetManager.UseBundles = true;
		this._baseUrl = "http://10.1.0.144:8080/" + BundleScene.BundleVersion + "/";
		this._isDownloading = true;
		this._isCompressedBundles = true;
		base.StartCoroutine(this.DownloadAssetBundles());
	}

	private void _UsingAssetBundlesFromSSKK()
	{
		this._AddLogOutput("Downloading from SSKK Server.");
		AssetManager.UseBundles = true;
		this._baseUrl = "http://203.172.121.116:8080/" + BundleScene.BundleVersion + "/";
		this._isDownloading = true;
		this._isCompressedBundles = true;
		base.StartCoroutine(this.DownloadAssetBundles());
	}

	private void _ClearCache()
	{
		this._isDownloading = true;
		base.StartCoroutine(this.ClearCache());
	}

	private void _OnDownloadFinished()
	{
		ExpansionVerifier.printLog("BundleScene : change scene to MainMenu");
		SceneDirector.ReplaceNow("MainMenu");
	}

	public IEnumerator DownloadAssetBundles()
	{
		Screen.sleepTimeout = -1;
		foreach (AssetManager.AssetFolder modfold in AssetManager.Folder)
		{
			Int32 curItem = 0;
			Int32 numItem = modfold.DictAssetBundleRefs.Count;
			foreach (KeyValuePair<String, AssetManager.AssetBundleRef> entry in modfold.DictAssetBundleRefs)
			{
				AssetManager.AssetBundleRef abRef = entry.Value;
				if (abRef.assetBundle != (UnityEngine.Object)null)
				{
					break;
				}
				yield return base.StartCoroutine(this.DownloadAssetBundle(abRef, curItem, numItem));
				curItem++;
			}
		}
		this._SetStatusText("Load Completed");
		ExpansionVerifier.printLog("BundleScene : Load Completed");
		Screen.sleepTimeout = -2;
		this._OnDownloadFinished();
		yield return null;
		yield break;
	}

	public IEnumerator DownloadAssetBundle(AssetManager.AssetBundleRef abRef, Int32 curItem, Int32 numItem)
	{
		while (!Caching.ready)
		{
			yield return null;
		}
		this._AddLogOutput("Downloading : " + abRef.fullUrl);
		String fullUrl = String.Concat(new String[]
		{
			this._baseUrl,
			abRef.fullUrl
		});
		this._AddLogOutput("    At : " + fullUrl);
		ExpansionVerifier.printLog("BundleScene : Downloading " + abRef.fullUrl + " AT " + fullUrl);
		if (!this._isCompressedBundles && fullUrl.IndexOf("http://") == -1)
		{
			this._AddLogOutput("Loading Uncompressed Bundle.");
			AssetBundle ab = AssetBundle.CreateFromFile(fullUrl);
			abRef.assetBundle = ab;
			yield break;
		}
		using (WWW www = WWW.LoadFromCacheOrDownload(fullUrl, BundleScene.BundleVersionInt))
		{
			this._AddLogOutput("Loading Compressed Bundle.");
			while (!www.isDone)
			{
				this._SetStatusText(String.Concat(new Object[]
				{
					"Loading Progress : [",
					Mathf.FloorToInt(www.progress * 100f),
					"%] ",
					curItem,
					"/",
					numItem,
					" | Will auto change scene when finished."
				}));
				yield return new WaitForSeconds(0.01f);
			}
			if (www.error != null)
			{
				this._AddLogOutput("Error : " + www.error);
				ExpansionVerifier.printLog("BundleScene : www.error = " + www.error);
				throw new Exception("WWW download:" + www.error);
			}
			abRef.assetBundle = www.assetBundle;
		}
		yield break;
	}

	public IEnumerator ClearCache()
	{
		this._AddLogOutput("Clearing cache.");
		AssetManager.ClearCache();
		this._ClearLogOutput();
		Single curTime = Time.realtimeSinceStartup;
		Single percent = 0f;
		while (Time.realtimeSinceStartup - curTime < 5f)
		{
			percent = (Time.realtimeSinceStartup - curTime) / 5f;
			this._SetStatusText("Clearing Cache... " + percent.ToString("P2"));
			yield return new WaitForSeconds(0.3f);
		}
		this._SetStatusText("Ready...");
		this._AddLogOutput("---------- LOG ----------");
		this._isDownloading = false;
		yield break;
	}

	public const String SstServerPath = "http://10.1.0.144:8080/";

	public const String SskkServerPath = "http://203.172.121.116:8080/";

    public static string BundleVersion = "ver50_160720_1124";

    private static Int32 bundleVersionInt = -999;

	private String _statusText;

	private StringBuilder _logOutput;

	private Vector2 _scrollPosition;

	private String _baseUrl;

	private Boolean _isDownloading;

	private Boolean _isCompressedBundles;

	private Boolean _skipBundleScene;
}
