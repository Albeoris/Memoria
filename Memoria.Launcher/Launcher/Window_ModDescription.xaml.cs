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

            ModNameLabel.Content = mod.Name;
            Document.Blocks.Clear();
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
                            Margin = new Thickness(0, 10, 0, 10),
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"))
                        };
                        Document.Blocks.Add(p);
                        paragraph = "";
                    }
                    ListItem item = new ListItem(new Paragraph(new Run(line.TrimStart(['-', ' ']))));
                    item.Margin = new Thickness(20, 0, 0, 0);
                    if (list == null)
                    {
                        list = new List();
                        list.Padding = new Thickness(0, 0, 0, 0);
                        list.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"));
                        Document.Blocks.Add(list);
                    }
                    list.ListItems.Add(item);
                }
                else if (line.StartsWith("#"))
                {
                    if (paragraph.Trim().Length > 0)
                    {
                        Paragraph p1 = new Paragraph(new Run(paragraph.Trim()))
                        {
                            Margin = new Thickness(0, 10, 0, 10),
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"))
                        };
                        Document.Blocks.Add(p1);
                        paragraph = "";
                    }
                    list = null;
                    Paragraph p = new Paragraph(new Run(line.TrimStart('#')))
                    {
                        Margin = new Thickness(0, 20, 0, 10),
                        FontSize = 20
                    };
                    Document.Blocks.Add(p);
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
                    Margin = new Thickness(0, 10, 0, 10),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"))
                };
                Document.Blocks.Add(p);
            }
            if (!String.IsNullOrEmpty(mod.DownloadUrl))
            {
                Hyperlink link = new Hyperlink(new Run("\n" + (String)Lang.Res["ModEditor.DirectDownload"]));
                link.NavigateUri = new Uri(mod.DownloadUrl);
                link.RequestNavigate += (s, e) =>
                {
                    Process.Start(e.Uri.ToString());
                };
                Document.Blocks.Add(new Paragraph(link) { Margin = new Thickness(0, 10, 0, 10), });
            }
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
            FlowDocumentScrollViewer o = (FlowDocumentScrollViewer)sender;
            ScrollViewer scrollViewer = o.Template?.FindName("PART_ContentHost", o) as ScrollViewer;
            if (scrollViewer is not null)
            {
                double offset = scrollViewer.VerticalOffset - (e.Delta / 3f);
                scrollViewer.ScrollToVerticalOffset(offset < 0 ? 0 : offset > scrollViewer.ExtentHeight ? scrollViewer.ExtentHeight : offset);
            }
        }
    }
}
