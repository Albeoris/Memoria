using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
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
    public partial class Window_ChangeLog : UserControl
    {
        public Window_ChangeLog()
        {
            InitializeComponent();

            if (changeLogHtml == null)
                LoadRemoteChangelog();
            else
                ParseChangeLogHtml();
        }

        private static String changeLogHtml = null;

        private async void LoadRemoteChangelog()
        {
            Document.Blocks.Clear();
            Paragraph loading = new Paragraph(new Run($"Loading changelog..."))
            {
                Margin = new Thickness(0, 20, 0, 20),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"))
            };
            Document.Blocks.Add(loading);
            try
            {
                // Load the release page from github and parse it into a changelog
                String url = "https://github.com/Albeoris/Memoria/releases";
                using (HttpClient client = new())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    using (var response = await client.GetAsync(url))
                    {
                        response.EnsureSuccessStatusCode();
                        changeLogHtml = await response.Content.ReadAsStringAsync();
                    }
                }
                ParseChangeLogHtml();
            }
            catch
            {
                Document.Blocks.Clear();
                Paragraph p = new Paragraph(new Run($"Couldn't load the changelog."))
                {
                    Margin = new Thickness(0, 20, 0, 20),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"))
                };
                Document.Blocks.Add(p);
            }
        }

        private void ParseChangeLogHtml()
        {
            Document.Blocks.Clear();

            try
            {
                foreach (Match sectionMatch in Regex.Matches(changeLogHtml, @"<section[^>]*>(.*?)</section>", RegexOptions.Singleline))
                {
                    if (!sectionMatch.Success) continue;

                    String section = sectionMatch.Groups[1].Value;
                    String version = Regex.Match(section, "v(20[^\\\"<]*)\\\"", RegexOptions.Singleline).Groups[1].Value;
                    String wikiLink = Regex.Match(section, @"https:\/\/github.com\/Albeoris\/Memoria\/wiki\/Changelog-v20[^\""]*", RegexOptions.Singleline).Groups?[0].Value;
                    String title = Regex.Match(section, @"<h2[^>]*>(.*?)</h2>", RegexOptions.Singleline).Groups[1].Value;

                    String content = Regex.Match(section, @"<div[^>]*body-content[^>]*>(.*?)<\/div>", RegexOptions.Singleline).Groups[1].Value;
                    // Removes changelog link
                    content = Regex.Replace(content, @"<p>(?!:<\/p>).*?COMPLETE CHANGELOG HERE<\/a><\/p>", "", RegexOptions.Singleline);
                    // Remove all links
                    content = Regex.Replace(content, @"<a[^>]*>(.*?)<\/a>", "$1", RegexOptions.Singleline);

                    // Main header
                    {
                        Paragraph p = new Paragraph(new Run($"Version {version}"))
                        {
                            Margin = new Thickness(0, 20, 0, 20),
                            FontSize = 26
                        };

                        if (!String.IsNullOrEmpty(wikiLink))
                        {
                            Hyperlink link = new Hyperlink();
                            link.FontSize = 16;
                            link.Inlines.Add("Complete changelog ↗");
                            link.NavigateUri = new Uri(wikiLink);
                            link.RequestNavigate += (s, e) =>
                            {
                                Process.Start(e.Uri.ToString());
                            };
                            p.Inlines.Add(new LineBreak());
                            p.Inlines.Add(link);
                        }
                        Document.Blocks.Add(p);
                    }

                    // Parse content
                    List list = null;
                    Int32 indent = 0;

                    String[] lines = content.Split('\n');
                    foreach (String line in lines)
                    {
                        String trimmed = line.Trim();
                        String plainText = WebUtility.HtmlDecode(Regex.Replace(trimmed, @"<[^>]*>", ""));

                        if (trimmed.StartsWith("<h2>"))
                        {
                            Paragraph p = new Paragraph(new Run(plainText))
                            {
                                Margin = new Thickness(0, 20, 0, 10),
                                FontSize = 20
                            };
                            Document.Blocks.Add(p);
                            continue;
                        }
                        if (trimmed.StartsWith("<ul>"))
                        {
                            indent++;
                            continue;
                        }
                        if (trimmed.StartsWith("<li>"))
                        {
                            Paragraph p = new Paragraph(new Run(plainText));
                            ListItem item = new ListItem(p)
                            {
                                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"))
                            };
                            item.Margin = new Thickness(20 + indent * 20, 0, 0, 0);
                            if (list == null)
                            {
                                list = new List();
                                list.Padding = new Thickness(0, 0, 0, 0);
                                Document.Blocks.Add(list);
                            }
                            list.ListItems.Add(item);
                            continue;
                        }
                        if (trimmed.StartsWith("</ul>"))
                        {
                            indent--;
                            if (indent == 0)
                                list = null;
                            continue;
                        }
                        if (trimmed.StartsWith("<p>"))
                        {
                            Paragraph p = new Paragraph(new Run(plainText))
                            {
                                Margin = new Thickness(0, 10, 0, 10),
                                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"))
                            };
                            Document.Blocks.Add(p);
                            continue;
                        }
                    }
                }
            }
            catch
            {
                Document.Blocks.Clear();
                Paragraph p = new Paragraph(new Run($"Couldn't parse the changelog."))
                {
                    Margin = new Thickness(0, 20, 0, 20),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee"))
                };
                Document.Blocks.Add(p);
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
