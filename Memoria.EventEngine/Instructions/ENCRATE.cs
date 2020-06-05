using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetRandomBattleFrequency
    /// Set the frequency of random battles.
    /// 
    /// 1st argument: frequency.
    ///  255 is the maximum frequency, corresponding to ~12 walking steps or ~7 running steps.
    ///  0 is the minimal frequency and disables random battles.
    /// AT_USPIN Frequency (1 bytes)
    /// ENCRATE = 0x057,
    /// </summary>
    internal sealed class ENCRATE : JsmInstruction
    {
        private readonly IJsmExpression _frequency;

        private ENCRATE(IJsmExpression frequency)
        {
            _frequency = frequency;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression frequency = reader.ArgumentByte();
            return new ENCRATE(frequency);
        }
        public override String ToString()
        {
            return $"{nameof(ENCRATE)}({nameof(_frequency)}: {_frequency})";
        }
    }
}