using Application = System.Windows.Application;

using Memoria.Launcher.Utils;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Advanced3 : UiGrid
    {
        public SettingsGrid_Advanced3()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;

            // TODO: implement these properly in the launcher
            CreateHeading("Launcher.AdvSettingsTitle");

            CreateCheckbox("VSync", "Settings.VSync", "Settings.VSync_Tooltip");
            CreateCheckbox("SwapConfirmCancel", "Settings.SwapConfirmCancel", "Settings.SwapConfirmCancel_Tooltip");
            CreateCombobox("DualLanguageMode", ComboBoxOptions.Localized(["Settings.DualLanguageModeChoice0", "Settings.DualLanguageModeChoice1", "Settings.DualLanguageModeChoice2"]), 50, "Settings.DualLanguageMode", "Settings.DualLanguageMode_Tooltip");
            CreateCombobox("DualLanguage", ComboBoxOptions.Literal(["English (US)", "English (UK)", "日本語", "Deutsch", "Français", "Italiano", "Español"]), 50, "Settings.DualLanguage", "Settings.DualLanguage_Tooltip");
        }
    }
}
