using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class Monotone : SoloudObject
    {
        public const int WAVE_SQUARE = 0;
        public const int WAVE_SAW = 1;
        public const int WAVE_SIN = 2;
        public const int WAVE_TRIANGLE = 3;
        public const int WAVE_BOUNCE = 4;
        public const int WAVE_JAWS = 5;
        public const int WAVE_HUMPS = 6;
        public const int WAVE_FSQUARE = 7;
        public const int WAVE_FSAW = 8;

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Monotone_create();
        public Monotone()
        {
            objhandle = Monotone_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Monotone_destroy(IntPtr aObjHandle);
        ~Monotone()
        {
            Monotone_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Monotone_setParamsEx(IntPtr aObjHandle, int aHardwareChannels, int aWaveform);
        public int setParams(int aHardwareChannels, int aWaveform = WAVE_SQUARE)
        {
            return Monotone_setParamsEx(objhandle, aHardwareChannels, aWaveform);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Monotone_load(IntPtr aObjHandle, [MarshalAs(UnmanagedType.LPStr)] string aFilename);
        public int load(string aFilename)
        {
            return Monotone_load(objhandle, aFilename);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Monotone_loadMemEx(IntPtr aObjHandle, IntPtr aMem, uint aLength, bool aCopy, bool aTakeOwnership);
        public int loadMem(IntPtr aMem, uint aLength, bool aCopy = false, bool aTakeOwnership = true)
        {
            return Monotone_loadMemEx(objhandle, aMem, aLength, aCopy, aTakeOwnership);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Monotone_loadFile(IntPtr aObjHandle, IntPtr aFile);
        public int loadFile(SoloudObject aFile)
        {
            return Monotone_loadFile(objhandle, aFile.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_setVolume(IntPtr aObjHandle, float aVolume);
        public void setVolume(float aVolume)
        {
            Monotone_setVolume(objhandle, aVolume);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_setLooping(IntPtr aObjHandle, int aLoop);
        public void setLooping(int aLoop)
        {
            Monotone_setLooping(objhandle, aLoop);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_set3dMinMaxDistance(IntPtr aObjHandle, float aMinDistance, float aMaxDistance);
        public void set3dMinMaxDistance(float aMinDistance, float aMaxDistance)
        {
            Monotone_set3dMinMaxDistance(objhandle, aMinDistance, aMaxDistance);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_set3dAttenuation(IntPtr aObjHandle, uint aAttenuationModel, float aAttenuationRolloffFactor);
        public void set3dAttenuation(uint aAttenuationModel, float aAttenuationRolloffFactor)
        {
            Monotone_set3dAttenuation(objhandle, aAttenuationModel, aAttenuationRolloffFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_set3dDopplerFactor(IntPtr aObjHandle, float aDopplerFactor);
        public void set3dDopplerFactor(float aDopplerFactor)
        {
            Monotone_set3dDopplerFactor(objhandle, aDopplerFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_set3dListenerRelative(IntPtr aObjHandle, int aListenerRelative);
        public void set3dListenerRelative(int aListenerRelative)
        {
            Monotone_set3dListenerRelative(objhandle, aListenerRelative);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_set3dDistanceDelay(IntPtr aObjHandle, int aDistanceDelay);
        public void set3dDistanceDelay(int aDistanceDelay)
        {
            Monotone_set3dDistanceDelay(objhandle, aDistanceDelay);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_set3dColliderEx(IntPtr aObjHandle, IntPtr aCollider, int aUserData);
        public void set3dCollider(SoloudObject aCollider, int aUserData = 0)
        {
            Monotone_set3dColliderEx(objhandle, aCollider.objhandle, aUserData);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_set3dAttenuator(IntPtr aObjHandle, IntPtr aAttenuator);
        public void set3dAttenuator(SoloudObject aAttenuator)
        {
            Monotone_set3dAttenuator(objhandle, aAttenuator.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_setInaudibleBehavior(IntPtr aObjHandle, int aMustTick, int aKill);
        public void setInaudibleBehavior(int aMustTick, int aKill)
        {
            Monotone_setInaudibleBehavior(objhandle, aMustTick, aKill);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_setLoopPoint(IntPtr aObjHandle, double aLoopPoint);
        public void setLoopPoint(double aLoopPoint)
        {
            Monotone_setLoopPoint(objhandle, aLoopPoint);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double Monotone_getLoopPoint(IntPtr aObjHandle);
        public double getLoopPoint()
        {
            return Monotone_getLoopPoint(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_setFilter(IntPtr aObjHandle, uint aFilterId, IntPtr aFilter);
        public void setFilter(uint aFilterId, SoloudObject aFilter)
        {
            Monotone_setFilter(objhandle, aFilterId, aFilter.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Monotone_stop(IntPtr aObjHandle);
        public void stop()
        {
            Monotone_stop(objhandle);
        }
    }
}
