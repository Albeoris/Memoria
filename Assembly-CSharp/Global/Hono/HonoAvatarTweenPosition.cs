using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TweenPosition))]
public class HonoAvatarTweenPosition : MonoBehaviour
{
	public Boolean Busy
	{
		get
		{
			return this.busy;
		}
	}

	private void Awake()
	{
		this.Initial();
	}

	private void Initial()
	{
		this.bgParent = this.bgAvatar.transform.parent;
		this.panel.transform.position = this.bgAvatar.transform.position;
		this.panel.transform.localScale = Vector3.one;
		this.tp = base.GetComponent<TweenPosition>();
		this.myUISprite = base.GetComponent<UISprite>();
		this.tp.enabled = false;
		this.myTransform = base.transform;
	}

	private void OnEnable()
	{
		this.myUISprite.width = this.bgAvatar.width;
		this.myUISprite.height = this.bgAvatar.height;
		this.bgAvatar.transform.localScale = Vector3.one;
	}

	public void Change(String spritName, HonoAvatarTweenPosition.Direction direction, Boolean isKnockOut, Action callBack)
	{
		if (!this.busy && base.gameObject.activeInHierarchy)
		{
			this.busy = true;
			base.StartCoroutine(this.Animate(spritName, direction, isKnockOut, callBack));
		}
	}

	private IEnumerator Animate(String spriteName, HonoAvatarTweenPosition.Direction direction, Boolean isKnockOut, Action callBack)
	{
		this.myUISprite.transform.localScale = Vector3.one;
		yield return new WaitForEndOfFrame();
		this.myUISprite.transform.position = new Vector3(this.startPositionXOffset, this.bgAvatar.transform.position.y);
		if (direction == HonoAvatarTweenPosition.Direction.LeftToRight)
		{
			this.tp.from = new Vector3(-this.startPositionXOffset, 0f, this.myTransform.localPosition.z);
			this.tp.to = Vector3.zero;
		}
		else
		{
			this.tp.from = new Vector3(this.startPositionXOffset, 0f, this.myTransform.localPosition.z);
			this.tp.to = Vector3.zero;
		}
		this.myTransform.localPosition = this.tp.from;
		yield return new WaitForEndOfFrame();
		this.myUISprite.spriteName = spriteName;
		this.myUISprite.alpha = ((!isKnockOut) ? 1f : 0.5f);
		yield return new WaitForEndOfFrame();
		this.tp.ResetToBeginning();
		this.tp.enabled = true;
		yield return new WaitForSeconds(this.tp.duration);
		this.busy = false;
		this.myUISprite.transform.localScale = Vector3.one;
		this.myUISprite.alpha = 0f;
		if (callBack != null)
		{
			callBack();
		}
		yield break;
	}

	private void OnDisable()
	{
		this.myTransform.localPosition = new Vector3(this.startPositionXOffset, 0f);
		this.tp.enabled = false;
	}

	private Boolean busy;

	private TweenPosition tp;

	private UISprite myUISprite;

	private Transform myTransform;

	private Int32 picNum;

	private Transform bgParent;

	public UIPanel panel;

	public UISprite bgAvatar;

	public Single startPositionXOffset = 132f;

	public Boolean debug;

	public enum Direction
	{
		LeftToRight,
		RightToLeft
	}
}
