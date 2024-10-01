using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Vanilla : UiGrid, INotifyPropertyChanged
    {
        public SettingsGrid_Vanilla()
        {
            SetCols(8);
            Width = 260;
            Margin = new Thickness(0);
            DataContext = this;

            CreateHeading(Lang.Settings.Advanced);

            CreateCheckbox("IsX64", "x64", Lang.Settings.Xsixfour_Tooltip, 0, "IsX64Enabled");
            CreateCheckbox("IsDebugMode", Lang.Settings.Debuggable, Lang.Settings.Debuggable_Tooltip);
            CreateCheckbox("CheckUpdates", Lang.Settings.CheckUpdates, Lang.Settings.CheckUpdates_Tooltip);

            String OSversion = $"{Environment.OSVersion}";
            if (OSversion.Contains("Windows") && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WINELOADER")))
                CreateCheckbox("SteamOverlayFix", Lang.SteamOverlay.OptionLabel, Lang.Settings.SteamOverlayFix_Tooltip);

            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        public Boolean IsX64
        {
            get { return _isX64; }
            set
            {
                if (_isX64 != value)
                {
                    _isX64 = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsX64Enabled
        {
            get { return _isX64Enabled; }
            set
            {
                if (_isX64Enabled != value)
                {
                    _isX64Enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsDebugMode
        {
            get { return _isDebugMode; }
            set
            {
                if (_isDebugMode != value)
                {
                    _isDebugMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public String[] DownloadMirrors
        {
            get { return _downloadMirrors; }
            set
            {
                if (_downloadMirrors != value)
                {
                    _downloadMirrors = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean CheckUpdates
        {
            get => _checkUpdates;
            set
            {
                if (_checkUpdates != value)
                {
                    _checkUpdates = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean SteamOverlayFix
        {
            get => IsSteamOverlayFixed();
            set
            {
                MessageBoxResult ShowMessage(String message, MessageBoxButton button, MessageBoxImage image)
                {
                    return MessageBox.Show((System.Windows.Window)this.GetRootElement(), message, Lang.SteamOverlay.Caption, button, image);
                }

                if (IsSteamOverlayFixed() == value)
                    return;

                if (value)
                {
                    if (ShowMessage(Lang.SteamOverlay.FixAreYouSure, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => OnPropertyChanged()), DispatcherPriority.ContextIdle, null);
                        return;
                    }

                    String currentLauncherPath = Process.GetCurrentProcess().MainModule.FileName;

                    Process process = Process.Start(new ProcessStartInfo("Memoria.SteamFix.exe", @$" ""{currentLauncherPath}"" ") { Verb = "runas" });
                    process.WaitForExit();
                }
                else
                {
                    if (ShowMessage(Lang.SteamOverlay.RollbackAreYouSure, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => OnPropertyChanged()), DispatcherPriority.ContextIdle, null);
                        return;
                    }

                    Process process = Process.Start(new ProcessStartInfo("Memoria.SteamFix.exe") { Verb = "runas" });
                    process.WaitForExit();
                }

                Application.Current.Dispatcher.BeginInvoke(new Action(() => OnPropertyChanged()), DispatcherPriority.ContextIdle, null);
            }
        }

        public Boolean AutoRunGame { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                IniFile iniFile = new IniFile(IniPath);
                switch (propertyName)
                {
                    case nameof(IsDebugMode):
                        iniFile.WriteValue("Memoria", propertyName, IsDebugMode.ToString());
                        break;
                    case nameof(IsX64):
                        iniFile.WriteValue("Memoria", propertyName, IsX64.ToString());
                        break;
                    case nameof(CheckUpdates):
                    {
                        iniFile.WriteValue("Memoria", propertyName, CheckUpdates.ToString());
                        if (CheckUpdates)
                        {
                            using (ManualResetEvent evt = new ManualResetEvent(false))
                            {
                                System.Windows.Window root = this.GetRootElement() as System.Windows.Window;
                                if (root != null)
                                    await UiLauncherPlayButton.CheckUpdates(root, evt, this);
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        public static readonly String IniPath = AppDomain.CurrentDomain.BaseDirectory + "\\Settings.ini";

        private Boolean _isX64 = true;
        private Boolean _isX64Enabled = true;
        private Boolean _isDebugMode;
        private Boolean _checkUpdates = true;
        private String[] _downloadMirrors;

        public void LoadSettings()
        {
            try
            {
                IniFile iniFile = new IniFile(IniPath);

                String value = iniFile.ReadValue("Memoria", nameof(IsX64));
                if (String.IsNullOrEmpty(value))
                    value = "true";
                if (!Boolean.TryParse(value, out _isX64))
                    _isX64 = true;
                if (!Environment.Is64BitOperatingSystem || !Directory.Exists("x64"))
                {
                    _isX64 = false;
                    _isX64Enabled = false;
                }
                else if (!Directory.Exists("x86"))
                {
                    _isX64 = true;
                    _isX64Enabled = false;
                }

                value = iniFile.ReadValue("Memoria", nameof(IsDebugMode));
                if (String.IsNullOrEmpty(value))
                    value = "false";
                if (!Boolean.TryParse(value, out _isDebugMode))
                    _isDebugMode = false;

                value = iniFile.ReadValue("Memoria", nameof(CheckUpdates));
                if (String.IsNullOrEmpty(value))
                    value = "true";
                if (!Boolean.TryParse(value, out _checkUpdates))
                    _checkUpdates = true;

                value = iniFile.ReadValue("Memoria", nameof(AutoRunGame));
                if (String.IsNullOrEmpty(value))
                    value = "false";
                AutoRunGame = App.AutoRunGame || (Boolean.TryParse(value, out var autoRunGame) && autoRunGame);

                value = iniFile.ReadValue("Memoria", nameof(DownloadMirrors));
                if (String.IsNullOrEmpty(value))
                {
                    _downloadMirrors = ["https://github.com/Albeoris/Memoria/releases/latest/download/Memoria.Patcher.exe"];
                    //if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru") _downloadMirrors = ["https://ff9.ffrtt.ru/rus/FF9RU.exe", "https://ff9.ffrtt.ru/rus/Memoria.Patcher.exe"];
                }
                else
                {
                    _downloadMirrors = value.Split(',');
                }

                OnPropertyChanged(nameof(IsX64));
                OnPropertyChanged(nameof(IsX64Enabled));
                OnPropertyChanged(nameof(IsDebugMode));
                OnPropertyChanged(nameof(CheckUpdates));
                OnPropertyChanged(nameof(DownloadMirrors));
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }
        private Boolean IsSteamOverlayFixed()
        {
            try
            {
                using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default))
                {
                    using (var subKey = registryKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\FF9_Launcher.exe"))
                    {
                        if (subKey?.GetValue("Debugger") == null)
                            return false;
                    }
                }

                var bak = new FileInfo("FF9_Launcher.bak");
                var exe = new FileInfo("FF9_Launcher.exe");

                // Patch again if FF9_Launcher.exe was rewrited
                if (bak.Exists && exe.Exists && bak.Length != exe.Length)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}