using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Memoria.Launcher.Controller
{
    /// <summary>
    /// Displays the focused control's tooltip while controller help mode is enabled.
    /// </summary>
    internal sealed class ControllerTooltipPresenter : IDisposable
    {
        private readonly Window _window;
        private readonly ControllerFocusManager _focus;
        private ToolTip _toolTip;
        private FrameworkElement _owner;
        private Boolean _enabled;
        private Boolean _reusedExistingToolTip;
        private PlacementMode _previousPlacement;
        private UIElement _previousTarget;
        private Double _previousHorizontalOffset;
        private Double _previousVerticalOffset;
        private Boolean _previousStaysOpen;

        public ControllerTooltipPresenter(Window window, ControllerFocusManager focus)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _focus = focus ?? throw new ArgumentNullException(nameof(focus));
            _focus.Focused += OnFocused;
        }

        public void Toggle(Control current)
        {
            if (_enabled)
            {
                Disable();
                return;
            }

            _enabled = true;
            Show(current);
        }

        public void Disable()
        {
            _enabled = false;
            Close();
        }

        public void CloseAutomaticTooltips()
        {
            foreach (FrameworkElement element in VisualTree.Enumerate<FrameworkElement>(_window))
            {
                if (ToolTipService.GetToolTip(element) is ToolTip toolTip && toolTip.IsOpen)
                    toolTip.IsOpen = false;
            }
        }

        public void Dispose()
        {
            _focus.Focused -= OnFocused;
            Disable();
        }

        private void OnFocused(Control control)
        {
            if (_enabled)
                Show(control);
        }

        private void Show(Control current)
        {
            FrameworkElement owner = ResolveOwner(current);
            if (owner == null)
            {
                Close();
                return;
            }

            if (ReferenceEquals(_owner, owner) && _toolTip?.IsOpen == true)
                return;

            Close();
            Object value = ToolTipService.GetToolTip(owner);
            ToolTip toolTip = value as ToolTip;
            _reusedExistingToolTip = toolTip != null;
            if (toolTip == null)
            {
                toolTip = new ToolTip { Content = value };
            }
            else
            {
                _previousPlacement = toolTip.Placement;
                _previousTarget = toolTip.PlacementTarget;
                _previousHorizontalOffset = toolTip.HorizontalOffset;
                _previousVerticalOffset = toolTip.VerticalOffset;
                _previousStaysOpen = toolTip.StaysOpen;
            }

            _toolTip = toolTip;
            _owner = owner;
            toolTip.PlacementTarget = owner;
            toolTip.Placement = PlacementMode.RelativePoint;
            toolTip.HorizontalOffset = Math.Max(0.0, owner.ActualWidth - 1.0);
            toolTip.VerticalOffset = Math.Max(0.0, owner.ActualHeight - 1.0);
            toolTip.StaysOpen = true;
            toolTip.IsOpen = true;
        }

        private static FrameworkElement ResolveOwner(Control current)
        {
            if (ToolTipService.GetToolTip(current) != null)
                return current;

            FrameworkElement configuredOwner = GamepadNavigation.GetTooltipOwner(current);
            if (configuredOwner != null && ToolTipService.GetToolTip(configuredOwner) != null)
                return configuredOwner;
            return null;
        }

        private void Close()
        {
            if (_toolTip == null)
                return;

            _toolTip.IsOpen = false;
            if (_reusedExistingToolTip)
            {
                _toolTip.Placement = _previousPlacement;
                _toolTip.PlacementTarget = _previousTarget;
                _toolTip.HorizontalOffset = _previousHorizontalOffset;
                _toolTip.VerticalOffset = _previousVerticalOffset;
                _toolTip.StaysOpen = _previousStaysOpen;
            }

            _toolTip = null;
            _owner = null;
            _reusedExistingToolTip = false;
        }
    }
}
