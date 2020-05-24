using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetTileLoopType
    /// Let tile be screen anchored or not.
    /// 
    /// 1st argument: background tile block.
    /// 2nd argument: boolean on/off.
    /// AT_TILE Tile Block (1 bytes)
    /// AT_BOOL Screen Anchored (1 bytes)
    /// BGLLOOPTYPE = 0x0E6,
    /// </summary>
    internal sealed class BGLLOOPTYPE : JsmInstruction
    {
        private readonly IJsmExpression _tileBlock;

        private readonly IJsmExpression _screenAnchored;

        private BGLLOOPTYPE(IJsmExpression tileBlock, IJsmExpression screenAnchored)
        {
            _tileBlock = tileBlock;
            _screenAnchored = screenAnchored;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileBlock = reader.ArgumentByte();
            IJsmExpression screenAnchored = reader.ArgumentByte();
            return new BGLLOOPTYPE(tileBlock, screenAnchored);
        }
        public override String ToString()
        {
            return $"{nameof(BGLLOOPTYPE)}({nameof(_tileBlock)}: {_tileBlock}, {nameof(_screenAnchored)}: {_screenAnchored})";
        }
    }
}