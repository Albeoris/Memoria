using System;
using System.Windows;
using System.Windows.Controls;
using UnityEngine;
using Xceed.Wpf.Toolkit;
using Object = System.Object;

namespace Memoria.Client.GameObjectExplorer.Views.TypeEditors
{
	public sealed class QuaternionControl : UserControl
	{
		private readonly SingleUpDown _x;
		private readonly SingleUpDown _y;
		private readonly SingleUpDown _z;
		private readonly SingleUpDown _w;

		public QuaternionControl()
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

			TextBlock zLabel = grid.AddUiElement(new TextBlock { Text = "z:" }, 0, 4);
			zLabel.Margin = new Thickness(10, 3, 5, 3);

			_z = grid.AddUiElement(new SingleUpDown(), 0, 5);
			_z.Value = 0;
			_z.Increment = 0.5f;
			_z.FormatString = "F3";
			_z.ValueChanged += ZChanged;
			_z.Margin = new Thickness(3);

			TextBlock wLabel = grid.AddUiElement(new TextBlock { Text = "w:" }, 0, 6);
			wLabel.Margin = new Thickness(10, 3, 5, 3);

			_w = grid.AddUiElement(new SingleUpDown(), 0, 7);
			_w.Value = 0;
			_w.Increment = 0.5f;
			_w.FormatString = "F3";
			_w.ValueChanged += WChanged;
			_w.Margin = new Thickness(3);

			this.Content = grid;
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Quaternion), typeof(QuaternionControl), new PropertyMetadata(default(Quaternion), OnValueChanged));

		private Boolean _disableCallback;

		public Quaternion Value
		{
			get { return (Quaternion)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			QuaternionControl self = (QuaternionControl)d;
			if (self._disableCallback)
				return;

			Quaternion vector = (Quaternion)e.NewValue;
			self._x.Value = vector.x;
			self._y.Value = vector.y;
			self._z.Value = vector.z;
			self._w.Value = vector.w;
		}

		private void XChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			if (e.NewValue == null)
				throw new ArgumentException("NewValue");

			Quaternion vector = Value;
			vector.x = (Single)e.NewValue;

			_disableCallback = true;
			Value = vector;
			_disableCallback = false;
		}

		private void YChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			if (e.NewValue == null)
				throw new ArgumentException("NewValue");

			Quaternion vector = Value;
			vector.y = (Single)e.NewValue;

			_disableCallback = true;
			Value = vector;
			_disableCallback = false;
		}

		private void ZChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			if (e.NewValue == null)
				throw new ArgumentException("NewValue");

			Quaternion vector = Value;
			vector.z = (Single)e.NewValue;

			_disableCallback = true;
			Value = vector;
			_disableCallback = false;
		}

		private void WChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			if (e.NewValue == null)
				throw new ArgumentException("NewValue");

			Quaternion vector = Value;
			vector.w = (Single)e.NewValue;

			_disableCallback = true;
			Value = vector;
			_disableCallback = false;
		}
	}
}
