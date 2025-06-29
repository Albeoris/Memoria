using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Application = System.Windows.Application;
using ComboBox = System.Windows.Controls.ComboBox;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_VanillaDisplay : UiGrid, INotifyPropertyChanged
    {
        public static String[] availableMonitors = GetAvailableMonitors();
        public SettingsGrid_VanillaDisplay()
        {
            DataContext = this;


            CreateHeading("Settings.Display");

            String[] comboboxchoices = GetAvailableMonitors();
            CreateCombobox("ActiveMonitor", comboboxchoices, 50, "Settings.ActiveMonitor", "Settings.ActiveMonitor_Tooltip", "", true);

            comboboxchoices = new String[]
            {
                "Settings.Window",
                "Settings.ExclusiveFullscreen",
                "Settings.BorderlessFullscreen",
                "Settings.BorderlessWindow"
            };
            ComboBox modeComboBox = CreateCombobox("WindowMode", comboboxchoices, 50, "Settings.WindowMode", "Settings.WindowMode_Tooltip");

            List<String> reschoices =
            [
                "Launcher.Auto",
                .. EnumerateDisplaySettings(true).OrderByDescending(x => Convert.ToInt32(x.Split('x')[0]))
            ];
            ComboBox resComboBox = CreateCombobox("ScreenResolution", reschoices, 50, "Settings.Resolution", "Settings.Resolution_Tooltip", "", true);

            modeComboBox.SelectionChanged += (s, e) =>
            {
                resComboBox.IsEnabled = modeComboBox.SelectedIndex != 2;
            };

            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        public String ScreenResolution
        {
            get { return _resolution == "0x0" ? (String)Lang.Res["Launcher.Auto"] : _resolution; }
            set
            {
                if (value != null && _resolution != value)
                {
                    if (value == (String)Lang.Res["Launcher.Auto"])
                        _resolution = "0x0";
                    else
                        _resolution = addRatio(value);
                    OnPropertyChanged();
                }
            }
        }
        public Int16 WindowMode
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

        public event PropertyChangedEventHandler PropertyChanged;

        private async void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                IniFile iniFile = IniFile.SettingsIni;
                switch (propertyName)
                {
                    case nameof(ScreenResolution):
                        iniFile.SetSetting("Settings", propertyName, _resolution?.Split('|')[0].Trim(' ') ?? "0x0");
                        break;
                    case nameof(ActiveMonitor):
                        iniFile.SetSetting("Settings", propertyName, ActiveMonitor ?? String.Empty);
                        break;
                    case nameof(WindowMode):
                        iniFile.SetSetting("Settings", propertyName, WindowMode.ToString());
                        break;
                }
                iniFile.Save();
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        private String _resolution = "";
        private String _activeMonitor = "";
        private Int16 _windowMode;

        public void LoadSettings()
        {
            try
            {
                IniFile.PreventWrite = true;
                IniFile iniFile = IniFile.SettingsIni;

                String value = iniFile.GetSetting("Settings", nameof(ScreenResolution)).Split('|')[0].Trim(' ');
                //if res in settings.ini exists AND corresponds to something in the res list
                if ((!String.IsNullOrEmpty(value)) && EnumerateDisplaySettings(false).ToArray().Any(value.Contains))
                    _resolution = addRatio(value);
                //else we choose the largest available one
                else if (value == "0x0")
                    _resolution = value;
                else
                    _resolution = EnumerateDisplaySettings(false).OrderByDescending(x => Convert.ToInt32(x.Split('x')[0])).ToArray()[0];

                value = iniFile.GetSetting("Settings", nameof(ActiveMonitor), "0");

                String[] tokens = value.Split('-');
                _activeMonitor = availableMonitors[0];
                foreach (String monitor in availableMonitors)
                {
                    if (monitor.StartsWith(tokens[0].Trim()))
                    {
                        _activeMonitor = monitor;
                        break;
                    }
                }
                if (tokens.Length > 1)
                {
                    String displayName = Regex.Replace(tokens[1], @"\[[^\]]*\]", "").Trim();
                    foreach (String monitor in availableMonitors)
                    {
                        if (monitor.Contains(displayName))
                        {
                            _activeMonitor = monitor;
                            break;
                        }
                    }
                }

                value = iniFile.GetSetting("Settings", nameof(WindowMode));
                if (!String.IsNullOrEmpty(value))
                {
                    String newvalue = "";
                    if (value == (String)Lang.Res["Settings.Window"]) newvalue = "0";
                    if (value == (String)Lang.Res["Settings.ExclusiveFullscreen"]) newvalue = "1";
                    if (value == (String)Lang.Res["Settings.BorderlessFullscreen"]) newvalue = "2";
                    if (newvalue.Length > 0)
                    {
                        value = newvalue;
                        IniFile.PreventWrite = false;
                        iniFile.SetSetting("Settings", nameof(WindowMode), value);
                        iniFile.Save();
                        IniFile.PreventWrite = true;
                    }
                }
                if (!Int16.TryParse(value, out _windowMode))
                    _windowMode = 0;

                OnPropertyChanged(nameof(ScreenResolution));
                OnPropertyChanged(nameof(ActiveMonitor));
                OnPropertyChanged(nameof(WindowMode));

            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
            finally
            {
                IniFile.PreventWrite = false;
            }
        }

        [DllImport("user32.dll")]
        private static extern Boolean EnumDisplaySettings(String deviceName, Int32 modeNum, ref DevMode devMode);

        public IEnumerable<String> EnumerateDisplaySettings(Boolean displayRatio)
        {
            HashSet<String> set = new HashSet<String>();
            DevMode devMode = new DevMode();
            Int32 modeNum = 0;
            while (EnumDisplaySettings(null, modeNum++, ref devMode))
            {
                if (devMode.dmPelsWidth >= 640 && devMode.dmPelsHeight >= 480)
                {
                    String resolution = $"{devMode.dmPelsWidth.ToString(CultureInfo.InvariantCulture)}x{devMode.dmPelsHeight.ToString(CultureInfo.InvariantCulture)}";

                    if (displayRatio)
                        resolution = addRatio(resolution);

                    if (set.Add(resolution))
                        yield return resolution;
                }
            }
        }

        private String addRatio(String resolution)
        {
            if (!resolution.Contains("|") && resolution.Contains("x"))
            {
                String ratio = "";
                Int32 x = Int32.Parse(resolution.Split('x')[0]);
                Int32 y = Int32.Parse(resolution.Split('x')[1]);

                if ((x / 16) == (y / 9)) ratio = " | 16:9";
                else if ((x / 8) == (y / 5)) ratio = " | 16:10";
                else if ((x / 4) == (y / 3)) ratio = " | 4:3";
                else if ((x / 14) == (y / 9)) ratio = " | 14:9";
                else if ((x / 32) == (y / 9)) ratio = " | 32:9";
                else if ((x / 64) == (y / 27)) ratio = " | 64:27";
                else if ((x / 3) == (y / 2)) ratio = " | 3:2";
                else if ((x / 5) == (y / 4)) ratio = " | 5:4";
                else if ((x / 256) == (y / 135)) ratio = " | 256:135";
                else if ((x / 25) == (y / 16)) ratio = " | 25:16";
                else if ((x) == (y)) ratio = " | 1:1";
                resolution += ratio;
            }
            return resolution;
        }

        public static String[] GetAvailableMonitors()
        {
            var displays = DisplayInfo.Displays;
            String[] result = new String[displays.Count];
            for (Int32 i = 0; i < displays.Count; i++)
            {
                result[i] = $"{i} - {displays[i].name}";
                if (displays[i].isPrimary)
                {
                    result[i] += (String)Lang.Res["Settings.PrimaryMonitor"];
                }
            }

            return result;
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
