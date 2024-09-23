using System;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Cheats : Settings
    {
        public SettingsGrid_Cheats()
        {

            CreateTextbloc("Battle", true);

            CreateCheckbox("StealingAlwaysWorks", Lang.Settings.MaxStealRate, Lang.Settings.MaxStealRate_Tooltip);
            CreateCheckbox("GarnetConcentrate", Lang.Settings.DisableCantConcentrate, Lang.Settings.DisableCantConcentrate_Tooltip);
            CreateCheckbox("BreakDamageLimit", Lang.Settings.BreakDamageLimit, Lang.Settings.BreakDamageLimit_Tooltip);
            
            CreateTextbloc(Lang.Settings.BattleTPS, false, Lang.Settings.BattleTPS_Tooltip);
            CreateSlider("BattleTPSDividedBy10", "BattleTPS", 15, 75, 1, "{0}x");



            CreateTextbloc("Vanilla Cheats", true);

            CreateCheckbox("SpeedMode", Lang.Settings.SpeedMode, Lang.Settings.SpeedMode_Tooltip);
            CreateSlider("SpeedFactor", "SpeedFactor", 2, 12, 1, "{0}x");

            CreateCheckbox("BattleAssistance", Lang.Settings.BattleAssistance, Lang.Settings.BattleAssistance_Tooltip);
            CreateCheckbox("NoRandomEncounter", Lang.Settings.NoRandomBattles, Lang.Settings.NoRandomBattles_Tooltip);
            CreateCheckbox("MasterSkill", Lang.Settings.PermanentCheats, Lang.Settings.PermanentCheats_Tooltip);

            LoadSettings();
        }

    }
}
