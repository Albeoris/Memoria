using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetWalkTurnSpeed
    /// Change the turn speed of the object when it walks or runs (default is 16).
    /// 
    /// 1st argument: turn speed (with 0, the object doesn't turn while moving).
    /// 
    /// Special treatments:
    /// Vivi's in Iifa Tree/Eidolon Moun (field 1656) is initialized to 48.
    /// Choco's in Chocobo's Paradise (field 2954) is initialized to 96.
    /// AT_USPIN Turn Speed (1 bytes)
    /// MROT = 0x055,
    /// </summary>
    internal sealed class MROT : JsmInstruction
    {
        private readonly IJsmExpression _turnSpeed;

        private MROT(IJsmExpression turnSpeed)
        {
            _turnSpeed = turnSpeed;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression turnSpeed = reader.ArgumentByte();
            return new MROT(turnSpeed);
        }
        public override String ToString()
        {
            return $"{nameof(MROT)}({nameof(_turnSpeed)}: {_turnSpeed})";
        }
    }
}