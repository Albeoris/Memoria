using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class BGOVERLAY_DEF
{
	public BGOVERLAY_DEF()
	{
		this.startOffset = 0L;
		this.spriteList = new List<BGSPRITE_LOC_DEF>();
		this.isSpecialParallax = false;
        this.canCombine = true;
        this.isCreated = false;
    }

	public void SetFlags(OVERLAY_FLAG flagDiff, Boolean isSet)
	{
		if (isSet)
		{
			this.flags |= flagDiff;
			if ((this.flags & OVERLAY_FLAG.Active) != 0)
				this.transform.gameObject.SetActive(true);
		}
		else
		{
			this.flags &= ~flagDiff;
			if ((this.flags & OVERLAY_FLAG.Active) == 0)
				this.transform.gameObject.SetActive(false);
		}
	}

	public void ReadData(BinaryReader reader)
	{
		this.startOffset = reader.BaseStream.Position;
		UInt32 buffer = reader.ReadUInt32();
		this.oriData = buffer;
		this.flags = (OVERLAY_FLAG)(buffer & 0xFFu);
		this.curZ = (UInt16)(buffer >> 8 & 0xFFFu);
		this.orgZ = (UInt16)(buffer >> 20 & 0xFFFu);
		this.w = reader.ReadUInt16();
		this.h = reader.ReadUInt16();
		this.orgX = reader.ReadInt16();
		this.orgY = reader.ReadInt16();
		this.curX = reader.ReadInt16();
		this.curY = reader.ReadInt16();
		this.minX = reader.ReadInt16();
		this.maxX = reader.ReadInt16();
		this.minY = reader.ReadInt16();
		this.maxY = reader.ReadInt16();
		this.scrX = reader.ReadInt16();
		this.scrY = reader.ReadInt16();
		this.dX = reader.ReadInt16();
		this.dY = reader.ReadInt16();
		this.fracX = reader.ReadInt16();
		this.fracY = reader.ReadInt16();
		Byte bitPos = 0;
		buffer = reader.ReadUInt32();
		this.camNdx = (Byte)BitUtil.ReadBits(buffer, ref bitPos, 8);
		this.isXOffset = (Byte)BitUtil.ReadBits(buffer, ref bitPos, 1);
		this.viewportNdx = (Byte)BitUtil.ReadBits(buffer, ref bitPos, 7);
		this.spriteCount = (UInt16)BitUtil.ReadBits(buffer, ref bitPos, 16);
		this.locOffset = reader.ReadUInt32();
		this.prmOffset = reader.ReadUInt32();
		this.sprtWork = reader.ReadUInt32();
		this.tpageWork = reader.ReadUInt32();
	}

	public OVERLAY_FLAG flags;

	public UInt16 curZ;
	public UInt16 orgZ;
	public UInt16 w;
	public UInt16 h;
	public Int16 orgX;
	public Int16 orgY;
	public Int16 curX;
	public Int16 curY;

	public Int16 minX;
	public Int16 maxX;
	public Int16 minY;
	public Int16 maxY;

	public Int16 scrX;
	public Int16 scrY;

	public Int16 dX;
	public Int16 dY;

	public Int16 fracX;
	public Int16 fracY;

	public Byte camNdx;
	public Byte isXOffset;
    public UInt32 indnum;
	public Byte viewportNdx;

	public UInt16 spriteCount;

	public UInt32 locOffset;
	public UInt32 prmOffset;
	public UInt32 sprtWork;
	public UInt32 tpageWork;
	public Int64 startOffset;

	public UInt32 oriData;

	public List<BGSPRITE_LOC_DEF> spriteList;

	public Transform transform;

    public Boolean canCombine;
    public Boolean isCreated;

    public Boolean isSpecialParallax;

	public Single parallaxCurX;
	public Single parallaxCurY;

	public Boolean isMemoria = false;
	public Vector2 memoriaSize;
	public Texture2D memoriaImage;
	public Material memoriaMaterial;

	[Flags]
	public enum OVERLAY_FLAG
	{
		ScreenAnchored = 1,
		Active = 2,
		Loop = 4,
		Parallax = 8,
		ScrollWithOffset = 128
	}
}
