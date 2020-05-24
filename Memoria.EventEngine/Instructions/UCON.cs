using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnableMove
    /// Enable the player's movement control.
    /// UCON = 0x02E,
    /// </summary>
    internal sealed class UCON : JsmInstruction
    {
        private UCON()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new UCON();
        }
        public override String ToString()
        {
            return $"{nameof(UCON)}()";
        }
    }
}