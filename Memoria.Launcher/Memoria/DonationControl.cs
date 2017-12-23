using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Memoria.Launcher
{
    public sealed class DonationControl : UiGrid
    {
        public DonationControl()
        {
            SetCols(2);
            SetRows(5);

            Width = 250;
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new Thickness(5);
            DataContext = this;
            ColumnDefinitions[0].Width = GridLength.Auto;

            LinearGradientBrush backgroundStroke = new LinearGradientBrush
            {
                EndPoint = new Point(0.5, 1),
                StartPoint = new Point(0.5, 0),
                RelativeTransform = new RotateTransform(115, 0.5, 0.5),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb(0xff, 0x61, 0x61, 0x61), 0),
                    new GradientStop(Color.FromArgb(0xff, 0xF2, 0xF2, 0xF2), 0.504),
                    new GradientStop(Color.FromArgb(0xff, 0xAE, 0xB1, 0xB1), 1)
                }
            };
            backgroundStroke.Freeze();

            LinearGradientBrush backgroundFill = new LinearGradientBrush
            {
                MappingMode = BrushMappingMode.RelativeToBoundingBox,
                StartPoint = new Point(0.5, 1.0),
                EndPoint = new Point(0.5, -0.4),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb(0xBB, 0x44, 0x71, 0xc1), 0),
                    new GradientStop(Color.FromArgb(0xBB, 0x28, 0x36, 0x65), 1)
                }
            };
            backgroundFill.Freeze();

            Rectangle backround = AddUiElement(new Rectangle { Stroke = backgroundStroke, Fill = backgroundFill, StrokeThickness = 5 }, 0, 0, 9, 2);

            UiTextBlock title = AddUiElement(UiTextBlockFactory.Create("Пожертвования"), 0, 0, 0, 2);
            title.FontSize = 18;
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.Margin = new Thickness(0, 4, 0, 0);

            AddUiElement(UiTextBoxFactory.Create("Яндекс: "), 1, 0);
            AddUiElement(UiTextBoxFactory.Create("410013254932482"), 1, 1);
            AddUiElement(UiTextBoxFactory.Create("WMR: "), 2, 0);
            AddUiElement(UiTextBoxFactory.Create("R255847965836"), 2, 1);
            AddUiElement(UiTextBoxFactory.Create("WMZ: "), 3, 0);
            AddUiElement(UiTextBoxFactory.Create("Z321220468886 "), 3, 1);
            AddUiElement(UiTextBoxFactory.Create("WME: "), 4, 0);
            AddUiElement(UiTextBoxFactory.Create("E223137827385"), 4, 1).Margin = new Thickness(0, 0, 0, 5);

            foreach (FrameworkElement child in Children)
            {
                if (!ReferenceEquals(child, backround))
                {
                    child.Margin = GetColumn(child) == 0
                        ? new Thickness(child.Margin.Left + 8, child.Margin.Top, child.Margin.Right, child.Margin.Bottom)
                        : new Thickness(child.Margin.Left, child.Margin.Top, child.Margin.Right + 8, child.Margin.Bottom);
                }

                TextBlock textBlock = child as TextBlock;
                if (textBlock != null)
                {
                    textBlock.Foreground = Brushes.WhiteSmoke;
                    textBlock.FontWeight = FontWeight.FromOpenTypeWeight(500);
                    continue;
                }

                TextBox textBox = child as TextBox;
                if (textBox != null)
                {
                    textBox.Foreground = Brushes.WhiteSmoke;
                    textBox.FontWeight = FontWeight.FromOpenTypeWeight(500);
                    textBox.Background = Brushes.Transparent;
                    textBox.BorderThickness = new Thickness(0);
                    textBox.IsReadOnly = true;
                }
            }
        }
    }
}