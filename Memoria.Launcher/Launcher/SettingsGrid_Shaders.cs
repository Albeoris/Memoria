using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;

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
            SetRows(2);
            SetCols(8);

            Width = 260;
            Margin = new Thickness(0);
            DataContext = this;


            CreateTextbloc(Lang.Settings.Shader_Enable, true);
            CreateTextbloc("╙ " + Lang.Settings.Shader_Field_chars);

            CreateCheckbox("Shader_Field_Realism", Lang.Settings.Shader_Realism);
            CreateCheckbox("Shader_Field_Toon", Lang.Settings.Shader_Toon, "", 4);
            CreateCheckbox("Shader_Field_Outlines", Lang.Settings.Shader_Outlines);

            CreateTextbloc("╙ " + Lang.Settings.Shader_Battle_chars);

            CreateCheckbox("Shader_Battle_Realism", Lang.Settings.Shader_Realism);
            CreateCheckbox("Shader_Battle_Toon", Lang.Settings.Shader_Toon, "", 4);
            CreateCheckbox("Shader_Battle_Outlines", Lang.Settings.Shader_Outlines);

            LoadSettings();
        }
    }
}
