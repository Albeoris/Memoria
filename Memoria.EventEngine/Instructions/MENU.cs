using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// Menu
    /// Open a menu.
    /// 
    /// 1st argument: menu type.
    /// 2nd argument: depends on the menu type.
    ///  Naming Menu: character to name.
    ///  Shop Menu: shop ID.
    /// AT_MENUTYPE Menu Type (1 bytes)
    /// AT_MENU Menu (1 bytes)
    /// MENU = 0x075,
    /// </summary>
    internal sealed class MENU : JsmInstruction
    {
        private readonly IJsmExpression _menuType;

        private readonly IJsmExpression _menu;

        private MENU(IJsmExpression menuType, IJsmExpression menu)
        {
            _menuType = menuType;
            _menu = menu;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression menuType = reader.ArgumentByte();
            IJsmExpression menu = reader.ArgumentByte();
            return new MENU(menuType, menu);
        }
        public override String ToString()
        {
            return $"{nameof(MENU)}({nameof(_menuType)}: {_menuType}, {nameof(_menu)}: {_menu})";
        }
    }
}