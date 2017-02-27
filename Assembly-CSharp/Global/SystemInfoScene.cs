using System;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.Common;
using UnityEngine;

public class SystemInfoScene : MonoBehaviour
{
	private void Start()
	{
		this.SystemInfoString = new String[32, 2];
		this.SystemInfoString[0, 0] = "deviceModel";
		this.SystemInfoString[0, 1] = "The model of the device (Read Only).";
		this.SystemInfoString[1, 0] = "deviceName";
		this.SystemInfoString[1, 1] = "The user defined name of the device (Read Only).";
		this.SystemInfoString[2, 0] = "deviceType";
		this.SystemInfoString[2, 1] = "Returns the kind of device the application is running on.";
		this.SystemInfoString[3, 0] = "deviceUniqueIdentifier";
		this.SystemInfoString[3, 1] = "A unique device identifier. It is guaranteed to be unique for every device (Read Only).";
		this.SystemInfoString[4, 0] = "graphicsDeviceID";
		this.SystemInfoString[4, 1] = "The identifier code of the graphics device (Read Only).";
		this.SystemInfoString[5, 0] = "graphicsDeviceName";
		this.SystemInfoString[5, 1] = "The name of the graphics device (Read Only).";
		this.SystemInfoString[6, 0] = "graphicsDeviceVendor";
		this.SystemInfoString[6, 1] = "The vendor of the graphics device (Read Only).";
		this.SystemInfoString[7, 0] = "graphicsDeviceVendorID";
		this.SystemInfoString[7, 1] = "The identifier code of the graphics device vendor (Read Only).";
		this.SystemInfoString[8, 0] = "graphicsDeviceVersion";
		this.SystemInfoString[8, 1] = "The graphics API version supported by the graphics device (Read Only).";
		this.SystemInfoString[9, 0] = "graphicsMemorySize";
		this.SystemInfoString[9, 1] = "Amount of video memory present (Read Only).";
		this.SystemInfoString[10, 0] = "graphicsPixelFillrate";
		this.SystemInfoString[10, 1] = "Approximate pixel fill-rate of the graphics device (Read Only).";
		this.SystemInfoString[11, 0] = "graphicsShaderLevel";
		this.SystemInfoString[11, 1] = "Graphics device shader capability level (Read Only).";
		this.SystemInfoString[12, 0] = "npotSupport";
		this.SystemInfoString[12, 1] = "What NPOT (ie, non-power of two resolution) support does the GPU provide? (Read Only)";
		this.SystemInfoString[13, 0] = "operatingSystem";
		this.SystemInfoString[13, 1] = "Operating system name with version (Read Only).";
		this.SystemInfoString[14, 0] = "processorCount";
		this.SystemInfoString[14, 1] = "Number of processors present (Read Only).";
		this.SystemInfoString[15, 0] = "processorType";
		this.SystemInfoString[15, 1] = "Processor name (Read Only).";
		this.SystemInfoString[16, 0] = "supportedRenderTargetCount";
		this.SystemInfoString[16, 1] = "How many simultaneous render targets (MRTs) are supported? (Read Only)";
		this.SystemInfoString[17, 0] = "supports3DTextures";
		this.SystemInfoString[17, 1] = "Are 3D (volume) textures supported? (Read Only)";
		this.SystemInfoString[18, 0] = "supportsAccelerometer";
		this.SystemInfoString[18, 1] = "Is an accelerometer available on the device?";
		this.SystemInfoString[19, 0] = "supportsComputeShaders";
		this.SystemInfoString[19, 1] = "Are compute shaders supported? (Read Only)";
		this.SystemInfoString[20, 0] = "supportsGyroscope";
		this.SystemInfoString[20, 1] = "Is a gyroscope available on the device?";
		this.SystemInfoString[21, 0] = "supportsImageEffects";
		this.SystemInfoString[21, 1] = "Are image effects supported? (Read Only)";
		this.SystemInfoString[22, 0] = "supportsInstancing";
		this.SystemInfoString[22, 1] = "Is GPU draw call instancing supported? (Read Only)";
		this.SystemInfoString[23, 0] = "supportsLocationService";
		this.SystemInfoString[23, 1] = "Is the device capable of reporting its location?";
		this.SystemInfoString[24, 0] = "supportsRenderTextures";
		this.SystemInfoString[24, 1] = "Are render textures supported? (Read Only)";
		this.SystemInfoString[25, 0] = "supportsRenderToCubemap";
		this.SystemInfoString[25, 1] = "Are cubemap render textures supported? (Read Only)";
		this.SystemInfoString[26, 0] = "supportsShadows";
		this.SystemInfoString[26, 1] = "Are built-in shadows supported? (Read Only)";
		this.SystemInfoString[27, 0] = "supportsSparseTextures";
		this.SystemInfoString[27, 1] = "Are sparse textures supported? (Read Only)";
		this.SystemInfoString[28, 0] = "supportsStencil";
		this.SystemInfoString[28, 1] = "Is the stencil buffer supported? (Read Only)";
		this.SystemInfoString[29, 0] = "supportsVibration";
		this.SystemInfoString[29, 1] = "Is the device capable of providing the user haptic feedback by vibration?";
		this.SystemInfoString[30, 0] = "systemMemorySize";
		this.SystemInfoString[30, 1] = "Amount of system memory present (Read Only).";
		this.SystemInfoString[31, 0] = "SupportsRenderTextureFormat";
		this.SystemInfoString[31, 1] = "Is render texture format supported?";
		this.ApplicationInfoString = new String[4, 2];
		this.ApplicationInfoString[0, 0] = "Application.dataPath";
		this.ApplicationInfoString[0, 1] = "(Read Only)\tpath to the game data folder";
		this.ApplicationInfoString[1, 0] = "Application.persistentDataPath";
		this.ApplicationInfoString[1, 1] = "(RW - Developer)\tpath to a persistent data directory";
		this.ApplicationInfoString[2, 0] = "AssetManagerUtil.GetStreamingAssetsPath()";
		this.ApplicationInfoString[2, 1] = "(Read Only)\tpath to the StreamingAssets folder on the target device";
		this.ApplicationInfoString[3, 0] = "Application.temporaryCachePath";
		this.ApplicationInfoString[3, 1] = "(RW by Unity Caching & Developer)\tpath to a temporary data / cache directory";
	}

	private void OnGUI()
	{
		DebugGuiSkin.ApplySkin();
		this.screenRect = DebugGuiSkin.GetFullscreenRect();
		GUILayout.BeginVertical("box", new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("back", new GUILayoutOption[0]))
		{
			global::Debug.Log("back");
			SceneDirector.Replace("MainMenu", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[]
		{
			GUILayout.Width(this.screenRect.width)
		});
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		GUILayout.Label(this.SystemInfoString[0, 0] + " : " + SystemInfo.deviceModel, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[1, 0] + " : " + SystemInfo.deviceName, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[2, 0] + " : " + SystemInfo.deviceType, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[4, 0] + " : " + SystemInfo.graphicsDeviceID, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[5, 0] + " : " + SystemInfo.graphicsDeviceName, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[6, 0] + " : " + SystemInfo.graphicsDeviceVendor, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[7, 0] + " : " + SystemInfo.graphicsDeviceVendorID, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[8, 0] + " : " + SystemInfo.graphicsDeviceVersion, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[9, 0] + " : " + SystemInfo.graphicsMemorySize, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[11, 0] + " : " + SystemInfo.graphicsShaderLevel, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[12, 0] + " : " + SystemInfo.npotSupport, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[13, 0] + " : " + SystemInfo.operatingSystem, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[14, 0] + " : " + SystemInfo.processorCount, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[15, 0] + " : " + SystemInfo.processorType, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[16, 0] + " : " + SystemInfo.supportedRenderTargetCount, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[17, 0] + " : " + SystemInfo.supports3DTextures, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[18, 0] + " : " + SystemInfo.supportsAccelerometer, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[19, 0] + " : " + SystemInfo.supportsComputeShaders, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[20, 0] + " : " + SystemInfo.supportsGyroscope, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[21, 0] + " : " + SystemInfo.supportsImageEffects, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[22, 0] + " : " + SystemInfo.supportsInstancing, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[23, 0] + " : " + SystemInfo.supportsLocationService, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[24, 0] + " : " + SystemInfo.supportsRenderTextures, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[25, 0] + " : " + SystemInfo.supportsRenderToCubemap, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[26, 0] + " : " + SystemInfo.supportsShadows, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[28, 0] + " : " + SystemInfo.supportsStencil, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[29, 0] + " : " + SystemInfo.supportsVibration, new GUILayoutOption[0]);
		GUILayout.Label(this.SystemInfoString[30, 0] + " : " + SystemInfo.systemMemorySize, new GUILayoutOption[0]);
		GUILayout.Label("------------------------", new GUILayoutOption[0]);
		GUILayout.Label(this.ApplicationInfoString[0, 0] + " : " + Application.dataPath, new GUILayoutOption[0]);
		GUILayout.Label(this.ApplicationInfoString[1, 0] + " : " + AssetManagerUtil.GetPersistentDataPath(), new GUILayoutOption[0]);
        GUILayout.Label(this.ApplicationInfoString[2, 0] + " : " + AssetManagerUtil.GetStreamingAssetsPath(), new GUILayoutOption[0]);
		GUILayout.Label(this.ApplicationInfoString[3, 0] + " : " + Application.temporaryCachePath, new GUILayoutOption[0]);
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
	}

	private void Update()
	{
	}

	private String[,] SystemInfoString;

	private String[,] ApplicationInfoString;

	private Rect screenRect;

	private Vector2 scrollPosition = new Vector2(0f, 0f);
}
