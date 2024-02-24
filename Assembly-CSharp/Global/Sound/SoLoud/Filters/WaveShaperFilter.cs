using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class WaveShaperFilter : SoloudObject
    {
        public const int WET = 0;
        public const int AMOUNT = 1;

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr WaveShaperFilter_create();
        public WaveShaperFilter()
        {
            objhandle = WaveShaperFilter_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr WaveShaperFilter_destroy(IntPtr aObjHandle);
        ~WaveShaperFilter()
        {
            WaveShaperFilter_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int WaveShaperFilter_setParams(IntPtr aObjHandle, float aAmount);
        public int setParams(float aAmount)
        {
            return WaveShaperFilter_setParams(objhandle, aAmount);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int WaveShaperFilter_getParamCount(IntPtr aObjHandle);
        public int getParamCount()
        {
            return WaveShaperFilter_getParamCount(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr WaveShaperFilter_getParamName(IntPtr aObjHandle, uint aParamIndex);
        public string getParamName(uint aParamIndex)
        {
            IntPtr p = WaveShaperFilter_getParamName(objhandle, aParamIndex);
            return Marshal.PtrToStringAnsi(p);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint WaveShaperFilter_getParamType(IntPtr aObjHandle, uint aParamIndex);
        public uint getParamType(uint aParamIndex)
        {
            return WaveShaperFilter_getParamType(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float WaveShaperFilter_getParamMax(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMax(uint aParamIndex)
        {
            return WaveShaperFilter_getParamMax(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float WaveShaperFilter_getParamMin(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMin(uint aParamIndex)
        {
            return WaveShaperFilter_getParamMin(objhandle, aParamIndex);
        }
    }
}
