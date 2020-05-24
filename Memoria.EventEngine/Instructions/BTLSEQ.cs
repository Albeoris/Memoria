using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// AttackSpecia
    /// Make the enemy instantatly use a special move. It doesn't use nor modify the battle state so it should be used when the battle is paused. The target(s) are to be set using the SV_Target variable.
    /// 
    /// 1st argument: attack to perform.
    /// AT_ATTACK Attack (1 bytes)
    /// BTLSEQ = 0x0E5,
    /// </summary>
    internal sealed class BTLSEQ : JsmInstruction
    {
        private readonly IJsmExpression _attack;

        private BTLSEQ(IJsmExpression attack)
        {
            _attack = attack;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression attack = reader.ArgumentByte();
            return new BTLSEQ(attack);
        }
        public override String ToString()
        {
            return $"{nameof(BTLSEQ)}({nameof(_attack)}: {_attack})";
        }
    }
}