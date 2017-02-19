using System;
using UnityEngine;

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
			{
				return (UInt16)GeoAnim.animFrame(btl, currentAnimationName);
			}
			return (UInt16)((num <= 0) ? 1 : num);
		}

		public static UInt16 geoAnimGetNumFrames(BTL_DATA btl, String animName)
		{
			return (UInt16)GeoAnim.animFrame(btl, animName);
		}

		public static Int32 animFrame(BTL_DATA btl, String animName)
		{
			if (btl.gameObject.GetComponent<Animation>().GetClip(animName) == (UnityEngine.Object)null)
			{
				return -1;
			}
			Single length = btl.gameObject.GetComponent<Animation>()[animName].clip.length;
			Single frameRate = btl.gameObject.GetComponent<Animation>()[animName].clip.frameRate;
			Single f = length * frameRate;
			Int32 num = Mathf.CeilToInt(f);
			Int32 num2 = Mathf.FloorToInt(f);
			Int32 num3 = Mathf.RoundToInt(f);
			Int32 num4 = num;
			if (num3 == num)
			{
				num4 = num;
			}
			else if (num3 == num2)
			{
				num4 = num2;
			}
			return num4 + 1;
		}
	}
}
