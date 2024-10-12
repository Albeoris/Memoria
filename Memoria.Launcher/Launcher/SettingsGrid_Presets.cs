using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Presets : UiGrid
    {
        public SettingsGrid_Presets()
        {
            DataContext = this;

            try
            {
                RefreshPresets();
            }
            catch { }

            CreateHeading(Lang.Settings.Presets);

            Row++;
            RowDefinitions.Add(new RowDefinition());

            comboBox = new ComboBox();
            comboBox.ItemsSource = Presets;
            comboBox.FontWeight = FontWeight.FromOpenTypeWeight(FontWeightCombobox);
            comboBox.Margin = CommonMargin;
            comboBox.Height = ComboboxHeight;
            comboBox.SetValue(RowProperty, Row);
            comboBox.SetValue(ColumnProperty, 0);
            comboBox.SetValue(RowSpanProperty, 1);
            comboBox.SetValue(ColumnSpanProperty, 60);
            comboBox.SelectedIndex = 0;
            MakeTooltip(comboBox, Presets[0].Description);
            comboBox.MouseEnter += (sender, e) =>
            {
                comboBox.Focus();
            };
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
            Children.Add(comboBox);

            Button applyBtn = new Button();
            applyBtn.Content = Lang.Settings.Apply;
            applyBtn.Height = ComboboxHeight + 2;
            applyBtn.SetResourceReference(Button.StyleProperty, "ButtonStyle");
            applyBtn.SetValue(RowProperty, Row);
            applyBtn.SetValue(ColumnProperty, 61);
            applyBtn.SetValue(RowSpanProperty, 1);
            applyBtn.SetValue(ColumnSpanProperty, MaxColumns - 61);
            applyBtn.Click += ApplyBtn_Click;
            Children.Add(applyBtn);
        }

        private ComboBox comboBox;

        private void ApplyBtn_Click(Object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Lang.Settings.ApplyPresetText, Lang.Settings.ApplyPresetCaption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                IniReader settings = Presets[comboBox.SelectedIndex].Settings;
                settings.WriteAllSettings(IniFile.IniPath, ["Preset"]);

                MainWindow mainWindow = (MainWindow)this.GetRootElement();
                mainWindow.LoadSettings();
            }
        }

        private void ComboBox_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.ToolTip is ToolTip toolTip)
                toolTip.IsOpen = false;
            comboBox.ToolTip = null;

            if (comboBox.SelectedIndex < 0)
                return;

            if (!String.IsNullOrEmpty(Presets[comboBox.SelectedIndex].Description))
            {
                MakeTooltip(comboBox, Presets[comboBox.SelectedIndex].Description);

                if (comboBox.ToolTip is ToolTip newToolTip && comboBox.IsMouseOver)
                    newToolTip.IsOpen = true;
            }
        }

        public struct Preset
        {
            public String Name;
            public String Description;

            public IniReader Settings;

            public override String ToString()
            {
                return Name;
            }
        }

        public ObservableCollection<Preset> Presets = new ObservableCollection<Preset>();

        public void RefreshPresets()
        {
            Presets.Clear();

            Presets.Add(new Preset()
            {
                Name = Lang.Settings.PresetMemoria,
                Description = Lang.Settings.PresetMemoria_ToolTip,
                Settings = new IniReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Memoria.ini"))
            });

            List<IniReader.Key> toRemove = new List<IniReader.Key>();
            foreach (var key in Presets[0].Settings.Options.Keys)
            {
                List<String> options = ["Audio.MusicVolume", "Audio.SoundVolume", "Audio.MovieVolume", "VoiceActing.Volume"];
                if (key.Section == "Mod" || options.Contains($"{key.Section}.{key.Name}"))
                    toRemove.Add(key);
            }
            foreach (var key in toRemove)
            {
                Presets[0].Settings.Options.Remove(key);
            }

            Presets.Add(new Preset()
            {
                Name = Lang.Settings.PresetSteam,
                Description = Lang.Settings.PresetSteam_ToolTip,
                Settings = new IniReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("SteamPreset.ini"))
            });

            Presets.Add(new Preset()
            {
                Name = Lang.Settings.PresetPSX,
                Description = Lang.Settings.PresetPSX_ToolTip,
                Settings = new IniReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("PSXPreset.ini"))
            });

            if (Directory.Exists("Presets"))
            {
                foreach (String file in Directory.EnumerateFiles("Presets", "*.ini"))
                {
                    IniReader settings = new IniReader(file);

                    IniReader.Key nameKey = new IniReader.Key("Preset", "Name");
                    IniReader.Key descKey = new IniReader.Key("Preset", "Description");
                    Presets.Add(new Preset()
                    {
                        Name = (settings.Options.ContainsKey(nameKey)) ? settings.Options[nameKey] : Path.GetFileNameWithoutExtension(file),
                        Description = (settings.Options.ContainsKey(descKey)) ? settings.Options[descKey].Replace("\\n", "\n") : "",
                        Settings = settings
                    });

                }
            }
            if (comboBox != null)
            {
                comboBox.SelectedIndex = 0;
                MakeTooltip(comboBox, Presets[0].Description);
            }
        }
    }
}
