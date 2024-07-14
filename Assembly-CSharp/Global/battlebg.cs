using Memoria.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class battlebg
{
    public static void nf_InitBattleBG(BBGINFO bbginfoPtr, GEOTEXHEADER tab)
    {
        battlebg.btlModel = FF9StateSystem.Battle.FF9Battle.map.btlBGPtr;
        battlebg.nf_BbgInfoPtr = bbginfoPtr;
        battlebg.nf_BbgNumber = battlebg.nf_BbgInfoPtr.bbgnumber;
        battlebg.nf_SkyFixPositionFlag = 0;
        if (battlebg.nf_BbgInfoPtr.fog != 0)
            battlebg.nf_SkyFixPositionFlag++;
        battlebg.nf_BbgSkyRotation = battlebg.nf_BbgInfoPtr.skyrotation;
        battlebg.nf_BbgTexAnm = battlebg.nf_BbgInfoPtr.texanim;
        battlebg.nf_BbgTabAddress = tab;
        battlebg.nf_BbgUVChangeCount = battlebg.nf_BbgInfoPtr.uvcount;
        battlebg.nf_SetBbgDispAttribute(battlebg.BBG_DISP_ATTRIBUTE_ALL);
        battlebg.nf_BbgSkyAngle_Y = 0;
        battlebg.SetDefaultShader(battlebg.btlModel);
        battlebg.objAnimModel = FF9StateSystem.Battle.FF9Battle.map.btlBGObjAnim;
        battlebg.objAnimModel = new GameObject[battlebg.nf_BbgInfoPtr.objanim];
        for (Int32 i = 0; i < battlebg.nf_BbgInfoPtr.objanim; i++)
        {
            String objName = $"BBG_B{battlebg.nf_BbgNumber:D3}_OBJ{i + 1}";
            battlebg.objAnimModel[i] = ModelFactory.CreateModel($"BattleMap/BattleModel/battleMap_all/{objName}/{objName}", false);
            battlebg.SetDefaultShader(battlebg.objAnimModel[i]);
            if (battlebg.nf_BbgNumber == 171 && i == 1) // Crystal World, Crystal
                battlebg.SetMaterailShader(battlebg.objAnimModel[i], "PSX/BattleMap_Cystal");
        }
        FF9StateSystem.Battle.FF9Battle.map.btlBGObjAnim = battlebg.objAnimModel;
        battlebg.nf_BbgTabAddress.InitBBGTextureAnim(battlebg.btlModel, battlebg.objAnimModel);
        if (battlebg.nf_BbgTexAnm != 0)
            for (Int32 i = 0; i < battlebg.nf_BbgTexAnm; i++)
                battlebg.geoBGTexAnimPlay(battlebg.nf_BbgTabAddress, i);
    }

    public static void nf_InitBattleSky(Texture2D cloudTex, string battleSceneName)
    {
        battlebg.SetSkyparams(battlebg.btlModel, "PSX/BattleMap_Sky", cloudTex, battleSceneName);
    }

    public static void SetDefaultShader(GameObject go)
    {
        foreach (Transform transform in go.transform)
        {
            if (battlebg.getBbgAttr(transform.name) == battlebg.BBG_ATTR_PLUS)
            {
                battlebg.SetMaterailShader(transform.gameObject, "PSX/BattleMap_Plus");
            }
            else if (battlebg.getBbgAttr(transform.name) == battlebg.BBG_ATTR_GROUND)
            {
                battlebg.SetMaterailShader(transform.gameObject, "PSX/BattleMap_Ground");
            }
            else if (battlebg.getBbgAttr(transform.name) == battlebg.BBG_ATTR_MINUS)
            {
                battlebg.SetMaterailShader(transform.gameObject, "PSX/BattleMap_Minus");
            }
            else if (battlebg.getBbgAttr(transform.name) == battlebg.BBG_ATTR_SKY)
            {
                battlebg.SetMaterailShader(transform.gameObject, "PSX/BattleMap_Sky");
                if (!battlebg.bbg_KeepSkyScaleList.Contains(battlebg.nf_BbgNumber))
                    transform.localScale = new Vector3(1.01f, 1.01f, 1.01f);
            }
        }
    }

    public static void SetMaterailShader(GameObject go, String shaderName)
    {
        MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
        for (Int32 i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;
            for (Int32 j = 0; j < materials.Length; j++)
            {
                Material material = materials[j];
                String text = material.name.Replace("(Instance)", String.Empty);
                if (battlebg.nf_BbgNumber == 171 && j == 0 && shaderName.Contains("Minus")) // Crystal World, Crystal
                {
                    material.shader = ShadersLoader.Find("PSX/BattleMap_Moon");
                }
                else if ((battlebg.nf_BbgNumber == 92 && j == 3 && shaderName.Contains("Plus"))  // Desert Palace, Dock
                      || (battlebg.nf_BbgNumber == 52 && j == 6 && shaderName.Contains("Plus"))) // Cleyra's Trunk, Inside, Sandfall
                {
                    material.shader = ShadersLoader.Find("PSX/BattleMap_Plus_Abr_1_Off");
                }
                else if (text.Contains("a"))
                {
                    material.shader = ShadersLoader.Find(shaderName + "_Abr_1");
                }
                else if (text.Contains("s"))
                {
                    material.shader = ShadersLoader.Find(shaderName + "_Abr_0");
                    material.SetColor("_Color", new Color32(Byte.MaxValue, Byte.MaxValue, Byte.MaxValue, 110));
                }
                else
                {
                    material.shader = ShadersLoader.Find(shaderName);
                    if (shaderName.CompareTo("PSX/BattleMap_Ground") == 0)
                        material.SetInt("_ZWrite", 0); // DEBUG: Can't make the default value in "_ZWrite ("ZWrite", Int) = 0" works correctly for some reason
                }
            }
        }
    }

    public static void SetSkyparams(GameObject go, String shaderName, Texture2D cloudTex, string battleSceneName)
    {
        var bgColor = new Color32(battlebg.nf_GetBbgInfoPtr().chr_r, battlebg.nf_GetBbgInfoPtr().chr_g,
            battlebg.nf_GetBbgInfoPtr().chr_b, 255);
        var preset = -1;
        var isCloudBg = BgWithSky.TryGetValue(battleSceneName, out preset);
       foreach (Transform transform in go.transform)
        {
            if (battlebg.getBbgAttr(transform.name) == battlebg.BBG_ATTR_SKY)
            {
                MeshRenderer[] renderers = transform.GetComponentsInChildren<MeshRenderer>();
                for (Int32 i = 0; i < renderers.Length; i++)
                {
                    Material[] materials = renderers[i].materials;
                    for (Int32 j = 0; j < materials.Length; j++)
                    {
                        Material material = materials[j];
                        material.SetTexture("_CloudTexture", cloudTex);
                        if (battleSceneName.Contains("WM"))
                        {
                            material.SetFloat("_Opacity1", Random.Range(0.413f, 0.9f));
                            material.SetFloat("_Opacity2", Random.Range(0.16f, 0.32f));
                            material.SetFloat("_Opacity3", Random.Range(0.2f, 1.0f));
                            material.SetFloat("_Opacity4", 0.095f);
                            material.SetColor("_ColorTint", Color.white);
                            material.SetColor("_ColorAmbientTint", bgColor+ new Color(0.15f,0.15f,0.3f));
                            material.SetFloat("_HeightOffset", 0.02f);
                            material.SetFloat("_Altitude", Random.Range(2000, 2500));
                            material.SetFloat("_Scroll", 0.001f);
                            material.SetFloat("_Opacity", 0.5f);
                            material.SetFloat("_Exposure", 2.7f);
                            material.SetFloat("_NoCloud", -1.0f);
                            material.SetFloat("_Angle", Random.Range(0,360));
                        }
                        else if (isCloudBg && preset == 1)
                        {
                            material.SetFloat("_Opacity1", 1.0f);
                            material.SetFloat("_Opacity2", 0.161f);
                            material.SetFloat("_Opacity3", 1.0f);
                            material.SetFloat("_Opacity4", 0.095f);
                            material.SetColor("_ColorTint", Color.white);
                            material.SetColor("_ColorAmbientTint", bgColor+ new Color(0.15f,0.15f,0.3f));
                            material.SetFloat("_HeightOffset", 0.0f);
                            material.SetFloat("_Altitude", 2000.0f);
                            material.SetFloat("_Scroll", 0.001f);
                            material.SetFloat("_Opacity", 0.5f);
                            material.SetFloat("_Exposure",2.7f);
                            material.SetFloat("_NoCloud", -1.0f);
                            material.SetFloat("_Angle", Random.Range(0,360));
                        }
                        else if (preset == 2)
                        {
                            material.SetFloat("_Opacity1", 0.9f);
                            material.SetFloat("_Opacity2", 0.161f);
                            material.SetFloat("_Opacity3", 1.0f);
                            material.SetFloat("_Opacity4", 0.095f);
                            material.SetColor("_ColorTint", Color.white);
                            material.SetColor("_ColorAmbientTint", bgColor + new Color(0.15f,0.15f,0.3f));
                            material.SetFloat("_HeightOffset", 0.1f);
                            material.SetFloat("_Altitude", 2000.0f);
                            material.SetFloat("_Scroll", 0.001f);
                            material.SetFloat("_Opacity", 0.5f);
                            material.SetFloat("_Exposure",2.7f);
                            material.SetFloat("_NoCloud", 1.0f);
                            material.SetFloat("_Angle", Random.Range(0,360));
                        }
                        else
                        {
                            material.SetFloat("_Opacity1", 0f);
                            material.SetFloat("_Opacity2", 0f);
                            material.SetFloat("_Opacity3", 0f);
                            material.SetFloat("_Opacity4", 0f);
                            material.SetColor("_ColorTint", Color.black);
                            material.SetColor("_ColorAmbientTint", Color.black);
                            material.SetFloat("_Exposure", 1.0f);
                            material.SetFloat("_NoCloud", 1.0f);
                        }
                    }
                }
            }
        }
    }
    
    public static List<Material> GetShaders(Int32 bbgAttr)
    {
        List<Material> list = new List<Material>();
        foreach (Transform transform in battlebg.btlModel.transform)
        {
            if (battlebg.getBbgAttr(transform.name) == bbgAttr)
            {
                MeshRenderer[] renderers = transform.gameObject.GetComponentsInChildren<MeshRenderer>();
                for (Int32 i = 0; i < renderers.Length; i++)
                {
                    Material[] materials = renderers[i].materials;
                    for (Int32 j = 0; j < materials.Length; j++)
                        list.Add(materials[j]);
                }
            }
        }
        return list;
    }

    public static void nf_BattleBG()
    {
        battlebg.nf_BbgTick++;
        if (battlebg.nf_BbgTexAnm != 0)
            battlebg.geoBGTexAnimService(battlebg.nf_BbgTabAddress);
        foreach (Transform transform in battlebg.btlModel.transform)
        {
            if (battlebg.getBbgAttr(transform.name) == 8 && battlebg.nf_BbgSkyRotation != 0)
            {
                battlebg.nf_BbgSkyAngle_Y += battlebg.nf_BbgSkyRotation;
                Vector3 eulerAngles = transform.localRotation.eulerAngles;
                eulerAngles.y = battlebg.nf_BbgSkyAngle_Y / 8f / 4096f * 360f;
                transform.localRotation = Quaternion.Euler(eulerAngles);
            }
            battlebg.setBGColor(transform.gameObject);
        }
        Int32 fullTime = (Int32)Time.realtimeSinceStartup;
        for (Int32 i = 0; i < battlebg.nf_BbgInfoPtr.objanim; i++)
        {
            battlebg.getBbgObjAnimation(battlebg.nf_BbgNumber, i, battlebg.nf_BbgTick, fullTime, out Vector3 bbgPos, out Quaternion bbgRot);
            battlebg.objAnimModel[i].transform.localPosition = bbgPos;
            battlebg.objAnimModel[i].transform.localRotation = bbgRot;
        }
    }

    public static void getBbgObjAnimation(Int32 bbgId, Int32 objIndex, Int32 tick, Int32 fullTime, out Vector3 pos, out Quaternion rot)
    {
        Boolean invertRot = false;
        Vector3 angles = default;
        pos = default;
        switch (bbgId)
        {
            case 7:
                if (objIndex == 0)
                {
                    if ((tick + 31 & 63) == 0)
                        battlebg.nf_b007a = UnityEngine.Random.Range(0, 512);
                    angles.y = battlebg.nf_b007a + UnityEngine.Random.Range(0, 64);
                }
                else
                {
                    if ((tick & 63) == 0)
                        battlebg.nf_b007b = UnityEngine.Random.Range(0, 1024);
                    angles.y = battlebg.nf_b007b + UnityEngine.Random.Range(0, 128);
                }
                break;
            case 68:
                angles.y += 3f;
                pos.x = 0f;
                pos.y = -10f;
                pos.z = 0f;
                break;
            case 110:
                angles.z = (Int32)(Mathf.Sin(((fullTime * 12) & 4095) / 4096f * 360f) * 4096f) / 64;
                angles.y = 512f;
                pos.x = 1500f;
                pos.y = -7000f;
                pos.z = 3750f;
                break;
            case 112:
                switch (objIndex)
                {
                    case 0:
                    {
                        angles.z = 4095 - ((fullTime * 5) & 4095);
                        angles.y = 4095 - ((fullTime * 3) & 4095);
                        pos.x = -2100f;
                        pos.y = -250f + (Int32)(Mathf.Sin((((fullTime + 8) * 22) & 4095) / 4096f * 360f) * 4096f) / 45;
                        pos.z = -850f;
                        break;
                    }
                    case 1:
                    {
                        angles.y = 0f;
                        angles.z = 0f;
                        pos.x = 1725f;
                        pos.y = -1500f + (Int32)(Mathf.Sin(((fullTime * 20) & 4095) / 4096f * 360f) * 4096f) / 64;
                        pos.z = -75f;
                        break;
                    }
                    case 2:
                    {
                        angles.z = (fullTime * 4) & 4095;
                        angles.y = (fullTime * 3) & 4095;
                        pos.x = 1750f;
                        pos.y = -775f + (Int32)(Mathf.Sin((((fullTime + 16) * 21) & 4095) / 4096f * 360f) * 4096f) / 50;
                        pos.z = 1025f;
                        break;
                    }
                }
                break;
            case 168:
                angles.x = 0f;
                angles.z = 0f;
                angles.y = (objIndex == 0 ? fullTime / 16 : fullTime / 8) & 4095;
                break;
            case 171:
                fullTime = tick * 2;
                if (objIndex == 0)
                {
                    angles.z = (fullTime * 12) & 4095;
                    pos.x = 0f;
                    pos.y = -2375f;
                    pos.z = 3750f;
                }
                else
                {
                    angles.z = 3584f;
                    angles.y = (fullTime * 22) & 4095;
                    pos.x = 0f;
                    pos.y = -2250f;
                    pos.z = 7625f;
                    invertRot = true;
                }
                break;
            case 111:
            case 169:
            case 170:
            default:
                angles.z = (Int32)(Mathf.Sin(((fullTime * 26) & 4095) / 4096f * 360f) * 4096f) / 5;
                pos.x = 1065f;
                pos.y = -1345f;
                pos.z = 3749f;
                break;
        }
        pos.y *= -1f;
        angles *= 360f / 4096f;
        rot = invertRot ? Quaternion.Inverse(Quaternion.Euler(angles)) : Quaternion.Euler(angles);
    }

    public static Int32 getBbgAttr(String name)
    {
        switch (name)
        {
            case "Group_0":
                return 0;
            case "Group_2":
                return 2;
            case "Group_4":
                return 4;
            case "Group_8":
                return 8;
        }
        return 0;
    }

    public static BBGINFO nf_GetBbgInfoPtr()
    {
        return FF9StateSystem.Battle.FF9Battle.map.btlBGInfoPtr;
    }

    public static void nf_SetBbgDispAttribute(Int32 attribute)
    {
        battlebg.nf_BbgDispAttribute = attribute;
    }

    public static void geoBGTexAnimPlay(GEOTEXHEADER tab, Int32 anum)
    {
        if (tab.geotex[anum].numframes == 0)
        {
            tab.materials[anum].mainTexture.wrapMode = TextureWrapMode.Repeat;
            if (battlebg.nf_BbgNumber == 57) // Cleyra, Observation post
                tab.bbgExtraAimMaterials[anum].mainTexture.wrapMode = TextureWrapMode.Repeat;
        }
        tab.geotex[anum].flags |= 1;
        tab.geotex[anum].frame = 0;
        tab.geotex[anum].lastframe = 4096;
    }

    public static void geoBGTexAnimService(GEOTEXHEADER texheaderptr)
    {
        UInt16 count = texheaderptr.count;
        GEOTEXANIMHEADER[] geotex = texheaderptr.geotex;
        for (Int32 i = 0; i < count; i++)
        {
            GEOTEXANIMHEADER geotexanimheader = geotex[i];
            if ((geotexanimheader.flags & 1) != 0)
            {
                // UV animation
                if (geotexanimheader.numframes != 0)
                {
                    Int32 frameLong = geotexanimheader.frame;
                    Int16 lastframe = geotexanimheader.lastframe;
                    Int16 frameShort = (Int16)(frameLong >> 12);
                    if (frameShort >= 0)
                    {
                        if (geotexanimheader.numframes <= 0)
                            continue;
                        if (frameShort != lastframe)
                        {
                            //for (Int32 j = 0; j < geotexanimheader.count; j++)
                            {
                                Single dx = (geotexanimheader.coords[frameShort].x - geotexanimheader.target.x) / texheaderptr.materials[i].mainTexture.width;
                                Single dy = (geotexanimheader.coords[frameShort].y - geotexanimheader.target.y) / texheaderptr.materials[i].mainTexture.height;
                                texheaderptr.materials[i].SetTextureOffset("_MainTex", new Vector2(dx, -dy));
                            }
                            geotexanimheader.lastframe = frameShort;
                        }
                        frameLong += geotexanimheader.rate;
                    }
                    else
                    {
                        frameLong += 4096;
                    }
                    if (frameLong >> 12 < geotexanimheader.numframes)
                    {
                        geotexanimheader.frame = frameLong;
                    }
                    else if (geotexanimheader.randrange > 0)
                    {
                        geotexanimheader.frame = -(Int32)((UInt64)UnityEngine.Random.Range(geotexanimheader.randmin, geotexanimheader.randrange + 1) << 12);
                    }
                    else if ((geotexanimheader.flags & 2) != 0)
                    {
                        geotexanimheader.flags &= unchecked((Byte)~3);
                    }
                    else
                    {
                        geotexanimheader.frame = 0;
                    }
                }
                else if ((geotexanimheader.flags & 4) != 0)
                {
                    // Vertical texture scroll
                    Int32 frameLong = geotexanimheader.frame;
                    Int16 frameShort = (Int16)(frameLong >> 12);
                    Single dy = frameShort / 256f;
                    if (battlebg.nf_BbgNumber == 69 && i == 3) // Fossil Roo, Road accross water
                        dy *= -1f;
                    //for (Int32 j = 0; j < geotexanimheader.count; j++)
                    texheaderptr.materials[i].SetTextureOffset("_MainTex", new Vector2(0f, -dy));
                    if (battlebg.nf_BbgNumber == 57) // Cleyra, Observation post
                        texheaderptr.bbgExtraAimMaterials[i].SetTextureOffset("_MainTex", new Vector2(0f, -dy));
                    else if (battlebg.nf_BbgNumber == 71) // Fossil Roo, Underground lake
                        texheaderptr.bbgExtraAimMaterials[i].SetTextureOffset("_MainTex", new Vector2(0f, -dy));
                    geotexanimheader.frame += geotexanimheader.rate;
                }
            }
        }
    }

    public static Int32 nf_GetBbgIntensity()
    {
        return battlebg.nf_BbgBrite;
    }

    public static void nf_SetBbgIntensity(Byte fade)
    {
        battlebg.nf_BbgBrite = fade;
    }

    private static void setBGColor(GameObject go)
    {
        Single intensity = battlebg.nf_BbgBrite;
        foreach (MeshRenderer renderer in go.GetComponentsInChildren<MeshRenderer>())
        {
            if (intensity == 0f)
            {
                renderer.enabled = false;
            }
            else
            {
                renderer.enabled = true;
                foreach (Material mat in renderer.materials)
                    mat.SetFloat("_Intensity", intensity);
            }
        }
    }

    #region Debugging API
    // The commented code is an alternative to the "relevant part" cut using triangle sets instead of rects
    // It tends to cut the relevant part too sharply
    // TODO: maybe allow to use custom textures that are cropped to the relevant part like with SPS textures
    public static List<Texture2D> GetAllTextures_RelevantPartOnly(String bbgName, Color32 clear)
    {
        GameObject go = ModelFactory.CreateModel($"BattleMap/BattleModel/battleMap_all/{bbgName}/{bbgName}", Vector3.zero, Vector3.zero, true);
        Dictionary<String, Texture2D> textureDict = new Dictionary<String, Texture2D>();
        Dictionary<String, Rect> areaDict = new Dictionary<String, Rect>();
        //Dictionary<String, List<Vector2[]>> areaDict = new Dictionary<String, List<Vector2[]>>();
        foreach (Renderer rend in go.GetComponentsInChildren<Renderer>())
            GetRelevantUVPartFromRenderer(rend, textureDict, areaDict);
        GEOTEXHEADER geotexheader = new GEOTEXHEADER();
        geotexheader.ReadBGTextureAnim(bbgName);
        if (geotexheader.geotex != null)
        {
            GameObject[] extraObj = null;
            if (geotexheader.bbgnumber == 7)
            {
                BBGINFO bbginfo = new BBGINFO();
                bbginfo.ReadBattleInfo(bbgName);
                extraObj = new GameObject[bbginfo.objanim];
                for (Int32 i = 0; i < bbginfo.objanim; i++)
                {
                    String objName = $"BBG_B{geotexheader.bbgnumber:D3}_OBJ{i + 1}";
                    extraObj[i] = ModelFactory.CreateModel($"BattleMap/BattleModel/battleMap_all/{objName}/{objName}", false);
                }
            }
            geotexheader.InitBBGTextureAnim(go, extraObj);
            for (Int32 i = 0; i < geotexheader.count; i++)
            {
                GEOTEXANIMHEADER anim = geotexheader.geotex[i];
                Texture2D texture = textureDict[geotexheader.materials[i].mainTexture.name];
                //List<Vector2[]> area = areaDict[texture.name];
                Rect area = areaDict[texture.name];
                Int32 w = texture.width;
                Int32 h = texture.height;
                if (anim.numframes != 0)
                {
                    //List<Vector2[]> newtriangles = new List<Vector2[]>();
                    List<Rect> newareas = new List<Rect>();
                    for (Int32 f = 0; f < anim.numframes; f++)
                    {
                        Single dx = (anim.coords[f].x - anim.target.x) / w;
                        Single dy = (anim.coords[f].y - anim.target.y) / h;
                        Vector2 dv = new Vector2(dx, -dy);
                        Rect newarea = area;
                        newarea.position += dv;
                        newareas.Add(newarea);
                        //foreach (Vector2[] tri in area)
                        //{
                        //    Vector2[] newtri = [tri[0] + dv, tri[1] + dv, tri[2] + dv];
                        //    newtriangles.Add(newtri);
                        //}
                    }
                    foreach (Rect newarea in newareas)
                    {
                        area.min = Vector2.Min(area.min, newarea.min);
                        area.max = Vector2.Max(area.max, newarea.max);
                    }
                    areaDict[texture.name] = area;
                    //area.AddRange(newtriangles);
                }
                else if ((anim.flags & 4) != 0)
                {
                    area.yMin = 0f;
                    area.yMax = 1f;
                    areaDict[texture.name] = area;
                    //if (area.Count > 0)
                    //{
                    //    Single minx = Math.Min(Math.Min(area[0][0].x, area[0][1].x), area[0][2].x);
                    //    Single maxx = Math.Max(Math.Max(area[0][0].x, area[0][1].x), area[0][2].x);
                    //    foreach (Vector2[] tri in area)
                    //    {
                    //        foreach (Vector2 v in tri)
                    //        {
                    //            minx = Math.Min(minx, v.x);
                    //            maxx = Math.Max(maxx, v.x);
                    //        }
                    //    }
                    //    area.Clear();
                    //    area.Add([new Vector2(minx, 0f), new Vector2(maxx, 0f), new Vector2(minx, 1f)]);
                    //    area.Add([new Vector2(maxx, 0f), new Vector2(minx, 1f), new Vector2(maxx, 1f)]);
                    //}
                    if (geotexheader.bbgnumber == 57 || geotexheader.bbgnumber == 71)
                    {
                        texture = textureDict[geotexheader.bbgExtraAimMaterials[i].mainTexture.name];
                        area = areaDict[texture.name];
                        area.yMin = 0f;
                        area.yMax = 1f;
                        areaDict[texture.name] = area;
                        //if (area.Count > 0)
                        //{
                        //    Single min = Math.Min(Math.Min(area[0][0].x, area[0][1].x), area[0][2].x);
                        //    Single max = Math.Max(Math.Max(area[0][0].x, area[0][1].x), area[0][2].x);
                        //    foreach (Vector2[] tri in area)
                        //    {
                        //        foreach (Vector2 v in tri)
                        //        {
                        //            min = Math.Min(min, v.x);
                        //            max = Math.Max(max, v.x);
                        //        }
                        //    }
                        //    area.Clear();
                        //    area.Add([new Vector2(min, 0f), new Vector2(max, 0f), new Vector2(min, 1f)]);
                        //    area.Add([new Vector2(max, 0f), new Vector2(min, 1f), new Vector2(max, 1f)]);
                        //}
                    }
                }
            }
        }
        foreach (var kvp in textureDict)
        {
            Texture2D texture = kvp.Value;
            Rect area = areaDict[kvp.Key];
            //List<Vector2[]> area = areaDict[kvp.Key];
            Color32[] colors = texture.GetPixels32();
            Int32 w = texture.width;
            Int32 h = texture.height;
            Single eps = 0.5f / w;
            Single rounding = 4f;
            Int32 roundedx = (Int32)Math.Floor(w * area.xMin / rounding);
            Int32 roundedy = (Int32)Math.Floor(h * area.yMin / rounding);
            Int32 roundedw = (Int32)Math.Ceiling(w * (area.xMax - area.xMin) / rounding);
            Int32 roundedh = (Int32)Math.Ceiling(h * (area.yMax - area.yMin) / rounding);
            area = new Rect((roundedx * rounding - 0.5f) / w, (roundedy * rounding - 0.5f) / h, (roundedw * rounding) / w, (roundedh * rounding) / h);
            for (Int32 x = 0; x < w; x++)
            {
                for (Int32 y = 0; y < h; y++)
                {
                    Vector2 v = new Vector2((Single)x / w, (Single)y / h);
                    //if (!area.Any(r => TriangleContains(r, v, eps)))
                    if (!area.Contains(v))
                        colors[x + w * y] = clear;
                }
            }
            texture.SetPixels32(colors);
            texture.Apply();
        }
        UnityEngine.Object.Destroy(go);
        return textureDict.Values.ToList();
    }

    private static void GetRelevantUVPartFromRenderer(Renderer renderer, Dictionary<String, Texture2D> textureDict, Dictionary<String, Rect> areaDict)
    {
        List<String> texts = new List<String>();
        foreach (Material mat in renderer.materials)
        {
            if (!textureDict.TryGetValue(mat.mainTexture.name, out Texture2D texture))
            {
                texture = Memoria.Assets.TextureHelper.CopyAsReadable(mat.mainTexture);
                texture.name = mat.mainTexture.name;
                textureDict.Add(texture.name, texture);
                areaDict.Add(texture.name, new Rect(100f, 100f, -200f, -200f));
                //areaDict.Add(texture.name, new List<Vector2[]>());
            }
            texts.Add(texture.name);
        }
        MeshFilter filter = renderer.GetComponent<MeshFilter>();
        SkinnedMeshRenderer skin = renderer.GetComponent<SkinnedMeshRenderer>();
        if (filter)
        {
            for (Int32 meshi = 0; meshi < texts.Count; meshi++)
            {
                Int32[] submeshtri = filter.sharedMesh.GetTriangles(meshi);
                for (Int32 tri = 0; tri < submeshtri.Length; tri += 3)
                {
                    Vector2 v1 = filter.sharedMesh.uv[submeshtri[tri]];
                    Vector2 v2 = filter.sharedMesh.uv[submeshtri[tri + 1]];
                    Vector2 v3 = filter.sharedMesh.uv[submeshtri[tri + 2]];
                    Rect prevRect = areaDict[texts[meshi]];
                    prevRect.min = Vector2.Min(prevRect.min, Vector2.Min(Vector2.Min(v1, v2), v3));
                    prevRect.max = Vector2.Max(prevRect.max, Vector2.Max(Vector2.Max(v1, v2), v3));
                    areaDict[texts[meshi]] = prevRect;
                    //areaDict[texts[meshi]].Add([v1, v2, v3]);
                }
            }
        }
        if (skin)
        {
            for (Int32 meshi = 0; meshi < texts.Count; meshi++)
            {
                Int32[] submeshtri = filter.sharedMesh.GetTriangles(meshi);
                for (Int32 tri = 0; tri < submeshtri.Length; tri += 3)
                {
                    Vector2 v1 = skin.sharedMesh.uv[submeshtri[tri]];
                    Vector2 v2 = skin.sharedMesh.uv[submeshtri[tri + 1]];
                    Vector2 v3 = skin.sharedMesh.uv[submeshtri[tri + 2]];
                    Rect prevRect = areaDict[texts[meshi]];
                    prevRect.min = Vector2.Min(prevRect.min, Vector2.Min(Vector2.Min(v1, v2), v3));
                    prevRect.max = Vector2.Max(prevRect.max, Vector2.Max(Vector2.Max(v1, v2), v3));
                    areaDict[texts[meshi]] = prevRect;
                    //areaDict[texts[meshi]].Add([v1, v2, v3]);
                }
            }
        }
    }

    private static Single LineDist(Vector2 l1, Vector2 l2, Vector2 p)
    {
        Vector2 pl = p - l1;
        Vector2 ll = l2 - l1;
        return (pl.x * ll.y - pl.y * ll.x) / ll.magnitude;
    }

    private static Boolean TriangleContains(Vector2[] tri, Vector2 p, Single eps)
    {
        if (tri[0] == tri[1] || tri[0] == tri[2] || tri[1] == tri[2])
            return false;
        Single d1 = LineDist(tri[0], tri[1], p);
        Single d2 = LineDist(tri[1], tri[2], p);
        Single d3 = LineDist(tri[2], tri[0], p);
        Int32 overallSign = Math.Sign(LineDist(tri[0], tri[1], tri[2]));
        if (overallSign >= 0)
            return d1 > -eps && d2 > -eps && d3 > -eps;
        return d1 < eps && d2 < eps && d3 < eps;
    }
    #endregion

    public const Int32 BBG_DISP_ATTRIBUTE_PLUS = 1;
    public const Int32 BBG_DISP_ATTRIBUTE_GROUND = 2;
    public const Int32 BBG_DISP_ATTRIBUTE_MINUS = 4;
    public const Int32 BBG_DISP_ATTRIBUTE_SKY = 8;
    public const Int32 BBG_DISP_ATTRIBUTE_ALL = 15;

    public const Byte BBG_ATTR_PLUS = 0;
    public const Byte BBG_ATTR_GROUND = 2;
    public const Byte BBG_ATTR_MINUS = 4;
    public const Byte BBG_ATTR_SKY = 8;

    public static Int32 nf_BbgNumber;

    public static BBGINFO nf_BbgInfoPtr;
    public static GEOTEXHEADER nf_BbgTabAddress;

    public static Int32 nf_BbgTexAnm;
    public static Int32 nf_BbgUVscroll;
    public static Int32 nf_BbgSkyRotation;
    public static Int32 nf_BbgSkyAngle_Y;
    public static Int32 nf_BbgUVChangeCount;
    public static Int32 nf_SkyFixPositionFlag;
    public static Int32 nf_BbgDispAttribute;
    public static Int32 nf_BbgRainFlag;

    public static Int32 nf_b007a;
    public static Int32 nf_b007b;
    public static Int32 nf_BbgTick;

    private static GameObject btlModel;
    private static GameObject[] objAnimModel;

    private static Byte nf_BbgBrite = 128;

    private static readonly HashSet<Int32> bbg_KeepSkyScaleList =
    [
        1,   // Prima Vista, Cargo room
        2,   // Prima Vista, Storage room
        12,  // Prima Vista, Cargo room in ruins
        109, // Memoria, Entrance & Recollection
        119, // Memoria, Birth
        68,  // Fossil Roo, Booby-Trapped road
        69,  // Fossil Roo, Road accross water
        60,  // Gargan Roo, Hall
        53,  // Cleyra's Trunk, Inside, Trunk forest
        49,  // Cleyra's Trunk, Inside, Sand-full room
        90,  // Mount Gulug, Extraction circle
        20,  // Ice Cavern, Road to waterfall
        77,  // Iifa Tree, Inside, Stone elevator
        125, // World Map, Mist Continent, Mountain + Mist
        141, // World Map, Lost Continent, Snow
        95   // Desert Palace, Library
    ];
    
    public static readonly Dictionary<string, int> BgWithSky = new Dictionary<string, int>
    {
        {"BSC_BU_R013", 1},
        {"BSC_BU_R010", 1},
        {"BSC_BU_R011", 1},
        {"BSC_BU_E015", 1},
        {"BSC_BU_R008", 1},
        {"BSC_BU_R005", 1},
        {"BSC_BU_R006", 1},
        {"BSC_GT_R014", 1},
        {"BSC_UV_R000", 1},
        {"BSC_IF_R003", 1},
        {"BSC_IF_R008", 1},
        {"BSC_GT_R003", 1},
        {"BSC_BU_R017", 1},
        {"BSC_BU_R015", 1},
        {"BSC_BU_R016", 1},
        {"BSC_BU_E072", 1},
        
        //boss battle
        {"BSC_AP_E012", 1},
        {"BSC_CA_E013", 2},
        {"BSC_SG_E016", 1},
        {"BSC_CY_E018", 1},
        {"BSC_CY_E022", 1},
        {"BSC_CM_E051", 1},
        {"BSC_CM_R007", 1},
        {"BSC_CM_R001", 1},
        {"BSC_CM_R004", 1},
        {"BSC_MS_E075", 1},
    };
}
