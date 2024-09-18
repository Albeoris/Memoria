namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Shaders : Settings
    {
        public SettingsGrid_Shaders()
        {
            CreateTextbloc(Lang.Settings.Shader_Enable, true);
            CreateTextbloc("╙ " + Lang.Settings.Shader_Field_chars);

            CreateCheckbox("Shader_Field_Realism", Lang.Settings.Shader_Realism);
            CreateCheckbox("Shader_Field_Toon", Lang.Settings.Shader_Toon, "", 4);
            CreateCheckbox("Shader_Field_Outlines", Lang.Settings.Shader_Outlines);

            CreateTextbloc("╙ " + Lang.Settings.Shader_Battle_chars);

            CreateCheckbox("Shader_Battle_Realism", Lang.Settings.Shader_Realism);
            CreateCheckbox("Shader_Battle_Toon", Lang.Settings.Shader_Toon, "", 4);
            CreateCheckbox("Shader_Battle_Outlines", Lang.Settings.Shader_Outlines);

            LoadSettings();
        }
    }
}
