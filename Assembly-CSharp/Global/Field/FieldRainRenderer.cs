using System;
using Assets.Scripts.Common;
using Memoria.Scripts;
using UnityEngine;
using Object = System.Object;

public class FieldRainRenderer : MonoBehaviour
{
	private void Awake()
	{
		this.InitRain();
	}

	public void InitRain()
	{
		this.maxRain = 222;
		this.rainList = new FieldRainRenderer.Rain[this.maxRain];
		for (Int32 i = 0; i < this.maxRain; i++)
		{
			this.rainList[i] = new FieldRainRenderer.Rain();
		}
		this.numRain = 0;
		this.strength = 0;
		this.speed = 0;
		this.over = 0;
		this.gen = 0;
		this.offset = new Vector2(-FieldMap.HalfFieldWidth, 0f);
		this.factor = 1f;
		this.mat = new Material(ShadersLoader.Find("SPS/SPSRain"));
		String b = "FBG_N32_IFUG_MAP568_IU_SDV_0";
		this.isIifaTreeMap = false;
		this.posOffset = Vector3.zero;
		if (FF9StateSystem.Field.SceneName == b)
		{
			this.isIifaTreeMap = true;
			this.posOffset = new Vector3(1000f, 0f, 1000f);
		}
	}

	private void OnPostRender()
	{
		if (PersistenSingleton<SceneDirector>.Instance.IsFading)
		{
			return;
		}
		if (this.numRain == 0)
		{
			return;
		}
		GL.PushMatrix();
		this.mat.SetPass(0);
		GL.LoadOrtho();
		GL.Begin(7);
		for (Int32 i = 0; i < this.numRain; i++)
		{
			FieldRainRenderer.Rain rain = this.rainList[i];
			Vector3 p = rain.p0;
			Vector3 p2 = rain.p1;
			Vector3 p3 = rain.p2;
			p.x -= this.offset.x;
			p2.x -= this.offset.x;
			p3.x -= this.offset.x;
			p.y -= this.offset.y;
			p2.y -= this.offset.y;
			p3.y -= this.offset.y;
			p.x /= FieldMap.PsxFieldWidth;
			p2.x /= FieldMap.PsxFieldWidth;
			p3.x /= FieldMap.PsxFieldWidth;
			p.y /= FieldMap.PsxFieldHeightNative;
			p2.y /= FieldMap.PsxFieldHeightNative;
			p3.y /= FieldMap.PsxFieldHeightNative;
			p.z = 0f;
			p2.z = 0f;
			p3.z = 0f;
			this.DrawFieldRain(p, p2, rain.col0, rain.col1);
			this.DrawFieldRain(p2, p3, rain.col1, rain.col0);
		}
		GL.End();
		GL.PopMatrix();
	}

	public void DrawFieldRain(Vector3 start, Vector3 end, Color col0, Color col1)
	{
		Vector3 lhs = Vector3.Cross(start, end);
		Vector3 a = Vector3.Cross(lhs, end - start);
		a.Normalize();
		Vector3 v = start + a * 0.0015625f;
		Vector3 v2 = start - a * 0.0015625f;
		Vector3 v3 = end + a * 0.0015625f;
		Vector3 v4 = end - a * 0.0015625f;
		GL.Color(col1);
		GL.Vertex(v4);
		GL.Color(col1);
		GL.Vertex(v3);
		GL.Color(col0);
		GL.Vertex(v);
		GL.Color(col0);
		GL.Vertex(v2);
	}

	public void ServiceRain()
	{
		if (this.gen < 2)
		{
			this._GenerateRain((Int32)((this.speed != 4080) ? 4000 : 6000));
		}
		if (this.gen != 0)
		{
			this.gen++;
		}
		if (this.numRain != 0)
		{
			this._RainLoop();
		}
	}

	public void SetRainParam(Int32 strength, Int32 speed)
	{
		global::Debug.Log(String.Concat(new Object[]
		{
			"Set Rain : strength : ",
			strength,
			", speed : ",
			speed
		}));
		if (strength != 0 && speed != 0)
		{
			this.strength = Mathf.Min(130, strength);
		}
		this.speed = speed << 4;
		this.strength *= 4;
		this.speed = (Int32)((Single)this.speed * 1.5f);
		if (this.isIifaTreeMap && strength == 1 && speed == 1)
		{
			this.strength = 16;
			this.speed = 16;
		}
		this.gen = (Int32)((strength != 0 || speed == 0) ? 0 : 1);
	}

	private UInt32 _rnd521()
	{
		Byte b = (Byte)UnityEngine.Random.Range(0, 255);
		Byte b2 = (Byte)UnityEngine.Random.Range(0, 255);
		Byte b3 = (Byte)UnityEngine.Random.Range(0, 255);
		Byte b4 = (Byte)UnityEngine.Random.Range(0, 255);
		return (UInt32)((Int32)b | (Int32)b2 << 8 | (Int32)b3 << 16 | (Int32)b4 << 24);
	}

	private Int32 _rndSign()
	{
		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			return -1;
		}
		return 1;
	}

	private void _GenerateRain(Int32 height)
	{
		Int32 num = this.strength;
		this.numRain = 0;
		this.over = 0;
		for (Int32 i = 0; i < num; i++)
		{
			if (this.numRain == this.maxRain)
			{
				this.over = 1;
				break;
			}
			if (this.rainList[i].pos.y > -2000f)
			{
				FieldRainRenderer.Rain rain = this.rainList[i];
				rain.pos.y = rain.pos.y - (Single)this.speed;
				this.numRain++;
			}
			else
			{
				UInt32 num2 = this._rnd521();
				UInt32 num3 = num2 << 16;
				num3 >>= 19;
				UInt32 num4 = num3 & 65535u;
				if (this.isIifaTreeMap)
				{
					this.rainList[i].pos.x = (Single)UnityEngine.Random.Range(-2000, 4000) + this.posOffset.x;
					this.rainList[i].pos.y = (Single)UnityEngine.Random.Range(1500, 2000) + this.posOffset.y;
					this.rainList[i].pos.z = (Single)UnityEngine.Random.Range(800, 2200) + this.posOffset.z;
				}
				else
				{
					this.rainList[i].pos.x = (Single)((Int32)((Int16)num4) * this._rndSign()) * 0.5f;
					num3 = (num2 & 255u);
					num3 += (UInt32)height;
					this.rainList[i].pos.y = (Single)((Int16)num3);
					num3 = num2 >> 19;
					this.rainList[i].pos.z = (Single)((Int32)((Int16)num3) * this._rndSign()) * 0.5f;
				}
				this.numRain++;
			}
		}
	}

	private void _RTPTRain(FieldRainRenderer.Rain p, Int32 rainSpeed, Int32 hold)
	{
		Vector3 pos = p.pos;
		p.p0 = pos;
		p.p1 = pos;
		p.p1.y = p.p1.y - (Single)rainSpeed;
		if (hold == 0)
		{
			p.pos = p.p1;
		}
		p.p2 = p.p1;
		p.p2.y = p.p2.y - (Single)rainSpeed;
	}

	private void _RainLoop()
	{
		FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
		if (fieldmap == (UnityEngine.Object)null)
		{
			return;
		}
		BGCAM_DEF currentBgCamera = fieldmap.GetCurrentBgCamera();
		for (Int32 i = 0; i < this.numRain; i++)
		{
			FieldRainRenderer.Rain rain = this.rainList[i];
			this._RTPTRain(rain, this.speed, this.gen);
			Vector3 p = PSX.CalculateGTE_RTPT_POS(rain.p0, Matrix4x4.identity, currentBgCamera.GetMatrixR(), currentBgCamera.GetViewDistance(), fieldmap.GetProjectionOffset(), false);
			Vector3 p2 = PSX.CalculateGTE_RTPT_POS(rain.p1, Matrix4x4.identity, currentBgCamera.GetMatrixR(), currentBgCamera.GetViewDistance(), fieldmap.GetProjectionOffset(), false);
			Vector3 p3 = PSX.CalculateGTE_RTPT_POS(rain.p2, Matrix4x4.identity, currentBgCamera.GetMatrixR(), currentBgCamera.GetViewDistance(), fieldmap.GetProjectionOffset(), false);
			rain.p0 = p;
			rain.p1 = p2;
			rain.p2 = p3;
			rain.col0 = new Color(0f, 0f, 0f, 1f);
			Int32 num = (Int32)p.z;
			Int32 num2 = num >> 6;
			Int32 num3 = 240;
			num3 -= num2;
			rain.col1 = new Color((Single)num3 / 255f, (Single)num3 / 255f, (Single)num3 / 255f, 1f);
		}
	}

	public FieldRainRenderer.Rain[] rainList;

	public Int32 numRain;

	public Int32 maxRain;

	public Int32 strength;

	public Int32 speed;

	public Int32 over;

	public Int32 gen;

	private Material mat;

	public Single factor;

	public Vector2 offset;

	public Vector3 posOffset;

	private Boolean isIifaTreeMap;

	public class Rain
	{
		public Vector3 pos;

		public Vector3 p0;

		public Vector3 p1;

		public Vector3 p2;

		public Color col0;

		public Color col1;
	}
}
