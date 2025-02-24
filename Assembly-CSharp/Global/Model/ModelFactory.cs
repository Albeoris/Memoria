﻿using Assets.Scripts.Common;
using Memoria;
using Memoria.Data;
using Memoria.Scripts;
using Memoria.Assets;
using Memoria.Prime.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ModelFactory
{
    public static String GetRenameModelPath(String upscalePath)
    {
        String text = Path.GetFileNameWithoutExtension(upscalePath);
        if (ModelFactory.revertUpscaleTable.ContainsKey(text))
            text = ModelFactory.revertUpscaleTable[text];
        else
            text = ModelFactory.GetNameFromFF9DBALL(text);
        Int32 geoId = ModelFactory.GetGEOID(text);
        if (geoId == -1)
            return upscalePath;
        ModelType modelType;
        if (text.StartsWith("GEO_WEP"))
        {
            modelType = ModelType.battle_weapon;
            return String.Format("BattleMap/BattleModel/{0}/{1}/{1}", (Int32)modelType, geoId);
        }
        modelType = ModelFactory.GetModelType(upscalePath);
        if (geoId == 429 || geoId == 430)
            modelType = ModelType.sub;
        return String.Format("Models/{0}/{1}/{1}", (Int32)modelType, geoId);
    }

    public static String GetRenameTexturePath(String texturePath)
    {
        OSDLogger.AddStaticMessage("---------End GetRenameTexturePath-----------------");
        String fileNameWithoutExtension = Path.GetFileNameWithoutExtension(texturePath);
        String nameFromFF9DBALL = ModelFactory.GetNameFromFF9DBALL(fileNameWithoutExtension);
        Int32 geoid = ModelFactory.GetGEOID(nameFromFF9DBALL);
        String modelName = FF9BattleDB.GEO.GetValue(geoid);
        ModelType modelType = ModelFactory.GetModelType(modelName);
        String result = String.Format("Models/{0}/{1}/{1}", (Int32)modelType, geoid);
        OSDLogger.AddStaticMessage("---------End GetRenameModelPath-----------------");
        return result;
    }

    public static GameObject CreateModel(String path, Boolean isBattle = false, Boolean checkTextureOnDisc = true, Int32 filtermode = -1)
    {
        String modelNameId = path;
        path = ModelFactory.CheckUpscale(path);
        String renameModelPath = ModelFactory.GetRenameModelPath(path);
        GameObject model;
        String externalPath = AssetManager.SearchAssetOnDisc(renameModelPath + ".fbx", true, false);
        if (!String.IsNullOrEmpty(externalPath))
        {
            // External model
            model = ModelImporter.CreateCustomModelFromFbx(externalPath);
            //model = ModelImporter.CreateCustomModelFromRawText(externalPath);
            if (model == null)
                return null;
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
                ModelFactory.SetMatFilter(renderer.material, filtermode);
        }
        else
        {
            // Model serialized in an Unity archive
            model = AssetManager.Load<GameObject>(renameModelPath, false);
            if (model == null)
                return null;
            model = UnityEngine.Object.Instantiate(model);
            if (modelNameId == "GEO_MAIN_F3_ZDN" || modelNameId == "GEO_MAIN_F4_ZDN" || modelNameId == "GEO_MAIN_F5_ZDN")
            {
                Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    String name = renderer.material.mainTexture.name;
                    Char textureId = name[name.Length - 1];
                    String geoId = ModelFactory.GetGEOID(modelNameId).ToString();
                    String textureFileName = geoId + "_" + textureId;
                    String texturePath = "Models/2/" + geoId + "/" + textureFileName;
                    Texture texture = AssetManager.Load<Texture>(texturePath, false);
                    texture.name = name;
                    renderer.material.SetTexture("_MainTex", texture);
                    ModelFactory.SetMatFilter(renderer.material, Configuration.Graphics.ElementsSmoothTexture);
                    checkTextureOnDisc = false;
                }
            }
            if (SceneDirector.IsFieldScene())
            {
                if (CustomModelField.TryGetValue(new KeyValuePair<Int32, String>(FF9StateSystem.Common.FF9.fldMapNo, modelNameId), out String[] modelSwapEntry))
                {
                    ChangeModelTexture(model, modelSwapEntry);
                    checkTextureOnDisc = false;
                }
            }
            if (checkTextureOnDisc)
            {
                String texturePath = Path.GetDirectoryName(renameModelPath) + "/%.png";
                foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
                {
                    foreach (Material mat in renderer.materials)
                    {
                        String externalTexturePath = AssetManager.SearchAssetOnDisc(texturePath.Replace("%", mat.mainTexture.name), true, false);
                        if (!String.IsNullOrEmpty(externalTexturePath))
                        {
                            Texture texture = AssetManager.LoadFromDisc<Texture2D>(externalTexturePath);
                            texture.name = mat.mainTexture.name;
                            mat.mainTexture = texture;
                        }
                    }
                }
            }
            Shader shader;
            if (modelNameId.Contains("GEO_SUB_W0"))
                shader = ShadersLoader.Find(modelNameId.Contains("GEO_SUB_W0_025") ? "WorldMap/ShadowActor" : "WorldMap/Actor");
            else
                shader = ShadersLoader.Find(isBattle ? "BattleMap_Common" : "Unlit/Transparent Cutout");

            SkinnedMeshRenderer[] skinnedRenderers = model.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer skinnedrenderer in skinnedRenderers)
            {
                skinnedrenderer.material.shader = shader;
                ModelFactory.SetMatFilter(skinnedrenderer.material, filtermode);
            }

            MeshRenderer[] meshRenderers = model.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshrenderer in meshRenderers)
            {
                foreach (Material material in meshrenderer.materials)
                {
                    String materialName = material.name.Replace("(Instance)", String.Empty);
                    if (meshrenderer.name == "Group_2")
                        material.shader = ShadersLoader.Find("BattleMap_Ground");
                    else if (materialName.Contains("a"))
                        material.shader = ShadersLoader.Find("PSX/BattleMap_Abr_1");
                    else
                        material.shader = shader;

                    ModelFactory.SetMatFilter(material, filtermode);
                }
            }
        }

        if (ModelFactory.garnetShortHairTable.Contains(modelNameId))
        {
            Boolean garnetShortHair;
            if (Configuration.Graphics.GarnetHair == 1)
                garnetShortHair = false;
            else if (Configuration.Graphics.GarnetHair == 2)
                garnetShortHair = true;
            else
                garnetShortHair = FF9StateSystem.EventState.ScenarioCounter >= 10300;

            if (garnetShortHair)
            {
                Renderer[] renderers = model.transform.GetChildByName("long_hair").GetComponentsInChildren<Renderer>();
                for (Int32 i = 0; i < renderers.Length; i++)
                    renderers[i].enabled = false;
            }
            else
            {
                Renderer[] renderers = model.transform.GetChildByName("short_hair").GetComponentsInChildren<Renderer>();
                for (Int32 i = 0; i < renderers.Length; i++)
                    renderers[i].enabled = false;
            }
        }
        AnimationFactory.AddAnimToGameObject(model, modelNameId, (isBattle && !modelNameId.Contains("_B1_")) || modelNameId.Contains("_W0_"));
        if (modelNameId.Contains("GEO_MON_"))
        {
            if (ModelFactory.upscaleTable.ContainsKey(modelNameId))
                modelNameId = ModelFactory.upscaleTable[modelNameId];
            modelNameId = modelNameId.Replace("_UP0", "_B3");
            AnimationFactory.AddAnimToGameObject(model, modelNameId, (isBattle && !modelNameId.Contains("_B1_")) || modelNameId.Contains("_W0_"));
        }
        if (isBattle)
        {
            Transform fieldSubModel = model.transform.GetChildByName("field_model");
            if (fieldSubModel != null)
            {
                Renderer[] renderers = fieldSubModel.GetComponentsInChildren<Renderer>();
                for (Int32 i = 0; i < renderers.Length; i++)
                    renderers[i].enabled = false;
            }
        }
        else
        {
            Transform battleSubModel = model.transform.GetChildByName("battle_model");
            if (battleSubModel != null)
            {
                Renderer[] renderers = battleSubModel.GetComponentsInChildren<Renderer>();
                for (Int32 i = 0; i < renderers.Length; i++)
                    renderers[i].enabled = false;
            }
        }
        if (isBattle && battlebg.BattleRoot != null)
            model.transform.parent = battlebg.BattleRoot.transform;
        return model;
    }

    public static Boolean IsUseAsEnemyCharacter(String path)
    {
        return path.Contains("GEO_MON_B3_148") || path.Contains("GEO_MON_B3_155") || path.Contains("GEO_MON_B3_168") || path.Contains("GEO_MON_B3_169") || path.Contains("GEO_MON_B3_170") || path.Contains("GEO_MON_B3_171") || path.Contains("GEO_MON_B3_172") || path.Contains("GEO_MON_B3_173") || path.Contains("GEO_MON_B3_174") || path.Contains("GEO_MON_B3_175") || path.Contains("GEO_MON_B3_182") || path.Contains("GEO_MON_B3_195");
    }

    public static GameObject CreateDefaultWeaponForCharacterWhenUseAsEnemy(String path)
    {
        RegularItem weaponId = ModelFactory.defaultWeaponTable[path];
        ItemAttack weapon = ff9item.GetItemWeapon(weaponId);
        if (weapon.ModelId == UInt16.MaxValue)
            return new GameObject(btl_eqp.DummyWeaponName);
        String geoName = FF9BattleDB.GEO.GetValue(weapon.ModelId);
        global::Debug.LogWarning("-------------------------------------------------------------------------");
        return ModelFactory.CreateModel("BattleMap/BattleModel/battle_weapon/" + geoName + "/" + geoName, true, true, Configuration.Graphics.ElementsSmoothTexture);
    }

    public static Int32 GetDefaultWeaponBoneIdForCharacterWhenUseAsEnemy(String path)
    {
        return ModelFactory.defaultWeaponBoneTable[path];
    }

    public static Boolean HaveUpScaleModel(String modelName)
    {
        return ModelFactory.upscaleTable.ContainsKey(modelName);
    }

    public static String GetUpScaleModel(String modelName)
    {
        String result;
        if (!ModelFactory.HaveUpScaleModel(modelName))
        {
            result = modelName;
        }
        else
        {
            result = ModelFactory.upscaleTable[modelName];
        }
        return result;
    }

    public static String CheckUpscale(String path)
    {
        String text = Path.GetFileNameWithoutExtension(path);
        String extension = Path.GetExtension(path);
        String text2 = String.Empty;
        if (ModelFactory.upscaleTable.ContainsKey(text))
        {
            text = ModelFactory.upscaleTable[text];
        }
        if (path.StartsWith("GEO_"))
        {
            text2 = ModelFactory.findModelFilePathFromModelCode(text);
        }
        else
        {
            text2 = Path.GetDirectoryName(Path.GetDirectoryName(path));
        }
        if (extension != String.Empty)
        {
            text = text + "." + extension;
        }
        return String.Concat(new String[]
        {
            text2,
            "/",
            text,
            "/",
            text
        });
    }

    private static String findModelFilePathFromModelCode(String modelCode)
    {
        if (modelCode == null)
            return null;
        if (modelCode.StartsWith("GEO_WEP"))
            return "BattleMap/BattleModel/battle_weapon";
        Int32 sep1 = modelCode.IndexOf('_');
        Int32 sep2 = modelCode.IndexOf('_', sep1 + 1);
        return "Models/" + modelCode.Substring(sep1 + 1, sep2 - sep1 - 1).ToLower();
    }

    public static GameObject CreateModel(String path, Vector3 position, Vector3 rotation, Boolean isBattle = false)
    {
        GameObject gameObject = ModelFactory.CreateModel(path, isBattle);
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = Quaternion.Euler(rotation);
        return gameObject;
    }

    public static void ChangeModelTexture(GameObject go, String[] newTexturePaths)
    {
        Texture[] newTextures = new Texture[newTexturePaths.Length];
        for (Int32 i = 0; i < newTexturePaths.Length; i++)
        {
            Texture2D texture = AssetManager.Load<Texture2D>(newTexturePaths[i], true);
            if (texture != null)
            {
                newTextures[i] = texture;
            }
            else
            {
                Byte[] raw = AssetManager.LoadBytes(newTexturePaths[i]);
                if (raw != null)
                    newTextures[i] = AssetManager.LoadTextureGeneric(raw);
                else
                    newTextures[i] = null;
            }
        }
        ChangeModelTextureRecursion(go.transform, "", newTextures);
    }

    private static void ChangeModelTextureRecursion(Transform transf, String hierarchyPath, Texture[] newTextures)
    {
        Material mat = transf.GetComponent<Material>();
        if (mat == null)
        {
            SkinnedMeshRenderer skin = transf.GetComponent<SkinnedMeshRenderer>();
            if (skin != null)
                mat = skin.material;
            if (mat == null)
            {
                MeshRenderer renderer = transf.GetComponent<MeshRenderer>();
                if (renderer != null)
                    mat = renderer.material;
            }
        }
        if (mat != null && mat.mainTexture != null)
        {
            // The texture name is [MODELID]_[TEXTUREINDEX] usually (always for enemy battle models?)
            // That model ID can be different from SB2_MON_PARM.Geo because of upscale tables and such
            Match formatMatch = new Regex(@"([0-9]+)_([0-9]+)").Match(mat.mainTexture.name);
            if (formatMatch.Success)
            {
                Int32 textureIndex;
                if (Int32.TryParse(formatMatch.Groups[2].Value, out textureIndex))
                    if (textureIndex < newTextures.Length && newTextures[textureIndex] != null)
                        mat.mainTexture = newTextures[textureIndex];
            }
        }
        foreach (Transform child in transf)
            ChangeModelTextureRecursion(child, hierarchyPath + transf.name + "/", newTextures);
    }

    public static String GetNameFromFF9DBALL(String modelName)
    {
        String geoName = Path.GetFileNameWithoutExtension(modelName);
        if (geoName == null)
            return null;
        switch (geoName)
        {
            case "GEO_MAIN_UP3_ZDN":
                geoName = "GEO_MAIN_F3_ZDN";
                break;
            case "GEO_MAIN_UP4_ZDN":
                geoName = "GEO_MAIN_F4_ZDN";
                break;
            case "GEO_MAIN_UP5_ZDN":
                geoName = "GEO_MAIN_F5_ZDN";
                break;
            case "GEO_MAIN_UP3_ZDN_0":
                geoName = "GEO_MAIN_F3_ZDN";
                break;
            case "GEO_MAIN_UP4_ZDN_0":
                geoName = "GEO_MAIN_F4_ZDN";
                break;
            case "GEO_MAIN_UP5_ZDN_0":
                geoName = "GEO_MAIN_F5_ZDN";
                break;
            case "GEO_MAIN_UP3_ZDN_1":
                geoName = "GEO_MAIN_F3_ZDN";
                break;
            case "GEO_MAIN_UP4_ZDN_1":
                geoName = "GEO_MAIN_F4_ZDN";
                break;
            case "GEO_MAIN_UP5_ZDN_1":
                geoName = "GEO_MAIN_F5_ZDN";
                break;
            case "GEO_MAIN_UP4_GRN":
                geoName = "GEO_MAIN_F4_GRN";
                break;
        }
        if (ModelFactory.revertUpscaleTable.TryGetValue(geoName, out String revertUpscaleName))
            return revertUpscaleName;
        return geoName;
    }

    public static Int32 GetGEOID(String modelName)
    {
        String fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modelName);
        if (fileNameWithoutExtension.Equals("GEO_MON_B3_110"))
            return 347;
        if (modelName.Equals("GEO_MON_B3_109"))
            return 5461;
        if (FF9BattleDB.GEO.TryGetKey(modelName, out Int32 id))
            return id;
        return -1;
    }

    public static ModelType GetModelType(String modelName)
    {
        Int32 sep1 = modelName.IndexOf('_');
        Int32 sep2 = modelName.IndexOf('_', sep1 + 1);
        String typeStr = modelName.Substring(sep1 + 1, sep2 - sep1 - 1).ToLower();
        if (typeStr.TryEnumParse(out ModelType type))
            return type;
        return ModelType.none;
    }

    public static GameObject CreateUIModel(PrimitiveType shape, Color color, Single size, Vector3 screenPosition, Vector3? screenPosition2 = null)
    {
        GameObject model = GameObject.CreatePrimitive(shape);
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
        {
            Texture2D colorTexture = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
            colorTexture.SetPixel(0, 0, color);
            colorTexture.Apply();
            renderer.material.mainTexture = colorTexture;
            renderer.material.shader = ShadersLoader.Find("PSX/FieldMap_Abr_0");
        }
        if (shape == PrimitiveType.Cylinder && screenPosition2.HasValue)
        {
            Vector3 endPoint = screenPosition2.Value;
            model.transform.position = (screenPosition + endPoint) / 2f;
            model.transform.localScale = new Vector3(size, (endPoint - screenPosition).magnitude / 2f, size);
            model.transform.up = endPoint - model.transform.position;
        }
        else
        {
            model.transform.position = screenPosition;
            model.transform.localScale = new Vector3(size, size, size);
        }
        return model;
    }

    /// <summary>Set filter mode of material's texture, 0=point 1=bilinear 2=trilinear, -1 = use default</summary>
    public static void SetMatFilter(Material material, Int32 filtermode, Int32 def = 1)
    {
        if (material != null)
            SetMatFilter(material.mainTexture, filtermode, def);
    }

    /// <summary>Set filter mode of texture, 0=point 1=bilinear 2=trilinear, -1 = use default</summary>
    public static void SetMatFilter(Texture texture, Int32 filtermode, Int32 def = 1)
    {
        if (texture != null)
        {
            if (filtermode == 0)
                texture.mipMapBias = -1f;

            texture.filterMode = GetFilterMode(filtermode, def);
        }
    }

    /// <summary>Returns filter mode of texture, 0=point 1=bilinear 2=trilinear, -1 = use default</summary>
    public static FilterMode GetFilterMode(Int32 filtermode, Int32 def)
    {
        filtermode = filtermode == -1 ? def : filtermode;
        filtermode = Mathf.Clamp(filtermode, 0, 2);
        return filtermode switch
        {
            0 => FilterMode.Point,
            2 => FilterMode.Trilinear,
            _ => FilterMode.Bilinear,
        };
    }

    public static Dictionary<String, String> upscaleTable = new Dictionary<String, String>
    {
        { "GEO_SUB_F0_BRN",  "GEO_SUB_UP0_BRN" },
        { "GEO_SUB_F0_CID",  "GEO_SUB_UP0_CID" },
        { "GEO_SUB_F0_CDW",  "GEO_SUB_UP0_CDW" },
        { "GEO_SUB_F0_KUW",  "GEO_SUB_UP0_KUW" },
        { "GEO_MAIN_F0_ZDD", "GEO_MAIN_UP0_ZDD" },
        { "GEO_SUB_F0_FLT",  "GEO_SUB_UP0_FLT" },
        { "GEO_SUB_F0_RBY",  "GEO_SUB_UP0_RBY" },
        { "GEO_SUB_F0_ZNR",  "GEO_SUB_UP0_ZNR" },
        { "GEO_SUB_F0_KUT",  "GEO_SUB_UP0_KUT" },
        { "GEO_NPC_F0_MOG",  "GEO_NPC_UP0_MOG" },
        { "GEO_NPC_F1_MOG",  "GEO_NPC_UP1_MOG" },
        { "GEO_NPC_F2_MOG",  "GEO_NPC_UP2_MOG" },
        { "GEO_NPC_F3_MOG",  "GEO_NPC_UP3_MOG" },
        { "GEO_NPC_F4_MOG",  "GEO_NPC_UP4_MOG" },
        { "GEO_NPC_F5_MOG",  "GEO_NPC_UP5_MOG" },
        { "GEO_NPC_F0_CHO",  "GEO_NPC_UP0_CHO" },
        { "GEO_NPC_F1_CHO",  "GEO_NPC_UP1_CHO" },
        { "GEO_NPC_F2_CHO",  "GEO_NPC_UP2_CHO" },
        { "GEO_NPC_F3_CHO",  "GEO_NPC_UP3_CHO" },
        { "GEO_NPC_F4_CHO",  "GEO_NPC_UP4_CHO" },
        { "GEO_NPC_F0_CHD",  "GEO_NPC_UP0_CHD" },
        { "GEO_MAIN_F0_ZDN", "GEO_MAIN_UP0_ZDN" },
        { "GEO_MAIN_B0_000", "GEO_MAIN_UP0_ZDN" },
        { "GEO_MAIN_B0_001", "GEO_MAIN_UP0_ZDN" },
        { "GEO_MON_B3_168",  "GEO_MON_UP0_168" },
        { "GEO_MAIN_B0_022", "GEO_MAIN_UP0_ZDNT" },
        { "GEO_MAIN_B0_023", "GEO_MAIN_UP0_ZDNT" },
        { "GEO_MAIN_B0_006", "GEO_MAIN_UP0_VIV" },
        { "GEO_MON_B3_151",  "GEO_MAIN_UP0_VIV" },
        { "GEO_MAIN_F0_VIV", "GEO_MAIN_UP1_VIV" },
        { "GEO_MON_B3_170",  "GEO_MAIN_UP1_VIV" },
        { "GEO_MAIN_B0_028", "GEO_MAIN_UP0_VIVT" },
        { "GEO_MAIN_F0_GRN", "GEO_MAIN_UP0_GRN" },
        { "GEO_MAIN_F1_GRN", "GEO_MAIN_UP0_GRN" },
        { "GEO_MAIN_B0_002", "GEO_MAIN_UP0_GRN" },
        { "GEO_MAIN_B0_003", "GEO_MAIN_UP0_GRN" },
        { "GEO_MAIN_B0_004", "GEO_MAIN_UP0_GRN" },
        { "GEO_MAIN_B0_005", "GEO_MAIN_UP0_GRN" },
        { "GEO_MON_B3_149",  "GEO_MAIN_UP0_GRN" },
        { "GEO_MON_B3_169",  "GEO_MAIN_UP0_GRN" },
        { "GEO_MAIN_B0_024", "GEO_MAIN_UP0_GRNT" },
        { "GEO_MAIN_B0_025", "GEO_MAIN_UP0_GRNT" },
        { "GEO_MAIN_B0_026", "GEO_MAIN_UP0_GRNT" },
        { "GEO_MAIN_B0_027", "GEO_MAIN_UP0_GRNT" },
        { "GEO_MAIN_F0_STN", "GEO_MAIN_UP0_STN" },
        { "GEO_MAIN_B0_007", "GEO_MAIN_UP0_STN" },
        { "GEO_MAIN_F1_STN", "GEO_MAIN_UP1_STN" },
        { "GEO_MAIN_B0_018", "GEO_MAIN_UP1_STN" },
        { "GEO_MON_B3_148",  "GEO_MAIN_UP1_STN" },
        { "GEO_MON_B3_171",  "GEO_MAIN_UP1_STN" },
        { "GEO_MAIN_B0_029", "GEO_MAIN_UP0_STNT" },
        { "GEO_MAIN_F0_FRJ", "GEO_MAIN_UP0_FRJ" },
        { "GEO_MAIN_B0_011", "GEO_MAIN_UP0_FRJ" },
        { "GEO_MON_B3_174",  "GEO_MAIN_UP0_FRJ" },
        { "GEO_MAIN_B0_033", "GEO_MAIN_UP0_FRJT" },
        { "GEO_MAIN_F0_KUI", "GEO_MAIN_UP0_KUI" },
        { "GEO_MAIN_B0_008", "GEO_MAIN_UP0_KUI" },
        { "GEO_MON_B3_172",  "GEO_MAIN_UP0_KUI" },
        { "GEO_MAIN_B0_030", "GEO_MAIN_UP0_KUIT" },
        { "GEO_MAIN_F0_SLM", "GEO_MAIN_UP0_SLM" },
        { "GEO_MAIN_B0_012", "GEO_MAIN_UP0_SLM" },
        { "GEO_MON_B3_175",  "GEO_MAIN_UP0_SLM" },
        { "GEO_MON_B3_182",  "GEO_MAIN_UP0_SLM" },
        { "GEO_MAIN_B0_034", "GEO_MAIN_UP0_SLMT" },
        { "GEO_MAIN_F0_EIK", "GEO_MAIN_UP0_EIK" },
        { "GEO_MAIN_B0_009", "GEO_MAIN_UP0_EIK" },
        { "GEO_MAIN_B0_010", "GEO_MAIN_UP0_EIK" },
        { "GEO_MON_B3_173",  "GEO_MAIN_UP0_EIK" },
        { "GEO_MAIN_B0_031", "GEO_MAIN_UP0_EIKT" },
        { "GEO_MAIN_B0_032", "GEO_MAIN_UP0_EIKT" },
        { "GEO_MAIN_F0_STD", "GEO_MAIN_UP0_STD" },
        { "GEO_MAIN_F1_ZDN", "GEO_MAIN_UP1_ZDN" },
        { "GEO_MAIN_F2_ZDN", "GEO_MAIN_UP2_ZDN" },
        { "GEO_MAIN_F3_ZDN", "GEO_MAIN_UP0_ZDN" },
        { "GEO_MAIN_F4_ZDN", "GEO_MAIN_UP0_ZDN" },
        { "GEO_MAIN_F5_ZDN", "GEO_MAIN_UP0_ZDN" },
        { "GEO_MAIN_F3_GRN", "GEO_MAIN_UP3_GRN" },
        { "GEO_MAIN_F4_GRN", "GEO_MAIN_UP3_GRN" },
        { "GEO_MAIN_F5_GRN", "GEO_MAIN_UP5_GRN" },
        { "GEO_SUB_F0_CNA",  "GEO_SUB_UP0_CNA" },
        { "GEO_SUB_F7_CNA",  "GEO_SUB_UP0_CNA" },
        { "GEO_MAIN_B0_013", "GEO_SUB_UP0_CNA" },
        { "GEO_SUB_F0_MRC",  "GEO_SUB_UP0_MRC" },
        { "GEO_SUB_F7_MRC",  "GEO_SUB_UP0_MRC" },
        { "GEO_MAIN_B0_014", "GEO_SUB_UP0_MRC" },
        { "GEO_SUB_F0_BLN",  "GEO_SUB_UP0_BLN" },
        { "GEO_SUB_F7_BLN",  "GEO_SUB_UP0_BLN" },
        { "GEO_MAIN_B0_015", "GEO_SUB_UP0_BLN" },
        { "GEO_MON_B3_195",  "GEO_SUB_UP0_BLN" },
        { "GEO_SUB_F1_BLN",  "GEO_SUB_UP1_BLN" },
        { "GEO_MAIN_B0_016", "GEO_SUB_UP1_BLN" },
        { "GEO_SUB_F0_BTX",  "GEO_SUB_UP0_BTX" },
        { "GEO_MAIN_B0_017", "GEO_SUB_UP0_BTX" },
        { "GEO_MON_B3_155",  "GEO_SUB_UP0_BTX" },
        { "GEO_SUB_F0_BAK",  "GEO_SUB_UP0_BAK" },
        { "GEO_SUB_F1_BAK",  "GEO_SUB_UP2_BAK" },
        { "GEO_MON_B3_106",  "GEO_SUB_UP2_BAK" },
        { "GEO_SUB_F2_BAK",  "GEO_SUB_UP1_BAK" },
        { "GEO_MON_B3_107",  "GEO_SUB_UP1_BAK" },
        { "GEO_MON_B3_118",  "GEO_MON_UP0_118" },
        { "GEO_MON_F0_CLB",  "GEO_MON_UP0_118" },
        { "GEO_MON_B3_132",  "GEO_MON_UP0_132" },
        { "GEO_MON_F0_EEE",  "GEO_MON_UP0_132" },
        { "GEO_MON_B3_114",  "GEO_MON_UP0_114" },
        { "GEO_MON_F0_DRA",  "GEO_MON_UP0_114" },
        { "GEO_MON_B3_110",  "GEO_MON_UP0_110" },
        { "GEO_MON_F2_EFM",  "GEO_MON_UP0_110" },
        { "GEO_MON_B3_109",  "GEO_MON_UP0_109" },
        { "GEO_MON_F0_EFM",  "GEO_MON_UP0_109" },
        { "GEO_MON_B3_117",  "GEO_MON_UP0_117" },
        { "GEO_MON_F0_TOM",  "GEO_MON_UP0_117" },
        { "GEO_MON_B3_176",  "GEO_MON_UP0_176" },
        { "GEO_MON_F0_CDR",  "GEO_MON_UP0_176" },
        { "GEO_MON_B3_131",  "GEO_MON_UP0_131" },
        { "GEO_MON_F0_DAH",  "GEO_MON_UP0_131" },
        { "GEO_MON_B3_136",  "GEO_MON_UP0_136" },
        { "GEO_MON_F0_SDR",  "GEO_MON_UP0_136" },
        { "GEO_MON_B3_124",  "GEO_MON_UP0_124" },
        { "GEO_MON_F0_ZZZ",  "GEO_MON_UP0_124" },
        { "GEO_MAIN_F7_VIV", "GEO_MAIN_UP1_VIV" },
        { "GEO_MON_B3_121",  "GEO_MON_UP3_121" },
        { "GEO_MON_F1_TOM",  "GEO_MON_UP3_121" },
        { "GEO_MON_B3_112",  "GEO_MON_UP3_112" },
        { "GEO_MON_B3_186",  "GEO_MON_UP3_186" },
        { "GEO_SUB_F0_BW1",  "GEO_MON_UP3_111" },
        { "GEO_MON_B3_111",  "GEO_MON_UP3_111" },
        { "GEO_SUB_F0_BW2",  "GEO_MON_UP3_113" },
        { "GEO_MON_B3_113",  "GEO_MON_UP3_113" },
        { "GEO_SUB_F0_BW3",  "GEO_MON_UP3_115" },
        { "GEO_MON_B3_115",  "GEO_MON_UP3_115" },
        { "GEO_SUB_F0_KJA",  "GEO_MON_UP3_125" },
        { "GEO_MON_B3_125",  "GEO_MON_UP3_125" },
        { "GEO_SUB_F3_KJA",  "GEO_SUB_UP1_KJA" },
        { "GEO_SUB_F0_KJG",  "GEO_SUB_UP0_KJAT" },
        { "GEO_MON_B3_122",  "GEO_MON_UP3_122" },
        { "GEO_SUB_F0_ZON",  "GEO_MON_UP3_119" },
        { "GEO_MON_B3_119",  "GEO_MON_UP3_119" },
        { "GEO_SUB_F1_BW3",  "GEO_MON_UP3_116" },
        { "GEO_MON_B3_116",  "GEO_MON_UP3_116" },
        { "GEO_MON_B3_185",  "GEO_MON_UP3_185" },
        { "GEO_MON_B3_144",  "GEO_MON_UP3_144" },
        { "GEO_MON_B3_181",  "GEO_MON_UP3_181" },
        { "GEO_SUB_F1_SBW",  "GEO_MON_UP3_122" },
        { "GEO_SUB_F1_ZON",  "GEO_MON_UP3_120" },
        { "GEO_MON_B3_120",  "GEO_MON_UP3_120" },
        { "GEO_MON_B3_145",  "GEO_MON_UP3_145" },
        { "GEO_MON_F1_EFM",  "GEO_MON_B3_108" },
        { "GEO_ACC_F1_LTT",  "GEO_ACC_F0_LTT" },
        { "GEO_ACC_F1_TKT",  "GEO_ACC_F0_TKT" },
        { "GEO_MON_B3_130",  "GEO_MON_UP3_130" },
        { "GEO_ACC_F1_HDB",  "GEO_ACC_F0_HDB" },
        { "GEO_MAIN_F8_GRN", "GEO_MAIN_UP8_GRN" },
        { "GEO_MON_B3_123",  "GEO_MON_UP3_123" },
        { "GEO_MON_B3_137",  "GEO_MON_UP3_137" },
        { "GEO_MON_B3_138",  "GEO_MON_UP3_138" },
        { "GEO_MON_B3_140",  "GEO_MON_UP3_140" },
        { "GEO_MON_B3_143",  "GEO_MON_UP3_143" },
        { "GEO_MON_B3_146",  "GEO_MON_UP3_146" },
        { "GEO_MON_B3_187",  "GEO_MON_UP3_187" },
        { "GEO_MON_B3_191",  "GEO_MON_UP3_191" },
        { "GEO_MON_B3_192",  "GEO_MON_UP3_192" },
        { "GEO_MON_B3_194",  "GEO_MON_UP3_194" },
        { "GEO_MON_B3_183",  "GEO_SUB_UP0_MASK_183" },
        { "GEO_MON_B3_184",  "GEO_SUB_UP0_MASK_184" },
        { "GEO_SUB_F0_GRL",  "GEO_SUB_UP0_GRL" },
        { "GEO_MON_B3_128",  "GEO_MON_UP3_128" },
        { "GEO_MON_B3_141",  "GEO_MON_UP3_141" },
        { "GEO_MON_B3_142",  "GEO_MON_UP3_142" },
        { "GEO_MON_B3_147",  "GEO_MON_UP3_147" },
        { "GEO_MON_B3_199",  "GEO_MON_UP3_199" },
        { "GEO_MON_B3_193",  "GEO_MON_UP3_193" },
        { "GEO_MON_B3_152",  "GEO_SUB_UP0_ZNR" },
        { "GEO_MON_B3_198",  "GEO_SUB_UP0_ZNR" },
        { "GEO_SUB_F0_SBW",  "GEO_SUB_UP0_SBW" }
    };

    public static Dictionary<String, String> revertUpscaleTable = new Dictionary<String, String>
    {
        { "GEO_SUB_UP0_BRN",      "GEO_SUB_F0_BRN" },
        { "GEO_SUB_UP0_CID",      "GEO_SUB_F0_CID" },
        { "GEO_SUB_UP0_CDW",      "GEO_SUB_F0_CDW" },
        { "GEO_SUB_UP0_KUW",      "GEO_SUB_F0_KUW" },
        { "GEO_MAIN_UP0_ZDD",     "GEO_MAIN_F0_ZDD" },
        { "GEO_SUB_UP0_FLT",      "GEO_SUB_F0_FLT" },
        { "GEO_SUB_UP0_RBY",      "GEO_SUB_F0_RBY" },
        { "GEO_SUB_UP0_ZNR",      "GEO_SUB_F0_ZNR" },
        { "GEO_SUB_UP0_KUT",      "GEO_SUB_F0_KUT" },
        { "GEO_NPC_UP0_MOG",      "GEO_NPC_F0_MOG" },
        { "GEO_NPC_UP1_MOG",      "GEO_NPC_F1_MOG" },
        { "GEO_NPC_UP2_MOG",      "GEO_NPC_F2_MOG" },
        { "GEO_NPC_UP3_MOG",      "GEO_NPC_F3_MOG" },
        { "GEO_NPC_UP4_MOG",      "GEO_NPC_F4_MOG" },
        { "GEO_NPC_UP5_MOG",      "GEO_NPC_F5_MOG" },
        { "GEO_NPC_UP0_CHO",      "GEO_NPC_F0_CHO" },
        { "GEO_NPC_UP1_CHO",      "GEO_NPC_F1_CHO" },
        { "GEO_NPC_UP2_CHO",      "GEO_NPC_F2_CHO" },
        { "GEO_NPC_UP3_CHO",      "GEO_NPC_F3_CHO" },
        { "GEO_NPC_UP4_CHO",      "GEO_NPC_F4_CHO" },
        { "GEO_NPC_UP0_CHD",      "GEO_NPC_F0_CHD" },
        { "GEO_MAIN_UP0_ZDN",     "GEO_MAIN_F0_ZDN" },
        { "GEO_MON_UP0_168",      "GEO_MON_B3_168" },
        { "GEO_MAIN_UP0_ZDNT",    "GEO_MAIN_B0_022" },
        { "GEO_MAIN_UP0_VIV",     "GEO_MAIN_B0_006" },
        { "GEO_MAIN_UP1_VIV",     "GEO_MAIN_F0_VIV" },
        { "GEO_MAIN_UP0_VIVT",    "GEO_MAIN_B0_028" },
        { "GEO_MAIN_UP0_GRN",     "GEO_MAIN_F0_GRN" },
        { "GEO_MAIN_UP0_GRNT",    "GEO_MAIN_B0_024" },
        { "GEO_MAIN_UP0_STN",     "GEO_MAIN_F0_STN" },
        { "GEO_MAIN_UP1_STN",     "GEO_MAIN_F1_STN" },
        { "GEO_MAIN_UP0_STNT",    "GEO_MAIN_B0_029" },
        { "GEO_MAIN_UP0_FRJ",     "GEO_MAIN_F0_FRJ" },
        { "GEO_MAIN_UP0_FRJT",    "GEO_MAIN_B0_033" },
        { "GEO_MAIN_UP0_KUI",     "GEO_MAIN_F0_KUI" },
        { "GEO_MAIN_UP0_KUIT",    "GEO_MAIN_B0_030" },
        { "GEO_MAIN_UP0_SLM",     "GEO_MAIN_F0_SLM" },
        { "GEO_MAIN_UP0_SLMT",    "GEO_MAIN_B0_034" },
        { "GEO_MAIN_UP0_EIK",     "GEO_MAIN_F0_EIK" },
        { "GEO_MAIN_UP0_EIKT",    "GEO_MAIN_B0_031" },
        { "GEO_MAIN_UP0_STD",     "GEO_MAIN_F0_STD" },
        { "GEO_MAIN_UP1_ZDN",     "GEO_MAIN_F1_ZDN" },
        { "GEO_MAIN_UP2_ZDN",     "GEO_MAIN_F2_ZDN" },
        { "GEO_MAIN_UP3_GRN",     "GEO_MAIN_F3_GRN" },
        { "GEO_MAIN_UP5_GRN",     "GEO_MAIN_F5_GRN" },
        { "GEO_SUB_UP0_CNA",      "GEO_SUB_F0_CNA" },
        { "GEO_SUB_UP0_MRC",      "GEO_SUB_F0_MRC" },
        { "GEO_SUB_UP0_BLN",      "GEO_SUB_F0_BLN" },
        { "GEO_SUB_UP1_BLN",      "GEO_SUB_F1_BLN" },
        { "GEO_SUB_UP0_BTX",      "GEO_SUB_F0_BTX" },
        { "GEO_SUB_UP0_BAK",      "GEO_SUB_F0_BAK" },
        { "GEO_SUB_UP2_BAK",      "GEO_SUB_F1_BAK" },
        { "GEO_SUB_UP1_BAK",      "GEO_SUB_F2_BAK" },
        { "GEO_MON_UP0_118",      "GEO_MON_B3_118" },
        { "GEO_MON_UP0_132",      "GEO_MON_B3_132" },
        { "GEO_MON_UP0_114",      "GEO_MON_B3_114" },
        { "GEO_MON_UP0_110",      "GEO_MON_F2_EFM" },
        { "GEO_MON_UP0_109",      "GEO_MON_F0_EFM" },
        { "GEO_MON_UP0_117",      "GEO_MON_B3_117" },
        { "GEO_MON_UP0_176",      "GEO_MON_B3_176" },
        { "GEO_MON_UP0_131",      "GEO_MON_B3_131" },
        { "GEO_MON_UP0_136",      "GEO_MON_B3_136" },
        { "GEO_MON_UP0_124",      "GEO_MON_B3_124" },
        { "GEO_MAIN_UP7_VIV",     "GEO_MAIN_F7_VIV" },
        { "GEO_MON_UP3_121",      "GEO_MON_B3_121" },
        { "GEO_MON_UP3_112",      "GEO_MON_B3_112" },
        { "GEO_MON_UP3_186",      "GEO_MON_B3_186" },
        { "GEO_MON_UP3_111",      "GEO_MON_B3_111" },
        { "GEO_MON_UP3_113",      "GEO_MON_B3_113" },
        { "GEO_MON_UP3_115",      "GEO_MON_B3_115" },
        { "GEO_MON_UP3_125",      "GEO_MON_B3_125" },
        { "GEO_SUB_UP1_KJA",      "GEO_SUB_F3_KJA" },
        { "GEO_SUB_UP0_KJAT",     "GEO_SUB_F0_KJG" },
        { "GEO_MON_UP3_122",      "GEO_MON_B3_122" },
        { "GEO_MON_UP3_119",      "GEO_MON_B3_119" },
        { "GEO_MON_UP3_116",      "GEO_MON_B3_116" },
        { "GEO_MON_UP3_185",      "GEO_MON_B3_185" },
        { "GEO_MON_UP3_144",      "GEO_MON_B3_144" },
        { "GEO_MON_UP3_181",      "GEO_MON_B3_181" },
        { "GEO_MON_UP3_120",      "GEO_MON_B3_120" },
        { "GEO_MON_UP3_145",      "GEO_MON_B3_145" },
        { "GEO_MON_B3_108",       "GEO_MON_F1_EFM" },
        { "GEO_ACC_F0_LTT",       "GEO_ACC_F1_LTT" },
        { "GEO_ACC_F0_TKT",       "GEO_ACC_F1_TKT" },
        { "GEO_MON_UP3_130",      "GEO_MON_B3_130" },
        { "GEO_ACC_F0_HDB",       "GEO_ACC_F1_HDB" },
        { "GEO_MON_UP3_123",      "GEO_MON_B3_123" },
        { "GEO_MON_UP3_137",      "GEO_MON_B3_137" },
        { "GEO_MON_UP3_138",      "GEO_MON_B3_138" },
        { "GEO_MON_UP3_140",      "GEO_MON_B3_140" },
        { "GEO_MON_UP3_143",      "GEO_MON_B3_143" },
        { "GEO_MON_UP3_146",      "GEO_MON_B3_146" },
        { "GEO_MON_UP3_187",      "GEO_MON_B3_187" },
        { "GEO_MON_UP3_191",      "GEO_MON_B3_191" },
        { "GEO_MON_UP3_192",      "GEO_MON_B3_192" },
        { "GEO_MON_UP3_194",      "GEO_MON_B3_194" },
        { "GEO_SUB_UP0_MASK_183", "GEO_MON_B3_183" },
        { "GEO_SUB_UP0_MASK_184", "GEO_MON_B3_184" },
        { "GEO_SUB_UP0_GRL",      "GEO_SUB_F0_GRL" },
        { "GEO_MON_UP3_128",      "GEO_MON_B3_128" },
        { "GEO_MON_UP3_141",      "GEO_MON_B3_141" },
        { "GEO_MON_UP3_142",      "GEO_MON_B3_142" },
        { "GEO_MON_UP3_147",      "GEO_MON_B3_147" },
        { "GEO_MON_UP3_199",      "GEO_MON_B3_199" },
        { "GEO_MON_UP3_193",      "GEO_MON_B3_193" },
        { "GEO_SUB_UP0_SBW",      "GEO_SUB_F0_SBW" },
        { "GEO_MAIN_UP8_GRN",     "GEO_MAIN_F8_GRN" }
    };

    private static Dictionary<String, RegularItem> defaultWeaponTable = new Dictionary<String, RegularItem>
    {
        { "GEO_MON_B3_168", RegularItem.Dagger },
        { "GEO_MON_B3_170", RegularItem.MageStaff },
        { "GEO_MON_B3_169", RegularItem.Rod },
        { "GEO_MON_B3_148", RegularItem.Broadsword },
        { "GEO_MON_B3_171", RegularItem.Broadsword },
        { "GEO_MON_B3_174", RegularItem.Javelin },
        { "GEO_MON_B3_172", RegularItem.Fork },
        { "GEO_MON_B3_173", RegularItem.GolemFlute },
        { "GEO_MON_B3_175", RegularItem.CatClaws },
        { "GEO_MON_B3_182", RegularItem.CatClaws },
        { "GEO_MON_B3_195", RegularItem.Broadsword },
        { "GEO_MON_B3_155", RegularItem.SaveTheQueen }
    };

    private static Dictionary<String, Int32> defaultWeaponBoneTable = new Dictionary<String, Int32>
    {
        { "GEO_MON_B3_168", 19 },
        { "GEO_MON_B3_170", 17 },
        { "GEO_MON_B3_169", 15 },
        { "GEO_MON_B3_148", 16 },
        { "GEO_MON_B3_171", 16 },
        { "GEO_MON_B3_174", 6 },
        { "GEO_MON_B3_172", 14 },
        { "GEO_MON_B3_173", 15 },
        { "GEO_MON_B3_175", 16 },
        { "GEO_MON_B3_182", 16 },
        { "GEO_MON_B3_195", 14 },
        { "GEO_MON_B3_155", 16 }
    };

    public static HashSet<String> garnetShortHairTable = new HashSet<String>
    {
        "GEO_MAIN_F0_GRN",
        "GEO_MAIN_F1_GRN",
        "GEO_MAIN_B0_002",
        "GEO_MAIN_B0_003",
        "GEO_MAIN_B0_004",
        "GEO_MAIN_B0_005",
        "GEO_MON_B3_149",
        "GEO_MON_B3_169",
        "GEO_MAIN_B0_024",
        "GEO_MAIN_B0_025",
        "GEO_MAIN_B0_026",
        "GEO_MAIN_B0_027"
    };

    public static Dictionary<KeyValuePair<Int32, String>, String[]> CustomModelField = new Dictionary<KeyValuePair<Int32, String>, String[]>();
}
