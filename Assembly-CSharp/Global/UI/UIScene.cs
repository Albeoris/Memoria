using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using Memoria.Scenes;
using System;
using UnityEngine;

public class UIScene : MonoBehaviour
{
    protected HonoFading FadingComponent
    {
        get => this.fading;
        set => this.fading = value;
    }

    public Boolean NextSceneIsModal
    {
        get => !this.isNeedFade;
        set
        {
            this.isNeedFade = !value;
            this.isNeedToHide = !value;
        }
    }

    public Boolean Loading
    {
        get => this.isLoading;
        set
        {
            this.isLoading = value;
            this.isNeedUpdate = true;
            this.Update();
        }
    }

    public Boolean ShowPointerWhenLoading
    {
        set
        {
            this.showPointerWhenLoading = value;
        }
    }

    public virtual void Show(UIScene.SceneVoidDelegate afterShowDelegate = null)
    {
        base.gameObject.SetActive(true);
        this.DisplayWindowBackground();
        this.AfterSceneShown = this.AfterShow;
        if (afterShowDelegate != null)
            this.AfterSceneShown += afterShowDelegate;
        if (this.fading != null && this.isNeedFade)
        {
            this.fading.fadeOutDuration = FF9StateSystem.Settings.IsFastForward ? Configuration.Interface.FadeDuration / FF9StateSystem.Settings.FastForwardFactor : Configuration.Interface.FadeDuration;
            this.fading.FadeOut(this.AfterSceneShown);
            this.Loading = true;
        }
        else
        {
            this.AfterSceneShown();
        }
        Type thisType = base.GetType();
        if (thisType == typeof(FieldHUD) || thisType == typeof(WorldHUD))
            Singleton<BubbleUI>.Instance.SetGameObjectActive(true);
    }

    private void AfterShow()
    {
        this.Loading = false;
        this.isNeedToHide = true;
        this.isNeedFade = true;
    }

    public virtual void Hide(UIScene.SceneVoidDelegate afterHideDelegate = null)
    {
        this.AfterSceneHidden = this.AfterHide;
        if (afterHideDelegate != null)
            this.AfterSceneHidden += afterHideDelegate;
        if (this.fading != null && this.isNeedFade)
        {
            this.fading.fadeInDuration = FF9StateSystem.Settings.IsFastForward ? Configuration.Interface.FadeDuration / FF9StateSystem.Settings.FastForwardFactor : Configuration.Interface.FadeDuration;
            this.fading.FadeIn(this.AfterSceneHidden);
            this.Loading = true;
        }
        else
        {
            this.AfterSceneHidden();
        }
        Type thisType = base.GetType();
        if (thisType == typeof(FieldHUD) || thisType == typeof(WorldHUD))
            Singleton<BubbleUI>.Instance.SetGameObjectActive(false);
    }

    private void AfterHide()
    {
        this.Loading = false;
        if (this.isNeedToHide)
            base.gameObject.SetActive(false);
        if (this.FadingComponent != null)
            this.FadingComponent.ForegroundSprite.alpha = 0f;
    }

    public virtual GameObject OnKeyNavigate(KeyCode direction, GameObject currentObj, GameObject nextObj)
    {
        return nextObj;
    }

    public virtual Boolean OnKeyConfirm(GameObject go)
    {
        if (this.isLoading)
            return false;
        if (PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI)
        {
            PersistenSingleton<UIManager>.Instance.QuitScene.OnKeyConfirm(go);
            return false;
        }
        if (go == null)
            go = ButtonGroupState.ActiveButton;
        return true;
    }

    public virtual Boolean OnKeyCancel(GameObject go)
    {
        if (this.isLoading)
            return false;
        if (PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI)
        {
            PersistenSingleton<UIManager>.Instance.QuitScene.OnKeyCancel(go);
            return false;
        }
        if (go == null)
            go = ButtonGroupState.ActiveButton;
        return true;
    }

    public virtual Boolean OnKeyMenu(GameObject go)
    {
        return !this.isLoading;
    }

    public virtual Boolean OnKeySpecial(GameObject go)
    {
        return !this.isLoading;
    }

    public virtual Boolean OnKeySelect(GameObject go)
    {
        if (this.isLoading)
            return false;
        ButtonGroupState.ToggleHelp(true);
        return true;
    }

    [NonSerialized] private Single _lastPausePressed;

    public virtual Boolean OnKeyPause(GameObject go)
    {
        Single currentTime = Time.realtimeSinceStartup;
        if (Math.Abs(currentTime - _lastPausePressed) < 1)
            return false;

        Boolean doPause = !this.isLoading && !PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI && UICamera.selectedObject?.GetComponent<UIInput>() == null;
        if (doPause)
            _lastPausePressed = currentTime;
        return doPause;
    }

    public virtual Boolean OnKeyLeftBumper(GameObject go)
    {
        if (this.isLoading)
            return false;
        ScrollButton activeScrollButton = ButtonGroupState.ActiveScrollButton;
        if (activeScrollButton)
            activeScrollButton.OnPageUpButtonClick();
        return true;
    }

    public virtual Boolean OnKeyRightBumper(GameObject go)
    {
        if (this.isLoading)
            return false;
        ScrollButton activeScrollButton = ButtonGroupState.ActiveScrollButton;
        if (activeScrollButton)
            activeScrollButton.OnPageDownButtonClick();
        return true;
    }

    public virtual Boolean OnKeyLeftTrigger(GameObject go)
    {
        return !this.isLoading;
    }

    public virtual Boolean OnKeyRightTrigger(GameObject go)
    {
        return !this.isLoading;
    }

    public virtual void OnKeyQuit()
    {
        PersistenSingleton<UIManager>.Instance.QuitScene.Show(null);
    }

    public void ShowQuitUI(Action onResumeFromQuit = null)
    {
        PersistenSingleton<UIManager>.Instance.QuitScene.Show(onResumeFromQuit);
    }

    public virtual void OnListItemClick(GameObject go)
    {
        if (SceneDirector.IsBattleScene() ? Configuration.Control.DisableMouseInBattles : Configuration.Control.DisableMouseForMenus)
            return;
        this.onPress(go, false);
    }

    public virtual Boolean OnItemSelect(GameObject go)
    {
        return !this.isLoading && go != PersistenSingleton<UIManager>.Instance.gameObject;
    }

    public virtual void onPress(GameObject go, Boolean isDown)
    {
        if (!isDown)
        {
            Int32 currentTouchID = UICamera.currentTouchID;
            switch (currentTouchID + 2)
            {
                case 1:
                    this.OnKeyConfirm(go);
                    break;
                case 2:
                case 3:
                    if (ButtonGroupState.ContainButtonInSecondaryGroup(go))
                    {
                        this.OnKeyConfirm(go);
                    }
                    else
                    {
                        ButtonGroupState buttonGroup = go.GetComponent<ButtonGroupState>();
                        if (buttonGroup)
                        {
                            if (buttonGroup.ProcessTouch())
                                this.OnKeyConfirm(go);
                        }
                        else
                        {
                            this.OnKeyConfirm(go);
                        }
                    }
                    break;
            }
        }
    }

    public virtual void onClick(GameObject go)
    {
        if (SceneDirector.IsBattleScene() ? Configuration.Control.DisableMouseInBattles : Configuration.Control.DisableMouseForMenus)
            return;
        this.onPress(go, false);
    }

    public virtual void DisplayWindowBackground()
    {
        this.DisplayWindowBackground(base.gameObject, null);
    }

    public virtual void DisplayWindowBackground(GameObject go, UIAtlas forceColor = null)
    {
        UIAtlas uiatlas = forceColor == null ? FF9UIDataTool.WindowAtlas : forceColor;
        UISprite[] allSprites = go.GetComponentsInChildren<UISprite>(true);
        foreach (UISprite uisprite in allSprites)
            if (uisprite.gameObject.tag == "Window Color" && uisprite.atlas != FF9UIDataTool.GeneralAtlas && uisprite.atlas != FF9UIDataTool.IconAtlas && uisprite.atlas != uiatlas)
                uisprite.atlas = uiatlas;
    }

    protected Boolean CheckAndroidTVModule(Control keyCode)
    {
        return FF9StateSystem.EnableAndroidTVJoystickMode && FF9StateSystem.AndroidTVPlatform && PersistenSingleton<HonoInputManager>.Instance.GetSource(keyCode) == SourceControl.Joystick;
    }

    private void Update()
    {
        if (this.isNeedUpdate)
        {
            this.isNeedUpdate = false;
            if (!this.isLoading && !String.IsNullOrEmpty(ButtonGroupState.ActiveGroup))
            {
                if (!this.showPointerWhenLoading)
                    Singleton<PointerManager>.Instance.SetAllPointerVisibility(true);
                ButtonGroupState.SetActiveGroupEnable(true);
            }
            else if (this.isLoading)
            {
                if (!this.showPointerWhenLoading)
                    Singleton<PointerManager>.Instance.SetAllPointerVisibility(false);
                ButtonGroupState.SetActiveGroupEnable(false);
            }
        }
    }

    /// <summary>Rarely, Yes/No dialog prompts are distinct UILabels instead of being part of a [CHOO] text</summary>
    public static void SetupYesNoLabels(GameObject yesGo, GameObject noGo, UIEventListener.VoidDelegate onButtonClick = null)
    {
        UILabel yesLabel = yesGo.GetComponent<UILabel>();
        UILabel noLabel = noGo.GetComponent<UILabel>();
        if (onButtonClick != null)
        {
            UIEventListener.Get(yesGo).Click += onButtonClick;
            UIEventListener.Get(noGo).Click += onButtonClick;
        }
        if (Configuration.Control.WrapSomeMenus)
        {
            yesGo.GetExactComponent<UIKeyNavigation>().wrapUpDown = true;
            noGo.GetExactComponent<UIKeyNavigation>().wrapUpDown = true;
        }
        yesLabel.alignment = NGUIText.Alignment.Center;
        noLabel.alignment = NGUIText.Alignment.Center;
        yesLabel.SetRawRect((yesLabel.pivotOffset.x - 0.5f) * yesLabel.width, yesLabel.transform.localPosition.y, 120f, yesLabel.height);
        //noLabel.SetRawRect(0f, noLabel.transform.localPosition.y, 120f, noLabel.height); // Unneeded: it is anchored to yesLabel
    }

    public UIScene.SceneVoidDelegate AfterSceneShown;
    public UIScene.SceneVoidDelegate AfterSceneHidden;

    [SerializeField]
    private Boolean isLoading;
    private Boolean isNeedUpdate;
    private Boolean isNeedFade = true;
    private Boolean isNeedToHide = true;
    private HonoFading fading;
    private Boolean showPointerWhenLoading;

    public delegate void SceneVoidDelegate();
}
