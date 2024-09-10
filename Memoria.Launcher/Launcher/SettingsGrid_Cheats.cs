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

            Int32 row = 0;

            TextBlock cheatOptionsText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.IniCheats), row, 0, 2, 8);
            cheatOptionsText.Padding = new Thickness(0, 4, 0, 2);
            cheatOptionsText.Foreground = Brushes.White;
            cheatOptionsText.FontSize = 14;
            cheatOptionsText.FontWeight = FontWeights.Bold;
            cheatOptionsText.Margin = rowMargin;

            row++;
            row++;

            CheckBox stealingAlwaysWorks = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.MaxStealRate, null), row, 0, 1, 8);
            stealingAlwaysWorks.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(StealingAlwaysWorks)) { Mode = BindingMode.TwoWay });
            stealingAlwaysWorks.Foreground = Brushes.White;
            stealingAlwaysWorks.Margin = rowMargin;
            stealingAlwaysWorks.ToolTip = Lang.Settings.MaxStealRate_Tooltip;

            row++;

            CheckBox noAutoTrance = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.NoAutoTrance, null), row, 0, 1, 8);
            noAutoTrance.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(NoAutoTrance)) { Mode = BindingMode.TwoWay });
            noAutoTrance.Foreground = Brushes.White;
            noAutoTrance.Margin = rowMargin;
            noAutoTrance.ToolTip = Lang.Settings.NoAutoTrance_Tooltip;

            row++;

            CheckBox garnetConcentrate = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.DisableCantConcentrate, null), row, 0, 1, 8);
            garnetConcentrate.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(GarnetConcentrate)) { Mode = BindingMode.TwoWay });
            garnetConcentrate.Foreground = Brushes.White;
            garnetConcentrate.Margin = rowMargin;
            garnetConcentrate.ToolTip = Lang.Settings.DisableCantConcentrate_Tooltip;

            row++;

            CheckBox breakDamageLimit = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.BreakDamageLimit, null), row, 0, 1, 8);
            breakDamageLimit.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(BreakDamageLimit)) { Mode = BindingMode.TwoWay });
            breakDamageLimit.Foreground = Brushes.White;
            breakDamageLimit.Margin = rowMargin;
            breakDamageLimit.ToolTip = Lang.Settings.BreakDamageLimit_Tooltip;

            row++;

            TextBlock accessBattleMenuText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.AccessBattleMenu), row, 0, 2, 4);
            accessBattleMenuText.Foreground = Brushes.White;
            accessBattleMenuText.Margin = rowMargin;
            accessBattleMenuText.TextWrapping = TextWrapping.WrapWithOverflow;
            accessBattleMenuText.ToolTip = Lang.Settings.AccessBattleMenu_Tooltip;
            ComboBox accessBattleMenuBox = AddUiElement(UiComboBoxFactory.Create(), row, 4, 2, 4);
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

            row++;
            row++;

            CheckBox speedMode = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SpeedMode, null), row, 0, 1, 8);
            speedMode.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(SpeedMode)) { Mode = BindingMode.TwoWay });
            speedMode.Foreground = Brushes.White;
            speedMode.Margin = rowMargin;
            speedMode.ToolTip = Lang.Settings.SpeedMode_Tooltip;

            row++;

            TextBlock speedFactorTextindex = AddUiElement(UiTextBlockFactory.Create(""), row, 0, 1, 1);
            speedFactorTextindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(SpeedFactor)) { Mode = BindingMode.TwoWay, StringFormat = "{0}x" });
            speedFactorTextindex.Foreground = Brushes.White;
            speedFactorTextindex.Margin = rowMargin;
            Slider speedFactor = AddUiElement(UiSliderFactory.Create(0), row, 1, 1, 7);
            speedFactor.SetBinding(Slider.ValueProperty, new Binding(nameof(SpeedFactor)) { Mode = BindingMode.TwoWay });
            speedFactor.TickFrequency = 1;
            speedFactor.IsSnapToTickEnabled = true;
            speedFactor.TickPlacement = TickPlacement.BottomRight;
            speedFactor.Height = 20;
            speedFactor.Minimum = 2;
            speedFactor.Maximum = 12;
            speedFactor.Margin = new Thickness(0, 0, 3, 0);

            row++;

            TextBlock BattleTPSText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.BattleTPS), row, 0, 1, 8);
            BattleTPSText.Foreground = Brushes.White;
            BattleTPSText.Margin = rowMargin;
            BattleTPSText.ToolTip = Lang.Settings.BattleTPS_Tooltip;

            row++;

            TextBlock BattleTPSindex = AddUiElement(UiTextBlockFactory.Create(""), row, 0, 1, 1);
            BattleTPSindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(BattleTPSDividedBy10)) { Mode = BindingMode.TwoWay, StringFormat = "{0}x" });
            BattleTPSindex.Foreground = Brushes.White;
            BattleTPSindex.Margin = rowMargin;
            Slider BattleTPSFactor = AddUiElement(UiSliderFactory.Create(0), row, 1, 1, 7);
            BattleTPSFactor.SetBinding(Slider.ValueProperty, new Binding(nameof(BattleTPS)) { Mode = BindingMode.TwoWay });
            BattleTPSFactor.TickFrequency = 1;
            BattleTPSFactor.IsSnapToTickEnabled = true;
            BattleTPSFactor.TickPlacement = TickPlacement.BottomRight;
            BattleTPSFactor.Height = 20;
            BattleTPSFactor.Minimum = 15;
            BattleTPSFactor.Maximum = 75;
            BattleTPSFactor.Margin = new Thickness(0, 0, 3, 0);

            row++;

            CheckBox battleAssistance = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.BattleAssistance, null), row, 0, 1, 8);
            battleAssistance.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(BattleAssistance)) { Mode = BindingMode.TwoWay });
            battleAssistance.Foreground = Brushes.White;
            battleAssistance.Margin = rowMargin;
            battleAssistance.ToolTip = Lang.Settings.BattleAssistance_Tooltip;

            row++;

            CheckBox noRandomEncounter = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.NoRandomBattles, null), row, 0, 1, 8);
            noRandomEncounter.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(NoRandomEncounter)) { Mode = BindingMode.TwoWay });
            noRandomEncounter.Foreground = Brushes.White;
            noRandomEncounter.Margin = rowMargin;
            noRandomEncounter.ToolTip = Lang.Settings.NoRandomBattles_Tooltip;

            row++;

            CheckBox masterSkill = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.PermanentCheats, null), row, 0, 1, 8);
            masterSkill.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(MasterSkill)) { Mode = BindingMode.TwoWay });
            masterSkill.Foreground = Brushes.White;
            masterSkill.Margin = rowMargin;
            masterSkill.ToolTip = Lang.Settings.PermanentCheats_Tooltip;

            row++;

            CheckBox maxTetraMasterCards = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.MaxCardCount, null), row, 0, 1, 8);
            maxTetraMasterCards.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(MaxCardCount)) { Mode = BindingMode.TwoWay });
            maxTetraMasterCards.Foreground = Brushes.White;
            maxTetraMasterCards.Margin = rowMargin;
            maxTetraMasterCards.ToolTip = Lang.Settings.MaxCardCount_Tooltip;

            row++;

            TextBlock tetraMasterReduceRandomText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.CardReduceRandom), row, 0, 2, 4);
            tetraMasterReduceRandomText.Foreground = Brushes.White;
            tetraMasterReduceRandomText.Margin = rowMargin;
            tetraMasterReduceRandomText.TextWrapping = TextWrapping.WrapWithOverflow;
            tetraMasterReduceRandomText.ToolTip = Lang.Settings.CardReduceRandom_Tooltip;
            ComboBox tetraMasterReduceRandomBox = AddUiElement(UiComboBoxFactory.Create(), row, 4, 2, 4);
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
