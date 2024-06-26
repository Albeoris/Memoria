using Memoria;
using System;
using System.Collections.Generic;
using System.IO;

public class BGI_DEF
{
    public BGI_DEF()
    {
        this.name = String.Empty;
        this.orgPos = new BGI_VEC_DEF();
        this.curPos = new BGI_VEC_DEF();
        this.minPos = new BGI_VEC_DEF();
        this.maxPos = new BGI_VEC_DEF();
        this.charPos = new BGI_VEC_DEF();
        this.triList = new List<BGI_TRI_DEF>();
        this.edgeList = new List<BGI_EDGE_DEF>();
        this.anmList = new List<BGI_ANM_DEF>();
        this.floorList = new List<BGI_FLOOR_DEF>();
        this.normalList = new List<BGI_FVEC_DEF>();
        this.vertexList = new List<BGI_VEC_DEF>();
    }

    public void ReadData(BinaryReader reader)
    {
        reader.BaseStream.Seek(4, SeekOrigin.Begin);
        this.dataSize = reader.ReadUInt16();
        this.orgPos.ReadData(reader);
        this.curPos.ReadData(reader);
        this.minPos.ReadData(reader);
        this.maxPos.ReadData(reader);
        this.charPos.ReadData(reader);
        this.activeFloor = reader.ReadInt16();
        this.activeTri = reader.ReadInt16();
        this.triCount = reader.ReadUInt16();
        this.triOffset = reader.ReadUInt16();
        this.edgeCount = reader.ReadUInt16();
        this.edgeOffset = reader.ReadUInt16();
        this.anmCount = reader.ReadUInt16();
        this.anmOffset = reader.ReadUInt16();
        this.floorCount = reader.ReadUInt16();
        this.floorOffset = reader.ReadUInt16();
        this.normalCount = reader.ReadUInt16();
        this.normalOffset = reader.ReadUInt16();
        this.vertexCount = reader.ReadUInt16();
        this.vertexOffset = reader.ReadUInt16();
        reader.BaseStream.Seek(4 + this.triOffset, SeekOrigin.Begin);
        for (UInt16 i = 0; i < this.triCount; i++)
        {
            BGI_TRI_DEF triangle = new BGI_TRI_DEF();
            triangle.ReadData(reader);
            triangle.triIdx = i;
            this.triList.Add(triangle);
        }
        reader.BaseStream.Seek(4 + this.edgeOffset, SeekOrigin.Begin);
        for (UInt16 i = 0; i < this.edgeCount; i++)
        {
            BGI_EDGE_DEF edge = new BGI_EDGE_DEF();
            edge.ReadData(reader);
            this.edgeList.Add(edge);
        }
        reader.BaseStream.Seek(4 + this.anmOffset, SeekOrigin.Begin);
        for (UInt16 i = 0; i < this.anmCount; i++)
        {
            BGI_ANM_DEF anim = new BGI_ANM_DEF();
            anim.ReadData(reader);
            this.anmList.Add(anim);
        }
        for (Int32 i = 0; i < this.anmCount; i++)
        {
            BGI_ANM_DEF anim = this.anmList[i];
            anim.frameList = new List<BGI_FRAME_DEF>();
            reader.BaseStream.Seek(4 + anim.frameOffset, SeekOrigin.Begin);
            for (Int32 j = 0; j < anim.frameCount; j++)
            {
                BGI_FRAME_DEF frame = new BGI_FRAME_DEF();
                frame.ReadData(reader);
                anim.frameList.Add(frame);
            }
        }
        for (Int32 i = 0; i < this.anmCount; i++)
        {
            BGI_ANM_DEF anim = this.anmList[i];
            for (Int32 j = 0; j < anim.frameCount; j++)
            {
                BGI_FRAME_DEF frame = anim.frameList[j];
                reader.BaseStream.Seek(4 + frame.triNdxOffset, SeekOrigin.Begin);
                frame.triIdxList = new List<Int32>();
                for (Int32 k = 0; k < frame.triCount; k++)
                    frame.triIdxList.Add(reader.ReadInt32());
            }
        }
        reader.BaseStream.Seek(4 + this.floorOffset, SeekOrigin.Begin);
        for (UInt16 i = 0; i < this.floorCount; i++)
        {
            BGI_FLOOR_DEF floor = new BGI_FLOOR_DEF();
            floor.ReadData(reader);
            this.floorList.Add(floor);
        }
        reader.BaseStream.Seek(4 + this.normalOffset, SeekOrigin.Begin);
        for (UInt16 i = 0; i < this.normalCount; i++)
        {
            BGI_FVEC_DEF normal = new BGI_FVEC_DEF();
            normal.ReadData(reader);
            this.normalList.Add(normal);
        }
        reader.BaseStream.Seek(4 + this.vertexOffset, SeekOrigin.Begin);
        for (UInt16 i = 0; i < this.vertexCount; i++)
        {
            BGI_VEC_DEF vertex = new BGI_VEC_DEF();
            vertex.ReadData(reader);
            this.vertexList.Add(vertex);
        }
    }

    public void LoadBGI(FieldMap fieldMap, String path, String name)
    {
        this.name = name;
        if (FF9StateSystem.Common.FF9.fldMapNo == 70 && !Configuration.Debug.StartFieldCreator) // Opening-For FMV
            return;
        Byte[] binAsset = AssetManager.LoadBytes(path + name + ".bgi");
        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(binAsset)))
            this.ReadData(binaryReader);
    }

    public void WriteData(BinaryWriter writer)
    {
        this.UpdateOffsets();
        writer.Write(0xACDCDEADu);
        writer.Write(this.dataSize);
        this.orgPos.WriteData(writer);
        this.curPos.WriteData(writer);
        this.minPos.WriteData(writer);
        this.maxPos.WriteData(writer);
        this.charPos.WriteData(writer);
        writer.Write(this.activeFloor);
        writer.Write(this.activeTri);
        writer.Write(this.triCount);
        writer.Write(this.triOffset);
        writer.Write(this.edgeCount);
        writer.Write(this.edgeOffset);
        writer.Write(this.anmCount);
        writer.Write(this.anmOffset);
        writer.Write(this.floorCount);
        writer.Write(this.floorOffset);
        writer.Write(this.normalCount);
        writer.Write(this.normalOffset);
        writer.Write(this.vertexCount);
        writer.Write(this.vertexOffset);
        writer.BaseStream.Seek(4 + this.triOffset, SeekOrigin.Begin);
        for (UInt16 i = 0; i < this.triList.Count; i++)
            this.triList[i].WriteData(writer);
        writer.BaseStream.Seek(4 + this.edgeOffset, SeekOrigin.Begin);
        for (UInt16 i = 0; i < this.edgeList.Count; i++)
            this.edgeList[i].WriteData(writer);
        writer.BaseStream.Seek(4 + this.anmOffset, SeekOrigin.Begin);
        for (UInt16 i = 0; i < this.anmList.Count; i++)
            this.anmList[i].WriteData(writer);
        for (Int32 i = 0; i < this.anmList.Count; i++)
        {
            BGI_ANM_DEF anim = this.anmList[i];
            writer.BaseStream.Seek(4 + anim.frameOffset, SeekOrigin.Begin);
            for (Int32 j = 0; j < anim.frameList.Count; j++)
                anim.frameList[j].WriteData(writer);
        }
        for (Int32 i = 0; i < this.anmList.Count; i++)
        {
            BGI_ANM_DEF anim = this.anmList[i];
            for (Int32 j = 0; j < anim.frameList.Count; j++)
            {
                BGI_FRAME_DEF frame = anim.frameList[j];
                writer.BaseStream.Seek(4 + frame.triNdxOffset, SeekOrigin.Begin);
                for (Int32 k = 0; k < frame.triIdxList.Count; k++)
                    writer.Write(frame.triIdxList[k]);
            }
        }
        writer.BaseStream.Seek(4 + this.floorOffset, SeekOrigin.Begin);
        for (UInt16 i = 0; i < this.floorList.Count; i++)
            this.floorList[i].WriteData(writer);
        writer.BaseStream.Seek(4 + this.normalOffset, SeekOrigin.Begin);
        for (UInt16 i = 0; i < this.normalList.Count; i++)
            this.normalList[i].WriteData(writer);
        writer.BaseStream.Seek(4 + this.vertexOffset, SeekOrigin.Begin);
        for (UInt16 i = 0; i < this.vertexList.Count; i++)
            this.vertexList[i].WriteData(writer);
    }

    public void UpdateOffsets()
    {
        this.triCount = (UInt16)this.triList.Count;
        this.edgeCount = (UInt16)this.edgeList.Count;
        this.anmCount = (UInt16)this.anmList.Count;
        this.floorCount = (UInt16)this.floorList.Count;
        this.normalCount = (UInt16)this.normalList.Count;
        this.vertexCount = (UInt16)this.vertexList.Count;
        UInt16 offset = 0x3C;
        this.triOffset = offset;
        offset += (UInt16)(0x28 * this.triCount);
        this.edgeOffset = offset;
        offset += (UInt16)(4 * this.edgeCount);
        this.anmOffset = offset;
        offset += (UInt16)(0x10 * this.anmCount);
        this.floorOffset = offset;
        offset += (UInt16)(0x20 * this.floorCount);
        this.normalOffset = offset;
        offset += (UInt16)(0x10 * this.normalCount);
        this.vertexOffset = offset;
        offset += (UInt16)(6 * this.vertexCount);
        for (Int32 i = 0; i < this.anmCount; i++)
        {
            this.anmList[i].frameOffset = offset;
            this.anmList[i].frameCount = (UInt16)this.anmList[i].frameList.Count;
            offset += (UInt16)(8 * this.anmList[i].frameCount);
        }
        for (Int32 i = 0; i < this.floorCount; i++)
        {
            this.floorList[i].triNdxOffset = offset;
            this.floorList[i].triCount = (UInt16)this.floorList[i].triNdxList.Count;
            offset += (UInt16)(4 * this.floorList[i].triCount);
        }
        for (Int32 i = 0; i < this.anmCount; i++)
        {
            for (Int32 j = 0; j < this.anmList[i].frameCount; j++)
            {
                this.anmList[i].frameList[j].triNdxOffset = offset;
                this.anmList[i].frameList[j].triCount = (UInt16)this.anmList[i].frameList[j].triIdxList.Count;
                offset += (UInt16)(4 * this.anmList[i].frameList[j].triCount);
            }
        }
        this.dataSize = offset;
    }

    public const Int32 MAGIC_SIZE = 4;

    public UInt16 dataSize;

    public BGI_VEC_DEF orgPos;
    public BGI_VEC_DEF curPos;
    public BGI_VEC_DEF minPos;
    public BGI_VEC_DEF maxPos;
    public BGI_VEC_DEF charPos;

    public Int16 activeFloor;
    public Int16 activeTri;

    public UInt16 triCount;
    public UInt16 triOffset;
    public UInt16 edgeCount;
    public UInt16 edgeOffset;
    public UInt16 anmCount;
    public UInt16 anmOffset;
    public UInt16 floorCount;
    public UInt16 floorOffset;
    public UInt16 normalCount;
    public UInt16 normalOffset;
    public UInt16 vertexCount;
    public UInt16 vertexOffset;

    public String name;
    public Byte attributeMask;

    public List<BGI_TRI_DEF> triList;
    public List<BGI_EDGE_DEF> edgeList;
    public List<BGI_ANM_DEF> anmList;
    public List<BGI_FLOOR_DEF> floorList;
    public List<BGI_FVEC_DEF> normalList;
    public List<BGI_VEC_DEF> vertexList;
}
