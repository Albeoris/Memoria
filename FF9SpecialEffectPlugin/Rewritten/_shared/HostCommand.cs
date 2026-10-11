namespace Recompiled.FF9SpecialEffectPlugin;

// Memoria SFX.COMMAND, ref:R001/Assembly-CSharp/Global/SFX/SFX.cs#L2330-L2385.
internal static class HostCommand
{
    internal const uint GetPosition = 0x01000000u;
    internal const uint SetPosition = 0x02000000u;
    internal const uint GetRotate = 0x03000000u;
    internal const uint SetRotate = 0x04000000u;
    internal const uint GetScale = 0x05000000u;
    internal const uint SetScale = 0x06000000u;
    internal const uint GetGeoFlag = 0x07000000u;
    internal const uint GetAutoPotionCommand = 0x08000000u;
    internal const uint GetMotionFrameMax = 0x09000000u;
    internal const uint GetMotionFrame = 0x0A000000u;
    internal const uint SetMotionFrame = 0x0B000000u;
    internal const uint SetMotion = 0x0C000000u;
    internal const uint StopMotion = 0x0D000000u;
    internal const uint GetMatrix = 0x0E000000u;
    internal const uint IsTarget = 0x0F000000u;
    internal const uint SetDefaultIdle = 0x10000000u;
    internal const uint GetDisappear = 0x11000000u;
    internal const uint SetDisappear = 0x12000000u;
    internal const uint ShowWeapon = 0x13000000u;
    internal const uint CheckStatus = 0x14000000u;
    internal const uint GetCmdIdle = 0x15000000u;
    internal const uint GetSlave = 0x16000000u;
    internal const uint ExecVfx = 0x17000000u;
    internal const uint Btl2dReq = 0x18000000u;
    internal const uint ShowMesh = 0x19000000u;
    internal const uint GetMeshCount = 0x1A000000u;
    internal const uint SetPresentColor = 0x1B000000u;
    internal const uint EyeBlink = 0x1C000000u;
    internal const uint SfxBattle = 0x1D000000u;
    internal const uint SfxPlayerAttack = 0x1E000000u;
    internal const uint SfxPlayHit = 0x1F000000u;
    internal const uint SfxPlay = 0x20000000u;
    internal const uint SetSpmotion = 0x21000000u;
    internal const uint LoadImage = 0x64000000u;
    internal const uint StoreImage = 0x65000000u;
    internal const uint MoveImage = 0x66000000u;
    internal const uint IncrementBattleSeq = 0x6E000000u;
    internal const uint GetCameraConfig = 0x6F000000u;
    internal const uint LbossFlagEnable = 0x70000000u;
    internal const uint EffectTitle = 0x71000000u;
    internal const uint SetCursor = 0x72000000u;
    internal const uint GetCursor = 0x73000000u;
    internal const uint SfxPause = 0x74000000u;
    internal const uint GetBgIntensity = 0x75000000u;
    internal const uint SetBgIntensity = 0x76000000u;
    internal const uint Vibrate = 0x77000000u;
    internal const uint CreateTexture = 0x78000000u;
    internal const uint GetBonus = 0x79000000u;
    internal const uint GetStartType = 0x7A000000u;
    internal const uint GetAliveCount = 0x7B000000u;
    internal const uint GetCameraNumber = 0x7C000000u;
    internal const uint SetFps = 0x7D000000u;
}
