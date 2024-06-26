using System;
using System.Collections;
using UnityEngine;

public static class Anim
{
	public static Int32 TimeToTick(Single time)
	{
		return (Int32)(time * (Single)Anim.VSYNC);
	}

	public static Single TickToTime(Int32 tick)
	{
		return (Single)tick / (Single)Anim.VSYNC;
	}

	public static IEnumerator Tick(Int32 i)
	{
		yield return new WaitForSeconds(1f * (Single)i / (Single)Anim.VSYNC);
		yield break;
	}

	public static IEnumerator Tick()
	{
		yield return new WaitForSeconds(1f / (Single)Anim.VSYNC);
		yield break;
	}

	public static IEnumerator Disable(GameObject o)
	{
		o.gameObject.SetActive(false);
		yield return 0;
		yield break;
	}

	public static IEnumerator Enable(GameObject o)
	{
		o.SetActive(true);
		yield return 0;
		yield break;
	}

	public static IEnumerator Sequence(params IEnumerator[] sequence)
	{
		for (Int32 i = 0; i < (Int32)sequence.Length; i++)
		{
			while (sequence[i].MoveNext())
			{
				yield return sequence[i].Current;
			}
		}
		yield break;
	}

	public static IEnumerator Delay(Single duration)
	{
		yield return new WaitForSeconds(duration);
		yield break;
	}

	public static IEnumerator MoveLerp(Transform origin, Vector3 target, Single duration, Boolean local = false)
	{
		if (local)
		{
			Vector3 init = origin.localPosition;
			for (Single i = 0f; i < duration; i += Time.deltaTime)
			{
				origin.localPosition = Vector3.Lerp(init, target, i / duration);
				yield return 0;
			}
			origin.localPosition = target;
		}
		else
		{
			Vector3 init2 = origin.position;
			for (Single j = 0f; j < duration; j += Time.deltaTime)
			{
				origin.position = Vector3.Lerp(init2, target, j / duration);
				yield return 0;
			}
			origin.position = target;
		}
		yield break;
	}

	public static IEnumerator ScaleLerp(Transform origin, Vector3 target, Single duration)
	{
		Vector3 init = origin.localScale;
		for (Single i = 0f; i < duration; i += Time.deltaTime)
		{
			origin.localScale = Vector3.Lerp(init, target, i / duration);
			yield return 0;
		}
		origin.localScale = target;
		yield break;
	}

	public static Int32 VSYNC = 90;
}
