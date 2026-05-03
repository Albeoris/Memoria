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
    public static Byte stateNew = 0;
    public static Byte stateRunning = 1;
    public static Byte stateInit = 2;
    public static Byte stateSuspend = 3;
    public static Int32 cEventLevelN = 8;
    public static Int32 actNeckT = 1;
    public static Int32 actNeckM = 2;
    public static Int32 actNeckTalk = 4;
    public static Int32 actJump = 8;
    public static Int32 actMove = 16;
    public static Int32 actLockDir = 32;
    public static Int32 actEye = 64;
    public static Int32 actAim = 128;
    public static Int32 actLook = 256;
    public static Int32 actLookTalker = 512;
    public static Int32 actLookedTalker = 1024;
    public static Int32 kInitialDist = Int32.MaxValue;
    public static Int32 afLower = 2;
    public static Int32 afFreeze = 4;
    public static Int32 afHold = 8;
    public static Int32 afLoop = 16;
    public static Int32 afPalindrome = 32;
    public static Int32 afDir = 64;
    public static Int32 afExec = 128;
    public static Int32 cSeqOfs = 64;
    public static Int32 kSItemOfs = 256;
    public static Int32 kCItemOfs = 512;
    private static Int32 kNeckRad = 500;
    private static Int32 kNeckNear2 = 1000;
    public static Int16 kNeckAngle = 640;
    public static Int16 kNeckAngle0 = (Int16)(EventEngine.kNeckAngle + 256);
    private static Int32 _btlCmdPrmCmd;
    private static Int32 _btlCmdPrmSub;
    private static Int32 kLook = 100;
    private static Int32 kDefaultHeight = 400;
    public static Int32 resyncBGMSignal = 0;
    public static Int32 sizeOfObj = 20;
    public static Int32 sizeOfQuad = 56;
    public static Int32 sizeOfActor = 160;
    public static Int32[] testEventIDs;
    private static Byte[,] d;
    private static PosObj sLastTalker;
    private static Int32 sTalkTimer;
    public static HashSet<Int16> moogleFldMap;
    public static HashSet<Int16> moogleFldSpecialMap;
    public static Single LastProcessTime = 0f;

    static EventEngine()
    {
        EventEngine.testEventIDs = new Int32[31] { 602, 350, 107, 262, 655, 350, 50, 101, 107, 103, 100, 310, 251, 250, 62, 752, 300, 453, 100, 2930, 301, 310, 157, 764, 60, 115, 64, 350, 1906, 2952, 852 };
        EventEngine.d = new Byte[4, 4]
        {
            {96, 168, 224, Byte.MaxValue},
            {64, 128, 192, Byte.MaxValue},
            {90, 167, 244, Byte.MaxValue},
            {115, 217, 244, Byte.MaxValue}
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
