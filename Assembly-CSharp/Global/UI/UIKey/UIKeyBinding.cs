using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Key Binding")]
public class UIKeyBinding : MonoBehaviour
{
    public static Boolean IsBound(KeyCode key)
    {
        Int32 i = 0;
        Int32 count = UIKeyBinding.mList.Count;
        while (i < count)
        {
            UIKeyBinding uikeyBinding = UIKeyBinding.mList[i];
            if (uikeyBinding != (UnityEngine.Object)null && uikeyBinding.keyCode == key)
            {
                return true;
            }
            i++;
        }
        return false;
    }

    protected virtual void OnEnable()
    {
        UIKeyBinding.mList.Add(this);
    }

    protected virtual void OnDisable()
    {
        UIKeyBinding.mList.Remove(this);
    }

    protected virtual void Start()
    {
        UIInput component = base.GetComponent<UIInput>();
        this.mIsInput = (component != (UnityEngine.Object)null);
        if (component != (UnityEngine.Object)null)
        {
            EventDelegate.Add(component.onSubmit, new EventDelegate.Callback(this.OnSubmit));
        }
    }

    protected virtual void OnSubmit()
    {
        if (UICamera.currentKey == this.keyCode && this.IsModifierActive())
        {
            this.mIgnoreUp = true;
        }
    }

    protected virtual Boolean IsModifierActive()
    {
        if (this.modifier == UIKeyBinding.Modifier.Any)
        {
            return true;
        }
        if (this.modifier == UIKeyBinding.Modifier.Alt)
        {
            if (UICamera.GetKey(KeyCode.LeftAlt) || UICamera.GetKey(KeyCode.RightAlt))
            {
                return true;
            }
        }
        else if (this.modifier == UIKeyBinding.Modifier.Control)
        {
            if (UICamera.GetKey(KeyCode.LeftControl) || UICamera.GetKey(KeyCode.RightControl))
            {
                return true;
            }
        }
        else if (this.modifier == UIKeyBinding.Modifier.Shift)
        {
            if (UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift))
            {
                return true;
            }
        }
        else if (this.modifier == UIKeyBinding.Modifier.None)
        {
            return !UICamera.GetKey(KeyCode.LeftAlt) && !UICamera.GetKey(KeyCode.RightAlt) && !UICamera.GetKey(KeyCode.LeftControl) && !UICamera.GetKey(KeyCode.RightControl) && !UICamera.GetKey(KeyCode.LeftShift) && !UICamera.GetKey(KeyCode.RightShift);
        }
        return false;
    }

    protected virtual void Update()
    {
        if (UICamera.inputHasFocus)
        {
            return;
        }
        if (this.keyCode == KeyCode.None || !this.IsModifierActive())
        {
            return;
        }
        Boolean flag = UICamera.GetKeyDown(this.keyCode);
        Boolean flag2 = UICamera.GetKeyUp(this.keyCode);
        if (flag)
        {
            this.mPress = true;
        }
        if (this.action == UIKeyBinding.Action.PressAndClick || this.action == UIKeyBinding.Action.All)
        {
            if (flag)
            {
                UICamera.currentKey = this.keyCode;
                this.OnBindingPress(true);
            }
            if (this.mPress && flag2)
            {
                UICamera.currentKey = this.keyCode;
                this.OnBindingPress(false);
                this.OnBindingClick();
            }
        }
        if ((this.action == UIKeyBinding.Action.Select || this.action == UIKeyBinding.Action.All) && flag2)
        {
            if (this.mIsInput)
            {
                if (!this.mIgnoreUp && !UICamera.inputHasFocus && this.mPress)
                {
                    UICamera.selectedObject = base.gameObject;
                }
                this.mIgnoreUp = false;
            }
            else if (this.mPress)
            {
                UICamera.hoveredObject = base.gameObject;
            }
        }
        if (flag2)
        {
            this.mPress = false;
        }
    }

    protected virtual void OnBindingPress(Boolean pressed)
    {
        UICamera.Notify(base.gameObject, "OnPress", pressed);
    }

    protected virtual void OnBindingClick()
    {
        UICamera.Notify(base.gameObject, "OnClick", null);
    }

    private static List<UIKeyBinding> mList = new List<UIKeyBinding>();

    public KeyCode keyCode;

    public UIKeyBinding.Modifier modifier;

    public UIKeyBinding.Action action;

    [NonSerialized]
    private Boolean mIgnoreUp;

    [NonSerialized]
    private Boolean mIsInput;

    [NonSerialized]
    private Boolean mPress;

    public enum Action
    {
        PressAndClick,
        Select,
        All
    }

    public enum Modifier
    {
        Any,
        Shift,
        Control,
        Alt,
        None
    }
}
