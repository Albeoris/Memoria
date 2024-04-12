using System.Windows;
using System.Windows.Media;

namespace Memoria.Launcher
{
	public static class FrameworkElementExm
	{
		public static FrameworkElement GetRootElement(this FrameworkElement self)
		{
			FrameworkElement element = self;
			while (element.Parent != null)
				element = (FrameworkElement)element.Parent;
			return element;
		}

		public static T GetParentElement<T>(this FrameworkElement self) where T : FrameworkElement
		{
			DependencyObject element = VisualTreeHelper.GetParent(self);
			while (element != null)
			{
				T result = element as T;
				if (result != null)
					return result;
				element = VisualTreeHelper.GetParent(element);
			}
			return null;
		}
	}
}
