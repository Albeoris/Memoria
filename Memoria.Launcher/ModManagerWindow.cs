using Ini;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.DataFormats;
using Application = System.Windows.Application;
using Color = System.Drawing.Color;
using GridView = System.Windows.Controls.GridView;
using GridViewColumnHeader = System.Windows.Controls.GridViewColumnHeader;
using ListView = System.Windows.Controls.ListView;
using MessageBox = System.Windows.Forms.MessageBox;
using Point = System.Drawing.Point;

namespace Memoria.Launcher
{
    public partial class ModManagerWindow : Window, IComponentConnector
    {
        public ObservableCollection<Mod> modListInstalled = new ObservableCollection<Mod>();
        public ObservableCollection<Mod> modListCatalog = new ObservableCollection<Mod>();
        public ObservableCollection<Mod> downloadList = new ObservableCollection<Mod>();
        public String StatusMessage = "";

        public String[] supportedArchives = { "rar", "unrar", "zip", "bzip2", "gzip", "tar", "7zip", "lzip", "gz" };

        public ModManagerWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            Closing += new CancelEventHandler(OnClosing);
        }

        private void OnLoaded(Object sender, RoutedEventArgs e)
        {
            SetupFrameLang();
            UpdateCatalog();
            LoadSettings();
            CheckForValidModFolder();
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
        }

        private void OnClosing(Object sender, CancelEventArgs e)
        {
            if (downloadList.Count > 0 || downloadingMod != null)
            {
                e.Cancel = true;
                MessageBox.Show($"Please don't close this window while downloads are on their way.", "Error", MessageBoxButtons.OK);
                return;
            }
            if (downloadCatalogClient != null && downloadCatalogClient.IsBusy)
                downloadCatalogClient.CancelAsync();
            UpdateSettings();
            ((MainWindow)this.Owner).ModdingWindow = null;
            ((MainWindow)this.Owner).MemoriaIniControl.ComeBackToLauncherFromModManager();
        }

        [DllImport("user32.dll")]
        public static extern Int32 SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll")]
        public static extern Boolean ReleaseCapture();

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
                btnDownload.Background = System.Windows.Media.Brushes.White;
            }
            else
            {
                btnDownload.Background = System.Windows.Media.Brushes.Transparent;
            }
        }
        private void OnModListDoubleClick(Object sender, RoutedEventArgs e)
        {
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
        private void OnClickCheckConflicts(Object sender, RoutedEventArgs e)
        {
            ModConflictWindow conflictWindow = new ModConflictWindow(modListInstalled, true);
            conflictWindow.Owner = this;
            conflictWindow.ShowDialog();
        }
        private void OnClickCheckAllConflicts(Object sender, RoutedEventArgs e)
        {
            ModConflictWindow conflictWindow = new ModConflictWindow(modListInstalled, false);
            conflictWindow.Owner = this;
            conflictWindow.ShowDialog();
        }
        private void OnClickUninstall(Object sender, RoutedEventArgs e)
        {
            List<Mod> selectedMods = new List<Mod>();
            foreach (Mod mod in lstMods.SelectedItems)
                selectedMods.Add(mod);
            foreach (Mod mod in selectedMods)
            {
                if (Directory.Exists(mod.InstallationPath))
                    if (MessageBox.Show($"The mod folder {mod.InstallationPath} will be deleted.\nProceed?", "Updating", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Directory.Delete(mod.InstallationPath, true);
                        modListInstalled.Remove(mod);
                        UpdateInstalledPriorityValue();
                    }
            }
            UpdateCatalogInstallationState();
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
        private void OnClickActivateAll(Object sender, RoutedEventArgs e)
        {
            foreach (Mod mod in modListInstalled)
                mod.IsActive = true;
            lstMods.Items.Refresh();
        }
        private void OnClickDeactivateAll(Object sender, RoutedEventArgs e)
        {
            foreach (Mod mod in modListInstalled)
                mod.IsActive = false;
            lstMods.Items.Refresh();
        }
        private void OnClickDownload(Object sender, RoutedEventArgs e)
        {
            foreach (Mod mod in lstCatalogMods.SelectedItems)
            {
                if (downloadList.Contains(mod) || String.IsNullOrEmpty(mod.DownloadUrl))
                    return;
                if (!String.IsNullOrEmpty(mod.MinimumMemoriaVersion))
                {
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
                }
                downloadList.Add(mod);
                DownloadStart(mod);
                mod.Installed = "⌛";
            }
            lstCatalogMods.Items.Refresh();
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
            Close();
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
            UpdateCatalogInstallationState();
        }
        private void OnClickCancelAll(Object sender, RoutedEventArgs e)
        {
            if (downloadThread != null)
                downloadThread.Abort();
            if (downloadClient != null)
                downloadClient.CancelAsync();
            downloadList.Clear();
            downloadingMod = null;
            UpdateCatalogInstallationState();
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
                downloadClient = new WebClient();
                downloadClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadLoop);
                downloadClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadEnd);
                downloadClient.DownloadFileAsync(new Uri(modUrl), downloadingPath);
            });
            downloadThread.Start();
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

        private void ExtractAllFileFromArchive(String archivePath, String extactTo)
        {
            using (Stream stream = File.OpenRead(archivePath))
            using (var reader = ReaderFactory.Open(stream))
            {
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        Console.WriteLine(reader.Entry.Key);
                        reader.WriteEntryToDirectory(extactTo, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }
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
            Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                String downloadingModName = downloadingMod.Name;
                String path = Mod.INSTALLATION_TMP + "/" + (downloadingMod.InstallationPath ?? downloadingModName);
                Boolean success = false;
                String downloadFormatExtLower = (downloadingMod.DownloadFormat ?? "zip").ToLower();
                if (String.IsNullOrEmpty(downloadingMod.DownloadFormat) || supportedArchives.Contains(downloadFormatLower))
                {
                    Directory.CreateDirectory(path);
                    try
                    {
                        Boolean proceedNext = false;
                        Boolean hasDesc = false;
                        Boolean moveDesc = false;
                        String sourcePath = "";
                        String destPath = "";
                        ExtractAllFileFromArchive(path + "." + downloadFormatExtLower, path);
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
                                MessageBox.Show($"Please install the mod folder manually.", "Warning", MessageBoxButtons.OK);
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
                                    MessageBox.Show($"Please install the mod folder manually.", "Warning", MessageBoxButtons.OK);
                                    Process.Start(Path.GetFullPath(path));
                                }
                            }
                        }
                        if (proceedNext)
                        {
                            if (Directory.Exists(destPath))
                            {
                                if (MessageBox.Show($"The current version of the mod folder, {destPath}, will be deleted before moving the new version.\nProceed?", "Updating", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    Directory.Delete(destPath, true);
                                }
                                else
                                {
                                    Process.Start(Path.GetFullPath(path));
                                    proceedNext = false;
                                }
                            }
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
                        MessageBox.Show($"Failed to automatically install the mod {path}\n\n{err.Message}", "Error", MessageBoxButtons.OK);
                    }
                }
                else if (downloadingMod.DownloadFormat.StartsWith("SingleFileWithPath:"))
                {
                    Boolean proceedNext = true;
                    String modInstallPath = downloadingMod.InstallationPath ?? downloadingModName;
                    if (Directory.Exists(modInstallPath))
                    {
                        if (MessageBox.Show($"The current version of the mod folder, {modInstallPath}, will be deleted before moving the new version.\nProceed?", "Updating", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
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
                CheckForValidModFolder();
                UpdateCatalogInstallationState();
                if (activateTheNewMod)
                {
                    Mod newMod = Mod.SearchWithName(modListInstalled, downloadingModName);
                    if (newMod != null)
                        newMod.IsActive = true;
                }
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
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK);
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
            }
            else
            {
                gridModName.Visibility = Visibility.Visible;
                gridModInfo.Visibility = Visibility.Visible;
                PreviewModName.Text = mod.Name;
                PreviewModVersion.Text = mod.CurrentVersion?.ToString() ?? "";
                PreviewModRelease.Text = mod.ReleaseDate ?? "";
                PreviewModReleaseOriginal.Text = mod.ReleaseDateOriginal ?? PreviewModRelease.Text;
                PreviewModAuthor.Text = mod.Author ?? "Unknown";
                PreviewModDescription.Text = mod.Description != null && mod.Description != "" ? mod.Description : "No description.";
                PreviewModReleaseNotes.Text = mod.PatchNotes ?? "";
                PreviewModCategory.Text = mod.Category ?? "Unknown";
                PreviewModWebsite.ToolTip = mod.Website ?? String.Empty;
                PreviewModWebsite.IsEnabled = !String.IsNullOrEmpty(mod.Website);
                PreviewModWebsite.Visibility = PreviewModWebsite.IsEnabled ? Visibility.Visible : Visibility.Collapsed;
                PreviewSubModPanel.Visibility = Visibility.Collapsed;
                Boolean hasSubMod = mod.SubMod != null && mod.SubMod.Count > 0;
                if (hasSubMod)
                {
                    PreviewSubModPanel.Visibility = Visibility.Visible;
                }
                ReleaseNotesBlock.Visibility = PreviewModReleaseNotes.Text == "" && PreviewModRelease.Text == "" ? Visibility.Collapsed : Visibility.Visible;
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
                        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/" + mod.InstallationPath + "/" + mod.PreviewFile))
                        {
                            mod.PreviewImage = new BitmapImage();
                            mod.PreviewImage.BeginInit();
                            mod.PreviewImage.UriSource = new Uri("file://" + AppDomain.CurrentDomain.BaseDirectory + "/" + mod.InstallationPath + "/" + mod.PreviewFile, UriKind.Absolute);
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
                    PreviewModImage.Source = null;
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
                    mod.Installed = "⌛";
                else if (Mod.SearchWithName(modListInstalled, mod.Name) != null)
                    mod.Installed = "✔";
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

        private void LoadSettings()
        {
            modListInstalled.Clear();
            try
            {
                IniFile iniFile = new IniFile(INI_PATH);
                String str = iniFile.ReadValue("Mod", "FolderNames");
                if (String.IsNullOrEmpty(str))
                    str = "";
                str = str.Trim().Trim('"');
                String[] iniModActiveList = Regex.Split(str, @""",\s*""");
                str = iniFile.ReadValue("Mod", "Priorities");
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
            Title = Lang.ModEditor.WindowTitle + " | " + ((MainWindow)this.Owner).MemoriaAssemblyCompileDate.ToString("yyyy-MM-dd");
            GroupModInfo.Header = Lang.ModEditor.ModInfos;
            PreviewModWebsite.Content = Lang.ModEditor.Website;
            CaptionModAuthor.Text = Lang.ModEditor.Author + ":";
            CaptionModCategory.Text = Lang.ModEditor.Category + ":";
            //CaptionModDescription.Text = Lang.ModEditor.Description + ":";
            CaptionModReleaseOriginal.Text = Lang.ModEditor.Release + ":";
            CaptionModReleaseNotes.Text = Lang.ModEditor.ReleaseNotes + ":";
            PreviewSubModActive.Content = Lang.ModEditor.Active;
            CaptionSubModPanel.Text = Lang.ModEditor.SubModPanel + ":";
            tabMyMods.Text = Lang.ModEditor.TabMyMods;
            //colMyModsPriority.Header = Lang.ModEditor.Priority;
            colMyModsName.Header = Lang.ModEditor.Name;
            colMyModsAuthor.Header = Lang.ModEditor.Author;
            colMyModsCategory.Header = Lang.ModEditor.Category;
            //colMyModsActive.Header = Lang.ModEditor.Active;
            btnMoveUp.ToolTip = Lang.ModEditor.TooltipMoveUp;
            btnMoveDown.ToolTip = Lang.ModEditor.TooltipMoveDown;
            btnCheckCompatibility.ToolTip = Lang.ModEditor.TooltipCheckCompatibility;
            //btnActivateAll.ToolTip = Lang.ModEditor.TooltipActivateAll;
            //btnDeactivateAll.ToolTip = Lang.ModEditor.TooltipDeactivateAll;
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
            header = new GridViewColumnHeader() { Content = "✔" }; // Lang.ModEditor.Installed
            header.Click += OnClickCatalogHeader;
            colCatalogInstalled.Header = header;
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

        private const String INI_PATH = "./Memoria.ini";
        private const String CATALOG_PATH = "./ModCatalog.xml";
        private const String CATALOG_URL = "https://raw.githubusercontent.com/Albeoris/Memoria/main/Memoria.Launcher/Catalogs/MemoriaCatalog.xml";
    }
}
