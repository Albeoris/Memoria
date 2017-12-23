using System;
using System.Threading.Tasks;
using System.Windows;

namespace Memoria.Launcher
{
    public sealed class UiLauncherExitButton : UiLauncherButton
    {
        public UiLauncherExitButton()
        {
            Label = Lang.Button.Exit;
        }

        protected override async Task DoAction()
        {
            Label = Lang.Button.Exiting;
            try
            {
                ((Window)this.GetRootElement()).Close();
            }
            finally
            {
                Label = Lang.Button.Exit;
            }
        }
    }
}