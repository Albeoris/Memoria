using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Advanced : UiGrid
    {
        public SettingsGrid_Advanced()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;

            CreateCheckbox("AudioBackend", Lang.Settings.AudioBackend, Lang.Settings.AudioBackend_Tooltip);

            CreateHeading("PS1");
            CreateCheckbox("WorldSmoothTexture", Lang.Settings.WorldSmoothTexture, Lang.Settings.WorldSmoothTexture_Tooltip);
            CreateCheckbox("BattleSmoothTexture", Lang.Settings.BattleSmoothTexture, Lang.Settings.BattleSmoothTexture_Tooltip);
            CreateCheckbox("ElementsSmoothTexture", Lang.Settings.ElementsSmoothTexture, Lang.Settings.ElementsSmoothTexture_Tooltip);
        }
    }
}
