﻿using Memoria;
using System;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public String Status
    {
        set
        {
            status.Text = value;
            if (Configuration.TetraMaster.TripleTriad > 0)
            {
                status.letters[0].transform.localPosition = new Vector3(0.05f, -0.05f, -0.02f);
                status.letters[1].transform.localPosition = new Vector3(0.05f, -0.19f, -0.02f);
                status.letters[2].transform.localPosition = new Vector3(0.02f, -0.12f, -0.02f);
                status.letters[3].transform.localPosition = new Vector3(0.08f, -0.12f, -0.02f);
            }
        }
    }

    public Int32 ID
    {
        get
        {
            return character.ID;
        }
        set
        {
            if (value < 100)
            {
                character.ID = value;
                block.gameObject.SetActive(false);
            }
            else
            {
                block.ID = value - 100;
                block.gameObject.SetActive(true);
            }
        }
    }

    public Int32 Side
    {
        get
        {
            if (background != null)
            {
                return background.ID;
            }
            return 0;
        }
        set
        {
            value = Mathf.Max(0, Mathf.Min(value, 1));
            if (background != null)
            {
                background.ID = value;
            }
            if (frame != null)
            {
                frame.ID = value + 5;
            }
        }
    }

    public Boolean Flip
    {
        get
        {
            return flip.gameObject.activeSelf;
        }
        set
        {
            flip.gameObject.SetActive(value);
        }
    }

    public Boolean Small
    {
        set
        {
            if (value)
            {
                base.transform.localScale = new Vector3(QuadMistCardUI.SIZESMALL_W / QuadMistCardUI.SIZE_W, QuadMistCardUI.SIZESMALL_H / QuadMistCardUI.SIZE_H, 1f);
                status.transform.localPosition = CardDisplay.STATUS_SMALL;
            }
            else
            {
                base.transform.localScale = Vector3.one;
                status.transform.localPosition = CardDisplay.STATUS;
            }
        }
    }

    public Boolean Select
    {
        get
        {
            return select.gameObject.activeSelf;
        }
        set
        {
            select.gameObject.SetActive(value);
        }
    }

    public Boolean IsBlock
    {
        get
        {
            return block.gameObject.activeSelf;
        }
    }

    public Boolean Contains(Vector3 worldPoint)
    {
        return background.GetComponent<SpriteClickable>().Contains(worldPoint);
    }

    public static Vector3 STATUS = new Vector3(0f, 0.02f, -0.02f);

    public static Vector3 STATUS_SMALL = new Vector3(-0.05f, 0.13f, -0.02f);

    public SpriteText status;

    public SpriteDisplay select;

    public SpriteDisplay character;

    public SpriteDisplay flip;

    public SpriteDisplay frame;

    public SpriteDisplay background;

    public SpriteDisplay block;
}
