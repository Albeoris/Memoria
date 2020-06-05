using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunBattleCode
    /// Run a battle code.
    /// 
    /// 1st argument: battle code.
    /// 2nd argument: depends on the battle code.
    ///  End Battle: 0 for a defeat (deprecated), 1 for a victory, 2 for a victory without victory pose, 3 for a defeat, 4 for an escape, 5 for an interrupted battle, 6 for a game over, 7 for an enemy escape.
    ///  Run Camera: Camera ID.
    ///  Change Field: ID of the destination field after the battle.
    ///  Add Gil: amount to add.
    /// AT_BATTLECODE Code (1 bytes)
    /// AT_SPIN Argument (2 bytes)
    /// BTLSET = 0x04A,
    /// </summary>
    internal sealed class BTLSET : JsmInstruction
    {
        private readonly IJsmExpression _code;

        private readonly IJsmExpression _argument;

        private BTLSET(IJsmExpression code, IJsmExpression argument)
        {
            _code = code;
            _argument = argument;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression code = reader.ArgumentByte();
            IJsmExpression argument = reader.ArgumentInt16();
            return new BTLSET(code, argument);
        }
        public override String ToString()
        {
            return $"{nameof(BTLSET)}({nameof(_code)}: {_code}, {nameof(_argument)}: {_argument})";
        }
    }
}