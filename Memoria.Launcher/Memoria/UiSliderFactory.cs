namespace Memoria.Launcher
{
	public static class UiSliderFactory
	{
		public static UiSlider Create(int value)
		{
			return new UiSlider
			{
				Value = value
			};
		}
	}
}
