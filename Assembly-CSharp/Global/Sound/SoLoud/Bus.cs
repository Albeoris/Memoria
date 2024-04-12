using SoLoud;
using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SoLoud
{
	public class Bus : SoloudObject
	{
		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr Bus_create();
		public Bus()
		{
			objhandle = Bus_create();
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr Bus_destroy(IntPtr aObjHandle);
		~Bus()
		{
			Bus_destroy(objhandle);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_setFilter(IntPtr aObjHandle, uint aFilterId, IntPtr aFilter);
		public void setFilter(uint aFilterId, SoloudObject aFilter)
		{
			Bus_setFilter(objhandle, aFilterId, aFilter.objhandle);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint Bus_playEx(IntPtr aObjHandle, IntPtr aSound, float aVolume, float aPan, int aPaused);
		public uint play(SoloudObject aSound, float aVolume = 1.0f, float aPan = 0.0f, int aPaused = 0)
		{
			return Bus_playEx(objhandle, aSound.objhandle, aVolume, aPan, aPaused);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint Bus_playClockedEx(IntPtr aObjHandle, double aSoundTime, IntPtr aSound, float aVolume, float aPan);
		public uint playClocked(double aSoundTime, SoloudObject aSound, float aVolume = 1.0f, float aPan = 0.0f)
		{
			return Bus_playClockedEx(objhandle, aSoundTime, aSound.objhandle, aVolume, aPan);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint Bus_play3dEx(IntPtr aObjHandle, IntPtr aSound, float aPosX, float aPosY, float aPosZ, float aVelX, float aVelY, float aVelZ, float aVolume, int aPaused);
		public uint play3d(SoloudObject aSound, float aPosX, float aPosY, float aPosZ, float aVelX = 0.0f, float aVelY = 0.0f, float aVelZ = 0.0f, float aVolume = 1.0f, int aPaused = 0)
		{
			return Bus_play3dEx(objhandle, aSound.objhandle, aPosX, aPosY, aPosZ, aVelX, aVelY, aVelZ, aVolume, aPaused);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint Bus_play3dClockedEx(IntPtr aObjHandle, double aSoundTime, IntPtr aSound, float aPosX, float aPosY, float aPosZ, float aVelX, float aVelY, float aVelZ, float aVolume);
		public uint play3dClocked(double aSoundTime, SoloudObject aSound, float aPosX, float aPosY, float aPosZ, float aVelX = 0.0f, float aVelY = 0.0f, float aVelZ = 0.0f, float aVolume = 1.0f)
		{
			return Bus_play3dClockedEx(objhandle, aSoundTime, aSound.objhandle, aPosX, aPosY, aPosZ, aVelX, aVelY, aVelZ, aVolume);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int Bus_setChannels(IntPtr aObjHandle, uint aChannels);
		public int setChannels(uint aChannels)
		{
			return Bus_setChannels(objhandle, aChannels);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_setVisualizationEnable(IntPtr aObjHandle, int aEnable);
		public void setVisualizationEnable(int aEnable)
		{
			Bus_setVisualizationEnable(objhandle, aEnable);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_annexSound(IntPtr aObjHandle, uint aVoiceHandle);
		public void annexSound(uint aVoiceHandle)
		{
			Bus_annexSound(objhandle, aVoiceHandle);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr Bus_calcFFT(IntPtr aObjHandle);
		public float[] calcFFT()
		{
			float[] ret = new float[256];
			IntPtr p = Bus_calcFFT(objhandle);

			byte[] buffer = new byte[4];
			for (int i = 0; i < ret.Length; ++i)
			{
				int f_bits = Marshal.ReadInt32(p, i * 4);
				buffer[0] = (byte)((f_bits >> 0) & 0xff);
				buffer[1] = (byte)((f_bits >> 8) & 0xff);
				buffer[2] = (byte)((f_bits >> 16) & 0xff);
				buffer[3] = (byte)((f_bits >> 24) & 0xff);
				ret[i] = BitConverter.ToSingle(buffer, 0);
			}
			return ret;
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr Bus_getWave(IntPtr aObjHandle);
		public float[] getWave()
		{
			float[] ret = new float[256];
			IntPtr p = Bus_getWave(objhandle);

			byte[] buffer = new byte[4];
			for (int i = 0; i < ret.Length; ++i)
			{
				int f_bits = Marshal.ReadInt32(p, i * 4);
				buffer[0] = (byte)((f_bits >> 0) & 0xff);
				buffer[1] = (byte)((f_bits >> 8) & 0xff);
				buffer[2] = (byte)((f_bits >> 16) & 0xff);
				buffer[3] = (byte)((f_bits >> 24) & 0xff);
				ret[i] = BitConverter.ToSingle(buffer, 0);
			}
			return ret;
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern float Bus_getApproximateVolume(IntPtr aObjHandle, uint aChannel);
		public float getApproximateVolume(uint aChannel)
		{
			return Bus_getApproximateVolume(objhandle, aChannel);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint Bus_getActiveVoiceCount(IntPtr aObjHandle);
		public uint getActiveVoiceCount()
		{
			return Bus_getActiveVoiceCount(objhandle);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_setVolume(IntPtr aObjHandle, float aVolume);
		public void setVolume(float aVolume)
		{
			Bus_setVolume(objhandle, aVolume);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_setLooping(IntPtr aObjHandle, int aLoop);
		public void setLooping(int aLoop)
		{
			Bus_setLooping(objhandle, aLoop);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_set3dMinMaxDistance(IntPtr aObjHandle, float aMinDistance, float aMaxDistance);
		public void set3dMinMaxDistance(float aMinDistance, float aMaxDistance)
		{
			Bus_set3dMinMaxDistance(objhandle, aMinDistance, aMaxDistance);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_set3dAttenuation(IntPtr aObjHandle, uint aAttenuationModel, float aAttenuationRolloffFactor);
		public void set3dAttenuation(uint aAttenuationModel, float aAttenuationRolloffFactor)
		{
			Bus_set3dAttenuation(objhandle, aAttenuationModel, aAttenuationRolloffFactor);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_set3dDopplerFactor(IntPtr aObjHandle, float aDopplerFactor);
		public void set3dDopplerFactor(float aDopplerFactor)
		{
			Bus_set3dDopplerFactor(objhandle, aDopplerFactor);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_set3dListenerRelative(IntPtr aObjHandle, int aListenerRelative);
		public void set3dListenerRelative(int aListenerRelative)
		{
			Bus_set3dListenerRelative(objhandle, aListenerRelative);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_set3dDistanceDelay(IntPtr aObjHandle, int aDistanceDelay);
		public void set3dDistanceDelay(int aDistanceDelay)
		{
			Bus_set3dDistanceDelay(objhandle, aDistanceDelay);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_set3dColliderEx(IntPtr aObjHandle, IntPtr aCollider, int aUserData);
		public void set3dCollider(SoloudObject aCollider, int aUserData = 0)
		{
			Bus_set3dColliderEx(objhandle, aCollider.objhandle, aUserData);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_set3dAttenuator(IntPtr aObjHandle, IntPtr aAttenuator);
		public void set3dAttenuator(SoloudObject aAttenuator)
		{
			Bus_set3dAttenuator(objhandle, aAttenuator.objhandle);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_setInaudibleBehavior(IntPtr aObjHandle, int aMustTick, int aKill);
		public void setInaudibleBehavior(int aMustTick, int aKill)
		{
			Bus_setInaudibleBehavior(objhandle, aMustTick, aKill);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_setLoopPoint(IntPtr aObjHandle, double aLoopPoint);
		public void setLoopPoint(double aLoopPoint)
		{
			Bus_setLoopPoint(objhandle, aLoopPoint);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern double Bus_getLoopPoint(IntPtr aObjHandle);
		public double getLoopPoint()
		{
			return Bus_getLoopPoint(objhandle);
		}

		[DllImport("soloud", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Bus_stop(IntPtr aObjHandle);
		public void stop()
		{
			Bus_stop(objhandle);
		}
	}
}
