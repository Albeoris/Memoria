using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// Bubble
    /// Display a speech bubble with a symbol inside over the head of player's character.
    /// 
    /// 1st argument: bubble symbol.
    /// AT_BUBBLESYMBOL Symbo (1 bytes)
    /// FICON = 0x068,
    /// </summary>
    internal sealed class FICON : JsmInstruction
    {
        private readonly IJsmExpression _symbo;

        private FICON(IJsmExpression symbo)
        {
            _symbo = symbo;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression symbo = reader.ArgumentByte();
            return new FICON(symbo);
        }
        public override String ToString()
        {
            return $"{nameof(FICON)}({nameof(_symbo)}: {_symbo})";
        }
    }
}