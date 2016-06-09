namespace Memoria
{
    public enum TargetType : byte
    {
        SingleAny = 0,
        SingleAlly = 1,
        SingleEnemy = 2,
        ManyAny = 3,
        ManyAlly = 4,
        ManyEnemy = 5,
        All = 6,
        AllAlly = 7,
        AllEnemy = 8,
        Random = 9,
        RandomAlly = 10,
        RandomEnemy = 11,
        Everyone = 12,
        Self = 13
    }
}