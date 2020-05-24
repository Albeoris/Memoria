using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// WalkXZY
    /// Make the character walk to destination. Make it synchronous if InitWalk is called before.
    /// 
    /// 1st argument: destination in (X, Z, Y) format.
    /// AT_POSITION_X DestinationX (2 bytes)
    /// AT_POSITION_Z DestinationZ (2 bytes)
    /// AT_POSITION_Y DestinationY (2 bytes)
    /// MOVE3 = 0x0A2,
    /// </summary>
    internal sealed class MOVE3 : JsmInstruction
    {
        private readonly IJsmExpression _destinationX;

        private readonly IJsmExpression _destinationZ;

        private readonly IJsmExpression _destinationY;

        private MOVE3(IJsmExpression destinationX, IJsmExpression destinationZ, IJsmExpression destinationY)
        {
            _destinationX = destinationX;
            _destinationZ = destinationZ;
            _destinationY = destinationY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression destinationX = reader.ArgumentInt16();
            IJsmExpression destinationZ = reader.ArgumentInt16();
            IJsmExpression destinationY = reader.ArgumentInt16();
            return new MOVE3(destinationX, destinationZ, destinationY);
        }
        public override String ToString()
        {
            return $"{nameof(MOVE3)}({nameof(_destinationX)}: {_destinationX}, {nameof(_destinationZ)}: {_destinationZ}, {nameof(_destinationY)}: {_destinationY})";
        }
    }
}