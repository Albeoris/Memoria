using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnableMenu
    /// Enable menu access by the player.
    /// MENUON = 0x0AA,
    /// </summary>
    internal sealed class MENUON : JsmInstruction
    {
        private MENUON()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new MENUON();
        }
        public override String ToString()
        {
            return $"{nameof(MENUON)}()";
        }
    }
}