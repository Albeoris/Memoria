using System;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;
using Rect = UnityEngine.Rect;

namespace Memoria.Client.GameObjectExplorer.Views.TypeEditors
{
	public sealed class RectControl : UserControl
	{
		private readonly SingleUpDown _x;
		private readonly SingleUpDown _y;
		private readonly SingleUpDown _w;
		private readonly SingleUpDown _h;

		public RectControl()
		{
			UiGrid grid = new UiGrid();
			grid.SetCols(8);
			grid.ColumnDefinitions[0].Width = GridLength.Auto;
			grid.ColumnDefinitions[2].Width = GridLength.Auto;
			grid.ColumnDefinitions[4].Width = GridLength.Auto;
			grid.ColumnDefinitions[6].Width = GridLength.Auto;

			TextBlock xLabel = grid.AddUiElement(new TextBlock { Text = "x:" }, 0, 0);
			xLabel.Margin = new Thickness(3, 3, 5, 3);

			_x = grid.AddUiElement(new SingleUpDown(), 0, 1);
			_x.Value = 0;
			_x.Increment = 0.5f;
			_x.FormatString = "F3";
			_x.ValueChanged += XChanged;
			_x.Margin = new Thickness(3);

			TextBlock yLabel = grid.AddUiElement(new TextBlock { Text = "y:" }, 0, 2);
			yLabel.Margin = new Thickness(10, 3, 5, 3);

			_y = grid.AddUiElement(new SingleUpDown(), 0, 3);
			_y.Value = 0;
			_y.Increment = 0.5f;
			_y.FormatString = "F3";
			_y.ValueChanged += YChanged;
			_y.Margin = new Thickness(3);

			TextBlock wLabel = grid.AddUiElement(new TextBlock { Text = "width:" }, 0, 6);
			wLabel.Margin = new Thickness(10, 3, 5, 3);

			_w = grid.AddUiElement(new SingleUpDown(), 0, 7);
			_w.Value = 0;
			_w.Increment = 0.5f;
			_w.FormatString = "F3";
			_w.ValueChanged += WChanged;
			_w.Margin = new Thickness(3);

			TextBlock hLabel = grid.AddUiElement(new TextBlock { Text = "height:" }, 0, 4);
			hLabel.Margin = new Thickness(10, 3, 5, 3);

			_h = grid.AddUiElement(new SingleUpDown(), 0, 5);
			_h.Value = 0;
			_h.Increment = 0.5f;
			_h.FormatString = "F3";
			_h.ValueChanged += HChanged;
			_h.Margin = new Thickness(3);

			this.Content = grid;
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Rect), typeof(RectControl), new PropertyMetadata(default(Rect), OnValueChanged));

		private Boolean _disableCallback;

		public Rect Value
		{
			get { return (Rect)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RectControl self = (RectControl)d;
			if (self._disableCallback)
				return;

			Rect vector = (Rect)e.NewValue;
			self._x.Value = vector.x;
			self._y.Value = vector.y;
			self._w.Value = vector.width;
			self._h.Value = vector.height;
		}

		private void XChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			if (e.NewValue == null)
				throw new ArgumentException("NewValue");

			Rect vector = Value;
			vector.x = (Single)e.NewValue;

			_disableCallback = true;
			Value = vector;
			_disableCallback = false;
		}

		private void YChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			if (e.NewValue == null)
				throw new ArgumentException("NewValue");

			Rect vector = Value;
			vector.y = (Single)e.NewValue;

			_disableCallback = true;
			Value = vector;
			_disableCallback = false;
		}

		private void WChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			if (e.NewValue == null)
				throw new ArgumentException("NewValue");

			Rect vector = Value;
			vector.width = (Single)e.NewValue;

			_disableCallback = true;
			Value = vector;
			_disableCallback = false;
		}

		private void HChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			if (e.NewValue == null)
				throw new ArgumentException("NewValue");

			Rect vector = Value;
			vector.height = (Single)e.NewValue;

			_disableCallback = true;
			Value = vector;
			_disableCallback = false;
		}
	}
}
