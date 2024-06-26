using System;
using System.Windows;
using System.Windows.Controls;
using UnityEngine;
using Xceed.Wpf.Toolkit;
using Object = System.Object;

namespace Memoria.Client.GameObjectExplorer.Views.TypeEditors
{
    public sealed class Vector3Control : UserControl
    {
        private readonly SingleUpDown _x;
        private readonly SingleUpDown _y;
        private readonly SingleUpDown _z;

        public Vector3Control()
        {
            UiGrid grid = new UiGrid();
            grid.SetCols(6);
            grid.ColumnDefinitions[0].Width = GridLength.Auto;
            grid.ColumnDefinitions[2].Width = GridLength.Auto;
            grid.ColumnDefinitions[4].Width = GridLength.Auto;

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

            this.Content = grid;
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Vector3), typeof(Vector3Control), new PropertyMetadata(default(Vector3), OnValueChanged));

        private Boolean _disableCallback;

        public Vector3 Value
        {
            get { return (Vector3)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Vector3Control self = (Vector3Control)d;
            if (self._disableCallback)
                return;

            Vector3 vector = (Vector3)e.NewValue;
            self._x.Value = vector.x;
            self._y.Value = vector.y;
            self._z.Value = vector.z;
        }

        private void XChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            if (e.NewValue == null)
                throw new ArgumentException("NewValue");

            Vector3 vector = Value;
            vector.x = (Single)e.NewValue;

            _disableCallback = true;
            Value = vector;
            _disableCallback = false;
        }

        private void YChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            if (e.NewValue == null)
                throw new ArgumentException("NewValue");

            Vector3 vector = Value;
            vector.y = (Single)e.NewValue;

            _disableCallback = true;
            Value = vector;
            _disableCallback = false;
        }

        private void ZChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            if (e.NewValue == null)
                throw new ArgumentException("NewValue");

            Vector3 vector = Value;
            vector.z = (Single)e.NewValue;

            _disableCallback = true;
            Value = vector;
            _disableCallback = false;
        }
    }
}
