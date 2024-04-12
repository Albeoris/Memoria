using System;

namespace Memoria.Launcher
{
	public static class UiTextBlockFactory
	{
		public static UiTextBlock Create(String text)
		{
			UiTextBlock textBlock = new UiTextBlock { Text = text };

			return textBlock;
		}
	}
}
