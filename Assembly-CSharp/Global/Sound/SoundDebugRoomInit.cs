using Assets.Scripts.Common;
using System;
using UnityEngine;

public class SoundDebugRoomInit : MonoBehaviour
{
    private void Start()
    {
        SceneDirector.Replace("SoundDebugRoom", SceneTransition.FadeOutToBlack_FadeIn, true);
    }
}
