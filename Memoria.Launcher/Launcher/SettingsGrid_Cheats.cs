using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Binding = System.Windows.Data.Binding;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable ArrangeStaticMemberQualifier
#pragma warning disable 649
#pragma warning disable 169

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Cheats : Settings
    {
        public SettingsGrid_Cheats()
        {
            SetRows(2);
            SetCols(8);

            Width = 260;
            Margin = new Thickness(0);

            DataContext = this;

            CreateTextbloc(Lang.Settings.IniCheats, true);
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
            Row++;
            TextBlock speedFactorTextindex = AddUiElement(UiTextBlockFactory.Create(""), Row, 0, 1, 8);
            speedFactorTextindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(SpeedFactor)) { Mode = BindingMode.TwoWay, StringFormat = "{0}x" });
            speedFactorTextindex.Foreground = Brushes.White;
            speedFactorTextindex.Margin = CommonMargin;
            Slider speedFactor = AddUiElement(UiSliderFactory.Create(0), Row, 1, 1, 8);
            speedFactor.SetBinding(Slider.ValueProperty, new Binding(nameof(SpeedFactor)) { Mode = BindingMode.TwoWay });
            speedFactor.TickFrequency = 1;
            speedFactor.IsSnapToTickEnabled = true;
            speedFactor.TickPlacement = TickPlacement.BottomRight;
            speedFactor.Height = 20;
            speedFactor.Minimum = 2;
            speedFactor.Maximum = 12;
            speedFactor.Margin = new Thickness(0, 0, 3, 0);

            CreateTextbloc(Lang.Settings.BattleTPS, false, Lang.Settings.BattleTPS_Tooltip);
            Row++;
            TextBlock BattleTPSindex = AddUiElement(UiTextBlockFactory.Create(""), Row, 0, 1, 8);
            BattleTPSindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(BattleTPSDividedBy10)) { Mode = BindingMode.TwoWay, StringFormat = "{0}x" });
            BattleTPSindex.Foreground = Brushes.White;
            BattleTPSindex.Margin = CommonMargin;
            Slider BattleTPSFactor = AddUiElement(UiSliderFactory.Create(0), Row, 1, 1, 8);
            BattleTPSFactor.SetBinding(Slider.ValueProperty, new Binding(nameof(BattleTPS)) { Mode = BindingMode.TwoWay });
            BattleTPSFactor.TickFrequency = 1;
            BattleTPSFactor.IsSnapToTickEnabled = true;
            BattleTPSFactor.TickPlacement = TickPlacement.BottomRight;
            BattleTPSFactor.Height = 20;
            BattleTPSFactor.Minimum = 15;
            BattleTPSFactor.Maximum = 75;
            BattleTPSFactor.Margin = new Thickness(0, 0, 3, 0);

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
