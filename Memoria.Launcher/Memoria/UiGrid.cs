using System;
using System.Windows;
using System.Windows.Controls;

namespace Memoria.Launcher
{
	public class UiGrid : Grid
	{
		public void SetRows(Int32 count)
		{
			count -= RowDefinitions.Count;
			if (count > 1) while (count-- > 0) RowDefinitions.Add(new RowDefinition());
		}

		public void SetCols(Int32 count)
		{
			count -= ColumnDefinitions.Count;
			if (count > 1) while (count-- > 0) ColumnDefinitions.Add(new ColumnDefinition());
		}

		public void SetRowsHeight(GridLength height)
		{
			foreach (RowDefinition row in RowDefinitions)
				row.Height = height;
		}

		public void SetColsWidth(GridLength width)
		{
			foreach (ColumnDefinition col in ColumnDefinitions)
				col.Width = width;
		}

		public T AddUiElement<T>(T uiElement, Int32 row, Int32 col, Int32 rowSpan = 0, Int32 colSpan = 0) where T : UIElement
		{
			if (row > 0) uiElement.SetValue(RowProperty, row);
			if (col > 0) uiElement.SetValue(ColumnProperty, col);
			if (rowSpan > 0) uiElement.SetValue(RowSpanProperty, rowSpan);
			if (colSpan > 0) uiElement.SetValue(ColumnSpanProperty, colSpan);

			Children.Add(uiElement);
			return uiElement;
		}

		public void SetChildrenMargin(Double uniformLength)
		{
			Thickness margin = new Thickness(uniformLength);
			foreach (FrameworkElement child in Children)
				child.Margin = margin;
		}
	}
}
