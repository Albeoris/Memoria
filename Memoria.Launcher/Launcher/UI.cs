﻿using System;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using Binding = System.Windows.Data.Binding;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using Image = System.Windows.Controls.Image;
using MessageBox = System.Windows.MessageBox;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Input;
namespace Memoria.Launcher
{
    public class UiGrid : Grid
    {
        public SolidColorBrush TextColor = Brushes.White;
        public Thickness CommonMargin = new Thickness(0, 3, 0, 3);
        public Int32 Row = -1;
        public Int32 MaxColumns = 100;
        public Int32 FontWeightNormal = 400;
        public Int32 FontWeightCombobox = 500;
        public Int32 FontSizeNormal = 14;
        //public Int32 FontSizeCombobox = 14;
        public Int32 ComboboxHeight = 22;
        public Int32 TooltipDisplayDelay = 1;
        public void SetRows(Int32 count)
        {
            count -= RowDefinitions.Count;
            if (count > 1) while (count-- > 0) RowDefinitions.Add(new RowDefinition());
        }
        public void SetCols(Int32 count)
        {
            count = 100;
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
        public void MakeTooltip(FrameworkElement uiElement, String text = "", String imageName = "")
        {
            if (text != "" || imageName != "")
            {
                StackPanel tooltipStackPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(0)
                };
                Image helpIcon = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/images/helpicon.png")),
                    MaxWidth = 28,
                    Opacity = 1,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 0, 0, 0)
                };
                tooltipStackPanel.Children.Add(helpIcon);

                if (text != "")
                {
                    DropShadowEffect dropShadow = new DropShadowEffect
                    {
                        Color = (Color)ColorConverter.ConvertFromString("#7f93a8"),  // Your specific color
                        BlurRadius = 0,
                        ShadowDepth = 1,
                        Direction = 320,
                        Opacity = 1
                    };
                    TextBlock tooltipTextBlock = new TextBlock
                    {
                        Text = text,
                        Opacity = 1,
                        MaxWidth = 275,
                        FontSize = 12,
                        TextWrapping = TextWrapping.Wrap,
                        Effect = dropShadow,
                        Margin = new Thickness(0)
                    };
                    tooltipStackPanel.Children.Add(tooltipTextBlock);
                }

                if (imageName != "")
                {
                    String imagePath = "pack://application:,,,/images/" + imageName;
                    Image tooltipImage = new Image
                    {
                        Source = new BitmapImage(new Uri(imagePath)),
                        MaxWidth = 275,
                        MaxHeight = 150,
                        Opacity = 1,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0)
                    };
                    tooltipStackPanel.Children.Add(tooltipImage);
                }

                Border bottomrightBorder = new Border
                {
                    BorderThickness = new Thickness(0, 0, 2, 2),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#487870")),
                    Padding = new Thickness(5, 0, 5, 5),
                    CornerRadius = new CornerRadius(7, 0, 7, 0),
                    Child = tooltipStackPanel
                };
                Border topleftBorder = new Border
                {
                    BorderThickness = new Thickness(2, 2, 0, 0),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#98c8b8")),
                    CornerRadius = new CornerRadius(7, 0, 7, 0),
                    Background = Brushes.Transparent,
                    Child = bottomrightBorder
                };
                Border outerBorder = new Border
                {
                    BorderThickness = new Thickness(2, 2, 2, 2),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#202830")),
                    CornerRadius = new CornerRadius(7, 0, 7, 0),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#78b0a7")),
                    Child = topleftBorder
                };
                ToolTip toolTip = new ToolTip
                {
                    Content = outerBorder,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#383840")),
                    Background = Brushes.Transparent,
                    HasDropShadow = false,
                    BorderBrush = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(0),
                    Margin = new Thickness(0),
                    PlacementTarget = uiElement,
                    Placement = PlacementMode.Bottom,
                    VerticalOffset = 0,
                    HorizontalOffset = 0,
                    ForceCursor = true,
                    Opacity = 1
                };
                //toolTip.Opened += (sender, e) => { Mouse.OverrideCursor = new Cursor(Application.GetResourceStream(new Uri("pack://application:,,,/images/moogle.cur")).Stream); };
                //toolTip.Closed += (sender, e) => { Mouse.OverrideCursor = null; };
                uiElement.MouseEnter += (sender, e) =>
                {
                    ToolTipService.SetToolTip(uiElement, toolTip);
                    ToolTipService.SetInitialShowDelay(uiElement, 0);
                    toolTip.IsOpen = true;
                    if (uiElement.IsMouseOver) Mouse.OverrideCursor = new Cursor(Application.GetResourceStream(new Uri("pack://application:,,,/images/moogle.cur")).Stream);
                };
                uiElement.MouseLeave += (sender, e) =>
                {
                    toolTip.IsOpen = false; // Force close the tooltip when the mouse leaves the element
                    ToolTipService.SetToolTip(uiElement, null);
                    if (!uiElement.IsMouseOver) Mouse.OverrideCursor = null;
                };
            }
        }
        public void CreateCheckbox(String property, object text, String tooltip = "", Int32 firstColumn = 0, String propertyToEnable = "", String tooltipImage = "")
        {
            if (firstColumn == 0)
            {
                Row++;
                RowDefinitions.Add(new RowDefinition());
            }
            CheckBox checkBox = new CheckBox();
            checkBox.Content = text;
            checkBox.IsChecked = null;
            MakeTooltip(checkBox, tooltip, tooltipImage);
            checkBox.Foreground = TextColor;
            checkBox.FontWeight = FontWeight.FromOpenTypeWeight(FontWeightNormal);
            checkBox.FontSize = FontSizeNormal;
            checkBox.SetBinding(ToggleButton.IsCheckedProperty, new Binding(property) { Mode = BindingMode.TwoWay });
            if (propertyToEnable != "")
                checkBox.SetBinding(ToggleButton.IsEnabledProperty, new Binding(propertyToEnable) { Mode = BindingMode.TwoWay });
            checkBox.VerticalContentAlignment = VerticalAlignment.Center;
            checkBox.Margin = new Thickness(0);
            checkBox.Height = 28;
            checkBox.Style = (Style)Application.Current.FindResource("ToggleStyle");
            checkBox.SetValue(RowProperty, Row);
            checkBox.SetValue(ColumnProperty, firstColumn);
            checkBox.SetValue(RowSpanProperty, 1);
            checkBox.SetValue(ColumnSpanProperty, MaxColumns);
            Children.Add(checkBox);
        }
        public void CreateTextbloc(String text, String tooltip = "", String tooltipImage = "", Int32 columns = 100)
        {
            Row++;
            RowDefinitions.Add(new RowDefinition());
            TextBlock textbloc = new TextBlock();
            textbloc.Text = text;
            MakeTooltip(textbloc, tooltip, tooltipImage);
            textbloc.Foreground = TextColor;
            Border border = new Border();
            textbloc.FontSize = FontSizeNormal;
            textbloc.VerticalAlignment = VerticalAlignment.Center;
            textbloc.Margin = new Thickness(0);
            textbloc.Padding = new Thickness(0, 4, 0, 6);
            border.VerticalAlignment = VerticalAlignment.Center;
            border.SetValue(RowProperty, Row);
            border.SetValue(ColumnProperty, 0);
            border.SetValue(RowSpanProperty, 1);
            border.SetValue(ColumnSpanProperty, columns);
            border.Child = textbloc;
            Children.Add(border);
        }

        public void CreateHeading(String text)
        {
            Row++;
            RowDefinitions.Add(new RowDefinition());
            TextBlock textbloc = new TextBlock();
            textbloc.Text = text.ToUpper();
            textbloc.Foreground = TextColor;
            textbloc.TextAlignment = TextAlignment.Center;
            textbloc.HorizontalAlignment = HorizontalAlignment.Center;
            textbloc.FontStretch = FontStretch.FromOpenTypeStretch(9);
            textbloc.FontWeight = FontWeight.FromOpenTypeWeight(500);
            textbloc.FontSize = 14;
            textbloc.VerticalAlignment = VerticalAlignment.Center;
            textbloc.Margin = new Thickness(0);
            textbloc.Height = 18;
            textbloc.Padding = new Thickness(0);
            textbloc.SetValue(TextBlock.FontFamilyProperty, Application.Current.FindResource("CenturyGothic") as FontFamily);
            Border border = new Border();
            border.SetValue(RowProperty, Row);
            border.SetValue(ColumnProperty, 0);
            border.SetValue(RowSpanProperty, 1);
            border.SetValue(ColumnSpanProperty, 100);
            border.Background = (SolidColorBrush)Application.Current.FindResource("BrushAccentColor");
            border.Opacity = 0.8;
            border.CornerRadius = new CornerRadius(5);
            border.Margin = new Thickness(0, 7, 0, 3);
            border.Height = 20;
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            border.VerticalAlignment = VerticalAlignment.Center;
            border.Child = textbloc;
            Children.Add(border);
        }
        public void CreateCombobox(String property, IEnumerable options, Int32 firstColumn = 50, String text = "", String tooltip = "", String tooltipImage = "", Boolean selectByName = false)
        {
            if (firstColumn == 0 || text != "")
            {
                Row++;
                RowDefinitions.Add(new RowDefinition());
            }
            if (text != "")
            {
                CreateTextbloc(text, tooltip, tooltipImage, firstColumn);
            }
            ComboBox comboBox = new ComboBox();
            comboBox.ItemsSource = options;
            MakeTooltip(comboBox, tooltip, tooltipImage);
            comboBox.Foreground = Brushes.Black;
            comboBox.FontWeight = FontWeight.FromOpenTypeWeight(FontWeightCombobox);
            comboBox.Margin = CommonMargin;
            comboBox.Height = ComboboxHeight;
            if (selectByName)
                comboBox.SetBinding(Selector.SelectedItemProperty, new Binding(property) { Mode = BindingMode.TwoWay });
            else
                comboBox.SetBinding(Selector.SelectedIndexProperty, new Binding(property) { Mode = BindingMode.TwoWay });
            comboBox.SetValue(RowProperty, Row);
            comboBox.SetValue(ColumnProperty, firstColumn);
            comboBox.SetValue(RowSpanProperty, 1);
            comboBox.SetValue(ColumnSpanProperty, MaxColumns - firstColumn);
            Children.Add(comboBox);
        }
        public void CreateSlider(String indexproperty, String sliderproperty, double min, double max, double tickFrequency, String stringFormat = "", Int32 firstColumn = 0, String text = "", String tooltip = "", String tooltipImage = "")
        {
            if (firstColumn == 0 || text != "")
            {
                Row++;
                RowDefinitions.Add(new RowDefinition());
            }
            if (text != "" && firstColumn > 0)
            {
                CreateTextbloc(text, tooltip, tooltipImage, firstColumn);
            }
            Slider slider = new Slider();
            slider.Value = 0;
            slider.SetBinding(Slider.ValueProperty, new Binding(sliderproperty) { Mode = BindingMode.TwoWay });
            slider.VerticalContentAlignment = VerticalAlignment.Center;
            slider.VerticalAlignment = VerticalAlignment.Center;
            slider.Margin = new Thickness(0);
            slider.Minimum = min;
            slider.Maximum = max;
            slider.TickFrequency = tickFrequency;
            slider.IsSnapToTickEnabled = true;
            slider.TickPlacement = TickPlacement.None;
            slider.SetValue(RowProperty, Row);
            slider.SetValue(ColumnProperty, firstColumn);
            slider.SetValue(RowSpanProperty, 1);
            slider.SetValue(ColumnSpanProperty, (MaxColumns - firstColumn - 12));
            Children.Add(slider);

            TextBlock textbloc = new TextBlock();
            textbloc.Text = "";
            if (stringFormat == "") stringFormat = "{0}";
            textbloc.SetBinding(TextBlock.TextProperty, new Binding(indexproperty) { Mode = BindingMode.TwoWay, StringFormat = stringFormat });
            textbloc.Foreground = TextColor;
            //textbloc.Background = Brushes.Black;
            textbloc.Opacity = 0.6;
            textbloc.FontWeight = FontWeight.FromOpenTypeWeight(FontWeightNormal);
            textbloc.FontSize = FontSizeNormal;
            textbloc.HorizontalAlignment = HorizontalAlignment.Right;
            textbloc.TextAlignment = TextAlignment.Right;
            textbloc.VerticalAlignment = VerticalAlignment.Center;
            textbloc.Margin = new Thickness(0);
            textbloc.Padding = new Thickness(0, 4, 0, 6);
            textbloc.Height = 28;
            textbloc.SetValue(RowProperty, Row);
            textbloc.SetValue(ColumnProperty, firstColumn);
            textbloc.SetValue(RowSpanProperty, 1);
            textbloc.SetValue(ColumnSpanProperty, MaxColumns);
            Children.Add(textbloc);
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