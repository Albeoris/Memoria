using System;
using System.Collections.Generic;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration : Ini
    {
        private readonly FontSection _font = new FontSection();
        private readonly ControlSection _ctrl = new ControlSection();
        private readonly GraphicsSection _graphics = new GraphicsSection();
        private readonly CheatsSection _cheats = new CheatsSection();
        private readonly ImportSection _import = new ImportSection();
        private readonly ExportSection _export = new ExportSection();
        private readonly TetraMasterSection _tetraMaster = new TetraMasterSection();
        private readonly FixesSection _fixes = new FixesSection();
        private readonly HacksSection _hacks = new HacksSection();
        private readonly BattleSection _battle = new BattleSection();
        private readonly DebugSection _debug = new DebugSection();

        public override IEnumerable<IniSection> GetSections()
        {
            yield return _font;
            yield return _ctrl;
            yield return _graphics;
            yield return _cheats;
            yield return _import;
            yield return _export;
            yield return _tetraMaster;
            yield return _fixes;
            yield return _hacks;
            yield return _battle;
            yield return _debug;
        }
    }
}