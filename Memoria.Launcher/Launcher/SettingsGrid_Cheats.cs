using System;
using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Cheats : UiGrid
    {
        public SettingsGrid_Cheats()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;
            CreateHeading("Settings.Battle");

            CreateCheckbox("StealingAlwaysWorks", "Settings.MaxStealRate", "Settings.MaxStealRate_Tooltip");
            CreateCheckbox("NoAutoTrance", "Settings.NoAutoTrance", "Settings.NoAutoTrance_Tooltip");
            CreateCheckbox("ViviAutoAttack", "Settings.ViviAutoAttack", "Settings.ViviAutoAttack_Tooltip");
            CreateCheckbox("BreakDamageLimit", "Settings.BreakDamageLimit", "Settings.BreakDamageLimit_Tooltip");
            //CreateCheckbox("AccessBattleMenuToggle", Lang.Settings.AccessBattleMenuToggle, Lang.Settings.AccessBattleMenuToggle_Tooltip);
            CreateCheckbox("GarnetConcentrate", "Settings.DisableCantConcentrate", "Settings.DisableCantConcentrate_Tooltip");


            String[] accessmenuchoices =
            {
                "Settings.AccessBattleMenuType0",
                "Settings.AccessBattleMenuType1",
                "Settings.AccessBattleMenuType2",
                "Settings.AccessBattleMenuType3"
            };
            CreateCombobox("AccessBattleMenu", accessmenuchoices, 50, "Settings.AccessBattleMenu", "Settings.AccessBattleMenu_Tooltip");



            CreateHeading("Settings.VanillaCheats");

            //CreateCheckbox("SpeedMode", Lang.Settings.SpeedMode, Lang.Settings.SpeedMode_Tooltip);
            CreateSlider("SpeedFactor", "SpeedFactor", 1, 12, 1, "{0}x", 50, "Settings.SpeedMode", "Settings.SpeedMode_Tooltip");
            CreateCheckbox("BattleAssistance", "Settings.PermanentTranse", "Settings.BattleAssistance_Tooltip");
            CreateCheckbox("Attack9999", "Settings.Attack9999", "Settings.Attack9999_Tooltip");
            CreateCheckbox("NoRandomEncounter", "Settings.NoRandomBattles", "Settings.NoRandomBattles_Tooltip");
            CreateCheckbox("MasterSkill", "Settings.PermanentCheats", "Settings.PermanentCheats_Tooltip");

        }
    }
}
