namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>
    /// Unless <see cref="IsReg0Flag8000Set"/>: <paramref name="objectIndex"/> == -1 sends host message 0x18 for each of the
    /// (signed byte +36) object pointers at +88, +96, ... of the record at off_180066C68[3] and returns that PSX
    /// address; otherwise sends 0x18 for object a1 and returns the callback result (the object pointer without one).
    /// </summary>
    public static long HostRequestBattleFiguresForObjects(int objectIndex)
    {
        Recompiler.Runtime.Tracer.Enter("HostRequestBattleFiguresForObjects(objectIndex)", objectIndex);
        long result = IsReg0Flag8000Set();
        if ((uint)result != 0)
        {
            return result;
        }

        if (objectIndex == -1)
        {
            long callback = (long)BattleCallbackAddress;
            long offset = 88;
            for (int i = 0; ; i++)
            {
                uint address = ((uint*)off_180066C68)[3];
                result = address;
                if (i >= (sbyte)ResolveGlobalPsxAddress(address)[36])
                {
                    return result;
                }

                long obj = *(long*)(ResolveGlobalPsxAddress(address) + offset);
                if (callback != 0)
                {
                    HostCall(callback, ObjCommand(obj, HostCommand.Btl2dReq), 0, 0, 0);
                    callback = (long)BattleCallbackAddress;
                }

                offset += 8;
            }
        }

        result = PsxGetContextQword(objectIndex);
        if (BattleCallbackAddress != null)
        {
            return HostCall((long)BattleCallbackAddress, ObjCommand(result, HostCommand.Btl2dReq), 0, 0, 0);
        }

        return result;
    }

    /// <summary>
    /// Sets Memoria's soundFPS (command 125); the default value 4 corresponds to sound pitch 1.0.
    /// Evidence: ref:R001/Assembly-CSharp/Global/SFX/SFX.cs#L962-L964.
    /// </summary>
    public static uint HostSetSoundFps(uint soundFps)
    {
        Recompiler.Runtime.Tracer.Enter("HostSetSoundFps(soundFps)", soundFps);
        long callback = (long)BattleCallbackAddress;
        if (callback == 0)
        {
            return 0;
        }

        return (uint)HostCall(callback, HostCommand.SetFps, soundFps, 0, 0);
    }
}
