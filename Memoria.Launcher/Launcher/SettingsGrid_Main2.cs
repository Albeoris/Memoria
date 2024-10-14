using System;
using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Main2 : UiGrid
    {
        public SettingsGrid_Main2()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;

            CreateHeading("Settings.Cards");

            String[] comboboxchoices = new String[]{
                "Settings.TripleTriadType0",
                "Settings.TripleTriadType0_ReduceRandom",
                "Settings.TripleTriadType1",
                "Settings.TripleTriadType2"
            };
            CreateCombobox("TripleTriad", comboboxchoices, 25, "Settings.TripleTriad", "Settings.TripleTriad_Tooltip", "comparison_cardgames.png");

            CreateCheckbox("MaxCardCount", "Settings.MaxCardCount", "Settings.MaxCardCount_Tooltip");
            CreateCheckbox("HideCards", "Settings.HideSteamBubbles", "Settings.HideSteamBubbles_Tooltip");





            CreateHeading("Settings.Worldmap");

            comboboxchoices = new String[]{
                "Settings.WorldmapMistPresetChoice0",
                "Settings.WorldmapMistPresetChoice1",
                "Settings.WorldmapMistPresetChoice2",
                "Settings.WorldmapMistPresetChoice3"
            };
            CreateCombobox("WorldmapMistPreset", comboboxchoices, 50, "Settings.WorldmapMistPreset", "Settings.WorldmapMistPreset_Tooltip", "comparison_mist.jpg");

            comboboxchoices = new String[]{
                "Settings.WorldmapDistancePresetChoice0",
                "Settings.WorldmapDistancePresetChoice1",
                "Settings.WorldmapDistancePresetChoice2",
                "Settings.WorldmapDistancePresetChoice3"
            };
            CreateCombobox("WorldmapDistancePreset", comboboxchoices, 50, "Settings.WorldmapDistancePreset", "Settings.WorldmapDistancePreset_Tooltip", "comparison_viewdistance.jpg");

            CreateSlider("WorldmapFOV", "WorldmapFOV", 30, 110, 1, "", 50, "Settings.WorldmapFOV", "Settings.WorldmapFOV_Tooltip", "comparison_worldmapfov.jpg");

            CreateSlider("WMCameraHeight", "WMCameraHeight", -200, 600, 50, "{0}", 50, "Settings.WMCameraHeight", "Settings.WMCameraHeight_Tooltip", "comparison_cameraheight.jpg");

            CreateSlider("WorldmapTPSDividedby20", "WorldmapTPS", 20, 100, 4, "{0}x", 50, "Settings.WorldmapTPS", "Settings.WorldmapTPS_Tooltip");

            CreateCheckbox("WorldmapBoost", "Settings.WorldmapBoost", "Settings.WorldmapBoost_Tooltip");
            CreateCheckbox("WorldmapShipTilt", "Settings.WorldmapShipTilt", "Settings.WorldmapShipTilt_Tooltip");
        }
    }
}
