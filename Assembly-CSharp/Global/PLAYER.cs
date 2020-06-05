using System;
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
		this.pa = new Byte[48];
		this.sa = new UInt32[2];
		this.sa[0] = 0u;
		this.sa[1] = 0u;
	}

    public CharacterIndex Index => info.slot_no;
    public Boolean IsSubCharacter => (Category & CharacterCategory.Subpc) == CharacterCategory.Subpc;

    public CharacterId CharacterId => PresetId.ToCharacterId();
    public EquipmentSetId DefaultEquipmentSetId => PresetId.ToEquipmentSetId();
    public CharacterPresetId PresetId
    {
        get { return info.menu_type; }
        set { info.menu_type = value; }
    }

    public CharacterCategory Category
    {
        get { return (CharacterCategory)category; }
        set { category = (Byte)value; }
    }

    public void ValidateSupportAbility()
	{
		Int32 activeSupportAbilityPoint = this.GetActiveSupportAbilityPoint();
		if ((Int32)this.cur.capa != (Int32)this.max.capa - activeSupportAbilityPoint)
		{
			this.cur.capa = this.max.capa;
			this.sa[0] = 0u;
			this.sa[1] = 0u;
			ff9play.FF9Play_Update(this);
		}
	}

	public void ValidateBasisStatus()
	{
		if (this.level == 99 && this.SetMaxBonusBasisStatus())
		{
			ff9play.FF9Play_Build((Int32)this.info.slot_no, (Int32)this.level, (PLAYER_INFO)null, false);
		}
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
		if (this.max.capa < (Byte)ff9level.FF9Level_GetCap((Int32)this.info.slot_no, (Int32)this.level, false))
		{
			ff9play.FF9Play_Build((Int32)this.info.slot_no, (Int32)this.level, (PLAYER_INFO)null, false);
		}
	}

	public Int32 GetActiveSupportAbilityPoint()
	{
		Int32 num = 0;
		Int32 num2 = 0;
		UInt32[] array = this.sa;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			UInt32 num3 = array[i];
			UInt32 num4 = num3;
			for (Int32 j = 0; j < 32; j++)
			{
				if (num4 % 2u == 1u)
				{
					CharacterAbilityGems sa_DATA = ff9abil._FF9Abil_SaData[num2];
					num += (Int32)sa_DATA.GemsCount;
				}
				num4 >>= 1;
				num2++;
			}
		}
		return num;
	}

    private String _name;

    public String GetRealName()
    {
        return _name;
    }

    public void SetRealName(String name)
    {
	    _name = name;
    }

    private String GetName()
    {
	    if ((category & 16) == 16)
        {
	        GetDefaultName(out _, out var altName);
	        return altName;
        }

	    SplitName(out var mainName, out _);
	    return mainName;
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

    // CharacterIndex

    public const Int32 PLAYER_NAME_MAX = 10;

	public const Int32 EQUIP_WEAPON = 0;

	public const Int32 EQUIP_ARMOR_HEAD = 1;

	public const Int32 EQUIP_ARMOR_WRIST = 2;

	public const Int32 EQUIP_ARMOR_BODY = 3;

	public const Int32 EQUIP_ACCESSORY = 4;

	public const Int32 EQUIP_MAX = 5;

    public String name
    {
        get { return GetName(); }
        set { SetName(value); }
    }

    private void SetName(String value)
    {
	    if ((category & 16) == 16)
	    {
		    // We cannot change the names of alternative characters
		    if (String.IsNullOrEmpty(_name))
		    {
			    GetDefaultName(out var mainName, out _);
			    _name = mainName;
		    }
	    }
	    else
	    {
		    _name = value;
		    SplitName(out var mainName, out _);
		    _name = mainName;
	    }
    }

    private void SplitName(out String mainName, out String altName)
    {
	    if (String.IsNullOrEmpty(_name))
        {
            GetDefaultName(out mainName, out altName);
        }
        else
        {
            String[] parts = _name.Split('\t');
            if (parts.Length == 2)
            {
                mainName = parts[0];
                altName = parts[1];
            }
            else if ((category & 16) == 16)
            {
                GetDefaultName(out mainName, out altName);
                altName = parts[0];
            }
            else
            {
                GetDefaultName(out mainName, out altName);
                mainName = parts[0];
            }
        }
    }

    private void GetDefaultName(out String mainName, out String altName)
    {
        CharacterPresetId presetId = PresetId;
        if (presetId == CharacterPresetId.Cinna1 || presetId == CharacterPresetId.Cinna2 || presetId == CharacterPresetId.StageCinna)
        {
            mainName = FF9TextTool.CharacterDefaultName(CharacterPresetId.Quina);
            altName = FF9TextTool.CharacterDefaultName(info.menu_type);
            return;
        }
        if (presetId == CharacterPresetId.Marcus1 || presetId == CharacterPresetId.Marcus2 || presetId == CharacterPresetId.StageMarcus)
        {
            mainName = FF9TextTool.CharacterDefaultName(CharacterPresetId.Eiko);
            altName = FF9TextTool.CharacterDefaultName(info.menu_type);
            return;
        }
        if (presetId == CharacterPresetId.Blank1 || presetId == CharacterPresetId.Blank2 || presetId == CharacterPresetId.StageBlank)
        {
            mainName = FF9TextTool.CharacterDefaultName(CharacterPresetId.Amarant);
            altName = FF9TextTool.CharacterDefaultName(info.menu_type);
            return;
        }

        mainName = FF9TextTool.CharacterDefaultName(info.menu_type);
        altName = FF9TextTool.CharacterDefaultName(info.menu_type);
    }

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

	public Byte status;

	public CharacterEquipment equip;

	public FF9LEVEL_BONUS bonus;

	public Byte[] pa;

	public UInt32[] sa;
}
