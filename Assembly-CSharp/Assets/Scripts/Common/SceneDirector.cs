using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Assets;
using Memoria.Prime;
using Memoria.Speedrun;
using Memoria.Prime.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            if (SceneDirector.IsBattleScene())
            {
                FPSManager.SetTargetFPS(Configuration.Graphics.BattleFPS);
                FPSManager.SetMainLoopSpeed(Configuration.Graphics.BattleTPS);
            }
            else if (SceneDirector.IsFieldScene())
            {
                FPSManager.SetTargetFPS(Configuration.Graphics.FieldFPS);
                FPSManager.SetMainLoopSpeed(Configuration.Graphics.FieldTPS);
            }
            else if (SceneDirector.IsWorldScene())
            {
                FPSManager.SetTargetFPS(Configuration.Graphics.WorldFPS);
                FPSManager.SetMainLoopSpeed(Configuration.Graphics.WorldTPS);
            }
            else
            {
                FPSManager.SetTargetFPS(Configuration.Graphics.MenuFPS);
                FPSManager.SetMainLoopSpeed(Configuration.Graphics.MenuTPS);
            }
        }

        public static Boolean IsBattleScene()
        {
            return String.Equals(PersistenSingleton<SceneDirector>.Instance.CurrentScene, SceneDirector.BattleMapSceneName)
                || String.Equals(PersistenSingleton<SceneDirector>.Instance.CurrentScene, "BattleMapDebug")
                || String.Equals(PersistenSingleton<SceneDirector>.Instance.CurrentScene, "SpecialEffectDebugRoom");
        }

        public static Boolean IsFieldScene()
        {
            return String.Equals(PersistenSingleton<SceneDirector>.Instance.CurrentScene, SceneDirector.FieldMapSceneName);
        }

        public static Boolean IsWorldScene()
        {
            return String.Equals(PersistenSingleton<SceneDirector>.Instance.CurrentScene, SceneDirector.WorldMapSceneName);
        }

        public static void ReplaceNow(String nextScene)
        {
            MemoriaExport();

            if (String.IsNullOrEmpty(nextScene))
                Log.Error($"[{nameof(SceneDirector)}] Someone tried to change the current scene [{Instance.CurrentScene}] to the invalid scene: [{nextScene}]. Stack: " + Environment.NewLine + Environment.StackTrace);

            if (nextScene != "MainMenu")
            {
                if (Singleton<BubbleUI>.Instance != null)
                    Singleton<BubbleUI>.Instance.SetGameObjectActive(false);
                EventHUD.Cleanup();
                EventInput.ClearPadMask();
            }
            PersistenSingleton<SceneDirector>.Instance.NextScene = nextScene;
            AutoSplitterPipe.SignalLoadStart();
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
                AutoSplitterPipe.SignalLoadEnd();
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
                Log.Error($"[{nameof(SceneDirector)}] Someone tried to change the current scene [{Instance.CurrentScene}] to the invalid scene: [{nextScene}]. Stack: " + Environment.NewLine + Environment.StackTrace);

            if (SceneDirector.IsBattleScene())
                SmoothFrameUpdater_Battle.Skip = 1;
            else if (SceneDirector.IsFieldScene())
                SmoothFrameUpdater_Field.Skip = 1;
            else if (SceneDirector.IsWorldScene())
                SmoothFrameUpdater_World.Skip = 1;

            if (nextScene != "MainMenu")
            {
                if (Singleton<BubbleUI>.Instance != null)
                    Singleton<BubbleUI>.Instance.SetGameObjectActive(false);
                EventHUD.Cleanup();
                EventInput.ClearPadMask();
            }
            PersistenSingleton<SceneDirector>.Instance.Transition = transition;
            if (transition == SceneTransition.SwirlInBlack || transition == SceneTransition.SwirlInWhite)
            {
                PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, null);
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
            for (Int32 updateCount = 0; updateCount < FPSManager.MainLoopUpdateCount; updateCount++)
            {
                this._logger.Update();
                SiliconStudio.Social.ProcessCallbacks();
            }
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
                this._OnGUI_Fade();
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
                    try
                    {
                        this.fadeAmount = Mathf.Clamp01(this.fadeAmount + Time.deltaTime / fadeOutTime);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                this.fadeAmount = 1f;
            }
            if (needChangeScene)
            {
                if (SceneDirector._discChange != 0)
                    yield return StartCoroutine(ChangeSceneAsync()); // to fix crash on disc changes for some people #757
                else
                    this.ChangeScene();
            }
            yield return new WaitForEndOfFrame();
            this.IsReady = true;
            if (fadeInTime < 0f)
            {
                if (needChangeScene)
                    this.IsFading = false;
                yield break;
            }
            this.fadeAmount = 1f;
            this.IsFading = true;
            this.curColor = fadeColor;
            while (this.fadeAmount > 0f)
            {
                try
                {
                    this.fadeAmount = Mathf.Clamp01(this.fadeAmount - Time.deltaTime / fadeInTime);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                yield return new WaitForEndOfFrame();
            }

            if (SceneDirector._discChange != 0)
            {
                Int32 discNum = SceneDirector._discChange + 1;

                try
                {
                    this.screenTex = AssetManager.Load<Texture2D>("EmbeddedAsset/UI/Sprites/changetodisc" + discNum + ".png");
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                if (this.screenTex != null)
                {
                    try
                    {
                        if (this.screenTex.width < 321)
                            this.screenTex.filterMode = FilterMode.Point;
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }

                    Single rectwidth = Screen.height / 0.7f;
                    Rect screenRect = new((Screen.width - rectwidth) / 2f, 0f, rectwidth, Screen.height);
                    Rect sourceRect = new(0f, 0f, 1f, 1f);
                    if (((Single)this.screenTex.height / (Single)this.screenTex.width) < 0.698f)
                        screenRect = new(0f, 0f, Screen.width, Screen.height);
                    for (Int32 i = 0; i <= 100; i++) // 100 frame fadein
                    {
                        try
                        {
                            Graphics.DrawTexture(screenRect, this.screenTex, sourceRect, 0, 0, 0, 0, new Color(0.5f, 0.5f, 0.5f, i / 100f));
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }

                        yield return new WaitForEndOfFrame();
                    }
                    while (!PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.Confirm)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.Cancel)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.Menu)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.Special)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.Pause)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.Select)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.Up)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.Down)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.Left)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.Right)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.LeftBumper)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.RightBumper)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.LeftTrigger)
                        && !PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.RightTrigger))
                    {
                        try
                        {
                            Graphics.DrawTexture(screenRect, this.screenTex);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    for (Int32 i = 100; i > 0; i--) // 100 frame fadeout
                    {
                        try
                        {
                            Graphics.DrawTexture(screenRect, this.screenTex, sourceRect, 0, 0, 0, 0, new Color(0.5f, 0.5f, 0.5f, i / 100f));
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                        yield return new WaitForEndOfFrame();
                    }
                }
            }

            this.IsFading = false;
            yield break;
        }

        private void ChangeScene()
        {
            AutoSplitterPipe.SignalLoadStart();
            if (this.CurrentScene == SceneDirector.BattleMapSceneName)
                AutoSplitterPipe.SignalBattleEnd();
            Application.LoadLevel("Loading");
            Resources.UnloadUnusedAssets();
            //GC.Collect();
            LoadNextScene();
            if (this.CurrentScene != this.NextScene)
            {
                SoundLib.StopAllSoundEffects(); // Issue #140: don't stop sounds in field transitions
                if (String.Equals(this.CurrentScene, "QuadMist"))
                    SceneDirector.FF9Wipe_FadeInEx(30);
                if (String.Equals(this.NextScene, "MainMenu"))
                    SoundLib.StopAllSounds(true);
                if (String.Equals(this.NextScene, SceneDirector.FieldMapSceneName))
                {
                    FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9FieldSoundDispatch);
                    if (String.Equals(this.CurrentScene, SceneDirector.WorldMapSceneName))
                        FF9Snd.HasJustChangedBetweenWorldAndField = true;
                }
                else if (String.Equals(this.NextScene, SceneDirector.WorldMapSceneName))
                {
                    AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
                    allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_STOPCURRENT();
                    allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP_ALL(new HashSet<Int32>
                    {
                        1261 // Sounds02/SE00/se000026, Save and Load game confirmed
                    });
                    allSoundDispatchPlayer.FF9SOUND_STREAM_STOP();
                    FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9WorldSoundDispatch);
                    if (String.Equals(this.CurrentScene, SceneDirector.FieldMapSceneName))
                        FF9Snd.HasJustChangedBetweenWorldAndField = true;
                }
                else if (!String.Equals(this.NextScene, SceneDirector.BattleMapSceneName))
                {
                    if (String.Equals(this.NextScene, "QuadMist"))
                        FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9MiniGameSoundDispatch);
                    else
                        FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9AllSoundDispatch);
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

        private IEnumerator ChangeSceneAsync()
        {
            AutoSplitterPipe.SignalLoadStart();
            if (this.CurrentScene == SceneDirector.BattleMapSceneName)
                AutoSplitterPipe.SignalBattleEnd();
            AsyncOperation loadingSceneOperation = Application.LoadLevelAsync("Loading");
            yield return loadingSceneOperation;
            yield return Resources.UnloadUnusedAssets();
            yield return StartCoroutine(LoadNextSceneCoroutine());
            if (this.CurrentScene != this.NextScene)
            {
                SoundLib.StopAllSoundEffects(); // Issue #140: don't stop sounds in field transitions
                if (String.Equals(this.CurrentScene, "QuadMist"))
                    SceneDirector.FF9Wipe_FadeInEx(30);
                if (String.Equals(this.NextScene, "MainMenu"))
                    SoundLib.StopAllSounds(true);
                if (String.Equals(this.NextScene, SceneDirector.FieldMapSceneName))
                {
                    FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9FieldSoundDispatch);
                    if (String.Equals(this.CurrentScene, SceneDirector.WorldMapSceneName))
                        FF9Snd.HasJustChangedBetweenWorldAndField = true;
                }
                else if (String.Equals(this.NextScene, SceneDirector.WorldMapSceneName))
                {
                    AllSoundDispatchPlayer allSoundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
                    allSoundDispatchPlayer.FF9SOUND_SNDEFFECTRES_STOPCURRENT();
                    allSoundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP_ALL(new HashSet<Int32>
                    {
                        1261 // Sounds02/SE00/se000026, Save and Load game confirmed
                    });
                    allSoundDispatchPlayer.FF9SOUND_STREAM_STOP();
                    FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9WorldSoundDispatch);
                    if (String.Equals(this.CurrentScene, SceneDirector.FieldMapSceneName))
                        FF9Snd.HasJustChangedBetweenWorldAndField = true;
                }
                else if (!String.Equals(this.NextScene, SceneDirector.BattleMapSceneName))
                {
                    if (String.Equals(this.NextScene, "QuadMist"))
                        FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9MiniGameSoundDispatch);
                    else
                        FF9Snd.sndFuncPtr = new FF9Snd.SoundDispatchDelegate(FF9Snd.FF9AllSoundDispatch);
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

        private IEnumerator LoadNextSceneCoroutine()
        {
            AsyncOperation nextSceneOperation = Application.LoadLevelAsync(this.NextScene);
            yield return nextSceneOperation;
        }

        private void Replace(Boolean needFade)
        {
            this.NeedFade = needFade;
            if (this.IsFading)
                return;
            Single num = 1f / FF9StateSystem.Settings.FastForwardFactor;
            SceneTransition transition = this.Transition;
            IEnumerator routine;
            if (transition == SceneTransition.FadeOutToBlack)
                routine = this._Fade(this.GetFadeDuration() * num, -1f, Color.black, true);
            else if (transition == SceneTransition.FadeOutToBlack_FadeIn)
                routine = this._Fade(this.GetFadeDuration() * num, this.GetFadeDuration() * num, Color.black, true);
            else
                routine = this._Fade(this.GetFadeDuration() * num, this.GetFadeDuration() * num, Color.black, true);
            base.StartCoroutine(routine);
        }

        public Single GetFadeDuration()
        {
            if (this.NeedFade)
                return this.DefaultFadeTime;
            return 0.01f;
        }

        public void Swirl(String nextScene, SceneTransition transition)
        {
            if (this.IsFading)
                return;
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
            PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, null);
            for (Int32 waitFrames = FF9StateSystem.Settings.IsFastForward ? (2 * FF9StateSystem.Settings.FastForwardFactor) : (PersistenSingleton<UIManager>.Instance.Dialogs.Visible ? 9 : 2); waitFrames > 0; waitFrames--)
                yield return new WaitForEndOfFrame();
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
                Shader.SetGlobalInt("_FadeMode", (Int32)SceneDirector.fadeMode);
            else
                Shader.SetGlobalInt("_FadeMode", -1);
        }

        private static void _SetFadeMode(FadeMode mode)
        {
            SceneDirector.fadeMode = mode;
            Boolean flag = PersistenSingleton<EventEngine>.Instance.IsEventContextValid()
                        && FF9StateSystem.Common.FF9.fldMapNo == 60 // Prima Vista/Interior
                        && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.getVarOperation(EBin.VariableSource.Map, EBin.VariableType.Byte, 24)) == 5
                        && mode == FadeMode.Add
                        && FF9StateSystem.Settings.IsFastForward;
            Boolean flag2 = PersistenSingleton<EventEngine>.Instance.IsEventContextValid()
                         && FF9StateSystem.Common.FF9.fldMapNo == 1661 // Brahne’s Fleet/Event
                         && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 6930
                         && mode == FadeMode.Add;
            if (flag || flag2)
                PersistenSingleton<SceneDirector>.Instance.StartCoroutine(SceneDirector.SetGlobalFade());
            else
                Shader.SetGlobalInt("_FadeMode", (Int32)mode);
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
            get => SceneDirector.fadeMode;
        }

        public static void ClearFadeColor()
        {
            Shader.SetGlobalColor("_FadeColor_ABR" + 1, new Color(0f, 0f, 0f));
            Shader.SetGlobalColor("_FadeColor_ABR" + 2, new Color(0f, 0f, 0f));
        }

        public static void InitDiscChange(Int32 disc_id)
        {
            try
            {
                InitFade(FadeMode.Sub, 10, Color.black, disc_id);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

        }

        public static void InitFade(FadeMode mode, Int32 frame, Color32 target, Int32 disc_id = 0)
        {
            SceneDirector._curFrame = 0f;
            SceneDirector._targetFrame = frame;
            SceneDirector._targetColor = target;
            SceneDirector._SetFadeMode(mode);
            SceneDirector._prevColor = SceneDirector.abrColor[(Int32)SceneDirector.fadeMode];
            if (disc_id > 0 && disc_id < 4)
                SceneDirector._discChange = disc_id;
            else
                SceneDirector._discChange = 0;
        }

        public static void ServiceFade()
        {
            if (SceneDirector._curFrame > SceneDirector._targetFrame)
                return;

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

        [NonSerialized]
        private static Int32 _discChange;

        private static void MemoriaExport()
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 0)
            {
                if (Configuration.Import.Enabled && Configuration.Export.Enabled)
                {
                    FF9StateSystem.Settings.ReadSystemData(() =>
                    {
                        Thread importerThread = new Thread(ResourceImporter.Initialize);
                        importerThread.Start();
                        while (importerThread.ThreadState == ThreadState.Running || (FF9TextTool.BattleImporter.InitializationTask != null && FF9TextTool.BattleImporter.InitializationTask.State == TaskState.Running) || (FF9TextTool.FieldImporter.InitializationTask != null && FF9TextTool.FieldImporter.InitializationTask.State == TaskState.Running))
                            Thread.Sleep(100);
                        ResourceExporter.ExportSafe();
                    });
                }
                else if (Configuration.Import.Enabled)
                {
                    ResourceImporter.Initialize();
                }
                else if (Configuration.Export.Enabled)
                {
                    FF9StateSystem.Settings.ReadSystemData(ResourceExporter.ExportSafe);
                }
            }
        }
    }
}
