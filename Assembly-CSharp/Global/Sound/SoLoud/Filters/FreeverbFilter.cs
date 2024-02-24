using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class FreeverbFilter : SoloudObject
    {
        public const int WET = 0;
        public const int FREEZE = 1;
        public const int ROOMSIZE = 2;
        public const int DAMP = 3;
        public const int WIDTH = 4;

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr FreeverbFilter_create();
        public FreeverbFilter()
        {
            objhandle = FreeverbFilter_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr FreeverbFilter_destroy(IntPtr aObjHandle);
        ~FreeverbFilter()
        {
            FreeverbFilter_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int FreeverbFilter_getParamCount(IntPtr aObjHandle);
        public int getParamCount()
        {
            return FreeverbFilter_getParamCount(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr FreeverbFilter_getParamName(IntPtr aObjHandle, uint aParamIndex);
        public string getParamName(uint aParamIndex)
        {
            IntPtr p = FreeverbFilter_getParamName(objhandle, aParamIndex);
            return Marshal.PtrToStringAnsi(p);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint FreeverbFilter_getParamType(IntPtr aObjHandle, uint aParamIndex);
        public uint getParamType(uint aParamIndex)
        {
            return FreeverbFilter_getParamType(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float FreeverbFilter_getParamMax(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMax(uint aParamIndex)
        {
            return FreeverbFilter_getParamMax(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float FreeverbFilter_getParamMin(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMin(uint aParamIndex)
        {
            return FreeverbFilter_getParamMin(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int FreeverbFilter_setParams(IntPtr aObjHandle, float aMode, float aRoomSize, float aDamp, float aWidth);
        public int setParams(float aMode, float aRoomSize, float aDamp, float aWidth)
        {
            return FreeverbFilter_setParams(objhandle, aMode, aRoomSize, aDamp, aWidth);
        }
    }
}
