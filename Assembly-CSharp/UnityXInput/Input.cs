// Decompiled with JetBrains decompiler
// Type: UnityXInput.Input
// Assembly: Assembly-CSharp, Version=1.0.6017.29125, Culture=neutral, PublicKeyToken=null
// MVID: 9E13D328-CC20-4729-BE81-A583B120F83A
// Compiler-generated code is shown

using System;
using UnityEngine;
using XInputDotNetPure;

namespace UnityXInput
{
    public static class Input
    {
        public const String horizontalKey = "Horizontal";
        public const String verticalKey = "Vertical";
        public const String leftTrigger = "LeftTrigger";
        public const String rightTrigger = "RightTrigger";

        public static Int32 touchCount
        {
            get
            {
                return UnityEngine.Input.touchCount;
            }
        }

        public static Boolean anyKey
        {
            get
            {
                return UnityEngine.Input.anyKey;
            }
        }

        public static Boolean anyKeyDown
        {
            get
            {
                return UnityEngine.Input.anyKeyDown;
            }
        }

        public static Vector2 mousePosition
        {
            get
            {
                return (Vector2)UnityEngine.Input.mousePosition;
            }
        }

        private static Single GetXAxis(String axisName)
        {
            if (!HonoInputManager.ApplicationIsActivated() || !PersistenSingleton<XInputManager>.Instance.CurrentState.IsConnected)
                return 0.0f;
            if (axisName.Contains("Horizontal"))
            {
                if (PersistenSingleton<XInputManager>.Instance.CurrentState.DPad.Left == ButtonState.Pressed)
                    return -1f;
                if (PersistenSingleton<XInputManager>.Instance.CurrentState.DPad.Right == ButtonState.Pressed)
                    return 1f;
                return PersistenSingleton<XInputManager>.Instance.CurrentState.ThumbSticks.Left.X;
            }
            if (axisName.Contains("Vertical"))
            {
                if (PersistenSingleton<XInputManager>.Instance.CurrentState.DPad.Down == ButtonState.Pressed)
                    return -1f;
                if (PersistenSingleton<XInputManager>.Instance.CurrentState.DPad.Up == ButtonState.Pressed)
                    return 1f;
                return PersistenSingleton<XInputManager>.Instance.CurrentState.ThumbSticks.Left.Y;
            }
            if (axisName.Contains("LeftTrigger"))
                return PersistenSingleton<XInputManager>.Instance.CurrentState.Triggers.Left;
            if (axisName.Contains("RightTrigger"))
                return PersistenSingleton<XInputManager>.Instance.CurrentState.Triggers.Right;
            return 0.0f;
        }

        private static Boolean GetXButton(String keyName)
        {
            if (!HonoInputManager.ApplicationIsActivated())
                return false;
            GamePadState currentState = PersistenSingleton<XInputManager>.Instance.CurrentState;
            GamePadState previousState = PersistenSingleton<XInputManager>.Instance.PreviousState;
            if (!currentState.IsConnected || !previousState.IsConnected)
                return false;
            if (keyName == "JoystickButton0")
            {
                if (currentState.Buttons.A == ButtonState.Pressed)
                    return previousState.Buttons.A == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton1")
            {
                if (currentState.Buttons.B == ButtonState.Pressed)
                    return previousState.Buttons.B == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton2")
            {
                if (currentState.Buttons.X == ButtonState.Pressed)
                    return previousState.Buttons.X == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton3")
            {
                if (currentState.Buttons.Y == ButtonState.Pressed)
                    return previousState.Buttons.Y == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton4")
            {
                if (currentState.Buttons.LeftShoulder == ButtonState.Pressed)
                    return previousState.Buttons.LeftShoulder == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton5")
            {
                if (currentState.Buttons.RightShoulder == ButtonState.Pressed)
                    return previousState.Buttons.RightShoulder == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton6")
            {
                if (currentState.Buttons.Back == ButtonState.Pressed)
                    return previousState.Buttons.Back == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton7")
            {
                if (currentState.Buttons.Start == ButtonState.Pressed)
                    return previousState.Buttons.Start == ButtonState.Pressed;
                return false;
            }
            if (keyName == "LeftTrigger")
                return (Double)currentState.Triggers.Left > 0.75;
            if (keyName == "RightTrigger")
                return (Double)currentState.Triggers.Right > 0.75;
            return false;
        }

        private static Boolean GetXButtonUp(String keyName)
        {
            if (!HonoInputManager.ApplicationIsActivated())
                return false;
            GamePadState currentState = PersistenSingleton<XInputManager>.Instance.CurrentState;
            GamePadState previousState = PersistenSingleton<XInputManager>.Instance.PreviousState;
            if (!currentState.IsConnected || !previousState.IsConnected)
                return false;
            if (keyName == "JoystickButton0")
            {
                if (currentState.Buttons.A == ButtonState.Released)
                    return previousState.Buttons.A == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton1")
            {
                if (currentState.Buttons.B == ButtonState.Released)
                    return previousState.Buttons.B == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton2")
            {
                if (currentState.Buttons.X == ButtonState.Released)
                    return previousState.Buttons.X == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton3")
            {
                if (currentState.Buttons.Y == ButtonState.Released)
                    return previousState.Buttons.Y == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton4")
            {
                if (currentState.Buttons.LeftShoulder == ButtonState.Released)
                    return previousState.Buttons.LeftShoulder == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton5")
            {
                if (currentState.Buttons.RightShoulder == ButtonState.Released)
                    return previousState.Buttons.RightShoulder == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton6")
            {
                if (currentState.Buttons.Back == ButtonState.Released)
                    return previousState.Buttons.Back == ButtonState.Pressed;
                return false;
            }
            if (keyName == "JoystickButton7")
            {
                if (currentState.Buttons.Start == ButtonState.Released)
                    return previousState.Buttons.Start == ButtonState.Pressed;
                return false;
            }
            if (keyName == "LeftTrigger")
            {
                if ((Double)currentState.Triggers.Left == 0.0)
                    return (Double)previousState.Triggers.Left > 0.0;
                return false;
            }
            if (keyName == "RightTrigger" && (Double)currentState.Triggers.Right == 0.0)
                return (Double)previousState.Triggers.Right > 0.0;
            return false;
        }

        private static Boolean GetXButtonDown(String keyName)
        {
            if (!HonoInputManager.ApplicationIsActivated())
                return false;
            GamePadState currentState = PersistenSingleton<XInputManager>.Instance.CurrentState;
            GamePadState previousState = PersistenSingleton<XInputManager>.Instance.PreviousState;
            if (!currentState.IsConnected || !previousState.IsConnected)
                return false;
            if (keyName == "JoystickButton0")
            {
                if (currentState.Buttons.A == ButtonState.Pressed)
                    return previousState.Buttons.A == ButtonState.Released;
                return false;
            }
            if (keyName == "JoystickButton1")
            {
                if (currentState.Buttons.B == ButtonState.Pressed)
                    return previousState.Buttons.B == ButtonState.Released;
                return false;
            }
            if (keyName == "JoystickButton2")
            {
                if (currentState.Buttons.X == ButtonState.Pressed)
                    return previousState.Buttons.X == ButtonState.Released;
                return false;
            }
            if (keyName == "JoystickButton3")
            {
                if (currentState.Buttons.Y == ButtonState.Pressed)
                    return previousState.Buttons.Y == ButtonState.Released;
                return false;
            }
            if (keyName == "JoystickButton4")
            {
                if (currentState.Buttons.LeftShoulder == ButtonState.Pressed)
                    return previousState.Buttons.LeftShoulder == ButtonState.Released;
                return false;
            }
            if (keyName == "JoystickButton5")
            {
                if (currentState.Buttons.RightShoulder == ButtonState.Pressed)
                    return previousState.Buttons.RightShoulder == ButtonState.Released;
                return false;
            }
            if (keyName == "JoystickButton6")
            {
                if (currentState.Buttons.Back == ButtonState.Pressed)
                    return previousState.Buttons.Back == ButtonState.Released;
                return false;
            }
            if (keyName == "JoystickButton7")
            {
                if (currentState.Buttons.Start == ButtonState.Pressed)
                    return previousState.Buttons.Start == ButtonState.Released;
                return false;
            }
            if (keyName == "LeftTrigger")
            {
                if ((Double)currentState.Triggers.Left > 0.0)
                    return (Double)previousState.Triggers.Left == 0.0;
                return false;
            }
            if (keyName == "RightTrigger" && (Double)currentState.Triggers.Right > 0.0)
                return (Double)previousState.Triggers.Right == 0.0;
            return false;
        }

        public static Single GetAxis(String axisName)
        {
            return Input.GetAxisRaw(axisName);
        }

        public static Single GetAxisRaw(String axisName)
        {
            float axisRaw = UnityEngine.Input.GetAxisRaw(axisName);
            axisRaw += Input.GetXAxis(axisName);
            return Mathf.Clamp(axisRaw, -1f, 1f);
        }

        public static Boolean GetButtonUp(String keyName)
        {
            return Input.GetXButtonUp(keyName);
        }

        public static Boolean GetButtonDown(String keyName)
        {
            return Input.GetXButtonDown(keyName);
        }

        public static Boolean GetButton(String keyName)
        {
            return Input.GetXButton(keyName);
        }

        public static Boolean GetKeyUp(KeyCode keyName)
        {
            return UnityEngine.Input.GetKeyUp(keyName);
        }

        public static Boolean GetKeyDown(KeyCode keyName)
        {
            return UnityEngine.Input.GetKeyDown(keyName);
        }

        public static Boolean GetKey(KeyCode keyName)
        {
            return UnityEngine.Input.GetKey(keyName);
        }

        public static Boolean GetMouseButtonUp(Int32 index)
        {
            return UnityEngine.Input.GetMouseButtonUp(index);
        }

        public static Boolean GetMouseButtonDown(Int32 index)
        {
            return UnityEngine.Input.GetMouseButtonDown(index);
        }

        public static Boolean GetMouseButton(Int32 index)
        {
            return UnityEngine.Input.GetMouseButton(index);
        }

        public static UnityEngine.Touch GetTouch(Int32 touchIndex)
        {
            return UnityEngine.Input.GetTouch(touchIndex);
        }

        public static String[] GetJoystickNames()
        {
            return UnityEngine.Input.GetJoystickNames();
        }
    }
}
