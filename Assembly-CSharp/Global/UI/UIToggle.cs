using AnimationOrTween;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Toggle")]
[ExecuteInEditMode]
public class UIToggle : UIWidgetContainer
{
    public Boolean value
    {
        get
        {
            return (!this.mStarted) ? this.startsActive : this.mIsActive;
        }
        set
        {
            if (!this.mStarted)
            {
                this.startsActive = value;
            }
            else if (this.group == 0 || value || this.optionCanBeNone || !this.mStarted)
            {
                this.Set(value);
            }
        }
    }

    public Boolean isColliderEnabled
    {
        get
        {
            Collider component = base.GetComponent<Collider>();
            if (component != (UnityEngine.Object)null)
            {
                return component.enabled;
            }
            Collider2D component2 = base.GetComponent<Collider2D>();
            return component2 != (UnityEngine.Object)null && component2.enabled;
        }
    }

    [Obsolete("Use 'value' instead")]
    public Boolean isChecked
    {
        get
        {
            return this.value;
        }
        set
        {
            this.value = value;
        }
    }

    public static UIToggle GetActiveToggle(Int32 group)
    {
        for (Int32 i = 0; i < UIToggle.list.size; i++)
        {
            UIToggle uitoggle = UIToggle.list[i];
            if (uitoggle != (UnityEngine.Object)null && uitoggle.group == group && uitoggle.mIsActive)
            {
                return uitoggle;
            }
        }
        return (UIToggle)null;
    }

    private void OnEnable()
    {
        UIToggle.list.Add(this);
    }

    private void OnDisable()
    {
        UIToggle.list.Remove(this);
    }

    private void Start()
    {
        if (this.startsChecked)
        {
            this.startsChecked = false;
            this.startsActive = true;
        }
        if (!Application.isPlaying)
        {
            if (this.checkSprite != (UnityEngine.Object)null && this.activeSprite == (UnityEngine.Object)null)
            {
                this.activeSprite = this.checkSprite;
                this.checkSprite = (UISprite)null;
            }
            if (this.checkAnimation != (UnityEngine.Object)null && this.activeAnimation == (UnityEngine.Object)null)
            {
                this.activeAnimation = this.checkAnimation;
                this.checkAnimation = (Animation)null;
            }
            if (Application.isPlaying && this.activeSprite != (UnityEngine.Object)null)
            {
                this.activeSprite.alpha = ((!this.startsActive) ? 0f : this.activeSpriteAlpha);
            }
            if (EventDelegate.IsValid(this.onChange))
            {
                this.eventReceiver = (GameObject)null;
                this.functionName = (String)null;
            }
        }
        else
        {
            this.mIsActive = !this.startsActive;
            this.mStarted = true;
            Boolean flag = this.instantTween;
            this.instantTween = true;
            this.Set(this.startsActive);
            this.instantTween = flag;
        }
    }

    private void OnClick()
    {
        if (base.enabled && this.isColliderEnabled && UICamera.currentTouchID != -2)
        {
            this.value = !this.value;
        }
    }

    public void Set(Boolean state)
    {
        if (this.validator != null && !this.validator(state))
        {
            return;
        }
        if (!this.mStarted)
        {
            this.mIsActive = state;
            this.startsActive = state;
            if (this.activeSprite != (UnityEngine.Object)null)
            {
                this.activeSprite.alpha = ((!state) ? 0f : this.activeSpriteAlpha);
            }
            if (this.idleSprite != (UnityEngine.Object)null)
            {
                this.idleSprite.alpha = (state ? 0f : this.activeSpriteAlpha);
            }
        }
        else if (this.mIsActive != state)
        {
            if (this.group != 0 && state)
            {
                Int32 i = 0;
                Int32 size = UIToggle.list.size;
                while (i < size)
                {
                    UIToggle uitoggle = UIToggle.list[i];
                    if (uitoggle != this && uitoggle.group == this.group)
                    {
                        uitoggle.Set(false);
                    }
                    if (UIToggle.list.size != size)
                    {
                        size = UIToggle.list.size;
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            this.mIsActive = state;
            if (this.activeSprite != (UnityEngine.Object)null)
            {
                if (this.instantTween || !NGUITools.GetActive(this))
                {
                    this.activeSprite.alpha = ((!this.mIsActive) ? 0f : this.activeSpriteAlpha);
                }
                else
                {
                    TweenAlpha.Begin(this.activeSprite.gameObject, 0.15f, (!this.mIsActive) ? 0f : this.activeSpriteAlpha);
                }
            }
            if (this.idleSprite != (UnityEngine.Object)null)
            {
                if (this.instantTween || !NGUITools.GetActive(this))
                {
                    this.idleSprite.alpha = (this.mIsActive ? 0f : this.activeSpriteAlpha);
                }
                else
                {
                    TweenAlpha.Begin(this.idleSprite.gameObject, 0.15f, this.mIsActive ? 0f : this.activeSpriteAlpha);
                }
            }
            if (UIToggle.current == (UnityEngine.Object)null)
            {
                UIToggle uitoggle2 = UIToggle.current;
                UIToggle.current = this;
                if (EventDelegate.IsValid(this.onChange))
                {
                    EventDelegate.Execute(this.onChange);
                }
                else if (this.eventReceiver != (UnityEngine.Object)null && !String.IsNullOrEmpty(this.functionName))
                {
                    this.eventReceiver.SendMessage(this.functionName, this.mIsActive, SendMessageOptions.DontRequireReceiver);
                }
                UIToggle.current = uitoggle2;
            }
            if (this.animator != (UnityEngine.Object)null)
            {
                ActiveAnimation activeAnimation = ActiveAnimation.Play(this.animator, (String)null, (Direction)((!state) ? Direction.Reverse : Direction.Forward), EnableCondition.IgnoreDisabledState, DisableCondition.DoNotDisable);
                if (activeAnimation != (UnityEngine.Object)null && (this.instantTween || !NGUITools.GetActive(this)))
                {
                    activeAnimation.Finish();
                }
            }
            else if (this.activeAnimation != (UnityEngine.Object)null)
            {
                ActiveAnimation activeAnimation2 = ActiveAnimation.Play(this.activeAnimation, (String)null, (Direction)((!state) ? Direction.Reverse : Direction.Forward), EnableCondition.IgnoreDisabledState, DisableCondition.DoNotDisable);
                if (activeAnimation2 != (UnityEngine.Object)null && (this.instantTween || !NGUITools.GetActive(this)))
                {
                    activeAnimation2.Finish();
                }
            }
        }
    }

    public static BetterList<UIToggle> list = new BetterList<UIToggle>();

    public static UIToggle current;

    public Int32 group;

    public UIWidget idleSprite;

    public UIWidget activeSprite;

    public Animation activeAnimation;

    public Animator animator;

    public Boolean startsActive;

    public Boolean instantTween;

    public Boolean optionCanBeNone;

    public List<EventDelegate> onChange = new List<EventDelegate>();

    public UIToggle.Validate validator;

    public Single activeSpriteAlpha = 1f;

    [HideInInspector]
    [SerializeField]
    private UISprite checkSprite;

    [SerializeField]
    [HideInInspector]
    private Animation checkAnimation;

    [SerializeField]
    [HideInInspector]
    private GameObject eventReceiver;

    [HideInInspector]
    [SerializeField]
    private String functionName = "OnActivate";

    [HideInInspector]
    [SerializeField]
    private Boolean startsChecked;

    private Boolean mIsActive = true;

    private Boolean mStarted;

    public delegate Boolean Validate(Boolean choice);
}
