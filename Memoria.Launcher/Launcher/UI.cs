using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Binding = System.Windows.Data.Binding;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;
using System.Collections;
namespace Memoria.Launcher
{
    public class UiGrid : Grid
    {
        public SolidColorBrush TextColor = Brushes.White;
        public Thickness CommonMargin = new Thickness(0, 3, 0, 3);
        public Int32 Row = -1;
        public Int32 MaxColumns = 8;
        public Int32 FontWeightNormal = 400;
        public Int32 FontWeightCombobox = 500;
        public Int32 FontSizeNormal = 14;
        public Int32 FontSizeCombobox = 10;
        public Int32 ComboboxHeight = 22;
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
        public T AddUiElement<T>(T uiElement, Int32 row, Int32 col, Int32 rowSpan = 0, Int32 colSpan = 0) where T : UIElement
        {
            if (row > 0) uiElement.SetValue(RowProperty, row);
            if (col > 0) uiElement.SetValue(ColumnProperty, col);
            if (rowSpan > 0) uiElement.SetValue(RowSpanProperty, rowSpan);
            if (colSpan > 0) uiElement.SetValue(ColumnSpanProperty, colSpan);

            Children.Add(uiElement);
            return uiElement;
        }
        public void CreateCheckbox(String property, object text, String tooltip = "", Int32 firstColumn = 0, String propertyToEnable = "")
        {
            if (firstColumn == 0)
            {
                Row++;
                RowDefinitions.Add(new RowDefinition());
            }
            CheckBox checkBox = new CheckBox();
            checkBox.Content = text;
            checkBox.IsChecked = null;
            if (tooltip != "")
                checkBox.ToolTip = tooltip;
            checkBox.Foreground = TextColor;
            checkBox.FontWeight = FontWeight.FromOpenTypeWeight(FontWeightNormal);
            checkBox.FontSize = FontSizeNormal;
            checkBox.SetBinding(ToggleButton.IsCheckedProperty, new Binding(property) { Mode = BindingMode.TwoWay });
            if (propertyToEnable != "")
                checkBox.SetBinding(ToggleButton.IsEnabledProperty, new Binding(propertyToEnable) { Mode = BindingMode.TwoWay });
            checkBox.Margin = CommonMargin;
            checkBox.Style = (Style)Application.Current.FindResource("ToggleStyle");
            checkBox.SetValue(RowProperty, Row);
            checkBox.SetValue(ColumnProperty, firstColumn);
            checkBox.SetValue(RowSpanProperty, 1);
            checkBox.SetValue(ColumnSpanProperty, MaxColumns);
            Children.Add(checkBox);
        }
        public void CreateTextbloc(String text, Boolean isHeading = false, String tooltip = "")
        {
            Row++;
            RowDefinitions.Add(new RowDefinition());
            TextBlock textbloc = new TextBlock();
            textbloc.Text = text;
            if (tooltip != "")
                textbloc.ToolTip = tooltip;
            textbloc.Foreground = TextColor;
            textbloc.Margin = new Thickness(0);

            Border border = new Border();
            textbloc.FontSize = FontSizeNormal;
            border.Margin = CommonMargin;
            border.SetValue(RowProperty, Row);
            border.SetValue(ColumnProperty, 0);
            border.SetValue(RowSpanProperty, 1);
            border.SetValue(ColumnSpanProperty, MaxColumns);

            if (isHeading)
            {
                textbloc.TextAlignment = TextAlignment.Center;  // Center the text
                textbloc.HorizontalAlignment = HorizontalAlignment.Center;
                textbloc.FontStretch = FontStretch.FromOpenTypeStretch(9);
                textbloc.FontWeight = FontWeight.FromOpenTypeWeight(500);
                textbloc.FontSize = 14;
                textbloc.Text = textbloc.Text.ToUpper();
                textbloc.SetValue(TextBlock.FontFamilyProperty, Application.Current.FindResource("CenturyGothic") as FontFamily);
                //border.Background = new SolidColorBrush(Color.FromArgb(0x88, 0x3A, 0x6B, 0x77));
                border.Background = (SolidColorBrush)Application.Current.FindResource("BrushAccentColor");
                border.Opacity = 0.8;
                border.CornerRadius = new CornerRadius(5);
                border.Padding = new Thickness(10, 2, 10, 2);
                border.Margin = new Thickness(0, 3, 0, 6); ;
                border.HorizontalAlignment = HorizontalAlignment.Stretch;
                border.VerticalAlignment = VerticalAlignment.Center;
            }
            border.Child = textbloc;
            Children.Add(border);
        }
        public void CreateCombobox(String property, IEnumerable options, Int32 firstColumn = 4, String tooltip = "", Boolean selectByName = false)
        {
            if (firstColumn == 0)
            {
                Row++;
                RowDefinitions.Add(new RowDefinition());
            }
            ComboBox comboBox = new ComboBox();
            comboBox.ItemsSource = options;
            if (tooltip != "")
                comboBox.ToolTip = tooltip;
            comboBox.Foreground = Brushes.Black;
            comboBox.FontWeight = FontWeight.FromOpenTypeWeight(FontWeightCombobox);
            comboBox.Margin = CommonMargin;
            comboBox.Height = ComboboxHeight;
            comboBox.FontSize = FontSizeCombobox;
            if (selectByName)
                comboBox.SetBinding(Selector.SelectedItemProperty, new Binding(property) { Mode = BindingMode.TwoWay });
            else
                comboBox.SetBinding(Selector.SelectedIndexProperty, new Binding(property) { Mode = BindingMode.TwoWay });
            comboBox.SetValue(RowProperty, Row);
            comboBox.SetValue(ColumnProperty, firstColumn);
            comboBox.SetValue(RowSpanProperty, 1);
            comboBox.SetValue(ColumnSpanProperty, MaxColumns);
            Children.Add(comboBox);
        }
        public void CreateSlider(String indexproperty, String sliderproperty, double min, double max, double tickFrequency, String stringFormat = "{0}", Int32 firstColumn = 1)
        {
            Row++;
            RowDefinitions.Add(new RowDefinition());

            TextBlock textbloc = new TextBlock();
            textbloc.Text = "";
            textbloc.SetBinding(TextBlock.TextProperty, new Binding(indexproperty) { Mode = BindingMode.TwoWay, StringFormat = stringFormat });
            textbloc.Foreground = TextColor;
            textbloc.FontWeight = FontWeight.FromOpenTypeWeight(FontWeightNormal);
            textbloc.FontSize = FontSizeNormal;
            textbloc.Margin = CommonMargin;
            textbloc.SetValue(RowProperty, Row);
            textbloc.SetValue(ColumnProperty, 0);
            textbloc.SetValue(RowSpanProperty, 1);
            textbloc.SetValue(ColumnSpanProperty, MaxColumns);
            Children.Add(textbloc);

            Slider slider = new Slider();
            slider.Value = 0;
            slider.SetBinding(Slider.ValueProperty, new Binding(sliderproperty) { Mode = BindingMode.TwoWay });
            slider.Height = 20;
            slider.Margin = CommonMargin;
            slider.Minimum = min;
            slider.Maximum = max;
            slider.TickFrequency = tickFrequency;
            slider.IsSnapToTickEnabled = true;
            slider.TickPlacement = TickPlacement.None;
            slider.SetValue(RowProperty, Row);
            slider.SetValue(ColumnProperty, firstColumn);
            slider.SetValue(RowSpanProperty, 1);
            slider.SetValue(ColumnSpanProperty, MaxColumns);
            Children.Add(slider);
        }
    }

    public static class UiTextBlockFactory
    {
        public static TextBlock Create(String text)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text
            };
            return textBlock;
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
    public sealed class UiLauncherModManagerButton : UiModManagerButton
    {
        public UiLauncherModManagerButton()
        {
            Label = Lang.Launcher.ModManager;
        }

        protected override async Task DoAction()
        {
            MainWindow mainWindow = (MainWindow)this.GetRootElement();
            if (mainWindow.ModdingWindow == null)
                mainWindow.ModdingWindow = new ModManagerWindow();
            mainWindow.ModdingWindow.Owner = mainWindow;
            mainWindow.ModdingWindow.Show();
            mainWindow.ModdingWindow.Activate();
        }
    }

    public sealed class UiLauncherModelViewerButton : UiModManagerButton
    {
        public UiLauncherModelViewerButton()
        {
            Label = Lang.Launcher.ModelViewer;
        }

        protected override async Task DoAction()
        {
            try
            {
                MainWindow mainWindow = (MainWindow)this.GetRootElement();
                mainWindow.PlayButton.Click(true);
            }
            catch (Exception) { }
        }
    }
}
