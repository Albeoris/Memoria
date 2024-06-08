using System;
using FF9;

public partial class EventEngine
{
    private void ProcessAnime(Actor actor)
    {
        if (((int)actor.animFlag & EventEngine.afFreeze) == 0)
        {
            if (this.gMode == 3)
            {
                if (actor.sid == 5 && WMUIData.ControlNo == 6 && WMUIData.StatusNo == 7)
                {
                    if (actor.anim == 8132)
                    {
                        actor.frameN = 30;
                    }
                    else if (actor.anim == 8134)
                    {
                        actor.frameN = 10;
                    }
                    else
                    {
                        actor.frameN = 20;
                    }
                }
                if (actor.uid == 12 && WMUIData.ControlNo == 6 && WMUIData.StatusNo == 7)
                {
                    if (actor.anim == 8151)
                    {
                        actor.frameN = 30;
                    }
                    else if (actor.anim == 8148)
                    {
                        actor.frameN = 11;
                    }
                    else if (actor.frameN == 14)
                    {
                        actor.frameN = 20;
                    }
                }
            }
            if (((int)actor.animFlag & EventEngine.afExec) != 0)
            {
                if (actor.parent != null)
                {
                    Actor parent = actor.parent;
                    bool flag = FF9StateSystem.Common.FF9.fldMapNo == 2950 || FF9StateSystem.Common.FF9.fldMapNo == 2951 || FF9StateSystem.Common.FF9.fldMapNo == 2952 || FF9StateSystem.Common.FF9.fldMapNo == 2953 || FF9StateSystem.Common.FF9.fldMapNo == 2954 || FF9StateSystem.Common.FF9.fldMapNo == 2955;
                    if (parent.anim == parent.idle)
                    {
                        if (actor.anim != actor.idle && flag)
                        {
                            this.SetAnim(actor, (int)actor.idle);
                        }
                        actor.anim = actor.idle;
                    }
                    else if (parent.anim == parent.walk)
                    {
                        if (actor.anim != actor.walk && flag)
                        {
                            this.SetAnim(actor, (int)actor.walk);
                        }
                        actor.anim = actor.walk;
                    }
                    else if (parent.anim == parent.run)
                    {
                        if (actor.anim != actor.run && flag)
                        {
                            this.SetAnim(actor, (int)actor.run);
                        }
                        actor.anim = actor.run;
                    }
                    if ((FF9StateSystem.Common.FF9.fldMapNo == 2950 && actor.sid == 25) || (FF9StateSystem.Common.FF9.fldMapNo == 2951 && actor.sid == 29) || (FF9StateSystem.Common.FF9.fldMapNo == 2952 && actor.sid == 23) || (FF9StateSystem.Common.FF9.fldMapNo == 2954 && actor.sid == 26) || (FF9StateSystem.Common.FF9.fldMapNo == 2955 && actor.sid == 28))
                    {
                        actor.animFrame = (byte)(actor.animFrame + 1);
                        actor.animFrame = (byte)(actor.animFrame % actor.frameN);
                    }
                    else
                    {
                        actor.animFrame = parent.animFrame;
                    }
                }
                else if (((int)actor.animFlag & EventEngine.afDir) == 0)
                {
                    int num = (int)actor.outFrame;
                    int num2 = (int)(actor.frameN - 1);
                    if (num > num2)
                    {
                        num = num2;
                    }
                    if (this.NextFrame(actor) > num)
                    {
                        this.AnimLoop(actor);
                    }
                    if ((actor.flags & 128) != 0)
                    {
                        this.DoTurn(actor);
                    }
                }
                else if (this.NextFrame(actor) < (int)actor.outFrame)
                {
                    this.AnimLoop(actor);
                }
            }
            else
            {
                if (NextFrame(actor) >= actor.frameN - 1)
                {
                    actor.animFrame = 0;
                    if (actor.anim == actor.idle)
                    {
                        this.idleAnimSpeed(actor);
                    }
                }
            }
        }
        if (this.gMode != 3 || actor.sid == 5)
        {
        }
    }

    private Int32 NextFrame(Actor actor)
    {
        Boolean flag = ((Int32)actor.animFlag & EventEngine.afDir) != 0;
        Int32 num1 = ((Int32)actor.animFrame << 4) + (Int32)actor.frameDif + (!flag ? (Int32)actor.aspeed : -1 * (Int32)actor.aspeed);
        if (flag)
            num1 += 15;
        actor.frameDif = (SByte)(num1 & 15);
        if (flag)
            actor.frameDif -= (SByte)15;
        actor.lastAnimFrame = actor.animFrame;
        Int32 num2 = num1 >> 4;
        actor.animFrame = (Byte)num2;
        return num2;
    }

    private void AnimLoop(Actor actor)
    {
        if (((Int32)actor.animFlag & (EventEngine.afLoop | EventEngine.afPalindrome)) != 0)
        {
            if ((Int32)actor.loopCount != 0)
            {
                --actor.loopCount;
                if ((Int32)actor.loopCount != 0)
                    this.AnimLoop0(actor);
                else
                    this.AnimStop(actor);
            }
            else
                this.AnimLoop0(actor);
        }
        else
            this.AnimStop(actor);
    }

    private void AnimLoop0(Actor actor)
    {
        if (((Int32)actor.animFlag & EventEngine.afPalindrome) != 0)
        {
            actor.animFrame += (Byte)(((Int32)actor.animFlag & EventEngine.afDir) == 0 ? 254 : 2);
            actor.animFlag ^= (Byte)EventEngine.afDir;
            Int32 num = (Int32)actor.inFrame;
            actor.inFrame = actor.outFrame;
            actor.outFrame = (Byte)num;
        }
        else
        {
            if (((Int32)actor.animFlag & EventEngine.afLoop) == 0)
                return;
            actor.animFrame = actor.inFrame;
        }
    }

    private void AnimStop(Actor actor)
    {
        if (((Int32)actor.animFlag & EventEngine.afHold) == 0)
        {
            this.DefaultAnim(actor);
            if (((Int32)actor.flags & 128) != 0)
                this.FinishTurn(actor);
            if (((Int32)actor.actf & EventEngine.actJump) == 0)
                return;
            this.FinishJump(actor);
        }
        else
        {
            actor.animFlag |= (Byte)EventEngine.afFreeze;
            actor.animFlag &= (Byte)~(EventEngine.afExec | EventEngine.afLower);
            actor.animFrame = actor.lastAnimFrame;
        }
    }

    private void SetAnim(Actor p, Int32 anim)
    {
        if (anim == (Int32)p.anim)
            return;
        p.anim = (UInt16)anim;
        p.animFrame = (Byte)0;
        p.frameDif = (SByte)0;
        p.frameN = (Byte)EventEngineUtils.GetCharAnimFrame(p.go, anim);
        p.aspeed = p.aspeed0;
    }

    private void DefaultAnim(Actor p)
    {
        this.SetAnim(p, (Int32)p.idle);
        p.animFrame = (Byte)0;
        this.idleAnimSpeed(p);
        p.animFlag &= (Byte)~(EventEngine.afExec | EventEngine.afLower | EventEngine.afFreeze | EventEngine.afDir);
    }

    private void idleAnimSpeed(Actor actor)
    {
        actor.aspeed = actor.idleSpeed[Comn.random8() & 3];
    }

    private void FinishJump(Actor actor)
    {
        actor.actf &= (UInt16)~EventEngine.actJump;
        ff9shadow.FF9ShadowOnField((Int32)actor.uid);
        actor.inFrame = (Byte)0;
        actor.outFrame = Byte.MaxValue;
    }
}
