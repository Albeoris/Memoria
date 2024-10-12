using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Advanced : UiGrid
    {
        public SettingsGrid_Advanced()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;

            CreateCheckbox("AudioBackend", Lang.Settings.AudioBackend, Lang.Settings.AudioBackend_Tooltip);

            CreateHeading(Lang.Settings.TextureFiltering);
            CreateCheckbox("WorldSmoothTexture", Lang.Settings.WorldSmoothTexture, Lang.Settings.WorldSmoothTexture_Tooltip, tooltipImage: "texture_filtering.png");
            CreateCheckbox("BattleSmoothTexture", Lang.Settings.BattleSmoothTexture, Lang.Settings.BattleSmoothTexture_Tooltip, tooltipImage: "texture_filtering.png");
            CreateCheckbox("ElementsSmoothTexture", Lang.Settings.ElementsSmoothTexture, Lang.Settings.ElementsSmoothTexture_Tooltip, tooltipImage: "texture_filtering2.png");
        }
    }
}
