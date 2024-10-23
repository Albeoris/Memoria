using Installer.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool IsInstall{
            get; private set;
        } = false;

        public static bool IsRepair
        {
            get; private set;
        } = false;

        public static bool IsModify
        {
            get; private set;
        } = false;
        public static bool IsUninstall
        {
            get; private set;
        } = false;
        public static bool IsFinished
        {
            get; private set;
        } = false;

        private string[] startupArgs;
        public MainWindow(string[] args)
        {
            startupArgs = args;
            InitializeComponent();

            if(RegistryValues.Instance.GetGameInstallPath != null)
            {
                GamePathText.Text = RegistryValues.Instance.GetGameInstallPath;
            }

            if(args.Length >= 1) {
                switch (args[0])
                {
                    case "repair":
                    {
                        IsRepair = true;
                        InstallBtn.Content = "Repair";
                        InstallBtn.IsEnabled = true;
                        TermsChkBx.Visibility = Visibility.Hidden;
                        GamePathBrowseBtn.IsEnabled = false;
                        break;
                    }
                    case "uninstall":
                    { 
                        IsUninstall = true;
                        InstallBtn.Content = "Remove";
                        InstallBtn.IsEnabled = true;
                        TermsChkBx.Visibility = Visibility.Hidden;
                        GamePathBrowseBtn.IsEnabled = false;
                        break; 
                    }
                    default:
                    case "install":
                    {
                        IsInstall = true;
                        InstallBtn.Content = "Install";
                        break;
                    }
                }
            }
            else
            {
                IsInstall = true;
            }
        }

        private void Finished()
        {
            IsFinished = true;
            ProgressBar.Value = 100;
            InstallBtn.Content = "Exit Setup.";
        }

        private async void Install()
        {
            ProgressGrid.Visibility = Visibility.Visible;
            CancelBtn.Visibility = Visibility.Hidden;
            InstallBtn.Content = "Please Wait.";
            GamePathBrowseBtn.IsEnabled = false;
            GamePathText.IsEnabled = false;

            await Classes.Installer.DownloadOrUsePatcher(GamePathText.Text, ProgressBar, ProgressText);
            Classes.Installer.RunPatcher(GamePathText.Text, ProgressBar, ProgressText);
            await Classes.Installer.CopySetup(GamePathText.Text, ProgressText);
            RegistryValues.Instance.AddToUninstallList(GamePathText.Text);

            ProgressText.Text = "Memoria Installed Successfully.";
            Finished();
        }

        private async void Repare()
        {
            ProgressGrid.Visibility = Visibility.Visible;
            CancelBtn.Visibility = Visibility.Hidden;
            InstallBtn.Content = "Please Wait.";
            GamePathBrowseBtn.IsEnabled = false;
            GamePathText.IsEnabled = false;

            await Classes.Installer.DownloadOrUsePatcher(GamePathText.Text, ProgressBar, ProgressText);
            Classes.Installer.RunPatcher(GamePathText.Text, ProgressBar, ProgressText);
            await Classes.Installer.CopySetup(GamePathText.Text, ProgressText);
            RegistryValues.Instance.AddToUninstallList(GamePathText.Text);

            ProgressText.Text = "Memoria Reinstalled.";
            Finished();
        }

        private void Uninstall()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to Unistall Memoria?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ProgressGrid.Visibility = Visibility.Visible;
                CancelBtn.Visibility = Visibility.Hidden;
                InstallBtn.Content = "Please Wait.";

                ProgressText.Text = "Please Wait steam is cleaning your game.";
                Classes.Uninstaller.CleanGameFiles(GamePathText.Text);
                RegistryValues.Instance.RemoveFromUninstallList();
                ProgressText.Text = "Memoria Removed Successfully.";
                MessageBox.Show("Memoria has been removed please use steam to validate your game files to restore you game to default.", "Game Validation Needed", MessageBoxButton.OK, MessageBoxImage.Warning);

                Finished();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void checkExit()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit the Memoria installer?", "Exit Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void AllowedToInstall()
        {
            bool lisence = TermsChkBx.IsChecked ?? false;
            bool fileExists = System.IO.File.Exists(GamePathText.Text + @"\FF9_Launcher.exe");
            if (lisence && fileExists)
            {
                InstallBtn.IsEnabled = true;
            }
            else
            {
                InstallBtn.IsEnabled = false;
            }
        }

        private void browseForFile()
        {
            if (GamePathBrowseBtn.IsEnabled)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = RegistryValues.Instance.GetGameInstallPath,
                    Filter = "Final Fantasy IX Launcher|FF9_Launcher.exe",
                    Title = "Select FF9_Launcher.exe"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    GamePathText.Text = (new FileInfo(openFileDialog.FileName)).Directory.FullName;
                }
            }
        }

        private void TitleGrid_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                try
                {
                    this.DragMove();
                }
                catch (InvalidOperationException){ }
            }
        }

        private void CloseBtnTxt_MouseEnter(Object sender, MouseEventArgs e)
        {
            CloseBtnTxt.Foreground = Brushes.Red;
        }

        private void CloseBtnTxt_MouseLeave(Object sender, MouseEventArgs e)
        {
            CloseBtnTxt.Foreground = Brushes.White;
        }

        private void CloseBtnTxt_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                checkExit();
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void TermsChkBx_Click(Object sender, RoutedEventArgs e)
        {
            AllowedToInstall();
        }

        private void GamePathText_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            browseForFile();
        }

        private void GamePathBrowseBtn_Click(Object sender, RoutedEventArgs e)
        {
            browseForFile();
        }

        private void GamePathText_TextChanged(Object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }

        private void CancelBtn_Click(Object sender, RoutedEventArgs e)
        {
            checkExit();
        }

        private void InstallBtn_Click(Object sender, RoutedEventArgs e)
        {
            if (IsFinished)
            {
                Environment.Exit(0);
            }

            if (IsRepair)
            {
                Repare();
            }

            if(IsInstall)
            {
                Install();
            }

            if (IsUninstall) { 
                Uninstall(); 
            }
        }
    }
}
