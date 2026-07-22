using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using static XInputDotNetPure.GamePadState;

namespace XInputDotNetPure
{
    class Imports
    {
        internal const string DLLName = "XInputInterface";

        [DllImport(DLLName)]
        public static extern uint XInputGamePadGetState(uint playerIndex, out GamePadState.RawState state);
        [DllImport(DLLName)]
        public static extern void XInputGamePadSetState(uint playerIndex, float leftMotor, float rightMotor);
    }

    public enum ButtonState
    {
        Pressed,
        Released
    }

    public struct GamePadButtons
    {
        ButtonState start, back, leftStick, rightStick, leftShoulder, rightShoulder, guide, a, b, x, y;

        internal GamePadButtons(ButtonState start, ButtonState back, ButtonState leftStick, ButtonState rightStick,
                                ButtonState leftShoulder, ButtonState rightShoulder, ButtonState guide,
                                ButtonState a, ButtonState b, ButtonState x, ButtonState y)
        {
            this.start = start;
            this.back = back;
            this.leftStick = leftStick;
            this.rightStick = rightStick;
            this.leftShoulder = leftShoulder;
            this.rightShoulder = rightShoulder;
            this.guide = guide;
            this.a = a;
            this.b = b;
            this.x = x;
            this.y = y;
        }

        public ButtonState Start
        {
            get { return start; }
        }

        public ButtonState Back
        {
            get { return back; }
        }

        public ButtonState LeftStick
        {
            get { return leftStick; }
        }

        public ButtonState RightStick
        {
            get { return rightStick; }
        }

        public ButtonState LeftShoulder
        {
            get { return leftShoulder; }
        }

        public ButtonState RightShoulder
        {
            get { return rightShoulder; }
        }

        public ButtonState Guide
        {
            get { return guide; }
        }

        public ButtonState A
        {
            get { return a; }
        }

        public ButtonState B
        {
            get { return b; }
        }

        public ButtonState X
        {
            get { return x; }
        }

        public ButtonState Y
        {
            get { return y; }
        }
    }

    public struct GamePadDPad
    {
        ButtonState up, down, left, right;

        internal GamePadDPad(ButtonState up, ButtonState down, ButtonState left, ButtonState right)
        {
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
        }

        public ButtonState Up
        {
            get { return up; }
        }

        public ButtonState Down
        {
            get { return down; }
        }

        public ButtonState Left
        {
            get { return left; }
        }

        public ButtonState Right
        {
            get { return right; }
        }
    }

    public struct GamePadThumbSticks
    {
        public struct StickValue
        {
            float x, y;

            internal StickValue(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public float X
            {
                get { return x; }
            }

            public float Y
            {
                get { return y; }
            }
        }

        StickValue left, right;

        internal GamePadThumbSticks(StickValue left, StickValue right)
        {
            this.left = left;
            this.right = right;
        }

        public StickValue Left
        {
            get { return left; }
        }

        public StickValue Right
        {
            get { return right; }
        }
    }

    public struct GamePadTriggers
    {
        float left;
        float right;

        internal GamePadTriggers(float left, float right)
        {
            this.left = left;
            this.right = right;
        }

        public float Left
        {
            get { return left; }
        }

        public float Right
        {
            get { return right; }
        }
    }

    public struct GamePadState
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct RawState
        {
            public uint dwPacketNumber;
            public GamePad Gamepad;

            [StructLayout(LayoutKind.Sequential)]
            public struct GamePad
            {
                public ushort wButtons;
                public byte bLeftTrigger;
                public byte bRightTrigger;
                public short sThumbLX;
                public short sThumbLY;
                public short sThumbRX;
                public short sThumbRY;
            }
        }

        bool isConnected;
        uint packetNumber;
        GamePadButtons buttons;
        GamePadDPad dPad;
        GamePadThumbSticks thumbSticks;
        GamePadTriggers triggers;

        public enum ButtonsConstants
        {
            DPadUp = 0x00000001,
            DPadDown = 0x00000002,
            DPadLeft = 0x00000004,
            DPadRight = 0x00000008,
            Start = 0x00000010,
            Back = 0x00000020,
            LeftThumb = 0x00000040,
            RightThumb = 0x00000080,
            LeftShoulder = 0x0100,
            RightShoulder = 0x0200,
            Guide = 0x0400,
            A = 0x1000,
            B = 0x2000,
            X = 0x4000,
            Y = 0x8000
        }

        internal GamePadState(bool isConnected, RawState rawState, GamePadDeadZone deadZone)
        {
            this.isConnected = isConnected;

            if (!isConnected)
            {
                rawState.dwPacketNumber = 0;
                rawState.Gamepad.wButtons = 0;
                rawState.Gamepad.bLeftTrigger = 0;
                rawState.Gamepad.bRightTrigger = 0;
                rawState.Gamepad.sThumbLX = 0;
                rawState.Gamepad.sThumbLY = 0;
                rawState.Gamepad.sThumbRX = 0;
                rawState.Gamepad.sThumbRY = 0;
            }

            packetNumber = rawState.dwPacketNumber;
            buttons = new GamePadButtons(
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.Start) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.Back) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.LeftThumb) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.RightThumb) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.LeftShoulder) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.RightShoulder) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.Guide) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.A) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.B) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.X) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.Y) != 0 ? ButtonState.Pressed : ButtonState.Released
            );
            dPad = new GamePadDPad(
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.DPadUp) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.DPadDown) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.DPadLeft) != 0 ? ButtonState.Pressed : ButtonState.Released,
                (rawState.Gamepad.wButtons & (uint)ButtonsConstants.DPadRight) != 0 ? ButtonState.Pressed : ButtonState.Released
            );

            thumbSticks = new GamePadThumbSticks(
                Utils.ApplyLeftStickDeadZone(rawState.Gamepad.sThumbLX, rawState.Gamepad.sThumbLY, deadZone),
                Utils.ApplyRightStickDeadZone(rawState.Gamepad.sThumbRX, rawState.Gamepad.sThumbRY, deadZone)
            );
            triggers = new GamePadTriggers(
                Utils.ApplyTriggerDeadZone(rawState.Gamepad.bLeftTrigger, deadZone),
                Utils.ApplyTriggerDeadZone(rawState.Gamepad.bRightTrigger, deadZone)
            );
        }

        public uint PacketNumber
        {
            get { return packetNumber; }
        }

        public bool IsConnected
        {
            get { return isConnected; }
        }

        public GamePadButtons Buttons
        {
            get { return buttons; }
        }

        public GamePadDPad DPad
        {
            get { return dPad; }
        }

        public GamePadTriggers Triggers
        {
            get { return triggers; }
        }

        public GamePadThumbSticks ThumbSticks
        {
            get { return thumbSticks; }
        }
    }

    public enum PlayerIndex
    {
        One = 0,
        Two,
        Three,
        Four
    }

    public enum GamePadDeadZone
    {
        Circular,
        IndependentAxes,
        None
    }

    public class GamePad
    {
        public static GamePadState GetState(PlayerIndex playerIndex)
        {
            return GetState(playerIndex, GamePadDeadZone.Circular);
        }

        public static GamePadState GetState(PlayerIndex playerIndex, GamePadDeadZone deadZone)
        {
            RawState rawState;
            uint result = Imports.XInputGamePadGetState((uint)playerIndex, out rawState);

            try
            {
                Int32[] devices;
                if (!TryGetJoyShockDevicesSnapshot(out devices))
                    return new GamePadState(result == Utils.Success, rawState, deadZone);

                if (devices.Length > 0)
                {
                    foreach (Int32 device in devices)
                    {
                        if (!JSL.StillConnected(device)) continue;

                        // We combine the inputs from all the connected gamepads
                        // Applying deazone before doing so would be better but who has more than one gamepad connected anyway
                        JSL.State jslState = JSL.GetSimpleState(device);
                        if ((jslState.buttons & (int)JSL.Button.Options) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.Start;
                        if ((jslState.buttons & (int)JSL.Button.TouchpadClick) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.Back;
                        if ((jslState.buttons & (int)JSL.Button.Minus) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.Back;
                        if ((jslState.buttons & (int)JSL.Button.ZL) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.LeftThumb;
                        if ((jslState.buttons & (int)JSL.Button.ZR) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.RightThumb;
                        if ((jslState.buttons & (int)JSL.Button.L) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.LeftShoulder;
                        if ((jslState.buttons & (int)JSL.Button.R) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.RightShoulder;
                        if ((jslState.buttons & (int)JSL.Button.PS) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.Guide;
                        if ((jslState.buttons & (int)JSL.Button.S) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.A;
                        if ((jslState.buttons & (int)JSL.Button.E) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.B;
                        if ((jslState.buttons & (int)JSL.Button.W) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.X;
                        if ((jslState.buttons & (int)JSL.Button.N) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.Y;

                        if ((jslState.buttons & (int)JSL.Button.Up) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.DPadUp;
                        if ((jslState.buttons & (int)JSL.Button.Down) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.DPadDown;
                        if ((jslState.buttons & (int)JSL.Button.Left) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.DPadLeft;
                        if ((jslState.buttons & (int)JSL.Button.Right) != 0) rawState.Gamepad.wButtons |= (ushort)ButtonsConstants.DPadRight;

                        rawState.Gamepad.sThumbLX = (short)Utils.Clamp((int)(jslState.stickLX * short.MaxValue + rawState.Gamepad.sThumbLX), short.MinValue, short.MaxValue);
                        rawState.Gamepad.sThumbLY = (short)Utils.Clamp((int)(jslState.stickLY * short.MaxValue + rawState.Gamepad.sThumbLY), short.MinValue, short.MaxValue);

                        rawState.Gamepad.sThumbRX = (short)Utils.Clamp((int)(jslState.stickRX * short.MaxValue + rawState.Gamepad.sThumbRX), short.MinValue, short.MaxValue);
                        rawState.Gamepad.sThumbRY = (short)Utils.Clamp((int)(jslState.stickRY * short.MaxValue + rawState.Gamepad.sThumbRY), short.MinValue, short.MaxValue);

                        rawState.Gamepad.bLeftTrigger = (byte)Utils.Clamp((int)(jslState.lTrigger * byte.MaxValue + rawState.Gamepad.bLeftTrigger), byte.MinValue, byte.MaxValue);
                        rawState.Gamepad.bRightTrigger = (byte)Utils.Clamp((int)(jslState.rTrigger * byte.MaxValue + rawState.Gamepad.bRightTrigger), byte.MinValue, byte.MaxValue);
                    }
                    result = Utils.Success;
                }
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
            return new GamePadState(result == Utils.Success, rawState, deadZone);
        }

        /// <summary>
        /// Refresh the list of connected gamepad devices
        /// </summary>
        /// <param name="forceReconnect">Disconnect all devices before (re)connecting</param>
        /// <returns>true if the number of devices has changed</returns></returns>
        public static Boolean RefreshDevices(Boolean forceReconnect = false)
        {
            RequestRefresh(forceReconnect);
            return ConsumeRefreshResult();
        }

        public static Single Threshold { set; get; }

        public static void SetVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor)
        {
            Imports.XInputGamePadSetState((uint)playerIndex, leftMotor, rightMotor);
            try
            {
                Int32[] devices;
                if (!TryGetJoyShockDevicesSnapshot(out devices))
                    return;

                if (devices.Length > 0)
                {
                    foreach (Int32 device in devices)
                    {
                        if (!JSL.StillConnected(device)) continue;
                        JSL.SetRumble(device, (int)(leftMotor * byte.MaxValue), (int)(rightMotor * byte.MaxValue));
                    }
                }
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
        }

        private static Int32[] jslDevices = new Int32[0];
        private static readonly Object jslSync = new Object();
        private static Boolean discoveryRequested = false;
        private static Boolean refreshInProgress = false;
        private static Boolean refreshRequested = false;
        private static Boolean forceReconnectRequested = false;
        private static Boolean hasRefreshResult = false;
        private static Boolean refreshChanged = false;
        private static long filePosition = 0;

        private static Boolean TryGetJoyShockDevicesSnapshot(out Int32[] devices)
        {
            lock (jslSync)
            {
                if (!discoveryRequested)
                {
                    discoveryRequested = true;
                    refreshRequested = true;
                    StartRefreshWorker_NoLock();
                }

                // JslConnectDevices takes an exclusive lock in JoyShockLibrary.
                // Skip JoyShock polling while refresh is running to guarantee this call stays non-blocking.
                if (refreshInProgress)
                {
                    devices = new Int32[0];
                    return false;
                }

                devices = jslDevices;
                return true;
            }
        }

        private static void RequestRefresh(Boolean forceReconnect)
        {
            lock (jslSync)
            {
                discoveryRequested = true;
                refreshRequested = true;
                if (forceReconnect)
                    forceReconnectRequested = true;

                StartRefreshWorker_NoLock();
            }
        }

        private static void StartRefreshWorker_NoLock()
        {
            if (refreshInProgress)
                return;

            refreshInProgress = true;
            ThreadPool.QueueUserWorkItem(_ => RefreshWorkerLoop());
        }

        private static void RefreshWorkerLoop()
        {
            while (true)
            {
                Boolean mustForceReconnect;

                lock (jslSync)
                {
                    mustForceReconnect = forceReconnectRequested;
                    forceReconnectRequested = false;
                    refreshRequested = false;
                }

                Boolean changed = false;
                Int32[] updatedDevices = new Int32[0];

                try
                {
                    Int32 previousCount;
                    lock (jslSync)
                        previousCount = jslDevices.Length;

                    if (mustForceReconnect)
                        JSL.DisconnectAndDisposeAll();

                    Int32 count = JSL.ConnectDevices();
                    updatedDevices = new Int32[count > 0 ? count : 0];
                    JSL.GetConnectedDeviceHandles(updatedDevices, updatedDevices.Length);
                    changed = updatedDevices.Length != previousCount;
                }
                catch (Exception e)
                {
                    Log(e.ToString());
                }

                lock (jslSync)
                {
                    jslDevices = updatedDevices;
                    hasRefreshResult = true;
                    refreshChanged |= changed;

                    if (refreshRequested)
                        continue;

                    refreshInProgress = false;
                    return;
                }
            }
        }

        private static Boolean ConsumeRefreshResult()
        {
            lock (jslSync)
            {
                if (!hasRefreshResult)
                    return false;

                Boolean hasChanged = refreshChanged;
                refreshChanged = false;
                hasRefreshResult = false;
                return hasChanged;
            }
        }

        private static void Log(string message)
        {
            using (var file = File.OpenWrite("XInpuDotNetPure.log"))
            using (var log = new StreamWriter(file))
            {
                file.Seek(filePosition, SeekOrigin.Begin);
                log.WriteLine(message);
                filePosition = file.Position;
            }
        }
    }
}
