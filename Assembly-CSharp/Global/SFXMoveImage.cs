using System;

public class SFXMoveImage : SFXMeshBase
{
	public SFXMoveImage(Int32 _rx, Int32 _ry, Int32 _rw, Int32 _rh, Int32 _x, Int32 _y)
	{
		this.rx = _rx;
		this.ry = _ry;
		this.rw = _rw;
		this.rh = _rh;
		this.x = _x;
		this.y = _y;
	}

	public override void Render(Int32 index)
	{
		PSXTextureMgr.MoveImage(this.rx, this.ry, this.rw, this.rh, this.x, this.y);
	}

	private Int32 rx;

	private Int32 ry;

	private Int32 rw;

	private Int32 rh;

	private Int32 x;

	private Int32 y;
}
