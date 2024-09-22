using System;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Cheats : Settings
    {
        public SettingsGrid_Cheats()
        {
            CreateCheckbox("StealingAlwaysWorks", Lang.Settings.MaxStealRate, Lang.Settings.MaxStealRate_Tooltip);
            CreateCheckbox("NoAutoTrance", Lang.Settings.NoAutoTrance, Lang.Settings.NoAutoTrance_Tooltip);
            CreateCheckbox("GarnetConcentrate", Lang.Settings.DisableCantConcentrate, Lang.Settings.DisableCantConcentrate_Tooltip);
            CreateCheckbox("BreakDamageLimit", Lang.Settings.BreakDamageLimit, Lang.Settings.BreakDamageLimit_Tooltip);
            CreateTextbloc(Lang.Settings.AccessBattleMenu, false, Lang.Settings.AccessBattleMenu_Tooltip);
            String[] accessmenuchoices =
            {
                Lang.Settings.AccessBattleMenuType0,
                Lang.Settings.AccessBattleMenuType1,
                Lang.Settings.AccessBattleMenuType2,
                Lang.Settings.AccessBattleMenuType3
            };
            CreateCombobox("AccessBattleMenu", accessmenuchoices, 4, Lang.Settings.AccessBattleMenu_Tooltip);

            CreateCheckbox("SpeedMode", Lang.Settings.SpeedMode, Lang.Settings.SpeedMode_Tooltip);
            CreateSlider("SpeedFactor", "SpeedFactor", 2, 12, 1, "{0}x");
            
            CreateTextbloc(Lang.Settings.BattleTPS, false, Lang.Settings.BattleTPS_Tooltip);
            CreateSlider("BattleTPSDividedBy10", "BattleTPS", 15, 75, 1, "{0}x");

            CreateCheckbox("BattleAssistance", Lang.Settings.BattleAssistance, Lang.Settings.BattleAssistance_Tooltip);
            CreateCheckbox("NoRandomEncounter", Lang.Settings.NoRandomBattles, Lang.Settings.NoRandomBattles_Tooltip);
            CreateCheckbox("MasterSkill", Lang.Settings.PermanentCheats, Lang.Settings.PermanentCheats_Tooltip);
            CreateCheckbox("MaxCardCount", Lang.Settings.MaxCardCount, Lang.Settings.MaxCardCount_Tooltip);

            CreateTextbloc(Lang.Settings.CardReduceRandom, false, Lang.Settings.CardReduceRandom_Tooltip);
            String[] reducerandomchoice =
            {
                Lang.Settings.tetraMasterReduceRandomBox0,
                Lang.Settings.tetraMasterReduceRandomBox1,
                Lang.Settings.tetraMasterReduceRandomBox2,
            };
            CreateCombobox("ReduceRandom", reducerandomchoice);

            LoadSettings();
        }

    }
}
