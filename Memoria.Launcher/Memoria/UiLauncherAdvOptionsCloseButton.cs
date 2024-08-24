using System;
using System.Threading.Tasks;
using System.Windows;

namespace Memoria.Launcher
{
    public sealed class UiLauncherAdvOptionsCloseButton : UiModManagerButton
    {
        public UiLauncherAdvOptionsCloseButton()
        {
            Label = "↩ " + Lang.Launcher.Return;
        }

        protected override async Task DoAction()
        {
            try
            {
                ((Window)this.GetRootElement()).Close();
            }
            catch (Exception) { }
        }
    }
}
