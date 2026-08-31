using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Memoria.Launcher.Controller
{
    /// <summary>
    /// Finds controller navigation targets and moves focus between them.
    /// </summary>
    internal sealed class ControllerFocusNavigator
    {
        private readonly Window _window;
        private readonly ControllerFocusManager _focus;

        public ControllerFocusNavigator(Window window, ControllerFocusManager focus)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _focus = focus ?? throw new ArgumentNullException(nameof(focus));
        }

        public FrameworkElement FindActiveScope()
        {
            return VisualTree.Enumerate<FrameworkElement>(_window)
                       .LastOrDefault(element => element.IsVisible && GamepadNavigation.GetIsModalScope(element))
                   ?? (FrameworkElement)_window;
        }

        public Control FindFocusedControl(FrameworkElement scope)
        {
            if (Keyboard.FocusedElement is not DependencyObject focused ||
                !VisualTree.IsDescendantOf(focused, scope))
            {
                return null;
            }

            ListBox ownerList = VisualTree.FindAncestor<ListBox>(focused, true);
            if (ownerList != null && VisualTree.IsDescendantOf(ownerList, scope))
                return ownerList;

            Control control = VisualTree.FindAncestor<Control>(focused, true);
            return control != null && IsNavigationCandidate(control) ? control : null;
        }

        public void FocusInitialControl(FrameworkElement scope)
        {
            List<Control> controls = GetCandidates(scope).ToList();
            if (controls.Count == 0)
                return;

            Control target = controls.FirstOrDefault(GamepadNavigation.GetIsDefaultFocus)
                          ?? controls.OrderBy(control => GetBounds(control).Top)
                                     .ThenBy(control => GetBounds(control).Left)
                                     .First();
            _focus.Focus(target);
        }

        public void Move(FrameworkElement scope, Control current, NavigationDirection direction)
        {
            if (TryNavigateTab(current, direction))
                return;

            if (current is not TabItem && MoveWithinSelectedTab(current, direction))
                return;

            NavigationRectangle currentBounds = GetBounds(current);
            IEnumerable<SpatialNavigationCandidate<Control>> candidates = GetCandidates(scope)
                .Where(control => !ReferenceEquals(control, current))
                .Select(control => new SpatialNavigationCandidate<Control>(control, GetBounds(control)));

            SpatialNavigationCandidate<Control> next = SpatialNavigation.FindNext(currentBounds, candidates, direction);
            if (next != null)
                _focus.Focus(ResolveVerticalTabEntry(next.Value, direction));
        }

        public void Cancel(FrameworkElement scope, Control current)
        {
            if (GamepadNavigation.GetIsModalScope(scope))
            {
                Button cancel = GetCandidates(scope)
                    .OfType<Button>()
                    .FirstOrDefault(GamepadNavigation.GetIsCancelAction);
                if (cancel != null)
                    ControllerControlInvoker.Invoke(cancel);
                return;
            }

            TabControl tabControl = FindNearestTabControl(current);
            if (tabControl == null)
                return;

            if (tabControl.SelectedItem is TabItem selectedTab && !ReferenceEquals(current, selectedTab))
            {
                _focus.Focus(selectedTab);
                return;
            }

            TabControl parent = FindNearestTabControl(VisualTree.GetParent(tabControl));
            if (parent?.SelectedItem is TabItem parentSelectedTab)
                _focus.Focus(parentSelectedTab);
        }

        public void SwitchTab(FrameworkElement scope, Control current, Int32 offset, Boolean root)
        {
            TabControl tabControl = root ? FindRootTabControl(scope) : FindNearestTabControl(current);
            ChangeSelectedTab(tabControl ?? FindRootTabControl(scope), offset);
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

        private Boolean TryNavigateTab(Control current, NavigationDirection direction)
        {
            if (current is not TabItem tab)
                return false;

            if (direction == NavigationDirection.Down && tab.IsSelected && MoveIntoSelectedTab(tab))
                return true;

            if (direction != NavigationDirection.Left && direction != NavigationDirection.Right)
                return false;

            if (ItemsControl.ItemsControlFromItemContainer(tab) is TabControl owner)
            {
                ChangeSelectedTab(owner, direction == NavigationDirection.Left ? -1 : 1);
                return true;
            }

            return false;
        }

        private Boolean MoveWithinSelectedTab(Control current, NavigationDirection direction)
        {
            TabControl owner = FindNearestTabControl(current);
            TabItem selectedTab = owner?.SelectedItem as TabItem;
            FrameworkElement content = selectedTab?.Content as FrameworkElement;
            if (content == null || !VisualTree.IsDescendantOf(current, content))
                return false;

            NavigationRectangle currentBounds = GetBounds(current);
            IEnumerable<SpatialNavigationCandidate<Control>> candidates = GetCandidates(content)
                .Where(control => !ReferenceEquals(control, current))
                .Select(control => new SpatialNavigationCandidate<Control>(control, GetBounds(control)));
            SpatialNavigationCandidate<Control> next = SpatialNavigation.FindNext(currentBounds, candidates, direction);
            if (next != null)
            {
                _focus.Focus(ResolveVerticalTabEntry(next.Value, direction));
                return true;
            }

            if (direction == NavigationDirection.Up)
                _focus.Focus(selectedTab);

            // Page edges are closed so focus cannot leak into another tab's content.
            return true;
        }

        private Boolean MoveIntoSelectedTab(TabItem tab)
        {
            if (tab.Content is not FrameworkElement content)
                return false;

            TabItem selectedNestedTab = VisualTree.Enumerate<TabControl>(content)
                .Where(owner => owner.IsVisible && owner.IsEnabled && owner.Items.Count > 1)
                .OrderBy(VisualTree.GetDepth)
                .Select(owner => owner.SelectedItem as TabItem)
                .FirstOrDefault(candidate => candidate != null && IsNavigationCandidate(candidate));

            if (selectedNestedTab != null)
            {
                _focus.Focus(selectedNestedTab);
                return true;
            }

            List<Control> controls = GetCandidates(content).ToList();
            if (controls.Count == 0)
                return false;

            NavigationRectangle tabBounds = GetBounds(tab);
            IEnumerable<SpatialNavigationCandidate<Control>> candidates = controls.Select(control =>
                new SpatialNavigationCandidate<Control>(control, GetBounds(control)));
            SpatialNavigationCandidate<Control> next = SpatialNavigation.FindNext(
                tabBounds, candidates, NavigationDirection.Down);

            Control target = next?.Value
                          ?? controls.OrderBy(control => GetBounds(control).Top)
                                     .ThenBy(control => Math.Abs(GetBounds(control).CenterX - tabBounds.CenterX))
                                     .First();
            _focus.Focus(ResolveVerticalTabEntry(target, NavigationDirection.Down));
            return true;
        }

        private static Control ResolveVerticalTabEntry(Control candidate, NavigationDirection direction)
        {
            if (direction != NavigationDirection.Up && direction != NavigationDirection.Down)
                return candidate;
            if (candidate is not TabItem tab)
                return candidate;

            TabControl owner = ItemsControl.ItemsControlFromItemContainer(tab) as TabControl;
            return owner?.SelectedItem as TabItem ?? tab;
        }

        private IEnumerable<Control> GetCandidates(FrameworkElement scope) =>
            VisualTree.Enumerate<Control>(scope).Where(IsNavigationCandidate);

        private static Boolean IsNavigationCandidate(Control control)
        {
            NavigationParticipation participation = GamepadNavigation.GetParticipation(control);
            if (participation == NavigationParticipation.Exclude)
                return false;

            Boolean explicitlyIncluded = participation == NavigationParticipation.Include;
            if (!control.IsVisible || !control.IsEnabled || !control.IsHitTestVisible ||
                (!control.Focusable && !explicitlyIncluded))
            {
                return false;
            }

            if ((!KeyboardNavigation.GetIsTabStop(control) && !explicitlyIncluded) || !HasVisibleLayout(control))
                return false;
            if (control.TemplatedParent != null && !explicitlyIncluded)
                return false;
            if (control is ListBoxItem || control is ComboBoxItem || control is TabControl ||
                control is ScrollBar || control is Thumb || control is RepeatButton)
            {
                return false;
            }

            if (control is TabItem tab)
            {
                TabControl owner = ItemsControl.ItemsControlFromItemContainer(tab) as TabControl;
                if (owner == null || owner.Items.Count <= 1)
                    return false;
            }

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

        private static TabControl FindNearestTabControl(DependencyObject element)
        {
            DependencyObject current = element;
            while (current != null)
            {
                TabControl fromItem = current is TabItem tab
                    ? ItemsControl.ItemsControlFromItemContainer(tab) as TabControl
                    : null;
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

        private static TabControl FindRootTabControl(FrameworkElement scope) =>
            VisualTree.Enumerate<TabControl>(scope)
                .Where(tab => tab.IsVisible && tab.IsEnabled && tab.Items.Count > 1)
                .OrderBy(VisualTree.GetDepth)
                .FirstOrDefault();

        private void ChangeSelectedTab(TabControl tabControl, Int32 offset)
        {
            if (tabControl == null || tabControl.Items.Count < 2)
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
                _focus.Focus(candidate);
                return;
            }
        }
    }
}
