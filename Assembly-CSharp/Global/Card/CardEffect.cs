using System;
using System.Collections;
using UnityEngine;

public class CardEffect : MonoBehaviour
{
    public Boolean Small
    {
        set
        {
            if (value)
            {
                base.transform.localScale = new Vector3(QuadMistCardUI.SIZESMALL_W / QuadMistCardUI.SIZE_W, QuadMistCardUI.SIZESMALL_H / QuadMistCardUI.SIZE_H, 1f);
            }
            else
            {
                base.transform.localScale = Vector3.one;
            }
            arrowWhiteOut.Small = value;
        }
    }

    public Single Black
    {
        get
        {
            return blackOut.GetComponent<SpriteRenderer>().color.a;
        }
        set
        {
            blackOut.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, value);
        }
    }

    public Single White
    {
        get
        {
            return whiteOut.GetComponent<SpriteRenderer>().color.a;
        }
        set
        {
            whiteOut.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, value);
        }
    }

    public IEnumerator Flash1Normal(params Action[] action)
    {
        Single time = 0f;
        Boolean dir = false;
        White = 0f;
        while (!dir || White > 0f)
        {
            White = Flash1BrightnessFormula(time, ref dir);
            if (dir && action != null)
            {
                Action[] array = action;
                for (Int32 i = 0; i < (Int32)array.Length; i++)
                {
                    Action item = array[i];
                    item();
                }
                action = null;
            }
            time += Time.deltaTime;
            yield return 0;
        }
        White = 0f;
        yield break;
    }

    public IEnumerator Flash2Combo(CardArrow.Type arrow, params Action[] action)
    {
        Single duration = 28f / (Single)Anim.VSYNC;
        Single thickness = 0.04f;
        Single width = QuadMistCardUI.SIZE_W;
        Single height = QuadMistCardUI.SIZE_H;
        Vector3 pos = Vector3.zero;
        Vector3 pos2 = Vector3.zero;
        Vector3 pos1f = Vector3.zero;
        Vector3 pos2f = Vector3.zero;
        Vector3 sca = Vector3.zero;
        Vector3 sca2 = Vector3.zero;
        Vector3 sca1f = Vector3.zero;
        Vector3 sca2f = Vector3.zero;
        Vector3 offset = CardArrow.ToOffset(arrow);
        offset.y *= -1f;
        offset *= -1f;
        pos = new Vector3(offset.x * width / 2f + width / 2f, offset.y * height / 2f - height / 2f, 0f);
        pos2 = new Vector3(offset.x * width / 2f + width / 2f, offset.y * height / 2f - height / 2f, 0f);
        pos1f = pos;
        pos2f = pos2;
        offset *= -1f;
        switch (arrow)
        {
            case CardArrow.Type.UP:
                pos += new Vector3(0f, thickness / 2f * offset.y, 0f);
                sca = new Vector3(width, thickness, 0f);
                pos1f += new Vector3(0f, (height - thickness / 2f) * offset.y, 0f);
                sca1f = sca;
                break;
            case CardArrow.Type.RIGHT_UP:
                pos += new Vector3(thickness / 2f * offset.x, thickness / 2f * offset.y, 0f);
                sca = new Vector3(thickness, thickness, 1f);
                pos2 += new Vector3(thickness / 2f * offset.x, thickness / 2f * offset.y, 0f);
                sca2 = new Vector3(thickness, thickness, 1f);
                pos1f += new Vector3((width - thickness / 2f) * offset.x, height / 2f * offset.y, 0f);
                sca1f = new Vector3(thickness, height, 0f);
                pos2f += new Vector3(width / 2f * offset.x, (height - thickness / 2f) * offset.y, 0f);
                sca2f = new Vector3(width, thickness, 0f);
                break;
            case CardArrow.Type.RIGHT:
                pos2 += new Vector3(thickness / 2f * offset.x, 0f, 0f);
                sca2 = new Vector3(thickness, height, 0f);
                pos2f += new Vector3((width - thickness / 2f) * offset.x, 0f, 0f);
                sca2f = sca2;
                break;
            case CardArrow.Type.RIGHT_DOWN:
                pos += new Vector3(thickness / 2f * offset.x, thickness / 2f * offset.y, 0f);
                sca = new Vector3(thickness, thickness, 1f);
                pos2 += new Vector3(thickness / 2f * offset.x, thickness / 2f * offset.y, 0f);
                sca2 = new Vector3(thickness, thickness, 1f);
                pos1f += new Vector3((width - thickness / 2f) * offset.x, height / 2f * offset.y, 0f);
                sca1f = new Vector3(thickness, height, 0f);
                pos2f += new Vector3(width / 2f * offset.x, (height - thickness / 2f) * offset.y, 0f);
                sca2f = new Vector3(width, thickness, 0f);
                break;
            case CardArrow.Type.DOWN:
                pos += new Vector3(0f, thickness / 2f * offset.y, 0f);
                sca = new Vector3(width, thickness, 0f);
                pos1f += new Vector3(0f, (height - thickness / 2f) * offset.y, 0f);
                sca1f = sca;
                break;
            case CardArrow.Type.LEFT_DOWN:
                pos += new Vector3(thickness / 2f * offset.x, thickness / 2f * offset.y, 0f);
                sca = new Vector3(thickness, thickness, 1f);
                pos2 += new Vector3(thickness / 2f * offset.x, thickness / 2f * offset.y, 0f);
                sca2 = new Vector3(thickness, thickness, 1f);
                pos1f += new Vector3((width - thickness / 2f) * offset.x, height / 2f * offset.y, 0f);
                sca1f = new Vector3(thickness, height, 0f);
                pos2f += new Vector3(width / 2f * offset.x, (height - thickness / 2f) * offset.y, 0f);
                sca2f = new Vector3(width, thickness, 0f);
                break;
            case CardArrow.Type.LEFT:
                pos2 += new Vector3(thickness / 2f * offset.x, 0f, 0f);
                sca2 = new Vector3(thickness, height, 0f);
                pos2f += new Vector3((width - thickness / 2f) * offset.x, 0f, 0f);
                sca2f = sca2;
                break;
            case CardArrow.Type.LEFT_UP:
                pos += new Vector3(thickness / 2f * offset.x, thickness / 2f * offset.y, 0f);
                sca = new Vector3(thickness, thickness, 1f);
                pos2 += new Vector3(thickness / 2f * offset.x, thickness / 2f * offset.y, 0f);
                sca2 = new Vector3(thickness, thickness, 1f);
                pos1f += new Vector3((width - thickness / 2f) * offset.x, height / 2f * offset.y, 0f);
                sca1f = new Vector3(thickness, height, 0f);
                pos2f += new Vector3(width / 2f * offset.x, (height - thickness / 2f) * offset.y, 0f);
                sca2f = new Vector3(width, thickness, 0f);
                break;
        }
        combo[0].gameObject.SetActive(true);
        combo[1].gameObject.SetActive(true);
        Single brightness = 0f;
        Boolean dir = false;
        combo[0].sharedMaterial.color = new Color(1f, 1f, 1f, 1f);
        combo[1].sharedMaterial.color = new Color(1f, 1f, 1f, 1f);
        for (Single time = 0f; time < duration; time += Time.deltaTime)
        {
            Flash1BrightnessFormula(time, ref dir);
            if (dir && action != null)
            {
                Action[] array = action;
                for (Int32 i = 0; i < (Int32)array.Length; i++)
                {
                    Action item = array[i];
                    item();
                }
                action = null;
            }
            brightness = Flash2BrightnessFormula(time);
            combo[0].transform.localPosition = Vector3.Lerp(pos, pos1f, time / duration);
            combo[0].transform.localScale = Vector3.Lerp(sca, sca1f, time / duration);
            combo[1].transform.localPosition = Vector3.Lerp(pos2, pos2f, time / duration);
            combo[1].transform.localScale = Vector3.Lerp(sca2, sca2f, time / duration);
            combo[0].sharedMaterial.color = new Color(1f, 1f, 1f, brightness);
            combo[1].sharedMaterial.color = new Color(1f, 1f, 1f, brightness);
            yield return 0;
        }
        combo[0].gameObject.SetActive(false);
        combo[1].gameObject.SetActive(false);
        yield break;
    }

    public IEnumerator Flash3Battle(params Action[] action)
    {
        Single time = 0f;
        Boolean dir = false;
        Single brightness = 0f;
        while (!dir || brightness > 0f)
        {
            Flash1BrightnessFormula(time, ref dir);
            if (dir && action != null)
            {
                Action[] array = action;
                for (Int32 i = 0; i < (Int32)array.Length; i++)
                {
                    Action item = array[i];
                    item();
                }
                action = null;
            }
            White = Flash3BrightnessFormula(time);
            time += Time.deltaTime;
            yield return 0;
        }
        White = 0f;
        yield break;
    }

    public IEnumerator FlashArrows(Int32 mask)
    {
        Boolean flip = false;
        arrowWhiteOut.Arrow = mask;
        for (Int32 i = 0; i <= 20; i++)
        {
            SetColorArrows((!flip) ? Color.black : Color.white);
            flip = !flip;
            yield return base.StartCoroutine(Anim.Tick());
        }
        arrowWhiteOut.Arrow = 0;
        yield break;
    }

    private Single Flash1BrightnessFormula(Single time, ref Boolean dir)
    {
        Single num = time * (Single)Anim.VSYNC;
        Single num2 = (!dir) ? (num * 30f / 255f) : ((255f - (num - 10f) * 15f) / 255f);
        if (num2 >= 1f)
        {
            num2 = 1f;
            dir = true;
        }
        if (num2 < 0f)
        {
            num2 = 0f;
        }
        return num2;
    }

    private Single Flash2BrightnessFormula(Single time)
    {
        Single num = time * (Single)Anim.VSYNC;
        Single num2 = (255f - num * 9f) / 255f;
        if (num2 < 0f)
        {
            num2 = 0f;
        }
        return num2;
    }

    private Single Flash3BrightnessFormula(Single time)
    {
        Int32 num = (Int32)(time * (Single)Anim.VSYNC);
        return ((num & 4) == 0) ? 0.4627451f : 0.5019608f;
    }

    private void SetColorArrows(Color c)
    {
        for (Int32 i = 0; i < (Int32)arrowWhiteOut.ui.Length; i++)
        {
            arrowWhiteOut.ui[i].GetComponent<SpriteRenderer>().color = c;
        }
    }

    public static Int32 FLASH_TICK_DURATION = 28;

    public SpriteDisplay whiteOut;

    public SpriteDisplay blackOut;

    public CardArrows arrowWhiteOut;

    public MeshRenderer[] combo = new MeshRenderer[2];
}
