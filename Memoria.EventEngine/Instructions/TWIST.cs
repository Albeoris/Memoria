using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetControlDirection
    /// Set the angles for the player's movement control.
    /// 
    /// 1st argument: angle used for arrow movements.
    /// 2nd argument: angle used for analogic stick movements.
    /// AT_SPIN Arrow Angle (1 bytes)
    /// AT_SPIN Analogic Angle (1 bytes)
    /// </summary>
    internal sealed class TWIST : JsmInstruction
    {
        private readonly IJsmExpression _digitAngle;
        private readonly IJsmExpression _analogAngle;

        private TWIST(IJsmExpression digitAngle, IJsmExpression analogAngle)
        {
            _digitAngle = digitAngle;
            _analogAngle = analogAngle;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression analogAngle = reader.ArgumentByte();
            IJsmExpression digitAngle = reader.ArgumentByte();
            return new TWIST(digitAngle, analogAngle);
        }

        public override String ToString()
        {
            return $"{nameof(TWIST)}({nameof(_digitAngle)}: {_digitAngle}, {nameof(_analogAngle)}: {_analogAngle})";
        }
    }
}