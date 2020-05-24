using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunSoundCode3
    /// Run a sound code.
    /// 
    /// 1st argument: sound code.
    /// 2nd argument: music or sound to process.
    /// 3rd to 5th arguments: depend on the sound code.
    /// AT_SOUNDCODE Code (2 bytes)
    /// AT_SOUND Sound (2 bytes)
    /// AT_SPIN Argument (3 bytes)
    /// AT_SPIN Argument (1 bytes)
    /// AT_SPIN Argument (1 bytes)
    /// FLDSND3 = 0x0C8,
    /// </summary>
    internal sealed class FLDSND3 : JsmInstruction
    {
        private readonly IJsmExpression _code;
        private readonly IJsmExpression _sound;
        private readonly IJsmExpression _argument1;
        private readonly IJsmExpression _argument2;
        private readonly IJsmExpression _argument3;

        private FLDSND3(IJsmExpression code, IJsmExpression sound, IJsmExpression argument1, IJsmExpression argument2, IJsmExpression argument3)
        {
            _code = code;
            _sound = sound;
            _argument1 = argument1;
            _argument2 = argument2;
            _argument3 = argument3;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression code = reader.ArgumentInt16();
            IJsmExpression sound = reader.ArgumentInt16();
            IJsmExpression argument1 = reader.ArgumentInt24();
            IJsmExpression argument2 = reader.ArgumentByte();
            IJsmExpression argument3 = reader.ArgumentByte();
            return new FLDSND3(code, sound, argument1, argument2, argument3);
        }
        public override String ToString()
        {
            return $"{nameof(FLDSND3)}({nameof(_code)}: {_code}, {nameof(_sound)}: {_sound}, {nameof(_argument1)}: {_argument1}, {nameof(_argument2)}: {_argument2}, {nameof(_argument3)}: {_argument3})";
        }
    }
}