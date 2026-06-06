using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Memoria.Launcher
{
    /// <summary>
    /// Interaction logic for Window_ReleaseNotes.xaml
    /// </summary>
    public partial class Window_ModDescription : UserControl
    {
        public Window_ModDescription(Mod mod)
        {
            InitializeComponent();
            Document.PreviewMouseLeftButtonUp += Document_PreviewMouseLeftButtonUp;
            Document.PreviewMouseMove += Document_PreviewMouseMove;
            Document.MouseLeave += Document_MouseLeave;

            ModNameLabel.Content = mod.Name;
            Document.Document.Blocks.Clear();
            String plainText = $"#{Lang.Res["ModEditor.Description"]}\n{mod.Description}";
            if (!String.IsNullOrEmpty(mod.PatchNotes))
                plainText = $"{plainText}\n#{Lang.Res["ModEditor.ReleaseNotes"]}\n{mod.PatchNotes}";
            String[] lines = Regex.Split(plainText, @"\n", RegexOptions.Singleline);
            List list = null;
            String paragraph = "";
            foreach (string line in lines)
            {
                if (line.StartsWith("- "))
                {
                    if (paragraph.Trim().Length > 0)
                    {
                        Paragraph p = new Paragraph(new Run(paragraph.Trim()))
                        {
                            Margin = new Thickness(),
                            Padding = new Thickness(0, 0, 0, 10),
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"))
                        };
                        Document.Document.Blocks.Add(p);
                        paragraph = "";
                    }
                    ListItem item = new ListItem(new Paragraph(new Run("• " + line.TrimStart(['-', ' ']))));
                    item.Margin = new Thickness();
                    item.Padding = new Thickness(20, 0, 0, 5);
                    if (list == null)
                    {
                        list = new List();
                        list.MarkerStyle = TextMarkerStyle.None;
                        list.Margin = new Thickness();
                        list.Padding = new Thickness(0, 10, 0, 10);
                        list.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"));
                        Document.Document.Blocks.Add(list);
                    }
                    list.ListItems.Add(item);
                }
                else if (line.StartsWith("#"))
                {
                    if (paragraph.Trim().Length > 0)
                    {
                        Paragraph p1 = new Paragraph(new Run(paragraph.Trim()))
                        {
                            Margin = new Thickness(),
                            Padding = new Thickness(0, 0, 0, 10),
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"))
                        };
                        Document.Document.Blocks.Add(p1);
                        paragraph = "";
                    }
                    list = null;
                    Paragraph p = new Paragraph(new Run(line.TrimStart('#')))
                    {
                        Margin = new Thickness(),
                        Padding = new Thickness(0, 0, 0, 10),
                        FontSize = 20
                    };
                    Document.Document.Blocks.Add(p);
                }
                else
                {
                    list = null;
                    if (line.Length > 0 || !paragraph.EndsWith("\n\n"))
                        paragraph += $"{line}\n";
                }
            }
            if (paragraph.Length > 0)
            {
                Paragraph p = new Paragraph(new Run(paragraph.Trim()))
                {
                    Margin = new Thickness(),
                    Padding = new Thickness(0, 0, 0, 10),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"))
                };
                Document.Document.Blocks.Add(p);
            }
            if (!String.IsNullOrEmpty(mod.DownloadUrl))
            {
                Hyperlink link = new Hyperlink(new Run("\n" + (String)Lang.Res["ModEditor.DirectDownload"]));
                link.NavigateUri = new Uri(mod.DownloadUrl);
                link.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4493f8"));
                link.TextDecorations = TextDecorations.Underline;
                link.Cursor = Cursors.Hand;
                link.Click += OnHyperlinkClick;
                Document.Document.Blocks.Add(new Paragraph(link) { Margin = new Thickness(), Padding = new Thickness(0, 0, 0, 10), });
            }
        }

        private static Hyperlink TryGetHyperlink(TextPointer pointer)
        {
            if (pointer?.Parent is Hyperlink hyperlink)
                return hyperlink;

            if (pointer?.Parent is Run run && run.Parent is Hyperlink runHyperlink)
                return runHyperlink;

            return null;
        }

        private void UpdateDocumentCursor(Point position)
        {
            TextPointer pointer = Document.GetPositionFromPoint(position, true);
            Hyperlink hyperlink = TryGetHyperlink(pointer);
            Document.Cursor = hyperlink?.NavigateUri != null ? Cursors.Hand : Cursors.Arrow;
        }

        private static void OpenUrl(String url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                return;
            }
            catch
            {
                // Fall back for Wine/Proton setups where shell execution is unreliable.
            }

            try
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start \"\" \"{url}\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
                return;
            }
            catch
            {
                // Try Linux host openers next.
            }

            try
            {
                Process.Start(new ProcessStartInfo("xdg-open", url)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
                return;
            }
            catch
            {
                // Last fallback for some Linux desktop stacks.
            }

            try
            {
                Process.Start(new ProcessStartInfo("gio", $"open \"{url}\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            catch
            {
                // Ignore if no opener is available.
            }
        }

        private static void OnHyperlinkClick(Object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperlink && hyperlink.NavigateUri != null)
                OpenUrl(hyperlink.NavigateUri.AbsoluteUri);
        }

        private void Document_PreviewMouseLeftButtonUp(Object sender, MouseButtonEventArgs e)
        {
            TextPointer pointer = Document.GetPositionFromPoint(e.GetPosition(Document), true);
            Hyperlink hyperlink = TryGetHyperlink(pointer);
            if (hyperlink?.NavigateUri == null)
                return;

            OpenUrl(hyperlink.NavigateUri.AbsoluteUri);
            e.Handled = true;
        }

        private void Document_PreviewMouseMove(Object sender, MouseEventArgs e)
        {
            UpdateDocumentCursor(e.GetPosition(Document));
        }

        private void Document_MouseLeave(Object sender, MouseEventArgs e)
        {
            Document.Cursor = Cursors.Arrow;
        }

        private void Close(Object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)this.GetRootElement();
            ((Grid)this.Parent).Children.Remove(this);

            if (mainWindow.GameSettings.AutoRunGame)
                mainWindow.PlayButton.Click();
        }
        private void Bg_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            Window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e333"));
        }

        private void Bg_MouseUp(Object sender, MouseButtonEventArgs e)
        {
            Window.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e000"));
        }

        private void Window_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void DocumentScrollViewer_PreviewMouseWheel(Object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            double offset = scrollViewer.VerticalOffset - (e.Delta / 3f);
            scrollViewer.ScrollToVerticalOffset(offset < 0 ? 0 : offset > scrollViewer.ExtentHeight ? scrollViewer.ExtentHeight : offset);
        }
    }
}
