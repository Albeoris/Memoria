using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class TedSid : SoloudObject
    {

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TedSid_create();
        public TedSid()
        {
            objhandle = TedSid_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TedSid_destroy(IntPtr aObjHandle);
        ~TedSid()
        {
            TedSid_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TedSid_load(IntPtr aObjHandle, [MarshalAs(UnmanagedType.LPStr)] string aFilename);
        public int load(string aFilename)
        {
            return TedSid_load(objhandle, aFilename);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TedSid_loadToMem(IntPtr aObjHandle, [MarshalAs(UnmanagedType.LPStr)] string aFilename);
        public int loadToMem(string aFilename)
        {
            return TedSid_loadToMem(objhandle, aFilename);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TedSid_loadMemEx(IntPtr aObjHandle, IntPtr aMem, uint aLength, bool aCopy, bool aTakeOwnership);
        public int loadMem(IntPtr aMem, uint aLength, bool aCopy = false, bool aTakeOwnership = true)
        {
            return TedSid_loadMemEx(objhandle, aMem, aLength, aCopy, aTakeOwnership);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TedSid_loadFileToMem(IntPtr aObjHandle, IntPtr aFile);
        public int loadFileToMem(SoloudObject aFile)
        {
            return TedSid_loadFileToMem(objhandle, aFile.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TedSid_loadFile(IntPtr aObjHandle, IntPtr aFile);
        public int loadFile(SoloudObject aFile)
        {
            return TedSid_loadFile(objhandle, aFile.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_setVolume(IntPtr aObjHandle, float aVolume);
        public void setVolume(float aVolume)
        {
            TedSid_setVolume(objhandle, aVolume);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_setLooping(IntPtr aObjHandle, int aLoop);
        public void setLooping(int aLoop)
        {
            TedSid_setLooping(objhandle, aLoop);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_set3dMinMaxDistance(IntPtr aObjHandle, float aMinDistance, float aMaxDistance);
        public void set3dMinMaxDistance(float aMinDistance, float aMaxDistance)
        {
            TedSid_set3dMinMaxDistance(objhandle, aMinDistance, aMaxDistance);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_set3dAttenuation(IntPtr aObjHandle, uint aAttenuationModel, float aAttenuationRolloffFactor);
        public void set3dAttenuation(uint aAttenuationModel, float aAttenuationRolloffFactor)
        {
            TedSid_set3dAttenuation(objhandle, aAttenuationModel, aAttenuationRolloffFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_set3dDopplerFactor(IntPtr aObjHandle, float aDopplerFactor);
        public void set3dDopplerFactor(float aDopplerFactor)
        {
            TedSid_set3dDopplerFactor(objhandle, aDopplerFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_set3dListenerRelative(IntPtr aObjHandle, int aListenerRelative);
        public void set3dListenerRelative(int aListenerRelative)
        {
            TedSid_set3dListenerRelative(objhandle, aListenerRelative);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_set3dDistanceDelay(IntPtr aObjHandle, int aDistanceDelay);
        public void set3dDistanceDelay(int aDistanceDelay)
        {
            TedSid_set3dDistanceDelay(objhandle, aDistanceDelay);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_set3dColliderEx(IntPtr aObjHandle, IntPtr aCollider, int aUserData);
        public void set3dCollider(SoloudObject aCollider, int aUserData = 0)
        {
            TedSid_set3dColliderEx(objhandle, aCollider.objhandle, aUserData);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_set3dAttenuator(IntPtr aObjHandle, IntPtr aAttenuator);
        public void set3dAttenuator(SoloudObject aAttenuator)
        {
            TedSid_set3dAttenuator(objhandle, aAttenuator.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_setInaudibleBehavior(IntPtr aObjHandle, int aMustTick, int aKill);
        public void setInaudibleBehavior(int aMustTick, int aKill)
        {
            TedSid_setInaudibleBehavior(objhandle, aMustTick, aKill);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_setLoopPoint(IntPtr aObjHandle, double aLoopPoint);
        public void setLoopPoint(double aLoopPoint)
        {
            TedSid_setLoopPoint(objhandle, aLoopPoint);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double TedSid_getLoopPoint(IntPtr aObjHandle);
        public double getLoopPoint()
        {
            return TedSid_getLoopPoint(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_setFilter(IntPtr aObjHandle, uint aFilterId, IntPtr aFilter);
        public void setFilter(uint aFilterId, SoloudObject aFilter)
        {
            TedSid_setFilter(objhandle, aFilterId, aFilter.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TedSid_stop(IntPtr aObjHandle);
        public void stop()
        {
            TedSid_stop(objhandle);
        }
    }
}
