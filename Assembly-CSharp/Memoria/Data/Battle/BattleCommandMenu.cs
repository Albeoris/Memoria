namespace Memoria.Data
{
    public enum BattleCommandMenu
    {
        // Only concerns commands that really emanate from a player's input (so no counter-attacks, no Berserk/Confuse attacks...)
        None = -1,
        Attack = 0,
        Defend,
        Ability1,
        Ability2,
        Item,
        Change,
        AccessMenu = 7,
    }
}
