using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class DCRemovalFilter : SoloudObject
    {

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr DCRemovalFilter_create();
        public DCRemovalFilter()
        {
            objhandle = DCRemovalFilter_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr DCRemovalFilter_destroy(IntPtr aObjHandle);
        ~DCRemovalFilter()
        {
            DCRemovalFilter_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int DCRemovalFilter_setParamsEx(IntPtr aObjHandle, float aLength);
        public int setParams(float aLength = 0.1f)
        {
            return DCRemovalFilter_setParamsEx(objhandle, aLength);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int DCRemovalFilter_getParamCount(IntPtr aObjHandle);
        public int getParamCount()
        {
            return DCRemovalFilter_getParamCount(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr DCRemovalFilter_getParamName(IntPtr aObjHandle, uint aParamIndex);
        public string getParamName(uint aParamIndex)
        {
            IntPtr p = DCRemovalFilter_getParamName(objhandle, aParamIndex);
            return Marshal.PtrToStringAnsi(p);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint DCRemovalFilter_getParamType(IntPtr aObjHandle, uint aParamIndex);
        public uint getParamType(uint aParamIndex)
        {
            return DCRemovalFilter_getParamType(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float DCRemovalFilter_getParamMax(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMax(uint aParamIndex)
        {
            return DCRemovalFilter_getParamMax(objhandle, aParamIndex);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern float DCRemovalFilter_getParamMin(IntPtr aObjHandle, uint aParamIndex);
        public float getParamMin(uint aParamIndex)
        {
            return DCRemovalFilter_getParamMin(objhandle, aParamIndex);
        }
    }
}
