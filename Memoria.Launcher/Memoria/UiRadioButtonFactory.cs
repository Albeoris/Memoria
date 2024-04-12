using System;

namespace Memoria.Launcher
{
	public static class UiRadioButtonFactory
	{
		public static UiRadioButton Create(String groupName, Object content, Boolean? isChecked)
		{
			return new UiRadioButton
			{
				Content = content,
				IsChecked = isChecked,
				GroupName = groupName
			};
		}
	}
}
