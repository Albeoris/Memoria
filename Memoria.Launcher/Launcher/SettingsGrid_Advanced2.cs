using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Advanced2 : UiGrid
    {
        public SettingsGrid_Advanced2()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;

            CreateHeading(Lang.Settings.SaveFiles);
            CreateCombobox("AutoSave", Lang.Settings.AutoSaveOptions.Replace(", ", ",").Split(','), 50, Lang.Settings.AutoSave, Lang.Settings.AutoSave_ToolTip);
            CreateCheckbox("SaveOnCloud", Lang.Settings.SaveOnCloud, Lang.Settings.SaveOnCloud_ToolTip);
        }
    }
}
