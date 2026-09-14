using Memoria;
using System;
using UnityEngine;

public partial class WMActor : MonoBehaviour
{
    public Vector3 pos
    {
        get => base.transform.position;
        set
        {
            base.transform.position = value;
            this.originalActor.pos[0] = this.RealPosition.x * 256f;
            this.originalActor.pos[1] = this.RealPosition.y * 256f;
            this.originalActor.pos[2] = this.RealPosition.z * 256f;
        }
    }

    public Single pos0
    {
        get => base.transform.position.x;
        set
        {
            Vector3 position = new Vector3(value, this.pos1, this.pos2);
            base.transform.position = position;
            this.originalActor.pos[0] = this.RealPosition.x * 256f;
        }
    }

    public Single pos1
    {
        get => base.transform.position.y;
        set
        {
            Vector3 position = new Vector3(this.pos0, value, this.pos2);
            base.transform.position = position;
            this.originalActor.pos[1] = (Int32)(this.RealPosition.y * 256f);
        }
    }

    public Single pos2
    {
        get => base.transform.position.z;
        set
        {
            Vector3 position = new Vector3(this.pos0, this.pos1, value);
            base.transform.position = position;
            this.originalActor.pos[2] = (Int32)(this.RealPosition.z * 256f);
        }
    }

    public Vector3 rot
    {
        get => base.transform.rotation.eulerAngles;
        set => base.transform.rotation = Quaternion.Euler(value);
    }

    public Single rot0
    {
        get => base.transform.rotation.eulerAngles.x;
        set
        {
            Quaternion rotation = Quaternion.Euler(value, this.rot1, this.rot2);
            base.transform.rotation = rotation;
            this.originalActor.rot[0] = (Int16)EventEngineUtils.ConvertFloatAngleToFixedPoint(this.rot0);
        }
    }

    public Single rot1
    {
        get => base.transform.rotation.eulerAngles.y;
        set
        {
            Quaternion rotation = Quaternion.Euler(this.rot0, value, this.rot2);
            base.transform.rotation = rotation;
            this.originalActor.rot[1] = (Int16)EventEngineUtils.ConvertFloatAngleToFixedPoint(this.rot1);
        }
    }

    public Single rot2
    {
        get => base.transform.rotation.eulerAngles.z;
        set
        {
            Quaternion rotation = Quaternion.Euler(this.rot0, this.rot1, value);
            base.transform.rotation = rotation;
            this.originalActor.rot[2] = (Int16)EventEngineUtils.ConvertFloatAngleToFixedPoint(this.rot2);
        }
    }

    public Int32 ControlNo => ff9.w_moveCHRStatus[this.originalActor.index].control;

    public WMWorld World { get; private set; }

    public void SetScale(Int32 scaleX = 64, Int32 scaleY = 64, Int32 scaleZ = 64)
    {
        Vector3 scale;
        scale.x = scaleX / 64f;
        scale.y = scaleY / 64f;
        scale.z = scaleZ / 64f;
        scale *= 0.00390625f;
        this.Animation.transform.localScale = scale;
    }

    public void SetPosition(Single x, Single y, Single z)
    {
        Vector3 position;
        position.x = x * 0.00390625f;
        position.y = y * 0.00390625f;
        position.z = z * 0.00390625f;
        base.transform.position = position;
        if (this.World && this.World.Settings.WrapWorld)
            this.World.SetAbsolutePositionOf(base.transform, new Vector3(position.x, position.y, position.z), 0f);
        this.lastx = base.transform.position.x;
        this.lasty = base.transform.position.y;
        this.lastz = base.transform.position.z;
    }

    public void SetAnimationSpeed(Single factor)
    {
        if (this.Animation == null)
            return;
        foreach (AnimationState animationState in this.Animation)
        {
            this.AnimationSpeed = 1f * factor;
            animationState.speed = 1f * factor;
        }
    }

    public Vector3 RealPosition
    {
        get
        {
            if (this.World && this.World.Settings.WrapWorld)
                return this.World.GetAbsolutePositionOf(base.transform);
            return this.pos;
        }
    }

    public Vector3 RealLastPosition
    {
        get
        {
            if (this.World && this.World.Settings.WrapWorld)
                return this.World.GetAbsolutePositionOf(new Vector3(this.lastx, this.lasty, this.lastz));
            return new Vector3(this.lastx, this.lasty, this.lastz);
        }
    }

    public void UpdateAnimationViaScript()
    {
        GameObject go = this.originalActor.go;
        if (go == null)
            return;
        Animation anim = go.GetComponent<Animation>();
        String animName = FF9DBAll.AnimationDB.GetValue(this.originalActor.anim);
        if (!anim.IsPlaying(animName))
        {
            if (anim.GetClip(animName) == null)
                return;
            anim.Play(animName);
        }
        AnimationState animState = anim[animName];
        animState.speed = 0f;
        animState.time = (Single)this.originalActor.animFrame / this.originalActor.frameN * animState.length;
        anim.Sample();
    }

    public void LateUpdate()
    {
    }

    public void LateUpdateFunction()
    {
        Actor actor = this.originalActor;
        if (actor.objParent != null)
        {
            WMActor thisActor = actor.wmActor;
            WMActor parentActor = ((Actor)actor.objParent).wmActor;
            if (actor.index == Obj.OBJINDEX_ZIDANE && WMUIData.ControlNo == 6 && WMUIData.StatusNo == 7)
            {
                if (actor.anim == 4724) // ANH_SUB_W0_001_IDLE1_CHO
                {
                    global::Debug.Log("4724 ### actor.animFrame = " + actor.animFrame);
                    if (actor.animFrame == 0)
                        thisActor.pos = parentActor.pos + new Vector3(0f, 0.60619f, 0f);
                    else if (actor.animFrame == 1)
                        thisActor.pos = parentActor.pos + new Vector3(0f, 0.50669f, 0f);
                    else if (actor.animFrame == 2)
                        thisActor.pos = parentActor.pos + new Vector3(0f, 0.36019f, 0f);
                    else if (actor.animFrame == 3)
                        thisActor.pos = parentActor.pos + new Vector3(0f, 0.23369f, 0f);
                    else if (actor.animFrame == 4)
                        thisActor.pos = parentActor.pos + new Vector3(0f, 0.23619f, 0f);
                    else if (actor.animFrame == 5)
                        thisActor.pos = parentActor.pos + new Vector3(0f, 0.25469f, 0f);
                    else if (actor.animFrame == 6)
                        thisActor.pos = parentActor.pos + new Vector3(0f, 0.24219f, 0f);
                    else if (actor.animFrame == 7)
                        thisActor.pos = parentActor.pos + new Vector3(0f, 0.41669f, 0f);
                    else if (actor.animFrame == 8)
                        thisActor.pos = parentActor.pos + new Vector3(0f, 0.62719f, 0f);
                    else if (actor.animFrame == 9)
                        thisActor.pos = parentActor.pos + new Vector3(0f, 0.45369f, 0f);
                    else if (actor.animFrame == 10)
                        thisActor.pos = parentActor.pos + new Vector3(0f, 0.41125f, 0f);
                }
                else
                {
                    thisActor.pos = parentActor.pos + new Vector3(0f, -0.35f, 0f);
                }
                thisActor.rot = parentActor.rot;
                this.previousActorAnim = actor.anim;
                this.previousAnimFrame = actor.animFrame;
            }
            else
            {
                thisActor.pos = parentActor.pos;
                thisActor.rot = parentActor.rot;
            }
        }
    }

    public Vector3 moveDirection { get; set; }
    public Single moveSpeed { get; set; }
    public Boolean isMoving { get; set; }
    public Boolean isControllable { get; set; }
    public Single bodyRotateSpeedZ { get; set; }
    public Single bodyMaxRotationZ { get; set; }
    public Single moveZ { get; set; }
    public Single MoveY { get; set; }
    public Single verticalSpeed { get; set; }
    public Single LockCameraTimer { get; set; }
    public Boolean MovingBack { get; set; }
    public Single lastGroundedTime { get; set; }

    public static WMActor ControlledDebugDebugActor
    {
        get
        {
            if (WMActor.controlledDebugActor == null)
                WMActor.controlledDebugActor = WMActor.GetStartupControlledActor();
            return WMActor.controlledDebugActor;
        }
        private set => WMActor.controlledDebugActor = value;
    }

    public static WMActor GetControlledDebugActor(Int32 status, Int32 controlNo)
    {
        WMWorld instance = Singleton<WMWorld>.Instance;
        for (ObjList objList = instance.ActorList; objList != null; objList = objList.next)
        {
            if (objList.obj.cid == 4)
            {
                WMActor wmActor = ((Actor)objList.obj).wmActor;
                if (FF9StateSystem.World.IsBeeScene)
                {
                    if (wmActor.name == "Zidane" && status == 1 && controlNo == 0)
                    {
                        global::Debug.Log("Zidane is active!");
                        return wmActor;
                    }
                    if (wmActor.name == "Dagger" && status == 2 && controlNo == 0)
                    {
                        global::Debug.Log("Dagger is active!");
                        return wmActor;
                    }
                    if (wmActor.name == "Chocobo" && status == 3 && controlNo == 1)
                    {
                        global::Debug.Log("Chocobo is active!");
                        return wmActor;
                    }
                    if (wmActor.name == "Chocobo" && status == 4 && controlNo == 2)
                    {
                        global::Debug.Log("Chocobo (Assase) is active!");
                        return wmActor;
                    }
                    if (wmActor.name == "Chocobo" && status == 5 && controlNo == 3)
                    {
                        global::Debug.Log("Chocobo (Yama) is active!");
                        return wmActor;
                    }
                    if (wmActor.name == "Chocobo" && status == 6 && controlNo == 4)
                    {
                        global::Debug.Log("Chocobo (Umi) is active!");
                        return wmActor;
                    }
                    if (wmActor.name == "Chocobo" && status == 7 && controlNo == 5)
                    {
                        global::Debug.Log("Chocobo (Sora walk) is active!");
                        return wmActor;
                    }
                    if (wmActor.name == "Chocobo" && status == 7 && controlNo == 6)
                    {
                        global::Debug.Log("Chocobo (Sora fly) is active!");
                        return wmActor;
                    }
                    if (wmActor.name == "Bluenalusisu" && status == 8 && controlNo == 7)
                    {
                        global::Debug.Log("Chocobo (Sora fly) is active!");
                        return wmActor;
                    }
                    if (wmActor.name == "Hirudagarude" && status == 9 && controlNo == 8)
                    {
                        global::Debug.Log("Hirudagarude is active!");
                        return wmActor;
                    }
                    if (wmActor.name == "Invincible" && status == 10 && controlNo == 9)
                    {
                        global::Debug.Log("Invincible is active!");
                        return wmActor;
                    }
                }
                else if (status == wmActor.originalActor.index && controlNo == wmActor.ControlNo)
                {
                    global::Debug.Log(wmActor.name + " is active!");
                    return wmActor;
                }
            }
        }
        global::Debug.LogWarning("Not found the actor!");
        return null;
    }

    public virtual void Intialize()
    {
        this.World = Singleton<WMWorld>.Instance;
        this.isControllable = true;
        this.moveDirection = base.transform.TransformDirection(Vector3.forward);
    }

    public static void SetControlledDebugActor(Int32 statusIndex, Int32 controlNo)
    {
        WMActor.ControlledDebugDebugActor = WMActor.GetControlledDebugActor(statusIndex, controlNo);
        WMBeeChocobo beeChocobo = WMActor.ControlledDebugDebugActor.GetComponent<WMBeeChocobo>();
        if (beeChocobo != null)
            beeChocobo.SetType(statusIndex);
    }

    public Material[] Materials
    {
        get
        {
            if (this.materials == null)
                this.materials = this.GetMaterials();
            return this.materials;
        }
    }

    private Material[] GetMaterials()
    {
        SkinnedMeshRenderer[] renderers = base.GetComponentsInChildren<SkinnedMeshRenderer>();
        Material[] materials = new Material[renderers.Length];
        for (Int32 i = 0; i< renderers.Length; i++)
        {
            ModelFactory.SetMatFilter(renderers[i].material, Configuration.Graphics.WorldSmoothTexture);
            materials[i] = renderers[i].material;
        }
        return materials;
    }

    public void SetFogByHeight()
    {
        this.SetFog(1f);
        /*
        if (this.pos.y < 22.5f)
        {
            if (ff9.w_moveActorPtr != this)
            {
                this.SetFog(1f);
            }
        }
        else if (ff9.w_moveActorPtr != this)
        {
            Byte index = this.originalActor.index;
            if (index == 9 || index == 10)
            {
                this.SetFog(0f);
            }
        }
        if (ff9.w_moveActorPtr == this)
        {
            this.SetFog(0f);
        }
        */
    }

    private void SetFog(Single enabled)
    {
        foreach (Material material in this.Materials)
            material.SetFloat("_FogEnabled", enabled);
    }

    private void SetFogStrength(Single strength)
    {
        foreach (Material material in this.Materials)
            material.SetFloat("_FogStrength", strength);
    }

    public static void Initialize()
    {
        ObjList objList = Singleton<WMWorld>.Instance.ActorList;
        for (objList = Singleton<WMWorld>.Instance.ActorList; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            obj.cid = 4;
            obj.state = 1;
            obj.flags = 1;
            WMActor wmActor = ((Actor)obj).wmActor;
            WMAnimationBank instance = Singleton<WMAnimationBank>.Instance;
            String name = wmActor.name;
            switch (name)
            {
                case "Zidane":
                    obj.index = Obj.OBJINDEX_ZIDANE;
                    wmActor.SetPosition(309934f, 1628f, -196612f);
                    wmActor.SetScale(67, 67, 67);
                    wmActor.Animation.AddClip(instance.ZidaneIdleClip, instance.ZidaneIdleClip.name);
                    wmActor.Animation.AddClip(instance.ZidaneRunClip, instance.ZidaneRunClip.name);
                    break;
                case "Dagger":
                    obj.index = Obj.OBJINDEX_GARNET;
                    wmActor.SetPosition(271994f, 1802f, -245750f);
                    wmActor.SetScale(67, 67, 67);
                    wmActor.Animation.AddClip(instance.DaggerIdleClip, instance.DaggerIdleClip.name);
                    wmActor.Animation.AddClip(instance.DaggerRunClip, instance.DaggerRunClip.name);
                    break;
                case "Chocobo":
                    obj.index = Obj.OBJINDEX_CHOCOBO_YELLOW;
                    wmActor.SetPosition(269958f, 1072f, -246044f);
                    wmActor.SetScale(67, 67, 67);
                    wmActor.Animation.AddClip(instance.ChocoboFlyClip, instance.ChocoboFlyClip.name);
                    wmActor.Animation.AddClip(instance.ChocoboIdleClip, instance.ChocoboIdleClip.name);
                    wmActor.Animation.AddClip(instance.ChocoboRunClip, instance.ChocoboRunClip.name);
                    break;
                case "Bluenalusisu":
                    obj.index = Obj.OBJINDEX_NARCISS;
                    wmActor.SetPosition(280576f, 327f, -261120f);
                    wmActor.SetScale(106, 106, 106);
                    wmActor.Animation.AddClip(instance.BluenalusisuIdleClip, instance.BluenalusisuIdleClip.name);
                    wmActor.Animation.AddClip(instance.BluenalusisuTakeOffClip, instance.BluenalusisuTakeOffClip.name);
                    break;
                case "Hirudagarude":
                    obj.index = Obj.OBJINDEX_HILDA_GARDE_3;
                    wmActor.SetPosition((Single)ff9.UnityUnit(775.1f), (Single)ff9.UnityUnit(39.9f), (Single)ff9.UnityUnit(-317.3f));
                    wmActor.SetScale(110, 95, 110);
                    wmActor.Animation.AddClip(instance.HirudagarudeIdleClip, instance.HirudagarudeIdleClip.name);
                    wmActor.Animation.AddClip(instance.HirudagarudeTakeOffClip, instance.HirudagarudeTakeOffClip.name);
                    break;
                case "Invincible":
                    obj.index = Obj.OBJINDEX_INVINCIBLE;
                    wmActor.SetPosition(272866f, 8550f, -244552f);
                    wmActor.SetScale(64, 64, 64);
                    wmActor.Animation.AddClip(instance.InvincibleIdleClip, instance.InvincibleIdleClip.name);
                    wmActor.Animation.AddClip(instance.InvincibleTakeOffClip, instance.InvincibleTakeOffClip.name);
                    break;
            }
            wmActor.Intialize();
        }
    }

    private static WMActor GetStartupControlledActor()
    {
        if (FF9StateSystem.World.IsBeeScene)
            return WMActor.GetControlledDebugActor(1, 0);
        PosObj controlChar = PersistenSingleton<EventEngine>.Instance.GetControlChar();
        if (controlChar != null)
            return ((Actor)controlChar).wmActor;
        return null;
    }

    public String GetAnimationClipName(WMActorStateDebug state)
    {
        return this.GetAnimationClip(state).name;
    }

    public AnimationClip GetAnimationClip(WMActorStateDebug state)
    {
        WMAnimationBank animBank = Singleton<WMAnimationBank>.Instance;
        if (this.originalActor.index == Obj.OBJINDEX_ZIDANE)
        {
            if (state == WMActorStateDebug.Idle)
                return animBank.ZidaneIdleClip;
            if (state == WMActorStateDebug.Running)
                return animBank.ZidaneRunClip;
        }
        else if (this.originalActor.index == Obj.OBJINDEX_GARNET)
        {
            if (state == WMActorStateDebug.Idle)
                return animBank.DaggerIdleClip;
            if (state == WMActorStateDebug.Running)
                return animBank.DaggerRunClip;
        }
        else if (this.originalActor.index >= Obj.OBJINDEX_CHOCOBO_YELLOW && this.originalActor.index <= Obj.OBJINDEX_CHOCOBO_GOLD)
        {
            if (ff9.w_moveCHRControl_No == 6)
            {
                if (state == WMActorStateDebug.Idle)
                    return animBank.ChocoboFlyClip;
                if (state == WMActorStateDebug.Running)
                    return animBank.ChocoboFlyClip;
            }
            else
            {
                if (state == WMActorStateDebug.Idle)
                    return animBank.ChocoboIdleClip;
                if (state == WMActorStateDebug.Running)
                    return animBank.ChocoboRunClip;
            }
        }
        else if (this.originalActor.index == Obj.OBJINDEX_NARCISS)
        {
            if (state == WMActorStateDebug.Idle)
                return animBank.BluenalusisuIdleClip;
            if (state == WMActorStateDebug.Running)
                return animBank.BluenalusisuIdleClip;
        }
        else if (this.originalActor.index == Obj.OBJINDEX_HILDA_GARDE_3)
        {
            if (state == WMActorStateDebug.Idle)
                return animBank.HirudagarudeIdleClip;
            if (state == WMActorStateDebug.Running)
                return animBank.HirudagarudeIdleClip;
        }
        else if (this.originalActor.index == Obj.OBJINDEX_INVINCIBLE)
        {
            if (state == WMActorStateDebug.Idle)
                return animBank.InvincibleIdleClip;
            if (state == WMActorStateDebug.Running)
                return animBank.InvincibleIdleClip;
        }
        global::Debug.Log("No animation clip found!");
        return null;
    }

    public void VerifyValidCastPosition()
    {
        Int32 posX = (Int32)originalActor.pos[0];
        Int32 posY = (Int32)originalActor.pos[1];
        Int32 posZ = (Int32)originalActor.pos[2];
        if (ff9.w_movementChrVerifyValidCastPosition(ref posX, ref posY, ref posZ))
        {
            originalActor.pos[0] = posX;
            originalActor.pos[1] = posY;
            originalActor.pos[2] = posZ;
            SetPosition(posX, posY, posZ);
        }
    }

    public Actor originalActor;

    public Single lastx;
    public Single lasty;
    public Single lastz;

    public Animation Animation;

    private Int32 previousActorAnim;
    private Int32 previousAnimFrame;
    private Int32 actorAnimFrameCount;

    public Boolean IsOnGround;
    public WMActorStateDebug State;
    public Single walkSpeed = 2f;
    public Single trotSpeed = 4f;
    public Single runSpeed = 6f;
    public Single SpeedSmoothing = 100f;
    public Single rotateSpeed = 500f;
    public Single AnimationSpeed = 0.5f;
    public Transform RotationTransform;
    public Single bodyRotationZ;
    public Single VerticalSpeed = 40f;
    public Single MaxVerticalSpeed = 2f;
    public Single RotateSpeed = 500f;
    public Single RollSpeed = 50f;
    public Single RollMaxAngle = 45f;

    private static WMActor controlledDebugActor;

    private Material[] materials;
}
