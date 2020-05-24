using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetAnimationFlags
    /// Set the current object's next animation looping flags.
    /// 
    /// 1st argument: looping flag list.
    ///  1: freeze at end
    ///  2: loop
    ///  3: loop back and forth
    /// 2nd arguments: times to repeat.
    /// AT_ANIMFLAG Flag (1 bytes)
    /// AT_USPIN Repeat (1 bytes)
    /// AMODE = 0x03F,
    /// </summary>
    internal sealed class AMODE : JsmInstruction
    {
        private readonly IJsmExpression _flag;

        private readonly IJsmExpression _repeat;

        private AMODE(IJsmExpression flag, IJsmExpression repeat)
        {
            _flag = flag;
            _repeat = repeat;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression flag = reader.ArgumentByte();
            IJsmExpression repeat = reader.ArgumentByte();
            return new AMODE(flag, repeat);
        }
        public override String ToString()
        {
            return $"{nameof(AMODE)}({nameof(_flag)}: {_flag}, {nameof(_repeat)}: {_repeat})";
        }
    }
}