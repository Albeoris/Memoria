using System;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
[RequireComponent(typeof(TweenPosition))]
[RequireComponent(typeof(TweenAlpha))]
public class HUDMessageChild : MonoBehaviour
{
	public HUDMessage.MessageStyle DisplayStyle
	{
		get
		{
			return this.displayStyle;
		}
	}

	public UIFollowTarget Follower
	{
		get
		{
			return this.follower;
		}
	}

	public Byte MessageId
	{
		get
		{
			return this.messageId;
		}
		set
		{
			this.messageId = value;
		}
	}

	public Single Countdown
	{
		get
		{
			return this.countdown;
		}
	}

	public String Label
	{
		set
		{
			this.label.text = value;
		}
	}

	public GameObject ParentGameObject
	{
		get
		{
			return this.parentGameObject;
		}
	}

	public void Initial()
	{
		if (!this.isInitialized)
		{
			this.myTransform = base.transform;
			this.parentGameObject = this.myTransform.parent.gameObject;
			this.label = base.GetComponent<UILabel>();
			this.uiWidget = base.GetComponent<UIWidget>();
			this.tweenPosition = base.GetComponent<TweenPosition>();
			this.tweenAlpha = base.GetComponent<TweenAlpha>();
			this.follower = this.myTransform.parent.GetComponent<UIFollowTarget>();
			this.follower.enabled = false;
			this.tweenPositionDuration = this.tweenPosition.duration;
			this.tweenAlphaDuration = this.tweenAlpha.duration;
			this.tweenAlpha.animationCurve = Singleton<HUDMessage>.Instance.alphaTweenCurve;
			this.isInitialized = true;
		}
	}

	private void EnableTween(Boolean isEnabled)
	{
		this.tweenPosition.enabled = isEnabled;
		this.tweenAlpha.enabled = isEnabled;
	}

	public void Show(Transform target, String message, HUDMessage.MessageStyle style, Vector3 offset)
	{
		this.displayStyle = style;
		this.follower.target = target;
		this.follower.targetTransformOffset = offset;
		this.label.text = message;
		this.tweenPosition.duration = this.tweenPositionDuration / Singleton<HUDMessage>.Instance.Speed;
		this.tweenAlpha.duration = this.tweenAlphaDuration / Singleton<HUDMessage>.Instance.Speed;
		switch (style)
		{
		case HUDMessage.MessageStyle.DAMAGE:
		case HUDMessage.MessageStyle.GUARD:
		case HUDMessage.MessageStyle.MISS:
		case HUDMessage.MessageStyle.DEATH:
			this.DamageSetting();
			break;
		case HUDMessage.MessageStyle.RESTORE_HP:
		case HUDMessage.MessageStyle.RESTORE_MP:
			this.RestoreSetting();
			break;
		case HUDMessage.MessageStyle.DEATH_SENTENCE:
			this.DeathSentencesSetting();
			break;
		case HUDMessage.MessageStyle.PETRIFY:
			this.PetrifySetting();
			break;
		case HUDMessage.MessageStyle.CRITICAL:
			this.CriticalSetting();
			break;
		}
		this.PlayAnimation();
		this.PrintLog(true);
	}

	private void DamageSetting()
	{
		this.uiWidget.color = Singleton<HUDMessage>.Instance.damageColor;
		this.tweenPosition.animationCurve = Singleton<HUDMessage>.Instance.damageTweenCurve;
		this.tweenPosition.to = HUDMessage.NormalTargetPosition;
		this.EnableTween(true);
		this.hideWithTarget = false;
	}

	private void CriticalSetting()
	{
		this.uiWidget.color = Singleton<HUDMessage>.Instance.criticalColor;
		this.tweenPosition.animationCurve = Singleton<HUDMessage>.Instance.criticalTweenCurve;
		this.tweenPosition.to = HUDMessage.NormalTargetPosition;
		this.EnableTween(true);
		this.hideWithTarget = false;
	}

	private void RestoreSetting()
	{
		this.label.text = NGUIText.EncodeColor(this.label.text, Singleton<HUDMessage>.Instance.restoreColor);
		this.tweenPosition.animationCurve = Singleton<HUDMessage>.Instance.restoreTweenCurve;
		this.tweenPosition.to = HUDMessage.RecoverTargetPosition;
		this.EnableTween(true);
		this.hideWithTarget = false;
	}

	private void DeathSentencesSetting()
	{
		this.uiWidget.color = Singleton<HUDMessage>.Instance.damageColor;
		this.EnableTween(false);
		this.hideWithTarget = true;
	}

	private void PetrifySetting()
	{
		this.uiWidget.color = Singleton<HUDMessage>.Instance.damageColor;
		this.EnableTween(false);
		this.hideWithTarget = true;
	}

	private void PlayAnimation()
	{
		this.parentGameObject.SetActive(true);
	}

	private void PrintLog(Boolean isShow)
	{
	}

	public void SetupCamera(Camera worldCamera, Camera uiCamera)
	{
		this.follower.worldCam = worldCamera;
		this.follower.uiCam = uiCamera;
	}

	public void Clear()
	{
		if (this.isInitialized)
		{
			this.PrintLog(false);
			this.parentGameObject.SetActive(false);
			this.EnableTween(false);
			this.tweenPosition.duration = this.tweenPositionDuration;
			this.tweenAlpha.duration = this.tweenAlphaDuration;
			this.tweenPosition.ResetToBeginning();
			this.tweenAlpha.ResetToBeginning();
			this.displayStyle = HUDMessage.MessageStyle.NONE;
			Singleton<HUDMessage>.Instance.FinishMessage(this.messageId);
		}
	}

	public void Pause(Boolean isPause)
	{
		this.tweenPosition.IsPause = isPause;
		this.tweenAlpha.IsPause = isPause;
	}

	private void Update()
	{
		if (!this.isInitialized || this.displayStyle == HUDMessage.MessageStyle.NONE || this.follower.target == null)
			return;
		if (this.hideWithTarget && !this.follower.target.gameObject.activeInHierarchy)
			this.label.text = String.Empty;
	}

	[SerializeField]
	private GameObject parentGameObject;

	private Transform myTransform;

	private UILabel label;

	private UIWidget uiWidget;

	private TweenPosition tweenPosition;

	private TweenAlpha tweenAlpha;

	private UIFollowTarget follower;

	[SerializeField]
	private HUDMessage.MessageStyle displayStyle;

	private Single tweenPositionDuration;

	private Single tweenAlphaDuration;

	[SerializeField]
	private Byte messageId;

	private Single countdown;

	private Boolean isInitialized;

	[NonSerialized]
	private Boolean hideWithTarget;
}
