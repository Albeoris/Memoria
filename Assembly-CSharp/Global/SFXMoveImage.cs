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

    public Int32 rx;
    public Int32 ry;
    public Int32 rw;
    public Int32 rh;
    public Int32 x;
    public Int32 y;
}
