using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetTileCamera
    /// Link a tile block to a specific field camera (useful for looping movement bounds).
    /// 
    /// 1st argument: background tile block.
    /// 2nd argument: camera ID.
    /// AT_TILE Tile Block (1 bytes)
    /// AT_USPIN Camera ID (1 bytes)
    /// BGVSET = 0x0C3,
    /// </summary>
    internal sealed class BGVSET : JsmInstruction
    {
        private readonly IJsmExpression _tileBlock;

        private readonly IJsmExpression _cameraId;

        private BGVSET(IJsmExpression tileBlock, IJsmExpression cameraId)
        {
            _tileBlock = tileBlock;
            _cameraId = cameraId;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileBlock = reader.ArgumentByte();
            IJsmExpression cameraId = reader.ArgumentByte();
            return new BGVSET(tileBlock, cameraId);
        }
        public override String ToString()
        {
            return $"{nameof(BGVSET)}({nameof(_tileBlock)}: {_tileBlock}, {nameof(_cameraId)}: {_cameraId})";
        }
    }
}