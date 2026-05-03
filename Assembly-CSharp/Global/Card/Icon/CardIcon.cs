using Memoria;
using Memoria.Data;
using System;

public static class CardIcon
{
    public static CardIcon.Attribute GetCardAttribute(Int32 id)
    {
        if (Configuration.TetraMaster.TripleTriad > 0)
        {
            TripleTriadCard baseCard = TripleTriad.TripleTriadCardStats[(TetraMasterCardId)id];
            switch (baseCard.icon)
            {
                case "MONSTER":
                    return CardIcon.Attribute.MONSTER;
                case "SUMMON":
                    return CardIcon.Attribute.SUMMON;
                case "WEAPON":
                    return CardIcon.Attribute.WEAPON;
                case "SHIP":
                    return CardIcon.Attribute.SHIP;
                case "ANIMAL":
                    return CardIcon.Attribute.ANIMAL;
                case "CASTLE":
                    return CardIcon.Attribute.CASTLE;
            }
            return CardIcon.Attribute.MYSTERY;
        }
        else
        {
            if (id <= CardIcon.MAXID_MONSTER)
                return CardIcon.Attribute.MONSTER;
            if (id <= CardIcon.MAXID_SUMMON)
                return CardIcon.Attribute.SUMMON;
            if (id <= CardIcon.MAXID_WEAPON)
                return CardIcon.Attribute.WEAPON;
            if (id <= CardIcon.MAXID_SHIP)
                return CardIcon.Attribute.SHIP;
            if (id <= CardIcon.MAXID_ANIMAL)
                return CardIcon.Attribute.ANIMAL;
            if (id <= CardIcon.MAXID_CASTLE)
                return CardIcon.Attribute.CASTLE;
            return CardIcon.Attribute.MYSTERY;
        }
    }

    public static Int32 MAXID_MONSTER = 55;
    public static Int32 MAXID_SUMMON = 69;
    public static Int32 MAXID_WEAPON = 79;
    public static Int32 MAXID_SHIP = 87;
    public static Int32 MAXID_ANIMAL = 92;
    public static Int32 MAXID_CASTLE = 94;
    public static Int32 MAXID_MYSTERY = 99;

    public static Int32 SINGLE = 1;
    public static Int32 MULTIPLE = 8;

    public static Int32 EMPTY_ATTRIBUTE;

    public enum Attribute
    {
        MONSTER,
        SUMMON,
        WEAPON,
        SHIP,
        ANIMAL,
        CASTLE,
        MYSTERY
    }
}
