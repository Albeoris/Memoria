using System;
using System.Collections;
using System.Diagnostics;
using Memoria.Data;
using Memoria.Assets;
using UnityEngine;

// ReSharper disable UnusedParameter.Global
// ReSharper disable RedundantExplicitArraySize
// ReSharper disable UnusedMember.Local
// ReSharper disable UnassignedField.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable EmptyConstructor

public class FieldHUD : UIScene
{
    public GameObject MinigameHUDContainer;
    private GameObject _chanbaraHUDPrefab;
    private GameObject _auctionHUDPrefab;
    private GameObject _mogTutorialHUDPrefab;
    private GameObject _jumpingRopeHUDPrefab;
    private GameObject _racingHippaulHUDPrefab;
    private GameObject _swingACageHUDPrefab;
    private MinigameHUD _currentMinigameHUD;
    private GameObject _currentMinigameHUDGameObject;
    private GameObject _chocoHotInstructionHUDGameObject;
    public Boolean ShowDebugButton;
    public GameObject MenuButtonGameObject;
    public GameObject PauseButtonGameObject;
    public GameObject HelpButtonGameObject;
    public GameObject ScreenFadeGameObject;
    public GameObject ATEGameObject;
    public GameObject MovieHitArea;
    //private GameObject _boosterSliderGameObject;
    private Boolean _previousDebugState;
    private Int32 _pauseWidth;
    public Boolean isShowSkipMovieDialog;
    private Single _previousVibLeft;
    private Single _previousVibRight;
    private EventEngine _eventEngineCache;

    public MinigameHUD CurrentMinigameHUD => _currentMinigameHUD;

    public Int32 PauseWidth
    {
        get
        {
            if (_pauseWidth == 0)
                _pauseWidth = PauseButtonGameObject.GetComponent<UISprite>().width;
            return _pauseWidth;
        }
    }

    public EventEngine eventEngine => _eventEngineCache ?? (_eventEngineCache = GameObject.Find("EventEngine").GetComponent<EventEngine>());

    public FieldHUD()
    {
    }

    public Boolean IsDisplayChanbaraHUD()
    {
        return _currentMinigameHUD == MinigameHUD.Chanbara;
    }

    public Boolean IsDisplayAuctionHUD()
    {
        return _currentMinigameHUD == MinigameHUD.Auction;
    }

    public Boolean IsDisplayTutorialHUD()
    {
        return _currentMinigameHUD == MinigameHUD.MogTutorial;
    }

    public Boolean IsDisplayJumpRopeHUD()
    {
        return _currentMinigameHUD == MinigameHUD.JumpingRope;
    }

    public Boolean IsDisplayTelescopeHUD()
    {
        return _currentMinigameHUD == MinigameHUD.Telescope;
    }

    public Boolean IsDisplayRacingHippaulHUD()
    {
        return _currentMinigameHUD == MinigameHUD.RacingHippaul;
    }

    public Boolean IsDisplaySwingACageHUD()
    {
        return _currentMinigameHUD == MinigameHUD.SwingACage;
    }

    public Boolean IsDisplayGetTheKeyHUD()
    {
        return _currentMinigameHUD == MinigameHUD.GetTheKey;
    }

    public Boolean IsDisplayChocoHot()
    {
        return _currentMinigameHUD == MinigameHUD.ChocoHot;
    }

    public Boolean IsDisplayChocoHotInstruction()
    {
        return _currentMinigameHUD == MinigameHUD.ChocoHotInstruction;
    }

    public Boolean IsDisplayPandoniumElevator()
    {
        return _currentMinigameHUD == MinigameHUD.PandoniumElevator;
    }

    public void DisplaySpecialHUD(MinigameHUD minigameHUD)
    {
        if (!FF9StateSystem.MobilePlatform)
            return;
        _currentMinigameHUD = minigameHUD;
        switch (minigameHUD)
        {
            case MinigameHUD.Chanbara:
                if (_chanbaraHUDPrefab == null)
                    _chanbaraHUDPrefab = Resources.Load("EmbeddedAsset/UI/Prefabs/Chanbara HUD Container") as GameObject;
                _currentMinigameHUDGameObject = NGUITools.AddChild(MinigameHUDContainer, _chanbaraHUDPrefab);
                break;
            case MinigameHUD.Auction:
            case MinigameHUD.PandoniumElevator:
                if (_auctionHUDPrefab == null)
                    _auctionHUDPrefab = Resources.Load("EmbeddedAsset/UI/Prefabs/Auction HUD Container") as GameObject;
                _currentMinigameHUDGameObject = NGUITools.AddChild(MinigameHUDContainer, _auctionHUDPrefab);
                if (minigameHUD == MinigameHUD.PandoniumElevator)
                {
                    _currentMinigameHUDGameObject.transform.GetChild(0).gameObject.SetActive(false);
                    _currentMinigameHUDGameObject.transform.GetChild(2).gameObject.SetActive(false);
                }
                StartCoroutine(SetAuctionHUDDepth(_currentMinigameHUDGameObject));
                break;
            case MinigameHUD.MogTutorial:
                if (_mogTutorialHUDPrefab == null)
                    _mogTutorialHUDPrefab = Resources.Load("EmbeddedAsset/UI/Prefabs/Mognet Tutorial HUD Container") as GameObject;
                _currentMinigameHUDGameObject = NGUITools.AddChild(MinigameHUDContainer, _mogTutorialHUDPrefab);
                break;
            case MinigameHUD.JumpingRope:
            case MinigameHUD.Telescope:
            case MinigameHUD.GetTheKey:
            case MinigameHUD.ChocoHot:
                if (_jumpingRopeHUDPrefab == null)
                    _jumpingRopeHUDPrefab = Resources.Load("EmbeddedAsset/UI/Prefabs/Jumping Rope HUD Container") as GameObject;
                _currentMinigameHUDGameObject = NGUITools.AddChild(MinigameHUDContainer, _jumpingRopeHUDPrefab);
                if (_currentMinigameHUD == MinigameHUD.ChocoHot)
                {
                    Transform child = _currentMinigameHUDGameObject.transform.GetChild(0);
                    child.GetComponent<OnScreenButton>().KeyCommand = Control.Special;
                    UISprite component1 = child.GetComponent<UISprite>();
                    UIButton component2 = child.GetComponent<UIButton>();
                    component1.spriteName = "button_chocobo_dig_idle";
                    component2.normalSprite = component1.spriteName;
                    component2.pressedSprite = "button_chocobo_dig_act";
                }
                break;
            case MinigameHUD.RacingHippaul:
                if (_racingHippaulHUDPrefab == null)
                    _racingHippaulHUDPrefab = Resources.Load("EmbeddedAsset/UI/Prefabs/Racing Hippaul HUD Container") as GameObject;
                _currentMinigameHUDGameObject = NGUITools.AddChild(MinigameHUDContainer, _racingHippaulHUDPrefab);
                if (FF9StateSystem.Settings.CurrentLanguage == "Japanese")
                {
                    _currentMinigameHUDGameObject.transform.GetChild(1).GetComponent<EventButton>().KeyCommand = Control.Confirm;
                }
                break;
            case MinigameHUD.SwingACage:
                if (_swingACageHUDPrefab == null)
                    _swingACageHUDPrefab = Resources.Load("EmbeddedAsset/UI/Prefabs/Swing a Cage HUD Container") as GameObject;
                _currentMinigameHUDGameObject = NGUITools.AddChild(MinigameHUDContainer, _swingACageHUDPrefab);
                break;
            case MinigameHUD.ChocoHotInstruction:
                if (_chocoHotInstructionHUDGameObject == null)
                    _chocoHotInstructionHUDGameObject = Resources.Load("EmbeddedAsset/UI/Prefabs/Choco Hot Instruction HUD Container") as GameObject;
                _currentMinigameHUDGameObject = NGUITools.AddChild(MinigameHUDContainer, _chocoHotInstructionHUDGameObject);
                _currentMinigameHUDGameObject.GetComponent<UIPanel>().depth = Dialog.DialogAdditionalRaiseDepth + Dialog.DialogMaximumDepth - Convert.ToInt32(Dialog.WindowID.ID0) * 2 + 2;
                break;
        }
        if (!(_currentMinigameHUDGameObject != null))
            return;
        UIWidget component3 = _currentMinigameHUDGameObject.GetComponent<UIWidget>();
        Int32 num = Singleton<DialogManager>.Instance.Widget.depth + 1;
        if (component3 != null)
            component3.depth = num++;

        foreach (Component component in _currentMinigameHUDGameObject.transform)
        {
            UIWidget widget = component.GetComponent<UIWidget>();
            if (widget != null)
                widget.depth = num;
        }
    }

    public void DestroySpecialHUD()
    {
        _currentMinigameHUD = MinigameHUD.None;
        if (!FF9StateSystem.MobilePlatform)
            return;

        Destroy(_currentMinigameHUDGameObject);
    }

    [DebuggerHidden]
    private IEnumerator SetAuctionHUDDepth(GameObject currentMinigameHUDGameObject)
    {
        yield return new WaitForEndOfFrame();

        if (currentMinigameHUDGameObject != null)
        {
            Int32 childCount = currentMinigameHUDGameObject.transform.childCount;
            Int32 buttonDepth = currentMinigameHUDGameObject.transform.GetChild(childCount - 1).GetComponent<UISprite>().depth + 1;
            for (Int32 i = 0; i < childCount - 1; i = i + 1)
                currentMinigameHUDGameObject.transform.GetChild(i).GetComponent<UISprite>().depth = buttonDepth;
        }
    }

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        SceneVoidDelegate action = OnShownAction;
        if (afterFinished != null)
            action = (SceneVoidDelegate)Delegate.Combine(action, afterFinished);
        base.Show(action);
        PersistenSingleton<UIManager>.Instance.Booster.SetBoosterState(PersistenSingleton<UIManager>.Instance.UnityScene);
        VirtualAnalog.Init(gameObject);
        VirtualAnalog.FallbackTouchWidgetList.Add(PersistenSingleton<UIManager>.Instance.gameObject);
        VirtualAnalog.FallbackTouchWidgetList.Add(PersistenSingleton<UIManager>.Instance.Dialogs.gameObject);
        VirtualAnalog.FallbackTouchWidgetList.Add(PersistenSingleton<UIManager>.Instance.Booster.OutsideBoosterHitPoint);
        PersistenSingleton<UIManager>.Instance.SetGameCameraEnable(true);
    }

    public override void Hide(SceneVoidDelegate afterFinished = null)
    {
        SceneVoidDelegate action = OnHideAfterHide;
        if (afterFinished != null)
            action = (SceneVoidDelegate)Delegate.Combine(action, afterFinished);
        base.Hide(action);
        PauseButtonGameObject.SetActive(false);
        HelpButtonGameObject.SetActive(false);
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, null);
        PersistenSingleton<UIManager>.Instance.SetEventEnable(false);
    }

    public override Boolean OnKeyMenu(GameObject go)
    {
        if (base.OnKeyMenu(go))
        {
            if (PersistenSingleton<UIManager>.Instance.Dialogs.Visible)
                PersistenSingleton<UIManager>.Instance.HideAllHUD();
            Hide(OnKeyMenuAfterHide);
            PersistenSingleton<UIManager>.Instance.MainMenuScene.NeedTweenAndHideSubMenu = true;
        }
        return true;
    }

    public override Boolean OnKeyPause(GameObject go)
    {
        if (base.OnKeyPause(go) && !isShowSkipMovieDialog)
        {
            NextSceneIsModal = true;
            Hide(OnKeyPauseAfterHide);
        }
        return true;
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (base.OnKeyConfirm(go) && MovieHitArea.activeSelf && (!MBG.IsNull && !MBG.Instance.IsFinished()))
        {
            MovieHitArea.SetActive(false);
            ETb.sChoose = 1;
            Dialog dialog = Singleton<DialogManager>.Instance.AttachDialog(Localization.Get("SkipMovieDialog"), 0, 0, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStylePlain, Vector2.zero, Dialog.CaptionType.Notice);
            isShowSkipMovieDialog = true;
            dialog.AfterDialogShown = OnKeyConfirmAfterDialogShown;
            dialog.AfterDialogHidden = OnKeyConfirmAfterDialogHidden;
        }
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (base.OnKeyCancel(go) && !PersistenSingleton<UIManager>.Instance.Dialogs.Visible && (EventHUD.CurrentHUD == MinigameHUD.None && UIManager.Input.ContainsAndroidQuitKey()))
            UIManager.Input.OnQuitCommandDetected();
        return true;
    }

    public void OnPressButton(GameObject go, Boolean isPress)
    {
        if (!isPress)
            return;
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, null);
    }

    public void EnableATE(Boolean isEnable, ATEType ateType)
    {
        ATEGameObject.GetComponent<ActiveTimeEvent>().EnableATE(isEnable, ateType);
    }

    public void InitializeATEText()
    {
        ATEGameObject.GetComponent<ActiveTimeEvent>().InitializeATE();
    }

    public void OnATEClick()
    {
        EnableATE(true, ATEType.Blue);
    }

    public void OnItemShopClick(GameObject go, Boolean isClicked)
    {
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, OnItemShopClickHide);
    }

    public void OnWeaponShopClick(GameObject go, Boolean isClicked)
    {
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, OnWeaponShopClickHide);
    }

    public void OnSynthesisShopClick(GameObject go, Boolean isClicked)
    {
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, OnSynthesisShopClickHide);
    }

    public void OnNameSettingClick(GameObject go, Boolean isClicked)
    {
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, OnNameSettingClickHide);
    }

    public void OnTutorialClick(GameObject go, Boolean isClicked)
    {
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, OnTutorialClickHide);
    }

    public void OnTitleClick(GameObject go, Boolean isClicked)
    {
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, OnTitleClickHide);
    }

    public void OnDialogClick(GameObject go, Boolean isClicked)
    {
        NextSceneIsModal = true;
        Hide(OnDialogClickAttach);
    }

    public void OnSaveClick(GameObject go, Boolean isClicked)
    {
        Hide(OnSaveClickSwitchScene);
    }

    public void OnPartyClick(GameObject go, Boolean isClicked)
    {
        Hide(OnPartyClickSwitchScene);
    }

    public void SetButtonVisible(Boolean isVisible)
    {
        if (!FF9StateSystem.MobilePlatform)
            return;
        MenuButtonGameObject.SetActive(isVisible);
        HelpButtonGameObject.SetActive(isVisible);
    }

    public void SetPauseVisible(Boolean isVisible)
    {
        if (!FF9StateSystem.MobilePlatform)
            return;
        PauseButtonGameObject.SetActive(isVisible);
    }

    private void Update()
    {
        if (_previousDebugState == ShowDebugButton)
            return;
        _previousDebugState = ShowDebugButton;

        gameObject.GetChild(1).SetActive(ShowDebugButton);
    }

    private void Awake()
    {
        FadingComponent = ScreenFadeGameObject.GetComponent<HonoFading>();

        UIEventListener uiEventListener1 = UIEventListener.Get(MenuButtonGameObject);
        uiEventListener1.Press += OnPressButton;

        UIEventListener uiEventListener2 = UIEventListener.Get(PauseButtonGameObject);
        uiEventListener2.Press += OnPressButton;
    }

    private void OnShownAction()
    {
        PersistenSingleton<UIManager>.Instance.SetEventEnable(true);
        PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(PersistenSingleton<EventEngine>.Instance.GetUserControl() && EventInput.IsMenuON && EventInput.IsMovementControl);
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(PersistenSingleton<EventEngine>.Instance.GetUserControl(), null);
        PauseButtonGameObject.SetActive(PersistenSingleton<UIManager>.Instance.IsPauseControlEnable && FF9StateSystem.MobilePlatform);
        ButtonGroupState.HelpEnabled = false;
    }

    private void OnHideAfterHide()
    {
        if (NextSceneIsModal)
            return;
        PersistenSingleton<UIManager>.Instance.SetGameCameraEnable(false);
    }

    private static void OnKeyMenuAfterHide()
    {
        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.MainMenu);
    }

    private static void OnKeyPauseAfterHide()
    {
        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Pause);
    }

    private void OnKeyConfirmAfterDialogShown(Int32 choice)
    {
        _previousVibLeft = vib.CurrentVibrateLeft;
        _previousVibRight = vib.CurrentVibrateRight;
        vib.VIB_actuatorReset(0);
        vib.VIB_actuatorReset(1);
    }

    private void OnKeyConfirmAfterDialogHidden(Int32 choice)
    {
        if (choice == 0)
        {
            MBG.IsSkip = true;
            fldfmv.FF9FieldFMVShutdown();
        }
        else if (!MBG.IsNull && !MBG.Instance.IsFinished())
        {
            MovieHitArea.SetActive(true);
            _previousVibLeft = vib.CurrentVibrateLeft;
            _previousVibRight = vib.CurrentVibrateRight;
            vib.VIB_actuatorSet(0, _previousVibLeft, _previousVibRight);
            vib.VIB_actuatorSet(1, _previousVibLeft, _previousVibRight);
        }
        isShowSkipMovieDialog = false;
    }

    private void OnItemShopClickHide()
    {
        Hide(OnItemShopClickAfterHide);
    }

    private void OnWeaponShopClickHide()
    {
        Hide(OnWeaponShopClickAfterHide);
    }

    private void OnSynthesisShopClickHide()
    {
        Hide(OnSynthesisShopClickAfterHide);
    }

    private void OnNameSettingClickHide()
    {
        Hide(OnNameSettingClickAfterHide);
    }

    private void OnTutorialClickHide()
    {
        NextSceneIsModal = true;
        Hide(OnTutorialClickAfterHide);
    }

    private void OnTitleClickHide()
    {
        Hide(OnTitleClickAfterHide);
    }

    private static void OnDialogClickAttach()
    {
        Singleton<DialogManager>.Instance.AttachDialog(NGUIText.GetTestingResource(), 0, 0, Dialog.TailPosition.AutoPosition, Dialog.WindowStyle.WindowStylePlain, Vector2.zero, Dialog.CaptionType.None);
    }

    private static void OnSaveClickSwitchScene()
    {
        PersistenSingleton<UIManager>.Instance.SaveLoadScene.Type = SaveLoadUI.SerializeType.Save;
        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Serialize);
    }

    private static void OnPartyClickSwitchScene()
    {
        FF9PARTY_INFO defaultParty = new FF9PARTY_INFO
        {
            menu = new CharacterId[4] { CharacterId.Zidane, CharacterId.Vivi, CharacterId.Garnet, CharacterId.Steiner },
            select = new CharacterId[4] { CharacterId.Freya, CharacterId.Quina, CharacterId.Eiko, CharacterId.Amarant }
        };
        defaultParty.fix = new Boolean[FF9StateSystem.Common.PlayerCount];
        defaultParty.fix[0] = true;
        defaultParty.party_ct = 4;
        PersistenSingleton<UIManager>.Instance.PartySettingScene.Info = defaultParty;
        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.PartySetting);
    }

    private static void OnItemShopClickAfterHide()
    {
        PersistenSingleton<UIManager>.Instance.ShopScene.Id = 25;
        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Shop);
    }

    private static void OnWeaponShopClickAfterHide()
    {
        PersistenSingleton<UIManager>.Instance.ShopScene.Id = 0;
        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Shop);
    }

    private static void OnSynthesisShopClickAfterHide()
    {
        PersistenSingleton<UIManager>.Instance.ShopScene.Id = 34;
        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Shop);
    }

    private static void OnNameSettingClickAfterHide()
    {
        PersistenSingleton<UIManager>.Instance.NameSettingScene.SubNo = CharacterId.Vivi;
        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.NameSetting);
    }

    private static void OnTutorialClickAfterHide()
    {
        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Tutorial);
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, null);
    }

    private static void OnTitleClickAfterHide()
    {
        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Title);
    }
}