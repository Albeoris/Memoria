using System;
using UnityEngine;

public class HonoRearTouchManager : PersistenSingleton<HonoRearTouchManager>
{
	public Boolean IsLeftPressed
	{
		get
		{
			return this.isLeftPressed;
		}
	}

	public Boolean IsLeftUp
	{
		get
		{
			return this.isLeftUp;
		}
	}

	public Boolean IsLeftDown
	{
		get
		{
			return this.isLeftDown;
		}
	}

	public Boolean IsRightPressed
	{
		get
		{
			return this.isRightPressed;
		}
	}

	public Boolean IsRightUp
	{
		get
		{
			return this.isRightUp;
		}
	}

	public Boolean IsRightDown
	{
		get
		{
			return this.isRightDown;
		}
	}

	private void Start()
	{
		this.leftFingerTouch.phase = TouchPhase.Canceled;
		this.rightFingerTouch.phase = TouchPhase.Canceled;
	}

	private RearTouch CreateRearTouch(Touch input)
	{
		return new RearTouch
		{
			deltaPosition = input.deltaPosition,
			deltaTime = input.deltaTime,
			fingerId = input.fingerId,
			phase = input.phase,
			position = input.position
		};
	}

	private void ClearTriggerFlags()
	{
		this.isLeftPressed = false;
		this.isRightPressed = false;
		this.isLeftUp = false;
		this.isRightUp = false;
		this.rightTouchNumber = 0;
		this.leftTouchNumber = 0;
		this.isRightDown = false;
		this.isLeftDown = false;
	}

	private const Single TouchPadHeight = 544f;

	private const Single TouchPadWidth = 960f;

	private static readonly Single HalfWidth = 480f;

	private Boolean isLeftPressed;

	private Boolean isLeftUp;

	private Boolean isRightPressed;

	private Boolean isRightUp;

	private RearTouch leftFingerTouch = default(RearTouch);

	private RearTouch rightFingerTouch = default(RearTouch);

	private Int32 rightTouchNumber;

	private Int32 leftTouchNumber;

	private Boolean isRightDown;

	private Boolean isLeftDown;
}
