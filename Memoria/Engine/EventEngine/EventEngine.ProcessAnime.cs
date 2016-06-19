using FF9;

public partial class EventEngine
{
    private void ProcessAnime(Actor actor)
    {
        if (((int)actor.animFlag & EventEngine.afFreeze) == 0)
        {
            if (this.gMode == 3)
            {
                if ((int)actor.sid == 5 && WMUIData.ControlNo == 6 && (int)WMUIData.StatusNo == 7)
                    actor.frameN = (byte)20;
                if ((int)actor.uid == 12 && WMUIData.ControlNo == 6 && ((int)WMUIData.StatusNo == 7 && (int)actor.frameN == 14))
                    actor.frameN = (byte)20;
            }
            if (((int)actor.animFlag & EventEngine.afExec) != 0)
            {
                if (actor.parent != null)
                {
                    Actor actor1 = actor.parent;
                    bool flag = (int)FF9StateSystem.Common.FF9.fldMapNo == 2950 || (int)FF9StateSystem.Common.FF9.fldMapNo == 2951 || ((int)FF9StateSystem.Common.FF9.fldMapNo == 2952 || (int)FF9StateSystem.Common.FF9.fldMapNo == 2953) || (int)FF9StateSystem.Common.FF9.fldMapNo == 2954 || (int)FF9StateSystem.Common.FF9.fldMapNo == 2955;
                    if ((int)actor1.anim == (int)actor1.idle)
                    {
                        if ((int)actor.anim != (int)actor.idle && flag)
                            this.SetAnim(actor, (int)actor.idle);
                        actor.anim = actor.idle;
                    }
                    else if ((int)actor1.anim == (int)actor1.walk)
                    {
                        if ((int)actor.anim != (int)actor.walk && flag)
                            this.SetAnim(actor, (int)actor.walk);
                        actor.anim = actor.walk;
                    }
                    else if ((int)actor1.anim == (int)actor1.run)
                    {
                        if ((int)actor.anim != (int)actor.run && flag)
                            this.SetAnim(actor, (int)actor.run);
                        actor.anim = actor.run;
                    }
                    if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2950 && (int)actor.sid == 25 || (int)FF9StateSystem.Common.FF9.fldMapNo == 2951 && (int)actor.sid == 29 || ((int)FF9StateSystem.Common.FF9.fldMapNo == 2952 && (int)actor.sid == 23 || (int)FF9StateSystem.Common.FF9.fldMapNo == 2954 && (int)actor.sid == 26) || (int)FF9StateSystem.Common.FF9.fldMapNo == 2955 && (int)actor.sid == 28)
                    {
                        ++actor.animFrame;
                        actor.animFrame %= (byte)(uint)actor.frameN;
                    }
                    else
                        actor.animFrame = actor1.animFrame;
                }
                else if (((int)actor.animFlag & EventEngine.afDir) == 0)
                {
                    int num1 = (int)actor.outFrame;
                    int num2 = (int)actor.frameN - 1;
                    if (num1 > num2)
                        num1 = num2;
                    if (this.NextFrame(actor) > num1)
                        this.AnimLoop(actor);
                    if (((int)actor.flags & 128) != 0)
                        this.DoTurn(actor);
                }
                else if (this.NextFrame(actor) < (int)actor.outFrame)
                    this.AnimLoop(actor);
            }
            else
            {
                int num = this.NextFrame(actor);
                if ((int)actor.sid != 17)
                ;
                if (num >= (int)actor.frameN)
                {
                    actor.animFrame = (byte)0;
                    if ((int)actor.anim == (int)actor.idle)
                        this.idleAnimSpeed(actor);
                }
            }
        }
        if (this.gMode != 3 || (int)actor.sid != 5)
        ;
    }

    private int NextFrame(Actor actor)
    {
        bool flag = ((int)actor.animFlag & EventEngine.afDir) != 0;
        int num1 = ((int)actor.animFrame << 4) + (int)actor.frameDif + (!flag ? (int)actor.aspeed : -1 * (int)actor.aspeed);
        if (flag)
            num1 += 15;
        actor.frameDif = (sbyte)(num1 & 15);
        if (flag)
            actor.frameDif -= (sbyte)15;
        actor.lastAnimFrame = actor.animFrame;
        int num2 = num1 >> 4;
        actor.animFrame = (byte)num2;
        return num2;
    }

    private void AnimLoop(Actor actor)
    {
        if (((int)actor.animFlag & (EventEngine.afLoop | EventEngine.afPalindrome)) != 0)
        {
            if ((int)actor.loopCount != 0)
            {
                --actor.loopCount;
                if ((int)actor.loopCount != 0)
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
        if (((int)actor.animFlag & EventEngine.afPalindrome) != 0)
        {
            actor.animFrame += (byte)(((int)actor.animFlag & EventEngine.afDir) == 0 ? 254 : 2);
            actor.animFlag ^= (byte)EventEngine.afDir;
            int num = (int)actor.inFrame;
            actor.inFrame = actor.outFrame;
            actor.outFrame = (byte)num;
        }
        else
        {
            if (((int)actor.animFlag & EventEngine.afLoop) == 0)
                return;
            actor.animFrame = actor.inFrame;
        }
    }

    private void AnimStop(Actor actor)
    {
        if (((int)actor.animFlag & EventEngine.afHold) == 0)
        {
            this.DefaultAnim(actor);
            if (((int)actor.flags & 128) != 0)
                this.FinishTurn(actor);
            if (((int)actor.actf & EventEngine.actJump) == 0)
                return;
            this.FinishJump(actor);
        }
        else
        {
            actor.animFlag |= (byte)EventEngine.afFreeze;
            actor.animFlag &= (byte)~(EventEngine.afExec | EventEngine.afLower);
            actor.animFrame = actor.lastAnimFrame;
        }
    }

    private void SetAnim(Actor p, int anim)
    {
        if (anim == (int)p.anim)
            return;
        p.anim = (ushort)anim;
        p.animFrame = (byte)0;
        p.frameDif = (sbyte)0;
        p.frameN = (byte)EventEngineUtils.GetCharAnimFrame(p.go, anim);
        p.aspeed = p.aspeed0;
    }

    private void DefaultAnim(Actor p)
    {
        this.SetAnim(p, (int)p.idle);
        p.animFrame = (byte)0;
        this.idleAnimSpeed(p);
        p.animFlag &= (byte)~(EventEngine.afExec | EventEngine.afLower | EventEngine.afFreeze | EventEngine.afDir);
    }

    private void idleAnimSpeed(Actor actor)
    {
        actor.aspeed = actor.idleSpeed[Comn.random8() & 3];
    }

    private void FinishJump(Actor actor)
    {
        actor.actf &= (ushort)~EventEngine.actJump;
        ff9shadow.FF9ShadowOnField((int)actor.uid);
        actor.inFrame = (byte)0;
        actor.outFrame = byte.MaxValue;
    }
}