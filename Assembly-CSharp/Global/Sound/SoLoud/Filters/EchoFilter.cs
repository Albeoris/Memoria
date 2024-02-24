using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class EchoFilter : SoloudObject
    {
        public const int WET = 0;
        public const int DELAY = 1;
        public const int DECAY = 2;
        public const int FILTER = 3;

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr EchoFilter_create();
        public EchoFilter()
        {
            objhandle = EchoFilter_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr EchoFilter_destroy(IntPtr aObjHandle);
        ~EchoFilter()
        {
            EchoFilter_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EchoFilter_getParamCount(IntPtr aObjHandle);
        public int getParamCount()
        {
            return EchoFilter_getParamCount(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr EchoFilter_getParamName(IntPtr aObjHandle, uint aParamIndex);
        public string getParamName(uint aParamIndex)
        {
            IntPtr p = EchoFilter_getParamName(objhandle, aParamIndex);
            return Marshal.PtrToStringAnsi(p);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint EchoFilter_getParamType(IntPtr aObjHandle, uint aParamIndex);
        public uint getParamType(uint aParamIndex)
        {
            return EchoFilter_getParamType(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float EchoFilter_getParamMax(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMax(uint aParamIndex)
        {
            return EchoFilter_getParamMax(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float EchoFilter_getParamMin(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMin(uint aParamIndex)
        {
            return EchoFilter_getParamMin(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EchoFilter_setParamsEx(IntPtr aObjHandle, float aDelay, float aDecay, float aFilter);
        public int setParams(float aDelay, float aDecay = 0.7f, float aFilter = 0.0f)
        {
            return EchoFilter_setParamsEx(objhandle, aDelay, aDecay, aFilter);
        }
    }
}
