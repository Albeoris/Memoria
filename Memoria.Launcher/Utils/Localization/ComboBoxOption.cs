using System;

namespace Memoria.Launcher.Utils
{
    public sealed class ComboBoxOption
    {
        public String Value { get; }
        public String DisplayText { get; }
        public String ResourceKey { get; }
        public Boolean IsLocalized => ResourceKey != null;

        private ComboBoxOption(String value, String displayText, String resourceKey)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            DisplayText = displayText;
            ResourceKey = resourceKey;
        }

        public static ComboBoxOption Literal(String value)
        {
            return Literal(value, value);
        }

        public static ComboBoxOption Literal(String value, String displayText)
        {
            if (displayText == null)
                throw new ArgumentNullException(nameof(displayText));

            return new ComboBoxOption(value, displayText, null);
        }

        public static ComboBoxOption Localized(String value, String resourceKey)
        {
            if (resourceKey == null)
                throw new ArgumentNullException(nameof(resourceKey));

            return new ComboBoxOption(value, null, resourceKey);
        }
    }
}
