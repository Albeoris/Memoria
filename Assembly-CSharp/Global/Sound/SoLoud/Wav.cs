using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class Wav : SoloudObject
    {

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Wav_create();
        public Wav()
        {
            objhandle = Wav_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Wav_destroy(IntPtr aObjHandle);
        ~Wav()
        {
            Wav_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Wav_load(IntPtr aObjHandle, [MarshalAs(UnmanagedType.LPStr)] string aFilename);
        public int load(string aFilename)
        {
            return Wav_load(objhandle, aFilename);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Wav_loadMemEx(IntPtr aObjHandle, IntPtr aMem, uint aLength, bool aCopy, bool aTakeOwnership);
        public int loadMem(IntPtr aMem, uint aLength, bool aCopy = false, bool aTakeOwnership = true)
        {
            return Wav_loadMemEx(objhandle, aMem, aLength, aCopy, aTakeOwnership);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Wav_loadFile(IntPtr aObjHandle, IntPtr aFile);
        public int loadFile(SoloudObject aFile)
        {
            return Wav_loadFile(objhandle, aFile.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Wav_loadRawWave8Ex(IntPtr aObjHandle, IntPtr aMem, uint aLength, float aSamplerate, uint aChannels);
        public int loadRawWave8(IntPtr aMem, uint aLength, float aSamplerate = 44100.0f, uint aChannels = 1)
        {
            return Wav_loadRawWave8Ex(objhandle, aMem, aLength, aSamplerate, aChannels);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Wav_loadRawWave16Ex(IntPtr aObjHandle, IntPtr aMem, uint aLength, float aSamplerate, uint aChannels);
        public int loadRawWave16(IntPtr aMem, uint aLength, float aSamplerate = 44100.0f, uint aChannels = 1)
        {
            return Wav_loadRawWave16Ex(objhandle, aMem, aLength, aSamplerate, aChannels);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Wav_loadRawWaveEx(IntPtr aObjHandle, float[] aMem, uint aLength, float aSamplerate, uint aChannels, bool aCopy, bool aTakeOwnership);
        public int loadRawWave(float[] aMem, uint aLength, float aSamplerate = 44100.0f, uint aChannels = 1, bool aCopy = false, bool aTakeOwnership = true)
        {
            return Wav_loadRawWaveEx(objhandle, aMem, aLength, aSamplerate, aChannels, aCopy, aTakeOwnership);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double Wav_getLength(IntPtr aObjHandle);
        public double getLength()
        {
            return Wav_getLength(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_setVolume(IntPtr aObjHandle, float aVolume);
        public void setVolume(float aVolume)
        {
            Wav_setVolume(objhandle, aVolume);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_setLooping(IntPtr aObjHandle, int aLoop);
        public void setLooping(int aLoop)
        {
            Wav_setLooping(objhandle, aLoop);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_set3dMinMaxDistance(IntPtr aObjHandle, float aMinDistance, float aMaxDistance);
        public void set3dMinMaxDistance(float aMinDistance, float aMaxDistance)
        {
            Wav_set3dMinMaxDistance(objhandle, aMinDistance, aMaxDistance);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_set3dAttenuation(IntPtr aObjHandle, uint aAttenuationModel, float aAttenuationRolloffFactor);
        public void set3dAttenuation(uint aAttenuationModel, float aAttenuationRolloffFactor)
        {
            Wav_set3dAttenuation(objhandle, aAttenuationModel, aAttenuationRolloffFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_set3dDopplerFactor(IntPtr aObjHandle, float aDopplerFactor);
        public void set3dDopplerFactor(float aDopplerFactor)
        {
            Wav_set3dDopplerFactor(objhandle, aDopplerFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_set3dListenerRelative(IntPtr aObjHandle, int aListenerRelative);
        public void set3dListenerRelative(int aListenerRelative)
        {
            Wav_set3dListenerRelative(objhandle, aListenerRelative);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_set3dDistanceDelay(IntPtr aObjHandle, int aDistanceDelay);
        public void set3dDistanceDelay(int aDistanceDelay)
        {
            Wav_set3dDistanceDelay(objhandle, aDistanceDelay);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_set3dColliderEx(IntPtr aObjHandle, IntPtr aCollider, int aUserData);
        public void set3dCollider(SoloudObject aCollider, int aUserData = 0)
        {
            Wav_set3dColliderEx(objhandle, aCollider.objhandle, aUserData);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_set3dAttenuator(IntPtr aObjHandle, IntPtr aAttenuator);
        public void set3dAttenuator(SoloudObject aAttenuator)
        {
            Wav_set3dAttenuator(objhandle, aAttenuator.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_setInaudibleBehavior(IntPtr aObjHandle, int aMustTick, int aKill);
        public void setInaudibleBehavior(int aMustTick, int aKill)
        {
            Wav_setInaudibleBehavior(objhandle, aMustTick, aKill);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_setLoopPoint(IntPtr aObjHandle, double aLoopPoint);
        public void setLoopPoint(double aLoopPoint)
        {
            Wav_setLoopPoint(objhandle, aLoopPoint);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double Wav_getLoopPoint(IntPtr aObjHandle);
        public double getLoopPoint()
        {
            return Wav_getLoopPoint(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_setFilter(IntPtr aObjHandle, uint aFilterId, IntPtr aFilter);
        public void setFilter(uint aFilterId, SoloudObject aFilter)
        {
            Wav_setFilter(objhandle, aFilterId, aFilter.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Wav_stop(IntPtr aObjHandle);
        public void stop()
        {
            Wav_stop(objhandle);
        }
    }
}
