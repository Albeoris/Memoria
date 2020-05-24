using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// TurnTowardObject
    /// Turn the character toward an entry object (animated).
    /// 
    /// 1st argument: object.
    /// 2nd argument: turn speed (1 is slowest).
    /// AT_ENTRY Object (1 bytes)
    /// AT_USPIN Speed (1 bytes)
    /// TURNA = 0x051,
    /// </summary>
    internal sealed class TURNA : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _speed;

        private TURNA(IJsmExpression @object, IJsmExpression speed)
        {
            _object = @object;
            _speed = speed;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression speed = reader.ArgumentByte();
            return new TURNA(@object, speed);
        }
        public override String ToString()
        {
            return $"{nameof(TURNA)}({nameof(_object)}: {_object}, {nameof(_speed)}: {_speed})";
        }
    }
}