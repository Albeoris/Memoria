using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Memoria;
using Memoria.Prime;
using UnityEngine;

#pragma warning disable 169
#pragma warning disable 414
#pragma warning disable 649

// ReSharper disable ArrangeThisQualifier
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable NotAccessedField.Local
// ReSharper disable ConvertToConstant.Global
// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable InconsistentNaming

public class ConfigUI : UIScene
{
    private enum TriggerState
    {

        Idle,

        Waiting,

        Triggered
    }

    public enum Configurator
    {
        Sound,
        SoundEffect,
        Controller,
        Cursor,
        ATB,
        BattleCamera,
        SkipBattleCamera,
        Movement,
        FieldMessage,
        BattleSpeed,
        HereIcon,
        WindowColor,
        Vibration,
        ControlTutorial,
        CombatTutorial,
        Title,
        QuitGame
    }

    public GameObject KeyboardButton;

    public GameObject JoystickButton;

    public GameObject CustomControllerKeyboardPanel;

    public GameObject CustomControllerJoystickPanel;

    public GameObject CustomControllerMobilePanel;

    private HonoTweenPosition controllerKeyboardTransition;

    private HonoTweenPosition controllerJoystickTransition;

    private List<ControllerField> ControllerKeyboardList = new List<ControllerField>();

    private List<ControllerField> ControllerJoystickList = new List<ControllerField>();

    private Boolean[] inputBool =
    {
        true,
        true,
        true,
        true,
        true,
        true,
        true,
        true
    };

    private Boolean[] hasJoyAxisSignal =
    {
        true,
        true,
        true,
        true,
        true,
        true,
        true,
        true
    };

    private Int32 customControllerCount = 8;

    private Int32 currentControllerIndex;

    private ControllerType currentControllerType;

    private String[] PCJoystickNormalButtons =
    {
        "JoystickButton0",
        "JoystickButton1",
        "JoystickButton2",
        "JoystickButton3",
        "JoystickButton4",
        "JoystickButton5"
    };

    private String[] iOSJoystickNormalButtons =
    {
        "JoystickButton13",
        "JoystickButton14",
        "JoystickButton12",
        "JoystickButton15",
        "JoystickButton8",
        "JoystickButton9",
        "JoystickButton10",
        "JoystickButton11"
    };

    private ConfigUI.TriggerState leftTrigger;

    private ConfigUI.TriggerState rightTrigger;

    private float leftTriggerTime;

    private float rightTriggerTime;

    private float triggerDelay = 0.1f;

    public GameObject ConfigList;

    public GameObject HelpDespLabelGameObject;

    public GameObject ScreenFadeGameObject;

    public GameObject WarningDialog;

    public GameObject WarningDialogHitPoint;

    public GameObject BoosterPanel;

    public GameObject TransitionGroup;

    public GameObject ControlPanelGroup;

    private static String ConfigGroupButton = "Config.Config";

    private static String WarningMenuGroupButton = "Config.Warning";

    private static String CustomControllerGroupButton = "Config.Controller";

    private static String ControllerTypeGroupButton = "Config.ControllerType";

    private static List<Configurator> ConfigSliderIdList = new List<Configurator>(new[]
    {
        Configurator.FieldMessage,
        Configurator.BattleSpeed
    });

    private List<ConfigField> ConfigFieldList;

    private SnapDragScrollView configScrollView;

    private ScrollButton configScrollButton;

    private OnScreenButton hitpointScreenButton;

    private GameObject toTitleGameObject;

    private GameObject masterSkillButtonGameObject;

    private GameObject lvMaxButtonGameObject;

    private GameObject gilMaxButtonGameObject;

    private GameObject backButtonGameObject;

    private UILabel masterSkillLabel;

    private UILabel lvMaxLabel;

    private UILabel gilMaxLabel;

    private HonoTweenClipping warningTransition;

    private Single fieldMessageSliderStep = 6f;

    private Single battleSpeedSliderStep = 2f;

    private Boolean fastSwitch;

    private Boolean cursorInList = true;

    private Boolean is_vibe;

    private Int32 vibe_tick;

    private Boolean helpEnable;

    [DebuggerHidden]
    private IEnumerator ShowButtonGroupDalay()
    {
        if (FF9StateSystem.aaaaPlatform || FF9StateSystem.IOSPlatform) // aaaa is Vita
        {
            ButtonGroupState.RemoveCursorMemorize(CustomControllerGroupButton);
            ButtonGroupState.ActiveGroup = CustomControllerGroupButton;
            ButtonGroupState.HoldActiveStateOnGroup(ConfigGroupButton);
            yield return null;
        }

        KeyboardButton.SetActive(true);
        JoystickButton.SetActive(true);
        yield return new WaitForEndOfFrame();

        ButtonGroupState.RemoveCursorMemorize(ControllerTypeGroupButton);
        ButtonGroupState.SetCursorStartSelect(currentControllerType != ControllerType.Keyboard ? JoystickButton : KeyboardButton, ControllerTypeGroupButton);
        ButtonGroupState.ActiveGroup = ControllerTypeGroupButton;
        ButtonGroupState.HoldActiveStateOnGroup(ConfigGroupButton);
        yield return new WaitForEndOfFrame();

        ButtonGroupState.RemoveCursorMemorize(CustomControllerGroupButton);
        ButtonGroupState.ActiveGroup = CustomControllerGroupButton;
        ButtonGroupState.HoldActiveStateOnGroup(ControllerTypeGroupButton);
    }

    private void DisplayCustomControllerPanel(ControllerType controllerType)
    {
        if (controllerType != ControllerType.Keyboard)
        {
            if (controllerType == ControllerType.Joystick)
            {
                if (FF9StateSystem.MobilePlatform)
                {
                    CustomControllerMobilePanel.SetActive(true);
                    customControllerCount = CustomControllerMobilePanel.GetChild(0).transform.childCount;
                    backButtonGameObject.GetComponent<OnScreenButton>().KeyCommand = Control.Pause;
                }
                else
                {
                    CustomControllerJoystickPanel.SetActive(true);
                    customControllerCount = CustomControllerJoystickPanel.GetChild(0).transform.childCount;
                }
            }
        }
        else if (FF9StateSystem.MobilePlatform)
        {
            CustomControllerKeyboardPanel.SetActive(true);
            customControllerCount = CustomControllerKeyboardPanel.GetChild(0).transform.childCount;
            backButtonGameObject.GetComponent<OnScreenButton>().KeyCommand = Control.Pause;
        }
        else
        {
            CustomControllerKeyboardPanel.SetActive(true);
            customControllerCount = CustomControllerKeyboardPanel.GetChild(0).transform.childCount;
        }
    }

    private void CloseCustomControllerPanel(ControllerType controllerType)
    {
        if (controllerType == ControllerType.Keyboard)
        {
            CustomControllerKeyboardPanel.SetActive(false);
        }
        else if (controllerType == ControllerType.Joystick)
        {
            if (FF9StateSystem.MobilePlatform)
            {
                CustomControllerMobilePanel.SetActive(false);
            }
            else
            {
                CustomControllerJoystickPanel.SetActive(false);
            }
        }
        backButtonGameObject.GetComponent<OnScreenButton>().KeyCommand = Control.Cancel;
    }

    private void DrawNormalButton(Int32 index, KeyCode keycode)
    {
        FF9UIDataTool.DrawLabel(ControllerKeyboardList[index].NormalController.GetChild(0), keycode);
    }

    private void DrawNewButton(Int32 index, KeyCode keycode)
    {
        FF9UIDataTool.DrawLabel(ControllerKeyboardList[index].NewController.GetChild(0), keycode);
    }

    private void DrawNormalButton(Int32 index, String key)
    {
        FF9UIDataTool.DrawSprite(ControllerJoystickList[index].NormalController, FF9UIDataTool.IconAtlas, FF9UIDataTool.GetJoystickSpriteByName(key));
    }

    private void DrawNewButton(Int32 index, String key)
    {
        FF9UIDataTool.DrawSprite(ControllerJoystickList[index].NewController, FF9UIDataTool.IconAtlas, FF9UIDataTool.GetJoystickSpriteByName(key));
    }

    private void SetControllerSettings()
    {
        PersistenSingleton<HonoInputManager>.Instance.SetPrimaryKeys();
    }

    private void InitializeCustomControllerKeyboard()
    {
        if (FF9StateSystem.MobilePlatform)
        {
            if (FF9StateSystem.AndroidPlatform)
            {
                this.CustomControllerKeyboardPanel.GetChild(2).GetChild(0).GetComponent<UILocalize>().key = "ControlPressBackspace";
            }
            else
            {
                this.CustomControllerKeyboardPanel.GetChild(2).GetChild(0).GetComponent<UILocalize>().key = "MobileControlPressStart";
            }
        }
        customControllerCount = CustomControllerKeyboardPanel.GetChild(0).transform.childCount;
        foreach (Transform trans in CustomControllerKeyboardPanel.GetChild(0).transform)
        {
            Int32 siblingIndex = trans.GetSiblingIndex();
            ControllerField controllerField = new ControllerField();
            GameObject obj = trans.gameObject;
            controllerField.NewController = obj.GetChild(0);
            controllerField.NormalController = obj.GetChild(2);
            ControllerKeyboardList.Add(controllerField);
            DrawNormalButton(siblingIndex, PersistenSingleton<HonoInputManager>.Instance.DefaultInputKeys[siblingIndex]);
        }
        SetCurrentKeyboardKey();
    }

    private void InitializeCustomControllerJoystick()
    {
        GameObject value = (!FF9StateSystem.MobilePlatform) ? CustomControllerJoystickPanel : CustomControllerMobilePanel;
        if (FF9StateSystem.MobilePlatform)
        {
            value = this.CustomControllerMobilePanel;
            if (FF9StateSystem.AndroidTVPlatform)
            {
                value.GetChild(2).GetChild(0).GetComponent<UILocalize>().key = "AndroidTVControlPressStart";
            }
        }
        customControllerCount = value.GetChild(0).transform.childCount;
        foreach (Transform trans in value.GetChild(0).transform)
        {
            Int32 siblingIndex = trans.GetSiblingIndex();
            ControllerField controllerField = new ControllerField();
            GameObject obj = trans.gameObject;
            controllerField.NewController = obj.GetChild(0);
            controllerField.NormalController = obj.GetChild(2);
            ControllerJoystickList.Add(controllerField);
            DrawNormalButton(siblingIndex, PersistenSingleton<HonoInputManager>.Instance.DefaultJoystickInputKeys[siblingIndex]);
        }
        SetCurrentJoystickKey();
    }

    private void SetCurrentKeyboardKey()
    {
        KeyCode[] inputKeysPrimary = PersistenSingleton<HonoInputManager>.Instance.InputKeysPrimary;
        for (Int32 i = 0; i < customControllerCount; i++)
        {
            FF9StateSystem.Settings.cfg.control_data_keyboard[i] = inputKeysPrimary[i];
            DrawNewButton(i, FF9StateSystem.Settings.cfg.control_data_keyboard[i]);
        }
    }

    private void SetCurrentJoystickKey()
    {
        String[] joystickKeysPrimary = PersistenSingleton<HonoInputManager>.Instance.JoystickKeysPrimary;
        for (Int32 i = 0; i < customControllerCount; i++)
        {
            FF9StateSystem.Settings.cfg.control_data_joystick[i] = joystickKeysPrimary[i];
            DrawNewButton(i, FF9StateSystem.Settings.cfg.control_data_joystick[i]);
        }
    }

    private void CheckDuplicate(KeyCode newKey)
    {
        for (Int32 i = 0; i < customControllerCount; i++)
        {
            if (newKey == FF9StateSystem.Settings.cfg.control_data_keyboard[i])
            {
                FF9StateSystem.Settings.cfg.control_data_keyboard[i] = FF9StateSystem.Settings.cfg.control_data_keyboard[currentControllerIndex];
                DrawNewButton(i, FF9StateSystem.Settings.cfg.control_data_keyboard[currentControllerIndex]);
            }
        }
    }

    private void CheckDuplicate(String newKey)
    {
        for (Int32 i = 0; i < customControllerCount; i++)
        {
            if (newKey == FF9StateSystem.Settings.cfg.control_data_joystick[i])
            {
                FF9StateSystem.Settings.cfg.control_data_joystick[i] = FF9StateSystem.Settings.cfg.control_data_joystick[currentControllerIndex];
                DrawNewButton(i, FF9StateSystem.Settings.cfg.control_data_joystick[currentControllerIndex]);
            }
        }
    }

    private void CheckKeyboardKeys()
    {
        if (ButtonGroupState.ActiveGroup != CustomControllerGroupButton)
        {
            return;
        }
        if (currentControllerType != ControllerType.Keyboard)
        {
            return;
        }
        if (Event.current.type == EventType.KeyDown && inputBool[currentControllerIndex] && Event.current.keyCode != KeyCode.None && Event.current.keyCode != PersistenSingleton<HonoInputManager>.Instance.InputKeysPrimary[8] && Event.current.keyCode != PersistenSingleton<HonoInputManager>.Instance.InputKeysPrimary[9] && HonoInputManager.AcceptKeyCodeList.Contains(Event.current.keyCode) && !UnityXInput.Input.GetButtonDown("Vertical") && !UnityXInput.Input.GetButtonDown("Horizontal"))
        {
            FF9Sfx.FF9SFX_Play(103);
            inputBool[currentControllerIndex] = false;
            KeyCode keyCode = Event.current.keyCode;
            CheckDuplicate(keyCode);
            FF9StateSystem.Settings.cfg.control_data_keyboard[currentControllerIndex] = keyCode;
            DrawNewButton(currentControllerIndex, keyCode);
        }
        if (Event.current.type == EventType.KeyUp)
        {
            inputBool[currentControllerIndex] = true;
        }
    }

    private void ValidateKeyboard()
    {
    }

    private void ValidateController()
    {
    }

    private void ChangeCustomKey(String keyCode, Int32 controllerIndex)
    {
        CheckDuplicate(keyCode);
        FF9StateSystem.Settings.cfg.control_data_joystick[controllerIndex] = keyCode;
        DrawNewButton(controllerIndex, keyCode);
    }

    private Boolean CheckJoystickNormalButton(String[] buttonNames, Int32 controllerIndex)
    {
        Boolean result = false;
        for (Int32 i = 0; i < buttonNames.Length; i++)
        {
            String text = buttonNames[i];
            if (UnityXInput.Input.GetButtonDown(text))
            {
                result = true;
                ChangeCustomKey(text, controllerIndex);
            }
        }
        return result;
    }

    private void CheckJoystickKeys()
    {
        if (ButtonGroupState.ActiveGroup != CustomControllerGroupButton)
        {
            return;
        }
        if (currentControllerType != ControllerType.Joystick)
        {
            return;
        }
        if (FF9StateSystem.PCPlatform || FF9StateSystem.aaaaPlatform || Application.isEditor) // aaaa is Vita
        {
            CheckPCVitaJoystickKeys();
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            if (FF9StateSystem.AndroidTVPlatform)
            {
                this.CheckAndroidTVJoystickKeys();
            }
            else
            {
                this.CheckAndroidJoystickKeys();
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            CheckiOSJoystickKeys();
        }
    }

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    private void CheckPCVitaJoystickKeys()
    {
        Boolean flag = CheckJoystickNormalButton(PCJoystickNormalButtons, currentControllerIndex);
        if (UnityXInput.Input.GetAxisRaw("LeftTrigger") != 0f && !hasJoyAxisSignal[0])
        {
            flag = true;
            hasJoyAxisSignal[0] = true;
            ChangeCustomKey("LeftTrigger", currentControllerIndex);
        }
        if (UnityXInput.Input.GetAxisRaw("LeftTrigger") == 0f)
        {
            hasJoyAxisSignal[0] = false;
        }
        if (UnityXInput.Input.GetAxisRaw("RightTrigger") != 0f && !hasJoyAxisSignal[1])
        {
            flag = true;
            hasJoyAxisSignal[1] = true;
            ChangeCustomKey("RightTrigger", currentControllerIndex);
        }
        if (UnityXInput.Input.GetAxisRaw("RightTrigger") == 0f)
        {
            hasJoyAxisSignal[1] = false;
        }
        if (FF9StateSystem.MobilePlatform && UnityXInput.Input.GetButtonDown("JoystickButton6"))
        {
            flag = true;
            ChangeCustomKey("JoystickButton6", currentControllerIndex);
        }
        if (flag)
        {
            FF9Sfx.FF9SFX_Play(103);
        }
    }

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    private void CheckAndroidJoystickKeys()
    {
        Boolean flag = CheckJoystickNormalButton(PCJoystickNormalButtons, currentControllerIndex);
        if (UnityXInput.Input.GetAxisRaw("LeftTrigger Android") != 0f && !hasJoyAxisSignal[0])
        {
            flag = true;
            hasJoyAxisSignal[0] = true;
            ChangeCustomKey("LeftTrigger Android", currentControllerIndex);
        }
        if (UnityXInput.Input.GetAxisRaw("LeftTrigger Android") == 0f)
        {
            hasJoyAxisSignal[0] = false;
        }
        if (UnityXInput.Input.GetAxisRaw("RightTrigger Android") != 0f && !hasJoyAxisSignal[1])
        {
            flag = true;
            hasJoyAxisSignal[1] = true;
            ChangeCustomKey("RightTrigger Android", currentControllerIndex);
        }
        if (UnityXInput.Input.GetAxisRaw("RightTrigger Android") == 0f)
        {
            hasJoyAxisSignal[1] = false;
        }
        if (PersistenSingleton<HonoInputManager>.Instance.IsRightAnalogDown)
        {
            flag = true;
            ChangeCustomKey("Empty", currentControllerIndex);
        }
        if (flag)
        {
            FF9Sfx.FF9SFX_Play(103);
        }
    }

    private void CheckAndroidTVJoystickKeys()
    {
        bool flag = this.CheckJoystickNormalButton(this.PCJoystickNormalButtons, this.currentControllerIndex);
        if (UnityXInput.Input.GetAxisRaw("LeftTrigger Android") != 0f)
        {
            ConfigUI.TriggerState triggerState = this.leftTrigger;
            if (triggerState != ConfigUI.TriggerState.Idle)
            {
                if (triggerState == ConfigUI.TriggerState.Waiting)
                {
                    if (Time.realtimeSinceStartup - this.leftTriggerTime > this.triggerDelay)
                    {
                        flag = true;
                        this.leftTrigger = ConfigUI.TriggerState.Triggered;
                        this.ChangeCustomKey("LeftTrigger Android", this.currentControllerIndex);
                    }
                }
            }
            else
            {
                this.leftTrigger = ConfigUI.TriggerState.Waiting;
                this.leftTriggerTime = Time.realtimeSinceStartup;
            }
        }
        if (UnityXInput.Input.GetAxisRaw("LeftTrigger Android") == 0f)
        {
            this.leftTrigger = ConfigUI.TriggerState.Idle;
        }
        if (UnityXInput.Input.GetAxisRaw("RightTrigger Android") != 0f)
        {
            ConfigUI.TriggerState triggerState = this.rightTrigger;
            if (triggerState != ConfigUI.TriggerState.Idle)
            {
                if (triggerState == ConfigUI.TriggerState.Waiting)
                {
                    if (Time.realtimeSinceStartup - this.rightTriggerTime > this.triggerDelay)
                    {
                        flag = true;
                        this.rightTrigger = ConfigUI.TriggerState.Triggered;
                        this.ChangeCustomKey("RightTrigger Android", this.currentControllerIndex);
                    }
                }
            }
            else
            {
                this.rightTrigger = ConfigUI.TriggerState.Waiting;
                this.rightTriggerTime = Time.realtimeSinceStartup;
            }
        }
        if (UnityXInput.Input.GetAxisRaw("RightTrigger Android") == 0f)
        {
            this.rightTrigger = ConfigUI.TriggerState.Idle;
        }
        if (PersistenSingleton<HonoInputManager>.Instance.IsRightAnalogDown)
        {
            flag = true;
            this.ChangeCustomKey("Empty", this.currentControllerIndex);
        }
        if (flag)
        {
            FF9Sfx.FF9SFX_Play(103);
        }
    }

    private void CheckiOSJoystickKeys()
    {
        Boolean flag = CheckJoystickNormalButton(iOSJoystickNormalButtons, currentControllerIndex);
        if (PersistenSingleton<HonoInputManager>.Instance.IsRightAnalogDown)
        {
            flag = true;
            ChangeCustomKey("Empty", currentControllerIndex);
        }
        if (flag)
        {
            FF9Sfx.FF9SFX_Play(103);
        }
    }

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        SceneVoidDelegate sceneVoidDelegate = delegate
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.SubMenuPanel.SetActive(false);
            WarningDialogHitPoint.SetActive(false);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(52f, 0f), ConfigGroupButton);
            ButtonGroupState.SetPointerOffsetToGroup(new Vector2(-30f, 10f), WarningMenuGroupButton);
            ButtonGroupState.SetPointerDepthToGroup(7, ControllerTypeGroupButton);
            ButtonGroupState.SetPointerDepthToGroup(7, CustomControllerGroupButton);
            ButtonGroupState.SetPointerLimitRectToGroup(ConfigList.GetComponent<UIWidget>(), configScrollView.ItemHeight, ConfigGroupButton);
            ButtonGroupState.SetScrollButtonToGroup(configScrollButton, ConfigGroupButton);
            ButtonGroupState.HelpEnabled = false;
            ButtonGroupState.ActiveGroup = ConfigGroupButton;
        };
        if (afterFinished != null)
        {
            sceneVoidDelegate = (SceneVoidDelegate)Delegate.Combine(sceneVoidDelegate, afterFinished);
        }
        SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
        base.Show(sceneVoidDelegate);
        DisplayConfigValue();
        DisplayHelp();
        InitializeCustomControllerJoystick();
        InitializeCustomControllerKeyboard();
        HelpDespLabelGameObject.SetActive(FF9StateSystem.PCPlatform);
        if (PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.MainMenu)
        {
            helpEnable = ButtonGroupState.HelpEnabled;
        }
        ButtonGroupState.HelpEnabled = false;
        HelpDespLabelGameObject.SetActive(false);
        NGUIText.ForceShowButton = true;
    }

    public override void Hide(SceneVoidDelegate afterFinished = null)
    {
        ButtonGroupState.HelpEnabled = helpEnable;
        base.Hide(afterFinished);
        if (!fastSwitch)
        {
            PersistenSingleton<UIManager>.Instance.MainMenuScene.StartSubmenuTweenIn();
            RemoveCursorMemorize();
        }
        NGUIText.ForceShowButton = false;
    }

    private void RemoveCursorMemorize()
    {
        ConfigField configField = ConfigFieldList.First(field => field.Configurator == Configurator.Sound);
        ButtonGroupState.SetCursorStartSelect(configField.ConfigParent, ConfigGroupButton);
        ButtonGroupState.RemoveCursorMemorize(ConfigGroupButton);
    }

    public override Boolean OnKeySelect(GameObject go)
    {
        return false;
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (!base.OnKeyConfirm(go))
            return true;

        if (ButtonGroupState.ActiveGroup == ConfigGroupButton)
        {
            if (go.GetParent() == BoosterPanel)
            {
                OnBoosterPanelKeyConfirm(go);
            }
            else
            {
                Int32 siblingIndex = go.transform.GetSiblingIndex();
                if (siblingIndex == 2)
                {
                    FF9Sfx.FF9SFX_Play(103);
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (ConfigFieldList[2].Value == 1f)
                    {
                        CheckAndDisplayCustomControllerPanel();
                    }
                }
                else if (siblingIndex == 15)
                {
                    FF9Sfx.FF9SFX_Play(103);
                    hitpointScreenButton.KeyCommand = Control.None;
                    WarningDialogHitPoint.SetActive(true);
                    Loading = true;
                    warningTransition.TweenIn(delegate
                    {
                        Loading = false;
                        ButtonGroupState.RemoveCursorMemorize(WarningMenuGroupButton);
                        ButtonGroupState.ActiveGroup = WarningMenuGroupButton;
                        ButtonGroupState.HoldActiveStateOnGroup(ConfigGroupButton);
                    });
                }
                else if (siblingIndex == 13)
                {
                    FF9Sfx.FF9SFX_Play(103);
                    hitpointScreenButton.KeyCommand = Control.Confirm;
                    WarningDialogHitPoint.SetActive(true);
                    NextSceneIsModal = true;
                    fastSwitch = true;
                    Hide(delegate
                    {
                        TutorialUI tutorialScene = PersistenSingleton<UIManager>.Instance.TutorialScene;
                        tutorialScene.DisplayMode = TutorialUI.Mode.BasicControl;
                        tutorialScene.BasicControlTutorialID = 0;
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Tutorial);
                        ButtonGroupState.HoldActiveStateOnGroup(ConfigGroupButton);
                    });
                }
                else if (siblingIndex == 14)
                {
                    FF9Sfx.FF9SFX_Play(103);
                    NextSceneIsModal = true;
                    fastSwitch = true;
                    Hide(delegate
                    {
                        TutorialUI tutorialScene = PersistenSingleton<UIManager>.Instance.TutorialScene;
                        tutorialScene.DisplayMode = TutorialUI.Mode.Battle;
                        PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Tutorial);
                        ButtonGroupState.HoldActiveStateOnGroup(ConfigGroupButton);
                    });
                }
                else if (siblingIndex == 16)
                {
                    FF9Sfx.FF9SFX_Play(103);
                    UIManager.Input.OnQuitCommandDetected();
                }
            }
        }
        else if (ButtonGroupState.ActiveGroup == WarningMenuGroupButton)
        {
            Int32 siblingIndex2 = go.transform.GetSiblingIndex();
            Int32 num = siblingIndex2;
            if (num != 2)
            {
                if (num == 3)
                {
                    FF9Sfx.FF9SFX_Play(101);
                    Loading = true;
                    WarningDialogHitPoint.SetActive(false);
                    warningTransition.TweenOut(delegate
                    {
                        Loading = false;
                    });
                    ButtonGroupState.ActiveGroup = ConfigGroupButton;
                }
            }
            else
            {
                FF9Sfx.FF9SFX_Play(103);
                WarningDialogHitPoint.SetActive(false);
                warningTransition.TweenOut();
                fastSwitch = true;
                Hide(delegate
                {
                    TimerUI.SetEnable(false);
                    ButtonGroupState.DisableAllGroup();
                    SceneDirector.Replace("Title");
                });
                Loading = true;
                RemoveCursorMemorize();
                PersistenSingleton<UIManager>.Instance.MainMenuScene.StartSubmenuTweenIn();
                PersistenSingleton<UIManager>.Instance.MainMenuScene.SetSubmenuVisibility(false);
                PersistenSingleton<UIManager>.Instance.MainMenuScene.NeedTweenAndHideSubMenu = true;
                PersistenSingleton<UIManager>.Instance.MainMenuScene.CurrentSubMenu = MainMenuUI.SubMenu.Item;
            }
        }
        else if (ButtonGroupState.ActiveGroup == ControllerTypeGroupButton)
        {
            FF9Sfx.FF9SFX_Play(103);
            ButtonGroupState.RemoveCursorMemorize(CustomControllerGroupButton);
            ButtonGroupState.ActiveGroup = CustomControllerGroupButton;
            ButtonGroupState.HoldActiveStateOnGroup(ControllerTypeGroupButton);
            if (FF9StateSystem.MobilePlatform)
            {
                backButtonGameObject.GetComponent<OnScreenButton>().KeyCommand = Control.Pause;
            }
        }
        return true;
    }

    private void OnBoosterPanelKeyConfirm(GameObject go)
    {
        if (go == masterSkillButtonGameObject)
        {
            if (!FF9StateSystem.Settings.IsMasterSkill)
            {
                if (Configuration.Cheats.MasterSkill)
                {
                    FF9Sfx.FF9SFX_Play(103);
                    PersistenSingleton<UIManager>.Instance.Booster.ShowWaringDialog(BoosterType.MasterSkill, AfterBoosterFinish);
                    hitpointScreenButton.KeyCommand = Control.None;
                    WarningDialogHitPoint.SetActive(true);
                }
                else
                {
                    Log.Message("[ConfigUI] MasterSkill was disabled.");
                    FF9Sfx.FF9SFX_Play(102);
                }
            }
            else
            {
                FF9Sfx.FF9SFX_Play(103);
                masterSkillLabel.color = FF9TextTool.White;
                FF9StateSystem.Settings.CallBoosterButtonFuntion(BoosterType.MasterSkill, false);
                PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.MasterSkill, false);
            }
        }
        else if (go == lvMaxButtonGameObject)
        {
            if (Configuration.Cheats.LvMax)
            {
                FF9Sfx.FF9SFX_Play(103);
                PersistenSingleton<UIManager>.Instance.Booster.ShowWaringDialog(BoosterType.LvMax, AfterBoosterFinish);
                hitpointScreenButton.KeyCommand = Control.None;
                WarningDialogHitPoint.SetActive(true);
            }
            else
            {
                Log.Message("[ConfigUI] LvMax was disabled.");
                FF9Sfx.FF9SFX_Play(102);
            }
        }
        else if (go == gilMaxButtonGameObject)
        {
            if (Configuration.Cheats.GilMax)
            {
                FF9Sfx.FF9SFX_Play(103);
                PersistenSingleton<UIManager>.Instance.Booster.ShowWaringDialog(BoosterType.GilMax, AfterBoosterFinish);
                hitpointScreenButton.KeyCommand = Control.None;
                WarningDialogHitPoint.SetActive(true);
            }
            else
            {
                Log.Message("[ConfigUI] GilMax was disabled.");
                FF9Sfx.FF9SFX_Play(102);
            }
        }
    }

    public override Boolean OnKeyPause(GameObject go)
    {
        if (base.OnKeyPause(go) && ButtonGroupState.ActiveGroup == ConfigUI.CustomControllerGroupButton)
        {
            FF9Sfx.FF9SFX_Play(101);
            this.SetControllerSettings();
            ButtonGroupState.ActiveGroup = ConfigUI.ControllerTypeGroupButton;
            this.backButtonGameObject.GetComponent<OnScreenButton>().KeyCommand = Control.Cancel;
        }
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (base.OnKeyCancel(go))
        {
            if (ButtonGroupState.ActiveGroup == ConfigGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                fastSwitch = false;
                Hide(delegate
                {
                    PersistenSingleton<UIManager>.Instance.MainMenuScene.NeedTweenAndHideSubMenu = false;
                    PersistenSingleton<UIManager>.Instance.MainMenuScene.CurrentSubMenu = MainMenuUI.SubMenu.Config;
                    PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.MainMenu);
                });
            }
            else if (ButtonGroupState.ActiveGroup == ControllerTypeGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                CloseCustomControllerPanel(ControllerType.Keyboard);
                CloseCustomControllerPanel(ControllerType.Joystick);
                ButtonGroupState.ActiveGroup = ConfigGroupButton;
                KeyboardButton.SetActive(false);
                JoystickButton.SetActive(false);
            }
            else if (ButtonGroupState.ActiveGroup == WarningMenuGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                Loading = true;
                WarningDialogHitPoint.SetActive(false);
                warningTransition.TweenOut(delegate
                {
                    Loading = false;
                });
                ButtonGroupState.ActiveGroup = ConfigGroupButton;
            }
        }
        return true;
    }

    public override Boolean OnKeyLeftBumper(GameObject go)
    {
        if (Loading)
        {
            return false;
        }
        ScrollButton activeScrollButton = ButtonGroupState.ActiveScrollButton;
        if (activeScrollButton && go.GetParent() != BoosterPanel)
        {
            activeScrollButton.OnPageUpButtonClick();
        }
        return true;
    }

    public override Boolean OnKeyRightBumper(GameObject go)
    {
        if (Loading)
        {
            return false;
        }
        ScrollButton activeScrollButton = ButtonGroupState.ActiveScrollButton;
        if (activeScrollButton && go.GetParent() != BoosterPanel)
        {
            activeScrollButton.OnPageDownButtonClick();
        }
        return true;
    }

    public override Boolean OnItemSelect(GameObject go)
    {
        if (base.OnItemSelect(go))
        {
            if (ButtonGroupState.ActiveGroup == ConfigGroupButton)
            {
                if (go.GetParent() == BoosterPanel && cursorInList)
                {
                    cursorInList = false;
                    ButtonGroupState.SetPointerLimitRectToGroup(new Vector4(-745f, -370f, 745f, 321f), ConfigGroupButton);
                    ButtonGroupState.UpdatePointerPropertyForGroup(ConfigGroupButton);
                    ButtonGroupState.UpdateActiveButton();
                }
                else if (go.GetParent().GetParent() == configScrollView.gameObject && !cursorInList)
                {
                    cursorInList = true;
                    ButtonGroupState.SetPointerLimitRectToGroup(new Vector4(-745f, -170f, -665f, 321f), ConfigGroupButton);
                    ButtonGroupState.UpdatePointerPropertyForGroup(ConfigGroupButton);
                    ButtonGroupState.UpdateActiveButton();
                }
                ButtonGroupState.SetCursorStartSelect(go, ConfigGroupButton);
            }
            if (ButtonGroupState.ActiveGroup == CustomControllerGroupButton)
            {
                currentControllerIndex = go.transform.GetSiblingIndex();
            }
            if (ButtonGroupState.ActiveGroup == ControllerTypeGroupButton)
            {
                if (go == KeyboardButton)
                {
                    currentControllerType = ControllerType.Keyboard;
                    CustomControllerKeyboardPanel.SetActive(true);
                    if (FF9StateSystem.PCPlatform)
                    {
                        CustomControllerJoystickPanel.SetActive(false);
                    }
                    else
                    {
                        CustomControllerMobilePanel.SetActive(false);
                    }
                }
                else if (go == JoystickButton)
                {
                    currentControllerType = ControllerType.Joystick;
                    CustomControllerKeyboardPanel.SetActive(false);
                    if (FF9StateSystem.PCPlatform)
                    {
                        CustomControllerJoystickPanel.SetActive(true);
                    }
                    else
                    {
                        CustomControllerMobilePanel.SetActive(true);
                    }
                }
            }
        }
        return true;
    }

    public void OnKeyChoice(GameObject go, KeyCode key)
    {
        if (ButtonGroupState.ActiveGroup == ConfigGroupButton)
        {
            ConfigField configField = ConfigFieldList.First(field => field.ConfigParent == go);
            if (configField.IsSlider)
            {
                if (key == KeyCode.LeftArrow)
                {
                    if (configField.Configurator == Configurator.FieldMessage)
                    {
                        setConfigValue(configField.ConfigParent, configField.Value - 1f / fieldMessageSliderStep, false);
                    }
                    else if (configField.Configurator == Configurator.BattleSpeed)
                    {
                        setConfigValue(configField.ConfigParent, configField.Value - 1f / battleSpeedSliderStep, false);
                    }
                }
                else if (key == KeyCode.RightArrow)
                {
                    if (configField.Configurator == Configurator.FieldMessage)
                    {
                        setConfigValue(configField.ConfigParent, configField.Value + 1f / fieldMessageSliderStep, false);
                    }
                    else if (configField.Configurator == Configurator.BattleSpeed)
                    {
                        setConfigValue(configField.ConfigParent, configField.Value + 1f / battleSpeedSliderStep, false);
                    }
                }
            }
            else if (configField.Configurator != Configurator.CombatTutorial && configField.Configurator != Configurator.ControlTutorial && configField.Configurator != Configurator.Title && configField.Configurator != Configurator.QuitGame && (key == KeyCode.LeftArrow || key == KeyCode.RightArrow))
            {
                setConfigValue(configField.ConfigParent, ((Int32)configField.Value + 1) % 2, false);
            }
        }
    }

    public void OnSelectValue(GameObject go, Boolean isSelected)
    {
        if (isSelected && UIKeyTrigger.IsOnlyTouchAndLeftClick() && ButtonGroupState.ActiveGroup == ConfigGroupButton)
        {
            ConfigField configField = ConfigFieldList.First(field => field.ConfigChoice.Contains(go));
            if (configField.Configurator == Configurator.Controller && configField.ConfigChoice.IndexOf(go) == 1)
            {
                setConfigValue(configField.ConfigParent, configField.ConfigChoice.IndexOf(go), false);
                ButtonGroupState.ActiveButton = configField.ConfigParent;
                CheckAndDisplayCustomControllerPanel();
            }
            else if (configField.IsSlider)
            {
                ButtonGroupState.ActiveButton = configField.ConfigParent;
            }
            else
            {
                setConfigValue(configField.ConfigParent, configField.ConfigChoice.IndexOf(go), false);
                ButtonGroupState.ActiveButton = configField.ConfigParent;
            }
        }
    }

    public void OnValueChange(GameObject go)
    {
        ConfigField configField = ConfigFieldList.First(field => field.ConfigChoice[0] == go);
        setConfigValue(configField.ConfigParent, configField.ConfigChoice[0].GetComponent<UISlider>().value, false);
    }

    private void OnBoosterClick(GameObject go)
    {
        if (!UIKeyTrigger.IsOnlyTouchAndLeftClick())
            return;

        if (!Configuration.Cheats.Enabled)
        {
            FF9Sfx.FF9SFX_Play(102);
            Log.Message("[ConfigUI] Cheats was disabled.");
            return;
        }

        if (go == masterSkillButtonGameObject && !Configuration.Cheats.MasterSkill)
        {
            FF9Sfx.FF9SFX_Play(102);
            Log.Message("[ConfigUI] MasterSkill was disabled.");
            return;
        }

        if (go == lvMaxButtonGameObject && !Configuration.Cheats.LvMax)
        {
            FF9Sfx.FF9SFX_Play(102);
            Log.Message("[ConfigUI] LvMax was disabled.");
            return;
        }

        if (go == gilMaxButtonGameObject && !Configuration.Cheats.GilMax)
        {
            FF9Sfx.FF9SFX_Play(102);
            Log.Message("[ConfigUI] GilMax was disabled.");
            return;
        }

        onPress(go, false);
    }

    private void OnBoosterNavigate(GameObject go, KeyCode key)
    {
        if (key == KeyCode.UpArrow)
        {
            UIPanel component = configScrollView.gameObject.GetComponent<UIPanel>();
            Transform child = configScrollView.transform.GetChild(0);
            GameObject obj = child.GetChild(0).gameObject;
            Boolean flag = false;
            for (Int32 i = 0; i < child.childCount; i++)
            {
                GameObject gameObject2 = child.GetChild(i).gameObject;
                UIWidget component2 = gameObject2.GetComponent<UIWidget>();
                if (gameObject2.activeSelf)
                {
                    if (!flag && component.IsVisible(component2))
                    {
                        flag = true;
                    }
                    else if (flag && !component.IsVisible(component2))
                    {
                        break;
                    }
                    obj = gameObject2;
                }
            }
            ButtonGroupState.SetCursorStartSelect(obj, ConfigGroupButton);
            ButtonGroupState.ActiveButton = obj;
        }
    }

    private void AfterBoosterFinish()
    {
        ButtonGroupState.ActiveGroup = ConfigGroupButton;
        WarningDialogHitPoint.SetActive(false);
        if (FF9StateSystem.Settings.IsMasterSkill)
        {
            masterSkillLabel.color = FF9TextTool.Green;
            PersistenSingleton<UIManager>.Instance.Booster.SetBoosterHudIcon(BoosterType.MasterSkill, true);
        }
    }

    private void DisplayHelp()
    {
        String str = (!FF9StateSystem.MobilePlatform) ? "PC" : "Mobile";
        foreach (ConfigField current in ConfigFieldList)
        {
            switch (current.Configurator)
            {
                case Configurator.Sound:
                    current.Button.Help.TextKey = "SoundHelp" + str;
                    break;
                case Configurator.SoundEffect:
                    current.Button.Help.TextKey = "SoundEffectHelp" + str;
                    break;
                case Configurator.Controller:
                    current.Button.Help.TextKey = "ControllerHelp" + str;
                    break;
                case Configurator.Cursor:
                    current.Button.Help.TextKey = "CursorHelp" + str;
                    break;
                case Configurator.ATB:
                    current.Button.Help.TextKey = "AtbHelp" + str;
                    break;
                case Configurator.BattleCamera:
                    current.Button.Help.TextKey = "BattleCameraHelp" + str;
                    break;
                case Configurator.SkipBattleCamera:
                    current.Button.Help.TextKey = "SkipBattleCameraHelp" + str;
                    break;
                case Configurator.Movement:
                    current.Button.Help.TextKey = "MovementHelp" + str;
                    break;
                case Configurator.FieldMessage:
                    current.Button.Help.TextKey = "FieldMessageHelp" + str;
                    break;
                case Configurator.BattleSpeed:
                    current.Button.Help.TextKey = "BattleSpeedHelp" + str;
                    break;
                case Configurator.HereIcon:
                    current.Button.Help.TextKey = "HereIconHelp" + str;
                    break;
                case Configurator.WindowColor:
                    current.Button.Help.TextKey = "WindowColorHelp" + str;
                    break;
                case Configurator.Vibration:
                    current.Button.Help.TextKey = "VibrationHelp" + str;
                    break;
                case Configurator.ControlTutorial:
                    current.Button.Help.TextKey = "ShowBasicTutorialHelp" + str;
                    break;
                case Configurator.CombatTutorial:
                    current.Button.Help.TextKey = "ShowBattleTutorialHelp" + str;
                    break;
                case Configurator.Title:
                    current.Button.Help.TextKey = "ToTitleHelp" + str;
                    break;
                case Configurator.QuitGame:
                    current.Button.Help.TextKey = "QuitGameHelp" + str;
                    break;
            }
        }
    }

    private void DisplayConfigValue()
    {
        foreach (ConfigField current in ConfigFieldList)
        {
            switch (current.Configurator)
            {
                case Configurator.Sound:
                    current.Value = FF9StateSystem.Settings.cfg.sound;
                    break;
                case Configurator.SoundEffect:
                    current.Value = FF9StateSystem.Settings.cfg.sound_effect;
                    break;
                case Configurator.Controller:
                    current.Value = FF9StateSystem.Settings.cfg.control;
                    break;
                case Configurator.Cursor:
                    current.Value = FF9StateSystem.Settings.cfg.cursor;
                    break;
                case Configurator.ATB:
                    current.Value = FF9StateSystem.Settings.cfg.atb;
                    break;
                case Configurator.BattleCamera:
                    current.Value = FF9StateSystem.Settings.cfg.camera;
                    break;
                case Configurator.SkipBattleCamera:
                    current.Value = FF9StateSystem.Settings.cfg.skip_btl_camera;
                    break;
                case Configurator.Movement:
                    current.Value = FF9StateSystem.Settings.cfg.move;
                    break;
                case Configurator.FieldMessage:
                    current.Value = FF9StateSystem.Settings.cfg.fld_msg / fieldMessageSliderStep;
                    break;
                case Configurator.BattleSpeed:
                    current.Value = FF9StateSystem.Settings.cfg.btl_speed / battleSpeedSliderStep;
                    break;
                case Configurator.HereIcon:
                    current.Value = FF9StateSystem.Settings.cfg.here_icon;
                    break;
                case Configurator.WindowColor:
                    current.Value = FF9StateSystem.Settings.cfg.win_type;
                    break;
                case Configurator.Vibration:
                    current.Value = FF9StateSystem.Settings.cfg.vibe;
                    break;
                default:
                    current.Value = 0f;
                    break;
            }
            setConfigValue(current.ConfigParent, current.Value, true);
        }
        if (!ButtonGroupState.HaveCursorMemorize(ConfigGroupButton))
        {
            configScrollView.ScrollToIndex(0);
        }

        if (!Configuration.Cheats.LvMax)
            lvMaxLabel.color = FF9TextTool.Gray;

        if (!Configuration.Cheats.GilMax)
            gilMaxLabel.color = FF9TextTool.Gray;

        if (FF9StateSystem.Settings.IsMasterSkill)
        {
            masterSkillLabel.color = FF9TextTool.Green;
        }
        else
        {
            if (!Configuration.Cheats.MasterSkill)
                masterSkillLabel.color = FF9TextTool.Gray;
            else
                masterSkillLabel.color = FF9TextTool.White;
        }
    }

    private void CheckAndDisplayCustomControllerPanel()
    {
        ValidateKeyboard();
        ValidateController();
        
        // TODO Check Native: #147 - Will incombaitble with Android and PC with Controller? O.o
        // this.currentControllerType = ControllerType.Keyboard;
        if (PersistenSingleton<HonoInputManager>.Instance.IsControllerConnect || FF9StateSystem.aaaaPlatform || FF9StateSystem.IOSPlatform) // aaaa is Vita
        {
            currentControllerType = ControllerType.Joystick;
        }
        else
        {
            currentControllerType = ControllerType.Keyboard;
        }
        DisplayCustomControllerPanel(currentControllerType);
        StartCoroutine(ShowButtonGroupDalay());
    }

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    private void setConfigValue(GameObject configGameObject, Single value, Boolean isForceSet = false)
    {
        ConfigField configField = ConfigFieldList.First(field => field.ConfigParent == configGameObject);
        Single value2 = configField.Value;
        Boolean flag = true;
        if (value2 != value || isForceSet)
        {
            if (isForceSet)
            {
                flag = false;
            }
            if (configField.IsSlider)
            {
                configField.Value = Mathf.Clamp(value, 0f, 1f);
                configField.ConfigChoice[0].GetComponent<UISlider>().value = configField.Value;
                if (configField.Value == value2)
                {
                    flag = false;
                }
            }
            else if (configField.Configurator != Configurator.Title && configField.Configurator != Configurator.QuitGame && configField.Configurator != Configurator.ControlTutorial && configField.Configurator != Configurator.CombatTutorial)
            {
                GameObject child = configField.ConfigChoice[0].GetChild(0);
                GameObject child2 = configField.ConfigChoice[1].GetChild(0);
                configField.Value = value;
                child.GetComponent<UILabel>().color = new Color(0.392156869f, 0.392156869f, 0.392156869f);
                child2.GetComponent<UILabel>().color = new Color(0.392156869f, 0.392156869f, 0.392156869f);
                if (configField.Value == 0f)
                {
                    child.GetComponent<UILabel>().color = new Color(0.784313738f, 0.784313738f, 0.784313738f);
                }
                else
                {
                    child2.GetComponent<UILabel>().color = new Color(0.784313738f, 0.784313738f, 0.784313738f);
                }
            }
            if (!isForceSet)
            {
                switch (configField.Configurator)
                {
                    case Configurator.Sound:
                        FF9StateSystem.Settings.cfg.sound = (UInt64)configField.Value;
                        FF9StateSystem.Settings.SetSound();
                        break;
                    case Configurator.SoundEffect:
                        FF9StateSystem.Settings.cfg.sound_effect = (UInt64)configField.Value;
                        FF9StateSystem.Settings.SetSoundEffect();
                        if (FF9StateSystem.Settings.cfg.sound_effect == 1uL)
                        {
                            flag = false;
                        }
                        break;
                    case Configurator.Controller:
                        FF9StateSystem.Settings.cfg.control = (UInt64)configField.Value;
                        SetControllerSettings();
                        break;
                    case Configurator.Cursor:
                        FF9StateSystem.Settings.cfg.cursor = (UInt64)configField.Value;
                        break;
                    case Configurator.ATB:
                        FF9StateSystem.Settings.cfg.atb = (UInt64)configField.Value;
                        break;
                    case Configurator.BattleCamera:
                        FF9StateSystem.Settings.cfg.camera = (UInt64)configField.Value;
                        break;
                    case Configurator.SkipBattleCamera:
                        FF9StateSystem.Settings.cfg.skip_btl_camera = (UInt64)configField.Value;
                        break;
                    case Configurator.Movement:
                        FF9StateSystem.Settings.cfg.move = (UInt64)configField.Value;
                        break;
                    case Configurator.FieldMessage:
                        FF9StateSystem.Settings.cfg.fld_msg = (UInt64)Math.Round(configField.Value * fieldMessageSliderStep);
                        break;
                    case Configurator.BattleSpeed:
                        FF9StateSystem.Settings.cfg.btl_speed = (UInt64)Math.Round(configField.Value * battleSpeedSliderStep);
                        break;
                    case Configurator.HereIcon:
                        FF9StateSystem.Settings.cfg.here_icon = (UInt64)configField.Value;
                        break;
                    case Configurator.WindowColor:
                        FF9StateSystem.Settings.cfg.win_type = (UInt64)configField.Value;
                        DisplayWindowBackground();
                        DisplayWindowBackground(PersistenSingleton<UIManager>.Instance.MainMenuScene.SubMenuPanel, null);
                        break;
                    case Configurator.Vibration:
                        FF9StateSystem.Settings.cfg.vibe = (UInt64)configField.Value;
                        if (FF9StateSystem.Settings.cfg.vibe == FF9CFG.FF9CFG_VIBE_ON)
                        {
                            is_vibe = true;
                            vibe_tick = 0;
                            vib.VIB_actuatorSet(0, 0.003921569f, 1f);
                            vib.VIB_actuatorSet(1, 0.003921569f, 1f);
                        }
                        break;
                    default:
                        configField.Value = 0f;
                        break;
                }
            }
            if (flag)
            {
                FF9Sfx.FF9SFX_Play(103);
            }
        }
    }

    private void OnGUI()
    {
        CheckKeyboardKeys();
    }

    private void Update()
    {
        CheckJoystickKeys();
        if (is_vibe && ++vibe_tick > 8)
        {
            is_vibe = false;
            vibe_tick = 0;
            vib.VIB_actuatorReset(0);
            vib.VIB_actuatorReset(1);
        }
    }

    private void Awake()
    {
        FadingComponent = ScreenFadeGameObject.GetComponent<HonoFading>();
        ConfigFieldList = new List<ConfigField>();
		// If the cheats of the Configuration menu are disabled, hide them and expand the ConfigList menu
		if (!Configuration.Cheats.Enabled || (!Configuration.Cheats.MasterSkill && !Configuration.Cheats.LvMax && !Configuration.Cheats.GilMax))
			ConfigList.GetComponent<UIWidget>().bottomAnchor.SetVertical(this.ConfigList.GetComponent<UIWidget>().cachedTransform.parent, -940f);
        Int32 num = ConfigList.GetChild(1).GetChild(0).transform.childCount;
        foreach (Transform trans in ConfigList.GetChild(1).GetChild(0).transform)
        {
            ConfigField configField = new ConfigField();
            GameObject obj = trans.gameObject;
            Int32 id = obj.GetComponent<ScrollItemKeyNavigation>().ID;
            
            // Hide vibration settings from menu
            if (!FF9StateSystem.IsPlatformVibration)
            {
                if (id == 12) // Configurator.Vibration
                {
                    num--;
                    gameObject.SetActive(false);
                }
                else if (id > 12) //  ControlTutorial, CombatTutorial, Title, QuitGame
                {
                    gameObject.GetComponent<ScrollItemKeyNavigation>().ID = id - 1;
                }
            }
            
            configField.ConfigParent = obj;
            configField.Button = obj.GetComponent<ButtonGroupState>();
            configField.Configurator = (Configurator)id;
            if (ConfigSliderIdList.Contains(configField.Configurator))
            {
                configField.ConfigChoice.Add(trans.GetChild(1).GetChild(1).gameObject);
                configField.IsSlider = true;
                UIEventListener expr_150 = UIEventListener.Get(configField.ConfigChoice[0]);
                expr_150.onSelect = (UIEventListener.BoolDelegate)Delegate.Combine(expr_150.onSelect, new UIEventListener.BoolDelegate(OnSelectValue));
            }
            else if (configField.Configurator == Configurator.Title)
            {
                toTitleGameObject = obj;
                UIEventListener expr_193 = UIEventListener.Get(obj);
                expr_193.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_193.onClick, new UIEventListener.VoidDelegate(onClick));
            }
            else if (configField.Configurator == Configurator.ControlTutorial || configField.Configurator == Configurator.CombatTutorial || configField.Configurator == Configurator.QuitGame)
            {
                UIEventListener expr_1EB = UIEventListener.Get(obj);
                expr_1EB.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_1EB.onClick, new UIEventListener.VoidDelegate(onClick));
            }
            else
            {
                configField.ConfigChoice.Add(trans.GetChild(1).gameObject);
                configField.ConfigChoice.Add(trans.GetChild(2).gameObject);
                configField.IsSlider = false;
                UIEventListener expr_25C = UIEventListener.Get(configField.ConfigChoice[0]);
                expr_25C.onSelect = (UIEventListener.BoolDelegate)Delegate.Combine(expr_25C.onSelect, new UIEventListener.BoolDelegate(OnSelectValue));
                UIEventListener expr_28F = UIEventListener.Get(configField.ConfigChoice[1]);
                expr_28F.onSelect = (UIEventListener.BoolDelegate)Delegate.Combine(expr_28F.onSelect, new UIEventListener.BoolDelegate(OnSelectValue));
            }
            UIEventListener expr_2BB = UIEventListener.Get(trans.gameObject);
            expr_2BB.onNavigate = (UIEventListener.KeyCodeDelegate)Delegate.Combine(expr_2BB.onNavigate, new UIEventListener.KeyCodeDelegate(OnKeyChoice));
            ConfigFieldList.Add(configField);
        }
        
        if (!FF9StateSystem.IsPlatformVibration)
        {
            int num3 = 12;
            this.ConfigFieldList[num3 - 1].ConfigParent.GetComponent<UIKeyNavigation>().onDown = this.ConfigFieldList[num3 + 1].ConfigParent;
            this.ConfigFieldList[num3 + 1].ConfigParent.GetComponent<UIKeyNavigation>().onUp = this.ConfigFieldList[num3 - 1].ConfigParent;
        }
        
        configScrollButton = ConfigList.GetChild(0).GetComponent<ScrollButton>();
        configScrollView = ConfigList.GetChild(1).GetComponent<SnapDragScrollView>();
        configScrollView.MaxItem = num;
        UIEventListener expr_40D = UIEventListener.Get(WarningDialog.GetChild(0).GetChild(2));
        expr_40D.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_40D.onClick, new UIEventListener.VoidDelegate(onClick));
        UIEventListener expr_446 = UIEventListener.Get(WarningDialog.GetChild(0).GetChild(3));
        expr_446.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_446.onClick, new UIEventListener.VoidDelegate(onClick));
        warningTransition = TransitionGroup.GetChild(0).GetComponent<HonoTweenClipping>();
        hitpointScreenButton = WarningDialogHitPoint.GetChild(0).GetComponent<OnScreenButton>();
        UIEventListener expr_4A1 = UIEventListener.Get(KeyboardButton);
        expr_4A1.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_4A1.onClick, new UIEventListener.VoidDelegate(onClick));
        UIEventListener expr_4CE = UIEventListener.Get(JoystickButton);
        expr_4CE.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_4CE.onClick, new UIEventListener.VoidDelegate(onClick));
        masterSkillButtonGameObject = BoosterPanel.GetChild(0);
        lvMaxButtonGameObject = BoosterPanel.GetChild(1);
        gilMaxButtonGameObject = BoosterPanel.GetChild(2);
        masterSkillLabel = masterSkillButtonGameObject.GetChild(0).GetComponent<UILabel>();
        lvMaxLabel = lvMaxButtonGameObject.GetChild(0).GetComponent<UILabel>();
        gilMaxLabel = gilMaxButtonGameObject.GetChild(0).GetComponent<UILabel>();
        UIEventListener expr_548 = UIEventListener.Get(masterSkillButtonGameObject);
        expr_548.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_548.onClick, new UIEventListener.VoidDelegate(OnBoosterClick));
        UIEventListener expr_574 = UIEventListener.Get(lvMaxButtonGameObject);
        expr_574.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_574.onClick, new UIEventListener.VoidDelegate(OnBoosterClick));
        UIEventListener expr_5A0 = UIEventListener.Get(gilMaxButtonGameObject);
        expr_5A0.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_5A0.onClick, new UIEventListener.VoidDelegate(OnBoosterClick));
        UIEventListener expr_5CC = UIEventListener.Get(masterSkillButtonGameObject);
        expr_5CC.onNavigate = (UIEventListener.KeyCodeDelegate)Delegate.Combine(expr_5CC.onNavigate, new UIEventListener.KeyCodeDelegate(OnBoosterNavigate));
        UIEventListener expr_5F8 = UIEventListener.Get(lvMaxButtonGameObject);
        expr_5F8.onNavigate = (UIEventListener.KeyCodeDelegate)Delegate.Combine(expr_5F8.onNavigate, new UIEventListener.KeyCodeDelegate(OnBoosterNavigate));
        UIEventListener expr_624 = UIEventListener.Get(gilMaxButtonGameObject);
        expr_624.onNavigate = (UIEventListener.KeyCodeDelegate)Delegate.Combine(expr_624.onNavigate, new UIEventListener.KeyCodeDelegate(OnBoosterNavigate));
        configScrollButton.DisplayScrollButton(false, false);
        transform.GetChild(3).GetChild(4).gameObject.SetActive(false);
        backButtonGameObject = ControlPanelGroup.GetChild(1);
		if (!Configuration.Cheats.Enabled || (!Configuration.Cheats.MasterSkill && !Configuration.Cheats.LvMax && !Configuration.Cheats.GilMax))
			BoosterPanel.SetActive(false);
    }
}