using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// DisableInactiveAnimation
    /// Prevent player's character to play its inactive animation.
    /// SLEEPINH = 0x090,
    /// </summary>
    internal sealed class SLEEPINH : JsmInstruction
    {
        private SLEEPINH()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new SLEEPINH();
        }
        public override String ToString()
        {
            return $"{nameof(SLEEPINH)}()";
        }
    }
}