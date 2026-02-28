// The (fixed) behaviour of Control / HonoInputManager / EventInput binding is that:
// HonoInputManager:
// - each Control is binded with at least 2 inputs: a keyboard key + a joystick button
// - there are a couple more for specific Controls (eg. the mouse button or the keyboard "Enter" are binded to Control.Confirm, or "Esc" is binded to Control.Cancel...)
// EventInput:
// - using any of these inputs triggers both the logical signal (eg. EventInput.Confirm) and the physical signal (eg. EventInput.Cross)

// Example: let's say Control.Confirm is binded by the player to the joystick button "Triangle" and the keyboard key "A" (in the Config Menu)
// Then pressing the button Triangle triggers both the event script checks "IsButton(4096)" and "IsButton(131072)", which respectively check for the physical button "Triangle" and the logical button "Confirm"
// The keyboard key's associated physical button also depends on the joystick configuration: pressing "A" triggers the same checks "IsButton(4096)" and "IsButton(131072)"

// [DV] L3 and R3 can be called with KeyCode.JoystickButton10 and KeyCode.JoystickButton11 (eg. Input.GetKeyDown(KeyCode.JoystickButton10) )

public enum Control
{
    Confirm,
    Cancel,
    Menu,
    Special,
    LeftBumper,
    RightBumper,
    LeftTrigger,
    RightTrigger,
    Pause,
    Select,
    Up,
    Down,
    Left,
    Right,
    DPad,
    None
}
