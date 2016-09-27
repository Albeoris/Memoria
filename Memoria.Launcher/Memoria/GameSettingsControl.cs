using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using Ini;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

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
            SetCols(2);
            SetRows(5);

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

            AddUiElement(UiTextBlockFactory.Create("Resolution:"), 0, 0).Margin = rowMargin;
            UiComboBox resolution = AddUiElement(UiComboBoxFactory.Create(), 1, 0);
            resolution.ItemsSource = EnumerateDisplaySettings().ToArray();
            resolution.SetBinding(Selector.SelectedItemProperty, new Binding("ScreenResolution") {Mode = BindingMode.TwoWay});
            resolution.Margin = rowMargin;

            UiCheckBox windowedCheckBox = AddUiElement(UiCheckBoxFactory.Create("Windowed", null), 1, 1);
            windowedCheckBox.Margin = rowMargin;
            windowedCheckBox.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Windowed)) {Mode = BindingMode.TwoWay});

            UiCheckBox x64 = AddUiElement(UiCheckBoxFactory.Create("X64", null), 2, 0);
            x64.Margin = rowMargin;
            x64.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(IsX64)) {Mode = BindingMode.TwoWay});
            x64.SetBinding(ToggleButton.IsEnabledProperty, new Binding(nameof(IsX64Enabled)) {Mode = BindingMode.TwoWay});

            UiCheckBox debuggableCheckBox = AddUiElement(UiCheckBoxFactory.Create("Debuggable", null), 2, 1);
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
        private String _resolution = "1280x960";
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

                value = iniFile.ReadValue("Memoria", nameof(IsDebugMode));
                if (String.IsNullOrEmpty(value))
                    value = "false";
                if (!Boolean.TryParse(value, out _isDebugMode))
                    _isDebugMode = false;

                OnPropertyChanged(nameof(ScreenResolution));
                OnPropertyChanged(nameof(Windowed));
                OnPropertyChanged(nameof(IsX64));
                OnPropertyChanged(nameof(IsX64Enabled));
                OnPropertyChanged(nameof(IsDebugMode));
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
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