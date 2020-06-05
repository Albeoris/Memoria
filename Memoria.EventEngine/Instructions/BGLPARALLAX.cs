using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// MoveTile
    /// Make the field moves depending on the camera position.
    /// 
    /// 1st argument: background tile block.
    /// 2nd argument: boolean on/off.
    /// 3rd and 4th arguments: parallax movement in (X, Y) format.
    /// AT_TILE Tile Block (1 bytes)
    /// AT_BOOL Activate (1 bytes)
    /// AT_SPIN Movement X (2 bytes)
    /// AT_SPIN Movement Y (2 bytes)
    /// BGLPARALLAX = 0x05D,
    /// </summary>
    internal sealed class BGLPARALLAX : JsmInstruction
    {
        private readonly IJsmExpression _tileBlock;

        private readonly IJsmExpression _activate;

        private readonly IJsmExpression _movementX;

        private readonly IJsmExpression _movementY;

        private BGLPARALLAX(IJsmExpression tileBlock, IJsmExpression activate, IJsmExpression movementX, IJsmExpression movementY)
        {
            _tileBlock = tileBlock;
            _activate = activate;
            _movementX = movementX;
            _movementY = movementY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileBlock = reader.ArgumentByte();
            IJsmExpression activate = reader.ArgumentByte();
            IJsmExpression movementX = reader.ArgumentInt16();
            IJsmExpression movementY = reader.ArgumentInt16();
            return new BGLPARALLAX(tileBlock, activate, movementX, movementY);
        }
        public override String ToString()
        {
            return $"{nameof(BGLPARALLAX)}({nameof(_tileBlock)}: {_tileBlock}, {nameof(_activate)}: {_activate}, {nameof(_movementX)}: {_movementX}, {nameof(_movementY)}: {_movementY})";
        }
    }
}