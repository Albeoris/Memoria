using System;
using UnityEngine;
using Memoria;

internal class EIcon
{
	public static FieldMap FieldMap
	{
		get
		{
			if (EIcon.currentFieldMap == (UnityEngine.Object)null)
			{
				EIcon.currentFieldMap = PersistenSingleton<EventEngine>.Instance.fieldmap;
			}
			return EIcon.currentFieldMap;
		}
	}

	public static Camera WorldCamera
	{
		get
		{
			if (EIcon.worldCamera == (UnityEngine.Object)null)
			{
				GameObject gameObject = GameObject.Find("WorldCamera");
				EIcon.worldCamera = gameObject.GetComponent<Camera>();
			}
			return EIcon.worldCamera;
		}
	}

	public static Single ShowDelay
	{
		get
		{
			return EIcon.showDelay;
		}
		set
		{
			EIcon.showDelay = value;
		}
	}

	public static Single HideDelay
	{
		get
		{
			return EIcon.hideDelay;
		}
		set
		{
			EIcon.hideDelay = value;
		}
	}

	public static Boolean IsProcessingFIcon
	{
		get
		{
			return EIcon.processFIcon;
		}
		set
		{
			EIcon.processFIcon = value;
		}
	}

	public static Boolean IsDialogBubble
	{
		get
		{
			return EIcon.dialogBubble;
		}
	}

	public static BubbleUI.IconType SFIconType
	{
		get
		{
			return EIcon.sFIconType;
		}
	}

	public static void InitFIcon()
	{
		EIcon.sFIconPolled = (EIcon.sFIconLastPolled = false);
		EIcon.dialogBubble = false;
		EIcon.lastPollType = EIcon.PollType.NONE;
		EIcon.hereIconShow = false;
		EIcon.processFIcon = true;
		EIcon.ShowDelay = 0f;
		EIcon.HideDelay = 0f;
	}

	public static void PollFIcon(BubbleUI.IconType type)
	{
		EIcon.lastPollType = EIcon.PollType.EVENT_SCRIPT;
		EIcon.sFIconPolled = true;
		EIcon.sFIconType = type;
		EIcon.CloseHereIcon();
	}

	public static Boolean PollCollisionIcon(Obj targetObject)
	{
		if (Configuration.Icons.HideSteam)
			return false;
		if (EventHUD.CurrentHUD == MinigameHUD.MogTutorial)
			return false;
		Boolean result = false;
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		if (instance.gMode == 1)
		{
			Boolean flag = instance.GetIP((Int32)targetObject.sid, 3, targetObject.ebData) != instance.nil && 1 < targetObject.level;
			Boolean flag2 = instance.GetIP((Int32)targetObject.sid, 8, targetObject.ebData) != instance.nil && 1 < targetObject.level;
			if (flag && flag2)
			{
				EIcon.PollFIcon(BubbleUI.IconType.ExclamationAndDuel);
				result = true;
			}
			else if (flag && instance.IsActuallyTalkable(targetObject))
			{
				EIcon.PollFIcon(BubbleUI.IconType.Exclamation);
				result = true;
			}
		}
		else
		{
			Boolean flag = instance.GetIP((Int32)targetObject.sid, 2, targetObject.ebData) != instance.nil && 1 < targetObject.level;
			if (flag)
			{
				if (EMinigame.CheckBeachMinigame() && !EventCollision.IsWorldTrigger())
					EIcon.PollFIcon(BubbleUI.IconType.ExclamationAndBeach);
				else
					EIcon.PollFIcon(BubbleUI.IconType.Exclamation);
				result = true;
			}
		}
		return result;
	}

	public static void ProcessFIcon()
	{
		if (!EIcon.processFIcon)
		{
			return;
		}
		if (EIcon.dialogBubble)
		{
			return;
		}
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		BubbleUI instance2 = Singleton<BubbleUI>.Instance;
		if (instance2 == (UnityEngine.Object)null)
		{
			return;
		}
		if (!instance2.gameObject.activeSelf)
		{
			instance2.gameObject.SetActive(true);
		}
		Int32 num = 0;
		if (!EIcon.sFIconLastPolled && EIcon.sFIconPolled)
		{
			num = 1;
		}
		else if (EIcon.sFIconLastPolled && !EIcon.sFIconPolled)
		{
			num = 2;
		}
		else if (EIcon.sFIconLastPolled && EIcon.sFIconPolled && !instance2.IsActive)
		{
			if (EIcon.HideDelay > 0f)
			{
				EIcon.HideDelay -= Time.deltaTime;
			}
			else
			{
				num = 1;
			}
		}
		else if (!EIcon.sFIconLastPolled && !EIcon.sFIconPolled && instance2.IsActive && !EIcon.hereIconShow)
		{
			if (EIcon.ShowDelay > 0f)
			{
				EIcon.ShowDelay -= Time.deltaTime;
			}
			else
			{
				num = 2;
			}
		}
		EIcon.sFIconLastPolled = EIcon.sFIconPolled;
		EIcon.sFIconPolled = false;
		if (num == 1)
		{
			EIcon.ShowBubble();
		}
		else if (num == 2)
		{
			EIcon.HideBubble();
		}
		else if (!EIcon.hereIconShow && instance2.IsActive)
        {
            Boolean flag = sFIconType != sFIconLastType;
            if (flag)
            {
                EIcon.HideBubble();
                EIcon.sFIconPolled = false;
                EIcon.sFIconLastPolled = EIcon.sFIconPolled;
            }
        }
        EIcon.sFIconLastType = EIcon.sFIconType;
	}

	private static void ShowBubble()
	{
		if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
		{
			EIcon.ShowFieldBubble();
		}
		else
		{
			EIcon.ShowWorldBubble();
		}
	}

	public static void HideBubble()
	{
		if (Singleton<BubbleUI>.Instance != (UnityEngine.Object)null && Singleton<BubbleUI>.Instance.IsActive)
		{
			EIcon.HideDelay = Singleton<BubbleUI>.Instance.AnimationDuration;
			EIcon.lastPollType = EIcon.PollType.NONE;
			Singleton<BubbleUI>.Instance.Hide();
		}
	}

	private static void ShowFieldBubble()
	{
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		EIcon.hereIconShow = false;
		Obj obj = instance.FindObjByUID((Int32)instance.GetControlUID());
		if (obj.go == (UnityEngine.Object)null)
		{
			return;
		}
		EIcon.ShowDelay = Singleton<BubbleUI>.Instance.AnimationDuration;
		BubbleUI.Flag[] bubbleFlagData = EIcon.GetBubbleFlagData(EIcon.sFIconType);
		if (obj.cid == 4 && obj.go.activeSelf)
		{
			Transform target;
			Vector3 offset;
			BubbleMappingInfo.GetActorInfo((PosObj)obj, out target, out offset);
			Singleton<BubbleUI>.Instance.Show(target, (PosObj)obj, (Obj)null, EIcon.FieldMap, offset, bubbleFlagData, null);
		}
		else
		{
			Singleton<BubbleUI>.Instance.Show((Transform)null, (PosObj)obj, (Obj)null, EIcon.FieldMap, Vector3.zero, bubbleFlagData, null);
		}
	}

	public static void ShowWorldBubble()
	{
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		BubbleUI.Flag[] bubbleFlagData = EIcon.GetBubbleFlagData(EIcon.sFIconType);
		Action<PosObj, Obj, UInt32>[] listener = new Action<PosObj, Obj, UInt32>[]
		{
			new Action<PosObj, Obj, UInt32>(EventCollision.BubbleUIListener)
		};
		PosObj controlChar = instance.GetControlChar();
		Vector3 uidefaultOffset = BubbleUI.UIDefaultOffset;
		if (controlChar.go == (UnityEngine.Object)null)
		{
			return;
		}
		if (EventCollision.IsChocoboFlyingOverForest() || (EIcon.dialogBubble && EIcon.dialogAlternativeKey))
		{
			Singleton<BubbleUI>.Instance.ChangePrimaryKey(Control.Cancel);
			EIcon.dialogAlternativeKey = false;
		}
		else
		{
			Singleton<BubbleUI>.Instance.ChangePrimaryKey(Control.Confirm);
		}
		EIcon.ShowDelay = Singleton<BubbleUI>.Instance.AnimationDuration;
		Vector3 transformOffset;
		EIcon.GetWorldActorOffset(out transformOffset, ref uidefaultOffset);
		Singleton<BubbleUI>.Instance.Show(controlChar.go.transform, controlChar, (Obj)null, EIcon.WorldCamera, transformOffset, uidefaultOffset, bubbleFlagData, listener);
	}

	public static void ShowDialogBubble(Boolean useAlternativeKey = false)
	{
		EIcon.dialogBubble = true;
		EIcon.dialogAlternativeKey = useAlternativeKey;
		EIcon.sFIconType = BubbleUI.IconType.Exclamation;
		EIcon.ShowWorldBubble();
	}

	public static void HideDialogBubble()
	{
		EIcon.dialogBubble = false;
		EIcon.HideBubble();
	}

	public static Vector3 GetWorldActorOffset(out Vector3 actorOffset, ref Vector3 uiOffset)
	{
		switch (WMUIData.ControlNo)
		{
		case 5:
			actorOffset = EIcon.worldActorOffset * 1.4f;
			goto IL_C3;
		case 6:
			actorOffset = EIcon.worldActorOffset * 1.9f;
			goto IL_C3;
		case 8:
			actorOffset = EIcon.worldActorOffset * 2.2f;
			uiOffset.x -= 70f;
			goto IL_C3;
		case 9:
			actorOffset = EIcon.worldActorOffset * 1.7f;
			uiOffset.x -= 70f;
			goto IL_C3;
		}
		actorOffset = EIcon.worldActorOffset;
		IL_C3:
		return actorOffset;
	}

    public static BubbleUI.Flag[] GetBubbleFlagData(BubbleUI.IconType pollCode)
    {
        return pollCode switch
        {
            BubbleUI.IconType.Question => [BubbleUI.Flag.QUESTION],
            BubbleUI.IconType.Exclamation => [BubbleUI.Flag.EXCLAMATION],
            BubbleUI.IconType.ExclamationAndDuel => [BubbleUI.Flag.EXCLAMATION, BubbleUI.Flag.DUEL],
            BubbleUI.IconType.Beach => [BubbleUI.Flag.BEACH],
            BubbleUI.IconType.ExclamationAndBeach => [BubbleUI.Flag.EXCLAMATION, BubbleUI.Flag.BEACH],
            _ => [BubbleUI.Flag.EXCLAMATION]
        };
    }

    public static void SetHereIcon(Int32 f)
	{
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		if (f <= 0 || EventHUD.CurrentHUD == MinigameHUD.ChocoHot)
		{
			EIcon.sHereIconTimer = 0;
			EIcon.sHereIconForce = false;
		}
		else
		{
			UInt64 num = (UInt64)((FF9StateSystem.Settings.cfg.here_icon > 0UL) ? 0UL : 1UL);
			if (f > 2 || (instance.GetUserControl() && (num > 1UL || (num == 1UL && (f == 2 || instance.gAnimCount > 0 || instance.eTb.gMesCount >= 3)))))
			{
				EIcon.sHereIconTimer = 60;
			}
		}
	}

	public static void ProcessHereIcon(PosObj po)
	{
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		if (instance.GetUserControl() && (ETb.KeyOn() & 1u) > 0u)
		{
			EIcon.sHereIconTimer = 60;
			EIcon.sHereIconForce = true;
			EIcon.hereIconShow = false;
		}
		if (EIcon.sHereIconTimer > 0)
		{
			EIcon.sHereIconTimer--;
			if (EIcon.sHereIconTimer <= 0)
			{
				EIcon.sHereIconForce = false;
			}
			if (instance.gMode == 1 && !EIcon.hereIconShow && EIcon.lastPollType == EIcon.PollType.NONE && EIcon.sHereIconTimer > 0 && EIcon.sHereIconTimer < 58)
			{
				EIcon.ShowHereIcon(po);
			}
		}
		else
		{
			EIcon.CloseHereIcon();
		}
	}

	private static void CloseHereIcon()
	{
		if (EIcon.hereIconShow && EIcon.sHereIconTimer <= 0)
		{
			EIcon.HideBubble();
			EIcon.hereIconShow = false;
		}
	}

	private static void ShowHereIcon(PosObj po)
	{
		EIcon.hereIconShow = true;
		EIcon.ShowDelay = 0.175f;
		BubbleUI.Flag flag = BubbleUI.Flag.CURSOR;
		Transform target;
		Vector3 offset;
		BubbleMappingInfo.GetActorInfo(po, out target, out offset);
		Singleton<BubbleUI>.Instance.Show(target, po, (Obj)null, EIcon.FieldMap, offset, new BubbleUI.Flag[]
		{
			flag
		}, null);
	}

    public static int AIconMode
    {
        get { return EIcon.sAIconMode; }
    }

    public static ATEType CurrentATE
	{
		get
		{
			return EIcon.currentATE;
		}
	}

	public static void ProcessAIcon()
	{
		if (EIcon.sAIconMode > 0 && ((EIcon.sAIconMode & 4) > 0 || PersistenSingleton<EventEngine>.Instance.GetUserControl()))
		{
			EIcon.sAIconTimer++;
			if ((EIcon.sAIconMode & 3) != 2)
			{
				EIcon.currentATE = ATEType.Blue;
				EIcon.ShowAIcon(true, EIcon.currentATE);
			}
			else if ((EIcon.sAIconTimer / 15 & 1) > 0)
			{
				EIcon.currentATE = ATEType.Gray;
				EIcon.ShowAIcon(true, EIcon.currentATE);
			}
		}
		else
		{
			EIcon.ShowAIcon(false, EIcon.currentATE);
		}
	}

	public static void ShowAIcon(Boolean isActive, ATEType type)
	{
		UIManager.Field.EnableATE(isActive, type);
	}

	public static void SetAIcon(Int32 mode)
	{
		if (mode == 0)
		{
			EIcon.ShowAIcon(false, EIcon.CurrentATE);
		}
		if (EIcon.sAIconMode != mode)
		{
			EIcon.sAIconMode = mode;
			EIcon.sAIconTimer = 44;
		}
	}

	public const Int32 kHereIconTime = 60;

	public const Int32 questionIcon = 0;

	public const Int32 exclamationIcon = 1;

	public const Int32 cardIcon = 2;

	public const Int32 beachIcon = 3;

	public const Int32 exclamationAndBeachIcon = 4;

	private const Int32 kAIconInterval = 44;

	private const Int32 kAIconInterval2 = 15;

	private const Int32 kAIconForce = 4;

	private const Int32 kAIconModeMask = 3;

	private static Boolean sFIconPolled;

	private static Boolean sFIconLastPolled;

	private static BubbleUI.IconType sFIconType;

	private static BubbleUI.IconType sFIconLastType;

	private static EIcon.PollType lastPollType;

	private static Boolean dialogBubble = false;

	private static Boolean dialogAlternativeKey = false;

	private static Boolean processFIcon = true;

	private static Boolean recheckScript = false;

	private static Boolean hereIconShow = false;

	private static Int32 sHereIconTimer;

	private static Boolean sHereIconForce;

	private static Single showDelay = 0f;

	private static Single hideDelay = 0f;

	public static readonly Vector3 worldActorOffset = new Vector3(0f, 1.8f, 0f);

	private static Camera worldCamera;

	private static FieldMap currentFieldMap;

	private static Int32 sAIconMode;

	private static Int32 sAIconTimer;

	private static ATEType currentATE = ATEType.Blue;

	public enum PollType
	{
		NONE,
		EVENT_SCRIPT
	}
}
