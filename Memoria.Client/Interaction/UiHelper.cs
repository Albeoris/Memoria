using Memoria.Prime;
using Memoria.Prime.Text;
using System;
using System.Text;
using System.Windows;

namespace Memoria.Client.Interaction
{
	public static class UiHelper
	{
		public static void ShowError(FrameworkElement owner, Exception exception, String formatMessage = null, params Object[] args)
		{
			Log.Error(exception, formatMessage, args);

			StringBuilder sb = new StringBuilder();

			if (!String.IsNullOrEmpty(formatMessage))
				sb.AppendFormatLine(formatMessage, args);
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
