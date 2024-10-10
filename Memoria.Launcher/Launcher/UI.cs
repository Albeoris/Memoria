using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Memoria.Launcher
{
    public static class UiTextBlockFactory
    {
        public static TextBlock Create(String text)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text
            };
            return textBlock;
        }
    }

    public static class UiHelper
    {
        public static void ShowError(FrameworkElement owner, Exception exception, String formatMessage = null, params Object[] args)
        {
            StringBuilder sb = new StringBuilder();

            if (!String.IsNullOrEmpty(formatMessage))
            {
                sb.AppendFormat(formatMessage, args);
                sb.AppendLine();
            }

            if (exception != null)
                sb.Append(exception);

            if (owner == null)
            {
                MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Window window = (Window)owner.GetRootElement();
                MessageBox.Show(window, sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public static class FrameworkElementExm
    {
        public static FrameworkElement GetRootElement(this FrameworkElement self)
        {
            FrameworkElement element = self;
            while (element.Parent != null)
                element = (FrameworkElement)element.Parent;
            return element;
        }

        public static T GetParentElement<T>(this FrameworkElement self) where T : FrameworkElement
        {
            DependencyObject element = VisualTreeHelper.GetParent(self);
            while (element != null)
            {
                T result = element as T;
                if (result != null)
                    return result;
                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }
    }
    /*
    public sealed class UiLauncherModManagerButton : UiModManagerButton
    {
        public UiLauncherModManagerButton()
        {
            Label = Lang.Launcher.ModManager;
        }

        protected override async Task DoAction()
        {
            MainWindow mainWindow = (MainWindow)this.GetRootElement();
            if (mainWindow.ModdingWindow == null)
                mainWindow.ModdingWindow = new ModManagerWindow();
            mainWindow.ModdingWindow.Owner = mainWindow;
            mainWindow.ModdingWindow.Show();
            mainWindow.ModdingWindow.Activate();
        }
    }
    */

    public sealed class UiLauncherModelViewerButton : UiModManagerButton
    {
        public UiLauncherModelViewerButton()
        {
            Label = Lang.Launcher.ModelViewer;
        }

        protected override async Task DoAction()
        {
            try
            {
                MainWindow mainWindow = (MainWindow)this.GetRootElement();
                mainWindow.PlayButton.Click(true);
            }
            catch (Exception) { }
        }
    }
}
