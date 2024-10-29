using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Reflection;
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

            Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChangeLog.txt");
            String[] lines;
            using (StreamReader reader = new(input))
            {
                lines = reader.ReadToEnd().Split('\n');
            }
            
            List list = null;
            foreach (String line in lines)
            {
                String trimmed = line.Trim();
                if (trimmed.Length == 0)
                    continue;

                if(trimmed.StartsWith("#Version") || trimmed.StartsWith("#Patch"))
                {
                    Paragraph p = new Paragraph(new Run(trimmed.TrimStart('#')))
                    {
                        Margin = new Thickness(0, 20, 0, 20),
                        FontSize = 26
                    };
                    String version = trimmed.Split(' ')?[1] ?? "";
                    if (trimmed.StartsWith("#Version") && DateTime.TryParseExact(version, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        Hyperlink link = new Hyperlink();
                        link.FontSize = 16;
                        link.Inlines.Add("Complete changelog ↗");
                        link.NavigateUri = new Uri("https://github.com/Albeoris/Memoria/wiki/Changelog-v" + version);
                        link.RequestNavigate += (s, e) =>
                        {
                            Process.Start(e.Uri.ToString());
                        };
                        p.Inlines.Add(new LineBreak());
                        p.Inlines.Add(link);
                    }
                    Document.Blocks.Add(p);
                    list = null;
                    continue;
                }

                if (trimmed.StartsWith("#"))
                {
                    Paragraph p = new Paragraph(new Run(trimmed.TrimStart('#')))
                    {
                        Margin = new Thickness(0, 20, 0, 10),
                        FontSize = 20
                    };
                    Document.Blocks.Add(p);
                    list = null;
                    continue;
                }

                String tabbed = line.Replace("    ", "\t");
                Int32 indent = tabbed.Length - tabbed.TrimStart().Length;
                if(indent == 0)
                {
                    Paragraph p = new Paragraph(new Run(trimmed))
                    {
                        Margin = new Thickness(0, 10, 0, 10),
                    };
                    Document.Blocks.Add(p);
                    list = null;
                }
                else
                {
                    Paragraph p = new Paragraph(new Run(trimmed));
                    ListItem item = new ListItem(p);
                    item.Margin = new Thickness(20 + indent * 20, 0, 0, 0);
                    if(list == null)
                    {
                        list = new List();
                        list.Padding = new Thickness(0, 0, 0, 0);
                        Document.Blocks.Add(list);
                    }
                    list.ListItems.Add(item);
                }
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
    }
}
