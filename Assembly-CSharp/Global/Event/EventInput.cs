using Memoria;
using System;
using UnityEngine;

public static class EventInput
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
        get => (EventInput.PSXCntlPadMask[0] & EventInput.MenuControl) == 0u;
    }

    public static Boolean IsKeyboardOrJoystickInput
    {
        get
        {
            switch (EventInput.GetCurrentInputSource())
            {
                case SourceControl.KeyBoard:
                case SourceControl.Joystick:
                    return true;
                case SourceControl.Touch:
                    return false;
            }
            return PersistenSingleton<HonoInputManager>.Instance.IsControllerConnect;
        }
    }

    public static void ChangeInputLayout(String language)
    {
        // TODO: maybe add a configuration for that? As a Localization entry (like "ReadingDirection")?
        if (language == "Japanese")
            EventInput.SetJapaneseLayout();
        else
            EventInput.SetOtherLangLayout();
    }

    private static void SetJapaneseLayout()
    {
        EventInput.isJapaneseLayout = true;
        NGUIText.ButtonNames[0] = "CIRCLE";
        NGUIText.ButtonNames[1] = "CROSS";
        PersistenSingleton<HonoInputManager>.Instance.SetJapaneseLayout(true);
    }

    private static void SetOtherLangLayout()
    {
        EventInput.isJapaneseLayout = false;
        NGUIText.ButtonNames[0] = "CROSS";
        NGUIText.ButtonNames[1] = "CIRCLE";
        PersistenSingleton<HonoInputManager>.Instance.SetJapaneseLayout(false);
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
                    if ((inputs & EventInput.Start) > 0u)
                    {
                        EventInput.IsNeedAddStartSignal = true;
                    }
                    else if (EventInput.addStartSignal > 0)
                    {
                        inputs |= EventInput.Start;
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
            inputs |= EventInput.GetKeyMaskFromControl(Control.Confirm);
            EventInput.isDialogConfirm = false;
        }
        if (HonoBehaviorSystem.Instance.IsFastForwardModeActive())
            if (!FF9StateSystem.MobilePlatform || fldMapNo == 909 || fldMapNo == 1909) // Treno/Auction Site or Treno/Auction House
                inputs = EventInput.FastForwardProcess(gMode, fldMapNo, inputs);
        inputs &= ~EventInput.PSXCntlPadMask[0];
        if (FF9StateSystem.MobilePlatform && gMode == 3 && EventCollision.IsRidingChocobo())
        {
            if ((inputs & EventInput.GetKeyMaskFromControl(Control.Special)) > 0u)
                EventInput.isPressedDig = true;
            else if ((inputs & EventInput.GetKeyMaskFromControl(Control.Menu)) > 0u)
                EventInput.isPressedDig = false;
            else if ((inputs & EventInput.GetKeyMaskFromControl(Control.Cancel)) > 0u)
                EventInput.isPressedDig = false;
            else if ((inputs & EventInput.Select) > 0u)
                EventInput.isPressedDig = false;
        }
        if (gMode == 3 && EventEngineUtils.IsMogCalled(PersistenSingleton<EventEngine>.Instance))
            ff9.w_isMogActive = true;
        if (gMode == 3 && EMinigame.CheckBeachMinigame() && EventCollision.IsWorldTrigger() && (inputs & EventInput.GetKeyMaskFromControl(Control.Confirm)) > 0u)
        {
            inputs &= ~EventInput.GetKeyMaskFromControl(Control.Confirm);
            EventInput.InputLog("Remove confirm mask for <SQEX> #2893");
        }
        EventInput.eventButtonInput = 0u;
        EventInput.ResetWorldTriggerButton();
        return inputs;
    }

    public static UInt32 ReadInputLight()
    {
        if (!EventInput.isProcessingInput)
            return 0u;
        Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
        Int32 gMode = PersistenSingleton<EventEngine>.Instance.gMode;
        UInt32 inputs = EventInput.ProcessInput(false, false);
        inputs |= EventInput.eventButtonInput;
        if (EventInput.isDialogConfirm)
        {
            inputs |= EventInput.GetKeyMaskFromControl(Control.Confirm);
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
            if (vector.magnitude > Configuration.AnalogControl.StickThreshold)
            {
                if (vector.y > 0f)
                    inputs |= EventInput.Up;
                if (vector.y < 0f)
                    inputs |= EventInput.Down;
                if (vector.x < 0f)
                    inputs |= EventInput.Left;
                if (vector.x > 0f)
                    inputs |= EventInput.Right;
            }
        }
        if (EventInput.GetKey(Control.Menu, isTriggerButton))
        {
            inputs |= EventInput.GetKeyMaskFromControl(Control.Menu);
            EventInput.InputLog("Press /_\\");
        }
        if (EventInput.GetKey(Control.Confirm, isTriggerButton))
        {
            inputs |= EventInput.GetKeyMaskFromControl(Control.Confirm);
            EventInput.InputLog("Press X");
        }
        if (EventInput.GetKey(Control.Cancel, isTriggerButton))
        {
            inputs |= EventInput.GetKeyMaskFromControl(Control.Cancel);
            EventInput.InputLog("Press 0");
        }
        if (EventInput.GetKey(Control.Special, isTriggerButton))
        {
            inputs |= EventInput.GetKeyMaskFromControl(Control.Special);
            EventInput.InputLog("Press []");
        }
        if (EventInput.GetKey(Control.Select, isTriggerButton))
        {
            inputs |= EventInput.Select;
            EventInput.InputLog("Press Select");
        }
        if (EventInput.GetKey(Control.Pause, isTriggerButton))
        {
            inputs |= EventInput.Start;
            EventInput.InputLog("Press Start");
        }
        if (EventInput.GetKey(Control.LeftBumper, isTriggerButton))
        {
            inputs |= EventInput.GetKeyMaskFromControl(Control.LeftBumper);
            EventInput.InputLog("Press L1");
        }
        if (EventInput.GetKey(Control.RightBumper, isTriggerButton))
        {
            inputs |= EventInput.GetKeyMaskFromControl(Control.RightBumper);
            EventInput.InputLog("Press R1");
        }
        if (EventInput.GetKey(Control.LeftTrigger, isTriggerButton))
        {
            inputs |= EventInput.GetKeyMaskFromControl(Control.LeftTrigger);
            EventInput.InputLog("Press L2");
        }
        if (EventInput.GetKey(Control.RightTrigger, isTriggerButton))
        {
            inputs |= EventInput.GetKeyMaskFromControl(Control.RightTrigger);
            EventInput.InputLog("Press R2");
        }
        if (EventInput.GetKey(Control.Up, isTriggerDirection))
        {
            inputs |= EventInput.Up;
            EventInput.InputLog("Press ^");
        }
        if (EventInput.GetKey(Control.Down, isTriggerDirection))
        {
            inputs |= EventInput.Down;
            EventInput.InputLog("Press v");
        }
        if (EventInput.GetKey(Control.Left, isTriggerDirection))
        {
            inputs |= EventInput.Left;
            EventInput.InputLog("Press <");
        }
        if (EventInput.GetKey(Control.Right, isTriggerDirection))
        {
            inputs |= EventInput.Right;
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
        if (EventHUD.CurrentHUD == MinigameHUD.RacingHippaul && (input & EventInput.HippaulMask) == EventInput.HippaulMask)
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
        for (Int32 i = 0; i < 14; i++)
            if ((sourceControl = PersistenSingleton<HonoInputManager>.Instance.GetSource((Control)i)) != SourceControl.None)
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
                inputs |= EventInput.GetKeyMaskFromControl(Control.Menu);
                EventInput.InputLog("Press /_\\");
            }
            if (PersistenSingleton<AndroidEventInputManager>.Instance.GetKeyTrigger(Control.Confirm))
            {
                inputs |= EventInput.GetKeyMaskFromControl(Control.Confirm);
                EventInput.InputLog("Press X");
            }
            if (PersistenSingleton<AndroidEventInputManager>.Instance.GetKeyTrigger(Control.Cancel))
            {
                inputs |= EventInput.GetKeyMaskFromControl(Control.Cancel);
                EventInput.InputLog("Press 0");
            }
            if (PersistenSingleton<AndroidEventInputManager>.Instance.GetKeyTrigger(Control.Special))
            {
                inputs |= EventInput.GetKeyMaskFromControl(Control.Special);
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

    public static UInt32 GetKeyMaskFromControl(Control control)
    {
        UInt32 logicalInput = 0u;
        switch (control)
        {
            case Control.Confirm:
                logicalInput = EventInput.Confirm;
                break;
            case Control.Cancel:
                logicalInput = EventInput.Cancel;
                break;
            case Control.Menu:
                logicalInput = EventInput.Menu;
                break;
            case Control.Special:
                logicalInput = EventInput.Special;
                break;
            case Control.LeftBumper:
                logicalInput = EventInput.LeftBumper;
                break;
            case Control.RightBumper:
                logicalInput = EventInput.RightBumper;
                break;
            case Control.LeftTrigger:
                logicalInput = EventInput.LeftTrigger;
                break;
            case Control.RightTrigger:
                logicalInput = EventInput.RightTrigger;
                break;
            case Control.Pause:
                return EventInput.Start;
            case Control.Select:
                return EventInput.Select;
            case Control.Up:
                return EventInput.Up;
            case Control.Down:
                return EventInput.Up;
            case Control.Left:
                return EventInput.Up;
            case Control.Right:
                return EventInput.Up;
            case Control.DPad:
            case Control.None:
                return 0u;
        }
        Int32 buttonIndex = PersistenSingleton<HonoInputManager>.Instance.LogicalControlToPhysicalButton(control);
        switch (buttonIndex)
        {
            case 0: return logicalInput | EventInput.Cross;
            case 1: return logicalInput | EventInput.Circle;
            case 2: return logicalInput | EventInput.Triangle;
            case 3: return logicalInput | EventInput.Square;
            case 4: return logicalInput | EventInput.L1;
            case 5: return logicalInput | EventInput.R1;
            case 6: return logicalInput | EventInput.L2;
            case 7: return logicalInput | EventInput.R2;
        }
        return logicalInput;
    }

    // Physical buttons
    public const UInt32 Select = 1u;
    public const UInt32 Start = 8u;
    public const UInt32 Up = 0x10u;
    public const UInt32 Right = 0x20u;
    public const UInt32 Down = 0x40u;
    public const UInt32 Left = 0x80u;
    public const UInt32 L2 = 0x100u;
    public const UInt32 R2 = 0x200u;
    public const UInt32 L1 = 0x400u;
    public const UInt32 R1 = 0x800u;
    public const UInt32 Triangle = 0x1000u;
    public const UInt32 Circle = 0x2000u;
    public const UInt32 Cross = 0x4000u;
    public const UInt32 Square = 0x8000u;

    // Logical buttons
    public const UInt32 Cancel = 0x10000u;
    public const UInt32 Confirm = 0x20000u;
    public const UInt32 MenuControl = 0x40000u;
    public const UInt32 Special = 0x80000u;
    public const UInt32 LeftBumper = 0x100000u;
    public const UInt32 RightBumper = 0x200000u;
    public const UInt32 LeftTrigger = 0x400000u;
    public const UInt32 RightTrigger = 0x800000u;
    public const UInt32 Menu = 0x1000000u;
    public const UInt32 NaviControl = 0x2000000u;

    public static Boolean isJapaneseLayout = false;

    public const Byte PSXCNTL_MAX_PADS = 2;

    private static Boolean showLog = false;

    private static UInt32[] PSXCntlPadMask = new UInt32[2];
    private static readonly UInt32 MovementMask = EventInput.Up | EventInput.Right | EventInput.Down | EventInput.Left;
    private static readonly UInt32 ChanbaraMask = ~EventInput.Select; // Blank sword fight mini-game
    private static readonly UInt32 OperationMask = EventInput.Confirm | EventInput.Cross | EventInput.Cancel | EventInput.Circle | EventInput.Special | EventInput.Square | EventInput.Menu | EventInput.Triangle;

    private static Boolean isMovementControl = true;
    private static Boolean isProcessingInput = true;
    private static Boolean isDialogConfirm = false;
    private static Int32 addStartSignal = 0;
    private static Single lastTimeInput = 0f;
    private static Boolean isPressedDig = false;
    private static UInt32 eventButtonInput = 0u;
    private static UInt32 HippaulMask = EventInput.Cancel | EventInput.Special; // Hippaul race mini-game
}
