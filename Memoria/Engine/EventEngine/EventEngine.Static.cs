public partial class EventEngine : PersistenSingleton<EventEngine>
{
    public const int classObj = 0;
    public const int classSeq = 1;
    public const int classThread = 2;
    public const int classQuad = 3;
    public const int classActor = 4;
    private const int classEncPoint = 5;
    public const int cObjN = 32;
    public const int cObjBufN = 1280;
    public const int cMapVarN = 80;
    public const int kWatchN = 16;
    public const int cEventGlobalN = 2048;
    public const int kChrScaleLog = 6;
    public const int PCNUM = 9;
    public const int cInitStackN = 16;
    public const int modeField = 1;
    public const int modeBattle = 2;
    public const int modeWorld = 3;
    public const int modeBattleResult = 4;
    public const int eNone = 0;
    public const int eWait = 1;
    public const int eDelete = 2;
    public const int eEncount = 3;
    public const int eJump = 4;
    public const int eWJump = 5;
    public const int eStop = 6;
    public const int eMinigame = 7;
    public const int eGameover = 8;
    public const byte flagShow = 1;
    public const byte flagCollInhC = 2;
    public const byte flagCollInhNC = 4;
    public const byte flagTalkInh = 8;
    public const byte flagLockFreeInh = 16;
    public const byte flagHideInh = 32;
    public const byte flagTurn = 128;
    public const int cFrameShift = 4;
    public const int cCollScale = 4;
    public const int kWalkSpeed = 30;
    public const int kRunSpeed = 60;
    public const int collModePush = 2;
    public const int collModeTalk = 4;
    public const int tagInit = 0;
    public const int tagDefault = 1;
    public const int tagPush = 2;
    public const int tagTalk = 3;
    public const int tagRefresh = 4;
    public const int tagTurn = 5;
    public const int tagCounter = 6;
    public const int tagReaction = 7;
    public const int tagCard = 8;
    public const int tagDying = 9;
    public const int tagBattleEnd = 10;
    public const int cListElmN = 8;
    public const float kEncInterval = 960f;
    public const int kTalkTimer = 30;
    public const int kLockFreeTime = 25;
    public const int kLockRecoverTime = 25;
    public const int kCollTime = 2;
    private const int ebFileHeaderSize = 128;
    private const int FF9_MAINCHARACTER_COUNT = 9;
    private const int kNoMotionTurnLeft = 32766;
    private const int kNoMotionTurnRight = 32767;
    private const int kTurnAngleLog = 10;
    private const int kTurnAngle = 1024;
    private const float kTurnAngleFloat = 90f;
    public static byte stateNew;
    public static byte stateRunning;
    public static byte stateInit;
    public static byte stateSuspend;
    public static int cEventLevelN;
    public static int actNeckT;
    public static int actNeckM;
    public static int actNeckTalk;
    public static int actJump;
    public static int actMove;
    public static int actLockDir;
    public static int actEye;
    public static int actAim;
    public static int actLook;
    public static int actLookTalker;
    public static int actLookedTalker;
    public static int kInitialDist;
    public static int afLower;
    public static int afFreeze;
    public static int afHold;
    public static int afLoop;
    public static int afPalindrome;
    public static int afDir;
    public static int afExec;
    public static int cSeqOfs;
    public static int kSItemOfs;
    public static int kCItemOfs;
    private static int kNeckRad;
    private static int kNeckNear2;
    public static short kNeckAngle;
    public static short kNeckAngle0;
    private static int _btlCmdPrm;
    private static int kLook;
    private static int kDefaultHeight;
    public static int resyncBGMSignal;
    public static int sizeOfObj;
    public static int sizeOfQuad;
    public static int sizeOfActor;
    public static int[] testEventIDs;
    private static byte[,] d;
    private static PosObj sLastTalker;
    private static int sTalkTimer;

    static EventEngine()
    {
        EventEngine.stateNew = (byte)0;
        EventEngine.stateRunning = (byte)1;
        EventEngine.stateInit = (byte)2;
        EventEngine.stateSuspend = (byte)3;
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
        EventEngine.kInitialDist = int.MaxValue;
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
        EventEngine.kNeckAngle = (short)640;
        EventEngine.kNeckAngle0 = (short)((int)EventEngine.kNeckAngle + 256);
        EventEngine.kLook = 100;
        EventEngine.kDefaultHeight = 400;
        EventEngine.resyncBGMSignal = 0;
        EventEngine.sizeOfObj = 20;
        EventEngine.sizeOfQuad = 56;
        EventEngine.sizeOfActor = 160;
        EventEngine.testEventIDs = new int[31] {602, 350, 107, 262, 655, 350, 50, 101, 107, 103, 100, 310, 251, 250, 62, 752, 300, 453, 100, 2930, 301, 310, 157, 764, 60, 115, 64, 350, 1906, 2952, 852};
        EventEngine.d = new byte[4, 4]
        {
            {(byte)96, (byte)168, (byte)224, byte.MaxValue},
            {(byte)64, (byte)128, (byte)192, byte.MaxValue},
            {(byte)90, (byte)167, (byte)244, byte.MaxValue},
            {(byte)115, (byte)217, (byte)244, byte.MaxValue}
        };
    }

    public static int FindArrayIDFromEventID(int eventID)
    {
        for (int index = 0; index < EventEngine.testEventIDs.Length; ++index)
        {
            if (eventID == EventEngine.testEventIDs[index])
                return index;
        }
        return -1;
    }
}