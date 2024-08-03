using FF9;
using Memoria.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria;

public static class btl_eqp
{
    public static void InitWeapon(PLAYER p, BTL_DATA btl)
    {
        if (btl.weapon_geo != null)
            UnityEngine.Object.Destroy(btl.weapon_geo);
        btl.builtin_weapon_mode = false;
        btl.weapon = ff9item.GetItemWeapon(p.equip[0]);
        if (btl.weapon.ModelId != UInt16.MaxValue)
        {
            String modelName = FF9BattleDB.GEO.GetValue(btl.weapon.ModelId);
            if (modelName.Contains("GEO_WEP"))
                btl.weapon_geo = ModelFactory.CreateModel("BattleMap/BattleModel/battle_weapon/" + modelName + "/" + modelName, true, true, Configuration.Graphics.ElementsSmoothTexture);
            else
                btl.weapon_geo = ModelFactory.CreateModel(modelName, true, true, Configuration.Graphics.ElementsSmoothTexture);
            if (btl.weapon_geo == null)
                btl.weapon_geo = new GameObject(DummyWeaponName);
            if (EnemyBuiltInWeaponTable.ContainsKey(btl.dms_geo_id))
                btl.builtin_weapon_mode = true;
        }
        else
        {
            btl.weapon_geo = new GameObject(DummyWeaponName);
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
                    btl.weaponRenderer[i].material.mainTexture = AssetManager.Load<Texture2D>(btl.weapon.CustomTexture[i], false);
            }
        }
        else if (btl.weapon.CustomTexture != null) // Other kind of model have no btl.weaponMeshCount
        {
            ModelFactory.ChangeModelTexture(btl.weapon_geo, btl.weapon.CustomTexture);
        } 
        btl_util.SetBBGColor(btl.weapon_geo);
        if (btl.is_monster_transform && !btl.builtin_weapon_mode)
            geo.geoAttach(btl.weapon_geo, btl.originalGo, btl.weapon_bone);
        else
            geo.geoAttach(btl.weapon_geo, btl.gameObject, btl.weapon_bone);
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

    public static void UpdateWeaponOffsets()
    {
        for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
        {
            Boolean rebalanceWeaponScale = false;
            if (btl.builtin_weapon_mode && btl.bi.disappear == 0 && EnemyBuiltInWeaponTable.TryGetValue(btl.dms_geo_id, out Int32[] weaponBoneIDs))
            {
                foreach (Int32 boneID in weaponBoneIDs)
                {
                    Transform builtInBone = btl.gameObject.transform.GetChildByName($"bone{boneID:D3}");
                    if (builtInBone != null)
                    {
                        builtInBone.localScale = SCALE_INVISIBLE;
                        if (boneID == btl.weapon_bone)
                            rebalanceWeaponScale = true;
                    }
                }
            }
            if (btl.weapon_geo != null)
            {
                if (rebalanceWeaponScale)
                {
                    btl.weapon_geo.transform.localScale = Vector3.Scale(btl.weapon_scale, SCALE_REBALANCE);
                    btl.weapon_geo.transform.localPosition = btl.weapon_offset_pos * SCALE_REBALANCE.x;
                }
                else
                {
                    btl.weapon_geo.transform.localScale = btl.weapon_scale;
                    btl.weapon_geo.transform.localPosition = btl.weapon_offset_pos;
                }
                btl.weapon_geo.transform.localRotation = Quaternion.Euler(btl.weapon_offset_rot);
            }
        }
    }

    public static Dictionary<Int32, Int32[]> EnemyBuiltInWeaponTable = new Dictionary<Int32, Int32[]>
    {
        { 152, [30] }, // Goblin - GEO_MON_B3_001
        { 162, [36] }, // Skeleton - GEO_MON_B3_015
		{ 5459, [6, 24] }, // Lizard Man - GEO_MON_B3_019
		{ 135, [40] }, // Vice - GEO_MON_B3_022
		{ 138, [29] }, // Sahagin - GEO_MON_B3_025
		{ 136, [25] }, // Lady Bug - GEO_MON_B3_027
		{ 148, [28] }, // Drakan - GEO_MON_B3_035
		{ 85, [10] }, // Ramya - GEO_MON_B3_041
		{ 46, [23] }, // Goblin Mage - GEO_MON_B3_043
		// { 92, [23] }, // Gnoll (weapon 2) - GEO_MON_B3_049, kind of shield.... ?
		{ 92, [48] }, // Gnoll (weapon 1) - GEO_MON_B3_049
		{ 184, [18] }, // Tomberry (weapon) - GEO_MON_B3_054 (bone n°28 => Lantern)
        // { 184, [28] }, // Tomberry (lantern) - GEO_MON_B3_054
		{ 265, [33] }, // Magic Vice - GEO_MON_B3_059
		{ 328, [52] }, // Agares - GEO_MON_B3_070
        { 327, [24, 15] }, // Ogre - GEO_MON_B3_071
        { 326, [27] }, // Troll - GEO_MON_B3_073
        // { 326, [19] }, // Troll - GEO_MON_B3_073 (shield)
        { 343, [29] }, // Iron Man - GEO_MON_B3_094
        { 344, [26] }, // Armodullahan - GEO_MON_B3_103
        // { 344, [10] }, // Armodullahan - GEO_MON_B3_103 (shield)
        { 301, [17] }, // Baku - GEO_MON_B3_105
        { 359, [19] }, // King Leo - GEO_MON_B3_106
        { 428, [17] }, // Masked Man - GEO_MON_B3_107
        { 450, [17] }, // Black Waltz 1 - GEO_MON_B3_111
        { 278, [26] }, // Black Waltz 3 (normal) - GEO_MON_B3_115
        { 593, [16] }, // Black Waltz 3 (broken) - GEO_MON_B3_116
        { 410, [31] }, // Lani - GEO_MON_B3_122 or GEO_MON_UP3_122
        { 38, [36, 41, 46, 65, 70, 75] }, // Maliris - GEO_MON_B3_141
        { 619, [36, 41, 46, 65, 70, 75] }, // Maliris (crystal) - GEO_MON_B3_192
        { 576, [9] }, // Hades - GEO_MON_B3_146
        { 405, [25] }, // Friendly Lady Bug - GEO_MON_B3_159
        { 445, [10] }, // Quale - GEO_MON_B3_181
        { 546, [29] }, // Cave Imp - GEO_MON_B3_189
        { 534, [49] }, // Ramuh - GEO_MON_F0_RAM
        { 217, [12] }, // Lindblum Soldier - GEO_NPC_F0_CSO
        { 431, [12] }, // Gaza Priest - GEO_NPC_F0_NAN
        { 218, [12] }, // Lindblum Elite Soldier - GEO_NPC_F2_CSO
        { 182, [12] }, // SouthGate Soldier - GEO_NPC_F4_CSO
        { 106, [28] }, // Fratley - GEO_SUB_F0_BRN
        { 167, [26] }, // Black Waltz 3 (normal) - GEO_SUB_F0_BW3
        { 107, [25] }, // Cinna - GEO_SUB_F0_CNA
        { 380, [21] }, // Fratley - GEO_SUB_F0_FLT
        { 207, [16] } // Black Waltz 3 (broken) - GEO_SUB_F1_BW3
    };

    public const String DummyWeaponName = "Dummy weapon";

    private static readonly Vector3 SCALE_INVISIBLE = new Vector3(0.01f, 0.01f, 0.01f);
    private static readonly Vector3 SCALE_REBALANCE = new Vector3(100f, 100f, 100f);
}
