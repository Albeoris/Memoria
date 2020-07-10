﻿using System;
using FF9;
using Memoria.Data;
using UnityEngine;

public class BTL_DATA
{
	public void SetDisappear(Byte value)
	{
		this.bi.disappear = value;
		if (this.bi.disappear != 0)
		{
			this.SetActiveBtlData(false);
		}
		else
		{
			this.SetActiveBtlData(true);
		}
	}

	public GameObject getShadow()
	{
		if (this.bi.player != 0)
		{
			return FF9StateSystem.Battle.FF9Battle.map.shadowArray[(Int32)this.bi.slot_no];
		}
		return FF9StateSystem.Battle.FF9Battle.map.shadowArray[(Int32)(9 + this.bi.slot_no)];
	}

	public void SetActiveBtlData(Boolean value)
	{
		GameObject shadow = this.getShadow();
		this.gameObject.SetActive(value);
		if (this.bi.shadow != 0)
		{
			shadow.SetActive(value);
		}
	}

	public void SetIsEnabledMeshRenderer(Int32 mesh, Boolean isEnabled)
	{
		if (this.meshIsRendering[mesh] != isEnabled && this.gameObject != (UnityEngine.Object)null)
		{
			Transform childByName = this.gameObject.transform.GetChildByName("mesh" + mesh);
			if (childByName != (UnityEngine.Object)null)
			{
				Renderer[] componentsInChildren = childByName.GetComponentsInChildren<Renderer>();
				Renderer[] array = componentsInChildren;
				for (Int32 i = 0; i < (Int32)array.Length; i++)
				{
					Renderer renderer = array[i];
					if (renderer.enabled != isEnabled)
					{
						renderer.enabled = isEnabled;
					}
				}
				this.meshIsRendering[mesh] = isEnabled;
			}
		}
	}

	public void SetIsEnabledWeaponRenderer(Boolean isEnabled)
	{
		if (this.weaponIsRendering != isEnabled && this.gameObject != (UnityEngine.Object)null)
		{
			Transform childByName = this.gameObject.transform.GetChildByName("bone000");
			if (childByName != (UnityEngine.Object)null)
			{
				Renderer[] componentsInChildren = childByName.GetComponentsInChildren<Renderer>();
				Renderer[] array = componentsInChildren;
				for (Int32 i = 0; i < (Int32)array.Length; i++)
				{
					Renderer renderer = array[i];
					if (renderer.enabled != isEnabled)
					{
						renderer.enabled = isEnabled;
					}
				}
				this.weaponIsRendering = isEnabled;
			}
		}
	}

	public void SetIsEnabledBattleModelRenderer(Boolean isEnabled)
	{
		if (this.battleModelIsRendering != isEnabled && this.gameObject != (UnityEngine.Object)null)
		{
			Transform childByName = this.gameObject.transform.GetChildByName("battle_model");
			if (childByName != (UnityEngine.Object)null)
			{
				Renderer[] componentsInChildren = childByName.GetComponentsInChildren<Renderer>();
				Renderer[] array = componentsInChildren;
				for (Int32 i = 0; i < (Int32)array.Length; i++)
				{
					Renderer renderer = array[i];
					if (renderer.enabled != isEnabled)
					{
						renderer.enabled = isEnabled;
					}
				}
				this.battleModelIsRendering = isEnabled;
			}
		}
	}

	public BTL_DATA next = null;

	public CMD_DATA[] cmd = new CMD_DATA[6];

	public REFLEC_DATA reflec = new REFLEC_DATA();

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

	public GameObject weapon_geo;

	public UInt16 mesh_current;

	public UInt16 mesh_banish;

	public UInt16 btl_id;

	public Byte tar_mode;

	public Byte sel_mode;

	public Int16[] finger_pos = new Int16[2];

	public Boolean finger_disp;

	public Byte[] shadow_bone = new Byte[2];

	public Byte shadow_x;

	public Byte shadow_z;

	public Byte[] add_col = new Byte[3];

	public UInt32[] sa;

	public Int32 fig_regene_hp;

	public Int32 fig_poison_hp;

	public Int32 fig_poison_mp;

	public Byte fig_stat_info;

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

	public HUDMessageChild deathMessage;

	public HUDMessageChild petrifyMessage;

	public Int32 attachOffset = 0;

	public GameObject originalGo;

	public GameObject tranceGo;

	public Boolean[] meshIsRendering;

	public Boolean weaponIsRendering = true;

	public Boolean battleModelIsRendering = true;

	public String currentAnimationName;

	public Int32 height;

	public Int32 radius;

	public UInt16 frameCount = 1;

	public Vector3 targetPos;

	public Int16 targetFrame;

	public Animation animation;

	// Custom fields
	public Boolean out_of_reach; // Instead of considering the global battle flag "NoNeighboring", we use a flag for each BTL_DATA

	public Boolean[] stat_modifier = new bool[6]; // Str, Mgc, Def, Ev, MgDef, MgEv; Flags checking if a stat has been modified by a spell; re-initialized to "false" when that stat gets modified by script

	public UInt16 summon_count; // Counter of the number of uses of a summon spell in a battle

	public Int16 critical_rate_deal_bonus; // Absolute increase/decrease in the % of critical strikes dealt by the BTL_DATA

	public Int16 critical_rate_receive_bonus; // Absolute increase/decrease in the % of critical strikes dealt to the BTL_DATA
}