using System;
using UnityEngine;
using Memoria;

namespace FF9
{
	public class GeoAnim
	{
		public static UInt16 geoAnimGetNumFrames(BTL_DATA btl)
		{
			Int32 num = 0;
			Animation component = btl.gameObject.GetComponent<Animation>();
			String currentAnimationName = btl.currentAnimationName;
			if (component[currentAnimationName] != (TrackedReference)null)
				return (UInt16)GeoAnim.geoAnimGetNumFrames(btl.gameObject, currentAnimationName);
			return (UInt16)((num <= 0) ? 1 : num);
		}

		public static UInt16 geoAnimGetNumFrames(BTL_DATA btl, String animName)
		{
			return (UInt16)geoAnimGetNumFrames(btl.gameObject, animName);
		}

		public static Int32 geoAnimGetNumFrames(GameObject go, String animName)
		{
			if (go.GetComponent<Animation>().GetClip(animName) == (UnityEngine.Object)null)
				return -1;
			Single length = go.GetComponent<Animation>()[animName].clip.length;
			Single frameRate = go.GetComponent<Animation>()[animName].clip.frameRate;
			Single f = length * frameRate;
			Int32 num = Mathf.CeilToInt(f);
			Int32 num2 = Mathf.FloorToInt(f);
			Int32 num3 = Mathf.RoundToInt(f);
			Int32 num4 = num;
			if (num3 == num)
				num4 = num;
			else if (num3 == num2)
				num4 = num2;
			return (int)((float)(num4 + 1) * ((float)Configuration.Graphics.BattleFPS / frameRate));
		}

		// Sometimes, the 1st frame of an animation is the same as its last frame... sometimes it's not
		public static Int32 getAnimationLoopFrame(BTL_DATA btl)
		{
			return getAnimationLoopFrame(btl, btl.currentAnimationName);
		}

		public static Int32 getAnimationLoopFrame(BTL_DATA btl, String animName)
		{
			Int32 animMaxFrame = geoAnimGetNumFrames(btl, animName);
			// TODO
			//String geoName = btl.gameObject.name;
			//geoName = geoName.Trim();
			//if (btl.gameObject.name == "355(Clone)" && animName == "ANH_MON_B3_166_000") // Grand Dragon idling
			//	return animMaxFrame;
			return animMaxFrame - 1;
		}
	}
}
