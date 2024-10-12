using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_VanillaDisplay : UiGrid, INotifyPropertyChanged
    {
        public SettingsGrid_VanillaDisplay()
        {
            DataContext = this;


            CreateHeading(Lang.Settings.Display);

            String[] comboboxchoices = GetAvailableMonitors();
            CreateCombobox("ActiveMonitor", comboboxchoices, 50, Lang.Settings.ActiveMonitor, Lang.Settings.ActiveMonitor_Tooltip, "", true);

            comboboxchoices = new String[]
            {
                Lang.Settings.Window,
                Lang.Settings.ExclusiveFullscreen,
                Lang.Settings.BorderlessFullscreen
            };
            CreateCombobox("WindowMode", comboboxchoices, 50, Lang.Settings.WindowMode, Lang.Settings.WindowMode_Tooltip);

            comboboxchoices = EnumerateDisplaySettings(true).OrderByDescending(x => Convert.ToInt32(x.Split('x')[0])).ToArray();
            CreateCombobox("ScreenResolution", comboboxchoices, 50, Lang.Settings.Resolution, Lang.Settings.Resolution_Tooltip, "", true);

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
            get { return _resolution; }
            set
            {
                if (_resolution != value)
                {
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

                IniFile iniFile = new IniFile(IniPath);
                switch (propertyName)
                {
                    case nameof(ScreenResolution):
                        iniFile.WriteValue("Settings", propertyName, ScreenResolution.Split('|')[0].Trim(' ') ?? "1280x960");
                        break;
                    case nameof(ActiveMonitor):
                        iniFile.WriteValue("Settings", propertyName, ActiveMonitor ?? String.Empty);
                        break;
                    case nameof(WindowMode):
                        iniFile.WriteValue("Settings", propertyName, WindowMode.ToString());
                        break;
                }
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        public static readonly String IniPath = AppDomain.CurrentDomain.BaseDirectory + "\\Settings.ini";

        private String _resolution = "";
        private String _activeMonitor = "";
        private Int16 _windowMode;

        public void LoadSettings()
        {
            try
            {
                IniReader iniReader = new IniReader(IniPath);

                String value = iniReader.GetSetting("Settings", nameof(ScreenResolution)).Split('|')[0].Trim(' ');
                //if res in settings.ini exists AND corresponds to something in the res list
                if ((!String.IsNullOrEmpty(value)) && EnumerateDisplaySettings(false).ToArray().Any(value.Contains))
                    _resolution = addRatio(value);
                //else we choose the largest available one
                else
                    _resolution = EnumerateDisplaySettings(false).OrderByDescending(x => Convert.ToInt32(x.Split('x')[0])).ToArray()[0];

                value = iniReader.GetSetting("Settings", nameof(ActiveMonitor));
                if (!String.IsNullOrEmpty(value))
                {
                    var i = value.IndexOf("[");
                    _activeMonitor = i < 0 ? value : value.Substring(0, i) + Lang.Settings.PrimaryMonitor;
                }

                value = iniReader.GetSetting("Settings", nameof(WindowMode));
                if (!String.IsNullOrEmpty(value))
                {
                    if (value == Lang.Settings.Window) value = "0";
                    if (value == Lang.Settings.ExclusiveFullscreen) value = "1";
                    if (value == Lang.Settings.BorderlessFullscreen) value = "2";
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

        public String[] GetAvailableMonitors()
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
