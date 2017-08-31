using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Ini;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MessageBox = System.Windows.MessageBox;

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

            SetCols(2);
            SetRows(8);

            Width = 200;
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new Thickness(5);
            DataContext = this;

            LinearGradientBrush backgroundStroke = new LinearGradientBrush
            {
                EndPoint = new Point(0.5, 1),
                StartPoint = new Point(0.5, 0),
                RelativeTransform = new RotateTransform(115, 0.5, 0.5),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb(0xff, 0x61, 0x61, 0x61), 0),
                    new GradientStop(Color.FromArgb(0xff, 0xF2, 0xF2, 0xF2), 0.504),
                    new GradientStop(Color.FromArgb(0xff, 0xAE, 0xB1, 0xB1), 1)
                }
            };
            backgroundStroke.Freeze();

            LinearGradientBrush backgroundFill = new LinearGradientBrush
            {
                MappingMode = BrushMappingMode.RelativeToBoundingBox,
                StartPoint = new Point(0.5, 1.0),
                EndPoint = new Point(0.5, -0.4),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb(0xBB, 0x44, 0x71, 0xc1), 0),
                    new GradientStop(Color.FromArgb(0xBB, 0x28, 0x36, 0x65), 1)
                }
            };
            backgroundFill.Freeze();

            Rectangle backround = AddUiElement(new Rectangle {Stroke = backgroundStroke, Fill = backgroundFill, StrokeThickness = 5}, 0, 0, 9, 2);

            Thickness rowMargin = new Thickness(0, 8, 0, 3);
            
            AddUiElement(UiTextBlockFactory.Create("Active monitor:"), row: 0, col: 0).Margin = rowMargin;
            UiComboBox monitor = AddUiElement(UiComboBoxFactory.Create(), row: 1, col: 0, rowSpan: 0, colSpan: 2);
            monitor.ItemsSource = GetAvailableMonitors();
            monitor.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(ActiveMonitor)) {Mode = BindingMode.TwoWay});
            monitor.Margin = rowMargin;

            AddUiElement(UiTextBlockFactory.Create("Resolution:"), row: 2, col: 0).Margin = rowMargin;
            UiComboBox resolution = AddUiElement(UiComboBoxFactory.Create(), row: 3, col: 0);
            resolution.ItemsSource = EnumerateDisplaySettings().ToArray();
            resolution.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(ScreenResolution)) {Mode = BindingMode.TwoWay});
            resolution.Margin = rowMargin;

            UiCheckBox windowedCheckBox = AddUiElement(UiCheckBoxFactory.Create("Windowed", null), row: 3, col: 1);
            windowedCheckBox.Margin = rowMargin;
            windowedCheckBox.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Windowed)) {Mode = BindingMode.TwoWay});

            AddUiElement(UiTextBlockFactory.Create("Audio sampling frequency:"), 4, 0, 0, 2).Margin = rowMargin;
            UiComboBox audio = AddUiElement(UiComboBoxFactory.Create(), 5, 0, 0, 2);
            audio.ItemStringFormat = "{0} Hz";
            audio.ItemsSource = EnumerateAudioSettings().ToArray();
            audio.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(AudioFrequency)) {Mode = BindingMode.TwoWay});
            audio.SetBinding(Selector.IsEnabledProperty, new Binding(nameof(AudioFrequencyEnabled)) {Mode = BindingMode.TwoWay});
            audio.Margin = rowMargin;

            UiCheckBox x64 = AddUiElement(UiCheckBoxFactory.Create("X64", null), 6, 0);
            x64.Margin = rowMargin;
            x64.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(IsX64)) {Mode = BindingMode.TwoWay});
            x64.SetBinding(ToggleButton.IsEnabledProperty, new Binding(nameof(IsX64Enabled)) {Mode = BindingMode.TwoWay});

            UiCheckBox debuggableCheckBox = AddUiElement(UiCheckBoxFactory.Create("Debuggable", null), 6, 1);
            debuggableCheckBox.Margin = new Thickness(0, 8, 0, 8);
            debuggableCheckBox.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(IsDebugMode)) {Mode = BindingMode.TwoWay});

            foreach (FrameworkElement child in Children)
            {
                if (!ReferenceEquals(child, backround))
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

            LoadSettings();
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
                    if (MessageBox.Show((Window)this.GetRootElement(), "Are you sure you want to patch SdLib.dll?", "Patch SdLib.dll", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Boolean? x64 = TryWriteAudioSamplingFrequency(true, value, false);
                        Boolean? x86 = TryWriteAudioSamplingFrequency(false, value, false);
                        if (x64 == true && x86 == true)
                        {
                            MessageBox.Show((Window)this.GetRootElement(), "SdLib.dll is successfully patched for both platforms.", "Patch SdLib.dll", MessageBoxButton.OK, MessageBoxImage.Information);
                            _audioFrequency = value;
                            OnPropertyChanged();
                        }
                        else if (x64 == true)
                        {
                            MessageBox.Show((Window)this.GetRootElement(), "SdLib.dll is successfully patched for the x64 platform only.", "Patch SdLib.dll", MessageBoxButton.OK, MessageBoxImage.Warning);
                            _audioFrequency = value;
                            OnPropertyChanged();
                        }
                        else if (x86 == true)
                        {
                            MessageBox.Show((Window)this.GetRootElement(), "SdLib.dll is successfully patched for the x86 platform only.", "Patch SdLib.dll", MessageBoxButton.OK, MessageBoxImage.Warning);
                            _audioFrequency = value;
                            OnPropertyChanged();
                        }
                        else
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => OnPropertyChanged()), DispatcherPriority.ContextIdle, null);
                            MessageBox.Show((Window)this.GetRootElement(), "Unfortunately, we could not patch the SdLib.dll. Please share your version of this file.", "Patch SdLib.dll", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public Boolean Windowed
        {
            get { return _isWindowMode; }
            set
            {
                if (_isWindowMode != value)
                {
                    _isWindowMode = value;
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

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                IniFile iniFile = new IniFile(_iniPath);
                switch (propertyName)
                {
                    case nameof(ScreenResolution):
                        iniFile.WriteValue("Settings", propertyName, ScreenResolution ?? "1280x960");
                        break;
                    case nameof(ActiveMonitor):
                        iniFile.WriteValue("Settings", propertyName, ActiveMonitor ?? String.Empty);
                        break;
                    case nameof(Windowed):
                        iniFile.WriteValue("Settings", propertyName, (Windowed).ToString());
                        break;
                    case nameof(IsDebugMode):
                        iniFile.WriteValue("Memoria", propertyName, (IsDebugMode).ToString());
                        break;
                    case nameof(IsX64):
                        iniFile.WriteValue("Memoria", propertyName, (IsX64).ToString());
                        break;
                }
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        #endregion

        private readonly String _iniPath = AppDomain.CurrentDomain.BaseDirectory + "\\Settings.ini";
        private readonly HashSet<UInt16> _validSamplingFrequency = new HashSet<UInt16>();

        private String _resolution = "1280x960";
        private String _activeMonitor = "";
        private UInt16 _audioFrequency = 32000;
        private Boolean _audioFrequencyEnabled = true;
        private Boolean _isWindowMode = true;
        private Boolean _isX64 = true;
        private Boolean _isX64Enabled = true;
        private Boolean _isDebugMode;

        private void LoadSettings()
        {
            try
            {
                IniFile iniFile = new IniFile(_iniPath);

                String value = iniFile.ReadValue("Settings", nameof(ScreenResolution));
                if (String.IsNullOrEmpty(value))
                    value = "1280x960";
                _resolution = value;

                value = iniFile.ReadValue("Settings", nameof(ActiveMonitor));
                if (!String.IsNullOrEmpty(value))
                    _activeMonitor = value;

                value = iniFile.ReadValue("Settings", nameof(Windowed));
                if (String.IsNullOrEmpty(value))
                    value = "true";
                if (!Boolean.TryParse(value, out _isWindowMode))
                    _isWindowMode = true;

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
                    _audioFrequency = 32000;
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

                OnPropertyChanged(nameof(ScreenResolution));
                OnPropertyChanged(nameof(ActiveMonitor));
                OnPropertyChanged(nameof(Windowed));
                OnPropertyChanged(nameof(AudioFrequency));
                OnPropertyChanged(nameof(AudioFrequencyEnabled));
                OnPropertyChanged(nameof(IsX64));
                OnPropertyChanged(nameof(IsX64Enabled));
                OnPropertyChanged(nameof(IsDebugMode));
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
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
                        throw new InvalidOperationException("Unexpected end of stream.");
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
                        throw new InvalidOperationException("Unexpected end of stream.");
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
                MessageBox.Show((Window)this.GetRootElement(), $"Cannot read audio sampling frequency from the file: {sdlibPath}{Environment.NewLine}{Environment.NewLine}{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        throw new InvalidOperationException("Unexpected end of stream.");
                    UInt16 originalFrequency = BitConverter.ToUInt16(buff, 0);

                    // Check the value
                    if (!_validSamplingFrequency.Contains(originalFrequency))
                        return false;

                    // Check values are same
                    input.Seek(offset2, SeekOrigin.Begin);
                    if (input.Read(buff, 0, buff.Length) != buff.Length)
                        throw new InvalidOperationException("Unexpected end of stream.");
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
                    MessageBox.Show((Window)this.GetRootElement(), $"Cannot read audio sampling frequency from the SdLib.dll ({(isX64 ? "x64" : "x86")}){Environment.NewLine}{Environment.NewLine}{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

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
                    String result = $"{devMode.dmPelsWidth.ToString(CultureInfo.InvariantCulture)}x{devMode.dmPelsHeight.ToString(CultureInfo.InvariantCulture)}";
                    if (set.Add(result))
                        yield return result;
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
                    sb.Append(" [Primary]");

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