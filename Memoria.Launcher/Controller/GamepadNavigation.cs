using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Memoria.Launcher.Controller
{
    public enum NavigationParticipation
    {
        Auto,
        Include,
        Exclude
    }

    /// <summary>
    /// Declarative controller-navigation metadata for launcher controls.
    /// </summary>
    public static class GamepadNavigation
    {
        private static readonly DependencyPropertyKey IsControllerInputActivePropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "IsControllerInputActive",
                typeof(Boolean),
                typeof(GamepadNavigation),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty IsControllerInputActiveProperty =
            IsControllerInputActivePropertyKey.DependencyProperty;

        public static readonly DependencyProperty IsDefaultFocusProperty = DependencyProperty.RegisterAttached(
            "IsDefaultFocus", typeof(Boolean), typeof(GamepadNavigation), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsModalScopeProperty = DependencyProperty.RegisterAttached(
            "IsModalScope", typeof(Boolean), typeof(GamepadNavigation), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsCancelActionProperty = DependencyProperty.RegisterAttached(
            "IsCancelAction", typeof(Boolean), typeof(GamepadNavigation), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty ParticipationProperty = DependencyProperty.RegisterAttached(
            "Participation", typeof(NavigationParticipation), typeof(GamepadNavigation),
            new FrameworkPropertyMetadata(NavigationParticipation.Auto));

        public static readonly DependencyProperty TooltipOwnerProperty = DependencyProperty.RegisterAttached(
            "TooltipOwner", typeof(FrameworkElement), typeof(GamepadNavigation), new FrameworkPropertyMetadata(null));

        public static readonly RoutedEvent ActivatedEvent = EventManager.RegisterRoutedEvent(
            "Activated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GamepadNavigation));

        public static Boolean IsControllerInputActive(DependencyObject element) =>
            (Boolean)element.GetValue(IsControllerInputActiveProperty);

        public static void SetIsDefaultFocus(DependencyObject element, Boolean value) =>
            element.SetValue(IsDefaultFocusProperty, value);

        public static Boolean GetIsDefaultFocus(DependencyObject element) =>
            (Boolean)element.GetValue(IsDefaultFocusProperty);

        public static void SetIsModalScope(DependencyObject element, Boolean value) =>
            element.SetValue(IsModalScopeProperty, value);

        public static Boolean GetIsModalScope(DependencyObject element) =>
            (Boolean)element.GetValue(IsModalScopeProperty);

        public static void SetIsCancelAction(DependencyObject element, Boolean value) =>
            element.SetValue(IsCancelActionProperty, value);

        public static Boolean GetIsCancelAction(DependencyObject element) =>
            (Boolean)element.GetValue(IsCancelActionProperty);

        public static void SetParticipation(DependencyObject element, NavigationParticipation value) =>
            element.SetValue(ParticipationProperty, value);

        public static NavigationParticipation GetParticipation(DependencyObject element) =>
            (NavigationParticipation)element.GetValue(ParticipationProperty);

        public static void SetTooltipOwner(DependencyObject element, FrameworkElement value) =>
            element.SetValue(TooltipOwnerProperty, value);

        public static FrameworkElement GetTooltipOwner(DependencyObject element) =>
            (FrameworkElement)element.GetValue(TooltipOwnerProperty);

        public static void AddActivatedHandler(DependencyObject element, RoutedEventHandler handler) =>
            ((UIElement)element).AddHandler(ActivatedEvent, handler);

        public static void RemoveActivatedHandler(DependencyObject element, RoutedEventHandler handler) =>
            ((UIElement)element).RemoveHandler(ActivatedEvent, handler);

        internal static void SetIsControllerInputActive(DependencyObject element, Boolean value) =>
            element.SetValue(IsControllerInputActivePropertyKey, value);

        internal static Boolean RaiseActivated(UIElement element)
        {
            RoutedEventArgs args = new RoutedEventArgs(ActivatedEvent, element);
            element.RaiseEvent(args);
            return args.Handled;
        }
    }

    /// <summary>
    /// Polls controller input and routes semantic actions to focused UI services.
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
        private readonly ControllerFocusManager _focus;
        private readonly ControllerFocusNavigator _navigator;
        private readonly ControllerControlInteractor _interaction;
        private readonly ControllerTooltipPresenter _tooltips;
        private readonly ControllerInputModeManager _inputMode;
        private Boolean _disposed;

        internal GamepadNavigationService(Window window, IControllerInputSource input)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _repeater = new ControllerButtonRepeater(InitialRepeatDelay, RepeatInterval);

            _focus = new ControllerFocusManager(window);
            _navigator = new ControllerFocusNavigator(window, _focus);
            _interaction = new ControllerControlInteractor(window, _focus);
            _tooltips = new ControllerTooltipPresenter(window, _focus);
            _inputMode = new ControllerInputModeManager(window, _focus, _interaction, _tooltips);

            _timer = new DispatcherTimer(DispatcherPriority.Input, window.Dispatcher)
            {
                Interval = PollInterval
            };
            _timer.Tick += OnTick;
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
            _window.Closed -= OnWindowClosed;
            _inputMode.Dispose();
            _tooltips.Dispose();
            _interaction.Dispose();
            _focus.Dispose();
            _input.Dispose();
        }

        private void OnTick(Object sender, EventArgs e)
        {
            if (_window.WindowState == WindowState.Minimized)
            {
                _repeater.Reset();
                return;
            }

            Boolean nativeDialogActive = !_window.IsActive &&
                                         NativeDialogControllerBridge.IsMessageBoxActiveForCurrentProcess();
            if (!_window.IsActive && !nativeDialogActive)
            {
                _repeater.Reset();
                return;
            }

            if (!_input.TryGetState(out ControllerState state))
            {
                _repeater.Reset();
                return;
            }

            ControllerButton actions = _repeater.Update(state.Buttons, _clock.Elapsed);
            if (actions == ControllerButton.None)
                return;

            _inputMode.EnterControllerMode();
            if (nativeDialogActive)
            {
                NativeDialogControllerBridge.Send(actions);
                return;
            }

            ProcessAction(actions);
        }

        internal void ProcessAction(ControllerButton actions)
        {
            FrameworkElement scope = _navigator.FindActiveScope();
            if (scope is ControllerTextInputOverlay textInput &&
                (actions & ControllerButton.SubmitTextInput) != 0)
            {
                textInput.Accept();
                return;
            }

            Control current = _interaction.GetFocusedControlOverride(scope)
                           ?? _navigator.FindFocusedControl(scope);
            if (current == null)
            {
                _navigator.FocusInitialControl(scope);
                current = _interaction.GetFocusedControlOverride(scope)
                       ?? _navigator.FindFocusedControl(scope);
                if (current == null)
                    return;

                const ControllerButton tabActions = ControllerButton.PreviousTab
                                                  | ControllerButton.NextTab
                                                  | ControllerButton.PreviousRootTab
                                                  | ControllerButton.NextRootTab;
                if ((actions & tabActions) == 0)
                    return;
            }

            _focus.EnsureControllerAppearance(current, _interaction.IsEditing(current));

            if ((actions & ControllerButton.ToggleTooltip) != 0)
            {
                _tooltips.Toggle(current);
                return;
            }

            if (_interaction.HandleEditingAction(actions))
                return;

            if ((actions & ControllerButton.Cancel) != 0)
            {
                if (!_interaction.TryCancel())
                    _navigator.Cancel(scope, current);
            }
            else if ((actions & ControllerButton.Confirm) != 0)
                _interaction.Activate(current);
            else if ((actions & ControllerButton.PreviousRootTab) != 0)
                _navigator.SwitchTab(scope, current, -1, true);
            else if ((actions & ControllerButton.NextRootTab) != 0)
                _navigator.SwitchTab(scope, current, 1, true);
            else if ((actions & ControllerButton.PreviousTab) != 0)
                _navigator.SwitchTab(scope, current, -1, false);
            else if ((actions & ControllerButton.NextTab) != 0)
                _navigator.SwitchTab(scope, current, 1, false);
            else if ((actions & ControllerButton.Up) != 0)
                Move(scope, current, NavigationDirection.Up);
            else if ((actions & ControllerButton.Down) != 0)
                Move(scope, current, NavigationDirection.Down);
            else if ((actions & ControllerButton.Left) != 0)
                Move(scope, current, NavigationDirection.Left);
            else if ((actions & ControllerButton.Right) != 0)
                Move(scope, current, NavigationDirection.Right);
        }

        private void Move(FrameworkElement scope, Control current, NavigationDirection direction)
        {
            if (!_interaction.TryHandleDirection(current, direction))
                _navigator.Move(scope, current, direction);
        }

        private void OnWindowClosed(Object sender, EventArgs e) => Dispose();

        private sealed class EmptyDisposable : IDisposable
        {
            public static readonly EmptyDisposable Instance = new EmptyDisposable();
            public void Dispose() { }
        }
    }
}
