using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Memoria.Launcher.Utils;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Vanilla : UiGrid, INotifyPropertyChanged
    {
        public SettingsGrid_Vanilla()
        {
            DataContext = this;

            CreateHeading("Settings.Advanced");

            CreateCheckbox("IsDebugMode", "Settings.Debuggable", "Settings.Debuggable_Tooltip");
            CreateCheckbox("CheckUpdates", "Settings.CheckUpdates", "Settings.CheckUpdates_Tooltip");
            CreateCombobox("UpdateChannel", ComboBoxOptions.Literal(["Stable", "Canary"]), 50, "Settings.UpdateChannel", "Settings.UpdateChannel_Tooltip", "", ComboBoxSelectionMode.Value);

            String OSversion = $"{Environment.OSVersion}";
            if (OSversion.Contains("Windows") && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WINELOADER")))
                CreateCheckbox("SteamOverlayFix", "SteamOverlay.OptionLabel", "Settings.SteamOverlayFix_Tooltip");


            CreateCombobox("LauncherLanguage", ComboBoxOptions.Literal(Lang.LauncherLanguageNames), 50, "Settings.LauncherLanguage", "Settings.LauncherLanguage_Tooltip", "");

            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
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

        public String UpdateChannel
        {
            get { return _updateChannel; }
            set
            {
                if (!String.Equals(_updateChannel, value, StringComparison.Ordinal))
                {
                    _updateChannel = value;
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
                    case nameof(UpdateChannel):
                        iniFile.SetSetting("Memoria", propertyName, UpdateChannel);
                        break;
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

        private Boolean _isDebugMode;
        private Boolean _checkUpdates = true;
        private String _updateChannel = "Stable";

        public void LoadSettings()
        {
            try
            {
                IniFile iniFile = IniFile.SettingsIni;

                if (!Environment.Is64BitOperatingSystem)
                    throw new NotSupportedException("The Memoria mod engine no longer supports x86 platforms. Use x64 OS.");
                
                if (!Directory.Exists("x64"))
                {
                    if (Directory.Exists("x86"))
                        throw new NotSupportedException("The Memoria mod engine no longer supports x86 platforms. Recover the game to run x64 version.");
                    
                    throw new NotSupportedException("The launcher must be ran from the game directory containing the x64 folder.");
                }

                String value = iniFile.GetSetting("Memoria", nameof(IsDebugMode), "false");
                if (!Boolean.TryParse(value, out _isDebugMode))
                    _isDebugMode = false;

                value = iniFile.GetSetting("Memoria", nameof(CheckUpdates), "true");
                if (!Boolean.TryParse(value, out _checkUpdates))
                    _checkUpdates = true;

                value = iniFile.GetSetting("Memoria", nameof(AutoRunGame), "false");
                AutoRunGame = App.AutoRunGame || (Boolean.TryParse(value, out var autoRunGame) && autoRunGame);

                _updateChannel = iniFile.GetSetting("Memoria", nameof(UpdateChannel), "Stable");

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
                OnPropertyChanged(nameof(IsDebugMode));
                OnPropertyChanged(nameof(CheckUpdates));
                OnPropertyChanged(nameof(UpdateChannel));
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
    }
}
