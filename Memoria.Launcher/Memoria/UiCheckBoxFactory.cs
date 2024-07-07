using System;

namespace Memoria.Launcher
{
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
}
