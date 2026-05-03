using System;

namespace Memoria
{
    [Flags]
    public enum ActorFlags : ushort
    {
        NeckT = 1,
        NeckM = 2,
        NeckTalk = 4,
        Jump = 8,
        Move = 16,
        LockDirection = 32,
        Eye = 64,
        Aim = 128,
        Look = 256,
        LookTalker = 512,
        LookedTalker = 1024
    }
}
