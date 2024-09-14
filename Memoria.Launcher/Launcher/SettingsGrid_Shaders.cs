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



            TextBlock separateLineField = AddUiElement(UiTextBlockFactory.Create("╙ " + Lang.Settings.Shader_Field_chars), Row, 0, 1, 8);
            separateLineField.Margin = rowMargin;

            CreateCheckbox("Shader_Field_Realism", Lang.Settings.Shader_Realism, "");
            CreateCheckbox("Shader_Field_Toon", Lang.Settings.Shader_Toon, "", 4);
            CreateCheckbox("Shader_Field_Outlines", Lang.Settings.Shader_Outlines, "");

            Row++;
            TextBlock separateLineBattle = AddUiElement(UiTextBlockFactory.Create("╙ " + Lang.Settings.Shader_Battle_chars), Row, 0, 1, 8);
            separateLineBattle.Margin = rowMargin;

            CreateCheckbox("Shader_Battle_Realism", Lang.Settings.Shader_Realism, "");
            CreateCheckbox("Shader_Battle_Toon", Lang.Settings.Shader_Toon, "", 4);
            CreateCheckbox("Shader_Battle_Outlines", Lang.Settings.Shader_Outlines, "");


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

            LoadSettings();
        }
    }
}
