using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Memoria.Launcher.Controller
{
    /// <summary>
    /// Attached metadata used by the generic controller navigation service.
    /// </summary>
    public static class GamepadNavigation
    {
        public static Boolean IsControllerInputActive { get; internal set; }

        public static readonly DependencyProperty IsDefaultFocusProperty = DependencyProperty.RegisterAttached(
            "IsDefaultFocus",
            typeof(Boolean),
            typeof(GamepadNavigation),
            new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsModalScopeProperty = DependencyProperty.RegisterAttached(
            "IsModalScope",
            typeof(Boolean),
            typeof(GamepadNavigation),
            new FrameworkPropertyMetadata(false));

        public static void SetIsDefaultFocus(DependencyObject element, Boolean value) => element.SetValue(IsDefaultFocusProperty, value);
        public static Boolean GetIsDefaultFocus(DependencyObject element) => (Boolean)element.GetValue(IsDefaultFocusProperty);
        public static void SetIsModalScope(DependencyObject element, Boolean value) => element.SetValue(IsModalScopeProperty, value);
        public static Boolean GetIsModalScope(DependencyObject element) => (Boolean)element.GetValue(IsModalScopeProperty);
    }

    /// <summary>
    /// Coordinates controller input, spatial focus navigation and standard WPF
    /// control actions without coupling individual launcher screens to XInput.
    /// </summary>
    internal sealed class GamepadNavigationService : IDisposable
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(16);
        private static readonly TimeSpan InitialRepeatDelay = TimeSpan.FromMilliseconds(350);
        private static readonly TimeSpan RepeatInterval = TimeSpan.FromMilliseconds(90);

        private readonly Window _window;
        private readonly IControllerInputSource _input;
        private readonly ControllerButtonRepeater _repeater;
        private readonly DispatcherTimer _timer;
        private readonly Stopwatch _clock = Stopwatch.StartNew();

        private ControllerFocusAdorner _focusAdorner;
        private Control _focusVisualOwner;
        private Object _focusVisualPreviousLocalValue;
        private Boolean _controllerFocusAppearanceActive;
        private ComboBox _openComboBox;
        private Int32 _openComboBoxInitialIndex;
        private UIElement _suppressedComboBoxPopupContent;
        private Boolean _popupContentWasHitTestVisible;
        private Slider _activeSlider;
        private Border _mouseInputShield;
        private NativePointerPosition _controllerPointerPosition;
        private Boolean _hasControllerPointerPosition;
        private Cursor _cursorBeforeControllerMode;
        private Boolean _controllerInputActive;
        private ToolTip _controllerToolTip;
        private FrameworkElement _controllerToolTipOwner;
        private Boolean _controllerTooltipsEnabled;
        private Boolean _controllerToolTipReused;
        private PlacementMode _tooltipPreviousPlacement;
        private UIElement _tooltipPreviousTarget;
        private Double _tooltipPreviousHorizontalOffset;
        private Double _tooltipPreviousVerticalOffset;
        private Boolean _tooltipPreviousStaysOpen;
        private Boolean _disposed;

        private GamepadNavigationService(Window window, IControllerInputSource input)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _repeater = new ControllerButtonRepeater(InitialRepeatDelay, RepeatInterval);
            _timer = new DispatcherTimer(DispatcherPriority.Input, window.Dispatcher)
            {
                Interval = PollInterval
            };
            _timer.Tick += OnTick;
            _window.PreviewMouseDown += OnPreviewMouseDown;
            _window.PreviewMouseMove += OnPreviewMouseMove;
            _window.PreviewMouseWheel += OnPreviewMouseWheel;
            InputManager.Current.PreProcessInput += OnPreProcessInput;
            _window.Closed += OnWindowClosed;
            _timer.Start();
        }

        public static IDisposable Attach(Window window)
        {
            try
            {
                return new GamepadNavigationService(window, new XInputControllerInputSource());
            }
            catch (Exception exception)
            {
                AppLogger.GetLogger().Warn(exception, "Controller navigation could not be initialized.");
                return EmptyDisposable.Instance;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _timer.Stop();
            _timer.Tick -= OnTick;
            _window.PreviewMouseDown -= OnPreviewMouseDown;
            _window.PreviewMouseMove -= OnPreviewMouseMove;
            _window.PreviewMouseWheel -= OnPreviewMouseWheel;
            InputManager.Current.PreProcessInput -= OnPreProcessInput;
            _window.Closed -= OnWindowClosed;
            EnterMouseMode();
            RemoveFocusAdorner();
            _input.Dispose();
        }

        private void OnTick(Object sender, EventArgs e)
        {
            if (_window.WindowState == WindowState.Minimized)
            {
                _repeater.Reset();
                return;
            }

            Boolean nativeDialogActive = !_window.IsActive && NativeDialogControllerBridge.IsCurrentProcessInForeground();
            if (!_window.IsActive && !nativeDialogActive)
            {
                _repeater.Reset();
                return;
            }

            ControllerState state;
            if (!_input.TryGetState(out state))
            {
                _repeater.Reset();
                return;
            }

            ControllerButton actions = _repeater.Update(state.Buttons, _clock.Elapsed);
            if (actions == ControllerButton.None)
                return;

            EnterControllerMode();

            if (nativeDialogActive)
            {
                NativeDialogControllerBridge.Send(actions);
                return;
            }

            FrameworkElement scope = FindActiveScope();
            if (scope is ControllerTextInputOverlay textInput && (actions & ControllerButton.SubmitTextInput) != 0)
            {
                textInput.Accept();
                return;
            }

            Control current = FindFocusedControl(scope);
            if (current == null)
            {
                FocusInitialControl(scope);
                current = FindFocusedControl(scope);
                if (current == null)
                    return;

                ControllerButton tabActions = ControllerButton.PreviousTab
                                            | ControllerButton.NextTab
                                            | ControllerButton.PreviousRootTab
                                            | ControllerButton.NextRootTab;
                if ((actions & tabActions) == 0)
                    return;
            }

            EnsureControllerFocusAppearance(current);

            if ((actions & ControllerButton.ToggleTooltip) != 0)
            {
                ToggleControllerTooltip(current);
                return;
            }

            if (_activeSlider != null)
            {
                if (!_activeSlider.IsVisible || !_activeSlider.IsEnabled)
                    _activeSlider = null;
                else
                {
                    if ((actions & (ControllerButton.Confirm | ControllerButton.Cancel)) != 0)
                        HandleCancel(scope, _activeSlider);
                    else if ((actions & ControllerButton.Left) != 0)
                        Move(scope, _activeSlider, NavigationDirection.Left);
                    else if ((actions & ControllerButton.Right) != 0)
                        Move(scope, _activeSlider, NavigationDirection.Right);

                    // A slider keeps directional focus until A or B releases it.
                    return;
                }
            }

            if ((actions & ControllerButton.Cancel) != 0)
                HandleCancel(scope, current);
            else if ((actions & ControllerButton.Confirm) != 0)
                Activate(current);
            else if ((actions & ControllerButton.PreviousRootTab) != 0)
                SwitchTab(scope, current, -1, true);
            else if ((actions & ControllerButton.NextRootTab) != 0)
                SwitchTab(scope, current, 1, true);
            else if ((actions & ControllerButton.PreviousTab) != 0)
                SwitchTab(scope, current, -1, false);
            else if ((actions & ControllerButton.NextTab) != 0)
                SwitchTab(scope, current, 1, false);
            else if ((actions & ControllerButton.Up) != 0)
                Move(scope, current, NavigationDirection.Up);
            else if ((actions & ControllerButton.Down) != 0)
                Move(scope, current, NavigationDirection.Down);
            else if ((actions & ControllerButton.Left) != 0)
                Move(scope, current, NavigationDirection.Left);
            else if ((actions & ControllerButton.Right) != 0)
                Move(scope, current, NavigationDirection.Right);
        }

        private FrameworkElement FindActiveScope()
        {
            return VisualTree
                       .Enumerate<FrameworkElement>(_window)
                       .LastOrDefault(element => element.IsVisible && GamepadNavigation.GetIsModalScope(element))
                   ?? (FrameworkElement)_window;
        }

        private Control FindFocusedControl(FrameworkElement scope)
        {
            if (_openComboBox != null && _openComboBox.IsDropDownOpen && VisualTree.IsDescendantOf(_openComboBox, scope))
                return _openComboBox;

            if (Keyboard.FocusedElement is not DependencyObject focused || !VisualTree.IsDescendantOf(focused, scope))
                return null;

            ListBox ownerList = VisualTree.FindAncestor<ListBox>(focused, true);
            if (ownerList != null && VisualTree.IsDescendantOf(ownerList, scope))
                return ownerList;

            Control control = VisualTree.FindAncestor<Control>(focused, true);
            return control != null && IsNavigationCandidate(control) ? control : null;
        }

        private void FocusInitialControl(FrameworkElement scope)
        {
            List<Control> controls = GetCandidates(scope).ToList();
            if (controls.Count == 0)
                return;

            Control target = controls.FirstOrDefault(GamepadNavigation.GetIsDefaultFocus)
                          ?? controls.OrderBy(control => GetBounds(control).Top)
                                     .ThenBy(control => GetBounds(control).Left)
                                     .First();
            Focus(target);
        }

        private void Move(FrameworkElement scope, Control current, NavigationDirection direction)
        {
            if (HandleControlDirection(current, direction))
                return;

            if (!(current is TabItem) && MoveWithinSelectedTab(current, direction))
                return;

            NavigationRectangle currentBounds = GetBounds(current);
            IEnumerable<SpatialNavigationCandidate<Control>> candidates = GetCandidates(scope)
                .Where(control => !ReferenceEquals(control, current))
                .Select(control => new SpatialNavigationCandidate<Control>(control, GetBounds(control)));
            
            SpatialNavigationCandidate<Control> next = SpatialNavigation.FindNext(currentBounds, candidates, direction);
            if (next != null)
                Focus(ResolveVerticalTabEntry(next.Value, direction));
        }

        private static Control ResolveVerticalTabEntry(Control candidate, NavigationDirection direction)
        {
            if (direction != NavigationDirection.Up && direction != NavigationDirection.Down)
                return candidate;

            if (candidate is not TabItem candidateTab)
                return candidate;

            TabItem selectedTab = ItemsControl.ItemsControlFromItemContainer(candidateTab) is not TabControl owner ? null : owner.SelectedItem as TabItem;
            return selectedTab ?? candidateTab;
        }

        private Boolean MoveWithinSelectedTab(Control current, NavigationDirection direction)
        {
            TabControl owner = FindNearestTabControl(current);
            TabItem selectedTab = owner == null ? null : owner.SelectedItem as TabItem;
            FrameworkElement content = selectedTab == null ? null : selectedTab.Content as FrameworkElement;
            if (content == null || !VisualTree.IsDescendantOf(current, content))
                return false;

            NavigationRectangle currentBounds = GetBounds(current);
            IEnumerable<SpatialNavigationCandidate<Control>> candidates = GetCandidates(content)
                .Where(control => !ReferenceEquals(control, current))
                .Select(control => new SpatialNavigationCandidate<Control>(control, GetBounds(control)));
            SpatialNavigationCandidate<Control> next = SpatialNavigation.FindNext(currentBounds, candidates, direction);
            if (next != null)
            {
                Focus(ResolveVerticalTabEntry(next.Value, direction));
                return true;
            }

            // Leaving through the top of a page always returns to the header of
            // that same page. Header position must not redirect the focus to a
            // different tab that merely happens to be vertically aligned.
            if (direction == NavigationDirection.Up)
                Focus(selectedTab);

            // Other edges are closed: without a neighbour on this page the
            // focus stays in place instead of leaking into another tab.
            return true;
        }

        private Boolean HandleControlDirection(Control current, NavigationDirection direction)
        {
            if (current is TabItem tab)
            {
                if (direction == NavigationDirection.Down && tab.IsSelected && MoveIntoSelectedTab(tab))
                    return true;

                if (direction == NavigationDirection.Left || direction == NavigationDirection.Right)
                {
                    if (ItemsControl.ItemsControlFromItemContainer(tab) is TabControl owner)
                    {
                        ChangeSelectedTab(owner, direction == NavigationDirection.Left ? -1 : 1);
                        return true;
                    }
                }
            }

            if (current is ComboBox comboBox)
            {
                if (comboBox.IsDropDownOpen && (direction == NavigationDirection.Up || direction == NavigationDirection.Down))
                {
                    ChangeSelection(comboBox, direction == NavigationDirection.Up ? -1 : 1);
                    return true;
                }
            }

            if (current is Slider slider && ReferenceEquals(slider, _activeSlider))
            {
                if (direction == NavigationDirection.Left || direction == NavigationDirection.Right)
                {
                    Double change = slider.TickFrequency > 0.0 ? slider.TickFrequency : slider.SmallChange;
                    if (change <= 0.0)
                        change = Math.Max((slider.Maximum - slider.Minimum) / 20.0, 1.0);
                    slider.Value = Math.Max(slider.Minimum, Math.Min(slider.Maximum,
                        slider.Value + (direction == NavigationDirection.Left ? -change : change)));
                }

                // Editing is modal for directional input. This prevents an
                // accidental vertical press from leaving the slider before B.
                return true;
            }

            if (current is ListBox list && (direction == NavigationDirection.Up || direction == NavigationDirection.Down))
            {
                Int32 nextIndex = list.SelectedIndex;
                if (nextIndex < 0)
                    nextIndex = direction == NavigationDirection.Up ? list.Items.Count - 1 : 0;
                else
                    nextIndex += direction == NavigationDirection.Up ? -1 : 1;

                if (nextIndex >= 0 && nextIndex < list.Items.Count)
                {
                    list.SelectedIndex = nextIndex;
                    list.ScrollIntoView(list.SelectedItem);
                    return true;
                }

                // At the first or last item, let spatial navigation leave the
                // list for a visible control in the requested direction.
                return false;
            }

            if (current is RichTextBox richText && richText.IsReadOnly && (direction == NavigationDirection.Up || direction == NavigationDirection.Down))
            {
                ScrollViewer viewer = VisualTree.FindDescendant<ScrollViewer>(richText);
                if (viewer != null)
                {
                    if (direction == NavigationDirection.Up)
                        viewer.LineUp();
                    else
                        viewer.LineDown();
                    return true;
                }
            }

            return false;
        }

        private Boolean MoveIntoSelectedTab(TabItem tab)
        {
            if (tab.Content is not FrameworkElement content)
                return false;

            // A page can start with another TabControl (for example the
            // Installed Mods / Catalog pair). Crossing that header row must
            // always land on its selected tab, independent of geometry.
            TabItem selectedNestedTab = VisualTree.Enumerate<TabControl>(content)
                .Where(owner => owner.IsVisible && owner.IsEnabled && owner.Items.Count > 1)
                .OrderBy(VisualTree.GetDepth)
                .Select(owner => owner.SelectedItem as TabItem)
                .FirstOrDefault(candidate => candidate != null && IsNavigationCandidate(candidate));
            
            if (selectedNestedTab != null)
            {
                Focus(selectedNestedTab);
                return true;
            }

            List<Control> controls = GetCandidates(content).ToList();
            if (controls.Count == 0)
                return false;

            NavigationRectangle tabBounds = GetBounds(tab);
            IEnumerable<SpatialNavigationCandidate<Control>> candidates = controls.Select(control =>
                new SpatialNavigationCandidate<Control>(control, GetBounds(control)));
            
            SpatialNavigationCandidate<Control> next = SpatialNavigation.FindNext(
                tabBounds,
                candidates,
                NavigationDirection.Down);

            // A custom TabControl template may arrange its content in a separate
            // presentation branch with coordinates that cannot be compared to
            // the header. In that case, enter through the top visual row.
            Control target = next == null
                ? controls.OrderBy(control => GetBounds(control).Top)
                          .ThenBy(control => Math.Abs(GetBounds(control).CenterX - tabBounds.CenterX))
                          .First()
                : next.Value;
            
            Focus(ResolveVerticalTabEntry(target, NavigationDirection.Down));
            return true;
        }

        private void Activate(Control current)
        {
            if (current is Slider slider)
            {
                _activeSlider = slider;
                ShowFocusAdorner(slider, true);
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
                if (!comboBox.IsDropDownOpen)
                {
                    _openComboBox = comboBox;
                    _openComboBoxInitialIndex = comboBox.SelectedIndex;
                }
                comboBox.IsDropDownOpen = !comboBox.IsDropDownOpen;
                if (comboBox.IsDropDownOpen)
                {
                    comboBox.Dispatcher.BeginInvoke(
                        DispatcherPriority.Input,
                        new Action(() => SuppressComboBoxPointerInput(comboBox)));
                }
                else
                {
                    RestoreComboBoxPointerInput();
                    _openComboBox = null;
                }
                return;
            }

            if (current is TabItem tab)
            {
                tab.IsSelected = true;
                return;
            }

            if (current is ToggleButton toggle)
            {
                Toggle(toggle);
                return;
            }

            if (current is Button button)
            {
                Invoke(button);
                return;
            }

            if (current is ListBox list)
                ActivateSelectedListItem(list);
        }

        private void ActivateSelectedListItem(ListBox list)
        {
            if (list.SelectedIndex < 0 && list.Items.Count > 0)
                list.SelectedIndex = 0;
            if (list.SelectedIndex < 0)
                return;

            DependencyObject container = list.ItemContainerGenerator.ContainerFromIndex(list.SelectedIndex);
            ToggleButton toggle = VisualTree.FindDescendant<ToggleButton>(container);
            if (toggle != null && toggle.IsEnabled && toggle.IsVisible)
            {
                Toggle(toggle);
                return;
            }

            if (container is Control item)
            {
                MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
                {
                    RoutedEvent = Control.MouseDoubleClickEvent,
                    Source = item
                };
                item.RaiseEvent(args);
            }
        }

        private void HandleCancel(FrameworkElement scope, Control current)
        {
            if (_activeSlider != null)
            {
                Slider slider = _activeSlider;
                _activeSlider = null;
                ShowFocusAdorner(slider, false);
                return;
            }

            if (_openComboBox != null && _openComboBox.IsDropDownOpen)
            {
                _openComboBox.SelectedIndex = _openComboBoxInitialIndex;
                _openComboBox.IsDropDownOpen = false;
                RestoreComboBoxPointerInput();
                _openComboBox = null;
                return;
            }

            if (GamepadNavigation.GetIsModalScope(scope))
            {
                Button close = GetCandidates(scope).OfType<Button>()
                    .FirstOrDefault(button => String.Equals(button.Name, "Cancel", StringComparison.OrdinalIgnoreCase))
                    ?? GetCandidates(scope).OfType<Button>()
                        .FirstOrDefault(button => String.Equals(button.Name, "Ok", StringComparison.OrdinalIgnoreCase));
                
                if (close != null)
                    Invoke(close);
                
                return;
            }

            TabControl tabControl = FindNearestTabControl(current);
            if (tabControl != null)
            {
                if (tabControl.SelectedItem is TabItem selectedTab && !ReferenceEquals(current, selectedTab))
                {
                    Focus(selectedTab);
                    return;
                }

                TabControl parentTabControl = FindNearestTabControl(VisualTree.GetParent(tabControl));
                TabItem parentSelectedTab = parentTabControl == null ? null : parentTabControl.SelectedItem as TabItem;
                if (parentSelectedTab != null)
                    Focus(parentSelectedTab);
            }
        }

        private void SwitchTab(FrameworkElement scope, Control current, Int32 offset, Boolean root)
        {
            TabControl tabControl = root ? FindRootTabControl(scope) : FindNearestTabControl(current);
            if (tabControl == null)
                tabControl = FindRootTabControl(scope);
            if (tabControl != null)
                ChangeSelectedTab(tabControl, offset);
        }

        private static TabControl FindNearestTabControl(DependencyObject element)
        {
            DependencyObject current = element;
            while (current != null)
            {
                TabControl fromItem = current is not TabItem tab ? null : ItemsControl.ItemsControlFromItemContainer(tab) as TabControl;
                if (fromItem != null && fromItem.Items.Count > 1)
                    return fromItem;

                TabControl ancestor = VisualTree.FindAncestor<TabControl>(current, false);
                if (ancestor == null)
                    return null;
                if (ancestor.Items.Count > 1)
                    return ancestor;
                current = VisualTree.GetParent(ancestor);
            }
            return null;
        }

        private static TabControl FindRootTabControl(FrameworkElement scope)
        {
            return VisualTree.Enumerate<TabControl>(scope)
                .Where(tab => tab.IsVisible && tab.IsEnabled && tab.Items.Count > 1)
                .OrderBy(VisualTree.GetDepth)
                .FirstOrDefault();
        }

        private void ChangeSelectedTab(TabControl tabControl, Int32 offset)
        {
            if (tabControl.Items.Count < 2)
                return;

            Int32 start = Math.Max(0, tabControl.SelectedIndex);
            for (Int32 step = 1; step <= tabControl.Items.Count; step++)
            {
                Int32 index = (start + offset * step + tabControl.Items.Count) % tabControl.Items.Count;
                TabItem candidate = tabControl.ItemContainerGenerator.ContainerFromIndex(index) as TabItem
                                 ?? tabControl.Items[index] as TabItem;
                if (candidate == null || !candidate.IsEnabled || candidate.Visibility != Visibility.Visible)
                    continue;

                tabControl.SelectedIndex = index;
                tabControl.UpdateLayout();
                Focus(candidate);
                return;
            }
        }

        private static void ChangeSelection(Selector selector, Int32 offset)
        {
            if (selector.Items.Count == 0)
                return;

            Int32 index = selector.SelectedIndex < 0 ? 0 : selector.SelectedIndex + offset;
            selector.SelectedIndex = Math.Max(0, Math.Min(selector.Items.Count - 1, index));
        }

        private void Focus(Control control)
        {
            if (control == null)
                return;

            // WPF selectors are not focusable by default. The list itself is
            // the controller navigation target while its selected row is the
            // item operated on by the D-pad and action button.
            ListBox list = control as ListBox;
            if (list != null)
                list.Focusable = true;

            ApplyControllerFocusVisualStyle(control);
            if (!control.Focus())
            {
                RestoreFocusVisualStyle();
                return;
            }

            if (_openComboBox != null && !ReferenceEquals(_openComboBox, control))
            {
                _openComboBox.IsDropDownOpen = false;
                RestoreComboBoxPointerInput();
                _openComboBox = null;
            }
            if (_activeSlider != null && !ReferenceEquals(_activeSlider, control))
                _activeSlider = null;

            if (list != null && list.SelectedIndex < 0 && list.Items.Count > 0)
            {
                list.SelectedIndex = 0;
                list.ScrollIntoView(list.SelectedItem);
            }

            Keyboard.Focus(control);
            control.BringIntoView();
            ShowFocusAdorner(control);
            _controllerFocusAppearanceActive = true;
            if (_controllerTooltipsEnabled)
                ShowControllerTooltip(control);
        }

        private void EnsureControllerFocusAppearance(Control control)
        {
            if (_controllerFocusAppearanceActive && ReferenceEquals(_focusVisualOwner, control))
                return;

            ApplyControllerFocusVisualStyle(control);
            ShowFocusAdorner(control, ReferenceEquals(control, _activeSlider));
            _controllerFocusAppearanceActive = true;
        }

        private void ApplyControllerFocusVisualStyle(Control control)
        {
            if (ReferenceEquals(_focusVisualOwner, control))
                return;

            RestoreFocusVisualStyle();
            _focusVisualOwner = control;
            _focusVisualPreviousLocalValue = control.ReadLocalValue(FrameworkElement.FocusVisualStyleProperty);
            control.SetValue(FrameworkElement.FocusVisualStyleProperty, null);
        }

        private void RestoreFocusVisualStyle()
        {
            if (_focusVisualOwner == null)
                return;

            if (_focusVisualPreviousLocalValue == DependencyProperty.UnsetValue)
                _focusVisualOwner.ClearValue(FrameworkElement.FocusVisualStyleProperty);
            else
                _focusVisualOwner.SetValue(FrameworkElement.FocusVisualStyleProperty, _focusVisualPreviousLocalValue);
            _focusVisualOwner = null;
            _focusVisualPreviousLocalValue = null;
        }

        private void DeactivateControllerFocusAppearance(Boolean clearKeyboardFocus)
        {
            RemoveFocusAdorner();
            RestoreFocusVisualStyle();
            _controllerFocusAppearanceActive = false;
            if (clearKeyboardFocus)
                Keyboard.ClearFocus();
        }

        private void ShowFocusAdorner(Control control, Boolean isEditing = false)
        {
            RemoveFocusAdorner();
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);
            if (layer == null)
                return;

            _focusAdorner = new ControllerFocusAdorner(control, isEditing);
            layer.Add(_focusAdorner);
        }

        private void RemoveFocusAdorner()
        {
            if (_focusAdorner == null)
                return;

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(_focusAdorner.AdornedElement);
            if (layer != null)
                layer.Remove(_focusAdorner);
            
            _focusAdorner = null;
        }

        private IEnumerable<Control> GetCandidates(FrameworkElement scope)
        {
            return VisualTree.Enumerate<Control>(scope).Where(IsNavigationCandidate);
        }

        private static Boolean IsNavigationCandidate(Control control)
        {
            Boolean isList = control is ListBox;
            if (!control.IsVisible || !control.IsEnabled || !control.IsHitTestVisible ||
                (!control.Focusable && !isList))
                return false;
            
            if ((!KeyboardNavigation.GetIsTabStop(control) && !isList) || !HasVisibleLayout(control))
                return false;
            
            if (control.TemplatedParent != null)
                return false;
            
            if (control is ListBoxItem || control is ComboBoxItem || control is TabControl ||
                control is ScrollBar || control is Thumb || control is RepeatButton)
                return false;

            TabItem tab = control as TabItem;
            TabControl tabOwner = tab == null ? null : ItemsControl.ItemsControlFromItemContainer(tab) as TabControl;
            if (tab != null && (tabOwner == null || tabOwner.Items.Count <= 1))
                return false;

            return VisualTree.FindAncestor<ListBoxItem>(control, false) == null
                && VisualTree.FindAncestor<ComboBoxItem>(control, false) == null;
        }

        private static Boolean HasVisibleLayout(FrameworkElement element)
        {
            DependencyObject current = element;
            while (current != null)
            {
                if (current is FrameworkElement ancestor &&
                    (!ancestor.IsVisible || ancestor.Opacity <= 0.01 ||
                     ancestor.Width == 0.0 || ancestor.Height == 0.0 ||
                     ancestor.ActualWidth <= 1.0 || ancestor.ActualHeight <= 1.0))
                {
                    return false;
                }

                current = VisualTree.GetParent(current);
            }

            return true;
        }

        private NavigationRectangle GetBounds(FrameworkElement element)
        {
            try
            {
                GeneralTransform transform = element.TransformToAncestor(_window);
                Rect bounds = transform.TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
                return new NavigationRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }
            catch (InvalidOperationException)
            {
                return new NavigationRectangle(0.0, 0.0, element.ActualWidth, element.ActualHeight);
            }
        }

        private static void Invoke(Button button)
        {
            if (new ButtonAutomationPeer(button).GetPattern(PatternInterface.Invoke) is IInvokeProvider provider)
                provider.Invoke();
            else
                button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
        }

        private static void Toggle(ToggleButton button)
        {
            if (new ToggleButtonAutomationPeer(button).GetPattern(PatternInterface.Toggle) is IToggleProvider provider)
                provider.Toggle();
            else
                button.IsChecked = button.IsChecked != true;
        }

        private void ToggleControllerTooltip(Control current)
        {
            if (_controllerTooltipsEnabled)
            {
                DisableControllerTooltips();
                return;
            }

            _controllerTooltipsEnabled = true;
            ShowControllerTooltip(current);
        }

        private void ShowControllerTooltip(Control current)
        {
            FrameworkElement owner = FindTooltipOwner(current);
            if (owner == null)
            {
                CloseControllerTooltip();
                return;
            }

            if (ReferenceEquals(_controllerToolTipOwner, owner) && _controllerToolTip != null && _controllerToolTip.IsOpen)
                return;

            CloseControllerTooltip();
            Object value = ToolTipService.GetToolTip(owner);
            ToolTip toolTip = value as ToolTip;
            _controllerToolTipReused = toolTip != null;
            if (toolTip == null)
                toolTip = new ToolTip { Content = value };
            else
            {
                _tooltipPreviousPlacement = toolTip.Placement;
                _tooltipPreviousTarget = toolTip.PlacementTarget;
                _tooltipPreviousHorizontalOffset = toolTip.HorizontalOffset;
                _tooltipPreviousVerticalOffset = toolTip.VerticalOffset;
                _tooltipPreviousStaysOpen = toolTip.StaysOpen;
            }

            _controllerToolTip = toolTip;
            _controllerToolTipOwner = owner;
            toolTip.PlacementTarget = owner;
            toolTip.Placement = PlacementMode.RelativePoint;
            toolTip.HorizontalOffset = Math.Max(0.0, owner.ActualWidth - 1.0);
            toolTip.VerticalOffset = Math.Max(0.0, owner.ActualHeight - 1.0);
            toolTip.StaysOpen = true;
            toolTip.IsOpen = true;
        }

        private void DisableControllerTooltips()
        {
            _controllerTooltipsEnabled = false;
            CloseControllerTooltip();
        }

        private FrameworkElement FindTooltipOwner(Control current)
        {
            if (ToolTipService.GetToolTip(current) != null)
                return current;

            // Sliders and combo boxes are generated next to their explanatory
            // text in UiGrid. The tooltip belongs to that text, but from a
            // controller the pair is perceived as one setting.
            UiGrid settingsGrid = VisualTree.FindAncestor<UiGrid>(current, false);
            if (settingsGrid == null)
                return null;

            Int32 row = Grid.GetRow(current);
            return VisualTree.Enumerate<FrameworkElement>(settingsGrid)
                .Where(element => ToolTipService.GetToolTip(element) != null)
                .Where(element => GetDirectGridRow(element, settingsGrid) == row)
                .OrderBy(element => DistanceSquared(GetBounds(current), GetBounds(element)))
                .FirstOrDefault();
        }

        private static Int32 GetDirectGridRow(DependencyObject element, Grid grid)
        {
            DependencyObject current = element;
            DependencyObject parent;
            while (current != null && (parent = VisualTree.GetParent(current)) != null)
            {
                if (ReferenceEquals(parent, grid))
                {
                    return current is not UIElement child ? -1 : Grid.GetRow(child);
                }
                current = parent;
            }
            return -1;
        }

        private static Double DistanceSquared(NavigationRectangle first, NavigationRectangle second)
        {
            Double x = first.CenterX - second.CenterX;
            Double y = first.CenterY - second.CenterY;
            return x * x + y * y;
        }

        private void CloseControllerTooltip()
        {
            if (_controllerToolTip == null)
                return;

            _controllerToolTip.IsOpen = false;
            if (_controllerToolTipReused)
            {
                _controllerToolTip.Placement = _tooltipPreviousPlacement;
                _controllerToolTip.PlacementTarget = _tooltipPreviousTarget;
                _controllerToolTip.HorizontalOffset = _tooltipPreviousHorizontalOffset;
                _controllerToolTip.VerticalOffset = _tooltipPreviousVerticalOffset;
                _controllerToolTip.StaysOpen = _tooltipPreviousStaysOpen;
            }

            _controllerToolTip = null;
            _controllerToolTipOwner = null;
            _controllerToolTipReused = false;
        }

        private void CloseAutomaticTooltips()
        {
            foreach (FrameworkElement element in VisualTree.Enumerate<FrameworkElement>(_window))
            {
                if (ToolTipService.GetToolTip(element) is ToolTip toolTip && toolTip.IsOpen)
                    toolTip.IsOpen = false;
            }
        }

        private void OnPreviewMouseDown(Object sender, MouseButtonEventArgs e)
        {
            EnterMouseMode();
            _activeSlider = null;
            RemoveFocusAdorner();
        }

        private void OnPreviewMouseMove(Object sender, MouseEventArgs e)
        {
            if (!_controllerInputActive)
                return;
            RestoreMouseModeAfterPhysicalMovement();
        }

        private void OnPreviewMouseWheel(Object sender, MouseWheelEventArgs e)
        {
            EnterMouseMode();
        }

        private void OnPreProcessInput(Object sender, PreProcessInputEventArgs e)
        {
            InputEventArgs input = e.StagingItem.Input;
            if (input is KeyEventArgs key && key.IsDown)
            {
                DisableControllerTooltips();
                DeactivateControllerFocusAppearance(false);
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
            NativePointerPosition position;
            if (!_hasControllerPointerPosition ||
                !NativePointer.TryGetPosition(out position) ||
                !position.Equals(_controllerPointerPosition))
            {
                EnterMouseMode();
            }
        }

        private void EnterControllerMode()
        {
            if (_controllerInputActive)
            {
                Mouse.OverrideCursor = Cursors.None;
                return;
            }

            _cursorBeforeControllerMode = Mouse.OverrideCursor;
            _controllerInputActive = true;
            GamepadNavigation.IsControllerInputActive = true;
            _hasControllerPointerPosition = NativePointer.TryGetPosition(out _controllerPointerPosition);
            Mouse.OverrideCursor = Cursors.None;
            CloseAutomaticTooltips();
            InstallMouseInputShield();
        }

        private void EnterMouseMode()
        {
            DisableControllerTooltips();
            DeactivateControllerFocusAppearance(true);
            if (!_controllerInputActive)
                return;

            _controllerInputActive = false;
            GamepadNavigation.IsControllerInputActive = false;
            _hasControllerPointerPosition = false;
            RestoreComboBoxPointerInput();
            RemoveMouseInputShield();
            Mouse.OverrideCursor = _cursorBeforeControllerMode;
            _cursorBeforeControllerMode = null;
        }

        private void SuppressComboBoxPointerInput(ComboBox comboBox)
        {
            if (!_controllerInputActive || !comboBox.IsDropDownOpen)
                return;

            comboBox.ApplyTemplate();
            UIElement content = comboBox.Template.FindName("PART_Popup", comboBox) is not Popup popup ? null : popup.Child;
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
            if (_mouseInputShield == null)
                return;

            if (_mouseInputShield.Parent is Panel parent)
                parent.Children.Remove(_mouseInputShield);
            
            _mouseInputShield = null;
        }

        private void OnWindowClosed(Object sender, EventArgs e)
        {
            Dispose();
        }

        private sealed class EmptyDisposable : IDisposable
        {
            public static readonly EmptyDisposable Instance = new EmptyDisposable();
            public void Dispose() { }
        }
    }

    internal sealed class ControllerFocusAdorner : Adorner
    {
        private const Double StrokeThickness = 3.0;
        private readonly Boolean _isEditing;

        public ControllerFocusAdorner(UIElement adornedElement, Boolean isEditing)
            : base(adornedElement)
        {
            _isEditing = isEditing;
            IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Brush brush = TryFindResource("BrushAccentColorPressed") as Brush ?? Brushes.DeepSkyBlue;
            Double thickness = _isEditing ? StrokeThickness + 2.0 : StrokeThickness;
            Pen pen = new Pen(brush, thickness);
            Rect bounds = new Rect(
                -thickness,
                -thickness,
                Math.Max(0.0, AdornedElement.RenderSize.Width + thickness * 2.0),
                Math.Max(0.0, AdornedElement.RenderSize.Height + thickness * 2.0));
            
            drawingContext.DrawRoundedRectangle(null, pen, bounds, 5.0, 5.0);
        }
    }

    internal static class VisualTree
    {
        public static IEnumerable<T> Enumerate<T>(DependencyObject root) where T : DependencyObject
        {
            if (root == null)
                yield break;

            if (root is T typed)
                yield return typed;

            Int32 count = VisualTreeHelper.GetChildrenCount(root);
            for (Int32 index = 0; index < count; index++)
            {
                foreach (T descendant in Enumerate<T>(VisualTreeHelper.GetChild(root, index)))
                    yield return descendant;
            }
        }

        public static T FindAncestor<T>(DependencyObject element, Boolean includeSelf) where T : DependencyObject
        {
            DependencyObject current = includeSelf ? element : GetParent(element);
            while (current != null)
            {
                if (current is T typed)
                    return typed;
                current = GetParent(current);
            }
            return null;
        }

        public static T FindDescendant<T>(DependencyObject root) where T : DependencyObject
        {
            return Enumerate<T>(root).FirstOrDefault();
        }

        public static Boolean IsDescendantOf(DependencyObject element, DependencyObject ancestor)
        {
            DependencyObject current = element;
            while (current != null)
            {
                if (ReferenceEquals(current, ancestor))
                    return true;
                current = GetParent(current);
            }
            return false;
        }

        public static DependencyObject GetParent(DependencyObject element)
        {
            if (element == null)
                return null;
            
            if (element is Visual || element is System.Windows.Media.Media3D.Visual3D)
                return VisualTreeHelper.GetParent(element);
            
            return (element is not ContentElement content ? null : ContentOperations.GetParent(content))
                   ?? LogicalTreeHelper.GetParent(element);
        }

        public static Int32 GetDepth(DependencyObject element)
        {
            Int32 depth = 0;
            
            while ((element = GetParent(element)) != null)
                depth++;
            
            return depth;
        }
    }

    internal struct NativePointerPosition : IEquatable<NativePointerPosition>
    {
        public Int32 X;
        public Int32 Y;

        public Boolean Equals(NativePointerPosition other)
        {
            return X == other.X && Y == other.Y;
        }

        public override Boolean Equals(Object obj)
        {
            return obj is NativePointerPosition && Equals((NativePointerPosition)obj);
        }

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
        public static Boolean TryGetPosition(out NativePointerPosition position)
        {
            return GetCursorPos(out position);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern Boolean GetCursorPos(out NativePointerPosition point);
    }

    internal static class NativeDialogControllerBridge
    {
        private const UInt32 Command = 0x0111;
        private const UInt32 KeyDown = 0x0100;
        private const UInt32 KeyUp = 0x0101;
        private const Int32 OkButton = 1;
        private const Int32 CancelButton = 2;
        private const Int32 NoButton = 7;
        private const Int32 Enter = 0x0D;
        private const Int32 Escape = 0x1B;
        private const Int32 ArrowLeft = 0x25;
        private const Int32 ArrowUp = 0x26;
        private const Int32 ArrowRight = 0x27;
        private const Int32 ArrowDown = 0x28;

        public static Boolean IsCurrentProcessInForeground()
        {
            IntPtr window = GetForegroundWindow();
            if (window == IntPtr.Zero)
                return false;

            UInt32 processId;
            GetWindowThreadProcessId(window, out processId);
            return processId == (UInt32)Process.GetCurrentProcess().Id;
        }

        public static void Send(ControllerButton actions)
        {
            if ((actions & ControllerButton.Cancel) != 0)
                Cancel();
            else if ((actions & ControllerButton.Confirm) != 0)
                SendKey(Enter);
            else if ((actions & ControllerButton.Up) != 0)
                SendKey(ArrowUp);
            else if ((actions & ControllerButton.Down) != 0)
                SendKey(ArrowDown);
            else if ((actions & ControllerButton.Left) != 0)
                SendKey(ArrowLeft);
            else if ((actions & ControllerButton.Right) != 0)
                SendKey(ArrowRight);
        }

        private static void Cancel()
        {
            IntPtr window = GetForegroundWindow();
            if (window == IntPtr.Zero)
                return;

            // MessageBox does not accept Escape for Yes/No dialogs. Invoke its
            // semantic cancel choice directly: Cancel, then No, or OK for an
            // informational dialog. Non-MessageBox modal windows fall back to
            // the ordinary Escape key path.
            if (TryInvokeDialogButton(window, CancelButton) ||
                TryInvokeDialogButton(window, NoButton) ||
                TryInvokeDialogButton(window, OkButton))
            {
                return;
            }

            SendKey(window, Escape);
        }

        private static Boolean TryInvokeDialogButton(IntPtr window, Int32 buttonId)
        {
            IntPtr button = GetDlgItem(window, buttonId);
            return button != IntPtr.Zero &&
                   PostMessage(window, Command, new IntPtr(buttonId), button);
        }

        private static void SendKey(Int32 virtualKey)
        {
            IntPtr window = GetForegroundWindow();
            if (window == IntPtr.Zero)
                return;
            SendKey(window, virtualKey);
        }

        private static void SendKey(IntPtr window, Int32 virtualKey)
        {
            PostMessage(window, KeyDown, new IntPtr(virtualKey), IntPtr.Zero);
            PostMessage(window, KeyUp, new IntPtr(virtualKey), IntPtr.Zero);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern UInt32 GetWindowThreadProcessId(IntPtr window, out UInt32 processId);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetDlgItem(IntPtr dialog, Int32 itemId);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern Boolean PostMessage(IntPtr window, UInt32 message, IntPtr wParam, IntPtr lParam);
    }
}
