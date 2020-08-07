using System;
using System.Collections.Generic;
using Memoria.Assets;
using UnityEngine;

public class ButtonGroupState : MonoBehaviour
{
	public static String ActiveGroup
	{
		get
		{
			return ButtonGroupState.activeGroup;
		}
		set
		{
			ButtonGroupState.ActiveGroupChanged(value);
		}
	}

	public static GameObject PrevActiveButton
	{
		get
		{
			return ButtonGroupState.prevActiveButton;
		}
		set
		{
			ButtonGroupState.prevActiveButton = value;
			if (value != (UnityEngine.Object)null && value.GetComponent<ButtonGroupState>())
			{
				ButtonGroupState.prevActiveGroup = value.GetComponent<ButtonGroupState>().GroupName;
			}
			else
			{
				ButtonGroupState.prevActiveGroup = String.Empty;
			}
		}
	}

	public static String PrevActiveGroup
	{
		get
		{
			return ButtonGroupState.prevActiveGroup;
		}
	}

	public static List<String> IgnorePrevendTouchList
	{
		get
		{
			return ButtonGroupState.ignorePrevendTouchList;
		}
	}

	public static GameObject ActiveButton
	{
		get
		{
			return (!ButtonGroupState.activeButtonList.ContainsKey(ButtonGroupState.activeGroup)) ? null : ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup];
		}
		set
		{
			ButtonGroupState.ActiveButtonChanged(value, true);
		}
	}

	public static Boolean HelpEnabled
	{
		get
		{
			return ButtonGroupState.helpEnabled;
		}
		set
		{
			ButtonGroupState.helpEnabled = value;
		}
	}

	public static Boolean AllTargetEnabled
	{
		get
		{
			return ButtonGroupState.allTarget;
		}
	}

	public static Boolean MuteActiveSound
	{
		set
		{
			ButtonGroupState.muteActiveSound = value;
		}
	}

	public static ScrollButton ActiveScrollButton
	{
		get
		{
			if (ButtonGroupState.scrollButtonList.ContainsKey(ButtonGroupState.activeGroup))
			{
				return ButtonGroupState.scrollButtonList[ButtonGroupState.activeGroup];
			}
			return (ScrollButton)null;
		}
	}

	private void Awake()
	{
		this.button = base.gameObject.GetComponent<UIButton>();
		this.widget = base.gameObject.GetComponent<UIWidget>();
	}

	private void OnEnable()
	{
		if (!ButtonGroupState.ButtonGroupList.ContainsKey(this.GroupName))
		{
			ButtonGroupState.ButtonGroupList.Add(this.GroupName, new List<GameObject>());
		}
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
		if (ButtonGroupState.activeButtonList.ContainsKey(ButtonGroupState.activeGroup) && ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup] == (UnityEngine.Object)null)
		{
			ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup] = base.gameObject;
		}
	}

	private void OnDisable()
	{
		if (base.gameObject != (UnityEngine.Object)null)
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
			{
				ButtonGroupState.ActiveButtonChanged(base.gameObject, false);
			}
		}
	}

	public Boolean ProcessTouch()
	{
		if (this.GroupName == ButtonGroupState.ActiveGroup)
		{
			if (!ButtonGroupState.activeButtonList.ContainsKey(ButtonGroupState.activeGroup))
			{
				return false;
			}
			if (UICamera.selectedObject == base.gameObject)
			{
				Boolean flag = ButtonGroupState.ignorePrevendTouchList.Contains(this.GroupName);
				if (ButtonGroupState.prevActiveButton == base.gameObject || ButtonGroupState.PrevActiveGroup != this.GroupName || ButtonGroupState.prevActiveGroup == String.Empty || flag)
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
		{
			return;
		}
		if (ButtonGroupState.activeGroup != this.GroupName)
		{
			return;
		}
		if (this.button.gameObject == UICamera.selectedObject && !ButtonGroupState.allTarget)
		{
			if ((PersistenSingleton<HonoInputManager>.Instance.IsInputDown(0) || PersistenSingleton<HonoInputManager>.Instance.IsInputDown(1)) && this.button.state != UIButtonColor.State.Pressed)
			{
				this.button.SetState(UIButtonColor.State.Pressed, false);
			}
			if ((PersistenSingleton<HonoInputManager>.Instance.IsInputUp(0) || PersistenSingleton<HonoInputManager>.Instance.IsInputUp(1)) && this.button.state != UIButtonColor.State.Hover)
			{
				this.button.SetState(UIButtonColor.State.Hover, false);
			}
		}
	}

	protected virtual void OnHover(Boolean isOver)
	{
		if (!NGUITools.GetActive(this))
		{
			return;
		}
		if (ButtonGroupState.secondaryGroup.Contains(this.GroupName))
		{
			return;
		}
		if (!isOver && UICamera.selectedObject == base.gameObject)
		{
			this.SetHover(false);
		}
	}

	protected virtual void OnClick()
	{
		if (!base.gameObject)
		{
			return;
		}
		if (ButtonGroupState.secondaryGroup.Contains(this.GroupName))
		{
			return;
		}
		if (base.enabled && UIKeyTrigger.IsOnlyTouchAndLeftClick())
		{
			this.SetHover(false);
		}
	}

	protected virtual void OnDragOver(GameObject draggedObject)
	{
		if (!base.gameObject)
		{
			return;
		}
		if (ButtonGroupState.secondaryGroup.Contains(this.GroupName))
		{
			return;
		}
		if (base.enabled && UIKeyTrigger.IsOnlyTouchAndLeftClick() && ButtonGroupState.activeGroup != String.Empty && base.gameObject.GetComponent<UIDragScrollView>() == (UnityEngine.Object)null)
		{
			ButtonGroupState.ActiveButtonChanged(base.gameObject, true);
		}
	}

	protected virtual void OnDragOut(GameObject draggedObject)
	{
		if (!base.gameObject)
		{
			return;
		}
		if (ButtonGroupState.secondaryGroup.Contains(this.GroupName))
		{
			return;
		}
		if (base.gameObject == UICamera.selectedObject && base.enabled)
		{
			this.button.SetState(UIButtonColor.State.Pressed, true);
		}
	}

	public static Boolean ContainButtonInGroup(GameObject go, String group)
	{
		return ButtonGroupState.ButtonGroupList.ContainsKey(group) && ButtonGroupState.ButtonGroupList[group].Contains(go);
	}

	public static Boolean ContainButtonInSecondaryGroup(GameObject go)
	{
		foreach (String group in ButtonGroupState.secondaryGroup)
		{
			if (ButtonGroupState.ContainButtonInGroup(go, group))
			{
				return true;
			}
		}
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
		Vector4 value = default(Vector4);
		Single x = widget.transform.localPosition.x;
		Single y = widget.transform.localPosition.y;
		Single num = (Single)widget.width;
		Single num2 = (Single)widget.height;
		value.x = x - num / 2f;
		value.y = y - num2 / 2f - 14f + itemHeight / 2f;
		value.z = x + num / 2f;
		value.w = y + num2 / 2f - 20f - itemHeight / 2f;
		ButtonGroupState.SetPointerLimitRectToGroup(value, group);
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
		if (ButtonGroupState.activeButtonList.ContainsKey(group))
		{
			Singleton<PointerManager>.Instance.SetPointerLimitRectBehavior(ButtonGroupState.activeButtonList[group], ButtonGroupState.pointerLimitBehavior[group]);
		}
	}

	public static void SetSecondaryOnGroup(String group)
	{
		if (ButtonGroupState.ButtonGroupList.ContainsKey(group))
		{
			foreach (GameObject gameObject in ButtonGroupState.ButtonGroupList[group])
			{
				gameObject.GetComponent<BoxCollider>().enabled = true;
			}
		}
		if (!ButtonGroupState.secondaryGroup.Contains(group))
		{
			ButtonGroupState.secondaryGroup.Add(group);
		}
	}

	public static void HoldActiveStateOnGroup(String oldGroup)
	{
		if (ButtonGroupState.activeButtonList.ContainsKey(oldGroup) && oldGroup != ButtonGroupState.ActiveGroup)
		{
			ButtonGroupState.UpdatePointerPropertyForGroup(oldGroup);
			UIButton component = ButtonGroupState.activeButtonList[oldGroup].GetComponent<UIButton>();
			if (component.state != UIButtonColor.State.Hover)
			{
				component.SetState(UIButtonColor.State.Hover, true);
			}
			Singleton<PointerManager>.Instance.AttachPointerToGameObject(ButtonGroupState.activeButtonList[oldGroup]);
			Singleton<PointerManager>.Instance.SetPointerBlinkAt(ButtonGroupState.activeButtonList[oldGroup], true);
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
			UIButton component = go.GetComponent<UIButton>();
			if (component.state != UIButtonColor.State.Normal)
			{
				component.SetState(UIButtonColor.State.Normal, true);
			}
			Singleton<PointerManager>.Instance.RemovePointerFromGameObject(go);
		}
	}

	public static void DisableAllGroup(Boolean needtoHidePointer = true)
	{
		foreach (KeyValuePair<String, List<GameObject>> keyValuePair in ButtonGroupState.ButtonGroupList)
		{
			foreach (GameObject gameObject in keyValuePair.Value)
			{
				gameObject.GetComponent<BoxCollider>().enabled = false;
				gameObject.GetComponent<UIButton>().enabled = false;
				gameObject.GetComponent<UIKeyNavigation>().enabled = false;
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
		if (ButtonGroupState.ButtonGroupList.ContainsKey(group))
		{
			foreach (GameObject go in ButtonGroupState.ButtonGroupList[group])
			{
				Singleton<PointerManager>.Instance.SetPointerVisibility(go, isVisible);
			}
		}
	}

	public static void SetIgnorePreventTouch(Boolean isEnable, String group)
	{
		if (isEnable && !ButtonGroupState.ignorePrevendTouchList.Contains(group))
		{
			ButtonGroupState.ignorePrevendTouchList.Add(group);
		}
		else if (!isEnable && ButtonGroupState.ignorePrevendTouchList.Contains(group))
		{
			ButtonGroupState.ignorePrevendTouchList.Remove(group);
		}
	}

	public static void SetButtonAnimation(GameObject go, Boolean isAnimate)
	{
		UIButtonColor component = go.GetComponent<UIButtonColor>();
		if (isAnimate)
		{
			component.hover = new Color(1f, 1f, 1f, 0.5882353f);
			component.pressed = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			component.SetState(UIButtonColor.State.Normal, true);
			component.hover = new Color(1f, 1f, 1f, 0f);
			component.pressed = new Color(1f, 1f, 1f, 0f);
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
		if (!ButtonGroupState.ButtonGroupList.ContainsKey(ButtonGroupState.activeGroup))
		{
			return;
		}
		foreach (GameObject gameObject in ButtonGroupState.ButtonGroupList[ButtonGroupState.activeGroup])
		{
			gameObject.GetComponent<BoxCollider>().enabled = isEnable;
			gameObject.GetComponent<UIButton>().enabled = isEnable;
			gameObject.GetComponent<UIKeyNavigation>().isPreventAutoStartSelect = true;
			gameObject.GetComponent<UIKeyNavigation>().enabled = isEnable;
			gameObject.GetComponent<UIKeyNavigation>().isPreventAutoStartSelect = false;
		}
		if (ButtonGroupState.activeButtonList.ContainsKey(ButtonGroupState.activeGroup) && ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup])
		{
			ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup].GetComponent<ButtonGroupState>().SetHover(false);
		}
	}

	public static void SetAllTarget(Boolean isEnable)
	{
		if (ButtonGroupState.allTarget != isEnable)
		{
			ButtonGroupState.allTarget = isEnable;
			if (isEnable)
			{
				foreach (GameObject gameObject in ButtonGroupState.ButtonGroupList[ButtonGroupState.activeGroup])
				{
					Singleton<PointerManager>.Instance.AttachPointerToGameObject(gameObject);
					Singleton<PointerManager>.Instance.SetPointerBlinkAt(gameObject, true);
					gameObject.GetComponent<BoxCollider>().enabled = false;
					gameObject.GetComponent<UIButton>().enabled = false;
					gameObject.GetComponent<UIKeyNavigation>().enabled = false;
				}
			}
			else
			{
				foreach (GameObject gameObject2 in ButtonGroupState.ButtonGroupList[ButtonGroupState.activeGroup])
				{
					gameObject2.GetComponent<BoxCollider>().enabled = true;
					gameObject2.GetComponent<UIButton>().enabled = true;
					gameObject2.GetComponent<UIKeyNavigation>().enabled = true;
					Singleton<PointerManager>.Instance.SetPointerBlinkAt(gameObject2, false);
					Singleton<PointerManager>.Instance.RemovePointerFromGameObject(gameObject2);
				}
				if (ButtonGroupState.ButtonGroupList[ButtonGroupState.activeGroup].Contains(ButtonGroupState.ActiveButton))
				{
					ButtonGroupState.ActiveButton = ButtonGroupState.ActiveButton;
				}
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
			foreach (GameObject gameObject in ButtonGroupState.ButtonGroupList[ButtonGroupState.activeGroup])
			{
				gameObject.GetComponent<BoxCollider>().enabled = false;
				gameObject.GetComponent<UIButton>().enabled = false;
				gameObject.GetComponent<UIKeyNavigation>().enabled = false;
			}
		}
		else
		{
			foreach (GameObject gameObject2 in ButtonGroupState.ButtonGroupList[ButtonGroupState.activeGroup])
			{
				gameObject2.GetComponent<BoxCollider>().enabled = true;
				gameObject2.GetComponent<UIButton>().enabled = true;
				gameObject2.GetComponent<UIKeyNavigation>().enabled = true;
			}
			foreach (GameObject go2 in goList)
			{
				Singleton<PointerManager>.Instance.SetPointerBlinkAt(go2, false);
				Singleton<PointerManager>.Instance.RemovePointerFromGameObject(go2);
			}
			if (goList.Contains(ButtonGroupState.ActiveButton))
			{
				ButtonGroupState.ActiveButton = ButtonGroupState.ActiveButton;
			}
		}
	}

	public void SetHover(Boolean isImmediate)
	{
		if (PersistenSingleton<UIManager>.Instance.IsLoading)
		{
			return;
		}
		if (base.gameObject.GetComponent<BoxCollider>().enabled)
		{
			foreach (GameObject gameObject in ButtonGroupState.ButtonGroupList[ButtonGroupState.ActiveGroup])
			{
				if (base.gameObject != gameObject)
				{
					gameObject.GetComponent<UIButton>().SetState(UIButtonColor.State.Normal, true);
					Singleton<PointerManager>.Instance.RemovePointerFromGameObject(gameObject);
				}
			}
		}
		this.button.SetState(UIButtonColor.State.Hover, isImmediate);
		Singleton<PointerManager>.Instance.AttachPointerToGameObject(this.button.gameObject);
		if (ButtonGroupState.pointerLimitBehavior.ContainsKey(ButtonGroupState.activeGroup))
		{
			Singleton<PointerManager>.Instance.SetPointerLimitRectBehavior(base.gameObject, ButtonGroupState.pointerLimitBehavior[ButtonGroupState.activeGroup]);
		}
		else
		{
			Singleton<PointerManager>.Instance.SetPointerLimitRectBehavior(base.gameObject, PointerManager.LimitRectBehavior.Limit);
		}
	}

	public static Boolean HaveCursorMemorize(String group)
	{
		return ButtonGroupState.activeButtonList.ContainsKey(group) && ButtonGroupState.activeButtonList[group] != (UnityEngine.Object)null;
	}

	public static void SetCursorMemorize(GameObject go, String group)
	{
		if (go != (UnityEngine.Object)null)
		{
			ButtonGroupState.activeButtonList[group] = go;
		}
	}

	public static void RemoveCursorMemorize(String group)
	{
		if (ButtonGroupState.activeButtonList.ContainsKey(group))
		{
			ButtonGroupState.activeButtonList.Remove(group);
		}
	}

	public static void SetCursorStartSelect(GameObject go, String group)
	{
		if (go != (UnityEngine.Object)null && ButtonGroupState.ButtonGroupList.ContainsKey(group))
		{
			foreach (GameObject gameObject in ButtonGroupState.ButtonGroupList[group])
			{
				UIKeyNavigation component = gameObject.GetComponent<UIKeyNavigation>();
				component.startsSelected = (gameObject == go);
			}
		}
	}

	public static GameObject GetCursorStartSelect(String group)
	{
		if (ButtonGroupState.ButtonGroupList.ContainsKey(group))
		{
			foreach (GameObject gameObject in ButtonGroupState.ButtonGroupList[group])
			{
				UIKeyNavigation component = gameObject.GetComponent<UIKeyNavigation>();
				if (component.startsSelected)
				{
					return gameObject;
				}
			}
		}
		return (GameObject)null;
	}

	public static void ToggleHelp(bool playSFX = true)
    {
		if (ButtonGroupState.activeGroup != String.Empty && ButtonGroupState.activeGroup != QuitUI.WarningMenuGroupButton && PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.WorldHUD)
		{
			ButtonGroupState.helpEnabled = !ButtonGroupState.helpEnabled;
			ButtonGroupState component = ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup].GetComponent<ButtonGroupState>();
			if (component.Help.Enable)
			{
				Singleton<PointerManager>.Instance.SetPointerHelpAt(ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup], ButtonGroupState.helpEnabled, false);
			}
			if (!ButtonGroupState.helpEnabled)
			{
				Singleton<HelpDialog>.Instance.HideDialog();
                if (playSFX)
                {
                    FF9Sfx.FF9SFX_Play(101);
                }
            }
            else if (playSFX)
            {
                FF9Sfx.FF9SFX_Play(682);
            }
        }
	}

	public static void UpdateActiveButton()
	{
		if (ButtonGroupState.activeButtonList.ContainsKey(ButtonGroupState.activeGroup))
		{
			if (ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup] == (UnityEngine.Object)null)
			{
				return;
			}
			ButtonGroupState component = ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup].GetComponent<ButtonGroupState>();
			if (component != (UnityEngine.Object)null)
			{
				component.SetHover(false);
			}
			if (component.Help.Enable)
			{
				Singleton<PointerManager>.Instance.SetPointerHelpAt(ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup], ButtonGroupState.helpEnabled, true);
				if (ButtonGroupState.helpEnabled)
				{
					ButtonGroupState.ShowHelpDialog(ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup]);
				}
			}
			else
			{
				Singleton<PointerManager>.Instance.SetPointerHelpAt(ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup], false, true);
				Singleton<HelpDialog>.Instance.HideDialog();
			}
			if (ButtonGroupState.pointerNumberList.ContainsKey(ButtonGroupState.activeGroup) && ButtonGroupState.pointerNumberList[ButtonGroupState.activeGroup] != 0)
			{
				Singleton<PointerManager>.Instance.SetPointerNumberAt(ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup], ButtonGroupState.pointerNumberList[ButtonGroupState.activeGroup]);
			}
		}
	}

	public static void RefreshHelpDialog()
	{
		if (!ButtonGroupState.activeButtonList.ContainsKey(ButtonGroupState.activeGroup))
		{
			return;
		}
		if (ButtonGroupState.helpEnabled)
		{
			ButtonGroupState.ShowHelpDialog(ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup]);
		}
	}

	public static void ShowHelpDialog(GameObject go)
	{
		if (PersistenSingleton<UIManager>.Instance.IsLoading)
		{
			return;
		}
		ButtonGroupState component = go.GetComponent<ButtonGroupState>();
		if (component.Help.Enable)
		{
			Vector3 v = UIRoot.list[0].LocalToUIRootPoint(component.transform);
			v.x -= (Single)component.widget.width / 2f;
			if (component.Help.TextKey != String.Empty)
			{
				Singleton<HelpDialog>.Instance.Phrase = Localization.Get(component.Help.TextKey);
			}
			else
			{
				Singleton<HelpDialog>.Instance.Phrase = component.Help.Text;
			}
			Singleton<HelpDialog>.Instance.PointerOffset = ((!ButtonGroupState.pointerOffsetList.ContainsKey(component.GroupName)) ? new Vector2(0f, 0f) : ButtonGroupState.pointerOffsetList[component.GroupName]);
			Singleton<HelpDialog>.Instance.PointerLimitRect = ((!ButtonGroupState.pointerLimitRectList.ContainsKey(component.GroupName)) ? UIManager.UIScreenCoOrdinate : ButtonGroupState.pointerLimitRectList[component.GroupName]);
			Singleton<HelpDialog>.Instance.Position = v;
			Singleton<HelpDialog>.Instance.Tail = component.Help.Tail;
			Singleton<HelpDialog>.Instance.Depth = (Int32)((!ButtonGroupState.pointerDepthList.ContainsKey(ButtonGroupState.activeGroup)) ? 4 : (ButtonGroupState.pointerDepthList[ButtonGroupState.activeGroup] - 1));
			Singleton<HelpDialog>.Instance.ShowDialog();
		}
	}

	private static void ActiveButtonChanged(GameObject go, Boolean setSelect)
	{
		ButtonGroupState.PrevActiveButton = ((!ButtonGroupState.activeButtonList.ContainsKey(ButtonGroupState.activeGroup) || !(ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup] != (UnityEngine.Object)null)) ? PersistenSingleton<UIManager>.Instance.gameObject : ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup]);
		ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup] = go;
		if (setSelect)
		{
			UICamera.selectedObject = ButtonGroupState.activeButtonList[ButtonGroupState.activeGroup];
		}
		if (go == (UnityEngine.Object)null)
		{
			return;
		}
		ButtonGroupState.UpdateActiveButton();
		if (ButtonGroupState.PrevActiveButton != go && ButtonGroupState.activeGroup != Dialog.DialogGroupButton && !ButtonGroupState.muteActiveSound)
		{
			FF9Sfx.FF9SFX_Play(103);
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
		Singleton<PointerManager>.Instance.PointerOffset = ((!ButtonGroupState.pointerOffsetList.ContainsKey(group)) ? new Vector2(0f, 0f) : ButtonGroupState.pointerOffsetList[group]);
		Singleton<PointerManager>.Instance.PointerLimitRect = ((!ButtonGroupState.pointerLimitRectList.ContainsKey(group)) ? UIManager.UIScreenCoOrdinate : ButtonGroupState.pointerLimitRectList[group]);
		Singleton<PointerManager>.Instance.PointerDepth = (Int32)((!ButtonGroupState.pointerDepthList.ContainsKey(group)) ? 5 : ButtonGroupState.pointerDepthList[group]);
	}

	public static Dictionary<String, List<GameObject>> ButtonGroupList = new Dictionary<String, List<GameObject>>();

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
