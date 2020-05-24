using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// Cinematic
    /// Run or setup a cinematic.
    /// 
    /// 1st argument: unknown.
    /// 2nd argument: cinematic ID (may depends on 1st argument's value).
    /// 3rd argument: unknown.
    /// 4th argument: unknown.
    /// AT_SPIN Unknown (1 bytes)
    /// AT_FMV Cinematic ID (1 bytes)
    /// AT_SPIN Unknown (1 bytes)
    /// AT_SPIN Unknown (1 bytes)
    /// FMV = 0x028,
    /// </summary>
    internal sealed class FMV : JsmInstruction
    {
        private readonly IJsmExpression _unknown1;

        private readonly IJsmExpression _cinematicId;

        private readonly IJsmExpression _unknown3;

        private readonly IJsmExpression _unknown4;

        private FMV(IJsmExpression unknown1, IJsmExpression cinematicId, IJsmExpression unknown3, IJsmExpression unknown4)
        {
            _unknown1 = unknown1;
            _cinematicId = cinematicId;
            _unknown3 = unknown3;
            _unknown4 = unknown4;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown1 = reader.ArgumentByte();
            IJsmExpression cinematicId = reader.ArgumentByte();
            IJsmExpression unknown3 = reader.ArgumentByte();
            IJsmExpression unknown4 = reader.ArgumentByte();
            return new FMV(unknown1, cinematicId, unknown3, unknown4);
        }
        public override String ToString()
        {
            return $"{nameof(FMV)}({nameof(_unknown1)}: {_unknown1}, {nameof(_cinematicId)}: {_cinematicId}, {nameof(_unknown3)}: {_unknown3}, {nameof(_unknown4)}: {_unknown4})";
        }
    }
}