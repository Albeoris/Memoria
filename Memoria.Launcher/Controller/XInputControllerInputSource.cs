using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Memoria.Launcher.Controller
{
    /// <summary>
    /// Small dependency-free XInput adapter. Steam Input and most Windows
    /// controller drivers expose their devices through this API as well.
    /// </summary>
    internal sealed class XInputControllerInputSource : IControllerInputSource
    {
        private const Int32 ErrorSuccess = 0;
        private const Int16 ThumbDeadZone = 16000;
        private const Byte TriggerThreshold = 30;

        private readonly XInputGetState _getState;
        private Int32 _preferredPlayer = -1;

        public XInputControllerInputSource()
        {
            _getState = LoadGetState();
        }

        public Boolean TryGetState(out ControllerState state)
        {
            XInputState nativeState;
            if (_preferredPlayer >= 0 && TryReadPlayer(_preferredPlayer, out nativeState))
            {
                state = Convert(nativeState.Gamepad);
                return true;
            }

            for (Int32 player = 0; player < 4; player++)
            {
                if (!TryReadPlayer(player, out nativeState))
                    continue;

                _preferredPlayer = player;
                state = Convert(nativeState.Gamepad);
                return true;
            }

            _preferredPlayer = -1;
            state = new ControllerState(ControllerButton.None);
            return false;
        }

        public void Dispose()
        {
            // The native module intentionally remains loaded for the process lifetime.
        }

        private Boolean TryReadPlayer(Int32 player, out XInputState state)
        {
            return _getState((UInt32)player, out state) == ErrorSuccess;
        }

        private static ControllerState Convert(XInputGamepad gamepad)
        {
            ControllerButton buttons = ControllerButton.None;
            XInputButton nativeButtons = (XInputButton)gamepad.Buttons;

            if ((nativeButtons & XInputButton.DPadUp) != 0 || gamepad.ThumbLeftY > ThumbDeadZone)
                buttons |= ControllerButton.Up;
            if ((nativeButtons & XInputButton.DPadDown) != 0 || gamepad.ThumbLeftY < -ThumbDeadZone)
                buttons |= ControllerButton.Down;
            if ((nativeButtons & XInputButton.DPadLeft) != 0 || gamepad.ThumbLeftX < -ThumbDeadZone)
                buttons |= ControllerButton.Left;
            if ((nativeButtons & XInputButton.DPadRight) != 0 || gamepad.ThumbLeftX > ThumbDeadZone)
                buttons |= ControllerButton.Right;
            if ((nativeButtons & XInputButton.A) != 0)
                buttons |= ControllerButton.Confirm;
            if ((nativeButtons & XInputButton.B) != 0)
                buttons |= ControllerButton.Cancel;
            if ((nativeButtons & XInputButton.X) != 0)
                buttons |= ControllerButton.ToggleTooltip;
            if ((nativeButtons & XInputButton.Start) != 0)
                buttons |= ControllerButton.SubmitTextInput;
            if ((nativeButtons & XInputButton.LeftShoulder) != 0)
                buttons |= ControllerButton.PreviousTab;
            if ((nativeButtons & XInputButton.RightShoulder) != 0)
                buttons |= ControllerButton.NextTab;
            if (gamepad.LeftTrigger > TriggerThreshold)
                buttons |= ControllerButton.PreviousRootTab;
            if (gamepad.RightTrigger > TriggerThreshold)
                buttons |= ControllerButton.NextRootTab;

            return new ControllerState(buttons);
        }

        private static XInputGetState LoadGetState()
        {
            String[] candidates = { "xinput1_4.dll", "xinput1_3.dll", "xinput9_1_0.dll" };
            foreach (String candidate in candidates)
            {
                IntPtr module = LoadLibrary(candidate);
                if (module == IntPtr.Zero)
                    continue;

                IntPtr address = GetProcAddress(module, "XInputGetState");
                if (address != IntPtr.Zero)
                    return (XInputGetState)Marshal.GetDelegateForFunctionPointer(address, typeof(XInputGetState));
            }

            throw new Win32Exception("No supported XInput library is available.");
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 XInputGetState(UInt32 playerIndex, out XInputState state);

        [StructLayout(LayoutKind.Sequential)]
        private struct XInputState
        {
            public UInt32 PacketNumber;
            public XInputGamepad Gamepad;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XInputGamepad
        {
            public UInt16 Buttons;
            public Byte LeftTrigger;
            public Byte RightTrigger;
            public Int16 ThumbLeftX;
            public Int16 ThumbLeftY;
            public Int16 ThumbRightX;
            public Int16 ThumbRightY;
        }

        [Flags]
        private enum XInputButton : UInt16
        {
            DPadUp = 0x0001,
            DPadDown = 0x0002,
            DPadLeft = 0x0004,
            DPadRight = 0x0008,
            Start = 0x0010,
            LeftShoulder = 0x0100,
            RightShoulder = 0x0200,
            A = 0x1000,
            B = 0x2000,
            X = 0x4000
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(String fileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr module, String procedureName);
    }
}
