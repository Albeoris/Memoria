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
                Lang.Settings.TripleTriadType1,
                Lang.Settings.TripleTriadType2
            };
            CreateCombobox("TripleTriad", comboboxchoices);

            CreateTextbloc(Lang.Settings.CardReduceRandom, false, Lang.Settings.CardReduceRandom_Tooltip);
            comboboxchoices = new String[]
            {
                Lang.Settings.tetraMasterReduceRandomBox0,
                Lang.Settings.tetraMasterReduceRandomBox1,
                Lang.Settings.tetraMasterReduceRandomBox2
            };
            CreateCombobox("ReduceRandom", comboboxchoices);
            CreateCheckbox("MaxCardCount", Lang.Settings.MaxCardCount, Lang.Settings.MaxCardCount_Tooltip);
            CreateCheckbox("HideCards", Lang.Settings.HideSteamBubbles, Lang.Settings.HideSteamBubbles_Tooltip);

            CreateTextbloc(Lang.Settings.Worldmap, true);

            LoadSettings();
        }
    }
}
