namespace Memoria.Launcher
{
    public static class UiTextBoxFactory
    {
        public static UiTextBox Create()
        {
            return new UiTextBox();
        }

        public static UiTextBox Create(string text)
        {
            return new UiTextBox { Text = text };
        }
    }
}