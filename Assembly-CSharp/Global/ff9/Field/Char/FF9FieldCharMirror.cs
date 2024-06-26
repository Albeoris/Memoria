using System;
using UnityEngine;

public class FF9FieldCharMirror
{
    public FF9FieldCharMirror()
    {
        this.clr = new Byte[4];
    }

    public FF9Char chr;

    public GameObject geo;

    public PosObj evt;

    public FF9Char parent;

    public Vector3 point;

    public Vector3 normal;

    public Byte[] clr;

    public FieldMapActor actor;
}
