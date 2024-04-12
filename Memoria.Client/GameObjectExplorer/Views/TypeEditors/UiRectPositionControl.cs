using System;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;
using Object = System.Object;

namespace Memoria.Client.GameObjectExplorer.Views.TypeEditors
{
	public sealed class UiRectPositionControl : UserControl
	{
		private readonly SingleUpDown _relative;
		private readonly IntegerUpDown _absolute;

		public UiRectPositionControl()
		{
			UiGrid grid = new UiGrid();
			grid.SetCols(4);
			grid.ColumnDefinitions[0].Width = GridLength.Auto;
			grid.ColumnDefinitions[2].Width = GridLength.Auto;

			TextBlock xLabel = grid.AddUiElement(new TextBlock { Text = "x:" }, 0, 0);
			xLabel.Margin = new Thickness(3, 3, 5, 3);

			_relative = grid.AddUiElement(new SingleUpDown(), 0, 1);
			_relative.Value = 0;
			_relative.Increment = 0.1f;
			_relative.FormatString = "F3";
			_relative.ValueChanged += RelativeChanged;
			_relative.Margin = new Thickness(3);

			TextBlock yLabel = grid.AddUiElement(new TextBlock { Text = "y:" }, 0, 2);
			yLabel.Margin = new Thickness(10, 3, 5, 3);

			_absolute = grid.AddUiElement(new IntegerUpDown(), 0, 3);
			_absolute.Value = 0;
			_absolute.Increment = 1;
			_absolute.FormatString = "F3";
			_absolute.ValueChanged += AbsoluteChanged;
			_absolute.Margin = new Thickness(3);

			this.Content = grid;
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(UIRect.Position), typeof(UiRectPositionControl), new PropertyMetadata(default(UIRect.Position), OnValueChanged));

		private Boolean _disableCallback;

		public UIRect.Position Value
		{
			get { return (UIRect.Position)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			UiRectPositionControl self = (UiRectPositionControl)d;
			if (self._disableCallback)
				return;

			UIRect.Position position = (UIRect.Position)e.NewValue;
			self._relative.Value = position.Relative;
			self._absolute.Value = position.Absolute;
		}

		private void RelativeChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			if (e.NewValue == null)
				throw new ArgumentException("NewValue");

			_disableCallback = true;
			Value = new UIRect.Position((Single)e.NewValue, Value.Absolute);
			_disableCallback = false;
		}

		private void AbsoluteChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			if (e.NewValue == null)
				throw new ArgumentException("NewValue");

			_disableCallback = true;
			Value = new UIRect.Position(Value.Relative, (Int32)e.NewValue);
			_disableCallback = false;
		}
	}
}
