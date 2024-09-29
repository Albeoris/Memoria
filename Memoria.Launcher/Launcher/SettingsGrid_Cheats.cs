using System;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Cheats : Settings
    {
        public SettingsGrid_Cheats()
        {
            CreateTextbloc(Lang.Settings.Battle, true);

            CreateCheckbox("StealingAlwaysWorks", Lang.Settings.MaxStealRate, Lang.Settings.MaxStealRate_Tooltip);
            CreateCheckbox("NoAutoTrance", Lang.Settings.NoAutoTrance, Lang.Settings.NoAutoTrance_Tooltip);
            CreateCheckbox("ViviAutoAttack", Lang.Settings.ViviAutoAttack, Lang.Settings.ViviAutoAttack_Tooltip);
            CreateCheckbox("BreakDamageLimit", Lang.Settings.BreakDamageLimit, Lang.Settings.BreakDamageLimit_Tooltip);
            CreateCheckbox("AccessBattleMenuToggle", Lang.Settings.AccessBattleMenuToggle, Lang.Settings.AccessBattleMenuToggle_Tooltip);
            CreateCheckbox("GarnetConcentrate", Lang.Settings.DisableCantConcentrate, Lang.Settings.DisableCantConcentrate_Tooltip);




            CreateTextbloc(Lang.Settings.VanillaCheats, true);

            CreateCheckbox("SpeedMode", Lang.Settings.SpeedMode, Lang.Settings.SpeedMode_Tooltip);
            CreateSlider("SpeedFactor", "SpeedFactor", 2, 12, 1, "{0}x");
            CreateCheckbox("BattleAssistance", Lang.Settings.PermanentTranse, Lang.Settings.BattleAssistance_Tooltip);
            CreateCheckbox("Attack9999", Lang.Settings.Attack9999, Lang.Settings.Attack9999_Tooltip);
            CreateCheckbox("NoRandomEncounter", Lang.Settings.NoRandomBattles, Lang.Settings.NoRandomBattles_Tooltip);
            CreateCheckbox("MasterSkill", Lang.Settings.PermanentCheats, Lang.Settings.PermanentCheats_Tooltip);

            LoadSettings();
        }
    }
}
