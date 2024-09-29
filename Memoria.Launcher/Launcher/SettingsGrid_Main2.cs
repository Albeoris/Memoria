using System;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Main2 : Settings
    {
        public SettingsGrid_Main2()
        {
            CreateTextbloc(Lang.Settings.Cards, true);

            CreateTextbloc(Lang.Settings.TripleTriad, false, Lang.Settings.TripleTriad_Tooltip);
            String[] comboboxchoices = new String[]{
                Lang.Settings.TripleTriadType0,
                Lang.Settings.TripleTriadType0_ReduceRandom,
                Lang.Settings.TripleTriadType1,
                Lang.Settings.TripleTriadType2
            };
            CreateCombobox("TripleTriad", comboboxchoices, 25);

            /*CreateTextbloc(Lang.Settings.CardReduceRandom, false, Lang.Settings.CardReduceRandom_Tooltip);
            comboboxchoices = new String[]
            {
                Lang.Settings.tetraMasterReduceRandomBox0,
                Lang.Settings.tetraMasterReduceRandomBox1,
                Lang.Settings.tetraMasterReduceRandomBox2
            };
            CreateCombobox("ReduceRandom", comboboxchoices);*/

            //CreateCheckbox("ReduceRandom", Lang.Settings.CardReduceRandom, Lang.Settings.CardReduceRandom_Tooltip);
            CreateCheckbox("MaxCardCount", Lang.Settings.MaxCardCount, Lang.Settings.MaxCardCount_Tooltip);
            CreateCheckbox("HideCards", Lang.Settings.HideSteamBubbles, Lang.Settings.HideSteamBubbles_Tooltip);





            CreateTextbloc(Lang.Settings.Worldmap, true);


            CreateTextbloc(Lang.Settings.WorldmapMistPreset, false, Lang.Settings.WorldmapMistPreset_Tooltip);
            comboboxchoices = new String[]{
                Lang.Settings.WorldmapMistPresetChoice0,
                Lang.Settings.WorldmapMistPresetChoice1,
                Lang.Settings.WorldmapMistPresetChoice2,
                Lang.Settings.WorldmapMistPresetChoice3
            };
            CreateCombobox("WorldmapMistPreset", comboboxchoices);

            CreateTextbloc(Lang.Settings.WorldmapDistancePreset, false, Lang.Settings.WorldmapDistancePreset_Tooltip);
            comboboxchoices = new String[]{
                Lang.Settings.WorldmapDistancePresetChoice0,
                Lang.Settings.WorldmapDistancePresetChoice1,
                Lang.Settings.WorldmapDistancePresetChoice2,
                Lang.Settings.WorldmapDistancePresetChoice3
            };
            CreateCombobox("WorldmapDistancePreset", comboboxchoices);

            CreateTextbloc(Lang.Settings.WorldmapFOV, false, Lang.Settings.WorldmapFOV_Tooltip);
            CreateSlider("WorldmapFOV", "WorldmapFOV", 30, 130, 1);

            CreateCheckbox("WorldmapBoost", Lang.Settings.WorldmapBoost, Lang.Settings.WorldmapBoost_Tooltip);
            CreateCheckbox("WorldmapShipTilt", Lang.Settings.WorldmapShipTilt, Lang.Settings.WorldmapShipTilt_Tooltip);

            LoadSettings();
        }
    }
}
