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
        // TODO: Maybe change the organisation of UID to allow using more entries
        // Currently, it seems to be the following
        // 0-63     -> entries initialised with default UID (same as SID)
        // 64-127   -> entries executed with "RunSharedScript"
        // 128-249  -> entries initialised with custom UID (for battle scripts or for duplicates of the same object/NPC)
        // 250      -> ref to the player character
        // 251-254  -> ref to party members
        // 255      -> ref to the "this" entry
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (uid == 0)
            uid = sid;
        Obj obj = PersistenSingleton<EventEngine>.Instance.FindObjByUID(uid);
        if (obj != null)
            PersistenSingleton<EventEngine>.Instance.DisposeObj(obj);
        size = size + 3 >> 2;
        Int32 localVarSize = instance.sObjTable[sid].varn + 3 >> 2;
        this.AllocObj(size + localVarSize + stackn);
        this.Clear();
        ObjList newObj = instance.GetFreeObjList();
        instance.SetFreeObjList(newObj.next);
        ObjList activeObjTailList = instance.GetActiveObjTailList();
        ObjList activeObjList = instance.GetActiveObjList();
        if (activeObjTailList != null)
        {
            activeObjTailList.next = newObj;
            instance.SetActiveObjTailList(newObj);
        }
        else
        {
            instance.SetActiveObjTailList(newObj);
            instance.SetActiveObjList(newObj);
        }
        newObj.next = null;
        newObj.obj = this;
        this.sid = (Byte)sid;
        this.uid = (Byte)uid;
        this.cid = 0;
        this.ebData = instance.allObjsEBData[sid];
        this.ip = instance.GetIP(sid, 0, this.ebData);
        this.vofs = (Byte)size;
        this.sofs = (Byte)(size + localVarSize);
        this.sn = (Byte)stackn;
        this.state = EventEngine.stateNew;
        this.winnum = Byte.MaxValue;
        this.currentByte = this.ebData;
    }

    static Obj()
    {
        Obj.movQData =
        [
            0,
            37,  // InitWalk
            160, // WalkToExit
            4,   // return
            0
        ];
        Obj.neckTurnData =
        [
            0,
            167, // Turn
			0,
            0,   // <- write angle there for autoturn
			80,  // WaitTurn
			4    // return
		];
    }

    public Int32 ip
    {
        get => getIntFromBuffer(0);
        set => setIntToBuffer(0, value);
    }

    public Byte level
    {
        get => getByteFromBuffer(4);
        set => setByteToBuffer(4, value);
    }

    public Byte cid
    {
        get => getByteFromBuffer(5);
        set => setByteToBuffer(5, value);
    }

    public Byte sid
    {
        get => getByteFromBuffer(6);
        set => setByteToBuffer(6, value);
    }

    public Byte uid
    {
        get => getByteFromBuffer(7);
        set => setByteToBuffer(7, value);
    }

    public Byte vofs
    {
        get => getByteFromBuffer(8);
        set => setByteToBuffer(8, value);
    }

    public Byte sofs
    {
        get => getByteFromBuffer(9);
        set => setByteToBuffer(9, value);
    }

    public Byte sx
    {
        get => getByteFromBuffer(10);
        set => setByteToBuffer(10, value);
    }

    public Byte btlchk
    {
        get => getByteFromBuffer(11);
        set => setByteToBuffer(11, value);
    }

    public Byte sn
    {
        get => getByteFromBuffer(12);
        set => setByteToBuffer(12, value);
    }

    public Byte wait
    {
        get => getByteFromBuffer(13);
        set => setByteToBuffer(13, value);
    }

    public Byte state
    {
        get => getByteFromBuffer(14);
        set => setByteToBuffer(14, value);
    }

    public Byte flags
    {
        get => getByteFromBuffer(15);
        set => setByteToBuffer(15, value);
    }

    public Byte winnum
    {
        get => getByteFromBuffer(16);
        set => setByteToBuffer(16, value);
    }

    public Byte index
    {
        get => getByteFromBuffer(17);
        set => setByteToBuffer(17, value);
    }

    public Byte state0
    {
        get => getByteFromBuffer(18);
        set => setByteToBuffer(18, value);
    }

    public Byte pad2
    {
        get => getByteFromBuffer(19);
        set => setByteToBuffer(19, value);
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
        this.buffer = new Byte[size << 2];
    }

    public void copy(Obj o)
    {
        this.buffer = null;
        this.buffer = new Byte[o.buffer.Length];
        for (Int32 i = 0; i < o.buffer.Length; i++)
            this.buffer[i] = o.buffer[i];
        if (o.ebData != null)
        {
            this.ebData = null;
            this.ebData = new Byte[o.ebData.Length];
            for (Int32 j = 0; j < o.ebData.Length; j++)
                this.ebData[j] = o.ebData[j];
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
            this.currentByte = Obj.movQData;
        else if (o.currentByte == Obj.neckTurnData)
            this.currentByte = Obj.neckTurnData;
        else
            this.currentByte = this.ebData;
        this.tempFlag = o.tempFlag;
        this.go = null;
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
        this.uid = 0;
        this.sid = 0;
        this.cid = 0;
        this.level = 0;
        this.btlchk = 0;
        this.sx = 0;
        this.sofs = 0;
        this.vofs = 0;
        this.flags = 0;
        this.state = 0;
        this.wait = 0;
        this.sn = 0;
        this.pad2 = 0;
        this.state0 = 0;
        this.index = 0;
        this.winnum = 0;
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
        Int32 ip = this.currentByte[this.ip] & Byte.MaxValue;
        ip |= (this.currentByte[this.ip + 1] & Byte.MaxValue) << 8;
        ip |= (this.currentByte[this.ip + 2] & Byte.MaxValue) << 16;
        return ip | (this.currentByte[this.ip + 3] & Byte.MaxValue) << 24;
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
