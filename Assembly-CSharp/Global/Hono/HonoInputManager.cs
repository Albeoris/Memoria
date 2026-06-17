using Memoria;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using XInputDotNetPure;

public class HonoInputManager : PersistenSingleton<HonoInputManager>
{
    public HonoInputManager()
    {
        this.analogControlEnabled = Configuration.AnalogControl.Enabled;
        XInputDotNetPure.GamePad.Threshold = Configuration.AnalogControl.StickThreshold;
        this.inputKeys3 = [KeyCode.KeypadEnter, KeyCode.Mouse1, KeyCode.None];
        this.inputKeysEmpty = new KeyCode[3];
        this.inputKeys2Android = new KeyCode[]
        {
            (KeyCode)10,
            KeyCode.Escape,
            KeyCode.Tab
        };
        this.joystickKeysPrimary = new String[10];
        this.defaultInputKeys = new KeyCode[]
        {
            KeyCode.X,
            KeyCode.C,
            KeyCode.V,
            KeyCode.B,
            KeyCode.G,
            KeyCode.H,
            KeyCode.F,
            KeyCode.J,
            MemoriaKeyBindings[4], // Backspace,
            MemoriaKeyBindings[5] // Alpha1
        };
        this.defaultJoystickInputKeys = new String[]
        {
            "JoystickButton0", // Control.Confirm -> Cross
            "JoystickButton1", // Control.Cancel  -> Circle
            "JoystickButton3", // Control.Menu    -> Triangle
            "JoystickButton2", // Control.Special -> Square
            "JoystickButton4", // Control.LeftBumper
            "JoystickButton5", // Control.RightBumper
            "LeftTrigger",     // Control.LeftTrigger
            "RightTrigger",    // Control.RightTrigger
            "JoystickButton7", // Control.Pause
            "JoystickButton6"  // Control.Select
        };
        this.defaultAndroidJoystickInputKeys = new String[]
        {
            "JoystickButton0",
            "JoystickButton1",
            "JoystickButton3",
            "JoystickButton2",
            "JoystickButton4",
            "JoystickButton5",
            "LeftTrigger Android",
            "RightTrigger Android",
            "JoystickButton10",
            "Empty"
        };
        this.defaultIOSJoystickInputKeys = new String[]
        {
            "JoystickButton13",
            "JoystickButton14",
            "JoystickButton12",
            "JoystickButton15",
            "JoystickButton8",
            "JoystickButton9",
            "JoystickButton10",
            "JoystickButton11",
            "JoystickButton0",
            "Empty"
        };
        this.defaultaaaaInputKeys = new String[]
        {
            "JoystickButton0",
            "JoystickButton1",
            "JoystickButton3",
            "JoystickButton2",
            "JoystickButton4",
            "JoystickButton5",
            "LeftTrigger",
            "RightTrigger",
            "JoystickButton7",
            "JoystickButton6"
        };
        this.androidJoystickSelectKeys2 = "JoystickButton11";
        this.defaultHorizontalInputKeys = "Horizontal";
        this.defaultVerticalInputKeys = "Vertical";
        this.isInput = new Boolean[10];
        this.isInputDown = new Boolean[10];
        this.isInputUp = new Boolean[10];
        this.hasJoyAxisSignal = new Boolean[10];
        this.hasJoyAxisSignalNotZero = new Boolean[10];
        this.isHAxisTriggered = new Boolean[2];
        this.isVAxisTriggered = new Boolean[2];
        this.isHAxisUp = new Boolean[2];
        this.isVAxisUp = new Boolean[2];
        this.hasHAxisSignal = new Boolean[2];
        this.hasVAxisSignal = new Boolean[2];
        this.hasHAxisSignalNotZero = new Boolean[2];
        this.hasVAxisSignalNotZero = new Boolean[2];
        this.rightAnalogButtonStatus = new Boolean[3];
        this.isButtonDown = new Boolean[10];
        this.logicalToButton = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
    }

    public static KeyCode[] MemoriaKeyBindings = InitKeyBindings();
    private static KeyCode[] InitKeyBindings()
    {
        KeyCode[] codes = [KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Backspace, KeyCode.Alpha1];
        // Movements
        try
        {
            KeyCode[] parsed = [
                (KeyCode)Enum.Parse(typeof(KeyCode), Configuration.Control.KeyBindings[0], true),
                (KeyCode)Enum.Parse(typeof(KeyCode), Configuration.Control.KeyBindings[1], true),
                (KeyCode)Enum.Parse(typeof(KeyCode), Configuration.Control.KeyBindings[2], true),
                (KeyCode)Enum.Parse(typeof(KeyCode), Configuration.Control.KeyBindings[3], true)
            ];
            Log.Message($"[KeyBinding] Succesfully parsed movement key bindings ({Configuration.Control.KeyBindings[0]}, {Configuration.Control.KeyBindings[1]}, {Configuration.Control.KeyBindings[2]}, {Configuration.Control.KeyBindings[3]})");
            codes[0] = parsed[0];
            codes[1] = parsed[1];
            codes[2] = parsed[2];
            codes[3] = parsed[3];
        }
        catch
        {
            Log.Warning("[KeyBinding] Couldn't parse movement key bindings. Defaulting to WASD");
        }
        // Pause
        try
        {
            KeyCode parsed = (KeyCode)Enum.Parse(typeof(KeyCode), Configuration.Control.KeyBindings[4], true);
            Log.Message($"[KeyBinding] Succesfully parsed pause key bindings ({Configuration.Control.KeyBindings[4]})");
            codes[4] = parsed;
        }
        catch
        {
            Log.Warning("[KeyBinding] Couldn't parse pause key bindings. Defaulting to Backspace");
        }
        // Help
        try
        {

            KeyCode parsed = (KeyCode)Enum.Parse(typeof(KeyCode), Configuration.Control.KeyBindings[5], true);
            Log.Message($"[KeyBinding] Succesfully parsed help key bindings ({Configuration.Control.KeyBindings[5]})");
            codes[5] = parsed;
        }
        catch
        {
            Log.Warning("[KeyBinding] Couldn't parse help key bindings. Defaulting to Alpha1");
        }
        return codes;
    }

    public Int32[] ThresholdLogicalKeys
    {
        get
        {
            UIManager.UIState state = PersistenSingleton<UIManager>.Instance.State;
            if (state != UIManager.UIState.Pause)
                return null;
            return HonoInputManager.PauseUIThresholdKeys;
        }
    }

    private void ScanAndroidTVJoystick()
    {
        if (FF9StateSystem.AndroidTVPlatform && this.IsControllerConnect && FF9StateSystem.EnableAndroidTVJoystickMode)
        {
            this.ValidateButtonDown();
            this.ScanPauseCombinationKey();
        }
    }

    private void ScanPauseCombinationKey()
    {
        Single axis = UnityEngine.Input.GetAxis(this.SpecificPlatformRightTriggerKey);
        Single axis2 = UnityEngine.Input.GetAxis(this.SpecificPlatformLeftTriggerKey);
        if (!this.isInputDown[HonoInputManager.PauseIndex])
        {
            if (axis > 0.19f && axis2 > 0.19f)
                this.isInputDown[HonoInputManager.PauseIndex] = true;
            if (this.isInputDown[HonoInputManager.PauseIndex] && this.lastFrameRightTriggerAxis > 0.19f && this.lastFrameLeftTriggerAxis > 0.19f)
                this.isInputDown[HonoInputManager.PauseIndex] = false;
        }
        this.lastFrameRightTriggerAxis = axis;
        this.lastFrameLeftTriggerAxis = axis2;
    }

    private void ValidateButtonDown()
    {
        Int32[] logicalPauseKeys = this.ThresholdLogicalKeys;
        if (logicalPauseKeys != null)
        {
            for (Int32 i = 0; i < logicalPauseKeys.Length; i++)
            {
                // No need for Control -> Int32 conversion as both Confirm/Cancel are in the list
                Int32 keyIndex = logicalPauseKeys[i];
                if (this.isInputDown[keyIndex])
                {
                    this.downTimer = 0f;
                    this.isButtonDown[keyIndex] = true;
                    this.isInputDown[keyIndex] = false;
                }
                else if (this.isInput[keyIndex])
                {
                    this.downTimer += Time.deltaTime;
                    if (this.downTimer > HonoInputManager.DownThreshold && this.isButtonDown[keyIndex])
                    {
                        this.downTimer = 0f;
                        this.isButtonDown[keyIndex] = false;
                        this.isInputDown[keyIndex] = true;
                    }
                }
                else if (this.isInputUp[keyIndex])
                {
                    this.downTimer = 0f;
                    this.isButtonDown[keyIndex] = false;
                }
            }
        }
    }

    public Boolean IgnoreCheckingDirectionSources { get; set; }

    public void SetJapaneseLayout(Boolean japaneseLayout)
    {
        this.defaultInputKeys[0] = japaneseLayout ? KeyCode.C : KeyCode.X; // Control.Confirm
        this.defaultInputKeys[1] = japaneseLayout ? KeyCode.X : KeyCode.C; // Control.Cancel
        this.defaultJoystickInputKeys[0] = japaneseLayout ? "JoystickButton1" : "JoystickButton0";
        this.defaultJoystickInputKeys[1] = japaneseLayout ? "JoystickButton0" : "JoystickButton1";
        this.defaultAndroidJoystickInputKeys[0] = japaneseLayout ? "JoystickButton1" : "JoystickButton0";
        this.defaultAndroidJoystickInputKeys[1] = japaneseLayout ? "JoystickButton0" : "JoystickButton1";
        this.defaultIOSJoystickInputKeys[0] = japaneseLayout ? "JoystickButton14" : "JoystickButton13";
        this.defaultIOSJoystickInputKeys[1] = japaneseLayout ? "JoystickButton13" : "JoystickButton14";
        this.defaultaaaaInputKeys[0] = japaneseLayout ? "JoystickButton1" : "JoystickButton0";
        this.defaultaaaaInputKeys[1] = japaneseLayout ? "JoystickButton0" : "JoystickButton1";
        SetPrimaryKeys();
    }

    private void ScanSources()
    {
        this.ScanKeySources();
        this.ScanDirectionSources();
        this.ScanOnScreenSources();
    }

    private void ScanKeySources()
    {
        this.ResetSources();
        if (this.isDisablePrimaryKey)
        {
            for (Int32 index = 0; index < 3 && (UnityEngine.Input.touchCount <= 1 || index != 1); ++index)
            {
                this.inputSources[index] = !UnityEngine.Input.GetButton(this.joystickKeysPrimary[index]) ? (UnityEngine.Input.GetKey(this.InputKeys2[index]) || UnityEngine.Input.GetKey(this.InputKeys3[index]) ? SourceControl.KeyBoard : SourceControl.None) : SourceControl.Joystick;
                this.inputDownSources[index] = this.inputDownSources[index] != SourceControl.None || !UnityEngine.Input.GetButtonDown(this.joystickKeysPrimary[index]) ? (this.inputDownSources[index] != SourceControl.None || !UnityEngine.Input.GetKeyDown(this.InputKeys2[index]) && !UnityEngine.Input.GetKeyDown(this.InputKeys3[index]) ? SourceControl.None : SourceControl.KeyBoard) : SourceControl.Joystick;
                this.inputUpSources[index] = this.inputUpSources[index] != SourceControl.None || !UnityEngine.Input.GetButtonUp(this.joystickKeysPrimary[index]) ? (this.inputUpSources[index] != SourceControl.None || !UnityEngine.Input.GetKeyUp(this.InputKeys2[index]) && !UnityEngine.Input.GetKeyUp(this.InputKeys3[index]) ? SourceControl.None : SourceControl.KeyBoard) : SourceControl.Joystick;
            }
        }
        else
        {
            for (Int32 index = 0; index < this.KeyName.Length && (UnityEngine.Input.touchCount <= 1 || index != 1); ++index)
            {
                this.inputSources[index] = this.joystickKeysPrimary[index] == this.SpecificPlatformLeftTriggerKey || this.joystickKeysPrimary[index] == this.SpecificPlatformRightTriggerKey ? (UnityEngine.Input.GetKey(this.inputKeysPrimary[index]) || index < 2 && (UnityEngine.Input.GetKey(this.InputKeys2[index]) || UnityEngine.Input.GetKey(this.InputKeys3[index])) ? SourceControl.KeyBoard : ((Double)UnityEngine.Input.GetAxisRaw(this.joystickKeysPrimary[index]) == 0.0 ? SourceControl.None : SourceControl.Joystick)) : (index >= 2 ? (!UnityEngine.Input.GetKey(this.inputKeysPrimary[index]) ? (!UnityEngine.Input.GetButton(this.joystickKeysPrimary[index]) ? SourceControl.None : SourceControl.Joystick) : SourceControl.KeyBoard) : (UnityEngine.Input.GetKey(this.inputKeysPrimary[index]) || UnityEngine.Input.GetKey(this.InputKeys2[index]) || UnityEngine.Input.GetKey(this.InputKeys3[index]) ? SourceControl.KeyBoard : (!UnityEngine.Input.GetButton(this.joystickKeysPrimary[index]) ? SourceControl.None : SourceControl.Joystick)));
                if (this.joystickKeysPrimary[index] == this.SpecificPlatformLeftTriggerKey || this.joystickKeysPrimary[index] == this.SpecificPlatformRightTriggerKey)
                {
                    if (this.inputDownSources[index] == SourceControl.None && (UnityEngine.Input.GetKeyDown(this.inputKeysPrimary[index]) || index < 2 && (UnityEngine.Input.GetKeyDown(this.InputKeys2[index]) || UnityEngine.Input.GetKeyDown(this.InputKeys3[index]))))
                        this.inputDownSources[index] = SourceControl.KeyBoard;
                    else if (this.inputDownSources[index] == SourceControl.None && UnityEngine.Input.GetKeyDown(this.inputKeysPrimary[index]))
                        this.inputDownSources[index] = SourceControl.KeyBoard;
                    else if (this.inputDownSources[index] == SourceControl.None && (Double)UnityEngine.Input.GetAxisRaw(this.joystickKeysPrimary[index]) != 0.0)
                    {
                        if (!this.hasJoyAxisSignal[index])
                            this.inputDownSources[index] = SourceControl.Joystick;
                    }
                    else
                        this.inputDownSources[index] = SourceControl.None;
                }
                else
                    this.inputDownSources[index] = index >= 2 ? (this.inputDownSources[index] != SourceControl.None || !UnityEngine.Input.GetKeyDown(this.inputKeysPrimary[index]) ? (this.inputDownSources[index] != SourceControl.None || !UnityEngine.Input.GetButtonDown(this.joystickKeysPrimary[index]) ? SourceControl.None : SourceControl.Joystick) : SourceControl.KeyBoard) : (this.inputDownSources[index] != SourceControl.None || !UnityEngine.Input.GetKeyDown(this.inputKeysPrimary[index]) && !UnityEngine.Input.GetKeyDown(this.InputKeys2[index]) && !UnityEngine.Input.GetKeyDown(this.InputKeys3[index]) ? (this.inputDownSources[index] != SourceControl.None || !UnityEngine.Input.GetButtonDown(this.joystickKeysPrimary[index]) ? SourceControl.None : SourceControl.Joystick) : SourceControl.KeyBoard);
                this.inputUpSources[index] = this.joystickKeysPrimary[index] == this.SpecificPlatformLeftTriggerKey || this.joystickKeysPrimary[index] == this.SpecificPlatformRightTriggerKey ? (this.inputUpSources[index] != SourceControl.None || !UnityEngine.Input.GetKeyUp(this.inputKeysPrimary[index]) && (index >= 2 || !UnityEngine.Input.GetKeyUp(this.InputKeys2[index]) && !UnityEngine.Input.GetKeyUp(this.InputKeys3[index])) ? (this.inputUpSources[index] != SourceControl.None || !UnityEngine.Input.GetKeyUp(this.inputKeysPrimary[index]) ? (this.inputUpSources[index] != SourceControl.None || (Double)UnityEngine.Input.GetAxisRaw(this.joystickKeysPrimary[index]) != 0.0 ? SourceControl.None : (!this.hasJoyAxisSignalNotZero[index] ? SourceControl.None : SourceControl.Joystick)) : SourceControl.KeyBoard) : SourceControl.KeyBoard) : (index >= 2 ? (this.inputUpSources[index] != SourceControl.None || !UnityEngine.Input.GetKeyUp(this.inputKeysPrimary[index]) ? (this.inputUpSources[index] != SourceControl.None || !UnityEngine.Input.GetButtonUp(this.joystickKeysPrimary[index]) ? SourceControl.None : SourceControl.Joystick) : SourceControl.KeyBoard) : (this.inputUpSources[index] != SourceControl.None || !UnityEngine.Input.GetKeyUp(this.inputKeysPrimary[index]) && !UnityEngine.Input.GetKeyUp(this.InputKeys2[index]) && !UnityEngine.Input.GetKeyUp(this.InputKeys3[index]) ? (this.inputUpSources[index] != SourceControl.None || !UnityEngine.Input.GetButtonUp(this.joystickKeysPrimary[index]) ? SourceControl.None : SourceControl.Joystick) : SourceControl.KeyBoard));
            }
        }
    }

    private void ScanDirectionSources()
    {
        Control lazyKey = UIManager.Input.GetLazyKey();
        Single dx;
        Single dy;
        if (VirtualAnalog.HasInput())
        {
            this.directionAxisSources = SourceControl.Touch;
            dx = VirtualAnalog.Value.x;
            dy = VirtualAnalog.Value.y;
        }
        else
        {
            this.directionAxisSources = (Double)UnityEngine.Input.GetAxis(this.SpecificPlatformHorizontalInputKey) != 0.0 || (Double)UnityEngine.Input.GetAxis(this.SpecificPlatformVerticalInputKey) != 0.0 ? SourceControl.Joystick : ((Double)UnityEngine.Input.GetAxis(this.DefaultHorizontalInputKey) != 0.0 || (Double)UnityEngine.Input.GetAxis(this.DefaultVerticalInputKey) != 0.0 ? SourceControl.KeyBoard : SourceControl.None);
            dx = this.GetHorizontalNavigation();
            dy = this.GetVerticalNavigation();
        }
        if (!this.IgnoreCheckingDirectionSources)
        {
            if (dx > 0f)
            {
                this.inputSources[13] = this.directionAxisSources;
            }
            else if (dx < 0f)
            {
                this.inputSources[12] = this.directionAxisSources;
            }
            else
            {
                if (lazyKey != Control.Right)
                    this.inputSources[13] = SourceControl.None;
                if (lazyKey != Control.Left)
                    this.inputSources[12] = SourceControl.None;
            }
            if (dy > 0f)
            {
                this.inputSources[10] = this.directionAxisSources;
            }
            else if (dy < 0f)
            {
                this.inputSources[11] = this.directionAxisSources;
            }
            else
            {
                if (lazyKey != Control.Up)
                    this.inputSources[10] = SourceControl.None;
                if (lazyKey != Control.Down)
                    this.inputSources[11] = SourceControl.None;
            }
            if (this.directionAxisSources == SourceControl.Joystick || this.directionAxisSources == SourceControl.KeyBoard)
            {
                this.inputDownSources[10] = this.isVAxisTriggered[0] ? this.directionAxisSources : SourceControl.None;
                this.inputDownSources[11] = this.isVAxisTriggered[1] ? this.directionAxisSources : SourceControl.None;
                this.inputDownSources[12] = this.isHAxisTriggered[0] ? this.directionAxisSources : SourceControl.None;
                this.inputDownSources[13] = this.isHAxisTriggered[1] ? this.directionAxisSources : SourceControl.None;
                this.inputUpSources[10] = this.isVAxisUp[0] ? this.directionAxisSources : SourceControl.None;
                this.inputUpSources[11] = this.isVAxisUp[1] ? this.directionAxisSources : SourceControl.None;
                this.inputUpSources[12] = this.isHAxisUp[0] ? this.directionAxisSources : SourceControl.None;
                this.inputUpSources[13] = this.isHAxisUp[1] ? this.directionAxisSources : SourceControl.None;
            }
        }
    }

    private void ScanOnScreenSources()
    {
        Control lazyKey = UIManager.Input.GetLazyKey();
        if (lazyKey != Control.None)
            this.inputSources[(Int32)lazyKey] = SourceControl.Touch;
    }

    private void ResetSources()
    {
        for (Int32 i = 0; i < this.inputSources.Length; i++)
        {
            this.inputSources[i] = SourceControl.None;
            this.inputDownSources[i] = SourceControl.None;
            this.inputUpSources[i] = SourceControl.None;
        }
    }

    private void TestSource()
    {
        for (Int32 i = 0; i < this.inputSources.Length; i++)
        {
            Control control = (Control)i;
            SourceControl source = this.GetSource(control);
            if (source != SourceControl.None)
                global::Debug.Log("Found input " + control.ToString() + " from " + source.ToString());
        }
    }

    public void SetInputDownSources(SourceControl source, Control key)
    {
        this.inputDownSources[(Int32)key] = source;
    }

    public SourceControl GetSource(Control key)
    {
        if (this.inputDownSources[(Int32)key] != SourceControl.None)
            return this.inputDownSources[(Int32)key];
        if (this.inputSources[(Int32)key] != SourceControl.None)
            return this.inputSources[(Int32)key];
        if (this.inputUpSources[(Int32)key] != SourceControl.None)
            return this.inputUpSources[(Int32)key];
        return SourceControl.None;
    }

    public SourceControl GetDirectionAxisSource()
    {
        return this.directionAxisSources;
    }

    public Boolean DisablePrimaryKey
    {
        get => this.isDisablePrimaryKey;
        set => this.isDisablePrimaryKey = value;
    }

    public KeyCode[] InputKeys2
    {
        get
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                return this.inputKeysEmpty;
            if (Application.platform == RuntimePlatform.Android)
                return this.inputKeys2Android;
            return this.inputKeys2;
        }
    }

    public KeyCode[] InputKeys3
    {
        get
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                return this.inputKeysEmpty;
            return this.inputKeys3;
        }
    }

    public KeyCode[] InputKeysPrimary => this.inputKeysPrimary;
    public String[] JoystickKeysPrimary => this.joystickKeysPrimary;
    public KeyCode[] DefaultInputKeys => this.defaultInputKeys;

    public String[] DefaultJoystickInputKeys
    {
        get
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                return this.defaultIOSJoystickInputKeys;
            if (Application.platform == RuntimePlatform.Android)
                return this.defaultAndroidJoystickInputKeys;
            if (FF9StateSystem.aaaaPlatform)
                return this.defaultaaaaInputKeys;
            return this.defaultJoystickInputKeys;
        }
    }

    public String DefaultHorizontalInputKey => this.defaultHorizontalInputKeys;

    public String SpecificPlatformHorizontalInputKey
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android)
                return "Horizontal Android";
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                return "Horizontal iOS";
            return "Horizontal";
            // I don't know what is that but for DualShock 4 it returns -0.085 for the X-Axis
            // return "Horizontal NonMobile";
        }
    }

    public String DefaultVerticalInputKey => this.defaultVerticalInputKeys;

    public String SpecificPlatformVerticalInputKey
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android)
                return "Vertical Android";
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                return "Vertical iOS";
            return "Vertical";
            // I don't know what is that but for DualShock 4 it returns -0.085 for the X-Axis
            // return "Vertical NonMobile";
        }
    }

    public String SpecificPlatformLeftTriggerKey
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android)
                return "LeftTrigger Android";
            return "LeftTrigger";
        }
    }

    public String SpecificPlatformRightTriggerKey
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android)
                return "RightTrigger Android";
            return "RightTrigger";
        }
    }

    public String SpecificPlatformRightAnalogHorizontalKey => "Right Analog Horizontal";
    public String SpecificPlatformRightAnlogVerticalKey => "Right Analog Vertical";

    public Boolean IsControllerConnect => this.IsJoystickConnect();
    public Boolean IsRightAnalogDown => this.rightAnalogButtonStatus[0];
    public Boolean IsRightAnalogPress => this.rightAnalogButtonStatus[1];
    public Boolean IsRightAnalogUp => this.rightAnalogButtonStatus[2];

    public static Boolean MouseEnabled => HonoInputManager.mouseEnabled;
    public static Boolean JoystickEnabled => HonoInputManager.joystickEnabled;
    public static Boolean VirtualAnalogEnabled => HonoInputManager.virtualAnalogEnabled;

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern Int32 GetWindowThreadProcessId(IntPtr handle, out Int32 processId);

    [DllImport("user32.dll")]
    private static extern Int16 GetAsyncKeyState(Int32 key);

    public static Boolean ApplicationIsActivated()
    {
        IntPtr foregroundWindow = HonoInputManager.GetForegroundWindow();
        if (foregroundWindow == IntPtr.Zero)
            return false;
        HonoInputManager.GetWindowThreadProcessId(foregroundWindow, out int foregroundId);
        return foregroundId == HonoInputManager.procId;
    }

    private Boolean CheckRawXInput(String unityButton, GamePadState padState)
    {
        if (unityButton == "JoystickButton0")
            return padState.Buttons.A == ButtonState.Pressed;
        if (unityButton == "JoystickButton1")
            return padState.Buttons.B == ButtonState.Pressed;
        if (unityButton == "JoystickButton2")
            return padState.Buttons.X == ButtonState.Pressed;
        if (unityButton == "JoystickButton3")
            return padState.Buttons.Y == ButtonState.Pressed;
        if (unityButton == "JoystickButton4")
            return padState.Buttons.LeftShoulder == ButtonState.Pressed;
        if (unityButton == "JoystickButton5")
            return padState.Buttons.RightShoulder == ButtonState.Pressed;
        if (unityButton == "JoystickButton6")
            return padState.Buttons.Back == ButtonState.Pressed;
        if (unityButton == "JoystickButton7")
            return padState.Buttons.Start == ButtonState.Pressed;
        if (unityButton == this.SpecificPlatformLeftTriggerKey)
            return padState.Triggers.Left > 0.75f;
        return unityButton == this.SpecificPlatformRightTriggerKey && padState.Triggers.Right > 0.75f;
    }

    private void CheckPersistentInput()
    {
        if (!this.isDisablePrimaryKey)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            for (Int32 i = 0; i < this.KeyName.Length; i++)
            {
                if (ApplicationIsActivated() && VKKeyCodeMapping.ContainsKey(this.inputKeysPrimary[i]) && GetAsyncKeyState(VKKeyCodeMapping[this.inputKeysPrimary[i]]) != 0)
                    this.isInput[i] = true;
                if ((Configuration.Control.AlwaysCaptureGamepad || ApplicationIsActivated()) && this.CheckRawXInput(this.joystickKeysPrimary[i], state))
                    this.isInput[i] = true;
            }
        }
    }

    public Boolean CheckPersistentDirectionInput(Control key)
    {
        if (!HonoInputManager.ApplicationIsActivated())
            return false;
        if (Application.isEditor)
            return false;
        if (key == Control.Left)
            return HonoInputManager.GetAsyncKeyState(37) != 0 || HonoInputManager.GetAsyncKeyState(65) != 0;
        if (key == Control.Right)
            return HonoInputManager.GetAsyncKeyState(39) != 0 || HonoInputManager.GetAsyncKeyState(68) != 0;
        if (key == Control.Up)
            return HonoInputManager.GetAsyncKeyState(38) != 0 || HonoInputManager.GetAsyncKeyState(87) != 0;
        return key == Control.Down && (HonoInputManager.GetAsyncKeyState(40) != 0 || HonoInputManager.GetAsyncKeyState(83) != 0);
    }

    private Boolean IsJoystickConnect()
    {
        return global::GamePad.GetState(PlayerIndex.One).IsConnected;
    }

    private void InitializeProcID()
    {
        HonoInputManager.procId = Process.GetCurrentProcess().Id;
    }

    private Boolean IsMobileJoystickConnect()
    {
        String[] joystickNames = UnityXInput.Input.GetJoystickNames();
        return joystickNames.Length > 0 && joystickNames[0].Length > 0;
    }

    protected override void Awake()
    {
        base.Awake();
        this.InitialInput();
        this.InitializeProcID();
    }

    private void Start()
    {
        String text = this.SpecificPlatformHorizontalInputKey;
        String text2 = this.SpecificPlatformVerticalInputKey;
        text = text + "," + this.DefaultHorizontalInputKey;
        text2 = text2 + "," + this.DefaultVerticalInputKey;
        UICamera.list[0].horizontalAxisName = text;
        UICamera.list[0].verticalAxisName = text2;
    }

    private void Update()
    {
        this.ScanKey();
        this.ScanDirectionKeys();
        this.ScanRightAnalogKeys();
        this.CheckPersistentInput();
        this.ScanSources();
        this.ScanAndroidTVJoystick();
    }

    public void InitialInput()
    {
        this.SetCustomKeys();
        this.SetPrimaryKeys();
        this.SetControlFlag();
    }

    public void ScanKey()
    {
        if (this.isDisablePrimaryKey)
        {
            for (Int32 i = 0; i < 3; i++)
            {
                if (UnityXInput.Input.touchCount > 1 && i == 1)
                    return;
                this.isInput[i] = UnityXInput.Input.GetButton(this.joystickKeysPrimary[i]) || UnityXInput.Input.GetKey(this.InputKeys2[i]) || UnityXInput.Input.GetKey(this.InputKeys3[i]);
                this.isInputDown[i] = !this.isInputDown[i] && (UnityXInput.Input.GetButtonDown(this.joystickKeysPrimary[i]) || UnityXInput.Input.GetKeyDown(this.InputKeys2[i]) || UnityXInput.Input.GetKeyDown(this.InputKeys3[i]));
                this.isInputUp[i] = !this.isInputUp[i] && (UnityXInput.Input.GetButtonUp(this.joystickKeysPrimary[i]) || UnityXInput.Input.GetKeyUp(this.InputKeys2[i]) || UnityXInput.Input.GetKeyUp(this.InputKeys3[i]));
            }
        }
        else
        {
            for (Int32 i = 0; i < this.KeyName.Length; i++)
            {
                if (UnityXInput.Input.touchCount > 1 && i == 1)
                    return;
                if (this.joystickKeysPrimary[i] == this.SpecificPlatformLeftTriggerKey || this.joystickKeysPrimary[i] == this.SpecificPlatformRightTriggerKey)
                    this.isInput[i] = UnityXInput.Input.GetKey(this.inputKeysPrimary[i]) || (i < 2 && (UnityXInput.Input.GetKey(this.InputKeys2[i]) || UnityXInput.Input.GetKey(this.InputKeys3[i]))) || UnityXInput.Input.GetAxisRaw(this.joystickKeysPrimary[i]) != 0f;
                else if (i < 2)
                    this.isInput[i] = UnityXInput.Input.GetKey(this.inputKeysPrimary[i]) || UnityXInput.Input.GetButton(this.joystickKeysPrimary[i]) || UnityXInput.Input.GetKey(this.InputKeys2[i]) || UnityXInput.Input.GetKey(this.InputKeys3[i]);
                else
                    this.isInput[i] = UnityXInput.Input.GetKey(this.inputKeysPrimary[i]) || UnityXInput.Input.GetButton(this.joystickKeysPrimary[i]);
                if (this.joystickKeysPrimary[i] == this.SpecificPlatformLeftTriggerKey || this.joystickKeysPrimary[i] == this.SpecificPlatformRightTriggerKey)
                {
                    if (!this.isInputDown[i] && (UnityXInput.Input.GetKeyDown(this.inputKeysPrimary[i]) || (i < 2 && (UnityXInput.Input.GetKeyDown(this.InputKeys2[i]) || UnityXInput.Input.GetKeyDown(this.InputKeys3[i])))))
                    {
                        this.isInputDown[i] = true;
                    }
                    else if (!this.isInputDown[i] && (UnityXInput.Input.GetKeyDown(this.inputKeysPrimary[i]) || UnityXInput.Input.GetAxisRaw(this.joystickKeysPrimary[i]) != 0f))
                    {
                        if (!this.hasJoyAxisSignal[i])
                        {
                            this.hasJoyAxisSignal[i] = true;
                            this.isInputDown[i] = true;
                        }
                    }
                    else
                    {
                        this.isInputDown[i] = false;
                    }
                    if (UnityXInput.Input.GetAxisRaw(this.joystickKeysPrimary[i]) == 0f)
                        this.hasJoyAxisSignal[i] = false;
                }
                else if (i < 2)
                {
                    this.isInputDown[i] = !this.isInputDown[i] && (UnityXInput.Input.GetKeyDown(this.inputKeysPrimary[i]) || UnityXInput.Input.GetButtonDown(this.joystickKeysPrimary[i]) || UnityXInput.Input.GetKeyDown(this.InputKeys2[i]) || UnityXInput.Input.GetKeyDown(this.InputKeys3[i]));
                }
                else
                {
                    this.isInputDown[i] = !this.isInputDown[i] && (UnityXInput.Input.GetKeyDown(this.inputKeysPrimary[i]) || UnityXInput.Input.GetButtonDown(this.joystickKeysPrimary[i]));
                }
                if (this.joystickKeysPrimary[i] == this.SpecificPlatformLeftTriggerKey || this.joystickKeysPrimary[i] == this.SpecificPlatformRightTriggerKey)
                {
                    if (!this.isInputUp[i] && (UnityXInput.Input.GetKeyUp(this.inputKeysPrimary[i]) || (i < 2 && (UnityXInput.Input.GetKeyUp(this.InputKeys2[i]) || UnityXInput.Input.GetKeyUp(this.InputKeys3[i])))))
                    {
                        this.isInputUp[i] = true;
                    }
                    else if (!this.isInputUp[i] && (UnityXInput.Input.GetKeyUp(this.inputKeysPrimary[i]) || UnityXInput.Input.GetAxisRaw(this.joystickKeysPrimary[i]) == 0f))
                    {
                        if (this.hasJoyAxisSignalNotZero[i])
                        {
                            this.hasJoyAxisSignalNotZero[i] = false;
                            this.isInputUp[i] = true;
                        }
                    }
                    else
                    {
                        this.isInputUp[i] = false;
                    }
                    if (UnityXInput.Input.GetAxisRaw(this.joystickKeysPrimary[i]) != 0f)
                        this.hasJoyAxisSignalNotZero[i] = true;
                }
                else if (i < 2)
                {
                    this.isInputUp[i] = !this.isInputUp[i] && (UnityXInput.Input.GetKeyUp(this.inputKeysPrimary[i]) || UnityXInput.Input.GetButtonUp(this.joystickKeysPrimary[i]) || UnityXInput.Input.GetKeyUp(this.InputKeys2[i]) || UnityXInput.Input.GetKeyUp(this.InputKeys3[i]));
                }
                else
                {
                    this.isInputUp[i] = !this.isInputUp[i] && (UnityXInput.Input.GetKeyUp(this.inputKeysPrimary[i]) || UnityXInput.Input.GetButtonUp(this.joystickKeysPrimary[i]));
                }
            }
            this.ScanMobilePrimarySelectKey();
            this.ScanAndroidSecondarySelectKey();
        }
    }

    private void ScanDirectionKeys()
    {
        if (!this.isHAxisTriggered[0] && this.GetHorizontalNavigation() < -HonoInputManager.AnalogThreadhold)
        {
            if (!this.hasHAxisSignal[0])
            {
                this.hasHAxisSignal[0] = true;
                this.isHAxisTriggered[0] = true;
            }
        }
        else
        {
            this.isHAxisTriggered[0] = false;
        }
        if (!this.isHAxisTriggered[1] && this.GetHorizontalNavigation() > HonoInputManager.AnalogThreadhold)
        {
            if (!this.hasHAxisSignal[1])
            {
                this.hasHAxisSignal[1] = true;
                this.isHAxisTriggered[1] = true;
            }
        }
        else
        {
            this.isHAxisTriggered[1] = false;
        }
        if (!this.hasVAxisSignal[0] && this.GetVerticalNavigation() > HonoInputManager.AnalogThreadhold)
        {
            if (!this.hasVAxisSignal[0])
            {
                this.hasVAxisSignal[0] = true;
                this.isVAxisTriggered[0] = true;
            }
        }
        else
        {
            this.isVAxisTriggered[0] = false;
        }
        if (!this.hasVAxisSignal[1] && this.GetVerticalNavigation() < -HonoInputManager.AnalogThreadhold)
        {
            if (!this.hasVAxisSignal[1])
            {
                this.hasVAxisSignal[1] = true;
                this.isVAxisTriggered[1] = true;
            }
        }
        else
        {
            this.isVAxisTriggered[1] = false;
        }
        if (this.GetHorizontalNavigation() == 0f)
        {
            this.hasHAxisSignal[0] = false;
            this.hasHAxisSignal[1] = false;
        }
        if (this.GetVerticalNavigation() == 0f)
        {
            this.hasVAxisSignal[0] = false;
            this.hasVAxisSignal[1] = false;
        }
        if (!this.isHAxisUp[0] && this.GetHorizontalNavigation() == 0f)
        {
            if (this.hasHAxisSignalNotZero[0])
            {
                this.hasHAxisSignalNotZero[0] = false;
                this.isHAxisUp[0] = true;
            }
        }
        else
        {
            this.isHAxisUp[0] = false;
        }
        if (!this.isHAxisUp[1] && this.GetHorizontalNavigation() == 0f)
        {
            if (this.hasHAxisSignalNotZero[1])
            {
                this.hasHAxisSignalNotZero[1] = false;
                this.isHAxisUp[1] = true;
            }
        }
        else
        {
            this.isHAxisUp[1] = false;
        }
        if (!this.isVAxisUp[0] && this.GetVerticalNavigation() == 0f)
        {
            if (this.hasVAxisSignalNotZero[0])
            {
                this.hasVAxisSignalNotZero[0] = false;
                this.isVAxisUp[0] = true;
            }
        }
        else
        {
            this.isVAxisUp[0] = false;
        }
        if (!this.isVAxisUp[1] && this.GetVerticalNavigation() == 0f)
        {
            if (this.hasVAxisSignalNotZero[1])
            {
                this.hasVAxisSignalNotZero[1] = false;
                this.isVAxisUp[1] = true;
            }
        }
        else
        {
            this.isVAxisUp[1] = false;
        }
        if (this.GetHorizontalNavigation() != 0f && this.isHAxisTriggered[0])
            this.hasHAxisSignalNotZero[0] = true;
        if (this.GetHorizontalNavigation() != 0f && this.isHAxisTriggered[1])
            this.hasHAxisSignalNotZero[1] = true;
        if (this.GetVerticalNavigation() != 0f && this.isVAxisTriggered[0])
            this.hasVAxisSignalNotZero[0] = true;
        if (this.GetVerticalNavigation() != 0f && this.isVAxisTriggered[1])
            this.hasVAxisSignalNotZero[1] = true;
    }

    private Boolean GetDirectionKey(Control key)
    {
        // Threshold: We don't want the stick to be too sensitive for UI navigation
        Single threshold = Mathf.Max(0.3f, AnalogThreadhold);
        if (key == Control.Left)
            return this.GetHorizontalNavigation() < -threshold || this.CheckPersistentDirectionInput(key);
        if (key == Control.Right)
            return this.GetHorizontalNavigation() > threshold || this.CheckPersistentDirectionInput(key);
        if (key == Control.Up)
            return this.GetVerticalNavigation() > threshold || this.CheckPersistentDirectionInput(key);
        return key == Control.Down && (this.GetVerticalNavigation() < -threshold || this.CheckPersistentDirectionInput(key));
    }

    private Boolean GetDirectionKeyDown(Control control)
    {
        switch (control)
        {
            case Control.Up:
                return this.isVAxisTriggered[0];
            case Control.Down:
                return this.isVAxisTriggered[1];
            case Control.Left:
                return this.isHAxisTriggered[0];
            case Control.Right:
                return this.isHAxisTriggered[1];
            default:
                return false;
        }
    }

    private Boolean GetDirectionKeyUp(Control control)
    {
        switch (control)
        {
            case Control.Up:
                return this.isVAxisUp[0];
            case Control.Down:
                return this.isVAxisUp[1];
            case Control.Left:
                return this.isHAxisUp[0];
            case Control.Right:
                return this.isHAxisUp[1];
            default:
                return false;
        }
    }

    public Boolean IsInput(Control index)
    {
        Int32 indexInt = (Int32)index;
        if (indexInt >= this.KeyName.Length || indexInt < 0)
            return this.GetDirectionKey(index);
        if (UIKeyTrigger.IsNeedToRemap() && index == Control.Confirm)
            return UnityXInput.Input.GetKey(KeyCode.Escape) || this.isInput[(Int32)Control.Confirm];
        return this.isInput[indexInt];
    }

    public Boolean IsInputDown(Control index)
    {
        Int32 indexInt = (Int32)index;
        if (indexInt >= this.KeyName.Length || index < 0)
            return this.GetDirectionKeyDown(index);
        if (UIKeyTrigger.IsNeedToRemap() && index == Control.Confirm)
            return UnityXInput.Input.GetKeyDown(KeyCode.Escape) || this.isInputDown[(Int32)Control.Confirm];
        return this.isInputDown[indexInt];
    }

    public Boolean IsInputUp(Control index)
    {
        Int32 indexInt = (Int32)index;
        if (indexInt >= this.KeyName.Length || indexInt < 0)
            return this.GetDirectionKeyUp(index);
        if (UIKeyTrigger.IsNeedToRemap() && index == Control.Confirm)
            return UnityXInput.Input.GetKeyUp(KeyCode.Escape) || this.isInputUp[(Int32)Control.Confirm];
        return this.isInputUp[indexInt];
    }

    public Vector2 GetAxis()
    {
        return this.AxisValue();
    }

    public void SetVirtualAnalogEnable(Boolean value)
    {
        if (value)
            VirtualAnalog.Enable();
        else
            VirtualAnalog.Disable();
    }

    public void SetPrimaryKeys()
    {
        if (FF9StateSystem.Settings.cfg.control == 0UL)
        {
            for (Int32 i = 0; i < this.defaultInputKeys.Length; i++)
            {
                this.inputKeysPrimary[i] = this.defaultInputKeys[i];
                this.joystickKeysPrimary[i] = this.DefaultJoystickInputKeys[i];
                this.logicalToButton[i] = i;
            }
            if (EventInput.isJapaneseLayout)
            {
                this.logicalToButton[0] = 1;
                this.logicalToButton[1] = 0;
            }
        }
        else
        {
            String[] defaultKeys = this.DefaultJoystickInputKeys;
            for (Int32 i = 0; i < this.defaultInputKeys.Length; i++)
            {
                // Kind of assume that "control_data_joystick" and "defaultKeys" contain the same strings (in different orders)
                // It should always be the case, except maybe if a game save generated on another machine (eg. iOS) is loaded...
                String customKey = FF9StateSystem.Settings.cfg.control_data_joystick[i];
                this.inputKeysPrimary[i] = FF9StateSystem.Settings.cfg.control_data_keyboard[i];
                this.joystickKeysPrimary[i] = customKey;
                this.logicalToButton[i] = i;
                for (Int32 j = 0; j < defaultKeys.Length; j++)
                {
                    if (customKey == defaultKeys[j])
                    {
                        this.logicalToButton[i] = j;
                        break;
                    }
                }
                if (EventInput.isJapaneseLayout)
                {
                    if (this.logicalToButton[i] == 0)
                        this.logicalToButton[i] = 1;
                    else if (this.logicalToButton[i] == 1)
                        this.logicalToButton[i] = 0;
                }
            }
        }
    }

    public void SetCustomKeys()
    {
        FF9StateSystem.Settings.cfg.control_data_keyboard = new KeyCode[HonoInputManager.DefaultInputKeysCount];
        FF9StateSystem.Settings.cfg.control_data_joystick = new String[HonoInputManager.DefaultInputKeysCount];
        for (Int32 i = 0; i < this.defaultInputKeys.Length; i++)
        {
            FF9StateSystem.Settings.cfg.control_data_keyboard[i] = this.defaultInputKeys[i];
            FF9StateSystem.Settings.cfg.control_data_joystick[i] = this.DefaultJoystickInputKeys[i];
        }
    }

    /// <summary>Eg.: NGUIText.ButtonNames[LogicalControlToPhysicalButton(control)]</summary>
    public Int32 LogicalControlToPhysicalButton(Control control)
    {
        return this.logicalToButton[(Int32)control];
    }

    public Control PhysicalButtonToLogicalControl(Int32 button)
    {
        for (Int32 control = 0; control < this.logicalToButton.Length; control++)
            if (this.logicalToButton[control] == button)
                return (Control)control;
        return (Control)button;
    }

    public void SetControlFlag()
    {
        RuntimePlatform platform = Application.platform;
        RuntimePlatform runtimePlatform = platform;
        switch (runtimePlatform)
        {
            case RuntimePlatform.IPhonePlayer:
                HonoInputManager.mouseEnabled = false;
                HonoInputManager.joystickEnabled = true;
                HonoInputManager.virtualAnalogEnabled = true;
                break;
            case RuntimePlatform.WindowsPlayer:
                HonoInputManager.mouseEnabled = true;
                HonoInputManager.joystickEnabled = true;
                HonoInputManager.virtualAnalogEnabled = false;
                break;
            case RuntimePlatform.Android:
                HonoInputManager.mouseEnabled = false;
                HonoInputManager.joystickEnabled = true;
                HonoInputManager.virtualAnalogEnabled = true;
                break;
            case RuntimePlatform.PS3:
            case RuntimePlatform.XBOX360:
            default:
                HonoInputManager.mouseEnabled = true;
                HonoInputManager.joystickEnabled = true;
                HonoInputManager.virtualAnalogEnabled = true;
                break;
        }
        if (HonoInputManager.virtualAnalogEnabled && HonoInputManager.joystickEnabled)
            this.AxisValue = new Func<Vector2>(this.GetInputAxis);
        else if (HonoInputManager.virtualAnalogEnabled)
            this.AxisValue = new Func<Vector2>(this.GetVirtualAnalogValue);
        else if (HonoInputManager.joystickEnabled)
            this.AxisValue = new Func<Vector2>(this.GetJoyStickValue);
    }

    public Vector2 GetVirtualAnalogValue()
    {
        return VirtualAnalog.Value;
    }

    public Single GetHorizontalNavigation()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Single axis2 = UnityXInput.Input.GetAxis(this.SpecificPlatformHorizontalInputKey);
            if (Math.Abs(axis2) > 0.01)
                return axis2;
        }
        return UnityXInput.Input.GetAxis(this.DefaultHorizontalInputKey);
    }

    public Single GetVerticalNavigation()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Single axis2 = UnityXInput.Input.GetAxis(this.SpecificPlatformVerticalInputKey);
            if (Math.Abs(axis2) > 0.01)
                return axis2;
        }
        return UnityXInput.Input.GetAxis(this.DefaultVerticalInputKey);
    }

    private Vector2 GetJoyStickValue()
    {
        Vector2 inputAxis = new Vector2(this.GetHorizontalNavigation(), this.GetVerticalNavigation());
        if (inputAxis.magnitude > 1 && analogControlEnabled)
            inputAxis.Normalize();
        return inputAxis;
    }

    private Vector2 GetInputAxis()
    {
        if (VirtualAnalog.HasInput())
            return VirtualAnalog.Value;
        Vector2 inputAxis = new Vector2(this.GetHorizontalNavigation(), this.GetVerticalNavigation());
        if (inputAxis.magnitude > 1 && analogControlEnabled)
            inputAxis.Normalize();
        return inputAxis;
    }

    private void ScanAndroidSecondarySelectKey()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (UnityXInput.Input.GetButton(this.androidJoystickSelectKeys2))
                this.isInput[9] = true;
            if (!this.isInputDown[9] && UnityXInput.Input.GetButtonDown(this.androidJoystickSelectKeys2))
                this.isInputDown[9] = true;
            if (!this.isInputUp[9] && UnityXInput.Input.GetButtonUp(this.androidJoystickSelectKeys2))
                this.isInputUp[9] = true;
        }
    }

    private void ScanMobilePrimarySelectKey()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.isEditor)
        {
            for (Int32 i = 0; i < this.KeyName.Length; i++)
            {
                if (this.joystickKeysPrimary[i] == "Empty")
                {
                    if (this.rightAnalogButtonStatus[1])
                        this.isInput[i] = true;
                    if (!this.isInputDown[i] && this.rightAnalogButtonStatus[0])
                        this.isInputDown[i] = true;
                    if (!this.isInputUp[i] && this.rightAnalogButtonStatus[2])
                        this.isInputUp[i] = true;
                }
            }
        }
    }

    private void ScanRightAnalogKeys()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.isEditor)
        {
            Vector2 vector = new Vector2(this.GetRightAnalogHorizontalNavigation(), this.GetRightAnalogVerticalNavigation());
            Single magnitude = vector.magnitude;
            this.rightAnalogButtonStatus[0] = false;
            this.rightAnalogButtonStatus[1] = false;
            this.rightAnalogButtonStatus[2] = false;
            if (magnitude > HonoInputManager.AnalogThreadhold && this.rightAnalogMagnitude == 0f)
                this.rightAnalogButtonStatus[0] = true;
            else if (magnitude > HonoInputManager.AnalogThreadhold && this.rightAnalogMagnitude > 0f)
                this.rightAnalogButtonStatus[1] = true;
            else if (magnitude == 0f && this.rightAnalogMagnitude > 0f)
                this.rightAnalogButtonStatus[2] = true;
            this.rightAnalogMagnitude = magnitude;
        }
    }

    private Single GetRightAnalogHorizontalNavigation()
    {
        return UnityXInput.Input.GetAxis(this.SpecificPlatformRightAnalogHorizontalKey);
    }

    private Single GetRightAnalogVerticalNavigation()
    {
        return UnityXInput.Input.GetAxis(this.SpecificPlatformRightAnlogVerticalKey);
    }
    public bool IsBindingInput { get; set; } = false;
    public bool IsL3Pressed()
    {
        if (IsBindingInput || Configuration.Control.LeftStick == -1) return false;

        if (Input.GetKeyDown((KeyCode)Configuration.Control.LeftStick))
            return true;

        return false;
    }

    public bool IsR3Pressed()
    {
        if (IsBindingInput || Configuration.Control.RightStick == -1) return false;

        if (Input.GetKeyDown((KeyCode)Configuration.Control.RightStick))
            return true;

        return false;
    }

    private void DebugControllerInput() // Debug to check input from joystick
    {
        for (int i = 0; i < 20; i++)
        {
            KeyCode key = KeyCode.JoystickButton0 + i;
            if (UnityEngine.Input.GetKey(key))
            {
                Log.Message("[DEBUG INPUT] Unity Key Pressed: " + key.ToString() + " (ID: " + (int)key + ")");
            }
        }

        float triggerLeft = UnityEngine.Input.GetAxis("LeftTrigger");
        if (triggerLeft > 0.1f)
        {
            Log.Message("[DEBUG INPUT] Unity Axis LeftTrigger: " + triggerLeft);
        }

        try
        {
            GamePadState state = XInputDotNetPure.GamePad.GetState(PlayerIndex.One);

            if (state.Buttons.LeftStick == ButtonState.Pressed)
                Log.Message("[DEBUG INPUT] XInput: LeftStick (L3) PRESSED");

            if (state.Buttons.RightStick == ButtonState.Pressed)
                Log.Message("[DEBUG INPUT] XInput: RightStick (R3) PRESSED");

            if (state.Triggers.Left > 0.0f)
                Log.Message("[DEBUG INPUT] XInput: LeftTrigger (L2) Value: " + state.Triggers.Left);
        }
        catch { }
    }

    public const Single TriggerThreshold = 0.19f;
    private const Single DownThreshold = 0.01f;
    private const Int32 PauseIndex = 8;

    private static readonly Int32[] PauseUIThresholdKeys = [0, 1, 2, 3, 5, 4, 7, 6];

    private Boolean[] isButtonDown;

    private Single downTimer;
    private Single lastFrameRightTriggerAxis;
    private Single lastFrameLeftTriggerAxis;

    private SourceControl[] inputSources = new SourceControl[14];
    private SourceControl[] inputDownSources = new SourceControl[14];
    private SourceControl[] inputUpSources = new SourceControl[14];

    private SourceControl directionAxisSources;

    public static Int32 DefaultInputKeysCount = 10;
    public static Single AnalogThreadhold = Configuration.AnalogControl.StickThreshold;

    private static Int32 procId;

    [SerializeField]
    private String[] KeyName = new String[]
    {
        "Confirm",
        "Cancel",
        "Menu",
        "Special",
        "LeftBumper",
        "RightBumper",
        "LeftTrigger",
        "RightTrigger",
        "Pause",
        "Select"
    };

    private KeyCode[] inputKeysPrimary = new KeyCode[10];
    private KeyCode[] inputKeys2 = new KeyCode[]
    {
        KeyCode.Return,
        // Disable Escape as cancel if bound to pause
        HonoInputManager.MemoriaKeyBindings[4] == KeyCode.Escape ? KeyCode.None : KeyCode.Escape,
        KeyCode.Tab
    };
    private KeyCode[] inputKeys3;
    private KeyCode[] inputKeysEmpty;
    private KeyCode[] inputKeys2Android;
    private String[] joystickKeysPrimary;
    private KeyCode[] defaultInputKeys;
    private String[] defaultJoystickInputKeys;
    private String[] defaultAndroidJoystickInputKeys;
    private String[] defaultIOSJoystickInputKeys;
    private String[] defaultaaaaInputKeys;

    private String androidJoystickSelectKeys2;
    private String defaultHorizontalInputKeys;
    private String defaultVerticalInputKeys;

    private Boolean[] isInput;
    private Boolean[] isInputDown;
    private Boolean[] isInputUp;
    private Boolean[] hasJoyAxisSignal;
    private Boolean[] hasJoyAxisSignalNotZero;
    private Boolean[] isHAxisTriggered;
    private Boolean[] isVAxisTriggered;
    private Boolean[] isHAxisUp;
    private Boolean[] isVAxisUp;
    private Boolean[] hasHAxisSignal;
    private Boolean[] hasVAxisSignal;
    private Boolean[] hasHAxisSignalNotZero;
    private Boolean[] hasVAxisSignalNotZero;

    private Boolean isDisablePrimaryKey;
    private Boolean analogControlEnabled;

    [SerializeField]
    private Boolean[] rightAnalogButtonStatus;
    private Single rightAnalogMagnitude;

    private Func<Vector2> AxisValue;

    [NonSerialized]
    private Int32[] logicalToButton;

    public static List<KeyCode> AcceptKeyCodeList = new List<KeyCode>
    {
        KeyCode.Comma,
        KeyCode.Minus,
        KeyCode.Period,
        KeyCode.Slash,
        KeyCode.Alpha0,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Equals,
        KeyCode.Semicolon,
        KeyCode.LeftBracket,
        KeyCode.Backslash,
        KeyCode.RightBracket,
        KeyCode.B,
        KeyCode.C,
        KeyCode.E,
        KeyCode.F,
        KeyCode.G,
        KeyCode.H,
        KeyCode.I,
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.M,
        KeyCode.N,
        KeyCode.O,
        KeyCode.P,
        KeyCode.Q,
        KeyCode.R,
        KeyCode.T,
        KeyCode.U,
        KeyCode.V,
        KeyCode.X,
        KeyCode.Y,
        KeyCode.Z,
        KeyCode.Keypad0,
        KeyCode.Keypad1,
        KeyCode.Keypad2,
        KeyCode.Keypad3,
        KeyCode.Keypad4,
        KeyCode.Keypad5,
        KeyCode.Keypad6,
        KeyCode.Keypad7,
        KeyCode.Keypad8,
        KeyCode.Keypad9,
        KeyCode.KeypadPeriod,
        KeyCode.KeypadDivide,
        KeyCode.KeypadMultiply,
        KeyCode.KeypadMinus,
        KeyCode.KeypadPlus,
        KeyCode.F8,
        KeyCode.F9,
        KeyCode.F10,
        KeyCode.F11,
        KeyCode.F12
    };

    private static Boolean mouseEnabled = false;
    private static Boolean joystickEnabled = false;
    private static Boolean virtualAnalogEnabled = false;

    private static Dictionary<KeyCode, Int32> VKKeyCodeMapping = new Dictionary<KeyCode, Int32>
    {
        { KeyCode.Comma,          188 },
        { KeyCode.Minus,          189 },
        { KeyCode.Period,         190 },
        { KeyCode.Slash,          191 },
        { KeyCode.Alpha0,         48 },
        { KeyCode.Alpha1,         49 },
        { KeyCode.Alpha2,         50 },
        { KeyCode.Alpha3,         51 },
        { KeyCode.Alpha4,         52 },
        { KeyCode.Alpha5,         53 },
        { KeyCode.Alpha6,         54 },
        { KeyCode.Alpha7,         55 },
        { KeyCode.Alpha8,         56 },
        { KeyCode.Alpha9,         57 },
        { KeyCode.Equals,         187 },
        { KeyCode.Semicolon,      186 },
        { KeyCode.LeftBracket,    219 },
        { KeyCode.Backslash,      220 },
        { KeyCode.RightBracket,   221 },
        { KeyCode.A,              65 },
        { KeyCode.B,              66 },
        { KeyCode.C,              67 },
        { KeyCode.D,              68 },
        { KeyCode.E,              69 },
        { KeyCode.F,              70 },
        { KeyCode.G,              71 },
        { KeyCode.H,              72 },
        { KeyCode.I,              73 },
        { KeyCode.J,              74 },
        { KeyCode.K,              75 },
        { KeyCode.L,              76 },
        { KeyCode.M,              77 },
        { KeyCode.N,              78 },
        { KeyCode.O,              79 },
        { KeyCode.P,              80 },
        { KeyCode.Q,              81 },
        { KeyCode.R,              82 },
        { KeyCode.S,              83 },
        { KeyCode.T,              84 },
        { KeyCode.U,              85 },
        { KeyCode.V,              86 },
        { KeyCode.W,              87 },
        { KeyCode.X,              88 },
        { KeyCode.Y,              89 },
        { KeyCode.Z,              90 },
        { KeyCode.Keypad0,        96 },
        { KeyCode.Keypad1,        97 },
        { KeyCode.Keypad2,        98 },
        { KeyCode.Keypad3,        99 },
        { KeyCode.Keypad4,        100 },
        { KeyCode.Keypad5,        101 },
        { KeyCode.Keypad6,        102 },
        { KeyCode.Keypad7,        103 },
        { KeyCode.Keypad8,        104 },
        { KeyCode.Keypad9,        105 },
        { KeyCode.KeypadPeriod,   110 },
        { KeyCode.KeypadDivide,   111 },
        { KeyCode.KeypadMultiply, 106 },
        { KeyCode.KeypadMinus,    109 },
        { KeyCode.KeypadPlus,     107 },
        { KeyCode.F8,             119 },
        { KeyCode.F9,             120 },
        { KeyCode.F10,            121 },
        { KeyCode.F11,            122 },
        { KeyCode.F12,            123 }
    };
}
