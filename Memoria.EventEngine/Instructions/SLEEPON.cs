using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnableInactiveAnimation
    /// Allow the player's character to play its inactive animation. The inaction time required is:
    /// First Time = 200 + 4 * Random[0, 255]
    /// Following Times = 200 + 2 * Random[0, 255]
    /// SLEEPON = 0x0EE,
    /// </summary>
    internal sealed class SLEEPON : JsmInstruction
    {
        private SLEEPON()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new SLEEPON();
        }
        public override String ToString()
        {
            return $"{nameof(SLEEPON)}()";
        }
    }
}