using System;
using System.Windows;
using System.Windows.Controls;

namespace Memoria.Client
{
	public class UiGrid : Grid
	{
		public void SetRows(int count)
		{
			count -= RowDefinitions.Count;
			if (count > 1) while (count-- > 0) RowDefinitions.Add(new RowDefinition());
		}

		public void SetCols(int count)
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

		public T AddUiElement<T>(T uiElement, int row, int col, int rowSpan = 0, int colSpan = 0) where T : UIElement
		{
			if (row > 0) uiElement.SetValue(RowProperty, row);
			if (col > 0) uiElement.SetValue(ColumnProperty, col);
			if (rowSpan > 0) uiElement.SetValue(RowSpanProperty, rowSpan);
			if (colSpan > 0) uiElement.SetValue(ColumnSpanProperty, colSpan);

			Children.Add(uiElement);
			return uiElement;
		}

		public GridSplitter AddVerticalSplitter(int col, int row = 0, int rowSpan = 0)
		{
			GridSplitter splitter = new GridSplitter
			{
				Width = 5,
				VerticalAlignment = VerticalAlignment.Stretch,
				HorizontalAlignment = HorizontalAlignment.Center
			};

			AddUiElement(splitter, row, col, rowSpan);
			return splitter;
		}

		public GridSplitter AddHorizontalSplitter(int row, int col = 0, int colSpan = 0)
		{
			GridSplitter splitter = new GridSplitter
			{
				Width = 5,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Center
			};

			AddUiElement(splitter, row, col, 0, colSpan);
			return splitter;
		}

		public void SetChildrenMargin(Double uniformLength)
		{
			Thickness margin = new Thickness(uniformLength);
			foreach (FrameworkElement child in Children)
				child.Margin = margin;
		}
	}
}
