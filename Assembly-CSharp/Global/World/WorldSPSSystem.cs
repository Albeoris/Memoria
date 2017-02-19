using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldSPSSystem : MonoBehaviour
{
	public List<WorldSPS> SpsList
	{
		get
		{
			return this._spsList;
		}
	}

	public void Init()
	{
		this._isReady = false;
		this._spsList = new List<WorldSPS>();
		this._spsBinDict = new Dictionary<Int32, KeyValuePair<Int32, Byte[]>>();
		for (Int32 i = 0; i < 50; i++)
		{
			GameObject gameObject = new GameObject("SPS_" + i.ToString("D4"));
			gameObject.transform.parent = base.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			WorldSPS worldSPS = gameObject.AddComponent<WorldSPS>();
			worldSPS.Init();
			worldSPS.spsIndex = i;
			worldSPS.spsTransform = gameObject.transform;
			worldSPS.meshRenderer = meshRenderer;
			worldSPS.meshFilter = meshFilter;
			this._spsList.Add(worldSPS);
		}
		for (Int32 j = 0; j < 40; j++)
		{
			if (!this._loadSPSBin(j))
			{
				global::Debug.Log("Can't load sps id : " + j);
			}
		}
		this._isReady = this._loadSPSTexture();
	}

	public void Service()
	{
		if (!this._isReady)
		{
			return;
		}
		for (Int32 i = 0; i < this._spsList.Count; i++)
		{
			WorldSPS worldSPS = this._spsList[i];
			if (worldSPS.spsBin != null)
			{
				if (worldSPS.no != -1)
				{
					Vector3 pos = worldSPS.pos;
					Vector3 position = pos;
					Vector3 rot = default(Vector3);
					Int32 no = worldSPS.no;
					switch (no)
					{
					case 14:
					case 18:
					case 19:
					{
						if (ff9.w_effectMoveStockHeight > 4095)
						{
							ff9.w_effectMoveStockHeight = 4095;
						}
						if (ff9.w_effectMoveStockHeight < 0)
						{
							ff9.w_effectMoveStockHeight = 0;
						}
						ff9.w_effectMoveStockHeightTrue += (ff9.w_effectMoveStockHeight - ff9.w_effectMoveStockHeightTrue) / 32;
						Int32 num = ff9.w_effectMoveStockTrue * ff9.w_effectMoveStockHeightTrue >> 12;
						ff9.w_frameShadowOTOffset = 50;
						if (num > 24)
						{
							worldSPS.S_WoSpsPut(worldSPS.spsBin, pos, rot, worldSPS.size, worldSPS.frame, worldSPS.type, num, 0, 0);
						}
						break;
					}
					case 15:
					case 22:
						rot = new Vector3(1024f, 0f, 0f);
						ff9.w_frameShadowOTOffset = 20;
						worldSPS.S_WoSpsPut(worldSPS.spsBin, pos, rot, worldSPS.size, worldSPS.frame, worldSPS.type, ff9.w_effectMoveStockTrue, 0, 0);
						break;
					case 16:
						rot = new Vector3(ff9.PsxRot(1024), 0f, 0f);
						ff9.w_frameShadowOTOffset = 20;
						position.y += ff9.S(400);
						worldSPS.size = (8 - worldSPS.frame) * 4000;
						if (ff9.effect16FrameCounter % 2 == 0)
						{
							rot.y = (Single)(worldSPS.frame * 128);
							worldSPS.S_WoSpsPut(worldSPS.spsBin, pos, rot, worldSPS.size, worldSPS.frame, 1, -1, 0, 0);
						}
						else
						{
							rot.y = (Single)(worldSPS.frame * 200);
							worldSPS.S_WoSpsPut(worldSPS.spsBin, pos, rot, worldSPS.size, worldSPS.frame, 2, -1, 0, 0);
						}
						ff9.effect16FrameCounter++;
						break;
					case 17:
					case 20:
					case 21:
					case 23:
					case 24:
						IL_93:
						if (no != 2)
						{
							if (no != 3)
							{
								if (no != 8)
								{
									ff9.w_frameShadowOTOffset = 40;
									worldSPS.S_WoSpsPut(worldSPS.spsBin, pos, rot, worldSPS.size, worldSPS.frame, worldSPS.type, -1, 0, 0);
								}
								else
								{
									global::Debug.LogWarning("Check this please!");
								}
							}
							else
							{
								ff9.w_frameShadowOTOffset = 0;
								worldSPS.size = worldSPS.frame * 400 + 6000;
								worldSPS.S_WoSpsPut(worldSPS.spsBin, pos, rot, worldSPS.size, worldSPS.frame / 2 % 23, worldSPS.type, -1, 0, 2);
							}
						}
						else
						{
							ff9.w_frameShadowOTOffset = 0;
							worldSPS.size = worldSPS.frame * 400 + 6000;
							worldSPS.S_WoSpsPut(worldSPS.spsBin, pos, rot, worldSPS.size, worldSPS.frame / 4 % 26, worldSPS.type, -1, 0, 2);
						}
						break;
					case 25:
					case 26:
					{
						rot = new Vector3(0f, 0f, 0f);
						Single y = ff9.S(196657) - worldSPS.pos[0];
						Single x = ff9.S(-81774) - worldSPS.pos[2];
						rot.y = (Single)ff9.UnityRot((Single)((Int16)ff9.ratan2(y, x)));
						ff9.w_frameShadowOTOffset = 0;
						worldSPS.S_WoSpsPut(worldSPS.spsBin, pos, rot, worldSPS.size, worldSPS.frame, worldSPS.type, -1, 0, 0);
						break;
					}
					default:
						goto IL_93;
					}
					worldSPS.transform.position = position;
					worldSPS.transform.rotation = Quaternion.Euler(worldSPS.rot);
					Single num2 = ff9.PsxScale(worldSPS.size);
					worldSPS.transform.localScale = new Vector3(num2, num2, num2) * 0.00390625f;
				}
			}
		}
	}

	public void EffectUpdate()
	{
		Boolean flag = false;
		Int32 speed_move = (Int32)ff9.w_moveCHRControlPtr.speed_move;
		if (speed_move != 0)
		{
			ff9.w_effectMoveStock = (Int32)ff9.abs(ff9.w_moveCHRControl_XZSpeed * 256f * 4096f) / speed_move;
		}
		else
		{
			ff9.w_effectMoveStock = 0;
		}
		ff9.w_effectMoveStockTrue += (ff9.w_effectMoveStock - ff9.w_effectMoveStockTrue) / 32;
		for (Int32 i = 0; i < this._spsList.Count; i++)
		{
			WorldSPS worldSPS = this._spsList[i];
			if (worldSPS.spsBin != null)
			{
				if (worldSPS.no != -1)
				{
					Int32 no = worldSPS.no;
					if (no != 2)
					{
						if (no != 3)
						{
							if (no != 23)
							{
								if (no == 24)
								{
									WorldSPS worldSPS2 = worldSPS;
									Int32 index2;
									Int32 index = index2 = 0;
									Single num = worldSPS2.pos[index2];
									worldSPS2.pos[index] = num + ff9.S(-(ff9.random8() % 130 + 40));
									WorldSPS worldSPS3 = worldSPS;
									Int32 index3 = index2 = 1;
									num = worldSPS3.pos[index2];
									worldSPS3.pos[index3] = num + ff9.S(ff9.random8() % 80 - 40);
									WorldSPS worldSPS4 = worldSPS;
									Int32 index4 = index2 = 2;
									num = worldSPS4.pos[index2];
									worldSPS4.pos[index4] = num + ff9.S(ff9.random8() % 300 + 300);
								}
							}
							else
							{
								worldSPS.pos[0] = (Single)(ff9.rsin(worldSPS.frame * 64 + worldSPS.prm0) / 2) + ff9.w_effectTwisPos.x;
								WorldSPS worldSPS5 = worldSPS;
								Int32 index2;
								Int32 index5 = index2 = 1;
								Single num = worldSPS5.pos[index2];
								worldSPS5.pos[index5] = num - 80f;
								worldSPS.pos[2] = (Single)(ff9.rcos(worldSPS.frame * 64 + worldSPS.prm0) / 2) + ff9.w_effectTwisPos.z;
							}
						}
						else
						{
							WorldSPS worldSPS6 = worldSPS;
							Int32 index2;
							Int32 index6 = index2 = 1;
							Single num = worldSPS6.pos[index2];
							worldSPS6.pos[index6] = num + ff9.S(ff9.random8() % 24 + 60);
							WorldSPS worldSPS7 = worldSPS;
							Int32 index7 = index2 = 0;
							num = worldSPS7.pos[index2];
							worldSPS7.pos[index7] = num + ff9.S(ff9.random8() % 60 - 60);
							WorldSPS worldSPS8 = worldSPS;
							Int32 index8 = index2 = 2;
							num = worldSPS8.pos[index2];
							worldSPS8.pos[index8] = num + ff9.S(ff9.random8() % 60 - 30);
						}
					}
					else
					{
						WorldSPS worldSPS9 = worldSPS;
						Int32 index2;
						Int32 index9 = index2 = 1;
						Single num = worldSPS9.pos[index2];
						worldSPS9.pos[index9] = num + ff9.S(ff9.random8() % 24 + 60);
						WorldSPS worldSPS10 = worldSPS;
						Int32 index10 = index2 = 0;
						num = worldSPS10.pos[index2];
						worldSPS10.pos[index10] = num + ff9.S(ff9.random8() % 60 - 30);
						WorldSPS worldSPS11 = worldSPS;
						Int32 index11 = index2 = 2;
						num = worldSPS11.pos[index2];
						worldSPS11.pos[index11] = num + ff9.S(ff9.random8() % 60 - 30);
					}
					worldSPS.frame++;
					WorldSPS worldSPS12 = worldSPS;
					Int32 index12 = i;
					worldSPS = worldSPS.next;
					no = worldSPS12.no;
					if (no != 2)
					{
						if (no != 3)
						{
							if (no != 16 && no != 17)
							{
								if (no != 32)
								{
									if (worldSPS12.frame == this._GetSpsFrameCount(worldSPS12.spsBin))
									{
										this.w_effectRelease(index12);
									}
								}
								else if (ff9.abs(worldSPS12.pos[0] - ff9.w_moveActorPtr.pos[0]) > ff9.S(32000) || ff9.abs(worldSPS12.pos[2] - ff9.w_moveActorPtr.pos[2]) > ff9.S(32000) || flag)
								{
									this.w_effectRelease(index12);
								}
							}
							else if (worldSPS12.frame > 8)
							{
								this.w_effectRelease(index12);
							}
						}
						else if (worldSPS12.frame > 45)
						{
							this.w_effectRelease(index12);
						}
					}
					else if (worldSPS12.frame > 76)
					{
						this.w_effectRelease(index12);
					}
				}
			}
		}
	}

	public void w_effectUpdateSP(Byte no)
	{
		no = (Byte)(no - 1);
		if ((Int32)no >= (Int32)ff9.w_effectDataList.Length)
		{
			global::Debug.LogWarning("no " + no + " doesn't exist");
			return;
		}
		for (Int32 i = 0; i < ff9.w_effectDataList[(Int32)no].effectData.Count; i++)
		{
			ff9.s_effectData s_effectData = ff9.w_effectDataList[(Int32)no].effectData[i];
			if (UnityEngine.Random.Range(0, 4096) < (Int32)s_effectData.rnd)
			{
				Int32 num = s_effectData.x - s_effectData.rx / 2;
				Int32 num2 = s_effectData.y - s_effectData.ry / 2;
				Int32 num3 = s_effectData.z - s_effectData.rz / 2;
				if (s_effectData.rx != 0)
				{
					num += ff9.random16() % s_effectData.rx;
				}
				if (s_effectData.ry != 0)
				{
					num2 += ff9.random16() % s_effectData.ry;
				}
				if (s_effectData.rz != 0)
				{
					num3 += ff9.random16() % s_effectData.rz;
				}
				ff9.w_effectRegist_FixedPoint(num, num2, num3, (Int32)s_effectData.no, (Int32)s_effectData.size);
			}
		}
	}

	public Int32 w_effectRegist(Single x, Single y, Single z, Int32 no, Int32 size)
	{
		if (!this._spsBinDict.ContainsKey(no))
		{
			return -1;
		}
		Int32 num = this._findFreeEffectSlot();
		if (num == -1)
		{
			return -1;
		}
		Int32 type;
		switch (no)
		{
		case 14:
			type = 1;
			break;
		case 15:
			type = 1;
			break;
		case 16:
			type = 255;
			break;
		default:
			if (no != 2)
			{
				if (no != 3)
				{
					if (no != 29)
					{
						type = 1;
					}
					else
					{
						type = 1;
					}
				}
				else
				{
					type = 2;
				}
			}
			else
			{
				type = 2;
			}
			break;
		}
		WorldSPS worldSPS = this._spsList[num];
		worldSPS.type = type;
		worldSPS.frame = 0;
		worldSPS.pos.x = x;
		worldSPS.pos.y = y;
		worldSPS.pos.z = z;
		worldSPS.spsBin = this._spsBinDict[no].Value;
		worldSPS.size = size;
		worldSPS.no = no;
		worldSPS.prm0 = 0;
		worldSPS.prm1 = 0;
		worldSPS.meshRenderer.enabled = true;
		return num;
	}

	public void ShiftRight()
	{
		for (Int32 i = 0; i < this._spsList.Count; i++)
		{
			WorldSPS worldSPS = this._spsList[i];
			if (worldSPS.no != -1)
			{
				Vector3 position = worldSPS.transform.position;
				position.x += 64f;
				if (position.x >= 1536f)
				{
					position.x -= 1536f;
				}
				worldSPS.pos = position;
				worldSPS.transform.position = position;
			}
		}
	}

	public void ShiftLeft()
	{
		for (Int32 i = 0; i < this._spsList.Count; i++)
		{
			WorldSPS worldSPS = this._spsList[i];
			if (worldSPS.no != -1)
			{
				Vector3 position = worldSPS.transform.position;
				position.x -= 64f;
				if (position.x < 0f)
				{
					position.x += 1536f;
				}
				worldSPS.pos = position;
				worldSPS.transform.position = position;
			}
		}
	}

	public void ShiftDown()
	{
		for (Int32 i = 0; i < this._spsList.Count; i++)
		{
			WorldSPS worldSPS = this._spsList[i];
			if (worldSPS.no != -1)
			{
				Vector3 position = worldSPS.transform.position;
				position.z -= 64f;
				if (position.z <= -1280f)
				{
					position.z += 1280f;
				}
				worldSPS.pos = position;
				worldSPS.transform.position = position;
			}
		}
	}

	public void ShiftUp()
	{
		for (Int32 i = 0; i < this._spsList.Count; i++)
		{
			WorldSPS worldSPS = this._spsList[i];
			if (worldSPS.no != -1)
			{
				Vector3 position = worldSPS.transform.position;
				position.z += 64f;
				if (position.z > 0f)
				{
					position.z -= 1280f;
				}
				worldSPS.pos = position;
				worldSPS.transform.position = position;
			}
		}
	}

	public void w_effectRelease(Int32 index)
	{
		WorldSPS worldSPS = this._spsList[index];
		worldSPS.spsBin = null;
		worldSPS.no = -1;
		worldSPS.meshRenderer.enabled = false;
	}

	public Boolean IsSlotAvailiable(Int32 index)
	{
		return this._spsList[index].no == -1;
	}

	private Int32 _findFreeEffectSlot()
	{
		for (Int32 i = 0; i < this._spsList.Count; i++)
		{
			if (this.IsSlotAvailiable(i))
			{
				return i;
			}
		}
		global::Debug.LogWarning("WorldSPSSystem : Cannot find free slot");
		return -1;
	}

	private Boolean _loadSPSTexture()
	{
		TextAsset textAsset = AssetManager.Load<TextAsset>("EmbeddedAsset/WorldSPS/fx.tcb", false);
		if (textAsset != (UnityEngine.Object)null)
		{
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(textAsset.bytes)))
			{
				UInt32 num = binaryReader.ReadUInt32();
				UInt32 num2 = binaryReader.ReadUInt32();
				Int32 num3 = binaryReader.ReadInt32();
				binaryReader.BaseStream.Seek((Int64)((UInt64)num), SeekOrigin.Begin);
				UInt32 num4 = binaryReader.ReadUInt32();
				Int32 num5 = binaryReader.ReadInt32();
				for (Int32 i = 0; i < num3; i++)
				{
					binaryReader.BaseStream.Seek((Int64)((UInt64)num2), SeekOrigin.Begin);
					Int32 x = (Int32)binaryReader.ReadInt16();
					Int32 y = (Int32)binaryReader.ReadInt16();
					Int32 num6 = (Int32)binaryReader.ReadInt16();
					Int32 num7 = (Int32)binaryReader.ReadInt16();
					PSXTextureMgr.LoadImageBin(x, y, num6, num7, binaryReader);
					UInt32 num8 = (UInt32)(num6 * num7 * 2);
					num2 += num8 + 8u;
				}
				num += 8u;
				for (Int32 j = 0; j < num5; j++)
				{
					binaryReader.BaseStream.Seek((Int64)((UInt64)num), SeekOrigin.Begin);
					Int32 x2 = (Int32)binaryReader.ReadInt16();
					Int32 y2 = (Int32)binaryReader.ReadInt16();
					Int32 num9 = (Int32)binaryReader.ReadInt16();
					Int32 num10 = (Int32)binaryReader.ReadInt16();
					binaryReader.BaseStream.Seek((Int64)((UInt64)num4), SeekOrigin.Begin);
					PSXTextureMgr.LoadImageBin(x2, y2, num9, num10, binaryReader);
					UInt32 num8 = (UInt32)(num9 * num10 * 2);
					num4 += num8;
					num += 8u;
				}
			}
			PSXTextureMgr.ClearObject();
			return true;
		}
		return false;
	}

	private Int32 _GetSpsFrameCount(Byte[] spsBin)
	{
		return (Int32)(BitConverter.ToUInt16(spsBin, 0) & 32767);
	}

	public Int32 GetSpsFrameCount(Byte[] spsBin)
	{
		return (Int32)(BitConverter.ToUInt16(spsBin, 0) & 32767);
	}

	private Boolean _loadSPSBin(Int32 spsNo)
	{
		if (this._spsBinDict.ContainsKey(spsNo))
		{
			return true;
		}
		TextAsset textAsset = AssetManager.Load<TextAsset>("EmbeddedAsset/WorldSPS/fx" + spsNo.ToString("D2") + ".sps", false);
		if (textAsset == (UnityEngine.Object)null)
		{
			return false;
		}
		Byte[] bytes = textAsset.bytes;
		Int32 key = this._GetSpsFrameCount(bytes);
		this._spsBinDict.Add(spsNo, new KeyValuePair<Int32, Byte[]>(key, bytes));
		return true;
	}

	private Boolean _isReady;

	private List<WorldSPS> _spsList;

	private Dictionary<Int32, KeyValuePair<Int32, Byte[]>> _spsBinDict;
}
