using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// GameOver
    /// Terminate the game with a Game Over screen.
    /// GAMEOVER = 0x0F5,
    /// </summary>
    internal sealed class GAMEOVER : JsmInstruction
    {
        private GAMEOVER()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new GAMEOVER();
        }
        public override String ToString()
        {
            return $"{nameof(GAMEOVER)}()";
        }
    }
}