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
        private string[] startupArgs;
        public MainWindow(string[] args)
        {
            startupArgs = args;
            InitializeComponent();
            if(RegistryValues.Instance.GetGameInstallPath != null)
            {
                GamePathText.Text = RegistryValues.Instance.GetGameInstallPath;
            }
        }

        private void checkExit()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit the installer?", "Exit Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
    }
}
