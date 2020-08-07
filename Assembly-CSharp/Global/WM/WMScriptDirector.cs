using System;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class WMScriptDirector : HonoBehavior
{
	public static WMScriptDirector Instance
	{
		get
		{
			if (WMScriptDirector.instance == (UnityEngine.Object)null)
			{
				WMScriptDirector.instance = (UnityEngine.Object.FindObjectOfType<WMScriptDirector>() ?? new GameObject(typeof(WMScriptDirector).Name).AddComponent<WMScriptDirector>());
			}
			return WMScriptDirector.instance;
		}
	}

	public override void HonoAwake()
	{
		global::Debug.Log("WMScriptDirector.HonoAwake(): This should be called first of all things in WorldMap, than other WM* signletons.");
		if (!FF9StateSystem.World.IsBeeScene)
		{
			EMinigame.InitializeAllTreasureAchievement();
			EMinigame.InitializeAllSandyBeachAchievement();
		}
		ff9.w_moveDummyCharacter = new GameObject("moveDummy").AddComponent<WMActor>();
		ff9.w_moveDummyCharacter.transform.parent = Singleton<WMWorld>.Instance.WorldMapRoot;
		ff9.w_moveDummyCharacter.originalActor = new Actor();
		PersistenSingleton<HonoInputManager>.Instance.IgnoreCheckingDirectionSources = true;
		Transform transform = GameObject.Find("Bee").transform;
		UnityEngine.Object.Destroy(transform.gameObject);
		Application.targetFrameRate = 20;
        Singleton<WMWorld>.Instance.Initialize();
		this.World = Singleton<WMWorld>.Instance;
		this.World.ClipDistance = 300000;
		this.World.Settings.WrapWorld = true;
		this.World.OnInitialize();
		if (FF9StateSystem.World.IsBeeScene)
		{
			Singleton<WMAnimationBank>.Instance.Initialize();
			WMActor.Initialize();
			Singleton<WMBeeMovementAnimation>.Instance.Initialize();
		}
		PersistenSingleton<FF9StateSystem>.Instance.mode = 3;
		this.FF9 = FF9StateSystem.Common.FF9;
		this.FF9Sys = PersistenSingleton<FF9StateSystem>.Instance;
		this.FF9World = FF9StateSystem.World.FF9World;
		this.FF9WorldMap = FF9StateSystem.World.FF9World.map;
		this.FF9WorldMap.nextMode = 3;
		ff9.ff9InitStateWorldMap((Int32)this.FF9.wldMapNo);
		base.StartCoroutine(PersistenSingleton<FF9TextTool>.Instance.UpdateFieldText(68));
		if (!FF9StateSystem.World.IsBeeScene)
		{
			PersistenSingleton<EventEngine>.Instance.ServiceEvents();
		}
		ff9.w_frameSystemConstructor();
		if (!FF9StateSystem.World.IsBeeScene)
		{
			ff9.w_frameMapConstructor();
		}
		if (FF9StateSystem.World.IsBeeScene)
		{
			this.BeeMovementAnimation = Singleton<WMBeeMovementAnimation>.Instance;
		}
		this.RenderTextureBank = Singleton<WMRenderTextureBank>.Instance;
		RenderSettings.fog = true;
	}

	public void CreateDebugObjects()
	{
		Transform transform = GameObject.Find("Bee").transform;
		if (FF9StateSystem.World.CreateHonoUpsDisplay)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("EmbeddedAsset/WorldMap_Local/Prefabs/HonoUPSDisplay"));
			gameObject.transform.parent = transform;
			this.HonoUpsText = gameObject.GetComponent<GUIText>();
			if (!this.UpdateHonoUpsText)
			{
				this.HonoUpsText.text = String.Empty;
			}
		}
		if (FF9StateSystem.World.CreateMonoUpsDisplay)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("EmbeddedAsset/WorldMap_Local/Prefabs/MonoUPSDisplay"));
			gameObject2.transform.parent = transform;
			this.MonoUpsText = gameObject2.GetComponent<GUIText>();
			if (!this.UpdateMonoUpsText)
			{
				this.MonoUpsText.text = String.Empty;
			}
		}
		if (FF9StateSystem.World.CreateFpsDisplay)
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("EmbeddedAsset/WorldMap_Local/Prefabs/FPSDisplay"));
			gameObject3.transform.parent = transform;
		}
	}

	public override void HonoUpdate()
	{
		if (this.World.LoadState == WMWorldLoadState.Initializing)
		{
			if (ff9.GetControlChar() != (UnityEngine.Object)null)
			{
				ff9.w_moveActorPtr = ff9.GetControlChar();
			}
			return;
		}
		if (this.World.LoadState == WMWorldLoadState.LoadingTheRestOfBlocksInBackground)
		{
			this.World.OnUpdateLoading();
		}
		this.World.OnUpdate();
		if (FF9StateSystem.World.IsBeeScene)
		{
			this.BeeMovementAnimation.OnUpdate();
		}
		if (HonoBehaviorSystem.Instance.IsFastForwardModeActive())
		{
			if (this.fastForwardFrameCounter == 0)
			{
				ff9.kPadPush.CollectInput();
			}
		}
		else
		{
			ff9.kPadPush.CollectInput();
		}
		this.honoCumulativeTime += Time.deltaTime;
		if (ff9.w_frameCounter < ff9.kframeEventStartLoop + 1)
		{
			this.HonoUpdate20FPS();
		}
		else if (this.honoCumulativeTime >= 0.05f)
		{
			this.HonoUpdate20FPS();
			this.honoFrameCounter++;
			this.honoCumulativeTime -= 0.05f;
			ff9.kPadPush.PurgeInput();
		}
		if (HonoBehaviorSystem.Instance.IsFastForwardModeActive())
		{
			this.fastForwardFrameCounter++;
			this.fastForwardFrameCounter %= HonoBehaviorSystem.Instance.GetFastForwardFactor();
		}
		WMGizmos.DrawFrustum(Singleton<WMWorld>.Instance.MainCamera);
	}

	public void HonoUpdate20FPS()
	{
		bool flag;
		if ((this.FF9.attr & 256U) == 0U)
		{
			if (ff9.w_frameCounter >= 5)
			{
				UIManager.World.SetPerspectiveToggle(ff9.w_cameraSysDataCamera.upperCounter == 4096);
				UIManager.World.SetRotationLockToggle(!ff9.w_cameraSysData.cameraNotrot);
			}

			if (this.FF9WorldMap.nextMode == 2)
			{
				flag = false;
			}
			else
			{
				flag = true;
				switch (ff9.w_frameMainRoutine())
				{
					case 3:
						this.FF9WorldMap.nextMode = 2;
						ff9.ff9worldInternalBattleEncountStart();
						PersistenSingleton<HonoInputManager>.Instance.IgnoreCheckingDirectionSources = false;
						break;
					case 4:
						this.FF9WorldMap.nextMode = 1;
						PersistenSingleton<HonoInputManager>.Instance.IgnoreCheckingDirectionSources = false;
						this.FF9Sys.attr |= 4096U;
						break;
				}
			}

			this.RenderTextureBank.OnUpdate20FPS();
			if ((this.FF9World.attr & 512U) == 0U)
			{
				SceneDirector.ServiceFade();
			}

			if ((this.FF9World.attr & 1024U) == 0U)
			{
				ff9.ff9worldInternalBattleEncountService();
			}

			if (HonoBehaviorSystem.Instance.IsFastForwardModeActive())
			{
				this.fastForwardFrameCounter20FPS++;
				if (this.fastForwardFrameCounter20FPS == HonoBehaviorSystem.Instance.GetFastForwardFactor())
				{
					this.UpdateTexture_Render();
					this.fastForwardFrameCounter20FPS = 0;
				}
			}
			else
			{
				this.UpdateTexture_Render();
			}
		}
		else
		{
			flag = false;
		}

		if ((this.FF9Sys.attr & 12289U) != 0U || (this.FF9Sys.attr & 4097U) != 0U)
		{
			ff9.ff9ShutdownStateWorldMap();
			ff9.ff9ShutdownStateWorldSystem();
			if (this.FF9Sys.mode == 1)
			{
				AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
				allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_STOPCURRENT();
				allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP_ALL(null);
				allSoundDispatchPlayer.FF9SOUND_STREAM_STOP();
				SceneDirector.Replace("FieldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
			}
			else if (this.FF9Sys.mode == 2)
			{
				EventEngine eventEngine = PersistenSingleton<EventEngine>.Instance;
				Obj objUID = eventEngine.GetObjUID(250);
				PosObj posObj = (PosObj) objUID;
				EventInput.IsProcessingInput = false;
				if (flag)
				{
					SoundLib.StopAllSounds(false);
					SFX_Rush.SetCenterPosition(1);
					SceneDirector.Replace("BattleMap", SceneTransition.SwirlInBlack, true);
				}
			}
			else if (this.FF9Sys.mode == 3)
			{
				SoundLib.StopAllSounds(true);
				SceneDirector.Replace("WorldMap", SceneTransition.FadeOutToBlack_FadeIn, true);
			}
		}

		if (!FF9StateSystem.World.IsBeeScene)
		{
			for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
			{
				Obj obj = objList.obj;
				if (PersistenSingleton<EventEngine>.Instance.isPosObj(obj))
				{
					WMActor wmActor = ((Actor) obj).wmActor;
					if (obj.cid == 4 && wmActor != null)
					{
						wmActor.UpdateAnimationViaScript();
					}
				}
			}
		}

		for (ObjList objList2 = this.World.ActorList; objList2 != null; objList2 = objList2.next)
		{
			if (objList2.obj.cid == 4)
			{
				WMActor wmActor2 = ((Actor) objList2.obj).wmActor;
				wmActor2.LateUpdateFunction();
			}
		}
	}

	private void UpdateTexture_Render()
	{
		this.RenderTextureBank.UpdateBeach1_Render();
		this.RenderTextureBank.UpdateBeach2_Render();
		this.RenderTextureBank.UpdateSea_10_64_0_Render();
		this.RenderTextureBank.UpdateSea_10_128_0_Render();
		this.RenderTextureBank.UpdateSea_10_128_64_Render();
		this.RenderTextureBank.UpdateSea_10_128_128_Render();
		this.RenderTextureBank.UpdateSea_11_64_0_Render();
		this.RenderTextureBank.UpdateSea_11_192_64_Render();
		this.RenderTextureBank.UpdateRiver_Render();
		this.RenderTextureBank.UpdateRiverJoint_Render();
	}

	private void OnUpdate20FPS()
	{
	}

	private void UpdateCheckingLowEndAndroidDevices()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		GlobalFog globalFog = ff9.world.GlobalFog;
		if (!globalFog.enabled)
		{
			FF9StateSystem.World.HasLowFpsOnDevice = true;
		}
		if (FF9StateSystem.World.HasLowFpsOnDevice)
		{
			this.OnLowFps();
			return;
		}
	}

	private void OnLowFps()
	{
		GlobalFog globalFog = ff9.world.GlobalFog;
		globalFog.enabled = false;
		ff9.world.Settings.EnableFog = true;
		RenderSettings.fog = true;
	}

	private void Update()
	{
		this.monoCumulativeTime += Time.deltaTime;
		if (this.monoCumulativeTime >= 0.05f)
		{
			this.OnUpdate20FPS();
			this.monoFrameCounter++;
			this.monoCumulativeTime -= 0.05f;
		}
		Single time = Time.time;
		if (time >= this.honoUpdateUpsTextTime)
		{
			Single num = time - this.honoUpdateUpsLastTime;
			this.Ups = (Single)this.honoFrameCounter / num;
			if (this.HonoUpsText)
			{
				this.HonoUpsText.text = String.Format("{0:F2} ({1}) HUPS", this.Ups, this.honoFrameCounter);
			}
			this.honoFrameCounter = 0;
			this.honoUpdateUpsTextTime = time + 1f;
			this.honoUpdateUpsLastTime = time;
		}
		time = Time.time;
		if (time >= this.monoUpdateUpsTextTime)
		{
			Single num2 = time - this.monoUpdateUpsLastTime;
			Single num3 = (Single)this.monoFrameCounter / num2;
			if (this.MonoUpsText)
			{
				this.MonoUpsText.text = String.Format("{0:F2} ({1}) MUPS", num3, this.monoFrameCounter);
			}
			this.monoFrameCounter = 0;
			this.monoUpdateUpsTextTime = time + 1f;
			this.monoUpdateUpsLastTime = time;
		}
		if (!this.didCheckTheDeviceModel && Application.platform == RuntimePlatform.Android)
		{
			String text = SystemInfo.deviceModel;
			if (text == "Nexus 10" || text == "Nexus 7" || text == "P-02E")
			{
				FF9StateSystem.World.HasLowFpsOnDevice = true;
			}
			text = SystemInfo.deviceModel.ToUpper();
			if (text.Contains("NEXUS 10") || text.Contains("NEXUS 7") || text.Contains("P-02E"))
			{
				FF9StateSystem.World.HasLowFpsOnDevice = true;
			}
			this.didCheckTheDeviceModel = true;
		}
		if (!this.didCheckTheGraphicsCard && (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor))
		{
			String text2 = SystemInfo.graphicsDeviceName.ToUpper();
			if (text2.Contains("INTEL") || text2.Contains("ADD_MORE_HERE"))
			{
				FF9StateSystem.World.HasLowFpsOnDevice = true;
			}
			this.didCheckTheGraphicsCard = true;
			if (FF9StateSystem.World.HasLowFpsOnDevice)
			{
				this.OnLowFps();
			}
		}
		this.UpdateCheckingLowEndAndroidDevices();
	}

	public override void HonoLateUpdate()
	{
		if (this.useCustomProjectionMatrix)
		{
			this.World.CreateProjectionMatrix();
		}
	}

	[ContextMenu("Use Custom Projection Matrix")]
	private void UseCustomProjectionMatrix()
	{
		this.useCustomProjectionMatrix = true;
	}

	[ContextMenu("Reset Projection Matrix")]
	private void ResetProjectionMatrix()
	{
		this.useCustomProjectionMatrix = false;
		this.World.MainCamera.ResetProjectionMatrix();
	}

	private void UpdateInput()
	{
		if (Input.GetKeyDown(KeyCode.Keypad9))
		{
			global::Debug.Log("ff9.w_worldChangeBlockSet()");
			ff9.w_worldChangeBlockSet();
		}
		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			Vector3 vector = ff9.w_moveActorPtr.RealPosition * 256f;
			global::Debug.Log(vector);
		}
		Vector3 vector2 = ff9.w_moveActorPtr.RealPosition * 256f;
		if (!FF9StateSystem.World.IsBeeScene)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.F9))
		{
			ff9.w_frameSetParameter(34, 0);
		}
		if (Input.GetKeyDown(KeyCode.Y))
		{
			this.SetToNextChracter();
		}
		else if (Input.GetKeyDown(KeyCode.U))
		{
			ff9.w_frameSetParameter(500, 9);
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			ff9.w_frameSetParameter(501, 5598);
		}
		if (Input.GetKeyDown(KeyCode.Keypad0))
		{
			ff9.w_frameSetParameter(25, 0);
		}
		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
		}
		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			ff9.w_frameSetParameter(501, 4990);
		}
		if (Input.GetKeyDown(KeyCode.Keypad3))
		{
			ff9.w_frameSetParameter(501, 5598);
		}
		if (Input.GetKeyDown(KeyCode.Keypad4))
		{
			ff9.w_frameSetParameter(501, 6200);
		}
		if (Input.GetKeyDown(KeyCode.Keypad5))
		{
			ff9.w_frameSetParameter(501, 6990);
		}
		if (Input.GetKeyDown(KeyCode.Keypad6))
		{
			ff9.w_frameSetParameter(501, 8800);
		}
		if (Input.GetKeyDown(KeyCode.Keypad7))
		{
			ff9.w_frameSetParameter(501, 10600);
		}
		if (Input.GetKeyDown(KeyCode.Keypad8))
		{
			ff9.w_frameSetParameter(501, 10700);
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			ff9.w_frameSetParameter(25, 0);
		}
	}

	public void SetToNextChracter()
	{
		this.currentCharacterIndex++;
		switch (this.currentCharacterIndex)
		{
		case 12:
			this.currentCharacterIndex = 1;
			break;
		}
		ff9.w_frameSetParameter(500, this.currentCharacterIndex);
	}

	public void SetChocoboAsMainCharacter()
	{
		this.currentCharacterIndex = 7;
		ff9.w_frameSetParameter(500, this.currentCharacterIndex);
	}

	public override void HonoOnStartFastForwardMode()
	{
		this.SetAnimationSpeeds(0.667f * (Single)HonoBehaviorSystem.Instance.GetFastForwardFactor());
		this.fastForwardFrameCounter = 0;
		this.fastForwardFrameCounter20FPS = 0;
	}

	public override void HonoOnStopFastForwardMode()
	{
		this.SetAnimationSpeeds(0.667f);
		this.fastForwardFrameCounter = 0;
		this.fastForwardFrameCounter20FPS = 0;
	}

	public override void HonoOnDestroy()
	{
	}

	public void SetAnimationSpeeds(Single speed)
	{
		for (ObjList objList = this.World.ActorList; objList != null; objList = objList.next)
		{
			if (objList.obj.cid == 4)
			{
				WMActor wmActor = ((Actor)objList.obj).wmActor;
				wmActor.SetAnimationSpeed(speed);
			}
		}
	}

	private const Single honoUpdateTime = 0.05f;

	private const Single monoUpdateTime = 0.05f;

	private const Single LowFpsTimeThreshold = 5f;

	private const Single ResetLowFpsTimeThreshold = 1.1f;

	private static WMScriptDirector instance;

	private Int32 currentCharacterIndex = 1;

	public WMWorld World;

	public WMBeeMovementAnimation BeeMovementAnimation;

	public WMRenderTextureBank RenderTextureBank;

	private Single honoCumulativeTime;

	private Int32 honoFrameCounter;

	public GUIText HonoUpsText;

	public Boolean UpdateHonoUpsText;

	private Single honoUpdateUpsTextTime;

	private Single honoUpdateUpsLastTime;

	private Single monoCumulativeTime;

	public Int32 monoFrameCounter;

	public GUIText MonoUpsText;

	public Boolean UpdateMonoUpsText;

	private Single monoUpdateUpsTextTime;

	private Single monoUpdateUpsLastTime;

	public Single Ups;

	public Boolean ForceUpdate20FPS = true;

	private FF9StateSystem FF9Sys;

	private FF9StateGlobal FF9;

	private FF9StateWorldSystem FF9World;

	private FF9StateWorldMap FF9WorldMap;

	private Boolean didCheckTheDeviceModel;

	private Boolean didCheckTheGraphicsCard;

	private Int32 fastForwardFrameCounter20FPS;

	public Single lowFpsTime;

	public Single resetLowFpsTime;

	private Boolean useCustomProjectionMatrix;

	private Int32 fastForwardFrameCounter;
}
