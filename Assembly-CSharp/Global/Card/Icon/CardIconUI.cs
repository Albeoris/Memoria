using System;
using UnityEngine;

public class CardIconUI : MonoBehaviour
{
    public Int32 ID
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
            UpdateIcon();
        }
    }

    public Int32 Count
    {
        get
        {
            return count;
        }
        set
        {
            count = value;
            if (count > 0)
            {
                countText.Text = count.ToString();
            }
            else
            {
                countText.Text = String.Empty;
            }
            UpdateIcon();
        }
    }

    public void UpdateIcon()
    {
        if (count == 0)
        {
            iconSprite.ID = CardIcon.EMPTY_ATTRIBUTE;
        }
        else if (count == 1)
        {
            iconSprite.ID = (Int32)(CardIcon.GetCardAttribute(id) + CardIcon.SINGLE);
        }
        else
        {
            iconSprite.ID = (Int32)(CardIcon.GetCardAttribute(id) + CardIcon.MULTIPLE);
        }
    }

    public Boolean Contains(Vector3 worldPoint)
    {
        return iconSprite.GetComponent<SpriteClickable>().Contains(worldPoint);
    }

    public static Single SIZE_H = 0.15f;

    public static Single SIZE_W = 0.15f;

    public SpriteDisplay iconSprite;

    public SpriteText countText;

    private Int32 id;

    private Int32 count;
}
