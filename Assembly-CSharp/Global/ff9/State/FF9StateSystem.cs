using System;
using Assets.SiliconSocial;
using UnityEngine;

public class FF9StateSystem : PersistenSingleton<FF9StateSystem>
{
	public static Boolean AndroidTVPlatform
	{
		get
		{
			return FF9StateSystem.IsAndroidTV;
		}
	}

	public static Boolean AndroidAmazonPlatform
	{
		get
		{
			return false;
		}
	}

	public static SettingsState Settings
	{
		get
		{
			return PersistenSingleton<FF9StateSystem>.Instance.settings;
		}
	}

	public static CommonState Common
	{
		get
		{
			return PersistenSingleton<FF9StateSystem>.Instance.common;
		}
	}

	public static FieldState Field
	{
		get
		{
			return PersistenSingleton<FF9StateSystem>.Instance.field;
		}
	}

	public static WorldState World
	{
		get
		{
			return PersistenSingleton<FF9StateSystem>.Instance.world;
		}
	}

	public static BattleState Battle
	{
		get
		{
			return PersistenSingleton<FF9StateSystem>.Instance.battle;
		}
	}

	public static MiniGameState MiniGame
	{
		get
		{
			return PersistenSingleton<FF9StateSystem>.Instance.miniGame;
		}
	}

	public static EventState EventState
	{
		get
		{
			return PersistenSingleton<FF9StateSystem>.Instance.eventState;
		}
	}

	public static SoundState Sound
	{
		get
		{
			return PersistenSingleton<FF9StateSystem>.Instance.sound;
		}
	}

	public static AchievementState Achievement
	{
		get
		{
			return PersistenSingleton<FF9StateSystem>.Instance.achievement;
		}
	}

	public static ISharedDataSerializer Serializer
	{
		get
		{
			return PersistenSingleton<FF9StateSystem>.Instance.serializer;
		}
	}

	public static QuadMistDatabase QuadMistDatabase
	{
		get
		{
			return PersistenSingleton<FF9StateSystem>.Instance.quadMistDatabase;
		}
	}

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
		this.battle = gameObject.AddComponent<BattleState>();
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
		GameObject gameObject = new GameObject("WinIosAndrSharedDataSerializer");
		WinIosAndrSharedDataSerializer winIosAndrSharedDataSerializer = gameObject.AddComponent<WinIosAndrSharedDataSerializer>();
		this.serializer = winIosAndrSharedDataSerializer;
		gameObject.transform.parent = base.transform;
		GameObject gameObject2 = new GameObject("JsonParser");
		JsonParser parser = gameObject2.AddComponent<JsonParser>();
		winIosAndrSharedDataSerializer.Parser = parser;
		gameObject2.transform.parent = winIosAndrSharedDataSerializer.transform;
		GameObject gameObject3 = new GameObject("SharedDataAesEncryption");
		winIosAndrSharedDataSerializer.Encryption = gameObject3.AddComponent<SharedDataAesEncryption>();
		winIosAndrSharedDataSerializer.Encryption.transform.parent = gameObject.transform;
		GameObject gameObject4 = new GameObject("SharedDataBytesStorage");
		winIosAndrSharedDataSerializer.Storage = gameObject4.AddComponent<SharedDataBytesStorage>();
		winIosAndrSharedDataSerializer.Storage.transform.parent = gameObject.transform;
		if (winIosAndrSharedDataSerializer.Storage is SharedDataBytesStorage)
		{
			GameObject gameObject5 = new GameObject("SharedDataRawStorage");
			SharedDataRawStorage sharedDataRawStorage = gameObject5.AddComponent<SharedDataRawStorage>();
			gameObject5.transform.parent = winIosAndrSharedDataSerializer.Storage.transform;
			winIosAndrSharedDataSerializer.RawStorage = sharedDataRawStorage;
			sharedDataRawStorage.Encryption = winIosAndrSharedDataSerializer.Encryption;
		}
		winIosAndrSharedDataSerializer.Storage.Encryption = winIosAndrSharedDataSerializer.Encryption;
		GameObject gameObject6 = new GameObject("SharedDataSlotPreviewUtil");
		gameObject6.AddComponent<SlotPreviewReadWriterUtil>();
		gameObject6.transform.parent = winIosAndrSharedDataSerializer.transform;
	}

	private void InitQuadMistDatabase()
	{
		GameObject gameObject = new GameObject("QuadMistDatabase");
		QuadMistDatabase quadMistDatabase = gameObject.AddComponent<QuadMistDatabase>();
		this.quadMistDatabase = quadMistDatabase;
		gameObject.transform.parent = base.transform;
		gameObject = new GameObject("QuadMistCardPool");
		CardPool cardPool = gameObject.AddComponent<CardPool>();
		this.quadMistCardPool = cardPool;
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

	private BattleState battle;

	private MiniGameState miniGame;

	private EventState eventState;

	private SoundState sound;

	private AchievementState achievement;

	private ISharedDataSerializer serializer;

	private QuadMistDatabase quadMistDatabase;

	private CardPool quadMistCardPool;
}
