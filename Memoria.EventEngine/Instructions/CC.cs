using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// DefinePlayerCharacter
    /// Apply the player's control over the entry's object.
    /// CC = 0x02C,
    /// </summary>
    internal sealed class CC : JsmInstruction
    {
        private CC()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new CC();
        }
        public override String ToString()
        {
            return $"{nameof(CC)}()";
        }
    }
}