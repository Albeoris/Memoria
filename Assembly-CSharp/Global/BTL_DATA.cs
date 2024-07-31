using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BTL_DATA
{
    public void ChangeModel(GameObject newModel, Int16 geoID)
    {
        gameObject.SetActive(false);
        newModel.SetActive(true);
        gameObject = newModel;
        meshCount = 0;
        foreach (Transform transform in gameObject.transform)
            if (transform.name.Contains("mesh"))
                meshCount++;
        meshIsRendering = new Boolean[meshCount];
        for (Int32 i = 0; i < meshCount; i++)
            meshIsRendering[i] = true;
        dms_geo_id = geoID;
        if (builtin_weapon_mode && weapon_geo != null)
            weapon_geo.transform.localScale = Vector3.one;
        builtin_weapon_mode = weapon_geo != null && weapon_geo.name != btl_eqp.DummyWeaponName && btl_eqp.EnemyBuiltInWeaponTable.ContainsKey(geoID);
        _smoothUpdateRegistered = false;
    }

    public void SetDisappear(Boolean disappear, Byte priority)
    {
        Byte value = disappear ? Math.Max(this.bi.disappear, priority) :
                     priority >= this.bi.disappear ? (Byte)0 : this.bi.disappear;
        // Move the gameObject out of camera view right before hiding it to avoid flickering when showing it back
        if (value != 0 && this.bi.disappear == 0)
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, -10000f, gameObject.transform.localPosition.z);
        this.bi.disappear = value;
        this.SetActiveBtlData(value == 0);
    }

    public GameObject getShadow()
    {
        return FF9StateSystem.Battle.FF9Battle.map.shadowArray[this];
    }

    public void SetActiveBtlData(Boolean value)
    {
        GameObject shadow = this.getShadow();
        this.gameObject.SetActive(value);
        btl_stat.SetStatusClut(this, btl_stat.CheckStatus(this, BattleStatusConst.ChgPolyClut));
        if (this.bi.shadow != 0)
            shadow.SetActive(value);
        if (value)
            _smoothUpdateRegistered = false;
    }

    public void SetIsEnabledMeshRenderer(Int32 mesh, Boolean isEnabled)
    {
        if (this.meshIsRendering[mesh] != isEnabled && this.gameObject != null)
        {
            Transform meshNode = this.gameObject.transform.GetChildByName("mesh" + mesh);
            if (meshNode != null)
            {
                Renderer[] renderers = meshNode.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                    if (renderer.enabled != isEnabled)
                        renderer.enabled = isEnabled;
                this.meshIsRendering[mesh] = isEnabled;
            }
        }
    }

    public void SetIsEnabledWeaponRenderer(Boolean isEnabled)
    {
        if (this.weaponIsRendering != isEnabled && this.gameObject != null)
        {
            Transform rootNode = this.gameObject.transform.GetChildByName("bone000");
            if (rootNode != null)
            {
                Renderer[] renderers = rootNode.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                    if (renderer.enabled != isEnabled)
                        renderer.enabled = isEnabled;
                this.weaponIsRendering = isEnabled;
            }
        }
    }

    public void SetIsEnabledBattleModelRenderer(Boolean isEnabled)
    {
        if (this.battleModelIsRendering != isEnabled && this.gameObject != null)
        {
            Transform battleNode = this.gameObject.transform.GetChildByName("battle_model");
            if (battleNode != null)
            {
                Renderer[] renderers = battleNode.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                    if (renderer.enabled != isEnabled)
                        renderer.enabled = isEnabled;
                this.battleModelIsRendering = isEnabled;
            }
        }
    }

    public void CheckDelayedModifier()
    {
        try
        {
            List<DelayedModifier> removedList = new List<DelayedModifier>();
            BattleUnit unit = new BattleUnit(this);
            foreach (DelayedModifier modifier in delayedModifierList)
            {
                if (!modifier.isDelayed(unit))
                {
                    if (modifier.apply != null)
                        modifier.apply(unit);
                    removedList.Add(modifier);
                }
            }
            foreach (DelayedModifier modifier in removedList)
                delayedModifierList.Remove(modifier);
        }
        catch (Exception err)
        {
            Log.Error(err);
        }
    }

    public BTL_DATA next = null;

    public List<CMD_DATA> cmd = new List<CMD_DATA>(new CMD_DATA[6]);

    public POINTS max = new POINTS();
    public POINTS cur = new POINTS();
    public ELEMENT elem = new ELEMENT();
    public STAT_INFO stat = new STAT_INFO();

    public BTL_INFO bi = new BTL_INFO();

    public ItemDefence defence = new ItemDefence();
    public DEF_ATTR def_attr = new DEF_ATTR();
    public ItemAttack weapon = null;

    public Byte trance;

    public Byte p_up_attr;

    public Byte level;

    public Byte escape_key;

    public Byte tar_bone;

    public Byte die_seq;

    public UInt16 fig_info;
    public Int32 fig;
    public Int32 m_fig;

    public Int16 dms_geo_id;

    public Quaternion rot;
    public Vector3 pos;
    public Vector3 base_pos;

    public PosObj evt = new PosObj();

    public String[] mot;

    public UInt16 animFlag;
    public Single animSpeed;
    public Single animFrameFrac;

    public GameObject weapon_geo;
    public Byte weapon_bone;
    public Vector3 weapon_scale;
    public Vector3 weapon_offset_pos;
    public Vector3 weapon_offset_rot;

    public UInt16 mesh_current;
    public UInt16 mesh_banish;

    public UInt16 btl_id;

    public Byte tar_mode; // FF9.Command.TAR_MODE_...
    public Byte sel_mode; // FF9.Command.SEL_MODE_...

    public Int16[] finger_pos = new Int16[2];
    public Boolean finger_disp;

    public Byte[] shadow_bone = new Byte[2];
    public Byte shadow_x;
    public Byte shadow_z;

    public UInt32[] sa;
    public HashSet<SupportAbility> saExtended;
    public List<SupportingAbilityFeature> saMonster;

    public Byte sel_menu;

    public Byte typeNo;

    public String idleAnimationName;

    public Vector3 original_pos;

    public GameObject gameObject;

    public GEOTEXHEADER texanimptr;
    public GEOTEXHEADER tranceTexanimptr;

    public Byte[] backupGeotex;
    public Byte[] backupGeoTrancetex;

    public UInt16 flags;
    public UInt32 meshflags;
    public UInt16 weaponFlags;
    public UInt32 weaponMeshFlags;

    public Int32 meshCount;
    public Int32 weaponMeshCount;

    public Renderer[] weaponRenderer;

    public Int32 attachOffset = 0;

    public GameObject originalGo;
    public GameObject tranceGo;

    public Boolean[] meshIsRendering;
    public Boolean weaponIsRendering = true;
    public Boolean battleModelIsRendering = true;

    public String currentAnimationName;

    public Int32 height;
    public Int32 radius_effect; // big radius (Scan etc...)
    public UInt16 radius_collision; // small radius (range for attacks etc...)

    public Vector3 targetPos;
    public Int16 targetFrame;

    public Animation animation;

    public Boolean out_of_reach; // Instead of considering the global battle flag "NoNeighboring", we use a flag for each BTL_DATA

    public UInt16 summon_count; // Counter of the number of uses of a summon spell in a battle

    public Int16 critical_rate_deal_bonus; // Absolute increase/decrease in the % of critical strikes dealt by the BTL_DATA
    public Int16 critical_rate_receive_resistance; // Absolute increase/decrease in the % of critical strikes dealt to the BTL_DATA

    public Int32 geo_scale_x; // For geo.geoScaleSet
    public Int32 geo_scale_y;
    public Int32 geo_scale_z;
    public Int32 geo_scale_default;
    public Boolean enable_trance_glow;

    public Boolean animEndFrame;
    public String endedAnimationName;

    public UInt32 maxDamageLimit;
    public UInt32 maxMpDamageLimit;

    public Color uiColorHP;
    public Color uiColorMP;
    public String uiLabelHP;
    public String uiLabelMP;
    public String uiSpriteATB;

    public Vector3 geoScaleStatus;
    public Single animSpeedStatusFactor;

    public List<DelayedModifier> delayedModifierList = new List<DelayedModifier>();

    public BTL_DATA killer_track;

    public Boolean builtin_weapon_mode;

    public Boolean is_monster_transform;
    public MONSTER_TRANSFORM monster_transform;

    public class MONSTER_TRANSFORM
    {
        public BattleCommandId base_command;
        public BattleCommandId new_command;
        public AA_DATA[] attack;
        public List<AA_DATA> spell;
        public Boolean replace_point;
        public Boolean replace_stat;
        public Boolean replace_defence;
        public Boolean replace_element;
        public Boolean cancel_on_death;
        public Byte[] cam_bone = new Byte[3];
        public Byte[] icon_bone = new Byte[6];
        public SByte[] icon_y = new SByte[6];
        public SByte[] icon_z = new SByte[6];
        public BattleStatus resist_added;
        public BattleStatus auto_added;
        public UInt16 death_sound;
        public Int32 fade_counter;
        public List<BattleCommandId> disable_commands;
        public String[] motion_normal;
        public String[] motion_alternate;
    }

    public class DelayedModifier
    {
        public delegate Boolean IsDelayedDelegate(BattleUnit btl);
        public delegate void ApplyDelegate(BattleUnit btl);

        public IsDelayedDelegate isDelayed = null;
        public ApplyDelegate apply = null;
    }
}
