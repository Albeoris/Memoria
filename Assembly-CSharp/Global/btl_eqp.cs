using FF9;
using Memoria.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class btl_eqp
{
    
    public static void InitWeapon(PLAYER p, BTL_DATA btl)
    {
        btl.builtin_weapon_mode = false;
        btl.weapon = ff9item.GetItemWeapon(p.equip[0]);
        p.wep_bone = btl_mot.BattleParameterList[p.info.serial_no].WeaponBone;
        if (btl.weapon.ModelId != UInt16.MaxValue)
        {
            String modelName = FF9BattleDB.GEO.GetValue(btl.weapon.ModelId);
            if (modelName.Contains("GEO_WEP"))
                btl.weapon_geo = ModelFactory.CreateModel("BattleMap/BattleModel/battle_weapon/" + modelName + "/" + modelName, true);
            else
                btl.weapon_geo = ModelFactory.CreateModel(modelName, true);      
            if (btl.weapon_geo == null)
                btl.weapon_geo = new GameObject("Dummy weapon");
            if (EnemyBuiltInWeaponTable.ContainsKey(btl.dms_geo_id))
                btl.builtin_weapon_mode = true;
        }
        else
        {
            btl.weapon_geo = new GameObject("Dummy weapon");
        }
        MeshRenderer[] componentsInChildren = btl.weapon_geo.GetComponentsInChildren<MeshRenderer>();
        btl.weaponMeshCount = componentsInChildren.Length;
        btl.weaponRenderer = new Renderer[btl.weaponMeshCount];
        if (btl.weaponMeshCount > 0)
        {
            for (Int32 i = 0; i < btl.weaponMeshCount; i++)
            {
                btl.weaponRenderer[i] = componentsInChildren[i].GetComponent<Renderer>();
                if (btl.weapon.CustomTexture != null && btl.weapon.CustomTexture.Length > i && !String.IsNullOrEmpty(btl.weapon.CustomTexture[i]))
                {
                    btl.weaponRenderer[i].material.mainTexture = AssetManager.Load<Texture2D>(btl.weapon.CustomTexture[i], false);
                }
            }
        }
        else if (btl.weapon.CustomTexture != null) // Other kind of model have no btl.weaponMeshCount
        {
            ModelFactory.ChangeModelTexture(btl.weapon_geo, btl.weapon.CustomTexture);
        } 
        btl_util.SetBBGColor(btl.weapon_geo);
        if (btl.is_monster_transform)
            geo.geoAttach(btl.weapon_geo, btl.originalGo, p.wep_bone);
        else
            geo.geoAttach(btl.weapon_geo, btl.gameObject, p.wep_bone);
        InitOffSetWeapon(btl);
    }

    public static void InitEquipPrivilegeAttrib(PLAYER p, BTL_DATA btl)
    {
        btl.def_attr.invalid = (btl.def_attr.absorb = (btl.def_attr.half = (btl.def_attr.weak = (btl.p_up_attr = 0))));
        for (Int32 i = 0; i < 5; i++)
        {
            if (p.equip[i] != RegularItem.NoItem)
            {
                btl_init.IncrementDefAttr(btl.def_attr, ff9equip.ItemStatsData[ff9item._FF9Item_Data[p.equip[i]].bonus].def_attr);
                btl.p_up_attr = (Byte)(btl.p_up_attr | ff9equip.ItemStatsData[ff9item._FF9Item_Data[p.equip[i]].bonus].p_up_attr);
            }
        }
    }

    public static void ProcessBuiltInWeapon()
    {
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            if (btl.builtin_weapon_mode && btl.bi.disappear == 0 && !btl.is_monster_transform && btl_eqp.EnemyBuiltInWeaponTable.TryGetValue(btl.dms_geo_id, out Int32 weaponBoneID))
            {
                Transform builtInBone = null;
                if (btl.gameObject == btl.originalGo)
                {
                    builtInBone = btl.originalGo.transform.GetChildByName($"bone{weaponBoneID:D3}");
                }
                else if (btl.gameObject == btl.tranceGo && btl.bi.player != 0)
                {
                    builtInBone = btl.tranceGo.transform.GetChildByName($"bone{weaponBoneID:D3}");
                }
                if (builtInBone != null)
                    builtInBone.localScale = SCALE_INVISIBLE;
                if (btl.weapon_geo != null && btl.weapon_bone == weaponBoneID)
                    btl.weapon_geo.transform.localScale = builtInBone != null ? SCALE_REBALANCE : Vector3.one;

                CharacterBattleParameter btlParam = btl_mot.BattleParameterList[btl_util.getSerialNumber(btl)];
                if (btlParam.WeaponSize.Any(off => off != 0f))
                {
                    Vector3 WeaponSizeVector = new Vector3(btlParam.WeaponSize[0], btlParam.WeaponSize[1], btlParam.WeaponSize[2]);
                    btl.weapon_geo.transform.localScale = Vector3.Scale(WeaponSizeVector, builtInBone != null && btl.weapon_bone == weaponBoneID ? SCALE_REBALANCE : Vector3.one);
                }
            }
            InitOffSetWeapon(btl);
        }
    }

    public static void InitOffSetWeapon(BTL_DATA btl)
    {
        if (btl.bi.player != 0)
        {
            CharacterBattleParameter btlParam = btl_mot.BattleParameterList[btl_util.getSerialNumber(btl)];

            if (btlParam.WeaponOffsetPos.Any(off => off != 0f)) // Don't edit values if all values are 0
            {
                Single[] CurrentWeaponOffsetPos;
                if (btl_stat.CheckStatus(btl, BattleStatus.Trance) && btlParam.TranceWeaponOffsetPos.Any(off => off != 0f))
                    CurrentWeaponOffsetPos = btlParam.TranceWeaponOffsetPos;
                else
                    CurrentWeaponOffsetPos = btlParam.WeaponOffsetPos;

                btl.weapon_geo.transform.localPosition = new Vector3(CurrentWeaponOffsetPos[0], CurrentWeaponOffsetPos[1], CurrentWeaponOffsetPos[2]);
            }
            if (btlParam.WeaponOffsetRot.Any(off => off != 0f)) // Don't edit values if all values are 0
            {
                Single[] CurrentWeaponOffsetRot;
                if (btl_stat.CheckStatus(btl, BattleStatus.Trance) && btlParam.TranceWeaponOffsetRot.Any(off => off != 0f))
                    CurrentWeaponOffsetRot = btlParam.TranceWeaponOffsetRot;
                else
                    CurrentWeaponOffsetRot = btlParam.WeaponOffsetRot;

                btl.weapon_geo.transform.localRotation = Quaternion.Euler(CurrentWeaponOffsetRot[0], CurrentWeaponOffsetRot[1], CurrentWeaponOffsetRot[2]);
            }

            if (btl_eqp.EnemyBuiltInWeaponTable.TryGetValue(btl.dms_geo_id, out Int32 weaponBoneID)) 
                if (btl.weapon_bone == weaponBoneID)
                    btl.weapon_geo.transform.localPosition *= SCALE_REBALANCE.x;
        }
        else
        {
            if (btl.weapon_geo != null)
            {
                if (btl.weapon_offset_pos.Any(off => off != 0f))
                {
                    btl.weapon_geo.transform.localPosition = new Vector3(btl.weapon_offset_pos[0], btl.weapon_offset_pos[1], btl.weapon_offset_pos[2]);
                }
                if (btl.weapon_offset_rot.Any(off => off != 0f))
                {
                    btl.weapon_geo.transform.localRotation = Quaternion.Euler(btl.weapon_offset_rot[0], btl.weapon_offset_rot[1], btl.weapon_offset_rot[2]);
                }
                btl.weapon_geo.transform.localScale = btl.weapon_scale;
            }
        }
    }

    // TODO: maybe apply the system to the enemies as well when there's a sb2MonParm.WeaponModel
    public static Dictionary<Int32, Int32> EnemyBuiltInWeaponTable = new Dictionary<Int32, Int32>
    {
        { 152, 30 }, // Goblin - GEO_MON_B3_001
        { 162, 36 }, // Skeleton - GEO_MON_B3_015
		{ 5459, 6 }, // Lizard Man (weapon 1) - GEO_MON_B3_019
		{ 5459, 24 }, // Lizard Man (weapon 2) - GEO_MON_B3_019
		{ 135, 40 }, // Vice - GEO_MON_B3_022
		{ 138, 29 }, // Sahagin - GEO_MON_B3_025
		{ 136, 25 }, // Lady Bug - GEO_MON_B3_027
		{ 148, 28 }, // Drakan - GEO_MON_B3_035
		{ 85, 10 }, // Ramya - GEO_MON_B3_041
		{ 46, 23 }, // Goblin Mage - GEO_MON_B3_043
		{ 92, 23 }, // Gnoll (weapon 1) - GEO_MON_B3_049
		{ 92, 48 }, // Gnoll (weapon 2) - GEO_MON_B3_049
		{ 184, 18 }, // Tomberry (weapon) - GEO_MON_B3_054
		{ 184, 28 }, // Tomberry (lantern) - GEO_MON_B3_054
		{ 265, 33 }, // Magic Vice - GEO_MON_B3_059
		{ 328, 52 }, // Agares - GEO_MON_B3_070
		
		{ 380, 21 }, // Fratley - GEO_SUB_F0_FLT
        { 410, 31 }, // Lani - GEO_MON_B3_122 or GEO_MON_UP3_122
        { 301, 17 }, // Baku - GEO_MON_B3_105
        { 359, 19 } // King Leo - GEO_MON_B3_106
    };

    private static readonly Vector3 SCALE_INVISIBLE = new Vector3(0.01f, 0.01f, 0.01f);
    private static readonly Vector3 SCALE_REBALANCE = new Vector3(100f, 100f, 100f);
}
