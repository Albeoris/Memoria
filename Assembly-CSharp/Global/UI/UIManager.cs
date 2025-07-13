using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Prime;
using Memoria.Scenes;
using Memoria.Scripts;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class UIManager : PersistenSingleton<UIManager>
{
    public static Vector2 UIActualScreenSize
    {
        get
        {
            Single pixelAdjustment = 1f;
            if (UIRoot.list.Count > 0)
                pixelAdjustment = UIRoot.list[0].pixelSizeAdjustment;
            return new Vector2(Screen.width * pixelAdjustment, Screen.height * pixelAdjustment);
        }
    }

    public static Vector3 UIPillarBoxSize
    {
        get
        {
            Single x = -(Single)UIRoot.list[0].manualWidth / 2;
            Single y = -(Single)UIRoot.list[0].manualHeight / 2;
            Camera mainCamera = UICamera.mainCamera;
            Vector3 position = UIRoot.list[0].transform.TransformPoint(new Vector3(x, y, 0f));
            return mainCamera.WorldToScreenPoint(position);
        }
    }

    public static Single UILetterBoxSize
    {
        get
        {
            Single y = -(Single)UIRoot.list[0].manualHeight / 2;
            Vector3 worldHeight = UIRoot.list[0].transform.TransformPoint(new Vector3(0f, y, 0f));
            return UICamera.mainCamera.WorldToScreenPoint(worldHeight).y;
        }
    }

    public static Vector2 UIScreenTopLeftCoOrdinate => new Vector2(-UIManager.UIActualScreenSize.x / 2f, -UIManager.UIActualScreenSize.y / 2f);
    public static Vector2 UIScreenBottomRightCoOrdinate => new Vector2(UIManager.UIActualScreenSize.x / 2f, UIManager.UIActualScreenSize.y / 2f);
    public static Vector4 UIScreenCoOrdinate => new Vector4(-UIManager.UIActualScreenSize.x / 2f, -UIManager.UIActualScreenSize.y / 2f, UIManager.UIActualScreenSize.x / 2f, UIManager.UIActualScreenSize.y / 2f);

    public UIManager.UIState State
    {
        get => this.state;
        set
        {
            if (value == UIState.FieldHUD && FieldMap.IsNarrowMap())
                Configuration.Graphics.DisableWidescreenSupportForSingleMap();
            else if (state == UIState.FieldHUD && value != UIState.Pause)
                Configuration.Graphics.RestoreDisabledWidescreenSupport();

            this.state = value;
        }
    }

    public UIManager.UIState PreviousState
    {
        get => this.prevState;
        set => this.prevState = value;
    }

    public UIManager.UIState HUDState
    {
        get
        {
            if (this.prevState == UIManager.UIState.Config || this.prevState == UIManager.UIState.QuadMistBattle)
                return this.prevState;
            switch (this.UnityScene)
            {
                case UIManager.Scene.Field:
                    return UIManager.UIState.FieldHUD;
                case UIManager.Scene.World:
                    return UIManager.UIState.WorldHUD;
                case UIManager.Scene.Battle:
                    return UIManager.UIState.BattleHUD;
                case UIManager.Scene.Title:
                    return UIManager.UIState.Title;
                case UIManager.Scene.QuadMist:
                    return UIManager.UIState.QuadMist;
                default:
                    return this.prevState;
            }
        }
    }

    public Boolean IsPause => this.state == UIManager.UIState.Pause || this.state == UIManager.UIState.Quit || this.QuitScene.isShowQuitUI;
    public Boolean IsEventEnable => (FF9StateSystem.Common.FF9.attr & 2u) == 0u;
    public Boolean IsLoading => (PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State) != null && PersistenSingleton<UIManager>.Instance.GetSceneFromState(PersistenSingleton<UIManager>.Instance.State).Loading) || PersistenSingleton<UIManager>.Instance.UnityScene == UIManager.Scene.None;

    public String StateName
    {
        get
        {
            switch (this.state)
            {
                case UIManager.UIState.Initial:
                    return "Initial";
                case UIManager.UIState.FieldHUD:
                    return "Field HUD";
                case UIManager.UIState.WorldHUD:
                    return "World HUD";
                case UIManager.UIState.BattleHUD:
                    return "Battle HUD";
                case UIManager.UIState.Pause:
                    return "Pause";
                case UIManager.UIState.MainMenu:
                    return "MainMenu";
                case UIManager.UIState.Item:
                    return "Item";
                case UIManager.UIState.Ability:
                    return "Ability";
                case UIManager.UIState.Equip:
                    return "Equip";
                case UIManager.UIState.Status:
                    return "Status";
                case UIManager.UIState.Card:
                    return "Card";
                case UIManager.UIState.Config:
                    return "Config";
                case UIManager.UIState.Serialize:
                    return "Save/Load";
                case UIManager.UIState.Cloud:
                    return "Cloud";
                case UIManager.UIState.Shop:
                    return "Shop";
                case UIManager.UIState.NameSetting:
                    return "Name Setting";
                case UIManager.UIState.BattleResult:
                    return "Battle Result";
                case UIManager.UIState.Tutorial:
                    return "Tutorial";
                case UIManager.UIState.GameOver:
                    return "GameOver";
                case UIManager.UIState.Title:
                    return "Title";
                case UIManager.UIState.QuadMist:
                    return "Card Select";
                case UIManager.UIState.QuadMistBattle:
                    return "QuadMist";
                case UIManager.UIState.Quit:
                    return "Quit";
                case UIManager.UIState.Chocograph:
                    return "Chocograph";
                case UIManager.UIState.PreEnding:
                    return "PreEnding";
                case UIManager.UIState.Ending:
                    return "Ending";
            }
            return String.Empty;
        }
    }

    public UITexture EventFadeTextureAdd
    {
        get
        {
            if (this.eventFadeTextureAdd == null)
                this.InitFadeTexture();
            return this.eventFadeTextureAdd;
        }
    }

    public UITexture EventFadeTextureSub
    {
        get
        {
            if (this.eventFadeTextureSub == null)
                this.InitFadeTexture();
            return this.eventFadeTextureSub;
        }
    }

    public static UIKeyTrigger Input => PersistenSingleton<UIManager>.Instance.gameObject?.GetComponent<UIKeyTrigger>();

    public static FieldHUD Field => PersistenSingleton<UIManager>.Instance.gameObject != null ? PersistenSingleton<UIManager>.Instance.FieldHUDScene : null;
    public static BattleHUD Battle => PersistenSingleton<UIManager>.Instance.gameObject != null ? PersistenSingleton<UIManager>.Instance.BattleHUDScene : null;
    public static WorldHUD World => PersistenSingleton<UIManager>.Instance.gameObject != null ? PersistenSingleton<UIManager>.Instance.WorldHUDScene : null;

    public Camera BattleCamera => this.battleCamera;

    public GameObject FastTrophy => this.fastTrophy;

    private void Start()
    {
        this.InitFadeTexture();
        OverlayCanvas instance = PersistenSingleton<OverlayCanvas>.Instance;
        this.fastTrophy = GameObject.Find("UI Root/Submenu Container/Fast Trophy");
        SceneDirector.GetDefaultFadeInTransition();
    }

    public void OnLevelWasLoaded(Int32 sceneNo)
    {
        this.WorldHUDScene.gameObject.SetActive(false);
        this.FieldHUDScene.gameObject.SetActive(false);
        this.BattleHUDScene.gameObject.SetActive(false);
        this.MainMenuScene.gameObject.SetActive(false);
        this.ItemScene.gameObject.SetActive(false);
        this.AbilityScene.gameObject.SetActive(false);
        this.EquipScene.gameObject.SetActive(false);
        this.StatusScene.gameObject.SetActive(false);
        this.CardScene.gameObject.SetActive(false);
        this.ConfigScene.gameObject.SetActive(false);
        this.SaveLoadScene.gameObject.SetActive(false);
        this.CloudScene.gameObject.SetActive(false);
        this.PauseScene.gameObject.SetActive(false);
        this.ShopScene.gameObject.SetActive(false);
        this.NameSettingScene.gameObject.SetActive(false);
        this.PartySettingScene.gameObject.SetActive(false);
        this.TutorialScene.gameObject.SetActive(false);
        this.BattleResultScene.gameObject.SetActive(false);
        this.GameOverScene.gameObject.SetActive(false);
        this.TitleScene.gameObject.SetActive(false);
        this.QuadMistScene.gameObject.SetActive(false);
        this.ChocographScene.gameObject.SetActive(false);
        this.EndingScene.gameObject.SetActive(false);
        this.EndGameScene.gameObject.SetActive(false);
        this.MainMenuScene.SubMenuPanel.SetActive(false);
        ButtonGroupState.DisableAllGroup(true);
        ButtonGroupState.HelpEnabled = false;
        if (this.Dialogs != null)
            this.Dialogs.CloseAll();
        String loadedLevelName = Application.loadedLevelName;
        Boolean active = false;
        Boolean isEnable = false;
        this.UnityScene = UIManager.Scene.None;
        if (loadedLevelName == SceneDirector.FieldMapSceneName)
        {
            this.UnityScene = UIManager.Scene.Field;
            this.fieldCamera = GameObject.Find("FieldMap Root/FieldMap Camera").GetComponent<Camera>();
            this.ChangeUIState(UIManager.UIState.FieldHUD);
            TimerUI.Init();
            this.FieldHUDScene.Loading = true;
            active = true;
        }
        else if (loadedLevelName == SceneDirector.BattleMapSceneName)
        {
            this.UnityScene = UIManager.Scene.Battle;
            InitializeBattleCamera();
            this.ChangeUIState(UIManager.UIState.BattleHUD);
            TimerUI.Init();
            this.BattleHUDScene.Loading = true;
            active = true;
        }
        else if (loadedLevelName == SceneDirector.WorldMapSceneName)
        {
            this.UnityScene = UIManager.Scene.World;
            this.worldCamera = GameObject.Find("WorldMapRoot/WorldCamera").GetComponent<Camera>();
            this.WorldHUDScene.Loading = true;
            active = true;
        }
        else if (loadedLevelName == "Title")
        {
            this.Booster.Initial();
            PersistenSingleton<OverlayCanvas>.Instance.Restart();
            this.UnityScene = UIManager.Scene.Title;
            this.ChangeUIState(UIManager.UIState.Title);
        }
        else if (loadedLevelName == "QuadMist")
        {
            this.UnityScene = UIManager.Scene.QuadMist;
        }
        else if (loadedLevelName == "UI")
        {
            this.UnityScene = UIManager.Scene.Pure;
            this.ChangeUIState(UIManager.UIState.FieldHUD);
        }
        else if (loadedLevelName == "FieldMapDebug")
        {
            this.UnityScene = UIManager.Scene.Field;
            isEnable = true;
            active = true;
        }
        else if (loadedLevelName == "BattleMapDebug")
        {
            this.UnityScene = UIManager.Scene.Battle;
            InitializeBattleCamera();
            this.ChangeUIState(UIManager.UIState.BattleHUD);
            TimerUI.Init();
            isEnable = true;
            active = true;
        }
        else if (loadedLevelName == "WorldMapDebug")
        {
            this.UnityScene = UIManager.Scene.World;
            this.worldCamera = GameObject.Find("WorldMapRoot/WorldCamera").GetComponent<Camera>();
            isEnable = true;
            active = true;
        }
        else if (loadedLevelName == "EndGame")
        {
            this.UnityScene = UIManager.Scene.EndGame;
        }
        else if (loadedLevelName == "SwirlScene")
        {
            this.UnityScene = UIManager.Scene.None;
        }
        else if (loadedLevelName == "Ending")
        {
            this.UnityScene = UIManager.Scene.Ending;
            isEnable = false;
            active = false;
        }
        this.Booster.CloseBoosterPanelImmediately();
        if (!FF9StateSystem.World.IsBeeScene)
        {
            PersistenSingleton<OverlayCanvas>.Instance.overlayBoosterUI.UpdateBoosterSize();
            PersistenSingleton<OverlayCanvas>.Instance.overlayBoosterUI.boosterContainer.SetActive(active);
            this.SetPlayerControlEnable(isEnable, null);
            this.SetMenuControlEnable(false);
        }
        FF9StateSystem.Settings.SetFastForward(FF9StateSystem.Settings.IsFastForward);
    }

    private void InitializeBattleCamera()
    {
        GameObject cameraObject = GameObject.Find("BattleMap Root/Battle Camera");
        //if (Configuration.Graphics.WidescreenSupport) // Always to switch an aspect in runtime by Alt+Space
        {
            cameraObject.EnsureExactComponent<PSXCameraAspect>();
        }
        this.battleCamera = cameraObject.GetComponent<Camera>();
    }

    public void AnchorToUIRoot(GameObject go)
    {
        go.GetComponent<UIWidget>().SetAnchor(base.gameObject);
    }

    public UIScene GetSceneFromState(UIManager.UIState state)
    {
        switch (state)
        {
            case UIManager.UIState.FieldHUD:
                return this.FieldHUDScene;
            case UIManager.UIState.WorldHUD:
                return this.WorldHUDScene;
            case UIManager.UIState.BattleHUD:
                return this.BattleHUDScene;
            case UIManager.UIState.Pause:
                return this.PauseScene;
            case UIManager.UIState.MainMenu:
                return this.MainMenuScene;
            case UIManager.UIState.Item:
                return this.ItemScene;
            case UIManager.UIState.Ability:
                return this.AbilityScene;
            case UIManager.UIState.Equip:
                return this.EquipScene;
            case UIManager.UIState.Status:
                return this.StatusScene;
            case UIManager.UIState.Card:
                return this.CardScene;
            case UIManager.UIState.Config:
                return this.ConfigScene;
            case UIManager.UIState.Serialize:
                return this.SaveLoadScene;
            case UIManager.UIState.Cloud:
                return this.CloudScene;
            case UIManager.UIState.Shop:
                return this.ShopScene;
            case UIManager.UIState.NameSetting:
                return this.NameSettingScene;
            case UIManager.UIState.PartySetting:
                return this.PartySettingScene;
            case UIManager.UIState.BattleResult:
                return this.BattleResultScene;
            case UIManager.UIState.Tutorial:
                return this.TutorialScene;
            case UIManager.UIState.GameOver:
                return this.GameOverScene;
            case UIManager.UIState.Title:
                return this.TitleScene;
            case UIManager.UIState.QuadMist:
                return this.QuadMistScene;
            case UIManager.UIState.Chocograph:
                return this.ChocographScene;
            case UIManager.UIState.Ending:
                return this.EndingScene;
            case UIManager.UIState.EndGame:
                return this.EndGameScene;
        }
        if (this.QuitScene.isShowQuitUI)
            return this.NoneScene;
        return null;
    }

    public void SetGameCameraEnable(Boolean isEnable)
    {
        Camera camera = null;
        switch (this.UnityScene)
        {
            case UIManager.Scene.Field:
                camera = this.fieldCamera;
                break;
            case UIManager.Scene.World:
                camera = this.worldCamera;
                break;
            case UIManager.Scene.Battle:
                camera = this.battleCamera;
                break;
        }
        if (camera != null)
            camera.enabled = isEnable;
    }

    public void SetEventEnable(Boolean isEnable)
    {
        if (isEnable)
            FF9StateSystem.Common.FF9.attr &= 0xFFFFFEFDu;
        else
            FF9StateSystem.Common.FF9.attr |= 0x102u;
    }

    public void SetPlayerControlEnable(Boolean isEnable, Action onFinished)
    {
        base.StartCoroutine(this.SetControlEnable_delay(isEnable, onFinished));
    }

    private IEnumerator SetControlEnable_delay(Boolean isEnable, Action onFinished)
    {
        yield return new WaitForEndOfFrame();
        this.IsPlayerControlEnable = isEnable;
        if (this.UnityScene == UIManager.Scene.Field)
        {
            Boolean is1655Scene = FF9StateSystem.Field.SceneName == "FBG_N31_IFTR_MAP556_IF_PTS_0" && !this.IsLoading;
            Boolean isTelescopeScene = EventHUD.CurrentHUD == MinigameHUD.Telescope;
            if (is1655Scene || isTelescopeScene)
            {
                isEnable = true;
                this.IsPlayerControlEnable = true;
            }
            if (PersistenSingleton<EventEngine>.Instance.fieldmap == null)
            {
                if (onFinished != null)
                    onFinished();
                yield break;
            }
            UIManager.PlayerActorController = PersistenSingleton<EventEngine>.Instance.fieldmap.playerController;
            if (UIManager.PlayerActorController != null)
                UIManager.PlayerActorController.SetActive(isEnable);
        }
        else if (this.UnityScene == UIManager.Scene.World)
        {
            PersistenSingleton<HonoInputManager>.Instance.SetVirtualAnalogEnable(isEnable);
            Single factor = HonoBehaviorSystem.Instance.IsFastForwardModeActive() ? HonoBehaviorSystem.Instance.GetFastForwardFactor() : 1f;
            WMScriptDirector.Instance.SetAnimationSpeeds((isEnable ? 0.667f : 0f) * factor);
            if (Singleton<VirtualAnalog>.Instance.IsActive != isEnable)
                PersistenSingleton<HonoInputManager>.Instance.SetVirtualAnalogEnable(isEnable);
        }
        else if (this.UnityScene == UIManager.Scene.Battle)
        {
            while (UIManager.battleMainBehavior == null)
            {
                GameObject go = GameObject.Find("Battle Main");
                if (go != null)
                    UIManager.battleMainBehavior = go.GetComponent<HonoluluBattleMain>();
                else
                    yield return new WaitForEndOfFrame();
            }
            UIManager.battleMainBehavior.SetActive(isEnable);
            if (!Singleton<VirtualAnalog>.Instance.IsActive)
                PersistenSingleton<HonoInputManager>.Instance.SetVirtualAnalogEnable(false);
        }
        else if (this.UnityScene == UIManager.Scene.QuadMist)
        {
            PersistenSingleton<HonoInputManager>.Instance.SetVirtualAnalogEnable(false);
        }
        if (onFinished != null)
            onFinished();
        yield break;
    }

    public void SetMenuControlEnable(Boolean isEnable)
    {
        base.StartCoroutine(this.SetMenuControlEnable_delay(isEnable));
    }

    private IEnumerator SetMenuControlEnable_delay(Boolean isEnable)
    {
        yield return new WaitForEndOfFrame();
        this.IsMenuControlEnable = isEnable;
        if (this.UnityScene == UIManager.Scene.Field)
        {
            Boolean is1655Scene = FF9StateSystem.Field.SceneName == "FBG_N31_IFTR_MAP556_IF_PTS_0";
            if (is1655Scene && this.state == UIManager.UIState.FieldHUD)
            {
                isEnable = true;
                this.IsMenuControlEnable = true;
            }
            Boolean isSpaceScene = FF9StateSystem.Field.SceneName == "FBG_N45_CYSW_MAPX21_CW_SPC_1";
            if (isSpaceScene && this.state == UIManager.UIState.FieldHUD)
            {
                isEnable = true;
                this.IsMenuControlEnable = true;
            }
            if (this.FieldHUDScene != null)
                this.FieldHUDScene.SetButtonVisible(isEnable);
        }
        else if (this.UnityScene == UIManager.Scene.World && this.WorldHUDScene != null)
        {
            this.WorldHUDScene.SetButtonVisible(isEnable);
        }
        yield break;
    }

    public void SetUIPauseEnable(Boolean isEnable)
    {
        this.IsPauseControlEnable = isEnable;
        if (this.UnityScene == UIManager.Scene.Field)
        {
            if (this.FieldHUDScene != null)
                this.FieldHUDScene.SetPauseVisible(isEnable);
        }
        else if (this.UnityScene == UIManager.Scene.World && this.WorldHUDScene != null)
        {
            this.WorldHUDScene.SetPauseVisible(isEnable);
        }
    }

    public void SetLoadingForSceneChange()
    {
        if (this.UnityScene == UIManager.Scene.Field)
            this.FieldHUDScene.Loading = true;
        else if (this.UnityScene == UIManager.Scene.World)
            this.WorldHUDScene.Loading = true;
        else if (this.UnityScene == UIManager.Scene.Battle)
            this.BattleResultScene.Loading = true;
    }

    public void MenuOpenEvent()
    {
        if (this.UnityScene == UIManager.Scene.World)
        {
            EventEngine instance = PersistenSingleton<EventEngine>.Instance;
            Int32 worldLocationId = PersistenSingleton<EventEngine>.Instance.GetSysvar(192);
            FF9StateSystem.Common.FF9.mapNameStr = FF9TextTool.WorldLocationText(worldLocationId);
            instance.Request(instance.FindObjByUID(0), 1, 4, false);
        }
    }

    public void HideAllHUD()
    {
        if (this.UnityScene == UIManager.Scene.Field || this.UnityScene == UIManager.Scene.World)
        {
            Singleton<DialogManager>.Instance.CloseAll();
            EIcon.SetHereIcon(0);
        }
    }

    public void ChangeUIState(UIManager.UIState uiState)
    {
        if (uiState != this.state)
        {
            try
            {
                this.prevState = this.state;
                this.State = uiState;
                UICamera.selectedObject = null;
                ButtonGroupState.DisableAllGroup(true);
                Singleton<HelpDialog>.Instance.SetDialogVisibility(false);
                this.Booster.CloseBoosterPanelImmediately();
                UIScene sceneFromState = this.GetSceneFromState(uiState);
                if (sceneFromState != null)
                    sceneFromState.Show(null);
                if (this.prevState == UIManager.UIState.MainMenu && this.state == UIManager.UIState.FieldHUD)
                {
                    global::Debug.Log("FIX SSTHON-3788 : Reset lazykey when state change from mainmenu to field");
                    UIManager.Input.ResetKeyCode();
                }
                // Use MenuFPS for the game menus even if the underlying SceneDirector's CurrentScene doesn't change
                if (UIManager.IsUIStateMenu(this.prevState))
                {
                    if (this.state == UIState.FieldHUD)
                        FPSManager.SetTargetFPS(Configuration.Graphics.FieldFPS);
                    else if (this.state == UIState.WorldHUD)
                        FPSManager.SetTargetFPS(Configuration.Graphics.WorldFPS);
                    else if (this.state == UIState.BattleHUD)
                        FPSManager.SetTargetFPS(Configuration.Graphics.BattleFPS);
                }
                else if (UIManager.IsUIStateMenu(this.state))
                {
                    FPSManager.SetTargetFPS(Configuration.Graphics.MenuFPS);
                }
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
        }
    }

    public void InitFadeTexture()
    {
        GameObject addTexture = GameObject.Find("UI Root/Submenu Container/Event Fader Panel/Fading Texture Add");
        if (addTexture != null)
        {
            this.eventFadeTextureAdd = addTexture.GetComponent<UITexture>();
            this.eventFadeTextureAdd.mainTexture = Texture2D.whiteTexture;
            this.eventFadeTextureAdd.material = new Material(ShadersLoader.Find("PSX/Fade_Abr_1 1"));
            if (Configuration.Graphics.WidescreenSupport)
                eventFadeTextureAdd.width = eventFadeTextureAdd.height * Screen.width / Screen.height;
        }

        GameObject subTexture = GameObject.Find("UI Root/Submenu Container/Event Fader Panel/Fading Texture Sub");
        if (subTexture != null)
        {
            this.eventFadeTextureSub = subTexture.GetComponent<UITexture>();
            this.eventFadeTextureSub.mainTexture = Texture2D.whiteTexture;
            this.eventFadeTextureSub.material = new Material(ShadersLoader.Find("PSX/Fade_Abr_2 1"));
            if (Configuration.Graphics.WidescreenSupport)
                eventFadeTextureSub.width = eventFadeTextureSub.height * Screen.width / Screen.height;
        }
    }

    public static Boolean IsUIStateMenu(UIManager.UIState state)
    {
        return (state >= UIState.MainMenu && state <= UIState.Tutorial) || state == UIState.QuadMist || state == UIState.Chocograph;
    }

    public static Vector2 UIContentSize = GetUIContentSize();

    private static Vector2 GetUIContentSize()
    {
        const Int32 contentWidth = 1543;
        const Int32 contentHeight = 1080;
        if (Configuration.Graphics.InitializeWidescreenSupport())
        {
            Single aspect = (Single)Screen.width / Screen.height;
            return new Vector2(contentHeight * aspect, contentHeight);
        }
        else
        {
            return new Vector2(contentWidth, contentHeight);
        }
    }

    public static Vector2 OriginScreenSize = new Vector2(FieldMap.PsxFieldWidth, FieldMap.PsxFieldHeightNative);

    public static volatile Single ResourceXMultipier = CalcResourceXMultipier();
    public static volatile Single ResourceYMultipier = CalcResourceYMultipier();

    private static FieldMapActorController PlayerActorController;
    private static HonoluluBattleMain battleMainBehavior;

    [SerializeField]
    private UIManager.UIState state;
    [SerializeField]
    private UIManager.UIState prevState;

    private Camera fieldCamera;
    private Camera battleCamera;
    private Camera worldCamera;

    private GameObject fastTrophy;

    private UITexture eventFadeTextureAdd;
    private UITexture eventFadeTextureSub;

    public UIManager.Scene UnityScene;

    public String CurrentButtonGroup;

    public Boolean IsPlayerControlEnable;
    public Boolean IsMenuControlEnable;
    public Boolean IsPauseControlEnable = true;
    public Boolean IsWarningDialogEnable;

    [Header("Scene")]
    public FieldHUD FieldHUDScene;
    public WorldHUD WorldHUDScene;
    public BattleHUD BattleHUDScene;
    public MainMenuUI MainMenuScene;
    public ItemUI ItemScene;
    public AbilityUI AbilityScene;
    public EquipUI EquipScene;
    public StatusUI StatusScene;
    public CardUI CardScene;
    public ConfigUI ConfigScene;
    public SaveLoadUI SaveLoadScene;
    public CloudUI CloudScene;
    public PauseUI PauseScene;
    public ShopUI ShopScene;
    public NameSettingUI NameSettingScene;
    public PartySettingUI PartySettingScene;
    public TutorialUI TutorialScene;
    public BattleResultUI BattleResultScene;
    public GameOverUI GameOverScene;
    public TitleUI TitleScene;
    public QuadMistUI QuadMistScene;
    public QuitUI QuitScene;
    public ChocographUI ChocographScene;
    public EndingUI EndingScene;
    public EndGameHUD EndGameScene;
    public NoneUI NoneScene;

    [Header("ETC")]
    public DialogManager Dialogs;
    public BoosterSlider Booster;

    public enum UIState
    {
        Initial,
        FieldHUD,
        WorldHUD,
        BattleHUD,
        Pause,
        MainMenu,
        Item,
        Ability,
        Equip,
        Status,
        Card,
        Config,
        Serialize,
        Cloud,
        Shop,
        NameSetting,
        PartySetting,
        BattleResult,
        Tutorial,
        GameOver,
        Title,
        QuadMist,
        QuadMistBattle,
        Quit,
        Chocograph,
        PreEnding,
        Ending,
        EndGame
    }

    public enum Scene
    {
        Bundle,
        Field,
        World,
        Battle,
        Title,
        QuadMist,
        Pure,
        Ending,
        EndGame,
        None
    }

    public void OnWidescreenSupportChanged()
    {
        UIContentSize = GetUIContentSize();
        ResourceXMultipier = CalcResourceXMultipier();
        ResourceYMultipier = CalcResourceYMultipier();

        if (eventFadeTextureAdd != null)
            eventFadeTextureAdd.width = eventFadeTextureAdd.height * Screen.width / Screen.height;

        if (eventFadeTextureSub != null)
            eventFadeTextureSub.width = eventFadeTextureSub.height * Screen.width / Screen.height;
    }

    private static Single CalcResourceYMultipier() => UIContentSize.y / OriginScreenSize.y;
    private static Single CalcResourceXMultipier() => UIContentSize.x / OriginScreenSize.x;

    public static void DebugLogComponents(GameObject startGo, Func<Component, String> logger = null, String indent = "")
    {
        if (logger == null)
        {
            logger = c =>
            {
                if (c is Transform t)
                {
                    if (t.parent != null)
                        return $"{t.gameObject} is the child {t.GetSiblingIndex()} of {t.parent.gameObject}";
                    return $"{t.gameObject} is a root node";
                }
                String res = $"└─> {c.GetType()}";
                if (c is UIRect rect)
                {
                    Vector3[] corners = rect.worldCorners;
                    res += $" - Position: ({corners[0].x}, {corners[0].y}), Size: ({corners[2].x - corners[0].x}, {corners[2].y - corners[0].y})";
                    if (rect.leftAnchor.target != null)
                        res += $", LeftAnchor: ({rect.leftAnchor.target.gameObject}, {rect.leftAnchor.absolute}, {rect.leftAnchor.relative})";
                    if (rect.rightAnchor.target != null)
                        res += $", RightAnchor: ({rect.rightAnchor.target.gameObject}, {rect.rightAnchor.absolute}, {rect.rightAnchor.relative})";
                    if (rect.bottomAnchor.target != null)
                        res += $", BottomAnchor: ({rect.bottomAnchor.target.gameObject}, {rect.bottomAnchor.absolute}, {rect.bottomAnchor.relative})";
                    if (rect.topAnchor.target != null)
                        res += $", TopAnchor: ({rect.topAnchor.target.gameObject}, {rect.topAnchor.absolute}, {rect.topAnchor.relative})";
                    if (c is UIWidget widget)
                        res += $", Depth: {widget.depth}";
                    if (c is UISprite sprite)
                        res += $", SpriteName: {sprite.spriteName}, Atlas: {sprite.atlas?.name ?? "[No Atlas]"}";
                    if (c is UILabel label)
                        res += $", Label: {label.rawText.Replace("\n", "\\n")}";
                }
                if (c is UILocalize localize)
                    res += $" - Localize: {localize.key}";
                if (c is ButtonGroupState buttonGroup)
                    res += $" - ButtonGroup: {buttonGroup.GroupName}";
                return res;
            };
        }
        foreach (Component comp in startGo.GetComponents<Component>())
        {
            String log = logger(comp);
            if (!String.IsNullOrEmpty(log))
                Log.Message($"[UIManager] {indent}{log}");
        }
        for (Int32 i = 0; i < startGo.transform.childCount; i++)
            DebugLogComponents(startGo.GetChild(i), logger, indent + " ");
    }

    public static void DebugLogStackTrace()
    {
        StackTrace trace = new StackTrace();
        String log = "'";
        for (Int32 i = 1; i < trace.FrameCount; i++)
        {
            if (i > 1)
                log += "' -> '";
            var method = trace.GetFrame(i).GetMethod();
            if (method.DeclaringType != null)
                log += method.DeclaringType.Name + "." + method.Name;
            else
                log += method.Name;
        }
        log += "'";
        Log.Message($"[UIManager] Stack Trace: {log}");
    }
}
