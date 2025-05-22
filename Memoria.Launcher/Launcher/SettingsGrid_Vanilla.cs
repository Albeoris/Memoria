using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Vanilla : UiGrid, INotifyPropertyChanged
    {
        public SettingsGrid_Vanilla()
        {
            DataContext = this;

            CreateHeading("Settings.Advanced");

            CreateCheckbox("IsX64", "x64", "Settings.Xsixfour_Tooltip", 0, "IsX64Enabled");
            CreateCheckbox("IsDebugMode", "Settings.Debuggable", "Settings.Debuggable_Tooltip");
            CreateCheckbox("CheckUpdates", "Settings.CheckUpdates", "Settings.CheckUpdates_Tooltip");

            String OSversion = $"{Environment.OSVersion}";
            if (OSversion.Contains("Windows") && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WINELOADER")))
                CreateCheckbox("SteamOverlayFix", "SteamOverlay.OptionLabel", "Settings.SteamOverlayFix_Tooltip");


            CreateCombobox("LauncherLanguage", Lang.LauncherLanguageNames, 50, "Settings.LauncherLanguage", "Settings.LauncherLanguage_Tooltip", "");

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
                    return MessageBox.Show((System.Windows.Window)this.GetRootElement(), message, (String)Lang.Res["teamOverlay.Caption"], button, image);
                }

                if (IsSteamOverlayFixed() == value)
                    return;

                if (value)
                {
                    if (ShowMessage((String)Lang.Res["SteamOverlay.FixAreYouSure"], MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes)
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
                    if (ShowMessage((String)Lang.Res["SteamOverlay.RollbackAreYouSure"], MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
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

        private Int32 _launcherlanguage;
        public Int32 LauncherLanguage
        {
            get => _launcherlanguage;
            set
            {
                if (_launcherlanguage != value)
                {
                    if (value >= 0 && !IniFile.PreventWrite)
                    {
                        _launcherlanguage = value;
                        OnPropertyChanged();
                        try
                        {
                            IniFile.PreventWrite = true;
                            Lang.LoadLanguageResources(Lang.LauncherLanguageList[value]);
                            RefereshComboBoxes();
                            Lang.Res["Settings.LauncherWindowTitle"] += " | v" + MainWindow.MemoriaAssemblyCompileDate.ToString("yyyy.MM.dd");
                            ((MainWindow)Application.Current.MainWindow).SettingsGrid_Presets.RefreshPresets();
                        }
                        catch (Exception ex)
                        {
                            UiHelper.ShowError(Application.Current.MainWindow, ex);
                        }
                        finally
                        {
                            IniFile.PreventWrite = false;
                        }
                    }
                }
            }
        }

        public Boolean AutoRunGame { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                IniFile iniFile = IniFile.SettingsIni;
                switch (propertyName)
                {
                    case nameof(IsDebugMode):
                        iniFile.SetSetting("Memoria", propertyName, IsDebugMode.ToString());
                        break;
                    case nameof(IsX64):
                        iniFile.SetSetting("Memoria", propertyName, IsX64.ToString());
                        break;
                    case nameof(CheckUpdates):
                    {
                        iniFile.SetSetting("Memoria", propertyName, CheckUpdates.ToString());
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
                    case nameof(LauncherLanguage):
                        iniFile.SetSetting("Memoria", propertyName, Lang.LauncherLanguageList[LauncherLanguage]);
                        break;
                }
                iniFile.Save();
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        private Boolean _isX64 = true;
        private Boolean _isX64Enabled = true;
        private Boolean _isDebugMode;
        private Boolean _checkUpdates = true;
        private String[] _downloadMirrors;

        public void LoadSettings()
        {
            try
            {
                IniFile iniFile = IniFile.SettingsIni;

                String value = iniFile.GetSetting("Memoria", nameof(IsX64), "true");
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

                value = iniFile.GetSetting("Memoria", nameof(IsDebugMode), "false");
                if (!Boolean.TryParse(value, out _isDebugMode))
                    _isDebugMode = false;

                value = iniFile.GetSetting("Memoria", nameof(CheckUpdates), "true");
                if (!Boolean.TryParse(value, out _checkUpdates))
                    _checkUpdates = true;

                value = iniFile.GetSetting("Memoria", nameof(AutoRunGame), "false");
                AutoRunGame = App.AutoRunGame || (Boolean.TryParse(value, out var autoRunGame) && autoRunGame);

                value = iniFile.GetSetting("Memoria", nameof(DownloadMirrors));
                if (String.IsNullOrEmpty(value))
                {
                    _downloadMirrors = ["https://github.com/Albeoris/Memoria/releases/latest/download/Memoria.Patcher.exe"];
                    //if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru") _downloadMirrors = ["https://ff9.ffrtt.ru/rus/FF9RU.exe", "https://ff9.ffrtt.ru/rus/Memoria.Patcher.exe"];
                }
                else
                {
                    _downloadMirrors = value.Split(',');
                }

                value = iniFile.GetSetting("Memoria", "LauncherLanguage", Lang.LangName);
                _launcherlanguage = 0;
                for (Int32 i = 0; i < Lang.LauncherLanguageList.Length; i++)
                {
                    if (Lang.LauncherLanguageList[i] == value)
                    {
                        _launcherlanguage = i;
                        break;
                    }
                }

                IniFile.PreventWrite = true;
                OnPropertyChanged(nameof(IsX64));
                OnPropertyChanged(nameof(IsX64Enabled));
                OnPropertyChanged(nameof(IsDebugMode));
                OnPropertyChanged(nameof(CheckUpdates));
                OnPropertyChanged(nameof(DownloadMirrors));
                OnPropertyChanged(nameof(LauncherLanguage));
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
            finally
            {
                IniFile.PreventWrite = false;
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

        private void ReloadApplication()
        {
            string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            System.Diagnostics.Process.Start(exePath);
            Application.Current.Shutdown();
        }
    }
}
