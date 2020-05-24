using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunSoundCode
    /// Same as RunSoundCode3( code, music, 0, 0, 0 ).
    /// AT_SOUNDCODE Code (2 bytes)
    /// AT_SOUND Sound (2 bytes)
    /// FLDSND0 = 0x0C5,
    /// </summary>
    internal sealed class FLDSND0 : JsmInstruction
    {
        private readonly IJsmExpression _code;

        private readonly IJsmExpression _sound;

        private FLDSND0(IJsmExpression code, IJsmExpression sound)
        {
            _code = code;
            _sound = sound;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression code = reader.ArgumentInt16();
            IJsmExpression sound = reader.ArgumentInt16();
            return new FLDSND0(code, sound);
        }
        public override String ToString()
        {
            return $"{nameof(FLDSND0)}({nameof(_code)}: {_code}, {nameof(_sound)}: {_sound})";
        }
    }
}