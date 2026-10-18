using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Memoria.Launcher.Controller
{
    /// <summary>
    /// Owns the visual and keyboard focus used while controller input is active.
    /// </summary>
    internal sealed class ControllerFocusManager : IDisposable
    {
        private ControllerFocusAdorner _adorner;
        private Control _focusVisualOwner;
        private Object _previousFocusVisualStyle;
        private Boolean _controllerAppearanceActive;

        public ControllerFocusManager(Window window)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));
        }

        public event Action<Control> FocusChanging;
        public event Action<Control> Focused;

        public void Focus(Control control)
        {
            if (control == null)
                return;

            if (!control.Focusable && GamepadNavigation.GetParticipation(control) == NavigationParticipation.Include)
                control.Focusable = true;

            ApplyControllerFocusVisualStyle(control);
            if (!control.Focus())
            {
                RestoreFocusVisualStyle();
                return;
            }

            FocusChanging?.Invoke(control);
            control.BringIntoView();
            ShowControllerAppearance(control, false);
            Focused?.Invoke(control);
        }

        public void EnsureControllerAppearance(Control control, Boolean isEditing)
        {
            if (_controllerAppearanceActive && ReferenceEquals(_focusVisualOwner, control))
                return;

            ApplyControllerFocusVisualStyle(control);
            ShowControllerAppearance(control, isEditing);
        }

        public void ShowControllerAppearance(Control control, Boolean isEditing)
        {
            RemoveAdorner();
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);
            if (layer != null)
            {
                _adorner = new ControllerFocusAdorner(control, isEditing);
                layer.Add(_adorner);
            }

            _controllerAppearanceActive = true;
        }

        public void DeactivateControllerAppearance(Boolean clearKeyboardFocus)
        {
            RemoveAdorner();
            RestoreFocusVisualStyle();
            _controllerAppearanceActive = false;
            if (clearKeyboardFocus)
                Keyboard.ClearFocus();
        }

        public void Dispose()
        {
            DeactivateControllerAppearance(false);
            FocusChanging = null;
            Focused = null;
        }

        private void ApplyControllerFocusVisualStyle(Control control)
        {
            if (ReferenceEquals(_focusVisualOwner, control))
                return;

            RestoreFocusVisualStyle();
            _focusVisualOwner = control;
            _previousFocusVisualStyle = control.ReadLocalValue(FrameworkElement.FocusVisualStyleProperty);
            control.SetValue(FrameworkElement.FocusVisualStyleProperty, null);
        }

        private void RestoreFocusVisualStyle()
        {
            if (_focusVisualOwner == null)
                return;

            if (_previousFocusVisualStyle == DependencyProperty.UnsetValue)
                _focusVisualOwner.ClearValue(FrameworkElement.FocusVisualStyleProperty);
            else
                _focusVisualOwner.SetValue(FrameworkElement.FocusVisualStyleProperty, _previousFocusVisualStyle);

            _focusVisualOwner = null;
            _previousFocusVisualStyle = null;
        }

        private void RemoveAdorner()
        {
            if (_adorner == null)
                return;

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(_adorner.AdornedElement);
            if (layer != null)
                layer.Remove(_adorner);
            _adorner = null;
        }
    }

    internal sealed class ControllerFocusAdorner : Adorner
    {
        private readonly Pen _pen;

        public ControllerFocusAdorner(UIElement adornedElement, Boolean isEditing)
            : base(adornedElement)
        {
            IsHitTestVisible = false;
            _pen = new Pen(isEditing ? Brushes.Gold : Brushes.DeepSkyBlue, isEditing ? 4.0 : 3.0);
            _pen.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Double inset = _pen.Thickness / 2.0;
            Rect bounds = new Rect(
                inset,
                inset,
                Math.Max(0.0, AdornedElement.RenderSize.Width - _pen.Thickness),
                Math.Max(0.0, AdornedElement.RenderSize.Height - _pen.Thickness));
            drawingContext.DrawRoundedRectangle(null, _pen, bounds, 4.0, 4.0);
        }
    }
}
