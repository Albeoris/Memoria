using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// CalculateExitPosition
    /// Calculate the field exit position based on the region's polygon.
    /// MJPOS = 0x0A4,
    /// </summary>
    internal sealed class MJPOS : JsmInstruction
    {
        private MJPOS()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new MJPOS();
        }
        public override String ToString()
        {
            return $"{nameof(MJPOS)}()";
        }
    }
}