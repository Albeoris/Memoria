using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Memoria.Launcher
{
    public partial class MainWindow : Window, IComponentConnector
    {
        //public ModManagerWindow ModdingWindow;
        public static DateTime MemoriaAssemblyCompileDate;

        public MainWindow()
        {
            Lang.Initialize();
            String launcherDirectory = Directory.GetCurrentDirectory();
            try
            {
                // Official version since Aug 6, 2020:                          1.0.7141.27878
                // Memoria versions based on it until Nov 11, 2022 (included):  1.0.7141.27878
                // Memoria versions after it:                                   1.1.*
                Directory.SetCurrentDirectory(Path.GetDirectoryName(launcherDirectory + ASSEMBLY_CSHARP_PATH));
                Version assemblyVersion = AssemblyName.GetAssemblyName(Path.GetFileName(ASSEMBLY_CSHARP_PATH)).Version;
                Directory.SetCurrentDirectory(launcherDirectory);
                MemoriaAssemblyCompileDate = new DateTime(2000, 1, 1).AddDays(assemblyVersion.Build).AddSeconds(assemblyVersion.Revision * 2);
            }
            catch (Exception)
            {
                Directory.SetCurrentDirectory(launcherDirectory);
                MemoriaAssemblyCompileDate = new DateTime(2000, 1, 1);
            }

            InitializeComponent();

            PlayButton.GameSettings = GameSettings;
            PlayButton.GameSettingsDisplay = GameSettingsDisplay;
            Loaded += OnLoaded;
            Closing += new CancelEventHandler(OnClosing);
            LoadSettings();
            KeyUp += ModManagerWindow_KeyUp;
        }

        public static readonly Color DefaultAccentColor = (Color)ColorConverter.ConvertFromString("#CC427599"); // CC355566
        public const String DefaultBackgroundImage = "pack://application:,,,/images/new_launcher_bg2.png";

        private async void OnLoaded(Object sender, RoutedEventArgs e)
        {
            try
            {
                if (Directory.Exists(Mod.INSTALLATION_TMP))
                    Directory.Delete(Mod.INSTALLATION_TMP, true);
            }
            catch { }
            UpdateLauncherTheme();
            HotfixForMoguriMod();
            Lang.Res["Settings.LauncherWindowTitle"] += " | v" + MemoriaAssemblyCompileDate.ToString("yyyy.MM.dd");
            Lang.Res["Settings.MemoriaEngine"] += " v" + MemoriaAssemblyCompileDate.ToString("yyyy.MM.dd");

            UiGrid.MakeTooltip(NewPresetButton, "Launcher.CreatePreset_Tooltip", "", "hand");
            UiGrid.MakeTooltip(ModelViewerButton, "Launcher.ModelViewerButton_Tooltip", "", "hand");
            UiGrid.MakeTooltip(CopyLogButton, "Launcher.CopyLogButton_Tooltip", "", "hand");

            SetupFrameLang();
            UpdateCatalog();
            LoadModSettings();
            CheckForValidModFolder();
            UpdateModListInstalled();
            lstCatalogMods.ItemsSource = ModListCatalog;
            lstMods.ItemsSource = ModListInstalled;
            lstDownloads.ItemsSource = DownloadList;
            UpdateCatalogInstallationState();

            lstCatalogMods.SelectionChanged += OnModListSelect;
            lstMods.SelectionChanged += OnModListSelect;
            tabCtrlMain.SelectionChanged += OnModListSelect;
            ModOptionsHeaderButton.MouseUp += ModOptionsHeaderButton_MouseUp;
            if (ModListInstalled.Count == 0)
                tabCtrlMain.SelectedIndex = 1;
            UpdateModDetails((Mod)null);
            CheckOutdatedAndIncompatibleMods();

            // add tooltip style to manager's buttons
            UiGrid.MakeTooltip(btnReorganize, "ModEditor.TooltipReorganize", "", "hand");
            UiGrid.MakeTooltip(btnMoveUp, "ModEditor.TooltipMoveUp", "", "hand");
            UiGrid.MakeTooltip(btnMoveDown, "ModEditor.TooltipMoveDown", "", "hand");
            UiGrid.MakeTooltip(btnUninstall, "ModEditor.TooltipUninstall", "", "hand");
            UiGrid.MakeTooltip(btnDownload, "ModEditor.TooltipDownload", "", "hand");
            UiGrid.MakeTooltip(btnCancel, "ModEditor.TooltipCancel", "", "hand");

            String version = IniFile.SettingsIni.GetSetting("Memoria", "Version", "2000.01.01");
            DateTime currentVersion = DateTime.ParseExact(MemoriaAssemblyCompileDate.ToString("yyyy.MM.dd"), "yyyy.MM.dd", CultureInfo.InvariantCulture);
            if (!DateTime.TryParseExact(version, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) || date < currentVersion)
            {
                ShowReleaseNotes(null, null);
                // One time patches
                if (date < new DateTime(2024, 11, 25))
                {
                    // Patch resolution to Auto
                    if (IniFile.SettingsIni.GetSetting("Settings", "WindowMode", "0") != "0")
                        GameSettingsDisplay.ScreenResolution = (String)Lang.Res["Launcher.Auto"];
                    // Set AntiAliasing to 0
                    IniFile.MemoriaIni.SetSetting("Graphics", "AntiAliasing", "0");
                }
                else if (date < new DateTime(2025, 05, 19))
                {
                    // Set FPS to auto
                    FPSDropboxChoice = 0;
                }
                else if (date < new DateTime(2025, 07, 05))
                {
                    // Enable check update
                    IniFile.SettingsIni.SetSetting("Version", "CheckUpdates", "True");
                    // Disable themes mod
                    foreach (var mod in ModListInstalled)
                    {
                        if (mod.InstallationPath == "MemoriaLauncherThemes")
                        {
                            mod.IsActive = false;
                            break;
                        }
                    }
                    UpdateModSettings();
                    UpdateLauncherTheme();
                }
                else if(date < new DateTime(2025, 07, 13))
                {
                    // Make sure to use the new back-end
                    IniFile.MemoriaIni.SetSetting("Audio", "Backend", "1");
                    IniFile.MemoriaIni.Save();
                }
                // Set windows mode to 0 if it can't be parsed
                if (!Int32.TryParse(IniFile.SettingsIni.GetSetting("Settings", "WindowMode", "null"), NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                    IniFile.SettingsIni.SetSetting("Settings", "WindowMode", "0");

                IniFile.SettingsIni.SetSetting("Memoria", "Version", MemoriaAssemblyCompileDate.ToString("yyyy.MM.dd"));
                IniFile.SettingsIni.Save();
            }
            else if (GameSettings.AutoRunGame)
                PlayButton.Click();

            String checkUpdates = IniFile.SettingsIni.GetSetting("Memoria", "CheckUpdates", "True");
            if (!Boolean.TryParse(checkUpdates, out Boolean result) || result)
            {
                await UiLauncherPlayButton.CheckUpdates((Window)this.GetRootElement(), new ManualResetEvent(false), GameSettings);
            }
        }

        private void ModOptionsHeaderButton_MouseUp(Object sender, MouseButtonEventArgs e)
        {
            Boolean collapsed = (String)ModOptionsHeaderArrow.Content == "▲";
            ModOptionsHeaderArrow.Content = collapsed ? "▼" : "▲";

            DoubleAnimation animation = new DoubleAnimation
            {
                From = collapsed ? ModOptionsHeaderButton.ActualHeight : GroupModInfoWrapper.ActualHeight,
                To = collapsed ? GroupModInfoWrapper.ActualHeight : ModOptionsHeaderButton.ActualHeight,
                Duration = new TimeSpan(0, 0, 0, 0, 150),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            ModOptions.BeginAnimation(RowDefinition.MaxHeightProperty, animation);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the window when the button is clicked
        }
        private void ModelViewerButton_Click(object sender, RoutedEventArgs e)
        {
            this.PlayButton.Click(true);
        }

        private void SavePreset_Click(object sender, RoutedEventArgs e)
        {
            var presetWindow = new Window_NewPreset();
            MainWindowGrid.Children.Add(presetWindow);
            Dispatcher.BeginInvoke(DispatcherPriority.Input,
                new Action(delegate ()
                {
                    presetWindow.PresetName.Focus();         // Set Logical Focus
                    Keyboard.Focus(presetWindow.PresetName); // Set Keyboard Focus
                }));
        }
        private void CopyLogButton_Click(object sender, RoutedEventArgs e)
        {
            string logFilePath = "Memoria.log";
            try
            {
                if (File.Exists(logFilePath))
                {
                    string logContent = File.ReadAllText(logFilePath);
                    if (!string.IsNullOrWhiteSpace(logContent))
                    {
                        Clipboard.SetText(logContent);
                        ShowTemporaryMessage((String)Lang.Res["Launcher.CopyLogButton_Success"], true);
                    }
                    else
                        ShowTemporaryMessage((String)Lang.Res["Launcher.CopyLogButton_Empty"], false);
                }
                else
                    ShowTemporaryMessage((String)Lang.Res["Launcher.CopyLogButton_Doesntexist"], false);
            }
            catch (Exception ex)
            {
                ShowTemporaryMessage((String)Lang.Res["Launcher.CopyLogButton_Error"] + ex.Message, false);
            }
        }

        private void MainWindow_DragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;
            try
            {
                if (!e.Data.GetDataPresent(DataFormats.FileDrop, true))
                    return;

                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                foreach (string filename in filenames)
                {
                    String ext = Path.GetExtension(filename).ToLowerInvariant();

                    // Check if it's a Preset
                    if (ext == ".ini")
                    {
                        foreach (String line in File.ReadLines(filename))
                        {
                            if (!line.StartsWith("[Preset]"))
                                continue;
                            // TODO language:
                            dropLabel.Content = "Install Preset";
                            dropBackground.Visibility = Visibility.Visible;
                            Activate();
                            break;
                        }
                        continue;
                    }

                    // Check if it's a Mod
                    if (!SupportedArchives.Contains(ext))
                        continue;

                    if (FindModRoot(filename) != null)
                    {
                        // TODO translate:
                        dropLabel.Content = Lang.Res["Launcher.InstallMod"];
                        dropBackground.Visibility = Visibility.Visible;
                    }
                }
            }
            catch { }
        }
        private void MainWindow_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            if (dropBackground.Visibility != Visibility.Visible)
                e.Effects = DragDropEffects.None;
            else
                e.Effects = DragDropEffects.Copy;
        }
        private void MainWindow_DragLeave(object sender, DragEventArgs e)
        {
            e.Handled = true;
            dropBackground.Visibility = Visibility.Hidden;
        }

        private async void MainWindow_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            try
            {
                if (!e.Data.GetDataPresent(DataFormats.FileDrop, true))
                    return;

                Activate();

                String[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as String[];
                foreach (string filename in filenames)
                {
                    String ext = Path.GetExtension(filename).ToLowerInvariant();

                    // Check if it's a Preset
                    if (ext == ".ini")
                    {
                        foreach (String line in File.ReadLines(filename))
                        {
                            if (!line.StartsWith("[Preset]"))
                                continue;

                            // Install the preset
                            IniFile preset = new IniFile(filename);
                            String dst = Path.Combine("Presets", Path.GetFileName(filename));
                            if (File.Exists(dst))
                            {
                                // TODO language:
                                if (MessageBox.Show($"The preset '{Path.GetFileNameWithoutExtension(filename)}' already exits.\nWould you like to overwrite it?", "Overwrite preset?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                                    continue;
                                File.Delete(dst);
                            }
                            if (!Directory.Exists("Presets"))
                                Directory.CreateDirectory("Presets");
                            File.Copy(filename, dst);
                            SettingsGrid_Presets.RefreshPresets();

                            String presetName = preset.GetSetting("Preset", "Name", Path.GetFileNameWithoutExtension(filename));
                            foreach (SettingsGrid_Presets.Preset item in SettingsGrid_Presets.Presets)
                            {
                                if (item.Name != presetName)
                                    continue;

                                SettingsGrid_Presets.PresetsComboBox.SelectedItem = item;
                                break;
                            }

                            // Apply the preset
                            // TODO language:
                            if (MessageBox.Show($"Would you like to apply the preset '{presetName}' now?", "Apply preset?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                                continue;

                            preset.WriteAllSettings(IniFile.MemoriaIniPath, ["Preset"]);
                            IniFile.MemoriaIni.Reload();

                            MainWindow mainWindow = (MainWindow)this.GetRootElement();
                            mainWindow.LoadSettings();
                            mainWindow.LoadModSettings();
                            mainWindow.UpdateLauncherTheme();
                            break;
                        }
                        continue;
                    }

                    if (!SupportedArchives.Contains(ext))
                        continue;

                    // Extract the archive
                    // TODO language:
                    dropLabel.Content = $"Extracting '{Path.GetFileName(filename)}'";
                    Mod modInfo = await InstallModFromArchive(filename, null, (progress) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            dropLabel.Content = $"Extracting '{Path.GetFileName(filename)}' - {progress}%";
                        });
                    });

                    // Refresh mods list and activate the mod
                    UpdateModListInstalled();
                    UpdateCatalogInstallationState();
                    CheckOutdatedAndIncompatibleMods();
                    UpdateModSettings();
                    modInfo = Mod.SearchMod(ModListInstalled, modInfo);
                    // TODO language:
                    MessageBox.Show($"The mod '{modInfo.Name}{(modInfo.CurrentVersion != null ? " " + modInfo.CurrentVersion : "")}' has been successfully installed", "Mod installed", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception err)
            {
                // TODO language:
                MessageBox.Show($"Failed to automatically install the mod\n{err.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dropBackground.Visibility = Visibility.Hidden;
            }
        }

        private void ShowTemporaryMessage(string message, bool success)
        {
            CopyLogButton_StatusMessage.Text = message;
            CopyLogButton_StatusMessage.Foreground = success ? Brushes.LightGreen : Brushes.Red;
            CopyLogButton_StatusMessage.Visibility = Visibility.Visible;

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, args) =>
            {
                CopyLogButton_StatusMessage.Text = "";
                CopyLogButton_StatusMessage.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
            timer.Start();
        }

        private void HotfixForMoguriMod()
        {
            // Make sure tiles are set to 64
            if (Directory.Exists("MoguriFiles") && !File.Exists("MoguriFiles/Memoria.ini"))
                File.WriteAllText("MoguriFiles/Memoria.ini", "[Graphics]\nEnabled = 1\nTileSize = 64\n");

            // Make sure mbg116.bytes doesn't exist - for crash at the end
            if (Directory.Exists("MoguriFiles"))
            {
                if (File.Exists("MoguriFiles/StreamingAssets/ma/mbg116.bytes"))
                {
                    if (File.Exists("MoguriFiles/StreamingAssets/ma/mbg116 - bugged.bytes")) // already copied
                        File.Delete("MoguriFiles/StreamingAssets/ma/mbg116.bytes");
                    else
                        File.Move("MoguriFiles/StreamingAssets/ma/mbg116.bytes", "MoguriFiles/StreamingAssets/ma/mbg116 - bugged.bytes");
                }
            }

            // Make sure FMV059.bytes and FMV060.bytes don't exist: slightly bugged for minimal impact
            /*
            if (Directory.Exists("MoguriVideo"))
            {
                if (File.Exists("MoguriVideo/StreamingAssets/ma/FMV059.bytes"))
                {
                    if (File.Exists("MoguriVideo/StreamingAssets/ma/FMV059 - bugged.bytes")) // already copied
                        File.Delete("MoguriVideo/StreamingAssets/ma/FMV059.bytes");
                    else
                        File.Move("MoguriVideo/StreamingAssets/ma/FMV059.bytes", "MoguriVideo/StreamingAssets/ma/FMV059 - bugged.bytes");
                }
                if (File.Exists("MoguriVideo/StreamingAssets/ma/FMV060.bytes"))
                {
                    if (File.Exists("MoguriVideo/StreamingAssets/ma/FMV060 - bugged.bytes")) // already copied
                        File.Delete("MoguriVideo/StreamingAssets/ma/FMV060.bytes");
                    else
                        File.Move("MoguriVideo/StreamingAssets/ma/FMV060.bytes", "MoguriVideo/StreamingAssets/ma/FMV060 - bugged.bytes");
                }
            }
            */
        }

        public static void SetAccentColor(ALChColor color)
        {
            float shade = 0.05f;

            ALChColor darker = new ALChColor(color);
            darker.L -= shade;
            Application.Current.Resources["AccentColorDarker"] = (Color)darker;
            Application.Current.Resources["BrushAccentColorDarker"] = new SolidColorBrush(darker);

            Application.Current.Resources["AccentColor"] = (Color)color;
            Application.Current.Resources["BrushAccentColor"] = new SolidColorBrush(color);
            color.L += shade;
            Application.Current.Resources["AccentColorHover"] = (Color)color;
            Application.Current.Resources["BrushAccentColorHover"] = new SolidColorBrush((Color)color);
            color.L += shade;
            Application.Current.Resources["AccentColorPressed"] = (Color)color;
            Application.Current.Resources["BrushAccentColorPressed"] = new SolidColorBrush((Color)color);
            color.L += shade;
            Application.Current.Resources["AccentColorBrighter"] = (Color)color;
            Application.Current.Resources["BrushAccentColorBrighter"] = new SolidColorBrush((Color)color);
        }

        [DllImport("user32.dll")]
        public static extern Int32 SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll")]
        public static extern Boolean ReleaseCapture();

        private void Launcher_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            Point position = Mouse.GetPosition(Launcher);
            if (position.Y > 50.0)
                return;

            ReleaseCapture();
            SendMessage(new WindowInteropHelper(this).Handle, 161, 2, 0);
        }

        private void ShowReleaseNotes(Object sender, MouseButtonEventArgs e)
        {
            var releaseWindow = new Window_ChangeLog();
            MainWindowGrid.Children.Add(releaseWindow);
        }

        private void OnHyperlinkClick(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private const String ASSEMBLY_CSHARP_PATH = "/x64/FF9_Data/Managed/Assembly-CSharp.dll";
    }
}
