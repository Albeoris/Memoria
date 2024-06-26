using System;

namespace Memoria.Data
{
    [Flags]
    public enum EnemyCategory : byte
    {
        Other = 0,
        Humanoid = 1,
        Beast = 2,
        Devil = 4,
        Dragon = 8,
        Undead = 16,
        Stone = 32,
        Soul = 64,
        Flight = 128
    }
}
