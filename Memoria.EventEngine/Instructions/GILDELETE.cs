using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RemoveGi
    /// Remove gil from the player.
    /// 
    /// 1st argument: gil amount.
    /// AT_SPIN Amount (3 bytes)
    /// GILDELETE = 0x0CF,
    /// </summary>
    internal sealed class GILDELETE : JsmInstruction
    {
        private readonly IJsmExpression _amount;

        private GILDELETE(IJsmExpression amount)
        {
            _amount = amount;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression amount = reader.ArgumentInt24();
            return new GILDELETE(amount);
        }
        public override String ToString()
        {
            return $"{nameof(GILDELETE)}({nameof(_amount)}: {_amount})";
        }
    }
}