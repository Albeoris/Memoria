using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Data;
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

        public ObservableCollection<Mod> ModListInstalled { get; } = new ObservableCollection<Mod>();
        public ObservableCollection<Mod> ModListCatalog { get; private set; } = new ObservableCollection<Mod>();
        public ObservableCollection<Mod> DownloadList { get; } = new ObservableCollection<Mod>();
        public String StatusMessage = "";

        public static String[] SupportedArchives { get; } = [".rar", ".unrar", ".zip", ".bzip2", ".gzip", ".tar", ".7z", ".lzip", ".gz"];

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
            try
            {
                IniFile.PreventWrite = true;
                AreThereModUpdates = false;
                AreThereModIncompatibilies = false;

                foreach (Mod mod in ModListInstalled)
                {
                    if (mod == null || mod.Name == null)
                        continue;

                    // reset state
                    mod.UpdateIcon = null;
                    mod.UpdateTooltip = null;
                    mod.IncompIcon = null;
                    mod.ActiveIncompatibleMods = null;

                    if (((mod.Name == "Moguri Mod" || mod.Name == "MoguriFiles") && mod.InstallationPath.Contains("MoguriFiles")) || (mod.Name == "Moguri - 3D textures" && mod.InstallationPath.Contains("Moguri_3Dtextures")))
                    {
                        mod.UpdateIcon = UpdateEmoji;
                        mod.CurrentVersion = Version.Parse("8.3");
                        AreThereModUpdates = true;
                        mod.Description = "Please download the latest Moguri Mod from the catalog and disable/remove this one";
                        mod.UpdateTooltip = "Please download the latest Moguri Mod from the catalog and disable/remove this one";
                    }

                    foreach (Mod catalog_mod in ModListCatalog) // check updates
                    {
                        if (catalog_mod != null && catalog_mod.Name != null && mod.Name == catalog_mod.Name)
                        {
                            mod.IsOutdated = catalog_mod.IsOutdated = mod.CurrentVersion < catalog_mod.CurrentVersion;
                            if (mod.IsOutdated)
                            {
                                mod.UpdateIcon = UpdateEmoji;
                                mod.UpdateTooltip = (String)Lang.Res["ModEditor.UpdateTooltip"] + catalog_mod.CurrentVersion;
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
                            foreach (Mod other_mod in ModListInstalled)
                            {
                                if (other_mod == null || other_mod.Name == null)
                                    continue;
                                if (other_mod.Name == incompName && other_mod.IsActive)
                                {
                                    mod.IncompIcon = ConflictEmoji;
                                    other_mod.IncompIcon = ConflictEmoji;

                                    if (mod.ActiveIncompatibleMods == null)
                                        mod.ActiveIncompatibleMods = (String)Lang.Res["ModEditor.ActiveIncompatibleMods"] + other_mod.Name;
                                    else
                                        mod.ActiveIncompatibleMods += ", " + other_mod.Name;

                                    if (other_mod.ActiveIncompatibleMods == null)
                                        other_mod.ActiveIncompatibleMods = (String)Lang.Res["ModEditor.ActiveIncompatibleMods"] + mod.Name;
                                    else
                                        other_mod.ActiveIncompatibleMods += ", " + mod.Name;

                                    AreThereModIncompatibilies = true;
                                }
                            }
                        }
                    }

                    AutoActivateSubMods(mod);

                    String ver = mod.CurrentVersion?.ToString() ?? "0";
                    foreach (String outdated in OutdatedModsVersions) // Memoria compatibility
                    {
                        if (outdated == mod.Name + "_" + ver)
                        {
                            mod.IncompIcon = ConflictEmoji;
                            if (mod.ActiveIncompatibleMods == null)
                                mod.ActiveIncompatibleMods = (String)Lang.Res["ModEditor.IncompatibleWithMemoria"];
                            else
                                mod.ActiveIncompatibleMods += "\n\n" + (String)Lang.Res["ModEditor.IncompatibleWithMemoria"];
                            AreThereModIncompatibilies = true;
                        }
                    }
                }

                colMyModsIcons.Width = colMyModsIcons.ActualWidth;
                colMyModsIcons.Width = double.NaN;
                lstMods.Items.Refresh();
                UpdateLauncherTheme();
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

        private void AutoActivateSubMods(Mod mod)
        {
            foreach (Mod subMod in mod.SubMod) // submod auto (de)activate based on others
            {
                if (subMod == null || subMod.Name == null)
                    continue;

                Int32 modFoundActive = 0;

                foreach (Mod other_mod in ModListInstalled)
                {
                    if (other_mod == null || other_mod.Name == null)
                        continue;

                    if (other_mod.IsActive && ((subMod.ActivateWithoutMod?.Contains(other_mod.Name) ?? false) || (subMod.ActivateWithMod?.Contains(other_mod.Name) ?? false)))
                        modFoundActive++;
                }

                if (subMod.ActivateWithMod != null)
                    subMod.IsActive = modFoundActive == subMod.ActivateWithMod.Count;
                if (subMod.ActivateWithoutMod != null)
                    subMod.IsActive = modFoundActive == 0;
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            Mod mod = (sender as CheckBox)?.DataContext as Mod;
            if (mod != null && mod.IsActive)
                mod.TryApplyPreset();
            CheckOutdatedAndIncompatibleMods();
            UpdateModSettings();
            RefreshModOptions();
        }

        private void UpdateIcon_Click(object sender, RoutedEventArgs e)
        {
            Mod installedMod = (sender as Button)?.DataContext as Mod;
            if (installedMod == null || installedMod.Name == null)
                return;

            Mod mod = Mod.SearchMod(ModListCatalog, installedMod);
            if (mod != null)
            {
                tabCtrlMain.SelectedIndex = 1;
                DownloadList.Add(mod);
                DownloadStart(mod);
                mod.Installed = WaitingEmoji;
                lstCatalogMods.SelectedItem = mod;
                lstCatalogMods.ScrollIntoView(mod);
            }
        }

        //---------------------------
        // ListView Drag & Drop
        //---------------------------

        private Point _startPoint;
        private bool _isDragging = false;
        private ListViewItem _draggedItem;
        private int _draggedIndex = -1;
        private Mod _draggedData;

        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (tabCtrlMain.SelectedIndex == 1 && priorityColumn == null) return;
            ObservableCollection<Mod> mods = tabCtrlMain.SelectedIndex == 0 ? ModListInstalled : ModListCatalog;

            _startPoint = e.GetPosition(null);

            // Find the ListViewItem that was clicked
            var item = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
            if (item != null)
            {
                _draggedItem = item;
                _draggedData = item.Content as Mod;
                _draggedIndex = mods.IndexOf(_draggedData);

                // Change cursor to indicate vertical movement
                var listView = FindAncestor<ListView>(_draggedItem);
                listView.Cursor = Cursors.SizeAll;
            }
        }

        private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (tabCtrlMain.SelectedIndex == 1 && priorityColumn == null) return;
            if (e.LeftButton == MouseButtonState.Pressed && !_isDragging && _draggedItem != null)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    StartDrag();
                }
            }
        }

        private void StartDrag()
        {
            if (tabCtrlMain.SelectedIndex == 1 && priorityColumn == null) return;
            if (_draggedItem != null && _draggedData != null)
            {
                _isDragging = true;

                // Capture mouse to continue receiving events even when outside the control
                var listView = FindAncestor<ListView>(_draggedItem);
                listView?.CaptureMouse();
            }
        }

        private void ListView_MouseMove(object sender, MouseEventArgs e)
        {
            if (tabCtrlMain.SelectedIndex == 1 && priorityColumn == null) return;
            ObservableCollection<Mod> mods = tabCtrlMain.SelectedIndex == 0 ? ModListInstalled : ModListCatalog;

            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                var listView = sender as ListView;
                Point position = e.GetPosition(listView);

                // Find which item we're hovering over
                var hitTest = VisualTreeHelper.HitTest(listView, position);
                if (hitTest != null)
                {
                    var targetItem = FindAncestor<ListViewItem>(hitTest.VisualHit);
                    if (targetItem != null && targetItem != _draggedItem)
                    {
                        var targetData = targetItem.Content as Mod;
                        if (targetData != null)
                        {
                            int newIndex = mods.IndexOf(targetData);
                            int currentIndex = mods.IndexOf(_draggedData);

                            if (newIndex >= 0 && currentIndex >= 0 && newIndex != currentIndex)
                            {
                                // Move the item in real-time
                                mods.Move(currentIndex, newIndex);
                            }
                        }
                    }
                }
            }
        }

        private void ListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EndDrag(sender as ListView);
        }

        private void ListView_MouseLeave(object sender, MouseEventArgs e)
        {
            EndDrag(sender as ListView);
        }

        private void EndDrag(ListView listView)
        {
            if (tabCtrlMain.SelectedIndex == 1 && priorityColumn == null) return;
            ObservableCollection<Mod> mods = tabCtrlMain.SelectedIndex == 0 ? ModListInstalled : ModListCatalog;

            if (_isDragging)
            {
                _isDragging = false;

                // Release mouse capture
                listView?.ReleaseMouseCapture();

                // Clear drag state
                _draggedItem = null;
                _draggedData = null;
                _draggedIndex = -1;

                if (tabCtrlMain.SelectedIndex == 0)
                    UpdateInstalledPriorityValue();
                else
                {
                    UpdateCatalogPriorities();
                    SaveCatalogPriorities();
                }
            }

            // Restore cursor
            if (listView != null)
                listView.Cursor = Cursors.Arrow;
        }

        // Helper method to find ancestor of specific type
        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        //---------------------------

        private void OnClosing(Object sender, CancelEventArgs e)
        {
            if (DownloadList.Count > 0 || downloadingMod != null)
            {
                e.Cancel = true;
                // TODO language:
                MessageBox.Show($"If you close this window while downloads are on their way, they will be cancelled.", "Warning", MessageBoxButton.OK);
                return;
            }
            if (downloadCatalogClient != null && downloadCatalogClient.IsBusy)
                downloadCatalogClient.CancelAsync();
        }


        private void Uninstall()
        {
            List<Mod> toRemove = new List<Mod>();
            foreach (Mod mod in lstMods.SelectedItems)
            {
                if (Directory.Exists(mod.InstallationPath))
                    // TODO language:
                    if (MessageBox.Show($"The mod folder {mod.InstallationPath} will be deleted.\nProceed?", "Updating", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Directory.Delete(mod.InstallationPath, true);
                        toRemove.Add(mod);
                    }
            }
            foreach (Mod mod in toRemove)
            {
                ModListInstalled.Remove(mod);
            }
            UpdateInstalledPriorityValue();
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
            foreach (Mod mod in ModListInstalled)
            {
                if (mod == null || mod.Name == null)
                    continue;
                foreach (Mod catalog_mod in ModListCatalog)
                {
                    if (catalog_mod == null || catalog_mod.Name == null)
                        continue;
                    if (mod.Name == catalog_mod.Name)
                    {
                        mod.Priority = catalog_mod.Priority;
                    }
                }
            }
            List<Mod> orderedMods = ModListInstalled.OrderByDescending(mod => mod.Priority).ToList();
            ModListInstalled.Clear();
            foreach (Mod mod in orderedMods)
                ModListInstalled.Add(mod);

            UpdateInstalledPriorityValue();
        }

        private void OnClickMoveUp(Object sender, RoutedEventArgs e)
        {
            if (lstMods.SelectedIndex > 0)
            {
                Int32 sel = lstMods.SelectedIndex;
                Mod i1 = ModListInstalled[sel];
                Mod i2 = ModListInstalled[sel - 1];
                ModListInstalled.Remove(i1);
                ModListInstalled.Remove(i2);
                ModListInstalled.Insert(sel - 1, i1);
                ModListInstalled.Insert(sel, i2);
                lstMods.SelectedItem = i1;
                UpdateInstalledPriorityValue();
            }
        }
        private void OnClickSendTop(Object sender, RoutedEventArgs e)
        {
            if (lstMods.SelectedIndex > 0)
            {
                Mod i1 = ModListInstalled[lstMods.SelectedIndex];
                ModListInstalled.Remove(i1);
                ModListInstalled.Insert(0, i1);
                lstMods.SelectedItem = i1;
                UpdateInstalledPriorityValue();
            }
        }
        private void OnClickMoveDown(Object sender, RoutedEventArgs e)
        {
            if (lstMods.SelectedIndex >= 0 && lstMods.SelectedIndex + 1 < lstMods.Items.Count)
            {
                Int32 sel = lstMods.SelectedIndex;
                Mod i1 = ModListInstalled[sel];
                Mod i2 = ModListInstalled[sel + 1];
                ModListInstalled.Remove(i1);
                ModListInstalled.Remove(i2);
                ModListInstalled.Insert(sel, i2);
                ModListInstalled.Insert(sel + 1, i1);
                lstMods.SelectedItem = i1;
                UpdateInstalledPriorityValue();
            }
        }
        private void OnClickSendBottom(Object sender, RoutedEventArgs e)
        {
            if (lstMods.SelectedIndex >= 0 && lstMods.SelectedIndex + 1 < lstMods.Items.Count)
            {
                Mod i1 = ModListInstalled[lstMods.SelectedIndex];
                ModListInstalled.Remove(i1);
                ModListInstalled.Insert(ModListInstalled.Count, i1);
                lstMods.SelectedItem = i1;
                UpdateInstalledPriorityValue();
            }
        }
        private void OnClickDownload(Object sender, RoutedEventArgs e)
        {
            foreach (Mod mod in lstCatalogMods.SelectedItems)
            {
                if (DownloadList.Contains(mod) || String.IsNullOrEmpty(mod.DownloadUrl))
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
                DownloadList.Add(mod);
                DownloadStart(mod);
                mod.Installed = WaitingEmoji;
            }
            lstCatalogMods.Items.Refresh();
            CheckOutdatedAndIncompatibleMods();
        }

        private GridViewColumn priorityColumn = null;

        private void OnClickPriority(Object sender, RoutedEventArgs e)
        {
            if (priorityColumn != null) return;

            GridView gridView = lstCatalogMods.View as GridView;

            priorityColumn = new GridViewColumn();
            priorityColumn.Header = "";
            priorityColumn.DisplayMemberBinding = new Binding("Priority");

            // Insert at index 0 to make it the first column
            gridView.Columns.Insert(0, priorityColumn);

            priorityColumn.Width = Double.NaN;
            lstCatalogMods.UpdateLayout();
            priorityColumn.Width = priorityColumn.ActualWidth;

            var sortedMods = ModListCatalog.OrderByDescending((mod) => mod.Priority).ToList();
            ModListCatalog.Clear();
            foreach (var mod in sortedMods)
            {
                ModListCatalog.Add(mod);
                if (mod.Priority == 1)
                {
                    Mod separator = new Mod();
                    separator.Name = "---------------------------";
                    ModListCatalog.Add(separator);
                }
            }
            UpdateCatalogPriorities();
        }

        private void UpdateCatalogPriorities()
        {
            Int32 count;
            for (count = 0; count < ModListCatalog.Count; count++)
            {
                if (ModListCatalog[count].Name == "---------------------------")
                    break;
            }
            for (Int32 i = 0; i < ModListCatalog.Count; i++)
            {
                ModListCatalog[i].Priority = count--;
            }
            CollectionViewSource.GetDefaultView(lstCatalogMods.ItemsSource).Refresh();
        }

        private void SaveCatalogPriorities()
        {
            String catalog = File.ReadAllText(CATALOG_PATH); ;
            Dictionary<String, Int32> priorities = new Dictionary<String, Int32>();

            foreach (Mod mod in ModListCatalog)
            {
                String search1 = @"(<Name>" + Regex.Escape(mod.Name) + @"((?!<\/Mod>).)*<\/Version>\s*<Priority>)((?!<\/Priority>).)*";
                String search2 = @"(<Name>" + Regex.Escape(mod.Name) + @"((?!<\/Mod>).)*<\/Version>)(?!\s*<Priority>)";
                Match m = Regex.Match(catalog, search2, RegexOptions.Singleline);
                catalog = Regex.Replace(catalog, search1, "${1}" + mod.Priority, RegexOptions.Singleline);
                catalog = Regex.Replace(catalog, search2, $"$1\r\n\t<Priority>{mod.Priority}</Priority>", RegexOptions.Singleline);
            }

            File.WriteAllText(CATALOG_PATH, catalog);
        }

        public static bool IsDate1EqualOrMoreRecent(string date1, string date2)
        {
            string format = "yyyy-MM-dd";
            DateTime dateTime1 = DateTime.ParseExact(date1, format, CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(date2, format, CultureInfo.InvariantCulture);
            return dateTime1 >= dateTime2;
        }

        private void OnClickCancel(Object sender, RoutedEventArgs e)
        {
            if (downloadThread != null)
                downloadThread.Abort();
            if (downloadClient != null)
                downloadClient.CancelAsync();
            if (downloadingMod != null)
                DownloadList.Remove(downloadingMod);
            downloadingMod = null;
            if (DownloadList.Count > 0)
                DownloadStart(DownloadList[0]);
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
            DownloadList.Clear();
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
        private void OnClickDescription(Object sender, RoutedEventArgs e)
        {
            MainWindowGrid.Children.Add(new Window_ModDescription(currentMod));
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
            if (accessors != null && (Mod)ModListInstalled[0] != null)
            {
                Boolean isFirstModActive = ModListInstalled[0].IsActive;
                foreach (Mod mod in ModListInstalled)
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
                    Int64 dlSpeed = (Int64)(e.BytesReceived / 1024.0 / timeSpan);
                    String measurement = (String)Lang.Res["Measurement.KByteAbbr"];
                    if (dlSpeed > 1024)
                    {
                        dlSpeed /= 1024;
                        measurement = (String)Lang.Res["Measurement.MByteAbbr"];
                    }

                    downloadingMod.DownloadSpeed = $"{dlSpeed} {measurement}/{(String)Lang.Res["Measurement.SecondAbbr"]}";
                    downloadingMod.RemainingTime = $"{TimeSpan.FromSeconds(Math.Round((e.TotalBytesToReceive - e.BytesReceived) * timeSpan / e.BytesReceived)):g}";
                }
                catch (NullReferenceException) // added to catch a race condition that sometimes occures where Ui tries to update but download has finished
                {

                }
                lstDownloads.Items.Refresh();
            });
        }

        private Task ExtractAllFileFromArchive(String archivePath, String extractTo)
        {
            return Task.Run(() =>
            {
                Extractor.ExtractAllFileFromArchive(archivePath, extractTo, ExtractionCancellationToken.Token);
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

            Dispatcher.BeginInvoke((MethodInvoker)async delegate
            {
                Thread extractingThread = new Thread(() =>
                {
                    downloadingMod.PercentComplete = 0;
                    downloadingMod.DownloadSpeed = $"{WaitingEmoji} {(String)Lang.Res["ModEditor.Extracting"]}";
                    downloadingMod.RemainingTime = "";
                    String dots = "";
                    while (downloadingMod != null)
                    {
                        try
                        {
                            dots = dots == "..." ? "" : dots + ".";
                            Dispatcher.BeginInvoke((MethodInvoker)delegate
                            {
                                downloadingMod.RemainingTime = dots;
                                lstDownloads.Items.Refresh();
                            });
                            Thread.Sleep(500);
                        }
                        catch
                        {
                            if (downloadingMod != null)
                            {
                                downloadingMod.DownloadSpeed = "";
                                downloadingMod.RemainingTime = "";
                            }
                        }
                    }
                });

                String downloadingModName = downloadingMod.Name;
                String path = Mod.INSTALLATION_TMP + "/" + (downloadingMod.InstallationPath ?? downloadingModName);
                Boolean success = false;
                String downloadFormatExtLower = "." + (downloadingMod.DownloadFormat ?? "zip").ToLower();
                if (String.IsNullOrEmpty(downloadingMod.DownloadFormat) || SupportedArchives.Contains(downloadFormatExtLower))
                {
                    try
                    {
                        if (Directory.Exists(path)) Directory.Delete(path, true);
                        Directory.CreateDirectory(path);

                        Boolean proceedNext = false;
                        Boolean moveDesc = false;
                        String sourcePath = "";
                        String destPath = "";
                        extractingThread.Start();
                        await ExtractAllFileFromArchive(path + downloadFormatExtLower, path);
                        extractingThread.Abort();
                        File.Delete(path + downloadFormatExtLower);

                        String descPath = null;
                        if (File.Exists(path + "/" + Mod.DESCRIPTION_FILE))
                            descPath = path + "/" + Mod.DESCRIPTION_FILE;
                        else if (File.Exists(path + "/" + (downloadingMod.InstallationPath ?? downloadingModName) + "/" + Mod.DESCRIPTION_FILE))
                            descPath = path + "/" + (downloadingMod.InstallationPath ?? downloadingModName) + "/" + Mod.DESCRIPTION_FILE;

                        if (descPath != null)
                        {
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
                                // TODO language:
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
                                        // TODO language:
                                        MessageBox.Show($"The mod folder name '{Path.GetFileName(sd)}' is different than expected '{downloadingMod.InstallationPath}'\n\nPlease inform the author of the mod{(downloadingMod.Author != null ? $" ({downloadingMod.Author})" : "")}.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        if (File.Exists(sd + "/" + Mod.DESCRIPTION_FILE))
                                        {
                                            descPath = sd + "/" + Mod.DESCRIPTION_FILE;
                                            moveDesc = true;
                                        }
                                        sourcePath = sd;
                                        destPath = downloadingMod.InstallationPath ?? downloadingModName;
                                        proceedNext = true;
                                        break;
                                    }
                                if (!proceedNext)
                                {
                                    // TODO language:
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
                                if (moveDesc && descPath != null && File.Exists(descPath))
                                    File.Move(descPath, destPath + "/" + Mod.DESCRIPTION_FILE);
                                else if (descPath == null)
                                    downloadingMod.GenerateDescription(destPath);
                                if (Directory.Exists(path))
                                    Directory.Delete(path, true);
                                success = true;
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        // TODO language:
                        MessageBox.Show($"Failed to automatically install the mod {path}\n\n{err.Message}", "Error", MessageBoxButton.OK);
                    }
                }
                else if (downloadingMod.DownloadFormat.StartsWith("SingleFileWithPath:"))
                {
                    try
                    {
                        Boolean proceedNext = true;
                        String modInstallPath = downloadingMod.InstallationPath ?? downloadingModName;
                        if (Directory.Exists(modInstallPath))
                        {
                            // TODO language:
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
                    catch (Exception err)
                    {
                        // TODO language:
                        MessageBox.Show($"Error while installing the mod {path} (SingleFileWithPath)\n\n{err.Message}", "Error", MessageBoxButton.OK);
                    }
                }
                try
                {
                    Boolean activateTheNewMod = success;
                    String installPath = downloadingMod.InstallationPath ?? downloadingModName;
                    if (success)
                    {
                        if (!Directory.EnumerateFileSystemEntries(Mod.INSTALLATION_TMP).GetEnumerator().MoveNext())
                            Directory.Delete(Mod.INSTALLATION_TMP);
                        Mod previousMod = Mod.SearchWithPath(ModListInstalled, installPath);
                        if (previousMod != null)
                        {
                            previousMod.CurrentVersion = null;
                            activateTheNewMod = false;
                        }
                    }
                    DownloadList.Remove(downloadingMod);
                    downloadingMod = null;
                    if (DownloadList.Count > 0)
                        DownloadStart(DownloadList[0]);
                    else
                    {
                        lstDownloads.MinHeight = 0;
                        lstDownloads.Height = 0;
                        btnCancelStackpanel.Height = 0;
                    }
                    UpdateModListInstalled();
                    CheckForValidModFolder();
                    UpdateCatalogInstallationState();
                    if (activateTheNewMod)
                    {
                        Mod newMod = Mod.SearchWithPath(ModListInstalled, installPath);
                        if (newMod != null)
                        {
                            newMod.IsActive = true;
                            foreach (Mod submod in newMod.SubMod)
                                submod.IsActive = submod.IsDefault;
                            newMod.TryApplyPreset();
                        }
                    }
                    CheckOutdatedAndIncompatibleMods();
                    UpdateModSettings();
                }
                catch (Exception err)
                {
                    // TODO language:
                    MessageBox.Show($"Error while activating the mod {path}\n\n{err.Message}", "Error", MessageBoxButton.OK);
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
                CheckOutdatedAndIncompatibleMods();
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
            ModListCatalog.Clear();
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
                ModListCatalog.Clear();
                using (Stream input = File.OpenRead(CATALOG_PATH))
                using (StreamReader reader = new StreamReader(input))
                    Mod.LoadModDescriptions(reader, ModListCatalog);
                UpdateCatalogInstallationState();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void UpdateModListInstalled()
        {
            Boolean hasChanged = false;
            foreach (String dir in Directory.EnumerateDirectories("."))
            {
                if (File.Exists(dir + "/" + Mod.DESCRIPTION_FILE))
                {
                    Mod updatedMod = new Mod(dir);
                    Mod previousMod = Mod.SearchMod(ModListInstalled, updatedMod);
                    if (previousMod == null)
                    {
                        Mod modCatalog = Mod.SearchMod(ModListCatalog, updatedMod);
                        if (modCatalog != null) updatedMod.Priority = modCatalog.Priority;

                        Int32 at = 0;
                        for (at = 0; at < ModListInstalled.Count; at++)
                        {
                            if (ModListInstalled[at].Priority < updatedMod.Priority)
                                break;
                        }

                        ModListInstalled.Insert(at, updatedMod);
                        hasChanged = true;
                    }
                    else if ((updatedMod.CurrentVersion != null && previousMod.CurrentVersion == null) || (previousMod.CurrentVersion != null && updatedMod.CurrentVersion != null && previousMod.CurrentVersion < updatedMod.CurrentVersion))
                    {
                        foreach (Mod subMod in updatedMod.SubMod)
                        {
                            Mod previousSub = Mod.SearchWithPath(previousMod.SubMod, subMod.InstallationPath);
                            if (previousSub != null)
                                subMod.IsActive = previousSub.IsActive;
                        }
                        Int32 index = ModListInstalled.IndexOf(previousMod);
                        ModListInstalled.RemoveAt(index);
                        ModListInstalled.Insert(index, updatedMod);
                        updatedMod.IsActive = previousMod.IsActive;
                        updatedMod.Priority = previousMod.Priority;

                        hasChanged = true;
                    }
                }
            }
            UpdateInstalledPriorityValue();
            if (hasChanged)
                UpdateModSettings();
        }

        private static Color currentColor;
        private static String currentImage;

        public void UpdateLauncherTheme()
        {
            Color color = DefaultAccentColor;
            String image = DefaultBackgroundImage;
            foreach (var mod in ModListInstalled)
            {
                if (!mod.IsActive) continue;

                if (FindTheme(mod, ref image, ref color)) break;

                Boolean found = false;
                foreach (Mod submod in mod.SubMod)
                {
                    if (!submod.IsActive) continue;
                    if (FindTheme(submod, ref image, ref color))
                    {
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            if (color != currentColor)
            {
                SetAccentColor(color);
                currentColor = color;
            }
            if (image != currentImage)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(image, UriKind.Absolute);
                    bitmap.EndInit();
                    Launcher.Source = bitmap;
                }
                catch { }
                currentImage = image;
            }
        }

        private bool FindTheme(Mod mod, ref String image, ref Color color)
        {
            Boolean found = false;
            if (mod.LauncherBackground != null)
            {
                String newImage = $"{Directory.GetCurrentDirectory()}\\{(mod.ParentMod != null ? mod.ParentMod.InstallationPath : mod.FullInstallationPath)}\\{mod.LauncherBackground}";
                if (File.Exists(newImage))
                {
                    image = newImage;
                    found = true;
                }
            }

            if (mod.LauncherColor != null)
            {
                String newcolor = $"#CC{mod.LauncherColor.Replace("#", "")}";
                try
                {
                    color = (Color)ColorConverter.ConvertFromString(newcolor);
                    found = true;
                }
                catch { }
            }
            return found;
        }

        private Boolean GenerateAutomaticDescriptionFile(String folderName)
        {
            if (!Directory.Exists(folderName) || File.Exists(folderName + "/" + Mod.DESCRIPTION_FILE))
                return false;
            Mod catalogVersion = Mod.SearchWithPath(ModListCatalog, folderName);
            if (catalogVersion != null)
            {
                catalogVersion.GenerateDescription(folderName);
                return true;
            }

            String name = folderName;
            String author = "";
            String category = "";
            String description = "";

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
        }

        private void UpdateModDetails(Mod mod)
        {
            if (mod != null && mod == currentMod) return;
            currentMod = mod;

            if (mod == null || mod.Name == null)
            {
                gridModName.Visibility = Visibility.Collapsed;
                gridModInfo.Visibility = Visibility.Collapsed;
                PreviewModWebsite.Visibility = Visibility.Collapsed;
                PreviewModCategoryTagline.Visibility = Visibility.Collapsed;
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri("pack://application:,,,/images/Gradient.png");
                    bitmap.EndInit();
                    PreviewModImage.Source = bitmap;
                }
                catch { }
                DoubleAnimation animation = new DoubleAnimation
                {
                    To = 0,
                    Duration = new TimeSpan(0),
                };
                ModOptions.BeginAnimation(RowDefinition.MaxHeightProperty, animation);
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
                ReleaseNotesBlock.Visibility = PreviewModReleaseNotes.Text == "" ? Visibility.Collapsed : Visibility.Visible;

                Mod installedVersion = Mod.SearchMod(ModListInstalled, mod);
                if (installedVersion != null)
                {
                    foreach (Mod subMod in mod.SubMod)
                    {
                        Mod installedSub = Mod.SearchWithPath(installedVersion.SubMod, subMod.InstallationPath);
                        if (installedSub != null)
                            subMod.IsActive = installedSub.IsActive;
                    }
                }

                Int32 nOptions = PopulateModOptions(mod, tabCtrlMain.SelectedIndex == 0);
                ModOptionsHeaderLabel.Content = $"{Lang.Res["ModEditor.SubModPanel"]} ({nOptions})";
                DoubleAnimation animation = new DoubleAnimation
                {
                    To = nOptions > 0 ? (String)ModOptionsHeaderArrow.Content == "▲" ? ModOptionsHeaderButton.ActualHeight : GroupModInfoWrapper.ActualHeight : 0,
                    Duration = new TimeSpan(0),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
                };
                ModOptions.BeginAnimation(RowDefinition.MaxHeightProperty, animation);
                ModOptionsGrid.MaxHeight = GroupModInfoWrapper.ActualHeight;
                ModOptionsScrollViewer.ScrollToTop();
                ModOptionsScrollViewer.ScrollToLeftEnd();

                if (mod.PreviewImage == null)
                {
                    if (tabCtrlMain.SelectedIndex == 0 && mod.PreviewFile != null)
                    {
                        String imagePath = $"./{mod.InstallationPath}/{mod.PreviewFile}";
                        if (File.Exists(imagePath))
                        {
                            try
                            {
                                mod.PreviewImage = new BitmapImage();
                                mod.PreviewImage.BeginInit();
                                mod.PreviewImage.UriSource = new Uri(imagePath, UriKind.Relative);
                                mod.PreviewImage.CacheOption = BitmapCacheOption.OnLoad;
                                mod.PreviewImage.EndInit();
                            }
                            catch { }
                        }
                    }
                    else if (tabCtrlMain.SelectedIndex == 0 && mod.PreviewFileUrl != null)
                    {
                        try
                        {
                            mod.PreviewImage = new BitmapImage(new Uri(mod.PreviewFileUrl, UriKind.Absolute));
                            mod.PreviewImage.DownloadCompleted += OnPreviewFileDownloaded;
                        }
                        catch { }
                    }
                    else if (tabCtrlMain.SelectedIndex == 1 && mod.PreviewFileUrl != null)
                    {
                        try
                        {
                            mod.PreviewImage = new BitmapImage(new Uri(mod.PreviewFileUrl, UriKind.Absolute));
                            mod.PreviewImage.DownloadCompleted += OnPreviewFileDownloaded;
                        }
                        catch { }
                    }
                }
                if (mod.PreviewImage == null)
                {
                    try
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("pack://application:,,,/images/Gradient.png");
                        bitmap.EndInit();
                        PreviewModImage.Source = bitmap;
                    }
                    catch { }
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

        private Int32 PopulateModOptions(Mod mod, Boolean isEnabled)
        {
            ModOptionsPanel.Children.Clear();
            Int32 count = 0;
            foreach (Mod submod in mod.SubMod)
            {
                if (submod.ActivateWithMod != null || submod.ActivateWithoutMod != null)
                    continue;
                if (submod.IsHeader)
                {
                    TextBlock textbloc = new TextBlock();
                    textbloc.Text = submod.Name;
                    textbloc.TextAlignment = TextAlignment.Center;
                    textbloc.HorizontalAlignment = HorizontalAlignment.Center;
                    textbloc.FontStretch = FontStretch.FromOpenTypeStretch(9);
                    textbloc.FontWeight = FontWeight.FromOpenTypeWeight(500);
                    textbloc.FontSize = 14;
                    textbloc.VerticalAlignment = VerticalAlignment.Center;
                    textbloc.Margin = new Thickness(0);
                    textbloc.Height = 18;
                    textbloc.Padding = new Thickness(0);
                    textbloc.SetValue(TextBlock.FontFamilyProperty, Application.Current.FindResource("CenturyGothic") as FontFamily);
                    Border border = new Border();
                    border.SetResourceReference(Border.BackgroundProperty, "BrushAccentColor");
                    border.CornerRadius = new CornerRadius(5);
                    border.Margin = new Thickness(0, 7, 0, 7);
                    border.Height = 20;
                    border.HorizontalAlignment = HorizontalAlignment.Stretch;
                    border.VerticalAlignment = VerticalAlignment.Center;
                    border.Child = textbloc;
                    ModOptionsPanel.Children.Add(border);
                    continue;
                }
                CheckBox checkBox = new CheckBox();
                checkBox.Tag = submod;
                checkBox.Content = submod.Name;
                checkBox.IsChecked = submod.IsActive && isEnabled;
                checkBox.IsEnabled = isEnabled && mod.IsActive;
                checkBox.Style = (Style)Application.Current.FindResource("CheckBoxStyle");
                checkBox.Margin = new Thickness(0, 0, 0, 4);

                checkBox.Checked += SubMod_CheckChanged;
                checkBox.Unchecked += SubMod_CheckChanged;

                Grid grid = new Grid();
                grid.Children.Add(checkBox);
                ModOptionsPanel.Children.Add(grid);

                if (!String.IsNullOrEmpty(submod.Description))
                {
                    String previewPath = "";
                    if (!String.IsNullOrEmpty(submod.PreviewFile))
                        previewPath = $"./{submod.ParentMod.InstallationPath}/{submod.PreviewFile}";
                    if (!String.IsNullOrEmpty(submod.PreviewFileUrl) && (String.IsNullOrEmpty(submod.PreviewFile) || !File.Exists(previewPath)))
                        previewPath = submod.PreviewFileUrl;
                    UiGrid.MakeTooltip(grid, submod.Description, previewPath, "", placement: System.Windows.Controls.Primitives.PlacementMode.Left);
                }
                count++;
            }

            return count;
        }

        private Boolean preventCheckLoop = false;
        private void SubMod_CheckChanged(Object sender, RoutedEventArgs e)
        {
            if (preventCheckLoop) return;

            CheckBox checkbox = sender as CheckBox;
            Mod mod = (Mod)checkbox.Tag;
            mod.IsActive = checkbox.IsChecked == true;

            if (mod.Group != null && checkbox.IsChecked == true)
            {
                // Deactivate all other sub-mods in the group
                foreach (Mod submod in mod.ParentMod.SubMod)
                {
                    if (submod == mod || submod.Group != mod.Group)
                        continue;
                    submod.IsActive = false;
                }
            }
            else if (mod.Group != null && checkbox.IsChecked == false)
            {
                // Activate the default of the group if found
                Mod defaultmod = null;
                foreach (Mod submod in mod.ParentMod.SubMod)
                {
                    if (submod.Group == mod.Group && submod.IsDefault)
                    {
                        defaultmod = submod;
                        break;
                    }
                }
                if (defaultmod != null) defaultmod.IsActive = true;
            }

            RefreshModOptions();
            UpdateLauncherTheme();
            // Save
            UpdateModSettings();
        }

        private void RefreshModOptions()
        {
            // Refresh the checkboxes
            preventCheckLoop = true;
            foreach (var el in ModOptionsPanel.Children)
            {
                if (el is not Grid grid) continue;
                CheckBox option = grid.Children[0] as CheckBox;
                option.IsEnabled = ((Mod)option.Tag).ParentMod.IsActive;
                option.IsChecked = ((Mod)option.Tag).IsActive;
            }
            preventCheckLoop = false;
        }

        private void UpdateCatalogInstallationState()
        {
            foreach (Mod mod in ModListCatalog)
            {
                if (Mod.SearchMod(DownloadList, mod) != null)
                    mod.Installed = WaitingEmoji;
                else if (Mod.SearchMod(ModListInstalled, mod) != null && Mod.SearchMod(ModListInstalled, mod).IsOutdated)
                    mod.Installed = UpdateEmoji;
                else if (Mod.SearchMod(ModListInstalled, mod) != null)
                    mod.Installed = InstalledEmoji;

                else
                    mod.Installed = "";
            }
            lstCatalogMods.Items.Refresh();
        }

        private void UpdateInstalledPriorityValue()
        {
            for (Int32 i = 0; i < ModListInstalled.Count; i++)
            {
                Mod mod = Mod.SearchMod(ModListCatalog, ModListInstalled[i]);
                if (mod != null)
                    ModListInstalled[i].Priority = mod.Priority;
            }
            lstMods.Items.Refresh();
            UpdateModSettings();
        }

        private void SortCatalog(MethodInfo sortGetter, Boolean ascending)
        {
            if (sortGetter == null || sortGetter.DeclaringType != typeof(Mod) || sortGetter.ReturnType.GetInterface(nameof(IComparable)) == null || sortGetter.GetParameters().Length > 0)
                return;
            List<Mod> catalogList = new List<Mod>(ModListCatalog);
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
            ModListCatalog = new ObservableCollection<Mod>(catalogList);
            lstCatalogMods.ItemsSource = ModListCatalog;
        }

        public void LoadModSettings()
        {
            ModListInstalled.Clear();
            try
            {
                IniFile iniFile = IniFile.MemoriaIni;

                // FolderNames
                String str = iniFile.GetSetting("Mod", "FolderNames");
                str = str.Trim().Trim('"');
                String[] iniModActiveList = Regex.Split(str, @""",\s*""");
                // Priorities
                str = iniFile.GetSetting("Mod", "Priorities");
                str = str.Trim().Trim('"');
                String[] iniModPriorityList = Regex.Split(str, @""",\s*""");
                // ActiveSubMods
                str = iniFile.GetSetting("Mod", "ActiveSubMods");
                str = str.Trim().Trim('"');
                String[] activeSubModList = Regex.Split(str, @""",\s*""");

                String[][] listCouple = [iniModPriorityList, iniModActiveList];
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
                        Mod mod = Mod.SearchWithPath(ModListInstalled, listCouple[listI][i]);
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
                        ModListInstalled.Add(mod);
                    }
                }

                foreach (Mod mod in ModListInstalled)
                    foreach (Mod sub in mod.SubMod)
                        if (activeSubModList.Contains($"{mod.InstallationPath}/{sub.InstallationPath}"))
                            sub.IsActive = true;

                foreach (String path in subModList)
                {
                    Int32 sepIndex = path.IndexOf("/");
                    String mainModPath = path.Substring(0, sepIndex);
                    String subModPath = path.Substring(sepIndex + 1);
                    Mod mod = Mod.SearchWithPath(ModListInstalled, mainModPath);
                    if (mod.SubMod == null)
                        continue;
                    foreach (Mod sub in mod.SubMod)
                        if (sub.InstallationPath == subModPath)
                            sub.IsActive = true;
                }
            }
            catch (Exception ex) { UiHelper.ShowError(Application.Current.MainWindow, ex); }
        }

        public void UpdateModSettings()
        {
            try
            {
                List<String> iniModActiveList = new List<String>();
                List<String> iniModActiveSubList = new List<String>();
                List<String> iniModPriorityList = new List<String>();

                foreach (Mod mod in ModListInstalled)
                {
                    // Saving the priority
                    iniModPriorityList.Add(mod.InstallationPath);

                    AutoActivateSubMods(mod);
                    foreach (Mod submod in mod.SubMod)
                    {
                        // Making sure the default of the group is active if none selected
                        if (submod.Group == null) continue;
                        Mod defaultmod = null;
                        foreach (Mod groupMod in mod.SubMod)
                        {
                            if (groupMod.Group != submod.Group) continue;
                            if (groupMod.IsDefault) defaultmod = groupMod;
                            if (groupMod.IsActive)
                            {
                                // We found an active mod in the group
                                defaultmod = null;
                                break;
                            }
                        }
                        if (defaultmod != null) defaultmod.IsActive = true;
                    }
                    // Saving the active sub-mods
                    foreach (Mod submod in mod.SubMod)
                        if (submod.IsActive && submod.ActivateWithMod == null && submod.ActivateWithoutMod == null)
                            iniModActiveSubList.Add($"{mod.InstallationPath}/{submod.InstallationPath}");

                    // Save active mods
                    if (!mod.IsActive) continue;
                    foreach (String path in mod.EnumerateModAndSubModFoldersOrdered(true))
                    {
                        // Only add existing paths
                        if (Directory.Exists(path))
                            iniModActiveList.Add(path);
                    }
                }

                IniFile iniFile = IniFile.MemoriaIni;
                iniFile.SetSetting("Mod", "FolderNames", iniModActiveList.Count > 0 ? "\"" + String.Join("\", \"", iniModActiveList) + "\"" : "");
                iniFile.SetSetting("Mod", "Priorities", iniModPriorityList.Count > 0 ? "\"" + String.Join("\", \"", iniModPriorityList) + "\"" : "");
                iniFile.SetSetting("Mod", "ActiveSubMods", iniModActiveSubList.Count > 0 ? "\"" + String.Join("\", \"", iniModActiveSubList) + "\"" : "");
                iniFile.Save();
            }
            catch (Exception ex) { UiHelper.ShowError(Application.Current.MainWindow, ex); }
        }

        private void SetupFrameLang()
        {
            GroupModInfo.SetResourceReference(TabItem.HeaderProperty, "ModEditor.ModInfos");
            PreviewModWebsite.SetResourceReference(ContentProperty, "ModEditor.Website");
            CaptionModAuthor.SetResourceReference(TextBlock.TextProperty, "ModEditor.Author");
            CaptionModRelease.SetResourceReference(TextBlock.TextProperty, "ModEditor.Release");
            CaptionModReleaseOriginal.SetResourceReference(TextBlock.TextProperty, "ModEditor.ReleaseOriginal");
            CaptionModReleaseNotes.SetResourceReference(TextBlock.TextProperty, "ModEditor.ReleaseNotes");
            tabMyMods.SetResourceReference(TextBlock.TextProperty, "ModEditor.TabMyMods");

            GridViewColumnHeader header = new GridViewColumnHeader();
            header.SetResourceReference(ContentProperty, "ModEditor.Name");
            colMyModsName.Header = header;

            header = new GridViewColumnHeader();
            header.SetResourceReference(ContentProperty, "ModEditor.Category");
            colMyModsCategory.Header = header;

            tabCatalog.SetResourceReference(TextBlock.TextProperty, "ModEditor.TabCatalog");

            header = new GridViewColumnHeader();
            header.SetResourceReference(ContentProperty, "ModEditor.Mod");
            colDownloadName.Header = header;

            header = new GridViewColumnHeader();
            header.SetResourceReference(ContentProperty, "ModEditor.Progress");
            colDownloadProgress.Header = header;

            header = new GridViewColumnHeader();
            header.SetResourceReference(ContentProperty, "ModEditor.Speed");
            colDownloadSpeed.Header = header;

            header = new GridViewColumnHeader();
            header.SetResourceReference(ContentProperty, "ModEditor.TimeLeft");
            colDownloadTimeLeft.Header = header;

            header = new GridViewColumnHeader();
            header.SetResourceReference(ContentProperty, "ModEditor.Name");
            header.Click += OnClickCatalogHeader;
            colCatalogName.Header = header;

            header = new GridViewColumnHeader();
            header.SetResourceReference(ContentProperty, "ModEditor.Author");
            header.Click += OnClickCatalogHeader;
            colCatalogAuthor.Header = header;

            header = new GridViewColumnHeader();
            header.SetResourceReference(ContentProperty, "ModEditor.Category");
            header.Click += OnClickCatalogHeader;
            colCatalogCategory.Header = header;

            header = new GridViewColumnHeader() { Content = "" };
            header.Click += OnClickCatalogHeader;
            colCatalogInstalled.Header = header;

            header = new GridViewColumnHeader() { Content = ActiveEmoji };
            header.Click += OnClickActiveHeader;
            colMyModsActive.Header = header;
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

        private const String CATALOG_PATH = "./ModCatalog.xml";
        private const String CATALOG_URL = "https://raw.githubusercontent.com/Albeoris/Memoria/main/Memoria.Launcher/Catalogs/MemoriaCatalog.xml";
    }
}
