using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Message (Legacy)")]
public class UIButtonMessage : MonoBehaviour
{
    private void Start()
    {
        this.mStarted = true;
    }

    private void OnEnable()
    {
        if (this.mStarted)
        {
            this.OnHover(UICamera.IsHighlighted(base.gameObject));
        }
    }

    private void OnHover(Boolean isOver)
    {
        if (base.enabled && ((isOver && this.trigger == UIButtonMessage.Trigger.OnMouseOver) || (!isOver && this.trigger == UIButtonMessage.Trigger.OnMouseOut)))
        {
            this.Send();
        }
    }

    private void OnPress(Boolean isPressed)
    {
        if (base.enabled && ((isPressed && this.trigger == UIButtonMessage.Trigger.OnPress) || (!isPressed && this.trigger == UIButtonMessage.Trigger.OnRelease)))
        {
            this.Send();
        }
    }

    private void OnSelect(Boolean isSelected)
    {
        if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
        {
            this.OnHover(isSelected);
        }
    }

    private void OnClick()
    {
        if (base.enabled && this.trigger == UIButtonMessage.Trigger.OnClick)
        {
            this.Send();
        }
    }

    private void OnDoubleClick()
    {
        if (base.enabled && this.trigger == UIButtonMessage.Trigger.OnDoubleClick)
        {
            this.Send();
        }
    }

    private void Send()
    {
        if (String.IsNullOrEmpty(this.functionName))
        {
            return;
        }
        if (this.target == (UnityEngine.Object)null)
        {
            this.target = base.gameObject;
        }
        if (this.includeChildren)
        {
            Transform[] componentsInChildren = this.target.GetComponentsInChildren<Transform>();
            Int32 i = 0;
            Int32 num = (Int32)componentsInChildren.Length;
            while (i < num)
            {
                Transform transform = componentsInChildren[i];
                transform.gameObject.SendMessage(this.functionName, base.gameObject, SendMessageOptions.DontRequireReceiver);
                i++;
            }
        }
        else
        {
            this.target.SendMessage(this.functionName, base.gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }

    public GameObject target;

    public String functionName;

    public UIButtonMessage.Trigger trigger;

    public Boolean includeChildren;

    private Boolean mStarted;

    public enum Trigger
    {
        OnClick,
        OnMouseOver,
        OnMouseOut,
        OnPress,
        OnRelease,
        OnDoubleClick
    }
}
