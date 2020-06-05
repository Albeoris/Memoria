using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x6E
    /// Unknown Opcode.
    /// MAPID = 0x06E,
    /// </summary>
    internal sealed class MAPID : JsmInstruction
    {
        private MAPID()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new MAPID();
        }
        public override String ToString()
        {
            return $"{nameof(MAPID)}()";
        }
    }
}