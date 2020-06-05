using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// BattleDialog
    /// Display text in battle for 60 frames.
    /// 
    /// 1st argument: text to display.
    /// AT_TEXT Text (2 bytes)
    /// MESB = 0x0D0,
    /// </summary>
    internal sealed class MESB : JsmInstruction
    {
        private readonly IJsmExpression _text;

        private MESB(IJsmExpression text)
        {
            _text = text;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression text = reader.ArgumentInt16();
            return new MESB(text);
        }
        public override String ToString()
        {
            return $"{nameof(MESB)}({nameof(_text)}: {_text})";
        }
    }
}