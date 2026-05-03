using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteDisplay))]
public class Coin : MonoBehaviour
{
    private void Start()
    {
        this.display = base.GetComponent<SpriteDisplay>();
        base.GetComponent<SpriteRenderer>().enabled = false;
    }

    public IEnumerator Toss(Int32 side)
    {
        Int32 offx = 0;
        Int32 offy = 0;
        Single alpha = 0f;
        Int32 id = 0;
        base.GetComponent<SpriteRenderer>().enabled = true;
        base.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        SoundEffect.Play(QuadMistSoundID.MINI_SE_COIN);
        for (Int32 tick = 0; tick <= 40; tick++)
        {
            id = (tick & 7);
            this.display.ID = id;
            base.transform.localPosition = new Vector3((Single)(this.offsets[this.display.ID] + offx + 157) * 0.01f, (Single)(-(Single)(92 + offy)) * 0.01f, -5f);
            yield return base.StartCoroutine(Anim.Tick());
        }
        SoundEffect.Stop(QuadMistSoundID.MINI_SE_COIN);
        if (side == 0)
        {
            this.display.ID = this.PLAYER_COIN;
        }
        else
        {
            this.display.ID = this.ENEMY_COIN;
        }
        yield return base.StartCoroutine(Anim.Tick(30));
        for (Int32 tick2 = 0; tick2 <= 12; tick2++)
        {
            if (side == 0)
            {
                offy = -82 * tick2 / 12;
                offx = 124 * tick2 / 12;
            }
            else
            {
                offy = -66 * tick2 / 12;
                offx = -119 * tick2 / 12;
            }
            alpha = (Single)((12 - tick2) * 10) / 255f;
            base.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);
            base.transform.localPosition = new Vector3((Single)(this.offsets[this.display.ID] + offx + 157) * 0.01f, (Single)(-(Single)(92 + offy)) * 0.01f, -5f);
            yield return base.StartCoroutine(Anim.Tick());
        }
        base.GetComponent<SpriteRenderer>().enabled = false;
        yield break;
    }

    private SpriteDisplay display;

    private Int32 ENEMY_COIN = 3;

    private Int32 PLAYER_COIN = 7;

    private Int32[] offsets = new Int32[]
    {
        -20,
        -20,
        -20,
        -20,
        -20,
        -20,
        -20,
        -20
    };
}
