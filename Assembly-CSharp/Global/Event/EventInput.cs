using System;
using UnityEngine;

public class EventInput
{
	public static Boolean IsJapaneseLayout
	{
		get
		{
			return EventInput.isJapaneseLayout;
		}
	}

	public static Boolean IsMovementControl
	{
		get
		{
			return EventInput.isMovementControl;
		}
	}

	public static Boolean IsProcessingInput
	{
		get
		{
			return EventInput.isProcessingInput;
		}
		set
		{
			EventInput.isProcessingInput = value;
		}
	}

	public static Boolean IsDialogConfirm
	{
		set
		{
			EventInput.isDialogConfirm = value;
		}
	}

	public static Boolean IsNeedAddStartSignal
	{
		set
		{
			EventInput.addStartSignal = (Int32)((!value) ? 0 : 1);
		}
	}

	public static Boolean IsPressedDig
	{
		get
		{
			return EventInput.isPressedDig;
		}
		set
		{
			EventInput.isPressedDig = value;
		}
	}

	public static Boolean IsMenuON
	{
		get
		{
			return (EventInput.PSXCntlPadMask[0] & 262144u) <= 0u;
		}
	}

	public static Boolean IsKeyboardOrJoystickInput
	{
		get
		{
			SourceControl currentInputSource = EventInput.GetCurrentInputSource();
			Boolean result = PersistenSingleton<HonoInputManager>.Instance.IsControllerConnect;
			switch (currentInputSource)
			{
			case SourceControl.KeyBoard:
			case SourceControl.Joystick:
				result = true;
				break;
			case SourceControl.Touch:
				result = false;
				break;
			}
			return result;
		}
	}

	public static void ChangeInputLayout(String language)
	{
		if (language == "Japanese")
		{
			EventInput.SetJapaneseLayput();
		}
		else
		{
			EventInput.SetOtherLangLayout();
		}
		EventInput.ConfirmMask = (EventInput.Lcircle | 8192u);
        CancelMask = (EventInput.Lx | 16384u);
	}

	private static void SetJapaneseLayput()
	{
		EventInput.Lcircle = 65536u;
		EventInput.Lx = 131072u;
		EventInput.Fcircle = 65536u;
		EventInput.Fx = 131072u;
		EventInput.isJapaneseLayout = true;
		EventInput.HippualMask = (EventInput.Lcircle | 524288u);
	}

	private static void SetOtherLangLayout()
	{
		EventInput.Lcircle = 131072u;
		EventInput.Lx = 65536u;
		EventInput.Fcircle = 65536u;
		EventInput.Fx = 131072u;
		EventInput.isJapaneseLayout = false;
		EventInput.HippualMask = (EventInput.Lx | 524288u);
	}

	public static UInt32 ReadInput()
	{
		UInt32 num = 0u;
		if (!EventInput.isProcessingInput)
		{
			EventInput.ResetWorldTriggerButton();
			return num;
		}
		Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
		Int32 gMode = PersistenSingleton<EventEngine>.Instance.gMode;
		Boolean isKeyboardOrJoystickInput = EventInput.IsKeyboardOrJoystickInput;
		if (Singleton<BubbleUI>.Instance.IsActive)
		{
			if (gMode == 1)
			{
				if (fldMapNo == 1420 && EventInput.CheckLadderFlag())
				{
					num = EventInput.ProcessInput(false, false);
				}
				else
				{
					num = EventInput.ProcessInput(false, true);
				}
			}
			else if (gMode == 3)
			{
				if (FF9StateSystem.MobilePlatform)
				{
					num = EventInput.ProcessInput(false, isKeyboardOrJoystickInput);
					EventInput.GetWorldTriggerButton(ref num);
				}
				else
				{
					num = EventInput.ProcessInput(false, true);
				}
			}
		}
		else if (EventHUD.CurrentHUD != MinigameHUD.None)
		{
			MinigameHUD currentHUD = EventHUD.CurrentHUD;
			if (currentHUD != MinigameHUD.Chanbara)
			{
				if (currentHUD != MinigameHUD.RacingHippaul)
				{
					num = EventInput.ProcessInput(false, true);
				}
				else
				{
					num = EventInput.ProcessInput(false, false);
				}
			}
			else
			{
				num = EventInput.ProcessInput(true, true);
				num &= EventInput.ChanbaraMask;
				if (FF9StateSystem.MobilePlatform)
				{
					if ((num & 8u) > 0u)
					{
						EventInput.IsNeedAddStartSignal = true;
					}
					else if (EventInput.addStartSignal > 0)
					{
						num |= 8u;
						EventInput.addStartSignal--;
						EventInput.InputLog("Extra Start");
					}
				}
			}
		}
		else if (fldMapNo == 606)
		{
			if (EventHUD.CurrentHUD == MinigameHUD.Telescope)
			{
				num = EventInput.ProcessInput(false, true);
			}
		}
		else if (fldMapNo == 2204 && TimerUI.Enable)
		{
			num = EventInput.ProcessInput(false, false);
		}
		else if (fldMapNo == 1607)
		{
			num = EventInput.ProcessInput(false, false);
		}
		else if (fldMapNo == 1420)
		{
			num = EventInput.ProcessInput(false, true);
		}
		else if (fldMapNo == 1422)
		{
			num = EventInput.ProcessInput(false, true);
		}
		else
		{
			Dialog mognetDialog = Singleton<DialogManager>.Instance.GetMognetDialog();
			if (mognetDialog != (UnityEngine.Object)null)
			{
				if (mognetDialog.IsChoiceReady)
				{
					num = EventInput.ProcessInput(false, true);
				}
			}
			else if (FF9StateSystem.MobilePlatform)
			{
				num = EventInput.ProcessInput(false, isKeyboardOrJoystickInput);
				if (isKeyboardOrJoystickInput)
				{
					UIManager.Input.ResetTriggerEvent();
				}
				EventInput.GetWorldTriggerButton(ref num);
			}
			else
			{
				num = EventInput.ProcessInput(false, true);
			}
		}
		num |= EventInput.eventButtonInput;
		if (EventInput.isDialogConfirm)
		{
			num |= EventInput.ConfirmMask;
			EventInput.isDialogConfirm = false;
		}
		if (HonoBehaviorSystem.Instance.IsFastForwardModeActive())
		{
			if (FF9StateSystem.MobilePlatform)
			{
				if (fldMapNo == 909 || fldMapNo == 1909)
				{
					num = EventInput.FastForwardProcess(gMode, fldMapNo, num);
				}
			}
			else
			{
				num = EventInput.FastForwardProcess(gMode, fldMapNo, num);
			}
		}
		num &= ~EventInput.PSXCntlPadMask[0];
		if (FF9StateSystem.MobilePlatform && gMode == 3 && EventCollision.IsRidingChocobo())
		{
			if ((num & 524288u) > 0u || (num & 32768u) > 0u)
			{
				EventInput.isPressedDig = true;
			}
			else if ((num & 16777216u) > 0u || (num & 4096u) > 0u)
			{
				EventInput.isPressedDig = false;
			}
			else if ((num & EventInput.Lx) > 0u || (num & 16384u) > 0u)
			{
				EventInput.isPressedDig = false;
			}
			else if ((num & 1u) > 0u || (num & 1u) > 0u)
			{
				EventInput.isPressedDig = false;
			}
		}
		if (gMode == 3 && EventEngineUtils.IsMogCalled(PersistenSingleton<EventEngine>.Instance))
		{
			ff9.w_isMogActive = true;
		}
		if (gMode == 3 && EMinigame.CheckBeachMinigame() && EventCollision.IsWorldTrigger() && (num & CancelMask) > 0u)
		{
			num &= ~CancelMask;
			EventInput.InputLog("Remove cancel mask for <SQEX> #2893");
		}
		EventInput.eventButtonInput = 0u;
		EventInput.ResetWorldTriggerButton();
		return num;
	}

	private static UInt32 ProcessBubbleInput(BubbleInputInfo bubbleInput)
	{
		UInt32 result = 0u;
		if (bubbleInput != null)
		{
			result = bubbleInput.buttonCode;
			Singleton<BubbleUI>.Instance.ResetInput();
		}
		return result;
	}

	private static UInt32 ProcessInput(Boolean isTriggerDirection, Boolean isTriggerButton)
	{
		UInt32 num = 0u;
		Vector2 vector = Vector2.zero;
		if (VirtualAnalog.HasInput())
		{
			vector = VirtualAnalog.GetAnalogValue();
			if (Mathf.Abs(vector.x) >= 0.1f || Mathf.Abs(vector.y) >= 0.1f)
			{
				if (vector.y > 0f)
				{
					num |= 16u;
				}
				if (vector.y < 0f)
				{
					num |= 64u;
				}
				if (vector.x < 0f)
				{
					num |= 128u;
				}
				if (vector.x > 0f)
				{
					num |= 32u;
				}
			}
		}
		if (EventInput.GetKey(Control.Menu, isTriggerButton))
		{
			num |= 16781312u;
			EventInput.InputLog("Press /_\\");
		}
		if (EventInput.GetKey(Control.Confirm, isTriggerButton))
		{
			num |= (8192u | EventInput.Lcircle);
			EventInput.InputLog("Press 0");
		}
		if (EventInput.GetKey(Control.Cancel, isTriggerButton))
		{
			num |= (16384u | EventInput.Lx);
			EventInput.InputLog("Press X");
		}
		if (EventInput.GetKey(Control.Special, isTriggerButton))
		{
			num |= 557056u;
			EventInput.InputLog("Press []");
		}
		if (EventInput.GetKey(Control.Select, isTriggerButton))
		{
			num |= 1u;
			EventInput.InputLog("Press Select");
		}
		if (EventInput.GetKey(Control.Pause, isTriggerButton))
		{
			num |= 8u;
			EventInput.InputLog("Press Start");
		}
		if (EventInput.GetKey(Control.LeftBumper, isTriggerButton))
		{
			num |= 1049600u;
			EventInput.InputLog("Press L1");
		}
		if (EventInput.GetKey(Control.RightBumper, isTriggerButton))
		{
			num |= 2099200u;
			EventInput.InputLog("Press R1");
		}
		if (EventInput.GetKey(Control.LeftTrigger, isTriggerButton))
		{
			num |= 4194560u;
			EventInput.InputLog("Press L2");
		}
		if (EventInput.GetKey(Control.RightTrigger, isTriggerButton))
		{
			num |= 8389120u;
			EventInput.InputLog("Press R2");
		}
		if (EventInput.GetKey(Control.Up, isTriggerDirection))
		{
			num |= 16u;
			EventInput.InputLog("Press ^");
		}
		if (EventInput.GetKey(Control.Down, isTriggerDirection))
		{
			num |= 64u;
			EventInput.InputLog("Press v");
		}
		if (EventInput.GetKey(Control.Left, isTriggerDirection))
		{
			num |= 128u;
			EventInput.InputLog("Press <");
		}
		if (EventInput.GetKey(Control.Right, isTriggerDirection))
		{
			num |= 32u;
			EventInput.InputLog("Press >");
		}
		return num;
	}

	public static Boolean GetKey(Control keyCode, Boolean isTriggerMode)
	{
		if (isTriggerMode)
		{
			return UIManager.Input.GetKeyTrigger(keyCode);
		}
		return UIManager.Input.GetKey(keyCode);
	}

	private static void InputLog(String log)
	{
		if (EventInput.showLog)
		{
			global::Debug.Log(log + " at " + RealTime.time);
		}
	}

	private static UInt32 FastForwardProcess(Int32 eventMode, Int32 fldMapNo, UInt32 input)
	{
		if (EventInput.lastTimeInput == RealTime.time)
		{
			if (fldMapNo != 1420 || eventMode != 1)
			{
				input &= ~EventInput.OperationMask;
			}
		}
		else if (input != 0u)
		{
			EventInput.lastTimeInput = RealTime.time;
		}
		if (EventHUD.CurrentHUD == MinigameHUD.RacingHippaul && (input & EventInput.HippualMask) == EventInput.HippualMask)
		{
			input = 0u;
		}
		return input;
	}

	public static void PSXCntlSetPadMask(Int32 _pad_no, UInt32 _lbtn_flags)
	{
		EventInput.PSXCntlPadMask[_pad_no] |= _lbtn_flags;
		EventInput.CheckPlayerControl();
	}

	public static void PSXCntlClearPadMask(Int32 _pad_no, UInt32 _lbtn_flags)
	{
		EventInput.PSXCntlPadMask[_pad_no] &= ~_lbtn_flags;
		EventInput.CheckPlayerControl();
	}

	public static void ClearPadMask()
	{
		EventInput.PSXCntlPadMask[0] = 0u;
		EventInput.PSXCntlPadMask[1] = 0u;
		EventInput.isMovementControl = true;
	}

	private static void CheckPlayerControl()
	{
		if ((EventInput.PSXCntlPadMask[0] & EventInput.MovementMask) > 0u)
		{
			EventInput.isMovementControl = false;
		}
		else
		{
			EventInput.isMovementControl = true;
		}
	}

	public static void RecieveDialogConfirm()
	{
		if (FF9StateSystem.PCPlatform && !Singleton<DialogManager>.Instance.IsDialogNeedControl() && !Singleton<BubbleUI>.Instance.IsActive && (PersistenSingleton<EventEngine>.Instance.gMode == 1 || PersistenSingleton<EventEngine>.Instance.gMode == 3) && EventHUD.CurrentHUD == MinigameHUD.None)
		{
			EventInput.isDialogConfirm = true;
			EventInput.InputLog("Press O from mouse");
		}
	}

	public static void ReceiveInput(UInt32 buttonBit)
	{
		EventInput.eventButtonInput |= buttonBit;
	}

	public static Boolean CheckLadderFlag()
	{
		Boolean result = false;
		PosObj controlChar = PersistenSingleton<EventEngine>.Instance.GetControlChar();
		if (controlChar != null)
		{
			GameObject go = controlChar.go;
			if (go != (UnityEngine.Object)null)
			{
				result = (go.GetComponent<FieldMapActorController>().GetLadderFlag() != 0);
			}
		}
		return result;
	}

	private static SourceControl GetCurrentInputSource()
	{
		SourceControl sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetDirectionAxisSource();
		if (sourceControl != SourceControl.None)
		{
			return sourceControl;
		}
		Int32 num = 14;
		for (Int32 i = 0; i < num; i++)
		{
			sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetSource((Control)i);
			if (sourceControl != SourceControl.None)
			{
				break;
			}
		}
		return sourceControl;
	}

	private static void GetWorldTriggerButton(ref UInt32 currentInput)
	{
		if (FF9StateSystem.AndroidPlatform && PersistenSingleton<EventEngine>.Instance.gMode == 3 && EventInput.IsKeyboardOrJoystickInput)
		{
			currentInput &= ~EventInput.OperationMask;
			UInt32 num = 0u;
			if (PersistenSingleton<AndroidEventInputManager>.Instance.GetKeyTrigger(Control.Menu))
			{
				num |= 16781312u;
				EventInput.InputLog("Press /_\\");
			}
			if (PersistenSingleton<AndroidEventInputManager>.Instance.GetKeyTrigger(Control.Confirm))
			{
				num |= (8192u | EventInput.Lcircle);
				EventInput.InputLog("Press 0");
			}
			if (PersistenSingleton<AndroidEventInputManager>.Instance.GetKeyTrigger(Control.Cancel))
			{
				num |= (16384u | EventInput.Lx);
				EventInput.InputLog("Press X");
			}
			if (PersistenSingleton<AndroidEventInputManager>.Instance.GetKeyTrigger(Control.Special))
			{
				num |= 557056u;
				EventInput.InputLog("Press []");
			}
			currentInput |= num;
		}
	}

	private static void ResetWorldTriggerButton()
	{
		if (FF9StateSystem.AndroidPlatform && PersistenSingleton<EventEngine>.Instance.gMode == 3)
		{
			PersistenSingleton<AndroidEventInputManager>.Instance.Reset();
		}
	}

	public const UInt32 Pselect = 1u;

	public const UInt32 Lselect = 1u;

	public const UInt32 Pstart = 8u;

	public const UInt32 Lstart = 8u;

	public const UInt32 Pup = 16u;

	public const UInt32 Lup = 16u;

	public const UInt32 Pright = 32u;

	public const UInt32 Lright = 32u;

	public const UInt32 Pdown = 64u;

	public const UInt32 Ldown = 64u;

	public const UInt32 Pleft = 128u;

	public const UInt32 Lleft = 128u;

	public const UInt32 PL2 = 256u;

	public const UInt32 PR2 = 512u;

	public const UInt32 PL1 = 1024u;

	public const UInt32 PR1 = 2048u;

	public const UInt32 Ptriangle = 4096u;

	public const UInt32 Pcircle = 8192u;

	public const UInt32 Px = 16384u;

	public const UInt32 Psquare = 32768u;

	public const UInt32 Lmenu = 262144u;

	public const UInt32 Lsquare = 524288u;

	public const UInt32 LL1 = 1048576u;

	public const UInt32 LR1 = 2097152u;

	public const UInt32 LL2 = 4194304u;

	public const UInt32 LR2 = 8388608u;

	public const UInt32 Ltriangle = 16777216u;

	public const UInt32 Lnavi = 33554432u;

	public const UInt32 Lmog = 524288u;

	public const Byte PSXCNTL_MAX_PADS = 2;

	public static UInt32 Lcircle = 131072u;

	public static UInt32 Lx = 65536u;

	public static UInt32 Fcircle = 65536u;

	public static UInt32 Fx = 131072u;

	private static Boolean showLog = false;

	private static Boolean isJapaneseLayout = false;

	private static UInt32[] PSXCntlPadMask = new UInt32[2];

	private static readonly UInt32 MovementMask = 240u;

	private static readonly UInt32 ChanbaraMask = 4294967294u;

	private static readonly UInt32 OperationMask = EventInput.Lx | 16384u | EventInput.Lcircle | 8192u | 32768u | 524288u | 16777216u | 4096u;

	private static UInt32 ConfirmMask = 0u;

	private static UInt32 CancelMask = 0u;

	private static Boolean isMovementControl = true;

	private static Boolean isProcessingInput = true;

	private static Boolean isDialogConfirm = false;

	private static Int32 addStartSignal = 0;

	private static Single lastTimeInput = 0f;

	private static Boolean isPressedDig = false;

	private static UInt32 eventButtonInput = 0u;

	private static UInt32 HippualMask = 0u;
}
