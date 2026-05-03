using System;
using UnityEngine;

public class Combo : MonoBehaviour
{
    public Int32 Number
    {
        set
        {
            this.comboText.Text = value.ToString();
            this.shadowText.Text = value.ToString();
        }
    }

    public SpriteText comboText;

    public SpriteText shadowText;
}
