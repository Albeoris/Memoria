using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Ini;
using Microsoft.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable ArrangeStaticMemberQualifier
#pragma warning disable 649
#pragma warning disable 169

namespace Memoria.Launcher
{
    public sealed class GameSettingsControl : UiGrid, INotifyPropertyChanged
    {
        public GameSettingsControl()
        {
            foreach (UInt16 frequency in EnumerateAudioSettings())
                _validSamplingFrequency.Add(frequency);

            SetRows(20);
            SetCols(8);

            Width = 260;
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new Thickness(5);
            DataContext = this;

            Thickness rowMargin = new Thickness(0, 7, 0, 5);

            UiTextBlock monitortext = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.ActiveMonitor), row: 0, col: 0, rowSpan: 3, colSpan: 8);
            monitortext.Margin = rowMargin;
            monitortext.ToolTip = Lang.Settings.ActiveMonitor_Tooltip;
            UiComboBox monitor = AddUiElement(UiComboBoxFactory.Create(), row: 2, col: 0, rowSpan: 3, colSpan: 8);
            monitor.ItemsSource = GetAvailableMonitors();
            monitor.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(ActiveMonitor)) { Mode = BindingMode.TwoWay });
            monitor.Margin = rowMargin;

            UiTextBlock windowModetext = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.WindowMode), row: 5, col: 0, rowSpan: 3, colSpan: 8);
            windowModetext.Margin = rowMargin;
            windowModetext.ToolTip = Lang.Settings.WindowMode_Tooltip;
            UiComboBox windowMode = AddUiElement(UiComboBoxFactory.Create(), row: 7, col: 0, rowSpan: 3, colSpan: 8);
            windowMode.ItemsSource = EnumerateWindowModeSettings().ToArray();
            windowMode.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(WindowMode)) { Mode = BindingMode.TwoWay });
            windowMode.Margin = rowMargin;

            UiTextBlock resolutiontext = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.Resolution), row: 10, col: 0, rowSpan: 3, colSpan: 3);
            resolutiontext.Margin = rowMargin;
            resolutiontext.ToolTip = Lang.Settings.Resolution_Tooltip;
            UiComboBox resolution = AddUiElement(UiComboBoxFactory.Create(), row: 10, col: 3, rowSpan: 3, colSpan: 5);
            resolution.ItemsSource = EnumerateDisplaySettings().OrderByDescending(x => Convert.ToInt32(x.Split('x')[0])).ToArray();
            resolution.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(ScreenResolution)) { Mode = BindingMode.TwoWay });
            resolution.Margin = rowMargin;

            /*UiTextBlock _audioText = UiTextBlockFactory.Create(Lang.Settings.AudioSamplingFrequency);
            _audioText.FontSize *= 0.8;
            _audioText.TextWrapping = TextWrapping.WrapWithOverflow;
            AddUiElement(_audioText, row: 13, col: 0, rowSpan: 3, colSpan: 2).Margin = rowMargin;
            UiComboBox audio = AddUiElement(UiComboBoxFactory.Create(), row: 13, col: 2, rowSpan: 3, colSpan: 2);
            audio.ItemStringFormat = Lang.Settings.AudioSamplingFrequencyFormat;
            audio.ItemsSource = EnumerateAudioSettings().ToArray();
            audio.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(AudioFrequency)) {Mode = BindingMode.TwoWay});
            audio.SetBinding(Selector.IsEnabledProperty, new Binding(nameof(AudioFrequencyEnabled)) {Mode = BindingMode.TwoWay});
            audio.Margin = rowMargin;*/

            UiCheckBox x64 = AddUiElement(UiCheckBoxFactory.Create(" X64", null), 13, 0, 3, 4);
            x64.Margin = rowMargin;
            x64.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(IsX64)) { Mode = BindingMode.TwoWay });
            x64.SetBinding(ToggleButton.IsEnabledProperty, new Binding(nameof(IsX64Enabled)) { Mode = BindingMode.TwoWay });
            x64.ToolTip = Lang.Settings.Xsixfour_Tooltip;

            UiCheckBox debuggableCheckBox = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Debuggable, null), 13, 3, 3, 5);
            debuggableCheckBox.Margin = rowMargin;
            debuggableCheckBox.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(IsDebugMode)) { Mode = BindingMode.TwoWay });
            debuggableCheckBox.ToolTip = Lang.Settings.Debuggable_Tooltip;

            UiCheckBox checkUpdates = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.CheckUpdates, null), 15, 0, 3, 8);
            checkUpdates.Margin = rowMargin;
            checkUpdates.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(CheckUpdates)) { Mode = BindingMode.TwoWay });
            checkUpdates.ToolTip = Lang.Settings.CheckUpdates_Tooltip;

            UiCheckBox steamOverlayFix = AddUiElement(UiCheckBoxFactory.Create(Lang.SteamOverlay.OptionLabel, null), 17, 0, 3, 8);
            steamOverlayFix.Margin = rowMargin;
            steamOverlayFix.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(SteamOverlayFix)) { Mode = BindingMode.TwoWay });
            steamOverlayFix.ToolTip = Lang.Settings.SteamOverlayFix_Tooltip;

            foreach (FrameworkElement child in Children)
            {
                //if (!ReferenceEquals(child, backround))
                child.Margin = new Thickness(child.Margin.Left + 8, child.Margin.Top, child.Margin.Right + 8, child.Margin.Bottom);

                TextBlock textblock = child as TextBlock;
                if (textblock != null)
                {
                    textblock.Foreground = Brushes.WhiteSmoke;
                    textblock.FontWeight = FontWeight.FromOpenTypeWeight(500);
                    continue;
                }

                Control control = child as Control;
                if (control != null && !(control is ComboBox))
                    control.Foreground = Brushes.WhiteSmoke;
            }
            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
            //LoadSettings("");
        }

        #region Properties

        public String ScreenResolution
        {
            get { return _resolution; }
            set
            {
                if (_resolution != value)
                {
                    _resolution = value;
                    OnPropertyChanged();
                }
            }
        }

        public String WindowMode
        {
            get { return _windowMode; }
            set
            {
                if (_windowMode != value)
                {
                    _windowMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public String ActiveMonitor
        {
            get { return _activeMonitor; }
            set
            {
                if (_activeMonitor != value)
                {
                    _activeMonitor = value;
                    OnPropertyChanged();
                }
            }
        }

        public UInt16 AudioFrequency
        {
            get { return _audioFrequency; }
            set
            {
                if (_audioFrequency != value)
                {
                    if (MessageBox.Show((System.Windows.Window)this.GetRootElement(), Lang.SdLib.AreYouSure, Lang.SdLib.Caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Boolean? x64 = TryWriteAudioSamplingFrequency(true, value, false);
                        Boolean? x86 = TryWriteAudioSamplingFrequency(false, value, false);
                        if (x64 == true && x86 == true)
                        {
                            MessageBox.Show((System.Windows.Window)this.GetRootElement(), Lang.SdLib.SuccessBoth, Lang.SdLib.Caption, MessageBoxButton.OK, MessageBoxImage.Information);
                            _audioFrequency = value;
                            OnPropertyChanged();
                        }
                        else if (x64 == true)
                        {
                            MessageBox.Show((System.Windows.Window)this.GetRootElement(), Lang.SdLib.SuccessX64 , Lang.SdLib.Caption, MessageBoxButton.OK, MessageBoxImage.Warning);
                            _audioFrequency = value;
                            OnPropertyChanged();
                        }
                        else if (x86 == true)
                        {
                            MessageBox.Show((System.Windows.Window)this.GetRootElement(), Lang.SdLib.SuccessX86, Lang.SdLib.Caption, MessageBoxButton.OK, MessageBoxImage.Warning);
                            _audioFrequency = value;
                            OnPropertyChanged();
                        }
                        else
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => OnPropertyChanged()), DispatcherPriority.ContextIdle, null);
                            MessageBox.Show((System.Windows.Window)this.GetRootElement(), Lang.SdLib.Fail, Lang.SdLib.Caption, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => OnPropertyChanged()), DispatcherPriority.ContextIdle, null);
                    }
                }
            }
        }

        public Boolean AudioFrequencyEnabled
        {
            get { return _audioFrequencyEnabled; }
            set
            {
                if (_audioFrequencyEnabled != value)
                {
                    _audioFrequencyEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsX64
        {
            get { return _isX64; }
            set
            {
                if (_isX64 != value)
                {
                    _isX64 = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsX64Enabled
        {
            get { return _isX64Enabled; }
            set
            {
                if (_isX64Enabled != value)
                {
                    _isX64Enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsDebugMode
        {
            get { return _isDebugMode; }
            set
            {
                if (_isDebugMode != value)
                {
                    _isDebugMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public String[] DownloadMirrors
        {
            get { return _downloadMirrors; }
            set
            {
                if (_downloadMirrors != value)
                {
                    _downloadMirrors = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean CheckUpdates
        {
            get => _checkUpdates;
            set
            {
                if (_checkUpdates != value)
                {
                    _checkUpdates = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean SteamOverlayFix
        {
            get => IsSteamOverlayFixed();
            set
            {
                MessageBoxResult ShowMessage(String message, MessageBoxButton button, MessageBoxImage image)
                {
                    return MessageBox.Show((System.Windows.Window) this.GetRootElement(), message, Lang.SteamOverlay.Caption, button, image);
                }

                if (IsSteamOverlayFixed() == value)
                    return;
                
                if (value)
                {
                    if (ShowMessage(Lang.SteamOverlay.FixAreYouSure, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => OnPropertyChanged()), DispatcherPriority.ContextIdle, null);
                        return;
                    }

                    String currentLauncherPath = Process.GetCurrentProcess().MainModule.FileName;
                    
                    Process process = Process.Start(new ProcessStartInfo("Memoria.SteamFix.exe", @$" ""{currentLauncherPath}"" ") {Verb = "runas"});
                    process.WaitForExit();
                }
                else
                {
                    if (ShowMessage(Lang.SteamOverlay.RollbackAreYouSure, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => OnPropertyChanged()), DispatcherPriority.ContextIdle, null);
                        return;
                    }

                    Process process = Process.Start(new ProcessStartInfo("Memoria.SteamFix.exe") {Verb = "runas"});
                    process.WaitForExit();
                }
                
                Application.Current.Dispatcher.BeginInvoke(new Action(() => OnPropertyChanged()), DispatcherPriority.ContextIdle, null);
            }
        }

        public Boolean AutoRunGame { get; private set; }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private async void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                IniFile iniFile = new IniFile(IniPath);
                switch (propertyName)
                {
                    case nameof(ScreenResolution):
                        iniFile.WriteValue("Settings", propertyName, ScreenResolution ?? "1280x960");
                        break;
                    case nameof(ActiveMonitor):
                        iniFile.WriteValue("Settings", propertyName, ActiveMonitor ?? String.Empty);
                        break;
                    case nameof(WindowMode):
                        iniFile.WriteValue("Settings", propertyName, WindowMode ?? Lang.Settings.Window);
                        break;
                    case nameof(IsDebugMode):
                        iniFile.WriteValue("Memoria", propertyName, IsDebugMode.ToString());
                        break;
                    case nameof(IsX64):
                        iniFile.WriteValue("Memoria", propertyName, IsX64.ToString());
                        break;
                    case nameof(CheckUpdates):
                    {
                        iniFile.WriteValue("Memoria", propertyName, CheckUpdates.ToString());
                        if (CheckUpdates)
                        {
                            using (ManualResetEvent evt = new ManualResetEvent(false))
                            {
                                System.Windows.Window root = this.GetRootElement() as System.Windows.Window;
                                if (root != null)
                                    await UiLauncherPlayButton.CheckUpdates(root, evt, this);
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        #endregion

        public static readonly String IniPath = AppDomain.CurrentDomain.BaseDirectory + "\\Settings.ini";
        public static readonly String MemoriaIniPath = AppDomain.CurrentDomain.BaseDirectory + @"Memoria.ini";

        private readonly HashSet<UInt16> _validSamplingFrequency = new HashSet<UInt16>();

        private String _resolution = "";
        private String _activeMonitor = "";
        private String _windowMode = "";
        private UInt16 _audioFrequency = 48000;
        private Boolean _audioFrequencyEnabled = true;
        private Boolean _isX64 = true;
        private Boolean _isX64Enabled = true;
        private Boolean _isDebugMode;
        private Boolean _checkUpdates;
        private String[] _downloadMirrors;

        private void LoadSettings()
        {
            try
            {
                IniFile iniFile = new IniFile(IniPath);

                String value = iniFile.ReadValue("Settings", nameof(ScreenResolution));
                //if res in settings.ini exists AND corresponds to something in the res list
                if ((!String.IsNullOrEmpty(value)) && EnumerateDisplaySettings().ToArray().Any(value.Contains)) 
                    _resolution = value;
                //else we choose the largest available one
                else
                    _resolution = EnumerateDisplaySettings().OrderByDescending(x => Convert.ToInt32(x.Split('x')[0])).ToArray()[0];

                value = iniFile.ReadValue("Settings", nameof(ActiveMonitor));
                if (!String.IsNullOrEmpty(value))
                    _activeMonitor = value;

                value = iniFile.ReadValue("Settings", nameof(WindowMode));
                if (!String.IsNullOrEmpty(value))
                    _windowMode = value;
                if (!EnumerateWindowModeSettings().Contains(_windowMode))
                    _windowMode = Lang.Settings.Window;

                value = iniFile.ReadValue("Memoria", nameof(IsX64));
                if (String.IsNullOrEmpty(value))
                    value = "true";
                if (!Boolean.TryParse(value, out _isX64))
                    _isX64 = true;
                if (!Environment.Is64BitOperatingSystem || !Directory.Exists("x64"))
                {
                    _isX64 = false;
                    _isX64Enabled = false;
                }
                else if (!Directory.Exists("x86"))
                {
                    _isX64 = true;
                    _isX64Enabled = false;
                }

                UInt16 x64SamplingFrequency;
                UInt16 x86SamplingFrequency;
                Boolean? x64SamplingReaded = TryReadAudioSamplingFrequency(@"x64\FF9_Data\Plugins\SdLib.dll", out x64SamplingFrequency);
                Boolean? x86SamplingReaded = TryReadAudioSamplingFrequency(@"x86\FF9_Data\Plugins\SdLib.dll", out x86SamplingFrequency);
                if (x64SamplingReaded != true && x86SamplingReaded != true)
                {
                    _audioFrequency = 48000;
                    _audioFrequencyEnabled = false;
                }
                else
                {
                    _audioFrequency = Math.Max(x86SamplingFrequency, x64SamplingFrequency);

                    if (x64SamplingFrequency < x86SamplingFrequency)
                        TryWriteAudioSamplingFrequency(true, _audioFrequency, true);
                    else if (x86SamplingFrequency < x64SamplingFrequency)
                        TryWriteAudioSamplingFrequency(false, _audioFrequency, true);
                }

                value = iniFile.ReadValue("Memoria", nameof(IsDebugMode));
                if (String.IsNullOrEmpty(value))
                    value = "false";
                if (!Boolean.TryParse(value, out _isDebugMode))
                    _isDebugMode = false;

                value = iniFile.ReadValue("Memoria", nameof(CheckUpdates));
                if (String.IsNullOrEmpty(value))
                    value = "false";
                if (!Boolean.TryParse(value, out _checkUpdates))
                    _checkUpdates = false;

                value = iniFile.ReadValue("Memoria", nameof(AutoRunGame));
                if (String.IsNullOrEmpty(value))
                    value = "false";
                AutoRunGame = App.AutoRunGame || (Boolean.TryParse(value, out var autoRunGame) && autoRunGame);

                value = iniFile.ReadValue("Memoria", nameof(DownloadMirrors));
                if (String.IsNullOrEmpty(value))
                {
                    _downloadMirrors = ["https://github.com/Albeoris/Memoria/releases/latest/download/Memoria.Patcher.exe"];
                    //if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru") _downloadMirrors = ["https://ff9.ffrtt.ru/rus/FF9RU.exe", "https://ff9.ffrtt.ru/rus/Memoria.Patcher.exe"];
                }
                else
                {
                    _downloadMirrors = value.Split(',');
                }

                OnPropertyChanged(nameof(ScreenResolution));
                OnPropertyChanged(nameof(ActiveMonitor));
                OnPropertyChanged(nameof(WindowMode));
                OnPropertyChanged(nameof(AudioFrequency));
                OnPropertyChanged(nameof(AudioFrequencyEnabled));
                OnPropertyChanged(nameof(IsX64));
                OnPropertyChanged(nameof(IsX64Enabled));
                OnPropertyChanged(nameof(IsDebugMode));
                OnPropertyChanged(nameof(CheckUpdates));
                OnPropertyChanged(nameof(DownloadMirrors));
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        private Boolean IsSteamOverlayFixed()
        {
            try
            {
                using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default))
                {
                    using (var subKey = registryKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\FF9_Launcher.exe"))
                    {
                        if (subKey?.GetValue("Debugger") == null)
                            return false;
                    }
                }

                var bak = new FileInfo("FF9_Launcher.bak");
                var exe = new FileInfo("FF9_Launcher.exe");

                // Patch again if FF9_Launcher.exe was rewrited
                if (bak.Exists && exe.Exists && bak.Length != exe.Length)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private Boolean? TryReadAudioSamplingFrequency(String sdlibPath, out UInt16 samplingFrequency)
        {
            try
            {
                const Int64 offset1 = 0x3E52;
                const Int64 offset2 = 0xFC38;

                if (!File.Exists(sdlibPath))
                {
                    samplingFrequency = 0;
                    return null;
                }

                Byte[] buff = new Byte[2];

                using (FileStream input = File.OpenRead(sdlibPath))
                {
                    if (input.Length <= offset2)
                    {
                        samplingFrequency = 0;
                        return false;
                    }

                    // Read value from by first offset
                    input.Seek(offset1, SeekOrigin.Begin);
                    if (input.Read(buff, 0, buff.Length) != buff.Length)
                        throw new InvalidOperationException(Lang.Error.File.EndOfStream);
                    samplingFrequency = BitConverter.ToUInt16(buff, 0);

                    // Check the value
                    if (!_validSamplingFrequency.Contains(samplingFrequency))
                    {
                        samplingFrequency = 0;
                        return false;
                    }

                    // Check values are same
                    input.Seek(offset2, SeekOrigin.Begin);
                    if (input.Read(buff, 0, buff.Length) != buff.Length)
                        throw new InvalidOperationException(Lang.Error.File.EndOfStream);
                    if (samplingFrequency != BitConverter.ToUInt16(buff, 0))
                    {
                        samplingFrequency = 0;
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show((System.Windows.Window) this.GetRootElement(), Lang.SdLib.CannotRead + $" {sdlibPath}{Environment.NewLine}{Environment.NewLine}{ex}", Lang.Message.Error.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                samplingFrequency = 0;
                return false;
            }
        }

        private Boolean? TryWriteAudioSamplingFrequency(Boolean isX64, UInt16 samplingFrequency, Boolean quiet)
        {
            try
            {
                const Int64 x64offset1 = 0x3E52;
                const Int64 x64offset2 = 0xFC38;

                const Int64 x86offset1 = 0x452D;
                const Int64 x86offset2 = 0xE59D;

                Int64 offset1, offset2;
                String sdlibPath = @"\FF9_Data\Plugins\SdLib.dll";
                if (isX64)
                {
                    offset1 = x64offset1;
                    offset2 = x64offset2;
                    sdlibPath = "x64" + sdlibPath;
                }
                else
                {
                    offset1 = x86offset1;
                    offset2 = x86offset2;
                    sdlibPath = "x86" + sdlibPath;
                }

                if (!File.Exists(sdlibPath))
                    return null;

                Byte[] buff = new Byte[2];

                using (FileStream input = File.Open(sdlibPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    if (input.Length <= offset2)
                        return false;

                    // Read value from by first offset
                    input.Seek(offset1, SeekOrigin.Begin);
                    if (input.Read(buff, 0, buff.Length) != buff.Length)
                        throw new InvalidOperationException(Lang.Error.File.EndOfStream);
                    UInt16 originalFrequency = BitConverter.ToUInt16(buff, 0);

                    // Check the value
                    if (!_validSamplingFrequency.Contains(originalFrequency))
                        return false;

                    // Check values are same
                    input.Seek(offset2, SeekOrigin.Begin);
                    if (input.Read(buff, 0, buff.Length) != buff.Length)
                        throw new InvalidOperationException(Lang.Error.File.EndOfStream);
                    if (originalFrequency != BitConverter.ToUInt16(buff, 0))
                        return false;

                    buff = BitConverter.GetBytes(samplingFrequency);

                    input.Seek(offset2, SeekOrigin.Begin);
                    input.Write(buff, 0, buff.Length);

                    input.Seek(offset1, SeekOrigin.Begin);
                    input.Write(buff, 0, buff.Length);

                    return true;
                }
            }
            catch (Exception ex)
            {
                if (!quiet)
                    MessageBox.Show((System.Windows.Window)this.GetRootElement(), Lang.SdLib.CannotWrite + $" ({(isX64 ? "x64" : "x86")}){Environment.NewLine}{Environment.NewLine}{ex}", Lang.Message.Error.Title, MessageBoxButton.OK, MessageBoxImage.Warning);

                samplingFrequency = 0;
                return false;
            }
        }

        [DllImport("user32.dll")]
        private static extern Boolean EnumDisplaySettings(String deviceName, Int32 modeNum, ref DevMode devMode);

        private IEnumerable<String> EnumerateDisplaySettings()
        {
            HashSet<String> set = new HashSet<String>();
            DevMode devMode = new DevMode();
            Int32 modeNum = 0;
            while (EnumDisplaySettings(null, modeNum++, ref devMode))
            {
                if (devMode.dmPelsWidth >= 640 && devMode.dmPelsHeight >= 480)
                {
                    String resolution = $"{devMode.dmPelsWidth.ToString(CultureInfo.InvariantCulture)}x{devMode.dmPelsHeight.ToString(CultureInfo.InvariantCulture)}";
                    String ratio = "";

                    if ((devMode.dmPelsWidth / 16) == (devMode.dmPelsHeight / 9)) ratio = " | 16:9";
                    else if ((devMode.dmPelsWidth / 8) == (devMode.dmPelsHeight / 5)) ratio = " | 16:10";
                    else if ((devMode.dmPelsWidth / 4) == (devMode.dmPelsHeight / 3)) ratio = " | 4:3";
                    else if ((devMode.dmPelsWidth / 14) == (devMode.dmPelsHeight / 9)) ratio = " | 14:9";
                    else if ((devMode.dmPelsWidth / 32) == (devMode.dmPelsHeight / 9)) ratio = " | 32:9";
                    else if ((devMode.dmPelsWidth / 64) == (devMode.dmPelsHeight / 27)) ratio = " | 64:27";
                    else if ((devMode.dmPelsWidth / 3) == (devMode.dmPelsHeight / 2)) ratio = " | 3:2";
                    else if ((devMode.dmPelsWidth / 5) == (devMode.dmPelsHeight / 4)) ratio = " | 5:4";
                    else if ((devMode.dmPelsWidth / 256) == (devMode.dmPelsHeight / 135)) ratio = " | 256:135";
                    else if ((devMode.dmPelsWidth / 25) == (devMode.dmPelsHeight / 16)) ratio = " | 25:16";
                    else if ((devMode.dmPelsWidth) == (devMode.dmPelsHeight)) ratio = " | 1:1";

                    resolution += ratio; 

                    if (set.Add(resolution))
                        yield return resolution;
                }
            }
        }

        private String[] GetAvailableMonitors()
        {
            Screen[] allScreens = Screen.AllScreens;
            Dictionary<Int32, String> friendlyNames = ScreenInterrogatory.GetAllMonitorFriendlyNamesSafe();
            String[] result = new String[allScreens.Length];
            for (Int32 index = 0; index < allScreens.Length; index++)
            {
                Screen screen = allScreens[index];
                StringBuilder sb = new StringBuilder();
                sb.Append(index);
                sb.Append(" - ");

                String name;
                if (!friendlyNames.TryGetValue(index, out name))
                    name = screen.DeviceName;
                sb.Append(name);

                if (screen.Primary)
                    sb.Append(Lang.Settings.PrimaryMonitor);

                result[index] = sb.ToString();

                if (screen.Primary)
                    _activeMonitor = result[index];
            }
            return result;
        }

        private IEnumerable<UInt16> EnumerateAudioSettings()
        {
            yield return 48000; // standard audio sampling rate used by professional digital video equipment such as tape recorders, video servers, vision mixers and so on.
            yield return 47250; // world's first commercial PCM sound recorder by Nippon Columbia (Denon)
            yield return 44100; // Audio CD, also most commonly used with MPEG-1 audio (VCD, SVCD, MP3). Originally chosen by Sony because it could be recorded on modified video equipment running at either 25 frames per second (PAL) or 30 frame/s (using an NTSC monochrome video recorder) and cover the 20 kHz bandwidth thought necessary to match professional analog recording equipment of the time. A PCM adaptor would fit digital audio samples into the analog video channel of, for example, PAL video tapes using 3 samples per line, 588 lines per frame, 25 frames per second.
            yield return 44056; // Used by digital audio locked to NTSC color video signals (3 samples per line, 245 lines per field, 59.94 fields per second = 29.97 frames per second).
            yield return 37800; // CD-XA audio
            yield return 32000; // miniDV digital video camcorder, video tapes with extra channels of audio (e.g. DVCAM with 4 Channels of audio), DAT (LP mode), Germany's Digitales Satellitenradio, NICAM digital audio, used alongside analogue television sound in some countries. High-quality digital wireless microphones.[12] Suitable for digitizing FM radio.[citation needed]
        }

        private IEnumerable<String> EnumerateWindowModeSettings()
        {
            yield return Lang.Settings.Window; // Unity Player launches in a window with an OS-styled title-bar. Can be moved around by dragging the title-bar
            yield return Lang.Settings.ExclusiveFullscreen; // Unity Player launches in full screen on the selected monitor. Screen disappears if the application loses focus (IE, by clicking on another application)
            yield return Lang.Settings.BorderlessFullscreen; // If the resolution matches the target display then Unity Player launches in borderless fullscreen mode on that display. Unlike exclusive fullscreen the player will not disappear if the app loses focus. If the resolution is smaller than the display resolution then Unity player will launch as a window without a title-bar (sized to the chosen resolution). 
        }

        private struct DevMode
        {
            private const Int32 CCHDEVICENAME = 32;
            private const Int32 CCHFORMNAME = 32;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public String dmDeviceName;
            public Int16 dmSpecVersion;
            public Int16 dmDriverVersion;
            public Int16 dmSize;
            public Int16 dmDriverExtra;
            public Int32 dmFields;
            public Int32 dmPositionX;
            public Int32 dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public Int32 dmDisplayFixedOutput;
            public Int16 dmColor;
            public Int16 dmDuplex;
            public Int16 dmYResolution;
            public Int16 dmTTOption;
            public Int16 dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public String dmFormName;
            public Int16 dmLogPixels;
            public Int32 dmBitsPerPel;
            public Int32 dmPelsWidth;
            public Int32 dmPelsHeight;
            public Int32 dmDisplayFlags;
            public Int32 dmDisplayFrequency;
            public Int32 dmICMMethod;
            public Int32 dmICMIntent;
            public Int32 dmMediaType;
            public Int32 dmDitherType;
            public Int32 dmReserved1;
            public Int32 dmReserved2;
            public Int32 dmPanningWidth;
            public Int32 dmPanningHeight;
        }
    }
}