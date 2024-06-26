using System;
using UnityEngine;

public class Actor : PosObj
{
    public Actor()
    {
        this.idleSpeed = new Byte[4];
        base.cid = 4;
    }

    public Actor(Int32 sid, Int32 uid, Int32 size) : base(sid, uid, size, 16)
    {
        this.idleSpeed = new Byte[4];
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        base.cid = 4;
        base.flags = (Byte)(base.flags | 4);
        this.speed = 30;
        this.speedth = 31;
        this.aspeed = (this.aspeed0 = 16);
        this.outFrame = Byte.MaxValue;
        this.omega = 16;
        if (FF9StateSystem.Common.FF9.fldMapNo == 2954 && sid == 4) // Chocobo’s Paradise, Gold Chocobo
        {
            this.omega = 96;
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 1656 && sid == 9) // Iifa Tree/Eidolon Mound, Vivi
        {
            this.omega = 48;
        }
        this.turninst0 = 4;
        this.turninst2 = 80;
        this.turninst3 = 4;
        this.tspeed = 16;
        this.listener = Byte.MaxValue;
        for (Int32 i = 0; i < (Int32)this.idleSpeed.Length; i++)
        {
            this.idleSpeed[i] = 16;
        }
        if (instance.gMode != 1)
        {
            if (instance.gMode == 2)
            {
                instance.addObjPtrList(this);
            }
        }
    }

    public void copy(Actor act)
    {
        this.turnl = act.turnl;
        this.turnr = act.turnr;
        this.actf = act.actf;
        this.turnRot = act.turnRot;
        this.idle = act.idle;
        this.walk = act.walk;
        this.run = act.run;
        this.lastAnim = act.lastAnim;
        this.speed = act.speed;
        this.aspeed = act.aspeed;
        this.speedth = act.speedth;
        this.omega = act.omega;
        this.inFrame = act.inFrame;
        this.outFrame = act.outFrame;
        this.animFlag = act.animFlag;
        this.loopCount = act.loopCount;
        this.frameDif = act.frameDif;
        this.pad3 = act.pad3;
        this.pad6 = act.pad6;
        this.pad7 = act.pad7;
        this.jframe = act.jframe;
        this.aspeed0 = act.aspeed0;
        this.neckMyID = act.neckMyID;
        this.neckTargetID = act.neckTargetID;
        this.turnAdd = act.turnAdd;
        this.turninst0 = act.turninst0;
        this.turninst1 = act.turninst1;
        this.turninst2 = act.turninst2;
        this.turninst3 = act.turninst3;
        this.tspeed = act.tspeed;
        this.sleep = act.sleep;
        this.jump = act.jump;
        this.lastdist = act.lastdist;
        for (Int32 i = 0; i < 4; i++)
        {
            this.idleSpeed[i] = act.idleSpeed[i];
        }
        this.trot = act.trot;
        this.trotAdd = act.trotAdd;
        this.xl = act.xl;
        this.yl = act.yl;
        this.zl = act.zl;
        this.mesofsX = act.mesofsX;
        this.mesofsY = act.mesofsY;
        this.mesofsZ = act.mesofsZ;
        this.jump0 = act.jump0;
        this.jump1 = act.jump1;
        this.jframeN = act.jframeN;
        this.listener = act.listener;
        this.jumpx = act.jumpx;
        this.jumpz = act.jumpz;
        this.colldist = act.colldist;
        this.x0 = act.x0;
        this.y0 = act.y0;
        this.z0 = act.z0;
        this.jumpy = act.jumpy;
        this.neckBoneIndex = act.neckBoneIndex;
    }

    public UInt16 turnl;

    public UInt16 turnr;

    public UInt16 actf;

    public Single turnRot;

    public UInt16 idle;

    public UInt16 walk;

    public UInt16 run;

    public UInt16 lastAnim;

    public Byte speed;

    public Byte aspeed;

    public Byte speedth;

    public Byte omega;

    public Byte inFrame;

    public Byte outFrame;

    public Byte animFlag;

    public Byte loopCount;

    public SByte frameDif;

    public SByte pad3;

    public SByte pad6;

    public SByte pad7;

    public Byte jframe;

    public Byte aspeed0;

    public Byte neckMyID;

    public Byte neckTargetID;

    public Single turnAdd;

    public Int16 turninst0;

    public Byte turninst1;

    public Byte turninst2;

    public Byte turninst3;

    public Byte tspeed;

    public UInt16 sleep;

    public UInt16 jump;

    public Single lastdist;

    public Byte[] idleSpeed;

    public Single trot;

    public Single trotAdd;

    public Single xl;

    public Single yl;

    public Single zl;

    public Int16 mesofsX;

    public Int16 mesofsY;

    public Int16 mesofsZ;

    public Byte jump0;

    public Byte jump1;

    public Byte jframeN;

    public Byte listener;

    public Int16 jumpx;

    public Int16 jumpz;

    public PosObj coll;

    public Single colldist;

    public Actor parent;

    public Int16 x0;

    public Int16 y0;

    public Int16 z0;

    public Int16 jumpy;

    public Vector3 tempRot;

    public Int32 neckBoneIndex;

    public FieldMapActor fieldMapActor;

    public FieldMapActorController fieldMapActorController;

    public WMActor wmActor;

    public Vector3 offsetTurn;
}
