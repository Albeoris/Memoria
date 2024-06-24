using System;
using System.IO;
using System.Text;
using Memoria.Scripts;
using UnityEngine;

public class GEOTEXANIMHEADER
{
    public void ReadData(BinaryReader reader)
    {
        this.flags = reader.ReadByte();
        this.numframes = reader.ReadByte();
        this.rate = reader.ReadInt16();
        this.randmin = reader.ReadUInt16();
        this.randrange = reader.ReadUInt16();
        this.frame = reader.ReadInt32();
        this.count = reader.ReadByte();
        this.texID = reader.ReadByte();
        this.lastframe = reader.ReadInt16();
        this.geotexanimoffset = reader.ReadUInt32();
        Int64 nextPos = reader.BaseStream.Position;
        this.lastframe = 1;
        reader.BaseStream.Seek(this.geotexanimoffset, SeekOrigin.Begin);
        this.target = new Rect
        {
            x = reader.ReadUInt16() * 2,
            y = reader.ReadUInt16(),
            width = reader.ReadUInt16() * 2,
            height = reader.ReadUInt16()
        };
        this.targetuv = this.target;
        UInt32 animPos = reader.ReadUInt32();
        this.coords = new Vector2[this.numframes];
        this.rectuvs = new Rect[this.numframes];
        reader.BaseStream.Seek(animPos, SeekOrigin.Begin);
        for (Int32 i = 0; i < this.numframes; i++)
        {
            Int32 x = reader.ReadInt16() * 2;
            Int32 y = reader.ReadInt16();
            this.coords[i] = new Vector2(x, y);
            this.rectuvs[i] = new Rect(x, y, this.target.width, this.target.height);
        }
        reader.BaseStream.Seek(nextPos, SeekOrigin.Begin);
    }

    public void DumpData()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("flags = " + this.flags + "\n");
        stringBuilder.Append("numframes = " + this.numframes + "\n");
        stringBuilder.Append("rate = " + this.rate + "\n");
        stringBuilder.Append("randmin = " + this.randmin + "\n");
        stringBuilder.Append("randrange = " + this.randrange + "\n");
        stringBuilder.Append("frame = " + this.frame + "\n");
        stringBuilder.Append("count = " + this.count + "\n");
        stringBuilder.Append("texID = " + this.texID + "\n");
        stringBuilder.Append("lastframe = " + this.lastframe + "\n");
        stringBuilder.Append("target = " + this.target + "\n");
        for (Int32 i = 0; i < this.numframes; i++)
        {
            stringBuilder.Append($"coords[{i}] = {this.coords[i]}\n");
            stringBuilder.Append($"rectuvs[{i}] = {this.rectuvs[i]}\n");
        }
        stringBuilder.Append("-----------");
        global::Debug.Log(stringBuilder.ToString());
    }

    public Byte count;
    public Byte flags;
    public Byte texID;

    public Byte numframes;
    public Int16 rate;
    public UInt16 randmin;
    public UInt16 randrange;
    public Int32 frame;
    public Int16 lastframe;

    public UInt32 geotexanimoffset;

    public Rect target;
    public Vector2[] coords;
    public Rect targetuv;
    public Rect[] rectuvs;

    public static Material texAnimMat = new Material(ShadersLoader.Find("PSX/BattleMap_TexAnim"));
}
