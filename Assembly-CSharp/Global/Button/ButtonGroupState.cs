using Memoria;
using Memoria.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonGroupState : MonoBehaviour
{
    public static String ActiveGroup
    {
        get => ButtonGroupState.activeGroup;
        set => ButtonGroupState.ActiveGroupChanged(value);
    }

    public static GameObject PrevActiveButton
    {
        get => ButtonGroupState.prevActiveButton;
        set
        {
            ButtonGroupState.prevActiveButton = value;
            if (value != null && value.GetComponent<ButtonGroupState>())
                ButtonGroupState.prevActiveGroup = value.GetComponent<ButtonGroupState>().GroupName;
            else
                ButtonGroupState.prevActiveGroup = String.Empty;
        }
    }

    public static String PrevActiveGroup
    {
        get => ButtonGroupState.prevActiveGroup;
    }

    public static List<String> IgnorePrevendTouchList
    {
        get => ButtonGroupState.ignorePrevendTouchList;
    }

    public static GameObject ActiveButton
    {
        get => ButtonGroupState.activeButtonList.TryGetValue(ButtonGroupState.activeGroup, out GameObject button) ? button : null;
        set => ButtonGroupState.ActiveButtonChanged(value, true);
    }

    public static Boolean HelpEnabled
    {
        get => ButtonGroupState.helpEnabled;
        set => ButtonGroupState.helpEnabled = value;
    }

    public static Boolean AllTargetEnabled
    {
        get => ButtonGroupState.allTarget;
    }

    public static Boolean MuteActiveSound
    {
        set => ButtonGroupState.muteActiveSound = value;
    }

    public static ScrollButton ActiveScrollButton
    {
        get => ButtonGroupState.scrollButtonList.TryGetValue(ButtonGroupState.activeGroup, out ScrollButton button) ? button : null;
    }

    private void Awake()
    {
        this.button = base.gameObject.GetComponent<UIButton>();
        this.widget = base.gameObject.GetComponent<UIWidget>();
    }

    private void OnEnable()
    {
        if (!ButtonGroupState.ButtonGroupList.ContainsKey(this.GroupName))
            ButtonGroupState.ButtonGroupList.Add(this.GroupName, new List<GameObject>());
        ButtonGroupState.ButtonGroupList[this.GroupName].Add(base.gameObject);
        if (this.GroupName == ButtonGroupState.activeGroup)
        {
            base.gameObject.GetComponent<BoxCollider>().enabled = true;
            base.gameObject.GetComponent<UIKeyNavigation>().enabled = true;
            base.gameObject.GetComponent<UIButton>().enabled = true;
        }
        else
        {
            base.gameObject.GetComponent<BoxCollider>().enabled = false;
            base.gameObject.GetComponent<UIKeyNavigation>().enabled = false;
            base.gameObject.GetComponent<UIButton>().enabled = false;
        }
        if (ButtonGroupState.activeButtonList.TryGetValue(ButtonGroupState.activeGroup, out GameObject button) && button == null)
            ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup] = base.gameObject;
    }

    private void OnDisable()
    {
        if (base.gameObject != null)
        {
            Singleton<PointerManager>.Instance.RemovePointerFromGameObject(base.gameObject);
            ButtonGroupState.ButtonGroupList[this.GroupName].Remove(base.gameObject);
        }
    }

    private void Update()
    {
        if (base.enabled)
        {
            this.processJoyStick();
            if (this.GroupName == ButtonGroupState.ActiveGroup && UICamera.selectedObject == base.gameObject && ButtonGroupState.activeButtonList.ContainsKey(ButtonGroupState.activeGroup) && ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup] != base.gameObject)
                ButtonGroupState.ActiveButtonChanged(base.gameObject, false);
        }
    }

    public Boolean ProcessTouch()
    {
        if (this.GroupName == ButtonGroupState.ActiveGroup)
        {
            if (!ButtonGroupState.activeButtonList.ContainsKey(ButtonGroupState.activeGroup))
                return false;
            if (UICamera.selectedObject == base.gameObject)
            {
                Boolean updateAnyway = ButtonGroupState.ignorePrevendTouchList.Contains(this.GroupName);
                if (ButtonGroupState.prevActiveButton == base.gameObject || ButtonGroupState.PrevActiveGroup != this.GroupName || ButtonGroupState.prevActiveGroup == String.Empty || updateAnyway)
                {
                    ButtonGroupState.ActiveButtonChanged(base.gameObject, false);
                    return true;
                }
                ButtonGroupState.PrevActiveButton = base.gameObject;
            }
        }
        return false;
    }

    private void processJoyStick()
    {
        if (PersistenSingleton<UIManager>.Instance.IsLoading)
            return;
        if (ButtonGroupState.activeGroup != this.GroupName)
            return;
        if (this.button.gameObject == UICamera.selectedObject && !ButtonGroupState.allTarget)
        {
            if ((PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.Confirm) || PersistenSingleton<HonoInputManager>.Instance.IsInputDown(Control.Cancel)) && this.button.state != UIButtonColor.State.Pressed)
                this.button.SetState(UIButtonColor.State.Pressed, false);
            if ((PersistenSingleton<HonoInputManager>.Instance.IsInputUp(Control.Confirm) || PersistenSingleton<HonoInputManager>.Instance.IsInputUp(Control.Cancel)) && this.button.state != UIButtonColor.State.Hover)
                this.button.SetState(UIButtonColor.State.Hover, false);
        }
    }

    protected virtual void OnHover(Boolean isOver)
    {
        if (!NGUITools.GetActive(this) || ButtonGroupState.secondaryGroup.Contains(this.GroupName))
            return;
        if (!isOver && UICamera.selectedObject == base.gameObject)
            this.SetHover(false);
    }

    protected virtual void OnClick()
    {
        if (!base.gameObject || ButtonGroupState.secondaryGroup.Contains(this.GroupName))
            return;
        if (base.enabled && UIKeyTrigger.IsOnlyTouchAndLeftClick())
            this.SetHover(false);
    }

    protected virtual void OnDragOver(GameObject draggedObject)
    {
        if (!base.gameObject || ButtonGroupState.secondaryGroup.Contains(this.GroupName))
            return;
        if (base.enabled && UIKeyTrigger.IsOnlyTouchAndLeftClick() && ButtonGroupState.activeGroup != String.Empty && base.gameObject.GetComponent<UIDragScrollView>() == null)
            ButtonGroupState.ActiveButtonChanged(base.gameObject, true);
    }

    protected virtual void OnDragOut(GameObject draggedObject)
    {
        if (!base.gameObject || ButtonGroupState.secondaryGroup.Contains(this.GroupName))
            return;
        if (base.gameObject == UICamera.selectedObject && base.enabled)
            this.button.SetState(UIButtonColor.State.Pressed, true);
    }

    public static Boolean ContainButtonInGroup(GameObject go, String group)
    {
        return ButtonGroupState.ButtonGroupList.TryGetValue(group, out List<GameObject> buttonGroup) && buttonGroup.Contains(go);
    }

    public static Boolean ContainButtonInSecondaryGroup(GameObject go)
    {
        foreach (String group in ButtonGroupState.secondaryGroup)
            if (ButtonGroupState.ContainButtonInGroup(go, group))
                return true;
        return false;
    }

    public static void SetScrollButtonToGroup(ScrollButton btn, String group)
    {
        ButtonGroupState.scrollButtonList[group] = btn;
    }

    public static void SetPointerOffsetToGroup(Vector2 value, String group)
    {
        ButtonGroupState.pointerOffsetList[group] = value;
    }

    public static void SetPointerLimitRectToGroup(Vector4 value, String group)
    {
        ButtonGroupState.pointerLimitRectList[group] = value;
    }

    public static void SetPointerLimitRectToGroup(UIWidget widget, Single itemHeight, String group)
    {
        Vector4 limits = default(Vector4);
        Single x = widget.transform.localPosition.x;
        Single y = widget.transform.localPosition.y;
        Single targetWidth = widget.width;
        Single targetHeight = widget.height;
        limits.x = x - targetWidth / 2f;
        limits.y = y - targetHeight / 2f - 14f + itemHeight / 2f;
        limits.z = x + targetWidth / 2f;
        limits.w = y + targetHeight / 2f - 20f - itemHeight / 2f;
        ButtonGroupState.SetPointerLimitRectToGroup(limits, group);
    }

    public static void SetPointerDepthToGroup(Int32 value, String group)
    {
        ButtonGroupState.pointerDepthList[group] = value;
    }

    public static void SetPointerNumberToGroup(Int32 number, String group)
    {
        ButtonGroupState.pointerNumberList[group] = number;
    }

    public static void SetOutsideLimitRectBehavior(PointerManager.LimitRectBehavior behavior, String group)
    {
        ButtonGroupState.pointerLimitBehavior[group] = behavior;
        if (ButtonGroupState.activeButtonList.TryGetValue(group, out GameObject go))
            Singleton<PointerManager>.Instance.SetPointerLimitRectBehavior(go, behavior);
    }

    public static void SetSecondaryOnGroup(String group)
    {
        if (ButtonGroupState.ButtonGroupList.TryGetValue(group, out List<GameObject> buttonGroup))
            foreach (GameObject buttonGo in buttonGroup)
                buttonGo.GetComponent<BoxCollider>().enabled = true;
        if (!ButtonGroupState.secondaryGroup.Contains(group))
            ButtonGroupState.secondaryGroup.Add(group);
    }

    public static void HoldActiveStateOnGroup(String oldGroup)
    {
        if (ButtonGroupState.activeButtonList.TryGetValue(oldGroup, out GameObject go) && oldGroup != ButtonGroupState.ActiveGroup)
        {
            ButtonGroupState.UpdatePointerPropertyForGroup(oldGroup);
            UIButton uiButton = go.GetComponent<UIButton>();
            if (uiButton.state != UIButtonColor.State.Hover)
                uiButton.SetState(UIButtonColor.State.Hover, true);
            Singleton<PointerManager>.Instance.AttachPointerToGameObject(go);
            Singleton<PointerManager>.Instance.SetPointerBlinkAt(go, true);
            ButtonGroupState.UpdatePointerPropertyForGroup(ButtonGroupState.activeGroup);
        }
    }

    public static void HoldActiveStateOnGroup(GameObject go, String oldGroup)
    {
        if (ButtonGroupState.activeButtonList.ContainsKey(oldGroup) && oldGroup != ButtonGroupState.ActiveGroup)
        {
            ButtonGroupState.activeButtonList[oldGroup] = go;
            ButtonGroupState.HoldActiveStateOnGroup(oldGroup);
        }
    }

    public static void RemoveActiveStateOnGroup(GameObject go, String oldGroup)
    {
        if (ButtonGroupState.activeButtonList.ContainsKey(oldGroup) && oldGroup != ButtonGroupState.ActiveGroup)
        {
            UIButton uiButton = go.GetComponent<UIButton>();
            if (uiButton.state != UIButtonColor.State.Normal)
                uiButton.SetState(UIButtonColor.State.Normal, true);
            Singleton<PointerManager>.Instance.RemovePointerFromGameObject(go);
        }
    }

    public static void DisableAllGroup(Boolean needtoHidePointer = true)
    {
        foreach (List<GameObject> buttonGroup in ButtonGroupState.ButtonGroupList.Values)
        {
            foreach (GameObject buttonGo in buttonGroup)
            {
                buttonGo.GetComponent<BoxCollider>().enabled = false;
                buttonGo.GetComponent<UIButton>().enabled = false;
                buttonGo.GetComponent<UIKeyNavigation>().enabled = false;
            }
        }
        ButtonGroupState.activeGroup = String.Empty;
        if (needtoHidePointer)
        {
            Singleton<PointerManager>.Instance.RemoveAllPointer();
            Singleton<HelpDialog>.Instance.HideDialog();
        }
    }

    public static void SetPointerVisibilityToGroup(Boolean isVisible, String group)
    {
        if (ButtonGroupState.ButtonGroupList.TryGetValue(group, out List<GameObject> buttonGroup))
            foreach (GameObject buttonGo in buttonGroup)
                Singleton<PointerManager>.Instance.SetPointerVisibility(buttonGo, isVisible);
    }

    public static void SetIgnorePreventTouch(Boolean isEnable, String group)
    {
        if (!isEnable)
            ButtonGroupState.ignorePrevendTouchList.Remove(group);
        else if (!ButtonGroupState.ignorePrevendTouchList.Contains(group))
            ButtonGroupState.ignorePrevendTouchList.Add(group);
    }

    public static void SetButtonAnimation(GameObject go, Boolean isAnimate)
    {
        UIButtonColor buttonColor = go.GetComponent<UIButtonColor>();
        if (isAnimate)
        {
            buttonColor.hover = new Color(1f, 1f, 1f, 0.5882353f);
            buttonColor.pressed = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            buttonColor.SetState(UIButtonColor.State.Normal, true);
            buttonColor.hover = new Color(1f, 1f, 1f, 0f);
            buttonColor.pressed = new Color(1f, 1f, 1f, 0f);
        }
    }

    public static void SetButtonEnable(GameObject go, Boolean isEnable)
    {
        go.GetComponent<ButtonGroupState>().enabled = isEnable;
        go.GetComponent<BoxCollider>().enabled = isEnable;
        go.GetComponent<UIButton>().enabled = isEnable;
        go.GetComponent<UIKeyNavigation>().enabled = isEnable;
        ButtonGroupState.SetButtonAnimation(go, isEnable);
    }

    public static void SetActiveGroupEnable(Boolean isEnable)
    {
        if (!ButtonGroupState.ButtonGroupList.TryGetValue(ButtonGroupState.activeGroup, out List<GameObject> buttonGroup))
            return;
        foreach (GameObject buttonGo in buttonGroup)
        {
            buttonGo.GetComponent<BoxCollider>().enabled = isEnable;
            buttonGo.GetComponent<UIButton>().enabled = isEnable;
            buttonGo.GetComponent<UIKeyNavigation>().isPreventAutoStartSelect = true;
            buttonGo.GetComponent<UIKeyNavigation>().enabled = isEnable;
            buttonGo.GetComponent<UIKeyNavigation>().isPreventAutoStartSelect = false;
        }
        if (ButtonGroupState.activeButtonList.TryGetValue(ButtonGroupState.activeGroup, out GameObject go) && go != null)
            go.GetComponent<ButtonGroupState>().SetHover(false);
    }

    public static void SetAllTarget(Boolean isEnable)
    {
        if (ButtonGroupState.allTarget != isEnable)
        {
            ButtonGroupState.allTarget = isEnable;
            if (isEnable)
            {
                foreach (GameObject buttonGo in ButtonGroupState.ButtonGroupList[ButtonGroupState.activeGroup])
                {
                    Singleton<PointerManager>.Instance.AttachPointerToGameObject(buttonGo);
                    Singleton<PointerManager>.Instance.SetPointerBlinkAt(buttonGo, true);
                    buttonGo.GetComponent<BoxCollider>().enabled = false;
                    buttonGo.GetComponent<UIButton>().enabled = false;
                    buttonGo.GetComponent<UIKeyNavigation>().enabled = false;
                }
            }
            else
            {
                foreach (GameObject buttonGo in ButtonGroupState.ButtonGroupList[ButtonGroupState.activeGroup])
                {
                    buttonGo.GetComponent<BoxCollider>().enabled = true;
                    buttonGo.GetComponent<UIButton>().enabled = true;
                    buttonGo.GetComponent<UIKeyNavigation>().enabled = true;
                    Singleton<PointerManager>.Instance.SetPointerBlinkAt(buttonGo, false);
                    Singleton<PointerManager>.Instance.RemovePointerFromGameObject(buttonGo);
                }
                if (ButtonGroupState.ButtonGroupList[ButtonGroupState.activeGroup].Contains(ButtonGroupState.ActiveButton))
                    ButtonGroupState.ActiveButton = ButtonGroupState.ActiveButton;
            }
        }
    }

    public static void SetMultipleTarget(List<GameObject> goList, Boolean isEnable)
    {
        ButtonGroupState.allTarget = isEnable;
        if (isEnable)
        {
            foreach (GameObject go in goList)
            {
                Singleton<PointerManager>.Instance.AttachPointerToGameObject(go);
                Singleton<PointerManager>.Instance.SetPointerBlinkAt(go, true);
            }
            foreach (GameObject buttonGo in ButtonGroupState.ButtonGroupList[ButtonGroupState.activeGroup])
            {
                buttonGo.GetComponent<BoxCollider>().enabled = false;
                buttonGo.GetComponent<UIButton>().enabled = false;
                buttonGo.GetComponent<UIKeyNavigation>().enabled = false;
            }
        }
        else
        {
            foreach (GameObject buttonGo in ButtonGroupState.ButtonGroupList[ButtonGroupState.activeGroup])
            {
                buttonGo.GetComponent<BoxCollider>().enabled = true;
                buttonGo.GetComponent<UIButton>().enabled = true;
                buttonGo.GetComponent<UIKeyNavigation>().enabled = true;
            }
            foreach (GameObject go in goList)
            {
                Singleton<PointerManager>.Instance.SetPointerBlinkAt(go, false);
                Singleton<PointerManager>.Instance.RemovePointerFromGameObject(go);
            }
            if (goList.Contains(ButtonGroupState.ActiveButton))
                ButtonGroupState.ActiveButton = ButtonGroupState.ActiveButton;
        }
    }

    public void SetHover(Boolean isImmediate)
    {
        if (PersistenSingleton<UIManager>.Instance.IsLoading)
            return;
        if (base.gameObject.GetComponent<BoxCollider>().enabled)
        {
            foreach (GameObject go in ButtonGroupState.ButtonGroupList[ButtonGroupState.ActiveGroup])
            {
                if (base.gameObject != go)
                {
                    go.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, true);
                    Singleton<PointerManager>.Instance.RemovePointerFromGameObject(go);
                }
            }
        }
        this.button.SetState(UIButtonColor.State.Hover, isImmediate);
        if (ButtonGroupState.DisablePointerCursor.Contains(this.GroupName))
            return;
        Singleton<PointerManager>.Instance.AttachPointerToGameObject(this.button.gameObject);
        if (ButtonGroupState.pointerLimitBehavior.TryGetValue(ButtonGroupState.activeGroup, out PointerManager.LimitRectBehavior limitBehaviour))
            Singleton<PointerManager>.Instance.SetPointerLimitRectBehavior(base.gameObject, limitBehaviour);
        else
            Singleton<PointerManager>.Instance.SetPointerLimitRectBehavior(base.gameObject, PointerManager.LimitRectBehavior.Limit);
    }

    public static Boolean HaveCursorMemorize(String group)
    {
        return ButtonGroupState.activeButtonList.TryGetValue(group, out GameObject go) && go != null;
    }

    public static void SetCursorMemorize(GameObject go, String group)
    {
        if (go != null)
            ButtonGroupState.activeButtonList[group] = go;
    }

    public static void RemoveCursorMemorize(String group)
    {
        ButtonGroupState.activeButtonList.Remove(group);
    }

    public static void SetCursorStartSelect(GameObject go, String group)
    {
        if (go != null && ButtonGroupState.ButtonGroupList.ContainsKey(group))
            foreach (GameObject buttonGo in ButtonGroupState.ButtonGroupList[group])
                buttonGo.GetComponent<UIKeyNavigation>().startsSelected = buttonGo == go;
    }

    public static GameObject GetCursorStartSelect(String group)
    {
        if (ButtonGroupState.ButtonGroupList.TryGetValue(group, out List<GameObject> objList))
            return objList.FirstOrDefault(button => button.GetComponent<UIKeyNavigation>().startsSelected);
        return null;
    }

    public static void ToggleHelp(bool playSFX = true)
    {
        if (ButtonGroupState.activeGroup != String.Empty && ButtonGroupState.activeGroup != QuitUI.WarningMenuGroupButton && PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.WorldHUD)
        {
            ButtonGroupState.helpEnabled = !ButtonGroupState.helpEnabled;
            ButtonGroupState component = ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup].GetComponent<ButtonGroupState>();
            if (component.Help.Enable)
                Singleton<PointerManager>.Instance.SetPointerHelpAt(ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup], ButtonGroupState.helpEnabled, false);
            if (!ButtonGroupState.helpEnabled)
            {
                Singleton<HelpDialog>.Instance.HideDialog();
                if (playSFX)
                    FF9Sfx.FF9SFX_Play(101);
            }
            else if (playSFX)
            {
                FF9Sfx.FF9SFX_Play(682);
            }
        }
    }

    public static void UpdateActiveButton()
    {
        if (ButtonGroupState.activeButtonList.TryGetValue(ButtonGroupState.activeGroup, out GameObject go) && go != null)
        {
            ButtonGroupState button = go.GetComponent<ButtonGroupState>();
            if (button != null)
                button.SetHover(false);
            if (button.Help.Enable)
            {
                Singleton<PointerManager>.Instance.SetPointerHelpAt(go, ButtonGroupState.helpEnabled, true);
                if (ButtonGroupState.helpEnabled)
                    ButtonGroupState.ShowHelpDialog(go);
            }
            else
            {
                Singleton<PointerManager>.Instance.SetPointerHelpAt(go, false, true);
                Singleton<HelpDialog>.Instance.HideDialog();
            }
            if (ButtonGroupState.pointerNumberList.TryGetValue(ButtonGroupState.activeGroup, out Int32 number) && number >= 0)
                Singleton<PointerManager>.Instance.SetPointerNumberAt(go, number);
        }
    }

    public static void RefreshHelpDialog()
    {
        if (ButtonGroupState.helpEnabled && ButtonGroupState.activeButtonList.TryGetValue(ButtonGroupState.activeGroup, out GameObject go))
            ButtonGroupState.ShowHelpDialog(go);
    }

    public static void ShowHelpDialog(GameObject go)
    {
        if (PersistenSingleton<UIManager>.Instance.IsLoading)
            return;
        ButtonGroupState button = go.GetComponent<ButtonGroupState>();
        if (button.Help.Enable)
        {
            Boolean invertPointer = NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft;
            Vector3 helpPos = UIRoot.list[0].transform.InverseTransformPoint(button.widget.worldCenter);
            helpPos.x += invertPointer ? button.widget.width / 2f + 3f * UIPointer.PointerSize.x / 4f : -button.widget.width / 2f;
            Singleton<HelpDialog>.Instance.Phrase = String.IsNullOrEmpty(button.Help.TextKey) ? button.Help.Text : Localization.GetWithDefault(button.Help.TextKey);
            Singleton<HelpDialog>.Instance.PointerOffset = ButtonGroupState.GetPointerOffsetOfGroup(button.GroupName);
            Singleton<HelpDialog>.Instance.PointerLimitRect = ButtonGroupState.pointerLimitRectList.TryGetValue(button.GroupName, out Vector4 limits) ? limits : UIManager.UIScreenCoOrdinate;
            Singleton<HelpDialog>.Instance.Position = helpPos;
            Singleton<HelpDialog>.Instance.Tail = button.Help.Tail;
            Singleton<HelpDialog>.Instance.Depth = ButtonGroupState.pointerDepthList.TryGetValue(ButtonGroupState.activeGroup, out Int32 depth) ? depth - 1 : 4;
            Singleton<HelpDialog>.Instance.ShowDialog();
        }
    }

    private static void ActiveButtonChanged(GameObject go, Boolean setSelect)
    {
        ButtonGroupState.PrevActiveButton = ButtonGroupState.activeButtonList.TryGetValue(ButtonGroupState.activeGroup, out GameObject prevGo) && prevGo != null ? prevGo : PersistenSingleton<UIManager>.Instance.gameObject;
        ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup] = go;
        if (setSelect)
            UICamera.selectedObject = go;
        if (go == null)
            return;
        ButtonGroupState.UpdateActiveButton();
        Boolean DontTriggerSound = Configuration.Interface.PSXBattleMenu && previousGo == go;
        if (Configuration.Interface.PSXBattleMenu && ButtonGroupState.activeGroup == BattleHUD.CommandGroupButton && (go.name == "Change" || go.name == "Defend"))
            FF9Sfx.FF9SFX_Play(1047);
        else if (ButtonGroupState.PrevActiveButton != go && ButtonGroupState.activeGroup != Dialog.DialogGroupButton && !ButtonGroupState.muteActiveSound && !DontTriggerSound)
        {
            FF9Sfx.FF9SFX_Play(103);
            previousGo = go;
        }
        UICamera.Notify(PersistenSingleton<UIManager>.Instance.gameObject, "OnItemSelect", go);      
    }

    private static void ActiveGroupChanged(String newGroupName)
    {
        if (newGroupName == ButtonGroupState.activeGroup)
        {
            return;
        }
        if (newGroupName == string.Empty)
        {
            Singleton<HelpDialog>.Instance.HideDialog();
            return;
        }
        string text = ButtonGroupState.activeGroup;
        ButtonGroupState.UpdatePointerPropertyForGroup(newGroupName);
        if (ButtonGroupState.secondaryGroup.Contains(newGroupName))
        {
            ButtonGroupState.secondaryGroup.Remove(newGroupName);
        }
        if (UICamera.selectedObject && !string.IsNullOrEmpty(text) && UICamera.selectedObject.GetComponent<UIKeyNavigation>() && UICamera.selectedObject.GetComponent<UIKeyNavigation>().enabled && UICamera.selectedObject.GetComponent<ButtonGroupState>().GroupName == text && ButtonGroupState.activeButtonList.ContainsKey(text) && ButtonGroupState.activeButtonList[text] != UICamera.selectedObject && ButtonGroupState.activeButtonList[text] != null)
        {
            Singleton<PointerManager>.Instance.RemovePointerFromGameObject(ButtonGroupState.activeButtonList[text]);
        }
        if (ButtonGroupState.ButtonGroupList.ContainsKey(text))
        {
            foreach (GameObject gameObject in ButtonGroupState.ButtonGroupList[text])
            {
                gameObject.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, true);
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.GetComponent<UIButton>().enabled = false;
                gameObject.GetComponent<UIKeyNavigation>().enabled = false;
            }
        }
        if (ButtonGroupState.scrollButtonList.ContainsKey(text))
        {
            ButtonGroupState.scrollButtonList[text].SetScrollButtonEnable(false);
        }
        if (ButtonGroupState.ButtonGroupList.ContainsKey(newGroupName))
        {
            foreach (GameObject gameObject2 in ButtonGroupState.ButtonGroupList[newGroupName])
            {
                UIKeyNavigation component = gameObject2.GetComponent<UIKeyNavigation>();
                if (component && component.startsSelected && !ButtonGroupState.activeButtonList.ContainsKey(newGroupName))
                {
                    ButtonGroupState.activeButtonList[newGroupName] = gameObject2;
                }
                gameObject2.GetComponent<BoxCollider>().enabled = true;
                gameObject2.GetComponent<UIButton>().enabled = true;
                gameObject2.GetComponent<UIKeyNavigation>().enabled = true;
            }
        }
        if (ButtonGroupState.scrollButtonList.ContainsKey(newGroupName))
        {
            ButtonGroupState.scrollButtonList[newGroupName].SetScrollButtonEnable(true);
        }
        if (!ButtonGroupState.activeButtonList.ContainsKey(newGroupName) || ButtonGroupState.activeButtonList[newGroupName] == null)
        {
            if (ButtonGroupState.ButtonGroupList.ContainsKey(newGroupName))
            {
                if (ButtonGroupState.ButtonGroupList[newGroupName].Count == 0)
                {
                    ButtonGroupState.activeButtonList[newGroupName] = null;
                }
                else
                {
                    ButtonGroupState.activeButtonList[newGroupName] = ButtonGroupState.ButtonGroupList[newGroupName][0];
                }
            }
            else
            {
                ButtonGroupState.activeButtonList[newGroupName] = null;
            }
        }
        ButtonGroupState.allTarget = false;
        ButtonGroupState.activeGroup = newGroupName;
        PersistenSingleton<UIManager>.Instance.CurrentButtonGroup = newGroupName;
        ButtonGroupState.ActiveButton = ButtonGroupState.activeButtonList[newGroupName];
        ButtonGroupState.prevActiveButton = ((!ButtonGroupState.activeButtonList.ContainsKey(text)) ? PersistenSingleton<UIManager>.Instance.gameObject : ButtonGroupState.activeButtonList[text]);
        ButtonGroupState.prevActiveGroup = text;
        if (ButtonGroupState.activeButtonList.ContainsKey(text))
        {
            Singleton<PointerManager>.Instance.SetPointerBlinkAt(ButtonGroupState.ActiveButton, false);
            Singleton<PointerManager>.Instance.RemovePointerFromGameObject(ButtonGroupState.activeButtonList[text]);
        }
    }

    public static void UpdatePointerPropertyForGroup(String group)
    {
        Singleton<PointerManager>.Instance.PointerOffset = ButtonGroupState.GetPointerOffsetOfGroup(group);
        Singleton<PointerManager>.Instance.PointerLimitRect = ButtonGroupState.pointerLimitRectList.TryGetValue(group, out Vector4 limits) ? limits : UIManager.UIScreenCoOrdinate;
        Singleton<PointerManager>.Instance.PointerDepth = ButtonGroupState.pointerDepthList.TryGetValue(group, out Int32 depth) ? depth : 5;
    }

    private static Vector2 GetPointerOffsetOfGroup(String group)
    {
        if (ButtonGroupState.pointerOffsetList.TryGetValue(group, out Vector2 offset))
        {
            if (NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft)
                offset.x *= -1;
            return offset;
        }
        return Vector2.zero;
    }

    public static Dictionary<String, List<GameObject>> ButtonGroupList = new Dictionary<String, List<GameObject>>();
    public static HashSet<String> DisablePointerCursor = new HashSet<String>();

    private static Dictionary<String, Vector2> pointerOffsetList = new Dictionary<String, Vector2>();
    private static Dictionary<String, Vector4> pointerLimitRectList = new Dictionary<String, Vector4>();
    private static Dictionary<String, Int32> pointerDepthList = new Dictionary<String, Int32>();
    private static Dictionary<String, Int32> pointerNumberList = new Dictionary<String, Int32>();
    private static Dictionary<String, PointerManager.LimitRectBehavior> pointerLimitBehavior = new Dictionary<String, PointerManager.LimitRectBehavior>();
    private static Dictionary<String, GameObject> activeButtonList = new Dictionary<String, GameObject>();
    private static Dictionary<String, ScrollButton> scrollButtonList = new Dictionary<String, ScrollButton>();
    private static List<String> ignorePrevendTouchList = new List<String>();
    private static List<String> secondaryGroup = new List<String>();

    private static GameObject prevActiveButton;
    private static GameObject previousGo;
    private static String prevActiveGroup = String.Empty;
    private static String activeGroup = String.Empty;
    private static Boolean muteActiveSound = false;
    private static Boolean allTarget = false;
    private static Boolean helpEnabled = false;

    public String GroupName = String.Empty;
    public ButtonGroupState.HelpDetail Help;

    private UIButton button;
    private UIWidget widget;

    [Serializable]
    public class HelpDetail
    {
        public Boolean Enable = true;
        public String TextKey;
        public String Text;
        public Boolean Tail = true;
    }
}
