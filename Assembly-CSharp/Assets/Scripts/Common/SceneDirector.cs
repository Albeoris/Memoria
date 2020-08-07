using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Memoria.Assets;
using Memoria.Prime;
using SiliconStudio;
using UnityEngine;

namespace Assets.Scripts.Common
{
	public class SceneDirector : PersistenSingleton<SceneDirector>
	{
		public Boolean IsFading { get; set; }

		public Boolean IsReady { get; set; }

		public Boolean NeedFade { get; set; }

		public static void SetTargetFrameRateForCurrentScene()
		{
			if (String.Compare(PersistenSingleton<SceneDirector>.Instance.CurrentScene, SceneDirector.BattleMapSceneName) == 0 || String.Compare(PersistenSingleton<SceneDirector>.Instance.CurrentScene, "BattleMapDebug") == 0 || String.Compare(PersistenSingleton<SceneDirector>.Instance.CurrentScene, "SpecialEffectDebugRoom") == 0)
			{
				Application.targetFrameRate = 30;
			}
			else if (String.Compare(PersistenSingleton<SceneDirector>.Instance.CurrentScene, SceneDirector.FieldMapSceneName) == 0)
			{
				Application.targetFrameRate = 30;
			}
			else if (String.Compare(PersistenSingleton<SceneDirector>.Instance.CurrentScene, SceneDirector.WorldMapSceneName) == 0)
			{
				Application.targetFrameRate = 20;
            }
			else
			{
				Application.targetFrameRate = 60;
			}
		}

		public static Boolean IsBattleScene()
		{
			return String.Compare(PersistenSingleton<SceneDirector>.Instance.CurrentScene, SceneDirector.BattleMapSceneName) == 0 || String.Compare(PersistenSingleton<SceneDirector>.Instance.CurrentScene, "BattleMapDebug") == 0 || String.Compare(PersistenSingleton<SceneDirector>.Instance.CurrentScene, "SpecialEffectDebugRoom") == 0;
		}

		public static void ReplaceNow(String nextScene)
		{
            MemoriaExport();

            if (String.IsNullOrEmpty(nextScene))
            {
                Log.Error($"[{nameof(SceneDirector)}] Someone tried to change the current scene [{Instance.CurrentScene}] to the invalid scene: [{nextScene}]. Stack: " + Environment.NewLine + Environment.StackTrace);
            }

            if (nextScene != "MainMenu")
			{
				if (Singleton<BubbleUI>.Instance != (UnityEngine.Object)null)
				{
					Singleton<BubbleUI>.Instance.SetGameObjectActive(false);
				}
				EventHUD.Cleanup();
				EventInput.ClearPadMask();
			}
			PersistenSingleton<SceneDirector>.Instance.NextScene = nextScene;
			Application.LoadLevel("Loading");
			Resources.UnloadUnusedAssets();
			//GC.Collect();
		    PersistenSingleton<SceneDirector>.Instance.LoadNextScene();
			PersistenSingleton<SceneDirector>.Instance.LastScene = PersistenSingleton<SceneDirector>.Instance.CurrentScene;
			PersistenSingleton<SceneDirector>.Instance.CurrentScene = PersistenSingleton<SceneDirector>.Instance.NextScene;
			PersistenSingleton<SceneDirector>.Instance.PendingCurrentScene = String.Empty;
			PersistenSingleton<SceneDirector>.Instance.PendingNextScene = String.Empty;
			PersistenSingleton<SceneDirector>.Instance.NextScene = String.Empty;
			global::Debug.Log("---------- Current Scene : " + PersistenSingleton<SceneDirector>.Instance.CurrentScene + " ----------");
		}

	    private void LoadNextScene()
	    {
	        try
	        {
	            String nextScene = this.NextScene;
	            if (nextScene == null)
                    throw new ArgumentNullException(nameof(nextScene));

                if (nextScene == String.Empty)
	                throw new ArgumentException(nameof(nextScene));

                Application.LoadLevel(this.NextScene);
	        }
	        catch (Exception ex)
	        {
	            Log.Error(ex, "Failed to load next scene.");
	            throw;
	        }
	    }

	    public static void Replace(String nextScene, SceneTransition transition = SceneTransition.FadeOutToBlack_FadeIn, Boolean needFade = true)
		{
		    if (String.IsNullOrEmpty(nextScene))
		    {
		        Log.Error($"[{nameof(SceneDirector)}] Someone tried to change the current scene [{Instance.CurrentScene}] to the invalid scene: [{nextScene}]. Stack: " + Environment.NewLine + Environment.StackTrace);
		    }

		    if (nextScene != "MainMenu")
			{
				if (Singleton<BubbleUI>.Instance != (UnityEngine.Object)null)
				{
					Singleton<BubbleUI>.Instance.SetGameObjectActive(false);
				}
				EventHUD.Cleanup();
				EventInput.ClearPadMask();
			}
			PersistenSingleton<SceneDirector>.Instance.Transition = transition;
			if (transition == SceneTransition.SwirlInBlack || transition == SceneTransition.SwirlInWhite)
			{
				PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, (Action)null);
				PersistenSingleton<SceneDirector>.Instance.Swirl(nextScene, transition);
				return;
			}
			PersistenSingleton<SceneDirector>.Instance.NextScene = nextScene;
			SoundLib.LazyLoadSoundResources();
			PersistenSingleton<UIManager>.Instance.SetLoadingForSceneChange();
			PersistenSingleton<SceneDirector>.Instance.Replace(needFade);
		}

		public static void ReplacePending(SceneTransition transition = SceneTransition.FadeOutToBlack_FadeIn, Boolean needFade = true)
		{
			PersistenSingleton<SceneDirector>.Instance.NextScene = PersistenSingleton<SceneDirector>.Instance.PendingNextScene;
            PersistenSingleton<SceneDirector>.Instance.PendingNextScene = String.Empty;
            PersistenSingleton<SceneDirector>.Instance.Transition = transition;
			PersistenSingleton<SceneDirector>.Instance.Replace(needFade);
		}

		public static Texture2D GetScreenTexture()
		{
			return PersistenSingleton<SceneDirector>.Instance.screenTex;
		}

		public static SceneTransition GetDefaultFadeOutTransition()
		{
			return PersistenSingleton<SceneDirector>.Instance.DefaultFadeOutTransition;
		}

		public static SceneTransition GetDefaultFadeInTransition()
		{
			return PersistenSingleton<SceneDirector>.Instance.DefaultFadeInTransition;
		}

		private void OnApplicationFocus(Boolean focusStatus)
		{
			SceneDirector.hasFocus = focusStatus;
		}

		protected override void Awake()
		{
			base.Awake();
			SceneDirector.abrColor = new Color32[2];
			SceneDirector.abrColor[0] = new Color32(0, 0, 0, Byte.MaxValue);
			SceneDirector.abrColor[1] = new Color32(0, 0, 0, Byte.MaxValue);
			SceneDirector.ClearFadeColor();
			String loadedLevelName = Application.loadedLevelName;
			this.LastScene = String.Empty;
			this.CurrentScene = loadedLevelName;
			this.NextScene = String.Empty;
			this.PendingCurrentScene = String.Empty;
			this.PendingNextScene = String.Empty;
			this.IsFading = false;
			this.IsReady = true;
			this.fadeTex = Texture2D.whiteTexture;
			this.NeedFade = true;
			this.screenTex = Texture2D.whiteTexture;
			this.swirlCount = 0;
			this._logger = new OSDLogger();
			this._logger.Init();
			OSDLogger.Instance = this._logger;
			GameObject gameObject = new GameObject("SoundPlayer");
			gameObject.transform.parent = base.transform;
			SoundLib soundLib = gameObject.AddComponent<SoundLib>();
			SoundLibInitializer soundLibInitializer = gameObject.AddComponent<SoundLibInitializer>();
		}

		private void Update()
		{
			this._logger.Update();
			SiliconStudio.Social.ProcessCallbacks();
		}

		private void OnGUI()
		{
			GUI.depth = -2147483647;
			if (this.IsFading && Event.current.type == EventType.Repaint)
			{
				this._OnGUI_FullscreenEffect();
			}
			if (this._logger.IsShowLog)
			{
				this._logger.OnGUI();
			}
		}

		private void OnEnable()
		{
			this._logger.EnableLog();
		}

		private void OnDisable()
		{
			this._logger.DisableLog();
		}

		private void _OnGUI_FullscreenEffect()
		{
			Matrix4x4 matrix = GUI.matrix;
			Color color = GUI.color;
			if (this.Transition != SceneTransition.SwirlInBlack && this.Transition != SceneTransition.SwirlInWhite)
			{
				this._OnGUI_Fade();
			}
			GUI.color = color;
			GUI.matrix = matrix;
		}

		private void _OnGUI_Fade()
		{
            Color color = this.curColor;
			color.a = this.fadeAmount;
			GUI.color = color;
		}

		private IEnumerator _Fade(Single fadeOutTime, Single fadeInTime, Color fadeColor, Boolean needChangeScene = true)
		{
			this.IsFading = true;
			this.IsReady = false;
			this.curColor = fadeColor;
			if (fadeOutTime > 0f)
			{
				this.fadeAmount = 0f;
				while (this.fadeAmount < 1f)
				{
					this.fadeAmount = Mathf.Clamp01(this.fadeAmount + Time.deltaTime / fadeOutTime);
					yield return new WaitForEndOfFrame();
				}
			}
			else
			{
				this.fadeAmount = 1f;
			}
			if (needChangeScene)
			{
				this.ChangeScene();
			}
			yield return new WaitForEndOfFrame();
			this.IsReady = true;
			if (fadeInTime < 0f)
			{
				if (needChangeScene)
				{
					this.IsFading = false;
				}
				yield break;
			}
			this.fadeAmount = 1f;
			this.IsFading = true;
			this.curColor = fadeColor;
			while (this.fadeAmount > 0f)
			{
				this.fadeAmount = Mathf.Clamp01(this.fadeAmount - Time.deltaTime / fadeInTime);
				yield return new WaitForEndOfFrame();
			}
			this.IsFading = false;
			yield break;
		}

		private void ChangeScene()
		{
			Application.LoadLevel("Loading");
			Resources.UnloadUnusedAssets();
			//GC.Collect();
		    LoadNextScene();
			SoundLib.StopAllSoundEffects();
			if (this.CurrentScene != this.NextScene)
			{
				if (String.Equals(this.CurrentScene, "QuadMist"))
				{
					SceneDirector.FF9Wipe_FadeInEx(30);
				}
				if (String.Equals(this.NextScene, "MainMenu"))
				{
					SoundLib.StopAllSounds(true);
				}
				if (String.Equals(this.NextScene, SceneDirector.FieldMapSceneName))
				{
					FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9FieldSoundDispatch);
					if (String.Equals(this.CurrentScene, SceneDirector.WorldMapSceneName))
					{
						FF9Snd.HasJustChangedBetweenWorldAndField = true;
					}
				}
				else if (String.Equals(this.NextScene, SceneDirector.WorldMapSceneName))
				{
					AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_STOPCURRENT();
					allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP_ALL(new HashSet<Int32>
					{
						1261
					});
					allSoundDispatchPlayer.FF9SOUND_STREAM_STOP();
					FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9WorldSoundDispatch);
					if (String.Equals(this.CurrentScene, SceneDirector.FieldMapSceneName))
					{
						FF9Snd.HasJustChangedBetweenWorldAndField = true;
					}
				}
				else if (!String.Equals(this.NextScene, SceneDirector.BattleMapSceneName))
				{
					if (String.Equals(this.NextScene, "QuadMist"))
					{
						FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9MiniGameSoundDispatch);
					}
					else
					{
						FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9AllSoundDispatch);
					}
				}
				if (String.IsNullOrEmpty(this.PendingCurrentScene))
				{
					this.LastScene = this.CurrentScene;
				}
				else
				{
					this.LastScene = this.PendingCurrentScene;
					this.PendingCurrentScene = String.Empty;
				}
				this.CurrentScene = this.NextScene;
				global::Debug.Log("---------- Current Scene : " + PersistenSingleton<SceneDirector>.Instance.CurrentScene + " ----------");
			}
			this.NextScene = String.Empty;
			SceneDirector.SetTargetFrameRateForCurrentScene();
		}

		private void Replace(Boolean needFade)
		{
			this.NeedFade = needFade;
			if (this.IsFading)
			{
				return;
			}
			Single num = 1f / (Single)FF9StateSystem.Settings.FastForwardFactor;
			SceneTransition transition = this.Transition;
			IEnumerator routine;
			if (transition != SceneTransition.FadeOutToBlack)
			{
				if (transition != SceneTransition.FadeOutToBlack_FadeIn)
				{
					routine = this._Fade(this.GetFadeDuration() * num, this.GetFadeDuration() * num, Color.black, true);
				}
				else
				{
					routine = this._Fade(this.GetFadeDuration() * num, this.GetFadeDuration() * num, Color.black, true);
				}
			}
			else
			{
				routine = this._Fade(this.GetFadeDuration() * num, -1f, Color.black, true);
			}
			base.StartCoroutine(routine);
		}

		public Single GetFadeDuration()
		{
			if (this.NeedFade)
			{
				return this.DefaultFadeTime;
			}
			return 0.01f;
		}

		public void Swirl(String nextScene, SceneTransition transition)
		{
			if (this.IsFading)
			{
				return;
			}
			base.StartCoroutine(this._Swirl(nextScene, transition));
		}

		private IEnumerator _Swirl(String nextScene, SceneTransition transition)
		{
            this.PendingCurrentScene = this.CurrentScene;
			this.PendingNextScene = nextScene;
			this.IsFading = true;
			UIManager.Field.Loading = true;
			PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(false);
			PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(false);
			PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, (Action)null);
			for (Int32 waitFrames = (Int32)((!FF9StateSystem.Settings.IsFastForward) ? ((Int32)((!PersistenSingleton<UIManager>.Instance.Dialogs.Visible) ? 2 : 9)) : (FF9StateSystem.Settings.FastForwardFactor * 2)); waitFrames > 0; waitFrames--)
			{
				yield return new WaitForEndOfFrame();
			}
			SFX_Rush.CreateScreen();
			yield return null;
			Application.LoadLevel("SwirlScene");
			this.LastScene = this.CurrentScene;
			this.CurrentScene = "SwirlScene";
			global::Debug.Log("---------- Current Scene : " + PersistenSingleton<SceneDirector>.Instance.CurrentScene + " ----------");
			this.NextScene = String.Empty;
			yield return new WaitForEndOfFrame();
			Resources.UnloadUnusedAssets();
			//GC.Collect();
			SceneDirector.ClearFadeColor();
			this.IsFading = false;
			yield break;
		}

		public static void FadeEventSetColor(FadeMode mode, Color32 target)
		{
			SceneDirector._SetFadeMode(mode);
			String propertyName = "_FadeColor_ABR" + (Int32)(SceneDirector.fadeMode + 1);
			SceneDirector.abrColor[(Int32)SceneDirector.fadeMode] = target;
			Shader.SetGlobalColor(propertyName, target);
		}

		public static void ToggleFadeAll(Boolean isEnable)
		{
			if (isEnable)
			{
				Shader.SetGlobalInt("_FadeMode", (Int32)SceneDirector.fadeMode);
			}
			else
			{
				Shader.SetGlobalInt("_FadeMode", -1);
			}
		}

		private static void _SetFadeMode(FadeMode mode)
		{
			SceneDirector.fadeMode = mode;
			Boolean flag = PersistenSingleton<EventEngine>.Instance.IsEventContextValid() && FF9StateSystem.Common.FF9.fldMapNo == 60 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(6357) == 5 && mode == FadeMode.Add && FF9StateSystem.Settings.IsFastForward;
			Boolean flag2 = PersistenSingleton<EventEngine>.Instance.IsEventContextValid() && FF9StateSystem.Common.FF9.fldMapNo == 1661 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 6930 && mode == FadeMode.Add;
			if (flag || flag2)
			{
				PersistenSingleton<SceneDirector>.Instance.StartCoroutine(SceneDirector.SetGlobalFade());
			}
			else
			{
				Shader.SetGlobalInt("_FadeMode", (Int32)mode);
			}
		}

		private static IEnumerator SetGlobalFade()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			Shader.SetGlobalInt("_FadeMode", (Int32)SceneDirector.fadeMode);
			yield break;
		}

		public static FadeMode GetFadeMode
		{
			get
			{
				return SceneDirector.fadeMode;
			}
		}

		public static void ClearFadeColor()
		{
			Shader.SetGlobalColor("_FadeColor_ABR" + 1, new Color(0f, 0f, 0f));
			Shader.SetGlobalColor("_FadeColor_ABR" + 2, new Color(0f, 0f, 0f));
		}

		public static void InitFade(FadeMode mode, Int32 frame, Color32 target)
		{
			SceneDirector._curFrame = 0f;
			SceneDirector._targetFrame = (Single)frame;
			SceneDirector._targetColor = target;
			SceneDirector._SetFadeMode(mode);
			SceneDirector._prevColor = SceneDirector.abrColor[(Int32)SceneDirector.fadeMode];
		}

		public static void ServiceFade()
		{
			if (SceneDirector._curFrame > SceneDirector._targetFrame)
			{
				return;
			}

			SceneDirector._curFrame += 1f;
			String propertyName = "_FadeColor_ABR" + (Int32)(SceneDirector.fadeMode + 1);
			Color32 color = Color32.Lerp(SceneDirector._prevColor, SceneDirector._targetColor, SceneDirector._curFrame / SceneDirector._targetFrame);
			SceneDirector.abrColor[(Int32)SceneDirector.fadeMode] = color;
			Shader.SetGlobalColor(propertyName, color);
		}

		public static void FF9Wipe_FadeInEx(Int32 frame)
		{
			SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.white);
			SceneDirector.InitFade(FadeMode.Sub, frame, Color.black);
		}

		public static void FF9Wipe_FadeOutEx(Int32 frame)
		{
			SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
			SceneDirector.InitFade(FadeMode.Sub, frame, Color.white);
		}

		public static void FF9Wipe_WhiteInEx(Int32 frame)
		{
			SceneDirector.FadeEventSetColor(FadeMode.Add, Color.white);
			SceneDirector.InitFade(FadeMode.Add, frame, Color.black);
		}

		public static void FF9Wipe_WhiteOutEx(Int32 frame)
		{
			SceneDirector.FadeEventSetColor(FadeMode.Add, Color.black);
			SceneDirector.InitFade(FadeMode.Add, frame, Color.white);
		}

		public static void FF9Wipe_FadeIn()
		{
			SceneDirector.FF9Wipe_FadeInEx(16);
		}

		public static void FF9Wipe_FadeOut()
		{
			SceneDirector.FF9Wipe_FadeOutEx(12);
		}

		public static void FF9Wipe_WhiteIn()
		{
			SceneDirector.FF9Wipe_WhiteInEx(16);
		}

		public static void FF9Wipe_WhiteOut()
		{
			SceneDirector.FF9Wipe_WhiteOutEx(12);
		}

		private const Int32 FF9WIPE_FADEIN_DEF_FRAME = 16;

		private const Int32 FF9WIPE_FADEOUT_DEF_FRAME = 12;

		public static String FieldMapSceneName = "FieldMap";

		public static String BattleMapSceneName = "BattleMap";

		public static String WorldMapSceneName = "WorldMap";

		public SceneTransition Transition = SceneTransition.FadeOutToBlack_FadeIn;

		public SceneTransition DefaultFadeOutTransition = SceneTransition.FadeOutToBlack;

		public SceneTransition DefaultFadeInTransition = SceneTransition.FadeInFromBlack;

		public Single DefaultFadeTime = 0.01f;

		public String LastScene;

		public String CurrentScene;

		public String NextScene;

		public String PendingCurrentScene;

		public String PendingNextScene;

		public Texture2D fadeTex;

		public Texture2D screenTex;

		public static Boolean hasFocus;

		private Color curColor;

		private Single fadeAmount;

		private IEnumerator currentFadeEventCoroutine;

		private Int32 swirlCount;

		private OSDLogger _logger;

		private static Color32[] abrColor;

		private static FadeMode fadeMode;

		private static Single _curFrame;

		private static Single _targetFrame;

		private static Color32 _targetColor;

		private static Color32 _prevColor;

        private static Int32 _initialized;

        private static void MemoriaExport()
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 0)
            {
                ResourceExporter.ExportSafe();
                ResourceImporter.Initialize();
            }
        }
    }
}
