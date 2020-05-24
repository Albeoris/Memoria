using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0xED
    /// Unknown opcode about tile looping movements.
    /// 
    /// 1st argument: camera ID.
    /// 2nd and 3rd arguments: unknown factors (X, Y).
    /// AT_USPIN Camera ID (1 bytes)
    /// AT_SPIN Unknown X (2 bytes)
    /// AT_SPIN Unknown Y (2 bytes)
    /// BGVALPHA = 0x0ED,
    /// </summary>
    internal sealed class BGVALPHA : JsmInstruction
    {
        private readonly IJsmExpression _cameraId;

        private readonly IJsmExpression _unknownX;

        private readonly IJsmExpression _unknownY;

        private BGVALPHA(IJsmExpression cameraId, IJsmExpression unknownX, IJsmExpression unknownY)
        {
            _cameraId = cameraId;
            _unknownX = unknownX;
            _unknownY = unknownY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression cameraId = reader.ArgumentByte();
            IJsmExpression unknownX = reader.ArgumentInt16();
            IJsmExpression unknownY = reader.ArgumentInt16();
            return new BGVALPHA(cameraId, unknownX, unknownY);
        }
        public override String ToString()
        {
            return $"{nameof(BGVALPHA)}({nameof(_cameraId)}: {_cameraId}, {nameof(_unknownX)}: {_unknownX}, {nameof(_unknownY)}: {_unknownY})";
        }
    }
}