using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class WavStream : SoloudObject
    {

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr WavStream_create();
        public WavStream()
        {
            objhandle = WavStream_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_destroy(IntPtr aObjHandle);
        ~WavStream()
        {
            WavStream_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int WavStream_load(IntPtr aObjHandle, [MarshalAs(UnmanagedType.LPStr)] string aFilename);
        public int load(string aFilename)
        {
            return WavStream_load(objhandle, aFilename);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int WavStream_loadMemEx(IntPtr aObjHandle, IntPtr aData, uint aDataLen, bool aCopy, bool aTakeOwnership);
        public int loadMem(IntPtr aData, uint aDataLen, bool aCopy = false, bool aTakeOwnership = true)
        {
            return WavStream_loadMemEx(objhandle, aData, aDataLen, aCopy, aTakeOwnership);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int WavStream_loadToMem(IntPtr aObjHandle, [MarshalAs(UnmanagedType.LPStr)] string aFilename);
        public int loadToMem(string aFilename)
        {
            return WavStream_loadToMem(objhandle, aFilename);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int WavStream_loadFile(IntPtr aObjHandle, IntPtr aFile);
        public int loadFile(SoloudObject aFile)
        {
            return WavStream_loadFile(objhandle, aFile.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int WavStream_loadFileToMem(IntPtr aObjHandle, IntPtr aFile);
        public int loadFileToMem(SoloudObject aFile)
        {
            return WavStream_loadFileToMem(objhandle, aFile.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern double WavStream_getLength(IntPtr aObjHandle);
        public double getLength()
        {
            return WavStream_getLength(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_setVolume(IntPtr aObjHandle, float aVolume);
        public void setVolume(float aVolume)
        {
            WavStream_setVolume(objhandle, aVolume);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_setLooping(IntPtr aObjHandle, int aLoop);
        public void setLooping(int aLoop)
        {
            WavStream_setLooping(objhandle, aLoop);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_set3dMinMaxDistance(IntPtr aObjHandle, float aMinDistance, float aMaxDistance);
        public void set3dMinMaxDistance(float aMinDistance, float aMaxDistance)
        {
            WavStream_set3dMinMaxDistance(objhandle, aMinDistance, aMaxDistance);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_set3dAttenuation(IntPtr aObjHandle, uint aAttenuationModel, float aAttenuationRolloffFactor);
        public void set3dAttenuation(uint aAttenuationModel, float aAttenuationRolloffFactor)
        {
            WavStream_set3dAttenuation(objhandle, aAttenuationModel, aAttenuationRolloffFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_set3dDopplerFactor(IntPtr aObjHandle, float aDopplerFactor);
        public void set3dDopplerFactor(float aDopplerFactor)
        {
            WavStream_set3dDopplerFactor(objhandle, aDopplerFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_set3dListenerRelative(IntPtr aObjHandle, int aListenerRelative);
        public void set3dListenerRelative(int aListenerRelative)
        {
            WavStream_set3dListenerRelative(objhandle, aListenerRelative);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_set3dDistanceDelay(IntPtr aObjHandle, int aDistanceDelay);
        public void set3dDistanceDelay(int aDistanceDelay)
        {
            WavStream_set3dDistanceDelay(objhandle, aDistanceDelay);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_set3dColliderEx(IntPtr aObjHandle, IntPtr aCollider, int aUserData);
        public void set3dCollider(SoloudObject aCollider, int aUserData = 0)
        {
            WavStream_set3dColliderEx(objhandle, aCollider.objhandle, aUserData);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_set3dAttenuator(IntPtr aObjHandle, IntPtr aAttenuator);
        public void set3dAttenuator(SoloudObject aAttenuator)
        {
            WavStream_set3dAttenuator(objhandle, aAttenuator.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_setInaudibleBehavior(IntPtr aObjHandle, int aMustTick, int aKill);
        public void setInaudibleBehavior(int aMustTick, int aKill)
        {
            WavStream_setInaudibleBehavior(objhandle, aMustTick, aKill);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_setLoopStartPoint(IntPtr aObjHandle, double aLoopPoint);
        public void setLoopStartPoint(double aLoopPoint)
        {
            WavStream_setLoopStartPoint(objhandle, aLoopPoint);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_setLoopEndPoint(IntPtr aObjHandle, double aLoopPoint);
        public void setLoopEndPoint(double aLoopPoint)
        {
            WavStream_setLoopEndPoint(objhandle, aLoopPoint);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern double WavStream_getLoopStartPoint(IntPtr aObjHandle);
        public double getLoopStartPoint()
        {
            return WavStream_getLoopStartPoint(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern double WavStream_getLoopEndPoint(IntPtr aObjHandle);
        public double getLoopEndPoint()
        {
            return WavStream_getLoopEndPoint(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_setFilter(IntPtr aObjHandle, uint aFilterId, IntPtr aFilter);
        public void setFilter(uint aFilterId, SoloudObject aFilter)
        {
            WavStream_setFilter(objhandle, aFilterId, aFilter.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WavStream_stop(IntPtr aObjHandle);
        public void stop()
        {
            WavStream_stop(objhandle);
        }
    }
}
