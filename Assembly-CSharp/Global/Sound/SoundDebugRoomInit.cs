using System;
using Assets.Scripts.Common;
using UnityEngine;

public class SoundDebugRoomInit : MonoBehaviour
{
	private void Start()
	{
		SceneDirector.Replace("SoundDebugRoom", SceneTransition.FadeOutToBlack_FadeIn, true);
	}
}
