using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SideWalkXZY
    /// Make the character walk to destination without changing his facing angle. Make it synchronous if InitWalk is called before.
    /// 
    /// 1st to 3rd arguments: destination in (X, Z, Y) format.
    /// AT_POSITION_X DestinationX (2 bytes)
    /// AT_POSITION_Z DestinationY (2 bytes)
    /// AT_POSITION_Y DestinationZ (2 bytes)
    /// MOVE3H = 0x0E8,
    /// </summary>
    internal sealed class Move3H : JsmInstruction
    {
        private readonly IJsmExpression _destinationX;

        private readonly IJsmExpression _destinationY;

        private readonly IJsmExpression _destinationZ;

        private Move3H(IJsmExpression destinationX, IJsmExpression destinationY, IJsmExpression destinationZ)
        {
            _destinationX = destinationX;
            _destinationY = destinationY;
            _destinationZ = destinationZ;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression destinationX = reader.ArgumentInt16();
            IJsmExpression destinationY = reader.ArgumentInt16();
            IJsmExpression destinationZ = reader.ArgumentInt16();
            return new Move3H(destinationX, destinationY, destinationZ);
        }
        public override String ToString()
        {
            return $"{nameof(Move3H)}({nameof(_destinationX)}: {_destinationX}, {nameof(_destinationY)}: {_destinationY}, {nameof(_destinationZ)}: {_destinationZ})";
        }
    }
}