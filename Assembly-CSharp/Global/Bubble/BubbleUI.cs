using System;
using System.Collections.Generic;
using UnityEngine;

public class BubbleUI : Singleton<BubbleUI>
{
	public Boolean IsActive
	{
		get
		{
			this.activeSelf = (this.suspectedButton.gameObject.activeSelf || this.helpButton.gameObject.activeSelf || this.duelButton.gameObject.activeSelf || this.cursor.gameObject.activeSelf || this.beachButton.gameObject.activeSelf);
			return this.activeSelf;
		}
	}

	public UIFieldMapFollowTarget Follower
	{
		get
		{
			if (this.follower == (UnityEngine.Object)null)
			{
				this.follower = base.GetComponent<UIFieldMapFollowTarget>();
			}
			return this.follower;
		}
	}

	public UIFollowTarget WorldFollower
	{
		get
		{
			if (this.worldFollower == (UnityEngine.Object)null)
			{
				this.worldFollower = base.GetComponent<UIFollowTarget>();
				this.worldFollower = ((!(this.worldFollower == (UnityEngine.Object)null)) ? this.worldFollower : base.gameObject.AddComponent<UIFollowTarget>());
			}
			return this.worldFollower;
		}
	}

	public Single AnimationDuration
	{
		get
		{
			return (!HonoBehaviorSystem.Instance.IsFastForwardModeActive()) ? 0.175f : (0.175f / (Single)FF9StateSystem.Settings.FastForwardFactor);
		}
	}

	public void ResetInput()
	{
		this.currentInput = (BubbleInputInfo)null;
	}

	private void SetInput(PosObj po, Obj coll, UInt32 buttonCode)
	{
		this.currentInput = new BubbleInputInfo(po, coll, buttonCode);
	}

	public void Show(Vector3 uiPos, BubbleUI.Flag[] flags, Action<PosObj, Obj, UInt32>[] listener)
	{
		this.WorldFollower.enabled = false;
		this.Follower.enabled = false;
		base.transform.localPosition = uiPos;
		this.InitialBubble((PosObj)null, (Obj)null, flags, listener);
	}

	public void Show(Transform target, PosObj po, Obj coll, Camera worldCam, Vector3 transformOffset, Vector3 uiOffset, BubbleUI.Flag[] flags, Action<PosObj, Obj, UInt32>[] listener)
	{
		this.Follower.enabled = false;
		this.WorldFollower.enabled = true;
		this.WorldFollower.target = target;
		this.WorldFollower.targetTransformOffset = transformOffset;
		this.WorldFollower.worldCam = worldCam;
		this.WorldFollower.UIOffset = uiOffset;
		this.WorldFollower.updateEveryFrame = true;
		this.InitialBubble(po, coll, flags, listener);
	}

	public void Show(Transform target, PosObj po, Obj coll, FieldMap fieldMap, Vector3 offset, BubbleUI.Flag[] flags, Action<PosObj, Obj, UInt32>[] listener)
	{
		this.WorldFollower.enabled = false;
		this.Follower.enabled = true;
		this.Follower.target = target;
		this.Follower.fieldMap = fieldMap;
		this.Follower.TransformOffset = offset;
		this.Follower.UIOffset = BubbleUI.UIDefaultOffset;
		this.Follower.updateEveryFrame = true;
		this.InitialBubble(po, coll, flags, listener);
	}

	public void UpdateWorldActor(PosObj po)
	{
		Vector3 uidefaultOffset = BubbleUI.UIDefaultOffset;
		Vector3 targetTransformOffset;
		EIcon.GetWorldActorOffset(out targetTransformOffset, ref uidefaultOffset);
		this.WorldFollower.target = po.go.transform;
		this.WorldFollower.targetTransformOffset = targetTransformOffset;
		this.WorldFollower.UIOffset = uidefaultOffset;
	}

	private void InitialBubble(PosObj po, Obj coll, BubbleUI.Flag[] flags, Action<PosObj, Obj, UInt32>[] listener)
	{
		this.currentPo = po;
		this.coll = coll;
		this.inputListener = listener;
		this.currentFlag.Clear();
		this.HideAllHud();
		Byte b = 0;
		List<Byte> list = new List<Byte>();
		if (this.GetCursorFlagIndex(flags) != -1)
		{
			this.cursor.Show();
			flags = new BubbleUI.Flag[]
			{
				BubbleUI.Flag.CURSOR
			};
			list.Add(0);
		}
		else
		{
			BubbleUI.Flag[] array = flags;
			for (Int32 i = 0; i < (Int32)array.Length; i++)
			{
				BubbleUI.Flag flag = array[i];
				switch (flag)
				{
				case BubbleUI.Flag.EXCLAMATION:
					this.ShowHud(flag);
					this.suspectedButton.Show();
					this.suspectedIndex = b;
					list.Add(1);
					break;
				case BubbleUI.Flag.QUESTION:
					this.ShowHud(flag);
					this.helpButton.Show();
					this.helpIndex = b;
					list.Add(2);
					break;
				case BubbleUI.Flag.DUEL:
					this.ShowHud(flag);
					this.duelButton.Show();
					this.duelIndex = b;
					list.Add(3);
					break;
				case BubbleUI.Flag.BEACH:
					this.ShowHud(flag);
					this.beachButton.Show();
					this.beachIndex = b;
					list.Add(4);
					break;
				}
				b = (Byte)(b + 1);
				this.currentFlag.Add(flag);
			}
		}
		Boolean flag2 = false;
		BubbleUI.Flag[] array2 = this.allFlag;
		for (Int32 j = 0; j < (Int32)array2.Length; j++)
		{
			BubbleUI.Flag flag3 = array2[j];
			BubbleUI.Flag[] array3 = flags;
			for (Int32 k = 0; k < (Int32)array3.Length; k++)
			{
				BubbleUI.Flag flag4 = array3[k];
				if (flag4 == flag3)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				this.Hide(flag3);
			}
			flag2 = false;
		}
		this.grid.Reposition(list.ToArray());
	}

	public void ChangePrimaryKey(Control keyCode)
	{
		if (this.suspectedOnScreenButton != (UnityEngine.Object)null && this.primaryOnScreenButton != (UnityEngine.Object)null)
		{
			this.suspectedOnScreenButton.KeyCommand = keyCode;
			this.primaryOnScreenButton.KeyCommand = keyCode;
		}
	}

	public void ShowAllHud()
	{
		foreach (BubbleUI.Flag flag in this.currentFlag)
		{
			this.ShowHud(flag);
		}
	}

	public void ShowHud(BubbleUI.Flag flag)
	{
		if (FF9StateSystem.MobilePlatform)
		{
			switch (EventHUD.CurrentHUD)
			{
			case MinigameHUD.MogTutorial:
			case MinigameHUD.JumpingRope:
			case MinigameHUD.Telescope:
			case MinigameHUD.ChocoHot:
				return;
			}
			switch (flag)
			{
			case BubbleUI.Flag.EXCLAMATION:
				this.primaryButton.SetActive(true);
				this.primaryButtonSprite.spriteName = "button_minigame";
				this.primaryButtonComponent.normalSprite = "button_minigame";
				this.primaryButtonComponent.hoverSprite = "button_minigame_act";
				this.primaryButtonComponent.pressedSprite = "button_minigame_act";
				break;
			case BubbleUI.Flag.QUESTION:
				this.primaryButton.SetActive(true);
				this.primaryButtonSprite.spriteName = "button_bubble_question_idle";
				this.primaryButtonComponent.normalSprite = "button_bubble_question_idle";
				this.primaryButtonComponent.hoverSprite = "button_bubble_question_act";
				this.primaryButtonComponent.pressedSprite = "button_bubble_question_act";
				break;
			case BubbleUI.Flag.DUEL:
				this.secondaryButton.SetActive(true);
				this.secondaryButtonSprite.spriteName = "button_card_idle";
				this.secondaryButtonComponent.normalSprite = "button_card_idle";
				this.secondaryButtonComponent.hoverSprite = "button_card_act";
				this.secondaryButtonComponent.pressedSprite = "button_card_act";
				this.secondaryOnScreenButton.KeyCommand = Control.Special;
				break;
			case BubbleUI.Flag.BEACH:
				this.secondaryButton.SetActive(true);
				this.secondaryButtonSprite.spriteName = "button_beach_idle";
				this.secondaryButtonComponent.normalSprite = "button_beach_idle";
				this.secondaryButtonComponent.hoverSprite = "button_beach_act";
				this.secondaryButtonComponent.pressedSprite = "button_beach_act";
				this.secondaryOnScreenButton.KeyCommand = Control.Cancel;
				break;
			}
		}
	}

	public void Hide()
	{
		BubbleUI.Flag[] array = this.allFlag;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			BubbleUI.Flag target = array[i];
			this.Hide(target);
		}
		this.HideAllHud();
		this.currentFlag.Clear();
	}

	private void Hide(BubbleUI.Flag target)
	{
		switch (target)
		{
		case BubbleUI.Flag.EXCLAMATION:
			this.suspectedButton.Hide();
			break;
		case BubbleUI.Flag.QUESTION:
			this.helpButton.Hide();
			break;
		case BubbleUI.Flag.DUEL:
			this.duelButton.Hide();
			break;
		case BubbleUI.Flag.CURSOR:
			this.cursor.Hide();
			break;
		case BubbleUI.Flag.BEACH:
			this.beachButton.Hide();
			break;
		}
	}

	public void HideAllHud()
	{
		if (FF9StateSystem.MobilePlatform)
		{
			if (this.primaryButton != (UnityEngine.Object)null)
			{
				this.primaryButton.SetActive(false);
			}
			if (this.secondaryButton != (UnityEngine.Object)null)
			{
				this.secondaryButton.SetActive(false);
			}
		}
	}

	private Int32 GetCursorFlagIndex(BubbleUI.Flag[] flags)
	{
		Int32 result = -1;
		Byte b = 0;
		for (Int32 i = 0; i < (Int32)flags.Length; i++)
		{
			BubbleUI.Flag flag = flags[i];
			if (flag == BubbleUI.Flag.CURSOR)
			{
				result = (Int32)b;
				break;
			}
			b = (Byte)(b + 1);
		}
		return result;
	}

	private void OnDisable()
	{
		if (this.suspectedButton != (UnityEngine.Object)null)
		{
			this.suspectedButton.gameObject.SetActive(false);
		}
		if (this.helpButton != (UnityEngine.Object)null)
		{
			this.helpButton.gameObject.SetActive(false);
		}
		if (this.duelButton != (UnityEngine.Object)null)
		{
			this.duelButton.gameObject.SetActive(false);
		}
		if (this.cursor != (UnityEngine.Object)null)
		{
			this.cursor.gameObject.SetActive(false);
		}
		if (this.beachButton != (UnityEngine.Object)null)
		{
			this.beachButton.gameObject.SetActive(false);
		}
		this.currentFlag.Clear();
		this.ChangePrimaryKey(Control.Confirm);
		this.HideAllHud();
	}

	public void SetGameObjectActive(Boolean isActive)
	{
		base.gameObject.SetActive(isActive);
		if (isActive)
		{
			this.AutoOpen();
		}
	}

	private void AutoOpen()
	{
		UIManager.UIState hudstate = PersistenSingleton<UIManager>.Instance.HUDState;
		if (hudstate == UIManager.UIState.WorldHUD)
		{
			Dialog dialogByTextId = Singleton<DialogManager>.Instance.GetDialogByTextId(40);
			Dialog dialogByTextId2 = Singleton<DialogManager>.Instance.GetDialogByTextId(41);
			if (dialogByTextId != (UnityEngine.Object)null)
			{
				EIcon.ShowDialogBubble(false);
			}
			else if (dialogByTextId2 != (UnityEngine.Object)null)
			{
				EIcon.ShowDialogBubble(true);
			}
		}
	}

	private void Start()
	{
		this.primaryButtonSprite = this.primaryButton.GetComponent<UISprite>();
		this.primaryButtonComponent = this.primaryButton.GetComponent<UIButton>();
		this.primaryOnScreenButton = this.primaryButton.GetComponent<OnScreenButton>();
		this.secondaryButtonSprite = this.secondaryButton.GetComponent<UISprite>();
		this.secondaryButtonComponent = this.secondaryButton.GetComponent<UIButton>();
		this.secondaryOnScreenButton = this.secondaryButton.GetComponent<OnScreenButton>();
		this.suspectedOnScreenButton = this.suspectedButton.gameObject.AddComponent<OnScreenButton>();
		this.SetupParent();
	}

	private void SetupParent()
	{
		Vector3 localPosition = base.transform.parent.localPosition;
		localPosition.z = Singleton<DialogManager>.Instance.transform.localPosition.z;
		base.transform.parent.localPosition = localPosition;
	}

	private const String ExclamationIdleSpriteName = "button_minigame";

	private const String ExclamationActSpriteName = "button_minigame_act";

	private const String QuestionIdleSpriteName = "button_bubble_question_idle";

	private const String QuestionActSpriteName = "button_bubble_question_act";

	private const String BeachActSpriteName = "button_beach_act";

	private const String BeachIdleSpriteName = "button_beach_idle";

	private const String CardActSpriteName = "button_card_act";

	private const String CardIdleSpriteName = "button_card_idle";

	public const Single animateTime = 0.175f;

	private UIFieldMapFollowTarget follower;

	private UIFollowTarget worldFollower;

	private Byte suspectedIndex;

	private Byte helpIndex;

	private Byte duelIndex;

	private Byte beachIndex;

	private Action<PosObj, Obj, UInt32>[] inputListener;

	private BubbleUI.Flag[] allFlag = new BubbleUI.Flag[]
	{
		BubbleUI.Flag.EXCLAMATION,
		BubbleUI.Flag.QUESTION,
		BubbleUI.Flag.DUEL,
		BubbleUI.Flag.CURSOR,
		BubbleUI.Flag.BEACH
	};

	private List<BubbleUI.Flag> currentFlag = new List<BubbleUI.Flag>();

	private Boolean activeSelf;

	private PosObj currentPo;

	private Obj coll;

	private BubbleInputInfo currentInput;

	private Boolean playerControlEnable;

	private UISprite primaryButtonSprite;

	private UISprite secondaryButtonSprite;

	private UIButton primaryButtonComponent;

	private UIButton secondaryButtonComponent;

	private OnScreenButton primaryOnScreenButton;

	private OnScreenButton secondaryOnScreenButton;

	private OnScreenButton suspectedOnScreenButton;

	public GameObject primaryButton;

	public GameObject secondaryButton;

	public BubbleGrid grid;

	public BubbleButton suspectedButton;

	public BubbleButton helpButton;

	public BubbleButton duelButton;

	public BubbleButton cursor;

	public BubbleButton beachButton;

	public static readonly Vector3 UIDefaultOffset = new Vector3(0f, 50f, 0f);

	public enum Flag
	{
		EXCLAMATION,
		QUESTION,
		DUEL,
		CURSOR,
		BEACH
	}
}
