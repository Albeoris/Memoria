using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using MethodInvoker = System.Windows.Forms.MethodInvoker;

namespace Memoria.Launcher
{
    public partial class MainWindow : Window, IComponentConnector
    {
        public static readonly String[] OutdatedModsVersions = // "ModName_Version"
        {
            "Alternate Fantasy_6.0",
            "Alternate Fantasy_6.1",
            "Alternate Fantasy_6.2",
            "Alternate Fantasy_6.3",
            "Alternate Fantasy_6.4",
            "Alternate Fantasy_6.5",
            "Trance Seek_0.3.20",
            "Trance Seek_0.3.21",
            "Trance Seek_0.3.22",
            "Trance Seek_0.3.23",
            "Playable Character Pack_1.0",
            "Playable Character Pack_1.1",
        };

        private readonly string UpdateEmoji = "⏫";
        private readonly string ConflictEmoji = "✖️";
        private readonly string WaitingEmoji = "⌛";
        private readonly string InstalledEmoji = "✔️";
        private readonly string ActiveEmoji = "☑️";

        public ObservableCollection<Mod> modListInstalled = new ObservableCollection<Mod>();
        public ObservableCollection<Mod> modListCatalog = new ObservableCollection<Mod>();
        public ObservableCollection<Mod> downloadList = new ObservableCollection<Mod>();
        public String StatusMessage = "";

        public String[] supportedArchives = { "rar", "unrar", "zip", "bzip2", "gzip", "tar", "7z", "lzip", "gz" };

        private CancellationTokenSource ExtractionCancellationToken = new CancellationTokenSource();


        private void ModManagerWindow_KeyUp(Object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete) Uninstall();
        }

        private Boolean previousTabWasMod = false;
        private void Tabs_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            if (!previousTabWasMod && ModManagerTab.IsSelected)
            {
                AnimateHeight(ContentTabControl, 520, 590, TimeSpan.FromMilliseconds(500));
                AnimateMargin(LogoImage, new Thickness(20, -53, 0, 0), new Thickness(20, -20, 0, 0), TimeSpan.FromMilliseconds(500));
                AnimateHeight(LogoImage, 250, 125, TimeSpan.FromMilliseconds(500));
                previousTabWasMod = true;
            }
            else if (previousTabWasMod && !ModManagerTab.IsSelected)
            {
                AnimateHeight(ContentTabControl, 590, 520, TimeSpan.FromMilliseconds(500));
                AnimateMargin(LogoImage, new Thickness(20, -20, 0, 0), new Thickness(20, -53, 0, 0), TimeSpan.FromMilliseconds(500));
                AnimateHeight(LogoImage, 125, 250, TimeSpan.FromMilliseconds(500));
                previousTabWasMod = false;
            }
        }
        private void AnimateHeight(FrameworkElement element, double from, double to, TimeSpan duration)
        {
            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(duration),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            element.BeginAnimation(FrameworkElement.HeightProperty, heightAnimation);
        }
        private void AnimateMargin(FrameworkElement element, Thickness from, Thickness to, TimeSpan duration)
        {
            ThicknessAnimation marginAnimation = new ThicknessAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(duration),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            element.BeginAnimation(FrameworkElement.MarginProperty, marginAnimation);
        }

        private void CheckOutdatedAndIncompatibleMods()
        {
            AreThereModUpdates = false;
            AreThereModIncompatibilies = false;

            foreach (Mod mod in modListInstalled) // reset state
            {
                mod.UpdateIcon = null;
                mod.UpdateTooltip = null;
                mod.IncompIcon = null;
                mod.ActiveIncompatibleMods = null;
            }

            foreach (Mod mod in modListInstalled)
            {
                if (mod == null || mod.Name == null)
                    continue;
                if (((mod.Name == "Moguri Mod" || mod.Name == "MoguriFiles") && mod.InstallationPath.Contains("MoguriFiles")) || (mod.Name == "Moguri - 3D textures" && mod.InstallationPath.Contains("Moguri_3Dtextures")))
                {
                    mod.UpdateIcon = UpdateEmoji;
                    mod.CurrentVersion = Version.Parse("8.3");
                    AreThereModUpdates = true;
                    mod.Description = "Please download the latest Moguri Mod from the catalog and disable/remove this one";
                    mod.UpdateTooltip = "Please download the latest Moguri Mod from the catalog and disable/remove this one";
                }

                foreach (Mod catalog_mod in modListCatalog) // check updates
                {
                    if (catalog_mod != null && catalog_mod.Name != null && mod.Name == catalog_mod.Name)
                    {
                        Boolean versionCorresponds = mod.CurrentVersion == catalog_mod.CurrentVersion;
                        mod.IsOutdated = catalog_mod.IsOutdated = !versionCorresponds;
                        if (mod.IsOutdated)
                        {
                            mod.UpdateIcon = UpdateEmoji;
                            mod.UpdateTooltip = Lang.ModEditor.UpdateTooltip + catalog_mod.CurrentVersion;
                            AreThereModUpdates = true;
                        }
                        else
                        {
                            mod.UpdateIcon = null;
                            if (catalog_mod.IncompatibleWith != null)
                                mod.IncompatibleWith = catalog_mod.IncompatibleWith;
                        }
                    }
                }


                if (mod.IncompatibleWith != null && mod.IsActive) // Inter-mod compatibility
                {
                    IEnumerable<String> Incomps = mod.IncompatibleWith.Split(',');
                    foreach (String Incomp in Incomps)
                    {
                        String incompName = Incomp.Trim();
                        foreach (Mod other_mod in modListInstalled)
                        {
                            if (other_mod == null || other_mod.Name == null)
                                continue;
                            if (other_mod.Name == incompName && other_mod.IsActive)
                            {
                                mod.IncompIcon = ConflictEmoji;
                                other_mod.IncompIcon = ConflictEmoji;

                                if (mod.ActiveIncompatibleMods == null)
                                    mod.ActiveIncompatibleMods = Lang.ModEditor.ActiveIncompatibleMods + other_mod.Name;
                                else
                                    mod.ActiveIncompatibleMods += ", " + other_mod.Name;

                                if (other_mod.ActiveIncompatibleMods == null)
                                    other_mod.ActiveIncompatibleMods = Lang.ModEditor.ActiveIncompatibleMods + mod.Name;
                                else
                                    other_mod.ActiveIncompatibleMods += ", " + mod.Name;

                                AreThereModIncompatibilies = true;
                            }
                        }
                    }
                }

                foreach (Mod subMod in mod.SubMod) // submod auto (de)activate based on others
                {
                    if (subMod == null || subMod.Name == null)
                        continue;

                    Boolean modFoundActive = false;

                    foreach (Mod other_mod in modListInstalled)
                    {
                        if (other_mod == null || other_mod.Name == null)
                            continue;

                        if (subMod.ActivateWithMod != null && subMod.ActivateWithMod == other_mod.Name && other_mod.IsActive)
                            subMod.IsActive = true;
                        if (subMod.DeactivateWithMod != null && subMod.DeactivateWithMod == other_mod.Name && other_mod.IsActive)
                            subMod.IsActive = false;

                        if (subMod.ActivateWithoutMod != null && subMod.ActivateWithoutMod == other_mod.Name && other_mod.IsActive)
                            modFoundActive = true;
                        if (subMod.DeactivateWithoutMod != null && subMod.DeactivateWithoutMod == other_mod.Name && other_mod.IsActive)
                            modFoundActive = true;
                    }

                    // apply when mod is not found active, even if not installed, and do nothing otherwise
                    if (!modFoundActive && subMod.ActivateWithoutMod != null)
                        subMod.IsActive = true;
                    if (!modFoundActive && subMod.DeactivateWithoutMod != null)
                        subMod.IsActive = false;

                    UpdateModDetails((Mod)lstMods.SelectedItem);
                }

                String ver = mod.CurrentVersion?.ToString() ?? "0";
                foreach (String outdated in OutdatedModsVersions) // Memoria compatibility
                {
                    if (outdated == mod.Name + "_" + ver)
                    {
                        mod.IncompIcon = ConflictEmoji;
                        if (mod.ActiveIncompatibleMods == null)
                            mod.ActiveIncompatibleMods = Lang.ModEditor.IncompatibleWithMemoria;
                        else
                            mod.ActiveIncompatibleMods += "\n\n" + Lang.ModEditor.IncompatibleWithMemoria;
                        AreThereModIncompatibilies = true;
                    }
                }
            }

            colMyModsIcons.Width = colMyModsIcons.ActualWidth;
            colMyModsIcons.Width = double.NaN;
            lstMods.Items.Refresh();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstMods.Items.Refresh();
            lstCatalogMods.Items.Refresh();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Mod mod = (sender as System.Windows.Controls.CheckBox)?.DataContext as Mod;
            if (mod != null && mod.IsActive)
                mod.TryApplyPreset();
            CheckOutdatedAndIncompatibleMods();
        }

        private void OnClosing(Object sender, CancelEventArgs e)
        {
            if (downloadList.Count > 0 || downloadingMod != null)
            {
                e.Cancel = true;
                MessageBox.Show($"If you close this window while downloads are on their way, they will be cancelled.", "Warning", MessageBoxButton.OK);
                return;
            }
            if (downloadCatalogClient != null && downloadCatalogClient.IsBusy)
                downloadCatalogClient.CancelAsync();
            UpdateSettings();
            //((MainWindow)this.Owner).ModdingWindow = null;
            //((MainWindow)this.Owner).LoadSettings();
            //((MainWindow)this.Owner).ComeBackToLauncherFromModManager(AreThereModUpdates, AreThereModIncompatibilies);
        }


        private void Uninstall()
        {
            List<Mod> selectedMods = new List<Mod>();
            foreach (Mod mod in lstMods.SelectedItems)
                selectedMods.Add(mod);
            foreach (Mod mod in selectedMods)
            {
                if (Directory.Exists(mod.InstallationPath))
                    if (MessageBox.Show($"The mod folder {mod.InstallationPath} will be deleted.\nProceed?", "Updating", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Directory.Delete(mod.InstallationPath, true);
                        modListInstalled.Remove(mod);
                        UpdateInstalledPriorityValue();
                    }
            }
            UpdateCatalogInstallationState();
        }

        private void OnModListSelect(Object sender, RoutedEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv == lstMods || lv == lstCatalogMods)
                UpdateModDetails((Mod)lv.SelectedItem);
            else if (sender == tabCtrlMain && tabCtrlMain.SelectedIndex == 0)
                UpdateModDetails((Mod)lstMods.SelectedItem);
            else if (sender == tabCtrlMain && tabCtrlMain.SelectedIndex == 1)
                UpdateModDetails((Mod)lstCatalogMods.SelectedItem);

            Boolean canDownload = false;
            foreach (Mod mod in lstCatalogMods.SelectedItems)
                if (!String.IsNullOrEmpty(mod.DownloadUrl))
                    canDownload = true;
            if (canDownload)
            {
                btnDownload.IsEnabled = true;
            }
            else
            {
                btnDownload.IsEnabled = false;
            }
        }
        private void OnModListDoubleClick(Object sender, RoutedEventArgs e)
        {
        }

        private void OnCatalogListDoubleClick(Object sender, RoutedEventArgs e)
        {
            OnClickDownload(sender, e);
        }

        private void OnSubModSelect(Object sender, RoutedEventArgs e)
        {
            UpdateSubModDetails((Mod)PreviewSubModList.SelectedItem);
        }

        private void OnSubModActivate(Object sender, RoutedEventArgs e)
        {
            Mod subMod = (Mod)PreviewSubModList.SelectedItem;
            if (subMod != null)
                subMod.IsActive = PreviewSubModActive.IsChecked ?? false;
        }

        private void OnClickUninstall(Object sender, RoutedEventArgs e)
        {
            Uninstall();
        }

        private void OnClickReorganize(Object sender, RoutedEventArgs e)
        {
            OrderInstalledByModsPriority();
        }

        public void OrderInstalledByModsPriority()
        {
            foreach (Mod mod in modListInstalled)
            {
                if (mod == null || mod.Name == null)
                    continue;
                foreach (Mod catalog_mod in modListCatalog)
                {
                    if (catalog_mod == null || catalog_mod.Name == null)
                        continue;
                    if (mod.Name == catalog_mod.Name)
                    {
                        mod.PriorityWeight = catalog_mod.PriorityWeight;
                    }
                }
            }
            List<Mod> orderedMods = modListInstalled.OrderByDescending(mod => mod.PriorityWeight).ToList();
            modListInstalled.Clear();
            foreach (Mod mod in orderedMods)
                modListInstalled.Add(mod);

            UpdateInstalledPriorityValue();
        }

        private void OnClickMoveUp(Object sender, RoutedEventArgs e)
        {
            if (lstMods.SelectedIndex > 0)
            {
                Int32 sel = lstMods.SelectedIndex;
                Mod i1 = modListInstalled[sel];
                Mod i2 = modListInstalled[sel - 1];
                modListInstalled.Remove(i1);
                modListInstalled.Remove(i2);
                modListInstalled.Insert(sel - 1, i1);
                modListInstalled.Insert(sel, i2);
                lstMods.SelectedItem = i1;
                UpdateInstalledPriorityValue();
            }
        }
        private void OnClickSendTop(Object sender, RoutedEventArgs e)
        {
            if (lstMods.SelectedIndex > 0)
            {
                Mod i1 = modListInstalled[lstMods.SelectedIndex];
                modListInstalled.Remove(i1);
                modListInstalled.Insert(0, i1);
                lstMods.SelectedItem = i1;
                UpdateInstalledPriorityValue();
            }
        }
        private void OnClickMoveDown(Object sender, RoutedEventArgs e)
        {
            if (lstMods.SelectedIndex >= 0 && lstMods.SelectedIndex + 1 < lstMods.Items.Count)
            {
                Int32 sel = lstMods.SelectedIndex;
                Mod i1 = modListInstalled[sel];
                Mod i2 = modListInstalled[sel + 1];
                modListInstalled.Remove(i1);
                modListInstalled.Remove(i2);
                modListInstalled.Insert(sel, i2);
                modListInstalled.Insert(sel + 1, i1);
                lstMods.SelectedItem = i1;
                UpdateInstalledPriorityValue();
            }
        }
        private void OnClickSendBottom(Object sender, RoutedEventArgs e)
        {
            if (lstMods.SelectedIndex >= 0 && lstMods.SelectedIndex + 1 < lstMods.Items.Count)
            {
                Mod i1 = modListInstalled[lstMods.SelectedIndex];
                modListInstalled.Remove(i1);
                modListInstalled.Insert(modListInstalled.Count, i1);
                lstMods.SelectedItem = i1;
                UpdateInstalledPriorityValue();
            }
        }
        private void OnClickDownload(Object sender, RoutedEventArgs e)
        {
            foreach (Mod mod in lstCatalogMods.SelectedItems)
            {
                if (downloadList.Contains(mod) || String.IsNullOrEmpty(mod.DownloadUrl))
                    return;
                if (!String.IsNullOrEmpty(mod.MinimumMemoriaVersion))
                {
                    /*
                    String memoriaVersion = ((MainWindow)this.Owner).MemoriaAssemblyCompileDate.ToString("yyyy-MM-dd");
                    DateTime tempDate;
                    Boolean isDate1Valid = DateTime.TryParseExact(memoriaVersion, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate);
                    Boolean isDate2Valid = DateTime.TryParseExact(mod.MinimumMemoriaVersion, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate);
                    if (isDate1Valid && isDate2Valid)
                    {
                        if (!IsDate1EqualOrMoreRecent(memoriaVersion, mod.MinimumMemoriaVersion))
                        {
                            MessageBox.Show($"The mod \"{mod.Name}\" requires Memoria v{mod.MinimumMemoriaVersion} or above to work correctly.\n\nPlease update Memoria to the latest version.", "Warning", MessageBoxButtons.OK);
                        }
                    }
                    */
                }
                downloadList.Add(mod);
                DownloadStart(mod);
                mod.Installed = WaitingEmoji;
            }
            lstCatalogMods.Items.Refresh();
            CheckOutdatedAndIncompatibleMods();
        }

        public static bool IsDate1EqualOrMoreRecent(string date1, string date2)
        {
            string format = "yyyy-MM-dd";
            DateTime dateTime1 = DateTime.ParseExact(date1, format, CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(date2, format, CultureInfo.InvariantCulture);
            return dateTime1 >= dateTime2;
        }

        private void OnClickClose(Object sender, RoutedEventArgs e)
        {
            //Close();
        }

        private void OnClickCancel(Object sender, RoutedEventArgs e)
        {
            if (downloadThread != null)
                downloadThread.Abort();
            if (downloadClient != null)
                downloadClient.CancelAsync();
            if (downloadingMod != null)
                downloadList.Remove(downloadingMod);
            downloadingMod = null;
            if (downloadList.Count > 0)
                DownloadStart(downloadList[0]);
            else
            {
                lstDownloads.MinHeight = 0;
                lstDownloads.Height = 0;
                btnCancelStackpanel.Height = 0;
            }
            UpdateCatalogInstallationState();
        }
        private void OnClickCancelAll(Object sender, RoutedEventArgs e)
        {
            if (downloadThread != null)
                downloadThread.Abort();
            if (downloadClient != null)
                downloadClient.CancelAsync();
            ExtractionCancellationToken.Cancel();
            downloadList.Clear();
            downloadingMod = null;
            UpdateCatalogInstallationState();
            lstDownloads.MinHeight = 0;
            lstDownloads.Height = 0;
            btnCancelStackpanel.Height = 0;
        }
        private void OnClickWebsite(Object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(currentMod.Website))
                Process.Start(currentMod.Website);
        }
        private void OnClickCatalogHeader(Object sender, EventArgs e)
        {
            MethodInfo[] accessors = null;
            if (sender == colCatalogName.Header)
                accessors = typeof(Mod).GetProperty("Name")?.GetAccessors();
            else if (sender == colCatalogAuthor.Header)
                accessors = typeof(Mod).GetProperty("Author")?.GetAccessors();
            else if (sender == colCatalogCategory.Header)
                accessors = typeof(Mod).GetProperty("Category")?.GetAccessors();
            else if (sender == colCatalogInstalled.Header)
                accessors = typeof(Mod).GetProperty("Installed")?.GetAccessors();
            if (accessors != null)
            {
                Boolean ascending = sender != ascendingSortedColumn;
                ascendingSortedColumn = ascending ? sender : null;
                if (accessors.Length > 0 && accessors[0].ReturnType != typeof(void))
                    SortCatalog(accessors[0], ascending);
                else if (accessors.Length > 1 && accessors[1].ReturnType != typeof(void))
                    SortCatalog(accessors[1], ascending);
            }
        }

        private void OnClickActiveHeader(Object sender, EventArgs e)
        {
            MethodInfo[] accessors = null;
            if (sender == colMyModsActive.Header)
                accessors = typeof(Mod).GetProperty("IsActive")?.GetAccessors();
            if (accessors != null && (Mod)modListInstalled[0] != null)
            {
                Boolean isFirstModActive = modListInstalled[0].IsActive;
                foreach (Mod mod in modListInstalled)
                {
                    mod.IsActive = !isFirstModActive;
                    if (mod.IsActive)
                        mod.TryApplyPreset();
                }
                lstMods.Items.Refresh();
            }
            CheckOutdatedAndIncompatibleMods();
        }

        private void OnPreviewFileDownloaded(Object sender, EventArgs e)
        {
            if (PreviewModImage.Source == sender)
                return;
        }

        private void DownloadStart(Mod mod)
        {
            if (downloadingMod != null)
                return;
            downloadingMod = mod;
            downloadBytesTime = DateTime.Now;
            downloadThread = new Thread(() =>
            {
                String modUrl = TryGetModdbMirror(mod.DownloadUrl);
                if (modUrl == null)
                    return;
                Directory.CreateDirectory(Mod.INSTALLATION_TMP);
                string ext = mod.DownloadFormat ?? "zip";
                if (ext.StartsWith("SingleFileWithPath"))
                {
                    ext = "zip";
                }
                downloadingPath = Mod.INSTALLATION_TMP + "/" + (mod.InstallationPath ?? mod.Name) + "." + ext;
                downloadClient = new ThrottledWebClient();
                downloadClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadLoop);
                downloadClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadEnd);
                downloadClient.DownloadFileAsync(new Uri(modUrl), downloadingPath);
            });
            downloadThread.Start();
            lstDownloads.MinHeight = 100;
            lstDownloads.Height = 100;
            btnCancelStackpanel.Height = 100;
        }
        private String TryGetModdbMirror(String modUrl)
        {
            String moddbLink = "moddb.com/downloads/";
            Int32 start = modUrl.IndexOf(moddbLink, StringComparison.InvariantCultureIgnoreCase);
            if (start < 0)
                return modUrl;

            try
            {
                // Get the mod ID
                start = modUrl.IndexOf("/", start + moddbLink.Length, StringComparison.InvariantCultureIgnoreCase) + 1;
                Int32 end = modUrl.IndexOfAny(['/', '?'], start);
                if (end < 0) end = modUrl.Length;
                String moddbID = modUrl.Substring(start, end - start);

                // Download the page
                WebClient client = new WebClient();
                String html = client.DownloadString($"https://www.moddb.com/downloads/start/{moddbID}/all");

                // Get the mirror
                start = html.IndexOf($@"<a href=""/downloads/mirror/{moddbID}/") + 9;
                end = html.IndexOf(@"""", start);

                String url = $"https://www.moddb.com" + html.Substring(start, end - start);
                return url;
            }
            catch
            {
                return null;
            }
        }
        private void DownloadLoop(Object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                Double timeSpan = (DateTime.Now - downloadBytesTime).TotalSeconds;
                if (timeSpan <= 0.0)
                    timeSpan = 0.1;
                try
                {
                    downloadingMod.PercentComplete = e.ProgressPercentage;
                    downloadingMod.DownloadSpeed = $"{(Int64)(e.BytesReceived / 1024.0 / timeSpan)} {Lang.Measurement.KByteAbbr}/{Lang.Measurement.SecondAbbr}";
                    downloadingMod.RemainingTime = $"{TimeSpan.FromSeconds((e.TotalBytesToReceive - e.BytesReceived) * timeSpan / e.BytesReceived):g}";
                }
                catch (NullReferenceException) // added to catch a race condition that sometimes occures where Ui tries to update but download has finished
                {

                }
                lstDownloads.Items.Refresh();
            });
        }

        private Task ExtractAllFileFromArchive(String archivePath, String extactTo)
        {
            return Task.Run(() =>
            {
                Extractor.ExtractAllFileFromArchive(archivePath, extactTo, ExtractionCancellationToken.Token);
                if (ExtractionCancellationToken.IsCancellationRequested)
                {
                    ExtractionCancellationToken.Dispose();
                    ExtractionCancellationToken = new CancellationTokenSource();
                }
            });
        }

        private void DownloadEnd(Object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                if (File.Exists(downloadingPath))
                    File.Delete(downloadingPath);
                if (!Directory.EnumerateFileSystemEntries(Mod.INSTALLATION_TMP).GetEnumerator().MoveNext())
                    Directory.Delete(Mod.INSTALLATION_TMP);
                downloadingPath = "";
                return;
            }
            downloadingPath = "";
            Dispatcher.BeginInvoke((MethodInvoker)async delegate
            {
                String downloadingModName = downloadingMod.Name;
                String path = Mod.INSTALLATION_TMP + "/" + (downloadingMod.InstallationPath ?? downloadingModName);
                Boolean success = false;
                String downloadFormatExtLower = (downloadingMod.DownloadFormat ?? "zip").ToLower();
                if (String.IsNullOrEmpty(downloadingMod.DownloadFormat) || supportedArchives.Contains(downloadFormatExtLower))
                {
                    Directory.CreateDirectory(path);
                    try
                    {
                        Boolean proceedNext = false;
                        Boolean hasDesc = false;
                        Boolean moveDesc = false;
                        String sourcePath = "";
                        String destPath = "";
                        await ExtractAllFileFromArchive(path + "." + downloadFormatExtLower, path);
                        File.Delete(path + "." + downloadFormatExtLower);
                        if (File.Exists(path + "/" + Mod.DESCRIPTION_FILE))
                        {
                            hasDesc = true;
                            Mod modInfo = new Mod(path);
                            if (!String.IsNullOrEmpty(modInfo.InstallationPath) && Directory.Exists(path + "/" + modInfo.InstallationPath))
                            {
                                sourcePath = path + "/" + modInfo.InstallationPath;
                                destPath = modInfo.InstallationPath;
                                moveDesc = true;
                                proceedNext = true;
                            }
                            else if (!String.IsNullOrEmpty(downloadingMod.InstallationPath) && Directory.Exists(path + "/" + downloadingMod.InstallationPath))
                            {
                                sourcePath = path + "/" + downloadingMod.InstallationPath;
                                destPath = downloadingMod.InstallationPath;
                                moveDesc = true;
                                proceedNext = true;
                            }
                            else if (Directory.Exists(path + "/" + (downloadingMod.InstallationPath ?? downloadingModName)))
                            {
                                sourcePath = path + "/" + (downloadingMod.InstallationPath ?? downloadingModName);
                                destPath = downloadingMod.InstallationPath ?? downloadingModName;
                                proceedNext = true;
                                moveDesc = true;
                            }
                            else if (Mod.LooksLikeAModFolder(path))
                            {
                                sourcePath = path;
                                destPath = downloadingMod.InstallationPath ?? downloadingModName;
                                proceedNext = true;
                            }
                            else
                            {
                                MessageBox.Show($"Please install the mod folder manually.", "Warning", MessageBoxButton.OK);
                                Process.Start(Path.GetFullPath(path));
                            }
                        }
                        else
                        {
                            if (Directory.Exists(path + "/" + (downloadingMod.InstallationPath ?? downloadingModName)))
                            {
                                sourcePath = path + "/" + downloadingMod.InstallationPath;
                                destPath = downloadingMod.InstallationPath ?? downloadingModName;
                                proceedNext = true;
                            }
                            else if (Mod.LooksLikeAModFolder(path))
                            {
                                sourcePath = path;
                                destPath = downloadingMod.InstallationPath ?? downloadingModName;
                                proceedNext = true;
                            }
                            else
                            {
                                String[] subDirectories = Directory.GetDirectories(path);
                                foreach (String sd in subDirectories)
                                    if (Mod.LooksLikeAModFolder(sd))
                                    {
                                        sourcePath = sd;
                                        destPath = downloadingMod.InstallationPath ?? downloadingModName;
                                        proceedNext = true;
                                        break;
                                    }
                                if (!proceedNext)
                                {
                                    MessageBox.Show($"Please install the mod folder manually.", "Warning", MessageBoxButton.OK);
                                    Process.Start(Path.GetFullPath(path));
                                }
                            }
                        }
                        if (proceedNext)
                        {
                            if (Directory.Exists(destPath))
                                Directory.Delete(destPath, true);
                            if (proceedNext)
                            {
                                Directory.Move(sourcePath, destPath);
                                if (moveDesc)
                                    File.Move(path + "/" + Mod.DESCRIPTION_FILE, destPath + "/" + Mod.DESCRIPTION_FILE);
                                else if (!hasDesc)
                                    downloadingMod.GenerateDescription(destPath);
                                if (Directory.Exists(path))
                                    Directory.Delete(path, true);
                                success = true;
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show($"Failed to automatically install the mod {path}\n\n{err.Message}", "Error", MessageBoxButton.OK);
                    }
                }
                else if (downloadingMod.DownloadFormat.StartsWith("SingleFileWithPath:"))
                {
                    Boolean proceedNext = true;
                    String modInstallPath = downloadingMod.InstallationPath ?? downloadingModName;
                    if (Directory.Exists(modInstallPath))
                    {
                        if (MessageBox.Show($"The current version of the mod folder, {modInstallPath}, will be deleted before moving the new version.\nProceed?", "Updating", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Directory.Delete(modInstallPath, true);
                        }
                        else
                        {
                            Process.Start(Path.GetFullPath(Path.GetDirectoryName(path)));
                            proceedNext = false;
                        }
                    }
                    if (proceedNext)
                    {
                        String filePath = downloadingMod.DownloadFormat.Substring("SingleFileWithPath:".Length);
                        String destPath = modInstallPath + "/" + filePath;
                        Directory.CreateDirectory(destPath.Substring(0, destPath.LastIndexOf('/')));
                        File.Move(path + ".zip", destPath);
                        downloadingMod.GenerateDescription(modInstallPath);
                        String singleFileList = null;
                        if (filePath.StartsWith("StreamingAssets/", StringComparison.OrdinalIgnoreCase))
                            singleFileList = filePath.Substring("StreamingAssets/".Length).ToLower();
                        else if (filePath.StartsWith("FF9_Data/", StringComparison.OrdinalIgnoreCase))
                            singleFileList = filePath.Substring("FF9_Data/".Length).ToLower();
                        else if (Mod.MEMORIA_ROOT_FILES.Any(str => String.Compare(filePath, str, StringComparison.OrdinalIgnoreCase) == 0))
                            singleFileList = filePath.ToLower();
                        if (!String.IsNullOrEmpty(singleFileList) && !Mod.ARCHIVE_BUNDLE_FILES.Contains(singleFileList))
                            File.WriteAllText(modInstallPath + "/" + Mod.MOD_CONTENT_FILE, singleFileList + "\n");
                        success = true;
                    }
                }
                Boolean activateTheNewMod = success;
                if (success)
                {
                    if (!Directory.EnumerateFileSystemEntries(Mod.INSTALLATION_TMP).GetEnumerator().MoveNext())
                        Directory.Delete(Mod.INSTALLATION_TMP);
                    Mod previousMod = Mod.SearchWithName(modListInstalled, downloadingModName);
                    if (previousMod != null)
                    {
                        previousMod.CurrentVersion = null;
                        activateTheNewMod = false;
                    }
                }
                downloadList.Remove(downloadingMod);
                downloadingMod = null;
                if (downloadList.Count > 0)
                    DownloadStart(downloadList[0]);
                else
                {
                    lstDownloads.MinHeight = 0;
                    lstDownloads.Height = 0;
                    btnCancelStackpanel.Height = 0;
                }
                CheckForValidModFolder();
                UpdateCatalogInstallationState();
                if (activateTheNewMod)
                {
                    Mod newMod = Mod.SearchWithName(modListInstalled, downloadingModName);
                    if (newMod != null)
                    {
                        newMod.IsActive = true;
                        newMod.TryApplyPreset();
                    }
                }
                CheckOutdatedAndIncompatibleMods();
            });
        }
        private void DownloadCatalogEnd(Object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                if (File.Exists(CATALOG_PATH + ".tmp"))
                    File.Delete(CATALOG_PATH + ".tmp");
                return;
            }
            Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                if (File.Exists(CATALOG_PATH))
                    File.Delete(CATALOG_PATH);
                File.Move(CATALOG_PATH + ".tmp", CATALOG_PATH);
                ReadCatalog();
            });
        }

        private void UpdateCatalog()
        {
            if (File.Exists(CATALOG_PATH))
            {
                FileInfo fi = new FileInfo(CATALOG_PATH);
                if (fi.IsReadOnly) // Local testing of catalog: put it as read-only
                {
                    ReadCatalog();
                    return;
                }
            }
            modListCatalog.Clear();
            ReadCatalog();
            downloadCatalogThread = new Thread(() =>
            {
                downloadCatalogClient = new WebClient();
                downloadCatalogClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCatalogEnd);
                downloadCatalogClient.DownloadFileAsync(new Uri(CATALOG_URL), CATALOG_PATH + ".tmp");
            });
            downloadCatalogThread.Start();
        }

        private void ReadCatalog()
        {
            if (!File.Exists(CATALOG_PATH))
                return;
            try
            {
                modListCatalog.Clear();
                using (Stream input = File.OpenRead(CATALOG_PATH))
                using (StreamReader reader = new StreamReader(input))
                    Mod.LoadModDescriptions(reader, ref modListCatalog);
                UpdateCatalogInstallationState();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void UpdateModListInstalled()
        {
            foreach (String dir in Directory.EnumerateDirectories("."))
            {
                if (File.Exists(dir + "/" + Mod.DESCRIPTION_FILE))
                {
                    Mod updatedMod = new Mod(dir);
                    Mod previousMod = Mod.SearchWithName(modListInstalled, updatedMod.Name);
                    if (previousMod == null)
                    {
                        modListInstalled.Insert(0, updatedMod);
                    }
                    else if ((updatedMod.CurrentVersion != null && previousMod.CurrentVersion == null) || (previousMod.CurrentVersion != null && updatedMod.CurrentVersion != null && previousMod.CurrentVersion < updatedMod.CurrentVersion))
                    {
                        foreach (Mod subMod in updatedMod.SubMod)
                        {
                            Mod previousSub = Mod.SearchWithPath(previousMod.SubMod, subMod.InstallationPath);
                            if (previousSub != null)
                                subMod.IsActive = previousSub.IsActive;
                        }
                        Int32 index = modListInstalled.IndexOf(previousMod);
                        modListInstalled.RemoveAt(index);
                        modListInstalled.Insert(index, updatedMod);
                        updatedMod.IsActive = previousMod.IsActive;
                    }
                }
            }
            UpdateInstalledPriorityValue();
            CheckOutdatedAndIncompatibleMods();
            //UpdateLauncherTheme();
        }

        private static String currentColor;
        private static String currentImage;

        private void UpdateLauncherTheme()
        {
            String color = DefaultAccentColor;
            String image = DefaultBackgroundImage;
            foreach (var mod in modListInstalled)
            {
                if (!mod.IsActive) continue;

                if (mod.LauncherBackground != null)
                {
                    String newImage = $"{Directory.GetCurrentDirectory()}\\{mod.FullInstallationPath}\\{mod.LauncherBackground}";
                    if (!File.Exists(newImage))
                        continue;
                    image = newImage;

                    if(mod.LauncherColor != null)
                        color = $"#CC{mod.LauncherColor.Replace("#", "")}";
                    break;
                }
            }
            if (color != currentColor)
            {
                SetAccentColor((Color)ColorConverter.ConvertFromString(color));
                currentColor = color;
            }
            if (image != currentImage)
            {
                Launcher.Source = new BitmapImage(new Uri(image, UriKind.Absolute));
                currentImage = image;
            }
        }

        private Boolean GenerateAutomaticDescriptionFile(String folderName)
        {
            if (!Directory.Exists(folderName) || File.Exists(folderName + "/" + Mod.DESCRIPTION_FILE))
                return false;
            Mod catalogVersion = Mod.SearchWithPath(modListCatalog, folderName);
            if (catalogVersion != null)
            {
                catalogVersion.GenerateDescription(folderName);
                return true;
            }

            String name = folderName;
            String author = "";
            String category = "";
            String description = "";

            /*
            if (folderName == "MoguriFiles")
            {
                name = "Moguri Mod";
                author = "ZePilot / Snouz";
                category = "Visual";
                description = "";
            }

            if (folderName == "MoguriSoundtrack")
            {
                name = "Moguri Soundtrack";
                author = "Pontus Hultgren / ZePilot";
                category = "Music";
                description = "";
            }

            if (folderName == "MoguriVideo")
            {
                name = "Moguri Video";
                author = "Snouz / Lykon";
                category = "Visual";
                description = "";
            }
            */

            File.WriteAllText(folderName + "/" + Mod.DESCRIPTION_FILE,
                "<Mod>\n" +
                "    <Name>" + (name ?? folderName) + "</Name>\n" +
                "    <Author>" + (author ?? "Unknown") + "</Author>\n" +
                "    <InstallationPath>" + folderName + "</InstallationPath>\n" +
                "    <Category>" + (category ?? "Unknown") + "</Category>\n" +
                "    <Description>" + (description ?? "") + "</Description>\n" +
                "</Mod>");
            return true;
        }

        private void CheckForValidModFolder()
        {
            foreach (String dir in Directory.EnumerateDirectories("."))
            {
                String shortName = dir.Substring(2);
                if (shortName != "x64" && shortName != "x86" && Mod.LooksLikeAModFolder(shortName))
                    GenerateAutomaticDescriptionFile(shortName);
            }
            UpdateModListInstalled();
        }

        private void UpdateModDetails(Mod mod)
        {
            currentMod = mod;
            if (mod == null || mod.Name == null)
            {
                gridModName.Visibility = Visibility.Collapsed;
                gridModInfo.Visibility = Visibility.Collapsed;
                PreviewModWebsite.Visibility = Visibility.Collapsed;
                PreviewModCategoryTagline.Visibility = Visibility.Collapsed;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,,/images/Gradient.png");
                bitmap.EndInit();
                PreviewModImage.Source = bitmap;
            }
            else
            {
                gridModName.Visibility = Visibility.Visible;
                gridModInfo.Visibility = Visibility.Visible;
                PreviewModCategoryTagline.Visibility = !String.IsNullOrEmpty(mod.Category) ? Visibility.Visible : Visibility.Collapsed;
                PreviewModName.Text = mod.Name;
                PreviewModVersion.Text = mod.CurrentVersion?.ToString() ?? "";
                PreviewModRelease.Text = mod.ReleaseDate ?? "";
                PreviewModReleaseOriginal.Text = mod.ReleaseDateOriginal ?? PreviewModRelease.Text;
                if (PreviewModRelease.Text == "" && PreviewModReleaseOriginal.Text != "") PreviewModRelease.Text = PreviewModReleaseOriginal.Text;
                PreviewModAuthor.Text = mod.Author ?? "Unknown";
                PreviewModDescription.Text = mod.Description != null && mod.Description != "" ? mod.Description : "No description.";
                if (mod.IncompatibleWith != null && mod.IncompatibleWith != "" && !mod.Description.Contains("⚠️"))
                    mod.Description = $"⚠️ The mod is incompatible with: {mod.IncompatibleWith}.\n\n{mod.Description}";
                PreviewModReleaseNotes.Text = mod.PatchNotes ?? "";
                PreviewModCategory.Text = mod.Category ?? "Unknown";
                PreviewModWebsite.ToolTip = null;
                UiGrid.MakeTooltip(PreviewModWebsite, mod.Website ?? String.Empty, "", "hand");
                PreviewModWebsite.IsEnabled = !String.IsNullOrEmpty(mod.Website);
                PreviewModWebsite.Visibility = PreviewModWebsite.IsEnabled ? Visibility.Visible : Visibility.Collapsed;
                PreviewSubModPanel.Visibility = Visibility.Collapsed;
                Boolean hasSubMod = mod.SubMod != null && mod.SubMod.Count > 0;
                if (hasSubMod)
                {
                    PreviewSubModPanel.Visibility = Visibility.Visible;
                }
                ReleaseNotesBlock.Visibility = PreviewModReleaseNotes.Text == "" ? Visibility.Collapsed : Visibility.Visible;
                if (hasSubMod)
                {
                    if (modListCatalog.Contains(mod))
                    {
                        Mod installedVersion = Mod.SearchWithName(modListInstalled, mod.Name);
                        if (installedVersion != null)
                        {
                            foreach (Mod subMod in mod.SubMod)
                            {
                                Mod installedSub = Mod.SearchWithPath(installedVersion.SubMod, subMod.InstallationPath);
                                if (installedSub != null)
                                    subMod.IsActive = installedSub.IsActive;
                            }
                        }
                    }
                    PreviewSubModList.ItemsSource = mod.SubMod;
                    PreviewSubModList.SelectedItem = mod.SubMod[0];
                    UpdateSubModDetails(mod.SubMod[0]);
                }
                if (mod.PreviewImage == null)
                {
                    if (tabCtrlMain.SelectedIndex == 0 && mod.PreviewFile != null)
                    {
                        String imagePath = $"./{mod.InstallationPath}/{mod.PreviewFile}";
                        if (File.Exists(imagePath))
                        {
                            mod.PreviewImage = new BitmapImage();
                            mod.PreviewImage.BeginInit();
                            mod.PreviewImage.UriSource = new Uri(imagePath, UriKind.Relative);
                            mod.PreviewImage.CacheOption = BitmapCacheOption.OnLoad;
                            mod.PreviewImage.EndInit();
                        }
                    }
                    else if (tabCtrlMain.SelectedIndex == 0 && mod.PreviewFileUrl != null)
                    {
                        mod.PreviewImage = new BitmapImage(new Uri(mod.PreviewFileUrl, UriKind.Absolute));
                        mod.PreviewImage.DownloadCompleted += OnPreviewFileDownloaded;
                    }
                    else if (tabCtrlMain.SelectedIndex == 1 && mod.PreviewFileUrl != null)
                    {
                        mod.PreviewImage = new BitmapImage(new Uri(mod.PreviewFileUrl, UriKind.Absolute));
                        mod.PreviewImage.DownloadCompleted += OnPreviewFileDownloaded;
                    }
                }
                if (mod.PreviewImage == null)
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,,/images/Gradient.png");
                    bitmap.EndInit();
                    PreviewModImage.Source = bitmap;
                }
                else if (mod.PreviewImage.IsDownloading)
                {
                    PreviewModImage.Source = mod.PreviewImage;
                }
                else
                {
                    PreviewModImage.Source = mod.PreviewImage;
                }
            }
            UpdateLauncherTheme();
        }

        private void UpdateSubModDetails(Mod subMod)
        {
            if (subMod == null)
                return;
            PreviewSubModActive.IsEnabled = tabCtrlMain.SelectedIndex == 0;
            PreviewSubModActive.IsChecked = subMod.IsActive;
            PreviewSubModDescription.Text = subMod.Description ?? "No description.";
        }

        private void UpdateCatalogInstallationState()
        {
            foreach (Mod mod in modListCatalog)
            {
                if (Mod.SearchWithName(downloadList, mod.Name) != null)
                    mod.Installed = WaitingEmoji;
                else if (Mod.SearchWithName(modListInstalled, mod.Name) != null && Mod.SearchWithName(modListInstalled, mod.Name).IsOutdated)
                    mod.Installed = UpdateEmoji;
                else if (Mod.SearchWithName(modListInstalled, mod.Name) != null)
                    mod.Installed = InstalledEmoji;

                else
                    mod.Installed = "";
            }
            lstCatalogMods.Items.Refresh();
        }

        private void UpdateInstalledPriorityValue()
        {
            for (Int32 i = 0; i < modListInstalled.Count; i++)
                modListInstalled[i].Priority = i + 1;
            lstMods.Items.Refresh();
        }

        private void SortCatalog(MethodInfo sortGetter, Boolean ascending)
        {
            if (sortGetter == null || sortGetter.DeclaringType != typeof(Mod) || sortGetter.ReturnType.GetInterface(nameof(IComparable)) == null || sortGetter.GetParameters().Length > 0)
                return;
            List<Mod> catalogList = new List<Mod>(modListCatalog);
            catalogList.Sort(delegate (Mod a, Mod b)
            {
                IComparable ac = sortGetter.Invoke(a, null) as IComparable;
                IComparable bc = sortGetter.Invoke(b, null) as IComparable;
                if (ac == null && bc == null)
                    return 0;
                if (ac == null)
                    return 1;
                if (bc == null)
                    return -1;
                return ascending ? ac.CompareTo(bc) : -ac.CompareTo(bc);
            });
            modListCatalog = new ObservableCollection<Mod>(catalogList);
            lstCatalogMods.ItemsSource = modListCatalog;
        }

        private void LoadSettings2()
        {
            modListInstalled.Clear();
            try
            {
                IniReader iniReader = new IniReader(INI_PATH);

                String str = iniReader.GetSetting("Mod", "FolderNames");
                if (String.IsNullOrEmpty(str))
                    str = "";
                str = str.Trim().Trim('"');
                String[] iniModActiveList = Regex.Split(str, @""",\s*""");
                str = iniReader.GetSetting("Mod", "Priorities");
                if (String.IsNullOrEmpty(str))
                    str = "";
                str = str.Trim().Trim('"');
                String[] iniModPriorityList = Regex.Split(str, @""",\s*""");
                String[][] listCouple = new String[][] { iniModPriorityList, iniModActiveList };
                List<String> subModList = new List<String>();
                for (Int32 listI = 0; listI < 2; ++listI)
                {
                    for (Int32 i = 0; i < listCouple[listI].Length; ++i)
                    {
                        if (String.IsNullOrEmpty(listCouple[listI][i]))
                            continue;
                        if (!Directory.Exists(listCouple[listI][i]))
                            continue;
                        if (listCouple[listI][i].Contains('/'))
                        {
                            if (listI == 1)
                                subModList.Add(listCouple[listI][i]);
                            continue;
                        }
                        Mod mod = Mod.SearchWithPath(modListInstalled, listCouple[listI][i]);
                        if (mod != null)
                        {
                            if (listI == 1)
                                mod.IsActive = true;
                            continue;
                        }
                        GenerateAutomaticDescriptionFile(listCouple[listI][i]);
                        if (File.Exists(listCouple[listI][i] + "/" + Mod.DESCRIPTION_FILE))
                            mod = new Mod(listCouple[listI][i]);
                        else
                            mod = new Mod(listCouple[listI][i], listCouple[listI][i]);
                        if (listI == 1)
                            mod.IsActive = true;
                        modListInstalled.Add(mod);
                    }
                }
                foreach (String path in subModList)
                {
                    Int32 sepIndex = path.IndexOf("/");
                    String mainModPath = path.Substring(0, sepIndex);
                    String subModPath = path.Substring(sepIndex + 1);
                    Mod mod = Mod.SearchWithPath(modListInstalled, mainModPath);
                    if (mod.SubMod == null)
                        continue;
                    foreach (Mod sub in mod.SubMod)
                        if (sub.InstallationPath == subModPath)
                            sub.IsActive = true;
                }
            }
            catch (Exception ex) { UiHelper.ShowError(Application.Current.MainWindow, ex); }
        }

        private void UpdateSettings()
        {
            try
            {
                List<String> iniModActiveList = new List<String>();
                List<String> iniModPriorityList = new List<String>();
                foreach (Mod mod in modListInstalled)
                {
                    iniModActiveList.AddRange(mod.EnumerateModAndSubModFoldersOrdered(true));
                    iniModPriorityList.Add(mod.InstallationPath);
                }
                IniFile iniFile = new IniFile(INI_PATH);
                iniFile.WriteValue("Mod", "FolderNames", iniModActiveList.Count > 0 ? " \"" + String.Join("\", \"", iniModActiveList) + "\"" : "");
                iniFile.WriteValue("Mod", "Priorities", iniModPriorityList.Count > 0 ? " \"" + String.Join("\", \"", iniModPriorityList) + "\"" : "");
            }
            catch (Exception ex) { UiHelper.ShowError(Application.Current.MainWindow, ex); }
        }

        private void SetupFrameLang()
        {
            GroupModInfo.Header = Lang.ModEditor.ModInfos;
            PreviewModWebsite.Content = Lang.ModEditor.Website;
            CaptionModAuthor.Text = Lang.ModEditor.Author + ":";
            CaptionModRelease.Text = Lang.ModEditor.Release + ":";
            CaptionModReleaseOriginal.Text = Lang.ModEditor.ReleaseOriginal + ":";
            CaptionModReleaseNotes.Text = Lang.ModEditor.ReleaseNotes + ":";
            PreviewSubModActive.Content = Lang.ModEditor.Active;
            CaptionSubModPanel.Text = Lang.ModEditor.SubModPanel + ":";
            tabMyMods.Text = Lang.ModEditor.TabMyMods;
            colMyModsName.Header = Lang.ModEditor.Name;
            colMyModsCategory.Header = Lang.ModEditor.Category;
            btnMoveUp.ToolTip = Lang.ModEditor.TooltipMoveUp;
            btnMoveDown.ToolTip = Lang.ModEditor.TooltipMoveDown;
            btnUninstall.ToolTip = Lang.ModEditor.TooltipUninstall;
            tabCatalog.Text = Lang.ModEditor.TabCatalog;
            GridViewColumnHeader header = new GridViewColumnHeader() { Content = Lang.ModEditor.Name };
            header.Click += OnClickCatalogHeader;
            colCatalogName.Header = header;
            header = new GridViewColumnHeader() { Content = Lang.ModEditor.Author };
            header.Click += OnClickCatalogHeader;
            colCatalogAuthor.Header = header;
            header = new GridViewColumnHeader() { Content = Lang.ModEditor.Category };
            header.Click += OnClickCatalogHeader;
            colCatalogCategory.Header = header;
            header = new GridViewColumnHeader() { Content = "" };
            header.Click += OnClickCatalogHeader;
            colCatalogInstalled.Header = header;
            header = new GridViewColumnHeader() { Content = ActiveEmoji };
            header.Click += OnClickActiveHeader;
            colMyModsActive.Header = header;
            colDownloadName.Header = Lang.ModEditor.Mod;
            colDownloadProgress.Header = Lang.ModEditor.Progress;
            colDownloadSpeed.Header = Lang.ModEditor.Speed;
            colDownloadTimeLeft.Header = Lang.ModEditor.TimeLeft;
            btnDownload.ToolTip = Lang.ModEditor.TooltipDownload;
            btnCancel.ToolTip = Lang.ModEditor.TooltipCancel;
        }

        private Mod currentMod;
        private Mod downloadingMod;
        private String downloadingPath;
        private DateTime downloadBytesTime;
        private Thread downloadThread;
        private Thread downloadCatalogThread;
        private WebClient downloadClient;
        private WebClient downloadCatalogClient;
        private object ascendingSortedColumn = null;

        private const String INI_PATH = IniFile.IniPath;
        private const String CATALOG_PATH = "./ModCatalog.xml";
        private const String CATALOG_URL = "https://raw.githubusercontent.com/Albeoris/Memoria/main/Memoria.Launcher/Catalogs/MemoriaCatalog.xml";
    }
}
