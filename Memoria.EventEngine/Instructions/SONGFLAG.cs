using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ContinueBattleMusic
    /// Continue the music after the battle end.
    /// 
    /// 1st argument: flag continue/don't continue.
    /// AT_BOOL Continue (1 bytes)
    /// SONGFLAG = 0x01B,
    /// </summary>
    internal sealed class SONGFLAG : JsmInstruction
    {
        private readonly IJsmExpression _continue;

        private SONGFLAG(IJsmExpression @continue)
        {
            _continue = @continue;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @continue = reader.ArgumentByte();
            return new SONGFLAG(@continue);
        }
        public override String ToString()
        {
            return $"{nameof(SONGFLAG)}({nameof(_continue)}: {_continue})";
        }
    }
}