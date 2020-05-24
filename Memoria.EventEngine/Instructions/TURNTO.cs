using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// TurnTowardPosition
    /// Turn the character toward a position (animated). The object's turn speed is used (default to 16).
    /// 
    /// 1st and 2nd arguments: coordinates in (X, Y) format.
    /// AT_POSITION_X CoordinateX (2 bytes)
    /// AT_POSITION_Y CoordinateY (2 bytes)
    /// TURNTO = 0x09B,
    /// </summary>
    internal sealed class TURNTO : JsmInstruction
    {
        private readonly IJsmExpression _coordinateX;

        private readonly IJsmExpression _coordinateY;

        private TURNTO(IJsmExpression coordinateX, IJsmExpression coordinateY)
        {
            _coordinateX = coordinateX;
            _coordinateY = coordinateY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression coordinateX = reader.ArgumentInt16();
            IJsmExpression coordinateY = reader.ArgumentInt16();
            return new TURNTO(coordinateX, coordinateY);
        }
        public override String ToString()
        {
            return $"{nameof(TURNTO)}({nameof(_coordinateX)}: {_coordinateX}, {nameof(_coordinateY)}: {_coordinateY})";
        }
    }
}