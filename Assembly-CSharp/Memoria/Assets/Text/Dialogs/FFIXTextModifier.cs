using System;
using UnityEngine;

namespace Memoria.Assets
{
    public class FFIXTextModifier
    {
        public BetterList<Color> colors = new BetterList<Color>();
        public Int32 sub;
        public Boolean bold;
        public Boolean italic;
        public Boolean underline;
        public Boolean strike;
        public Boolean ignoreColor;
        public Boolean highShadow;
        public Boolean center;
        public Boolean justified;
        public Boolean mirror;
        public Int32 ff9Signal;
        public Vector3 extraOffset;
        public Single? tabX;
        public Dialog.DialogImage insertImage;

        public void Reset()
        {
            colors.Clear();
            sub = 0;
            bold = false;
            italic = false;
            underline = false;
            strike = false;
            ignoreColor = false;
            highShadow = false;
            center = false;
            justified = false;
            mirror = false;
            ff9Signal = 0;
            extraOffset = Vector3.zero;
            tabX = null;
            insertImage = null;
        }

        public void ResetLine()
        {
            center = false;
            //justified = false; // TODO: check usage of {Justified} Memoria tag?
            extraOffset = Vector3.zero;
        }
    }
}
