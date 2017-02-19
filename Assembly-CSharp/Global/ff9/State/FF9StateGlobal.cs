using System;
using System.Collections.Generic;
using UnityEngine;

public class FF9StateGlobal
{
	public FF9StateGlobal()
	{
		this.ot = new UInt32[2];
		this.player = new PLAYER[9];
		for (Int32 i = 0; i < (Int32)this.player.Length; i++)
		{
			this.player[i] = new PLAYER();
		}
		this.party = new PARTY_DATA();
		this.item = new FF9ITEM[256];
		this.rare_item = new Byte[64];
		this.charArray = new Dictionary<Int32, FF9Char>();
		this.frog_no = 0;
		this.steal_no = 0;
		this.dragon_no = 0;
	}

	public FF9Char FF9GetCharPtr(Int32 uid)
	{
		return this.charArray[uid];
	}

	public void ff9ResetStateGlobal()
	{
		this.hintmap_id = 0;
	}

	public const Int32 FF9_SIZE_OT = 4096;

	public const Int32 FF9_BUFFER_COUNT = 2;

	public UInt32 attr;

	public Char usage;

	public Byte id;

	public Byte mainUsed;

	public UInt16 proj;

	public Int16 fldMapNo;

	public Int16 btlMapNo;

	public UInt32[] ot;

	public Matrix4x4 cam;

	public Byte npcCount;

	public Byte npcUsed;

	public Int16 fldLocNo;

	public PLAYER[] player;

	public PARTY_DATA party;

	public Int16 frog_no;

	public Int16 steal_no;

	public Int16 dragon_no;

	public Byte btl_result;

	public Byte btl_flag;

	public Byte btl_rain;

	public Byte steiner_state;

	public FF9ITEM[] item;

	public Byte[] rare_item;

	public SByte btlSubMapNo;

	public Int16 wldMapNo;

	public Int16 wldLocNo;

	public UInt16 miniGameArg;

	public Int32 hintmap_id;

	public String mapNameStr;

	public ff9.sworldState worldState = new ff9.sworldState();

	public Boolean timerControl;

	public Boolean timerDisplay;

	public Vector2 projectionOffset;

	public CharInitFuncPtr charInitFuncPtr;

	public Dictionary<Int32, FF9Char> charArray;
}
