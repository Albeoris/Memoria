using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Memoria.Launcher.Utils.Downloads;

namespace Memoria.Launcher.Utils.Updates
{
    public sealed class UiLauncherUpdatePanel : UserControl, IDisposable
    {
        private readonly Logger _log = AppLogger.GetLogger(nameof(UiLauncherUpdatePanel));
        private readonly LauncherUpdateService _updateService = new LauncherUpdateService();
        private readonly LauncherUpdateInstaller _installer = new LauncherUpdateInstaller();
        private readonly Dictionary<UpdateBuildKind, Button> _buttons = new Dictionary<UpdateBuildKind, Button>();
        private readonly Dictionary<UpdateBuildKind, UpdateBuildInfo> _availableBuilds = new Dictionary<UpdateBuildKind, UpdateBuildInfo>();
        private readonly HashSet<UpdateBuildKind> _failedChecks = new HashSet<UpdateBuildKind>();
        private readonly HashSet<UpdateBuildKind> _checkingBuilds = new HashSet<UpdateBuildKind>();
        private readonly SemaphoreSlim _operationGate = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource _lifetimeCancellation = new CancellationTokenSource();
        private SettingsGrid_Vanilla _settings;
        private DateTime _currentVersionUtc;
        private CancellationTokenSource _automaticCheckCancellation;
        private Int32 _automaticRefreshGeneration;
        private Boolean _busy;
        private Boolean _disposed;

        public UiLauncherUpdatePanel()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            for (Int32 index = 0; index < UpdateBuildCatalog.All.Count; index++)
            {
                UpdateBuild build = UpdateBuildCatalog.All[index];
                Button button = new Button { Height = 46, Margin = index == 0 ? new Thickness(0, 0, 3, 0) : new Thickness(3, 0, 0, 0), Padding = new Thickness(3), FontSize = 11, FontFamily = new FontFamily("Segoe UI"), Cursor = System.Windows.Input.Cursors.Hand, Tag = build };
                button.SetResourceReference(StyleProperty, "ButtonStyle");
                button.Click += OnBuildClick;
                Grid.SetColumn(button, index);
                grid.Children.Add(button);
                _buttons.Add(build.Kind, button);
                SetButtonContent(button, GetBuildName(build), GetText("Updater.WaitingStatus"));
            }
            Content = grid;
        }

        public async Task InitializeAsync(SettingsGrid_Vanilla settings, DateTime currentVersion)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _currentVersionUtc = currentVersion.Kind == DateTimeKind.Utc ? currentVersion : currentVersion.Kind == DateTimeKind.Local ? currentVersion.ToUniversalTime() : DateTime.SpecifyKind(currentVersion, DateTimeKind.Utc);
            _settings.PropertyChanged += OnSettingsChanged;
            RefreshLanguage();
            if (_settings.CheckUpdates)
                await RefreshAllAsync();
        }

        public void RefreshLanguage()
        {
            foreach (UpdateBuild build in UpdateBuildCatalog.All)
                RefreshButton(build);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _lifetimeCancellation.Cancel();
            if (_settings != null)
                _settings.PropertyChanged -= OnSettingsChanged;
            InvalidateAutomaticRefresh();
            foreach (Button button in _buttons.Values)
                button.Click -= OnBuildClick;
        }

        private async void OnSettingsChanged(Object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SettingsGrid_Vanilla.CheckUpdates))
                return;

            try
            {
                if (_settings.CheckUpdates)
                    await RefreshAllAsync();
                else
                {
                    InvalidateAutomaticRefresh();
                    _availableBuilds.Clear();
                    _failedChecks.Clear();
                    RefreshLanguage();
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _log.Error(exception, "Unexpected failure while changing automatic update checks.");
            }
        }

        private async void OnBuildClick(Object sender, RoutedEventArgs e)
        {
            if (_busy || !(sender is Button button) || !(button.Tag is UpdateBuild build))
                return;

            Boolean gateEntered = false;
            Boolean resumeAutomaticRefresh = false;
            _busy = true;
            SetButtonsEnabled(false);
            try
            {
                resumeAutomaticRefresh = _automaticCheckCancellation != null;
                InvalidateAutomaticRefresh();
                await _operationGate.WaitAsync(_lifetimeCancellation.Token);
                gateEntered = true;
                if (_disposed)
                    return;

                if (!_availableBuilds.TryGetValue(build.Kind, out UpdateBuildInfo buildInfo))
                {
                    await CheckOneAsync(build, _lifetimeCancellation.Token);
                    return;
                }

                if (!ConfirmInstall(buildInfo))
                    return;

                await DownloadAndInstallAsync(buildInfo);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _log.Error(exception, "Launcher update interaction failed for the {Build} build.", build.Name);
                MessageBox.Show(GetOwner(), GetText("Updater.DownloadFailed"), GetText("Launcher.ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (gateEntered)
                    _operationGate.Release();
                _busy = false;
                if (!_disposed)
                    RefreshLanguage();
                if (!_disposed && resumeAutomaticRefresh && _settings?.CheckUpdates == true)
                    await RefreshAllAsync();
            }
        }

        private async Task RefreshAllAsync()
        {
            Int32 generation = ++_automaticRefreshGeneration;
            _automaticCheckCancellation?.Cancel();
            CancellationTokenSource cancellation = null;
            await _operationGate.WaitAsync(_lifetimeCancellation.Token);
            try
            {
                if (_disposed || generation != _automaticRefreshGeneration || _settings?.CheckUpdates != true)
                    return;

                cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeCancellation.Token);
                _automaticCheckCancellation = cancellation;
                Task[] checks = new Task[UpdateBuildCatalog.All.Count];
                for (Int32 index = 0; index < UpdateBuildCatalog.All.Count; index++)
                    checks[index] = CheckOneAsync(UpdateBuildCatalog.All[index], cancellation.Token);
                await Task.WhenAll(checks);
            }
            finally
            {
                if (ReferenceEquals(_automaticCheckCancellation, cancellation))
                    _automaticCheckCancellation = null;
                cancellation?.Dispose();
                _operationGate.Release();
            }
        }

        private async Task CheckOneAsync(UpdateBuild build, CancellationToken cancellationToken)
        {
            _checkingBuilds.Add(build.Kind);
            RefreshButton(build);
            try
            {
                UpdateBuildInfo buildInfo = await _updateService.CheckAsync(build, cancellationToken);
                _availableBuilds[build.Kind] = buildInfo;
                _failedChecks.Remove(build.Kind);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                _availableBuilds.Remove(build.Kind);
                _failedChecks.Add(build.Kind);
                _log.Error(exception, "Unable to display the {Build} launcher build.", build.Name);
            }
            finally
            {
                _checkingBuilds.Remove(build.Kind);
                RefreshButton(build);
            }
        }

        private Boolean ConfirmInstall(UpdateBuildInfo buildInfo)
        {
            UpdateVersionRelation relation = UpdateVersionComparer.Compare(_currentVersionUtc, buildInfo.PublishedAtUtc);
            String messageKey = relation == UpdateVersionRelation.Downgrade ? "Updater.ConfirmDowngrade" : relation == UpdateVersionRelation.Upgrade ? "Updater.ConfirmUpgrade" : "Updater.ConfirmReinstall";
            String message = String.Format(CultureInfo.CurrentCulture, GetText(messageKey), FormatDateWithUtc(_currentVersionUtc), FormatDateWithUtc(buildInfo.PublishedAtUtc), GetBuildName(buildInfo.Build));
            MessageBoxImage image = relation == UpdateVersionRelation.Downgrade ? MessageBoxImage.Warning : MessageBoxImage.Question;
            return MessageBox.Show(GetOwner(), message, GetText("Updater.ConfirmTitle"), MessageBoxButton.YesNo, image) == MessageBoxResult.Yes;
        }

        private async Task DownloadAndInstallAsync(UpdateBuildInfo buildInfo)
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            using UpdateDownloadWindow progressWindow = new UpdateDownloadWindow(String.Format(CultureInfo.CurrentCulture, GetText("Updater.DownloadingBuild"), GetBuildName(buildInfo.Build)), cancellation) { Owner = GetOwner() };
            IProgress<DownloadProgress> progress = new Progress<DownloadProgress>(progressWindow.Report);
            Task<PendingUpdatePackage> downloadTask = _updateService.DownloadAsync(buildInfo, Path.GetFullPath("./"), progress, cancellation.Token);
            Task closeTask = CloseWindowWhenDownloadFinishesAsync(downloadTask, progressWindow);
            progressWindow.ShowDialog();

            PendingUpdatePackage package;
            try
            {
                package = await downloadTask;
            }
            catch (Exception exception) when (progressWindow.UserCancellationRequested || cancellation.IsCancellationRequested)
            {
                _log.Info(exception, "The {Build} launcher build download ended after the user cancelled it; suppressing the user-facing error.", buildInfo.Build.Name);
                await closeTask;
                return;
            }

            using (package)
            {
                await closeTask;
                if (progressWindow.UserCancellationRequested || cancellation.IsCancellationRequested)
                    return;

                String patcherPath = package.Commit(cancellation.Token);
                _installer.Start(patcherPath);
                Environment.Exit(2);
            }
        }

        private static async Task CloseWindowWhenDownloadFinishesAsync(Task downloadTask, UpdateDownloadWindow window)
        {
            try
            {
                await downloadTask;
            }
            catch
            {
            }
            finally
            {
                if (!window.UserCancellationRequested)
                    window.CompleteAndClose();
            }
        }

        private void RefreshButton(UpdateBuild build)
        {
            Button button = _buttons[build.Kind];
            SetButtonTooltip(button, GetBuildTooltipWithRecommendation(build));
            button.IsEnabled = !_busy && !_checkingBuilds.Contains(build.Kind);

            if (_checkingBuilds.Contains(build.Kind))
            {
                SetButtonContent(button, GetBuildName(build), GetText("Updater.CheckingStatus"));
                button.Opacity = 1;
                return;
            }

            if (_availableBuilds.TryGetValue(build.Kind, out UpdateBuildInfo info))
            {
                SetButtonContent(button, GetBuildName(build), FormatLocalDate(info.PublishedAtUtc));
                button.Opacity = UpdateVersionComparer.Compare(_currentVersionUtc, info.PublishedAtUtc) == UpdateVersionRelation.Downgrade ? 0.45 : 1;
                return;
            }

            if (_failedChecks.Contains(build.Kind))
            {
                SetButtonContent(button, GetBuildName(build), GetText("Updater.UnavailableStatus"));
                SetButtonTooltip(button, GetBuildTooltipWithRecommendation(build) + Environment.NewLine + Environment.NewLine + GetText("Updater.CheckFailedTooltip"));
                button.Opacity = 0.65;
                return;
            }

            if (_settings == null || !_settings.CheckUpdates)
            {
                SetButtonContent(button, GetBuildName(build), _settings == null ? GetText("Updater.WaitingStatus") : GetText("Updater.CheckStatus"));
                button.Opacity = 1;
                return;
            }

            SetButtonContent(button, GetBuildName(build), GetText("Updater.WaitingStatus"));
            button.Opacity = 1;
        }

        private String GetBuildTooltip(UpdateBuild build)
        {
            return GetText(build.Kind == UpdateBuildKind.Stable ? "Updater.StableTooltip" : "Updater.CanaryTooltip");
        }

        private String GetBuildTooltipWithRecommendation(UpdateBuild build)
        {
            return GetBuildTooltip(build) + (_settings?.CheckUpdates != true ? Environment.NewLine + Environment.NewLine + GetText("Updater.EnableAutoCheckRecommendation") : String.Empty);
        }

        private static String GetBuildName(UpdateBuild build)
        {
            return GetText(build.Kind == UpdateBuildKind.Stable ? "Updater.StableName" : "Updater.CanaryName");
        }

        private static String FormatLocalDate(DateTime value)
        {
            return UpdateTimeFormatter.FormatLocal(value, TimeZoneInfo.Local);
        }

        private static String FormatDateWithUtc(DateTime value)
        {
            return UpdateTimeFormatter.FormatWithUtc(value, TimeZoneInfo.Local);
        }

        private static void SetButtonContent(Button button, String buildName, String status)
        {
            button.Content = new TextBlock { Text = buildName + Environment.NewLine + status, TextAlignment = TextAlignment.Center, TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        }

        private static void SetButtonTooltip(Button button, String text)
        {
            button.ToolTip = new ToolTip { Content = new TextBlock { Text = text, MaxWidth = 380, TextWrapping = TextWrapping.Wrap } };
            ToolTipService.SetShowDuration(button, 30000);
        }

        private Window GetOwner()
        {
            return Window.GetWindow(this) ?? Application.Current.MainWindow;
        }

        private void SetButtonsEnabled(Boolean enabled)
        {
            foreach (Button button in _buttons.Values)
                button.IsEnabled = enabled;
        }

        private void InvalidateAutomaticRefresh()
        {
            _automaticRefreshGeneration++;
            _automaticCheckCancellation?.Cancel();
        }

        private static String GetText(String key)
        {
            return Lang.Res[key] as String ?? key;
        }
    }
}
