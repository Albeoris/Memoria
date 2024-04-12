using System;
using UnityEngine;

public class EventInput
{
	public static Boolean IsJapaneseLayout
	{
		get => EventInput.isJapaneseLayout;
	}

	public static Boolean IsMovementControl
	{
		get => EventInput.isMovementControl;
	}

	public static Boolean IsProcessingInput
	{
		get => EventInput.isProcessingInput;
		set => EventInput.isProcessingInput = value;
	}

	public static Boolean IsDialogConfirm
	{
		set => EventInput.isDialogConfirm = value;
	}

	public static Boolean IsNeedAddStartSignal
	{
		set => EventInput.addStartSignal = value ? 1 : 0;
	}

	public static Boolean IsPressedDig
	{
		get => EventInput.isPressedDig;
		set => EventInput.isPressedDig = value;
	}

	public static Boolean IsMenuON
	{
		get => (EventInput.PSXCntlPadMask[0] & EventInput.Lmenu) == 0u;
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
			EventInput.SetJapaneseLayput();
		else
			EventInput.SetOtherLangLayout();
		EventInput.ConfirmMask = EventInput.Lcircle | EventInput.Pcircle;
		EventInput.CancelMask = EventInput.Lx | EventInput.Px;
	}

	private static void SetJapaneseLayput()
	{
		EventInput.Lcircle = 0x10000u;
		EventInput.Lx = 0x20000u;
		EventInput.Fcircle = 0x10000u;
		EventInput.Fx = 0x20000u;
		EventInput.isJapaneseLayout = true;
		EventInput.HippualMask = EventInput.Lcircle | EventInput.Lsquare;
	}

	private static void SetOtherLangLayout()
	{
		EventInput.Lcircle = 0x20000u;
		EventInput.Lx = 0x10000u;
		EventInput.Fcircle = 0x10000u;
		EventInput.Fx = 0x20000u;
		EventInput.isJapaneseLayout = false;
		EventInput.HippualMask = EventInput.Lx | EventInput.Lsquare;
	}

	public static UInt32 ReadInput()
	{
		UInt32 inputs = 0u;
		if (!EventInput.isProcessingInput)
		{
			EventInput.ResetWorldTriggerButton();
			return inputs;
		}
		Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
		Int32 gMode = PersistenSingleton<EventEngine>.Instance.gMode;
		Boolean isKeyboardOrJoystickInput = EventInput.IsKeyboardOrJoystickInput;
		if (Singleton<BubbleUI>.Instance.IsActive)
		{
			if (gMode == 1)
			{
				if (fldMapNo == 1420 && EventInput.CheckLadderFlag()) // Fossil Roo/Cavern, climbing ivy
					inputs = EventInput.ProcessInput(false, false);
				else
					inputs = EventInput.ProcessInput(false, true);
			}
			else if (gMode == 3)
			{
				if (FF9StateSystem.MobilePlatform)
				{
					inputs = EventInput.ProcessInput(false, isKeyboardOrJoystickInput);
					EventInput.GetWorldTriggerButton(ref inputs);
				}
				else
				{
					inputs = EventInput.ProcessInput(false, true);
				}
			}
		}
		else if (EventHUD.CurrentHUD != MinigameHUD.None)
		{
			MinigameHUD currentHUD = EventHUD.CurrentHUD;
			if (currentHUD != MinigameHUD.Chanbara)
			{
				if (currentHUD != MinigameHUD.RacingHippaul)
					inputs = EventInput.ProcessInput(false, true);
				else
					inputs = EventInput.ProcessInput(false, false);
			}
			else
			{
				inputs = EventInput.ProcessInput(true, true);
				inputs &= EventInput.ChanbaraMask;
				if (FF9StateSystem.MobilePlatform)
				{
					if ((inputs & EventInput.Pstart) > 0u)
					{
						EventInput.IsNeedAddStartSignal = true;
					}
					else if (EventInput.addStartSignal > 0)
					{
						inputs |= EventInput.Pstart;
						EventInput.addStartSignal--;
						EventInput.InputLog("Extra Start");
					}
				}
			}
		}
		else if (fldMapNo == 606) // L. Castle/Event
		{
			if (EventHUD.CurrentHUD == MinigameHUD.Telescope)
				inputs = EventInput.ProcessInput(false, true);
		}
		else if (fldMapNo == 2204 && TimerUI.Enable) // Palace/Odyssey
		{
			inputs = EventInput.ProcessInput(false, false);
		}
		else if (fldMapNo == 1607) // Mdn. Sari/Kitchen
		{
			inputs = EventInput.ProcessInput(false, false);
		}
		else if (fldMapNo == 1420) // Fossil Roo/Cavern
		{
			inputs = EventInput.ProcessInput(false, true);
		}
		else if (fldMapNo == 1422) // Fossil Roo/Entrance
		{
			inputs = EventInput.ProcessInput(false, true);
		}
		else
		{
			Dialog mognetDialog = Singleton<DialogManager>.Instance.GetMognetDialog();
			if (mognetDialog != null)
			{
				if (mognetDialog.IsChoiceReady)
					inputs = EventInput.ProcessInput(false, true);
			}
			else if (FF9StateSystem.MobilePlatform)
			{
				inputs = EventInput.ProcessInput(false, isKeyboardOrJoystickInput);
				if (isKeyboardOrJoystickInput)
					UIManager.Input.ResetTriggerEvent();
				EventInput.GetWorldTriggerButton(ref inputs);
			}
			else
			{
				inputs = EventInput.ProcessInput(false, true);
			}
		}
		inputs |= EventInput.eventButtonInput;
		if (EventInput.isDialogConfirm)
		{
			inputs |= EventInput.ConfirmMask;
			EventInput.isDialogConfirm = false;
		}
		if (HonoBehaviorSystem.Instance.IsFastForwardModeActive())
		{
			if (FF9StateSystem.MobilePlatform)
			{
				if (fldMapNo == 909 || fldMapNo == 1909) // Treno/Auction Site or Treno/Auction House
					inputs = EventInput.FastForwardProcess(gMode, fldMapNo, inputs);
			}
			else
			{
				inputs = EventInput.FastForwardProcess(gMode, fldMapNo, inputs);
			}
		}
		inputs &= ~EventInput.PSXCntlPadMask[0];
		if (FF9StateSystem.MobilePlatform && gMode == 3 && EventCollision.IsRidingChocobo())
		{
			if ((inputs & EventInput.Lsquare) > 0u || (inputs & EventInput.Psquare) > 0u)
				EventInput.isPressedDig = true;
			else if ((inputs & EventInput.Ltriangle) > 0u || (inputs & EventInput.Ptriangle) > 0u)
				EventInput.isPressedDig = false;
			else if ((inputs & EventInput.Lx) > 0u || (inputs & EventInput.Px) > 0u)
				EventInput.isPressedDig = false;
			else if ((inputs & EventInput.Lselect) > 0u || (inputs & EventInput.Pselect) > 0u)
				EventInput.isPressedDig = false;
		}
		if (gMode == 3 && EventEngineUtils.IsMogCalled(PersistenSingleton<EventEngine>.Instance))
			ff9.w_isMogActive = true;
		if (gMode == 3 && EMinigame.CheckBeachMinigame() && EventCollision.IsWorldTrigger() && (inputs & CancelMask) > 0u)
		{
			inputs &= ~CancelMask;
			EventInput.InputLog("Remove cancel mask for <SQEX> #2893");
		}
		EventInput.eventButtonInput = 0u;
		EventInput.ResetWorldTriggerButton();
		return inputs;
	}

	public static UInt32 ReadInputLight()
	{
		UInt32 inputs = 0u;
		if (!EventInput.isProcessingInput)
			return inputs;
		Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
		Int32 gMode = PersistenSingleton<EventEngine>.Instance.gMode;
		inputs = EventInput.ProcessInput(false, false);
		inputs |= EventInput.eventButtonInput;
		if (EventInput.isDialogConfirm)
		{
			inputs |= EventInput.ConfirmMask;
			EventInput.isDialogConfirm = false;
		}
		if (HonoBehaviorSystem.Instance.IsFastForwardModeActive())
			inputs = EventInput.FastForwardProcess(gMode, fldMapNo, inputs);
		inputs &= ~EventInput.PSXCntlPadMask[0];
		//if (FF9StateSystem.MobilePlatform && gMode == 3 && EventCollision.IsRidingChocobo())
		//{
		//	if ((inputs & EventInput.Lsquare) > 0u || (inputs & EventInput.Psquare) > 0u)
		//		EventInput.isPressedDig = true;
		//	else if ((inputs & EventInput.Ltriangle) > 0u || (inputs & EventInput.Ptriangle) > 0u)
		//		EventInput.isPressedDig = false;
		//	else if ((inputs & EventInput.Lx) > 0u || (inputs & EventInput.Px) > 0u)
		//		EventInput.isPressedDig = false;
		//	else if ((inputs & EventInput.Lselect) > 0u || (inputs & EventInput.Pselect) > 0u)
		//		EventInput.isPressedDig = false;
		//}
		if (gMode == 3 && EventEngineUtils.IsMogCalled(PersistenSingleton<EventEngine>.Instance))
			ff9.w_isMogActive = true;
		EventInput.eventButtonInput = 0u;
		return inputs;
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

	public static UInt32 ProcessInput(Boolean isTriggerDirection, Boolean isTriggerButton)
	{
		UInt32 inputs = 0u;
		if (VirtualAnalog.HasInput())
		{
			Vector2 vector = VirtualAnalog.GetAnalogValue();
			if (Mathf.Abs(vector.x) >= 0.1f || Mathf.Abs(vector.y) >= 0.1f)
			{
				if (vector.y > 0f)
					inputs |= EventInput.Pup;
				if (vector.y < 0f)
					inputs |= EventInput.Pdown;
				if (vector.x < 0f)
					inputs |= EventInput.Pleft;
				if (vector.x > 0f)
					inputs |= EventInput.Pright;
			}
		}
		if (EventInput.GetKey(Control.Menu, isTriggerButton))
		{
			inputs |= EventInput.Ptriangle | EventInput.Ltriangle;
			EventInput.InputLog("Press /_\\");
		}
		if (EventInput.GetKey(Control.Confirm, isTriggerButton))
		{
			inputs |= EventInput.Pcircle | EventInput.Lcircle;
			EventInput.InputLog("Press 0");
		}
		if (EventInput.GetKey(Control.Cancel, isTriggerButton))
		{
			inputs |= EventInput.Px | EventInput.Lx;
			EventInput.InputLog("Press X");
		}
		if (EventInput.GetKey(Control.Special, isTriggerButton))
		{
			inputs |= EventInput.Psquare | EventInput.Lsquare;
			EventInput.InputLog("Press []");
		}
		if (EventInput.GetKey(Control.Select, isTriggerButton))
		{
			inputs |= EventInput.Pselect;
			EventInput.InputLog("Press Select");
		}
		if (EventInput.GetKey(Control.Pause, isTriggerButton))
		{
			inputs |= EventInput.Pstart;
			EventInput.InputLog("Press Start");
		}
		if (EventInput.GetKey(Control.LeftBumper, isTriggerButton))
		{
			inputs |= EventInput.PL1 | EventInput.LL1;
			EventInput.InputLog("Press L1");
		}
		if (EventInput.GetKey(Control.RightBumper, isTriggerButton))
		{
			inputs |= EventInput.PR1 | EventInput.LR1;
			EventInput.InputLog("Press R1");
		}
		if (EventInput.GetKey(Control.LeftTrigger, isTriggerButton))
		{
			inputs |= EventInput.PL2 | EventInput.LL2;
			EventInput.InputLog("Press L2");
		}
		if (EventInput.GetKey(Control.RightTrigger, isTriggerButton))
		{
			inputs |= EventInput.PR2 | EventInput.LR2;
			EventInput.InputLog("Press R2");
		}
		if (EventInput.GetKey(Control.Up, isTriggerDirection))
		{
			inputs |= EventInput.Pup;
			EventInput.InputLog("Press ^");
		}
		if (EventInput.GetKey(Control.Down, isTriggerDirection))
		{
			inputs |= EventInput.Pdown;
			EventInput.InputLog("Press v");
		}
		if (EventInput.GetKey(Control.Left, isTriggerDirection))
		{
			inputs |= EventInput.Pleft;
			EventInput.InputLog("Press <");
		}
		if (EventInput.GetKey(Control.Right, isTriggerDirection))
		{
			inputs |= EventInput.Pright;
			EventInput.InputLog("Press >");
		}
		return inputs;
	}

	public static Boolean GetKey(Control keyCode, Boolean isTriggerMode)
	{
		if (isTriggerMode)
			return UIManager.Input.GetKeyTrigger(keyCode);
		return UIManager.Input.GetKey(keyCode);
	}

	private static void InputLog(String log)
	{
		if (EventInput.showLog)
			global::Debug.Log(log + " at " + RealTime.time);
	}

	private static UInt32 FastForwardProcess(Int32 eventMode, Int32 fldMapNo, UInt32 input)
	{
		if (EventInput.lastTimeInput == RealTime.time)
			if (fldMapNo != 1420 || eventMode != 1) // Fossil Roo/Cavern, climbing ivy
				input &= ~EventInput.OperationMask;
			else if (input != 0u)
				EventInput.lastTimeInput = RealTime.time;
		if (EventHUD.CurrentHUD == MinigameHUD.RacingHippaul && (input & EventInput.HippualMask) == EventInput.HippualMask)
			input = 0u;
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
			EventInput.isMovementControl = false;
		else
			EventInput.isMovementControl = true;
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
		PosObj controlChar = PersistenSingleton<EventEngine>.Instance.GetControlChar();
		if (controlChar != null)
		{
			GameObject go = controlChar.go;
			if (go != null)
				return go.GetComponent<FieldMapActorController>().GetLadderFlag() != 0;
		}
		return false;
	}

	private static SourceControl GetCurrentInputSource()
	{
		SourceControl sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetDirectionAxisSource();
		if (sourceControl != SourceControl.None)
			return sourceControl;
		Int32 num = 14;
		for (Int32 i = 0; i < num; i++)
			if (PersistenSingleton<HonoInputManager>.Instance.GetSource((Control)i) != SourceControl.None)
				break;
		return sourceControl;
	}

	private static void GetWorldTriggerButton(ref UInt32 currentInput)
	{
		if (FF9StateSystem.AndroidPlatform && PersistenSingleton<EventEngine>.Instance.gMode == 3 && EventInput.IsKeyboardOrJoystickInput)
		{
			currentInput &= ~EventInput.OperationMask;
			UInt32 inputs = 0u;
			if (PersistenSingleton<AndroidEventInputManager>.Instance.GetKeyTrigger(Control.Menu))
			{
				inputs |= EventInput.Ptriangle | EventInput.Ltriangle;
				EventInput.InputLog("Press /_\\");
			}
			if (PersistenSingleton<AndroidEventInputManager>.Instance.GetKeyTrigger(Control.Confirm))
			{
				inputs |= EventInput.Pcircle | EventInput.Lcircle;
				EventInput.InputLog("Press 0");
			}
			if (PersistenSingleton<AndroidEventInputManager>.Instance.GetKeyTrigger(Control.Cancel))
			{
				inputs |= EventInput.Px | EventInput.Lx;
				EventInput.InputLog("Press X");
			}
			if (PersistenSingleton<AndroidEventInputManager>.Instance.GetKeyTrigger(Control.Special))
			{
				inputs |= EventInput.Psquare | EventInput.Lsquare;
				EventInput.InputLog("Press []");
			}
			currentInput |= inputs;
		}
	}

	private static void ResetWorldTriggerButton()
	{
		if (FF9StateSystem.AndroidPlatform && PersistenSingleton<EventEngine>.Instance.gMode == 3)
			PersistenSingleton<AndroidEventInputManager>.Instance.Reset();
	}

	public const UInt32 Pselect = 1u;

	public const UInt32 Lselect = 1u;

	public const UInt32 Pstart = 8u;

	public const UInt32 Lstart = 8u;

	public const UInt32 Pup = 0x10u;

	public const UInt32 Lup = 0x10u;

	public const UInt32 Pright = 0x20u;

	public const UInt32 Lright = 0x20u;

	public const UInt32 Pdown = 0x40u;

	public const UInt32 Ldown = 0x40u;

	public const UInt32 Pleft = 0x80u;

	public const UInt32 Lleft = 0x80u;

	public const UInt32 PL2 = 0x100u;

	public const UInt32 PR2 = 0x200u;

	public const UInt32 PL1 = 0x400u;

	public const UInt32 PR1 = 0x800u;

	public const UInt32 Ptriangle = 0x1000u;

	public const UInt32 Pcircle = 0x2000u;

	public const UInt32 Px = 0x4000u;

	public const UInt32 Psquare = 0x8000u;

	public const UInt32 Lmenu = 0x40000u;

	public const UInt32 Lsquare = 0x80000u;

	public const UInt32 LL1 = 0x100000u;

	public const UInt32 LR1 = 0x200000u;

	public const UInt32 LL2 = 0x400000u;

	public const UInt32 LR2 = 0x800000u;

	public const UInt32 Ltriangle = 0x1000000u;

	public const UInt32 Lnavi = 0x2000000u;

	public const UInt32 Lmog = EventInput.Lsquare;

	public const Byte PSXCNTL_MAX_PADS = 2;

	public static UInt32 Lcircle = 0x20000u;

	public static UInt32 Lx = 0x10000u;

	public static UInt32 Fcircle = 0x10000u;

	public static UInt32 Fx = 0x20000u;

	private static Boolean showLog = false;

	private static Boolean isJapaneseLayout = false;

	private static UInt32[] PSXCntlPadMask = new UInt32[2];

	private static readonly UInt32 MovementMask = EventInput.Pup | EventInput.Pright | EventInput.Pdown | EventInput.Pleft; // 0xF0

	private static readonly UInt32 ChanbaraMask = 0xFFFFFFFEu;

	private static readonly UInt32 OperationMask = EventInput.Lx | EventInput.Px | EventInput.Lcircle | EventInput.Pcircle | EventInput.Lsquare | EventInput.Psquare | EventInput.Ltriangle | EventInput.Ptriangle; // 0x10BF000u

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
