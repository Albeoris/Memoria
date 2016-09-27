using System;
using System.Threading.Tasks;
using System.Windows;

namespace Memoria.Launcher
{
    public sealed class UiLauncherExitButton : UiLauncherButton
    {
        private const String ExitLabel = "Exit";
        private const String ExitingLabel = "Exiting...";

        public UiLauncherExitButton()
        {
            Label = ExitLabel;
        }

        protected override async Task DoAction()
        {
            Label = ExitingLabel;
            try
            {
                ((Window)this.GetRootElement()).Close();
            }
            finally
            {
                Label = ExitLabel;
            }
        }
    }
}