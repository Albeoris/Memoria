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
        public ComboBox PresetsComboBox { get; private set; }
        public Button DeleteButton { get; private set; }
        public Button ApplyButton { get; private set; }

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
            PresetsComboBox = new ComboBox();
            PresetsComboBox.ItemsSource = Presets;
            PresetsComboBox.FontWeight = FontWeight.FromOpenTypeWeight(FontWeightCombobox);
            PresetsComboBox.Margin = CommonMargin;
            PresetsComboBox.Height = ComboboxHeight;
            PresetsComboBox.SetValue(RowProperty, Row);
            PresetsComboBox.SetValue(ColumnProperty, 0);
            PresetsComboBox.SetValue(RowSpanProperty, 1);
            PresetsComboBox.SetValue(ColumnSpanProperty, 60);
            try
            {
                PresetsComboBox.SelectedIndex = 0;
                MakeTooltip(PresetsComboBox, Presets[0].Description);
            }
            catch { }
            PresetsComboBox.MouseEnter += (sender, e) =>
            {
                PresetsComboBox.Focus();
            };
            PresetsComboBox.SelectionChanged += PresetsComboBox_SelectionChanged;
            Children.Add(PresetsComboBox);

            DeleteButton = new Button();
            DeleteButton.SetResourceReference(Button.StyleProperty, "ButtonStyleRed");
            DeleteButton.Height = ComboboxHeight + 2;
            DeleteButton.IsEnabled = false;
            DeleteButton.Visibility = Visibility.Hidden;
            DeleteButton.SetValue(RowProperty, Row);
            DeleteButton.SetValue(ColumnProperty, 52);
            DeleteButton.SetValue(RowSpanProperty, 1);
            DeleteButton.SetValue(ColumnSpanProperty, 8);
            try
            {
                DeleteButton.Content = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/images/btnUninstallimg_small.png")),
                    Height = 15,
                    Width = 15,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0)
                };
            }
            catch { }
            DeleteButton.Click += DeleteBtn_Click;
            Children.Add(DeleteButton);

            MakeTooltip(DeleteButton, "Launcher.DeletePreset_Tooltip", "", "hand");

            ApplyButton = new Button();
            ApplyButton.SetResourceReference(Button.ContentProperty, "Settings.Apply");
            ApplyButton.SetResourceReference(Button.StyleProperty, "ButtonStyle");
            ApplyButton.Height = ComboboxHeight + 2;
            ApplyButton.SetValue(RowProperty, Row);
            ApplyButton.SetValue(ColumnProperty, 61);
            ApplyButton.SetValue(RowSpanProperty, 1);
            ApplyButton.SetValue(ColumnSpanProperty, MaxColumns);
            ApplyButton.Click += ApplyBtn_Click;
            Children.Add(ApplyButton);
        }

        private void DeleteBtn_Click(Object sender, RoutedEventArgs e)
        {
            if (PresetsComboBox.SelectedIndex < 0 || Presets[PresetsComboBox.SelectedIndex].Path == null)
                return;

            if (MessageBox.Show((String)Lang.Res["Launcher.DeletePresetText"], (String)Lang.Res["Launcher.DeletePresetCaption"], MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                File.Delete(Presets[PresetsComboBox.SelectedIndex].Path);
                RefreshPresets();
            }
        }

        private void ApplyBtn_Click(Object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show((String)Lang.Res["Settings.ApplyPresetText"], (String)Lang.Res["Settings.ApplyPresetCaption"], MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                IniFile settings = Presets[PresetsComboBox.SelectedIndex].Settings;
                settings.WriteAllSettings(IniFile.MemoriaIniPath, ["Preset"]);
                IniFile.MemoriaIni.Reload();

                MainWindow mainWindow = (MainWindow)this.GetRootElement();
                mainWindow.LoadSettings();
                mainWindow.LoadModSettings();
                mainWindow.UpdateLauncherTheme();
            }
        }

        private void PresetsComboBox_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (PresetsComboBox.ToolTip is ToolTip toolTip)
                toolTip.IsOpen = false;
            PresetsComboBox.ToolTip = null;

            if (PresetsComboBox.SelectedIndex < 0)
                return;

            if (PresetsComboBox.SelectedIndex < 3)
            {
                PresetsComboBox.SetValue(ColumnSpanProperty, 60);
                DeleteButton.IsEnabled = false;
                DeleteButton.Visibility = Visibility.Hidden;
            }
            else
            {
                PresetsComboBox.SetValue(ColumnSpanProperty, 51);
                DeleteButton.IsEnabled = true;
                DeleteButton.Visibility = Visibility.Visible;
            }

            if (!String.IsNullOrEmpty(Presets[PresetsComboBox.SelectedIndex].Description))
            {
                MakeTooltip(PresetsComboBox, Presets[PresetsComboBox.SelectedIndex].Description);

                if (PresetsComboBox.ToolTip is ToolTip newToolTip && PresetsComboBox.IsMouseOver)
                    newToolTip.IsOpen = true;
            }
        }

        public struct Preset
        {
            public String Path;
            public String Name;
            public String Description;

            public IniFile Settings;

            public override String ToString()
            {
                return Name;
            }
        }

        public ObservableCollection<Preset> Presets = new ObservableCollection<Preset>();

        public void RefreshPresets()
        {
            Presets.Clear();

            if (PresetsComboBox != null && DeleteButton != null)
            {
                PresetsComboBox.SetValue(ColumnSpanProperty, 60);
                DeleteButton.IsEnabled = false;
                DeleteButton.Visibility = Visibility.Hidden;
            }

            Presets.Add(new Preset()
            {
                Name = (String)Lang.Res["Settings.PresetMemoria"],
                Description = (String)Lang.Res["Settings.PresetMemoria_Tooltip"],
                Settings = new IniFile(Assembly.GetExecutingAssembly().GetManifestResourceStream("Memoria.ini"))
            });

            List<IniFile.Key> toRemove = new List<IniFile.Key>();
            foreach (var key in Presets[0].Settings.Options.Keys)
            {
                List<String> options = ["Graphics.AntiAliasing", "Control.KeyBindings", "AnalogControl.StickThreshold", "Audio.MusicVolume", "Audio.SoundVolume", "Audio.MovieVolume", "VoiceActing.Volume"];
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
                Settings = new IniFile(Assembly.GetExecutingAssembly().GetManifestResourceStream("SteamPreset.ini"))
            });

            Presets.Add(new Preset()
            {
                Name = (String)Lang.Res["Settings.PresetPSX"],
                Description = (String)Lang.Res["Settings.PresetPSX_Tooltip"],
                Settings = new IniFile(Assembly.GetExecutingAssembly().GetManifestResourceStream("PSXPreset.ini"))
            });

            if (Directory.Exists("Presets"))
            {
                foreach (String file in Directory.EnumerateFiles("Presets", "*.ini"))
                {
                    IniFile settings = new IniFile(file);

                    IniFile.Key nameKey = new IniFile.Key("Preset", "Name");
                    IniFile.Key descKey = new IniFile.Key("Preset", "Description");
                    Presets.Add(new Preset()
                    {
                        Path = file,
                        Name = (settings.Options.ContainsKey(nameKey)) ? settings.Options[nameKey] : Path.GetFileNameWithoutExtension(file),
                        Description = (settings.Options.ContainsKey(descKey)) ? settings.Options[descKey].Replace("\\n", "\n") : "",
                        Settings = settings
                    });

                }
            }
            if (PresetsComboBox != null)
            {
                PresetsComboBox.SelectedIndex = 0;
                MakeTooltip(PresetsComboBox, Presets[0].Description);
            }
        }
    }
}
