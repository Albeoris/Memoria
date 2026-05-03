using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Forward Events (Legacy)")]
public class UIForwardEvents : MonoBehaviour
{
    private void OnHover(Boolean isOver)
    {
        if (this.onHover && this.target != (UnityEngine.Object)null)
        {
            this.target.SendMessage("OnHover", isOver, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnPress(Boolean pressed)
    {
        if (this.onPress && this.target != (UnityEngine.Object)null)
        {
            this.target.SendMessage("OnPress", pressed, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnClick()
    {
        if (this.onClick && this.target != (UnityEngine.Object)null)
        {
            this.target.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnDoubleClick()
    {
        if (this.onDoubleClick && this.target != (UnityEngine.Object)null)
        {
            this.target.SendMessage("OnDoubleClick", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnSelect(Boolean selected)
    {
        if (this.onSelect && this.target != (UnityEngine.Object)null)
        {
            this.target.SendMessage("OnSelect", selected, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnDrag(Vector2 delta)
    {
        if (this.onDrag && this.target != (UnityEngine.Object)null)
        {
            this.target.SendMessage("OnDrag", delta, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnDrop(GameObject go)
    {
        if (this.onDrop && this.target != (UnityEngine.Object)null)
        {
            this.target.SendMessage("OnDrop", go, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnSubmit()
    {
        if (this.onSubmit && this.target != (UnityEngine.Object)null)
        {
            this.target.SendMessage("OnSubmit", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnScroll(Single delta)
    {
        if (this.onScroll && this.target != (UnityEngine.Object)null)
        {
            this.target.SendMessage("OnScroll", delta, SendMessageOptions.DontRequireReceiver);
        }
    }

    public GameObject target;

    public Boolean onHover;

    public Boolean onPress;

    public Boolean onClick;

    public Boolean onDoubleClick;

    public Boolean onSelect;

    public Boolean onDrag;

    public Boolean onDrop;

    public Boolean onSubmit;

    public Boolean onScroll;
}
