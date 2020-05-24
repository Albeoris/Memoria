using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// DisableMenu
    /// Disable menu access by the player.
    /// MENUOFF = 0x0AB,
    /// </summary>
    internal sealed class MENUOFF : JsmInstruction
    {
        private MENUOFF()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new MENUOFF();
        }
        public override String ToString()
        {
            return $"{nameof(MENUOFF)}()";
        }
    }
}