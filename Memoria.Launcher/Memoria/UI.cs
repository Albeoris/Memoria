using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

    public class UiTextBlock : TextBlock
    {
    }
    public static class UiTextBlockFactory
    {
        public static UiTextBlock Create(String text)
        {
            UiTextBlock textBlock = new UiTextBlock { Text = text };

            return textBlock;
        }
    }

    public class UiCheckBox : CheckBox
    {
    }
    public static class UiCheckBoxFactory
    {
        public static UiCheckBox Create(Object content, Boolean? isChecked)
        {
            return new UiCheckBox
            {
                Content = content,
                IsChecked = isChecked
            };
        }
    }

    public class UiComboBox : ComboBox
    {
    }
    public static class UiComboBoxFactory
    {
        public static UiComboBox Create()
        {
            return new UiComboBox();
        }
    }

    public static class UiSliderFactory
    {
        public static Slider Create(int value)
        {
            return new Slider
            {
                Value = value
            };
        }
    }

    public static class UiHelper
    {
        public static void ShowError(FrameworkElement owner, Exception exception, String formatMessage = null, params Object[] args)
        {
            StringBuilder sb = new StringBuilder();

            if (!String.IsNullOrEmpty(formatMessage))
            {
                sb.AppendFormat(formatMessage, args);
                sb.AppendLine();
            }

            if (exception != null)
                sb.Append(exception);

            if (owner == null)
            {
                MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Window window = (Window)owner.GetRootElement();
                MessageBox.Show(window, sb.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    
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
