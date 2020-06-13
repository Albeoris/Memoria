using System;
using Random = UnityEngine.Random;

namespace Memoria
{
    public static class GameRandom
    {
        public static Int32 Next8()
        {
            return Random.Range(0, 255);
        }

        public static Int32 Next16()
        {
            return Random.Range(0, 65535);
        }
    }
}
