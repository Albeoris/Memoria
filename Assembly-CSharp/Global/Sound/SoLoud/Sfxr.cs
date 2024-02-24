using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class Sfxr : SoloudObject
    {
        public const int COIN = 0;
        public const int LASER = 1;
        public const int EXPLOSION = 2;
        public const int POWERUP = 3;
        public const int HURT = 4;
        public const int JUMP = 5;
        public const int BLIP = 6;

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Sfxr_create();
        public Sfxr()
        {
            objhandle = Sfxr_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Sfxr_destroy(IntPtr aObjHandle);
        ~Sfxr()
        {
            Sfxr_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_resetParams(IntPtr aObjHandle);
        public void resetParams()
        {
            Sfxr_resetParams(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Sfxr_loadParams(IntPtr aObjHandle, [MarshalAs(UnmanagedType.LPStr)] string aFilename);
        public int loadParams(string aFilename)
        {
            return Sfxr_loadParams(objhandle, aFilename);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Sfxr_loadParamsMemEx(IntPtr aObjHandle, IntPtr aMem, uint aLength, bool aCopy, bool aTakeOwnership);
        public int loadParamsMem(IntPtr aMem, uint aLength, bool aCopy = false, bool aTakeOwnership = true)
        {
            return Sfxr_loadParamsMemEx(objhandle, aMem, aLength, aCopy, aTakeOwnership);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Sfxr_loadParamsFile(IntPtr aObjHandle, IntPtr aFile);
        public int loadParamsFile(SoloudObject aFile)
        {
            return Sfxr_loadParamsFile(objhandle, aFile.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Sfxr_loadPreset(IntPtr aObjHandle, int aPresetNo, int aRandSeed);
        public int loadPreset(int aPresetNo, int aRandSeed)
        {
            return Sfxr_loadPreset(objhandle, aPresetNo, aRandSeed);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_setVolume(IntPtr aObjHandle, float aVolume);
        public void setVolume(float aVolume)
        {
            Sfxr_setVolume(objhandle, aVolume);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_setLooping(IntPtr aObjHandle, int aLoop);
        public void setLooping(int aLoop)
        {
            Sfxr_setLooping(objhandle, aLoop);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_set3dMinMaxDistance(IntPtr aObjHandle, float aMinDistance, float aMaxDistance);
        public void set3dMinMaxDistance(float aMinDistance, float aMaxDistance)
        {
            Sfxr_set3dMinMaxDistance(objhandle, aMinDistance, aMaxDistance);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_set3dAttenuation(IntPtr aObjHandle, uint aAttenuationModel, float aAttenuationRolloffFactor);
        public void set3dAttenuation(uint aAttenuationModel, float aAttenuationRolloffFactor)
        {
            Sfxr_set3dAttenuation(objhandle, aAttenuationModel, aAttenuationRolloffFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_set3dDopplerFactor(IntPtr aObjHandle, float aDopplerFactor);
        public void set3dDopplerFactor(float aDopplerFactor)
        {
            Sfxr_set3dDopplerFactor(objhandle, aDopplerFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_set3dListenerRelative(IntPtr aObjHandle, int aListenerRelative);
        public void set3dListenerRelative(int aListenerRelative)
        {
            Sfxr_set3dListenerRelative(objhandle, aListenerRelative);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_set3dDistanceDelay(IntPtr aObjHandle, int aDistanceDelay);
        public void set3dDistanceDelay(int aDistanceDelay)
        {
            Sfxr_set3dDistanceDelay(objhandle, aDistanceDelay);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_set3dColliderEx(IntPtr aObjHandle, IntPtr aCollider, int aUserData);
        public void set3dCollider(SoloudObject aCollider, int aUserData = 0)
        {
            Sfxr_set3dColliderEx(objhandle, aCollider.objhandle, aUserData);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_set3dAttenuator(IntPtr aObjHandle, IntPtr aAttenuator);
        public void set3dAttenuator(SoloudObject aAttenuator)
        {
            Sfxr_set3dAttenuator(objhandle, aAttenuator.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_setInaudibleBehavior(IntPtr aObjHandle, int aMustTick, int aKill);
        public void setInaudibleBehavior(int aMustTick, int aKill)
        {
            Sfxr_setInaudibleBehavior(objhandle, aMustTick, aKill);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_setLoopPoint(IntPtr aObjHandle, double aLoopPoint);
        public void setLoopPoint(double aLoopPoint)
        {
            Sfxr_setLoopPoint(objhandle, aLoopPoint);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double Sfxr_getLoopPoint(IntPtr aObjHandle);
        public double getLoopPoint()
        {
            return Sfxr_getLoopPoint(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_setFilter(IntPtr aObjHandle, uint aFilterId, IntPtr aFilter);
        public void setFilter(uint aFilterId, SoloudObject aFilter)
        {
            Sfxr_setFilter(objhandle, aFilterId, aFilter.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Sfxr_stop(IntPtr aObjHandle);
        public void stop()
        {
            Sfxr_stop(objhandle);
        }
    }
}
