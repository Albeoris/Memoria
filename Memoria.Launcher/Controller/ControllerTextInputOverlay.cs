using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Memoria.Launcher.Controller
{
    /// <summary>
    /// Minimal controller-operated text keyboard for launcher search and preset fields.
    /// </summary>
    internal sealed class ControllerTextInputOverlay : Grid
    {
        private static readonly String[] KeyRows =
        {
            "1234567890",
            "QWERTYUIOP",
            "ASDFGHJKL",
            "ZXCVBNM-_.'"
        };

        private readonly TextBox _target;
        private readonly String _originalText;
        private readonly TextBox _preview;

        public ControllerTextInputOverlay(TextBox target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _originalText = target.Text;

            Background = new SolidColorBrush(Color.FromArgb(176, 0, 0, 0));
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            // Keep the controller mouse-input shield above this visual while
            // still presenting the keyboard above regular launcher content.
            Panel.SetZIndex(this, Int32.MaxValue - 1);
            GamepadNavigation.SetIsModalScope(this, true);

            Border window = new Border
            {
                Width = 660,
                Padding = new Thickness(18),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush(Color.FromArgb(245, 10, 10, 10)),
                BorderBrush = TryFindResource("BrushAccentColor") as Brush ?? Brushes.SteelBlue,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(8)
            };

            StackPanel content = new StackPanel();
            _preview = new TextBox
            {
                Text = _target.Text,
                IsReadOnly = true,
                Focusable = false,
                FontSize = 19,
                Height = 38,
                Margin = new Thickness(2, 2, 2, 12),
                Padding = new Thickness(8, 4, 8, 4),
                CaretIndex = _target.CaretIndex
            };
            content.Children.Add(_preview);

            Boolean isFirstKey = true;
            foreach (String row in KeyRows)
            {
                UniformGrid keyRow = new UniformGrid { Columns = row.Length };
                foreach (Char character in row)
                {
                    Button key = CreateButton(character.ToString());
                    Char captured = character;
                    key.Click += (sender, args) => Insert(captured.ToString());
                    if (isFirstKey)
                    {
                        GamepadNavigation.SetIsDefaultFocus(key, true);
                        isFirstKey = false;
                    }
                    keyRow.Children.Add(key);
                }
                content.Children.Add(keyRow);
            }

            UniformGrid commandRow = new UniformGrid { Columns = 5, Margin = new Thickness(0, 8, 0, 0) };
            Button backspace = CreateButton("⌫");
            backspace.Click += (sender, args) => Backspace();
            commandRow.Children.Add(backspace);

            Button space = CreateButton("␠");
            space.Click += (sender, args) => Insert(" ");
            commandRow.Children.Add(space);

            Button clear = CreateButton("×");
            clear.Click += (sender, args) => SetText(String.Empty, 0);
            commandRow.Children.Add(clear);

            Button cancel = CreateButton(null);
            cancel.Name = "Cancel";
            cancel.SetResourceReference(ContentControl.ContentProperty, "Launcher.Cancel");
            cancel.Click += (sender, args) => Close(false);
            commandRow.Children.Add(cancel);

            Button done = CreateButton(null);
            done.Name = "Ok";
            done.SetResourceReference(ContentControl.ContentProperty, "Launcher.OK");
            done.Click += (sender, args) => Close(true);
            commandRow.Children.Add(done);

            content.Children.Add(commandRow);
            window.Child = content;
            Children.Add(window);
        }

        private Button CreateButton(String content)
        {
            Button button = new Button
            {
                Content = content,
                Height = 42,
                Margin = new Thickness(2),
                FontSize = 16
            };
            Style style = TryFindResource("ButtonStyle") as Style;
            if (style != null)
                button.Style = style;
            return button;
        }

        private void Insert(String value)
        {
            Int32 selectionStart = Math.Max(0, Math.Min(_target.Text.Length, _target.SelectionStart));
            Int32 selectionLength = Math.Max(0, Math.Min(_target.Text.Length - selectionStart, _target.SelectionLength));
            Int32 available = _target.MaxLength <= 0
                ? value.Length
                : Math.Max(0, _target.MaxLength - (_target.Text.Length - selectionLength));
            String inserted = value.Substring(0, Math.Min(value.Length, available));
            String text = _target.Text.Remove(selectionStart, selectionLength).Insert(selectionStart, inserted);
            SetText(text, selectionStart + inserted.Length);
        }

        private void Backspace()
        {
            Int32 selectionStart = Math.Max(0, Math.Min(_target.Text.Length, _target.SelectionStart));
            Int32 selectionLength = Math.Max(0, Math.Min(_target.Text.Length - selectionStart, _target.SelectionLength));
            if (selectionLength > 0)
            {
                SetText(_target.Text.Remove(selectionStart, selectionLength), selectionStart);
                return;
            }
            if (selectionStart > 0)
                SetText(_target.Text.Remove(selectionStart - 1, 1), selectionStart - 1);
        }

        private void SetText(String text, Int32 caretIndex)
        {
            _target.Text = text;
            _target.CaretIndex = Math.Max(0, Math.Min(text.Length, caretIndex));
            _target.SelectionLength = 0;
            _preview.Text = text;
            _preview.CaretIndex = _target.CaretIndex;
            _preview.ScrollToEnd();
        }

        internal void Accept()
        {
            Close(true);
        }

        private void Close(Boolean accept)
        {
            if (!accept)
                SetText(_originalText, Math.Min(_originalText.Length, _target.CaretIndex));

            Panel parent = Parent as Panel;
            if (parent != null)
                parent.Children.Remove(this);
            _target.Focus();
            Keyboard.Focus(_target);
        }
    }
}
