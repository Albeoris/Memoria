using System;
using System.Threading.Tasks;
using System.Windows;

namespace Memoria.Launcher
{
    public sealed class UiLauncherModManagerButton : UiLauncherButton
    {
        public UiLauncherModManagerButton()
        {
            Label = Lang.Button.ModManager;
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
}
