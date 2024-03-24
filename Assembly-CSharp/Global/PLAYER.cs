using System;
using System.Collections.Generic;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria.Data;

public class PLAYER
{
    public PLAYER()
	{
		this.cur = new POINTS();
		this.max = new POINTS();
		this.elem = new ELEMENT();
		this.defence = new ItemDefence();
		this.basis = new PLAYER_BASE();
	    this.equip = new CharacterEquipment();
		this.bonus = new FF9LEVEL_BONUS();
		this.sa = new UInt32[2];
		this.sa[0] = 0u;
		this.sa[1] = 0u;
		this.saExtended = new HashSet<SupportAbility>();
		this.mpCostFactor = 100;
        this.mpCostFactorSkill1 = 100;
        this.mpCostFactorSkill2 = 100;
        this.maxHpLimit = ff9play.FF9PLAY_HP_MAX;
		this.maxMpLimit = ff9play.FF9PLAY_MP_MAX;
		this.maxDamageLimit = ff9play.FF9PLAY_DAMAGE_MAX;
		this.maxMpDamageLimit = ff9play.FF9PLAY_MPDAMAGE_MAX;
	}

    public CharacterId Index => info.slot_no;
    public Boolean IsSubCharacter => (Category & CharacterCategory.Subpc) == CharacterCategory.Subpc;

    public CharacterPresetId PresetId
    {
        get => info.menu_type;
        set => info.menu_type = value;
    }

    public CharacterCategory Category
    {
        get => (CharacterCategory)category;
        set => category = (Byte)value;
    }

    public void ValidateSupportAbility()
	{
		Int32 activeSupportAbilityPoint = this.GetActiveSupportAbilityPoint();
		if (this.cur.capa != this.max.capa - activeSupportAbilityPoint)
		{
			this.cur.capa = this.max.capa;
			this.sa[0] = 0u;
			this.sa[1] = 0u;
			this.saExtended.Clear();
		}
	}

	public void ValidateBasisStatus()
	{
		if (this.level >= ff9level.LEVEL_COUNT && this.SetMaxBonusBasisStatus())
			ff9play.FF9Play_Build(this, ff9level.LEVEL_COUNT, false, false);
	}

	public Boolean SetMaxBonusBasisStatus()
	{
		Boolean result = false;
		if (this.bonus.str < 294)
		{
			result = true;
			this.bonus.str = 294;
		}
		if (this.bonus.mgc < 294)
		{
			result = true;
			this.bonus.mgc = 294;
		}
		if (this.bonus.wpr < 98)
		{
			result = true;
			this.bonus.wpr = 98;
		}
		return result;
	}

	public void ValidateMaxStone()
	{
		if (this.max.capa < (Byte)ff9level.FF9Level_GetCap(this, this.level, false))
			ff9play.FF9Play_Build(this, this.level, false, false);
	}

	public Int32 GetActiveSupportAbilityPoint()
	{
		Int32 gemCount = 0;
		foreach (SupportAbility saIndex in saExtended)
			gemCount += ff9abil._FF9Abil_SaData[saIndex].GemsCount;
		return gemCount;
	}

	public void SetPermanentStatus(BattleStatus newPerm)
	{
		BattleStatus oldPerm = permanent_status;
		permanent_status = newPerm;
		status = (status | permanent_status) & ~(oldPerm & ~permanent_status);
	}

	public String Name
	{
		get
		{
			if (String.IsNullOrEmpty(_name))
				_name = FF9TextTool.CharacterDefaultName(info.slot_no);
			return _name;
		}
		set => _name = value;
	}

	public const Byte PLAYER_CATEGORY_MALE = 1;
	public const Byte PLAYER_CATEGORY_FEMALE = 2;
	public const Byte PLAYER_CATEGORY_GAIA = 4;
	public const Byte PLAYER_CATEGORY_TERRA = 8;
	public const Byte PLAYER_CATEGORY_SUBPC = 16;
	public const Byte PLAYER_CATEGORY_RESERVE1 = 32;
	public const Byte PLAYER_CATEGORY_RESERVE2 = 64;
	public const Byte PLAYER_CATEGORY_RESERVE3 = 128;

	public const Byte ADVANCED_GUARD = 1;
	public const Byte REAR_GUARD = 0;

	public const Int32 TRANCE_MODEL_ID_OFS = 19;
	public const Int32 SLOT_MAX = 9;
	public const Int32 PARTY_MAX = 4;

    public const Int32 PLAYER_NAME_MAX = 10;

	public const Int32 EQUIP_WEAPON = 0;
	public const Int32 EQUIP_ARMOR_HEAD = 1;
	public const Int32 EQUIP_ARMOR_WRIST = 2;
	public const Int32 EQUIP_ARMOR_BODY = 3;
	public const Int32 EQUIP_ACCESSORY = 4;
	public const Int32 EQUIP_MAX = 5;

	private String _name;

	public Byte category;

	public Byte level;
	public UInt32 exp;

	public POINTS cur;
	public POINTS max;

	public Byte trance;

	public Byte wep_bone;

	public ELEMENT elem;
	public ItemDefence defence;
	public PLAYER_BASE basis;
	public PLAYER_INFO info;

	public BattleStatus status;
	public BattleStatus permanent_status;

	public CharacterEquipment equip;
	public FF9LEVEL_BONUS bonus;

	public Byte[] pa;

	public UInt32[] sa;
	public HashSet<SupportAbility> saExtended;

	// Custom fields
	public Int16 mpCostFactor;
    public Int16 mpCostFactorSkill1;
    public Int16 mpCostFactorSkill2;
    public UInt32 maxHpLimit;
	public UInt32 maxMpLimit;
	public UInt32 maxDamageLimit;
	public UInt32 maxMpDamageLimit;
}
