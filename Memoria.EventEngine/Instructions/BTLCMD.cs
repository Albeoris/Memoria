using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// Attack
    /// Make the enemy attack. The target(s) are to be set using the SV_Target variable.
    /// Inside an ATB function, the attack is added to the queue.
    /// Inside a counter function, the attack occurs directly.
    /// 
    /// 1st argument: attack to perform.
    /// AT_ATTACK Attack (1 bytes)
    /// BTLCMD = 0x038,
    /// </summary>
    internal sealed class BTLCMD : JsmInstruction
    {
        private readonly IJsmExpression _attack;

        private BTLCMD(IJsmExpression attack)
        {
            _attack = attack;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression attack = reader.ArgumentByte();
            return new BTLCMD(attack);
        }
        public override String ToString()
        {
            return $"{nameof(BTLCMD)}({nameof(_attack)}: {_attack})";
        }
    }
}