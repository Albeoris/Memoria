using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunWorldCode
    /// Run one of the World Map codes, which effects have a large range. May modify the weather, the music, call the chocobo or enable the auto-pilot.
    /// 
    /// 1st argument: world code.
    /// 2nd argument: depends on the code.
    /// AT_WORLDCODE Code (1 bytes)
    /// AT_SPIN Argument (2 bytes)
    /// WPRM = 0x0C4,
    /// </summary>
    internal sealed class WPRM : JsmInstruction
    {
        private readonly IJsmExpression _code;

        private readonly IJsmExpression _argument;

        private WPRM(IJsmExpression code, IJsmExpression argument)
        {
            _code = code;
            _argument = argument;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression code = reader.ArgumentByte();
            IJsmExpression argument = reader.ArgumentInt16();
            return new WPRM(code, argument);
        }
        public override String ToString()
        {
            return $"{nameof(WPRM)}({nameof(_code)}: {_code}, {nameof(_argument)}: {_argument})";
        }
    }
}