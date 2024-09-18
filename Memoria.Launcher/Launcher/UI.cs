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
        public Thickness CommonMargin = new Thickness(0, 2, 0, 2);
        public Int32 Row = -1;
        public Int32 MaxColumns = 8;
        public Int32 FontWeightNormal = 500;
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
            checkBox.SetBinding(ToggleButton.IsCheckedProperty, new Binding(property) { Mode = BindingMode.TwoWay });
            if (propertyToEnable != "")
                checkBox.SetBinding(ToggleButton.IsEnabledProperty, new Binding(propertyToEnable) { Mode = BindingMode.TwoWay });
            checkBox.Margin = CommonMargin;
            checkBox.SetValue(RowProperty, Row);
            checkBox.SetValue(ColumnProperty, firstColumn);
            checkBox.SetValue(RowSpanProperty, 1);
            checkBox.SetValue(ColumnSpanProperty, MaxColumns);
            Children.Add(checkBox);
        }
        public void CreateTextbloc(String text, Boolean isBold = false, String tooltip = "")
        {
            Row++;
            RowDefinitions.Add(new RowDefinition());
            TextBlock textbloc = new TextBlock();
            textbloc.Text = text;
            if (tooltip != "")
                textbloc.ToolTip = tooltip;
            textbloc.Foreground = TextColor;
            textbloc.FontWeight = isBold ? FontWeights.Bold : FontWeight.FromOpenTypeWeight(FontWeightNormal);
            textbloc.Margin = CommonMargin;
            textbloc.SetValue(RowProperty, Row);
            textbloc.SetValue(ColumnProperty, 0);
            textbloc.SetValue(RowSpanProperty, 1);
            textbloc.SetValue(ColumnSpanProperty, MaxColumns);
            Children.Add(textbloc);
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
            comboBox.FontWeight = FontWeight.FromOpenTypeWeight(FontWeightNormal);
            comboBox.Margin = CommonMargin;
            comboBox.Height = 18;
            comboBox.FontSize = 10;
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
            textbloc.Margin = CommonMargin;
            textbloc.SetValue(RowProperty, Row);
            textbloc.SetValue(ColumnProperty, 0);
            textbloc.SetValue(RowSpanProperty, 1);
            textbloc.SetValue(ColumnSpanProperty, MaxColumns);
            Children.Add(textbloc);

            Slider slider = new Slider();
            slider.Value = 0;
            slider.SetBinding(Slider.ValueProperty, new Binding(sliderproperty) { Mode = BindingMode.TwoWay });
            slider.Height = 18;
            slider.Margin = CommonMargin;
            slider.Minimum = min;
            slider.Maximum = max;
            slider.TickFrequency = tickFrequency;
            slider.IsSnapToTickEnabled = true;
            slider.TickPlacement = TickPlacement.BottomRight;
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

    public static class UiCheckBoxFactory
    {
        public static CheckBox Create(Object content, Boolean? isChecked)
        {
            CheckBox checkBox = new CheckBox
            {
                Content = content,
                IsChecked = isChecked
            };
            return checkBox;
        }
    }

    public static class UiComboBoxFactory
    {
        public static ComboBox Create()
        {
            ComboBox comboBox = new ComboBox
            {

            };
            return comboBox;
        }
    }

    public static class UiSliderFactory
    {
        public static Slider Create(int value)
        {
            Slider slider = new Slider
            {
                Value = value
            };
            return slider;
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

    public sealed class UiLauncherAdvOptionsButton : UiModManagerButton
    {
        public UiLauncherAdvOptionsButton()
        {
            Label = Lang.Launcher.AdvSettings;
        }

        protected override async Task DoAction()
        {
            MainWindow mainWindow = (MainWindow)this.GetRootElement();
            if (mainWindow.AdvOptionsWindow == null)
                mainWindow.AdvOptionsWindow = new AdvOptionsWindow();
            mainWindow.AdvOptionsWindow.Owner = mainWindow;
            mainWindow.AdvOptionsWindow.Show();
            mainWindow.AdvOptionsWindow.Activate();
        }
    }

    public sealed class UiLauncherAdvOptionsCloseButton : UiModManagerButton
    {
        public UiLauncherAdvOptionsCloseButton()
        {
            Label = "↩ " + Lang.Launcher.Return;
        }

        protected override async Task DoAction()
        {
            try
            {
                ((Window)this.GetRootElement()).Close();
            }
            catch (Exception) { }
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
                Window adv = (Window)this.GetRootElement();
                MainWindow mainWindow = (MainWindow)adv.Owner;
                mainWindow.PlayButton.Click(true);
            }
            catch (Exception) { }
        }
    }
}
