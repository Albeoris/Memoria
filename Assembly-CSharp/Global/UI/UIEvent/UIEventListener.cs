using System;
using UnityEngine;
using Object = System.Object;

// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

[AddComponentMenu("NGUI/Internal/Event Listener")]
public class UIEventListener : MonoBehaviour
{
    public Object parameter;
    public VoidDelegate onSubmit;
    public VoidDelegate onClick;
    public VoidDelegate onDoubleClick;
    public BoolDelegate onHover;
    public BoolDelegate onPress;
    public BoolDelegate onSelect;
    public FloatDelegate onScroll;
    public VoidDelegate onDragStart;
    public VectorDelegate onDrag;
    public VoidDelegate onDragOver;
    public VoidDelegate onDragOut;
    public VoidDelegate onDragEnd;
    public ObjectDelegate onDrop;
    public KeyCodeDelegate onKey;
    public KeyCodeDelegate onNavigate;
    public BoolDelegate onTooltip;

    #region Memoria

    public event VoidDelegate Submit
    {
        add { onSubmit = (VoidDelegate)Delegate.Combine(onSubmit, value); }
        remove { throw new NotSupportedException(nameof(Submit)); }
    }

    public event VoidDelegate Click
    {
        add { onClick = (VoidDelegate)Delegate.Combine(onClick, value); }
        remove { throw new NotSupportedException(nameof(Click)); }
    }

    public event VoidDelegate DoubleClick
    {
        add { onDoubleClick = (VoidDelegate)Delegate.Combine(onDoubleClick, value); }
        remove { throw new NotSupportedException(nameof(DoubleClick)); }
    }

    public event BoolDelegate Hover
    {
        add { onHover = (BoolDelegate)Delegate.Combine(onHover, value); }
        remove { throw new NotSupportedException(nameof(Hover)); }
    }

    public event BoolDelegate Press
    {
        add { onPress = (BoolDelegate)Delegate.Combine(onPress, value); }
        remove { throw new NotSupportedException(nameof(Press)); }
    }

    public event BoolDelegate Select
    {
        add { onSelect = (BoolDelegate)Delegate.Combine(onSelect, value); }
        remove { throw new NotSupportedException(nameof(Select)); }
    }

    public event FloatDelegate Scroll
    {
        add { onScroll = (FloatDelegate)Delegate.Combine(onScroll, value); }
        remove { throw new NotSupportedException(nameof(Scroll)); }
    }

    public event VoidDelegate DragStart
    {
        add { onDragStart = (VoidDelegate)Delegate.Combine(onDragStart, value); }
        remove { throw new NotSupportedException(nameof(DragStart)); }
    }

    public event VectorDelegate Drag
    {
        add { onDrag = (VectorDelegate)Delegate.Combine(onDrag, value); }
        remove { throw new NotSupportedException(nameof(Drag)); }
    }

    public event VoidDelegate DragOver
    {
        add { onDragOver = (VoidDelegate)Delegate.Combine(onDragOver, value); }
        remove { throw new NotSupportedException(nameof(DragOver)); }
    }

    public event VoidDelegate DragOut
    {
        add { onDragOut = (VoidDelegate)Delegate.Combine(onDragOut, value); }
        remove { throw new NotSupportedException(nameof(DragOut)); }
    }

    public event VoidDelegate DragEnd
    {
        add { onDragEnd = (VoidDelegate)Delegate.Combine(onDragEnd, value); }
        remove { throw new NotSupportedException(nameof(DragEnd)); }
    }

    public event ObjectDelegate Drop
    {
        add { onDrop = (ObjectDelegate)Delegate.Combine(onDrop, value); }
        remove { throw new NotSupportedException(nameof(Drop)); }
    }

    public event KeyCodeDelegate Key
    {
        add { onKey = (KeyCodeDelegate)Delegate.Combine(onKey, value); }
        remove { throw new NotSupportedException(nameof(Key)); }
    }

    public event KeyCodeDelegate Navigate
    {
        add { onNavigate = (KeyCodeDelegate)Delegate.Combine(onNavigate, value); }
        remove { throw new NotSupportedException(nameof(Navigate)); }
    }

    public event BoolDelegate Tooltip
    {
        add { onTooltip = (BoolDelegate)Delegate.Combine(onTooltip, value); }
        remove { throw new NotSupportedException(nameof(Tooltip)); }
    }

    #endregion

    private Boolean isColliderEnabled
    {
        get
        {
            Collider component1 = GetComponent<Collider>();
            if (component1 != null)
                return component1.enabled;
            Collider2D component2 = GetComponent<Collider2D>();
            if (component2 != null)
                return component2.enabled;
            return false;
        }
    }

    private void OnSubmit()
    {
        if (!isColliderEnabled || onSubmit == null)
            return;
        onSubmit(gameObject);
    }

    private void OnClick()
    {
        if (!isColliderEnabled || onClick == null)
            return;
        onClick(gameObject);
    }

    private void OnDoubleClick()
    {
        if (!isColliderEnabled || onDoubleClick == null)
            return;
        onDoubleClick(gameObject);
    }

    private void OnHover(Boolean isOver)
    {
        if (!isColliderEnabled || onHover == null)
            return;
        onHover(gameObject, isOver);
    }

    private void OnPress(Boolean isPressed)
    {
        if (!isColliderEnabled || onPress == null)
            return;
        onPress(gameObject, isPressed);
    }

    private void OnSelect(Boolean selected)
    {
        if (!isColliderEnabled || onSelect == null)
            return;
        onSelect(gameObject, selected);
    }

    private void OnScroll(Single delta)
    {
        if (!isColliderEnabled || onScroll == null)
            return;
        onScroll(gameObject, delta);
    }

    private void OnDragStart()
    {
        onDragStart?.Invoke(gameObject);
    }

    private void OnDrag(Vector2 delta)
    {
        onDrag?.Invoke(gameObject, delta);
    }

    private void OnDragOver()
    {
        if (!isColliderEnabled || onDragOver == null)
            return;
        onDragOver(gameObject);
    }

    private void OnDragOut()
    {
        if (!isColliderEnabled || onDragOut == null)
            return;
        onDragOut(gameObject);
    }

    private void OnDragEnd()
    {
        onDragEnd?.Invoke(gameObject);
    }

    private void OnDrop(GameObject go)
    {
        if (!isColliderEnabled || onDrop == null)
            return;
        onDrop(gameObject, go);
    }

    private void OnKey(KeyCode key)
    {
        if (!isColliderEnabled || onKey == null)
            return;
        onKey(gameObject, key);
    }

    private void OnNavigate(KeyCode key)
    {
        onNavigate?.Invoke(gameObject, key);
    }

    private void OnTooltip(Boolean show)
    {
        if (!isColliderEnabled || onTooltip == null)
            return;
        onTooltip(gameObject, show);
    }

    public static UIEventListener Get(GameObject go)
    {
        UIEventListener uiEventListener = go.GetComponent<UIEventListener>() ?? go.AddComponent<UIEventListener>();
        return uiEventListener;
    }

    public delegate void VoidDelegate(GameObject go);

    public delegate void BoolDelegate(GameObject go, Boolean state);

    public delegate void FloatDelegate(GameObject go, Single delta);

    public delegate void VectorDelegate(GameObject go, Vector2 delta);

    public delegate void ObjectDelegate(GameObject go, GameObject obj);

    public delegate void KeyCodeDelegate(GameObject go, KeyCode key);
}