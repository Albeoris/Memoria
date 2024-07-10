using FF9;
using Memoria.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class btl_eqp
{
    // TODO: maybe apply the system to the enemies as well when there's a sb2MonParm.WeaponModel
    public static Dictionary<Int32, Int32> EnemyBuiltInWeaponTable = new Dictionary<Int32, Int32>
    {
        { 380, 21 }, // Fratley - GEO_SUB_F0_FLT
        { 410, 31 }, // Lani - GEO_MON_B3_122 or GEO_MON_UP3_122
        { 301, 17 }, // Baku - GEO_MON_B3_105
        { 359, 19 }, // King Leo - GEO_MON_B3_106
    };

    public static void InitWeapon(PLAYER p, BTL_DATA btl)
    {
        btl.builtin_weapon_mode = false;
        btl.weapon = ff9item.GetItemWeapon(p.equip[0]);
        p.wep_bone = btl_mot.BattleParameterList[p.info.serial_no].WeaponBone;
        if (btl.weapon.ModelId != UInt16.MaxValue)
        {
            String modelName = FF9BattleDB.GEO.GetValue(btl.weapon.ModelId);
            btl.weapon_geo = ModelFactory.CreateModel("BattleMap/BattleModel/battle_weapon/" + modelName + "/" + modelName, true);
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
        for (Int32 i = 0; i < btl.weaponMeshCount; i++)
        {
            btl.weaponRenderer[i] = componentsInChildren[i].GetComponent<Renderer>();
            if (btl.weapon.CustomTexture != null && btl.weapon.CustomTexture.Length > i && !String.IsNullOrEmpty(btl.weapon.CustomTexture[i]))
                btl.weaponRenderer[i].material.mainTexture = AssetManager.Load<Texture2D>(btl.weapon.CustomTexture[i], false);
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
            InitOffSetWeapon(btl);
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
                if (btl.weapon_geo != null)
                    btl.weapon_geo.transform.localScale = builtInBone != null ? SCALE_REBALANCE : Vector3.one;
            }
        }
    }

    public static void InitOffSetWeapon(BTL_DATA btl)
    {
        if (btl.bi.player != 0)
        {
            CharacterBattleParameter btlParam = btl_mot.BattleParameterList[btl_util.getSerialNumber(btl)];

            if (btlParam.WeaponOffset.Any(off => off != 0f)) // Don't edit values if all values are 0
            {
                Single[] CurrentWeaponOffset;
                if (btl_stat.CheckStatus(btl, BattleStatus.Trance) && btlParam.TranceWeaponOffset.Any(off => off != 0f))
                    CurrentWeaponOffset = btlParam.TranceWeaponOffset;
                else
                    CurrentWeaponOffset = btlParam.WeaponOffset;

                btl.weapon_geo.transform.localPosition = new Vector3(CurrentWeaponOffset[0], CurrentWeaponOffset[1], CurrentWeaponOffset[2]);
                btl.weapon_geo.transform.localRotation = Quaternion.Euler(CurrentWeaponOffset[3], CurrentWeaponOffset[4], CurrentWeaponOffset[5]);
            }
        }
        else
        {
            if (btl.weapon_offset.Any(off => off != 0f) && btl.weapon_geo != null)
            {
                btl.weapon_geo.transform.localPosition = new Vector3(btl.weapon_offset[0], btl.weapon_offset[1], btl.weapon_offset[2]);
                btl.weapon_geo.transform.localRotation = Quaternion.Euler(btl.weapon_offset[3], btl.weapon_offset[4], btl.weapon_offset[5]);
            }
        }
    }

    private static readonly Vector3 SCALE_INVISIBLE = new Vector3(0.01f, 0.01f, 0.01f);
    private static readonly Vector3 SCALE_REBALANCE = new Vector3(100f, 100f, 100f);
}
