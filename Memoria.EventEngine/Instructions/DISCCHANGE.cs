using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ChangeDisc
    /// Allow to save the game and change disc.
    /// 
    /// 1st argument: gathered field destination and disc destination.
    /// AT_DISC_FIELD Disc (2 bytes)
    /// DISCCHANGE = 0x0AC,
    /// </summary>
    internal sealed class DISCCHANGE : JsmInstruction
    {
        private readonly IJsmExpression _disc;

        private DISCCHANGE(IJsmExpression disc)
        {
            _disc = disc;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression disc = reader.ArgumentInt16();
            return new DISCCHANGE(disc);
        }
        public override String ToString()
        {
            return $"{nameof(DISCCHANGE)}({nameof(_disc)}: {_disc})";
        }
    }
}