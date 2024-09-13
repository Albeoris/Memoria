using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable ArrangeStaticMemberQualifier
#pragma warning disable 649
#pragma warning disable 169

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Shaders : Settings
    {
        public SettingsGrid_Shaders()
        {
            SetRows(7);
            SetCols(8);

            Width = 260;
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new Thickness(0);
            DataContext = this;

            Thickness rowMargin = new Thickness(0, 2, 0, 2);

            Int32 row = 0;


            TextBlock separateLineField = AddUiElement(UiTextBlockFactory.Create("╙ " + Lang.Settings.Shader_Field_chars), row: row, col: 0, rowSpan: 1, colSpan: 8);
            separateLineField.Margin = rowMargin;

            row++;

            CheckBox EnableRealismShadingForField = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Realism, null), row, 0, 1, 4);
            EnableRealismShadingForField.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Shader_Field_Realism)) { Mode = BindingMode.TwoWay });
            EnableRealismShadingForField.Foreground = Brushes.White;

            CheckBox EnableToonShadingForField = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Toon, null), row, 3, 1, 4);
            EnableToonShadingForField.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Shader_Field_Toon)) { Mode = BindingMode.TwoWay });
            EnableToonShadingForField.Foreground = Brushes.White;

            row++;

            CheckBox EnableOutlineForFieldCharacter = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Outlines, null), row, 0, 1, 8);
            EnableOutlineForFieldCharacter.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Shader_Field_Outlines)) { Mode = BindingMode.TwoWay });
            EnableOutlineForFieldCharacter.Foreground = Brushes.White;

            row++;

            TextBlock separateLineBattle = AddUiElement(UiTextBlockFactory.Create("╙ " + Lang.Settings.Shader_Battle_chars), row: row, col: 0, rowSpan: 1, colSpan: 8);
            separateLineBattle.Margin = rowMargin;

            row++;

            CheckBox EnableRealismShadingForBattle = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Realism, null), row, 0, 1, 4);
            EnableRealismShadingForBattle.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Shader_Battle_Realism)) { Mode = BindingMode.TwoWay });
            EnableRealismShadingForBattle.Foreground = Brushes.White;

            CheckBox EnableToonShadingForBattle = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Toon, null), row, 3, 1, 4);
            EnableToonShadingForBattle.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Shader_Battle_Toon)) { Mode = BindingMode.TwoWay });
            EnableToonShadingForBattle.Foreground = Brushes.White;

            row++;

            CheckBox EnableOutlineForBattleCharacter = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Outlines, null), row, 0, 1, 8);
            EnableOutlineForBattleCharacter.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Shader_Battle_Outlines)) { Mode = BindingMode.TwoWay });
            EnableOutlineForBattleCharacter.Foreground = Brushes.White;

            foreach (FrameworkElement child in Children)
            {
                //if (!ReferenceEquals(child, backround))
                //child.Margin = new Thickness(child.Margin.Left + 8, child.Margin.Top, child.Margin.Right + 8, child.Margin.Bottom);

                TextBlock textblock = child as TextBlock;
                if (textblock != null)
                {
                    textblock.Foreground = Brushes.WhiteSmoke;
                    textblock.FontWeight = FontWeight.FromOpenTypeWeight(500);
                    continue;
                }

                Control control = child as Control;
                if (control != null && !(control is ComboBox))
                    control.Foreground = Brushes.WhiteSmoke;
            }

            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }
    }
}
