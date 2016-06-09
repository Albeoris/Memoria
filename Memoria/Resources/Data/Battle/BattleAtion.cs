namespace Memoria
{
    public enum BattleAtion
    {
        ItemPotionRestoreHp = 69,
        ItemEtherRestoreMp = 70,
        ItemElexirRestoreHpMp = 71,
        ItemPhoenixDownRevive = 72,
        ItemRemoveStatus = 73,
        ItemGemRestoreHp = 74, //  tg_hp = Items[cmd.sub_no - 224].Power * (GemCount(cmd.sub_no)) + 1));
        ItemPepperRestoreHp = 75,
    }
}