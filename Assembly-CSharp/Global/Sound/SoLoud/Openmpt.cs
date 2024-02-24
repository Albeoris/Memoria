using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class Openmpt : SoloudObject
    {

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Openmpt_create();
        public Openmpt()
        {
            objhandle = Openmpt_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Openmpt_destroy(IntPtr aObjHandle);
        ~Openmpt()
        {
            Openmpt_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Openmpt_load(IntPtr aObjHandle, [MarshalAs(UnmanagedType.LPStr)] string aFilename);
        public int load(string aFilename)
        {
            return Openmpt_load(objhandle, aFilename);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Openmpt_loadMemEx(IntPtr aObjHandle, IntPtr aMem, uint aLength, bool aCopy, bool aTakeOwnership);
        public int loadMem(IntPtr aMem, uint aLength, bool aCopy = false, bool aTakeOwnership = true)
        {
            return Openmpt_loadMemEx(objhandle, aMem, aLength, aCopy, aTakeOwnership);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Openmpt_loadFile(IntPtr aObjHandle, IntPtr aFile);
        public int loadFile(SoloudObject aFile)
        {
            return Openmpt_loadFile(objhandle, aFile.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_setVolume(IntPtr aObjHandle, float aVolume);
        public void setVolume(float aVolume)
        {
            Openmpt_setVolume(objhandle, aVolume);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_setLooping(IntPtr aObjHandle, int aLoop);
        public void setLooping(int aLoop)
        {
            Openmpt_setLooping(objhandle, aLoop);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_set3dMinMaxDistance(IntPtr aObjHandle, float aMinDistance, float aMaxDistance);
        public void set3dMinMaxDistance(float aMinDistance, float aMaxDistance)
        {
            Openmpt_set3dMinMaxDistance(objhandle, aMinDistance, aMaxDistance);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_set3dAttenuation(IntPtr aObjHandle, uint aAttenuationModel, float aAttenuationRolloffFactor);
        public void set3dAttenuation(uint aAttenuationModel, float aAttenuationRolloffFactor)
        {
            Openmpt_set3dAttenuation(objhandle, aAttenuationModel, aAttenuationRolloffFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_set3dDopplerFactor(IntPtr aObjHandle, float aDopplerFactor);
        public void set3dDopplerFactor(float aDopplerFactor)
        {
            Openmpt_set3dDopplerFactor(objhandle, aDopplerFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_set3dListenerRelative(IntPtr aObjHandle, int aListenerRelative);
        public void set3dListenerRelative(int aListenerRelative)
        {
            Openmpt_set3dListenerRelative(objhandle, aListenerRelative);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_set3dDistanceDelay(IntPtr aObjHandle, int aDistanceDelay);
        public void set3dDistanceDelay(int aDistanceDelay)
        {
            Openmpt_set3dDistanceDelay(objhandle, aDistanceDelay);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_set3dColliderEx(IntPtr aObjHandle, IntPtr aCollider, int aUserData);
        public void set3dCollider(SoloudObject aCollider, int aUserData = 0)
        {
            Openmpt_set3dColliderEx(objhandle, aCollider.objhandle, aUserData);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_set3dAttenuator(IntPtr aObjHandle, IntPtr aAttenuator);
        public void set3dAttenuator(SoloudObject aAttenuator)
        {
            Openmpt_set3dAttenuator(objhandle, aAttenuator.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_setInaudibleBehavior(IntPtr aObjHandle, int aMustTick, int aKill);
        public void setInaudibleBehavior(int aMustTick, int aKill)
        {
            Openmpt_setInaudibleBehavior(objhandle, aMustTick, aKill);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_setLoopPoint(IntPtr aObjHandle, double aLoopPoint);
        public void setLoopPoint(double aLoopPoint)
        {
            Openmpt_setLoopPoint(objhandle, aLoopPoint);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double Openmpt_getLoopPoint(IntPtr aObjHandle);
        public double getLoopPoint()
        {
            return Openmpt_getLoopPoint(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_setFilter(IntPtr aObjHandle, uint aFilterId, IntPtr aFilter);
        public void setFilter(uint aFilterId, SoloudObject aFilter)
        {
            Openmpt_setFilter(objhandle, aFilterId, aFilter.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Openmpt_stop(IntPtr aObjHandle);
        public void stop()
        {
            Openmpt_stop(objhandle);
        }
    }
}
