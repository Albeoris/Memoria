using System.Threading.Tasks;

namespace Memoria.Launcher
{
	public sealed class UiLauncherModManagerButton : UiModManagerButton
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
