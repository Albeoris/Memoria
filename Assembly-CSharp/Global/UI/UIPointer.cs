using System;
using UnityEngine;
using Memoria.Assets;

public class UIPointer : MonoBehaviour
{
    public static Vector2 PointerSize { get; private set; }

    public PointerManager.LimitRectBehavior Behavior
    {
        set
        {
            if (this.behavior != value)
            {
                this.behavior = value;
                this.RefreshPosition();
            }
        }
    }

    public Vector2 PointerOffset
    {
        get => this.pointerOffset;
        set => this.pointerOffset = value;
    }

    public Vector4 PointerLimitRect
    {
        get => this.pointerLimitRect;
        set => this.pointerLimitRect = value;
    }

    public void AttachToGameObject(Transform target, Boolean autoUpdatePos, Vector2 pointerOffset, Vector4 pointerLimitRect)
    {
        UIPointer.PointerSize = new Vector2(this.GetComponent<UIWidget>().width, this.GetComponent<UIWidget>().height);
        this.target = target;
        this.referenceVector = Vector3.zero;
        this.autoUpdateReference = autoUpdatePos;
        this.pointerOffset = pointerOffset;
        this.pointerLimitRect = pointerLimitRect;
        this.RefreshPosition();
    }

    private void RefreshPosition()
    {
        Boolean invertPointer = NGUIText.readingDirection == UnicodeBIDI.LanguageReadingDirection.RightToLeft;
        base.transform.position = this.target.position;
        if (autoUpdateReference)
        {
            UIWidget pointerWidget = this.GetComponent<UIWidget>();
            UIWidget targetWidget = this.target.GetComponent<UIWidget>();
            if (targetWidget.pivot != UIWidget.Pivot.Center)
                base.transform.position = targetWidget.worldCenter;
            this.referenceVector = new Vector3(-targetWidget.width / 2f + pointerWidget.width / 4f, -pointerWidget.height / 4f, 0f);
            if (invertPointer)
                this.referenceVector.x *= -1;
        }
        Vector3 targetPosition = base.transform.localPosition + this.referenceVector;
        targetPosition.x += invertPointer ? -this.pointerOffset.x : this.pointerOffset.x;
        targetPosition.y += this.pointerOffset.y;
        if (this.behavior == PointerManager.LimitRectBehavior.Hide)
        {
            if (Mathf.Floor(targetPosition.x) + 2f < this.pointerLimitRect.x || Mathf.Floor(targetPosition.x) - 2f > this.pointerLimitRect.z || Mathf.Floor(targetPosition.y) + 2f < this.pointerLimitRect.y || Mathf.Floor(targetPosition.y) - 2f > this.pointerLimitRect.w)
            {
                if (!this.isHidden)
                {
                    this.lastBlinkStat = this.blinkActive;
                    this.SetBlinkActive(false);
                    this.isHidden = true;
                    this.pointerSprite.alpha = 0f;
                }
            }
            else if (this.isHidden)
            {
                this.isHidden = false;
                this.pointerSprite.alpha = 1f;
                this.SetBlinkActive(this.lastBlinkStat);
            }
        }
        else if (this.behavior == PointerManager.LimitRectBehavior.Limit)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, this.pointerLimitRect.x, this.pointerLimitRect.z);
            targetPosition.y = Mathf.Clamp(targetPosition.y, this.pointerLimitRect.y, this.pointerLimitRect.w);
        }
        base.transform.localPosition = targetPosition;
        base.transform.localScale = new Vector3(invertPointer ? -1f : 1f, 1f, 1f);
        if (ButtonGroupState.HelpEnabled && ButtonGroupState.ActiveGroup != String.Empty && ButtonGroupState.ActiveButton != null)
        {
            ButtonGroupState button = ButtonGroupState.ActiveButton.GetComponent<ButtonGroupState>();
            if (button != null && button.Help.Enable)
                ButtonGroupState.ShowHelpDialog(ButtonGroupState.ActiveButton);
        }
        this.lastPosition = this.target.position;
    }

    public void SetActive(Boolean isActive)
    {
        base.gameObject.SetActive(isActive);
        if (isActive)
        {
            this.lastPosition = default(Vector3);
            if (!this.isHidden)
            {
                this.pointerSprite.color = Color.white;
                this.pointerTween.enabled = false;
            }
        }
    }

    public void SetBlinkActive(Boolean isActive)
    {
        if (!this.isHidden)
        {
            this.blinkActive = isActive;
            if (this.pointerSprite != null)
                this.pointerSprite.color = Color.white;
            if (this.pointerTween != null)
                this.pointerTween.enabled = isActive;
        }
    }

    public void SetHelpActive(Boolean isActive, Boolean isImmediate)
    {
        if (PersistenSingleton<UIManager>.Instance.UnityScene != UIManager.Scene.Battle)
        {
            if (isActive)
            {
                this.mogSprite.gameObject.SetActive(true);
                if (!isImmediate)
                    this.SetMoggleAnimation(isActive);
            }
            else
            {
                this.SetMoggleAnimation(isActive);
            }
        }
        else
        {
            this.SetMoggleAnimation(false);
            if (isActive)
                ButtonGroupState.ShowHelpDialog(ButtonGroupState.ActiveButton);
        }
    }

    public void SetNumberActive(Boolean isActive, Int32 number)
    {
        GameObject numberGo = this.numberSprite?.gameObject;
        if (numberGo == null)
            return;
        if (isActive)
        {
            numberGo.SetActive(true);
            this.numberSprite.spriteName = $"hand_battle_{number}";
        }
        else
        {
            numberGo.SetActive(false);
        }
    }

    public void Reset()
    {
        this.isHidden = false;
        this.behavior = PointerManager.LimitRectBehavior.Limit;
        this.SetBlinkActive(false);
        this.SetNumberActive(false, 0);
        this.SetActive(false);
        this.blinkActive = this.lastBlinkStat;
    }

    private void SetMoggleAnimation(Boolean isShow)
    {
        if (isShow)
        {
            this.mogSprite.spriteName = "help_mog_hand_1";
            this.mogTween.ResetToBeginning();
            this.mogTween.PlayForward();
        }
        else
        {
            this.mogSprite.gameObject.SetActive(false);
        }
    }

    private void OnFinishMogTween()
    {
        this.mogSprite.spriteName = "help_mog_hand_2";
        ButtonGroupState.ShowHelpDialog(ButtonGroupState.ActiveButton);
    }

    private void Update()
    {
        if (this.target == null)
        {
            Singleton<PointerManager>.Instance.ReleasePointerToPool(this);
            return;
        }
        if (this.target.position != this.lastPosition)
            this.RefreshPosition();
    }

    private void Awake()
    {
        this.pointerTween = base.gameObject.GetComponent<TweenAlpha>();
        this.pointerSprite = base.gameObject.GetComponent<UISprite>();
        this.numberSprite = base.gameObject.GetChild(0).GetComponent<UISprite>();
        this.mogTween = base.gameObject.GetChild(1).GetComponent<TweenScale>();
        this.mogSprite = base.gameObject.GetChild(1).GetComponent<UISprite>();
        EventDelegate.Add(this.mogTween.onFinished, new EventDelegate.Callback(this.OnFinishMogTween));
    }

    private TweenAlpha pointerTween;
    private UISprite pointerSprite;
    private TweenScale mogTween;
    private UISprite mogSprite;
    private UISprite numberSprite;

    private Vector3 referenceVector;

    [SerializeField]
    private Boolean helpActive;

    [SerializeField]
    private Boolean blinkActive;

    [SerializeField]
    private Vector2 pointerOffset;

    [SerializeField]
    private PointerManager.LimitRectBehavior behavior;

    [SerializeField]
    private Vector4 pointerLimitRect;

    [SerializeField]
    private Transform target;

    private Vector3 lastPosition;
    private Boolean lastBlinkStat;
    private Boolean isHidden;

    [NonSerialized]
    private Boolean autoUpdateReference;
}
