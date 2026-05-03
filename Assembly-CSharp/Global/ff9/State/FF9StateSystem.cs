using Assets.SiliconSocial;
using System;
using UnityEngine;

public class FF9StateSystem : PersistenSingleton<FF9StateSystem>
{
    public static Boolean AndroidTVPlatform => FF9StateSystem.IsAndroidTV;
    public static Boolean AndroidAmazonPlatform => false;
    public static SettingsState Settings => PersistenSingleton<FF9StateSystem>.Instance.settings;
    public static CommonState Common => PersistenSingleton<FF9StateSystem>.Instance.common;
    public static FieldState Field => PersistenSingleton<FF9StateSystem>.Instance.field;
    public static WorldState World => PersistenSingleton<FF9StateSystem>.Instance.world;
    public static BattleStateSystem Battle => PersistenSingleton<FF9StateSystem>.Instance.battle;
    public static MiniGameState MiniGame => PersistenSingleton<FF9StateSystem>.Instance.miniGame;
    public static EventState EventState => PersistenSingleton<FF9StateSystem>.Instance.eventState;
    public static SoundState Sound => PersistenSingleton<FF9StateSystem>.Instance.sound;
    public static AchievementState Achievement => PersistenSingleton<FF9StateSystem>.Instance.achievement;
    public static ISharedDataSerializer Serializer => PersistenSingleton<FF9StateSystem>.Instance.serializer;
    public static QuadMistDatabase QuadMistDatabase => PersistenSingleton<FF9StateSystem>.Instance.quadMistDatabase;

    protected override void Awake()
    {
        base.Awake();
        this.InitStateSystem();
        this.InitStateSerializer();
        this.InitQuadMistDatabase();
        PSXTextureMgr.InitOnce();
        FF9StateSystem.CheckAndroidTVPlatform();
    }

    private void InitStateSystem()
    {
        Transform transform = base.transform;
        GameObject gameObject = new GameObject("Settings State");
        this.settings = gameObject.AddComponent<SettingsState>();
        gameObject.transform.parent = transform;
        gameObject = new GameObject("Common State");
        this.common = gameObject.AddComponent<CommonState>();
        gameObject.transform.parent = transform;
        gameObject = new GameObject("Field State");
        this.field = gameObject.AddComponent<FieldState>();
        gameObject.transform.parent = transform;
        gameObject = new GameObject("WMWorld State");
        this.world = gameObject.AddComponent<WorldState>();
        gameObject.transform.parent = transform;
        gameObject = new GameObject("Battle State");
        this.battle = gameObject.AddComponent<BattleStateSystem>();
        gameObject.transform.parent = transform;
        gameObject = new GameObject("MiniGame State");
        this.miniGame = gameObject.AddComponent<MiniGameState>();
        gameObject.transform.parent = transform;
        gameObject = new GameObject("Event State");
        this.eventState = gameObject.AddComponent<EventState>();
        gameObject.transform.parent = transform;
        gameObject = new GameObject("Sound State");
        this.sound = gameObject.AddComponent<SoundState>();
        gameObject.transform.parent = transform;
        gameObject = new GameObject("Achievement State");
        this.achievement = gameObject.AddComponent<AchievementState>();
        gameObject.transform.parent = transform;
        FF9StateSystem.InitFF9GameState();
    }

    public static void ReInitStateSystem()
    {
        FF9StateSystem.Settings.Initial();
        FF9StateSystem.Common.Init();
        FF9StateSystem.Field.Init();
        FF9StateSystem.World.NewGame();
        FF9StateSystem.Battle.Init();
        FF9StateSystem.MiniGame.NewGame();
        FF9StateSystem.Sound.NewGame();
        FF9StateSystem.Achievement.Initial();
        AchievementManager.NewGame();
        PersistenSingleton<EventEngine>.Instance.NewGame();
        FF9StateSystem.InitFF9GameState();
    }

    private static void InitFF9GameState()
    {
        ff9item.FF9Item_Init();
        ff9play.FF9Play_Init();
        FF9StateSystem.Common.FF9.ff9ResetStateGlobal();
    }

    private void InitStateSerializer()
    {
        GameObject serializerGo = new GameObject("WinIosAndrSharedDataSerializer");
        WinIosAndrSharedDataSerializer sharedSerializer = serializerGo.AddComponent<WinIosAndrSharedDataSerializer>();
        this.serializer = sharedSerializer;
        serializerGo.transform.parent = base.transform;
        GameObject gameObject = new GameObject("JsonParser");
        sharedSerializer.Parser = gameObject.AddComponent<JsonParser>();
        gameObject.transform.parent = sharedSerializer.transform;
        gameObject = new GameObject("SharedDataAesEncryption");
        sharedSerializer.Encryption = gameObject.AddComponent<SharedDataAesEncryption>();
        sharedSerializer.Encryption.transform.parent = serializerGo.transform;
        gameObject = new GameObject("SharedDataBytesStorage");
        sharedSerializer.Storage = gameObject.AddComponent<SharedDataBytesStorage>();
        sharedSerializer.Storage.transform.parent = serializerGo.transform;
        if (sharedSerializer.Storage is SharedDataBytesStorage)
        {
            gameObject = new GameObject("SharedDataRawStorage");
            sharedSerializer.RawStorage = gameObject.AddComponent<SharedDataRawStorage>();
            sharedSerializer.RawStorage.Encryption = sharedSerializer.Encryption;
            gameObject.transform.parent = sharedSerializer.Storage.transform;
        }
        sharedSerializer.Storage.Encryption = sharedSerializer.Encryption;
        gameObject = new GameObject("SharedDataSlotPreviewUtil");
        gameObject.AddComponent<SlotPreviewReadWriterUtil>();
        gameObject.transform.parent = sharedSerializer.transform;
    }

    private void InitQuadMistDatabase()
    {
        GameObject gameObject = new GameObject("QuadMistDatabase");
        this.quadMistDatabase = gameObject.AddComponent<QuadMistDatabase>();
        gameObject.transform.parent = base.transform;
        gameObject = new GameObject("QuadMistCardPool");
        this.quadMistCardPool = gameObject.AddComponent<CardPool>();
        gameObject.transform.parent = base.transform;
    }

    private static void CheckAndroidTVPlatform()
    {
    }

    private const Boolean IsEStoreEnabled = false;
    private static Boolean IsAndroidTV = false;

    public static Boolean Editor = Application.platform == RuntimePlatform.WindowsEditor;
    public static Boolean MobilePlatform = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || FF9StateSystem.Editor;
    public static Boolean PCPlatform = Application.platform == RuntimePlatform.WindowsPlayer;
    public static Boolean aaaaPlatform = false;
    public static Boolean MobileAndaaaaPlatform = FF9StateSystem.MobilePlatform || FF9StateSystem.aaaaPlatform;
    public static Boolean PCEStorePlatform = false;
    public static Boolean IOSPlatform = Application.platform == RuntimePlatform.IPhonePlayer;
    public static Boolean AndroidPlatform = Application.platform == RuntimePlatform.Android;
    public static Boolean AndroidSQEXMarket = false;
    public static Boolean EnableAndroidTVJoystickMode = true;
    public static Boolean IsPlatformVibration => Editor || PCPlatform;

    public UInt32 attr;
    public Byte mode;
    public Byte prevMode;

    public static Double LatestTimestamp = -1.0;
    public static Int32 latestSlot = -1;
    public static Int32 latestSave = -1;

    private SettingsState settings;
    private CommonState common;
    private FieldState field;
    private WorldState world;
    private BattleStateSystem battle;
    private MiniGameState miniGame;
    private EventState eventState;
    private SoundState sound;
    private AchievementState achievement;
    private ISharedDataSerializer serializer;
    private QuadMistDatabase quadMistDatabase;
    private CardPool quadMistCardPool;
}
