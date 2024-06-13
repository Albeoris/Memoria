using System;
using System.Threading.Tasks;
using System.Windows;

namespace Memoria.Launcher
{
    public sealed class UiLauncherGraphicsOptionButton : UiModManagerButton
    {
        public UiLauncherGraphicsOptionButton()
        {
            Label = "Graphics Option";
        }

        protected override async Task DoAction()
        {
            MainWindow mainWindow = (MainWindow)this.GetRootElement();
            if (mainWindow.GraphicsOptionWindow == null)
                mainWindow.GraphicsOptionWindow = new GraphicsOptionWindow();
            mainWindow.GraphicsOptionWindow.Owner = mainWindow;
            mainWindow.GraphicsOptionWindow.Show();
            mainWindow.GraphicsOptionWindow.Activate();
        }
    }
}
