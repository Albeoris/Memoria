using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetPathing
    /// Change the pathing of the character.
    /// 
    /// 1st argument: boolean pathing on/off.
    /// AT_BOOL Pathing (1 bytes)
    /// BGI = 0x0A8,
    /// </summary>
    internal sealed class BGI : JsmInstruction
    {
        private readonly IJsmExpression _pathing;

        private BGI(IJsmExpression pathing)
        {
            _pathing = pathing;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression pathing = reader.ArgumentByte();
            return new BGI(pathing);
        }
        public override String ToString()
        {
            return $"{nameof(BGI)}({nameof(_pathing)}: {_pathing})";
        }
    }
}