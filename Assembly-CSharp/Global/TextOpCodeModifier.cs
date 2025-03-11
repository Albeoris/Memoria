using Assets.Sources.Scripts.UI.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class TextOpCodeModifier
{
    public static String Modify(String source)
    {
        // [DBG] Check what can be removed / updated
        source = TextOpCodeModifier.ReplaceMogIconText(source); // [DBG] change into a gif
        source = TextOpCodeModifier.ReplaceChanbaraText(source);
        source = TextOpCodeModifier.ReplacePotionShopText(source);
        source = TextOpCodeModifier.ReplaceFossilRouteText(source);
        source = TextOpCodeModifier.ReplaceAuctionText(source);
        source = TextOpCodeModifier.ReplaceGyshalShopText(source);
        source = TextOpCodeModifier.ReplaceOreShopText(source);
        source = TextOpCodeModifier.ReplaceEikoCookingText(source);
        source = TextOpCodeModifier.ReplacePandoniumText(source);
        return source;
    }

    private static String ReplaceMogIconText(String source)
    {
        for (Int32 i = 0; i < TextOpCodeModifier.MogIconTargetOpCode.Length; i++)
            source = source.Replace(TextOpCodeModifier.MogIconTargetOpCode[i], TextOpCodeModifier.MogIconReplacedOpCode[i]);
        return source;
    }

    private static String ReplaceChanbaraText(String source)
    {
        if (FF9TextTool.FieldZoneId != 2)
            return source;
        Int32 searchIndex;
        Int32 replaceIndex;
        switch (FF9StateSystem.Settings.CurrentLanguage)
        {
            case "German":
                searchIndex = 0;
                replaceIndex = 3;
                break;
            case "Spanish":
                searchIndex = 2;
                replaceIndex = 2;
                break;
            case "French":
                searchIndex = 3;
                replaceIndex = 3;
                break;
            case "Italian":
                searchIndex = 4;
                replaceIndex = 0;
                break;
            case "Japanese":
                searchIndex = 5;
                replaceIndex = 1;
                break;
            default:
                searchIndex = 1;
                replaceIndex = 2;
                break;
        }
        source = source.Replace(TextOpCodeModifier.ChanbaraTargetOpCode[searchIndex], TextOpCodeModifier.ChanbaraReplacedOpCode[replaceIndex]);
        return source;
    }

    private static String ReplacePotionShopText(String source)
    {
        if (FF9TextTool.FieldZoneId != 23)
            return source;
        Int32 startIndex;
        Int32 endIndex;
        switch (FF9StateSystem.Settings.CurrentLanguage)
        {
            case "German":
                return source;
            case "Spanish":
                startIndex = 4;
                endIndex = 5;
                break;
            case "French":
                startIndex = 6;
                endIndex = 7;
                break;
            case "Italian":
                startIndex = 2;
                endIndex = 3;
                break;
            case "Japanese":
                startIndex = 8;
                endIndex = 10;
                break;
            default:
                startIndex = 0;
                endIndex = 1;
                break;
        }
        for (Int32 i = startIndex; i <= endIndex; i++)
            source = source.Replace(TextOpCodeModifier.PotionShopTargetOpCode[i], TextOpCodeModifier.PotionShopReplacedOpCode[i]);
        return source;
    }

    /// <summary>Correcting position of yellow text path in Fossil Roo switches 1,3,4</summary>
	private static String ReplaceFossilRouteText(String source)
    {
        if (FF9TextTool.FieldZoneId != 361)
            return source;
        Int16 RealScreenSize = Math.Max(FieldMap.PsxScreenWidth, (Int16)(FieldMap.PsxScreenHeightNative * Screen.width / Screen.height));
        if (source.Contains("[MPOS=224,12]")) // switch 1
        {
            source = source.Replace("[MPOS=224,12]", $"[MPOS={(Int16)(RealScreenSize - 90)},16]");
        }
        if (source.Contains("[MPOS=212,12]")) // switch 3/4
        {
            source = source.Replace("[STRT=84,6][NANI][MPOS=212,12][IMME]                     ", "[STRT=84,6][NANI][MPOS=212,12][IMME]                                          ");
            source = source.Replace("[MPOS=212,12]", $"[MPOS={(Int16)(RealScreenSize - 102)},16]");
        }
        // was:
        // "[MPOS=224,12]" -> "[MPOS=230,16]",
        // "[MPOS=212,12]" -> "[MPOS=218,16]"
        return source;
    }

    private static String ReplaceAuctionText(String source)
    {
        if (FF9TextTool.FieldZoneId != 70 && FF9TextTool.FieldZoneId != 741)
            return source;
        Int32 startIndex;
        Int32 endIndex;
        switch (FF9StateSystem.Settings.CurrentLanguage)
        {
            case "German":
                startIndex = 6;
                endIndex = 8;
                break;
            case "Spanish":
                startIndex = 9;
                endIndex = 11;
                break;
            case "French":
                startIndex = 12;
                endIndex = 14;
                break;
            case "Italian":
                startIndex = 3;
                endIndex = 5;
                break;
            case "Japanese":
                startIndex = 15;
                endIndex = 17;
                break;
            default:
                startIndex = 0;
                endIndex = 2;
                break;
        }
        for (Int32 i = startIndex; i <= endIndex; i++)
            source = source.Replace(TextOpCodeModifier.AuctionTargetOpCode[i], TextOpCodeModifier.AuctionReplacedOpCode[i]);
        return source;
    }

    private static String ReplaceGyshalShopText(String source)
    {
        if (FF9TextTool.FieldZoneId != 945)
            return source;
        Int32 startIndex;
        Int32 endIndex;
        switch (FF9StateSystem.Settings.CurrentLanguage)
        {
            case "German":
                startIndex = 4;
                endIndex = 5;
                break;
            case "Spanish":
                startIndex = 6;
                endIndex = 7;
                break;
            case "French":
                startIndex = 0;
                endIndex = -1;
                break;
            case "Italian":
                startIndex = 2;
                endIndex = 3;
                break;
            case "Japanese":
                startIndex = 8;
                endIndex = 9;
                break;
            default:
                startIndex = 0;
                endIndex = 1;
                break;
        }
        for (Int32 i = startIndex; i <= endIndex; i++)
            source = source.Replace(TextOpCodeModifier.GysahlGreenTargetOpCode[i], TextOpCodeModifier.GysahlGreenReplacedOpCode[i]);
        return source;
    }

    private static String ReplaceOreShopText(String source)
    {
        if (FF9TextTool.FieldZoneId != 166)
            return source;
        Int32 startIndex;
        Int32 endIndex;
        switch (FF9StateSystem.Settings.CurrentLanguage)
        {
            case "German":
                startIndex = 6;
                endIndex = 7;
                break;
            case "Spanish":
                startIndex = 4;
                endIndex = 5;
                break;
            case "French":
                startIndex = 2;
                endIndex = 3;
                break;
            case "Italian":
                return source;
            case "Japanese":
                startIndex = 0;
                endIndex = 1;
                break;
            default:
                return source;
        }
        for (Int32 i = startIndex; i <= endIndex; i++)
            source = source.Replace(TextOpCodeModifier.OreShopTargetOpCode[i], TextOpCodeModifier.OreShopReplacedOpCode[i]);
        return source;
    }

    private static String ReplaceEikoCookingText(String source)
    {
        if (FF9TextTool.FieldZoneId != 358)
            return source;
        Int32 startIndex;
        Int32 endIndex;
        switch (FF9StateSystem.Settings.CurrentLanguage)
        {
            case "German":
                startIndex = 8;
                endIndex = 9;
                break;
            case "Spanish":
                startIndex = 6;
                endIndex = 7;
                break;
            case "French":
                startIndex = 4;
                endIndex = 5;
                break;
            case "Italian":
                return source;
            case "Japanese":
                startIndex = 2;
                endIndex = 3;
                break;
            default:
                startIndex = 0;
                endIndex = 1;
                break;
        }
        for (Int32 i = startIndex; i <= endIndex; i++)
            source = source.Replace(TextOpCodeModifier.EikoCookingTargetOpCode[i], TextOpCodeModifier.EikoCookingReplacedOpCode[i]);
        return source;
    }

    private static String ReplacePandoniumText(String source)
    {
        if (FF9TextTool.FieldZoneId != 344)
            return source;
        if (FF9StateSystem.Settings.CurrentLanguage != "Japanese")
            return source;

        for (Int32 i = 0; i < TextOpCodeModifier.PandoniumTargetOpCode.Length; i++)
            source = source.Replace(TextOpCodeModifier.PandoniumTargetOpCode[i], TextOpCodeModifier.PandoniumReplacedOpcode[i]);
        return source;
    }

    public static readonly String[] ChanbaraTargetOpCode = new String[]
    {
        "[XTAB=93][YADD=6][DBTN=UP][MOBI=268][XTAB=137][DBTN=TRIANGLE][MOBI=272]\n[CENT=77][DBTN=LEFT][MOBI=267][FEED=6][DBTN=RIGHT][MOBI=269][FEED=13][DBTN=SQUARE][MOBI=271][FEED=6][DBTN=CIRCLE][MOBI=273]\n[XTAB=93][YSUB=6][DBTN=DOWN][MOBI=270][XTAB=137][DBTN=CROSS][MOBI=274]",
        "[XTAB=88][YADD=6][DBTN=UP][MOBI=268][XTAB=132][DBTN=TRIANGLE][MOBI=272]\n[CENT=77][DBTN=LEFT][MOBI=267][FEED=6][DBTN=RIGHT][MOBI=269][FEED=13][DBTN=SQUARE][MOBI=271][FEED=6][DBTN=CIRCLE][MOBI=273]\n[XTAB=88][YSUB=6][DBTN=DOWN][MOBI=270][XTAB=132][DBTN=CROSS][MOBI=274]",
        "[XTAB=83][YADD=6][DBTN=UP][MOBI=268][XTAB=128][DBTN=TRIANGLE][MOBI=272]\n[CENT=77][DBTN=LEFT][MOBI=267][FEED=6][DBTN=RIGHT][MOBI=269][FEED=13][DBTN=SQUARE][MOBI=271][FEED=6][DBTN=CIRCLE][MOBI=273]\n[XTAB=83][YSUB=6][DBTN=DOWN][MOBI=270][XTAB=128][DBTN=CROSS][MOBI=274]",
        "[XTAB=91][YADD=6][DBTN=UP][MOBI=268][XTAB=135][DBTN=TRIANGLE][MOBI=272]\n[CENT=77][DBTN=LEFT][MOBI=267][FEED=6][DBTN=RIGHT][MOBI=269][FEED=13][DBTN=SQUARE][MOBI=271][FEED=6][DBTN=CIRCLE][MOBI=273]\n[XTAB=91][YSUB=6][DBTN=DOWN][MOBI=270][XTAB=135][DBTN=CROSS][MOBI=274]",
        "[XTAB=63][YADD=6][DBTN=UP][MOBI=268][XTAB=107][DBTN=TRIANGLE][MOBI=272]\n[CENT=77][DBTN=LEFT][MOBI=267][FEED=6][DBTN=RIGHT][MOBI=269][FEED=13][DBTN=SQUARE][MOBI=271][FEED=6][DBTN=CIRCLE][MOBI=273]\n[XTAB=63][YSUB=6][DBTN=DOWN][MOBI=270][XTAB=107][DBTN=CROSS][MOBI=274]",
        "[XTAB=75][YADD=6][DBTN=UP][MOBI=268][XTAB=120][DBTN=TRIANGLE][MOBI=272]\n[CENT=77][DBTN=LEFT][MOBI=267][FEED=6][DBTN=RIGHT][MOBI=269][FEED=13][DBTN=SQUARE][MOBI=271][FEED=6][DBTN=CIRCLE][MOBI=273]\n[XTAB=75][YSUB=6][DBTN=DOWN][MOBI=270][XTAB=120][DBTN=CROSS][MOBI=274]"
    };

    public static readonly String[] ChanbaraReplacedOpCode = new String[]
{
        "[XTAB=52][YADD=1][DBTN=UP][MOBI=268][XTAB=102][DBTN=TRIANGLE][MOBI=272]\n[XTAB=39][DBTN=LEFT][MOBI=267][FEED=14][DBTN=RIGHT][MOBI=269][FEED=11][DBTN=SQUARE][MOBI=271][FEED=13][DBTN=CIRCLE][MOBI=273]\n[XTAB=52][YSUB=1][DBTN=DOWN][MOBI=270][XTAB=102][DBTN=CROSS][MOBI=274]",
        "[XTAB=39][YADD=1][DBTN=UP][MOBI=268][XTAB=88][DBTN=TRIANGLE][MOBI=272]\n[XTAB=26][DBTN=LEFT][MOBI=267][FEED=14][DBTN=RIGHT][MOBI=269][FEED=11][DBTN=SQUARE][MOBI=271][FEED=13][DBTN=CIRCLE][MOBI=273]\n[XTAB=39][YSUB=1][DBTN=DOWN][MOBI=270][XTAB=88][DBTN=CROSS][MOBI=274]",
        "[XTAB=72][YADD=1][DBTN=UP][MOBI=268][XTAB=121][DBTN=TRIANGLE][MOBI=272]\n[XTAB=59][DBTN=LEFT][MOBI=267][FEED=14][DBTN=RIGHT][MOBI=269][FEED=11][DBTN=SQUARE][MOBI=271][FEED=13][DBTN=CIRCLE][MOBI=273]\n[XTAB=72][YSUB=1][DBTN=DOWN][MOBI=270][XTAB=121][DBTN=CROSS][MOBI=274]",
        "[XTAB=77][YADD=1][DBTN=UP][MOBI=268][XTAB=126][DBTN=TRIANGLE][MOBI=272]\n[XTAB=64][DBTN=LEFT][MOBI=267][FEED=14][DBTN=RIGHT][MOBI=269][FEED=11][DBTN=SQUARE][MOBI=271][FEED=13][DBTN=CIRCLE][MOBI=273]\n[XTAB=77][YSUB=1][DBTN=DOWN][MOBI=270][XTAB=126][DBTN=CROSS][MOBI=274]"
};

    public static readonly String[] AuctionTargetOpCode = new String[]
    {
        "[STRT=0,1][MPOS=33,80]",
        "[STRT=0,1][MPOS=40,80]",
        "[STRT=0,1][MPOS=47,80]",
        "[STRT=0,1][MPOS=64,80]",
        "[STRT=0,1][MPOS=71,80]",
        "[STRT=0,1][MPOS=78,80]",
        "[STRT=0,1][MPOS=49,80]",
        "[STRT=0,1][MPOS=56,80]",
        "[STRT=0,1][MPOS=63,80]",
        "[STRT=0,1][MPOS=61,80]",
        "[STRT=0,1][MPOS=68,80]",
        "[STRT=0,1][MPOS=75,80]",
        "[STRT=0,1][MPOS=82,80]",
        "[STRT=0,1][MPOS=89,80]",
        "[STRT=0,1][MPOS=96,80]",
        "[STRT=0,1][MPOS=44,80]",
        "[STRT=0,1][MPOS=53,80]",
        "[STRT=0,1][MPOS=62,80]"
    };

    public static readonly String[] AuctionReplacedOpCode = new String[]
    {
        "[STRT=0,1][MPOS=31,80]",
        "[STRT=0,1][MPOS=38,80]",
        "[STRT=0,1][MPOS=45,80]",
        "[STRT=0,1][MPOS=57,80]",
        "[STRT=0,1][MPOS=64,80]",
        "[STRT=0,1][MPOS=71,80]",
        "[STRT=0,1][MPOS=45,80]",
        "[STRT=0,1][MPOS=52,80]",
        "[STRT=0,1][MPOS=59,80]",
        "[STRT=0,1][MPOS=54,80]",
        "[STRT=0,1][MPOS=61,80]",
        "[STRT=0,1][MPOS=68,80]",
        "[STRT=0,1][MPOS=73,80]",
        "[STRT=0,1][MPOS=80,80]",
        "[STRT=0,1][MPOS=87,80]",
        "[STRT=0,1][MPOS=38,80]",
        "[STRT=0,1][MPOS=45,80]",
        "[STRT=0,1][MPOS=52,80]"
    };

    public static readonly String ChocoboPrizeTargetOpCode = "[XTAB=128]";

    public static readonly String ChocoboPrizeReplacedOpCode = "[XTAB=121]";

    public static readonly String[] GysahlGreenTargetOpCode = new String[]
    {
        "[STRT=0,1][MPOS=96,44]",
        "[STRT=0,1][MPOS=103,44]",
        "[STRT=0,1][MPOS=87,58]",
        "[STRT=0,1][MPOS=94,58]",
        "[STRT=0,1][MPOS=69,58]",
        "[STRT=0,1][MPOS=76,58]",
        "[STRT=0,1][MPOS=102,58]",
        "[STRT=0,1][MPOS=109,58]",
        "[STRT=0,1][MPOS=108,46]",
        "[STRT=0,1][MPOS=117,46]"
    };

    public static readonly String[] GysahlGreenReplacedOpCode = new String[]
    {
        "[STRT=0,1][MPOS=93,44]",
        "[STRT=0,1][MPOS=100,44]",
        "[STRT=0,1][MPOS=83,58]",
        "[STRT=0,1][MPOS=90,58]",
        "[STRT=0,1][MPOS=66,58]",
        "[STRT=0,1][MPOS=73,58]",
        "[STRT=0,1][MPOS=95,58]",
        "[STRT=0,1][MPOS=102,58]",
        "[STRT=0,1][MPOS=96,44]",
        "[STRT=0,1][MPOS=103,44]"
    };

    public static readonly String[] PotionShopTargetOpCode = new String[]
    {
        "[STRT=0,1][MPOS=49,58]",
        "[STRT=0,1][MPOS=56,58]",
        "[STRT=0,1][MPOS=65,58]",
        "[STRT=0,1][MPOS=72,58]",
        "[STRT=0,1][MPOS=88,62]",
        "[STRT=0,1][MPOS=95,62]",
        "[STRT=0,1][MPOS=66,58]",
        "[STRT=0,1][MPOS=73,58]",
        "[STRT=0,1][MPOS=49,46]",
        "[STRT=0,1][MPOS=58,46]",
        "[STRT=66,1][MPOS=80,46]"
    };

    public static readonly String[] PotionShopReplacedOpCode = new String[]
    {
        "[STRT=0,1][MPOS=43,58]",
        "[STRT=0,1][MPOS=50,58]",
        "[STRT=0,1][MPOS=60,58]",
        "[STRT=0,1][MPOS=67,58]",
        "[STRT=0,1][MPOS=77,62]",
        "[STRT=0,1][MPOS=84,62]",
        "[STRT=0,1][MPOS=55,58]",
        "[STRT=0,1][MPOS=62,58]",
        "[STRT=0,1][MPOS=36,44]",
        "[STRT=0,1][MPOS=43,44]",
        "[STRT=66,1][MPOS=80,44]"
    };

    public static readonly String[] MogIconTargetOpCode = new String[]
    {
        "[YADD=24][ICON=29][XTAB=0][YSUB=8][ICON=28][YSUB=16][XTAB=0][ICON=27]",
        "[ICON=27][XTAB=0][YADD=16][ICON=28][YADD=8][XTAB=0][ICON=29][YSUB=8][FEED=4]",
        "[ICON=27][XTAB=0][YADD=16][ICON=28][YADD=8][XTAB=0][ICON=29]"
    };

    public static readonly String[] MogIconReplacedOpCode = new String[]
    {
        "[ICON=27]",
        "[YADD=14][ICON=27][FEED=18]",
        "[ICON=27]"
    };

    public static readonly String[] OreShopTargetOpCode = new String[]
    {
        "[MPOS=48,46]",
        "[MPOS=57,46]",
        "[MPOS=66,44]",
        "[MPOS=73,44]",
        "[MPOS=47,58]",
        "[MPOS=54,58]",
        "[MPOS=31,58]",
        "[MPOS=38,58]"
    };

    public static readonly String[] OreShopReplacedOpCode = new String[]
    {
        "[MPOS=36,44]",
        "[MPOS=43,44]",
        "[MPOS=55,44]",
        "[MPOS=62,44]",
        "[MPOS=43,58]",
        "[MPOS=50,58]",
        "[MPOS=30,58]",
        "[MPOS=37,58]"
    };

    public static readonly String[] EikoCookingTargetOpCode = new String[]
    {
        "[MPOS=108,58]",
        "[MPOS=115,58]",
        "[MPOS=108,46]",
        "[MPOS=117,46]",
        "[MPOS=116,34]",
        "[MPOS=123,34]",
        "[MPOS=80,34]",
        "[MPOS=87,34]",
        "[MPOS=98,48]",
        "[MPOS=105,48]"
    };

    public static readonly String[] EikoCookingReplacedOpCode = new String[]
    {
        "[MPOS=103,58]",
        "[MPOS=110,58]",
        "[MPOS=96,44]",
        "[MPOS=103,44]",
        "[MPOS=105,34]",
        "[MPOS=112,34]",
        "[MPOS=76,34]",
        "[MPOS=83,34]",
        "[MPOS=92,48]",
        "[MPOS=99,48]"
    };

    public static readonly String[] PandoniumTargetOpCode = new String[]
    {
        "[WDTH=0,80,64,0,-1,0,90,64,1,-1]",
        "[MPOS=20,68]"
    };

    public static readonly String[] PandoniumReplacedOpcode = new String[]
    {
        "[WDTH=0,80,64,0,-1,0,100,64,1,-1]",
        "[MPOS=20,62]"
    };
}
