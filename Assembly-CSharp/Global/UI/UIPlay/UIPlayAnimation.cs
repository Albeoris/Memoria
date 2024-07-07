using AnimationOrTween;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Play Animation")]
[ExecuteInEditMode]
public class UIPlayAnimation : MonoBehaviour
{
    private Boolean dualState
    {
        get
        {
            return this.trigger == Trigger.OnPress || this.trigger == Trigger.OnHover;
        }
    }

    private void Awake()
    {
        UIButton component = base.GetComponent<UIButton>();
        if (component != (UnityEngine.Object)null)
        {
            this.dragHighlight = component.dragHighlight;
        }
        if (this.eventReceiver != (UnityEngine.Object)null && EventDelegate.IsValid(this.onFinished))
        {
            this.eventReceiver = (GameObject)null;
            this.callWhenFinished = (String)null;
        }
    }

    private void Start()
    {
        this.mStarted = true;
        if (this.target == (UnityEngine.Object)null && this.animator == (UnityEngine.Object)null)
        {
            this.animator = base.GetComponentInChildren<Animator>();
        }
        if (this.animator != (UnityEngine.Object)null)
        {
            if (this.animator.enabled)
            {
                this.animator.enabled = false;
            }
            return;
        }
        if (this.target == (UnityEngine.Object)null)
        {
            this.target = base.GetComponentInChildren<Animation>();
        }
        if (this.target != (UnityEngine.Object)null && this.target.enabled)
        {
            this.target.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (this.mStarted)
        {
            this.OnHover(UICamera.IsHighlighted(base.gameObject));
        }
        if (UICamera.currentTouch != null)
        {
            if (this.trigger == Trigger.OnPress || this.trigger == Trigger.OnPressTrue)
            {
                this.mActivated = (UICamera.currentTouch.pressed == base.gameObject);
            }
            if (this.trigger == Trigger.OnHover || this.trigger == Trigger.OnHoverTrue)
            {
                this.mActivated = (UICamera.currentTouch.current == base.gameObject);
            }
        }
        UIToggle component = base.GetComponent<UIToggle>();
        if (component != (UnityEngine.Object)null)
        {
            EventDelegate.Add(component.onChange, new EventDelegate.Callback(this.OnToggle));
        }
    }

    private void OnDisable()
    {
        UIToggle component = base.GetComponent<UIToggle>();
        if (component != (UnityEngine.Object)null)
        {
            EventDelegate.Remove(component.onChange, new EventDelegate.Callback(this.OnToggle));
        }
    }

    private void OnHover(Boolean isOver)
    {
        if (!base.enabled)
        {
            return;
        }
        if (this.trigger == Trigger.OnHover || (this.trigger == Trigger.OnHoverTrue && isOver) || (this.trigger == Trigger.OnHoverFalse && !isOver))
        {
            this.Play(isOver, this.dualState);
        }
    }

    private void OnPress(Boolean isPressed)
    {
        if (!base.enabled)
        {
            return;
        }
        if (UICamera.currentTouchID < -1 && UICamera.currentScheme != UICamera.ControlScheme.Controller)
        {
            return;
        }
        if (this.trigger == Trigger.OnPress || (this.trigger == Trigger.OnPressTrue && isPressed) || (this.trigger == Trigger.OnPressFalse && !isPressed))
        {
            this.Play(isPressed, this.dualState);
        }
    }

    private void OnClick()
    {
        if (UICamera.currentTouchID < -1 && UICamera.currentScheme != UICamera.ControlScheme.Controller)
        {
            return;
        }
        if (base.enabled && this.trigger == Trigger.OnClick)
        {
            this.Play(true, false);
        }
    }

    private void OnDoubleClick()
    {
        if (UICamera.currentTouchID < -1 && UICamera.currentScheme != UICamera.ControlScheme.Controller)
        {
            return;
        }
        if (base.enabled && this.trigger == Trigger.OnDoubleClick)
        {
            this.Play(true, false);
        }
    }

    private void OnSelect(Boolean isSelected)
    {
        if (!base.enabled)
        {
            return;
        }
        if (this.trigger == Trigger.OnSelect || (this.trigger == Trigger.OnSelectTrue && isSelected) || (this.trigger == Trigger.OnSelectFalse && !isSelected))
        {
            this.Play(isSelected, this.dualState);
        }
    }

    private void OnToggle()
    {
        if (!base.enabled || UIToggle.current == (UnityEngine.Object)null)
        {
            return;
        }
        if (this.trigger == Trigger.OnActivate || (this.trigger == Trigger.OnActivateTrue && UIToggle.current.value) || (this.trigger == Trigger.OnActivateFalse && !UIToggle.current.value))
        {
            this.Play(UIToggle.current.value, this.dualState);
        }
    }

    private void OnDragOver()
    {
        if (base.enabled && this.dualState)
        {
            if (UICamera.currentTouch.dragged == base.gameObject)
            {
                this.Play(true, true);
            }
            else if (this.dragHighlight && this.trigger == Trigger.OnPress)
            {
                this.Play(true, true);
            }
        }
    }

    private void OnDragOut()
    {
        if (base.enabled && this.dualState && UICamera.hoveredObject != base.gameObject)
        {
            this.Play(false, true);
        }
    }

    private void OnDrop(GameObject go)
    {
        if (base.enabled && this.trigger == Trigger.OnPress && UICamera.currentTouch.dragged != base.gameObject)
        {
            this.Play(false, true);
        }
    }

    public void Play(Boolean forward)
    {
        this.Play(forward, true);
    }

    public void Play(Boolean forward, Boolean onlyIfDifferent)
    {
        if (this.target || this.animator)
        {
            if (onlyIfDifferent)
            {
                if (this.mActivated == forward)
                {
                    return;
                }
                this.mActivated = forward;
            }
            if (this.clearSelection && UICamera.selectedObject == base.gameObject)
            {
                UICamera.selectedObject = (GameObject)null;
            }
            Int32 num = (Int32)(-(Int32)this.playDirection);
            Direction direction = (Direction)((!forward) ? num : ((Int32)this.playDirection));
            ActiveAnimation activeAnimation = (!this.target) ? ActiveAnimation.Play(this.animator, this.clipName, direction, this.ifDisabledOnPlay, this.disableWhenFinished) : ActiveAnimation.Play(this.target, this.clipName, direction, this.ifDisabledOnPlay, this.disableWhenFinished);
            if (activeAnimation != (UnityEngine.Object)null)
            {
                if (this.resetOnPlay)
                {
                    activeAnimation.Reset();
                }
                for (Int32 i = 0; i < this.onFinished.Count; i++)
                {
                    EventDelegate.Add(activeAnimation.onFinished, new EventDelegate.Callback(this.OnFinished), true);
                }
            }
        }
    }

    public void PlayForward()
    {
        this.Play(true);
    }

    public void PlayReverse()
    {
        this.Play(false);
    }

    private void OnFinished()
    {
        if (UIPlayAnimation.current == (UnityEngine.Object)null)
        {
            UIPlayAnimation.current = this;
            EventDelegate.Execute(this.onFinished);
            if (this.eventReceiver != (UnityEngine.Object)null && !String.IsNullOrEmpty(this.callWhenFinished))
            {
                this.eventReceiver.SendMessage(this.callWhenFinished, SendMessageOptions.DontRequireReceiver);
            }
            this.eventReceiver = (GameObject)null;
            UIPlayAnimation.current = (UIPlayAnimation)null;
        }
    }

    public static UIPlayAnimation current;

    public Animation target;

    public Animator animator;

    public String clipName;

    public Trigger trigger;

    public Direction playDirection = Direction.Forward;

    public Boolean resetOnPlay;

    public Boolean clearSelection;

    public EnableCondition ifDisabledOnPlay;

    public DisableCondition disableWhenFinished;

    public List<EventDelegate> onFinished = new List<EventDelegate>();

    [HideInInspector]
    [SerializeField]
    private GameObject eventReceiver;

    [SerializeField]
    [HideInInspector]
    private String callWhenFinished;

    private Boolean mStarted;

    private Boolean mActivated;

    private Boolean dragHighlight;
}
