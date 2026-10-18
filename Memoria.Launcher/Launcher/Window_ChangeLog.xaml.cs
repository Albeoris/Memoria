using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Memoria.Launcher.Utils.Updates;

namespace Memoria.Launcher
{
    public partial class Window_ChangeLog : UserControl
    {
        public Window_ChangeLog()
        {
            InitializeComponent();
        }

        internal Window_ChangeLog(ReleaseNotesDocument releaseNotes) : this()
        {
            Render(releaseNotes ?? throw new ArgumentNullException(nameof(releaseNotes)));
        }

        private void Render(ReleaseNotesDocument releaseNotes)
        {
            Document.Document.Blocks.Clear();
            Paragraph title = new Paragraph(new Run($"{releaseNotes.Build.Name}: {releaseNotes.Title}")) { Margin = new Thickness(), Padding = new Thickness(0, 10, 0, 4), FontSize = 26 };
            Hyperlink releaseLink = new Hyperlink(new Run(releaseNotes.Tag + " ↗")) { NavigateUri = releaseNotes.ReleasePage, FontSize = 16 };
            releaseLink.RequestNavigate += OpenLink;
            title.Inlines.Add(new LineBreak());
            title.Inlines.Add(releaseLink);
            Document.Document.Blocks.Add(title);

            List currentList = null;
            foreach (ReleaseNotesBlock block in releaseNotes.Blocks)
            {
                if (block.Kind == ReleaseNotesBlockKind.Bullet)
                {
                    if (currentList == null)
                    {
                        currentList = new List { MarkerStyle = TextMarkerStyle.Disc, Margin = new Thickness(), Padding = new Thickness(24, 0, 0, 0) };
                        Document.Document.Blocks.Add(currentList);
                    }
                    currentList.ListItems.Add(new ListItem(new Paragraph(new Run(block.Text)) { Margin = new Thickness(), Padding = new Thickness(0, 0, 0, 5) }));
                    continue;
                }

                currentList = null;
                if (block.Kind == ReleaseNotesBlockKind.Link)
                {
                    Hyperlink link = new Hyperlink(new Run(block.Text + " ↗")) { NavigateUri = block.Link };
                    link.RequestNavigate += OpenLink;
                    Document.Document.Blocks.Add(new Paragraph(link) { Margin = new Thickness(), Padding = new Thickness(0, 8, 0, 8) });
                    continue;
                }

                Double fontSize = block.Kind == ReleaseNotesBlockKind.Heading ? 20 : 16;
                Thickness padding = block.Kind == ReleaseNotesBlockKind.Heading ? new Thickness(0, 20, 0, 8) : new Thickness(0, 6, 0, 6);
                Document.Document.Blocks.Add(new Paragraph(new Run(block.Text)) { Margin = new Thickness(), Padding = padding, FontSize = fontSize, Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aeee")) });
            }
        }

        private static void OpenLink(Object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink link && link.NavigateUri != null)
                Process.Start(link.NavigateUri.AbsoluteUri);
            e.Handled = true;
        }

        private void Close(Object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)this.GetRootElement();
            ((Grid)Parent).Children.Remove(this);
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
            Double offset = scrollViewer.VerticalOffset - e.Delta / 3f;
            scrollViewer.ScrollToVerticalOffset(offset < 0 ? 0 : offset > scrollViewer.ExtentHeight ? scrollViewer.ExtentHeight : offset);
        }
    }
}
