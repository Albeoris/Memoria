using System;
using UnityEngine;
using Object = System.Object;

public class Obj
{
    public Obj()
    {
        this.buffer = new Byte[20];
    }

    public Obj(Int32 sid, Int32 uid, Int32 size, Int32 stackn)
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (sid < 0 || sid >= instance.sSourceObjN)
        {
        }
        if (uid == 0)
        {
            uid = sid;
        }
        Obj obj = PersistenSingleton<EventEngine>.Instance.FindObjByUID(uid);
        if (obj != null)
        {
            PersistenSingleton<EventEngine>.Instance.DisposeObj(obj);
        }
        size = size + 3 >> 2;
        Int32 num = instance.sObjTable[sid].varn + 3 >> 2;
        this.AllocObj(size + num + stackn);
        this.Clear();
        ObjList freeObjList = instance.GetFreeObjList();
        instance.SetFreeObjList(freeObjList.next);
        ObjList activeObjTailList = instance.GetActiveObjTailList();
        ObjList activeObjList = instance.GetActiveObjList();
        if (activeObjTailList != null)
        {
            activeObjTailList.next = freeObjList;
            instance.SetActiveObjTailList(freeObjList);
        }
        else
        {
            instance.SetActiveObjTailList(freeObjList);
            instance.SetActiveObjList(freeObjList);
        }
        freeObjList.next = (ObjList)null;
        freeObjList.obj = this;
        this.sid = (Byte)sid;
        this.uid = (Byte)uid;
        this.cid = 0;
        this.ebData = instance.allObjsEBData[sid];
        this.ip = instance.GetIP(sid, 0, this.ebData);
        this.vofs = (Byte)size;
        this.sofs = (Byte)(size + num);
        this.sn = (Byte)stackn;
        this.state = EventEngine.stateNew;
        this.winnum = Byte.MaxValue;
        this.currentByte = this.ebData;
    }

    static Obj()
    {
        // Note: this type is marked as 'beforefieldinit'.
        Byte[] array = new Byte[5];
        array[1] = 37;
        array[2] = 160;
        array[3] = 4;
        Obj.movQData = array;
        Obj.neckTurnData = new Byte[]
        {
            0,
            167, // Turn
			0,
            0, // <- write angle there for autoturn
			80, // WaitTurn
			4 // return
		};
    }

    public Int32 ip
    {
        get
        {
            return this.getIntFromBuffer(0);
        }
        set
        {
            this.setIntToBuffer(0, value);
        }
    }

    public Byte level
    {
        get
        {
            return this.getByteFromBuffer(4);
        }
        set
        {
            this.setByteToBuffer(4, value);
        }
    }

    public Byte cid
    {
        get
        {
            return this.getByteFromBuffer(5);
        }
        set
        {
            this.setByteToBuffer(5, value);
        }
    }

    public Byte sid
    {
        get
        {
            return this.getByteFromBuffer(6);
        }
        set
        {
            this.setByteToBuffer(6, value);
        }
    }

    public Byte uid
    {
        get
        {
            return this.getByteFromBuffer(7);
        }
        set
        {
            this.setByteToBuffer(7, value);
        }
    }

    public Byte vofs
    {
        get
        {
            return this.getByteFromBuffer(8);
        }
        set
        {
            this.setByteToBuffer(8, value);
        }
    }

    public Byte sofs
    {
        get
        {
            return this.getByteFromBuffer(9);
        }
        set
        {
            this.setByteToBuffer(9, value);
        }
    }

    public Byte sx
    {
        get
        {
            return this.getByteFromBuffer(10);
        }
        set
        {
            this.setByteToBuffer(10, value);
        }
    }

    public Byte btlchk
    {
        get
        {
            return this.getByteFromBuffer(11);
        }
        set
        {
            this.setByteToBuffer(11, value);
        }
    }

    public Byte sn
    {
        get
        {
            return this.getByteFromBuffer(12);
        }
        set
        {
            this.setByteToBuffer(12, value);
        }
    }

    public Byte wait
    {
        get
        {
            return this.getByteFromBuffer(13);
        }
        set
        {
            this.setByteToBuffer(13, value);
        }
    }

    public Byte state
    {
        get
        {
            return this.getByteFromBuffer(14);
        }
        set
        {
            this.setByteToBuffer(14, value);
        }
    }

    public Byte flags
    {
        get
        {
            return this.getByteFromBuffer(15);
        }
        set
        {
            this.setByteToBuffer(15, value);
        }
    }

    public Byte winnum
    {
        get
        {
            return this.getByteFromBuffer(16);
        }
        set
        {
            this.setByteToBuffer(16, value);
        }
    }

    public Byte index
    {
        get
        {
            return this.getByteFromBuffer(17);
        }
        set
        {
            this.setByteToBuffer(17, value);
        }
    }

    public Byte state0
    {
        get
        {
            return this.getByteFromBuffer(18);
        }
        set
        {
            this.setByteToBuffer(18, value);
        }
    }

    public Byte pad2
    {
        get
        {
            return this.getByteFromBuffer(19);
        }
        set
        {
            this.setByteToBuffer(19, value);
        }
    }

    public Int32 getIntFromBuffer(Int32 startID)
    {
        if (startID > (Int32)this.buffer.Length - 4)
        {
            EventEngineUtils.E_Error("getIntFromBuffer: there is no enought data on obj.buffer to return int");
            return -1;
        }
        Int32 num = (Int32)this.buffer[startID + 3] << 24;
        num |= (Int32)this.buffer[startID + 2] << 16;
        num |= (Int32)this.buffer[startID + 1] << 8;
        return num | (Int32)this.buffer[startID];
    }

    public Int16 getShortFromBuffer(Int32 startID)
    {
        if (startID > (Int32)this.buffer.Length - 2)
        {
            EventEngineUtils.E_Error("getShortFromBuffer: there is no enought data on obj.buffer to return short");
            return -1;
        }
        Int32 num = (Int32)this.buffer[startID + 1] << 8;
        num |= (Int32)this.buffer[startID];
        return (Int16)num;
    }

    public Byte getByteFromBuffer(Int32 startID)
    {
        if (startID > (Int32)this.buffer.Length - 1)
        {
            global::Debug.Log(String.Concat(new Object[]
            {
                "getByteFromBuffer: there is no enought data on obj.buffer to return byte : startID = ",
                startID,
                ", buffer.Length-1 = ",
                (Int32)this.buffer.Length - 1
            }));
            return Byte.MaxValue;
        }
        return this.buffer[startID];
    }

    public void setIntToBuffer(Int32 startID, Int32 value)
    {
        if (startID >= (Int32)this.buffer.Length - 4)
        {
            EventEngineUtils.E_Error("setIntToBuffer: there is no enought data on obj.buffer to set int");
            return;
        }
        this.buffer[startID + 3] = (Byte)(((Int64)value & (Int64)(-16777216)) >> 24);
        this.buffer[startID + 2] = (Byte)((value & 16711680) >> 16);
        this.buffer[startID + 1] = (Byte)((value & 65280) >> 8);
        this.buffer[startID] = (Byte)(value & 255);
    }

    public void setShortToBuffer(Int32 startID, Int16 value)
    {
        if (startID > (Int32)this.buffer.Length - 2)
        {
            EventEngineUtils.E_Error("setShortToBuffer: there is no enought data on obj.buffer to set short");
            return;
        }
        this.buffer[startID + 1] = (Byte)(((Int32)value & 65280) >> 8);
        this.buffer[startID] = (Byte)(value & 255);
    }

    public void setByteToBuffer(Int32 startID, Byte value)
    {
        if (startID > (Int32)this.buffer.Length - 1)
        {
            EventEngineUtils.E_Error("setByteToBuffer: there is no enought data on obj.buffer to set byte");
            return;
        }
        this.buffer[startID] = (Byte)(value & Byte.MaxValue);
    }

    ~Obj()
    {
        this.buffer = null;
    }

    private void AllocObj(Int32 size)
    {
        size *= 4;
        this.buffer = new Byte[size];
    }

    public void copy(Obj o)
    {
        this.buffer = null;
        this.buffer = new Byte[(Int32)o.buffer.Length];
        for (Int32 i = 0; i < (Int32)o.buffer.Length; i++)
        {
            this.buffer[i] = o.buffer[i];
        }
        if (o.ebData != null)
        {
            this.ebData = null;
            this.ebData = new Byte[(Int32)o.ebData.Length];
            for (Int32 j = 0; j < (Int32)o.ebData.Length; j++)
            {
                this.ebData[j] = o.ebData[j];
            }
        }
        this.ip = o.ip;
        this.level = o.level;
        this.cid = o.cid;
        this.sid = o.sid;
        this.uid = o.uid;
        this.vofs = o.vofs;
        this.sofs = o.sofs;
        this.sx = o.sx;
        this.btlchk = o.btlchk;
        this.sn = o.sn;
        this.wait = o.wait;
        this.state = o.state;
        this.flags = o.flags;
        this.winnum = o.winnum;
        this.index = o.index;
        this.state0 = o.state0;
        this.pad2 = o.pad2;
        this.isAdditionCommand = o.isAdditionCommand;
        if (o.currentByte == Obj.movQData)
        {
            this.currentByte = Obj.movQData;
        }
        else if (o.currentByte == Obj.neckTurnData)
        {
            this.currentByte = Obj.neckTurnData;
        }
        else
        {
            this.currentByte = this.ebData;
        }
        this.tempFlag = o.tempFlag;
        this.go = (GameObject)null;
        if (o.cid == 4)
        {
            PosObj po = (PosObj)o;
            PosObj posObj = (PosObj)this;
            posObj.copy(po);
        }
        if (o.cid == 3)
        {
            Quad quad = (Quad)o;
            Quad quad2 = (Quad)this;
            quad2.copy(quad);
        }
        if (o.cid == 1)
        {
            Seq seq = (Seq)o;
            Seq seq2 = (Seq)this;
            seq2.copy(seq);
        }
    }

    public void Clear()
    {
        this.ip = 0;
        Byte b = 0;
        this.uid = b;
        this.sid = b;
        this.cid = b;
        this.level = b;
        b = 0;
        this.btlchk = b;
        this.sx = b;
        this.sofs = b;
        this.vofs = b;
        b = 0;
        this.flags = b;
        this.state = b;
        this.wait = b;
        this.sn = b;
        b = 0;
        this.pad2 = b;
        this.state0 = b;
        this.index = b;
        this.winnum = b;
    }

    public Byte getByteIP()
    {
        if (this.ip == 0)
        {
            global::Debug.Log("ip == 0, just do nothing return 04");
            return 4;
        }
        Byte result;
        try
        {
            result = this.currentByte[this.ip];
        }
        catch
        {
            global::Debug.LogError(String.Concat(new Object[]
            {
                "Could not access address ",
                this.ip,
                ", ebData.length = ",
                (Int32)this.currentByte.Length,
                ", sid = ",
                this.sid,
                ", ip = ",
                this.ip
            }));
            global::Debug.Break();
            result = this.currentByte[0];
        }
        return result;
    }

    public SByte getSByteIP()
    {
        if (this.ip == 0)
        {
            global::Debug.Log("ip == 0, just do nothing return 04");
            return 4;
        }
        SByte result;
        try
        {
            result = (SByte)this.currentByte[this.ip];
        }
        catch
        {
            global::Debug.LogError(String.Concat(new Object[]
            {
                "Could not access address ",
                this.ip,
                ", ebData.length = ",
                (Int32)this.currentByte.Length,
                ", sid = ",
                this.sid,
                ", ip = ",
                this.ip
            }));
            global::Debug.Break();
            result = (SByte)this.currentByte[0];
        }
        return result;
    }

    public Byte getByteIP(Int32 offset)
    {
        return this.currentByte[this.ip + offset];
    }

    public SByte getSByteIP(Int32 offset)
    {
        return (SByte)this.currentByte[this.ip + offset];
    }

    public UInt16 getUShortIP()
    {
        UInt16 ip = (UInt16)(this.currentByte[this.ip] & Byte.MaxValue);
        return (UInt16)(ip | (UInt16)((this.currentByte[this.ip + 1] & Byte.MaxValue) << 8));
    }

    public Int16 getShortIP()
    {
        Int16 ip = (Int16)(this.currentByte[this.ip] & Byte.MaxValue);
        return (Int16)(ip | (Int16)((this.currentByte[this.ip + 1] & Byte.MaxValue) << 8));
    }

    public Int32 getIntIP()
    {
        Int32 ip = (Int32)(this.currentByte[this.ip] & Byte.MaxValue);
        ip |= (Int32)(this.currentByte[this.ip + 1] & Byte.MaxValue) << 8;
        ip |= (Int32)(this.currentByte[this.ip + 2] & Byte.MaxValue) << 16;
        return ip | (Int32)(this.currentByte[this.ip + 3] & Byte.MaxValue) << 24;
    }

    public Byte getByteFromCurrentByte(Int32 index)
    {
        return this.currentByte[index];
    }

    public void printDataOnIP(Int32 numOfBytes)
    {
        for (Int32 i = 0; i < numOfBytes; i++)
        {
            Int32 ip = this.ip + i;
        }
    }

    public void CallAdditionCommand(Byte[] additionCommand)
    {
        this.currentByte = additionCommand;
        this.isAdditionCommand = true;
        this.ip = 1;
    }

    public void ReturnCall()
    {
        this.isAdditionCommand = false;
        this.currentByte = this.ebData;
    }

    public const Int32 IP_POS = 0;

    public const Int32 LEVEL_POS = 4;

    public const Int32 CID_POS = 5;

    public const Int32 SID_POS = 6;

    public const Int32 UID_POS = 7;

    public const Int32 VOFS_POS = 8;

    public const Int32 SOFS_POS = 9;

    public const Int32 SX_POS = 10;

    public const Int32 BTLCHK_POS = 11;

    public const Int32 SN_POS = 12;

    public const Int32 WAIT_POS = 13;

    public const Int32 STATE_POS = 14;

    public const Int32 FLAGS_POS = 15;

    public const Int32 WINNUM_POS = 16;

    public const Int32 INDEX_POS = 17;

    public const Int32 STATE0_POS = 18;

    public const Int32 PAD2_POS = 19;

    public Byte[] buffer;

    public Byte[] ebData;

    public GameObject go;

    public Obj objParent;

    public Boolean isAdditionCommand;

    public Byte[] currentByte;

    public Int32 tempFlag = -1;

    public Boolean isEnableRenderer = true;

    public static readonly Byte[] movQData;

    public static Byte[] neckTurnData;
}
