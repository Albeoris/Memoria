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
    }
}
