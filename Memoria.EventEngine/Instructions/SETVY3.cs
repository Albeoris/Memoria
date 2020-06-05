using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetupJump
    /// Setup datas for a Jump call.
    /// 
    /// 1st to 3rd arguments: destination in (X, Z, Y) format.
    /// 4th argument: number of steps for the jump.
    /// AT_POSITION_X DestinationX (2 bytes)
    /// AT_POSITION_Z DestinationZ (2 bytes)
    /// AT_POSITION_Y DestinationY (2 bytes)
    /// AT_USPIN Steps (1 bytes)
    /// SETVY3 = 0x0E2,
    /// </summary>
    internal sealed class SETVY3 : JsmInstruction
    {
        private readonly IJsmExpression _destinationX;

        private readonly IJsmExpression _destinationZ;

        private readonly IJsmExpression _destinationY;

        private readonly IJsmExpression _steps;

        private SETVY3(IJsmExpression destinationX, IJsmExpression destinationZ, IJsmExpression destinationY, IJsmExpression steps)
        {
            _destinationX = destinationX;
            _destinationZ = destinationZ;
            _destinationY = destinationY;
            _steps = steps;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression destinationX = reader.ArgumentInt16();
            IJsmExpression destinationZ = reader.ArgumentInt16();
            IJsmExpression destinationY = reader.ArgumentInt16();
            IJsmExpression steps = reader.ArgumentByte();
            return new SETVY3(destinationX, destinationZ, destinationY, steps);
        }
        public override String ToString()
        {
            return $"{nameof(SETVY3)}({nameof(_destinationX)}: {_destinationX}, {nameof(_destinationZ)}: {_destinationZ}, {nameof(_destinationY)}: {_destinationY}, {nameof(_steps)}: {_steps})";
        }
    }
}