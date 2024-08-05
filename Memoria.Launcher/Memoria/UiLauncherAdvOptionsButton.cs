using System;
using System.Threading.Tasks;
using System.Windows;

namespace Memoria.Launcher
{
    public sealed class UiLauncherAdvOptionsButton : UiModManagerButton
    {
        public UiLauncherAdvOptionsButton()
        {
            Label = "Adv. settings";
        }

        protected override async Task DoAction()
        {
            MainWindow mainWindow = (MainWindow)this.GetRootElement();
            if (mainWindow.AdvOptionsWindow == null)
                mainWindow.AdvOptionsWindow = new AdvOptionsWindow();
            mainWindow.AdvOptionsWindow.Owner = mainWindow;
            mainWindow.AdvOptionsWindow.Show();
            mainWindow.AdvOptionsWindow.Activate();
        }
    }
}
