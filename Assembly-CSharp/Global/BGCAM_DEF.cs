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
            for (Int32 j = 0; j < 3; j++)
                this.r[i, j] = reader.ReadInt16();
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

    public BGCAM_DEF Copy()
	{
        BGCAM_DEF copy = new BGCAM_DEF();
        copy.proj = this.proj;
        for (Int32 i = 0; i < 3; i++)
            for (Int32 j = 0; j < 3; j++)
                copy.r[i, j] = this.r[i, j];
        copy.t[0] = this.t[0];
        copy.t[1] = this.t[1];
        copy.t[2] = this.t[2];
        copy.centerOffset[0] = this.centerOffset[0];
        copy.centerOffset[1] = this.centerOffset[1];
        copy.w = this.w;
        copy.h = this.h;
        copy._vrpMinX = this._vrpMinX;
        copy._vrpMaxX = this._vrpMaxX;
        copy.vrpMinY = this.vrpMinY;
        copy.vrpMaxY = this.vrpMaxY;
        copy.depthOffset = this.depthOffset;
        copy.RefreshCache(true);
        return copy;
    }

    public Matrix4x4 GetMatrixR()
	{
		Matrix4x4 result = default(Matrix4x4);
		result.SetRow(1, new Vector4(this.r[1, 0] * ROTATTION_INVFACTOR, this.r[1, 1] * ROTATTION_INVFACTOR, this.r[1, 2] * ROTATTION_INVFACTOR, 0f));
		result.SetRow(2, new Vector4(this.r[2, 0] * ROTATTION_INVFACTOR, this.r[2, 1] * ROTATTION_INVFACTOR, this.r[2, 2] * ROTATTION_INVFACTOR, 0f));
		result.SetRow(0, new Vector4(this.r[0, 0] * ROTATTION_INVFACTOR, this.r[0, 1] * ROTATTION_INVFACTOR, this.r[0, 2] * ROTATTION_INVFACTOR, 0f));
		result.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		return result;
	}

	public Matrix4x4 GetMatrixRT()
	{
		Matrix4x4 result = default(Matrix4x4);
		result.SetRow(0, new Vector4(this.r[0, 0] * ROTATTION_INVFACTOR, this.r[0, 1] * ROTATTION_INVFACTOR, this.r[0, 2] * ROTATTION_INVFACTOR, this.t[0]));
		result.SetRow(1, new Vector4(this.r[1, 0] * ROTATTION_INVFACTOR, this.r[1, 1] * ROTATTION_INVFACTOR, this.r[1, 2] * ROTATTION_INVFACTOR, this.t[1]));
		result.SetRow(2, new Vector4(this.r[2, 0] * ROTATTION_INVFACTOR, this.r[2, 1] * ROTATTION_INVFACTOR, this.r[2, 2] * ROTATTION_INVFACTOR, this.t[2]));
		result.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		return result;
	}

	public Matrix4x4 GetMatrixRTI()
	{
		Matrix4x4 result = default(Matrix4x4);
		result.SetRow(1, new Vector4(this.r[1, 0] * ROTATTION_INVFACTOR, this.r[1, 1] * ROTATTION_INVFACTOR, this.r[1, 2] * ROTATTION_INVFACTOR, -this.t[1]));
		result.SetRow(2, new Vector4(this.r[2, 0] * ROTATTION_INVFACTOR, this.r[2, 1] * ROTATTION_INVFACTOR, this.r[2, 2] * ROTATTION_INVFACTOR, -this.t[2]));
		result.SetRow(0, new Vector4(this.r[0, 0] * ROTATTION_INVFACTOR, this.r[0, 1] * ROTATTION_INVFACTOR, this.r[0, 2] * ROTATTION_INVFACTOR, -this.t[0]));
		result.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		return result;
    }

    public void SetMatrixRT(Matrix4x4 matrix)
    {
        for (Int32 i = 0; i < 3; i++)
            for (Int32 j = 0; j < 3; j++)
                this.r[i, j] = (Int16)(ROTATTION_FACTOR * matrix[i, j]);
        for (Int32 i = 0; i < 3; i++)
            this.t[i] = (Int32)matrix[i, 3];
    }

    public Vector2 GetCenterOffset()
	{
		return new Vector2(this.centerOffset[0], this.centerOffset[1]);
    }

	public Single GetViewDistance()
	{
		return this.proj;
	}

	public Vector3 GetCamPos()
	{
		return new Vector3(this.t[0], this.t[1], this.t[2]);
    }

    private void RefreshCache(Boolean force)
    {
        Int16 actualFieldWidth = FieldMap.PsxFieldWidth;
        if (!force && _knownFieldWidth == actualFieldWidth)
            return;

        _knownFieldWidth = actualFieldWidth;

        if (FieldMap.IsNarrowMap())
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

    public const Single ROTATTION_FACTOR = 4096f;
    public const Single ROTATTION_INVFACTOR = 0.000244140625f; // 1 / 4096

	public Int16[,] r;
	public Int32[] t;

	public Int16[] centerOffset;

	public Int16 w;
	public Int16 h;

    public Int16 vrpMinY;
    public Int16 vrpMaxY;
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

    public UInt16 proj;
    public Int32 depthOffset;

	public Int64 startOffset;
	public Transform transform;
	public GameObject projectedWalkMesh;
}
