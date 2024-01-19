using System;
using Memoria;
using Memoria.Data;

public static class CardElement
{
    public static CardElement.Type GetCardElement(Int32 id)
    {
        TripleTriadCard baseCard = TripleTriad.TripleTriadCardStats[(TetraMasterCardId)id];
        switch (baseCard.icon)
        {
            case "FIRE":
                return CardElement.Type.FIRE;
            case "ICE":
                return CardElement.Type.ICE;
            case "THUNDER":
                return CardElement.Type.THUNDER;
            case "WATER":
                return CardElement.Type.WATER;
            case "EARTH":
                return CardElement.Type.EARTH;
            case "WIND":
                return CardElement.Type.WIND;
            case "SHADOW":
                return CardElement.Type.SHADOW;
            case "HOLY":
                return CardElement.Type.HOLY;
        }
        return CardElement.Type.NONE;
    }


    public enum Type
    {
        FIRE,
        ICE,
        THUNDER,
        WATER,
        EARTH,
        WIND,
        SHADOW,
        HOLY,
        NONE
    }
}
