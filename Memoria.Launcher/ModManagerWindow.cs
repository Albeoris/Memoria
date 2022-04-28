using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using Ini;
using ListView = System.Windows.Controls.ListView;
using MessageBox = System.Windows.Forms.MessageBox;
using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public partial class ModManagerWindow : Window, IComponentConnector
    {
        public ObservableCollection<Mod> modListInstalled = new ObservableCollection<Mod>();
        public ObservableCollection<Mod> modListCatalog = new ObservableCollection<Mod>();
        public ObservableCollection<Mod> downloadList = new ObservableCollection<Mod>();
        public String StatusMessage = "";

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
        }

        private void OnClosing(Object sender, CancelEventArgs e)
        {
            if (downloadList.Count > 0 || downloadingMod !=null)
			{
                e.Cancel = true;
                MessageBox.Show($"Do NOT close this window while downloads are on their way.", "Error", MessageBoxButtons.OK);
                return;
			}
            UpdateSettings();
            ((MainWindow)this.Owner).ModdingWindow = null;
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
        }
        private void OnModListDoubleClick(Object sender, RoutedEventArgs e)
        {
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
                downloadList.Add(mod);
                DownloadStart(mod);
                mod.Installed = "...";
            }
            lstCatalogMods.Items.Refresh();
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

        private void DownloadStart(Mod mod)
        {
            if (downloadingMod != null)
                return;
            downloadingMod = mod;
            downloadBytesTime = DateTime.Now;
            downloadThread = new Thread(() =>
            {
                Directory.CreateDirectory(Mod.INSTALLATION_TMP);
                downloadingPath = Mod.INSTALLATION_TMP + "/" + (mod.InstallationPath ?? mod.Name) + ".zip";
                downloadClient = new WebClient();
                downloadClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadLoop);
                downloadClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadEnd);
                downloadClient.DownloadFileAsync(new Uri(mod.DownloadUrl), downloadingPath);
            });
            downloadThread.Start();
        }
        private void DownloadLoop(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                Double timeSpan = (DateTime.Now - downloadBytesTime).TotalSeconds;
                if (timeSpan <= 0.0)
                    timeSpan = 0.1;
                downloadingMod.PercentComplete = e.ProgressPercentage;
                downloadingMod.DownloadSpeed = $"{(Int64)(e.BytesReceived / 1024.0 / timeSpan)} {Lang.Measurement.KByteAbbr}/{Lang.Measurement.SecondAbbr}";
                downloadingMod.RemainingTime = $"{TimeSpan.FromSeconds((e.TotalBytesToReceive - e.BytesReceived) * timeSpan / e.BytesReceived):g}";
                lstDownloads.Items.Refresh();
            });
        }
        private void DownloadEnd(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
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
                String path = Mod.INSTALLATION_TMP + "/" + (downloadingMod.InstallationPath ?? downloadingMod.Name);
                if (String.IsNullOrEmpty(downloadingMod.DownloadFormat) || downloadingMod.DownloadFormat == "Zip")
                {
                    Directory.CreateDirectory(path);
                    try
                    {
                        Boolean proceedNext = false;
                        Boolean hasDesc = false;
                        Boolean moveDesc = false;
                        String sourcePath = "";
                        String destPath = "";
                        ZipFile.ExtractToDirectory(path + ".zip", path);
                        File.Delete(path + ".zip");
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
                            else if (Directory.Exists(path + "/" + (downloadingMod.InstallationPath ?? downloadingMod.Name)))
                            {
                                sourcePath = path + "/" + (downloadingMod.InstallationPath ?? downloadingMod.Name);
                                destPath = downloadingMod.InstallationPath ?? downloadingMod.Name;
                                proceedNext = true;
                                moveDesc = true;
                            }
                            else if (Mod.LooksLikeAModFolder(path))
                            {
                                sourcePath = path;
                                destPath = downloadingMod.InstallationPath ?? downloadingMod.Name;
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
                            if (Directory.Exists(path + "/" + (downloadingMod.InstallationPath ?? downloadingMod.Name)))
                            {
                                sourcePath = path + "/" + downloadingMod.InstallationPath;
                                destPath = downloadingMod.InstallationPath ?? downloadingMod.Name;
                                proceedNext = true;
                            }
                            else if (Mod.LooksLikeAModFolder(path))
                            {
                                sourcePath = path;
                                destPath = downloadingMod.InstallationPath ?? downloadingMod.Name;
                                proceedNext = true;
                            }
                            else
							{
                                String[] subDirectories = Directory.GetDirectories(path);
                                foreach (String sd in subDirectories)
                                    if (Mod.LooksLikeAModFolder(sd))
                                    {
                                        sourcePath = sd;
                                        destPath = downloadingMod.InstallationPath ?? downloadingMod.Name;
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
                                if (!Directory.EnumerateFileSystemEntries(Mod.INSTALLATION_TMP).GetEnumerator().MoveNext())
                                    Directory.Delete(Mod.INSTALLATION_TMP);
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
                    String destPath = (downloadingMod.InstallationPath ?? downloadingMod.Name) + "/" + downloadingMod.DownloadFormat.Substring("SingleFileWithPath:".Length);
                    Directory.CreateDirectory(destPath.Substring(0, destPath.LastIndexOf('/')));
                    File.Move(path + ".zip", destPath);
                    downloadingMod.GenerateDescription(downloadingMod.InstallationPath ?? downloadingMod.Name);
                    if (!Directory.EnumerateFileSystemEntries(Mod.INSTALLATION_TMP).GetEnumerator().MoveNext())
                        Directory.Delete(Mod.INSTALLATION_TMP);
                }
                downloadList.Remove(downloadingMod);
                downloadingMod = null;
                if (downloadList.Count > 0)
                    DownloadStart(downloadList[0]);
                CheckForValidModFolder();
                UpdateCatalogInstallationState();
            });
        }

        private void UpdateCatalog()
        {
            // TODO: fetch the catalog online
            // Also, TehMighty's Scaled UI is painfully missing; it can't be implemented only with a mod folder for now
            modListCatalog.Clear();
            modListCatalog.Add(new Mod()
            {
                Name = "Moguri Mod",
                CurrentVersion = new Version(8, 3, 0, 0),
                InstallationPath = "MoguriFiles",
                Author = "Ze_PilOt / Snouz",
                Description = "Moguri Mod is a faithful revamp of the PC version of Final Fantasy IX helped by deep learning techniques (ESRGAN). The most important changes are in the background arts, that are now cleaner, more detailed and higher resolution.\n\n" +
                    "Main Features:\n" +
                    "- HD backgrounds (aided by AI and polished by hand)\n" +
                    "- Manual redraw of all 11k layer edges and area names\n" +
                    "- HD textures (worldmap, NPC, battles...)\n" +
                    "- Many bugfixes\n" +
                    "...And much more!\n\n" +
                    "Note: this mod doesn't support automatic installation yet. Download it from its website, install it, and then update Memoria. Also, it is usually better to place it at the bottom of the list of installed mods for compatibility purpose.",
                Category = "Visual",
                Website = "https://sites.google.com/view/moguri-mod",
                //DownloadUrl = "https://www.moddb.com/downloads/mirror/216757/122/3381820ec0869d559baf7ca59abcc388"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "Alternate Fantasy",
                CurrentVersion = new Version(5,2),
                InstallationPath = "AlternateFantasy",
                Author = "Tirlititi",
                Description = "A gameplay mod.",
                Category = "Gameplay",
                Website = "https://forums.qhimm.com/index.php?topic=16324.0",
                DownloadUrl = "https://www.hiveworkshop.com/attachments/alternatefantasy_v5-2-zip.384576/"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "Beatrix Mod",
                CurrentVersion = new Version(5, 2),
                InstallationPath = "BeatrixMod",
                Author = "Tirlititi",
                Description = "Allow the optional recruitment of Beatrix as a permanent character.\n" +
                        "She must be met in Alexandria once the Hilda Gard III is obtained and before moving to Terra.\n\n" +
                        "Alternate Fantasy contains it already so do not use them both at the same time.",
                Category = "Gameplay",
                Website = "https://forums.qhimm.com/index.php?topic=16324.0",
                DownloadUrl = "https://www.hiveworkshop.com/attachments/afbeatrixonly-zip.384575/"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "PSX Buttons",
                InstallationPath = "PSXButtons",
                Author = "gdi",
                Description = "Use the Playstation button icons instead of the Xbox button icons.",
                Category = "Visual",
                Website = "https://steamcommunity.com/groups/ff-modding/discussions/13/350533172685612333/",
                DownloadUrl = "https://www.dropbox.com/s/815zeapy32xp821/PC_PSXButtons.zip?dl=1"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "No Cutscene",
                CurrentVersion = new Version(0, 5),
                InstallationPath = "NoCutsceneMemoria",
                Author = "Tirlititi",
                Description = "Removes most of cutscenes from the game for speedrunning purpose.",
                Category = "Story",
                DownloadUrl = "https://www.dropbox.com/s/uczf83l2a81jma2/PC_NoCutscene.zip?dl=1"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "Play as Kuja",
                CurrentVersion = new Version(0, 1),
                InstallationPath = "PlayAsKuja",
                Author = "Tirlititi",
                Description = "Turn Zidane's 3D model on the field and in battles into Kuja's 3D model.\n" +
                    "The animations on the field are messed up.",
                Category = "Visual",
                Website = "https://steamcommunity.com/app/377840/discussions/0/4472613273101569368/",
                DownloadUrl = "https://www.dropbox.com/s/y3dw2gxfdw5kkl8/PC_PlayAsKuja.zip?dl=1"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "Garnet is Main Character",
                InstallationPath = "GarnetIsMainCharacter",
                Author = "piano221",
                Description = "This mod allow you to change Zidane for Garnet as the field main character.",
                Category = "Visual",
                Website = "https://forums.qhimm.com/index.php?topic=20605.0",
                DownloadUrl = "https://download944.mediafire.com/ud6vuwre8sag/10illq7tkuxixhb/p0data7.bin",
                DownloadFormat = "SingleFileWithPath:StreamingAssets/p0data7.bin"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "David Bowie Edition",
                InstallationPath = "DavidBowie",
                Author = "Clem Fandango",
                Description = "Final Fantasy IX: David Bowie Edition (DBE) is a gameplay mod that is meant to function as a rebalance, or better yet a remix of the original game. This mod includes new skills, items, enemy behavior, Chocograph rewards and a number of smaller tweaks.",
                Category = "Gameplay",
                Website = "https://forums.qhimm.com/index.php?topic=19499.0",
                DownloadUrl = "https://www.dropbox.com/s/k5o64e3g8iyah63/FF9%20DBE%20v1.16%20Memoria.zip?dl=1"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "High-Res Chocographs",
                InstallationPath = "HDChocographs",
                Author = "Caledor",
                Description = "Chocographs remade from scratch by taking actual screenshots in FF9 Steam modded with Moguri Mod 8.2.\n" +
                    "(English layout only)",
                Category = "Visual",
                Website = "https://forums.qhimm.com/index.php?topic=20657.0",
                DownloadUrl = "https://i.imgur.com/tpdfrUQ.png",
                DownloadFormat = "SingleFileWithPath:FF9_Data/EmbeddedAsset/UI/Atlas/Chocograph Atlas"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "Tweaked Portraits",
                InstallationPath = "TweakedPortraits",
                Author = "Lykon",
                Description = "This is a simple mod that slightly tweaks the portrait shown in the main menu.",
                Category = "Visual",
                Website = "https://forums.qhimm.com/index.php?topic=19964.0",
                DownloadUrl = "https://i.imgur.com/MkJA680.png",
                DownloadFormat = "SingleFileWithPath:FF9_Data/EmbeddedAsset/UI/Atlas/Face Atlas"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "Trance Seek",
                CurrentVersion = new Version(0, 2, 11),
                InstallationPath = "TranceSeek",
                Author = "DVlad666",
                Description = "A gameplay mod.\n\n" +
                    "Note: this mod doesn't support automatic installation yet. Download it from its website.",
                Category = "Gameplay",
                Website = "https://steamcommunity.com/app/377840/discussions/0/5362100202713255072/",
                //DownloadUrl = "https://drive.google.com/file/d/1ONGayLWtVctUgM33aYWVvzSGqDBEsOIE"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "Playstation Sounds",
                CurrentVersion = new Version(1, 6),
                InstallationPath = "PlaystationSounds",
                Author = "DVlad666",
                Description = "Replace the sounds by the PSX sounds.\n\n" +
                    "Note: this mod doesn't support automatic installation yet. Download it from its website.",
                Category = "Audio",
                Website = "https://steamcommunity.com/app/377840/discussions/0/3189117724409401717/",
                //DownloadUrl = "https://drive.google.com/file/d/1RLcIQ9M6AB19IPGDXskoBGcC3siVbLQQ"
            });
            // TODO: manage translation mods; many download links should be changed + several ones use the "[Import] Text" feature of Memoria instead of "[Mod] FolderNames"
            modListCatalog.Add(new Mod()
            {
                Name = "Brazilian-Portuguese Translation",
                InstallationPath = "BrazilianTranslation",
                Author = "P.O.B.R.E. / Brigandier",
                Description = "Tradução do jogo em português do Brasil.\n\n" +
                    "Note: this mod doesn't support automatic installation yet. Download it from its website.",
                Category = "Translation",
                Website = "https://steamcommunity.com/sharedfiles/filedetails/?id=2111796362"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "Polish Translation",
                InstallationPath = "PolishTranslation",
                Author = "Grupa CosmoFF (w składzie Em i Robin)",
                Description = "Tłumaczenie gry na język polski.\n\n" +
                    "Note: this mod doesn't support automatic installation yet. Download it from its website.",
                Category = "Translation",
                Website = "https://steamcommunity.com/sharedfiles/filedetails/?id=2621590946"
            });
            modListCatalog.Add(new Mod()
            {
                Name = "Russian Translation",
                InstallationPath = "RussianTranslation",
                Author = "RGR Studio", // Note to Albeoris: you know better about the genesis of it
                Description = "Перевод игры на русский язык.\n\n" +
                    "Note: this mod doesn't support automatic installation yet. Download it from its website.",
                Category = "Translation",
                Website = "https://ff9.ffrtt.ru/viewtopic.php?f=14&t=38"
            });
            // This one doesn't seem to have a valid download link anymore
            //modListCatalog.Add(new Mod()
            //{
            //    Name = "Chinese Translation",
            //    InstallationPath = "ChineseTranslation",
            //    Description = "把游戏翻译成中文.\n\n" +
            //        "Note: this mod doesn't support automatic installation yet. Download it from its website.",
            //    Category = "Translation",
            //    Website = "https://steamcommunity.com/sharedfiles/filedetails/?id=2797209969"
            //});
        }

        private void UpdateModListInstalled()
        {
            foreach (String dir in Directory.EnumerateDirectories("."))
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
                        Int32 index = modListInstalled.IndexOf(previousMod);
                        modListInstalled.RemoveAt(index);
                        modListInstalled.Insert(index, updatedMod);
                    }
                }
            UpdateInstalledPriorityValue();
        }

        private Boolean GenerateAutomaticDescriptionFile(String folderName)
        {
            if (!Directory.Exists(folderName) || File.Exists(folderName + "/" + Mod.DESCRIPTION_FILE))
                return false;
            if (folderName == "MoguriFiles" && !File.Exists("MoguriFiles/" + Mod.DESCRIPTION_FILE) && Mod.SearchWithName(modListCatalog, "Moguri Mod") != null)
            {
                Mod.SearchWithName(modListCatalog, "Moguri Mod").GenerateDescription(folderName);
                return true;
            }
            String name = null;
            if (folderName == "MoguriFiles")
                name = "Moguri Mod";
            else if (folderName == "MoguriSoundtrack")
                name = "Moguri Soundtrack";
            else if (folderName == "MoguriVideo")
                name = "Moguri Video";
            File.WriteAllText(folderName + "/" + Mod.DESCRIPTION_FILE,
                "<Mod>\n" +
                "    <Name>" + (name ?? folderName) + "</Name>\n" +
                "    <InstallationPath>" + folderName + "</InstallationPath>\n" +
                "    <Category>Unknown</Category>\n" +
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
            }
            else
            {
                PreviewModName.Text = mod.Name;
                PreviewModVersion.Text = mod.CurrentVersion?.ToString() ?? "Unknown version";
                PreviewModRelease.Text = mod.ReleaseDate ?? "Unknown date";
                PreviewModAuthor.Text = mod.Author ?? "Unknown author";
                PreviewModDescription.Text = mod.Description ?? "No description.";
                PreviewModReleaseNotes.Text = mod.PatchNotes ?? "";
                PreviewModCategory.Text = mod.Category ?? "Unknown";
                PreviewModWebsite.ToolTip = mod.Website ?? String.Empty;
                PreviewModWebsite.IsEnabled = !String.IsNullOrEmpty(mod.Website);
            }
        }

        private void UpdateCatalogInstallationState()
		{
            foreach (Mod mod in modListCatalog)
			{
                if (Mod.SearchWithName(downloadList, mod.Name) != null)
                    mod.Installed = "...";
                else if (Mod.SearchWithName(modListInstalled, mod.Name) != null)
                    mod.Installed = "✔";
                else
                    mod.Installed = "✘";
            }
            lstCatalogMods.Items.Refresh();
        }

        private void UpdateInstalledPriorityValue()
        {
            for (Int32 i = 0; i < modListInstalled.Count; i++)
                modListInstalled[i].Priority = i + 1;
            lstMods.Items.Refresh();
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
                str.Trim();
                str.Trim('"');
                String[] iniModActiveList = Regex.Split(str, "\",\\s*\"");
                str = iniFile.ReadValue("Mod", "Priorities");
                if (String.IsNullOrEmpty(str))
                    str = "";
                str.Trim('"');
                String[] iniModPriorityList = Regex.Split(str, "\",\\s*\"");
                String[][] listCouple = new String[][] { iniModPriorityList, iniModActiveList };
                for (Int32 listI = 0; listI < 2; ++listI)
                {
                    for (Int32 i = 0; i < listCouple[listI].Length; ++i)
                    {
                        if (String.IsNullOrEmpty(listCouple[listI][i]))
                            continue;
                        Mod mod = Mod.SearchWithPath(modListInstalled, listCouple[listI][i]);
                        if (mod != null)
                        {
                            if (listI == 1)
                                mod.IsActive = true;
                            continue;
                        }
                        mod = Mod.SearchWithPath(modListCatalog, listCouple[listI][i]);
                        if (mod == null)
                        {
                            GenerateAutomaticDescriptionFile(listCouple[listI][i]);
                            if (File.Exists(listCouple[listI][i] + "/" + Mod.DESCRIPTION_FILE))
                                mod = new Mod(listCouple[listI][i]);
                            else
                                mod = new Mod(listCouple[listI][i], listCouple[listI][i]);
                        }
                        if (listI == 1)
                            mod.IsActive = true;
                        modListInstalled.Add(mod);
                    }
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
                for (Int32 i = 0; i < modListInstalled.Count; ++i)
				{
                    if (modListInstalled[i].IsActive)
                        iniModActiveList.Add(modListInstalled[i].InstallationPath);
                    iniModPriorityList.Add(modListInstalled[i].InstallationPath);
                }
                IniFile iniFile = new IniFile(INI_PATH);
                iniFile.WriteValue("Mod", "FolderNames", iniModActiveList.Count > 0 ? " \"" + String.Join("\", \"", iniModActiveList) + "\"" : "");
                iniFile.WriteValue("Mod", "Priorities", iniModPriorityList.Count > 0 ? " \"" + String.Join("\", \"", iniModPriorityList) + "\"" : "");
            }
            catch (Exception ex) { UiHelper.ShowError(Application.Current.MainWindow, ex); }
        }

        private void SetupFrameLang()
		{
            Title = Lang.ModEditor.WindowTitle;
            GroupModInfo.Header = Lang.ModEditor.ModInfos;
            PreviewModWebsite.Content = Lang.ModEditor.Website;
            CaptionModName.Text = Lang.ModEditor.Name + ":";
            CaptionModAuthor.Text = Lang.ModEditor.Author + ":";
            CaptionModRelease.Text = Lang.ModEditor.Release + ":";
            CaptionModCategory.Text = Lang.ModEditor.Category + ":";
            CaptionModVersion.Text = Lang.ModEditor.Version + ":";
            CaptionModDescription.Text = Lang.ModEditor.Description + ":";
            CaptionModReleaseNotes.Text = Lang.ModEditor.ReleaseNotes + ":";
            CaptionPreviewImageMissing.Text = Lang.ModEditor.PreviewImageMissing;
            tabMyMods.Text = Lang.ModEditor.TabMyMods;
            colMyModsPriority.Header = Lang.ModEditor.Priority;
            colMyModsName.Header = Lang.ModEditor.Name;
            colMyModsAuthor.Header = Lang.ModEditor.Author;
            colMyModsCategory.Header = Lang.ModEditor.Category;
            colMyModsActive.Header = Lang.ModEditor.Active;
            btnMoveUp.ToolTip = Lang.ModEditor.TooltipMoveUp;
            btnMoveDown.ToolTip = Lang.ModEditor.TooltipMoveDown;
            btnActivateAll.ToolTip = Lang.ModEditor.TooltipActivateAll;
            btnDeactivateAll.ToolTip = Lang.ModEditor.TooltipDeactivateAll;
            btnUninstall.ToolTip = Lang.ModEditor.TooltipUninstall;
            tabCatalog.Text = Lang.ModEditor.TabCatalog;
            colCatalogName.Header = Lang.ModEditor.Name;
            colCatalogAuthor.Header = Lang.ModEditor.Author;
            colCatalogCategory.Header = Lang.ModEditor.Category;
            colCatalogInstalled.Header = Lang.ModEditor.Installed;
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
        private WebClient downloadClient;

        private const String INI_PATH = "./Memoria.ini";
    }
}
