using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class BassboostFilter : SoloudObject
    {
        public const int WET = 0;
        public const int BOOST = 1;

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr BassboostFilter_create();
        public BassboostFilter()
        {
            objhandle = BassboostFilter_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr BassboostFilter_destroy(IntPtr aObjHandle);
        ~BassboostFilter()
        {
            BassboostFilter_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int BassboostFilter_getParamCount(IntPtr aObjHandle);
        public int getParamCount()
        {
            return BassboostFilter_getParamCount(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr BassboostFilter_getParamName(IntPtr aObjHandle, uint aParamIndex);
        public string getParamName(uint aParamIndex)
        {
            IntPtr p = BassboostFilter_getParamName(objhandle, aParamIndex);
            return Marshal.PtrToStringAnsi(p);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint BassboostFilter_getParamType(IntPtr aObjHandle, uint aParamIndex);
        public uint getParamType(uint aParamIndex)
        {
            return BassboostFilter_getParamType(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float BassboostFilter_getParamMax(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMax(uint aParamIndex)
        {
            return BassboostFilter_getParamMax(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float BassboostFilter_getParamMin(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMin(uint aParamIndex)
        {
            return BassboostFilter_getParamMin(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int BassboostFilter_setParams(IntPtr aObjHandle, float aBoost);
        public int setParams(float aBoost)
        {
            return BassboostFilter_setParams(objhandle, aBoost);
        }
    }
}
