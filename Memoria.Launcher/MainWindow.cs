using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Memoria.Launcher
{
    public partial class MainWindow : Window, IComponentConnector
    {
        //public ModManagerWindow ModdingWindow;
        public DateTime MemoriaAssemblyCompileDate;

        public MainWindow()
        {
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

            //SetAccentColor((Color)ColorConverter.ConvertFromString("#CC355566"));
            //SetAccentColor((Color)ColorConverter.ConvertFromString("#CCC9AD1D"));
            //Launcher.Source = new BitmapImage(new Uri($"{Directory.GetCurrentDirectory()}\\Echo-S-9\\LauncherBackground.jpg", UriKind.Absolute));
        }

        public const String DefaultAccentColor = "#CC355566";
        public const String DefaultBackgroundImage = "pack://application:,,,/images/new_launcher_bg.jpg";

        private void OnLoaded(Object sender, RoutedEventArgs e)
        {
            HotfixForMoguriMod();
            Title = Lang.Settings.LauncherWindowTitle + " | v" + MemoriaAssemblyCompileDate.ToString("yyyy.MM.dd");
            Nameandversion.Text = Lang.Settings.MemoriaEngine + " v" + MemoriaAssemblyCompileDate.ToString("yyyy.MM.dd");
            menuSettings.Header = Lang.Settings.menuSettings;
            menuCheats.Header = Lang.Settings.menuCheats;
            menuAdvanced.Header = Lang.Settings.menuAdvanced;
            ModelViewerButton.Content = Lang.Launcher.ModelViewer;
            UiGrid.MakeTooltip(ModelViewerButton, Lang.Launcher.ModelViewerButton_Tooltip, "", "hand");
            CopyLogButton.Content = Lang.Launcher.CopyLogButton;
            UiGrid.MakeTooltip(CopyLogButton, Lang.Launcher.CopyLogButton_Tooltip, "", "hand");

            SetupFrameLang();
            UpdateCatalog();
            LoadSettings2();
            CheckForValidModFolder();
            CheckOutdatedAndIncompatibleMods();
            lstCatalogMods.ItemsSource = modListCatalog;
            lstMods.ItemsSource = modListInstalled;
            lstDownloads.ItemsSource = downloadList;
            UpdateCatalogInstallationState();

            lstCatalogMods.SelectionChanged += OnModListSelect;
            lstMods.SelectionChanged += OnModListSelect;
            tabCtrlMain.SelectionChanged += OnModListSelect;
            PreviewSubModList.SelectionChanged += OnSubModSelect;
            PreviewSubModActive.Checked += OnSubModActivate;
            PreviewSubModActive.Unchecked += OnSubModActivate;
            if (modListInstalled.Count == 0)
                tabCtrlMain.SelectedIndex = 1;
            UpdateModDetails((Mod)null);
            CheckOutdatedAndIncompatibleMods();

            // add tooltip style to manager's buttons
            UiGrid.MakeTooltip(btnReorganize, btnReorganize.ToolTip.ToString(), "", "hand");
            UiGrid.MakeTooltip(btnMoveUp, btnMoveUp.ToolTip.ToString(), "", "hand");
            UiGrid.MakeTooltip(btnMoveDown, btnMoveDown.ToolTip.ToString(), "", "hand");
            UiGrid.MakeTooltip(btnUninstall, btnUninstall.ToolTip.ToString(), "", "hand");
            UiGrid.MakeTooltip(btnDownload, btnDownload.ToolTip.ToString(), "", "hand");
            UiGrid.MakeTooltip(btnCancel, btnCancel.ToolTip.ToString(), "", "hand");

            if (GameSettings.AutoRunGame)
                PlayButton.Click();
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
                        ShowTemporaryMessage(Lang.Launcher.CopyLogButton_Success, true);
                    }
                    else
                        ShowTemporaryMessage(Lang.Launcher.CopyLogButton_Empty, false);
                }
                else
                    ShowTemporaryMessage(Lang.Launcher.CopyLogButton_Doesntexist, false);
            }
            catch (Exception ex)
            {
                ShowTemporaryMessage(Lang.Launcher.CopyLogButton_Error + ex.Message, false);
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

        private void OnHyperlinkClick(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private const String ASSEMBLY_CSHARP_PATH = "/x64/FF9_Data/Managed/Assembly-CSharp.dll";
    }
}
