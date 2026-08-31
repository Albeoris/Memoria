using System;
using System.Collections.Generic;

namespace Memoria.Launcher.Controller
{
    [Flags]
    internal enum ControllerButton
    {
        None = 0,
        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        Confirm = 1 << 4,
        Cancel = 1 << 5,
        PreviousTab = 1 << 6,
        NextTab = 1 << 7,
        PreviousRootTab = 1 << 8,
        NextRootTab = 1 << 9,
        ToggleTooltip = 1 << 10,
        SubmitTextInput = 1 << 11
    }

    internal struct ControllerState
    {
        public ControllerState(ControllerButton buttons)
        {
            Buttons = buttons;
        }

        public ControllerButton Buttons { get; }
    }

    internal interface IControllerInputSource : IDisposable
    {
        Boolean TryGetState(out ControllerState state);
    }

    /// <summary>
    /// Converts polled controller state into edge-triggered actions and applies
    /// console-like key repeat only to directional navigation.
    /// </summary>
    internal sealed class ControllerButtonRepeater
    {
        private static readonly ControllerButton[] RepeatableButtons =
        {
            ControllerButton.Up,
            ControllerButton.Down,
            ControllerButton.Left,
            ControllerButton.Right
        };

        private readonly TimeSpan _initialDelay;
        private readonly TimeSpan _repeatInterval;
        private readonly Dictionary<ControllerButton, TimeSpan> _nextRepeat = new Dictionary<ControllerButton, TimeSpan>();
        private ControllerButton _previous;

        public ControllerButtonRepeater(TimeSpan initialDelay, TimeSpan repeatInterval)
        {
            if (initialDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(initialDelay));
            if (repeatInterval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(repeatInterval));

            _initialDelay = initialDelay;
            _repeatInterval = repeatInterval;
        }

        public ControllerButton Update(ControllerButton current, TimeSpan now)
        {
            ControllerButton actions = current & ~_previous;

            foreach (ControllerButton button in RepeatableButtons)
            {
                Boolean isDown = (current & button) != 0;
                Boolean wasDown = (_previous & button) != 0;
                if (!isDown)
                {
                    _nextRepeat.Remove(button);
                    continue;
                }

                if (!wasDown)
                {
                    _nextRepeat[button] = now + _initialDelay;
                    continue;
                }

                TimeSpan next;
                if (!_nextRepeat.TryGetValue(button, out next) || now < next)
                    continue;

                actions |= button;
                do
                {
                    next += _repeatInterval;
                } while (next <= now);
                _nextRepeat[button] = next;
            }

            _previous = current;
            return actions;
        }

        public void Reset()
        {
            _previous = ControllerButton.None;
            _nextRepeat.Clear();
        }
    }
}
