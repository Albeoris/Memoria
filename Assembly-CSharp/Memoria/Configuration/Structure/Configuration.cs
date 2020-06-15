using Memoria.Prime.Ini;

#pragma warning disable 420

namespace Memoria
{
    public sealed partial class Configuration : Ini
    {
        private volatile ModSection _mod;
        private volatile FontSection _font;
        private volatile AnalogControlSection _analogControl;
        private volatile GraphicsSection _graphics;
        private volatile CheatsSection _cheats;
        private volatile ImportSection _import;
        private volatile ExportSection _export;
        private volatile TetraMasterSection _tetraMaster;
        private volatile FixesSection _fixes;
        private volatile HacksSection _hacks;
        private volatile BattleSection _battle;
        private volatile IconsSection _icons;
        private volatile AudioSection _audio;
        private volatile DebugSection _debug;

        public Configuration()
        {
            BindingSection(out _mod, v => _mod = v);
            BindingSection(out _font, v => _font = v);
            BindingSection(out _analogControl, v => _analogControl = v);
            BindingSection(out _graphics, v => _graphics = v);
            BindingSection(out _cheats, v => _cheats = v);
            BindingSection(out _import, v => _import = v);
            BindingSection(out _export, v => _export = v);
            BindingSection(out _tetraMaster, v => _tetraMaster = v);
            BindingSection(out _fixes, v => _fixes = v);
            BindingSection(out _hacks, v => _hacks = v);
            BindingSection(out _battle, v => _battle = v);
            BindingSection(out _icons, v => _icons = v);
            BindingSection(out _audio, v => _audio = v);
            BindingSection(out _debug, v => _debug = v);
        }
    }
}