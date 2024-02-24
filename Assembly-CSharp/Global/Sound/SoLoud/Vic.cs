using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
    public class Vic : SoloudObject
    {
        public const int PAL = 0;
        public const int NTSC = 1;
        public const int BASS = 0;
        public const int ALTO = 1;
        public const int SOPRANO = 2;
        public const int NOISE = 3;
        public const int MAX_REGS = 4;

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Vic_create();
        public Vic()
        {
            objhandle = Vic_create();
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Vic_destroy(IntPtr aObjHandle);
        ~Vic()
        {
            Vic_destroy(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_setModel(IntPtr aObjHandle, int model);
        public void setModel(int model)
        {
            Vic_setModel(objhandle, model);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Vic_getModel(IntPtr aObjHandle);
        public int getModel()
        {
            return Vic_getModel(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_setRegister(IntPtr aObjHandle, int reg, byte value);
        public void setRegister(int reg, byte value)
        {
            Vic_setRegister(objhandle, reg, value);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern byte Vic_getRegister(IntPtr aObjHandle, int reg);
        public byte getRegister(int reg)
        {
            return Vic_getRegister(objhandle, reg);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_setVolume(IntPtr aObjHandle, float aVolume);
        public void setVolume(float aVolume)
        {
            Vic_setVolume(objhandle, aVolume);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_setLooping(IntPtr aObjHandle, int aLoop);
        public void setLooping(int aLoop)
        {
            Vic_setLooping(objhandle, aLoop);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_set3dMinMaxDistance(IntPtr aObjHandle, float aMinDistance, float aMaxDistance);
        public void set3dMinMaxDistance(float aMinDistance, float aMaxDistance)
        {
            Vic_set3dMinMaxDistance(objhandle, aMinDistance, aMaxDistance);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_set3dAttenuation(IntPtr aObjHandle, uint aAttenuationModel, float aAttenuationRolloffFactor);
        public void set3dAttenuation(uint aAttenuationModel, float aAttenuationRolloffFactor)
        {
            Vic_set3dAttenuation(objhandle, aAttenuationModel, aAttenuationRolloffFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_set3dDopplerFactor(IntPtr aObjHandle, float aDopplerFactor);
        public void set3dDopplerFactor(float aDopplerFactor)
        {
            Vic_set3dDopplerFactor(objhandle, aDopplerFactor);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_set3dListenerRelative(IntPtr aObjHandle, int aListenerRelative);
        public void set3dListenerRelative(int aListenerRelative)
        {
            Vic_set3dListenerRelative(objhandle, aListenerRelative);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_set3dDistanceDelay(IntPtr aObjHandle, int aDistanceDelay);
        public void set3dDistanceDelay(int aDistanceDelay)
        {
            Vic_set3dDistanceDelay(objhandle, aDistanceDelay);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_set3dColliderEx(IntPtr aObjHandle, IntPtr aCollider, int aUserData);
        public void set3dCollider(SoloudObject aCollider, int aUserData = 0)
        {
            Vic_set3dColliderEx(objhandle, aCollider.objhandle, aUserData);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_set3dAttenuator(IntPtr aObjHandle, IntPtr aAttenuator);
        public void set3dAttenuator(SoloudObject aAttenuator)
        {
            Vic_set3dAttenuator(objhandle, aAttenuator.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_setInaudibleBehavior(IntPtr aObjHandle, int aMustTick, int aKill);
        public void setInaudibleBehavior(int aMustTick, int aKill)
        {
            Vic_setInaudibleBehavior(objhandle, aMustTick, aKill);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_setLoopPoint(IntPtr aObjHandle, double aLoopPoint);
        public void setLoopPoint(double aLoopPoint)
        {
            Vic_setLoopPoint(objhandle, aLoopPoint);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double Vic_getLoopPoint(IntPtr aObjHandle);
        public double getLoopPoint()
        {
            return Vic_getLoopPoint(objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_setFilter(IntPtr aObjHandle, uint aFilterId, IntPtr aFilter);
        public void setFilter(uint aFilterId, SoloudObject aFilter)
        {
            Vic_setFilter(objhandle, aFilterId, aFilter.objhandle);
        }

        [DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Vic_stop(IntPtr aObjHandle);
        public void stop()
        {
            Vic_stop(objhandle);
        }
    }
}
