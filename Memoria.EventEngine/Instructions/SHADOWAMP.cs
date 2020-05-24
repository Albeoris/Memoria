using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetShadowAmplifier
    /// Amplify or reduce the shadow transparancy.
    /// 
    /// 1st argument: amplification factor.
    /// AT_USPIN Amplification Factor (1 bytes)
    /// SHADOWAMP = 0x085,
    /// </summary>
    internal sealed class SHADOWAMP : JsmInstruction
    {
        private readonly IJsmExpression _amplificationFactor;

        private SHADOWAMP(IJsmExpression amplificationFactor)
        {
            _amplificationFactor = amplificationFactor;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression amplificationFactor = reader.ArgumentByte();
            return new SHADOWAMP(amplificationFactor);
        }
        public override String ToString()
        {
            return $"{nameof(SHADOWAMP)}({nameof(_amplificationFactor)}: {_amplificationFactor})";
        }
    }
}