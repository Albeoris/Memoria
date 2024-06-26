using System;
using System.Collections.Generic;

public partial class EventEngine : PersistenSingleton<EventEngine>
{
    public const Int32 classObj = 0;
    public const Int32 classSeq = 1;
    public const Int32 classThread = 2;
    public const Int32 classQuad = 3;
    public const Int32 classActor = 4;
    private const Int32 classEncPoint = 5;
    public const Int32 cObjN = 32;
    public const Int32 cObjBufN = 1280;
    public const Int32 cMapVarN = 80;
    public const Int32 kWatchN = 16;
    public const Int32 cEventGlobalN = 2048;
    public const Int32 kChrScaleLog = 6;
    public const Int32 PCNUM = 9;
    public const Int32 cInitStackN = 16;
    public const Int32 modeField = 1;
    public const Int32 modeBattle = 2;
    public const Int32 modeWorld = 3;
    public const Int32 modeBattleResult = 4;
    public const Int32 eNone = 0;
    public const Int32 eWait = 1;
    public const Int32 eDelete = 2;
    public const Int32 eEncount = 3;
    public const Int32 eJump = 4;
    public const Int32 eWJump = 5;
    public const Int32 eStop = 6;
    public const Int32 eMinigame = 7;
    public const Int32 eGameover = 8;
    public const Byte flagShow = 1;
    public const Byte flagCollInhC = 2;
    public const Byte flagCollInhNC = 4;
    public const Byte flagTalkInh = 8;
    public const Byte flagLockFreeInh = 16;
    public const Byte flagHideInh = 32;
    public const Byte flagTurn = 128;
    public const Int32 cFrameShift = 4;
    public const Int32 cCollScale = 4;
    public const Int32 kWalkSpeed = 30;
    public const Int32 kRunSpeed = 60;
    public const Int32 collModePush = 2;
    public const Int32 collModeTalk = 4;
    public const Int32 tagInit = 0;
    public const Int32 tagDefault = 1;
    public const Int32 tagPush = 2;
    public const Int32 tagTalk = 3;
    public const Int32 tagRefresh = 4;
    public const Int32 tagTurn = 5;
    public const Int32 tagCounter = 6;
    public const Int32 tagReaction = 7;
    public const Int32 tagCard = 8;
    public const Int32 tagDying = 9;
    public const Int32 tagBattleEnd = 10;
    public const Int32 cListElmN = 8;
    public const Single kEncInterval = 960f;
    public const Int32 kTalkTimer = 30;
    public const Int32 kLockFreeTime = 25;
    public const Int32 kLockRecoverTime = 25;
    public const Int32 kCollTime = 2;
    private const Int32 ebFileHeaderSize = 128;
    private const Int32 FF9_MAINCHARACTER_COUNT = 9;
    private const Int32 kNoMotionTurnLeft = 32766;
    private const Int32 kNoMotionTurnRight = 32767;
    private const Int32 kTurnAngleLog = 10;
    private const Int32 kTurnAngle = 1024;
    private const Single kTurnAngleFloat = 90f;
    public static Byte stateNew;
    public static Byte stateRunning;
    public static Byte stateInit;
    public static Byte stateSuspend;
    public static Int32 cEventLevelN;
    public static Int32 actNeckT;
    public static Int32 actNeckM;
    public static Int32 actNeckTalk;
    public static Int32 actJump;
    public static Int32 actMove;
    public static Int32 actLockDir;
    public static Int32 actEye;
    public static Int32 actAim;
    public static Int32 actLook;
    public static Int32 actLookTalker;
    public static Int32 actLookedTalker;
    public static Int32 kInitialDist;
    public static Int32 afLower;
    public static Int32 afFreeze;
    public static Int32 afHold;
    public static Int32 afLoop;
    public static Int32 afPalindrome;
    public static Int32 afDir;
    public static Int32 afExec;
    public static Int32 cSeqOfs;
    public static Int32 kSItemOfs;
    public static Int32 kCItemOfs;
    private static Int32 kNeckRad;
    private static Int32 kNeckNear2;
    public static Int16 kNeckAngle;
    public static Int16 kNeckAngle0;
    private static Int32 _btlCmdPrmCmd;
    private static Int32 _btlCmdPrmSub;
    private static Int32 kLook;
    private static Int32 kDefaultHeight;
    public static Int32 resyncBGMSignal;
    public static Int32 sizeOfObj;
    public static Int32 sizeOfQuad;
    public static Int32 sizeOfActor;
    public static Int32[] testEventIDs;
    private static Byte[,] d;
    private static PosObj sLastTalker;
    private static Int32 sTalkTimer;
    public static HashSet<Int16> moogleFldMap;
    public static HashSet<Int16> moogleFldSpecialMap;

    static EventEngine()
    {
        EventEngine.stateNew = (Byte)0;
        EventEngine.stateRunning = (Byte)1;
        EventEngine.stateInit = (Byte)2;
        EventEngine.stateSuspend = (Byte)3;
        EventEngine.cEventLevelN = 8;
        EventEngine.actNeckT = 1;
        EventEngine.actNeckM = 2;
        EventEngine.actNeckTalk = 4;
        EventEngine.actJump = 8;
        EventEngine.actMove = 16;
        EventEngine.actLockDir = 32;
        EventEngine.actEye = 64;
        EventEngine.actAim = 128;
        EventEngine.actLook = 256;
        EventEngine.actLookTalker = 512;
        EventEngine.actLookedTalker = 1024;
        EventEngine.kInitialDist = Int32.MaxValue;
        EventEngine.afLower = 2;
        EventEngine.afFreeze = 4;
        EventEngine.afHold = 8;
        EventEngine.afLoop = 16;
        EventEngine.afPalindrome = 32;
        EventEngine.afDir = 64;
        EventEngine.afExec = 128;
        EventEngine.cSeqOfs = 64;
        EventEngine.kSItemOfs = 256;
        EventEngine.kCItemOfs = 512;
        EventEngine.kNeckRad = 500;
        EventEngine.kNeckNear2 = 1000;
        EventEngine.kNeckAngle = (Int16)640;
        EventEngine.kNeckAngle0 = (Int16)((Int32)EventEngine.kNeckAngle + 256);
        EventEngine.kLook = 100;
        EventEngine.kDefaultHeight = 400;
        EventEngine.resyncBGMSignal = 0;
        EventEngine.sizeOfObj = 20;
        EventEngine.sizeOfQuad = 56;
        EventEngine.sizeOfActor = 160;
        EventEngine.testEventIDs = new Int32[31] {602, 350, 107, 262, 655, 350, 50, 101, 107, 103, 100, 310, 251, 250, 62, 752, 300, 453, 100, 2930, 301, 310, 157, 764, 60, 115, 64, 350, 1906, 2952, 852};
        EventEngine.d = new Byte[4, 4]
        {
            {(Byte)96, (Byte)168, (Byte)224, Byte.MaxValue},
            {(Byte)64, (Byte)128, (Byte)192, Byte.MaxValue},
            {(Byte)90, (Byte)167, (Byte)244, Byte.MaxValue},
            {(Byte)115, (Byte)217, (Byte)244, Byte.MaxValue}
        };
        EventEngine.moogleFldMap = new HashSet<Int16>
        {
            115, // Alexandria/Steeple
            150, // A. Castle/Guardhouse
            206, // Prima Vista/Crash Site
            253, // Evil Forest/Spring
            300, // Ice Cavern/Entrance (moogle is not always there)
            306, // Ice Cavern/Cave
            351, // Dali/Inn
            407, // Dali/Storage Area
            554, // Lindblum/Inn
            602, // L. Castle/Dragon's Gate
            611, // L. Castle/Guest Room
            662, // Marsh/Thicket (moogle is not always there)
            706, // Gizamaluke/Cavern
            764, // Burmecia/Vault
            802, // S. Gate/Bohden Station
            810, // S. Gate/Rest Stop
            853, // S. Gate/Treno Arch
            904, // Treno/Knight's House
            950, // Gargan Roo/Entrance
            1008, // Cleyra/Tree Trunk
            1056, // Cleyra/Inn
            1102, // Cleyra/Sandpit (moogle is not always there)
            1106, // Cleyra/Inn
            1109, // Cleyra/Cathedral (moogle is not always there)
            1152, // Red Rose/Cabin
            1205, // A. Castle/Chapel (moogle is not always there)
            1213, // A. Castle/Guardhouse
            1252, // Pinnacle Rocks/Path
            1304, // Lindblum/Inn
            1352, // L. Castle/Dragon's Gate
            1418, // Fossil Roo/Cavern
            1421, // Fossil Roo/Mining Site (moogle must be unlocked)
            1458, // Mage Village/Water Mill
            1509, // Conde Petie/Item Shop
            1553, // Mountain Path/Roots
            1652, // Iifa Tree/Roots
            1663, // Iifa Tree/Tree Trunk
            1759, // Iifa Tree/Roots
            1803, // A. Castle/Guardhouse
            1865, // Alexandria/Steeple
            1904, // Treno/Knight's House
            2104, // Lindblum/Inn
            2152, // L. Castle/Dragon's Gate
            2161, // L. Castle/Guest Room
            2203, // Palace/Rack
            2220, // Palace/Library (moogle area must be unlocked)
            2250, // Oeilvert/Outside
            2259, // Oeilvert/Star Display
            2304, // Esto Gaza/Terrace
            2354, // Gulug/Room
            2360, // Gulug/Path
            2456, // Alexandria/Steeple
            2504, // I. Castle/Small Room
            2655, // Bran Bal/Storage (moogle must be unlocked)
            2706, // Pand./Hall (moogle is not always here)
            2714, // Pand./Maze
            2801, // Daguerreo/Right Hall
            2901, // Memoria/Entrance
            2905, // Memoria/The Past (hidden)
            2909, // Memoria/Ruins (hidden)
            2913, // Memoria/Portal
            2916, // Memoria/Time Warp (hidden)
            2919, // Memoria/Gate to Space (hidden)
            2925, // Crystal World
            3057 // Mage Village/Water Mill
        };
        EventEngine.moogleFldSpecialMap = new HashSet<Int16>
        {
            300, // Ice Cavern/Entrance (moogle is not always there)
            662, // Marsh/Thicket (moogle is not always there)
            1102, // Cleyra/Sandpit (moogle is not always there)
            1109, // Cleyra/Cathedral (moogle is not always there)
            1205, // A. Castle/Chapel (moogle is not always there)
            1421, // Fossil Roo/Mining Site (moogle must be unlocked)
            2220, // Palace/Library (moogle area must be unlocked)
            2655, // Bran Bal/Storage (moogle must be unlocked)
            2706  // Pand./Hall (moogle is not always here)
        };
    }

    public static Int32 FindArrayIDFromEventID(Int32 eventID)
    {
        for (Int32 index = 0; index < EventEngine.testEventIDs.Length; ++index)
        {
            if (eventID == EventEngine.testEventIDs[index])
                return index;
        }
        return -1;
    }

    public static Boolean IsMoogleField(Int16 fldId, Int32 scCounter, Int32 mapIndex)
	{
        if (!EventEngine.moogleFldMap.Contains(fldId))
            return false;
        if (EventEngine.moogleFldSpecialMap.Contains(fldId))
        {
            if (fldId == 300 && scCounter < 3900) // Ice Cavern/Entrance
                return false;
            if (fldId == 662 && scCounter < 11100) // Marsh/Thicket
                return false;
            if (fldId == 1102 && scCounter < 4900) // Cleyra/Sandpit
                return false;
            if (fldId == 1109 && scCounter < 4900) // Cleyra/Cathedral
                return false;
            if (fldId == 2706 && mapIndex == -1) // Pand./Hall
                return false;
        }
        return true;
    }
}