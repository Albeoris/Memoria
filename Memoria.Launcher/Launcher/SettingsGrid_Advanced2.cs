using System;
using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Advanced2 : UiGrid
    {
        public SettingsGrid_Advanced2()
        {

            DataContext = (MainWindow)Application.Current.MainWindow;

            CreateHeading("Settings.SaveFiles");
            String[] autosavechoices =
            {
                "Settings.AutoSaveChoice0",
                "Settings.AutoSaveChoice1",
                "Settings.AutoSaveChoice2"
            };
            CreateCombobox("AutoSave", autosavechoices, 50, "Settings.AutoSave", "Settings.AutoSave_Tooltip");
            CreateCheckbox("SaveOnCloud", "Settings.SaveOnCloud", "Settings.SaveOnCloud_Tooltip");
        }
    }
}
