using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Advanced3 : UiGrid
    {
        public SettingsGrid_Advanced3()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;
        }
    }
}
