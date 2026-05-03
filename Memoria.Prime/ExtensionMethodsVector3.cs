using System;
using UnityEngine;

namespace Memoria.Prime
{
    public static class ExtensionMethodsVector3
    {
        public static Vector3 SetX(this Vector3 self, Single value)
        {
            return new Vector3(value, self.y, self.z);
        }

        public static Vector3 SetY(this Vector3 self, Single value)
        {
            return new Vector3(self.x, value, self.z);
        }

        public static Vector3 SetZ(this Vector3 self, Single value)
        {
            return new Vector3(self.x, self.y, value);
        }

        public static Vector3 SetXY(this Vector3 self, Single x, Single y)
        {
            return new Vector3(x, y, self.z);
        }

        public static Vector3 SetXZ(this Vector3 self, Single x, Single z)
        {
            return new Vector3(x, self.y, z);
        }

        public static Vector3 SetYZ(this Vector3 self, Single y, Single z)
        {
            return new Vector3(self.x, y, z);
        }

        public static Vector3 ToVector3(this Single[] array, Boolean asScale)
        {
            if (array == null || array.Length == 0)
                return asScale ? Vector3.one : Vector3.zero;
            if (array.Length >= 3)
                return new Vector3(array[0], array[1], array[2]);
            if (array.Length == 1)
                return asScale ? new Vector3(array[0], array[0], array[0]) : new Vector3(array[0], 0f, 0f);
            return asScale ? new Vector3(array[0], 1f, array[1]) : new Vector3(array[0], 0f, array[1]);
        }
    }
}
