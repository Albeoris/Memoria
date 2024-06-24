using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class GEOTEXHEADER
{
    public void ReadTextureAnim(String path)
    {
        Byte[] binAsset = AssetManager.LoadBytes(path, true);
        if (binAsset == null)
        {
            global::Debug.LogWarning("Cannot find GeoTexAnim for : " + path);
            return;
        }
        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(binAsset)))
        {
            this.count = binaryReader.ReadUInt16();
            this.pad = binaryReader.ReadUInt16();
            this.geotex = new GEOTEXANIMHEADER[this.count];
            for (Int32 i = 0; i < this.count; i++)
            {
                this.geotex[i] = new GEOTEXANIMHEADER();
                this.geotex[i].ReadData(binaryReader);
            }
        }
    }

    public void ReadBattleTextureAnim(BTL_DATA btl, String path)
    {
        this.ReadTextureAnim(path);
        if (this.geotex == null)
            return;
        String geoName = FF9BattleDB.GEO.GetValue(btl.dms_geo_id);
        if (btl.originalGo != null)
            this.materials = btl.originalGo.GetComponentsInChildren<SkinnedMeshRenderer>().Select(smr => smr.material).ToArray();
        else
            this.materials = btl.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>().Select(smr => smr.material).ToArray();
        GeoTexAnim.SetTexAnimIndexs(geoName, out this._mainTextureIndexs, out this._subTextureIndexs);
        this.InitMultiTexAnim(geoName);
    }

    public void ReadTranceTextureAnim(BTL_DATA btl, String geoName)
    {
        this.ReadTextureAnim($"Models/GeoTexAnim/{geoName}.tab");
        if (this.geotex == null)
            return;
        btl.tranceGo.SetActive(true);
        this.materials = btl.tranceGo.GetComponentsInChildren<SkinnedMeshRenderer>().Select(smr => smr.material).ToArray();
        btl.tranceGo.SetActive(false);
        GeoTexAnim.SetTexAnimIndexs(geoName, out this._mainTextureIndexs, out this._subTextureIndexs);
        this.InitMultiTexAnim(geoName);
    }

    public void InitMultiTexAnim(String geoName)
    {
        for (Int32 i = 0; i < this.count; i++)
        {
            MapTextureToTexAnimIndex(this._mainTextureIndexs[i], this.materials[this._mainTextureIndexs[i]].mainTexture);
            MapTextureToTexAnimIndex(this._subTextureIndexs[i], this.materials[this._subTextureIndexs[i]].mainTexture);
            MapRenderTexToTexAnimIndex(this._mainTextureIndexs[i], this.materials[this._mainTextureIndexs[i]], this.TextureMapping[this._mainTextureIndexs[i]]);
        }
        for (Int32 i = 0; i < this.count; i++)
        {
            Single scalex = this.TextureMapping[this._mainTextureIndexs[i]].width / 128f;
            Single scaley = this.TextureMapping[this._mainTextureIndexs[i]].height / 128f;
            Single subw = this.TextureMapping[this._subTextureIndexs[i]].width;
            Single subh = this.TextureMapping[this._subTextureIndexs[i]].height;
            Single subscalex = subw / 128f;
            Single subscaley = subh / 128f;
            Rect rect = this.geotex[i].targetuv;
            rect.x *= scalex;
            rect.y *= scaley;
            rect.width *= scalex;
            rect.height *= scaley;
            this.geotex[i].targetuv = rect;
            for (Int32 j = 0; j < this.geotex[i].numframes; j++)
            {
                rect = this.geotex[i].rectuvs[j];
                rect.x = (rect.x * subscalex + 0.5f) / subw;
                rect.y = (subh - (rect.y + rect.height) * subscaley + 0.5f) / subh;
                rect.width = (rect.width * subscalex - 1f) / subw;
                rect.height = (rect.height * subscaley - 1f) / subh;
                this.geotex[i].rectuvs[j] = rect;
            }
        }
    }

    private void MapTextureToTexAnimIndex(Int32 texAnimIndex, Texture texture)
    {
        if (!this.TextureMapping.ContainsKey(texAnimIndex))
            this.TextureMapping.Add(texAnimIndex, texture);
    }

    private void MapRenderTexToTexAnimIndex(Int32 texAnimIndex, Material mat, Texture mainTexture)
    {
        Single textureWidth = mainTexture.width;
        Single textureHeight = mainTexture.height;
        if (!this.RenderTexMapping.ContainsKey(texAnimIndex))
        {
            RenderTexture renderTexture = new RenderTexture((Int32)textureWidth, (Int32)textureHeight, 24);
            renderTexture.filterMode = FilterMode.Bilinear;
            renderTexture.wrapMode = TextureWrapMode.Repeat;
            renderTexture.name = mat.name + "_RT";
            mat.mainTexture = renderTexture;
            this.RenderTexMapping.Add(texAnimIndex, renderTexture);
            RenderTexture active = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.PushMatrix();
            GL.Clear(true, true, Color.clear);
            GL.LoadPixelMatrix(0f, textureWidth, textureHeight, 0f);
            Graphics.DrawTexture(new Rect(0f, 0f, textureWidth, textureHeight), mainTexture);
            GL.PopMatrix();
            RenderTexture.active = active;
        }
    }

    public void MultiTexAnimService(Int32 i, Int16 framenum)
    {
        GEOTEXANIMHEADER animHeader = this.geotex[i];
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = this.RenderTexMapping[this._mainTextureIndexs[i]];
        GL.PushMatrix();
        GL.LoadPixelMatrix(0f, RenderTexture.active.width, RenderTexture.active.height, 0f);
        Graphics.DrawTexture(new Rect(0f, 0f, RenderTexture.active.width, RenderTexture.active.height), this.TextureMapping[this._mainTextureIndexs[i]], GEOTEXANIMHEADER.texAnimMat);
        Graphics.DrawTexture(animHeader.targetuv, this.TextureMapping[this._subTextureIndexs[i]], animHeader.rectuvs[framenum], 0, 0, 0, 0);
        GL.PopMatrix();
        RenderTexture.active = active;
    }

    public void ReadBGTextureAnim(String battleModelPath)
    {
        this.bbgnumber = Int32.Parse(battleModelPath.Replace("BBG_B", String.Empty));
        this.ReadTextureAnim($"BattleMap/BattleTexAnim/{battleModelPath.Replace("BBG", "TAM")}.tab");
    }

    public void CheckRenderTextures(MonoBehaviour framework)
    {
        for (Int32 i = 0; i < this.count; i++)
        {
            RenderTexture renderTexture = this.RenderTexMapping[this._mainTextureIndexs[i]];
            if (!renderTexture.IsCreated())
            {
                if (this.geotex != null)
                    framework.StartCoroutine(this.WaitForRecreateRenderTexture(i));
            }
        }
    }

    private void SetDefaultTextures(Int32 i)
    {
        this.materials[this._mainTextureIndexs[i]].mainTexture = this.TextureMapping[this._mainTextureIndexs[i]];
        this.materials[this._subTextureIndexs[i]].mainTexture = this.TextureMapping[this._subTextureIndexs[i]];
    }

    private void SetBackRenderTexture(Int32 i)
    {
        this.materials[this._mainTextureIndexs[i]].mainTexture = this.RenderTexMapping[this._mainTextureIndexs[i]];
    }

    public void RecreateMultiTexAnim(Int32 i)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = this.RenderTexMapping[this._mainTextureIndexs[i]];
        GL.PushMatrix();
        GL.Clear(true, true, Color.clear);
        GL.LoadPixelMatrix(0f, RenderTexture.active.width, RenderTexture.active.height, 0f);
        Graphics.DrawTexture(new Rect(0f, 0f, RenderTexture.active.width, RenderTexture.active.height), this.TextureMapping[this._mainTextureIndexs[i]]);
        GL.PopMatrix();
        RenderTexture.active = active;
    }

    private IEnumerator WaitForRecreateRenderTexture(Int32 i)
    {
        this.SetDefaultTextures(i);
        yield return new WaitForEndOfFrame();
        this.SetBackRenderTexture(i);
        this.RecreateMultiTexAnim(i);
        yield break;
    }

    public void InitBBGTextureAnim()
    {
        if (this.geotex == null)
            return;
        BGObjIndex bgobjIndex = GEOTEXHEADER.bgObjMappings[this.bbgnumber];
        MeshRenderer[] bbgmrs = FF9StateSystem.Battle.FF9Battle.map.btlBGPtr.GetComponentsInChildren<MeshRenderer>();
        this.materials = new Material[this.count];
        this.bbgExtraAimMaterials = new Material[this.count];
        for (Int32 i = 0; i < this.count; i++)
        {
            if (this.bbgnumber == 7 && i != 0) // Memoria, Nova Dragon battle
            {
                if (i == 1)
                    this.materials[i] = FF9StateSystem.Battle.FF9Battle.map.btlBGObjAnim[0].GetComponentsInChildren<MeshRenderer>()[bgobjIndex.groupIndex[i]].materials[bgobjIndex.materialIndex[i]];
                else
                    this.materials[i] = FF9StateSystem.Battle.FF9Battle.map.btlBGObjAnim[1].GetComponentsInChildren<MeshRenderer>()[bgobjIndex.groupIndex[i]].materials[bgobjIndex.materialIndex[i]];
            }
            else
            {
                this.materials[i] = bbgmrs[bgobjIndex.groupIndex[i]].materials[bgobjIndex.materialIndex[i]];
                if (this.bbgnumber == 57) // Cleyra, Observation post
                    this.bbgExtraAimMaterials[i] = bbgmrs[2].materials[0];
                else if (this.bbgnumber == 71) // Fossil Roo, Underground lake
                    this.bbgExtraAimMaterials[i] = bbgmrs[0].materials[11];
            }
        }
    }

    public UInt16 count;
    public UInt16 pad;

    public GEOTEXANIMHEADER[] geotex;
    public Material[] materials;

    public Material[] bbgExtraAimMaterials;

    public Int32[] _mainTextureIndexs;
    public Int32[] _subTextureIndexs;

    public Dictionary<Int32, Texture> TextureMapping = new Dictionary<Int32, Texture>();
    public Dictionary<Int32, RenderTexture> RenderTexMapping = new Dictionary<Int32, RenderTexture>();

    public Int32 bbgnumber;

    public static Dictionary<Int32, BGObjIndex> bgObjMappings = new Dictionary<Int32, BGObjIndex>()
    {
        { 4,   new BGObjIndex([0], [0]) }, // Prima Vista, Stage (Night)
        { 7,   new BGObjIndex([1, 0, 0], [1, 0, 0]) }, // Memoria, Nova Dragon battle
        { 9,   new BGObjIndex([0], [10]) }, // Alexandria Castle, Extraction altar
        { 29,  new BGObjIndex([0], [0]) }, // Gizamaluke's Grotto, Hall
        { 32,  new BGObjIndex([2], [3]) }, // Oeilvert, Grand Hall
        { 42,  new BGObjIndex([1], [3]) }, // Oeilvert, Ark battle
        { 51,  new BGObjIndex([0], [10]) }, // Cleyra's Trunk, Inside, Platform with less sand
        { 52,  new BGObjIndex([0], [6]) }, // Cleyra's Trunk, Inside, Sandfall
        { 56,  new BGObjIndex([1], [0]) }, // Cleyra, Antlion battle
        { 57,  new BGObjIndex([0], [2]) }, // Cleyra, Observation post
        { 66,  new BGObjIndex([0], [1]) }, // Alexandria Castle, Queen's Chamber
        { 67,  new BGObjIndex([0], [10]) }, // Fossil Roo, Upper level
        { 69,  new BGObjIndex([2, 3, 0, 0], [5, 0, 5, 4]) }, // Fossil Roo, Road accross water
        { 71,  new BGObjIndex([0, 0, 2], [10, 9, 4]) }, // Fossil Roo, Underground lake
        { 82,  new BGObjIndex([2], [0]) }, // Iifa Tree, Inside, Leaf elevator
        { 92,  new BGObjIndex([2], [3]) }, // Desert Palace, Dock
        { 108, new BGObjIndex([0], [11]) }, // Ipsen's Castle, Mirrors' room
        { 118, new BGObjIndex([0], [4]) }, // Memoria, Portal
        { 128, new BGObjIndex([0, 0], [2, 3]) }, // World Map, Mist Continent, Beach + Mist
        { 137, new BGObjIndex([0, 0], [2, 3]) }, // World Map, Mist Continent, Beach
        { 143, new BGObjIndex([0], [0]) }, // World Map, Lost Continent, Beach
        { 147, new BGObjIndex([0], [0]) }, // World Map, Outer Continent, Beach
        { 155, new BGObjIndex([0], [0]) }, // World Map, Outer Continent, Beach + Mist
        { 164, new BGObjIndex([0], [0]) }, // World Map, Lost Continent, Beach + Mist
        { 172, new BGObjIndex([0], [5]) }, // Crystal World, Necron battle
    };
}
