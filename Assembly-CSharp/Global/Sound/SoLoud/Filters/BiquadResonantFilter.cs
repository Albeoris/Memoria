using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class BiquadResonantFilter : SoloudObject
    {
        public const int LOWPASS = 0;
        public const int HIGHPASS = 1;
        public const int BANDPASS = 2;
        public const int WET = 0;
        public const int TYPE = 1;
        public const int FREQUENCY = 2;
        public const int RESONANCE = 3;

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr BiquadResonantFilter_create();
        public BiquadResonantFilter()
        {
            objhandle = BiquadResonantFilter_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr BiquadResonantFilter_destroy(IntPtr aObjHandle);
        ~BiquadResonantFilter()
        {
            BiquadResonantFilter_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int BiquadResonantFilter_getParamCount(IntPtr aObjHandle);
        public int getParamCount()
        {
            return BiquadResonantFilter_getParamCount(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr BiquadResonantFilter_getParamName(IntPtr aObjHandle, uint aParamIndex);
        public string getParamName(uint aParamIndex)
        {
            IntPtr p = BiquadResonantFilter_getParamName(objhandle, aParamIndex);
            return Marshal.PtrToStringAnsi(p);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint BiquadResonantFilter_getParamType(IntPtr aObjHandle, uint aParamIndex);
        public uint getParamType(uint aParamIndex)
        {
            return BiquadResonantFilter_getParamType(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float BiquadResonantFilter_getParamMax(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMax(uint aParamIndex)
        {
            return BiquadResonantFilter_getParamMax(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float BiquadResonantFilter_getParamMin(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMin(uint aParamIndex)
        {
            return BiquadResonantFilter_getParamMin(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int BiquadResonantFilter_setParams(IntPtr aObjHandle, int aType, float aFrequency, float aResonance);
        public int setParams(int aType, float aFrequency, float aResonance)
        {
            return BiquadResonantFilter_setParams(objhandle, aType, aFrequency, aResonance);
        }
    }
}
