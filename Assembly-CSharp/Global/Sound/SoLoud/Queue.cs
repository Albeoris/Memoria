using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class Queue : SoloudObject
    {

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_create")]
        internal static extern IntPtr Queue_create();
        public Queue()
        {
            objhandle = Queue_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_destroy")]
        internal static extern IntPtr Queue_destroy(IntPtr aObjHandle);
        ~Queue()
        {
            // Can cause a crash if called while quitting 
            if (!SdLibAPIWithSoloud.isQuitting) Queue_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_play")]
        internal static extern int Queue_play(IntPtr aObjHandle, IntPtr aSound);
        public int play(SoloudObject aSound)
        {
            return Queue_play(objhandle, aSound.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_getQueueCount")]
        internal static extern uint Queue_getQueueCount(IntPtr aObjHandle);
        public uint getQueueCount()
        {
            return Queue_getQueueCount(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_isCurrentlyPlaying")]
        internal static extern int Queue_isCurrentlyPlaying(IntPtr aObjHandle, IntPtr aSound);
        public int isCurrentlyPlaying(SoloudObject aSound)
        {
            return Queue_isCurrentlyPlaying(objhandle, aSound.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_setParamsFromAudioSource")]
        internal static extern int Queue_setParamsFromAudioSource(IntPtr aObjHandle, IntPtr aSound);
        public int setParamsFromAudioSource(SoloudObject aSound)
        {
            return Queue_setParamsFromAudioSource(objhandle, aSound.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_setParamsEx")]
        internal static extern int Queue_setParamsEx(IntPtr aObjHandle, float aSamplerate, uint aChannels);
        public int setParams(float aSamplerate, uint aChannels = 2)
        {
            return Queue_setParamsEx(objhandle, aSamplerate, aChannels);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_setVolume")]
        internal static extern void Queue_setVolume(IntPtr aObjHandle, float aVolume);
        public void setVolume(float aVolume)
        {
            Queue_setVolume(objhandle, aVolume);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_setLooping")]
        internal static extern void Queue_setLooping(IntPtr aObjHandle, int aLoop);
        public void setLooping(int aLoop)
        {
            Queue_setLooping(objhandle, aLoop);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_set3dMinMaxDistance")]
        internal static extern void Queue_set3dMinMaxDistance(IntPtr aObjHandle, float aMinDistance, float aMaxDistance);
        public void set3dMinMaxDistance(float aMinDistance, float aMaxDistance)
        {
            Queue_set3dMinMaxDistance(objhandle, aMinDistance, aMaxDistance);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_set3dAttenuation")]
        internal static extern void Queue_set3dAttenuation(IntPtr aObjHandle, uint aAttenuationModel, float aAttenuationRolloffFactor);
        public void set3dAttenuation(uint aAttenuationModel, float aAttenuationRolloffFactor)
        {
            Queue_set3dAttenuation(objhandle, aAttenuationModel, aAttenuationRolloffFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_set3dDopplerFactor")]
        internal static extern void Queue_set3dDopplerFactor(IntPtr aObjHandle, float aDopplerFactor);
        public void set3dDopplerFactor(float aDopplerFactor)
        {
            Queue_set3dDopplerFactor(objhandle, aDopplerFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_set3dListenerRelative")]
        internal static extern void Queue_set3dListenerRelative(IntPtr aObjHandle, int aListenerRelative);
        public void set3dListenerRelative(int aListenerRelative)
        {
            Queue_set3dListenerRelative(objhandle, aListenerRelative);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_set3dDistanceDelay")]
        internal static extern void Queue_set3dDistanceDelay(IntPtr aObjHandle, int aDistanceDelay);
        public void set3dDistanceDelay(int aDistanceDelay)
        {
            Queue_set3dDistanceDelay(objhandle, aDistanceDelay);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_set3dColliderEx")]
        internal static extern void Queue_set3dColliderEx(IntPtr aObjHandle, IntPtr aCollider, int aUserData);
        public void set3dCollider(SoloudObject aCollider, int aUserData = 0)
        {
            Queue_set3dColliderEx(objhandle, aCollider.objhandle, aUserData);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_set3dAttenuator")]
        internal static extern void Queue_set3dAttenuator(IntPtr aObjHandle, IntPtr aAttenuator);
        public void set3dAttenuator(SoloudObject aAttenuator)
        {
            Queue_set3dAttenuator(objhandle, aAttenuator.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_setInaudibleBehavior")]
        internal static extern void Queue_setInaudibleBehavior(IntPtr aObjHandle, int aMustTick, int aKill);
        public void setInaudibleBehavior(int aMustTick, int aKill)
        {
            Queue_setInaudibleBehavior(objhandle, aMustTick, aKill);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_setLoopPoint")]
        internal static extern void Queue_setLoopPoint(IntPtr aObjHandle, double aLoopPoint);
        public void setLoopPoint(double aLoopPoint)
        {
            Queue_setLoopPoint(objhandle, aLoopPoint);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_getLoopPoint")]
        internal static extern double Queue_getLoopPoint(IntPtr aObjHandle);
        public double getLoopPoint()
        {
            return Queue_getLoopPoint(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_setFilter")]
        internal static extern void Queue_setFilter(IntPtr aObjHandle, uint aFilterId, IntPtr aFilter);
        public void setFilter(uint aFilterId, SoloudObject aFilter)
        {
            Queue_setFilter(objhandle, aFilterId, aFilter.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Queue_stop")]
        internal static extern void Queue_stop(IntPtr aObjHandle);
        public void stop()
        {
            Queue_stop(objhandle);
        }
    }
}
