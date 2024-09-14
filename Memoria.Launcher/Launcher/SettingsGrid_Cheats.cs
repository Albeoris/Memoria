using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Application = System.Windows.Application;
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
            SetRows(18);
            SetCols(8);

            Width = 260;
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Center;
            Margin = new Thickness(0);

            DataContext = this;

            Thickness rowMargin = new Thickness(8, 2, 3, 2);


            TextBlock cheatOptionsText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.IniCheats), Row, 0, 2, 8);
            cheatOptionsText.Padding = new Thickness(0, 4, 0, 2);
            cheatOptionsText.Foreground = Brushes.White;
            cheatOptionsText.FontSize = 14;
            cheatOptionsText.FontWeight = FontWeights.Bold;
            cheatOptionsText.Margin = rowMargin;

            Row++;

            CreateCheckbox("StealingAlwaysWorks", Lang.Settings.MaxStealRate, Lang.Settings.MaxStealRate_Tooltip);
            CreateCheckbox("NoAutoTrance", Lang.Settings.NoAutoTrance, Lang.Settings.NoAutoTrance_Tooltip);
            CreateCheckbox("GarnetConcentrate", Lang.Settings.DisableCantConcentrate, Lang.Settings.DisableCantConcentrate_Tooltip);
            CreateCheckbox("BreakDamageLimit", Lang.Settings.BreakDamageLimit, Lang.Settings.BreakDamageLimit_Tooltip);

            Row++;

            TextBlock accessBattleMenuText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.AccessBattleMenu), Row, 0, 2, 4);
            accessBattleMenuText.Foreground = Brushes.White;
            accessBattleMenuText.Margin = rowMargin;
            accessBattleMenuText.TextWrapping = TextWrapping.WrapWithOverflow;
            accessBattleMenuText.ToolTip = Lang.Settings.AccessBattleMenu_Tooltip;
            ComboBox accessBattleMenuBox = AddUiElement(UiComboBoxFactory.Create(), Row, 4, 2, 4);
            accessBattleMenuBox.ItemsSource = new String[]{
                Lang.Settings.AccessBattleMenuType0,
                Lang.Settings.AccessBattleMenuType1,
                Lang.Settings.AccessBattleMenuType2,
                Lang.Settings.AccessBattleMenuType3
            };
            accessBattleMenuBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(AccessBattleMenu)) { Mode = BindingMode.TwoWay });
            accessBattleMenuBox.Height = 20;
            accessBattleMenuBox.FontSize = 10;
            accessBattleMenuBox.Margin = rowMargin;

            Row++;

            CreateCheckbox("SpeedMode", Lang.Settings.SpeedMode, Lang.Settings.SpeedMode_Tooltip);

            Row++;

            TextBlock speedFactorTextindex = AddUiElement(UiTextBlockFactory.Create(""), Row, 0, 1, 1);
            speedFactorTextindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(SpeedFactor)) { Mode = BindingMode.TwoWay, StringFormat = "{0}x" });
            speedFactorTextindex.Foreground = Brushes.White;
            speedFactorTextindex.Margin = rowMargin;
            Slider speedFactor = AddUiElement(UiSliderFactory.Create(0), Row, 1, 1, 7);
            speedFactor.SetBinding(Slider.ValueProperty, new Binding(nameof(SpeedFactor)) { Mode = BindingMode.TwoWay });
            speedFactor.TickFrequency = 1;
            speedFactor.IsSnapToTickEnabled = true;
            speedFactor.TickPlacement = TickPlacement.BottomRight;
            speedFactor.Height = 20;
            speedFactor.Minimum = 2;
            speedFactor.Maximum = 12;
            speedFactor.Margin = new Thickness(0, 0, 3, 0);

            Row++;

            TextBlock BattleTPSText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.BattleTPS), Row, 0, 1, 8);
            BattleTPSText.Foreground = Brushes.White;
            BattleTPSText.Margin = rowMargin;
            BattleTPSText.ToolTip = Lang.Settings.BattleTPS_Tooltip;

            Row++;

            TextBlock BattleTPSindex = AddUiElement(UiTextBlockFactory.Create(""), Row, 0, 1, 1);
            BattleTPSindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(BattleTPSDividedBy10)) { Mode = BindingMode.TwoWay, StringFormat = "{0}x" });
            BattleTPSindex.Foreground = Brushes.White;
            BattleTPSindex.Margin = rowMargin;
            Slider BattleTPSFactor = AddUiElement(UiSliderFactory.Create(0), Row, 1, 1, 7);
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

            Row++;

            TextBlock tetraMasterReduceRandomText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.CardReduceRandom), Row, 0, 2, 4);
            tetraMasterReduceRandomText.Foreground = Brushes.White;
            tetraMasterReduceRandomText.Margin = rowMargin;
            tetraMasterReduceRandomText.TextWrapping = TextWrapping.WrapWithOverflow;
            tetraMasterReduceRandomText.ToolTip = Lang.Settings.CardReduceRandom_Tooltip;
            ComboBox tetraMasterReduceRandomBox = AddUiElement(UiComboBoxFactory.Create(), Row, 4, 2, 4);
            tetraMasterReduceRandomBox.ItemsSource = new String[]{
                Lang.Settings.tetraMasterReduceRandomBox0,
                Lang.Settings.tetraMasterReduceRandomBox1,
                Lang.Settings.tetraMasterReduceRandomBox2,
            };
            tetraMasterReduceRandomBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(ReduceRandom)) { Mode = BindingMode.TwoWay });
            tetraMasterReduceRandomBox.Height = 20;
            tetraMasterReduceRandomBox.FontSize = 10;
            tetraMasterReduceRandomBox.Margin = rowMargin;


            LoadSettings();
        }

    }
}
