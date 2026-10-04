using System;
using System.Windows;
using System.Windows.Controls;

namespace Memoria.Launcher
{
    public static class TextBoxHelper
    {
        public static string GetPlaceholder(DependencyObject obj) =>
            (string)obj.GetValue(PlaceholderProperty);

        public static void SetPlaceholder(DependencyObject obj, string value) =>
            obj.SetValue(PlaceholderProperty, value);

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(
                "Placeholder",
                typeof(string),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(
                    defaultValue: null,
                    propertyChangedCallback: OnPlaceholderChanged)
                );

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.RegisterAttached(
                "PlaceholderText",
                typeof(string),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(String.Empty));

        public static string GetPlaceholderText(DependencyObject obj) =>
            (string)obj.GetValue(PlaceholderTextProperty);

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBoxControl)
            {
                if (!textBoxControl.IsLoaded)
                {
                    // Ensure that the events are not added multiple times
                    textBoxControl.Loaded -= TextBoxControl_Loaded;
                    textBoxControl.Loaded += TextBoxControl_Loaded;
                }

                RefreshPlaceholder(textBoxControl);
            }
        }

        private static void TextBoxControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBoxControl)
            {
                textBoxControl.Loaded -= TextBoxControl_Loaded;
                RefreshPlaceholder(textBoxControl);
            }
        }

        public static void RefreshPlaceholder(TextBox textBoxControl)
        {
            if (textBoxControl != null)
            {
                string placeholder = GetPlaceholder(textBoxControl);
                string resolvedPlaceholder = ResolveResourceValue(placeholder);
                textBoxControl.SetValue(PlaceholderTextProperty, String.IsNullOrEmpty(resolvedPlaceholder) ? placeholder : resolvedPlaceholder);
            }
        }

        private static string ResolveResourceValue(string key)
        {
            try
            {
                // Try to get from Lang.Res first (which is Application.Current.Resources)
                if (Lang.Res.Contains(key))
                {
                    var value = Lang.Res[key];
                    if (value != null)
                        return value.ToString();
                }

                // Fallback: Try Application.Current.Resources directly
                if (Application.Current?.Resources != null && Application.Current.Resources.Contains(key))
                {
                    var value = Application.Current.Resources[key];
                    if (value != null)
                        return value.ToString();
                }
            }
            catch { }

            // If not found, return empty string so the original string displays as fallback
            return string.Empty;
        }

    }
}
