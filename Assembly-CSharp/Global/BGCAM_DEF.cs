using System;
using System.IO;
using Memoria;
using UnityEngine;
using Object = System.Object;

public class BGCAM_DEF
{
	public BGCAM_DEF()
	{
		this.r = new Int16[3, 3];
		this.t = new Int32[3];
		this.centerOffset = new Int16[2];
		this.startOffset = 0L;
	}

    public void ReadData(BinaryReader reader)
    {
        this.startOffset = reader.BaseStream.Position;
        this.proj = reader.ReadUInt16();
        for (Int32 i = 0; i < 3; i++)
        {
            for (Int32 j = 0; j < 3; j++)
            {
                this.r[i, j] = reader.ReadInt16();
            }
        }
        this.t[0] = reader.ReadInt32();
        this.t[1] = reader.ReadInt32();
        this.t[2] = reader.ReadInt32();
        this.centerOffset[0] = reader.ReadInt16();
        this.centerOffset[1] = reader.ReadInt16();
        this.w = reader.ReadInt16();
        this.h = reader.ReadInt16();
        this._vrpMinX = reader.ReadInt16();
        this._vrpMaxX = reader.ReadInt16();
        this.vrpMinY = reader.ReadInt16();
        this.vrpMaxY = reader.ReadInt16();
        this.depthOffset = reader.ReadInt32();

        RefreshCache(true);
    }

    public Matrix4x4 GetMatrixR()
	{
		Matrix4x4 result = default(Matrix4x4);
		result.SetRow(1, new Vector4((Single)this.r[1, 0] * 0.000244140625f, (Single)this.r[1, 1] * 0.000244140625f, (Single)this.r[1, 2] * 0.000244140625f, 0f));
		result.SetRow(2, new Vector4((Single)this.r[2, 0] * 0.000244140625f, (Single)this.r[2, 1] * 0.000244140625f, (Single)this.r[2, 2] * 0.000244140625f, 0f));
		result.SetRow(0, new Vector4((Single)this.r[0, 0] * 0.000244140625f, (Single)this.r[0, 1] * 0.000244140625f, (Single)this.r[0, 2] * 0.000244140625f, 0f));
		result.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		return result;
	}

	public Matrix4x4 GetMatrixRT()
	{
		Matrix4x4 result = default(Matrix4x4);
		result.SetRow(0, new Vector4((Single)this.r[0, 0] * 0.000244140625f, (Single)this.r[0, 1] * 0.000244140625f, (Single)this.r[0, 2] * 0.000244140625f, (Single)this.t[0] * 1f));
		result.SetRow(1, new Vector4((Single)this.r[1, 0] * 0.000244140625f, (Single)this.r[1, 1] * 0.000244140625f, (Single)this.r[1, 2] * 0.000244140625f, (Single)this.t[1] * 1f));
		result.SetRow(2, new Vector4((Single)this.r[2, 0] * 0.000244140625f, (Single)this.r[2, 1] * 0.000244140625f, (Single)this.r[2, 2] * 0.000244140625f, (Single)this.t[2] * 1f));
		result.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		return result;
	}

	public Matrix4x4 GetMatrixRTI()
	{
		Matrix4x4 result = default(Matrix4x4);
		result.SetRow(1, new Vector4((Single)this.r[1, 0] * 0.000244140625f, (Single)this.r[1, 1] * 0.000244140625f, (Single)this.r[1, 2] * 0.000244140625f, (Single)(-(Single)this.t[1]) * 1f));
		result.SetRow(2, new Vector4((Single)this.r[2, 0] * 0.000244140625f, (Single)this.r[2, 1] * 0.000244140625f, (Single)this.r[2, 2] * 0.000244140625f, (Single)(-(Single)this.t[2]) * 1f));
		result.SetRow(0, new Vector4((Single)this.r[0, 0] * 0.000244140625f, (Single)this.r[0, 1] * 0.000244140625f, (Single)this.r[0, 2] * 0.000244140625f, (Single)(-(Single)this.t[0]) * 1f));
		result.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		return result;
	}

	public Vector2 GetCenterOffset()
	{
		Vector2 result = new Vector2((Single)this.centerOffset[0], (Single)this.centerOffset[1]);
		return result;
	}

	public Single GetViewDistance()
	{
		return (Single)this.proj;
	}

	public Vector3 GetCamPos()
	{
		return new Vector3((Single)this.t[0] * 1f, (Single)this.t[1] * 1f, (Single)this.t[2] * 1f);
	}

	public const Single rotFactor = 0.000244140625f;

	public const Single transFactor = 1f;

	public UInt16 proj;

	public Int16[,] r;

	public Int32[] t;

	public Int16[] centerOffset;

	public Int16 w;

	public Int16 h;

    private Int16 _vrpMinX, _vrpMaxX, _cachedMinX, _cachedMaxX, _delta, _knownFieldWidth;

    public Int16 vrpMinX
    {
        get
        {
            RefreshCache(false);
            return _cachedMinX;
        }
        set
        {
            _cachedMinX = value;
            _vrpMinX = (Int16)(value - _delta);
        }
    }

    public Int16 vrpMaxX
    {
        get
        {
            RefreshCache(false);
            return _cachedMaxX;
        }
        set
        {
            _cachedMaxX = value;
            _vrpMaxX = (Int16)(value + _delta);
        }
    }

    private void RefreshCache(Boolean force)
    {
        Int16 actualFieldWidth = FieldMap.PsxFieldWidth;
        if (!force && _knownFieldWidth == actualFieldWidth)
            return;

        _knownFieldWidth = actualFieldWidth;

	    
	bool ForceCameraToNarrow = false;
        int c = -1; //camera index
        int f = FF9StateSystem.Common.FF9.fldMapNo; //mapnumber
        GameObject go = GameObject.Find("FieldMap");
        if (go != null)
        {
            c = go.GetComponent<FieldMap>().prevCamIdx;
            
        }

        if ((f == 116 && c == 1)
            || (f == 153 && c == 0)
            || (f == 154 && c == 0)
            || (f == 352 && c == 1)
            || (f == 355 && c == 1)
            || (f == 600 && c == 1)
            || (f == 615 && c == 1)
            || (f == 801 && c == 1)
            || (f == 932 && c == 1)
            || (f == 951 && c == 1)
            || (f == 1205 && c == 1)
            || (f == 1206 && c == 1)
            || (f == 1214 && c == 0)
            || (f == 1215 && c == 0)
            || (f == 1462 && c == 0)
            || (f == 1652 && c == 1)
            || (f == 1663 && c == 0)
            || (f == 1759 && c == 1)
            || (f == 1823 && c == 1)
            || (f == 2150 && c == 1)
            || (f == 2172 && c == 1)
            || (f == 2217 && c == 1)
            || (f == 2217 && c == 2)
            || (f == 2510 && c == 0)
            || (f == 2553 && c == 1))
        {
            ForceCameraToNarrow = true;
        }
        if (FieldMap.IsNarrowMap() || ForceCameraToNarrow == true))
        {
            _cachedMinX = _vrpMinX;
            _cachedMaxX = _vrpMaxX;
            _delta = 0;
            Configuration.Graphics.DisableWidescreenSupportForSingleMap();
            return;
        }
        else
        {
	        Configuration.Graphics.RestoreDisabledWidescreenSupport();
        }

        Int32 desiredDiff = FieldMap.PsxFieldWidth - FieldMap.PsxFieldWidthNative;
        Int32 maxDiff = _vrpMaxX - _vrpMinX;
        _delta = (Int16)(Math.Min(desiredDiff, maxDiff) / 2);

        _cachedMinX = (Int16)(_vrpMinX + _delta);
        _cachedMaxX = (Int16)(_vrpMaxX - _delta);
    }

	public Int16 vrpMinY;

	public Int16 vrpMaxY;

	public Int32 depthOffset;

	public Int64 startOffset;

	public Transform transform;

	public GameObject projectedWalkMesh;
}
