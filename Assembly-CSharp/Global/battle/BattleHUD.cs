using Assets.Sources.Scripts.UI.Common;
using FF9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Database;
using Memoria.Prime;
using UnityEngine;
using Object = System.Object;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable RedundantExplicitArraySize
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable InconsistentNaming
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Local
// ReSharper disable EmptyConstructor
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable UnusedParameter.Global

public class BattleHUD : UIScene
{
    public const Byte BTLMES_LEVEL_FOLLOW_0 = 0;
    public const Byte BTLMES_LEVEL_FOLLOW_1 = 1;
    public const Byte BTLMES_LEVEL_TITLE = 1;
    public const Byte BTLMES_LEVEL_LIBRA = 2;
    public const Byte BTLMES_LEVEL_EVENT = 3;
    public const Byte LIBRA_MES_NO = 10;
    public const Byte PEEPING_MES_NO = 8;
    public const Byte BTLMES_ATTRIBUTE_START = 0;

    public static String CommandGroupButton;
    public static String TargetGroupButton;
    public static String AbilityGroupButton;
    public static String ItemGroupButton;
    public ModelButtonManager modelButtonManager;
    public GameObject AutoBattleHud;
    public GameObject AutoBattleButton;
    public GameObject AllTargetButton;
    public GameObject RunButton;
    public GameObject BackButton;
    public GameObject PauseButtonGameObject;
    public GameObject HelpButtonGameObject;
    public GameObject HideHudHitAreaGameObject;
    public GameObject AllMenuPanel;
    public GameObject TargetPanel;
    public GameObject AbilityPanel;
    public GameObject ItemPanel;
    public GameObject CommandPanel;
    public GameObject PartyDetailPanel;
    public GameObject BattleDialogGameObject;
    public GameObject StatusContainer;
    public GameObject TransitionGameObject;
    public GameObject ScreenFadeGameObject;
    public Boolean BtlWorkLibra => _currentLibraMessageNumber > 0;
    public Boolean BtlWorkPeep => _currentPeepingMessageCount > 0;
    public GameObject PlayerTargetPanel => TargetPanel.GetChild(0);
    public GameObject EnemyTargetPanel => TargetPanel.GetChild(1);
    public List<Int32> ReadyQueue { get; }
    public List<Int32> InputFinishList { get; }
    public Int32 CurrentPlayerIndex { get; private set; }
    public Boolean IsDoubleCast => (Int32)_currentCommandId == 23 || (Int32)_currentCommandId == 21;

    private static readonly Byte[] BattleMessageTimeTick;
    private static readonly Byte[] CmdTitleTable;
    private static readonly Int32 YINFO_ANIM_HPMP_MIN;
    private static readonly Int32 YINFO_ANIM_HPMP_MAX;
    private static readonly Int32 AbilFenril;
    private static readonly Int32 AbilCarbuncle;
    private static readonly Int32 AbilSaMpHalf;
    private static readonly String ATENormal;
    private static readonly String ATEGray;
    private static readonly String ATEOrange;
    private static readonly Single DefaultPartyPanelPosY;
    private static readonly Single PartyItemHeight;
    private static readonly Dictionary<UInt32, Byte> BadIconDict;
    private static readonly Dictionary<UInt32, Byte> GoodIconDict;

    private readonly Dictionary<Int32, AbilityPlayerDetail> _abilityDetailDict;
    private readonly MagicSwordCondition _magicSwordCond;
    private readonly List<ParameterStatus> _currentCharacterHp;
    private readonly List<Boolean> _currentEnemyDieState;
    private readonly List<InfoVal> _hpInfoVal;
    private readonly List<InfoVal> _mpInfoVal;
    private readonly List<Int32> _currentTrancePlayer;
    private readonly Color[] _tranceTextColor;
    private readonly Dictionary<Int32, CommandMenu> _commandCursorMemorize;
    private readonly Dictionary<Int32, Int32> _ability1CursorMemorize;
    private readonly Dictionary<Int32, Int32> _ability2CursorMemorize;
    private readonly Dictionary<Int32, Int32> _itemCursorMemorize;
    private readonly List<Int32> _matchBattleIdPlayerList;
    private readonly List<Int32> _matchBattleIdEnemyList;
    private readonly List<Int32> _itemIdList;
    private readonly List<TargetHUD> _targetHudList;

    private Single lastFrameRightTriggerAxis;
    private Boolean lastFramePressOnMenu;
    private Byte _currentLibraMessageNumber;
    private Byte _currentLibraMessageCount;
    private BTL_DATA _libraBtlData;
    private Byte _currentPeepingMessageCount;
    private ENEMY _peepingEnmData;
    private Byte _currentMessagePriority;
    private Single _battleMessageCounter;
    private CommandHUD _commandDetailHUD;
    private Int32 _enemyCount;
    private Int32 _playerCount;
    private RecycleListPopulator _itemScrollList;
    private RecycleListPopulator _abilityScrollList;
    private Boolean _isTranceMenu;
    private Boolean _needItemUpdate;
    private Boolean _currentSilenceStatus;
    private Int32 _currentMpValue;
    private Single _blinkAlphaCounter;
    private Int32 _tranceColorCounter;
    private UIWidget _battleDialogWidget;
    private UILabel _battleDialogLabel;
    private UIToggle _autoBattleToggle;
    private UIToggle _allTargetToggle;
    private UIButton _autoBattleButtonComponent;
    private UIButton _allTargetButtonComponent;
    private TargetPanelDescriptor _targetPanel;
    //private HonoTweenClipping _itemTransition;
    //private HonoTweenClipping _abilityTransition;
    //private HonoTweenClipping _targetTransition;
    private StatusContainerDescriptor _status;
    private PartyDetailPanelDescriptor _partyDetail;
    private Boolean _commandEnable;
    private Boolean _beforePauseCommandEnable;
    private Boolean _isFromPause;
    private Boolean _isNeedToInit;
    private CommandMenu _currentCommandIndex;
    private UInt32 _currentCommandId;
    private String _currentButtonGroup;
    private Int32 _currentSubMenuIndex;
    private Int32 _currentTargetIndex;
    private List<Int32> _targetIndexList;
    private SubMenuType _subMenuType;
    private List<Int32> _unconsciousStateList;
    private Single _runCounter;
    private Boolean _hidingHud;
    private CursorGroup _cursorType;
    private Boolean _defaultTargetCursor;
    private Boolean _defaultTargetDead;
    private Boolean _targetDead;
    private TargetType _targetCursor;
    private Boolean _isTryingToRun;
    private Boolean _isAutoAttack;
    private Boolean _isAllTarget;
    private Byte _doubleCastCount;
    private CommandDetail _firstCommand;
    private Action _onResumeFromQuit;
    private Boolean _oneTime;
    private Int32 CurrentBattlePlayerIndex => _matchBattleIdPlayerList.IndexOf(CurrentPlayerIndex);

    static BattleHUD()
    {
        BattleMessageTimeTick = new Byte[7] { 54, 46, 48, 30, 24, 18, 12 };

        CmdTitleTable = new Byte[192]
        {
            0,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            153,
            254,
            154,
            254,
            155,
            254,
            192,
            254,
            254,
            157,
            254,
            158,
            254,
            159,
            254,
            160,
            254,
            194,
            193,
            195,
            196,
            197,
            198,
            73,
            Byte.MaxValue,
            187,
            254,
            254,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            192,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            0,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            0,
            0,
            0,
            Byte.MaxValue,
            0,
            0,
            Byte.MaxValue,
            0,
            0,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            Byte.MaxValue,
            254,
            0,
            254
        };

        YINFO_ANIM_HPMP_MIN = 4;
        YINFO_ANIM_HPMP_MAX = 16;
        AbilFenril = 66;
        AbilCarbuncle = 68;
        AbilSaMpHalf = 226;
        ATENormal = "battle_bar_atb";
        ATEGray = "battle_bar_slow";
        ATEOrange = "battle_bar_haste";
        DefaultPartyPanelPosY = -350f;
        PartyItemHeight = 82f;
        BadIconDict = new Dictionary<UInt32, Byte>
        {
            {1U, 154},
            {4096U, 145},
            {2U, 153},
            {65536U, 144},
            {4U, 152},
            {64U, 148},
            {16777216U, 141},
            {33554432U, 140},
            {1024U, 147},
            {131072U, 143},
            {2048U, 146},
            {32U, 149},
            {8U, 151},
            {16U, 150},
            {268435456U, 142},
            {1048576U, 139}
        };

        GoodIconDict = new Dictionary<UInt32, Byte>
        {
            {8192U, 131},
            {262144U, 138},
            {536870912U, 132},
            {524288U, 137},
            {4194304U, 135},
            {8388608U, 134},
            {2097152U, 136},
            {67108864U, 133}
        };
        CommandGroupButton = "Battle.Command";
        TargetGroupButton = "Battle.Target";
        AbilityGroupButton = "Battle.Ability";
        ItemGroupButton = "Battle.Item";
    }

    public BattleHUD()
    {
        _abilityDetailDict = new Dictionary<Int32, AbilityPlayerDetail>();
        _magicSwordCond = new MagicSwordCondition();
        _enemyCount = -1;
        _playerCount = -1;
        _currentCharacterHp = new List<ParameterStatus>();
        _currentEnemyDieState = new List<Boolean>();
        _hpInfoVal = new List<InfoVal>();
        _mpInfoVal = new List<InfoVal>();
        _currentTrancePlayer = new List<Int32>();
        _currentMpValue = -1;
        _tranceTextColor = new[]
        {
            // 13
            new Color(1f, 0.2156863f, 0.3176471f),
            new Color(1f, 0.3490196f, 0.3529412f),
            new Color(1f, 0.4862745f, 0.3921569f),
            new Color(1f, 0.6235294f, 0.427451f),
            new Color(1f, 0.7568628f, 0.4666667f),
            new Color(1f, 0.8941177f, 0.5058824f),
            new Color(1f, 0.9647059f, 0.5254902f),
            new Color(1f, 0.8941177f, 0.5058824f),
            new Color(1f, 0.7568628f, 0.4666667f),
            new Color(1f, 0.6235294f, 0.427451f),
            new Color(1f, 0.4862745f, 0.3921569f),
            new Color(1f, 0.3490196f, 0.3529412f),
            new Color(1f, 0.2156863f, 0.3176471f)
        };

        _currentButtonGroup = String.Empty;
        CurrentPlayerIndex = -1;
        _currentTargetIndex = -1;
        _targetIndexList = new List<Int32>();
        ReadyQueue = new List<Int32>();
        InputFinishList = new List<Int32>();
        _unconsciousStateList = new List<Int32>();
        _firstCommand = new CommandDetail();
        _commandCursorMemorize = new Dictionary<Int32, CommandMenu>();
        _ability1CursorMemorize = new Dictionary<Int32, Int32>();
        _ability2CursorMemorize = new Dictionary<Int32, Int32>();
        _itemCursorMemorize = new Dictionary<Int32, Int32>();
        _matchBattleIdPlayerList = new List<Int32>();
        _matchBattleIdEnemyList = new List<Int32>();
        _itemIdList = new List<Int32>();
        _targetHudList = new List<TargetHUD>();
        _oneTime = true;
    }

    public Boolean AndroidTVOnKeyRightTrigger(GameObject go)
    {
        return CheckAndroidTVModule(Control.RightTrigger);
    }

    private void UpdateAndroidTV()
    {
        HonoInputManager instance = PersistenSingleton<HonoInputManager>.Instance;
        if (!FF9StateSystem.AndroidTVPlatform || !instance.IsControllerConnect || !FF9StateSystem.EnableAndroidTVJoystickMode)
            return;
        Single axisRaw = Input.GetAxisRaw(instance.SpecificPlatformRightTriggerKey);
        Boolean button = Input.GetButton(instance.DefaultJoystickInputKeys[2]);
        Boolean flag = axisRaw > 0.189999997615814 && button;
        if (flag && lastFrameRightTriggerAxis > 0.189999997615814 && lastFramePressOnMenu)
            flag = false;
        if (flag && !_hidingHud)
            ProcessAutoBattleInput();
        lastFrameRightTriggerAxis = axisRaw;
        lastFramePressOnMenu = button;
    }

    private void UpdateMessage()
    {
        if (!BattleDialogGameObject.activeSelf || PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.BattleHUD)
            return;

        _battleMessageCounter += Time.deltaTime * FF9StateSystem.Settings.FastForwardFactor;
        if (_battleMessageCounter < BattleMessageTimeTick[FF9StateSystem.Settings.cfg.btl_msg] / 15.0)
            return;

        BattleDialogGameObject.SetActive(false);
        _currentMessagePriority = 0;

        if (_currentLibraMessageNumber > 0)
            DisplayMessageLibra();

        if (_currentPeepingMessageCount <= 0)
            return;

        DisplayMessagePeeping();
    }

    private void DisplayBattleMessage(String str, Boolean isRect)
    {
        BattleDialogGameObject.SetActive(false);
        if (isRect)
        {
            _battleDialogWidget.width = (Int32)(128.0 * UIManager.ResourceXMultipier);
            _battleDialogWidget.height = 120;
            _battleDialogWidget.transform.localPosition = new Vector3(0.0f, 445f, 0.0f);
        }
        else
        {
            _battleDialogWidget.width = (Int32)(240.0 * UIManager.ResourceXMultipier);
            if (str.Contains("\n"))
            {
                _battleDialogWidget.height = 200;
                _battleDialogWidget.transform.localPosition = new Vector3(-10f, 405f, 0.0f);
            }
            else
            {
                _battleDialogWidget.height = 120;
                _battleDialogWidget.transform.localPosition = new Vector3(-10f, 445f, 0.0f);
            }
        }

        Single additionalWidth = 0.0f;
        _battleDialogLabel.text = _battleDialogLabel.PhrasePreOpcodeSymbol(str, ref additionalWidth);
        BattleDialogGameObject.SetActive(true);
    }

    private void DisplayMessageLibra()
    {
        if (_libraBtlData == null)
            return;

        String str = String.Empty;
        if (_currentLibraMessageNumber == 1)
        {
            str = (_libraBtlData.bi.player == 0 ? btl_util.getEnemyPtr(_libraBtlData).et.name : btl_util.getPlayerPtr(_libraBtlData).name) + FF9TextTool.BattleLibraText(10) + _libraBtlData.level.ToString();
            _currentLibraMessageNumber = 2;
        }
        else if (_currentLibraMessageNumber == 2)
        {
            str = FF9TextTool.BattleLibraText(11) + _libraBtlData.cur.hp + FF9TextTool.BattleLibraText(13) + _libraBtlData.max.hp + FF9TextTool.BattleLibraText(12) + _libraBtlData.cur.mp + FF9TextTool.BattleLibraText(13) + _libraBtlData.max.mp;
            _currentLibraMessageCount = 0;
            _currentLibraMessageNumber = 3;
        }
        else if (_currentLibraMessageNumber == 3)
        {
            if (_libraBtlData.bi.player == 0)
            {
                Int32 num1 = FF9StateSystem.Battle.FF9Battle.enemy[_libraBtlData.bi.slot_no].et.category;
                Int32 id;
                do
                {
                    Byte num2 = _currentLibraMessageCount++;
                    if ((id = num2) >= 8)
                        goto label_11;
                } while ((num1 & 1 << id) == 0);
                SetBattleMessage(FF9TextTool.BattleLibraText(id), 2);
                return;
            }
            label_11:
            _currentLibraMessageCount = 0;
            _currentLibraMessageNumber = 4;
        }

        if (_currentLibraMessageNumber == 4)
        {
            Int32 num1 = _libraBtlData.def_attr.weak & ~_libraBtlData.def_attr.invalid;
            Int32 num2;
            do
            {
                Byte num3 = _currentLibraMessageCount++;
                if ((num2 = num3) >= 8)
                    goto label_17;
            } while ((num1 & 1 << num2) == 0);
            SetBattleMessage(Localization.GetSymbol() != "JP" ? str + FF9TextTool.BattleLibraText(14 + num2) : BtlGetAttrName(1 << num2) + FF9TextTool.BattleLibraText(14), 2);
            return;
            label_17:
            _currentLibraMessageCount = 0;
            _currentLibraMessageNumber = 5;
        }

        if (_currentLibraMessageNumber == 5)
        {
            _libraBtlData = null;
            _currentLibraMessageCount = 0;
            _currentLibraMessageNumber = 0;
        }
        else
        {
            SetBattleMessage(str, 2);
        }
    }

    private void DisplayMessagePeeping()
    {
        if (_peepingEnmData == null)
            return;

        Int32 id;
        do
        {
            Byte num1 = _currentPeepingMessageCount++;
            Int32 num2;
            if ((num2 = num1) < _peepingEnmData.steal_item.Length + 1)
                id = _peepingEnmData.steal_item[_peepingEnmData.steal_item.Length - num2];
            else
                goto label_5;
        } while (id == Byte.MaxValue);

        SetBattleMessage(Localization.GetSymbol() != "JP"
            ? FF9TextTool.BattleLibraText(8) + FF9TextTool.ItemName(id)
            : FF9TextTool.ItemName(id) + FF9TextTool.BattleLibraText(8), 2);

        return;
        label_5:
        _peepingEnmData = null;
        _currentPeepingMessageCount = 0;
    }

    public void SetBattleFollowMessage(Int32 pMesNo, params Object[] args)
    {
        String str1 = FF9TextTool.BattleFollowText(pMesNo + 7);
        if (String.IsNullOrEmpty(str1))
            return;

        Byte priority = (Byte)Char.GetNumericValue(str1[0]);
        String str2 = str1.Substring(1);

        if (args.Length > 0)
        {
            String str3 = args[0].ToString();
            Int32 result;
            str2 = !Int32.TryParse(str3, out result) ? str2.Replace("%", str3) : str2.Replace("&", str3);
        }

        SetBattleMessage(str2, priority);
    }

    public void SetBattleFollowMessage(Byte priority, String formatMessage, params Object[] args)
    {
        if (String.IsNullOrEmpty(formatMessage))
            return;

        String message;
        try
        {
            message = String.Format(formatMessage, args);
        }
        catch (FormatException ex)
        {
            Log.Error(ex, "Cannot format a battle follow message.");
            message = formatMessage;
        }
        
        SetBattleMessage(message, priority);
    }

    public void SetBattleCommandTitle(CMD_DATA pCmd)
    {
        String str1 = String.Empty;
        switch (pCmd.cmd_no)
        {
            case 14:
            case 15:
                str1 = FF9TextTool.ItemName(pCmd.sub_no);
                break;
            case 50:
                str1 = pCmd.aa.Name;
                break;
            default:
                if (pCmd.sub_no < 192)
                {
                    Int32 id = CmdTitleTable[pCmd.sub_no];
                    switch (id)
                    {
                        case 254: // Magic sword
                            str1 = FormatMagicSwordAbility(pCmd);
                            break;
                        case Byte.MaxValue:
                            str1 = FF9TextTool.ActionAbilityName(pCmd.sub_no);
                            break;
                        case 0:
                            break;
                        default:
                            str1 = id >= 192 ? FF9TextTool.BattleCommandTitleText((id & 63) + 1) : FF9TextTool.ActionAbilityName(id);
                            break;
                    }
                }
                break;
        }

        if (String.IsNullOrEmpty(str1))
            return;

        SetBattleTitle(str1, 1);
    }

    private static String FormatMagicSwordAbility(CMD_DATA pCmd)
    {
        // TODO: Move it to an external file
        String abilityName = FF9TextTool.ActionAbilityName(pCmd.sub_no);
        switch (abilityName)
        {
            case "Огонь":
                return "Огненный клинок";
            case "Огонь II":
                return "Огненный клинок II";
            case "Огонь III":
                return "Огненный клинок III";
            case "Буран":
                return "Ледяной клинок";
            case "Буран II":
                return "Ледяной клинок II";
            case "Буран III":
                return "Ледяной клинок III";
            case "Молния":
                return "Электрический клинок";
            case "Молния II":
                return "Электрический клинок II";
            case "Молния III":
                return "Электрический клинок III";
            case "Био":
                return "Ядовитый клинок";
            case "Вода":
                return "Водный клинок";
            case "Взрыв":
                return "Взрывной клинок";
            case "Судный день":
                return "Клинок Судного дня";
        }

        String str2 = Localization.GetSymbol() == "JP" ? String.Empty : " ";
        return abilityName + str2 + FF9TextTool.BattleCommandTitleText(0);
    }

    public String BtlGetAttrName(Int32 pAttr)
    {
        Int32 id = 0;
        while ((pAttr >>= 1) != 0)
            ++id;

        return FF9TextTool.BattleFollowText(id);
    }

    public void SetBattleLibra(BTL_DATA pBtl)
    {
        _currentLibraMessageNumber = 1;
        _libraBtlData = pBtl;
        DisplayMessageLibra();
    }

    public void SetBattlePeeping(BTL_DATA pBtl)
    {
        if (pBtl.bi.player != 0)
            return;

        _peepingEnmData = FF9StateSystem.Battle.FF9Battle.enemy[pBtl.bi.slot_no];
        Boolean flag = false;
        for (Int32 index = 0; index < 4; ++index)
        {
            if (_peepingEnmData.steal_item[index] == Byte.MaxValue)
                continue;

            flag = true;
            break;
        }

        if (!flag)
        {
            SetBattleMessage(FF9TextTool.BattleLibraText(9), 2);
            _currentPeepingMessageCount = 5;
        }
        else
        {
            _currentPeepingMessageCount = 1;
            DisplayMessagePeeping();
        }
    }

    public void SetBattleTitle(String str, Byte priority)
    {
        if (_currentMessagePriority > priority)
            return;

        _currentMessagePriority = priority;
        _battleMessageCounter = 0.0f;
        DisplayBattleMessage(str, true);
    }

    public void SetBattleMessage(String str, Byte priority)
    {
        if (_currentMessagePriority > priority)
            return;

        _currentMessagePriority = priority;
        _battleMessageCounter = 0.0f;
        DisplayBattleMessage(str, false);
    }

    private void DisplayCommand()
    {
        BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex];
        Byte presetId = FF9StateSystem.Common.FF9.party.member[btl.bi.line_no].info.menu_type;
        command_tags command1;
        command_tags command2;

        if (Status.checkCurStat(btl, 16384U))
        {
            command1 = CharacterCommands.CommandSets[presetId].Trance1;
            command2 = CharacterCommands.CommandSets[presetId].Trance2;
            _commandDetailHUD.CaptionLabel.text = Localization.Get("TranceCaption");
            _isTranceMenu = true;
        }
        else
        {
            command1 = CharacterCommands.CommandSets[presetId].Regular1;
            command2 = CharacterCommands.CommandSets[presetId].Regular2;
            _commandDetailHUD.CaptionLabel.text = Localization.Get("CommandCaption");
            _commandDetailHUD.CaptionLabel.color = FF9TextTool.White;
            _isTranceMenu = false;
        }

        String str1 = FF9TextTool.CommandName((Byte)command1);
        String str2 = FF9TextTool.CommandName((Byte)command2);
        Boolean flag1 = command1 != 0;
        Boolean flag2 = command2 != 0;

        if (command2 == command_tags.CMD_MAGIC_SWORD)
        {
            if (!_magicSwordCond.IsViviExist)
            {
                str2 = String.Empty;
                flag2 = false;
            }
            else if (_magicSwordCond.IsViviDead || _magicSwordCond.IsSteinerMini)
            {
                flag2 = false;
            }
        }

        _commandDetailHUD.Skill1Component.Label.text = str1;
        ButtonGroupState.SetButtonEnable(_commandDetailHUD.Skill1, flag1);
        ButtonGroupState.SetButtonAnimation(_commandDetailHUD.Skill1, flag1);

        if (flag1)
        {
            _commandDetailHUD.Skill1Component.Label.color = FF9TextTool.White;
            _commandDetailHUD.Skill1Component.ButtonGroup.Help.Enable = true;
            _commandDetailHUD.Skill1Component.ButtonGroup.Help.TextKey = String.Empty;
            _commandDetailHUD.Skill1Component.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription((Byte)command1);
        }
        else
        {
            _commandDetailHUD.Skill1Component.Label.color = FF9TextTool.Gray;
            _commandDetailHUD.Skill1Component.UIBoxCollider.enabled = false;
            _commandDetailHUD.Skill1Component.ButtonGroup.Help.Enable = false;
        }

        _commandDetailHUD.Skill2Component.Label.text = str2;
        ButtonGroupState.SetButtonEnable(_commandDetailHUD.Skill2, flag2);
        ButtonGroupState.SetButtonAnimation(_commandDetailHUD.Skill2, flag2);

        if (flag2)
        {
            _commandDetailHUD.Skill2Component.Label.color = FF9TextTool.White;
            _commandDetailHUD.Skill2Component.ButtonGroup.Help.Enable = true;
            _commandDetailHUD.Skill2Component.ButtonGroup.Help.TextKey = String.Empty;
            _commandDetailHUD.Skill2Component.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription((Byte)command2);
        }
        else
        {
            _commandDetailHUD.Skill2Component.Label.color = FF9TextTool.Gray;
            _commandDetailHUD.Skill2Component.UIBoxCollider.enabled = false;
            _commandDetailHUD.Skill2Component.ButtonGroup.Help.Enable = false;
        }

        _commandDetailHUD.AttackComponent.Label.text = FF9TextTool.CommandName(1);
        _commandDetailHUD.DefendComponent.Label.text = FF9TextTool.CommandName(4);
        _commandDetailHUD.ItemComponent.Label.text = FF9TextTool.CommandName(14);
        _commandDetailHUD.ChangeComponent.Label.text = FF9TextTool.CommandName(7);
        _commandDetailHUD.AttackComponent.ButtonGroup.Help.TextKey = String.Empty;
        _commandDetailHUD.AttackComponent.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription(1);
        _commandDetailHUD.DefendComponent.ButtonGroup.Help.TextKey = String.Empty;
        _commandDetailHUD.DefendComponent.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription(4);
        _commandDetailHUD.ItemComponent.ButtonGroup.Help.TextKey = String.Empty;
        _commandDetailHUD.ItemComponent.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription(14);
        _commandDetailHUD.ChangeComponent.ButtonGroup.Help.TextKey = String.Empty;
        _commandDetailHUD.ChangeComponent.ButtonGroup.Help.Text = FF9TextTool.CommandHelpDescription(7);
        if (ButtonGroupState.ActiveGroup != CommandGroupButton)
            return;

        SetCommandVisibility(true, false);
    }

    private void DisplayStatus(TargetDisplay subMode)
    {
        StatusContainer.SetActive(true);
        _status.SetActive(false);
        _partyDetail.SetActive(true);

        List<Int32> list = new List<Int32>(new[] { 0, 1, 2, 3 });
        switch (subMode)
        {
            case TargetDisplay.Hp:
                _status.HpStatusPanel.SetActive(true);
                _partyDetail.SetActive(false);
                for (BTL_DATA bd = FF9StateSystem.Battle.FF9Battle.btl_list.next; bd != null; bd = bd.next)
                {
                    Int32 num = 0;
                    while (1 << num != bd.btl_id)
                        ++num;

                    if (bd.bi.player == 0 || !_matchBattleIdPlayerList.Contains(num))
                        continue;

                    Int32 index = _matchBattleIdPlayerList.IndexOf(num);
                    NumberSubModeHUD numberSubModeHud = _status.HpStatusHudList[index];
                    numberSubModeHud.Self.SetActive(true);
                    numberSubModeHud.Current.text = bd.cur.hp.ToString();
                    numberSubModeHud.Max.text = bd.max.hp.ToString();
                    switch (CheckHPState(bd))
                    {
                        case ParameterStatus.Empty:
                            numberSubModeHud.TextColor = FF9TextTool.Red;
                            break;
                        case ParameterStatus.Critical:
                            numberSubModeHud.TextColor = FF9TextTool.Yellow;
                            break;
                        default:
                            numberSubModeHud.TextColor = FF9TextTool.White;
                            break;
                    }
                    list.Remove(index);
                }

                foreach (Int32 index in list)
                    _status.HpStatusHudList[index].Self.SetActive(false);

                break;
            case TargetDisplay.Mp:
                _status.MpStatusPanel.SetActive(true);
                _partyDetail.SetActive(false);
                for (BTL_DATA bd = FF9StateSystem.Battle.FF9Battle.btl_list.next; bd != null; bd = bd.next)
                {
                    Int32 num = 0;
                    while (1 << num != bd.btl_id)
                        ++num;

                    if (bd.bi.player == 0 || !_matchBattleIdPlayerList.Contains(num))
                        continue;

                    Int32 index = _matchBattleIdPlayerList.IndexOf(num);
                    NumberSubModeHUD numberSubModeHud = _status.MpStatusHudList[index];
                    numberSubModeHud.Self.SetActive(true);
                    numberSubModeHud.Current.text = bd.cur.mp.ToString();
                    numberSubModeHud.Max.text = bd.max.mp.ToString();
                    numberSubModeHud.TextColor = CheckMPState(bd) != ParameterStatus.Empty ? FF9TextTool.White : FF9TextTool.Yellow;
                    list.Remove(index);
                }

                foreach (Int32 index in list)
                    _status.MpStatusHudList[index].Self.SetActive(false);

                break;
            case TargetDisplay.Debufs:
                _status.BadStatusPanel.SetActive(true);
                _partyDetail.SetActive(false);
                for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
                {
                    Int32 num1 = 0;
                    while (1 << num1 != btlData.btl_id)
                        ++num1;

                    if (btlData.bi.player == 0 || !_matchBattleIdPlayerList.Contains(num1))
                        continue;

                    Int32 index1 = _matchBattleIdPlayerList.IndexOf(num1);
                    StatusSubModeHUD statusSubModeHud = _status.BadStatusHudList[index1];
                    UInt32 num2 = btlData.stat.cur | btlData.stat.permanent;
                    statusSubModeHud.Self.SetActive(true);
                    foreach (UISprite uiWidget in statusSubModeHud.SpriteList)
                        uiWidget.alpha = 0.0f;

                    Int32 index2 = 0;
                    foreach (KeyValuePair<UInt32, Byte> current in BadIconDict)
                    {
                        if (((Int32)num2 & (Int32)current.Key) != 0)
                        {
                            statusSubModeHud.SpriteList[0, index2].alpha = 1f;
                            statusSubModeHud.SpriteList[0, index2].spriteName = FF9UIDataTool.IconSpriteName[current.Value];
                            ++index2;
                            if (index2 > statusSubModeHud.SpriteList.Length)
                                break;
                        }
                    }
                    list.Remove(index1);
                }

                foreach (Int32 index in list)
                    _status.BadStatusHudList[index].Self.SetActive(false);

                break;

            case TargetDisplay.Bufs:
                _status.GoodStatusPanel.SetActive(true);
                _partyDetail.SetActive(false);
                for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
                {
                    Int32 num1 = 0;
                    while (1 << num1 != btlData.btl_id)
                        ++num1;

                    if (btlData.bi.player == 0 || !_matchBattleIdPlayerList.Contains(num1))
                        continue;

                    Int32 index1 = _matchBattleIdPlayerList.IndexOf(num1);
                    StatusSubModeHUD statusSubModeHud = _status.GoodStatusHudList[index1];
                    UInt32 num2 = btlData.stat.cur | btlData.stat.permanent;
                    statusSubModeHud.Self.SetActive(true);
                    foreach (UISprite uiWidget in statusSubModeHud.SpriteList)
                        uiWidget.alpha = 0.0f;

                    Int32 index2 = 0;
                    foreach (KeyValuePair<UInt32, Byte> current in GoodIconDict)
                    {
                        if (((Int32)num2 & (Int32)current.Key) == 0)
                            continue;

                        statusSubModeHud.SpriteList[0, index2].alpha = 1f;
                        statusSubModeHud.SpriteList[0, index2].spriteName = FF9UIDataTool.IconSpriteName[current.Value];
                        ++index2;

                        if (index2 > statusSubModeHud.SpriteList.Length)
                            break;
                    }
                    list.Remove(index1);
                }

                foreach (Int32 index in list)
                    _status.GoodStatusHudList[index].Self.SetActive(false);

                break;
        }
    }

    private void DisplayAbilityRealTime()
    {
        BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex];
        if (_currentSilenceStatus != btl_stat.CheckStatus(btl, 8U))
        {
            _currentSilenceStatus = !_currentSilenceStatus;
            DisplayAbility();
        }

        if (_currentMpValue == btl.cur.mp)
            return;

        _currentMpValue = btl.cur.mp;
        DisplayAbility();
    }

    private void DisplayItemRealTime()
    {
        if (!_needItemUpdate)
            return;

        _needItemUpdate = false;
        DisplayItem(CharacterCommands.Commands[_currentCommandId].Type == CharacterCommandType.Throw);
    }

    private void DisplayItem(Boolean isThrow)
    {
        _itemIdList.Clear();
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();
        foreach (FF9ITEM ff9Item in FF9StateSystem.Common.FF9.item)
        {
            if (!isThrow)
            {
                if (!citem.YCITEM_IS_ITEM(ff9Item.id) || ff9Item.count <= 0)
                    continue;

                _itemIdList.Add(ff9Item.id);
                BattleItemListData battleItemListData = new BattleItemListData
                {
                    Count = ff9Item.count,
                    Id = ff9Item.id
                };
                inDataList.Add(battleItemListData);
            }
            else if (citem.YCITEM_IS_THROW(ff9Item.id) && ff9Item.count > 0)
            {
                _itemIdList.Add(ff9Item.id);
                BattleItemListData battleItemListData = new BattleItemListData
                {
                    Count = ff9Item.count,
                    Id = ff9Item.id
                };
                inDataList.Add(battleItemListData);
            }
        }

        if (inDataList.Count == 0)
        {
            _itemIdList.Add(Byte.MaxValue);
            BattleItemListData battleItemListData = new BattleItemListData
            {
                Count = 0,
                Id = Byte.MaxValue
            };
            inDataList.Add(battleItemListData);
        }

        if (_itemScrollList.ItemsPool.Count == 0)
        {
            _itemScrollList.PopulateListItemWithData = DisplayItemDetail;
            RecycleListPopulator recycleListPopulator = _itemScrollList;
            BattleHUD battleHud = this;
            RecycleListPopulator.RecycleListItemClick recycleListItemClick = battleHud.OnListItemClick;
            recycleListPopulator.OnRecycleListItemClick += recycleListItemClick;
            _itemScrollList.InitTableView(inDataList, 0);
        }
        else
        {
            _itemScrollList.SetOriginalData(inDataList);
        }
    }

    public void DisplayItemDetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        BattleItemListData battleItemListData = (BattleItemListData)data;
        ItemListDetailWithIconHUD detailWithIconHud = new ItemListDetailWithIconHUD(item.gameObject, true);
        if (isInit)
            DisplayWindowBackground(item.gameObject, null);

        if (battleItemListData.Id == Byte.MaxValue)
        {
            detailWithIconHud.IconSprite.alpha = 0.0f;
            detailWithIconHud.NameLabel.text = String.Empty;
            detailWithIconHud.NumberLabel.text = String.Empty;
            detailWithIconHud.Button.Help.Enable = false;
            detailWithIconHud.Button.Help.TextKey = String.Empty;
            detailWithIconHud.Button.Help.Text = String.Empty;
        }
        else
        {
            FF9UIDataTool.DisplayItem(battleItemListData.Id, detailWithIconHud.IconSprite, detailWithIconHud.NameLabel, true);
            detailWithIconHud.NumberLabel.text = battleItemListData.Count.ToString();
            detailWithIconHud.Button.Help.Enable = true;
            detailWithIconHud.Button.Help.TextKey = String.Empty;
            detailWithIconHud.Button.Help.Text = FF9TextTool.ItemBattleDescription(battleItemListData.Id);
        }
    }

    private void DisplayAbility()
    {
        CharacterCommand ff9Command = CharacterCommands.Commands[_currentCommandId];
        SetAbilityAp(_abilityDetailDict[CurrentPlayerIndex]);
        List<ListDataTypeBase> inDataList = new List<ListDataTypeBase>();

        foreach (Byte id in ff9Command.Abilities)
        {
            BattleAbilityListData battleAbilityListData = new BattleAbilityListData { Id = id };
            inDataList.Add(battleAbilityListData);
        }

        if (_abilityScrollList.ItemsPool.Count == 0)
        {
            _abilityScrollList.PopulateListItemWithData = DisplayAbilityDetail;
            RecycleListPopulator recycleListPopulator = _abilityScrollList;
            BattleHUD battleHud = this;

            RecycleListPopulator.RecycleListItemClick recycleListItemClick = battleHud.OnListItemClick;
            recycleListPopulator.OnRecycleListItemClick += recycleListItemClick;
            _abilityScrollList.InitTableView(inDataList, 0);
        }
        else
        {
            _abilityScrollList.SetOriginalData(inDataList);
        }
    }

    private void DisplayAbilityDetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        BattleAbilityListData battleAbilityListData = (BattleAbilityListData)data;

        ItemListDetailHUD itemListDetailHud = new ItemListDetailHUD(item.gameObject);
        if (isInit)
            DisplayWindowBackground(item.gameObject, null);

        Int32 abilityId = battleAbilityListData.Id;
        AbilityStatus abilityState = GetAbilityState(abilityId);
        AA_DATA aaData = FF9StateSystem.Battle.FF9Battle.aa_data[abilityId];

        if (abilityState == AbilityStatus.None)
        {
            itemListDetailHud.Content.SetActive(false);
            ButtonGroupState.SetButtonAnimation(itemListDetailHud.Self, false);
            itemListDetailHud.Button.Help.TextKey = String.Empty;
            itemListDetailHud.Button.Help.Text = String.Empty;
        }
        else
        {
            itemListDetailHud.Content.SetActive(true);
            itemListDetailHud.NameLabel.text = FF9TextTool.ActionAbilityName(abilityId);
            Int32 mp = GetMp(aaData);
            itemListDetailHud.NumberLabel.text = mp == 0 ? String.Empty : mp.ToString();

            if (abilityState == AbilityStatus.Disable)
            {
                itemListDetailHud.NameLabel.color = FF9TextTool.Gray;
                itemListDetailHud.NumberLabel.color = FF9TextTool.Gray;
                ButtonGroupState.SetButtonAnimation(itemListDetailHud.Self, false);
            }
            else
            {
                itemListDetailHud.NameLabel.color = FF9TextTool.White;
                itemListDetailHud.NumberLabel.color = FF9TextTool.White;
                ButtonGroupState.SetButtonAnimation(itemListDetailHud.Self, true);
            }

            itemListDetailHud.Button.Help.TextKey = String.Empty;
            itemListDetailHud.Button.Help.Text = FF9TextTool.ActionAbilityHelpDescription(abilityId);
        }
    }

    public void DisplayInfomation()
    {
        DisplayParty();
    }

    private void DisplayParty()
    {
        Int32 index = 0;
        for (BTL_DATA bd = FF9StateSystem.Battle.FF9Battle.btl_list.next; bd != null; bd = bd.next)
        {
            Int32 num = 0;
            while (1 << num != bd.btl_id)
                ++num;

            if (bd.bi.player == 0)
                continue;

            PlayerDetailHUD playerHud = _partyDetail.List[index];
            InfoVal hp = _hpInfoVal[index];
            InfoVal mp = _mpInfoVal[index];
            playerHud.PlayerId = num;
            playerHud.Self.SetActive(true);
            DisplayCharacterParameter(playerHud, bd, hp, mp);
            playerHud.TranceSliderGameObject.SetActive(bd.bi.t_gauge != 0);
            ++index;
        }

        PartyDetailPanel.transform.localPosition = new Vector3(PartyDetailPanel.transform.localPosition.x, DefaultPartyPanelPosY - PartyItemHeight * (_partyDetail.List.Count - index), PartyDetailPanel.transform.localPosition.z);
        for (; index < _partyDetail.List.Count; ++index)
        {
            _partyDetail.List[index].Self.SetActive(false);
            _partyDetail.List[index].PlayerId = -1;
        }
    }

    public void DisplayPartyRealtime()
    {
        Int32 index = 0;
        for (BTL_DATA bd = FF9StateSystem.Battle.FF9Battle.btl_list.next; bd != null; bd = bd.next)
        {
            if (bd.bi.player == 0)
                continue;

            PlayerDetailHUD playerHud = _partyDetail.List[index];
            InfoVal hp = _hpInfoVal[index];
            InfoVal mp = _mpInfoVal[index];
            ++index;
            DisplayCharacterParameter(playerHud, bd, hp, mp);
        }
    }

    private void DisplayTarget()
    {
        Boolean flag1 = false;
        Int32 enCount = _enemyCount;
        Int32 plCount = _playerCount;
        Int32 num3 = 0;
        Int32 num4 = 0;

        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            if (btlData.bi.player != 0)
            {
                if (btlData.bi.target != 0)
                    ++num4;
            }
            else if (btlData.bi.target != 0)
            {
                ++num3;
            }
        }

        if (num3 != enCount || num4 != plCount)
        {
            flag1 = true;
            _matchBattleIdPlayerList.Clear();
            _currentCharacterHp.Clear();
            _matchBattleIdEnemyList.Clear();
            _currentEnemyDieState.Clear();
            _enemyCount = num3;
            _playerCount = num4;
        }

        Int32 index1 = 0;
        Int32 index2 = 0;
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            Int32 num5 = 0;
            while (1 << num5 != btlData.btl_id)
                ++num5;

            if (btlData.btl_id == 0 || btlData.bi.target == 0)
                continue;

            if (btlData.bi.player != 0)
            {
                ParameterStatus parameterStatus = CheckHPState(btlData);
                if (index1 >= _currentCharacterHp.Count)
                {
                    _currentCharacterHp.Add(parameterStatus);
                    _matchBattleIdPlayerList.Add(num5);
                    flag1 = true;
                }
                else if (parameterStatus != _currentCharacterHp[index1])
                {
                    _currentCharacterHp[index1] = parameterStatus;
                    flag1 = true;
                }
                ++index1;
            }
            else
            {
                Boolean flag2 = Status.checkCurStat(btlData, 256U);
                if (index2 >= _currentEnemyDieState.Count)
                {
                    _currentEnemyDieState.Add(flag2);
                    _matchBattleIdEnemyList.Add(num5);
                    flag1 = true;
                }
                else if (flag2 != _currentEnemyDieState[index2])
                {
                    _currentEnemyDieState[index2] = flag2;
                    flag1 = true;
                }
                ++index2;
            }
        }

        if (!flag1)
            return;

        foreach (TargetHUD current in _targetHudList)
        {
            current.KeyNavigate.startsSelected = false;
            current.Self.SetActive(false);
        }

        GameObject obj = null;
        Int32 index3 = 0;
        Int32 index4 = 4;
        if (_cursorType == CursorGroup.Individual)
            obj = ButtonGroupState.ActiveButton;

        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.btl_id == 0 || btl.bi.target == 0)
                continue;

            if (btl.bi.player != 0)
            {
                TargetHUD targetHud = _targetHudList[index3];
                GameObject go = targetHud.Self;
                UILabel uiLabel = targetHud.NameLabel;
                go.SetActive(true);
                uiLabel.text = btl_util.getPlayerPtr(btl).name;
                if (_currentCharacterHp[index3] == ParameterStatus.Empty)
                {
                    if (_cursorType == CursorGroup.Individual)
                    {
                        if (_targetDead == false)
                        {
                            ButtonGroupState.SetButtonEnable(go, false);
                            if (go == obj)
                            {
                                Int32 firstPlayer = GetFirstPlayer();
                                if (firstPlayer != -1)
                                {
                                    _currentTargetIndex = firstPlayer;
                                    obj = _targetHudList[firstPlayer].Self;
                                }
                                else
                                    Debug.LogError("NO player active !!");
                                Singleton<PointerManager>.Instance.RemovePointerFromGameObject(go);
                            }
                        }
                        else
                            ButtonGroupState.SetButtonEnable(go, true);
                    }
                    uiLabel.color = FF9TextTool.Red;
                }
                else if (_currentCharacterHp[index3] == ParameterStatus.Critical)
                {
                    if (_cursorType == CursorGroup.Individual)
                        ButtonGroupState.SetButtonEnable(go, true);
                    uiLabel.color = FF9TextTool.Yellow;
                }
                else
                {
                    if (_cursorType == CursorGroup.Individual)
                        ButtonGroupState.SetButtonEnable(go, true);
                    uiLabel.color = FF9TextTool.White;
                }
                ++index3;
            }
            else
            {
                TargetHUD targetHud = _targetHudList[index4];
                GameObject go = targetHud.Self;
                UILabel uiLabel = targetHud.NameLabel;
                Single additionalWidth = 0.0f;
                go.SetActive(true);
                uiLabel.text = uiLabel.PhrasePreOpcodeSymbol(btl_util.getEnemyPtr(btl).et.name, ref additionalWidth);
                if (_currentEnemyDieState[index4 - 4])
                {
                    if (_cursorType == CursorGroup.Individual)
                    {
                        ButtonGroupState.SetButtonEnable(go, false);
                        if (_targetDead == false)
                        {
                            if (go == obj)
                            {
                                Int32 index5 = GetFirstEnemy() + HonoluluBattleMain.EnemyStartIndex;
                                if (index5 != -1)
                                {
                                    if (_currentCommandIndex == CommandMenu.Attack && FF9StateSystem.PCPlatform && _enemyCount > 1)
                                    {
                                        Int32 num5 = _currentTargetIndex != index5 ? index5 : index5 + 1;
                                        Int32 firstIndex = num5 >= _targetHudList.Count ? index5 : num5;
                                        ValidateDefaultTarget(ref firstIndex);
                                        index5 = firstIndex;
                                    }
                                    _currentTargetIndex = index5;
                                    obj = _targetHudList[index5].Self;
                                }
                                else
                                    Debug.LogError("NO enemy active !!");
                                Singleton<PointerManager>.Instance.RemovePointerFromGameObject(go);
                            }
                        }
                        else
                            ButtonGroupState.SetButtonEnable(go, true);
                    }
                    uiLabel.color = FF9TextTool.Gray;
                }
                else
                {
                    if (_cursorType == CursorGroup.Individual)
                        ButtonGroupState.SetButtonEnable(go, true);
                    uiLabel.color = FF9TextTool.White;
                }
                ++index4;
            }
        }

        if ((enCount != num3 || plCount != num4) && ButtonGroupState.ActiveGroup == TargetGroupButton)
        {
            SetTargetDefault();
            modelButtonManager.Reset();
            EnableTargetArea();
            SetTargetHelp();
            ButtonGroupState.DisableAllGroup(true);
            ButtonGroupState.ActiveGroup = TargetGroupButton;
        }

        if (obj != null && _cursorType == CursorGroup.Individual && obj.activeSelf)
            ButtonGroupState.ActiveButton = obj;
        else
            DisplayTargetPointer();
    }

    private void DisplayCharacterParameter(PlayerDetailHUD playerHud, BTL_DATA bd, InfoVal hp, InfoVal mp)
    {
        playerHud.NameLabel.text = btl_util.getPlayerPtr(bd).name;
        playerHud.HPLabel.text = hp.disp_val.ToString();
        playerHud.MPLabel.text = mp.disp_val.ToString();
        ParameterStatus parameterStatus = CheckHPState(bd);

        switch (parameterStatus)
        {
            case ParameterStatus.Empty:
                playerHud.ATBSlider.value = 0.0f;
                playerHud.HPLabel.color = FF9TextTool.Red;
                playerHud.NameLabel.color = FF9TextTool.Red;
                playerHud.ATBBlink = false;
                playerHud.TranceBlink = false;
                break;
            case ParameterStatus.Critical:
                playerHud.ATBSlider.value = bd.cur.at / (Single)bd.max.at;
                playerHud.HPLabel.color = FF9TextTool.Yellow;
                playerHud.NameLabel.color = FF9TextTool.Yellow;
                break;
            default:
                playerHud.ATBSlider.value = bd.cur.at / (Single)bd.max.at;
                playerHud.HPLabel.color = FF9TextTool.White;
                playerHud.NameLabel.color = FF9TextTool.White;
                break;
        }

        playerHud.MPLabel.color = CheckMPState(bd) == ParameterStatus.Critical ? FF9TextTool.Yellow : FF9TextTool.White;
        String str = ATENormal;

        if (btl_stat.CheckStatus(bd, 1052672U))
            str = ATEGray;
        else if (btl_stat.CheckStatus(bd, 524288U))
            str = ATEOrange;

        playerHud.ATBForegroundSprite.spriteName = str;
        if (bd.bi.t_gauge == 0)
            return;

        playerHud.TranceSlider.value = bd.trance / 256f;
        if (parameterStatus == ParameterStatus.Empty)
            return;

        if (bd.trance == Byte.MaxValue && !playerHud.TranceBlink)
        {
            playerHud.TranceBlink = true;
            if (_currentTrancePlayer.Contains(bd.bi.line_no))
                return;
            _currentTrancePlayer.Add(bd.bi.line_no);
            //currentTranceTrigger = true;
        }
        else
        {
            if (bd.trance == Byte.MaxValue)
                return;
            playerHud.TranceBlink = false;
            if (!_currentTrancePlayer.Contains(bd.bi.line_no))
                return;
            _currentTrancePlayer.Remove(bd.bi.line_no);
            //currentTranceTrigger = true;
        }
    }

    public void AddPlayerToReady(Int32 playerId)
    {
        if (_unconsciousStateList.Contains(playerId))
            return;

        ReadyQueue.Add(playerId);
        _partyDetail.List.First(h => h.PlayerId == playerId).ATBBlink = true;
    }

    public void RemovePlayerFromAction(Int32 btlId, Boolean isNeedToClearCommand)
    {
        Int32 num = 0;
        while (1 << num != btlId)
            ++num;

        if (InputFinishList.Contains(num) && isNeedToClearCommand)
            InputFinishList.Remove(num);

        if (!ReadyQueue.Contains(num) || !isNeedToClearCommand)
            return;

        ReadyQueue.Remove(num);
    }

    private void ManageAbility()
    {
        _abilityDetailDict.Clear();

        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            Int32 index = 0;
            while (1 << index != btlData.btl_id)
                ++index;

            if (btlData.bi.player == 0)
                continue;

            AbilityPlayerDetail abilityPlayer = new AbilityPlayerDetail { Player = FF9StateSystem.Common.FF9.player[btlData.bi.slot_no] };
            abilityPlayer.HasAp = ff9abil.FF9Abil_HasAp(abilityPlayer.Player);
            SetAbilityAp(abilityPlayer);
            SetAbilityEquip(abilityPlayer);
            SetAbilityTrance(abilityPlayer);
            SetAbilityMagic(abilityPlayer);
            _abilityDetailDict[index] = abilityPlayer;
        }
    }

    private static ParameterStatus CheckHPState(BTL_DATA bd)
    {
        if (bd.cur.hp == 0)
            return ParameterStatus.Empty;

        return bd.cur.hp <= bd.max.hp / 6.0 && bd.bi.player != 0 ? ParameterStatus.Critical : ParameterStatus.Normal;
    }

    private static ParameterStatus CheckMPState(BTL_DATA bd)
    {
        return bd.cur.mp <= bd.max.mp / 6.0 ? ParameterStatus.Critical : ParameterStatus.Normal;
    }

    private void CheckDoubleCast(Int32 battleIndex, CursorGroup cursorGroup)
    {
        if (IsDoubleCast && _doubleCastCount == 2 || !IsDoubleCast)
        {
            _doubleCastCount = 0;
            SetTarget(battleIndex);
        }
        else
        {
            if (!IsDoubleCast || _doubleCastCount >= 2)
                return;

            ++_doubleCastCount;
            _firstCommand = ProcessCommand(battleIndex, cursorGroup);
            _subMenuType = SubMenuType.Ability;

            DisplayAbility();
            SetTargetVisibility(false);
            SetAbilityPanelVisibility(true, true);
            BackButton.SetActive(FF9StateSystem.MobilePlatform);
        }
    }

    private void CheckPlayerState()
    {
        for (BTL_DATA cur = FF9StateSystem.Battle.FF9Battle.btl_list.next; cur != null; cur = cur.next)
        {
            Int32 num = 0;
            while (1 << num != cur.btl_id)
                ++num;

            if (cur.bi.player == 0)
                continue;

            if (!IsEnableInput(cur))
            {
                if (!_unconsciousStateList.Contains(num))
                    _unconsciousStateList.Add(num);
            }
            else if (_unconsciousStateList.Contains(num))
            {
                _unconsciousStateList.Remove(num);
            }
        }
    }

    public void ActivateTurnForPlayer(Int32 playerId)
    {
        PlayerDetailHUD playerDetailHud = _partyDetail.List.Find(h => h.PlayerId == playerId);
        playerDetailHud.Component.UIBoxCollider.enabled = false;
        playerDetailHud.Component.ButtonColor.SetState(UIButtonColor.State.Pressed, false);
        DisplayCommand();
        SetCommandVisibility(true, false);
    }

    private void SwitchPlayer(Int32 playerId)
    {
        SetIdle();
        FF9Sfx.FF9SFX_Play(1044);
        CurrentPlayerIndex = playerId;
        ActivateTurnForPlayer(playerId);
    }

    private void UpdatePlayer()
    {
        _blinkAlphaCounter += RealTime.deltaTime * 3f;
        _blinkAlphaCounter = _blinkAlphaCounter <= 2.0 ? _blinkAlphaCounter : 0.0f;
        Single num = _blinkAlphaCounter > 1.0 ? 2f - _blinkAlphaCounter : _blinkAlphaCounter;
        if (!_commandEnable)
            return;

        foreach (PlayerDetailHUD current in _partyDetail.List)
        {
            if (current.PlayerId == -1)
                continue;

            BTL_DATA cur = FF9StateSystem.Battle.FF9Battle.btl_data[current.PlayerId];
            if ((Status.checkCurStat(cur, 1024u) || Status.checkCurStat(cur, 2048u)) && current.ATBBlink)
            {
                current.ATBBlink = false;
            }
            if (IsEnableInput(cur) && !_isAutoAttack)
            {
                if (current.ATBBlink)
                    current.ATBForegroundWidget.alpha = num;
                if (current.TranceBlink && cur.bi.t_gauge != 0)
                    current.TranceForegroundWidget.alpha = num;
            }
            else
            {
                if (current.ATBBlink)
                {
                    current.ATBForegroundWidget.alpha = 1f;
                    current.ATBHighlightSprite.alpha = 0.0f;
                }
                if (current.TranceBlink && cur.bi.t_gauge != 0)
                {
                    current.TranceForegroundWidget.alpha = 1f;
                    current.TranceHighlightSprite.alpha = 0.0f;
                }
            }
        }

        YMenu_ManagerHpMp();
        CheckPlayerState();
        DisplayPartyRealtime();

        if (TargetPanel.activeSelf)
        {
            DisplayTarget();
            _status.DisplayStatusRealtime(_matchBattleIdPlayerList);
        }

        ManagerTarget();
        ManagerInfo();

        if (CurrentPlayerIndex > -1)
        {
            BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex];
            if (ButtonGroupState.ActiveGroup == CommandGroupButton && _isTranceMenu)
            {
                _tranceColorCounter = (_tranceColorCounter + 1) % _tranceTextColor.Length;
                _commandDetailHUD.CaptionLabel.color = _tranceTextColor[_tranceColorCounter];
            }

            if (!IsEnableInput(btlData))
            {
                SetIdle();
                return;
            }

            if (TargetPanel.activeSelf)
            {
                if (!ManageTargetCommand())
                    return;
            }
            else if (AbilityPanel.activeSelf || ItemPanel.activeSelf)
            {
                if (AbilityPanel.activeSelf)
                    DisplayAbilityRealTime();

                if (ItemPanel.activeSelf)
                    DisplayItemRealTime();

                if ((Int32)_currentCommandId == 31 && (!_magicSwordCond.IsViviExist || _magicSwordCond.IsViviDead || _magicSwordCond.IsSteinerMini))
                {
                    FF9Sfx.FF9SFX_Play(101);
                    ResetToReady();
                    return;
                }

                if (!_isTranceMenu && btl_stat.CheckStatus(btlData, 16384U))
                {
                    FF9Sfx.FF9SFX_Play(101);
                    ResetToReady();
                    return;
                }
            }
        }

        if (ReadyQueue.Count <= 0 || CurrentPlayerIndex != -1)
            return;

        for (Int32 index = ReadyQueue.Count - 1; index >= 0; --index)
        {
            if (!_unconsciousStateList.Contains(ReadyQueue[index]))
                continue;

            BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_data[ReadyQueue[index]];
            RemovePlayerFromAction(btl.btl_id, btl_stat.CheckStatus(btl, 134403U));
        }

        using (List<Int32>.Enumerator enumerator = ReadyQueue.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                Int32 current = enumerator.Current;
                if (InputFinishList.Contains(current) || _unconsciousStateList.Contains(current))
                    continue;

                if (_isAutoAttack)
                {
                    SendAutoAttackCommand(current);
                    break;
                }

                SwitchPlayer(current);
                break;
            }
        }
    }

    private AbilityStatus CheckAbilityStatus(Int32 subMenuIndex)
    {
        CharacterCommand command = CharacterCommands.Commands[_currentCommandId];
        if (subMenuIndex >= command.Abilities.Length)
            return AbilityStatus.None;

        Int32 abilityId = command.Abilities[subMenuIndex];
        return GetAbilityState(abilityId);
    }

    public void YMenu_ManagerHpMp()
    {
        BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next;
        Int32 index1 = 0;
        for (; btlData != null; btlData = btlData.next)
        {
            if (btlData.bi.player == 0)
                continue;

            InfoVal infoVal1 = _hpInfoVal[index1];
            InfoVal infoVal2 = _mpInfoVal[index1];
            for (Int32 index2 = 0; index2 < 2; ++index2)
            {
                InfoVal infoVal3 = index2 != 0 ? infoVal2 : infoVal1;
                if (infoVal3.anim_frm != 0)
                {
                    if (0 <= infoVal3.inc_val)
                    {
                        if (infoVal3.disp_val + infoVal3.inc_val >= infoVal3.req_val)
                        {
                            infoVal3.disp_val = infoVal3.req_val;
                            infoVal3.anim_frm = 0;
                        }
                        else
                        {
                            infoVal3.disp_val += infoVal3.inc_val;
                            --infoVal3.anim_frm;
                        }
                    }
                    else if (infoVal3.disp_val + infoVal3.inc_val <= infoVal3.req_val)
                    {
                        infoVal3.disp_val = infoVal3.req_val;
                        infoVal3.anim_frm = 0;
                    }
                    else
                    {
                        infoVal3.disp_val += infoVal3.inc_val;
                        --infoVal3.anim_frm;
                    }
                }
                else
                {
                    Int32 num1 = index2 != 0 ? btlData.cur.mp : (Int16)btlData.cur.hp;
                    Int32 num2 = index2 != 0 ? btlData.max.mp : (Int16)btlData.max.hp;
                    Int32 num3;
                    if ((num3 = num1 - infoVal3.disp_val) == 0)
                        continue;

                    Int32 num4 = Mathf.Abs(num3);
                    infoVal3.req_val = (Int16)num1;
                    if (num4 < YINFO_ANIM_HPMP_MIN)
                    {
                        infoVal3.anim_frm = num4;
                    }
                    else
                    {
                        infoVal3.anim_frm = num4 * YINFO_ANIM_HPMP_MAX / num2;
                        if (YINFO_ANIM_HPMP_MIN > infoVal3.anim_frm)
                            infoVal3.anim_frm = YINFO_ANIM_HPMP_MIN;
                    }
                    infoVal3.inc_val = 0 > num3 ? (num3 - (infoVal3.anim_frm - 1)) / infoVal3.anim_frm : (num3 + (infoVal3.anim_frm - 1)) / infoVal3.anim_frm;
                }
            }
            ++index1;
        }
    }

    private void ManagerInfo()
    {
        BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next;
        MagicSwordCondition magicSwordCondition1 = new MagicSwordCondition();
        MagicSwordCondition magicSwordCondition2 = new MagicSwordCondition();
        magicSwordCondition1.IsViviExist = _magicSwordCond.IsViviExist;
        magicSwordCondition1.IsViviDead = _magicSwordCond.IsViviDead;
        magicSwordCondition1.IsSteinerMini = _magicSwordCond.IsSteinerMini;
        for (; btl != null && btl.bi.player != 0; btl = btl.next)
        {
            if (btl.bi.slot_no == 1)
            {
                magicSwordCondition2.IsViviExist = true;
                if (btl.cur.hp == 0)
                    magicSwordCondition2.IsViviDead = true;
                else if (btl_stat.CheckStatus(btl, 318905611U))
                    magicSwordCondition2.IsViviDead = true;
            }
            else if (btl.bi.slot_no == 3)
            {
                magicSwordCondition2.IsSteinerMini = btl_stat.CheckStatus(btl, 268435456U);
            }
        }
        if (magicSwordCondition1.Changed(magicSwordCondition2))
        {
            _magicSwordCond.IsViviExist = magicSwordCondition2.IsViviExist;
            _magicSwordCond.IsViviDead = magicSwordCondition2.IsViviDead;
            _magicSwordCond.IsSteinerMini = magicSwordCondition2.IsSteinerMini;
            if (CurrentPlayerIndex == -1)
                return;

            DisplayCommand();
        }
        else
        {
            if (_isTranceMenu || CurrentPlayerIndex == -1 || !btl_stat.CheckStatus(FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex], 16384U))
                return;

            DisplayCommand();
        }
    }

    private Boolean ManageTargetCommand()
    {
        BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex];
        if ((Int32)_currentCommandId == 31 && (!_magicSwordCond.IsViviExist || _magicSwordCond.IsViviDead || _magicSwordCond.IsSteinerMini))
        {
            FF9Sfx.FF9SFX_Play(101);
            ResetToReady();
            return false;
        }

        if (!_isTranceMenu && btl_stat.CheckStatus(btl, 16384U))
        {
            FF9Sfx.FF9SFX_Play(101);
            ResetToReady();
            return false;
        }

        if (_subMenuType == SubMenuType.Ability)
        {
            CharacterCommand command = CharacterCommands.Commands[_currentCommandId];
            if (_currentSubMenuIndex >= command.Abilities.Length)
                return true;

            AA_DATA aaData = FF9StateSystem.Battle.FF9Battle.aa_data[PatchAbility(command.Abilities[_currentSubMenuIndex])];
            Int32 num2 = ff9abil.FF9Abil_GetEnableSA(btl.bi.slot_no, AbilSaMpHalf) ? aaData.MP >> 1 : aaData.MP;

            if (btl.cur.mp < num2)
            {
                FF9Sfx.FF9SFX_Play(101);
                DisplayAbility();
                SetTargetVisibility(false);
                ClearModelPointer();
                SetAbilityPanelVisibility(true, true);
                return false;
            }

            if ((aaData.Category & 2) != 0 && btl_stat.CheckStatus(btl, 8U))
            {
                FF9Sfx.FF9SFX_Play(101);
                DisplayAbility();
                SetTargetVisibility(false);
                ClearModelPointer();
                SetAbilityPanelVisibility(true, true);
                return false;
            }
        }

        if ((_subMenuType == SubMenuType.Item || _subMenuType == SubMenuType.Throw) && ff9item.FF9Item_GetCount(_itemIdList[_currentSubMenuIndex]) == 0)
        {
            FF9Sfx.FF9SFX_Play(101);
            DisplayItem(_subMenuType == SubMenuType.Throw);
            SetTargetVisibility(false);
            ClearModelPointer();
            SetItemPanelVisibility(true, true);
            return false;
        }

        return true;
    }

    private static void ManagerTarget()
    {
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            if (btlData.tar_mode < 2)
                continue;

            if (btlData.tar_mode == 2)
            {
                btlData.bi.target = btlData.bi.atb = 0;
                btlData.tar_mode = 0;
            }
            else if (btlData.tar_mode == 3)
            {
                btlData.bi.target = btlData.bi.atb = 1;
                btlData.tar_mode = 1;
            }
        }
    }

    private void InitHpMp()
    {
        _hpInfoVal.Clear();
        _mpInfoVal.Clear();
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            InfoVal infoVal1 = new InfoVal();
            InfoVal infoVal2 = new InfoVal();
            infoVal1.req_val = infoVal1.disp_val = (Int16)btlData.cur.hp;
            infoVal2.req_val = infoVal2.disp_val = btlData.cur.mp;
            infoVal1.anim_frm = infoVal2.anim_frm = 0;
            infoVal1.inc_val = infoVal2.inc_val = 0;
            _hpInfoVal.Add(infoVal1);
            _mpInfoVal.Add(infoVal2);
        }
    }

    private Int32 GetMp(AA_DATA aaData)
    {
        Int32 num = aaData.MP;
        Int32 slotId = FF9StateSystem.Common.FF9.party.member[FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].bi.line_no].info.slot_no;
        if ((aaData.Type & 4) != 0 && FF9StateSystem.EventState.gEventGlobal[18] != 0)
            num <<= 2;

        if (ff9abil.FF9Abil_GetEnableSA(slotId, AbilSaMpHalf))
            num >>= 1;

        return num;
    }

    private AbilityStatus GetAbilityState(Int32 abilId)
    {
        AbilityPlayerDetail abilityPlayerDetail = _abilityDetailDict[CurrentPlayerIndex];
        BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex];
        AA_DATA aaData = FF9StateSystem.Battle.FF9Battle.aa_data[abilId];

        if (abilityPlayerDetail.HasAp && !abilityPlayerDetail.AbilityEquipList.ContainsKey(abilId) && (!abilityPlayerDetail.AbilityPaList.ContainsKey(abilId) || abilityPlayerDetail.AbilityPaList[abilId] < abilityPlayerDetail.AbilityMaxPaList[abilId]))
            return AbilityStatus.None;

        return (aaData.Category & 2) != 0 && (btl_stat.CheckStatus(btl, 8U) || FF9StateSystem.Battle.FF9Battle.btl_scene.Info.NoMagical != 0) || GetMp(aaData) > btl.cur.mp ? AbilityStatus.Disable : AbilityStatus.Enable;
    }

    private void SetAbilityAp(AbilityPlayerDetail abilityPlayer)
    {
        PLAYER player = abilityPlayer.Player;
        if (!abilityPlayer.HasAp)
            return;

        CharacterAbility[] paDataArray = ff9abil._FF9Abil_PaData[player.info.menu_type];
        for (Int32 abilId = 0; abilId < 192; ++abilId)
        {
            Int32 index;
            if ((index = ff9abil.FF9Abil_GetIndex(player.info.slot_no, abilId)) < 0)
                continue;

            abilityPlayer.AbilityPaList[abilId] = player.pa[index];
            abilityPlayer.AbilityMaxPaList[abilId] = paDataArray[index].Ap;
        }
    }

    private static void SetAbilityEquip(AbilityPlayerDetail abilityPlayer)
    {
        PLAYER player = abilityPlayer.Player;
        for (Int32 index1 = 0; index1 < 5; ++index1)
        {
            Int32 index2 = player.equip[index1];
            if (index2 == Byte.MaxValue)
                continue;

            FF9ITEM_DATA ff9ItemData = ff9item._FF9Item_Data[index2];
            for (Int32 index3 = 0; index3 < 3; ++index3)
            {
                Int32 index4 = ff9ItemData.ability[index3];
                if (index4 != 0 && 192 > index4)
                    abilityPlayer.AbilityEquipList[index4] = true;
            }
        }
    }

    private static void SetAbilityTrance(AbilityPlayerDetail abilityPlayer)
    {
        PLAYER play = abilityPlayer.Player;
        Int32 presetId = play.info.menu_type;
        if (!ff9abil.FF9Abil_HasAp(play))
            return;

        for (Int32 k = 0; k < 2; k++)
        {
            command_tags normalCommandId = CharacterCommands.CommandSets[presetId].GetRegular(k);
            command_tags tranceCommandId = CharacterCommands.CommandSets[presetId].GetTrance(k);
            if (normalCommandId == tranceCommandId)
                continue;

            CharacterCommand normalCommand = CharacterCommands.Commands[(Byte)normalCommandId];
            CharacterCommand tranceCommand = CharacterCommands.Commands[(Byte)tranceCommandId];
            Int32 count = Math.Min(normalCommand.Abilities.Length, tranceCommand.Abilities.Length);
            for (Int32 i = 0; i < count; ++i)
            {
                Int32 normalId = normalCommand.Abilities[i];
                Int32 tranceId = tranceCommand.Abilities[i];
                if (normalId == tranceId)
                    continue;

                if (abilityPlayer.AbilityPaList.ContainsKey(normalId))
                {
                    abilityPlayer.AbilityPaList[tranceId] = abilityPlayer.AbilityPaList[normalId];
                    abilityPlayer.AbilityMaxPaList[tranceId] = abilityPlayer.AbilityMaxPaList[normalId];
                }

                if (abilityPlayer.AbilityEquipList.ContainsKey(normalId))
                    abilityPlayer.AbilityEquipList[tranceId] = abilityPlayer.AbilityEquipList[normalId];
            }
        }
    }

    private static void SetAbilityMagic(AbilityPlayerDetail abilityPlayer)
    {
        PLAYER character = abilityPlayer.Player;
        if (character.info.slot_no != (Int32)CharacterPresetId.Steiner)
            return;

        CharacterCommand magicSwordCommand = CharacterCommands.Commands[(Int32)command_tags.CMD_MAGIC_SWORD];
        PLAYER player2 = FF9StateSystem.Common.FF9.player[(Int32)CharacterPresetId.Vivi];
        CharacterAbility[] paDataArray = ff9abil._FF9Abil_PaData[(Int32)CharacterPresetId.Vivi];
        Int32[] abilities = { 25, 26, 27, 29, 30, 31, 33, 34, 35, 38, 45, 47, 48 }; // TODO: Move to the resource file

        Int32 count = Math.Min(magicSwordCommand.Abilities.Length, abilities.Length);
        for (Int32 i = 0; i < count; ++i)
        {
            Int32 abilityId = magicSwordCommand.Abilities[i];
            Int32 index = ff9abil.FF9Abil_GetIndex(1, abilities[i]);
            if (index > -1)
            {
                abilityPlayer.AbilityPaList[abilityId] = player2.pa[index];
                abilityPlayer.AbilityMaxPaList[abilityId] = paDataArray[index].Ap;
            }
        }

        for (Int32 equipSlot = 0; equipSlot < 5; ++equipSlot)
        {
            Int32 equipId = player2.equip[equipSlot];
            if (equipId == Byte.MaxValue)
                continue;

            FF9ITEM_DATA ff9ItemData = ff9item._FF9Item_Data[equipId];
            for (Int32 equipAbilitySlot = 0; equipAbilitySlot < 3; ++equipAbilitySlot)
            {
                Int32 equipAbilityId = ff9ItemData.ability[equipAbilitySlot];
                if (equipAbilityId == 0 || equipAbilityId >= 192)
                    continue;

                for (Int32 i = 0; i < count; ++i)
                {
                    if (equipAbilityId != abilities[i])
                        continue;

                    Int32 abilityId = magicSwordCommand.Abilities[i];
                    abilityPlayer.AbilityEquipList[abilityId] = true;
                }
            }
        }
    }

    public override void Show(SceneVoidDelegate afterFinished = null)
    {
        SceneVoidDelegate action = GeneratedShow;
        if (afterFinished != null)
            action = (SceneVoidDelegate)Delegate.Combine(action, afterFinished);

        if (!_isFromPause)
        {
            base.Show(action);
            PersistenSingleton<UIManager>.Instance.Booster.SetBoosterState(PersistenSingleton<UIManager>.Instance.UnityScene);
            FF9StateSystem.Settings.SetMasterSkill();
            AllMenuPanel.SetActive(false);
        }
        else
        {
            _commandEnable = _beforePauseCommandEnable;
            _isTryingToRun = false;
            Singleton<HUDMessage>.Instance.Pause(false);
            base.Show(action);
            if (_commandEnable && !_hidingHud)
            {
                FF9BMenu_EnableMenu(true);
                ButtonGroupState.ActiveGroup = _currentButtonGroup;
                DisplayTargetPointer();
            }
        }
        _isFromPause = false;
        _oneTime = true;
    }

    public override void Hide(SceneVoidDelegate afterFinished = null)
    {
        base.Hide(afterFinished);
        PauseButtonGameObject.SetActive(false);
        HelpButtonGameObject.SetActive(false);
        if (_isFromPause)
            return;

        RemoveCursorMemorize();
    }

    private void RemoveCursorMemorize()
    {
        _commandCursorMemorize.Clear();
        _ability1CursorMemorize.Clear();
        _ability2CursorMemorize.Clear();
        _itemCursorMemorize.Clear();

        ButtonGroupState.RemoveCursorMemorize(CommandGroupButton);
        ButtonGroupState.RemoveCursorMemorize(ItemGroupButton);
        ButtonGroupState.RemoveCursorMemorize(AbilityGroupButton);
        ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
    }

    public override Boolean OnKeyConfirm(GameObject go)
    {
        if (!base.OnKeyConfirm(go) || _hidingHud)
            return true;

        if (ButtonGroupState.ActiveGroup == CommandGroupButton)
        {
            FF9Sfx.FF9SFX_Play(103);
            _currentCommandIndex = (CommandMenu)go.transform.GetSiblingIndex();
            _currentCommandId = (UInt32)GetCommandFromCommandIndex(_currentCommandIndex, CurrentPlayerIndex);
            _commandCursorMemorize[CurrentPlayerIndex] = _currentCommandIndex;
            _subMenuType = SubMenuType.Normal;
            if (IsDoubleCast && _doubleCastCount < 2)
                ++_doubleCastCount;

            switch (_currentCommandIndex)
            {
                case CommandMenu.Attack:
                    SetCommandVisibility(false, false);
                    SetTargetVisibility(true);
                    break;
                case CommandMenu.Defend:
                    _targetCursor = 0;
                    SendCommand(ProcessCommand(CurrentPlayerIndex, CursorGroup.Individual));
                    SetIdle();
                    break;
                case CommandMenu.Ability1:
                case CommandMenu.Ability2:
                    //int num = currentCommandIndex != CommandMenu.Ability2 ? 0 : 1;
                    CharacterCommand ff9Command = CharacterCommands.Commands[_currentCommandId];
                    if (ff9Command.Type == CharacterCommandType.Normal)
                    {
                        _subMenuType = SubMenuType.Normal;
                        SetCommandVisibility(false, false);
                        SetTargetVisibility(true);
                        break;
                    }
                    if (ff9Command.Type == CharacterCommandType.Ability)
                    {
                        _subMenuType = SubMenuType.Ability;
                        DisplayAbility();
                        SetCommandVisibility(false, false);
                        SetAbilityPanelVisibility(true, false);
                        break;
                    }
                    if (ff9Command.Type == CharacterCommandType.Throw)
                    {
                        _subMenuType = SubMenuType.Throw;
                        DisplayItem(true);
                        SetCommandVisibility(false, false);
                        SetItemPanelVisibility(true, false);
                    }
                    break;
                case CommandMenu.Item:
                    DisplayItem(false);
                    SetCommandVisibility(false, false);
                    SetItemPanelVisibility(true, false);
                    break;
                case CommandMenu.Change:
                    _targetCursor = 0;
                    SendCommand(ProcessCommand(CurrentPlayerIndex, CursorGroup.Individual));
                    SetIdle();
                    break;
            }
        }
        else if (ButtonGroupState.ActiveGroup == TargetGroupButton)
        {
            FF9Sfx.FF9SFX_Play(103);
            if (_cursorType == CursorGroup.Individual)
            {
                Int32 index = _targetHudList.IndexOf(_targetHudList.Single(h => h.Self == go));
                if (index < HonoluluBattleMain.EnemyStartIndex)
                {
                    if (index < _matchBattleIdPlayerList.Count)
                        CheckDoubleCast(_matchBattleIdPlayerList[index], _cursorType);
                }
                else if (index - HonoluluBattleMain.EnemyStartIndex < _matchBattleIdEnemyList.Count)
                    CheckDoubleCast(_matchBattleIdEnemyList[index - HonoluluBattleMain.EnemyStartIndex], _cursorType);
            }
            else if (_cursorType == CursorGroup.AllPlayer || _cursorType == CursorGroup.AllEnemy || _cursorType == CursorGroup.All)
                CheckDoubleCast(-1, _cursorType);
        }
        else if (ButtonGroupState.ActiveGroup == AbilityGroupButton)
        {
            if (CheckAbilityStatus(go.GetComponent<RecycleListItem>().ItemDataIndex) == AbilityStatus.Enable)
            {
                FF9Sfx.FF9SFX_Play(103);
                _currentSubMenuIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                if (_currentCommandIndex == CommandMenu.Ability1)
                    _ability1CursorMemorize[CurrentPlayerIndex] = _currentSubMenuIndex;
                else
                    _ability2CursorMemorize[CurrentPlayerIndex] = _currentSubMenuIndex;

                SetAbilityPanelVisibility(false, false);
                SetTargetVisibility(true);
            }
            else
                FF9Sfx.FF9SFX_Play(102);
        }
        else if (ButtonGroupState.ActiveGroup == ItemGroupButton)
        {
            if (_itemIdList[_currentSubMenuIndex] != Byte.MaxValue)
            {
                FF9Sfx.FF9SFX_Play(103);
                _currentSubMenuIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
                _itemCursorMemorize[CurrentPlayerIndex] = _currentSubMenuIndex;
                SetItemPanelVisibility(false, false);
                SetTargetVisibility(true);
            }
            else
                FF9Sfx.FF9SFX_Play(102);
        }
        return true;
    }

    public override Boolean OnKeyCancel(GameObject go)
    {
        if (UIManager.Input.GetKey(Control.Special))
        {
            return true;
        }

        if (base.OnKeyCancel(go) && !_hidingHud && ButtonGroupState.ActiveGroup != CommandGroupButton)
        {
            if (ButtonGroupState.ActiveGroup == TargetGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                SetTargetVisibility(false);
                ClearModelPointer();
                switch (_currentCommandIndex)
                {
                    case CommandMenu.Attack:
                        SetCommandVisibility(true, true);
                        break;
                    case CommandMenu.Ability1:
                    case CommandMenu.Ability2:
                        if (_subMenuType == SubMenuType.Ability)
                        {
                            SetAbilityPanelVisibility(true, true);
                            break;
                        }
                        if (_subMenuType == SubMenuType.Throw)
                        {
                            SetItemPanelVisibility(true, true);
                            break;
                        }
                        SetCommandVisibility(true, true);
                        break;
                    case CommandMenu.Item:
                        SetItemPanelVisibility(true, true);
                        break;
                }
            }
            else if (ButtonGroupState.ActiveGroup == AbilityGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                if (IsDoubleCast && _doubleCastCount > 0)
                    --_doubleCastCount;
                if (_doubleCastCount == 0)
                {
                    SetAbilityPanelVisibility(false, false);
                    SetCommandVisibility(true, true);
                }
                else
                    SetAbilityPanelVisibility(true, false);
            }
            else if (ButtonGroupState.ActiveGroup == ItemGroupButton)
            {
                FF9Sfx.FF9SFX_Play(101);
                SetItemPanelVisibility(false, false);
                SetCommandVisibility(true, true);
            }
            else if (ButtonGroupState.ActiveGroup == String.Empty && UIManager.Input.ContainsAndroidQuitKey())
                OnKeyQuit();
        }
        return true;
    }

    public override Boolean OnKeyMenu(GameObject go)
    {
        if (base.OnKeyMenu(go) && !_hidingHud && ButtonGroupState.ActiveGroup == CommandGroupButton)
        {
            if (ReadyQueue.Count > 1)
            {
                Int32 num = ReadyQueue[0];
                ReadyQueue.RemoveAt(0);
                ReadyQueue.Add(num);
                using (List<Int32>.Enumerator enumerator = ReadyQueue.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Int32 current = enumerator.Current;
                        if (!InputFinishList.Contains(current) && !_unconsciousStateList.Contains(current) && current != CurrentPlayerIndex)
                        {
                            if (ReadyQueue.IndexOf(current) > 0)
                            {
                                ReadyQueue.Remove(current);
                                ReadyQueue.Insert(0, current);
                            }
                            SwitchPlayer(current);
                            break;
                        }
                    }
                }
            }
            else if (ReadyQueue.Count == 1)
                SwitchPlayer(ReadyQueue[0]);
        }
        return true;
    }

    public override Boolean OnKeyPause(GameObject go)
    {
        if (base.OnKeyPause(go) && FF9StateSystem.Battle.FF9Battle.btl_seq != 2 && FF9StateSystem.Battle.FF9Battle.btl_seq != 1)
        {
            NextSceneIsModal = true;
            _isFromPause = true;
            _beforePauseCommandEnable = _commandEnable;
            _currentButtonGroup = !_hidingHud ? ButtonGroupState.ActiveGroup : _currentButtonGroup;
            FF9BMenu_EnableMenu(false);
            Singleton<HUDMessage>.Instance.Pause(true);
            Hide(() => PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Pause));
        }
        return base.OnKeyPause(go);
    }

    public override void OnKeyQuit()
    {
        if (Loading || FF9StateSystem.Battle.FF9Battle.btl_seq == 2 || FF9StateSystem.Battle.FF9Battle.btl_seq == 1)
            return;
        _beforePauseCommandEnable = _commandEnable;
        _currentButtonGroup = ButtonGroupState.ActiveGroup;
        FF9BMenu_EnableMenu(false);
        ShowQuitUI(_onResumeFromQuit);
    }

    public override Boolean OnKeyLeftBumper(GameObject go)
    {
        if (base.OnKeyLeftBumper(go) && !_hidingHud && ButtonGroupState.ActiveGroup == TargetGroupButton && (_targetCursor == TargetType.ManyAny || _targetCursor == TargetType.ManyEnemy || _targetCursor == TargetType.ManyAlly))
        {
            FF9Sfx.FF9SFX_Play(103);
            _isAllTarget = !_isAllTarget;
            _allTargetToggle.value = _isAllTarget;
            _allTargetButtonComponent.SetState(UIButtonColor.State.Normal, false);
            ToggleAllTarget();
        }
        return true;
    }

    public override Boolean OnKeyRightBumper(GameObject go)
    {
        if (base.OnKeyRightBumper(go) && !_hidingHud && ButtonGroupState.ActiveGroup == TargetGroupButton && (_targetCursor == TargetType.ManyAny || _targetCursor == TargetType.ManyEnemy || _targetCursor == TargetType.ManyAlly))
        {
            FF9Sfx.FF9SFX_Play(103);
            _isAllTarget = !_isAllTarget;
            _allTargetToggle.value = _isAllTarget;
            _allTargetButtonComponent.SetState(UIButtonColor.State.Normal, false);
            ToggleAllTarget();
        }
        return true;
    }

    public override Boolean OnKeyRightTrigger(GameObject go)
    {
        if (base.OnKeyRightTrigger(go) && !_hidingHud && !AndroidTVOnKeyRightTrigger(go))
            ProcessAutoBattleInput();
        return true;
    }

    public override Boolean OnItemSelect(GameObject go)
    {
        if (base.OnItemSelect(go))
        {
            if (ButtonGroupState.ActiveGroup == CommandGroupButton)
            {
                Int32 siblingIndex = go.transform.GetSiblingIndex();
                _currentCommandIndex = (CommandMenu)siblingIndex;
            }
            else if (ButtonGroupState.ActiveGroup == AbilityGroupButton || ButtonGroupState.ActiveGroup == ItemGroupButton)
                _currentSubMenuIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;
            if (ButtonGroupState.ActiveGroup == TargetGroupButton)
            {
                if (go.transform.parent == modelButtonManager.transform)
                {
                    if (_cursorType == CursorGroup.Individual)
                    {
                        Int32 num = go.GetComponent<ModelButton>().index;
                        Int32 index = num >= HonoluluBattleMain.EnemyStartIndex ? _matchBattleIdEnemyList.IndexOf(num) + 4 : _matchBattleIdPlayerList.IndexOf(num);
                        if (index != -1)
                        {
                            TargetHUD targetHud = _targetHudList[index];
                            if (targetHud.ButtonGroup.enabled)
                                ButtonGroupState.ActiveButton = targetHud.Self;
                        }
                    }
                }
                else if (go.transform.parent.parent == TargetPanel.transform && _cursorType == CursorGroup.Individual)
                {
                    Int32 siblingIndex = go.transform.GetSiblingIndex();
                    if (go.GetParent().transform.GetSiblingIndex() == 1)
                        siblingIndex += HonoluluBattleMain.EnemyStartIndex;
                    if (_currentTargetIndex != siblingIndex)
                    {
                        _currentTargetIndex = siblingIndex;
                        DisplayTargetPointer();
                    }
                }
            }
        }
        return true;
    }

    private void OnAllTargetHover(GameObject go, Boolean isHover)
    {
        if (!isHover || _cursorType != CursorGroup.AllEnemy && _cursorType != CursorGroup.AllPlayer)
            return;

        if (go == _targetPanel.AllPlayerButton)
        {
            if (_cursorType == CursorGroup.AllPlayer)
                return;
            FF9Sfx.FF9SFX_Play(103);
            _cursorType = CursorGroup.AllPlayer;
            DisplayTargetPointer();
        }
        else
        {
            if (!(go == _targetPanel.AllEnemyButton) || _cursorType == CursorGroup.AllEnemy)
                return;
            FF9Sfx.FF9SFX_Play(103);
            _cursorType = CursorGroup.AllEnemy;
            DisplayTargetPointer();
        }
    }

    private void OnTargetNavigate(GameObject go, KeyCode key)
    {
        if (_cursorType == CursorGroup.AllEnemy)
        {
            if (_targetCursor != TargetType.ManyAny || key != KeyCode.RightArrow)
                return;
            FF9Sfx.FF9SFX_Play(103);
            _cursorType = CursorGroup.AllPlayer;
            DisplayTargetPointer();
        }
        else
        {
            if (_cursorType != CursorGroup.AllPlayer || _targetCursor != TargetType.ManyAny || key != KeyCode.LeftArrow)
                return;
            FF9Sfx.FF9SFX_Play(103);
            _cursorType = CursorGroup.AllEnemy;
            DisplayTargetPointer();
        }
    }

    private void OnAllTargetClick(GameObject go)
    {
        if (_cursorType == CursorGroup.All)
        {
            FF9Sfx.FF9SFX_Play(103);
            CheckDoubleCast(-1, _cursorType);
        }
        else if (UICamera.currentTouchID == 0 || UICamera.currentTouchID == 1)
        {
            FF9Sfx.FF9SFX_Play(103);
            if (go == _targetPanel.AllPlayerButton)
            {
                if (_cursorType == CursorGroup.AllPlayer)
                    CheckDoubleCast(-1, _cursorType);
                else
                    OnTargetNavigate(go, KeyCode.RightArrow);
            }
            else
            {
                if (!(go == _targetPanel.AllEnemyButton))
                    return;

                if (_cursorType == CursorGroup.AllEnemy)
                    CheckDoubleCast(-1, _cursorType);
                else
                    OnTargetNavigate(go, KeyCode.LeftArrow);
            }
        }
        else
        {
            if (UICamera.currentTouchID != -1)
                return;

            FF9Sfx.FF9SFX_Play(103);
            if (go == _targetPanel.AllPlayerButton)
                _cursorType = CursorGroup.AllPlayer;
            else if (go == _targetPanel.AllEnemyButton)
                _cursorType = CursorGroup.AllEnemy;
            CheckDoubleCast(-1, _cursorType);
        }
    }



    private void OnRunPress(GameObject go, Boolean isDown)
    {
        _runCounter = 0.0f;
        _isTryingToRun = isDown;
    }

    private Boolean OnAllTargetToggleValidate(Boolean choice)
    {
        if (_isAllTarget != _allTargetToggle.value)
            return true;

        _allTargetButtonComponent.SetState(UIButtonColor.State.Normal, false);
        return false;
    }

    private Boolean OnAutoToggleValidate(Boolean choice)
    {
        if (_isAutoAttack != _autoBattleToggle.value)
            return true;

        _autoBattleButtonComponent.SetState(UIButtonColor.State.Normal, false);
        return false;
    }

    private void InitialBattle()
    {
        _currentCommandIndex = CommandMenu.Attack;
        _currentSubMenuIndex = 0;
        CurrentPlayerIndex = -1;
        _subMenuType = SubMenuType.Normal;
        _runCounter = 0.0f;
        _isTryingToRun = false;
        _unconsciousStateList.Clear();
        ReadyQueue.Clear();
        InputFinishList.Clear();
        _matchBattleIdPlayerList.Clear();
        _matchBattleIdEnemyList.Clear();
        _itemIdList.Clear();

        foreach (AbilityPlayerDetail value in _abilityDetailDict.Values)
            value.Clear();

        _currentCharacterHp.Clear();
        _currentTrancePlayer.Clear();
        _enemyCount = 0;
        _playerCount = 0;

        _partyDetail.SetBlink(false);

        AutoBattleHud.SetActive(_isAutoAttack);
        Singleton<HUDMessage>.Instance.WorldCamera = PersistenSingleton<UIManager>.Instance.BattleCamera;
        modelButtonManager.WorldCamera = PersistenSingleton<UIManager>.Instance.BattleCamera;
        ManageAbility();
        InitHpMp();
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            Int32 num = 0;
            while (1 << num != btlData.btl_id)
                ++num;
            if (btlData.bi.target != 0)
            {
                if (btlData.bi.player != 0)
                    _matchBattleIdPlayerList.Add(num);
                else
                    _matchBattleIdEnemyList.Add(num);
            }
        }
        Int32 num1 = 0;
        using (List<Int32>.Enumerator enumerator = _matchBattleIdPlayerList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                Int32 current = enumerator.Current;
                if (num1 != current)
                {
                    Debug.LogWarning("This Battle, player index and id not the same. Please be careful.");
                    break;
                }
                ++num1;
            }
        }
    }

    public void GoToBattleResult()
    {
        if (!_oneTime)
            return;
        _oneTime = false;
        Application.targetFrameRate = 60;
        Hide(() => PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.BattleResult));
    }

    public void GoToGameOver()
    {
        if (!_oneTime)
            return;
        _oneTime = false;
        Application.targetFrameRate = 60;
        Hide(() => PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.GameOver));
    }

    private void SendAutoAttackCommand(Int32 playerIndex)
    {
        BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_data[playerIndex];
        CMD_DATA cmd = btlData.cmd[0];
        if (cmd != null && btl_cmd.CheckUsingCommand(cmd))
            return;
        CurrentPlayerIndex = playerIndex;
        _currentCommandIndex = CommandMenu.Attack;
        BTL_DATA firstEnemyPtr = GetFirstEnemyPtr();
        btl_cmd.SetCommand(btlData.cmd[0], 1U, 176U, firstEnemyPtr.btl_id, 0U);
        InputFinishList.Add(CurrentPlayerIndex);
        CurrentPlayerIndex = -1;
    }

    private CommandDetail ProcessCommand(Int32 target, CursorGroup cursor)
    {
        CommandDetail commandDetail = new CommandDetail
        {
            CommandId = _currentCommandId,
            SubId = 0U
        };

        CharacterCommandType commandType = CharacterCommands.Commands[commandDetail.CommandId].Type;
        if (commandType == CharacterCommandType.Normal)
            commandDetail.SubId = CharacterCommands.Commands[commandDetail.CommandId].Ability;

        if (commandType == CharacterCommandType.Ability)
        {
            Int32 abilityId = CharacterCommands.Commands[commandDetail.CommandId].Abilities[_currentSubMenuIndex];
            commandDetail.SubId = (UInt32)PatchAbility(abilityId);
        }
        else if (commandType == CharacterCommandType.Item || commandType == CharacterCommandType.Throw)
        {
            Int32 num2 = _itemIdList[_currentSubMenuIndex];
            commandDetail.SubId = (UInt32)num2;
        }

        commandDetail.TargetId = 0;

        switch (cursor)
        {
            case CursorGroup.Individual:
                commandDetail.TargetId = (UInt16)(1 << target);
                break;
            case CursorGroup.AllPlayer:
                commandDetail.TargetId = 15;
                break;
            case CursorGroup.AllEnemy:
                commandDetail.TargetId = 240;
                break;
            case CursorGroup.All:
                commandDetail.TargetId = Byte.MaxValue;
                break;
        }

        commandDetail.TargetType = (UInt32)GetSelectMode(cursor);
        return commandDetail;
    }

    private void SendCommand(CommandDetail command)
    {
        CMD_DATA cmd = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].cmd[0];
        cmd.regist.sel_mode = 1;
        btl_cmd.SetCommand(cmd, command.CommandId, command.SubId, command.TargetId, command.TargetType);
        SetPartySwapButtonActive(false);
        InputFinishList.Add(CurrentPlayerIndex);

        PlayerDetailHUD playerDetailHud = _partyDetail.List.Find(hud => hud.PlayerId == CurrentPlayerIndex);
        if (playerDetailHud == null)
            return;

        playerDetailHud.ATBBlink = false;
        playerDetailHud.TranceBlink = false;
    }

    private void SendDoubleCastCommand(CommandDetail first, CommandDetail secondCommand)
    {
        CMD_DATA cmd = FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].cmd[3];
        cmd.regist.sel_mode = 1;
        btl_cmd.SetCommand(cmd, first.CommandId, first.SubId, first.TargetId, first.TargetType);
        btl_cmd.SetCommand(FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].cmd[0], secondCommand.CommandId, secondCommand.SubId, secondCommand.TargetId, secondCommand.TargetType);
        SetPartySwapButtonActive(false);
        InputFinishList.Add(CurrentPlayerIndex);

        PlayerDetailHUD playerDetailHud = _partyDetail.List.Find(hud => hud.PlayerId == CurrentPlayerIndex);
        if (playerDetailHud == null)
            return;

        playerDetailHud.ATBBlink = false;
        playerDetailHud.TranceBlink = false;
    }

    private command_tags GetCommandFromCommandIndex(CommandMenu commandIndex, Int32 playerIndex)
    {
        BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_data[playerIndex];
        Int32 presetId = FF9StateSystem.Common.FF9.party.member[btl.bi.line_no].info.menu_type;
        switch (commandIndex)
        {
            case CommandMenu.Attack:
                return command_tags.CMD_ATTACK;
            case CommandMenu.Defend:
                return command_tags.CMD_DEFEND;
            case CommandMenu.Ability1:
                return CharacterCommands.CommandSets[presetId].Get(Status.checkCurStat(btl, 16384U), 0);
            case CommandMenu.Ability2:
                return CharacterCommands.CommandSets[presetId].Get(Status.checkCurStat(btl, 16384U), 1);
            case CommandMenu.Item:
                return command_tags.CMD_ITEM;
            case CommandMenu.Change:
                return command_tags.CMD_CHANGE;
            default:
                return command_tags.CMD_NONE;
        }
    }

    private void SetCommandVisibility(Boolean isVisible, Boolean forceCursorMemo)
    {
        GameObject go = _commandDetailHUD.Attack;
        SetPartySwapButtonActive(isVisible);
        BackButton.SetActive(!isVisible && FF9StateSystem.MobilePlatform);
        if (isVisible)
        {
            if (!_commandDetailHUD.Self.activeSelf)
            {
                _commandDetailHUD.Self.SetActive(true);
                ButtonGroupState.RemoveCursorMemorize(CommandGroupButton);
                if (_commandCursorMemorize.ContainsKey(CurrentPlayerIndex) && (Int64)FF9StateSystem.Settings.cfg.cursor != 0L || forceCursorMemo)
                    go = _commandDetailHUD.GetGameObjectFromCommand(_commandCursorMemorize[CurrentPlayerIndex]);

                ButtonGroupState.SetCursorMemorize(go.GetComponent<ButtonGroupState>().enabled
                    ? go
                    : _commandDetailHUD.Attack, CommandGroupButton);
            }
            else
            {
                if (_commandCursorMemorize.ContainsKey(CurrentPlayerIndex) && (Int64)FF9StateSystem.Settings.cfg.cursor != 0L || forceCursorMemo)
                    go = _commandDetailHUD.GetGameObjectFromCommand(_commandCursorMemorize[CurrentPlayerIndex]);
                ButtonGroupState.ActiveButton = !go.GetComponent<ButtonGroupState>().enabled ? _commandDetailHUD.Attack : go;
            }
            if (!_hidingHud)
                ButtonGroupState.ActiveGroup = CommandGroupButton;
            else
                _currentButtonGroup = CommandGroupButton;
        }
        else
        {
            _commandCursorMemorize[CurrentPlayerIndex] = _currentCommandIndex;
            _commandDetailHUD.Self.SetActive(false);
        }
    }

    private void SetItemPanelVisibility(Boolean isVisible, Boolean forceCursorMemo)
    {
        if (isVisible)
        {
            ItemPanel.SetActive(true);
            ButtonGroupState.RemoveCursorMemorize(ItemGroupButton);
            if (_itemCursorMemorize.ContainsKey(CurrentPlayerIndex) && (Int64)FF9StateSystem.Settings.cfg.cursor != 0L || forceCursorMemo)
                _itemScrollList.JumpToIndex(_itemCursorMemorize[CurrentPlayerIndex], true);
            else
                _itemScrollList.JumpToIndex(0, false);
            ButtonGroupState.RemoveCursorMemorize(ItemGroupButton);
            ButtonGroupState.ActiveGroup = ItemGroupButton;
        }
        else
        {
            if (_currentCommandIndex == CommandMenu.Item && _currentSubMenuIndex != -1)
                _itemCursorMemorize[CurrentPlayerIndex] = _currentSubMenuIndex;
            ItemPanel.SetActive(false);
        }
    }

    private void SetAbilityPanelVisibility(Boolean isVisible, Boolean forceCursorMemo)
    {
        if (isVisible)
        {
            if (!AbilityPanel.activeSelf)
            {
                AbilityPanel.SetActive(true);
                Dictionary<Int32, Int32> dictionary = _currentCommandIndex != CommandMenu.Ability1 ? _ability2CursorMemorize : _ability1CursorMemorize;
                ButtonGroupState.RemoveCursorMemorize(AbilityGroupButton);
                if (dictionary.ContainsKey(CurrentPlayerIndex) && (Int64)FF9StateSystem.Settings.cfg.cursor != 0L || forceCursorMemo)
                    _abilityScrollList.JumpToIndex(dictionary[CurrentPlayerIndex], true);
                else
                    _abilityScrollList.JumpToIndex(0, true);
            }
            if (IsDoubleCast && _doubleCastCount == 1)
                ButtonGroupState.SetPointerNumberToGroup(1, AbilityGroupButton);
            else if (IsDoubleCast && _doubleCastCount == 2)
                ButtonGroupState.SetPointerNumberToGroup(2, AbilityGroupButton);
            else
                ButtonGroupState.SetPointerNumberToGroup(0, AbilityGroupButton);
            ButtonGroupState.ActiveGroup = AbilityGroupButton;
            ButtonGroupState.UpdateActiveButton();
        }
        else
        {
            if (_currentCommandIndex == CommandMenu.Ability1 && _currentSubMenuIndex != -1)
                _ability1CursorMemorize[CurrentPlayerIndex] = _currentSubMenuIndex;
            else if (_currentCommandIndex == CommandMenu.Ability2 && _currentSubMenuIndex != -1)
                _ability2CursorMemorize[CurrentPlayerIndex] = _currentSubMenuIndex;
            AbilityPanel.SetActive(false);
        }
    }

    private void SetTargetVisibility(Boolean isVisible)
    {
        if (isVisible)
        {
            TargetType cursor = 0;
            TargetDisplay subMode = 0;
            _defaultTargetCursor = false;
            _defaultTargetDead = false;
            _targetDead = false;
            if (_currentCommandIndex == CommandMenu.Ability1 || _currentCommandIndex == CommandMenu.Ability2)
            {
                CharacterCommand ff9Command = CharacterCommands.Commands[_currentCommandId];
                AA_DATA aaData = FF9StateSystem.Battle.FF9Battle.aa_data[ff9Command.Type != CharacterCommandType.Ability ? ff9Command.Ability : ff9Command.Abilities[_currentSubMenuIndex]];
                cursor = aaData.Info.Target;
                _defaultTargetCursor = aaData.Info.DefaultAlly;
                _defaultTargetDead = aaData.Info.DefaultOnDead;
                _targetDead = aaData.Info.ForDead;
                subMode = aaData.Info.DisplayStats;
            }
            else if (_currentCommandIndex != CommandMenu.Attack && _currentCommandIndex == CommandMenu.Item)
            {
                ITEM_DATA itemData = ff9item._FF9Item_Info[_itemIdList[_currentSubMenuIndex] - 224];
                cursor = itemData.info.Target;
                _defaultTargetCursor = itemData.info.DefaultAlly;
                _defaultTargetDead = itemData.info.ForDead;
                _targetDead = itemData.info.ForDead;
                subMode = itemData.info.DisplayStats;
            }
            _isAllTarget = false;
            TargetPanel.SetActive(true);
            EnableTargetArea();
            DisplayTarget();
            DisplayStatus(subMode);
            SetTargetAvalability(cursor);
            SetTargetDefault();
            SetTargetHelp();
            ButtonGroupState.ActiveGroup = TargetGroupButton;
            _allTargetToggle.Set(_isAllTarget);
            DisplayTargetPointer();
        }
        else
        {
            DisableTargetArea();
            ClearModelPointer();
            ButtonGroupState.SetAllTarget(false);
            _cursorType = CursorGroup.Individual;
            _allTargetToggle.value = false;
            ButtonGroupState.DisableAllGroup(true);
            AllTargetButton.SetActive(false);
            _targetPanel.SetActive(false);
            StatusContainer.SetActive(false);
            _partyDetail.SetActive(true);
            TargetPanel.SetActive(false);
        }
    }

    private void SetTargetAvalability(TargetType cursor)
    {
        _targetCursor = cursor;
        if (cursor == TargetType.SingleAny)
        {
            _cursorType = CursorGroup.Individual;

            ChangeTargetAvalability(player: true, enemy: true, all: false, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.SingleEnemy)
        {
            _cursorType = CursorGroup.Individual;

            ChangeTargetAvalability(player: false, enemy: true, all: false, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.SingleAlly)
        {
            _cursorType = CursorGroup.Individual;

            ChangeTargetAvalability(player: true, enemy: false, all: false, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.ManyAny)
        {
            ChangeTargetAvalability(player: true, enemy: true, all: FF9StateSystem.MobilePlatform, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.ManyEnemy)
        {
            ChangeTargetAvalability(player: false, enemy: true, all: FF9StateSystem.MobilePlatform, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.ManyAlly)
        {
            ChangeTargetAvalability(player: true, enemy: false, all: FF9StateSystem.MobilePlatform, allPlayers: false, allEnemies: false);
        }
        else if (cursor == TargetType.AllEnemy || cursor == TargetType.RandomEnemy)
        {
            _cursorType = CursorGroup.AllEnemy;

            ChangeTargetAvalability(player: false, enemy: true, all: false, allPlayers: false, allEnemies: true);

            _isAllTarget = true;
        }
        else if (cursor == TargetType.AllAlly || cursor == TargetType.RandomAlly)
        {
            _cursorType = CursorGroup.AllPlayer;

            ChangeTargetAvalability(player: true, enemy: false, all: false, allPlayers: true, allEnemies: false);

            _isAllTarget = true;
        }
        else if (cursor == TargetType.All || cursor == TargetType.Everyone || cursor == TargetType.Random)
        {
            _cursorType = CursorGroup.All;

            ChangeTargetAvalability(player: true, enemy: true, all: false, allPlayers: true, allEnemies: true);

            _isAllTarget = true;
        }
        else if (cursor == TargetType.Self)
        {
            _cursorType = CursorGroup.Individual;

            ChangeTargetAvalability(player: false, enemy: false, all: false, allPlayers: false, allEnemies: false);

            ButtonGroupState.SetButtonEnable(PlayerTargetPanel.GetChild(CurrentBattlePlayerIndex), true);
        }
    }

    private void ChangeTargetAvalability(Boolean player, Boolean enemy, Boolean all, Boolean allPlayers, Boolean allEnemies)
    {
        foreach (Component component in PlayerTargetPanel.transform)
            ButtonGroupState.SetButtonEnable(component.gameObject, player);

        foreach (Component component in EnemyTargetPanel.transform)
            ButtonGroupState.SetButtonEnable(component.gameObject, enemy);

        AllTargetButton.SetActive(all);
        _targetPanel.AllPlayerButton.SetActive(allPlayers);
        _targetPanel.AllEnemyButton.SetActive(allEnemies);
    }

    private void SetTargetDefault()
    {
        Int32 index1 = 0;
        Int32 index2 = 4;
        if (_targetDead == false)
        {
            for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
            {
                Int32 num = 0;
                while (1 << num != btl.btl_id)
                    ++num;

                if (btl.btl_id == 0 || btl.bi.target == 0)
                    continue;

                if (btl.bi.player != 0)
                {
                    if (btl_stat.CheckStatus(btl, 256U))
                        ButtonGroupState.SetButtonEnable(_targetHudList[index1].Self, false);
                    ++index1;
                }
                else
                {
                    if (btl_stat.CheckStatus(btl, 256U))
                        ButtonGroupState.SetButtonEnable(_targetHudList[index2].Self, false);
                    ++index2;
                }
            }
        }

        
        if (_targetCursor == TargetType.SingleAny
            ||_targetCursor == TargetType.SingleAlly
            ||_targetCursor == TargetType.SingleEnemy
            ||_targetCursor == TargetType.ManyAny
            ||_targetCursor == TargetType.ManyAlly
            ||_targetCursor == TargetType.ManyEnemy)
        {
            if (_defaultTargetCursor)
            {
                ButtonGroupState.SetCursorStartSelect(_defaultTargetDead
                    ? _targetHudList[GetDead(true)].Self
                    : _targetHudList[CurrentBattlePlayerIndex].Self, TargetGroupButton);

                _currentTargetIndex = 0;
                ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
            }
            else
            {
                //int num = HonoluluBattleMain.EnemyStartIndex;
                Int32 firstIndex;
                if (_defaultTargetDead)
                {
                    firstIndex = GetDead(false);
                    ButtonGroupState.SetCursorStartSelect(_targetHudList[firstIndex].Self, TargetGroupButton);
                }
                else
                {
                    firstIndex = GetFirstEnemy() + HonoluluBattleMain.EnemyStartIndex;
                    if (firstIndex != -1)
                    {
                        if (_currentCommandIndex == CommandMenu.Attack && FF9StateSystem.PCPlatform)
                            ValidateDefaultTarget(ref firstIndex);
                        ButtonGroupState.SetCursorStartSelect(_targetHudList[firstIndex].Self, TargetGroupButton);
                    }
                }
                _currentTargetIndex = firstIndex;
                ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
            }
        }
        else
        {
            if (_targetCursor != TargetType.Self)
                return;

            Int32 currentPlayerIndex = CurrentBattlePlayerIndex;
            ButtonGroupState.SetCursorStartSelect(_targetHudList[currentPlayerIndex].Self, TargetGroupButton);
            _currentTargetIndex = currentPlayerIndex;
            ButtonGroupState.RemoveCursorMemorize(TargetGroupButton);
        }
    }

    private void SetTargetHelp()
    {
        String str1 = String.Empty;
        Boolean flag = (Int32)_targetCursor < 6 || (Int32)_targetCursor > 12;
        switch (_targetCursor)
        {
            case TargetType.SingleAny:
                str1 = Localization.Get("BattleTargetHelpIndividual");
                break;
            case TargetType.SingleAlly:
                str1 = Localization.Get("BattleTargetHelpIndividualPC");
                break;
            case TargetType.SingleEnemy:
                str1 = Localization.Get("BattleTargetHelpIndividualNPC");
                break;
            case TargetType.ManyAny:
                str1 = Localization.Get("BattleTargetHelpMultiS");
                break;
            case TargetType.ManyAlly:
                str1 = Localization.Get("BattleTargetHelpMultiPCS");
                break;
            case TargetType.ManyEnemy:
                str1 = Localization.Get("BattleTargetHelpMultiNPCS");
                break;
            case TargetType.All:
                str1 = Localization.Get("BattleTargetHelpAll");
                break;
            case TargetType.AllAlly:
                str1 = Localization.Get("BattleTargetHelpAllPC");
                break;
            case TargetType.AllEnemy:
                str1 = Localization.Get("BattleTargetHelpAllNPC");
                break;
            case TargetType.Random:
                str1 = Localization.Get("BattleTargetHelpRand");
                break;
            case TargetType.RandomAlly:
                str1 = Localization.Get("BattleTargetHelpRandPC");
                break;
            case TargetType.RandomEnemy:
                str1 = Localization.Get("BattleTargetHelpRandNPC");
                break;
            case TargetType.Everyone:
                str1 = Localization.Get("BattleTargetHelpWhole");
                break;
            case TargetType.Self:
                str1 = Localization.Get("BattleTargetHelpSelf");
                break;
        }

        if (_isAllTarget)
        {
            flag = false;
            switch (_targetCursor)
            {
                case TargetType.ManyAny:
                    str1 = Localization.Get("BattleTargetHelpMultiM");
                    break;
                case TargetType.ManyAlly:
                    str1 = Localization.Get("BattleTargetHelpMultiPCM");
                    break;
                case TargetType.ManyEnemy:
                    str1 = Localization.Get("BattleTargetHelpMultiNPCM");
                    break;
            }
        }

        Int32 index1 = 0;
        Int32 index2 = 4;
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.btl_id == 0 || btl.bi.target == 0)
                continue;

            if (btl.bi.player != 0)
            {
                TargetHUD targetHud = _targetHudList[index1];
                String str2 = !flag ? String.Empty : btl_util.getPlayerPtr(btl).name;
                targetHud.ButtonGroup.Help.Enable = true;
                targetHud.ButtonGroup.Help.Text = str1 + "\n" + str2;
                ++index1;
            }
            else
            {
                TargetHUD targetHud = _targetHudList[index2];
                Single additionalWidth = 0.0f;
                String str2 = !flag ? String.Empty : Singleton<HelpDialog>.Instance.PhraseLabel.PhrasePreOpcodeSymbol(btl_util.getEnemyPtr(btl).et.name, ref additionalWidth);
                targetHud.ButtonGroup.Help.Enable = true;
                targetHud.ButtonGroup.Help.Text = str1 + "\n" + str2;
                ++index2;
            }
        }
    }

    private void SetHelpMessageVisibility(Boolean active)
    {
        if (!ButtonGroupState.HelpEnabled)
            return;
        Singleton<HelpDialog>.Instance.gameObject.SetActive(active && (CommandPanel.activeSelf || ItemPanel.activeSelf || AbilityPanel.activeSelf || TargetPanel.activeSelf));
    }

    private void SetHudVisibility(Boolean active)
    {
        if (_hidingHud != active)
            return;

        _hidingHud = !active;
        AllMenuPanel.SetActive(active);

        SetHelpMessageVisibility(active);

        if (!active)
        {
            _currentButtonGroup = ButtonGroupState.ActiveGroup;
            ButtonGroupState.DisableAllGroup(false);
            ButtonGroupState.SetPointerVisibilityToGroup(false, _currentButtonGroup);
        }
        else
        {
            if (_currentButtonGroup == CommandGroupButton && !CommandPanel.activeSelf)
                _currentButtonGroup = String.Empty;

            _isTryingToRun = false;
            ButtonGroupState.ActiveGroup = _currentButtonGroup;
            DisplayTargetPointer();
        }
    }

    private void ProcessAutoBattleInput()
    {
        _isAutoAttack = !_isAutoAttack;
        _autoBattleToggle.value = _isAutoAttack;
        AutoBattleHud.SetActive(_isAutoAttack);
        _autoBattleButtonComponent.SetState(UIButtonColor.State.Normal, false);
        if (_isAutoAttack)
        {
            SetIdle();
            SetPartySwapButtonActive(false);
        }
        else
        {
            SetPartySwapButtonActive(true);
            foreach (var current in _partyDetail.List)
                current.ATBBlink = ReadyQueue.Contains(current.PlayerId) && !InputFinishList.Contains(current.PlayerId);
        }
    }

    public Boolean FF9BMenu_IsEnable()
    {
        return _commandEnable;
    }

    public static Boolean ForceNextTurn;

    public Boolean FF9BMenu_IsEnableAtb()
    {
        if (!IsNativeEnableAtb())
            return false;

        if (Configuration.Hacks.BattleSpeed != 2)
            return true;

        if (FF9StateSystem.Battle.FF9Battle.cmd_queue.next != null || FF9StateSystem.Battle.FF9Battle.cur_cmd != null)
            return false;

        if (UIManager.Battle.CurrentPlayerIndex == -1)
            return true;

        if (!ForceNextTurn)
            return false;

        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.sel_mode != 0 || btl.sel_menu != 0 || btl.cur.hp == 0 || btl.bi.atb == 0 || btl.bi.player == 0)
                continue;

            if (btl.cur.at < btl.max.at)
                return true;
        }

        return false;
    }

    internal Boolean IsNativeEnableAtb()
    {
        if (!_commandEnable)
            return false;

        if ((Int64)FF9StateSystem.Settings.cfg.atb != 1L)
            return true;

        if (_hidingHud)
            return CurrentPlayerIndex == -1 || _currentButtonGroup == CommandGroupButton || _currentButtonGroup == String.Empty;

        return CurrentPlayerIndex == -1 || ButtonGroupState.ActiveGroup == CommandGroupButton || ButtonGroupState.ActiveGroup == String.Empty;
    }

    public void FF9BMenu_EnableMenu(Boolean active)
    {
        if (PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI)
            return;

        if (PersistenSingleton<UIManager>.Instance.State == UIManager.UIState.BattleHUD)
        {
            _commandEnable = active;
            AllMenuPanel.SetActive(active);
            HideHudHitAreaGameObject.SetActive(active);
            if (!active)
            {
                ButtonGroupState.DisableAllGroup(true);
            }
            else
            {
                if ((_isFromPause || ButtonGroupState.ActiveGroup != String.Empty) && !_isNeedToInit)
                    return;

                _isNeedToInit = false;
                InitialBattle();
                DisplayParty();
                SetIdle();
            }
        }
        else
        {
            _beforePauseCommandEnable = active;
            _isNeedToInit = active;
        }
    }

    public void FF9BMenu_Enable(Boolean enable)
    {
    }

    private Int32 PatchAbility(Int32 id)
    {
        if (AbilCarbuncle == id)
        {
            switch (FF9StateSystem.Common.FF9.party.member[FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].bi.line_no].equip[4])
            {
                case 227:
                    id += 3;
                    break;
                case 228:
                    ++id;
                    break;
                case 229:
                    id += 2;
                    break;
            }
        }
        else if (AbilFenril == id)
        {
            Byte num = FF9StateSystem.Common.FF9.party.member[FF9StateSystem.Battle.FF9Battle.btl_data[CurrentPlayerIndex].bi.line_no].equip[4];
            id += num != 222 ? 0 : 1;
        }
        return id;
    }

    private UInt16 GetDead(Boolean player)
    {
        UInt16 num = 0;
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            if (btlData.bi.target == 0 || ((Int32)btlData.stat.cur & 256) == 0)
                continue;

            if (player && btlData.bi.player != 0 || !player && btlData.bi.player == 0)
                return num;

            ++num;
        }
        return (UInt16)CurrentBattlePlayerIndex;
    }

    private static Int32 GetFirstPlayer()
    {
        Int32 num = -1;
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            if (btlData.bi.player != 0)
                ++num;
            if (btlData.bi.player != 0 && btlData.cur.hp != 0)
                return num;
        }
        return num;
    }

    private static Int32 GetFirstEnemy()
    {
        Int32 num = -1;
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            if (btlData.bi.player == 0)
                ++num;
            if (btlData.bi.player == 0 && btlData.cur.hp != 0)
                return num;
        }
        return num;
    }

    private static BTL_DATA GetFirstEnemyPtr()
    {
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            if (btlData.bi.player == 0 && btlData.cur.hp != 0)
                return btlData;
        }
        return null;
    }

    public void ItemRequest(Int32 id)
    {
        _needItemUpdate = true;
    }

    public void ItemUse(Int32 id)
    {
        if (ff9item.FF9Item_Remove(id, 1) == 0)
            return;
        _needItemUpdate = true;
    }

    public void ItemUnuse(Int32 id)
    {
        _needItemUpdate = true;
    }

    public void ItemRemove(Int32 id)
    {
        if (ff9item.FF9Item_Remove(id, 1) == 0)
            return;
        _needItemUpdate = true;
    }

    public void ItemAdd(Int32 id)
    {
        if (ff9item.FF9Item_Add(id, 1) == 0)
            return;
        _needItemUpdate = true;
    }

    private Boolean IsEnableInput(BTL_DATA cur)
    {
        return cur != null && cur.cur.hp != 0 && (!btl_stat.CheckStatus(cur, 1107434755U) && (battle.btl_bonus.member_flag & 1 << cur.bi.line_no) != 0);
    }

    private Int32 GetSelectMode(CursorGroup cursor)
    {
        if (_targetCursor == TargetType.Random || _targetCursor == TargetType.RandomAlly || _targetCursor == TargetType.RandomEnemy)
            return 2;
        return cursor == CursorGroup.Individual ? 0 : 1;
    }

    private void EnableTargetArea()
    {
        for (BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_list.next; btlData != null; btlData = btlData.next)
        {
            if (btlData.bi.target == 0)
                continue;

            Int32 index = 0;
            while (1 << index != btlData.btl_id)
                ++index;

            modelButtonManager.Show(btlData.gameObject.transform.GetChildByName("bone" + btlData.tar_bone.ToString("D3")).gameObject.transform, index, btlData.bi.player == 0, btlData.radius, btlData.height);
        }
    }

    private void DisableTargetArea()
    {
        modelButtonManager.Reset();
        _targetIndexList.Clear();
    }

    private void ClearModelPointer()
    {
        using (List<Int32>.Enumerator enumerator = _targetIndexList.GetEnumerator())
        {
            while (enumerator.MoveNext())
                Singleton<PointerManager>.Instance.RemovePointerFromGameObject(modelButtonManager.GetGameObject(enumerator.Current));
        }
        _targetIndexList.Clear();
    }

    private void PointToModel(CursorGroup selectType, Int32 targetIndex = 0)
    {
        ClearModelPointer();
        Boolean isBlink = false;
        switch (selectType)
        {
            case CursorGroup.Individual:
                if (targetIndex < HonoluluBattleMain.EnemyStartIndex)
                {
                    if (targetIndex < _matchBattleIdPlayerList.Count)
                        _targetIndexList.Add(_matchBattleIdPlayerList[targetIndex]);
                }
                else if (targetIndex - HonoluluBattleMain.EnemyStartIndex < _matchBattleIdEnemyList.Count)
                {
                    _targetIndexList.Add(_matchBattleIdEnemyList[targetIndex - HonoluluBattleMain.EnemyStartIndex]);
                }
                break;
            case CursorGroup.AllPlayer:
                _targetIndexList = modelButtonManager.GetAllPlayerIndex();
                isBlink = true;
                break;
            case CursorGroup.AllEnemy:
                _targetIndexList = modelButtonManager.GetAllEnemyIndex();
                isBlink = true;
                break;
            case CursorGroup.All:
                _targetIndexList = modelButtonManager.GetAllIndex();
                isBlink = true;
                break;
        }

        foreach (Int32 index in _targetIndexList)
        {
            GameObject obj = modelButtonManager.GetGameObject(index);
            Singleton<PointerManager>.Instance.PointerDepth = 0;
            Singleton<PointerManager>.Instance.AttachPointerToGameObject(obj, true);
            Singleton<PointerManager>.Instance.SetPointerBlinkAt(obj, isBlink);
            Singleton<PointerManager>.Instance.SetPointerLimitRectBehavior(obj, PointerManager.LimitRectBehavior.Hide);
            Singleton<PointerManager>.Instance.PointerDepth = 5;
        }
    }

    private void ToggleAllTarget()
    {
        if (_cursorType == CursorGroup.AllEnemy || _cursorType == CursorGroup.AllPlayer)
        {
            if (ButtonGroupState.ActiveButton)
            {
                ButtonGroupState.SetButtonAnimation(ButtonGroupState.ActiveButton, true);
            }
            else
            {
                using (List<TargetHUD>.Enumerator enumerator = _targetHudList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                        ButtonGroupState.SetButtonAnimation(enumerator.Current.Self, true);
                }
                ButtonGroupState.ActiveButton = ButtonGroupState.GetCursorStartSelect(TargetGroupButton);
            }
            _cursorType = CursorGroup.Individual;
            _targetPanel.SetActive(false);
        }
        else
        {
            ButtonGroupState.SetButtonAnimation(ButtonGroupState.ActiveButton, false);
            Singleton<PointerManager>.Instance.RemovePointerFromGameObject(ButtonGroupState.ActiveButton);
            _cursorType = _currentTargetIndex >= HonoluluBattleMain.EnemyStartIndex ? CursorGroup.AllEnemy : CursorGroup.AllPlayer;
            _targetPanel.SetActive(true);
        }
        SetTargetHelp();
        DisplayTargetPointer();
    }

    private void DisplayTargetPointer()
    {
        if (ButtonGroupState.ActiveGroup != TargetGroupButton)
            return;
        if (_cursorType == CursorGroup.Individual)
        {
            PointToModel(_cursorType, _currentTargetIndex);
            ButtonGroupState.SetAllTarget(false);
        }
        else
        {
            PointToModel(_cursorType, 0);
            using (List<TargetHUD>.Enumerator enumerator = _targetHudList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    Singleton<PointerManager>.Instance.SetPointerVisibility(enumerator.Current.Self, false);
            }
            if (_cursorType == CursorGroup.AllPlayer)
            {
                List<GameObject> goList = new List<GameObject>();
                for (Int32 index = 0; index < _playerCount; ++index)
                {
                    if (_currentCharacterHp[index] != ParameterStatus.Empty || _targetDead)
                        goList.Add(_targetHudList[index].Self);
                }
                ButtonGroupState.SetMultipleTarget(goList, true);
            }
            else if (_cursorType == CursorGroup.AllEnemy)
            {
                List<GameObject> goList = new List<GameObject>();
                for (Int32 index = 0; index < _enemyCount; ++index)
                {
                    if (!_currentEnemyDieState[index] || _targetDead)
                        goList.Add(_targetHudList[index + HonoluluBattleMain.EnemyStartIndex].Self);
                }
                ButtonGroupState.SetMultipleTarget(goList, true);
            }
            else
                ButtonGroupState.SetAllTarget(true);
        }
    }

    public void SetIdle()
    {
        SetCommandVisibility(false, false);
        SetTargetVisibility(false);
        SetItemPanelVisibility(false, false);
        SetAbilityPanelVisibility(false, false);
        BackButton.SetActive(false);
        _currentSilenceStatus = false;
        _currentMpValue = -1;
        _currentCommandIndex = CommandMenu.Attack;
        _currentSubMenuIndex = -1;
        CurrentPlayerIndex = -1;
        //currentTranceTrigger = false;
        ButtonGroupState.DisableAllGroup(true);

        _partyDetail.SetDetailButtonState(UIButtonColor.State.Normal, false);
    }

    public void ResetToReady()
    {
        SetItemPanelVisibility(false, false);
        SetAbilityPanelVisibility(false, false);
        SetTargetVisibility(false);
        ClearModelPointer();
        DisplayCommand();
        SetCommandVisibility(true, false);
    }

    public void SetPartySwapButtonActive(Boolean isActive)
    {
        _partyDetail.SetPartySwapButtonActive(isActive);
    }

    private void Update()
    {
        if (PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI || PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.BattleHUD)
            return;
        UpdatePlayer();
        UpdateMessage();
        if (!_commandEnable)
            return;
        if (UIManager.Input.GetKey(Control.LeftBumper) && UIManager.Input.GetKey(Control.RightBumper) || _isTryingToRun)
        {
            _runCounter += RealTime.deltaTime;
            FF9StateSystem.Battle.FF9Battle.btl_escape_key = 1;
            if (_runCounter > 1.0)
            {
                _runCounter = 0.0f;
                btl_sys.CheckEscape(true);
            }
            else
                btl_sys.CheckEscape(false);
        }
        else
        {
            _runCounter = 0.0f;
            FF9StateSystem.Battle.FF9Battle.btl_escape_key = 0;
        }

        SetHudVisibility(!UIManager.Input.GetKey(Control.Special));
        UpdateAndroidTV();
    }

    public void ForceClearReadyQueue()
    {
        for (Int32 index = ReadyQueue.Count - 1; index >= 0; --index)
        {
            //BTL_DATA btlData = FF9StateSystem.Battle.FF9Battle.btl_data[ReadyQueue[index]];
            if (InputFinishList.Contains(ReadyQueue[index]))
                InputFinishList.Remove(ReadyQueue[index]);
            ReadyQueue.RemoveAt(index);
        }
    }

    public void VerifyTarget(Int32 modelIndex)
    {
        if (_hidingHud || !_commandEnable || _cursorType != CursorGroup.Individual)
            return;
        Int32 index = modelIndex >= HonoluluBattleMain.EnemyStartIndex ? _matchBattleIdEnemyList.IndexOf(modelIndex) + 4 : _matchBattleIdPlayerList.IndexOf(modelIndex);
        if (index == -1)
            return;
        FF9Sfx.FF9SFX_Play(103);
        if (!_targetHudList[index].ButtonGroup.enabled)
            return;
        CheckDoubleCast(modelIndex, CursorGroup.Individual);
    }

    private void SetTarget(Int32 battleIndex)
    {
        if (IsDoubleCast)
            SendDoubleCastCommand(_firstCommand, ProcessCommand(battleIndex, _cursorType));
        else
            SendCommand(ProcessCommand(battleIndex, _cursorType));
        SetTargetVisibility(false);
        SetIdle();
    }

    private void ValidateDefaultTarget(ref Int32 firstIndex)
    {
        for (Int32 index = firstIndex; index < _targetHudList.Count; ++index)
        {
            TargetHUD targetHud = _targetHudList[index];
            if (targetHud.Self.activeSelf && targetHud.NameLabel.color != FF9TextTool.Gray)
            {
                firstIndex = index;
                break;
            }
        }
    }

    private void Awake()
    {
        FadingComponent = ScreenFadeGameObject.GetComponent<HonoFading>();

        _partyDetail = new PartyDetailPanelDescriptor(this, PartyDetailPanel);
        _autoBattleButtonComponent = AutoBattleButton.GetComponent<UIButton>();
        _allTargetButtonComponent = AllTargetButton.GetComponent<UIButton>();
        _autoBattleToggle = AutoBattleButton.GetComponent<UIToggle>();
        _allTargetToggle = AllTargetButton.GetComponent<UIToggle>();
        _autoBattleToggle.validator = OnAutoToggleValidate;
        _allTargetToggle.validator = OnAllTargetToggleValidate;
        _targetPanel = new TargetPanelDescriptor(TargetPanel);

        if (FF9StateSystem.MobilePlatform)
        {
            RunButton.SetActive(true);
            UIEventListener eventListener = UIEventListener.Get(RunButton);
            eventListener.Press += OnRunPress;
        }
        else
            RunButton.SetActive(false);

        _battleDialogWidget = BattleDialogGameObject.GetComponent<UIWidget>();
        _battleDialogLabel = BattleDialogGameObject.GetChild(1).GetComponent<UILabel>();

        _commandDetailHUD = new CommandHUD(this, CommandPanel);

        foreach (Component component in PlayerTargetPanel.transform)
        {
            GameObject obj = component.gameObject;

            UIEventListener eventListener = UIEventListener.Get(obj);
            eventListener.Click += onClick;
            eventListener.Navigate += OnTargetNavigate;
            _targetHudList.Add(new TargetHUD(obj));
        }

        foreach (Component component in EnemyTargetPanel.transform)
        {
            GameObject obj = component.gameObject;
            UIEventListener eventListener = UIEventListener.Get(obj);
            eventListener.Click += onClick;
            eventListener.Navigate += OnTargetNavigate;
            _targetHudList.Add(new TargetHUD(obj));
        }

        UIEventListener.Get(_targetPanel.AllPlayerButton).Click += OnAllTargetClick;
        UIEventListener.Get(_targetPanel.AllPlayerButton).Hover += OnAllTargetHover;
        UIEventListener.Get(_targetPanel.AllEnemyButton).Click += OnAllTargetClick;
        UIEventListener.Get(_targetPanel.AllEnemyButton).Hover += OnAllTargetHover;

        _status = new StatusContainerDescriptor(StatusContainer);
        _itemScrollList = ItemPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        _abilityScrollList = AbilityPanel.GetChild(1).GetComponent<RecycleListPopulator>();
        //_itemTransition = TransitionGameObject.GetChild(0).GetComponent<HonoTweenClipping>();
        //_abilityTransition = TransitionGameObject.GetChild(1).GetComponent<HonoTweenClipping>();
        //_targetTransition = TransitionGameObject.GetChild(2).GetComponent<HonoTweenClipping>();
        _onResumeFromQuit = GeneratedAwake;
    }

    private sealed class TargetPanelDescriptor
    {
        public readonly GameObject AllPlayerButton;
        public readonly GameObject AllEnemyButton;

        public TargetPanelDescriptor(GameObject targetPanel)
        {
            GameObject buttons = targetPanel.GetChild(2);
            AllPlayerButton = buttons.GetChild(0);
            AllEnemyButton = buttons.GetChild(1);
        }

        public void SetActive(Boolean value)
        {
            AllEnemyButton.SetActive(value);
            AllPlayerButton.SetActive(value);
        }
    }

    private sealed class PartyDetailPanelDescriptor
    {
        public readonly List<PlayerDetailHUD> List;
        public readonly GameObject HpCaption;
        public readonly GameObject MpCaption;
        public readonly GameObject AtbCaption;

        private readonly BattleHUD _scene;
        private readonly GameObject _details;

        public PartyDetailPanelDescriptor(BattleHUD scene, GameObject partyDetail)
        {
            _scene = scene;
            _details = partyDetail.GetChild(0);

            List = GetDetailList();

            GameObject captions = partyDetail.GetChild(1);
            HpCaption = captions.GetChild(2);
            MpCaption = captions.GetChild(3);
            AtbCaption = captions.GetChild(4);
        }

        public void SetActive(Boolean value)
        {
            HpCaption.SetActive(value);
            MpCaption.SetActive(value);
            AtbCaption.SetActive(value);
        }

        public void SetBlink(Boolean value)
        {
            foreach (PlayerDetailHUD current in List)
            {
                current.ATBBlink = value;
                current.TranceBlink = value;
            }
        }

        public void SetDetailButtonState(UIButtonColor.State state, Boolean instant)
        {
            using (List<PlayerDetailHUD>.Enumerator enumerator = List.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    enumerator.Current.Component.ButtonColor.SetState(state, instant);
            }
        }

        public void SetPartySwapButtonActive(Boolean isActive)
        {
            foreach (PlayerDetailHUD current in List)
            {
                if (_scene.CurrentPlayerIndex == current.PlayerId)
                {
                    current.Component.UIBoxCollider.enabled = false;
                    current.Component.ButtonColor.disabledColor = current.Component.ButtonColor.pressed;
                }
                else
                {
                    current.Component.UIBoxCollider.enabled = isActive;
                    current.Component.ButtonColor.disabledColor = current.Component.ButtonColor.defaultColor;
                }
            }
        }

        private List<PlayerDetailHUD> GetDetailList()
        {
            List<PlayerDetailHUD> result = new List<PlayerDetailHUD>();
            foreach (Component component in _details.transform)
            {
                GameObject obj = component.gameObject;
                UIEventListener.Get(obj).Click += OnPartyDetailClick;
                result.Add(new PlayerDetailHUD(obj));
            }
            return result;
        }

        private void OnPartyDetailClick(GameObject go)
        {
            if (go.GetParent() != _details)
            {
                _scene.onClick(go);
                return;
            }

            Int32 playerId = List[go.transform.GetSiblingIndex()].PlayerId;
            if (!_scene.ReadyQueue.Contains(playerId)
                || _scene.InputFinishList.Contains(playerId)
                || _scene._unconsciousStateList.Contains(playerId)
                || playerId == _scene.CurrentPlayerIndex)
                return;

            _scene.SwitchPlayer(playerId);
        }
    }

    private sealed class StatusContainerDescriptor
    {
        public readonly List<NumberSubModeHUD> HpStatusHudList = new List<NumberSubModeHUD>();
        public readonly List<NumberSubModeHUD> MpStatusHudList = new List<NumberSubModeHUD>();
        public readonly List<StatusSubModeHUD> GoodStatusHudList = new List<StatusSubModeHUD>();
        public readonly List<StatusSubModeHUD> BadStatusHudList = new List<StatusSubModeHUD>();

        public readonly GameObject HpStatusPanel;
        public readonly GameObject MpStatusPanel;
        public readonly GameObject GoodStatusPanel;
        public readonly GameObject BadStatusPanel;

        public StatusContainerDescriptor(GameObject statusContainer)
        {
            HpStatusPanel = statusContainer.GetChild(0);
            MpStatusPanel = statusContainer.GetChild(1);
            GoodStatusPanel = statusContainer.GetChild(2);
            BadStatusPanel = statusContainer.GetChild(3);

            GetSubModels(HpStatusPanel, HpStatusHudList);
            GetSubModels(MpStatusPanel, MpStatusHudList);
            GetSubModels(GoodStatusPanel, GoodStatusHudList);
            GetSubModels(BadStatusPanel, BadStatusHudList);
        }

        public void SetActive(Boolean value)
        {
            HpStatusPanel.SetActive(value);
            MpStatusPanel.SetActive(value);
            GoodStatusPanel.SetActive(value);
            BadStatusPanel.SetActive(value);
        }

        public void DisplayStatusRealtime(List<Int32> playerIds)
        {
            if (HpStatusPanel.activeSelf)
                DisplayData(playerIds, DisplayHp);
            else if (MpStatusPanel.activeSelf)
                DisplayData(playerIds, DisplayMp);
            else if (BadStatusPanel.activeSelf)
                DisplayData(playerIds, DisplayBadStatus);
            else if (GoodStatusPanel.activeSelf)
                DisplayData(playerIds, DisplayGoodStatus);
        }

        private static void DisplayData(List<Int32> playerIds, Action<Int32, BTL_DATA> action)
        {
            for (BTL_DATA bd = FF9StateSystem.Battle.FF9Battle.btl_list.next; bd != null; bd = bd.next)
            {
                if (bd.bi.player == 0)
                    continue;

                Int32 num = 0;
                while (1 << num != bd.btl_id)
                    ++num;

                Int32 playerId = playerIds.IndexOf(num);
                if (playerId < 0)
                    continue;

                action(playerId, bd);
            }
        }

        private void DisplayHp(Int32 playerId, BTL_DATA bd)
        {
            NumberSubModeHUD hud = HpStatusHudList[playerId];
            hud.Self.SetActive(true);
            hud.Current.text = bd.cur.hp.ToString();
            hud.Max.text = bd.max.hp.ToString();
            switch (CheckHPState(bd))
            {
                case ParameterStatus.Empty:
                    hud.TextColor = FF9TextTool.Red;
                    return;
                case ParameterStatus.Critical:
                    hud.TextColor = FF9TextTool.Yellow;
                    return;
                default:
                    hud.TextColor = FF9TextTool.White;
                    return;
            }
        }

        private void DisplayMp(Int32 playerId, BTL_DATA bd)
        {
            NumberSubModeHUD numberSubModeHud = MpStatusHudList[playerId];
            numberSubModeHud.Self.SetActive(true);
            numberSubModeHud.Current.text = bd.cur.mp.ToString();
            numberSubModeHud.Max.text = bd.max.mp.ToString();
            numberSubModeHud.TextColor = CheckMPState(bd) != ParameterStatus.Empty ? FF9TextTool.White : FF9TextTool.Yellow;
        }

        private void DisplayBadStatus(Int32 playerId, BTL_DATA bd)
        {
            StatusSubModeHUD statusHud = BadStatusHudList[playerId];
            DisplayStatusHud(bd, statusHud, BadIconDict);
        }

        private void DisplayGoodStatus(Int32 playerId, BTL_DATA bd)
        {
            StatusSubModeHUD statusHud = GoodStatusHudList[playerId];
            DisplayStatusHud(bd, statusHud, GoodIconDict);
        }

        private static void DisplayStatusHud(BTL_DATA bd, StatusSubModeHUD statusHud, Dictionary<UInt32, Byte> iconDic)
        {
            UInt32 num2 = bd.stat.cur | bd.stat.permanent;
            statusHud.Self.SetActive(true);
            foreach (UISprite uiWidget in statusHud.SpriteList)
                uiWidget.alpha = 0.0f;

            Int32 index = 0;
            foreach (KeyValuePair<UInt32, Byte> current in iconDic)
            {
                if (((Int32)num2 & (Int32)current.Key) == 0)
                    continue;

                statusHud.SpriteList[0, index].alpha = 1f;
                statusHud.SpriteList[0, index].spriteName = FF9UIDataTool.IconSpriteName[current.Value];
                if (++index > statusHud.SpriteList.Length)
                    break;
            }
        }

        private static void GetSubModels(GameObject panel, List<NumberSubModeHUD> output)
        {
            foreach (Component component in panel.GetChild(0).transform)
                output.Add(new NumberSubModeHUD(component.gameObject));
        }

        private static void GetSubModels(GameObject panel, List<StatusSubModeHUD> output)
        {
            foreach (Component component in panel.GetChild(0).transform)
                output.Add(new StatusSubModeHUD(component.gameObject));
        }
    }

    [CompilerGenerated]
    private void GeneratedShow()
    {
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, null);
        PersistenSingleton<UIManager>.Instance.SetGameCameraEnable(true);
        PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(true);
        PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(true);
        PauseButtonGameObject.SetActive(PersistenSingleton<UIManager>.Instance.IsPauseControlEnable && FF9StateSystem.MobilePlatform);
        HelpButtonGameObject.SetActive(PersistenSingleton<UIManager>.Instance.IsPauseControlEnable && FF9StateSystem.MobilePlatform);
        ButtonGroupState.SetScrollButtonToGroup(_abilityScrollList.ScrollButton, AbilityGroupButton);
        ButtonGroupState.SetScrollButtonToGroup(_itemScrollList.ScrollButton, ItemGroupButton);
        ButtonGroupState.SetPointerOffsetToGroup(new Vector2(34f, 0.0f), AbilityGroupButton);
        ButtonGroupState.SetPointerOffsetToGroup(new Vector2(34f, 0.0f), ItemGroupButton);
        ButtonGroupState.SetPointerOffsetToGroup(new Vector2(16f, 0.0f), TargetGroupButton);
        ButtonGroupState.SetPointerOffsetToGroup(new Vector2(10f, 0.0f), CommandGroupButton);
        ButtonGroupState.SetPointerLimitRectToGroup(AbilityPanel.GetComponent<UIWidget>(), _abilityScrollList.cellHeight, AbilityGroupButton);
        ButtonGroupState.SetPointerLimitRectToGroup(ItemPanel.GetComponent<UIWidget>(), _itemScrollList.cellHeight, ItemGroupButton);
    }


    private void GeneratedAwake()
    {
        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, null);
        PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(true);
        PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(true);
        _commandEnable = _beforePauseCommandEnable;
        if (!_commandEnable)
            return;

        _isFromPause = true;
        FF9BMenu_EnableMenu(true);
        DisplayTargetPointer();
        _isFromPause = false;
    }

    public class InfoVal
    {
        public Int32 inc_val;
        public Int32 disp_val;
        public Int32 req_val;
        public Int32 anim_frm;
        public Byte pad;

        public InfoVal()
        {
        }
    }

    public class BattleItemListData : ListDataTypeBase
    {
        public Int32 Id;
        public Int32 Count;

        public BattleItemListData()
        {
        }
    }

    public class BattleAbilityListData : ListDataTypeBase
    {
        public Int32 Id;

        public BattleAbilityListData()
        {
        }
    }

    public enum SubMenu
    {
        Command,
        Target,
        Ability,
        Item,
        None,
    }

    public enum CommandMenu
    {
        Attack,
        Defend,
        Ability1,
        Ability2,
        Item,
        Change,
        None,
    }

    public enum CursorGroup
    {
        Individual,
        AllPlayer,
        AllEnemy,
        All,
        None,
    }

    private struct HUDComponent
    {
        public readonly Transform Transform;
        public readonly UIWidget UIWidget;
        public readonly UIButton Button;
        public readonly UIKeyNavigation UIKeyNavigation;
        public readonly BoxCollider UIBoxCollider;
        public readonly ButtonGroupState ButtonGroup;

        public readonly UILabel Label;

        public HUDComponent(GameObject go)
        {
            Transform = go.GetComponent<Transform>();
            UIWidget = go.GetComponent<UIWidget>();
            Button = go.GetComponent<UIButton>();
            UIKeyNavigation = go.GetComponent<UIKeyNavigation>();
            UIBoxCollider = go.GetComponent<BoxCollider>();
            ButtonGroup = go.GetComponent<ButtonGroupState>();

            GameObject labelChild = go.GetChild(0);
            //Transform labelTransform = labelChild.GetComponent<Transform>();
            Label = labelChild.GetComponent<UILabel>();

            //GameObject spriteChild = go.GetChild(1);
            //Transform spriteTransform = spriteChild.GetComponent<Transform>();
            //UISprite sprite = spriteChild.GetComponent<UISprite>();
            //
            //GameObject widgetChild = go.GetChild(2);
            //Transform widgetTransform = widgetChild.GetComponent<Transform>();
            //UIWidget widget = widgetChild.GetComponent<UIWidget>();
            //
            //GameObject widgetSpriteChild = widgetChild.GetChild(0);
            //Transform widgetSpriteTransform = widgetSpriteChild.GetComponent<Transform>();
            //UISprite widgetSprite = widgetSpriteChild.GetComponent<UISprite>();
        }
    }

    private struct HUDPlayerDetailComponent
    {
        public readonly ButtonGroupState ButtonGroup;
        public readonly UILabel Label;
        public readonly BoxCollider UIBoxCollider;
        public readonly UIButtonColor ButtonColor;
        public readonly UIButton Button;

        public HUDPlayerDetailComponent(GameObject go)
        {
            UIBoxCollider = go.GetComponent<BoxCollider>();
            Button = go.GetComponent<UIButton>();
            ButtonGroup = go.GetComponent<ButtonGroupState>();
            Label = go.GetComponent<UILabel>();
            ButtonColor = go.GetComponent<UIButtonColor>();
        }
    }

    private class CommandHUD
    {
        private readonly BattleHUD _scene;

        public readonly GameObject Self;
        public readonly UIWidget UiWidget;
        public readonly GameObject Attack;
        public readonly GameObject Skill1;
        public readonly GameObject Skill2;
        public readonly GameObject Item;
        public readonly GameObject Defend;
        public readonly GameObject Change;
        public readonly HUDComponent AttackComponent;
        public readonly HUDComponent Skill1Component;
        public readonly HUDComponent Skill2Component;
        public readonly HUDComponent ItemComponent;
        public readonly HUDComponent DefendComponent;
        public readonly HUDComponent ChangeComponent;
        public readonly UILabel CaptionLabel;

        public CommandHUD(BattleHUD scene, GameObject commandPanel)
        {
            _scene = scene;

            Self = commandPanel;
            UiWidget = Self.GetComponent<UIWidget>();
            Attack = commandPanel.GetChild(0);
            Defend = commandPanel.GetChild(1);
            Skill1 = commandPanel.GetChild(2);
            Skill2 = commandPanel.GetChild(3);
            Item = commandPanel.GetChild(4);
            Change = commandPanel.GetChild(5);

            SubscribeOnClick();

            CaptionLabel = commandPanel.GetChild(6).GetChild(2).GetComponent<UILabel>();
            AttackComponent = new HUDComponent(Attack);
            Skill1Component = new HUDComponent(Skill1);
            Skill2Component = new HUDComponent(Skill2);
            ItemComponent = new HUDComponent(Item);
            DefendComponent = new HUDComponent(Defend);
            ChangeComponent = new HUDComponent(Change);
        }

        public GameObject GetGameObjectFromCommand(CommandMenu menu)
        {
            switch (menu)
            {
                case CommandMenu.Attack:
                    return Attack;
                case CommandMenu.Defend:
                    return Defend;
                case CommandMenu.Ability1:
                    return Skill1;
                case CommandMenu.Ability2:
                    return Skill2;
                case CommandMenu.Item:
                    return Item;
                case CommandMenu.Change:
                    return Change;
                default:
                    return Attack;
            }
        }

        private void SubscribeOnClick()
        {
            SubscribeOnClick(Attack);
            SubscribeOnClick(Defend);
            SubscribeOnClick(Skill1);
            SubscribeOnClick(Skill2);
            SubscribeOnClick(Item);
            SubscribeOnClick(Change);
        }

        private void SubscribeOnClick(GameObject item)
        {
            UIEventListener listener = UIEventListener.Get(item);
            listener.Click += _scene.onClick;
        }
    }

    private class PlayerDetailHUD
    {
        private Boolean _atbBlink;
        private Boolean _tranceBlink;

        public Int32 PlayerId;
        public readonly GameObject Self;
        public readonly HUDPlayerDetailComponent Component;
        public readonly UILabel NameLabel;
        public readonly UILabel HPLabel;
        public readonly UILabel MPLabel;
        public readonly UISprite ATBHighlightSprite;
        public readonly UIWidget ATBForegroundWidget;
        public readonly UISprite ATBForegroundSprite;
        public readonly UIProgressBar ATBSlider;
        public readonly UISprite TranceHighlightSprite;
        public readonly UIWidget TranceForegroundWidget;
        public readonly UIProgressBar TranceSlider;
        public readonly GameObject TranceSliderGameObject;

        public PlayerDetailHUD(GameObject go)
        {
            _atbBlink = false;
            _tranceBlink = false;

            Self = go;
            Component = new HUDPlayerDetailComponent(Self);
            NameLabel = go.GetChild(0).GetComponent<UILabel>();
            HPLabel = go.GetChild(1).GetComponent<UILabel>();
            MPLabel = go.GetChild(2).GetComponent<UILabel>();
            ATBSlider = go.GetChild(3).GetComponent<UIProgressBar>();
            ATBForegroundWidget = go.GetChild(3).GetChild(0).GetComponent<UIWidget>();
            ATBForegroundSprite = go.GetChild(3).GetChild(0).GetChild(1).GetComponent<UISprite>();
            ATBHighlightSprite = go.GetChild(3).GetChild(0).GetChild(0).GetComponent<UISprite>();
            TranceSlider = go.GetChild(4).GetComponent<UIProgressBar>();
            TranceSliderGameObject = go.GetChild(4);
            TranceForegroundWidget = go.GetChild(4).GetChild(0).GetComponent<UIWidget>();
            TranceHighlightSprite = go.GetChild(4).GetChild(0).GetChild(0).GetComponent<UISprite>();
        }

        public Boolean ATBBlink
        {
            get { return _atbBlink; }
            set
            {
                ATBHighlightSprite.alpha = !value ? 0.0f : 0.6f;
                ATBForegroundWidget.alpha = !value ? 1f : 0.0f;
                _atbBlink = value;
            }
        }

        public Boolean TranceBlink
        {
            get { return _tranceBlink; }
            set
            {
                TranceHighlightSprite.alpha = !value ? 0.0f : 0.6f;
                TranceForegroundWidget.alpha = !value ? 1f : 0.0f;
                _tranceBlink = value;
            }
        }
    }

    private class NumberSubModeHUD
    {
        public readonly GameObject Self;
        public readonly UILabel Current;
        public readonly UILabel Max;
        public readonly UILabel Slash;

        public NumberSubModeHUD(GameObject go)
        {
            Self = go;
            Current = go.GetChild(0).GetComponent<UILabel>();
            Slash = go.GetChild(1).GetComponent<UILabel>();
            Max = go.GetChild(2).GetComponent<UILabel>();
        }

        public Color TextColor
        {
            set
            {
                Current.color = value;
                Slash.color = value;
                Max.color = value;
            }
        }
    }

    private class StatusSubModeHUD
    {
        public readonly GameObject Self;
        public readonly UISprite[,] SpriteList;

        public StatusSubModeHUD(GameObject go)
        {
            Self = go;
            SpriteList = new UISprite[1, 8];

            GameObject child = go.GetChild(0);
            for (Int32 i = 0; i < 8; i++)
                SpriteList[0, i] = child.GetChild(i).GetComponent<UISprite>();
        }
    }

    private enum AbilityStatus
    {
        None,
        Disable,
        Enable,
    }

    private enum ParameterStatus
    {
        Normal,
        Critical,
        Empty,
    }

    private enum SubMenuType
    {
        Normal,
        Ability,
        Item,
        Throw,
        Slide,
    }

    private class AbilityPlayerDetail
    {
        public PLAYER Player;
        public Boolean HasAp;
        public readonly Dictionary<Int32, Boolean> AbilityEquipList;
        public readonly Dictionary<Int32, Int32> AbilityPaList;
        public readonly Dictionary<Int32, Int32> AbilityMaxPaList;

        public AbilityPlayerDetail()
        {
            AbilityEquipList = new Dictionary<Int32, Boolean>();
            AbilityPaList = new Dictionary<Int32, Int32>();
            AbilityMaxPaList = new Dictionary<Int32, Int32>();
        }

        public void Clear()
        {
            AbilityEquipList.Clear();
            AbilityPaList.Clear();
            AbilityMaxPaList.Clear();
        }
    }

    private class MagicSwordCondition
    {
        public Boolean IsViviExist;
        public Boolean IsViviDead;
        public Boolean IsSteinerMini;

        public Boolean Changed(MagicSwordCondition other)
        {
            return IsViviExist != other.IsViviExist || IsViviDead != other.IsViviDead || IsSteinerMini != other.IsSteinerMini;
        }
    }

    private class CommandDetail
    {
        public UInt32 CommandId;
        public UInt32 SubId;
        public UInt16 TargetId;
        public UInt32 TargetType;
    }

    private struct TargetHUD
    {
        public readonly GameObject Self;
        public readonly ButtonGroupState ButtonGroup;
        public readonly UIKeyNavigation KeyNavigate;
        public readonly UILabel NameLabel;

        public TargetHUD(GameObject go)
        {
            Self = go;
            ButtonGroup = go.GetComponent<ButtonGroupState>();
            NameLabel = go.GetChild(0).GetComponent<UILabel>();
            KeyNavigate = go.GetComponent<UIKeyNavigation>();
        }
    }
}