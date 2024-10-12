using System;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;

namespace Memoria.Launcher
{
    /// <summary>
    /// Interaction logic for Window_NewPreset.xaml
    /// </summary>
    public partial class Window_NewPreset : UserControl
    {
        public Window_NewPreset()
        {
            InitializeComponent();
            // TODO Language
            UiGrid.MakeTooltip(IncludeMods, "If enabled, this preset will override enabled mods when applied.\nOtherwise enabled mods will remain totally unaffected when applying this preset.");
        }

        private void Close(Object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Children.Remove(this);
        }

        private void Bg_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            Window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e333"));
        }

        private void Bg_MouseUp(Object sender, MouseButtonEventArgs e)
        {
            Window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e000"));
        }

        private void Window_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void PresetName_TextChanged(Object sender, TextChangedEventArgs e)
        {
            int i = PresetName.CaretIndex;
            PresetName.Text = RemoveSpecialCharacters(PresetName.Text);
            PresetName.CaretIndex = i;

            Ok.IsEnabled = PresetName.Text.Trim().Length > 0;
        }
        public String RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_ ]+", " ", RegexOptions.Compiled);
        }

        private void Ok_Click(Object sender, RoutedEventArgs e)
        {
            String path = $"Presets/{PresetName.Text.Trim()}.ini";
            if (File.Exists(path))
            {
                // TODO Language
                if (MessageBox.Show("A preset with this name already exists. Do you want to replace it?", "Overwrite Preset?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            }

            if (!Directory.Exists("Presets"))
                Directory.CreateDirectory("Presets");

            String desc = PresetDescription.Text.Trim().Replace("\r", "").Replace("\n", "\\n");

            String header = $"[Preset]\nName = {PresetName.Text}";
            if (!String.IsNullOrEmpty(desc))
                header = $"{header}\nDescription = {desc}";

            File.WriteAllText(path, header);

            IniReader reader = new IniReader(IniFile.IniPath);
            reader.WriteAllSettings(path, IncludeMods.IsChecked == true ? ["Debug", "Export", "Import"] : ["Mod", "Debug", "Export", "Import"], ["Audio.MusicVolume", "Audio.SoundVolume", "Audio.MovieVolume", "VoiceActing.Volume"]);

            ((MainWindow)((Grid)Parent).Parent).SettingsGrid_Presets.RefreshPresets();

            Close(this, null);
        }

        private void PresetName_KeyUp(Object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Ok_Click(this, null);
                e.Handled = true;
            }
        }
    }
}
