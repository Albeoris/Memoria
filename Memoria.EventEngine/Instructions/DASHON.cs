using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnableRun
    /// Allow the player's character to run.
    /// DASHON = 0x0F0,
    /// </summary>
    internal sealed class DASHON : JsmInstruction
    {
        private DASHON()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new DASHON();
        }
        public override String ToString()
        {
            return $"{nameof(DASHON)}()";
        }
    }
}