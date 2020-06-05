using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnablePath
    /// Enable a field path.
    /// 
    /// 1st argument: field path ID.
    /// 2nd argument: boolean enable/disable.
    /// AT_WALKPATH Path (1 bytes)
    /// AT_BOOL Enable (1 bytes)
    /// BGIACTIVEF = 0x0CB,
    /// </summary>
    internal sealed class BGIACTIVEF : JsmInstruction
    {
        private readonly IJsmExpression _path;

        private readonly IJsmExpression _enable;

        private BGIACTIVEF(IJsmExpression path, IJsmExpression enable)
        {
            _path = path;
            _enable = enable;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression path = reader.ArgumentByte();
            IJsmExpression enable = reader.ArgumentByte();
            return new BGIACTIVEF(path, enable);
        }
        public override String ToString()
        {
            return $"{nameof(BGIACTIVEF)}({nameof(_path)}: {_path}, {nameof(_enable)}: {_enable})";
        }
    }
}