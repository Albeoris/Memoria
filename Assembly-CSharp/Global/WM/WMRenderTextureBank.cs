using Memoria;
using System;
using UnityEngine;

public class WMRenderTextureBank : Singleton<WMRenderTextureBank>
{
    protected override void Awake()
    {
        base.Awake();
        this.Initialize();
    }

    private void Initialize()
    {
        if (this.initialized)
            return;
        WMBlock.LoadMaterialsFromDisc();
        this.VolcanoCrater1 = AssetManager.Load<RenderTexture>("WorldMap/RenderTextures/VolcanoCrater1", false);
        if (!WMBlock.MaterialDatabase.TryGetValue("VolcanoCrater1", out this.VolcanoCrater1Material))
            this.VolcanoCrater1Material = AssetManager.Load<Material>("WorldMap/Materials/0_4_0_128_mat", false);
        this.VolcanoCrater1Texture = (Texture2D)this.VolcanoCrater1Material.mainTexture;
        this.VolcanoLava1 = AssetManager.Load<RenderTexture>("WorldMap/RenderTextures/VolcanoLava1", false);
        if (!WMBlock.MaterialDatabase.TryGetValue("VolcanoLava1", out this.VolcanoLava1Material))
            this.VolcanoLava1Material = AssetManager.Load<Material>("WorldMap/Materials/0_4_0_192_mat", false);
        this.VolcanoLava1Texture = (Texture2D)this.VolcanoLava1Material.mainTexture;
        this.VolcanoCrater2 = AssetManager.Load<RenderTexture>("WorldMap/RenderTextures/VolcanoCrater2", false);
        if (!WMBlock.MaterialDatabase.TryGetValue("VolcanoCrater2", out this.VolcanoCrater2Material))
            this.VolcanoCrater2Material = AssetManager.Load<Material>("WorldMap/Materials/1_4_0_128_mat", false);
        this.VolcanoCrater2Texture = (Texture2D)this.VolcanoCrater2Material.mainTexture;
        this.VolcanoLava2 = AssetManager.Load<RenderTexture>("WorldMap/RenderTextures/VolcanoLava2", false);
        if (!WMBlock.MaterialDatabase.TryGetValue("VolcanoLava2", out this.VolcanoLava2Material))
            this.VolcanoLava2Material = AssetManager.Load<Material>("WorldMap/Materials/1_4_0_192_mat", false);
        this.VolcanoLava2Texture = (Texture2D)this.VolcanoLava2Material.mainTexture;
        this.Beach1Material = AssetManager.Load<Material>("WorldMap/Materials/Beach1", false);
        for (Int32 i = 0; i < 4; i++)
            this.Beach1Textures[i] = AssetManager.Load<Texture2D>("WorldMap/Textures/11_0_128_" + i, false);
        this.Beach2Material = AssetManager.Load<Material>("WorldMap/Materials/Beach2", false);
        for (Int32 i = 0; i < 4; i++)
            this.Beach2Textures[i] = AssetManager.Load<Texture2D>("WorldMap/Textures/11_128_128_" + i, false);
        this.RiverMaterial = AssetManager.Load<Material>("WorldMap/Materials/River", false);
        for (Int32 i = 0; i < 6; i++)
            this.RiverTextures[i] = AssetManager.Load<Texture2D>("WorldMap/Textures/11_128_0_" + i, false);
        this.RiverJointMaterial = AssetManager.Load<Material>("WorldMap/Materials/RiverJoint", false);
        for (Int32 i = 0; i < 6; i++)
            this.RiverJointTextures[i] = AssetManager.Load<Texture2D>("WorldMap/Textures/10_0_128_" + i, false);
        this.Sea_10_64_0Material = AssetManager.Load<Material>("WorldMap/Materials/Sea1", false);
        for (Int32 i = 0; i < 6; i++)
            this.Sea_10_64_0Textures[i] = AssetManager.Load<Texture2D>("WorldMap/Textures/10_64_0_" + i, false);
        this.Sea_10_128_0Material = AssetManager.Load<Material>("WorldMap/Materials/Sea2", false);
        for (Int32 i = 0; i < 6; i++)
            this.Sea_10_128_0Textures[i] = AssetManager.Load<Texture2D>("WorldMap/Textures/10_128_0_" + i, false);
        this.Sea_10_128_64Material = AssetManager.Load<Material>("WorldMap/Materials/Sea3", false);
        for (Int32 i = 0; i < 6; i++)
            this.Sea_10_128_64Textures[i] = AssetManager.Load<Texture2D>("WorldMap/Textures/10_128_64_" + i, false);
        this.Sea_10_128_128Material = AssetManager.Load<Material>("WorldMap/Materials/Sea4", false);
        for (Int32 i = 0; i < 6; i++)
            this.Sea_10_128_128Textures[i] = AssetManager.Load<Texture2D>("WorldMap/Textures/10_128_128_" + i, false);
        this.Sea_11_64_0Material = AssetManager.Load<Material>("WorldMap/Materials/Sea5", false);
        for (Int32 i = 0; i < 6; i++)
            this.Sea_11_64_0Textures[i] = AssetManager.Load<Texture2D>("WorldMap/Textures/11_64_0_" + i, false);
        this.Sea_11_192_64Material = AssetManager.Load<Material>("WorldMap/Materials/Sea6", false);
        for (Int32 i = 0; i < 4; i++)
            this.Sea_11_192_64Textures[i] = AssetManager.Load<Texture2D>("WorldMap/Textures/11_192_64_" + i, false);
        if (!WMBlock.MaterialDatabase.TryGetValue("Falls", out this.Falls))
            this.Falls = AssetManager.Load<Material>("WorldMap/Materials/Falls", false);
        if (!WMBlock.MaterialDatabase.TryGetValue("Stream", out this.Stream))
            this.Stream = AssetManager.Load<Material>("WorldMap/Materials/Stream", false);
        this.initialized = true;
    }

    public void OnUpdate20FPS()
    {
        this.FallsOffset += 0.05f;
        this.FallsOffset %= 1f;
        this.Falls.SetFloat("_Offset", this.FallsOffset);
        this.StreamOffset += 0.018f;
        this.StreamOffset %= 1f;
        this.Stream.SetFloat("_Offset", this.StreamOffset);
    }

    public void UpdateVolcanoCrater1()
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = this.VolcanoCrater1;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0f, (Single)this.VolcanoCrater1.width, (Single)this.VolcanoCrater1.height, 0f);
        if (this.VolcanoCrater1Enabled)
        {
            Rect screenRect = new Rect(0f, 0f, (Single)this.VolcanoCrater1Texture.width, (Single)this.VolcanoCrater1Texture.height);
            Graphics.DrawTexture(screenRect, this.VolcanoCrater1Texture, this.VolcanoCrater1Material);
        }
        GL.PopMatrix();
        RenderTexture.active = active;
    }

    public void UpdateVolcanoCrater1_Render()
    {
        if (!this.VolcanoCrater1Enabled)
        {
            return;
        }
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = this.VolcanoCrater1;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0f, (Single)this.VolcanoCrater1.width, (Single)this.VolcanoCrater1.height, 0f);
        Rect screenRect = new Rect(0f, 0f, (Single)this.VolcanoCrater1Texture.width, (Single)this.VolcanoCrater1Texture.height);
        Graphics.DrawTexture(screenRect, this.VolcanoCrater1Texture, this.VolcanoCrater1Material);
        GL.PopMatrix();
        RenderTexture.active = active;
    }

    public void UpdateVolcanoLava1()
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = this.VolcanoLava1;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0f, (Single)this.VolcanoLava1.width, (Single)this.VolcanoLava1.height, 0f);
        if (this.VolcanoLava1Enabled)
        {
            Rect screenRect = new Rect(0f, 0f, (Single)this.VolcanoLava1Texture.width, (Single)this.VolcanoLava1Texture.height);
            Graphics.DrawTexture(screenRect, this.VolcanoLava1Texture, this.VolcanoLava1Material);
        }
        GL.PopMatrix();
        RenderTexture.active = active;
    }

    public void UpdateVolcanoCrater2()
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = this.VolcanoCrater2;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0f, (Single)this.VolcanoCrater2.width, (Single)this.VolcanoCrater2.height, 0f);
        if (this.VolcanoCrater2Enabled)
        {
            Rect screenRect = new Rect(0f, 0f, (Single)this.VolcanoCrater2Texture.width, (Single)this.VolcanoCrater2Texture.height);
            Graphics.DrawTexture(screenRect, this.VolcanoCrater2Texture, this.VolcanoCrater2Material);
        }
        GL.PopMatrix();
        RenderTexture.active = active;
    }

    public void UpdateVolcanoLava2()
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = this.VolcanoLava2;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0f, (Single)this.VolcanoLava2.width, (Single)this.VolcanoLava2.height, 0f);
        if (this.VolcanoLava2Enabled)
        {
            Rect screenRect = new Rect(0f, 0f, (Single)this.VolcanoLava2Texture.width, (Single)this.VolcanoLava2Texture.height);
            Graphics.DrawTexture(screenRect, this.VolcanoLava2Texture, this.VolcanoLava2Material);
        }
        GL.PopMatrix();
        RenderTexture.active = active;
    }

    public void UpdateBeach1()
    {
        if (!this.Beach1Enabled)
        {
            return;
        }
        if (this.playingForwardBeach1)
        {
            this.currentFrameBeach1++;
            if (this.currentFrameBeach1 > 3)
            {
                this.currentFrameBeach1 = 2;
                this.playingForwardBeach1 = false;
            }
        }
        else
        {
            this.currentFrameBeach1--;
            if (this.currentFrameBeach1 < 0)
            {
                this.currentFrameBeach1 = 1;
                this.playingForwardBeach1 = true;
            }
        }
        this.timeBeach1 = 0f;
    }

    public void UpdateBeach1_Render()
    {
        if (!this.Beach1Enabled)
        {
            return;
        }
        Texture2D mainTexture = this.Beach1Textures[this.currentFrameBeach1];
        mainTexture.filterMode = filterMode;
        this.Beach1Material.mainTexture = mainTexture;
    }

    public void UpdateBeach2()
    {
        if (!this.Beach2Enabled)
        {
            return;
        }
        if (this.playingForwardBeach2)
        {
            this.currentFrameBeach2++;
            if (this.currentFrameBeach2 > 3)
            {
                this.currentFrameBeach2 = 2;
                this.playingForwardBeach2 = false;
            }
        }
        else
        {
            this.currentFrameBeach2--;
            if (this.currentFrameBeach2 < 0)
            {
                this.currentFrameBeach2 = 1;
                this.playingForwardBeach2 = true;
            }
        }
        this.timeBeach2 = 0f;
    }

    public void UpdateBeach2_Render()
    {
        if (!this.Beach2Enabled)
        {
            return;
        }
        Texture2D mainTexture = this.Beach2Textures[this.currentFrameBeach2];
        mainTexture.filterMode = filterMode;
        this.Beach2Material.mainTexture = mainTexture;
    }

    public void UpdateSea_10_64_0_Render()
    {
        if (!this.Sea_10_64_0Enabled)
        {
            return;
        }
        Texture2D mainTexture = this.Sea_10_64_0Textures[(Int32)this.Sea_IndexOffset];
        mainTexture.filterMode = filterMode;
        this.Sea_10_64_0Material.mainTexture = mainTexture;
    }

    public void UpdateSea_10_128_0_Render()
    {
        if (!this.Sea_10_128_0Enabled)
        {
            return;
        }
        Texture2D mainTexture = this.Sea_10_128_0Textures[(Int32)this.Sea_IndexOffset];
        mainTexture.filterMode = filterMode;
        this.Sea_10_128_0Material.mainTexture = mainTexture;
    }

    public void UpdateSea_10_128_64_Render()
    {
        if (!this.Sea_10_128_64Enabled)
        {
            return;
        }
        Texture2D mainTexture = this.Sea_10_128_64Textures[(Int32)this.Sea_IndexOffset];
        mainTexture.filterMode = filterMode;
        this.Sea_10_128_64Material.mainTexture = mainTexture;
    }

    public void UpdateSea_10_128_128_Render()
    {
        if (!this.Sea_10_128_128Enabled)
        {
            return;
        }
        Texture2D mainTexture = this.Sea_10_128_128Textures[(Int32)this.Sea_IndexOffset];
        mainTexture.filterMode = filterMode;
        this.Sea_10_128_128Material.mainTexture = mainTexture;
    }

    public void UpdateSea_11_64_0_Render()
    {
        if (!this.Sea_11_64_0Enabled)
        {
            return;
        }
        Texture2D mainTexture = this.Sea_11_64_0Textures[(Int32)this.Sea_IndexOffset];
        mainTexture.filterMode = filterMode;
        this.Sea_11_64_0Material.mainTexture = mainTexture;
    }

    public void UpdateSea_11_192_64_Render()
    {
        if (!this.Sea_11_192_64Enabled)
        {
            return;
        }
        Texture2D mainTexture = this.Sea_11_192_64Textures[(Int32)this.Sea6_IndexOffset];
        mainTexture.filterMode = filterMode;
        this.Sea_11_192_64Material.mainTexture = mainTexture;
    }

    public void UpdateRiver_Render()
    {
        if (!this.RiverEnabled)
        {
            return;
        }
        Texture2D mainTexture = this.RiverTextures[(Int32)this.River_IndexOffset];
        mainTexture.filterMode = filterMode;
        this.RiverMaterial.mainTexture = mainTexture;
    }

    public void UpdateRiverJoint_Render()
    {
        if (!this.RiverJointEnabled)
        {
            return;
        }
        Texture2D mainTexture = this.RiverJointTextures[(Int32)this.River_IndexOffset];
        mainTexture.filterMode = filterMode;
        this.RiverJointMaterial.mainTexture = mainTexture;
    }

    // don't modify the order of these:

    public const String RenderTexturesPath = "WorldMap/RenderTextures/";
    public const String MaterialsPath = "WorldMap/Materials/";
    public const String TexturesPath = "WorldMap/Textures/";

    public RenderTexture VolcanoCrater1;
    public Material VolcanoCrater1Material;
    public Texture2D VolcanoCrater1Texture;
    public Boolean VolcanoCrater1Enabled = true;

    public RenderTexture VolcanoLava1;
    public Material VolcanoLava1Material;
    public Texture2D VolcanoLava1Texture;
    public Boolean VolcanoLava1Enabled = true;

    public RenderTexture VolcanoCrater2;
    public Material VolcanoCrater2Material;
    public Texture2D VolcanoCrater2Texture;
    public Boolean VolcanoCrater2Enabled = true;

    public RenderTexture VolcanoLava2;
    public Material VolcanoLava2Material;
    public Texture2D VolcanoLava2Texture;
    public Boolean VolcanoLava2Enabled = true;

    public Material Beach1Material;
    public Texture2D[] Beach1Textures = new Texture2D[4];
    public Boolean Beach1Enabled = true;

    public Material Beach2Material;
    public Texture2D[] Beach2Textures = new Texture2D[4];
    public Boolean Beach2Enabled = true;

    public Boolean playingForwardBeach1;
    public Int32 currentFrameBeach1;
    public Single timeBeach1 = 1f;
    public Single intervalTimeBeach1 = 1f;

    public Boolean playingForwardBeach2;
    public Int32 currentFrameBeach2;
    public Single timeBeach2 = 2f;
    public Single intervalTimeBeach2 = 2f;

    public Material RiverMaterial;
    public Texture2D[] RiverTextures = new Texture2D[6];
    public Boolean RiverEnabled = true;
    public Int32 currentFrameRiver1;

    public Material RiverJointMaterial;
    public Texture2D[] RiverJointTextures = new Texture2D[6];
    public Boolean RiverJointEnabled = true;
    public Int32 currentFrameRiverJoint1;

    public Single Sea_IndexOffset;
    public Int32 Sea_IndexOffsetMax = 6;

    public Single River_IndexOffset;
    public Int32 River_IndexOffsetMax = 6;

    public Single Sea6_IndexOffset;
    public Int32 Sea6_IndexOffsetMax = 4;

    public Material Sea_10_64_0Material;
    public Texture2D[] Sea_10_64_0Textures = new Texture2D[6];
    public Boolean Sea_10_64_0Enabled = true;

    public Material Sea_10_128_0Material;
    public Texture2D[] Sea_10_128_0Textures = new Texture2D[6];
    public Boolean Sea_10_128_0Enabled = true;

    public Material Sea_10_128_64Material;
    public Texture2D[] Sea_10_128_64Textures = new Texture2D[6];
    public Boolean Sea_10_128_64Enabled = true;

    public Material Sea_10_128_128Material;
    public Texture2D[] Sea_10_128_128Textures = new Texture2D[6];
    public Boolean Sea_10_128_128Enabled = true;

    public Material Sea_11_64_0Material;
    public Texture2D[] Sea_11_64_0Textures = new Texture2D[6];
    public Boolean Sea_11_64_0Enabled = true;

    public Material Sea_11_192_64Material;
    public Texture2D[] Sea_11_192_64Textures = new Texture2D[6];
    public Boolean Sea_11_192_64Enabled = true;

    public Material Falls;
    public Single FallsOffset;
    public Boolean FallsEnabled = true;

    public Material Stream;
    public Single StreamOffset;
    public Boolean StreamEnabled = true;

    private Boolean initialized;
    
    [NonSerialized]
    private FilterMode filterMode = ModelFactory.GetFilterMode(Configuration.Graphics.WorldSmoothTexture, 1);
}
