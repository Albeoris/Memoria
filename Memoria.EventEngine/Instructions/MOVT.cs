using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// MakeAnimationLoop
    /// Make current object's currently playing animation loop.
    /// 
    /// 1st argument: loop amount.
    /// AT_USPIN Amount (1 bytes)
    /// MOVT = 0x098,
    /// </summary>
    internal sealed class MOVT : JsmInstruction
    {
        private readonly IJsmExpression _amount;

        private MOVT(IJsmExpression amount)
        {
            _amount = amount;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression amount = reader.ArgumentByte();
            return new MOVT(amount);
        }
        public override String ToString()
        {
            return $"{nameof(MOVT)}({nameof(_amount)}: {_amount})";
        }
    }
}