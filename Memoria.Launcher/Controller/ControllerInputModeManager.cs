using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Memoria.Launcher.Controller
{
    /// <summary>
    /// Switches between mouse/keyboard presentation and controller presentation.
    /// </summary>
    internal sealed class ControllerInputModeManager : IDisposable
    {
        private readonly Window _window;
        private readonly ControllerFocusManager _focus;
        private readonly ControllerControlInteractor _interaction;
        private readonly ControllerTooltipPresenter _tooltips;
        private Border _mouseInputShield;
        private NativePointerPosition _controllerPointerPosition;
        private Boolean _hasControllerPointerPosition;
        private Cursor _cursorBeforeControllerMode;
        private Boolean _controllerInputActive;
        private Boolean _disposed;

        public ControllerInputModeManager(
            Window window,
            ControllerFocusManager focus,
            ControllerControlInteractor interaction,
            ControllerTooltipPresenter tooltips)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _focus = focus ?? throw new ArgumentNullException(nameof(focus));
            _interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
            _tooltips = tooltips ?? throw new ArgumentNullException(nameof(tooltips));

            _window.PreviewMouseDown += OnPreviewMouseDown;
            _window.PreviewMouseMove += OnPreviewMouseMove;
            _window.PreviewMouseWheel += OnPreviewMouseWheel;
            InputManager.Current.PreProcessInput += OnPreProcessInput;
        }

        public void EnterControllerMode()
        {
            if (_controllerInputActive)
            {
                Mouse.OverrideCursor = Cursors.None;
                return;
            }

            _cursorBeforeControllerMode = Mouse.OverrideCursor;
            _controllerInputActive = true;
            GamepadNavigation.SetIsControllerInputActive(_window, true);
            _hasControllerPointerPosition = NativePointer.TryGetPosition(out _controllerPointerPosition);
            Mouse.OverrideCursor = Cursors.None;
            _tooltips.CloseAutomaticTooltips();
            InstallMouseInputShield();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _window.PreviewMouseDown -= OnPreviewMouseDown;
            _window.PreviewMouseMove -= OnPreviewMouseMove;
            _window.PreviewMouseWheel -= OnPreviewMouseWheel;
            InputManager.Current.PreProcessInput -= OnPreProcessInput;
            EnterMouseMode();
        }

        private void OnPreviewMouseDown(Object sender, MouseButtonEventArgs e)
        {
            _interaction.EndSliderEditing();
            EnterMouseMode();
        }

        private void OnPreviewMouseMove(Object sender, MouseEventArgs e)
        {
            if (_controllerInputActive)
                RestoreMouseModeAfterPhysicalMovement();
        }

        private void OnPreviewMouseWheel(Object sender, MouseWheelEventArgs e) => EnterMouseMode();

        private void OnPreProcessInput(Object sender, PreProcessInputEventArgs e)
        {
            InputEventArgs input = e.StagingItem.Input;
            if (input is KeyEventArgs key && key.IsDown)
            {
                _tooltips.Disable();
                _focus.DeactivateControllerAppearance(false);
                return;
            }

            if (!_controllerInputActive)
                return;

            if (input is MouseButtonEventArgs || input is MouseWheelEventArgs)
            {
                EnterMouseMode();
                return;
            }

            if (input is MouseEventArgs)
                RestoreMouseModeAfterPhysicalMovement();
        }

        private void RestoreMouseModeAfterPhysicalMovement()
        {
            if (!_hasControllerPointerPosition ||
                !NativePointer.TryGetPosition(out NativePointerPosition position) ||
                !position.Equals(_controllerPointerPosition))
            {
                EnterMouseMode();
            }
        }

        private void EnterMouseMode()
        {
            _tooltips.Disable();
            if (!_controllerInputActive)
                return;

            _focus.DeactivateControllerAppearance(true);
            _controllerInputActive = false;
            GamepadNavigation.SetIsControllerInputActive(_window, false);
            _hasControllerPointerPosition = false;
            _interaction.RestorePointerInput();
            RemoveMouseInputShield();
            Mouse.OverrideCursor = _cursorBeforeControllerMode;
            _cursorBeforeControllerMode = null;
        }

        private void InstallMouseInputShield()
        {
            if (_window.Content is not Panel root || _mouseInputShield != null)
                return;

            _mouseInputShield = new Border
            {
                Background = Brushes.Transparent,
                Cursor = Cursors.None,
                ForceCursor = true,
                Focusable = false,
                IsHitTestVisible = true
            };
            Panel.SetZIndex(_mouseInputShield, Int32.MaxValue);
            root.Children.Add(_mouseInputShield);
        }

        private void RemoveMouseInputShield()
        {
            if (_mouseInputShield?.Parent is Panel parent)
                parent.Children.Remove(_mouseInputShield);
            _mouseInputShield = null;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativePointerPosition : IEquatable<NativePointerPosition>
    {
        public Int32 X;
        public Int32 Y;

        public Boolean Equals(NativePointerPosition other) => X == other.X && Y == other.Y;
        public override Boolean Equals(Object obj) => obj is NativePointerPosition other && Equals(other);

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }

    internal static class NativePointer
    {
        public static Boolean TryGetPosition(out NativePointerPosition position) => GetCursorPos(out position);

        [DllImport("user32.dll")]
        private static extern Boolean GetCursorPos(out NativePointerPosition point);
    }
}
