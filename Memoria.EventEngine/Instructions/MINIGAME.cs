using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// TetraMaster
    /// Begin a card game.
    /// 
    /// 1st argument: card deck of the opponent.
    /// AT_DECK Card Deck (2 bytes)
    /// MINIGAME = 0x0AE,
    /// </summary>
    internal sealed class MINIGAME : JsmInstruction
    {
        private readonly IJsmExpression _cardDeck;

        private MINIGAME(IJsmExpression cardDeck)
        {
            _cardDeck = cardDeck;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression cardDeck = reader.ArgumentInt16();
            return new MINIGAME(cardDeck);
        }
        public override String ToString()
        {
            return $"{nameof(MINIGAME)}({nameof(_cardDeck)}: {_cardDeck})";
        }
    }
}