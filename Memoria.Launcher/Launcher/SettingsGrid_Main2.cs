using System;
using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Main2 : UiGrid
    {
        public SettingsGrid_Main2()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;

            CreateHeading(Lang.Settings.Cards);

            String[] comboboxchoices = new String[]{
                Lang.Settings.TripleTriadType0,
                Lang.Settings.TripleTriadType0_ReduceRandom,
                Lang.Settings.TripleTriadType1,
                Lang.Settings.TripleTriadType2
            };
            CreateCombobox("TripleTriad", comboboxchoices, 25, Lang.Settings.TripleTriad, Lang.Settings.TripleTriad_Tooltip, "comparison_cardgames.png");

            CreateCheckbox("MaxCardCount", Lang.Settings.MaxCardCount, Lang.Settings.MaxCardCount_Tooltip);
            CreateCheckbox("HideCards", Lang.Settings.HideSteamBubbles, Lang.Settings.HideSteamBubbles_Tooltip);





            CreateHeading(Lang.Settings.Worldmap);

            comboboxchoices = new String[]{
                Lang.Settings.WorldmapMistPresetChoice0,
                Lang.Settings.WorldmapMistPresetChoice1,
                Lang.Settings.WorldmapMistPresetChoice2,
                Lang.Settings.WorldmapMistPresetChoice3
            };
            CreateCombobox("WorldmapMistPreset", comboboxchoices, 50, Lang.Settings.WorldmapMistPreset, Lang.Settings.WorldmapMistPreset_Tooltip, "comparison_mist.jpg");

            comboboxchoices = new String[]{
                Lang.Settings.WorldmapDistancePresetChoice0,
                Lang.Settings.WorldmapDistancePresetChoice1,
                Lang.Settings.WorldmapDistancePresetChoice2,
                Lang.Settings.WorldmapDistancePresetChoice3
            };
            CreateCombobox("WorldmapDistancePreset", comboboxchoices, 50, Lang.Settings.WorldmapDistancePreset, Lang.Settings.WorldmapDistancePreset_Tooltip, "comparison_viewdistance.jpg");

            CreateSlider("WorldmapFOV", "WorldmapFOV", 30, 110, 1, "", 50, Lang.Settings.WorldmapFOV, Lang.Settings.WorldmapFOV_Tooltip, "comparison_worldmapfov.jpg");

            CreateSlider("WMCameraHeight", "WMCameraHeight", -200, 600, 50, "{0}", 50, Lang.Settings.WMCameraHeight, Lang.Settings.WMCameraHeight_Tooltip, "comparison_cameraheight.jpg");

            CreateSlider("WorldmapTPSDividedby20", "WorldmapTPS", 20, 100, 4, "{0}x", 50, Lang.Settings.WorldmapTPS, Lang.Settings.WorldmapTPS_Tooltip);

            CreateCheckbox("WorldmapBoost", Lang.Settings.WorldmapBoost, Lang.Settings.WorldmapBoost_Tooltip);
            CreateCheckbox("WorldmapShipTilt", Lang.Settings.WorldmapShipTilt, Lang.Settings.WorldmapShipTilt_Tooltip);
        }
    }
}
