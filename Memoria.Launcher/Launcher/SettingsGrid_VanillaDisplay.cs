using System;
using System.Linq;
using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_VanillaDisplay : Settings_Vanilla
    {
        public SettingsGrid_VanillaDisplay()
        {
            CreateTextbloc(Lang.Settings.ActiveMonitor, false, Lang.Settings.ActiveMonitor_Tooltip);
            String[] comboboxchoices = GetAvailableMonitors();
            CreateCombobox("ActiveMonitor", comboboxchoices, 4, "", true);

            CreateTextbloc(Lang.Settings.WindowMode, false, Lang.Settings.WindowMode_Tooltip);
            comboboxchoices = new String[]
            {
                Lang.Settings.Window,
                Lang.Settings.ExclusiveFullscreen,
                Lang.Settings.BorderlessFullscreen
            };
            CreateCombobox("WindowMode", comboboxchoices, 4);

            CreateTextbloc(Lang.Settings.Resolution, false, Lang.Settings.Resolution_Tooltip);
            comboboxchoices = EnumerateDisplaySettings(true).OrderByDescending(x => Convert.ToInt32(x.Split('x')[0])).ToArray();
            CreateCombobox("ScreenResolution", comboboxchoices, 4, "", true);

            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }
    }
}
