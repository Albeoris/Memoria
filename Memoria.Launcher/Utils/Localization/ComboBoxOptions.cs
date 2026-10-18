using System;
using System.Collections.Generic;

namespace Memoria.Launcher.Utils
{
    public static class ComboBoxOptions
    {
        public static IEnumerable<ComboBoxOption> Literal(IEnumerable<String> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            foreach (String value in values)
                yield return ComboBoxOption.Literal(value);
        }

        public static IEnumerable<ComboBoxOption> Localized(IEnumerable<String> resourceKeys)
        {
            if (resourceKeys == null)
                throw new ArgumentNullException(nameof(resourceKeys));

            foreach (String resourceKey in resourceKeys)
                yield return ComboBoxOption.Localized(resourceKey, resourceKey);
        }
    }
}
