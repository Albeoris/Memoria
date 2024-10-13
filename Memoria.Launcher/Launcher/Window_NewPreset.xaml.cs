using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            UiGrid.MakeTooltip(IncludeMods, "Launcher.IncludeMods_Tooltip");
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
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.UpdateModSettings();

            String path = $"Presets/{PresetName.Text.Trim()}.ini";
            if (File.Exists(path))
            {
                if (MessageBox.Show((String)Lang.Res["Launcher.OverridePresetMessage"], (String)Lang.Res["Launcher.OverridePresetCaption"], MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
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
            if(IncludeMods.IsChecked == true)
                reader.WriteAllSettings(path, ["Debug", "Export", "Import"], ["Mod.UseFileList", "Mod.MergeScripts", "Audio.MusicVolume", "Audio.SoundVolume", "Audio.MovieVolume", "VoiceActing.Volume"]);
            else
                reader.WriteAllSettings(path, ["Mod", "Debug", "Export", "Import"], ["Audio.MusicVolume", "Audio.SoundVolume", "Audio.MovieVolume", "VoiceActing.Volume"]);


            mainWindow.SettingsGrid_Presets.RefreshPresets();

            Close(this, null);
        }

        private void PresetName_KeyUp(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Ok_Click(this, null);
                e.Handled = true;
            }
        }
    }
}
