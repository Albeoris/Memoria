using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunSoundCode1
    /// Same as RunSoundCode3( code, music, arg1, 0, 0 ).
    /// AT_SOUNDCODE Code (2 bytes)
    /// AT_SOUND Sound (2 bytes)
    /// AT_SPIN Argument (3 bytes)
    /// FLDSND1 = 0x0C6,
    /// </summary>
    internal sealed class FLDSND1 : JsmInstruction
    {
        private readonly IJsmExpression _code;

        private readonly IJsmExpression _sound;

        private readonly IJsmExpression _argument;

        private FLDSND1(IJsmExpression code, IJsmExpression sound, IJsmExpression argument)
        {
            _code = code;
            _sound = sound;
            _argument = argument;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression code = reader.ArgumentInt16();
            IJsmExpression sound = reader.ArgumentInt16();
            IJsmExpression argument = reader.ArgumentInt24();
            return new FLDSND1(code, sound, argument);
        }
        public override String ToString()
        {
            return $"{nameof(FLDSND1)}({nameof(_code)}: {_code}, {nameof(_sound)}: {_sound}, {nameof(_argument)}: {_argument})";
        }
    }
}