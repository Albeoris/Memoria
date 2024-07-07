using System;
using UnityEngine;

public class SoundState : MonoBehaviour
{
    public void Start()
    {
        this.NewGame();
    }

    public void NewGame()
    {
        this.auto_save_bgm_id = -1;
    }

    public Int32 auto_save_bgm_id;
}
