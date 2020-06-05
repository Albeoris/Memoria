using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// TerminateBattle
    /// Return to the field (or world map) when the rewards are disabled.
    /// BEND = 0x0E1,
    /// </summary>
    internal sealed class BEND : JsmInstruction
    {
        private BEND()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new BEND();
        }
        public override String ToString()
        {
            return $"{nameof(BEND)}()";
        }
    }
}