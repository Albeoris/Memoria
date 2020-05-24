using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// AddGi
    /// Give gil to the player.
    /// 
    /// 1st argument: gil amount.
    /// AT_SPIN Amount (3 bytes)
    /// GILADD = 0x0CE,
    /// </summary>
    internal sealed class GILADD : JsmInstruction
    {
        private readonly IJsmExpression _amount;

        private GILADD(IJsmExpression amount)
        {
            _amount = amount;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression amount = reader.ArgumentInt24();
            return new GILADD(amount);
        }
        public override String ToString()
        {
            return $"{nameof(GILADD)}({nameof(_amount)}: {_amount})";
        }
    }
}