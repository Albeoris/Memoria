namespace Recompiled.FF9SpecialEffectPlugin;

// Memoria's legacy SFX wrappers, ref:R001/Assembly-CSharp/Global/SFX/SFX.cs#L1989-L2091.
internal enum SfxIntCommand
{
    SetCameraCaster = 1,
    SetCameraTarget = 2,
    SetCamera = 3,
    SetEnemyCamera = 4,
    GetEffectOvRun = 5,
    GetEffCamTrigger = 6,
    SetEffCamTrigger = 7,
    GetTaskMonsteraStartOk = 8,
    SetTaskMonsteraStart = 9,
    GetCameraPhase = 10,
    SetCameraPhase = 11,
    GetEffectJTexUsed = 12
}

internal enum FreeCameraOperation
{
    Rotate = 0,
    TranslateXY = 1,
    TranslateZ = 2
}

// ref:R001/Assembly-CSharp/Global/SFX/SFX.cs#L2395-L2403.
[System.Flags]
internal enum SfxRequestFlags : ushort
{
    NoExecutorAction = 1,
    Item = 2,
    MagicSword = 4,
    MagicOrSummon = 8,
    ReflectNext = 16
}
