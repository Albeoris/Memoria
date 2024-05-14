using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using Memoria;
using System;
using UnityEngine;

public class UIScene : MonoBehaviour
{
    protected HonoFading FadingComponent
    {
        get
        {
            return this.fading;
        }
        set
        {
            this.fading = value;
        }
    }

    public Boolean NextSceneIsModal
    {
        get
        {
            return !this.isNeedFade;
        }
        set
        {
            this.isNeedFade = !value;
            this.isNeedToHide = !value;
        }
    }

    public Boolean Loading
    {
        get
        {
            return this.isLoading;
        }
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

    public virtual void Show(UIScene.SceneVoidDelegate action = null)
    {
        base.gameObject.SetActive(true);
        this.DisplayWindowBackground();
        this.AfterSceneShown = new UIScene.SceneVoidDelegate(this.AfterShow);
        if (this.AfterSceneShown != null)
        {
            this.AfterSceneShown = (UIScene.SceneVoidDelegate)Delegate.Combine(this.AfterSceneShown, action);
        }
        if (this.fading != (UnityEngine.Object)null && this.isNeedFade)
        {
            if (Configuration.Interface.DisableFading)
                this.fading.fadeOutDuration = 0f;
            else
                this.fading.fadeOutDuration = ((!FF9StateSystem.Settings.IsFastForward) ? 0.3f : 0.15f);

            this.fading.FadeOut(this.AfterSceneShown);
            this.Loading = true;
        }
        else
        {
            this.AfterSceneShown();
        }
        Type type = base.GetType();
        if (type == typeof(FieldHUD) || type == typeof(WorldHUD))
        {
            Singleton<BubbleUI>.Instance.SetGameObjectActive(true);
        }
    }

    private void AfterShow()
    {
        this.Loading = false;
        this.isNeedToHide = true;
        this.isNeedFade = true;
    }

    public virtual void Hide(UIScene.SceneVoidDelegate action = null)
    {
        this.AfterSceneHidden = new UIScene.SceneVoidDelegate(this.AfterHide);
        if (this.AfterSceneHidden != null)
        {
            this.AfterSceneHidden = (UIScene.SceneVoidDelegate)Delegate.Combine(this.AfterSceneHidden, action);
        }
        if (this.fading != (UnityEngine.Object)null && this.isNeedFade)
        {
            if (Configuration.Interface.DisableFading)
                this.fading.fadeInDuration = 0f;
            else
                this.fading.fadeInDuration = ((!FF9StateSystem.Settings.IsFastForward) ? 0.3f : 0.15f);

            this.fading.FadeIn(this.AfterSceneHidden);
            this.Loading = true;
        }
        else
        {
            this.AfterSceneHidden();
        }
        Type type = base.GetType();
        if (type == typeof(FieldHUD) || type == typeof(WorldHUD))
        {
            Singleton<BubbleUI>.Instance.SetGameObjectActive(false);
        }
    }

    private void AfterHide()
    {
        this.Loading = false;
        if (this.isNeedToHide)
        {
            base.gameObject.SetActive(false);
        }
        if (this.FadingComponent != (UnityEngine.Object)null)
        {
            this.FadingComponent.ForegroundSprite.alpha = 0f;
        }
    }

    public virtual GameObject OnKeyNavigate(KeyCode direction, GameObject currentObj, GameObject nextObj)
    {
        return nextObj;
    }

    public virtual Boolean OnKeyConfirm(GameObject go)
    {
        if (this.isLoading)
        {
            return false;
        }
        if (PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI)
        {
            PersistenSingleton<UIManager>.Instance.QuitScene.OnKeyConfirm(go);
            return false;
        }
        if (go == (UnityEngine.Object)null)
        {
            go = ButtonGroupState.ActiveButton;
        }
        return true;
    }

    public virtual Boolean OnKeyCancel(GameObject go)
    {
        if (this.isLoading)
        {
            return false;
        }
        if (PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI)
        {
            PersistenSingleton<UIManager>.Instance.QuitScene.OnKeyCancel(go);
            return false;
        }
        if (go == (UnityEngine.Object)null)
        {
            go = ButtonGroupState.ActiveButton;
        }
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
        {
            return false;
        }
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
        {
            return false;
        }
        ScrollButton activeScrollButton = ButtonGroupState.ActiveScrollButton;
        if (activeScrollButton)
        {
            activeScrollButton.OnPageUpButtonClick();
        }
        return true;
    }

    public virtual Boolean OnKeyRightBumper(GameObject go)
    {
        if (this.isLoading)
        {
            return false;
        }
        ScrollButton activeScrollButton = ButtonGroupState.ActiveScrollButton;
        if (activeScrollButton)
        {
            activeScrollButton.OnPageDownButtonClick();
        }
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
        PersistenSingleton<UIManager>.Instance.QuitScene.Show((Action)null);
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
        return !this.isLoading && !(go == PersistenSingleton<UIManager>.Instance.gameObject);
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
                        ButtonGroupState component = go.GetComponent<ButtonGroupState>();
                        if (component)
                        {
                            if (component.ProcessTouch())
                            {
                                this.OnKeyConfirm(go);
                            }
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
        this.DisplayWindowBackground(base.gameObject, (UIAtlas)null);
    }

    public virtual void DisplayWindowBackground(GameObject go, UIAtlas forceColor = null)
    {
        UIAtlas uiatlas = (!(forceColor != (UnityEngine.Object)null)) ? FF9UIDataTool.WindowAtlas : forceColor;
        UISprite[] componentsInChildren = go.GetComponentsInChildren<UISprite>(true);
        UISprite[] array = componentsInChildren;
        for (Int32 i = 0; i < (Int32)array.Length; i++)
        {
            UISprite uisprite = array[i];
            GameObject gameObject = uisprite.gameObject;
            if (gameObject.tag == "Window Color" && uisprite.atlas != FF9UIDataTool.GeneralAtlas && uisprite.atlas != FF9UIDataTool.IconAtlas && uisprite.atlas != uiatlas)
            {
                uisprite.atlas = uiatlas;
            }
        }
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
                {
                    Singleton<PointerManager>.Instance.SetAllPointerVisibility(true);
                }
                ButtonGroupState.SetActiveGroupEnable(true);
            }
            else if (this.isLoading)
            {
                if (!this.showPointerWhenLoading)
                {
                    Singleton<PointerManager>.Instance.SetAllPointerVisibility(false);
                }
                ButtonGroupState.SetActiveGroupEnable(false);
            }
        }
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
