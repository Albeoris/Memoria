using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace Memoria.Launcher.Controller
{
    /// <summary>
    /// Applies controller actions to standard WPF controls and owns temporary
    /// editing sessions such as an open ComboBox or an active Slider.
    /// </summary>
    internal sealed class ControllerControlInteractor : IDisposable
    {
        private readonly Window _window;
        private readonly ControllerFocusManager _focus;
        private ComboBox _openComboBox;
        private Int32 _openComboBoxInitialIndex;
        private UIElement _suppressedComboBoxPopupContent;
        private Boolean _popupContentWasHitTestVisible;
        private Slider _activeSlider;

        public ControllerControlInteractor(Window window, ControllerFocusManager focus)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _focus = focus ?? throw new ArgumentNullException(nameof(focus));
            _focus.FocusChanging += OnFocusChanging;
        }

        public Boolean IsEditing(Control control) => ReferenceEquals(control, _activeSlider);

        public Control GetFocusedControlOverride(FrameworkElement scope)
        {
            return _openComboBox != null && _openComboBox.IsDropDownOpen &&
                   VisualTree.IsDescendantOf(_openComboBox, scope)
                ? _openComboBox
                : null;
        }

        public Boolean HandleEditingAction(ControllerButton actions)
        {
            if (_activeSlider == null)
                return false;
            if (!_activeSlider.IsVisible || !_activeSlider.IsEnabled)
            {
                _activeSlider = null;
                return false;
            }

            if ((actions & (ControllerButton.Confirm | ControllerButton.Cancel)) != 0)
                EndSliderEditing();
            else if ((actions & ControllerButton.Left) != 0)
                AdjustSlider(_activeSlider, -1);
            else if ((actions & ControllerButton.Right) != 0)
                AdjustSlider(_activeSlider, 1);

            // Slider editing captures all directional actions until A or B.
            return true;
        }

        public Boolean TryHandleDirection(Control current, NavigationDirection direction)
        {
            if (current is ComboBox comboBox && comboBox.IsDropDownOpen &&
                (direction == NavigationDirection.Up || direction == NavigationDirection.Down))
            {
                ChangeSelection(comboBox, direction == NavigationDirection.Up ? -1 : 1);
                return true;
            }

            if (current is ListBox list &&
                (direction == NavigationDirection.Up || direction == NavigationDirection.Down))
            {
                Int32 nextIndex = list.SelectedIndex < 0
                    ? direction == NavigationDirection.Up ? list.Items.Count - 1 : 0
                    : list.SelectedIndex + (direction == NavigationDirection.Up ? -1 : 1);
                if (nextIndex < 0 || nextIndex >= list.Items.Count)
                    return false;

                list.SelectedIndex = nextIndex;
                list.ScrollIntoView(list.SelectedItem);
                return true;
            }

            if (current is RichTextBox richText && richText.IsReadOnly &&
                (direction == NavigationDirection.Up || direction == NavigationDirection.Down))
            {
                ScrollViewer viewer = VisualTree.FindDescendant<ScrollViewer>(richText);
                if (viewer == null)
                    return false;

                if (direction == NavigationDirection.Up)
                    viewer.LineUp();
                else
                    viewer.LineDown();
                return true;
            }

            return false;
        }

        public void Activate(Control current)
        {
            if (GamepadNavigation.RaiseActivated(current))
                return;

            if (current is Slider slider)
            {
                _activeSlider = slider;
                _focus.ShowControllerAppearance(slider, true);
                return;
            }

            if (current is TextBox textBox && !textBox.IsReadOnly)
            {
                if (_window.Content is Panel root)
                    root.Children.Add(new ControllerTextInputOverlay(textBox));
                return;
            }

            if (current is ComboBox comboBox)
            {
                ToggleComboBox(comboBox);
                return;
            }

            if (current is TabItem tab)
            {
                tab.IsSelected = true;
                return;
            }

            if (current is ToggleButton toggle)
            {
                ControllerControlInvoker.Toggle(toggle);
                return;
            }

            if (current is Button button)
                ControllerControlInvoker.Invoke(button);
        }

        public Boolean TryCancel()
        {
            if (_activeSlider != null)
            {
                EndSliderEditing();
                return true;
            }

            if (_openComboBox == null || !_openComboBox.IsDropDownOpen)
                return false;

            _openComboBox.SelectedIndex = _openComboBoxInitialIndex;
            _openComboBox.IsDropDownOpen = false;
            RestoreComboBoxPointerInput();
            _openComboBox = null;
            return true;
        }

        public void EndSliderEditing()
        {
            if (_activeSlider == null)
                return;

            Slider slider = _activeSlider;
            _activeSlider = null;
            _focus.ShowControllerAppearance(slider, false);
        }

        public void RestorePointerInput() => RestoreComboBoxPointerInput();

        public void Dispose()
        {
            _focus.FocusChanging -= OnFocusChanging;
            RestoreComboBoxPointerInput();
            _activeSlider = null;
            _openComboBox = null;
        }

        private void OnFocusChanging(Control control)
        {
            if (_openComboBox != null && !ReferenceEquals(_openComboBox, control))
            {
                _openComboBox.IsDropDownOpen = false;
                RestoreComboBoxPointerInput();
                _openComboBox = null;
            }

            if (_activeSlider != null && !ReferenceEquals(_activeSlider, control))
                _activeSlider = null;

            if (control is ListBox list && list.SelectedIndex < 0 && list.Items.Count > 0)
            {
                list.SelectedIndex = 0;
                list.ScrollIntoView(list.SelectedItem);
            }
        }

        private void ToggleComboBox(ComboBox comboBox)
        {
            if (!comboBox.IsDropDownOpen)
            {
                _openComboBox = comboBox;
                _openComboBoxInitialIndex = comboBox.SelectedIndex;
            }

            comboBox.IsDropDownOpen = !comboBox.IsDropDownOpen;
            if (!comboBox.IsDropDownOpen)
            {
                RestoreComboBoxPointerInput();
                _openComboBox = null;
                return;
            }

            comboBox.Dispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new Action(() => SuppressComboBoxPointerInput(comboBox)));
        }

        private static void AdjustSlider(Slider slider, Int32 direction)
        {
            Double change = slider.TickFrequency > 0.0 ? slider.TickFrequency : slider.SmallChange;
            if (change <= 0.0)
                change = Math.Max((slider.Maximum - slider.Minimum) / 20.0, 1.0);
            slider.Value = Math.Max(slider.Minimum, Math.Min(slider.Maximum, slider.Value + direction * change));
        }

        private static void ChangeSelection(Selector selector, Int32 offset)
        {
            if (selector.Items.Count == 0)
                return;

            Int32 index = selector.SelectedIndex < 0 ? 0 : selector.SelectedIndex + offset;
            selector.SelectedIndex = Math.Max(0, Math.Min(selector.Items.Count - 1, index));
        }

        private void SuppressComboBoxPointerInput(ComboBox comboBox)
        {
            if (!GamepadNavigation.IsControllerInputActive(_window) || !comboBox.IsDropDownOpen)
                return;

            comboBox.ApplyTemplate();
            UIElement content = comboBox.Template.FindName("PART_Popup", comboBox) is Popup popup
                ? popup.Child
                : null;
            if (content == null)
                return;

            RestoreComboBoxPointerInput();
            _suppressedComboBoxPopupContent = content;
            _popupContentWasHitTestVisible = content.IsHitTestVisible;
            content.IsHitTestVisible = false;
            Mouse.OverrideCursor = Cursors.None;
        }

        private void RestoreComboBoxPointerInput()
        {
            if (_suppressedComboBoxPopupContent == null)
                return;

            _suppressedComboBoxPopupContent.IsHitTestVisible = _popupContentWasHitTestVisible;
            _suppressedComboBoxPopupContent = null;
        }
    }

    internal static class ControllerControlInvoker
    {
        public static void Invoke(Button button)
        {
            if (new ButtonAutomationPeer(button).GetPattern(PatternInterface.Invoke) is IInvokeProvider provider)
                provider.Invoke();
            else
                button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
        }

        public static void Toggle(ToggleButton button)
        {
            if (new ToggleButtonAutomationPeer(button).GetPattern(PatternInterface.Toggle) is IToggleProvider provider)
                provider.Toggle();
            else
                button.IsChecked = button.IsChecked != true;
        }
    }
}
