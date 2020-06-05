using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ExitField
    /// Make the player's character walk to the field exit and prepare to flush the field datas.
    /// MOVQ = 0x09E,
    /// </summary>
    internal sealed class MOVQ : JsmInstruction
    {
        private MOVQ()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new MOVQ();
        }
        public override String ToString()
        {
            return $"{nameof(MOVQ)}()";
        }
    }
}