using Assets.Scripts.Common;
using FF9;
using Memoria;
using Memoria.Scripts;
using System;
using UnityEngine;

public class BattleRainRenderer : MonoBehaviour
{
    private void Awake()
    {
        this.randSeed = -1;
        this.maxRain = 31;
        this.nf_BbgRainFlag = (Int32)FF9StateSystem.Common.FF9.btl_rain;
        if (this.nf_BbgRainFlag > this.maxRain)
        {
            this.nf_BbgRainFlag = this.maxRain;
        }
        this.mat = ShadersLoader.CreateShaderMaterial("SPS/SPSRain");
        if (this.nf_BbgRainFlag > 0 && !String.IsNullOrEmpty(WorldConfiguration.BattleRainSoundPath))
        {
            this._rainSoundPlayer = new WorldSoundPlayer();
            this._rainSoundPlayer.Load(WorldConfiguration.BattleRainSoundPath);
        }
    }

    public void nf_BbgRain()
    {
        if (PersistenSingleton<SceneDirector>.Instance.IsFading)
        {
            return;
        }
        this.nf_BbgRainFlag = (Int32)FF9StateSystem.Common.FF9.btl_rain;
        if (this.nf_BbgRainFlag > this.maxRain)
        {
            this.nf_BbgRainFlag = this.maxRain;
        }
        if (this.nf_BbgRainFlag == 0)
        {
            if (this._rainSoundPlaying)
            {
                this._rainSoundPlaying = false;
                this._rainSoundPlayer?.Stop();
            }
            return;
        }
        // Play rain sound if loaded
        if (this._rainSoundPlayer != null)
        {
            Single vol = Math.Min(1f, this.nf_BbgRainFlag / 16f);
            this._rainSoundPlayer.Play(vol);
            this._rainSoundPlaying = true;
        }
        GL.PushMatrix();
        this.mat.SetPass(0);
        GL.Begin(7);
        if ((FF9StateSystem.Battle.FF9Battle.attr & ff9btl.ATTR.NOPUTDISPENV) != 0)
        {
            if (this.randSeed == -1)
            {
                this.randSeed = UnityEngine.Random.seed;
            }
            else
            {
                UnityEngine.Random.seed = this.randSeed;
            }
        }
        else if (this.randSeed != -1)
        {
            this.randSeed = -1;
        }
        for (Int32 i = 0; i < this.nf_BbgRainFlag; i++)
        {
            Vector3 vector;
            vector.x = (Single)(((this._rand() & 511) - 256) * 41 / 32);
            vector.y = (Single)((this._rand() & 255) * 50 / 32 - 220);
            vector.z = (Single)(((this._rand() & 511) - 256) * 41 / 32);
            vector.x *= 10f;
            vector.y *= 10f;
            vector.z *= 10f;
            Vector3 end = vector;
            end.y += (Single)((85 + (this._rand() & 31)) * 10);
            vector.y *= -1f;
            end.y *= -1f;
            Color col = new Color32(25, 25, 50, Byte.MaxValue);
            Color col2 = new Color32(80, 80, 130, Byte.MaxValue);
            BattleRainRenderer.DrawBattleRain(vector, end, col, col2);
        }
        GL.End();
        GL.PopMatrix();
    }

    private Int32 _rand()
    {
        return UnityEngine.Random.Range(-4095, 4095);
    }

    public static void DrawBattleRain(Vector3 start, Vector3 end, Color col0, Color col1)
    {
        Vector3 lhs = Vector3.Cross(start, end);
        Vector3 a = Vector3.Cross(lhs, end - start);
        a.Normalize();
        Vector3 v = start + a * 5f;
        Vector3 v2 = start + a * -5f;
        Vector3 v3 = end + a * 5f;
        Vector3 v4 = end + a * -5f;
        GL.Color(col1);
        GL.Vertex(v4);
        GL.Color(col1);
        GL.Vertex(v3);
        GL.Color(col0);
        GL.Vertex(v);
        GL.Color(col0);
        GL.Vertex(v2);
    }

    public Int32 maxRain;

    public Int32 nf_BbgRainFlag;

    private Material mat;

    private Int32 randSeed = -1;

    private WorldSoundPlayer _rainSoundPlayer;

    private Boolean _rainSoundPlaying;

    private void OnDestroy()
    {
        _rainSoundPlayer?.Unload();
    }
}
