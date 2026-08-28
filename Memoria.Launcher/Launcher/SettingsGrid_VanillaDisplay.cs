using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Memoria.Launcher.Utils;
using Application = System.Windows.Application;
using ComboBox = System.Windows.Controls.ComboBox;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_VanillaDisplay : UiGrid, INotifyPropertyChanged
    {
        private static readonly String[] AvailableMonitors = BuildMonitorChoices();

        public SettingsGrid_VanillaDisplay()
        {
            DataContext = this;


            CreateHeading("Settings.Display");

            String[] comboboxchoices = AvailableMonitors;
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
                .. GetAvailableResolutionStrings(true).OrderByDescending(x => Convert.ToInt32(x.Split('x')[0]))
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
                        _resolution = AddAspectRatio(value);
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
                String[] availableResolutions = GetAvailableResolutionStrings(false).ToArray();
                if (!String.IsNullOrEmpty(value) && availableResolutions.Contains(value))
                    _resolution = AddAspectRatio(value);
                else if (value == "0x0")
                    _resolution = value;
                else if (availableResolutions.Length > 0)
                    _resolution = AddAspectRatio(availableResolutions.OrderByDescending(ParseResolutionWidth).First());
                else if (DisplayService.Current.PrimaryResolution.IsValid)
                    _resolution = AddAspectRatio(DisplayService.Current.PrimaryResolution.ToString());
                else
                    _resolution = "1920x1080";

                value = iniFile.GetSetting("Settings", nameof(ActiveMonitor), "0");

                String[] tokens = value.Split('-');
                _activeMonitor = AvailableMonitors[0];
                foreach (String monitor in AvailableMonitors)
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
                    foreach (String monitor in AvailableMonitors)
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

        private static IEnumerable<String> GetAvailableResolutionStrings(Boolean includeAspectRatio)
        {
            foreach (DisplayResolution resolution in DisplayService.Current.SupportedResolutions)
            {
                if (resolution.Width < 640 || resolution.Height < 480)
                    continue;

                String value = resolution.ToString();
                yield return includeAspectRatio ? AddAspectRatio(value) : value;
            }
        }

        private static String AddAspectRatio(String resolution)
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

        private static Int32 ParseResolutionWidth(String resolution)
        {
            return Int32.Parse(resolution.Split('x')[0], CultureInfo.InvariantCulture);
        }

        private static String[] BuildMonitorChoices()
        {
            IReadOnlyList<DisplayMonitor> monitors = DisplayService.Current.Monitors;
            if (monitors.Count == 0)
                return ["0 - Default display"];

            String[] result = new String[monitors.Count];
            for (Int32 index = 0; index < monitors.Count; index++)
            {
                DisplayMonitor monitor = monitors[index];
                result[index] = $"{monitor.Index} - {monitor.Name}";
                if (monitor.IsPrimary)
                    result[index] += (String)Lang.Res["Settings.PrimaryMonitor"];
            }

            return result;
        }
    }
}
