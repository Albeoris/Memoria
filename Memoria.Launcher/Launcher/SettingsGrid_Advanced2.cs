using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Advanced2 : UiGrid
    {
        public SettingsGrid_Advanced2()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;
        }
    }
}
