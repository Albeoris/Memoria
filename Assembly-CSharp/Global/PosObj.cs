using Memoria;
using System;
using UnityEngine;
using Object = System.Object;

public class PosObj : Obj
{
	public PosObj(Int32 sid, Int32 uid, Int32 size, Int32 stackn) : base(sid, uid, size, stackn)
	{
		this.pos = new Single[3];
		this.rot = new Int16[3];
		this.rotAngle = Vector3.zero;
		this.collRad = 16;
		this.talkRad = 16;
		this.model = UInt16.MaxValue;
		this.follow = Byte.MaxValue;
		this.scaley = 64;
		this.posField = Vector3.zero;
		this.rotField = Quaternion.identity;
		this.geo_struct_lookat = Vector3.zero;
	}

	public PosObj()
	{
		this.pos = new Single[3];
		this.rot = new Int16[3];
		this.rotAngle = Vector3.zero;
		this.collRad = 16;
		this.talkRad = 16;
		this.model = UInt16.MaxValue;
		this.follow = Byte.MaxValue;
		this.scaley = 64;
		this.posField = Vector3.zero;
		this.rotField = Quaternion.identity;
	}

	public void copy(PosObj po)
	{
		for (Int32 i = 0; i < 3; i++)
		{
			this.pos[i] = po.pos[i];
		}
		for (Int32 j = 0; j < 3; j++)
		{
			this.rot[j] = po.rot[j];
			this.rotAngle[j] = po.rotAngle[j];
		}
		this.bgiRad = po.bgiRad;
		this.pad0 = po.pad0;
		this.lastTri = po.lastTri;
		this.pflags = po.pflags;
		this.ovalRatio = po.ovalRatio;
		this.lastFloor = po.lastFloor;
		this.rot0 = po.rot0;
		this.eye = po.eye;
		this.model = po.model;
		this.anim = po.anim;
		this.frameN = po.frameN;
		this.lastAnimFrame = po.lastAnimFrame;
		this.animFrame = po.animFrame;
		this.modelID = po.modelID;
		this.collRad = po.collRad;
		this.talkRad = po.talkRad;
		this.follow = po.follow;
		this.scaley = po.scaley;
		this.lastx = po.lastx;
		this.lasty = po.lasty;
		this.lastz = po.lastz;
		this.posField = po.posField;
		this.rotField = po.rotField;
		this.meshflags = po.meshflags;
		this.garnet = po.garnet;
		this.shortHair = po.shortHair;
		this.attatchTargetUid = po.attatchTargetUid;
		this.attachTargetBoneIndex = po.attachTargetBoneIndex;
		this.isShadowOff = po.isShadowOff;
		Actor actor = (Actor)po;
		this.charFlags = actor.charFlags;
		this.activeTri = actor.activeTri;
		this.activeFloor = actor.activeFloor;
		if (actor.meshIsRendering != null)
		{
			this.meshIsRendering = actor.meshIsRendering;
			for (Int32 k = 0; k < (Int32)this.meshIsRendering.Length; k++)
			{
				this.meshIsRendering[k] = false;
			}
		}
		this.geo_struct_flags = actor.geo_struct_flags;
		this.geo_struct_lookat = actor.geo_struct_lookat;
		Actor actor2 = (Actor)this;
		actor2.copy(actor);
	}

	public void geoMeshHide(Int32 mesh)
	{
		this.meshflags |= (UInt32)((UInt16)(1 << mesh));
	}

	public void geoMeshShow(Int32 mesh)
	{
		this.meshflags &= (UInt32)((UInt16)(~(UInt16)(1 << mesh)));
	}

	public Int16 geoMeshChkFlags(Int32 mesh)
	{
		return (Int16)((UInt64)this.meshflags & (UInt64)(1L << (mesh & 31)));
	}

	public Int32 geoGetMeshCount()
	{
		if (this.meshIsRendering == null)
		{
			return 0;
		}
		return (Int32)this.meshIsRendering.Length;
	}

	public void SetIsEnabledMeshRenderer(Int32 mesh, Boolean isEnabled)
	{
		if (this.go != (UnityEngine.Object)null)
		{
			foreach (Object obj in this.go.transform)
			{
				Transform transform = (Transform)obj;
				String b = "mesh" + mesh;
				if (mesh == 1 && this.garnet)
				{
					Boolean garnetShortHair;

					if (Configuration.Graphics.GarnetHair == 1)
						garnetShortHair = false;
					else if (Configuration.Graphics.GarnetHair == 2)
						garnetShortHair = true;
					else
						garnetShortHair = this.shortHair;

					if (garnetShortHair)
					{
						b = "short_hair";
					}
					else
					{
						b = "long_hair";
					}
				}
				if (transform.name == b)
				{
					Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>();
					Renderer[] array = componentsInChildren;
					for (Int32 i = 0; i < (Int32)array.Length; i++)
					{
						Renderer renderer = array[i];
						if (renderer.enabled != isEnabled)
						{
							renderer.enabled = isEnabled;
						}
					}
					this.meshIsRendering[mesh] = isEnabled;
				}
			}
		}
	}

	public Vector3 rotAngle;

	public Int16[] rot;

	public Single[] pos;

	public Byte bgiRad;

	public Byte pad0;

	public Int16 activeTri;

	public Int16 lastTri;

	public Int16 charFlags;

	public Byte pflags;

	public Byte ovalRatio;

	public Byte activeFloor;

	public Byte lastFloor;

	public Single rot0;

	public Int16 eye;

	public UInt16 model;

	public UInt16 anim;

	public Byte frameN;

	public Byte lastAnimFrame;

	public Byte animFrame;

	public Byte modelID;

	public Byte collRad;

	public Byte talkRad;

	public Byte follow;

	public Byte scaley;

	public Single lastx;

	public Single lasty;

	public Single lastz;

	public Vector3 posBattle;

	public Quaternion rotBattle;

	public Vector3 posField;

	public Quaternion rotField;

	public UInt32 meshflags;

	public UInt32 tempMeshflags;

	public Boolean[] meshIsRendering;

	public Boolean garnet;

	public Boolean shortHair;

	public Boolean frontCamera = true;

	public Boolean tempfrontCamera;

	public UInt16 geo_struct_flags;

	public Vector3 geo_struct_lookat;

	public Int32 attatchTargetUid = -1;

	public Int32 attachTargetBoneIndex = -1;

	public Boolean isShadowOff;
}
