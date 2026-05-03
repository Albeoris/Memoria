using System;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;

public class TextOpCodeModifier
{
    public static String Modify(String source, Int32 textId)
    {
        source = TextOpCodeModifier.ReplaceMogIconText(source); // Mog icon of Mognet letters
        source = TextOpCodeModifier.ReplaceChanbaraText(source); // Blank sword minigame
        source = TextOpCodeModifier.ReplaceFossilRouteText(source); // Fossil Roo yellow paths switches 1,3,4
        source = TextOpCodeModifier.ReplacePandoniumText(source); // Pandemonium elevator pick, japanese only
        return source;
    }

    private static String ReplaceMogIconText(String source)
    {
        for (Int32 i = 0; i < TextOpCodeModifier.MogIconTargetOpCode.Length; i++)
            source = source.Replace(TextOpCodeModifier.MogIconTargetOpCode[i], TextOpCodeModifier.MogIconReplacedOpCode[i]);
        return source;
    }

    private static readonly String[] MogIconTargetOpCode = new String[]
    {
        "[YADD=24][ICON=29][XTAB=0][YSUB=8][ICON=28][YSUB=16][XTAB=0][ICON=27]",
        "[ICON=27][XTAB=0][YADD=16][ICON=28][YADD=8][XTAB=0][ICON=29][YSUB=8][FEED=4]",
        "[ICON=27][XTAB=0][YADD=16][ICON=28][YADD=8][XTAB=0][ICON=29]"
    };

    private static readonly String[] MogIconReplacedOpCode = new String[]
    {
        "[ANIM=ICON,Constant,0.05,Constant,0.1,29,28,27]",
        "[ANIM=ICON,Constant,0.05,Constant,0.1,29,28,27][YADD=14][FEED=18]",
        "[ANIM=ICON,Constant,0.05,Constant,0.1,29,28,27]"
    };

    private static String ReplaceChanbaraText(String source)
    {
        if (FF9TextTool.FieldZoneId != 2)
            return source;
        Int32 searchIndex;
        Int32 replaceIndex;
        switch (Localization.CurrentDisplaySymbol)
        {
            case "GR":
                searchIndex = 0;
                replaceIndex = 3;
                break;
            case "ES":
                searchIndex = 2;
                replaceIndex = 2;
                break;
            case "FR":
                searchIndex = 3;
                replaceIndex = 3;
                break;
            case "IT":
                searchIndex = 4;
                replaceIndex = 0;
                break;
            case "JP":
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

    private static readonly String[] ChanbaraTargetOpCode = new String[]
    {
        "[XTAB=93][YADD=6][DBTN=UP][MOBI=268][XTAB=137][DBTN=TRIANGLE][MOBI=272]\n[CENT=77][DBTN=LEFT][MOBI=267][FEED=6][DBTN=RIGHT][MOBI=269][FEED=13][DBTN=SQUARE][MOBI=271][FEED=6][DBTN=CIRCLE][MOBI=273]\n[XTAB=93][YSUB=6][DBTN=DOWN][MOBI=270][XTAB=137][DBTN=CROSS][MOBI=274]",
        "[XTAB=88][YADD=6][DBTN=UP][MOBI=268][XTAB=132][DBTN=TRIANGLE][MOBI=272]\n[CENT=77][DBTN=LEFT][MOBI=267][FEED=6][DBTN=RIGHT][MOBI=269][FEED=13][DBTN=SQUARE][MOBI=271][FEED=6][DBTN=CIRCLE][MOBI=273]\n[XTAB=88][YSUB=6][DBTN=DOWN][MOBI=270][XTAB=132][DBTN=CROSS][MOBI=274]",
        "[XTAB=83][YADD=6][DBTN=UP][MOBI=268][XTAB=128][DBTN=TRIANGLE][MOBI=272]\n[CENT=77][DBTN=LEFT][MOBI=267][FEED=6][DBTN=RIGHT][MOBI=269][FEED=13][DBTN=SQUARE][MOBI=271][FEED=6][DBTN=CIRCLE][MOBI=273]\n[XTAB=83][YSUB=6][DBTN=DOWN][MOBI=270][XTAB=128][DBTN=CROSS][MOBI=274]",
        "[XTAB=91][YADD=6][DBTN=UP][MOBI=268][XTAB=135][DBTN=TRIANGLE][MOBI=272]\n[CENT=77][DBTN=LEFT][MOBI=267][FEED=6][DBTN=RIGHT][MOBI=269][FEED=13][DBTN=SQUARE][MOBI=271][FEED=6][DBTN=CIRCLE][MOBI=273]\n[XTAB=91][YSUB=6][DBTN=DOWN][MOBI=270][XTAB=135][DBTN=CROSS][MOBI=274]",
        "[XTAB=63][YADD=6][DBTN=UP][MOBI=268][XTAB=107][DBTN=TRIANGLE][MOBI=272]\n[CENT=77][DBTN=LEFT][MOBI=267][FEED=6][DBTN=RIGHT][MOBI=269][FEED=13][DBTN=SQUARE][MOBI=271][FEED=6][DBTN=CIRCLE][MOBI=273]\n[XTAB=63][YSUB=6][DBTN=DOWN][MOBI=270][XTAB=107][DBTN=CROSS][MOBI=274]",
        "[XTAB=75][YADD=6][DBTN=UP][MOBI=268][XTAB=120][DBTN=TRIANGLE][MOBI=272]\n[CENT=77][DBTN=LEFT][MOBI=267][FEED=6][DBTN=RIGHT][MOBI=269][FEED=13][DBTN=SQUARE][MOBI=271][FEED=6][DBTN=CIRCLE][MOBI=273]\n[XTAB=75][YSUB=6][DBTN=DOWN][MOBI=270][XTAB=120][DBTN=CROSS][MOBI=274]"
    };

    private static readonly String[] ChanbaraReplacedOpCode = new String[]
    {
        "[XTAB=52][YADD=1][DBTN=UP][MOBI=268][XTAB=102][DBTN=TRIANGLE][MOBI=272]\n[XTAB=39][DBTN=LEFT][MOBI=267][FEED=13][DBTN=RIGHT][MOBI=269][XTAB=89][DBTN=SQUARE][MOBI=271][FEED=13][DBTN=CIRCLE][MOBI=273]\n[XTAB=52][YSUB=1][DBTN=DOWN][MOBI=270][XTAB=102][DBTN=CROSS][MOBI=274]",
        "[XTAB=39][YADD=1][DBTN=UP][MOBI=268][XTAB=88][DBTN=TRIANGLE][MOBI=272]\n[XTAB=26][DBTN=LEFT][MOBI=267][FEED=13][DBTN=RIGHT][MOBI=269][XTAB=75][DBTN=SQUARE][MOBI=271][FEED=13][DBTN=CIRCLE][MOBI=273]\n[XTAB=39][YSUB=1][DBTN=DOWN][MOBI=270][XTAB=88][DBTN=CROSS][MOBI=274]",
        "[XTAB=72][YADD=1][DBTN=UP][MOBI=268][XTAB=121][DBTN=TRIANGLE][MOBI=272]\n[XTAB=59][DBTN=LEFT][MOBI=267][FEED=13][DBTN=RIGHT][MOBI=269][XTAB=108][DBTN=SQUARE][MOBI=271][FEED=13][DBTN=CIRCLE][MOBI=273]\n[XTAB=72][YSUB=1][DBTN=DOWN][MOBI=270][XTAB=121][DBTN=CROSS][MOBI=274]",
        "[XTAB=77][YADD=1][DBTN=UP][MOBI=268][XTAB=126][DBTN=TRIANGLE][MOBI=272]\n[XTAB=64][DBTN=LEFT][MOBI=267][FEED=13][DBTN=RIGHT][MOBI=269][XTAB=113][DBTN=SQUARE][MOBI=271][FEED=13][DBTN=CIRCLE][MOBI=273]\n[XTAB=77][YSUB=1][DBTN=DOWN][MOBI=270][XTAB=126][DBTN=CROSS][MOBI=274]"
    };

    /// <summary>Correcting position of yellow text path in Fossil Roo switches 1,3,4</summary>
	private static String ReplaceFossilRouteText(String source)
    {
        if (FF9TextTool.FieldZoneId != 361)
            return source;
        // Notes:
        // - Full path of Switch 1 is [TAIL=UPR] and thus displayed at top-right of the screen
        // - Full path and yellow paths of Switch 2 are [TAIL=UPL] and thus displayed at top-left of the screen
        // - Full path of Switch 3 is [TAIL=UPR]
        // - All the others rely on [MPOS] by default: these are the ones that must be fixed
        source = source.Replace("[MPOS=224,12]", "[MPOS=12,0,2]"); // Switch 1 (yellow path)
        if (source.Contains("[MPOS=212,12]"))
        {
            if (source.Contains("[909090]")) // Switch 4 (full path)
                source = source.Replace("[MPOS=212,12]", "[TAIL=UPR]");
            else // Switch 3 and 4 (yellow path)
                source = source.Replace("[MPOS=212,12]", "[MPOS=0,0,2]");
        }
        return source;
    }

    private static String ReplacePandoniumText(String source)
    {
        if (FF9TextTool.FieldZoneId != 344)
            return source;
        if (Localization.CurrentDisplaySymbol != "JP")
            return source;
        // This is the position of the modifiable number in the text for Pandemonium's elevator: it is adjusted for the usual font heights but it is not a robust adjustment
        return source.Replace("[MPOS=20,68]", "[MPOS=20,62]");
    }
}
