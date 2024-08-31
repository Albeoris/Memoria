﻿using FF9;
using Memoria;
using Memoria.Data;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BattlePlayerCharacter : MonoBehaviour
{
    public static String GetCasterAnimationName(String nameId, BattlePlayerCharacter.PlayerMotionIndex motID)
    {
        return "ANH_MAIN_" + nameId + "_" + BattlePlayerCharacter.PlayerMotionNum[(Int32)motID];
    }

    public static String GetCasterAnimationNameID(String name)
    {
        String key = name.Substring(9, 6);
        String text = BattlePlayerCharacter.PlayerModelToAnimationID.TryGetValue(key, out text) ? text : "no key";
        return text == "no key" ? name : text;
    }

    public static Int32 PlayMotion(GameObject goPlayerCharacter, BattlePlayerCharacter.PlayerMotionIndex playerMotionIndex)
    {
        Animation component = goPlayerCharacter.GetComponent<Animation>();
        String casterAnimationNameID = BattlePlayerCharacter.GetCasterAnimationNameID(component.name);
        String casterAnimationName = BattlePlayerCharacter.GetCasterAnimationName(casterAnimationNameID, playerMotionIndex);
        Int32 result = 0;
        if (component[casterAnimationName] != null)
        {
            Int32 num = Mathf.CeilToInt(component[casterAnimationName].length * component[casterAnimationName].clip.frameRate);
            result = num;
            component.Play(casterAnimationName);
        }
        return result;
    }

    public static void InitMaxFrame(BTL_DATA btl, Byte[] maxFrames)
    {
        Animation component = btl.gameObject.GetComponent<Animation>();
        String dbgMessage = String.Empty;
        for (Int32 i = 0; i < btl.mot.Length; i++)
        {
            String motAnim = btl.mot[i];
            maxFrames[i] = 1;
            if (component[motAnim] != null)
            {
                component[motAnim].speed = 0.5f * FF9StateSystem.Settings.FastForwardFactor;
                maxFrames[i] = (Byte)Mathf.CeilToInt(component[motAnim].length * component[motAnim].clip.frameRate);
            }
            dbgMessage += "animName:" + motAnim + ":" + maxFrames[i] + "\n";
        }
        global::Debug.Log(dbgMessage);
    }

    public static void InitAnimation(BTL_DATA btl)
    {
        for (Int32 i = 0; i < 34; i++)
        {
            String motAnim = btl.mot[i];
            if (btl.gameObject.GetComponent<Animation>()[motAnim] != null)
            {
                AnimationState animationState = btl.gameObject.GetComponent<Animation>()[motAnim];
                if (i == 0 || i == 1 || i == 9 || i == 13 || i == 19 || i == 27 || i == 17 || animationState.name.Substring(animationState.name.Length - 3) == "000")
                    animationState.wrapMode = WrapMode.Loop;
                else
                    animationState.wrapMode = WrapMode.Once;
                animationState.speed = 0.5f * FF9StateSystem.Settings.FastForwardFactor;
            }
        }
    }

    public static void CreatePlayer(BTL_DATA btl, PLAYER p)
    {
        if (btl.gameObject != null)
            UnityEngine.Object.Destroy(btl.gameObject);
        CharacterBattleParameter btlParam = btl_mot.BattleParameterList[p.info.serial_no];
        p.wep_bone = btlParam.WeaponBone;
        btl.gameObject = ModelFactory.CreateModel(btlParam.ModelId, true, true, Configuration.Graphics.ElementsSmoothTexture);
        btl.originalGo = btl.gameObject;
        BattlePlayerCharacter.CreateTranceModel(btl, btlParam.TranceModelId);
        if (p.info.serial_no == CharacterSerialNumber.ZIDANE_SWORD)
        {
            BattlePlayerCharacter.HideZidaneOrichalcum(btl.gameObject);
            BattlePlayerCharacter.HideZidaneOrichalcum(btl.tranceGo);
        }
    }

    private static void CreateTranceModel(BTL_DATA btl, String path)
    {
        if (btl.tranceGo != null)
            UnityEngine.Object.Destroy(btl.tranceGo);
        btl.tranceGo = ModelFactory.CreateModel(path, true, true, Configuration.Graphics.ElementsSmoothTexture);
        btl.tranceGo.transform.localPosition = new Vector3(btl.tranceGo.transform.localPosition.x, -10000f, btl.tranceGo.transform.localPosition.z);
        // Set custom trance texture, if they exist
        String tranceTexturePath = Path.GetDirectoryName(ModelFactory.GetRenameModelPath(ModelFactory.CheckUpscale(path))) + "/%_trance.png";
        foreach (Renderer renderer in btl.tranceGo.GetComponentsInChildren<Renderer>())
        {
            String externalPath = AssetManager.SearchAssetOnDisc(tranceTexturePath.Replace("%", renderer.material.mainTexture.name), true, false);
            if (!String.IsNullOrEmpty(externalPath))
            {
                Texture texture = AssetManager.LoadFromDisc<Texture2D>(externalPath, "");
                texture.name = renderer.material.mainTexture.name;
                renderer.material.mainTexture = texture;
            }
        }
        SkinnedMeshRenderer[] componentsInChildren = btl.tranceGo.GetComponentsInChildren<SkinnedMeshRenderer>();
        MeshRenderer[] componentsInChildren2 = btl.tranceGo.GetComponentsInChildren<MeshRenderer>();
        NormalSolver.SmoothCharacterMesh(componentsInChildren);
        NormalSolver.SmoothCharacterMesh(componentsInChildren2);
        btl.tranceGo.SetActive(false);
    }

    private static void HideZidaneOrichalcum(GameObject characterGo)
    {
        Transform childByName = characterGo.transform.GetChildByName("battle_model");
        if (childByName != null)
        {
            Renderer[] renderers = childByName.GetComponentsInChildren<Renderer>();
            for (Int32 i = 0; i < renderers.Length; i++)
                renderers[i].enabled = false;
        }
    }

    public static readonly String[] PlayerMotionNum = new String[] // Indented by "PlayerMotionIndex"
    {
        "000", // MP_IDLE_NORMAL
        "022", // MP_IDLE_DYING
        "300", // MP_DAMAGE1
        "310", // MP_DAMAGE2
        "033", // MP_DISABLE
        "020", // MP_GET_UP_DYING
        "032", // MP_GET_UP_DISABLE
        "002", // MP_DOWN_DYING
        "023", // MP_DOWN_DISABLE
        "011", // MP_IDLE_CMD
        "001", // MP_NORMAL_TO_CMD
        "021", // MP_DYING_TO_CMD
        "400", // MP_IDLE_TO_DEF
        "401", // MP_DEFENCE
        "402", // MP_DEF_TO_IDLE
        "410", // MP_COVER
        "420", // MP_AVOID
        "430", // MP_ESCAPE
        "500", // MP_WIN
        "501", // MP_WIN_LOOP
        "100", // MP_SET
        "101", // MP_RUN
        "102", // MP_RUN_TO_ATTACK
        "103", // MP_ATTACK
        "104", // MP_BACK
        "105", // MP_ATK_TO_NORMAL
        "200", // MP_IDLE_TO_CHANT
        "201", // MP_CHANT
        "202", // MP_MAGIC
        "040", // MP_STEP_FORWARD
        "050", // MP_STEP_BACK
        "210", // MP_ITEM1
        "010", // MP_CMD_TO_NORMAL
        "220"  // MP_SPECIAL1
    };

    public static readonly Dictionary<String, String> PlayerModelToAnimationID = new Dictionary<String, String>
    {
        {
            "B0_000",
            "B0_000"
        },
        {
            "B0_001",
            "B0_001"
        },
        {
            "B0_006",
            "B0_006"
        },
        {
            "B0_002",
            "B0_002"
        },
        {
            "B0_003",
            "B0_003"
        },
        {
            "B0_004",
            "B0_004"
        },
        {
            "B0_005",
            "B0_005"
        },
        {
            "B0_007",
            "B0_007"
        },
        {
            "B0_018",
            "B0_007"
        },
        {
            "B0_008",
            "B0_008"
        },
        {
            "B0_009",
            "B0_009"
        },
        {
            "B0_010",
            "B0_010"
        },
        {
            "B0_011",
            "B0_011"
        },
        {
            "B0_012",
            "B0_012"
        },
        {
            "B0_013",
            "B0_013"
        },
        {
            "B0_014",
            "B0_014"
        },
        {
            "B0_015",
            "B0_015"
        },
        {
            "B0_016",
            "B0_016"
        },
        {
            "B0_017",
            "B0_017"
        },
        {
            "F0_ZDN",
            "B0_000"
        },
        {
            "F0_GRN",
            "B0_002"
        }
    };

    // Enemies have 6 default battle animations that more or less relate to the first 6 PlayerMotionIndex:
    // Idle, Idle Alternate, Hit, Hit Alternate, Death, Death Alternate
    public enum PlayerMotionIndex
    {
        MP_IDLE_NORMAL,
        MP_IDLE_DYING,
        MP_DAMAGE1,
        MP_DAMAGE2,
        MP_DISABLE,
        MP_GET_UP_DYING,
        MP_GET_UP_DISABLE,
        MP_DOWN_DYING,
        MP_DOWN_DISABLE,
        MP_IDLE_CMD,
        MP_NORMAL_TO_CMD,
        MP_DYING_TO_CMD,
        MP_IDLE_TO_DEF,
        MP_DEFENCE,
        MP_DEF_TO_IDLE,
        MP_COVER,
        MP_AVOID,
        MP_ESCAPE,
        MP_WIN,
        MP_WIN_LOOP,
        MP_SET,
        MP_RUN,
        MP_RUN_TO_ATTACK,
        MP_ATTACK,
        MP_BACK,
        MP_ATK_TO_NORMAL,
        MP_IDLE_TO_CHANT,
        MP_CHANT,
        MP_MAGIC,
        MP_STEP_FORWARD,
        MP_STEP_BACK,
        MP_ITEM1,
        MP_CMD_TO_NORMAL,
        MP_SPECIAL1,
        MP_MAX
    }

    public enum PlayerMotionStance
    {
        NORMAL,
        DYING,
        DISABLE,
        CMD,
        DEFEND,
        WIN,
        SPECIAL_ANY_IDLE,
        SPECIAL_INDIFFERENT,
        SPECIAL_UNKNOWN
    }
}
