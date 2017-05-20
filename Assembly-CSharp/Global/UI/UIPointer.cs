using System;
using UnityEngine;

public class UIPointer : MonoBehaviour
{
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
		get
		{
			return this.pointerOffset;
		}
		set
		{
			this.pointerOffset = value;
		}
	}

	public Vector4 PointerLimitRect
	{
		get
		{
			return this.pointerLimitRect;
		}
		set
		{
			this.pointerLimitRect = value;
		}
	}

	public void AttachToGameObject(Transform target, Vector3 referenceVector, Vector2 pointerOffset, Vector4 pointerLimitRect)
	{
		this.target = target;
		this.referenceVector = referenceVector;
		this.pointerOffset = pointerOffset;
		this.pointerLimitRect = pointerLimitRect;
		this.RefreshPosition();
	}

	private void RefreshPosition()
	{
		base.transform.position = this.target.position;
		Vector3 localPosition = base.transform.localPosition + this.referenceVector;
		localPosition.x += this.pointerOffset.x;
		localPosition.y += this.pointerOffset.y;
		if (this.behavior == PointerManager.LimitRectBehavior.Hide)
		{
			if (Mathf.Floor(localPosition.x) + 2f < this.pointerLimitRect.x || Mathf.Floor(localPosition.x) - 2f > this.pointerLimitRect.z || Mathf.Floor(localPosition.y) + 2f < this.pointerLimitRect.y || Mathf.Floor(localPosition.y) - 2f > this.pointerLimitRect.w)
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
			localPosition.x = Mathf.Clamp(localPosition.x, this.pointerLimitRect.x, this.pointerLimitRect.z);
			localPosition.y = Mathf.Clamp(localPosition.y, this.pointerLimitRect.y, this.pointerLimitRect.w);
		}
		base.transform.localPosition = localPosition;
		if (ButtonGroupState.HelpEnabled && ButtonGroupState.ActiveGroup != String.Empty && ButtonGroupState.ActiveButton != (UnityEngine.Object)null)
		{
			ButtonGroupState component = ButtonGroupState.ActiveButton.GetComponent<ButtonGroupState>();
			if (component != (UnityEngine.Object)null && component.Help.Enable)
			{
				ButtonGroupState.ShowHelpDialog(ButtonGroupState.ActiveButton);
			}
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
				{
					this.SetMoggleAnimation(isActive);
				}
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
			{
				ButtonGroupState.ShowHelpDialog(ButtonGroupState.ActiveButton);
			}
		}
	}

	public void SetNumberActive(Boolean isActive, Int32 number)
	{
	    UISprite sprite = this.numberSprite;
	    if (sprite == null)
	        return;

	    GameObject gameObj = sprite.gameObject;
        if (ReferenceEquals(gameObj, null))
	        return;

        if (isActive)
		{
            gameObj.SetActive(true);
            sprite.spriteName = (number != 1) ? "hand_battle_2" : "hand_battle_1";
		}
		else
		{
            gameObj.SetActive(false);
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
		if (this.target == (UnityEngine.Object)null)
		{
			Singleton<PointerManager>.Instance.ReleasePointerToPool(this);
			return;
		}
		if (this.target.position != this.lastPosition)
		{
			this.RefreshPosition();
		}
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
}
