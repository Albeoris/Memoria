using Assets.Scripts.Common;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GeoTexAnim : HonoBehavior
{
    public override void HonoAwake()
    {
        this._renderTexuresCreated = false;
        this._isLoaded = false;
        this._cFrameCounter = 0;
    }

    public override void HonoOnDestroy()
    {
        base.HonoOnDestroy();
        if (this.TextureAnim != null)
        {
            foreach (RenderTexture rt in this.TextureAnim.RenderTexMapping.Values)
                rt.Release();
            this.TextureAnim = null;
        }
    }

    public void Load(String modelName, Int32[] mainTextureIndex, Int32[] subTextureIndex)
    {
        this.TextureAnim = new GEOTEXHEADER();
        this.TextureAnim.ReadTextureAnim(modelName + ".tab");
        if (this.TextureAnim.geotex == null)
            return;
        this.TextureAnim._mainTextureIndexs = mainTextureIndex;
        this.TextureAnim._subTextureIndexs = subTextureIndex;
        SkinnedMeshRenderer[] skmrs = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (modelName.Contains("GEO_SUB_F0_KUW"))
            this.TextureAnim.materials = skmrs[0].materials;
        else
            this.TextureAnim.materials = skmrs.Select(smr => smr.material).ToArray();
        this.TextureAnim.InitMultiTexAnim(modelName + ".tab");
        this._isLoaded = true;
    }

    public Int32 geoTexAnimGetCount()
    {
        return this.TextureAnim?.count ?? 0;
    }

    public void geoTexAnimPlay(Int32 anum)
    {
        if (!this._isLoaded)
            return;
        this.TextureAnim.geotex[anum].flags |= 1;
        this.TextureAnim.geotex[anum].frame = 0;
        this.TextureAnim.geotex[anum].lastframe = 4096;
        if (anum == 0)
            this.TextureAnim.geotex[2].flags &= unchecked((Byte)~3);
    }

    public static void geoTexAnimPlay(GEOTEXHEADER tab, Int32 anum)
    {
        if (tab == null || tab.geotex == null)
            return;
        tab.geotex[anum].flags |= 1;
        tab.geotex[anum].frame = 0;
        tab.geotex[anum].lastframe = 4096;
    }

    public void geoTexAnimPlayOnce(Int32 anum)
    {
        if (!this._isLoaded)
            return;
        this.TextureAnim.geotex[anum].flags |= 3;
        this.TextureAnim.geotex[anum].frame = 0;
        this.TextureAnim.geotex[anum].lastframe = 4096;
    }

    public static void geoTexAnimPlayOnce(GEOTEXHEADER tab, Int32 anum)
    {
        if (tab == null || tab.geotex == null)
            return;
        tab.geotex[anum].flags |= 3;
        tab.geotex[anum].frame = 0;
        tab.geotex[anum].lastframe = 4096;
    }

    public void geoTexAnimStop(Int32 anum)
    {
        if (!this._isLoaded)
            return;
        this.TextureAnim.geotex[anum].flags &= unchecked((Byte)~3);
    }

    public static void geoTexAnimStop(GEOTEXHEADER tab, Int32 anum)
    {
        if (tab == null || tab.geotex == null)
            return;
        tab.geotex[anum].flags &= unchecked((Byte)~3);
    }

    public static void geoTexAnimFreezeState(BTL_DATA btl)
    {
        if (btl.backupGeotex != null || btl.texanimptr == null || btl.texanimptr.geotex == null)
            return;
        Int32 animCount = btl.texanimptr.geotex.Length;
        btl.backupGeotex = new Byte[animCount];
        for (Int32 i = 0; i < animCount; i++)
        {
            btl.backupGeotex[i] = btl.texanimptr.geotex[i].flags;
            GeoTexAnim.geoTexAnimStop(btl.texanimptr, i);
        }
        if (btl.bi.player != 0 && btl.tranceTexanimptr != null && btl.tranceTexanimptr.geotex != null)
        {
            animCount = btl.tranceTexanimptr.geotex.Length;
            btl.backupGeoTrancetex = new Byte[animCount];
            for (Int32 i = 0; i < animCount; i++)
            {
                btl.backupGeoTrancetex[i] = btl.tranceTexanimptr.geotex[i].flags;
                GeoTexAnim.geoTexAnimStop(btl.tranceTexanimptr, i);
            }
        }
    }

    public static void geoTexAnimReturnState(BTL_DATA btl)
    {
        if (btl.texanimptr == null || btl.texanimptr.geotex == null)
            return;
        Int32 animCount = btl.texanimptr.geotex.Length;
        if (btl.backupGeotex != null)
            for (Int32 i = 0; i < animCount; i++)
                btl.texanimptr.geotex[i].flags = btl.backupGeotex[i];
        if (btl.bi.player != 0 && btl.tranceTexanimptr != null && btl.tranceTexanimptr.geotex != null)
        {
            animCount = btl.tranceTexanimptr.geotex.Length;
            if (btl.backupGeoTrancetex != null)
                for (Int32 i = 0; i < animCount; i++)
                    btl.tranceTexanimptr.geotex[i].flags = btl.backupGeoTrancetex[i];
        }
        btl.backupGeotex = null;
        btl.backupGeoTrancetex = null;
    }

    public Boolean ff9fieldCharIsTexAnimActive()
    {
        if (!this._isLoaded)
            return false;
        for (Int32 i = this.geoTexAnimGetCount() - 1; i >= 0; i--)
            if ((this.TextureAnim.geotex[i].flags & 1) != 0)
                return true;
        return false;
    }

    private void Update()
    {
        if (!this._isLoaded || this._renderTexuresCreated)
            return;
        this.TextureAnim.CheckRenderTextures(this);
        this._renderTexuresCreated = true;
    }

    public override void HonoUpdate()
    {
        if (!this._isLoaded)
            return;
        this._cFrameCounter++;
        if (this._cFrameCounter >= (SceneDirector.IsBattleScene() ? 2 : 1))
        {
            this._cFrameCounter = 0;
            geoTexAnimService(this.TextureAnim);
        }
    }

    private static Int32 geoTexAnimRandom(Int32 randmin, Int32 randrange)
    {
        return UnityEngine.Random.Range(randmin, randrange + 1);
    }

    public static void geoTexAnimService(GEOTEXHEADER texHeader)
    {
        if (texHeader == null)
            return;
        for (Int32 i = 0; i < texHeader.count; i++)
        {
            GEOTEXANIMHEADER geotexanimheader = texHeader.geotex[i];
            if ((geotexanimheader.flags & 1) != 0)
            {
                if (geotexanimheader.numframes != 0)
                {
                    Int32 frameLong = geotexanimheader.frame;
                    Int16 lastframe = geotexanimheader.lastframe;
                    Int16 frameShort = (Int16)(frameLong >> 12);
                    if (frameShort >= 0)
                    {
                        if (frameShort != lastframe)
                        {
                            //for (Int32 j = 0; j < geotexanimheader.count; j++)
                            texHeader.MultiTexAnimService(i, frameShort);
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
                        geotexanimheader.frame = -(geoTexAnimRandom(geotexanimheader.randmin, geotexanimheader.randrange) << 12);
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
                    global::Debug.Log("SCROLL!!!");
                }
            }
        }
    }

    public static void addTexAnim(GameObject go, String geoName)
    {
        GeoTexAnim geoTexAnim = go.AddComponent<GeoTexAnim>();
        Boolean garnetShortHair = FF9StateSystem.EventState.ScenarioCounter >= 10300;
        if (geoName.Equals("GEO_MAIN_F0_GRN") && garnetShortHair)
            geoName = "GEO_MAIN_F1_GRN";
        GeoTexAnim.SetTexAnimIndexs(geoName, out Int32[] mainTextureIndex, out Int32[] subTextureIndex);
        geoTexAnim.Load("Models/GeoTexAnim/" + geoName, mainTextureIndex, subTextureIndex);
    }

    public static void SetTexAnimIndexs(String geoName, out Int32[] mainTextureIndexs, out Int32[] subTextureIndexs)
    {
        String csvAsset = AssetManager.LoadString($"EmbeddedAsset/GeoTexAnimIndex/{geoName}.csv", true);
        if (csvAsset == null)
        {
            mainTextureIndexs = null;
            subTextureIndexs = null;
            return;
        }
        String[] entries = csvAsset.Split('\n');
        Int32 cnt = entries.Length;
        mainTextureIndexs = new Int32[cnt - 1];
        subTextureIndexs = new Int32[cnt - 1];
        for (Int32 i = 1; i < cnt; i++)
        {
            String[] entry = entries[i].Split(',');
            mainTextureIndexs[i - 1] = Int32.Parse(entry[0]);
            subTextureIndexs[i - 1] = Int32.Parse(entry[1]);
        }
    }

    public GEOTEXHEADER TextureAnim;

    private Boolean _renderTexuresCreated;
    private Boolean _isLoaded;
    private Int32 _cFrameCounter;
}
