using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class FlangerFilter : SoloudObject
    {
        public const int WET = 0;
        public const int DELAY = 1;
        public const int FREQ = 2;

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr FlangerFilter_create();
        public FlangerFilter()
        {
            objhandle = FlangerFilter_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr FlangerFilter_destroy(IntPtr aObjHandle);
        ~FlangerFilter()
        {
            FlangerFilter_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int FlangerFilter_getParamCount(IntPtr aObjHandle);
        public int getParamCount()
        {
            return FlangerFilter_getParamCount(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr FlangerFilter_getParamName(IntPtr aObjHandle, uint aParamIndex);
        public string getParamName(uint aParamIndex)
        {
            IntPtr p = FlangerFilter_getParamName(objhandle, aParamIndex);
            return Marshal.PtrToStringAnsi(p);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint FlangerFilter_getParamType(IntPtr aObjHandle, uint aParamIndex);
        public uint getParamType(uint aParamIndex)
        {
            return FlangerFilter_getParamType(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float FlangerFilter_getParamMax(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMax(uint aParamIndex)
        {
            return FlangerFilter_getParamMax(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float FlangerFilter_getParamMin(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMin(uint aParamIndex)
        {
            return FlangerFilter_getParamMin(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int FlangerFilter_setParams(IntPtr aObjHandle, float aDelay, float aFreq);
        public int setParams(float aDelay, float aFreq)
        {
            return FlangerFilter_setParams(objhandle, aDelay, aFreq);
        }
    }
}
