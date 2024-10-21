using System;
using System.Collections.Generic;
using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Advanced2 : UiGrid
    {
        public SettingsGrid_Advanced2()
        {

            DataContext = (MainWindow)Application.Current.MainWindow;


            CreateHeading("Settings.AnalogControls");

            CreateSlider("StickThreshold", "StickThreshold", 0, 50, 1, "{0}%", 50, "Settings.StickThreshold", "Settings.StickThreshold_Tooltip");

            CreateCheckbox("RightStickCamera", "Settings.RightStickCamera", "Settings.RightStickCamera_Tooltip");
            CreateCheckbox("InvertedCameraY", "Settings.InvertedCameraY", "Settings.InvertedCameraY_Tooltip");
            CreateCheckbox("InvertedFlightY", "Settings.InvertedFlightY", "Settings.InvertedFlightY_Tooltip");
            CreateCheckbox("UseAbsoluteOrientation", "Settings.UseAbsoluteOrientation", "Settings.UseAbsoluteOrientation_Tooltip");
            CreateCheckbox("AlwaysCaptureGamepad", "Settings.AlwaysCaptureGamepad", "Settings.AlwaysCaptureGamepad_Tooltip");


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
