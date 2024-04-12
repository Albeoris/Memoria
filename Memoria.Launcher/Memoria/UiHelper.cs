using System;
using System.Text;
using System.Windows;

namespace Memoria.Launcher
{
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
}
