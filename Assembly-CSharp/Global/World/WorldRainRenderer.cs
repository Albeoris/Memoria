using Assets.Scripts.Common;
using Memoria.Scripts;
using System;
using UnityEngine;

public class WorldRainRenderer : MonoBehaviour
{
	private void Awake()
	{
		this.InitRain();
	}

	private void OnPostRender()
	{
		if (PersistenSingleton<SceneDirector>.Instance.IsFading)
			return;
		SFXData.RenderEventSFX();
		if (this.numRain == 0)
			return;
		GL.PushMatrix();
		this.mat.SetPass(0);
		GL.Begin(7);
		for (Int32 i = 0; i < this.numRain; i++)
		{
			WorldRainRenderer.Rain rain = this.rainList[i];
			Vector3 p = rain.p0;
			Vector3 p2 = rain.p1;
			Vector3 p3 = rain.p2;
			p.x -= this.offset.x;
			p2.x -= this.offset.x;
			p3.x -= this.offset.x;
			p.y -= this.offset.y;
			p2.y -= this.offset.y;
			p3.y -= this.offset.y;
			p.z -= this.offset.z;
			p2.z -= this.offset.z;
			p3.z -= this.offset.z;
			WorldRainRenderer.DrawWorldRain(p, p2, rain.col0, rain.col1);
			WorldRainRenderer.DrawWorldRain(p2, p3, rain.col1, rain.col0);
		}
		GL.End();
		GL.PopMatrix();
	}

	public void InitRain()
	{
		this.maxRain = 222;
		this.rainList = new WorldRainRenderer.Rain[this.maxRain];
		for (Int32 i = 0; i < this.maxRain; i++)
		{
			this.rainList[i] = new WorldRainRenderer.Rain();
		}
		this.numRain = 0;
		this.strength = 0;
		this.speed = 0;
		this.over = 0;
		this.gen = 0;
		this.offset = new Vector3(0f, -3474f, 0f);
		this.factor = 1f;
		this.mat = new Material(ShadersLoader.Find("SPS/SPSRain"));
	}

	public void ServiceRain()
	{
		if (this.gen < 2)
		{
			this._GenerateRain((Int32)((this.speed != 4080) ? -4000 : -6000));
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
		if (strength != 0 && speed != 0)
		{
			this.strength = Mathf.Min(130, strength);
		}
		this.speed = speed << 4;
		this.gen = (Int32)((strength != 0 || speed == 0) ? 0 : 1);
		this.strength = strength;
		this.speed /= 4;
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
		Vector3 position = ff9.w_moveActorPtr.transform.position;
		for (Int32 i = 0; i < num; i++)
		{
			if (this.numRain == this.maxRain)
			{
				this.over = 1;
				break;
			}
			UInt32 num2 = this._rnd521();
			UInt32 num3 = num2 << 16;
			num3 >>= 19;
			UInt32 num4 = num3 & 65535u;
			this.rainList[i].pos.x = (Single)((Int32)((Int16)num4) * this._rndSign()) * 0.5f * 0.25f + position.x;
			num3 = (num2 & 255u);
			num3 += (UInt32)height;
			this.rainList[i].pos.y = (Single)((Int16)num3);
			num3 = num2 >> 19;
			this.rainList[i].pos.z = (Single)((Int32)((Int16)num3) * this._rndSign()) * 0.5f * 0.25f + position.z;
			this.numRain++;
		}
	}

	private void _RTPTRain(WorldRainRenderer.Rain p, Int32 rainSpeed, Int32 hold)
	{
		Vector3 pos = p.pos;
		p.p0 = pos;
		p.p1 = pos;
		p.p1.y = p.p1.y + (Single)rainSpeed * this.factor;
		if (hold == 0)
		{
			p.pos = p.p1;
		}
		p.p2 = p.p1;
		p.p2.y = p.p2.y + (Single)rainSpeed * this.factor;
	}

	private void _RainLoop()
	{
		Camera component = base.gameObject.GetComponent<Camera>();
		Vector3 position = component.transform.position;
		for (Int32 i = 0; i < this.numRain; i++)
		{
			WorldRainRenderer.Rain rain = this.rainList[i];
			this._RTPTRain(rain, this.speed, this.gen);
			rain.col0 = new Color(0f, 0f, 0f, 0f);
			Int32 num = Mathf.FloorToInt((position - rain.pos).magnitude);
			Int32 num2 = num >> 6;
			Int32 num3 = 240;
			num3 -= num2;
			rain.col1 = new Color((Single)num3 / 255f, (Single)num3 / 255f, (Single)num3 / 255f, 1f);
		}
	}

	public static void DrawWorldRain(Vector3 start, Vector3 end, Color col0, Color col1)
	{
		Vector3 lhs = Vector3.Cross(start, end);
		Vector3 a = Vector3.Cross(lhs, end - start);
		a.Normalize();
		Vector3 v = start + a * 0.5f;
		Vector3 v2 = start + a * -0.5f;
		Vector3 v3 = end + a * 0.5f;
		Vector3 v4 = end + a * -0.5f;
		GL.Color(col1);
		GL.Vertex(v4);
		GL.Color(col1);
		GL.Vertex(v3);
		GL.Color(col0);
		GL.Vertex(v);
		GL.Color(col0);
		GL.Vertex(v2);
	}

	public WorldRainRenderer.Rain[] rainList;

	public Int32 numRain;

	public Int32 maxRain;

	public Int32 strength;

	public Int32 speed;

	public Int32 over;

	public Int32 gen;

	private Material mat;

	public Single factor;

	public Vector3 offset;

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
