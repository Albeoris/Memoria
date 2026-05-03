using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Advanced : UiGrid
    {
        public SettingsGrid_Advanced()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;

            CreateCheckbox("AudioBackend", "Settings.AudioBackend", "Settings.AudioBackend_Tooltip");

            CreateHeading("Settings.TextureFiltering");
            CreateCheckbox("WorldSmoothTexture", "Settings.WorldSmoothTexture", "Settings.WorldSmoothTexture_Tooltip", tooltipImage: "texture_filtering.png");
            CreateCheckbox("BattleSmoothTexture", "Settings.BattleSmoothTexture", "Settings.BattleSmoothTexture_Tooltip", tooltipImage: "texture_filtering.png");
            CreateCheckbox("ElementsSmoothTexture", "Settings.ElementsSmoothTexture", "Settings.ElementsSmoothTexture_Tooltip", tooltipImage: "texture_filtering2.png");
        }
    }
}
