using Memoria;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

// ReSharper disable ArrangeThisQualifier
// ReSharper disable UnusedMember.Global
// ReSharper disable StringCompareIsCultureSpecific.1
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

namespace Assets.Scripts.Common
{
    [ExportedType("ÈħĽģ&!!!ŃĄ%î}òÆĐz+ýĦ0Ð¹}A!!!ā6bÓ¤oÀênįî=¸ìyß¸zXĵĹ+Ru¼ÝëoğyĮXÐëiîDà¶ôÐĴĸÙxęÆ)<ĭŁ'Á30Ĭ¬ÜDÞłóĵ8İğĬOą|Yõ¡ÝįõúĢtÙÑ|:ĀÓaĤ-N<ðßĞX«Ąk%ċ'rJě±þHýGÎİ>ÌbeŁŃ-ÖāÑ2Ò=*P!!!ªüÈĭBĄçéy-ĐĠÛąrąēâ|ĖPØÕûg¨j§Łÿ(ďàÅñ7īEJ«7Oa_X:¢īôćeìŀ¿!:%¿5ĖĴôŀPİV¦¨ÿ¦ø9ď<¾0FĘ¹bæÂĴ)²¹é·)æÏÞ0ÁİÞøą9æþĸTÈM&}º¹FèL-jXīĤİĤ¹h1ċÑ[WĚ½ÛĪ¼fĄ¬f©i;Ġ,£ŃĬëÇ$DQéŁĤZ1ÀĎħ­ĵfÁéī^Lbhèl×ŀ,Àă¤ĻCãĂTĪß¾k4%!!!áæÎû$!!!ÁńhîĹµÏr-!!!Bĉĉı´ęĒĦXÂP²Å§ÉĜÞ¤łĤ¿ĐWĖ@bË~pr'ĶåàÞÞqË¯bĚX?Ļ(!!!ĄÀļěx,ĲOĒ§ýÈ8L{ÇÏrþ¦jÅpZńńńń5ÐĖ±$!!!ÁńhîĹµÏr(!!!VČûq¢©§ĹÞ¤łĤ¿ĐWĖëBCØĚX?Ļ(!!!ĄÀļěīHñ¤ÃqľĶ}¬³ċĈŁc#jÅpZńńńńöĽ§Å$!!!ÁńhîĹµÏr$!!!Þ¤łĤ¿ĐWĖ(!!!ĄÀļěėľ3-mÖgê³§ÒY¨ÿ¢°jÅpZńńńń")]
    public class SceneDirector : PersistenSingleton<SceneDirector>
    {
        private const Int32 FF9WipeFadeinDefFrame = 16;
        private const Int32 FF9WipeFadeoutDefFrame = 12;

        public static String FieldMapSceneName;
        public static String BattleMapSceneName;
        public static String WorldMapSceneName;
        public static Boolean hasFocus;
        public SceneTransition Transition;
        public SceneTransition DefaultFadeOutTransition;
        public SceneTransition DefaultFadeInTransition;
        public Single DefaultFadeTime;
        public String LastScene;
        public String CurrentScene;
        public String NextScene;
        public String PendingCurrentScene;
        public String PendingNextScene;
        public Texture2D fadeTex;
        public Texture2D screenTex;
        private Color curColor;
        private Single fadeAmount;
        private OSDLogger _logger;
        private static Color32[] abrColor;
        private static FadeMode fadeMode;
        private static Single _curFrame;
        private static Single _targetFrame;
        private static Color32 _targetColor;
        private static Color32 _prevColor;

        public Boolean IsFading { get; set; }
        public Boolean IsReady { get; set; }
        public Boolean NeedFade { get; set; }

        public static FadeMode GetFadeMode => fadeMode;

        static SceneDirector()
        {
            FieldMapSceneName = "FieldMap";
            BattleMapSceneName = "BattleMap";
            WorldMapSceneName = "WorldMap";
        }

        public SceneDirector()
        {
            Transition = SceneTransition.FadeOutToBlack_FadeIn;
            DefaultFadeOutTransition = SceneTransition.FadeOutToBlack;
            DefaultFadeInTransition = SceneTransition.FadeInFromBlack;
            DefaultFadeTime = 0.01f;
        }

        public static void SetTargetFrameRateForCurrentScene()
        {
            if (String.Compare(Instance.CurrentScene, BattleMapSceneName) == 0
                || String.Compare(Instance.CurrentScene, "BattleMapDebug") == 0
                || String.Compare(Instance.CurrentScene, "SpecialEffectDebugRoom") == 0)
                Application.targetFrameRate = 30;
            else if (String.Compare(Instance.CurrentScene, FieldMapSceneName) == 0)
                Application.targetFrameRate = 30;
            else if (String.Compare(Instance.CurrentScene, WorldMapSceneName) == 0)
                Application.targetFrameRate = 20;
            else
                Application.targetFrameRate = 60;
        }

        public static Boolean IsBattleScene()
        {
            if (String.Compare(Instance.CurrentScene, BattleMapSceneName) != 0 && String.Compare(Instance.CurrentScene, "BattleMapDebug") != 0)
                return String.Compare(Instance.CurrentScene, "SpecialEffectDebugRoom") == 0;
            return true;
        }

        public static void ReplaceNow(string nextScene)
        {
            MemoriaExport();
            if (nextScene != "MainMenu")
            {
                if (Singleton<BubbleUI>.Instance != null)
                    Singleton<BubbleUI>.Instance.SetGameObjectActive(false);
                EventHUD.Cleanup();
                EventInput.ClearPadMask();
            }

            Instance.NextScene = nextScene;
            Application.LoadLevel("Loading");
            Resources.UnloadUnusedAssets();
            GC.Collect();
            Application.LoadLevel(Instance.NextScene);
            Instance.LastScene = Instance.CurrentScene;
            Instance.CurrentScene = Instance.NextScene;
            Instance.PendingCurrentScene = string.Empty;
            Instance.PendingNextScene = string.Empty;
            Instance.NextScene = string.Empty;
            UnityEngine.Debug.Log("---------- Current Scene : " + Instance.CurrentScene + " ----------");
        }

        public static void Replace(string nextScene, SceneTransition transition = SceneTransition.FadeOutToBlack_FadeIn, bool needFade = true)
        {
            if (nextScene != "MainMenu")
            {
                if (Singleton<BubbleUI>.Instance != null)
                    Singleton<BubbleUI>.Instance.SetGameObjectActive(false);

                EventHUD.Cleanup();
                EventInput.ClearPadMask();
            }

            Instance.Transition = transition;
            if (transition == SceneTransition.SwirlInBlack || transition == SceneTransition.SwirlInWhite)
            {
                PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, null);
                Instance.Swirl(nextScene, transition);
            }
            else
            {
                Instance.NextScene = nextScene;
                SoundLib.LazyLoadSoundResources();
                PersistenSingleton<UIManager>.Instance.SetLoadingForSceneChange();
                Instance.Replace(needFade);
            }
        }

        public static void ReplacePending(SceneTransition transition = SceneTransition.FadeOutToBlack_FadeIn, bool needFade = true)
        {
            Instance.NextScene = Instance.PendingNextScene;
            Instance.Transition = transition;
            Instance.Replace(needFade);
        }

        public static Texture2D GetScreenTexture()
        {
            return Instance.screenTex;
        }

        public static SceneTransition GetDefaultFadeOutTransition()
        {
            return Instance.DefaultFadeOutTransition;
        }

        public static SceneTransition GetDefaultFadeInTransition()
        {
            return Instance.DefaultFadeInTransition;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnApplicationFocus(bool focusStatus)
        {
            hasFocus = focusStatus;
        }

        protected override void Awake()
        {
            base.Awake();

            abrColor = new Color32[2];
            abrColor[0] = new Color32(0, 0, 0, byte.MaxValue);
            abrColor[1] = new Color32(0, 0, 0, byte.MaxValue);
            ClearFadeColor();
            String loadedLevelName = Application.loadedLevelName;
            LastScene = String.Empty;
            CurrentScene = loadedLevelName;
            NextScene = String.Empty;
            PendingCurrentScene = String.Empty;
            PendingNextScene = String.Empty;
            IsFading = false;
            IsReady = true;
            this.fadeTex = Texture2D.whiteTexture;
            NeedFade = true;
            this.screenTex = Texture2D.whiteTexture;
            _logger = new OSDLogger();
            _logger.Init();
            OSDLogger.Instance = this._logger;

            GameObject obj = new GameObject("SoundPlayer");
            obj.transform.parent = this.transform;
            obj.AddComponent<SoundLib>();
            obj.AddComponent<SoundLibInitializer>();
        }

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            _logger.Update();
            SiliconStudio.Social.ProcessCallbacks();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnGUI()
        {
            GUI.depth = -2147483647;
            if (IsFading && Event.current.type == EventType.Repaint)
                OnGUIFullscreenEffect();

            if (!_logger.IsShowLog)
                return;

            _logger.OnGUI();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnEnable()
        {
            _logger.EnableLog();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDisable()
        {
            _logger.DisableLog();
        }

        private void OnGUIFullscreenEffect()
        {
            Matrix4x4 matrix = GUI.matrix;
            Color color = GUI.color;
            if (Transition != SceneTransition.SwirlInBlack && Transition != SceneTransition.SwirlInWhite)
                OnGUIFade();

            GUI.color = color;
            GUI.matrix = matrix;
        }

        private void OnGUIFade()
        {
            Color color = this.curColor;
            color.a = this.fadeAmount;
            GUI.color = color;
        }

        [DebuggerHidden]
        private IEnumerator FadeIterator(float fadeOutTime, float fadeInTime, Color fadeColor, bool needChangeScene = true)
        {
            IsFading = true;
            IsReady = false;
            this.curColor = fadeColor;

            if (fadeOutTime > 0f)
            {
                fadeAmount = 0f;

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
                ChangeScene();

            yield return new WaitForEndOfFrame();

            IsReady = true;
            if (fadeInTime <= 0f)
            {
                if (needChangeScene)
                    IsFading = false;

                yield break;
            }

            this.fadeAmount = 1f;
            IsFading = true;
            this.curColor = fadeColor;

            while (this.fadeAmount > 0f)
            {
                this.fadeAmount = Mathf.Clamp01(this.fadeAmount - Time.deltaTime / fadeInTime);
                yield return new WaitForEndOfFrame();
            }

            IsFading = false;
        }

        private void ChangeScene()
        {
            Application.LoadLevel("Loading");
            Resources.UnloadUnusedAssets();
            GC.Collect();
            Application.LoadLevel(NextScene);
            SoundLib.StopAllSoundEffects();
            if (CurrentScene != NextScene)
            {
                if (String.Equals(CurrentScene, "QuadMist"))
                    FF9Wipe_FadeInEx(30);
                if (String.Equals(NextScene, "MainMenu"))
                    SoundLib.StopAllSounds();
                if (String.Equals(NextScene, FieldMapSceneName))
                {
                    FF9Snd.sndFuncPtr = FF9Snd.FF9FieldSoundDispatch;
                    if (String.Equals(CurrentScene, WorldMapSceneName))
                        FF9Snd.HasJustChangedBetweenWorldAndField = true;
                }
                else if (String.Equals(NextScene, WorldMapSceneName))
                {
                    AllSoundDispatchPlayer soundDispatchPlayer = SoundLib.GetAllSoundDispatchPlayer();
                    soundDispatchPlayer.FF9SOUND_SNDEFFECTRES_STOPCURRENT();
                    HashSet<Int32> exceptObjNo = new HashSet<Int32> {1261};
                    soundDispatchPlayer.FF9SOUND_SNDEFFECT_STOP_ALL(exceptObjNo);
                    soundDispatchPlayer.FF9SOUND_STREAM_STOP();

                    FF9Snd.sndFuncPtr = FF9Snd.FF9WorldSoundDispatch;
                    if (String.Equals(CurrentScene, FieldMapSceneName))
                        FF9Snd.HasJustChangedBetweenWorldAndField = true;
                }
                else if (!String.Equals(NextScene, BattleMapSceneName))
                {
                    FF9Snd.sndFuncPtr = !String.Equals(NextScene, "QuadMist") ? FF9Snd.FF9AllSoundDispatch : (FF9Snd.SoundDispatchDelegate)FF9Snd.FF9MiniGameSoundDispatch;
                }
                if (String.IsNullOrEmpty(PendingCurrentScene))
                {
                    LastScene = CurrentScene;
                }
                else
                {
                    LastScene = PendingCurrentScene;
                    PendingCurrentScene = String.Empty;
                }
                CurrentScene = NextScene;
                UnityEngine.Debug.Log("---------- Current Scene : " + Instance.CurrentScene + " ----------");
            }
            NextScene = String.Empty;
            SetTargetFrameRateForCurrentScene();
        }

        private void Replace(bool needFade)
        {
            NeedFade = needFade;
            if (IsFading)
                return;

            float num = 1f / FF9StateSystem.Settings.FastForwardFactor;
            IEnumerator enumerator;
            switch (Transition)
            {
                case SceneTransition.FadeOutToBlack:
                    enumerator = this.FadeIterator(this.GetFadeDuration() * num, -1f, Color.black, true);
                    break;
                case SceneTransition.FadeOutToBlack_FadeIn:
                    enumerator = this.FadeIterator(this.GetFadeDuration() * num, this.GetFadeDuration() * num, Color.black, true);
                    break;
                default:
                    enumerator = this.FadeIterator(this.GetFadeDuration() * num, this.GetFadeDuration() * num, Color.black, true);
                    break;
            }

            StartCoroutine(enumerator);
        }

        public float GetFadeDuration()
        {
            return NeedFade ? DefaultFadeTime : 0.01f;
        }

        // ReSharper disable once UnusedParameter.Global
        public void Swirl(string nextScene, SceneTransition transition)
        {
            if (IsFading)
                return;

            StartCoroutine(SwirlIterator(nextScene));
        }

        [DebuggerHidden]
        private IEnumerator SwirlIterator(string nextScene)
        {
            PendingCurrentScene = CurrentScene;
            PendingNextScene = nextScene;
            IsFading = true;
            UIManager.Field.Loading = true;
            PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(false);
            PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(false);
            PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, null);

            //int waitFrames = !FF9StateSystem.Settings.IsFastForward ? (!PersistenSingleton<UIManager>.Instance.Dialogs.Visible ? 2 : 9) : FF9StateSystem.Settings.FastForwardFactor * 2;
            //while (waitFrames > 0)
            //{
            //    yield return new WaitForEndOfFrame();
            //    waitFrames = waitFrames - 1;
            //}

            SFX_Rush.CreateScreen();

            Application.LoadLevel("SwirlScene");
            LastScene = CurrentScene;
            CurrentScene = "SwirlScene";
            UnityEngine.Debug.Log("---------- Current Scene : " + Instance.CurrentScene + " ----------");
            NextScene = string.Empty;
            yield return new WaitForEndOfFrame();

            Resources.UnloadUnusedAssets();
            GC.Collect();
            ClearFadeColor();
            IsFading = false;
        }

        public static void FadeEventSetColor(FadeMode mode, Color32 target)
        {
            SetFadeMode(mode);
            String propertyName = "_FadeColor_ABR" + (fadeMode + 1);
            abrColor[(int)fadeMode] = target;
            Shader.SetGlobalColor(propertyName, target);
        }

        public static void ToggleFadeAll(bool isEnable)
        {
            if (isEnable)
                Shader.SetGlobalInt("_FadeMode", (int)fadeMode);
            else
                Shader.SetGlobalInt("_FadeMode", -1);
        }

        private static void SetFadeMode(FadeMode mode)
        {
            fadeMode = mode;
            if (PersistenSingleton<EventEngine>.Instance.IsEventContextValid() && FF9StateSystem.Common.FF9.fldMapNo == 60 && (PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(6357) == 5 && mode == FadeMode.Add) && FF9StateSystem.Settings.IsFastForward || PersistenSingleton<EventEngine>.Instance.IsEventContextValid() && FF9StateSystem.Common.FF9.fldMapNo == 1661 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 6930 && mode == FadeMode.Add)
                Instance.StartCoroutine(SetGlobalFade());
            else
                Shader.SetGlobalInt("_FadeMode", (int)mode);
        }

        [DebuggerHidden]
        private static IEnumerator SetGlobalFade()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Shader.SetGlobalInt("_FadeMode", (int)fadeMode);
        }

        public static void ClearFadeColor()
        {
            Shader.SetGlobalColor("_FadeColor_ABR" + 1, new Color(0.0f, 0.0f, 0.0f));
            Shader.SetGlobalColor("_FadeColor_ABR" + 2, new Color(0.0f, 0.0f, 0.0f));
        }

        public static void InitFade(FadeMode mode, int frame, Color32 target)
        {
            _curFrame = 0.0f;
            _targetFrame = frame;
            _targetColor = target;
            SetFadeMode(mode);
            _prevColor = abrColor[(int)fadeMode];
        }

        public static void ServiceFade()
        {
            if (_curFrame++ > (double)_targetFrame)
                return;

            String propertyName = "_FadeColor_ABR" + (fadeMode + 1);
            Color32 color32 = Color32.Lerp(_prevColor, _targetColor, _curFrame / _targetFrame);
            abrColor[(int)fadeMode] = color32;
            Shader.SetGlobalColor(propertyName, color32);
        }

        public static void FF9Wipe_FadeInEx(int frame)
        {
            FadeEventSetColor(FadeMode.Sub, Color.white);
            InitFade(FadeMode.Sub, frame, Color.black);
        }

        public static void FF9Wipe_FadeOutEx(int frame)
        {
            FadeEventSetColor(FadeMode.Sub, Color.black);
            InitFade(FadeMode.Sub, frame, Color.white);
        }

        public static void FF9Wipe_WhiteInEx(int frame)
        {
            FadeEventSetColor(FadeMode.Add, Color.white);
            InitFade(FadeMode.Add, frame, Color.black);
        }

        public static void FF9Wipe_WhiteOutEx(int frame)
        {
            FadeEventSetColor(FadeMode.Add, Color.black);
            InitFade(FadeMode.Add, frame, Color.white);
        }

        public static void FF9Wipe_FadeIn()
        {
            FF9Wipe_FadeInEx(FF9WipeFadeinDefFrame);
        }

        public static void FF9Wipe_FadeOut()
        {
            FF9Wipe_FadeOutEx(FF9WipeFadeoutDefFrame);
        }

        public static void FF9Wipe_WhiteIn()
        {
            FF9Wipe_WhiteInEx(FF9WipeFadeinDefFrame);
        }

        public static void FF9Wipe_WhiteOut()
        {
            FF9Wipe_WhiteOutEx(FF9WipeFadeoutDefFrame);
        }

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