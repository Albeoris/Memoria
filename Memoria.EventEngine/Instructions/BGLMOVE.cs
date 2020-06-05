using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetTilePositionEx
    /// Move a field tile block.
    /// 
    /// 1st argument: background tile block.
    /// 2nd and 3rd argument: position in (X, Y) format.
    /// 4th argument: closeness, defining whether 3D models are over or under that background tile.
    /// AT_TILE Tile Block (1 bytes)
    /// AT_SPIN Position X (2 bytes)
    /// AT_SPIN Position Y (2 bytes)
    /// AT_SPIN Position Closeness (2 bytes)
    /// BGLMOVE = 0x05A,
    /// </summary>
    internal sealed class BGLMOVE : JsmInstruction
    {
        private readonly IJsmExpression _tileBlock;

        private readonly IJsmExpression _positionX;

        private readonly IJsmExpression _positionY;

        private readonly IJsmExpression _positionCloseness;

        private BGLMOVE(IJsmExpression tileBlock, IJsmExpression positionX, IJsmExpression positionY, IJsmExpression positionCloseness)
        {
            _tileBlock = tileBlock;
            _positionX = positionX;
            _positionY = positionY;
            _positionCloseness = positionCloseness;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileBlock = reader.ArgumentByte();
            IJsmExpression positionX = reader.ArgumentInt16();
            IJsmExpression positionY = reader.ArgumentInt16();
            IJsmExpression positionCloseness = reader.ArgumentInt16();
            return new BGLMOVE(tileBlock, positionX, positionY, positionCloseness);
        }
        public override String ToString()
        {
            return $"{nameof(BGLMOVE)}({nameof(_tileBlock)}: {_tileBlock}, {nameof(_positionX)}: {_positionX}, {nameof(_positionY)}: {_positionY}, {nameof(_positionCloseness)}: {_positionCloseness})";
        }
    }
}