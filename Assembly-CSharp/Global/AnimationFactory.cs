using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnimationFactory
{
    public static void LoadAnimationUseInEvent(String animationEventData)
    {
        AnimationFactory.animationEventClip.Clear();
        AnimationFactory.animationMapping.Clear();
        String animDB = AssetManager.LoadString("CommonAsset/EventEngine/EventAnimation/" + animationEventData + ".txt");
        if (animDB == null)
            return;
        animDB = animDB.Replace("\r", String.Empty);
        if (!animDB.StartsWith("animation:"))
            return;
        String[] allLines = animDB.Split(new Char[] { '\n' });
        String[] allAnims = allLines[0].Replace("animation:", String.Empty).Split(new Char[] { ',' });
        for (Int32 i = 0; i < allAnims.Length; i++)
        {
            String animName = allAnims[i];
            if (!String.IsNullOrEmpty(animName) && !AnimationFactory.animationEventClip.ContainsKey(animName))
            {
                String animationFolder = AnimationFactory.GetAnimationFolder(animName);
                String animPath = "Animations/" + animationFolder + "/" + animName;
                animPath = AnimationFactory.GetRenameAnimationPath(animPath);
                AnimationClip value = AssetManager.Load<AnimationClip>(animPath, false);
                AnimationFactory.animationEventClip.Add(animName, value);
            }
        }
        for (Int32 i = 1; i < allLines.Length; i++)
        {
            String modelAnimDB = allLines[i];
            if (!String.IsNullOrEmpty(modelAnimDB))
            {
                String modelName = modelAnimDB.Split(new Char[] { ':' })[0];
                modelAnimDB = modelAnimDB.Replace(modelName + ":", String.Empty);
                String[] animNames = modelAnimDB.Split(new Char[] { ',' });
                AnimationFactory.animationMapping.Add(modelName, animNames);
            }
        }
    }

    private static String GetAnimationFolder(String animationName)
    {
        String text = String.Empty;
        String[] array = animationName.Split(new Char[]
        {
            '_'
        });
        text = String.Concat(new String[]
        {
            "GEO_",
            array[1],
            "_",
            array[2],
            "_",
            array[3]
        });
        if (AnimationFactory.animationMapping.ContainsKey(text))
        {
            text = AnimationFactory.animationPathTable[text];
        }
        return text;
    }

    public static void AddAnimWithAnimatioName(GameObject go, String animationName)
    {
        Animation component = go.GetComponent<Animation>();
        if (component.GetClip(animationName) == (UnityEngine.Object)null)
        {
            String[] array = animationName.Split(new Char[]
            {
                '_'
            });
            String str = String.Concat(new String[]
            {
                "GEO_",
                array[1],
                "_",
                array[2],
                "_",
                array[3]
            });
            String text = "Animations/" + str + "/" + animationName;
            text = AnimationFactory.GetRenameAnimationPath(text);
            AnimationClip clip = AssetManager.Load<AnimationClip>(text, false);
            if (clip != null)
                component.AddClip(clip, animationName);
        }
    }

    public static void AddAnimToGameObject(GameObject go, String modelName, Boolean addAutoAnim = false)
    {
        Animation goAnimations = go.GetComponent<Animation>();
        if (AnimationFactory.animationMapping.ContainsKey(modelName))
        {
            String[] animNameList = AnimationFactory.animationMapping[modelName];
            for (Int32 i = 0; i < animNameList.Length; i++)
            {
                String animName = animNameList[i];
                AnimationClip clip = AnimationFactory.animationEventClip[animName];
                goAnimations.AddClip(clip, animName);
            }
        }
        if (addAutoAnim)
        {
            String animPath = "Animations/" + modelName;
            AnimationClip[] modelAnim = AssetManager.LoadAll<AnimationClip>(animPath);
            if (modelAnim == null)
                return;
            for (Int32 i = 0; i < modelAnim.Length; i++)
            {
                AnimationClip clip = modelAnim[i];
                Int32 animKey;
                String animName = clip.name;
                if (Int32.TryParse(clip.name, out animKey))
                    animName = FF9DBAll.AnimationDB.GetValue(animKey);
                if (!AnimationFactory.animationEventClip.ContainsKey(animName))
                    AnimationFactory.animationEventClip.Add(animName, clip);
                goAnimations.AddClip(clip, animName);
            }
            // Wraith (Ice): the following code used to load Wraith (Fire)'s casting animations but it wasn't enough
            // The current fix is to make Wraith (Ice) use Wraith (Ice)'s casting animations
            //if (modelName.CompareTo("GEO_MON_B3_034") == 0)
            //{
            //	String[] array5 = new String[]
            //	{
            //		"Animations/GEO_MON_B3_156/ANH_MON_B3_156_020",
            //		"Animations/GEO_MON_B3_156/ANH_MON_B3_156_021",
            //		"Animations/GEO_MON_B3_156/ANH_MON_B3_156_022"
            //	};
            //	for (Int32 k = 0; k < (Int32)array5.Length; k++)
            //	{
            //		String renameAnimationPath = AnimationFactory.GetRenameAnimationPath(array5[k]);
            //		String[] animInfo;
            //		AnimationClip animationClip2 = AssetManager.Load<AnimationClip>(renameAnimationPath, out animInfo, false);
            //		String name2 = animationClip2.name;
            //		component.AddClip(animationClip2, name2);
            //	}
            //}
        }
    }

    public static String GetRenameAnimationDirectory(String animationDirectory)
    {
        String modelName = Path.GetFileNameWithoutExtension(animationDirectory);
        if (modelName.Equals("GEO_MON_B3_110"))
            return "Animations/347";
        if (modelName.Equals("GEO_MON_B3_109"))
            return "Animations/5461";
        Int32 animKey;
        if (FF9BattleDB.GEO.TryGetKey(modelName, out animKey))
            return "Animations/" + animKey;
        return animationDirectory;
    }

    public static String GetRenameAnimationPath(String animationPath)
    {
        String animName = Path.GetFileNameWithoutExtension(animationPath);
        Int32 animKey;
        if (FF9DBAll.AnimationDB.TryGetKey(animName, out animKey))
        {
            String animDir = Path.GetDirectoryName(animationPath);
            animDir = AnimationFactory.GetRenameAnimationDirectory(animDir);
            return animDir + "/" + animKey;
        }
        return animationPath;
    }

    public static Dictionary<String, String> animationPathTable = new Dictionary<String, String>
    {
        {
            "GEO_MAIN_F1_GRN",
            "GEO_MAIN_F0_GRN"
        },
        {
            "GEO_MAIN_F1_STN",
            "GEO_MAIN_F0_STN"
        },
        {
            "GEO_MAIN_F3_GRN",
            "GEO_MAIN_F4_GRN"
        },
        {
            "GEO_MAIN_F7_VIV",
            "GEO_MAIN_F0_VIV"
        },
        {
            "GEO_NPC_F1_APF",
            "GEO_NPC_F0_APF"
        },
        {
            "GEO_NPC_F1_APM",
            "GEO_NPC_F0_APM"
        },
        {
            "GEO_NPC_F1_BBA",
            "GEO_NPC_F0_BBA"
        },
        {
            "GEO_NPC_F1_CAT",
            "GEO_NPC_F0_CAT"
        },
        {
            "GEO_NPC_F1_CHO",
            "GEO_NPC_F0_CHO"
        },
        {
            "GEO_NPC_F1_DAC",
            "GEO_NPC_F0_DAC"
        },
        {
            "GEO_NPC_F1_DAF",
            "GEO_NPC_F0_DAF"
        },
        {
            "GEO_NPC_F1_DOC",
            "GEO_NPC_F0_DOC"
        },
        {
            "GEO_NPC_F1_DOF",
            "GEO_NPC_F0_DOF"
        },
        {
            "GEO_NPC_F1_DOG",
            "GEO_NPC_F0_DOG"
        },
        {
            "GEO_NPC_F1_DOK",
            "GEO_NPC_F0_DOK"
        },
        {
            "GEO_NPC_F1_DOM",
            "GEO_NPC_F0_DOM"
        },
        {
            "GEO_NPC_F1_G17",
            "GEO_NPC_F0_G17"
        },
        {
            "GEO_NPC_F1_GUD",
            "GEO_NPC_F0_GUD"
        },
        {
            "GEO_NPC_F1_HTH",
            "GEO_NPC_F0_HTH"
        },
        {
            "GEO_NPC_F1_HUF",
            "GEO_NPC_F0_HUF"
        },
        {
            "GEO_NPC_F1_HUM",
            "GEO_NPC_F0_HUM"
        },
        {
            "GEO_NPC_F1_JJY",
            "GEO_NPC_F0_JJY"
        },
        {
            "GEO_NPC_F1_KAC",
            "GEO_NPC_F0_KAC"
        },
        {
            "GEO_NPC_F1_MOG",
            "GEO_NPC_F0_MOG"
        },
        {
            "GEO_NPC_F1_OFF",
            "GEO_NPC_F0_OFF"
        },
        {
            "GEO_NPC_F1_RAS",
            "GEO_NPC_F0_RAS"
        },
        {
            "GEO_NPC_F1_TBY",
            "GEO_NPC_F0_TBY"
        },
        {
            "GEO_NPC_F1_TCK",
            "GEO_NPC_F0_TCK"
        },
        {
            "GEO_NPC_F1_TGR",
            "GEO_NPC_F0_TGR"
        },
        {
            "GEO_NPC_F1_TMF",
            "GEO_NPC_F0_TMF"
        },
        {
            "GEO_NPC_F1_TMM",
            "GEO_NPC_F0_TMM"
        },
        {
            "GEO_NPC_F1_WRK",
            "GEO_NPC_F0_WRK"
        },
        {
            "GEO_NPC_F2_APF",
            "GEO_NPC_F0_APF"
        },
        {
            "GEO_NPC_F2_APM",
            "GEO_NPC_F0_APM"
        },
        {
            "GEO_NPC_F2_BBA",
            "GEO_NPC_F0_BBA"
        },
        {
            "GEO_NPC_F2_CHO",
            "GEO_NPC_F0_CHO"
        },
        {
            "GEO_NPC_F2_DAC",
            "GEO_NPC_F0_DAC"
        },
        {
            "GEO_NPC_F2_DOM",
            "GEO_NPC_F0_DOM"
        },
        {
            "GEO_NPC_F2_G17",
            "GEO_NPC_F0_G17"
        },
        {
            "GEO_NPC_F2_HTH",
            "GEO_NPC_F0_HTH"
        },
        {
            "GEO_NPC_F2_HUM",
            "GEO_NPC_F0_HUM"
        },
        {
            "GEO_NPC_F2_JJY",
            "GEO_NPC_F0_JJY"
        },
        {
            "GEO_NPC_F2_KAC",
            "GEO_NPC_F0_KAC"
        },
        {
            "GEO_NPC_F2_TBY",
            "GEO_NPC_F0_TBY"
        },
        {
            "GEO_NPC_F2_TGR",
            "GEO_NPC_F0_TGR"
        },
        {
            "GEO_NPC_F3_APM",
            "GEO_NPC_F0_APM"
        },
        {
            "GEO_NPC_F3_BBA",
            "GEO_NPC_F0_BBA"
        },
        {
            "GEO_NPC_F3_CHO",
            "GEO_NPC_F0_CHO"
        },
        {
            "GEO_NPC_F3_HUF",
            "GEO_NPC_F0_HUF"
        },
        {
            "GEO_NPC_F3_JJY",
            "GEO_NPC_F0_JJY"
        },
        {
            "GEO_NPC_F3_TBY",
            "GEO_NPC_F0_TBY"
        },
        {
            "GEO_NPC_F3_TGR",
            "GEO_NPC_F0_TGR"
        },
        {
            "GEO_NPC_F4_APM",
            "GEO_NPC_F0_APM"
        },
        {
            "GEO_NPC_F4_CHO",
            "GEO_NPC_F0_CHO"
        },
        {
            "GEO_NPC_F4_CSM",
            "GEO_NPC_F1_CSM"
        },
        {
            "GEO_NPC_F4_CSO",
            "GEO_NPC_F0_CSO"
        },
        {
            "GEO_NPC_F4_JJY",
            "GEO_NPC_F0_JJY"
        },
        {
            "GEO_NPC_F5_CSA",
            "GEO_NPC_F1_CSA"
        },
        {
            "GEO_NPC_F5_CSM",
            "GEO_NPC_F2_CSM"
        },
        {
            "GEO_NPC_F5_MOG",
            "GEO_NPC_F0_MOG"
        },
        {
            "GEO_NPC_F6_CSA",
            "GEO_NPC_F2_CSA"
        },
        {
            "GEO_NPC_F6_CSM",
            "GEO_NPC_F3_CSM"
        },
        {
            "GEO_NPC_F7_CSM",
            "GEO_NPC_F0_CSM"
        },
        {
            "GEO_SUB_F1_ZON",
            "GEO_SUB_F0_ZON"
        },
        {
            "GEO_SUB_F4_SSB",
            "GEO_SUB_F1_SSB"
        },
        {
            "GEO_SUB_F7_BLN",
            "GEO_SUB_F0_BLN"
        },
        {
            "GEO_SUB_F7_CNA",
            "GEO_SUB_F0_CNA"
        },
        {
            "GEO_SUB_F7_MRC",
            "GEO_SUB_F0_MRC"
        },
        {
            "GEO_SUB_F3_KJA",
            "GEO_SUB_F0_KJA"
        },
        {
            "GEO_NPC_F1_OSC",
            "GEO_NPC_F0_OSC"
        },
        {
            "GEO_NPC_F2_OSC",
            "GEO_NPC_F0_OSC"
        },
        {
            "GEO_NPC_F1_FRM",
            "GEO_NPC_F0_FRM"
        },
        {
            "GEO_NPC_F0_FRF",
            "GEO_NPC_F0_FRM"
        },
        {
            "GEO_NPC_F0_FRC",
            "GEO_NPC_F0_FRM"
        },
        {
            "GEO_NPC_F1_G20",
            "GEO_NPC_F0_G20"
        },
        {
            "GEO_NPC_F2_G20",
            "GEO_NPC_F0_G20"
        },
        {
            "GEO_ACC_F2_TBX",
            "GEO_ACC_F0_TBX"
        },
        {
            "GEO_ACC_F3_TBX",
            "GEO_ACC_F1_TBX"
        },
        {
            "GEO_ACC_F1_MGP",
            "GEO_ACC_F0_MGP"
        },
        {
            "GEO_ACC_F2_LTT",
            "GEO_ACC_F0_LTT"
        },
        {
            "GEO_ACC_F1_BLL",
            "GEO_ACC_F0_BLL"
        },
        {
            "GEO_ACC_F2_BLL",
            "GEO_ACC_F0_BLL"
        },
        {
            "GEO_ACC_F3_BLL",
            "GEO_ACC_F0_BLL"
        },
        {
            "GEO_ACC_F0_KGG",
            "GEO_ACC_F0_LEV"
        },
        {
            "GEO_ACC_F0_BBX",
            "GEO_ACC_F0_LEV"
        },
        {
            "GEO_ACC_F0_BBT",
            "GEO_ACC_F0_LEV"
        },
        {
            "GEO_ACC_F0_IFE",
            "GEO_ACC_F0_SUP"
        },
        {
            "GEO_ACC_F0_GAS",
            "GEO_ACC_F0_GAB"
        },
        {
            "GEO_ACC_F0_BON",
            "GEO_ACC_F0_CUP"
        },
        {
            "GEO_ACC_F1_HDB",
            "GEO_ACC_F0_HDB"
        },
        {
            "GEO_ACC_F0_KOR",
            "GEO_ACC_F0_CUP"
        },
        {
            "GEO_ACC_F1_SWD",
            "GEO_ACC_F0_CUP"
        },
        {
            "GEO_ACC_F0_LNW",
            "GEO_ACC_F0_CUP"
        },
        {
            "GEO_ACC_F0_KOM",
            "GEO_ACC_F0_CUP"
        },
        {
            "GEO_ACC_F0_KOS",
            "GEO_ACC_F0_CUP"
        },
        {
            "GEO_ACC_F0_ELE",
            "GEO_ACC_F0_CUP"
        },
        {
            "GEO_ACC_F0_HDB",
            "GEO_ACC_F0_CUP"
        },
        {
            "GEO_ACC_F0_STQ",
            "GEO_ACC_F0_CUP"
        },
        {
            "GEO_MON_F0_DRA",
            "GEO_MON_F0_EFM"
        },
        {
            "GEO_MON_F0_EEE",
            "GEO_MON_F0_EFM"
        },
        {
            "GEO_MON_F0_FFF",
            "GEO_MON_F0_EFM"
        },
        {
            "GEO_MON_F0_WWW",
            "GEO_MON_F0_EFM"
        }
    };

    /*private static String[] defaultAnimation = new String[]
    {
        "IDLE",
        "WALK",
        "RUN",
        "TURN_L",
        "TURN_R"
    };*/

    private static Dictionary<String, AnimationClip> animationEventClip = new Dictionary<String, AnimationClip>();

    private static Dictionary<String, String[]> animationMapping = new Dictionary<String, String[]>();

    public static Int64 timeUse;
}
