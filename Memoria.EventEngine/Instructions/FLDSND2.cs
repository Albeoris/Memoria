using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunSoundCode2
    /// Same as RunSoundCode3( code, music, arg1, arg2, 0 ).
    /// AT_SOUNDCODE Code (2 bytes)
    /// AT_SOUND Sound (2 bytes)
    /// AT_SPIN Argument (3 bytes)
    /// AT_SPIN Argument (1 bytes)
    /// FLDSND2 = 0x0C7,
    /// </summary>
    internal sealed class FLDSND2 : JsmInstruction
    {
        private readonly IJsmExpression _code;

        private readonly IJsmExpression _sound;

        private readonly IJsmExpression _argument1;

        private readonly IJsmExpression _argument2;

        private FLDSND2(IJsmExpression code, IJsmExpression sound, IJsmExpression argument1, IJsmExpression argument2)
        {
            _code = code;
            _sound = sound;
            _argument1 = argument1;
            _argument2 = argument2;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression code = reader.ArgumentInt16();
            IJsmExpression sound = reader.ArgumentInt16();
            IJsmExpression argument1 = reader.ArgumentInt24();
            IJsmExpression argument2 = reader.ArgumentByte();
            return new FLDSND2(code, sound, argument1, argument2);
        }
        public override String ToString()
        {
            return $"{nameof(FLDSND2)}({nameof(_code)}: {_code}, {nameof(_sound)}: {_sound}, {nameof(_argument1)}: {_argument1}, {nameof(_argument2)}: {_argument2})";
        }
    }
}