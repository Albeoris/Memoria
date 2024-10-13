using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Presets : UiGrid
    {
        private ComboBox comboBox;
        private Button deleteBtn;

        public SettingsGrid_Presets()
        {
            DataContext = this;

            try
            {
                RefreshPresets();

            }
            catch { }

            CreateHeading("Settings.Presets");

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
            try
            {
                comboBox.SelectedIndex = 0;
                MakeTooltip(comboBox, Presets[0].Description);
            }
            catch { }
            comboBox.MouseEnter += (sender, e) =>
            {
                comboBox.Focus();
            };
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
            Children.Add(comboBox);

            deleteBtn = new Button();
            deleteBtn.SetResourceReference(Button.StyleProperty, "ButtonStyleRed");
            deleteBtn.Height = ComboboxHeight + 2;
            deleteBtn.IsEnabled = false;
            deleteBtn.Visibility = Visibility.Hidden;
            deleteBtn.SetValue(RowProperty, Row);
            deleteBtn.SetValue(ColumnProperty, 52);
            deleteBtn.SetValue(RowSpanProperty, 1);
            deleteBtn.SetValue(ColumnSpanProperty, 8);
            deleteBtn.Content = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/images/btnUninstallimg_small.png")),
                Height = 15,
                Width = 15,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0)
            };
            deleteBtn.Click += DeleteBtn_Click;
            Children.Add(deleteBtn);

            MakeTooltip(deleteBtn, "Launcher.DeletePreset_Tooltip", "", "hand");

            Button applyBtn = new Button();
            applyBtn.SetResourceReference(Button.ContentProperty, "Settings.Apply");
            applyBtn.SetResourceReference(Button.StyleProperty, "ButtonStyle");
            applyBtn.Height = ComboboxHeight + 2;
            applyBtn.SetValue(RowProperty, Row);
            applyBtn.SetValue(ColumnProperty, 61);
            applyBtn.SetValue(RowSpanProperty, 1);
            applyBtn.SetValue(ColumnSpanProperty, MaxColumns);
            applyBtn.Click += ApplyBtn_Click;
            Children.Add(applyBtn);
        }

        private void DeleteBtn_Click(Object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedIndex < 0 || Presets[comboBox.SelectedIndex].Path == null)
                return;

            if (MessageBox.Show((String)Lang.Res["Launcher.DeletePresetText"], (String)Lang.Res["Launcher.DeletePresetCaption"], MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                File.Delete(Presets[comboBox.SelectedIndex].Path);
                RefreshPresets();
            }
        }

        private void ApplyBtn_Click(Object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show((String)Lang.Res["Settings.ApplyPresetText"], (String)Lang.Res["Settings.ApplyPresetCaption"], MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                IniReader settings = Presets[comboBox.SelectedIndex].Settings;
                settings.WriteAllSettings(IniFile.IniPath, ["Preset"]);

                MainWindow mainWindow = (MainWindow)this.GetRootElement();
                mainWindow.LoadSettings();
                mainWindow.LoadModSettings();
                mainWindow.UpdateLauncherTheme();
            }
        }

        private void ComboBox_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.ToolTip is ToolTip toolTip)
                toolTip.IsOpen = false;
            comboBox.ToolTip = null;

            if (comboBox.SelectedIndex < 0)
                return;

            if (comboBox.SelectedIndex < 3)
            {
                comboBox.SetValue(ColumnSpanProperty, 60);
                deleteBtn.IsEnabled = false;
                deleteBtn.Visibility = Visibility.Hidden;
            }
            else
            {
                comboBox.SetValue(ColumnSpanProperty, 51);
                deleteBtn.IsEnabled = true;
                deleteBtn.Visibility = Visibility.Visible;
            }

            if (!String.IsNullOrEmpty(Presets[comboBox.SelectedIndex].Description))
            {
                MakeTooltip(comboBox, Presets[comboBox.SelectedIndex].Description);

                if (comboBox.ToolTip is ToolTip newToolTip && comboBox.IsMouseOver)
                    newToolTip.IsOpen = true;
            }
        }

        public struct Preset
        {
            public String Path;
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

            if (comboBox != null && deleteBtn != null)
            {
                comboBox.SetValue(ColumnSpanProperty, 60);
                deleteBtn.IsEnabled = false;
                deleteBtn.Visibility = Visibility.Hidden;
            }

            Presets.Add(new Preset()
            {
                Name = (String)Lang.Res["Settings.PresetMemoria"],
                Description = (String)Lang.Res["Settings.PresetMemoria_Tooltip"],
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
                Name = (String)Lang.Res["Settings.PresetSteam"],
                Description = (String)Lang.Res["Settings.PresetSteam_Tooltip"],
                Settings = new IniReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("SteamPreset.ini"))
            });

            Presets.Add(new Preset()
            {
                Name = (String)Lang.Res["Settings.PresetPSX"],
                Description = (String)Lang.Res["Settings.PresetPSX_Tooltip"],
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
                        Path = file,
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
