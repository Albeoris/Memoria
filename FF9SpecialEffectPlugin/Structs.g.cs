#nullable enable
#pragma warning disable CS0169, CS0649, CS8981, IDE1006 // field names mirror the original layout
using System.Runtime.InteropServices;

namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>S244: size unknown, 24 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitScrollLayers(ushort*, int):address).</summary>
[StructLayout(LayoutKind.Explicit, Size = 24)]
internal unsafe struct Area
{
    [FieldOffset(0)] public fixed byte gap_0[3]; // unknown
    /// <summary>uint8_t; medium; reads 0, writes 2; Rewritten/u0090_EmitScrollLayers.cs:37 Rewritten/u0090_EmitScrollLayers.cs:49</summary>
    [FieldOffset(3)] public byte field_3;
    /// <summary>int32_t; medium; union member: overlaps union_4_8, union_7_1; reads 0, writes 3; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0090_EmitScrollLayers.cs:38 Rewritten/u0090_EmitScrollLayers.cs:50 Rewritten/u0090_EmitScrollLayers.cs:60</summary>
    [FieldOffset(4)] public int union_4_4;
    /// <summary>uint64_t; medium; union member: overlaps union_4_4, union_7_1, union_8_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0090_EmitScrollLayers.cs:100</summary>
    [FieldOffset(4)] public ulong union_4_8;
    /// <summary>uint8_t; medium; union member: overlaps union_4_4, union_4_8; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0090_EmitScrollLayers.cs:101</summary>
    [FieldOffset(7)] public byte union_7_1;
    /// <summary>int32_t; medium; union member: overlaps union_4_8; reads 0, writes 2; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0090_EmitScrollLayers.cs:39 Rewritten/u0090_EmitScrollLayers.cs:51</summary>
    [FieldOffset(8)] public int union_8_4;
    /// <summary>uint16_t; medium; union member: overlaps union_12_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0090_EmitScrollLayers.cs:102</summary>
    [FieldOffset(12)] public ushort union_12_2;
    /// <summary>uint32_t; medium; union member: overlaps union_12_2, union_14_2, union_15_1; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0090_EmitScrollLayers.cs:61</summary>
    [FieldOffset(12)] public uint union_12_4;
    /// <summary>uint16_t; medium; union member: overlaps union_12_4, union_15_1; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0090_EmitScrollLayers.cs:103</summary>
    [FieldOffset(14)] public ushort union_14_2;
    /// <summary>uint8_t; medium; union member: overlaps union_12_4, union_14_2; reads 0, writes 2; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0090_EmitScrollLayers.cs:42 Rewritten/u0090_EmitScrollLayers.cs:53</summary>
    [FieldOffset(15)] public byte union_15_1;
    /// <summary>int32_t; medium; reads 0, writes 2; Rewritten/u0090_EmitScrollLayers.cs:43 Rewritten/u0090_EmitScrollLayers.cs:54</summary>
    [FieldOffset(16)] public int field_16;
    /// <summary>int32_t; medium; reads 0, writes 2; Rewritten/u0090_EmitScrollLayers.cs:44 Rewritten/u0090_EmitScrollLayers.cs:55</summary>
    [FieldOffset(20)] public int field_20;
}

/// <summary>S286: 32 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct BTL_DATA_INIT
{
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public byte bi_player;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(1)] public byte bi_slot_no;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(2)] public byte bi_slave;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(3)] public byte bi_line_no;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte tar_bone;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte player_equip;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte player_serial_no;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte player_wep_bone;
    /// <summary>int32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public int enemy_radius;
    /// <summary>int32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public int geo_radius;
    /// <summary>int32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public int geo_height;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public ushort btl_id;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(22)] public short pad;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(24)] public byte enemy_cam_bone0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(25)] public byte enemy_cam_bone1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(26)] public byte enemy_cam_bone2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(27)] public byte weapon_category;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(28)] public short weapon_offset0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(30)] public short weapon_offset1;
}

/// <summary>S288: 4 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct BTL_DATA_REQ
{
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public ushort btl_id;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(2)] public short pad;
}

/// <summary>S026: size unknown, 10 bytes = extent (last offset + width); inferred shape H4: 7 access(es) in 6 function(s); size unknown (extent 10); used by SfxBeginBlock (return).</summary>
[StructLayout(LayoutKind.Explicit, Size = 10)]
internal unsafe struct Block
{
    /// <summary>int16_t[]; medium; reads 3, writes 0; indexed with stride 2; 1 elements to the next field; fn:SFX_StartPlungeCamera_0:24 fn:SFX_InitBattle_0:64 fn:SFX_SendIntData_0:44</summary>
    [FieldOffset(0)] public ShortArray1 field_0;
    /// <summary>int16_t; medium; reads 2, writes 0; fn:SfxBeginBlock:11 fn:CamScriptTick:271</summary>
    [FieldOffset(2)] public short field_2;
    [FieldOffset(4)] public fixed byte gap_4[2]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; fn:SfxTriggerReaction:52</summary>
    [FieldOffset(6)] public short field_6;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:SfxTriggerReaction:58</summary>
    [FieldOffset(8)] public short field_8;
}

/// <summary>S221: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (global:dword_18032320C).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Block2
{
    /// <summary>uint8_t; medium; union member: overlaps union_0_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0070_SfxLoaderStep.cs:103</summary>
    [FieldOffset(0)] public byte union_0_1;
    /// <summary>int16_t; medium; union member: overlaps union_0_1, union_1_1; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0070_SfxLoaderStep.cs:53</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>uint8_t; medium; union member: overlaps union_0_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0070_SfxLoaderStep.cs:105</summary>
    [FieldOffset(1)] public byte union_1_1;
    /// <summary>int16_t; medium; reads 2, writes 0; Rewritten/u0070_SfxLoaderStep.cs:55 Rewritten/u0070_SfxLoaderStep.cs:116</summary>
    [FieldOffset(2)] public short field_2;
}

/// <summary>S223: size unknown, 388 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (global:dword_180323244).</summary>
[StructLayout(LayoutKind.Explicit, Size = 388)]
internal unsafe struct Block3
{
    /// <summary>int16_t; medium; reads 3, writes 0; Rewritten/u0070_SfxLoaderStep.cs:141 Rewritten/u0070_SfxLoaderStep.cs:421 Rewritten/u0070_SfxLoaderStep.cs:467</summary>
    [FieldOffset(0)] public short field_0;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0070_SfxLoaderStep.cs:470</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0070_SfxLoaderStep.cs:429</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0070_SfxLoaderStep.cs:420</summary>
    [FieldOffset(6)] public short field_6;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0070_SfxLoaderStep.cs:426</summary>
    [FieldOffset(8)] public uint field_8;
    [FieldOffset(12)] public fixed byte gap_12[4]; // unknown
    /// <summary>uint32_t; medium; reads 3, writes 0; Rewritten/u0070_SfxLoaderStep.cs:475 Rewritten/u0070_SfxLoaderStep.cs:476 Rewritten/u0070_SfxLoaderStep.cs:477</summary>
    [FieldOffset(16)] public uint field_16;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0070_SfxLoaderStep.cs:468</summary>
    [FieldOffset(20)] public uint field_20;
    [FieldOffset(24)] public fixed byte gap_24[36]; // unknown
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0070_SfxLoaderStep.cs:467</summary>
    [FieldOffset(60)] public uint field_60;
    /// <summary>uint32_t; medium; reads 1, writes 1; Rewritten/u0070_SfxLoaderStep.cs:468 Rewritten/u0070_SfxLoaderStep.cs:482</summary>
    [FieldOffset(64)] public uint field_64;
    /// <summary>uint32_t; medium; reads 1, writes 1; Rewritten/u0070_SfxLoaderStep.cs:146 Rewritten/u0070_SfxLoaderStep.cs:475</summary>
    [FieldOffset(68)] public uint field_68;
    /// <summary>int32_t; medium; reads 1, writes 1; Rewritten/u0070_SfxLoaderStep.cs:148 Rewritten/u0070_SfxLoaderStep.cs:478</summary>
    [FieldOffset(72)] public int field_72;
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0070_SfxLoaderStep.cs:132</summary>
    [FieldOffset(76)] public uint field_76;
    /// <summary>uint32_t; medium; reads 1, writes 1; Rewritten/u0006_sub_18003D800.cs:38 Rewritten/u0070_SfxLoaderStep.cs:150</summary>
    [FieldOffset(80)] public uint field_80;
    [FieldOffset(84)] public fixed byte gap_84[300]; // unknown
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0070_SfxLoaderStep.cs:481</summary>
    [FieldOffset(384)] public uint field_384;
}

/// <summary>S228: size unknown, 32 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.ComputeLocalYawDistance(ushort*, ushort*, ushort*, uint*, uint*, uint*):table[5]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Camera
{
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0050_ComputeLocalYawDistance.cs:50</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0050_ComputeLocalYawDistance.cs:51</summary>
    [FieldOffset(4)] public uint field_4;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0050_ComputeLocalYawDistance.cs:52</summary>
    [FieldOffset(8)] public uint field_8;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0050_ComputeLocalYawDistance.cs:53</summary>
    [FieldOffset(12)] public uint field_12;
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0050_ComputeLocalYawDistance.cs:54</summary>
    [FieldOffset(16)] public ushort field_16;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0050_ComputeLocalYawDistance.cs:57</summary>
    [FieldOffset(20)] public uint field_20;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0050_ComputeLocalYawDistance.cs:58</summary>
    [FieldOffset(24)] public uint field_24;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0050_ComputeLocalYawDistance.cs:59</summary>
    [FieldOffset(28)] public uint field_28;
}

/// <summary>S230: size unknown, 18 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.DrawFacingRibbon(ulong, short*, int, short, short, short, short, int, int, int, byte*, byte*, int):table[5]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 18)]
internal unsafe struct Camera2
{
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0052_DrawFacingRibbon.cs:48</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0052_DrawFacingRibbon.cs:49</summary>
    [FieldOffset(4)] public uint field_4;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0052_DrawFacingRibbon.cs:50</summary>
    [FieldOffset(8)] public uint field_8;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0052_DrawFacingRibbon.cs:51</summary>
    [FieldOffset(12)] public uint field_12;
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0052_DrawFacingRibbon.cs:52</summary>
    [FieldOffset(16)] public ushort field_16;
}

/// <summary>S233: size unknown, 18 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.DrawScreenAlignedRibbon(ulong, int, short, short, short, short, int, int, int, byte*, byte*, int):table[5]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 18)]
internal unsafe struct Camera3
{
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0057_CreateRibbonEffectSlot.cs:143</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0057_CreateRibbonEffectSlot.cs:144</summary>
    [FieldOffset(4)] public uint field_4;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0057_CreateRibbonEffectSlot.cs:145</summary>
    [FieldOffset(8)] public uint field_8;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0057_CreateRibbonEffectSlot.cs:146</summary>
    [FieldOffset(12)] public uint field_12;
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0057_CreateRibbonEffectSlot.cs:147</summary>
    [FieldOffset(16)] public ushort field_16;
}

/// <summary>S242: size unknown, 32 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.EmitInterpolatedSpriteTrail(ushort*, long*, int, int):table[5]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Camera4
{
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0092_EmitInterpolatedSpriteTrail.cs:30</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0092_EmitInterpolatedSpriteTrail.cs:31</summary>
    [FieldOffset(4)] public uint field_4;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0092_EmitInterpolatedSpriteTrail.cs:32</summary>
    [FieldOffset(8)] public uint field_8;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0092_EmitInterpolatedSpriteTrail.cs:33</summary>
    [FieldOffset(12)] public uint field_12;
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0092_EmitInterpolatedSpriteTrail.cs:34</summary>
    [FieldOffset(16)] public ushort field_16;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0092_EmitInterpolatedSpriteTrail.cs:36</summary>
    [FieldOffset(20)] public uint field_20;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0092_EmitInterpolatedSpriteTrail.cs:37</summary>
    [FieldOffset(24)] public uint field_24;
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0092_EmitInterpolatedSpriteTrail.cs:38</summary>
    [FieldOffset(28)] public int field_28;
}

/// <summary>S247: size unknown, 32 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.EmitSpriteAnimFrame(long, long*, short*, int, int, int, int):context[5]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Camera5
{
    [FieldOffset(0)] public fixed byte gap_0[16]; // unknown
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0077_EmitSpriteAnimFrame.cs:61</summary>
    [FieldOffset(16)] public ushort field_16;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0077_EmitSpriteAnimFrame.cs:63</summary>
    [FieldOffset(20)] public uint field_20;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0077_EmitSpriteAnimFrame.cs:64</summary>
    [FieldOffset(24)] public uint field_24;
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0077_EmitSpriteAnimFrame.cs:65</summary>
    [FieldOffset(28)] public int field_28;
}

/// <summary>S250: size unknown, 32 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.EmitSpriteCellFrame(ushort*, long*, short*, int, int, int, int, int, int):context[5]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Camera6
{
    [FieldOffset(0)] public fixed byte gap_0[16]; // unknown
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0073_EmitSpriteCellFrame.cs:107</summary>
    [FieldOffset(16)] public ushort field_16;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0073_EmitSpriteCellFrame.cs:109</summary>
    [FieldOffset(20)] public uint field_20;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0073_EmitSpriteCellFrame.cs:110</summary>
    [FieldOffset(24)] public uint field_24;
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0073_EmitSpriteCellFrame.cs:111</summary>
    [FieldOffset(28)] public int field_28;
}

/// <summary>S254: size unknown, 32 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.GetObjectHostPosition(int, int, ushort*):table[5]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Camera7
{
    [FieldOffset(0)] public fixed byte gap_0[16]; // unknown
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0094_HostSetObjectMotion.cs:65</summary>
    [FieldOffset(16)] public ushort field_16;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
    /// <summary>int64_t; medium; reads 1, writes 0; Rewritten/u0094_HostSetObjectMotion.cs:67</summary>
    [FieldOffset(20)] public long field_20;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0094_HostSetObjectMotion.cs:68</summary>
    [FieldOffset(28)] public uint field_28;
}

/// <summary>S263: size unknown, 192 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.PsxGetContextQword(int):ContextAddress(3)).</summary>
[StructLayout(LayoutKind.Explicit, Size = 192)]
internal unsafe struct Context
{
    [FieldOffset(0)] public fixed byte gap_0[36]; // unknown
    /// <summary>int8_t; medium; reads 1, writes 0; Rewritten/u0006_sub_18003D800.cs:130</summary>
    [FieldOffset(36)] public sbyte field_36;
    [FieldOffset(37)] public fixed byte gap_37[2]; // unknown
    /// <summary>int8_t; medium; reads 1, writes 0; Rewritten/u0006_sub_18003D800.cs:114</summary>
    [FieldOffset(39)] public sbyte field_39;
    [FieldOffset(40)] public fixed byte gap_40[40]; // unknown
    /// <summary>int64_t; medium; reads 1, writes 0; Rewritten/u0006_sub_18003D800.cs:104</summary>
    [FieldOffset(80)] public long field_80;
    [FieldOffset(88)] public fixed byte gap_88[96]; // unknown
    /// <summary>int64_t; medium; reads 1, writes 0; Rewritten/u0006_sub_18003D800.cs:109</summary>
    [FieldOffset(184)] public long field_184;
}

/// <summary>S264: size unknown, 160 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.PsxGetContextQword8Or0():ContextAddress(3)).</summary>
[StructLayout(LayoutKind.Explicit, Size = 160)]
internal unsafe struct Context2
{
    [FieldOffset(0)] public fixed byte gap_0[39]; // unknown
    /// <summary>int8_t; medium; reads 2, writes 0; Rewritten/u0009_EvalOrbitTrack.cs:319 Rewritten/u0009_EvalOrbitTrack.cs:324</summary>
    [FieldOffset(39)] public sbyte field_39;
    [FieldOffset(40)] public fixed byte gap_40[112]; // unknown
    /// <summary>int64_t; medium; reads 1, writes 0; Rewritten/u0009_EvalOrbitTrack.cs:329</summary>
    [FieldOffset(152)] public long field_152;
}

/// <summary>S294: 20 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct DISPENV
{
    /// <summary>RECT; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public RECT disp;
    /// <summary>RECT; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public RECT screen;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public byte isinter;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(17)] public byte isrgb24;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public byte pad0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(19)] public byte pad1;
}

/// <summary>S293: 92 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 92)]
internal unsafe struct DRAWENV
{
    /// <summary>RECT; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public RECT clip;
    /// <summary>int16_t[2]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public ShortArray2 ofs;
    /// <summary>RECT; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public RECT tw;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public ushort tpage;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(22)] public byte dtd;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(23)] public byte dfe;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(24)] public byte isbg;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(25)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(26)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(27)] public byte b0;
    /// <summary>DR_ENV; high; file:MemoriaSfx.h</summary>
    [FieldOffset(28)] public DR_ENV dr_env;
}

/// <summary>S316: 12 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 12)]
internal unsafe struct DR_AREA
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint32_t[2]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public UIntArray2 code;
}

/// <summary>S292: 64 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 64)]
internal unsafe struct DR_ENV
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint32_t[15]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public UIntArray15 code;
}

/// <summary>S319: 68 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 68)]
internal unsafe struct DR_LOAD
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint32_t[3]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public UIntArray3 code;
    /// <summary>uint32_t[13]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public UIntArray13 p;
}

/// <summary>S314: 12 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 12)]
internal unsafe struct DR_MODE
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint32_t[2]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public UIntArray2 code;
}

/// <summary>S318: 24 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 24)]
internal unsafe struct DR_MOVE
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint32_t[5]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public UIntArray5 code;
}

/// <summary>S317: 12 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 12)]
internal unsafe struct DR_OFFSET
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint32_t[2]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public UIntArray2 code;
}

/// <summary>S321: 12 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 12)]
internal unsafe struct DR_STP
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint32_t[2]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public UIntArray2 code;
}

/// <summary>S320: 8 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct DR_TPAGE
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public uint code;
}

/// <summary>S315: 12 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 12)]
internal unsafe struct DR_TWIN
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint32_t[2]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public UIntArray2 code;
}

/// <summary>S284: size unknown, 78 bytes = extent (last offset + width); object returned by the address translator Sub1800354E0Ptr (local:Native.Sub1800354E0SpawnOne(Sub1800354E0Frame*, long, long, byte*):*(uint*)(e+36)).</summary>
[StructLayout(LayoutKind.Explicit, Size = 78)]
internal unsafe struct Def
{
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:386</summary>
    [FieldOffset(0)] public uint field_0;
    [FieldOffset(4)] public fixed byte gap_4[2]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 1; Rewritten/u0106g_sub_1800354E0.cs:161 Rewritten/u0106g_sub_1800354E0.cs:574</summary>
    [FieldOffset(6)] public short field_6;
    [FieldOffset(8)] public fixed byte gap_8[4]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:126</summary>
    [FieldOffset(12)] public short field_12;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:207</summary>
    [FieldOffset(14)] public short field_14;
    [FieldOffset(16)] public fixed byte gap_16[34]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:664</summary>
    [FieldOffset(50)] public short field_50;
    [FieldOffset(52)] public fixed byte gap_52[6]; // unknown
    /// <summary>int16_t; medium; reads 6, writes 0; Rewritten/u0106g_sub_1800354E0.cs:673 Rewritten/u0106g_sub_1800354E0.cs:675 Rewritten/u0106g_sub_1800354E0.cs:677</summary>
    [FieldOffset(58)] public short field_58;
    [FieldOffset(60)] public fixed byte gap_60[8]; // unknown
    /// <summary>uint8_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:416</summary>
    [FieldOffset(68)] public byte field_68;
    /// <summary>uint8_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:417</summary>
    [FieldOffset(69)] public byte field_69;
    /// <summary>uint8_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:418</summary>
    [FieldOffset(70)] public byte field_70;
    /// <summary>uint8_t; medium; reads 2, writes 0; Rewritten/u0106g_sub_1800354E0.cs:398 Rewritten/u0106g_sub_1800354E0.cs:405</summary>
    [FieldOffset(71)] public byte field_71;
    /// <summary>uint8_t; medium; union member: overlaps union_72_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0106g_sub_1800354E0.cs:421</summary>
    [FieldOffset(72)] public byte union_72_1;
    /// <summary>uint16_t; medium; union member: overlaps union_72_1, union_73_1; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0106g_sub_1800354E0.cs:616</summary>
    [FieldOffset(72)] public ushort union_72_2;
    /// <summary>uint8_t; medium; union member: overlaps union_72_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0106g_sub_1800354E0.cs:422</summary>
    [FieldOffset(73)] public byte union_73_1;
    /// <summary>uint8_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:423</summary>
    [FieldOffset(74)] public byte field_74;
    [FieldOffset(75)] public fixed byte gap_75[1]; // unknown
    /// <summary>uint16_t; medium; reads 3, writes 0; Rewritten/u0106g_sub_1800354E0.cs:447 Rewritten/u0106g_sub_1800354E0.cs:483 Rewritten/u0106g_sub_1800354E0.cs:524</summary>
    [FieldOffset(76)] public ushort field_76;
}

/// <summary>S271: size unknown, 38 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.SfxEmitParticles(long, long, short*, int*):*(uint*)(state+4)).</summary>
[StructLayout(LayoutKind.Explicit, Size = 38)]
internal unsafe struct Desc
{
    /// <summary>uint8_t; medium; union member: overlaps union_0_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0064_SfxEmitParticles.cs:321</summary>
    [FieldOffset(0)] public byte union_0_1;
    /// <summary>int16_t; medium; union member: overlaps union_0_1, union_1_1; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0064_SfxEmitParticles.cs:249</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>uint8_t; medium; union member: overlaps union_0_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0064_SfxEmitParticles.cs:209</summary>
    [FieldOffset(1)] public byte union_1_1;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0064_SfxEmitParticles.cs:473</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0064_SfxEmitParticles.cs:474</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0064_SfxEmitParticles.cs:476</summary>
    [FieldOffset(6)] public short field_6;
    /// <summary>int16_t; medium; reads 5, writes 0; Rewritten/u0064_SfxEmitParticles.cs:326 Rewritten/u0064_SfxEmitParticles.cs:353 Rewritten/u0064_SfxEmitParticles.cs:374</summary>
    [FieldOffset(8)] public short field_8;
    /// <summary>int16_t; medium; reads 5, writes 0; Rewritten/u0064_SfxEmitParticles.cs:327 Rewritten/u0064_SfxEmitParticles.cs:354 Rewritten/u0064_SfxEmitParticles.cs:375</summary>
    [FieldOffset(10)] public short field_10;
    /// <summary>int16_t; medium; reads 5, writes 0; Rewritten/u0064_SfxEmitParticles.cs:331 Rewritten/u0064_SfxEmitParticles.cs:353 Rewritten/u0064_SfxEmitParticles.cs:380</summary>
    [FieldOffset(12)] public short field_12;
    /// <summary>int16_t; medium; reads 4, writes 0; Rewritten/u0064_SfxEmitParticles.cs:334 Rewritten/u0064_SfxEmitParticles.cs:354 Rewritten/u0064_SfxEmitParticles.cs:372</summary>
    [FieldOffset(14)] public short field_14;
    /// <summary>int16_t; medium; reads 4, writes 0; Rewritten/u0064_SfxEmitParticles.cs:334 Rewritten/u0064_SfxEmitParticles.cs:348 Rewritten/u0064_SfxEmitParticles.cs:411</summary>
    [FieldOffset(16)] public short field_16;
    /// <summary>int16_t; medium; reads 3, writes 0; Rewritten/u0064_SfxEmitParticles.cs:348 Rewritten/u0064_SfxEmitParticles.cs:413 Rewritten/u0064_SfxEmitParticles.cs:500</summary>
    [FieldOffset(18)] public short field_18;
    /// <summary>int16_t; medium; reads 2, writes 0; Rewritten/u0064_SfxEmitParticles.cs:501 Rewritten/u0064_SfxEmitParticles.cs:502</summary>
    [FieldOffset(20)] public short field_20;
    /// <summary>int16_t; medium; reads 3, writes 0; Rewritten/u0064_SfxEmitParticles.cs:361 Rewritten/u0064_SfxEmitParticles.cs:408 Rewritten/u0064_SfxEmitParticles.cs:503</summary>
    [FieldOffset(22)] public short field_22;
    /// <summary>int16_t; medium; reads 5, writes 0; Rewritten/u0064_SfxEmitParticles.cs:355 Rewritten/u0064_SfxEmitParticles.cs:373 Rewritten/u0064_SfxEmitParticles.cs:402</summary>
    [FieldOffset(24)] public short field_24;
    /// <summary>int16_t; medium; reads 3, writes 0; Rewritten/u0064_SfxEmitParticles.cs:356 Rewritten/u0064_SfxEmitParticles.cs:403 Rewritten/u0064_SfxEmitParticles.cs:424</summary>
    [FieldOffset(26)] public short field_26;
    [FieldOffset(28)] public fixed byte gap_28[4]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0064_SfxEmitParticles.cs:341</summary>
    [FieldOffset(32)] public short field_32;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0064_SfxEmitParticles.cs:342</summary>
    [FieldOffset(34)] public short field_34;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0064_SfxEmitParticles.cs:346</summary>
    [FieldOffset(36)] public short field_36;
}

/// <summary>S009: 48 bytes; Effect model slot; 32 of them at 0x180220230 (reset by sub_180015940, freed by Hi_FreeEffModel sub_1800159A0).</summary>
[StructLayout(LayoutKind.Explicit, Size = 48)]
internal unsafe struct EffModelSlot
{
    /// <summary>uint64_t; high; host pointer to a 200-byte model record (assigned from a caller buffer)</summary>
    [FieldOffset(0)] public ulong model;
    /// <summary>uint64_t; high</summary>
    [FieldOffset(8)] public ulong field_8;
    /// <summary>uint64_t; high</summary>
    [FieldOffset(16)] public ulong field_16;
    /// <summary>uint64_t; high</summary>
    [FieldOffset(24)] public ulong field_24;
    /// <summary>uint8_t; high; checked by Hi_FreeEffModel</summary>
    [FieldOffset(32)] public byte inUse;
    /// <summary>uint8_t; high; set to 1 by Hi_SetEffModelSlice (sub_180018A90)</summary>
    [FieldOffset(33)] public byte sliceEnabled;
    /// <summary>uint16_t; high; slot index</summary>
    [FieldOffset(34)] public ushort id;
    /// <summary>uint32_t; high; cleared by Hi_RegisterSolidEffModel</summary>
    [FieldOffset(36)] public uint field_36;
    /// <summary>uint16_t; high; cleared by Hi_RegisterSolidEffModel</summary>
    [FieldOffset(40)] public ushort field_40;
    [FieldOffset(42)] public fixed byte gap_42[6]; // unknown
}

/// <summary>S018: 40 bytes; Part entry of a geoslice mesh (40 bytes): primitive counts per type, PSX addresses of primitive records, UV and color tables; consumed by sub_18000B200; used by CamPushTrackKey (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 40)]
internal unsafe struct GeoSlicePart
{
    [FieldOffset(0)] public fixed byte gap_0[2]; // unknown
    /// <summary>uint16_t; high</summary>
    [FieldOffset(2)] public ushort countFt4;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(4)] public ushort countFt3;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(6)] public ushort countGt4;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(8)] public ushort countGt3;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(10)] public ushort countG4;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(12)] public ushort countG3;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(14)] public ushort countF4;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(16)] public ushort countF3;
    [FieldOffset(18)] public fixed byte gap_18[10]; // unknown
    /// <summary>uint32_t; high</summary>
    [FieldOffset(28)] public uint primsAddr;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(32)] public uint uvTableAddr;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(36)] public uint colorTableAddr;
}

/// <summary>S210: size unknown, 4 bytes = extent (last offset + width); inferred shape H108: 65 access(es) in 19 function(s); size unknown (extent 4).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct GteIr3
{
    /// <summary>uint32_t; medium; reads 0, writes 65; fn:EmitFacingRibbonStrip:980 fn:EmitFacingRibbonStrip:1013 fn:EmitFacingRibbonStrip:1074</summary>
    [FieldOffset(0)] public uint field_0;
}

/// <summary>S201: size unknown, 4 bytes = extent (last offset + width); inferred shape H24: 2 access(es) in 1 function(s); size unknown (extent 4).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct GteV0
{
    /// <summary>uint32_t[]; medium; reads 0, writes 2; indexed with stride 4; 1 elements to the next field; fn:PsxInterpreterRun:514 fn:PsxInterpreterRun:526</summary>
    [FieldOffset(0)] public UIntArray1 field_0;
}

/// <summary>S225: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.BuildSpritePrims(ulong, ulong, ulong, ulong):packet).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Head
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0079_BuildSpritePrims.cs:67 Rewritten/u0079_BuildSpritePrims.cs:68</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0079_BuildSpritePrims.cs:66</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S226: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.BuildSpriteQuads(ulong, ulong, ulong, ulong):packet).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Head2
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0076_BuildSpriteQuads.cs:107</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0076_BuildSpriteQuads.cs:105</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S248: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitSpriteBatch(ulong, ulong, ulong, ulong):otEntry).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Head3
{
    /// <summary>uint32_t; medium; reads 1, writes 2; Rewritten/u0078_EmitSpriteBatch.cs:74 Rewritten/u0078_EmitSpriteBatch.cs:79 Rewritten/u0078_EmitSpriteBatch.cs:80</summary>
    [FieldOffset(0)] public uint field_0;
}

/// <summary>S219: size unknown, 20 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (field:Shape_PsgBuildBoneMatrices_a1.field_8).</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct Header
{
    [FieldOffset(0)] public fixed byte gap_0[3]; // unknown
    /// <summary>uint8_t; medium; reads 2, writes 0; Rewritten/u0039_sub_18000C880.cs:25 Rewritten/u0041_TintObjectPrims.cs:68</summary>
    [FieldOffset(3)] public byte field_3;
    [FieldOffset(4)] public fixed byte gap_4[12]; // unknown
    /// <summary>uint32_t; medium; reads 2, writes 0; Rewritten/u0014_sub_180015370.cs:15 Rewritten/u0029_sub_18000B200.cs:32</summary>
    [FieldOffset(16)] public uint field_16;
}

/// <summary>S265: size unknown, 16 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.PsxInitEntryBlock(uint, long, int, int, int):a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct Header2
{
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0004_sub_1800488C0.cs:78</summary>
    [FieldOffset(0)] public ushort field_0;
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0004_sub_1800488C0.cs:79</summary>
    [FieldOffset(2)] public ushort field_2;
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0004_sub_1800488C0.cs:77</summary>
    [FieldOffset(4)] public uint field_4;
    /// <summary>int64_t; medium; reads 0, writes 1; Rewritten/u0004_sub_1800488C0.cs:80</summary>
    [FieldOffset(8)] public long field_8;
}

/// <summary>S006: 64 bytes; battle object slot; 8 slots at 0x1800694F8 linked through Next (SFX_InitBattle_0); used by HostCallbackSend (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 64)]
internal unsafe struct HostObject
{
    /// <summary>HostObject*; high</summary>
    [FieldOffset(0)] public void* Next;
    /// <summary>uint8_t*; high</summary>
    [FieldOffset(8)] public byte* Aux;
    /// <summary>uint8_t; high; Copied from BTL_DATA_INIT; ref:R001/Assembly-CSharp/Global/BTL_DATA_INIT.cs#L3-L43</summary>
    [FieldOffset(16)] public byte bi_player;
    /// <summary>uint8_t; high; Copied from BTL_DATA_INIT; ref:R001/Assembly-CSharp/Global/BTL_DATA_INIT.cs#L3-L43</summary>
    [FieldOffset(17)] public byte bi_slot_no;
    /// <summary>uint8_t; high; Copied from BTL_DATA_INIT; ref:R001/Assembly-CSharp/Global/BTL_DATA_INIT.cs#L3-L43</summary>
    [FieldOffset(18)] public byte bi_line_no;
    [FieldOffset(19)] public fixed byte gap_19[5]; // unknown
    /// <summary>uint16_t; high</summary>
    [FieldOffset(24)] public ushort HostId;
    /// <summary>uint8_t; high; Copied from BTL_DATA_INIT; ref:R001/Assembly-CSharp/Global/BTL_DATA_INIT.cs#L3-L43</summary>
    [FieldOffset(26)] public byte tar_bone;
    [FieldOffset(27)] public fixed byte gap_27[5]; // unknown
    /// <summary>uint8_t; high; Copied from BTL_DATA_INIT; ref:R001/Assembly-CSharp/Global/BTL_DATA_INIT.cs#L3-L43</summary>
    [FieldOffset(32)] public byte player_serial_no;
    /// <summary>uint8_t; high; Copied from BTL_DATA_INIT; ref:R001/Assembly-CSharp/Global/BTL_DATA_INIT.cs#L3-L43</summary>
    [FieldOffset(33)] public byte player_equip;
    /// <summary>uint8_t; high; Copied from BTL_DATA_INIT; ref:R001/Assembly-CSharp/Global/BTL_DATA_INIT.cs#L3-L43</summary>
    [FieldOffset(34)] public byte player_wep_bone;
    [FieldOffset(35)] public fixed byte gap_35[5]; // unknown
    /// <summary>uint32_t; high; Copied from BTL_DATA_INIT; ref:R001/Assembly-CSharp/Global/BTL_DATA_INIT.cs#L3-L43</summary>
    [FieldOffset(40)] public uint enemy_radius;
    [FieldOffset(44)] public fixed byte gap_44[1]; // unknown
    /// <summary>uint8_t[3]; high; Copied from BTL_DATA_INIT; ref:R001/Assembly-CSharp/Global/BTL_DATA_INIT.cs#L3-L43</summary>
    [FieldOffset(45)] public ByteArray3 enemy_cam_bone;
    /// <summary>uint8_t*; high; Block + Block[5+n] for slots whose Flipped byte is 0</summary>
    [FieldOffset(48)] public byte* SfxData;
    /// <summary>uint32_t; high; Copied from BTL_DATA_INIT; ref:R001/Assembly-CSharp/Global/BTL_DATA_INIT.cs#L3-L43</summary>
    [FieldOffset(56)] public uint geo_radius;
    /// <summary>int32_t; high; Copied from BTL_DATA_INIT; ref:R001/Assembly-CSharp/Global/BTL_DATA_INIT.cs#L3-L43</summary>
    [FieldOffset(60)] public int geo_height;
}

/// <summary>S278: size unknown, 58 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.SfxUpdateKeyframeTracksStep(byte*, int, ref int):*(uint*)(track+36)).</summary>
[StructLayout(LayoutKind.Explicit, Size = 58)]
internal unsafe struct Key
{
    [FieldOffset(0)] public fixed byte gap_0[4]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:100</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:65</summary>
    [FieldOffset(6)] public short field_6;
    /// <summary>int16_t; medium; reads 2, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:103 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:107</summary>
    [FieldOffset(8)] public short field_8;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:103</summary>
    [FieldOffset(10)] public short field_10;
    [FieldOffset(12)] public fixed byte gap_12[4]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:192</summary>
    [FieldOffset(16)] public short field_16;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:192</summary>
    [FieldOffset(18)] public short field_18;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:192</summary>
    [FieldOffset(20)] public short field_20;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:183</summary>
    [FieldOffset(22)] public short field_22;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:199</summary>
    [FieldOffset(24)] public short field_24;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:199</summary>
    [FieldOffset(26)] public short field_26;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:199</summary>
    [FieldOffset(28)] public short field_28;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:89</summary>
    [FieldOffset(30)] public short field_30;
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:209</summary>
    [FieldOffset(32)] public ushort field_32;
    [FieldOffset(34)] public fixed byte gap_34[10]; // unknown
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:137</summary>
    [FieldOffset(44)] public uint field_44;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:138</summary>
    [FieldOffset(48)] public uint field_48;
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:131</summary>
    [FieldOffset(52)] public ushort field_52;
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:132</summary>
    [FieldOffset(54)] public ushort field_54;
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:133</summary>
    [FieldOffset(56)] public ushort field_56;
}

/// <summary>S301: 16 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct LINE_F2
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public short y1;
}

/// <summary>S303: 24 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 24)]
internal unsafe struct LINE_F3
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public short y1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public short x2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short y2;
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public uint pad;
}

/// <summary>S305: 28 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 28)]
internal unsafe struct LINE_F4
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public short y1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public short x2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short y2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public short x3;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(22)] public short y3;
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(24)] public uint pad;
}

/// <summary>S302: 20 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct LINE_G2
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public byte r1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(13)] public byte g1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public byte b1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(15)] public byte p1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short y1;
}

/// <summary>S304: 32 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct LINE_G3
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public byte r1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(13)] public byte g1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public byte b1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(15)] public byte p1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short y1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public byte r2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(21)] public byte g2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(22)] public byte b2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(23)] public byte p2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(24)] public short x2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(26)] public short y2;
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(28)] public uint pad;
}

/// <summary>S306: 40 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 40)]
internal unsafe struct LINE_G4
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public byte r1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(13)] public byte g1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public byte b1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(15)] public byte p1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short y1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public byte r2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(21)] public byte g2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(22)] public byte b2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(23)] public byte p2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(24)] public short x2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(26)] public short y2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(28)] public byte r3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(29)] public byte g3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(30)] public byte b3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(31)] public byte p3;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(32)] public short x3;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(34)] public short y3;
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(36)] public uint pad;
}

/// <summary>S161: 32 bytes; inferred from 1 allocation site(s).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Matrix
{
    /// <summary>uint32_t; high; reads 0, writes 1; fn:ComputeLocalYawDistance:27</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>int16_t; high; reads 0, writes 1; fn:ComputeLocalYawDistance:30</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>uint32_t; high; reads 0, writes 1; fn:ComputeLocalYawDistance:26</summary>
    [FieldOffset(6)] public uint field_6;
    /// <summary>uint64_t; high; union member: overlaps union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ComputeLocalYawDistance:33</summary>
    [FieldOffset(10)] public ulong union_10_8;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:ComputeLocalYawDistance:40</summary>
    [FieldOffset(16)] public short union_16_2;
    [FieldOffset(18)] public fixed byte gap_18[14]; // unknown
}

/// <summary>S236: size unknown, 18 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitDepthCuedRibbon(ulong, int, short, short, short, short, int, int, int, byte*, byte*, int):ctx[5]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 18)]
internal unsafe struct Matrix2
{
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0055_EmitDepthCuedRibbon.cs:76</summary>
    [FieldOffset(0)] public int field_0;
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0055_EmitDepthCuedRibbon.cs:77</summary>
    [FieldOffset(4)] public int field_4;
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0055_EmitDepthCuedRibbon.cs:78</summary>
    [FieldOffset(8)] public int field_8;
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0055_EmitDepthCuedRibbon.cs:79</summary>
    [FieldOffset(12)] public int field_12;
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0055_EmitDepthCuedRibbon.cs:80</summary>
    [FieldOffset(16)] public ushort field_16;
}

/// <summary>S253: size unknown, 32 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.GeoSfxRenderPart(long, byte, uint*, short):table[5]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Matrix3
{
    [FieldOffset(0)] public fixed byte gap_0[16]; // unknown
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0025_GeoSfxRenderPart.cs:37</summary>
    [FieldOffset(16)] public ushort field_16;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0025_GeoSfxRenderPart.cs:39</summary>
    [FieldOffset(20)] public uint field_20;
    /// <summary>uint64_t; medium; reads 1, writes 0; Rewritten/u0025_GeoSfxRenderPart.cs:40</summary>
    [FieldOffset(24)] public ulong field_24;
}

/// <summary>S257: size unknown, 32 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.PsgDrawMeshPart(long, byte, long):table[5]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Matrix4
{
    [FieldOffset(0)] public fixed byte gap_0[16]; // unknown
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0022_PsgDrawMeshPart.cs:29</summary>
    [FieldOffset(16)] public ushort field_16;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0022_PsgDrawMeshPart.cs:31</summary>
    [FieldOffset(20)] public uint field_20;
    /// <summary>uint64_t; medium; reads 1, writes 0; Rewritten/u0022_PsgDrawMeshPart.cs:32</summary>
    [FieldOffset(24)] public ulong field_24;
}

/// <summary>S259: size unknown, 8 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.PsxAddPrimWithMode(int, uint*, ulong, int):cursor).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Mode2
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0071_sub_18003F3A0.cs:27 Rewritten/u0071_sub_18003F3A0.cs:29</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0071_sub_18003F3A0.cs:26</summary>
    [FieldOffset(3)] public byte union_3_1;
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0071_sub_18003F3A0.cs:28</summary>
    [FieldOffset(4)] public uint field_4;
}

/// <summary>S260: size unknown, 8 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.PsxAllocDrawMode(int, sbyte):cursor).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Mode3
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0081_EmitWorldSpriteFrame.cs:145</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0081_EmitWorldSpriteFrame.cs:144</summary>
    [FieldOffset(3)] public byte union_3_1;
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0071_sub_18003F3A0.cs:79</summary>
    [FieldOffset(4)] public uint field_4;
}

/// <summary>S258: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.PsxAddGroundTriFan(ulong, ulong, ulong, ulong):mode).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct ModePacket
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0087_sub_180029890.cs:129</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0087_sub_180029890.cs:127</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S218: size unknown, 3 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (field:PsgModelHandle.ModelAddr).</summary>
[StructLayout(LayoutKind.Explicit, Size = 3)]
internal unsafe struct Model
{
    [FieldOffset(0)] public fixed byte gap_0[2]; // unknown
    /// <summary>uint8_t; medium; reads 4, writes 0; Rewritten/u0021_GeoTransformPart.cs:23 Rewritten/u0024_PsgBuildBoneMatrices.cs:53 Rewritten/u0028_GeoSliceTransformPart.cs:26</summary>
    [FieldOffset(2)] public byte field_2;
}

/// <summary>S269: size unknown, 106 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.SeqStateUpdateRun():((uint*)off_180066C68)[1]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 106)]
internal unsafe struct Obj
{
    [FieldOffset(0)] public fixed byte gap_0[80]; // unknown
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0010_SeqStateUpdate.cs:208</summary>
    [FieldOffset(80)] public ushort field_80;
    /// <summary>uint8_t; medium; reads 1, writes 0; Rewritten/u0010_SeqStateUpdate.cs:209</summary>
    [FieldOffset(82)] public byte field_82;
    /// <summary>uint8_t; medium; reads 1, writes 0; Rewritten/u0010_SeqStateUpdate.cs:210</summary>
    [FieldOffset(83)] public byte field_83;
    [FieldOffset(84)] public fixed byte gap_84[20]; // unknown
    /// <summary>uint8_t; medium; reads 1, writes 0; Rewritten/u0010_SeqStateUpdate.cs:282</summary>
    [FieldOffset(104)] public byte field_104;
    /// <summary>uint8_t; medium; reads 1, writes 0; Rewritten/u0010_SeqStateUpdate.cs:226</summary>
    [FieldOffset(105)] public byte field_105;
}

/// <summary>S267: size unknown, 16 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.PsxObjectAlloc(long, int):address).</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct ObjectArena
{
    /// <summary>int64_t; medium; reads 0, writes 1; Rewritten/u0004_sub_1800488C0.cs:155</summary>
    [FieldOffset(0)] public long field_0;
    /// <summary>uint32_t; medium; reads 1, writes 1; Rewritten/u0004_sub_1800488C0.cs:149 Rewritten/u0004_sub_1800488C0.cs:154</summary>
    [FieldOffset(8)] public uint field_8;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0004_sub_1800488C0.cs:154</summary>
    [FieldOffset(12)] public uint field_12;
}

/// <summary>S239: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitFadedDots(ulong, ulong, ulong, ulong):ot).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct OtEntry
{
    /// <summary>uint32_t; medium; reads 1, writes 2; Rewritten/u0080_EmitFadedDots.cs:61 Rewritten/u0080_EmitFadedDots.cs:68 Rewritten/u0080_EmitFadedDots.cs:69</summary>
    [FieldOffset(0)] public uint field_0;
}

/// <summary>S297: 20 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct POLY_F3
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public short y1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public short x2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short y2;
}

/// <summary>S298: 24 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 24)]
internal unsafe struct POLY_F4
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public short y1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public short x2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short y2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public short x3;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(22)] public short y3;
}

/// <summary>S323: 32 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct POLY_FT3
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public byte u0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(13)] public byte v0;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public ushort clut;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short y1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public byte u1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(21)] public byte v1;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(22)] public ushort tpage;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(24)] public short x2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(26)] public short y2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(28)] public byte u2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(29)] public byte v2;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(30)] public ushort pad1;
}

/// <summary>S324: 40 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 40)]
internal unsafe struct POLY_FT4
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public byte u0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(13)] public byte v0;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public ushort clut;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short y1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public byte u1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(21)] public byte v1;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(22)] public ushort tpage;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(24)] public short x2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(26)] public short y2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(28)] public byte u2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(29)] public byte v2;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(30)] public ushort pad2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(32)] public short x3;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(34)] public short y3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(36)] public byte u3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(37)] public byte v3;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(38)] public ushort pad3;
}

/// <summary>S299: 28 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 28)]
internal unsafe struct POLY_G3
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public byte r1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(13)] public byte g1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public byte b1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(15)] public byte pad1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short y1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public byte r2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(21)] public byte g2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(22)] public byte b2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(23)] public byte pad2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(24)] public short x2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(26)] public short y2;
}

/// <summary>S300: 36 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 36)]
internal unsafe struct POLY_G4
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public byte r1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(13)] public byte g1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public byte b1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(15)] public byte pad1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short y1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public byte r2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(21)] public byte g2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(22)] public byte b2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(23)] public byte pad2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(24)] public short x2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(26)] public short y2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(28)] public byte r3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(29)] public byte g3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(30)] public byte b3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(31)] public byte pad3;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(32)] public short x3;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(34)] public short y3;
}

/// <summary>S325: 40 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 40)]
internal unsafe struct POLY_GT3
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public byte u0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(13)] public byte v0;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public ushort clut;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public byte r1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(17)] public byte g1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public byte b1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(19)] public byte p1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(22)] public short y1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(24)] public byte u1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(25)] public byte v1;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(26)] public ushort tpage;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(28)] public byte r2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(29)] public byte g2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(30)] public byte b2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(31)] public byte p2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(32)] public short x2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(34)] public short y2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(36)] public byte u2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(37)] public byte v2;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(38)] public ushort pad2;
}

/// <summary>S326: 52 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 52)]
internal unsafe struct POLY_GT4
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public byte u0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(13)] public byte v0;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public ushort clut;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public byte r1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(17)] public byte g1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public byte b1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(19)] public byte p1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public short x1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(22)] public short y1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(24)] public byte u1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(25)] public byte v1;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(26)] public ushort tpage;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(28)] public byte r2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(29)] public byte g2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(30)] public byte b2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(31)] public byte p2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(32)] public short x2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(34)] public short y2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(36)] public byte u2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(37)] public byte v2;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(38)] public ushort pad2;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(40)] public byte r3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(41)] public byte g3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(42)] public byte b3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(43)] public byte p3;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(44)] public short x3;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(46)] public short y3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(48)] public byte u3;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(49)] public byte v3;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(50)] public ushort pad3;
}

/// <summary>S322: 32 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct PSXMAT
{
    /// <summary>int16_t[9]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public ShortArray9 r;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short pad;
    /// <summary>int32_t[3]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public IntArray3 t;
}

/// <summary>S296: 4 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct P_CODE
{
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(1)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(2)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(3)] public byte code;
}

/// <summary>S295: 8 bytes; imported from MemoriaSfx.h; used by SFX_GetPrim_0 (return).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct P_TAG
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint addr_len;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
}

/// <summary>S240: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitFadedDots(ulong, ulong, ulong, ulong):prim).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Packet
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0080_EmitFadedDots.cs:61 Rewritten/u0080_EmitFadedDots.cs:62</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0080_EmitFadedDots.cs:60</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S246: size unknown, 44 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.EmitSplitE7Prims(int, short, short, short, int, short, ushort):cursor).</summary>
[StructLayout(LayoutKind.Explicit, Size = 44)]
internal unsafe struct Packet2
{
    [FieldOffset(0)] public fixed byte gap_0[3]; // unknown
    /// <summary>uint8_t; medium; reads 0, writes 1; Rewritten/u0100_EmitSplitE7Prims.cs:29</summary>
    [FieldOffset(3)] public byte field_3;
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0100_EmitSplitE7Prims.cs:30</summary>
    [FieldOffset(4)] public uint field_4;
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0100_EmitSplitE7Prims.cs:31</summary>
    [FieldOffset(8)] public uint field_8;
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0100_EmitSplitE7Prims.cs:32</summary>
    [FieldOffset(12)] public uint field_12;
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0100_EmitSplitE7Prims.cs:33</summary>
    [FieldOffset(16)] public uint field_16;
    [FieldOffset(20)] public fixed byte gap_20[8]; // unknown
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0100_EmitSplitE7Prims.cs:44</summary>
    [FieldOffset(28)] public uint field_28;
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0100_EmitSplitE7Prims.cs:45</summary>
    [FieldOffset(32)] public uint field_32;
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0100_EmitSplitE7Prims.cs:46</summary>
    [FieldOffset(36)] public uint field_36;
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0100_EmitSplitE7Prims.cs:47</summary>
    [FieldOffset(40)] public uint field_40;
}

/// <summary>S249: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitSpriteBatch(ulong, ulong, ulong, ulong):packets).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Packet3
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0078_EmitSpriteBatch.cs:74 Rewritten/u0078_EmitSpriteBatch.cs:75</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0078_EmitSpriteBatch.cs:73</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S252: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitSpriteQuadList(ulong, ulong, ulong, ulong):prim).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Packet4
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0072_sub_18001ECB0.cs:85</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0072_sub_18001ECB0.cs:83</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S262: size unknown, 20 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.PsxGetCallArgPtr(long, int):address).</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct Packet5
{
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106d_SfxScriptDispatch.cs:261</summary>
    [FieldOffset(0)] public short field_0;
    /// <summary>int16_t; medium; union member: overlaps union_3_1; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0106d_SfxScriptDispatch.cs:261</summary>
    [FieldOffset(2)] public short union_2_2;
    /// <summary>uint8_t; medium; union member: overlaps union_2_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0106d_SfxScriptDispatch.cs:259</summary>
    [FieldOffset(3)] public byte union_3_1;
    /// <summary>int16_t; medium; union member: overlaps union_4_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0106d_SfxScriptDispatch.cs:262</summary>
    [FieldOffset(4)] public short union_4_2;
    /// <summary>uint32_t; medium; union member: overlaps union_4_2, union_6_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0106d_SfxScriptDispatch.cs:260</summary>
    [FieldOffset(4)] public uint union_4_4;
    /// <summary>int16_t; medium; union member: overlaps union_4_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0106d_SfxScriptDispatch.cs:262</summary>
    [FieldOffset(6)] public short union_6_2;
    /// <summary>int32_t; medium; reads 0, writes 1; Rewritten/u0106d_SfxScriptDispatch.cs:261</summary>
    [FieldOffset(8)] public int field_8;
    /// <summary>int32_t; medium; reads 0, writes 1; Rewritten/u0106d_SfxScriptDispatch.cs:262</summary>
    [FieldOffset(12)] public int field_12;
    /// <summary>int32_t; medium; reads 0, writes 1; Rewritten/u0106d_SfxScriptDispatch.cs:263</summary>
    [FieldOffset(16)] public int field_16;
}

/// <summary>S227: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.ClearPoolSlotHeaders(uint):a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Pool
{
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0059_sub_180034710.cs:104</summary>
    [FieldOffset(0)] public short field_0;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0059_sub_180034710.cs:101</summary>
    [FieldOffset(2)] public short field_2;
}

/// <summary>S274: size unknown, 16 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.SfxStartScriptSlot(uint):a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct Pool2
{
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106b_PsxCallScript7.cs:90</summary>
    [FieldOffset(0)] public short field_0;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106b_PsxCallScript7.cs:89</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0106b_PsxCallScript7.cs:115</summary>
    [FieldOffset(4)] public int field_4;
    /// <summary>int64_t; medium; reads 1, writes 0; Rewritten/u0106b_PsxCallScript7.cs:113</summary>
    [FieldOffset(8)] public long field_8;
}

/// <summary>S275: size unknown, 16 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.SfxTickScriptSlots(uint):a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct Pool3
{
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106b_PsxCallScript7.cs:131</summary>
    [FieldOffset(0)] public short field_0;
    /// <summary>int16_t; medium; reads 2, writes 0; Rewritten/u0106b_PsxCallScript7.cs:129 Rewritten/u0106b_PsxCallScript7.cs:155</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0106b_PsxCallScript7.cs:142</summary>
    [FieldOffset(4)] public int field_4;
    /// <summary>int64_t; medium; reads 1, writes 0; Rewritten/u0106b_PsxCallScript7.cs:138</summary>
    [FieldOffset(8)] public long field_8;
}

/// <summary>S256: size unknown, 6 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.MoveObjectWithAttachments(long, long, short*):*(uint*)(a2+40)).</summary>
[StructLayout(LayoutKind.Explicit, Size = 6)]
internal unsafe struct Position
{
    /// <summary>int16_t; medium; union member: overlaps union_0_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0059_sub_180034710.cs:14</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>uint32_t; medium; union member: overlaps union_0_2, union_2_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0059_sub_180034710.cs:37</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>int16_t; medium; union member: overlaps union_0_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0059_sub_180034710.cs:17</summary>
    [FieldOffset(2)] public short union_2_2;
    /// <summary>int16_t; medium; reads 1, writes 1; Rewritten/u0059_sub_180034710.cs:18 Rewritten/u0059_sub_180034710.cs:39</summary>
    [FieldOffset(4)] public short field_4;
}

/// <summary>S234: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitConcentricRingQuadsPacket(int, int, int, uint, uint, uint, int, int, int, int, uint, int, int, int, int):packet).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Prim
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0082_EmitConcentricRingQuads.cs:182</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0082_EmitConcentricRingQuads.cs:180</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S235: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitDepthCuedQuadStrip(int, ulong, ulong, ulong):packet).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Prim2
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0053_EmitDepthCuedQuadStrip.cs:168</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0053_EmitDepthCuedQuadStrip.cs:166</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S238: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitFacingRibbonStrip(int, ulong, ulong, ulong):packet).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Prim3
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0051_EmitFacingRibbonStrip.cs:171</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0051_EmitFacingRibbonStrip.cs:169</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S243: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitScreenAlignedQuadStrip(int, ulong, ulong, ulong):packet).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Prim4
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0056_EmitScreenAlignedQuadStrip.cs:217</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0056_EmitScreenAlignedQuadStrip.cs:215</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S245: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitShadedRingQuadsLink(uint, uint, int, byte):packet).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Prim5
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0083_EmitShadedRingQuads.cs:193</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0083_EmitShadedRingQuads.cs:191</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S008: 40 bytes; 40-byte part entry of a PSG model (array at PSX address model+16); counts of 4 primitive kinds and PSX addresses of the primitive list and UV table.</summary>
[StructLayout(LayoutKind.Explicit, Size = 40)]
internal unsafe struct PsgMeshPart
{
    [FieldOffset(0)] public fixed byte gap_0[2]; // unknown
    /// <summary>uint16_t; high</summary>
    [FieldOffset(2)] public ushort Quad24Count;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(4)] public ushort Tri20Count;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(6)] public ushort Quad32Count;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(8)] public ushort Tri24Count;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(10)] public ushort ExtraQuadCount1;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(12)] public ushort ExtraTriCount1;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(14)] public ushort ExtraQuadCount2;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(16)] public ushort ExtraTriCount2;
    [FieldOffset(18)] public fixed byte gap_18[1]; // unknown
    /// <summary>int8_t; high; signed; OT base = OT(ctx[8]) + 4*(8 - OtShift) in sub_180009150 / PsgDrawMeshPart</summary>
    [FieldOffset(19)] public sbyte OtShift;
    /// <summary>uint32_t; high; offset from the model, relocated to a PSX address by sub_180007120</summary>
    [FieldOffset(20)] public uint Addr20;
    /// <summary>uint32_t; high; offset from the model, relocated to a PSX address by sub_180007120</summary>
    [FieldOffset(24)] public uint Addr24;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(28)] public uint PrimitivesAddr;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(32)] public uint UvTableAddr;
    /// <summary>uint32_t; high; PSX address of the uint per-vertex colour table (gouraud); relocated by sub_180007120</summary>
    [FieldOffset(36)] public uint Addr36;
}

/// <summary>S010: size unknown, 20 bytes = extent (last offset + width); PSG model header in PSX memory (reached through the PSX address at handle+8): part count at +3, PSX address of the 40-byte PsgMeshPart array at +16; used by InitE7Packet (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct PsgModel
{
    /// <summary>uint16_t; high; bit 0: offsets at +12/+16 and in the parts already relocated to PSX addresses</summary>
    [FieldOffset(0)] public ushort Flags;
    /// <summary>uint8_t; high; number of 32-byte bone matrices at handle+56; per-bone vertex counts at part Addr20</summary>
    [FieldOffset(2)] public byte BoneCount;
    /// <summary>uint8_t; high</summary>
    [FieldOffset(3)] public byte PartCount;
    [FieldOffset(4)] public fixed byte gap_4[8]; // unknown
    /// <summary>uint32_t; high; offset from the model, relocated to a PSX address</summary>
    [FieldOffset(12)] public uint Addr12;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(16)] public uint PartsAddr;
}

/// <summary>S016: 200 bytes; 200-byte effect model record (EffModelSlot.model); reset by sub_180007120.</summary>
[StructLayout(LayoutKind.Explicit, Size = 200)]
internal unsafe struct PsgModelHandle
{
    /// <summary>uint16_t; high</summary>
    [FieldOffset(0)] public ushort State;
    [FieldOffset(2)] public fixed byte gap_2[2]; // unknown
    /// <summary>uint8_t; high; index of the parent handle's (handle+48) bone matrix this model is attached to (PsgBuildBoneMatrices)</summary>
    [FieldOffset(4)] public byte ParentBoneIndex;
    [FieldOffset(5)] public fixed byte gap_5[3]; // unknown
    /// <summary>uint32_t; high; PSX address of the PsgModel</summary>
    [FieldOffset(8)] public uint ModelAddr;
    [FieldOffset(12)] public fixed byte gap_12[4]; // unknown
    /// <summary>uint64_t; high</summary>
    [FieldOffset(16)] public ulong field_16;
    /// <summary>uint64_t; high; first packet pointer, set by sub_180009150 when 0</summary>
    [FieldOffset(24)] public ulong field_24;
    /// <summary>uint32_t; high; bit i set: Hi_DrawSummonModel skips part i (hidden mesh mask)</summary>
    [FieldOffset(32)] public uint field_32;
    [FieldOffset(36)] public fixed byte gap_36[4]; // unknown
    /// <summary>uint64_t; high; host pointer to uint per texture index: low16 tpage, high16 clut (a2 of sub_180007120)</summary>
    [FieldOffset(40)] public ulong Arg40;
    /// <summary>uint64_t; high</summary>
    [FieldOffset(48)] public ulong field_48;
    /// <summary>uint64_t; high; host pointer to 32-byte GTE matrices (RT + TR), one per bone</summary>
    [FieldOffset(56)] public ulong field_56;
    /// <summary>int16_t[16]; high; 32-byte GTE matrix (RT + TR) of the root; Hi_DrawEffModelByBone copies a summon bone matrix here and multiplies it by a scale</summary>
    [FieldOffset(64)] public ShortArray16 RootMatrix;
    /// <summary>uint64_t; high</summary>
    [FieldOffset(96)] public ulong field_96;
    [FieldOffset(104)] public fixed byte gap_104[8]; // unknown
    /// <summary>uint64_t; high</summary>
    [FieldOffset(112)] public ulong field_112;
    /// <summary>uint32_t; high; reset to 0x10001000</summary>
    [FieldOffset(120)] public uint field_120;
    /// <summary>uint16_t; high; reset to 4096</summary>
    [FieldOffset(124)] public ushort field_124;
    /// <summary>int16_t; high; geoslice: transformed IR2 &gt;= this is clamped to it and flagged 0x10000000 in SZ</summary>
    [FieldOffset(126)] public short SliceLimit;
    /// <summary>uint64_t; high; host pointer to the morph target PsgModel (Hi_DrawMorphModelByBone)</summary>
    [FieldOffset(128)] public ulong field_128;
    /// <summary>uint64_t; high</summary>
    [FieldOffset(136)] public ulong field_136;
    [FieldOffset(144)] public fixed byte gap_144[16]; // unknown
    /// <summary>uint32_t; high</summary>
    [FieldOffset(160)] public uint field_160;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(164)] public ushort field_164;
    [FieldOffset(166)] public fixed byte gap_166[34]; // unknown
}

/// <summary>S007: 80 bytes; Per-texture-group UV bounding boxes filled by sub_180015370 (Hi_DebugPSGData); 8 groups; built on the stack by sub_180015200; used by AccumulatePsgUvBounds (a3).</summary>
[StructLayout(LayoutKind.Explicit, Size = 80)]
internal unsafe struct PsgUvBounds
{
    /// <summary>int32_t; high</summary>
    [FieldOffset(0)] public int Id;
    /// <summary>uint16_t[8]; high</summary>
    [FieldOffset(4)] public UShortArray8 MinU;
    /// <summary>uint16_t[8]; high</summary>
    [FieldOffset(20)] public UShortArray8 MinV;
    /// <summary>uint16_t[8]; high</summary>
    [FieldOffset(36)] public UShortArray8 MaxU;
    /// <summary>uint16_t[8]; high</summary>
    [FieldOffset(52)] public UShortArray8 MaxV;
    /// <summary>uint8_t; high</summary>
    [FieldOffset(68)] public byte SkipBounds;
    [FieldOffset(69)] public fixed byte gap_69[11]; // unknown
}

/// <summary>S001: 32 bytes; Slot of PsxVirtualAddrMapper64 (sub_180012940): window [start-lo .. end) mapped to segment index.</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct PsxAddrMapEntry
{
    /// <summary>uint8_t; high</summary>
    [FieldOffset(0)] public byte used;
    /// <summary>uint8_t; high</summary>
    [FieldOffset(1)] public byte index;
    [FieldOffset(2)] public fixed byte gap_2[6]; // unknown
    /// <summary>uint64_t; high; start - 0x20000 (sub_180012940); subtracted to form the 22-bit offset</summary>
    [FieldOffset(8)] public ulong baseAddress;
    /// <summary>uint64_t; high</summary>
    [FieldOffset(16)] public ulong start;
    /// <summary>uint64_t; high; start + 0x300000</summary>
    [FieldOffset(24)] public ulong end;
}

/// <summary>S002: 9176 bytes; PsxVirtualAddrMapper64: 254 x 32-byte entries, count, RAM host base for segment 128; used by PsxAddrToPtr (a1), PsxAddrMapperRegister (a1), PsxPtrToAddr (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 9176)]
internal unsafe struct PsxAddrMapper
{
    /// <summary>PsxAddrMapEntry[254]; high; array of PsxAddrMapEntry (254 x 32 bytes); index with (PsxAddrMapEntry*)ptr</summary>
    [FieldOffset(0)] public ByteArray8128 entries;
    /// <summary>int32_t; high</summary>
    [FieldOffset(8128)] public int count;
    [FieldOffset(8132)] public fixed byte gap_8132[4]; // unknown
    /// <summary>uint32_t; high; PSX address of RAM (dword_1805789D8)</summary>
    [FieldOffset(8136)] public uint ramPsxAddress;
    [FieldOffset(8140)] public fixed byte gap_8140[4]; // unknown
    /// <summary>uint64_t; high; entry 128 (segment 0x80) maps through this base with | 0x80000000</summary>
    [FieldOffset(8144)] public ulong ramHostBase;
    /// <summary>uint8_t[1024]; high; 1 KB scratchpad for 0x1F800000..0x1F8003FF (unk_1805789E8)</summary>
    [FieldOffset(8152)] public ByteArray1024 scratchpad;
}

/// <summary>S005: 16 bytes; Bump allocator header; data buffer follows at +16. Instances: global at 0x180620BE0, per-object arenas resolved via PSX address in sub_1800488C0..</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct PsxBumpArena
{
    /// <summary>uint8_t*; high</summary>
    [FieldOffset(0)] public byte* Cursor;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(8)] public uint Remaining;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(12)] public uint Capacity;
}

/// <summary>S003: 16 bytes; One argument slot of a native-to-PSX call (sub_18000E130): kind byte, 32-bit value or 64-bit host pointer.</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct PsxCallArg
{
    /// <summary>uint8_t; high; 1 for 'p', 0 for 'v' in the signature string</summary>
    [FieldOffset(0)] public byte isPointer;
    [FieldOffset(1)] public fixed byte gap_1[3]; // unknown
    /// <summary>uint32_t; high; 'v' argument (low 32 bits of the vararg slot); 0 for 'p'</summary>
    [FieldOffset(4)] public uint value;
    /// <summary>uint64_t; high; 'p' argument (full 64-bit vararg slot); 0 for 'v'</summary>
    [FieldOffset(8)] public ulong pointer;
}

/// <summary>S004: 11968 bytes; Recompiled-PSX call context (a1 of sub_18000DBD0 / sub_18000E130); used by SfxIndexChunkTable (a1), PsxResetBankFrame (a1), PsxCallContextCtor (a1), PsxCallContextCtor (return) +11 more.</summary>
[StructLayout(LayoutKind.Explicit, Size = 11968)]
internal unsafe struct PsxCallContext
{
    /// <summary>int32_t; high; -1 after construction (PsxCallContextCtor)</summary>
    [FieldOffset(0)] public int state;
    [FieldOffset(4)] public fixed byte gap_4[4]; // unknown
    /// <summary>uint64_t; high</summary>
    [FieldOffset(8)] public ulong field_8;
    /// <summary>uint8_t; high</summary>
    [FieldOffset(16)] public byte field_10;
    [FieldOffset(17)] public fixed byte gap_17[3]; // unknown
    /// <summary>uint32_t; high</summary>
    [FieldOffset(20)] public uint field_14;
    [FieldOffset(24)] public fixed byte gap_24[3072]; // unknown
    /// <summary>uint8_t[128]; high; 2 x 64-byte buffer objects, ctor SfxBufferReset, dtor SfxBufferFreeData (data pointer at +0x18)</summary>
    [FieldOffset(3096)] public ByteArray128 buffers;
    /// <summary>uint32_t[64]; high; 2 banks x 32 MIPS GPRs (r0..r31); current bank = bank field, stride 128 (sub_1800126C0: a0 at +16, sp at +116)</summary>
    [FieldOffset(3224)] public UIntArray64 gprBanks;
    /// <summary>PsxAddrMapper*; high</summary>
    [FieldOffset(3480)] public PsxAddrMapper* memory;
    /// <summary>int32_t; high</summary>
    [FieldOffset(3488)] public int bank;
    [FieldOffset(3492)] public fixed byte gap_3492[4]; // unknown
    /// <summary>uint64_t[2]; high</summary>
    [FieldOffset(3496)] public ULongArray2 field_DA8;
    /// <summary>uint8_t[8192]; high; 2 x 4 KB PSX stacks registered with the mapper; initial sp of bank i = PSX address of stacks + 4096*i + 4016</summary>
    [FieldOffset(3512)] public ByteArray8192 stacks;
    /// <summary>uint32_t[]; medium; reads 1, writes 4; indexed with stride 4; 2 elements to the next field; fn:PsxInterpreterRun:121 fn:PsxInterpreterRun:124 fn:PsxInterpreterRun:128</summary>
    [FieldOffset(11704)] public UIntArray2 field_11704;
    /// <summary>uint32_t[]; medium; reads 1, writes 4; indexed with stride 4; 2 elements to the next field; fn:PsxInterpreterRun:113 fn:PsxInterpreterRun:116 fn:PsxInterpreterRun:129</summary>
    [FieldOffset(11712)] public UIntArray2 field_11712;
    /// <summary>uint8_t[2]; high</summary>
    [FieldOffset(11720)] public ByteArray2 field_2DC8;
    [FieldOffset(11722)] public fixed byte gap_11722[2]; // unknown
    /// <summary>uint32_t[]; medium; reads 1, writes 1; indexed with stride 4; 3 elements to the next field; fn:PsxInterpreterRun:365 fn:PsxInterpreterRun:455</summary>
    [FieldOffset(11724)] public UIntArray3 field_11724;
    /// <summary>PsxCallArg[14]; high; indexed by signature character position; array of PsxCallArg (14 x 16 bytes); index with (PsxCallArg*)ptr</summary>
    [FieldOffset(11736)] public ByteArray224 args;
    /// <summary>int32_t; high; strlen of the signature string</summary>
    [FieldOffset(11960)] public int argCount;
    [FieldOffset(11964)] public fixed byte gap_11964[4]; // unknown
}

/// <summary>S019: 36 bytes; PSX GPU POLY_G4 packet (code 0x38): tag, then rgb/xy pairs for 4 vertices.</summary>
[StructLayout(LayoutKind.Explicit, Size = 36)]
internal unsafe struct PsxPolyG4
{
    /// <summary>uint32_t; high; low 24 bits: next PSX address, byte 3: word count</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint32_t; high; byte 3 = code</summary>
    [FieldOffset(4)] public uint rgb0;
    /// <summary>int16_t; high</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(12)] public uint rgb1;
    /// <summary>int16_t; high</summary>
    [FieldOffset(16)] public short x1;
    /// <summary>int16_t; high</summary>
    [FieldOffset(18)] public short y1;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(20)] public uint rgb2;
    /// <summary>int16_t; high</summary>
    [FieldOffset(24)] public short x2;
    /// <summary>int16_t; high</summary>
    [FieldOffset(26)] public short y2;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(28)] public uint rgb3;
    /// <summary>int16_t; high</summary>
    [FieldOffset(32)] public short x3;
    /// <summary>int16_t; high</summary>
    [FieldOffset(34)] public short y3;
}

/// <summary>S015: size unknown, 32 bytes = extent (last offset + width); used by PsxInitEntryBlock (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct PsxTaskSlot
{
    /// <summary>uint32_t; high; reads 0, writes 1; fn:StartPsxTask:49</summary>
    [FieldOffset(0)] public uint state;
    /// <summary>uint16_t; high; reads 1, writes 2; fn:StartPsxTask:39 fn:StartPsxTask:47 fn:StartPsxTask:124</summary>
    [FieldOffset(4)] public ushort used;
    /// <summary>uint16_t; high; reads 0, writes 1; fn:StartPsxTask:46</summary>
    [FieldOffset(6)] public ushort index;
    /// <summary>int16_t; high; reads 0, writes 1; fn:StartPsxTask:50</summary>
    [FieldOffset(8)] public short arenaSlot;
    /// <summary>uint16_t; high; reads 0, writes 1; fn:StartPsxTask:48</summary>
    [FieldOffset(10)] public ushort useOwnArena;
    /// <summary>uint32_t; high; reads 3, writes 2; fn:StartPsxTask:66 fn:StartPsxTask:88 fn:StartPsxTask:91</summary>
    [FieldOffset(12)] public uint scratchAddr;
    /// <summary>uint32_t; high; reads 2, writes 1; fn:StartPsxTask:79 fn:StartPsxTask:162 fn:StartPsxTask:178</summary>
    [FieldOffset(16)] public uint entriesAddr;
    /// <summary>uint32_t; high; reads 0, writes 1; fn:StartPsxTask:60</summary>
    [FieldOffset(20)] public uint param;
    /// <summary>uint64_t; high; reads 0, writes 1; fn:StartPsxTask:64</summary>
    [FieldOffset(24)] public ulong handler;
}

/// <summary>S020: 16 bytes; PSX GPU TILE packet (code 0x60): tag, rgb+code, xy, wh.</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct PsxTile
{
    /// <summary>uint32_t; high</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high</summary>
    [FieldOffset(4)] public byte r;
    /// <summary>uint8_t; high</summary>
    [FieldOffset(5)] public byte g;
    /// <summary>uint8_t; high</summary>
    [FieldOffset(6)] public byte b;
    /// <summary>uint8_t; high</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high</summary>
    [FieldOffset(8)] public short x;
    /// <summary>int16_t; high</summary>
    [FieldOffset(10)] public short y;
    /// <summary>int16_t; high</summary>
    [FieldOffset(12)] public short w;
    /// <summary>int16_t; high</summary>
    [FieldOffset(14)] public short h;
}

/// <summary>S290: 8 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct RECT
{
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public short x;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(2)] public short y;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public short w;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public short h;
}

/// <summary>S291: 16 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct RECT32
{
    /// <summary>int32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public int x;
    /// <summary>int32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public int y;
    /// <summary>int32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public int w;
    /// <summary>int32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public int h;
}

/// <summary>S272: size unknown, 62 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.SfxOpCtx16BlendNearObjAnchor(long, Span&lt;short&gt;):*(uint*)(a2+12)).</summary>
[StructLayout(LayoutKind.Explicit, Size = 62)]
internal unsafe struct Record2
{
    [FieldOffset(0)] public fixed byte gap_0[56]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0159_SfxOpCtx16BlendNearObj.cs:308</summary>
    [FieldOffset(56)] public short field_56;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0159_SfxOpCtx16BlendNearObj.cs:309</summary>
    [FieldOffset(58)] public short field_58;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0159_SfxOpCtx16BlendNearObj.cs:310</summary>
    [FieldOffset(60)] public short field_60;
}

/// <summary>S276: size unknown, 76 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.SfxUpdateKeyframeTracksColour(byte*, int, int, short):address).</summary>
[StructLayout(LayoutKind.Explicit, Size = 76)]
internal unsafe struct Rgb
{
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:242</summary>
    [FieldOffset(0)] public int field_0;
    [FieldOffset(4)] public fixed byte gap_4[56]; // unknown
    /// <summary>int16_t; medium; reads 3, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:246 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:256 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:273</summary>
    [FieldOffset(60)] public short field_60;
    /// <summary>int16_t; medium; reads 3, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:247 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:256 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:274</summary>
    [FieldOffset(62)] public short field_62;
    /// <summary>int16_t; medium; reads 3, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:246 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:257 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:273</summary>
    [FieldOffset(64)] public short field_64;
    /// <summary>int16_t; medium; reads 3, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:247 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:257 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:274</summary>
    [FieldOffset(66)] public short field_66;
    /// <summary>int16_t; medium; reads 2, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:263 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:273</summary>
    [FieldOffset(68)] public short field_68;
    /// <summary>int16_t; medium; reads 2, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:263 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:274</summary>
    [FieldOffset(70)] public short field_70;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:264</summary>
    [FieldOffset(72)] public short field_72;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:264</summary>
    [FieldOffset(74)] public short field_74;
}

/// <summary>S038: size unknown, 34 bytes = extent (last offset + width); inferred shape H18: 72 access(es) in 1 function(s); size unknown (extent 34); used by PsgBuildBoneMatrices (a3).</summary>
[StructLayout(LayoutKind.Explicit, Size = 34)]
internal unsafe struct Root
{
    /// <summary>int16_t; medium; union member: overlaps union_0_4; reads 1, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:118 fn:PsgBuildBoneMatrices:331 fn:PsgBuildBoneMatrices:336</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>uint32_t; medium; union member: overlaps union_0_2, union_2_2; reads 0, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:89 fn:PsgBuildBoneMatrices:301</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>int16_t; medium; union member: overlaps union_0_4; reads 1, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:119 fn:PsgBuildBoneMatrices:341 fn:PsgBuildBoneMatrices:346</summary>
    [FieldOffset(2)] public short union_2_2;
    /// <summary>int16_t; medium; union member: overlaps union_4_4; reads 1, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:120 fn:PsgBuildBoneMatrices:351 fn:PsgBuildBoneMatrices:356</summary>
    [FieldOffset(4)] public short union_4_2;
    /// <summary>uint32_t; medium; union member: overlaps union_4_2, union_6_2; reads 0, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:90 fn:PsgBuildBoneMatrices:299</summary>
    [FieldOffset(4)] public uint union_4_4;
    /// <summary>int16_t; medium; union member: overlaps union_4_4; reads 1, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:121 fn:PsgBuildBoneMatrices:332 fn:PsgBuildBoneMatrices:338</summary>
    [FieldOffset(6)] public short union_6_2;
    /// <summary>int16_t; medium; union member: overlaps union_8_4; reads 1, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:122 fn:PsgBuildBoneMatrices:342 fn:PsgBuildBoneMatrices:348</summary>
    [FieldOffset(8)] public short union_8_2;
    /// <summary>uint32_t; medium; union member: overlaps union_8_2, union_10_2; reads 0, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:91 fn:PsgBuildBoneMatrices:300</summary>
    [FieldOffset(8)] public uint union_8_4;
    /// <summary>int16_t; medium; union member: overlaps union_8_4; reads 1, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:123 fn:PsgBuildBoneMatrices:352 fn:PsgBuildBoneMatrices:358</summary>
    [FieldOffset(10)] public short union_10_2;
    /// <summary>int16_t; medium; union member: overlaps union_12_4; reads 1, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:124 fn:PsgBuildBoneMatrices:333 fn:PsgBuildBoneMatrices:340</summary>
    [FieldOffset(12)] public short union_12_2;
    /// <summary>uint32_t; medium; union member: overlaps union_12_2, union_14_2; reads 0, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:92 fn:PsgBuildBoneMatrices:302</summary>
    [FieldOffset(12)] public uint union_12_4;
    /// <summary>int16_t; medium; union member: overlaps union_12_4; reads 1, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:125 fn:PsgBuildBoneMatrices:343 fn:PsgBuildBoneMatrices:350</summary>
    [FieldOffset(14)] public short union_14_2;
    /// <summary>int16_t; medium; union member: overlaps union_16_4; reads 3, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:103 fn:PsgBuildBoneMatrices:126 fn:PsgBuildBoneMatrices:313</summary>
    [FieldOffset(16)] public short union_16_2;
    /// <summary>uint32_t; medium; union member: overlaps union_16_2; reads 0, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:97 fn:PsgBuildBoneMatrices:308</summary>
    [FieldOffset(16)] public uint union_16_4;
    /// <summary>int16_t; medium; union member: overlaps union_20_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:361</summary>
    [FieldOffset(20)] public short union_20_2;
    /// <summary>uint32_t; medium; union member: overlaps union_20_2; reads 2, writes 8; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:98 fn:PsgBuildBoneMatrices:111 fn:PsgBuildBoneMatrices:127</summary>
    [FieldOffset(20)] public uint union_20_4;
    /// <summary>int16_t; medium; union member: overlaps union_24_4, union_24_8; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:362</summary>
    [FieldOffset(24)] public short union_24_2;
    /// <summary>uint32_t; medium; union member: overlaps union_24_2, union_24_8; reads 2, writes 7; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:99 fn:PsgBuildBoneMatrices:112 fn:PsgBuildBoneMatrices:128</summary>
    [FieldOffset(24)] public uint union_24_4;
    /// <summary>uint64_t; medium; union member: overlaps union_24_2, union_24_4, union_28_2, union_28_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:368</summary>
    [FieldOffset(24)] public ulong union_24_8;
    /// <summary>int16_t; medium; union member: overlaps union_24_8, union_28_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:363</summary>
    [FieldOffset(28)] public short union_28_2;
    /// <summary>uint32_t; medium; union member: overlaps union_24_8, union_28_2; reads 2, writes 7; union member: shares its bytes with the other union_* fields of the range; fn:PsgBuildBoneMatrices:100 fn:PsgBuildBoneMatrices:113 fn:PsgBuildBoneMatrices:129</summary>
    [FieldOffset(28)] public uint union_28_4;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:PsgBuildBoneMatrices:364</summary>
    [FieldOffset(32)] public short field_32;
}

/// <summary>S231: size unknown, 32 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.DrawRingMeshEffect(byte*, long*, short*, int, int, int, int):workTable[5]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Rotation
{
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0084_DrawRingMeshEffect.cs:38</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0084_DrawRingMeshEffect.cs:39</summary>
    [FieldOffset(4)] public uint field_4;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0084_DrawRingMeshEffect.cs:40</summary>
    [FieldOffset(8)] public uint field_8;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0084_DrawRingMeshEffect.cs:41</summary>
    [FieldOffset(12)] public uint field_12;
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0084_DrawRingMeshEffect.cs:42</summary>
    [FieldOffset(16)] public ushort field_16;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0084_DrawRingMeshEffect.cs:44</summary>
    [FieldOffset(20)] public int field_20;
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0084_DrawRingMeshEffect.cs:45</summary>
    [FieldOffset(24)] public int field_24;
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0084_DrawRingMeshEffect.cs:46</summary>
    [FieldOffset(28)] public int field_28;
}

/// <summary>S287: 300 bytes; imported from MemoriaSfx.h; used by SFX_InitBattle_0 (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 300)]
internal unsafe struct SFX_INIT_PARAM
{
    /// <summary>uint8_t[2]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public ByteArray2 pad0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(2)] public byte btl_scene_FixedCamera1;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(3)] public byte btl_scene_FixedCamera2;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public short btl_scene_patAddr_putX;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public short btl_scene_patAddr_putZ;
    /// <summary>BTL_DATA_INIT; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public BTL_DATA_INIT btl_data_init0;
    /// <summary>BTL_DATA_INIT; high; file:MemoriaSfx.h</summary>
    [FieldOffset(40)] public BTL_DATA_INIT btl_data_init1;
    /// <summary>BTL_DATA_INIT; high; file:MemoriaSfx.h</summary>
    [FieldOffset(72)] public BTL_DATA_INIT btl_data_init2;
    /// <summary>BTL_DATA_INIT; high; file:MemoriaSfx.h</summary>
    [FieldOffset(104)] public BTL_DATA_INIT btl_data_init3;
    /// <summary>BTL_DATA_INIT; high; file:MemoriaSfx.h</summary>
    [FieldOffset(136)] public BTL_DATA_INIT btl_data_init4;
    /// <summary>BTL_DATA_INIT; high; file:MemoriaSfx.h</summary>
    [FieldOffset(168)] public BTL_DATA_INIT btl_data_init5;
    /// <summary>BTL_DATA_INIT; high; file:MemoriaSfx.h</summary>
    [FieldOffset(200)] public BTL_DATA_INIT btl_data_init6;
    /// <summary>BTL_DATA_INIT; high; file:MemoriaSfx.h</summary>
    [FieldOffset(232)] public BTL_DATA_INIT btl_data_init7;
    /// <summary>int16_t[9]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(264)] public ShortArray9 camMat;
    [FieldOffset(282)] public fixed byte gap_282[2]; // unknown
    /// <summary>int32_t[3]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(284)] public IntArray3 camTrans;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(296)] public short camProj;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(298)] public short pad1;
}

/// <summary>S327: 20 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct SPRT
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public byte u0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(13)] public byte v0;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public ushort clut;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public short w;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(18)] public short h;
}

/// <summary>S328: 16 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct SPRT_16
{
    /// <summary>uint8_t[4]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public ByteArray4 reserved_tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public byte u0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(13)] public byte v0;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public ushort clut;
}

/// <summary>S329: 16 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct SPRT_8
{
    /// <summary>uint8_t[4]; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public ByteArray4 reserved_tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public byte u0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(13)] public byte v0;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public ushort clut;
}

/// <summary>S331: 8 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct SVECTOR
{
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public short vx;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(2)] public short vy;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public short vz;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public short pad;
}

/// <summary>S289: 72 bytes; imported from MemoriaSfx.h; used by SFX_Play_0 (a4), SfxResolveActors (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 72)]
internal unsafe struct S_Eff_Req_Org
{
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public BTL_DATA_REQ exe;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public BTL_DATA_REQ mexe;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public BTL_DATA_REQ trg0;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public BTL_DATA_REQ trg1;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(16)] public BTL_DATA_REQ trg2;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(20)] public BTL_DATA_REQ trg3;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(24)] public BTL_DATA_REQ trg4;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(28)] public BTL_DATA_REQ trg5;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(32)] public BTL_DATA_REQ trg6;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(36)] public BTL_DATA_REQ trg7;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(40)] public BTL_DATA_REQ rtrg0;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(44)] public BTL_DATA_REQ rtrg1;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(48)] public BTL_DATA_REQ rtrg2;
    /// <summary>BTL_DATA_REQ; high; file:MemoriaSfx.h</summary>
    [FieldOffset(52)] public BTL_DATA_REQ rtrg3;
    /// <summary>int8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(56)] public sbyte trgno;
    /// <summary>int8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(57)] public sbyte rtrgno;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(58)] public byte monbone0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(59)] public byte monbone1;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(60)] public short trgcpos_x;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(62)] public short trgcpos_y;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(64)] public short trgcpos_z;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(66)] public short pad;
    /// <summary>uint16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(68)] public ushort flgs;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(70)] public short arg0;
}

/// <summary>S100: size unknown, 14 bytes = extent (last offset + width); inferred shape H83: 14 access(es) in 2 function(s); size unknown (extent 14); used by SfxObjRotateHandler (a2), SfxObjRotateStep (a1), SfxObjRotateStep (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 14)]
internal unsafe struct Scratch
{
    /// <summary>uint8_t; medium; reads 3, writes 1; fn:SfxObjRotateHandler:11 fn:SfxObjRotateStep:18 fn:SfxObjRotateStep:47</summary>
    [FieldOffset(0)] public byte field_0;
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxObjRotateHandler:13 fn:SfxObjRotateStep:21</summary>
    [FieldOffset(1)] public byte field_1;
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxObjRotateHandler:15 fn:SfxObjRotateStep:19</summary>
    [FieldOffset(2)] public byte field_2;
    [FieldOffset(3)] public fixed byte gap_3[1]; // unknown
    /// <summary>uint32_t; medium; reads 2, writes 1; fn:SfxObjRotateHandler:14 fn:SfxObjRotateStep:56 fn:SfxObjRotateStep:63</summary>
    [FieldOffset(4)] public uint field_4;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:SfxObjRotateStep:57</summary>
    [FieldOffset(8)] public short field_8;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:SfxObjRotateStep:59</summary>
    [FieldOffset(10)] public short field_10;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:SfxObjRotateStep:58</summary>
    [FieldOffset(12)] public short field_12;
}

/// <summary>S098: size unknown, 18 bytes = extent (last offset + width); inferred shape H81: 24 access(es) in 2 function(s); size unknown (extent 18); used by SfxObjPairTaskHandler (a2), SfxOpTurnTowardsStep (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 18)]
internal unsafe struct Scratch2
{
    /// <summary>uint8_t; medium; reads 5, writes 1; fn:SfxObjPairTaskHandler:17 fn:SfxOpTurnTowardsStep:48 fn:SfxOpTurnTowardsStep:57</summary>
    [FieldOffset(0)] public byte field_0;
    /// <summary>uint8_t; medium; reads 3, writes 1; fn:SfxObjPairTaskHandler:19 fn:SfxOpTurnTowardsStep:31 fn:SfxOpTurnTowardsStep:36</summary>
    [FieldOffset(1)] public byte field_1;
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxObjPairTaskHandler:22 fn:SfxOpTurnTowardsStep:82</summary>
    [FieldOffset(2)] public byte field_2;
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxObjPairTaskHandler:25 fn:SfxOpTurnTowardsStep:29</summary>
    [FieldOffset(3)] public byte field_3;
    /// <summary>uint32_t; medium; reads 2, writes 1; fn:SfxObjPairTaskHandler:21 fn:SfxOpTurnTowardsStep:96 fn:SfxOpTurnTowardsStep:103</summary>
    [FieldOffset(4)] public uint field_4;
    /// <summary>uint32_t; medium; reads 0, writes 1; fn:SfxObjPairTaskHandler:24</summary>
    [FieldOffset(8)] public uint field_8;
    /// <summary>int16_t; medium; reads 1, writes 1; fn:SfxOpTurnTowardsStep:83 fn:SfxOpTurnTowardsStep:97</summary>
    [FieldOffset(12)] public short field_12;
    /// <summary>int16_t; medium; reads 2, writes 1; fn:SfxOpTurnTowardsStep:85 fn:SfxOpTurnTowardsStep:99</summary>
    [FieldOffset(14)] public short field_14;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:SfxOpTurnTowardsStep:98</summary>
    [FieldOffset(16)] public short field_16;
}

/// <summary>S220: size unknown, 65 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (field:Shape_SfxOpMoveObjToPoint_a3.field_12).</summary>
[StructLayout(LayoutKind.Explicit, Size = 65)]
internal unsafe struct Script
{
    [FieldOffset(0)] public fixed byte gap_0[64]; // unknown
    /// <summary>uint8_t; medium; reads 2, writes 0; Rewritten/u0122_sub_18003B0D0.cs:31 Rewritten/u0122_sub_18003B0D0.cs:37</summary>
    [FieldOffset(64)] public byte field_64;
}

/// <summary>S011: 56 bytes; 56-byte entry of the attachment list at qword_180211E68 (+16 = qword_180211E78): fixed 20.12 position, active flag, owner object.</summary>
[StructLayout(LayoutKind.Explicit, Size = 56)]
internal unsafe struct SfxAttachment
{
    /// <summary>int32_t; high</summary>
    [FieldOffset(0)] public int X;
    /// <summary>int32_t; high</summary>
    [FieldOffset(4)] public int Y;
    /// <summary>int32_t; high</summary>
    [FieldOffset(8)] public int Z;
    [FieldOffset(12)] public fixed byte gap_12[14]; // unknown
    /// <summary>uint8_t; high</summary>
    [FieldOffset(26)] public byte State;
    /// <summary>uint8_t; high</summary>
    [FieldOffset(27)] public byte DetachedState;
    [FieldOffset(28)] public fixed byte gap_28[3]; // unknown
    /// <summary>uint8_t; high</summary>
    [FieldOffset(31)] public byte Active;
    [FieldOffset(32)] public fixed byte gap_32[16]; // unknown
    /// <summary>int64_t; high</summary>
    [FieldOffset(48)] public long Owner;
}

/// <summary>S012: size unknown, 24 bytes = extent (last offset + width); Header of the attachment list (global instance at qword_180211E68): entry count at +0, SfxAttachment array pointer at +16.</summary>
[StructLayout(LayoutKind.Explicit, Size = 24)]
internal unsafe struct SfxAttachmentList
{
    /// <summary>int32_t; high</summary>
    [FieldOffset(0)] public int Count;
    [FieldOffset(4)] public fixed byte gap_4[4]; // unknown
    /// <summary>int32_t; high; highest free-slot index handed out (raised by SfxEmitParticles)</summary>
    [FieldOffset(8)] public int HighestUsedIndex;
    [FieldOffset(12)] public fixed byte gap_12[4]; // unknown
    /// <summary>SfxAttachment*; high</summary>
    [FieldOffset(16)] public SfxAttachment* Entries;
}

/// <summary>S014: size unknown, 120 bytes = extent (last offset + width); Object passed to SfxIndexChunkTable: +8 chunk directory/data, +20 slot count, +24 SfxChunkSlot[] (stride 96).</summary>
[StructLayout(LayoutKind.Explicit, Size = 120)]
internal unsafe struct SfxChunkSet
{
    [FieldOffset(0)] public fixed byte gap_0[8]; // unknown
    /// <summary>int16_t*; high</summary>
    [FieldOffset(8)] public short* directory;
    [FieldOffset(16)] public fixed byte gap_16[4]; // unknown
    /// <summary>uint32_t; high</summary>
    [FieldOffset(20)] public uint slotCount;
    /// <summary>SfxChunkSlot; high; first of slotCount slots</summary>
    [FieldOffset(24)] public SfxChunkSlot firstSlot;
}

/// <summary>S013: 96 bytes; Slot filled by SfxIndexChunkTable (sub_18000D390) for a type-3 chunk; array at +24 of the chunk set, stride 96.</summary>
[StructLayout(LayoutKind.Explicit, Size = 96)]
internal unsafe struct SfxChunkSlot
{
    /// <summary>uint32_t; high</summary>
    [FieldOffset(0)] public uint psxBase;
    [FieldOffset(4)] public fixed byte gap_4[4]; // unknown
    /// <summary>uint32_t*; high</summary>
    [FieldOffset(8)] public uint* data;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(16)] public uint size;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(20)] public uint offsetCount;
    /// <summary>uint32_t[16]; high</summary>
    [FieldOffset(24)] public UIntArray16 offsets;
    /// <summary>uint32_t; high</summary>
    [FieldOffset(88)] public uint firstOffset;
    [FieldOffset(92)] public fixed byte gap_92[4]; // unknown
}

/// <summary>S202: size unknown, 6 bytes = extent (last offset + width); inferred shape H35: 3 access(es) in 1 function(s); size unknown (extent 6); used by CamLookAtMatrix (a3).</summary>
[StructLayout(LayoutKind.Explicit, Size = 6)]
internal unsafe struct Shape_CamLookAtMatrix_a3
{
    /// <summary>int16_t; medium; reads 1, writes 0; fn:CamLookAtMatrix:35</summary>
    [FieldOffset(0)] public short field_0;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:CamLookAtMatrix:36</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:CamLookAtMatrix:37</summary>
    [FieldOffset(4)] public short field_4;
}

/// <summary>S044: size unknown, 6 bytes = extent (last offset + width); inferred shape H26: 8 access(es) in 1 function(s); size unknown (extent 6); used by CamPushTrackKey (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 6)]
internal unsafe struct Shape_CamPushTrackKey_a2
{
    /// <summary>uint8_t; medium; reads 2, writes 0; fn:CamPushTrackKey:11 fn:CamPushTrackKey:16</summary>
    [FieldOffset(0)] public byte field_0;
    /// <summary>uint8_t; medium; reads 1, writes 0; fn:CamPushTrackKey:12</summary>
    [FieldOffset(1)] public byte field_1;
    /// <summary>uint16_t; medium; reads 3, writes 0; fn:CamPushTrackKey:13 fn:CamPushTrackKey:38 fn:CamPushTrackKey:39</summary>
    [FieldOffset(2)] public ushort field_2;
    /// <summary>uint16_t; medium; reads 2, writes 0; fn:CamPushTrackKey:40 fn:CamPushTrackKey:41</summary>
    [FieldOffset(4)] public ushort field_4;
}

/// <summary>S056: size unknown, 12 bytes = extent (last offset + width); inferred shape H39: 3 access(es) in 1 function(s); size unknown (extent 12); used by ComputePsgVertexBytes (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 12)]
internal unsafe struct Shape_ComputePsgVertexBytes_a1
{
    [FieldOffset(0)] public fixed byte gap_0[8]; // unknown
    /// <summary>uint32_t; medium; reads 3, writes 0; fn:ComputePsgVertexBytes:19 fn:ComputePsgVertexBytes:25 fn:ComputePsgVertexBytes:56</summary>
    [FieldOffset(8)] public uint field_8;
}

/// <summary>S049: size unknown, 6 bytes = extent (last offset + width); inferred shape H31: 14 access(es) in 1 function(s); size unknown (extent 6); used by GetAnchorTransform (a2), GetAnchorTransform (a3).</summary>
[StructLayout(LayoutKind.Explicit, Size = 6)]
internal unsafe struct Shape_GetAnchorTransform_a2
{
    /// <summary>uint16_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:GetAnchorTransform:34</summary>
    [FieldOffset(0)] public ushort union_0_2;
    /// <summary>uint32_t; medium; union member: overlaps union_0_2, union_2_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:GetAnchorTransform:19</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint16_t; medium; union member: overlaps union_0_4; reads 0, writes 3; union member: shares its bytes with the other union_* fields of the range; fn:GetAnchorTransform:35 fn:GetAnchorTransform:172 fn:GetAnchorTransform:174</summary>
    [FieldOffset(2)] public ushort union_2_2;
    /// <summary>uint16_t; medium; reads 0, writes 2; fn:GetAnchorTransform:20 fn:GetAnchorTransform:36</summary>
    [FieldOffset(4)] public ushort field_4;
}

/// <summary>S031: size unknown, 18 bytes = extent (last offset + width); inferred shape H11: 2 access(es) in 1 function(s); size unknown (extent 18); used by GteSetRotMatrix (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 18)]
internal unsafe struct Shape_GteSetRotMatrix_a1
{
    [FieldOffset(0)] public fixed byte gap_0[16]; // unknown
    /// <summary>uint16_t; medium; reads 2, writes 0; fn:GteSetRotMatrix:7 fn:GteSetRotMatrix:8</summary>
    [FieldOffset(16)] public ushort field_16;
}

/// <summary>S198: size unknown, 8 bytes = extent (last offset + width); inferred shape H7: 4 access(es) in 1 function(s); size unknown (extent 8); used by HostSendRect64 (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Shape_HostSendRect64_a1
{
    /// <summary>int16_t; medium; reads 1, writes 0; fn:HostSendRect64:14</summary>
    [FieldOffset(0)] public short field_0;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:HostSendRect64:14</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:HostSendRect64:7</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:HostSendRect64:10</summary>
    [FieldOffset(6)] public short field_6;
}

/// <summary>S199: size unknown, 8 bytes = extent (last offset + width); inferred shape H8: 4 access(es) in 1 function(s); size unknown (extent 8); used by HostSendRect65 (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Shape_HostSendRect65_a1
{
    /// <summary>int16_t; medium; reads 1, writes 0; fn:HostSendRect65:5</summary>
    [FieldOffset(0)] public short field_0;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:HostSendRect65:5</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:HostSendRect65:5</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:HostSendRect65:5</summary>
    [FieldOffset(6)] public short field_6;
}

/// <summary>S200: size unknown, 8 bytes = extent (last offset + width); inferred shape H10: 4 access(es) in 1 function(s); size unknown (extent 8); used by InitE7Packet (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Shape_InitE7Packet_a2
{
    /// <summary>int16_t; medium; reads 1, writes 0; fn:InitE7Packet:8</summary>
    [FieldOffset(0)] public short field_0;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:InitE7Packet:8</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:InitE7Packet:10</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:InitE7Packet:10</summary>
    [FieldOffset(6)] public short field_6;
}

/// <summary>S034: size unknown, 18 bytes = extent (last offset + width); inferred shape H14: 9 access(es) in 1 function(s); size unknown (extent 18); used by MulMatrix0 (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 18)]
internal unsafe struct Shape_MulMatrix0_a1
{
    /// <summary>int16_t; medium; reads 1, writes 0; fn:MulMatrix0:25</summary>
    [FieldOffset(0)] public short field_0;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:MulMatrix0:26</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:MulMatrix0:27</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:MulMatrix0:28</summary>
    [FieldOffset(6)] public short field_6;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:MulMatrix0:29</summary>
    [FieldOffset(8)] public short field_8;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:MulMatrix0:30</summary>
    [FieldOffset(10)] public short field_10;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:MulMatrix0:31</summary>
    [FieldOffset(12)] public short field_12;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:MulMatrix0:32</summary>
    [FieldOffset(14)] public short field_14;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:MulMatrix0:34</summary>
    [FieldOffset(16)] public short field_16;
}

/// <summary>S035: size unknown, 168 bytes = extent (last offset + width); inferred shape H15: 164 access(es) in 25 function(s); size unknown (extent 168); used by GeoTransformPart (a1), ObjApplyMatrixScale (a1), PsgDrawMeshPart (a1), PsgBuildBoneMatrices (a1) +12 more.</summary>
[StructLayout(LayoutKind.Explicit, Size = 168)]
internal unsafe struct Shape_PsgBuildBoneMatrices_a1
{
    /// <summary>uint8_t; medium; reads 6, writes 0; fn:ObjApplyMatrixScale:9 fn:PsgDrawMeshPart:232 fn:TintObjectPrims:58</summary>
    [FieldOffset(0)] public byte field_0;
    [FieldOffset(1)] public fixed byte gap_1[3]; // unknown
    /// <summary>uint8_t; medium; reads 4, writes 0; fn:PsgBuildBoneMatrices:85 fn:PsgBuildBoneMatrices:93 fn:PsgBuildBoneMatrices:294</summary>
    [FieldOffset(4)] public byte field_4;
    /// <summary>uint8_t; medium; reads 0, writes 1; fn:PsxObjectSetBlendMode:15</summary>
    [FieldOffset(5)] public byte field_5;
    [FieldOffset(6)] public fixed byte gap_6[2]; // unknown
    /// <summary>uint32_t; medium; reads 44, writes 0; fn:GeoTransformPart:56 fn:GeoTransformPart:63 fn:PsgDrawMeshPart:234</summary>
    [FieldOffset(8)] public uint field_8;
    [FieldOffset(12)] public fixed byte gap_12[4]; // unknown
    /// <summary>void*; medium; reads 4, writes 1; fn:PsgBuildBoneMatrices:77 fn:DrawSummonModel:33 fn:DrawSummonModel:44</summary>
    [FieldOffset(16)] public void* field_16;
    /// <summary>uint64_t; medium; reads 5, writes 9; fn:PsgDrawMeshPart:367 fn:PsgDrawMeshPart:369 fn:TintObjectPrims:60</summary>
    [FieldOffset(24)] public ulong field_24;
    /// <summary>uint32_t; medium; reads 3, writes 0; fn:TintObjectPrims:88 fn:PsxObjectSetBlendMode:43 fn:DrawSummonModel:113</summary>
    [FieldOffset(32)] public uint field_32;
    [FieldOffset(36)] public fixed byte gap_36[4]; // unknown
    /// <summary>void*; medium; reads 3, writes 0; fn:PsgDrawMeshPart:374 fn:GeoSfxRenderPart:361 fn:GeoSliceEmitPrims:291</summary>
    [FieldOffset(40)] public void* field_40;
    /// <summary>void*; medium; reads 4, writes 0; fn:PsgBuildBoneMatrices:81 fn:PsgBuildBoneMatrices:93 fn:PsgBuildBoneMatrices:290</summary>
    [FieldOffset(48)] public void* field_48;
    /// <summary>Root*; medium; reads 28, writes 1; fn:GeoTransformPart:108 fn:PsgBuildBoneMatrices:78 fn:PsgBuildBoneMatrices:90</summary>
    [FieldOffset(56)] public Root* field_56;
    /// <summary>uint16_t; medium; union member: overlaps union_64_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjApplyMatrixScale:19 fn:PsgBuildBoneMatrices:118</summary>
    [FieldOffset(64)] public ushort union_64_2;
    /// <summary>uint32_t; medium; union member: overlaps union_64_2, union_66_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjSetLocalMatrix:19</summary>
    [FieldOffset(64)] public uint union_64_4;
    /// <summary>uint16_t; medium; union member: overlaps union_64_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjApplyMatrixScale:26 fn:PsgBuildBoneMatrices:119</summary>
    [FieldOffset(66)] public ushort union_66_2;
    /// <summary>uint16_t; medium; reads 1, writes 2; fn:ObjApplyMatrixScale:34 fn:PsgBuildBoneMatrices:120 fn:ObjSetLocalMatrix:18</summary>
    [FieldOffset(68)] public ushort field_68;
    /// <summary>uint16_t; medium; union member: overlaps union_70_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjApplyMatrixScale:20 fn:PsgBuildBoneMatrices:121</summary>
    [FieldOffset(70)] public ushort union_70_2;
    /// <summary>uint32_t; medium; union member: overlaps union_70_2, union_72_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjSetLocalMatrix:17</summary>
    [FieldOffset(70)] public uint union_70_4;
    /// <summary>uint16_t; medium; union member: overlaps union_70_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjApplyMatrixScale:27 fn:PsgBuildBoneMatrices:122</summary>
    [FieldOffset(72)] public ushort union_72_2;
    /// <summary>uint16_t; medium; union member: overlaps union_74_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjApplyMatrixScale:36 fn:PsgBuildBoneMatrices:123</summary>
    [FieldOffset(74)] public ushort union_74_2;
    /// <summary>uint32_t; medium; union member: overlaps union_74_2, union_76_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjSetLocalMatrix:20</summary>
    [FieldOffset(74)] public uint union_74_4;
    /// <summary>uint16_t; medium; union member: overlaps union_74_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjApplyMatrixScale:21 fn:PsgBuildBoneMatrices:124</summary>
    [FieldOffset(76)] public ushort union_76_2;
    /// <summary>uint16_t; medium; union member: overlaps union_78_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjApplyMatrixScale:28 fn:PsgBuildBoneMatrices:125</summary>
    [FieldOffset(78)] public ushort union_78_2;
    /// <summary>uint32_t; medium; union member: overlaps union_78_2, union_80_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjSetLocalMatrix:16</summary>
    [FieldOffset(78)] public uint union_78_4;
    /// <summary>uint16_t; medium; union member: overlaps union_78_4; reads 3, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjApplyMatrixScale:13 fn:ObjApplyMatrixScale:38 fn:PsgBuildBoneMatrices:126</summary>
    [FieldOffset(80)] public ushort union_80_2;
    [FieldOffset(82)] public fixed byte gap_82[2]; // unknown
    /// <summary>uint8_t[12]; unknown; reads 5, writes 5; conflict: 4:uint32_t, 4:uint32_t@88, 4:uint32_t@92, 8:uint64_t, 8:uint64_t@88; possibly a union; fn:PsgBuildBoneMatrices:127 fn:PsgBuildBoneMatrices:128 fn:PsgBuildBoneMatrices:129</summary>
    [FieldOffset(84)] public ByteArray12 raw_84;
    [FieldOffset(96)] public fixed byte gap_96[16]; // unknown
    /// <summary>uint64_t; medium; reads 3, writes 0; fn:StartSummonTexAnim:14 fn:StartSummonTexAnim:20 fn:StartSummonTexAnim:21</summary>
    [FieldOffset(112)] public ulong field_112;
    /// <summary>int16_t; medium; union member: overlaps union_120_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:ObjApplyMatrixScale:11</summary>
    [FieldOffset(120)] public short union_120_2;
    /// <summary>uint32_t; medium; union member: overlaps union_120_2, union_122_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ObjSetLocalMatrix:59</summary>
    [FieldOffset(120)] public uint union_120_4;
    /// <summary>int16_t; medium; union member: overlaps union_120_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:ObjApplyMatrixScale:18</summary>
    [FieldOffset(122)] public short union_122_2;
    /// <summary>int16_t; medium; reads 1, writes 1; fn:ObjApplyMatrixScale:25 fn:ObjSetLocalMatrix:60</summary>
    [FieldOffset(124)] public short field_124;
    /// <summary>int16_t; medium; reads 1, writes 1; fn:GeoSliceTransformPart:105 fn:DrawSliceEffModel:57</summary>
    [FieldOffset(126)] public short field_126;
    /// <summary>void*; medium; reads 2, writes 2; fn:GeoMorphTransformPart:132 fn:GeoMorphTransformPart:145 fn:DrawMorphEffModel:65</summary>
    [FieldOffset(128)] public void* field_128;
    [FieldOffset(136)] public fixed byte gap_136[24]; // unknown
    /// <summary>uint64_t; medium; reads 2, writes 0; fn:PsgBuildBoneMatrices:104 fn:PsgBuildBoneMatrices:314</summary>
    [FieldOffset(160)] public ulong field_160;
}

/// <summary>S058: size unknown, 12 bytes = extent (last offset + width); inferred shape H41: 2 access(es) in 1 function(s); size unknown (extent 12); used by PsgHandlePartCount (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 12)]
internal unsafe struct Shape_PsgHandlePartCount_a1
{
    [FieldOffset(0)] public fixed byte gap_0[8]; // unknown
    /// <summary>uint32_t; medium; reads 2, writes 0; fn:PsgHandlePartCount:6 fn:PsgHandlePartCount:14</summary>
    [FieldOffset(8)] public uint field_8;
}

/// <summary>S102: size unknown, 5 bytes = extent (last offset + width); inferred shape H86: 3 access(es) in 1 function(s); size unknown (extent 5); used by SfxObjCmd1EHandler (a5), SfxObjCmd1FHandler (a5), SfxObjCmd1DHandler (a5), SfxObjCmd0CHandler (a5) +22 more.</summary>
[StructLayout(LayoutKind.Explicit, Size = 5)]
internal unsafe struct Shape_SfxObjAnimWaitHandler_a5
{
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjFlagsMatchHandler:13</summary>
    [FieldOffset(0)] public byte union_0_1;
    /// <summary>uint32_t; medium; union member: overlaps union_0_1; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjFlagsMatchHandler:27</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; reads 1, writes 0; fn:SfxObjFlagsMatchHandler:15</summary>
    [FieldOffset(4)] public byte field_4;
}

/// <summary>S105: size unknown, 4 bytes = extent (last offset + width); inferred shape H92: 2 access(es) in 1 function(s); size unknown (extent 4); used by SfxObjResetHandler (a5), SfxOpHostCmd0D (a5), SfxObjCallKindScriptHandler (a5), SfxOpMoveObjToPoint (a5).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Shape_SfxObjCallKindScriptHandler_a5
{
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxOpMoveObjToPoint:26</summary>
    [FieldOffset(0)] public byte union_0_1;
    /// <summary>uint32_t; medium; union member: overlaps union_0_1; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxOpMoveObjToPoint:20</summary>
    [FieldOffset(0)] public uint union_0_4;
}

/// <summary>S206: size unknown, 2 bytes = extent (last offset + width); inferred shape H51: 5 access(es) in 1 function(s); size unknown (extent 2); used by SfxObjCmd0CHandler (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 2)]
internal unsafe struct Shape_SfxObjCmd0CHandler_a2
{
    /// <summary>uint8_t; medium; reads 2, writes 1; fn:SfxObjCmd0CHandler:12 fn:SfxObjCmd0CHandler:18 fn:SfxObjCmd0CHandler:19</summary>
    [FieldOffset(0)] public byte field_0;
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxObjCmd0CHandler:13 fn:SfxObjCmd0CHandler:20</summary>
    [FieldOffset(1)] public byte field_1;
}

/// <summary>S205: size unknown, 2 bytes = extent (last offset + width); inferred shape H45: 4 access(es) in 1 function(s); size unknown (extent 2); used by SfxObjCmd1EHandler (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 2)]
internal unsafe struct Shape_SfxObjCmd1EHandler_a2
{
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxObjCmd1EHandler:14 fn:SfxObjCmd1EHandler:23</summary>
    [FieldOffset(0)] public byte field_0;
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxObjCmd1EHandler:15 fn:SfxObjCmd1EHandler:18</summary>
    [FieldOffset(1)] public byte field_1;
}

/// <summary>S064: size unknown, 4 bytes = extent (last offset + width); inferred shape H47: 7 access(es) in 1 function(s); size unknown (extent 4); used by SfxObjCmd1FHandler (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Shape_SfxObjCmd1FHandler_a2
{
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxObjCmd1FHandler:14 fn:SfxObjCmd1FHandler:24</summary>
    [FieldOffset(0)] public byte field_0;
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxObjCmd1FHandler:15 fn:SfxObjCmd1FHandler:24</summary>
    [FieldOffset(1)] public byte field_1;
    /// <summary>uint8_t; medium; union member: overlaps union_2_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjCmd1FHandler:24</summary>
    [FieldOffset(2)] public byte union_2_1;
    /// <summary>uint16_t; medium; union member: overlaps union_2_1, union_3_1; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjCmd1FHandler:16</summary>
    [FieldOffset(2)] public ushort union_2_2;
    /// <summary>uint8_t; medium; union member: overlaps union_2_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjCmd1FHandler:19</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S208: size unknown, 2 bytes = extent (last offset + width); inferred shape H65: 4 access(es) in 1 function(s); size unknown (extent 2); used by SfxObjCmd21Handler (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 2)]
internal unsafe struct Shape_SfxObjCmd21Handler_a2
{
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxObjCmd21Handler:8 fn:SfxObjCmd21Handler:14</summary>
    [FieldOffset(0)] public byte field_0;
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxObjCmd21Handler:9 fn:SfxObjCmd21Handler:14</summary>
    [FieldOffset(1)] public byte field_1;
}

/// <summary>S209: size unknown, 2 bytes = extent (last offset + width); inferred shape H95: 2 access(es) in 1 function(s); size unknown (extent 2); used by SfxObjFlagsMatchHandler (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 2)]
internal unsafe struct Shape_SfxObjFlagsMatchHandler_a2
{
    /// <summary>uint8_t; medium; reads 0, writes 1; fn:SfxObjFlagsMatchHandler:14</summary>
    [FieldOffset(0)] public byte field_0;
    /// <summary>uint8_t; medium; reads 0, writes 1; fn:SfxObjFlagsMatchHandler:18</summary>
    [FieldOffset(1)] public byte field_1;
}

/// <summary>S138: size unknown, 44 bytes = extent (last offset + width); inferred shape H126: 4 access(es) in 1 function(s); size unknown (extent 44); used by SfxObjectEffectHandler (a3).</summary>
[StructLayout(LayoutKind.Explicit, Size = 44)]
internal unsafe struct Shape_SfxObjectEffectHandler_a3
{
    [FieldOffset(0)] public fixed byte gap_0[12]; // unknown
    /// <summary>uint32_t; medium; reads 2, writes 0; fn:SfxObjectEffectHandler:60 fn:SfxObjectEffectHandler:61</summary>
    [FieldOffset(12)] public uint field_12;
    [FieldOffset(16)] public fixed byte gap_16[24]; // unknown
    /// <summary>uint32_t; medium; reads 0, writes 2; fn:SfxObjectEffectHandler:124 fn:SfxObjectEffectHandler:255</summary>
    [FieldOffset(40)] public uint field_40;
}

/// <summary>S128: size unknown, 8 bytes = extent (last offset + width); inferred shape H116: 4 access(es) in 1 function(s); size unknown (extent 8); used by SfxObjectStrikeSequence (a5).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Shape_SfxObjectStrikeSequence_a5
{
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectStrikeSequence:74</summary>
    [FieldOffset(0)] public byte union_0_1;
    /// <summary>uint32_t; medium; union member: overlaps union_0_1; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectStrikeSequence:66</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_4_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectStrikeSequence:73</summary>
    [FieldOffset(4)] public byte union_4_1;
    /// <summary>int32_t; medium; union member: overlaps union_4_1; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectStrikeSequence:131</summary>
    [FieldOffset(4)] public int union_4_4;
}

/// <summary>S212: size unknown, 8 bytes = extent (last offset + width); inferred shape H123: 4 access(es) in 1 function(s); size unknown (extent 8); used by SfxOpCopyVarToEntry (a5).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Shape_SfxOpCopyVarToEntry_a5
{
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxOpCopyVarToEntry:9</summary>
    [FieldOffset(0)] public int field_0;
    /// <summary>int32_t; medium; reads 3, writes 0; fn:SfxOpCopyVarToEntry:10 fn:SfxOpCopyVarToEntry:11 fn:SfxOpCopyVarToEntry:15</summary>
    [FieldOffset(4)] public int field_4;
}

/// <summary>S144: size unknown, 8 bytes = extent (last offset + width); inferred shape H132: 4 access(es) in 1 function(s); size unknown (extent 8); used by SfxOpHostLevelFade (a5).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Shape_SfxOpHostLevelFade_a5
{
    /// <summary>int16_t; medium; union member: overlaps union_0_4; reads 2, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxOpHostLevelFade:28 fn:SfxOpHostLevelFade:29</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>uint32_t; medium; union member: overlaps union_0_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxOpHostLevelFade:56</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint32_t; medium; reads 1, writes 0; fn:SfxOpHostLevelFade:33</summary>
    [FieldOffset(4)] public uint field_4;
}

/// <summary>S147: size unknown, 12 bytes = extent (last offset + width); inferred shape H135: 11 access(es) in 1 function(s); size unknown (extent 12); used by SfxOpMotionFade (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 12)]
internal unsafe struct Shape_SfxOpMotionFade_a2
{
    /// <summary>uint32_t; medium; reads 2, writes 1; fn:SfxOpMotionFade:22 fn:SfxOpMotionFade:34 fn:SfxOpMotionFade:35</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>int16_t; medium; union member: overlaps union_4_4; reads 2, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxOpMotionFade:25 fn:SfxOpMotionFade:34</summary>
    [FieldOffset(4)] public short union_4_2;
    /// <summary>uint32_t; medium; union member: overlaps union_4_2, union_6_2; reads 0, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:SfxOpMotionFade:16 fn:SfxOpMotionFade:18</summary>
    [FieldOffset(4)] public uint union_4_4;
    /// <summary>int16_t; medium; union member: overlaps union_4_4; reads 2, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxOpMotionFade:25 fn:SfxOpMotionFade:34</summary>
    [FieldOffset(6)] public short union_6_2;
    /// <summary>uint32_t; medium; reads 1, writes 1; fn:SfxOpMotionFade:20 fn:SfxOpMotionFade:33</summary>
    [FieldOffset(8)] public uint field_8;
}

/// <summary>S214: size unknown, 8 bytes = extent (last offset + width); inferred shape H134: 5 access(es) in 1 function(s); size unknown (extent 8); used by SfxOpMotionFade (a5).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Shape_SfxOpMotionFade_a5
{
    /// <summary>int32_t; medium; reads 2, writes 1; fn:SfxOpMotionFade:10 fn:SfxOpMotionFade:21 fn:SfxOpMotionFade:22</summary>
    [FieldOffset(0)] public int field_0;
    /// <summary>int32_t; medium; reads 2, writes 0; fn:SfxOpMotionFade:15 fn:SfxOpMotionFade:19</summary>
    [FieldOffset(4)] public int field_4;
}

/// <summary>S132: size unknown, 16 bytes = extent (last offset + width); inferred shape H120: 4 access(es) in 1 function(s); size unknown (extent 16); used by SfxOpMoveObjToPoint (a3).</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct Shape_SfxOpMoveObjToPoint_a3
{
    [FieldOffset(0)] public fixed byte gap_0[12]; // unknown
    /// <summary>uint32_t; medium; reads 4, writes 0; fn:SfxOpMoveObjToPoint:27 fn:SfxOpMoveObjToPoint:39 fn:SfxOpMoveObjToPoint:54</summary>
    [FieldOffset(12)] public uint field_12;
}

/// <summary>S217: size unknown, 8 bytes = extent (last offset + width); inferred shape H140: 8 access(es) in 1 function(s); size unknown (extent 8); used by SfxOpScreenTilesFade (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Shape_SfxOpScreenTilesFade_a2
{
    /// <summary>uint32_t; medium; reads 1, writes 1; fn:SfxOpScreenTilesFade:30 fn:SfxOpScreenTilesFade:62</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>uint32_t; medium; reads 4, writes 2; fn:SfxOpScreenTilesFade:29 fn:SfxOpScreenTilesFade:55 fn:SfxOpScreenTilesFade:59</summary>
    [FieldOffset(4)] public uint field_4;
}

/// <summary>S216: size unknown, 8 bytes = extent (last offset + width); inferred shape H138: 4 access(es) in 1 function(s); size unknown (extent 8); used by SfxOpScreenTilesFade (a5).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Shape_SfxOpScreenTilesFade_a5
{
    /// <summary>int32_t; medium; reads 2, writes 1; fn:SfxOpScreenTilesFade:14 fn:SfxOpScreenTilesFade:50 fn:SfxOpScreenTilesFade:51</summary>
    [FieldOffset(0)] public int field_0;
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxOpScreenTilesFade:61</summary>
    [FieldOffset(4)] public int field_4;
}

/// <summary>S211: size unknown, 5 bytes = extent (last offset + width); inferred shape H111: 2 access(es) in 1 function(s); size unknown (extent 5); used by SfxOpSetParamBytes23 (a5).</summary>
[StructLayout(LayoutKind.Explicit, Size = 5)]
internal unsafe struct Shape_SfxOpSetParamBytes23_a5
{
    /// <summary>uint8_t; medium; reads 1, writes 0; fn:SfxOpSetParamBytes23:6</summary>
    [FieldOffset(0)] public byte field_0;
    [FieldOffset(1)] public fixed byte gap_1[3]; // unknown
    /// <summary>uint8_t; medium; reads 1, writes 0; fn:SfxOpSetParamBytes23:7</summary>
    [FieldOffset(4)] public byte field_4;
}

/// <summary>S142: size unknown, 10 bytes = extent (last offset + width); inferred shape H130: 2 access(es) in 1 function(s); size unknown (extent 10); used by SfxOpStartTrackBlock (a4).</summary>
[StructLayout(LayoutKind.Explicit, Size = 10)]
internal unsafe struct Shape_SfxOpStartTrackBlock_a4
{
    [FieldOffset(0)] public fixed byte gap_0[8]; // unknown
    /// <summary>int16_t; medium; reads 2, writes 0; fn:SfxOpStartTrackBlock:26 fn:SfxOpStartTrackBlock:46</summary>
    [FieldOffset(8)] public short field_8;
}

/// <summary>S213: size unknown, 8 bytes = extent (last offset + width); inferred shape H129: 2 access(es) in 1 function(s); size unknown (extent 8); used by SfxOpStartTrackBlock (a5).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Shape_SfxOpStartTrackBlock_a5
{
    /// <summary>uint32_t; medium; reads 1, writes 0; fn:SfxOpStartTrackBlock:18</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>uint32_t; medium; reads 1, writes 0; fn:SfxOpStartTrackBlock:15</summary>
    [FieldOffset(4)] public uint field_4;
}

/// <summary>S215: size unknown, 8 bytes = extent (last offset + width); inferred shape H137: 3 access(es) in 1 function(s); size unknown (extent 8); used by SfxOpVoicePlay (a5).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Shape_SfxOpVoicePlay_a5
{
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxOpVoicePlay:11</summary>
    [FieldOffset(0)] public int field_0;
    /// <summary>int32_t; medium; reads 2, writes 0; fn:SfxOpVoicePlay:9 fn:SfxOpVoicePlay:10</summary>
    [FieldOffset(4)] public int field_4;
}

/// <summary>S125: size unknown, 16 bytes = extent (last offset + width); inferred shape H113: 2 access(es) in 1 function(s); size unknown (extent 16); used by SfxTrailSequenceHandler (a3).</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct Shape_SfxTrailSequenceHandler_a3
{
    [FieldOffset(0)] public fixed byte gap_0[12]; // unknown
    /// <summary>uint32_t; medium; reads 2, writes 0; fn:SfxTrailSequenceHandler:26 fn:SfxTrailSequenceHandler:51</summary>
    [FieldOffset(12)] public uint field_12;
}

/// <summary>S207: size unknown, 8 bytes = extent (last offset + width); inferred shape H59: 10 access(es) in 2 function(s); size unknown (extent 8); used by SfxWaitObjCmd0C17OffsetHandler (a2), SfxWaitObjCmd0C17Offset (a1), SfxWaitObjCmd0C17Offset (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Shape_SfxWaitObjCmd0C17Offset_a1
{
    /// <summary>uint8_t; medium; reads 4, writes 1; fn:SfxWaitObjCmd0C17OffsetHandler:8 fn:SfxWaitObjCmd0C17Offset:15 fn:SfxWaitObjCmd0C17Offset:16</summary>
    [FieldOffset(0)] public byte field_0;
    /// <summary>uint8_t; medium; reads 0, writes 1; fn:SfxWaitObjCmd0C17OffsetHandler:9</summary>
    [FieldOffset(1)] public byte field_1;
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxWaitObjCmd0C17OffsetHandler:10 fn:SfxWaitObjCmd0C17Offset:36</summary>
    [FieldOffset(2)] public byte field_2;
    [FieldOffset(3)] public fixed byte gap_3[1]; // unknown
    /// <summary>uint32_t; medium; reads 1, writes 1; fn:SfxWaitObjCmd0C17Offset:36 fn:SfxWaitObjCmd0C17Offset:38</summary>
    [FieldOffset(4)] public uint field_4;
}

/// <summary>S203: size unknown, 5 bytes = extent (last offset + width); inferred shape H42: 2 access(es) in 1 function(s); size unknown (extent 5); used by StoreBytePairA (a5).</summary>
[StructLayout(LayoutKind.Explicit, Size = 5)]
internal unsafe struct Shape_StoreBytePairA_a5
{
    /// <summary>uint8_t; medium; reads 1, writes 0; fn:StoreBytePairA:6</summary>
    [FieldOffset(0)] public byte field_0;
    [FieldOffset(1)] public fixed byte gap_1[3]; // unknown
    /// <summary>uint8_t; medium; reads 1, writes 0; fn:StoreBytePairA:7</summary>
    [FieldOffset(4)] public byte field_4;
}

/// <summary>S204: size unknown, 5 bytes = extent (last offset + width); inferred shape H43: 2 access(es) in 1 function(s); size unknown (extent 5); used by StoreBytePairB (a5).</summary>
[StructLayout(LayoutKind.Explicit, Size = 5)]
internal unsafe struct Shape_StoreBytePairB_a5
{
    /// <summary>uint8_t; medium; reads 1, writes 0; fn:StoreBytePairB:6</summary>
    [FieldOffset(0)] public byte field_0;
    [FieldOffset(1)] public fixed byte gap_1[3]; // unknown
    /// <summary>uint8_t; medium; reads 1, writes 0; fn:StoreBytePairB:7</summary>
    [FieldOffset(4)] public byte field_4;
}

/// <summary>S120: size unknown, 6 bytes = extent (last offset + width); inferred shape H107: 2 access(es) in 1 function(s); size unknown (extent 6); used by VectorToYawDistance (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 6)]
internal unsafe struct Shape_VectorToYawDistance_a1
{
    /// <summary>uint32_t; medium; reads 1, writes 0; fn:VectorToYawDistance:7</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>int16_t; medium; reads 1, writes 0; fn:VectorToYawDistance:8</summary>
    [FieldOffset(4)] public short field_4;
}

/// <summary>S136: size unknown, 2 bytes = extent (last offset + width); inferred shape H124: 3 access(es) in 2 function(s); size unknown (extent 2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 2)]
internal unsafe struct Shape_qword_180323260
{
    /// <summary>uint16_t[]; medium; reads 3, writes 0; indexed with stride 2; 1 elements to the next field; fn:SfxOpCopyVarToEntry:13 fn:SfxOpCopyVarToEntry:17 fn:SfxObjectEffectHandler:83</summary>
    [FieldOffset(0)] public UShortArray1 field_0;
}

/// <summary>S222: size unknown, 8 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (global:dword_180323234).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Site_PsxAddrToPtr
{
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0067_SfxBuildRotScaleMatrix.cs:287</summary>
    [FieldOffset(0)] public ushort field_0;
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0067_SfxBuildRotScaleMatrix.cs:288</summary>
    [FieldOffset(2)] public ushort field_2;
    [FieldOffset(4)] public fixed byte gap_4[2]; // unknown
    /// <summary>uint16_t; medium; reads 1, writes 0; Rewritten/u0067_SfxBuildRotScaleMatrix.cs:293</summary>
    [FieldOffset(6)] public ushort field_6;
}

/// <summary>S266: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.PsxInterpreterRunMem(long, int, uint):baseValue+(uint)offset).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Site_PsxAddrToPtr2
{
    /// <summary>int8_t; medium; union member: overlaps union_0_2, union_0_4; reads 2, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0106e_PsxInterpreterRun.cs:195 Rewritten/u0106e_PsxInterpreterRun.cs:206 Rewritten/u0106e_PsxInterpreterRun.cs:214</summary>
    [FieldOffset(0)] public sbyte union_0_1;
    /// <summary>int16_t; medium; union member: overlaps union_0_1, union_0_4; reads 2, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0106e_PsxInterpreterRun.cs:198 Rewritten/u0106e_PsxInterpreterRun.cs:209 Rewritten/u0106e_PsxInterpreterRun.cs:220</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>int32_t; medium; union member: overlaps union_0_1, union_0_2; reads 2, writes 2; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0106e_PsxInterpreterRun.cs:203 Rewritten/u0106e_PsxInterpreterRun.cs:228 Rewritten/u0106e_PsxInterpreterRun.cs:276</summary>
    [FieldOffset(0)] public int union_0_4;
}

/// <summary>S224: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.BuildSpritePrims(ulong, ulong, ulong, ulong):ot).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Site_ResolveGlobalPsxAddress
{
    /// <summary>uint32_t; medium; reads 1, writes 2; Rewritten/u0079_BuildSpritePrims.cs:67 Rewritten/u0079_BuildSpritePrims.cs:76 Rewritten/u0079_BuildSpritePrims.cs:78</summary>
    [FieldOffset(0)] public uint field_0;
}

/// <summary>S241: size unknown, 38 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitFt4Quad(ushort*, short, short, short, short, int, int, int, int, long, int, int):cursor).</summary>
[StructLayout(LayoutKind.Explicit, Size = 38)]
internal unsafe struct Site_ResolveGlobalPsxAddress2
{
    [FieldOffset(0)] public fixed byte gap_0[4]; // unknown
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:123</summary>
    [FieldOffset(4)] public ushort field_4;
    /// <summary>uint8_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:124</summary>
    [FieldOffset(6)] public byte field_6;
    /// <summary>uint8_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:87</summary>
    [FieldOffset(7)] public byte field_7;
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:97</summary>
    [FieldOffset(8)] public ushort field_8;
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:102</summary>
    [FieldOffset(10)] public ushort field_10;
    /// <summary>uint8_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:105</summary>
    [FieldOffset(12)] public byte field_12;
    /// <summary>uint8_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:114</summary>
    [FieldOffset(13)] public byte field_13;
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:88</summary>
    [FieldOffset(14)] public ushort field_14;
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:96</summary>
    [FieldOffset(16)] public ushort field_16;
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:103</summary>
    [FieldOffset(18)] public ushort field_18;
    /// <summary>uint8_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:112</summary>
    [FieldOffset(20)] public byte field_20;
    /// <summary>uint8_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:113</summary>
    [FieldOffset(21)] public byte field_21;
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:94</summary>
    [FieldOffset(22)] public ushort field_22;
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:100</summary>
    [FieldOffset(24)] public ushort field_24;
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:98</summary>
    [FieldOffset(26)] public ushort field_26;
    /// <summary>uint8_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:104</summary>
    [FieldOffset(28)] public byte field_28;
    /// <summary>uint8_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:122</summary>
    [FieldOffset(29)] public byte field_29;
    [FieldOffset(30)] public fixed byte gap_30[2]; // unknown
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:95</summary>
    [FieldOffset(32)] public ushort field_32;
    /// <summary>uint16_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:99</summary>
    [FieldOffset(34)] public ushort field_34;
    /// <summary>uint8_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:111</summary>
    [FieldOffset(36)] public byte field_36;
    /// <summary>uint8_t; medium; reads 0, writes 1; Rewritten/u0089_EmitSpriteFrame.cs:121</summary>
    [FieldOffset(37)] public byte field_37;
}

/// <summary>S251: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitSpriteQuad(ulong, ulong, ulong, ulong):packet).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Site_ResolveGlobalPsxAddress3
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0085_EmitSpriteQuad.cs:79</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0085_EmitSpriteQuad.cs:78</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S261: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.PsxField(uint, uint):address+offset).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Site_ResolveGlobalPsxAddress4
{
    /// <summary>int8_t; medium; union member: overlaps union_0_2, union_0_4; reads 6, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0074_sub_18001FA80.cs:37 Rewritten/u0074_sub_18001FA80.cs:38 Rewritten/u0074_sub_18001FA80.cs:41</summary>
    [FieldOffset(0)] public sbyte union_0_1;
    /// <summary>int16_t; medium; union member: overlaps union_0_1, union_0_4; reads 9, writes 4; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0074_sub_18001FA80.cs:39 Rewritten/u0074_sub_18001FA80.cs:40 Rewritten/u0074_sub_18001FA80.cs:43</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>int32_t; medium; union member: overlaps union_0_1, union_0_2; reads 13, writes 14; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0074_sub_18001FA80.cs:22 Rewritten/u0074_sub_18001FA80.cs:23 Rewritten/u0074_sub_18001FA80.cs:24</summary>
    [FieldOffset(0)] public int union_0_4;
}

/// <summary>S270: size unknown, 88 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.SetMeshFadeForObjectListBlock():((uint*)off_180066C68)[3]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 88)]
internal unsafe struct Site_ResolveGlobalPsxAddress5
{
    [FieldOffset(0)] public fixed byte gap_0[80]; // unknown
    /// <summary>int64_t; medium; reads 2, writes 0; Rewritten/u0095_SetMotionForObjectList.cs:57 Rewritten/u0095_SetMotionForObjectList.cs:71</summary>
    [FieldOffset(80)] public long field_80;
}

/// <summary>S279: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.SpriteResolveTracked(uint, ref long):address).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Site_ResolveGlobalPsxAddress6
{
    /// <summary>int32_t; medium; reads 2, writes 0; Rewritten/u0072_sub_18001ECB0.cs:31 Rewritten/u0072_sub_18001ECB0.cs:32</summary>
    [FieldOffset(0)] public int field_0;
}

/// <summary>S280: size unknown, 38 bytes = extent (last offset + width); object returned by the address translator Sub1800354E0Ptr (local:Native.Sub1800354E0Finish(Sub1800354E0Frame*, long, long, short*):typeAddress).</summary>
[StructLayout(LayoutKind.Explicit, Size = 38)]
internal unsafe struct Site_Sub1800354E0Ptr
{
    /// <summary>uint8_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:702</summary>
    [FieldOffset(0)] public byte field_0;
    [FieldOffset(1)] public fixed byte gap_1[7]; // unknown
    /// <summary>uint16_t; medium; reads 2, writes 0; Rewritten/u0106g_sub_1800354E0.cs:786 Rewritten/u0106g_sub_1800354E0.cs:843</summary>
    [FieldOffset(8)] public ushort field_8;
    /// <summary>uint16_t; medium; reads 2, writes 0; Rewritten/u0106g_sub_1800354E0.cs:790 Rewritten/u0106g_sub_1800354E0.cs:845</summary>
    [FieldOffset(10)] public ushort field_10;
    /// <summary>int16_t; medium; reads 6, writes 0; Rewritten/u0106g_sub_1800354E0.cs:758 Rewritten/u0106g_sub_1800354E0.cs:786 Rewritten/u0106g_sub_1800354E0.cs:822</summary>
    [FieldOffset(12)] public short field_12;
    /// <summary>int16_t; medium; reads 5, writes 0; Rewritten/u0106g_sub_1800354E0.cs:764 Rewritten/u0106g_sub_1800354E0.cs:787 Rewritten/u0106g_sub_1800354E0.cs:812</summary>
    [FieldOffset(14)] public short field_14;
    /// <summary>int16_t; medium; reads 4, writes 0; Rewritten/u0106g_sub_1800354E0.cs:761 Rewritten/u0106g_sub_1800354E0.cs:781 Rewritten/u0106g_sub_1800354E0.cs:799</summary>
    [FieldOffset(16)] public short field_16;
    /// <summary>int16_t; medium; reads 5, writes 0; Rewritten/u0106g_sub_1800354E0.cs:781 Rewritten/u0106g_sub_1800354E0.cs:800 Rewritten/u0106g_sub_1800354E0.cs:852</summary>
    [FieldOffset(18)] public short field_18;
    /// <summary>int16_t; medium; reads 2, writes 0; Rewritten/u0106g_sub_1800354E0.cs:801 Rewritten/u0106g_sub_1800354E0.cs:802</summary>
    [FieldOffset(20)] public short field_20;
    /// <summary>int16_t; medium; reads 5, writes 0; Rewritten/u0106g_sub_1800354E0.cs:791 Rewritten/u0106g_sub_1800354E0.cs:803 Rewritten/u0106g_sub_1800354E0.cs:847</summary>
    [FieldOffset(22)] public short field_22;
    /// <summary>int16_t; medium; reads 3, writes 0; Rewritten/u0106g_sub_1800354E0.cs:804 Rewritten/u0106g_sub_1800354E0.cs:818 Rewritten/u0106g_sub_1800354E0.cs:870</summary>
    [FieldOffset(24)] public short field_24;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:861</summary>
    [FieldOffset(26)] public short field_26;
    [FieldOffset(28)] public fixed byte gap_28[8]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:778</summary>
    [FieldOffset(36)] public short field_36;
}

/// <summary>S281: size unknown, 8 bytes = extent (last offset + width); object returned by the address translator Sub1800354E0Ptr (local:Native.Sub1800354E0LerpFade(Sub1800354E0Frame*, long, uint, out int):*(uint*)e).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Site_Sub1800354E0Ptr2
{
    [FieldOffset(0)] public fixed byte gap_0[2]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:235</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:236</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:239</summary>
    [FieldOffset(6)] public short field_6;
}

/// <summary>S285: size unknown, 8 bytes = extent (last offset + width); object returned by the address translator Sub1800354E0Ptr (local:Native.Sub1800354E0TemplateColour(Sub1800354E0Frame*, long, long, int, uint, out int):*(uint*)e).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Site_Sub1800354E0Ptr3
{
    [FieldOffset(0)] public fixed byte gap_0[2]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:269</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:270</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:273</summary>
    [FieldOffset(6)] public short field_6;
}

/// <summary>S124: size unknown, 80 bytes = extent (last offset + width); inferred shape H112: 43 access(es) in 1 function(s); size unknown (extent 80); used by SfxTrailSequenceHandler (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 80)]
internal unsafe struct Slots
{
    /// <summary>int32_t; medium; union member: overlaps union_0_8; reads 16, writes 4; union member: shares its bytes with the other union_* fields of the range; fn:SfxTrailSequenceHandler:40 fn:SfxTrailSequenceHandler:70 fn:SfxTrailSequenceHandler:80</summary>
    [FieldOffset(0)] public int union_0_4;
    /// <summary>uint64_t; medium; union member: overlaps union_0_4, union_4_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxTrailSequenceHandler:25</summary>
    [FieldOffset(0)] public ulong union_0_8;
    /// <summary>int32_t; medium; union member: overlaps union_0_8; reads 1, writes 5; union member: shares its bytes with the other union_* fields of the range; fn:SfxTrailSequenceHandler:37 fn:SfxTrailSequenceHandler:66 fn:SfxTrailSequenceHandler:79</summary>
    [FieldOffset(4)] public int union_4_4;
    [FieldOffset(8)] public fixed byte gap_8[32]; // unknown
    /// <summary>uint16_t; medium; reads 1, writes 1; fn:SfxTrailSequenceHandler:96 fn:SfxTrailSequenceHandler:133</summary>
    [FieldOffset(40)] public ushort field_40;
    /// <summary>uint16_t; medium; reads 2, writes 1; fn:SfxTrailSequenceHandler:97 fn:SfxTrailSequenceHandler:134</summary>
    [FieldOffset(42)] public ushort field_42;
    /// <summary>uint16_t; medium; reads 1, writes 0; fn:SfxTrailSequenceHandler:98</summary>
    [FieldOffset(44)] public ushort field_44;
    [FieldOffset(46)] public fixed byte gap_46[2]; // unknown
    /// <summary>uint64_t[]; medium; reads 5, writes 2; indexed with stride 8; 1 elements to the next field; fn:SfxTrailSequenceHandler:44 fn:SfxTrailSequenceHandler:114 fn:SfxTrailSequenceHandler:119</summary>
    [FieldOffset(48)] public ULongArray1 field_48;
    /// <summary>uint64_t; medium; reads 0, writes 1; fn:SfxTrailSequenceHandler:45</summary>
    [FieldOffset(56)] public ulong field_56;
    /// <summary>uint64_t; medium; reads 0, writes 1; fn:SfxTrailSequenceHandler:46</summary>
    [FieldOffset(64)] public ulong field_64;
    /// <summary>uint64_t; medium; reads 0, writes 1; fn:SfxTrailSequenceHandler:47</summary>
    [FieldOffset(72)] public ulong field_72;
}

/// <summary>S277: size unknown, 36 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.SfxUpdateKeyframeTracksSource(byte*, int):*(uint*)track).</summary>
[StructLayout(LayoutKind.Explicit, Size = 36)]
internal unsafe struct Source
{
    /// <summary>uint8_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:287</summary>
    [FieldOffset(0)] public byte field_0;
    [FieldOffset(1)] public fixed byte gap_1[7]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:293</summary>
    [FieldOffset(8)] public short field_8;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:293</summary>
    [FieldOffset(10)] public short field_10;
    [FieldOffset(12)] public fixed byte gap_12[12]; // unknown
    /// <summary>int16_t; medium; reads 2, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:296 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:300</summary>
    [FieldOffset(24)] public short field_24;
    /// <summary>int16_t; medium; reads 2, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:296 Rewritten/u0048_SfxUpdateKeyframeTracks.cs:300</summary>
    [FieldOffset(26)] public short field_26;
    [FieldOffset(28)] public fixed byte gap_28[4]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:297</summary>
    [FieldOffset(32)] public short field_32;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0048_SfxUpdateKeyframeTracks.cs:297</summary>
    [FieldOffset(34)] public short field_34;
}

/// <summary>S282: size unknown, 6 bytes = extent (last offset + width); object returned by the address translator Sub1800354E0Ptr (local:Native.Sub1800354E0Place(Sub1800354E0Frame*, long, short*):*(uint*)(e+40)).</summary>
[StructLayout(LayoutKind.Explicit, Size = 6)]
internal unsafe struct Source2
{
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:512</summary>
    [FieldOffset(0)] public short field_0;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:513</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:514</summary>
    [FieldOffset(4)] public short field_4;
}

/// <summary>S021: 28 bytes; Header of PsxAddSpriteList (sub_18001FA80), in PSX memory.</summary>
[StructLayout(LayoutKind.Explicit, Size = 28)]
internal unsafe struct SpriteListHeader
{
    /// <summary>uint32_t; high; copied to word 1 of every SPRT packet</summary>
    [FieldOffset(0)] public uint colorCode;
    /// <summary>int32_t; high; low 16 bits: x base; record dx * 4 added</summary>
    [FieldOffset(4)] public int x;
    /// <summary>int32_t; high; low 16 bits: y base; record dy * 4 added</summary>
    [FieldOffset(8)] public int y;
    /// <summary>uint32_t; high; OR-ed into the DR_MODE word with the record tpage and 0xE1000200</summary>
    [FieldOffset(12)] public uint modeBits;
    /// <summary>uint32_t; high; shifted &lt;&lt; 16 and OR-ed with the record uv</summary>
    [FieldOffset(16)] public uint clut;
    /// <summary>int32_t; high</summary>
    [FieldOffset(20)] public int count;
    /// <summary>uint32_t; high; PSX address where DR_MODE packets (8 bytes) are appended</summary>
    [FieldOffset(24)] public uint modeCursor;
}

/// <summary>S022: 10 bytes; 10-byte sprite record read by PsxAddSpriteList (sub_18001FA80); used by SfxObjSetFacingHandler (a2), SfxObjAnimWaitHandler (a2), SfxObjFlagsDifferWaitHandler (a2), SfxObjFlagsMatchWaitHandler (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 10)]
internal unsafe struct SpriteRecord
{
    /// <summary>int8_t; high</summary>
    [FieldOffset(0)] public sbyte dx;
    /// <summary>int8_t; high</summary>
    [FieldOffset(1)] public sbyte dy;
    /// <summary>uint8_t; high</summary>
    [FieldOffset(2)] public byte w;
    /// <summary>uint8_t; high</summary>
    [FieldOffset(3)] public byte h;
    /// <summary>uint16_t; high</summary>
    [FieldOffset(4)] public ushort uv;
    [FieldOffset(6)] public fixed byte gap_6[2]; // unknown
    /// <summary>uint16_t; high; a change emits the DR_MODE of the previous group</summary>
    [FieldOffset(8)] public ushort tpage;
}

/// <summary>S268: size unknown, 28 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.PsxSetCallArgs7(long, int, int, long, long, int, long, int):CurrentGprBank(context)[RegSp]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 28)]
internal unsafe struct Stack
{
    [FieldOffset(0)] public fixed byte gap_0[16]; // unknown
    /// <summary>int32_t; medium; reads 0, writes 1; Rewritten/u0060_PsxSetCallArgs7.cs:19</summary>
    [FieldOffset(16)] public int field_16;
    /// <summary>uint32_t; medium; reads 0, writes 1; Rewritten/u0060_PsxSetCallArgs7.cs:21</summary>
    [FieldOffset(20)] public uint field_20;
    /// <summary>int32_t; medium; reads 0, writes 1; Rewritten/u0060_PsxSetCallArgs7.cs:23</summary>
    [FieldOffset(24)] public int field_24;
}

/// <summary>S168: 112 bytes; C# stack buffer desc of DrawScreenAlignedRibbon.</summary>
[StructLayout(LayoutKind.Explicit, Size = 112)]
internal unsafe struct Stack112_DrawScreenAlignedRibbon_desc
{
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0057_CreateRibbonEffectSlot.cs:169</summary>
    [FieldOffset(0)] public int field_0;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0057_CreateRibbonEffectSlot.cs:164</summary>
    [FieldOffset(4)] public int field_4;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0057_CreateRibbonEffectSlot.cs:163</summary>
    [FieldOffset(8)] public int field_8;
    /// <summary>int32_t; high; union member: overlaps union_15_1; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0057_CreateRibbonEffectSlot.cs:128</summary>
    [FieldOffset(12)] public int union_12_4;
    /// <summary>uint8_t; high; union member: overlaps union_12_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0057_CreateRibbonEffectSlot.cs:139</summary>
    [FieldOffset(15)] public byte union_15_1;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0057_CreateRibbonEffectSlot.cs:140</summary>
    [FieldOffset(16)] public int field_16;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0057_CreateRibbonEffectSlot.cs:166</summary>
    [FieldOffset(20)] public short field_20;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0057_CreateRibbonEffectSlot.cs:165</summary>
    [FieldOffset(22)] public short field_22;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0057_CreateRibbonEffectSlot.cs:137</summary>
    [FieldOffset(24)] public int field_24;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0057_CreateRibbonEffectSlot.cs:138</summary>
    [FieldOffset(28)] public int field_28;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0057_CreateRibbonEffectSlot.cs:162</summary>
    [FieldOffset(32)] public int field_32;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0057_CreateRibbonEffectSlot.cs:135</summary>
    [FieldOffset(36)] public short field_36;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0057_CreateRibbonEffectSlot.cs:136</summary>
    [FieldOffset(38)] public short field_38;
    [FieldOffset(40)] public fixed byte gap_40[72]; // unknown
}

/// <summary>S169: 16 bytes; C# stack buffer request of StartPsxTask.</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct Stack16_StartPsxTask_request
{
    /// <summary>int32_t; high; union member: overlaps union_0_8; reads 2, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0005_sub_180048B10.cs:88 Rewritten/u0005_sub_180048B10.cs:90</summary>
    [FieldOffset(0)] public int union_0_4;
    /// <summary>int64_t; high; union member: overlaps union_0_4, union_4_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0005_sub_180048B10.cs:62</summary>
    [FieldOffset(0)] public long union_0_8;
    /// <summary>int32_t; high; union member: overlaps union_0_8; reads 2, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0005_sub_180048B10.cs:70 Rewritten/u0005_sub_180048B10.cs:81</summary>
    [FieldOffset(4)] public int union_4_4;
    /// <summary>int32_t; high; reads 3, writes 1; Rewritten/u0005_sub_180048B10.cs:63 Rewritten/u0005_sub_180048B10.cs:73 Rewritten/u0005_sub_180048B10.cs:74</summary>
    [FieldOffset(8)] public int field_8;
    /// <summary>int32_t; high; reads 1, writes 1; Rewritten/u0005_sub_180048B10.cs:64 Rewritten/u0005_sub_180048B10.cs:83</summary>
    [FieldOffset(12)] public int field_12;
}

/// <summary>S165: 20 bytes; inferred from 1 allocation site(s).</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct Stack20_DrawRingMeshEffect_v74
{
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:DrawRingMeshEffect:203</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>uint32_t; high; union member: overlaps union_0_2, union_2_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:DrawRingMeshEffect:195</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:DrawRingMeshEffect:205</summary>
    [FieldOffset(2)] public short union_2_2;
    /// <summary>int16_t; high; reads 1, writes 2; fn:DrawRingMeshEffect:196 fn:DrawRingMeshEffect:206</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; high; union member: overlaps union_6_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:DrawRingMeshEffect:207</summary>
    [FieldOffset(6)] public short union_6_2;
    /// <summary>uint32_t; high; union member: overlaps union_6_2, union_8_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:DrawRingMeshEffect:194</summary>
    [FieldOffset(6)] public uint union_6_4;
    /// <summary>int16_t; high; union member: overlaps union_6_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:DrawRingMeshEffect:208</summary>
    [FieldOffset(8)] public short union_8_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:DrawRingMeshEffect:209</summary>
    [FieldOffset(10)] public short union_10_2;
    /// <summary>uint64_t; high; union member: overlaps union_10_2, union_12_2, union_14_2, union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:DrawRingMeshEffect:197</summary>
    [FieldOffset(10)] public ulong union_10_8;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:DrawRingMeshEffect:210</summary>
    [FieldOffset(12)] public short union_12_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:DrawRingMeshEffect:211</summary>
    [FieldOffset(14)] public short union_14_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 3, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:DrawRingMeshEffect:213 fn:DrawRingMeshEffect:226 fn:DrawRingMeshEffect:232</summary>
    [FieldOffset(16)] public short union_16_2;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
}

/// <summary>S197: 20 bytes; inferred from 1 allocation site(s).</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct Stack20_EmitScaledSpriteFrame_v51
{
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitScaledSpriteFrame:164</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>uint32_t; high; union member: overlaps union_0_2, union_2_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitScaledSpriteFrame:157</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitScaledSpriteFrame:165</summary>
    [FieldOffset(2)] public short union_2_2;
    /// <summary>int16_t; high; reads 1, writes 2; fn:EmitScaledSpriteFrame:158 fn:EmitScaledSpriteFrame:166</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; high; union member: overlaps union_6_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitScaledSpriteFrame:167</summary>
    [FieldOffset(6)] public short union_6_2;
    /// <summary>uint32_t; high; union member: overlaps union_6_2, union_8_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitScaledSpriteFrame:156</summary>
    [FieldOffset(6)] public uint union_6_4;
    /// <summary>int16_t; high; union member: overlaps union_6_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitScaledSpriteFrame:168</summary>
    [FieldOffset(8)] public short union_8_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitScaledSpriteFrame:169</summary>
    [FieldOffset(10)] public short union_10_2;
    /// <summary>uint64_t; high; union member: overlaps union_10_2, union_12_2, union_14_2, union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitScaledSpriteFrame:159</summary>
    [FieldOffset(10)] public ulong union_10_8;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitScaledSpriteFrame:171</summary>
    [FieldOffset(12)] public short union_12_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitScaledSpriteFrame:174</summary>
    [FieldOffset(14)] public short union_14_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 2, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitScaledSpriteFrame:176 fn:EmitScaledSpriteFrame:191</summary>
    [FieldOffset(16)] public short union_16_2;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
}

/// <summary>S195: 20 bytes; inferred from 1 allocation site(s).</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct Stack20_EmitSpriteAnimFrame_v47
{
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteAnimFrame:169</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>uint32_t; high; union member: overlaps union_0_2, union_2_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteAnimFrame:161</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteAnimFrame:170</summary>
    [FieldOffset(2)] public short union_2_2;
    /// <summary>int16_t; high; reads 1, writes 2; fn:EmitSpriteAnimFrame:162 fn:EmitSpriteAnimFrame:171</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; high; union member: overlaps union_6_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteAnimFrame:172</summary>
    [FieldOffset(6)] public short union_6_2;
    /// <summary>uint32_t; high; union member: overlaps union_6_2, union_8_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteAnimFrame:160</summary>
    [FieldOffset(6)] public uint union_6_4;
    /// <summary>int16_t; high; union member: overlaps union_6_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteAnimFrame:173</summary>
    [FieldOffset(8)] public short union_8_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteAnimFrame:174</summary>
    [FieldOffset(10)] public short union_10_2;
    /// <summary>uint64_t; high; union member: overlaps union_10_2, union_12_2, union_14_2, union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteAnimFrame:163</summary>
    [FieldOffset(10)] public ulong union_10_8;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteAnimFrame:175</summary>
    [FieldOffset(12)] public short union_12_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteAnimFrame:176</summary>
    [FieldOffset(14)] public short union_14_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 2, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteAnimFrame:177 fn:EmitSpriteAnimFrame:195</summary>
    [FieldOffset(16)] public short union_16_2;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
}

/// <summary>S194: 20 bytes; inferred from 1 allocation site(s).</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct Stack20_EmitSpriteCellFrame_v50
{
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteCellFrame:207</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>uint32_t; high; union member: overlaps union_0_2, union_2_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteCellFrame:199</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteCellFrame:209</summary>
    [FieldOffset(2)] public short union_2_2;
    /// <summary>int16_t; high; reads 1, writes 2; fn:EmitSpriteCellFrame:200 fn:EmitSpriteCellFrame:210</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; high; union member: overlaps union_6_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteCellFrame:211</summary>
    [FieldOffset(6)] public short union_6_2;
    /// <summary>uint32_t; high; union member: overlaps union_6_2, union_8_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteCellFrame:198</summary>
    [FieldOffset(6)] public uint union_6_4;
    /// <summary>int16_t; high; union member: overlaps union_6_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteCellFrame:212</summary>
    [FieldOffset(8)] public short union_8_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteCellFrame:213</summary>
    [FieldOffset(10)] public short union_10_2;
    /// <summary>uint64_t; high; union member: overlaps union_10_2, union_12_2, union_14_2, union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteCellFrame:201</summary>
    [FieldOffset(10)] public ulong union_10_8;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteCellFrame:214</summary>
    [FieldOffset(12)] public short union_12_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteCellFrame:215</summary>
    [FieldOffset(14)] public short union_14_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 2, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitSpriteCellFrame:217 fn:EmitSpriteCellFrame:231</summary>
    [FieldOffset(16)] public short union_16_2;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
}

/// <summary>S196: 20 bytes; inferred from 1 allocation site(s).</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct Stack20_EmitWorldSpriteFrame_v54
{
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitWorldSpriteFrame:195</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>uint32_t; high; union member: overlaps union_0_2, union_2_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitWorldSpriteFrame:187</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitWorldSpriteFrame:196</summary>
    [FieldOffset(2)] public short union_2_2;
    /// <summary>int16_t; high; reads 1, writes 2; fn:EmitWorldSpriteFrame:188 fn:EmitWorldSpriteFrame:197</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>int16_t; high; union member: overlaps union_6_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitWorldSpriteFrame:198</summary>
    [FieldOffset(6)] public short union_6_2;
    /// <summary>uint32_t; high; union member: overlaps union_6_2, union_8_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitWorldSpriteFrame:186</summary>
    [FieldOffset(6)] public uint union_6_4;
    /// <summary>int16_t; high; union member: overlaps union_6_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitWorldSpriteFrame:199</summary>
    [FieldOffset(8)] public short union_8_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitWorldSpriteFrame:200</summary>
    [FieldOffset(10)] public short union_10_2;
    /// <summary>uint64_t; high; union member: overlaps union_10_2, union_12_2, union_14_2, union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitWorldSpriteFrame:189</summary>
    [FieldOffset(10)] public ulong union_10_8;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitWorldSpriteFrame:201</summary>
    [FieldOffset(12)] public short union_12_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitWorldSpriteFrame:202</summary>
    [FieldOffset(14)] public short union_14_2;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 2, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:EmitWorldSpriteFrame:203 fn:EmitWorldSpriteFrame:222</summary>
    [FieldOffset(16)] public short union_16_2;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
}

/// <summary>S191: 20 bytes; inferred from 1 allocation site(s).</summary>
[StructLayout(LayoutKind.Explicit, Size = 20)]
internal unsafe struct Stack20_TransformPointByAngles_v22
{
    /// <summary>uint32_t; high; reads 0, writes 1; fn:TransformPointByAngles:25</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>int16_t; high; reads 0, writes 1; fn:TransformPointByAngles:26</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>uint32_t; high; reads 0, writes 1; fn:TransformPointByAngles:24</summary>
    [FieldOffset(6)] public uint field_6;
    /// <summary>uint64_t; high; union member: overlaps union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:TransformPointByAngles:27</summary>
    [FieldOffset(10)] public ulong union_10_8;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:TransformPointByAngles:31</summary>
    [FieldOffset(16)] public short union_16_2;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
}

/// <summary>S193: 32 bytes; inferred from 1 allocation site(s).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Stack32_ComputeLocalYawDistance_v24
{
    /// <summary>uint32_t; high; reads 0, writes 1; fn:ComputeLocalYawDistance:27</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>int16_t; high; reads 0, writes 1; fn:ComputeLocalYawDistance:30</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>uint32_t; high; reads 0, writes 1; fn:ComputeLocalYawDistance:26</summary>
    [FieldOffset(6)] public uint field_6;
    /// <summary>uint64_t; high; union member: overlaps union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:ComputeLocalYawDistance:33</summary>
    [FieldOffset(10)] public ulong union_10_8;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:ComputeLocalYawDistance:40</summary>
    [FieldOffset(16)] public short union_16_2;
    [FieldOffset(18)] public fixed byte gap_18[14]; // unknown
}

/// <summary>S170: 32 bytes; C# stack buffer scale of DrawEffModelByBone.</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Stack32_DrawEffModelByBone_scale
{
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0030_DrawEffModelByBone.cs:63</summary>
    [FieldOffset(0)] public short field_0;
    [FieldOffset(2)] public fixed byte gap_2[6]; // unknown
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0030_DrawEffModelByBone.cs:64</summary>
    [FieldOffset(8)] public short field_8;
    [FieldOffset(10)] public fixed byte gap_10[6]; // unknown
    /// <summary>int16_t; high; union member: overlaps union_16_8; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0030_DrawEffModelByBone.cs:66</summary>
    [FieldOffset(16)] public short union_16_2;
    /// <summary>int64_t; high; union member: overlaps union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0030_DrawEffModelByBone.cs:65</summary>
    [FieldOffset(16)] public long union_16_8;
    /// <summary>int64_t; high; reads 0, writes 1; Rewritten/u0030_DrawEffModelByBone.cs:67</summary>
    [FieldOffset(24)] public long field_24;
}

/// <summary>S172: 32 bytes; C# stack buffer m of EmitSpriteAnimFrame.</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Stack32_EmitSpriteAnimFrame_m
{
    /// <summary>uint32_t; high; reads 0, writes 1; Rewritten/u0077_EmitSpriteAnimFrame.cs:76</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0077_EmitSpriteAnimFrame.cs:77</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>uint32_t; high; reads 0, writes 1; Rewritten/u0077_EmitSpriteAnimFrame.cs:75</summary>
    [FieldOffset(6)] public uint field_6;
    /// <summary>uint64_t; high; union member: overlaps union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0077_EmitSpriteAnimFrame.cs:78</summary>
    [FieldOffset(10)] public ulong union_10_8;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0077_EmitSpriteAnimFrame.cs:107</summary>
    [FieldOffset(16)] public short union_16_2;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0077_EmitSpriteAnimFrame.cs:93</summary>
    [FieldOffset(20)] public int field_20;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0077_EmitSpriteAnimFrame.cs:94</summary>
    [FieldOffset(24)] public int field_24;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0077_EmitSpriteAnimFrame.cs:95</summary>
    [FieldOffset(28)] public int field_28;
}

/// <summary>S173: 32 bytes; C# stack buffer rt of EmitSpriteAnimFrame.</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Stack32_EmitSpriteAnimFrame_rt
{
    [FieldOffset(0)] public fixed byte gap_0[16]; // unknown
    /// <summary>int64_t; high; reads 0, writes 1; Rewritten/u0077_EmitSpriteAnimFrame.cs:101</summary>
    [FieldOffset(16)] public long field_16;
    /// <summary>uint32_t; high; reads 0, writes 1; Rewritten/u0077_EmitSpriteAnimFrame.cs:102</summary>
    [FieldOffset(24)] public uint field_24;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0077_EmitSpriteAnimFrame.cs:103</summary>
    [FieldOffset(28)] public int field_28;
}

/// <summary>S176: 32 bytes; C# stack buffer matrix of GetObjectHostPosition.</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Stack32_GetObjectHostPosition_matrix
{
    [FieldOffset(0)] public fixed byte gap_0[16]; // unknown
    /// <summary>uint16_t; high; reads 1, writes 0; Rewritten/u0094_HostSetObjectMotion.cs:32</summary>
    [FieldOffset(16)] public ushort field_16;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
    /// <summary>int64_t; high; reads 1, writes 0; Rewritten/u0094_HostSetObjectMotion.cs:34</summary>
    [FieldOffset(20)] public long field_20;
    /// <summary>int32_t; high; reads 1, writes 0; Rewritten/u0094_HostSetObjectMotion.cs:35</summary>
    [FieldOffset(28)] public int field_28;
}

/// <summary>S190: 32 bytes; inferred from 1 allocation site(s).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Stack32_RotateDepthVector_v17
{
    /// <summary>uint32_t; high; reads 0, writes 1; fn:RotateDepthVector:20</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>int16_t; high; reads 0, writes 1; fn:RotateDepthVector:21</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>uint32_t; high; reads 0, writes 1; fn:RotateDepthVector:19</summary>
    [FieldOffset(6)] public uint field_6;
    /// <summary>uint64_t; high; union member: overlaps union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:RotateDepthVector:22</summary>
    [FieldOffset(10)] public ulong union_10_8;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:RotateDepthVector:28</summary>
    [FieldOffset(16)] public short union_16_2;
    [FieldOffset(18)] public fixed byte gap_18[14]; // unknown
}

/// <summary>S192: 32 bytes; inferred from 1 allocation site(s).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Stack32_RotationToLookAngles_v21
{
    /// <summary>uint32_t; high; reads 0, writes 1; fn:RotationToLookAngles:26</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>int16_t; high; reads 0, writes 1; fn:RotationToLookAngles:27</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>uint32_t; high; reads 0, writes 1; fn:RotationToLookAngles:25</summary>
    [FieldOffset(6)] public uint field_6;
    /// <summary>uint64_t; high; union member: overlaps union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:RotationToLookAngles:28</summary>
    [FieldOffset(10)] public ulong union_10_8;
    /// <summary>int16_t; high; union member: overlaps union_10_8; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:RotationToLookAngles:34</summary>
    [FieldOffset(16)] public short union_16_2;
    [FieldOffset(18)] public fixed byte gap_18[14]; // unknown
}

/// <summary>S178: 32 bytes; C# stack buffer m of SFX_MoveFreeCamera_0.</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Stack32_SFX_MoveFreeCamera_0_m
{
    /// <summary>uint32_t; high; reads 0, writes 1; Rewritten/u0114_SFX_MoveFreeCamera_0.cs:45</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0114_SFX_MoveFreeCamera_0.cs:46</summary>
    [FieldOffset(4)] public short field_4;
    /// <summary>uint64_t; high; reads 0, writes 1; Rewritten/u0114_SFX_MoveFreeCamera_0.cs:47</summary>
    [FieldOffset(6)] public ulong field_6;
    /// <summary>uint32_t; high; union member: overlaps union_16_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0114_SFX_MoveFreeCamera_0.cs:48</summary>
    [FieldOffset(14)] public uint union_14_4;
    /// <summary>int16_t; high; union member: overlaps union_14_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0114_SFX_MoveFreeCamera_0.cs:54</summary>
    [FieldOffset(16)] public short union_16_2;
    [FieldOffset(18)] public fixed byte gap_18[2]; // unknown
    /// <summary>int32_t; high; reads 1, writes 0; Rewritten/u0114_SFX_MoveFreeCamera_0.cs:78</summary>
    [FieldOffset(20)] public int field_20;
    /// <summary>int64_t; high; reads 1, writes 0; Rewritten/u0114_SFX_MoveFreeCamera_0.cs:79</summary>
    [FieldOffset(24)] public long field_24;
}

/// <summary>S180: 44 bytes; C# stack buffer desc of EmitDepthCuedRibbon.</summary>
[StructLayout(LayoutKind.Explicit, Size = 44)]
internal unsafe struct Stack44_EmitDepthCuedRibbon_desc
{
    /// <summary>uint32_t; high; reads 0, writes 1; Rewritten/u0055_EmitDepthCuedRibbon.cs:96</summary>
    [FieldOffset(0)] public uint field_0;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0055_EmitDepthCuedRibbon.cs:95</summary>
    [FieldOffset(4)] public int field_4;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0055_EmitDepthCuedRibbon.cs:94</summary>
    [FieldOffset(8)] public int field_8;
    /// <summary>int32_t; high; union member: overlaps union_15_1; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0055_EmitDepthCuedRibbon.cs:70</summary>
    [FieldOffset(12)] public int union_12_4;
    /// <summary>uint8_t; high; union member: overlaps union_12_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0055_EmitDepthCuedRibbon.cs:72</summary>
    [FieldOffset(15)] public byte union_15_1;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0055_EmitDepthCuedRibbon.cs:71</summary>
    [FieldOffset(16)] public int field_16;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0055_EmitDepthCuedRibbon.cs:59</summary>
    [FieldOffset(20)] public short field_20;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0055_EmitDepthCuedRibbon.cs:60</summary>
    [FieldOffset(22)] public short field_22;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0055_EmitDepthCuedRibbon.cs:57</summary>
    [FieldOffset(24)] public int field_24;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0055_EmitDepthCuedRibbon.cs:58</summary>
    [FieldOffset(28)] public int field_28;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0055_EmitDepthCuedRibbon.cs:93</summary>
    [FieldOffset(32)] public int field_32;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0055_EmitDepthCuedRibbon.cs:44</summary>
    [FieldOffset(36)] public short field_36;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0055_EmitDepthCuedRibbon.cs:45</summary>
    [FieldOffset(38)] public short field_38;
    /// <summary>int32_t; high; reads 0, writes 2; Rewritten/u0055_EmitDepthCuedRibbon.cs:54 Rewritten/u0055_EmitDepthCuedRibbon.cs:66</summary>
    [FieldOffset(40)] public int field_40;
}

/// <summary>S181: 8 bytes; C# stack buffer vector of CamLookAtMatrix; used by SfxUploadTexturePages (a1).</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Stack8_CamLookAtMatrix_vector
{
    /// <summary>uint16_t; high; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0112_CamLookAtMatrix.cs:249</summary>
    [FieldOffset(0)] public ushort union_0_2;
    /// <summary>int32_t; high; union member: overlaps union_0_2, union_2_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0112_CamLookAtMatrix.cs:253</summary>
    [FieldOffset(0)] public int union_0_4;
    /// <summary>uint16_t; high; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0112_CamLookAtMatrix.cs:250</summary>
    [FieldOffset(2)] public ushort union_2_2;
    /// <summary>uint16_t; high; union member: overlaps union_4_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0112_CamLookAtMatrix.cs:251</summary>
    [FieldOffset(4)] public ushort union_4_2;
    /// <summary>int32_t; high; union member: overlaps union_4_2, union_6_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0112_CamLookAtMatrix.cs:254</summary>
    [FieldOffset(4)] public int union_4_4;
    /// <summary>uint16_t; high; union member: overlaps union_4_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0112_CamLookAtMatrix.cs:252</summary>
    [FieldOffset(6)] public ushort union_6_2;
}

/// <summary>S182: 8 bytes; C# stack buffer moved of SfxObjStepToFacingPoint.</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Stack8_SfxObjStepToFacingPoint_moved
{
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:64</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>int32_t; high; union member: overlaps union_0_2, union_2_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:23</summary>
    [FieldOffset(0)] public int union_0_4;
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:65</summary>
    [FieldOffset(2)] public short union_2_2;
    /// <summary>int16_t; high; union member: overlaps union_4_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:66</summary>
    [FieldOffset(4)] public short union_4_2;
    /// <summary>int32_t; high; union member: overlaps union_4_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:24</summary>
    [FieldOffset(4)] public int union_4_4;
}

/// <summary>S184: 8 bytes; C# stack buffer anchor of SfxObjStepToPoint.</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Stack8_SfxObjStepToPoint_anchor
{
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 2, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:160 Rewritten/u0118_SfxObjStepToFacingPoint.cs:203</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>int32_t; high; union member: overlaps union_0_2, union_2_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:172</summary>
    [FieldOffset(0)] public int union_0_4;
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 2, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:199 Rewritten/u0118_SfxObjStepToFacingPoint.cs:203</summary>
    [FieldOffset(2)] public short union_2_2;
    /// <summary>int16_t; high; reads 3, writes 0; Rewritten/u0118_SfxObjStepToFacingPoint.cs:160 Rewritten/u0118_SfxObjStepToFacingPoint.cs:172 Rewritten/u0118_SfxObjStepToFacingPoint.cs:203</summary>
    [FieldOffset(4)] public short field_4;
    [FieldOffset(6)] public fixed byte gap_6[2]; // unknown
}

/// <summary>S186: 8 bytes; C# stack buffer position of SfxObjStepToPoint.</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Stack8_SfxObjStepToPoint_position
{
    /// <summary>int32_t; high; reads 1, writes 0; Rewritten/u0118_SfxObjStepToFacingPoint.cs:172</summary>
    [FieldOffset(0)] public int field_0;
    /// <summary>int16_t; high; reads 1, writes 0; Rewritten/u0118_SfxObjStepToFacingPoint.cs:172</summary>
    [FieldOffset(4)] public short field_4;
    [FieldOffset(6)] public fixed byte gap_6[2]; // unknown
}

/// <summary>S187: 8 bytes; C# stack buffer goal of SfxObjTurnTowardsPoint.</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct Stack8_SfxObjTurnTowardsPoint_goal
{
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:115</summary>
    [FieldOffset(0)] public short union_0_2;
    /// <summary>int32_t; high; union member: overlaps union_0_2, union_2_2; reads 0, writes 2; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:107 Rewritten/u0118_SfxObjStepToFacingPoint.cs:126</summary>
    [FieldOffset(0)] public int union_0_4;
    /// <summary>int16_t; high; union member: overlaps union_0_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:120</summary>
    [FieldOffset(2)] public short union_2_2;
    /// <summary>int16_t; high; union member: overlaps union_4_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:129</summary>
    [FieldOffset(4)] public short union_4_2;
    /// <summary>uint32_t; high; union member: overlaps union_4_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0118_SfxObjStepToFacingPoint.cs:108</summary>
    [FieldOffset(4)] public uint union_4_4;
}

/// <summary>S189: 96 bytes; C# stack buffer desc of DrawFacingRibbon; used by SfxObjectStrikeSequence (a3).</summary>
[StructLayout(LayoutKind.Explicit, Size = 96)]
internal unsafe struct Stack96_DrawFacingRibbon_desc
{
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:86</summary>
    [FieldOffset(0)] public int field_0;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:81</summary>
    [FieldOffset(4)] public int field_4;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:68</summary>
    [FieldOffset(8)] public int field_8;
    /// <summary>int32_t; high; union member: overlaps union_15_1; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0052_DrawFacingRibbon.cs:33</summary>
    [FieldOffset(12)] public int union_12_4;
    /// <summary>uint8_t; high; union member: overlaps union_12_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0052_DrawFacingRibbon.cs:45</summary>
    [FieldOffset(15)] public byte union_15_1;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:42</summary>
    [FieldOffset(16)] public int field_16;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:82</summary>
    [FieldOffset(20)] public short field_20;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:83</summary>
    [FieldOffset(22)] public short field_22;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:43</summary>
    [FieldOffset(24)] public int field_24;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:44</summary>
    [FieldOffset(28)] public int field_28;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:67</summary>
    [FieldOffset(32)] public int field_32;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:40</summary>
    [FieldOffset(36)] public short field_36;
    /// <summary>int16_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:41</summary>
    [FieldOffset(38)] public short field_38;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:79</summary>
    [FieldOffset(40)] public int field_40;
    /// <summary>int32_t; high; reads 0, writes 1; Rewritten/u0052_DrawFacingRibbon.cs:80</summary>
    [FieldOffset(44)] public int field_44;
    [FieldOffset(48)] public fixed byte gap_48[48]; // unknown
}

/// <summary>S024: size unknown, 420 bytes = extent (last offset + width); inferred shape H1: 1449 access(es) in 186 function(s); size unknown (extent 420); used by SfxObjectEffectHandler (a2).</summary>
[StructLayout(LayoutKind.Explicit, Size = 420)]
internal unsafe struct State2
{
    /// <summary>uint8_t; medium; reads 6, writes 1; fn:SfxObjectEffectHandler:54 fn:SfxObjectEffectHandler:71 fn:SfxObjectEffectHandler:73</summary>
    [FieldOffset(0)] public byte field_0;
    /// <summary>uint8_t; medium; reads 5, writes 1; fn:SfxObjectEffectHandler:57 fn:SfxObjectEffectHandler:120 fn:SfxObjectEffectHandler:125</summary>
    [FieldOffset(1)] public byte field_1;
    /// <summary>int16_t; medium; reads 2, writes 1; fn:SfxObjectEffectHandler:53 fn:SfxObjectEffectHandler:123 fn:SfxObjectEffectHandler:214</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>uint8_t; medium; union member: overlaps union_4_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectEffectHandler:220</summary>
    [FieldOffset(4)] public byte union_4_1;
    /// <summary>int16_t; medium; union member: overlaps union_4_1; reads 3, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectEffectHandler:55 fn:SfxObjectEffectHandler:90 fn:SfxObjectEffectHandler:139</summary>
    [FieldOffset(4)] public short union_4_2;
    /// <summary>uint16_t; medium; reads 1, writes 3; fn:SfxObjectEffectHandler:79 fn:SfxObjectEffectHandler:98 fn:SfxObjectEffectHandler:108</summary>
    [FieldOffset(6)] public ushort field_6;
    /// <summary>int16_t; medium; reads 1, writes 4; fn:SfxObjectEffectHandler:78 fn:SfxObjectEffectHandler:88 fn:SfxObjectEffectHandler:95</summary>
    [FieldOffset(8)] public short field_8;
    /// <summary>int16_t; medium; reads 2, writes 1; fn:SfxObjectEffectHandler:89 fn:SfxObjectEffectHandler:198 fn:SfxObjectEffectHandler:210</summary>
    [FieldOffset(10)] public short field_10;
    /// <summary>uint8_t; medium; union member: overlaps union_12_2; reads 18, writes 4; union member: shares its bytes with the other union_* fields of the range; fn:SfxOpMoveCtx16ToObj:16 fn:SfxOpCtx16Advance:12 fn:SfxOpCtx16BlendNearObj:13</summary>
    [FieldOffset(12)] public byte union_12_1;
    /// <summary>uint16_t; medium; union member: overlaps union_12_1, union_13_1; reads 0, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectEffectHandler:77 fn:SfxObjectEffectHandler:106</summary>
    [FieldOffset(12)] public ushort union_12_2;
    /// <summary>uint8_t; medium; union member: overlaps union_12_2; reads 6, writes 4; union member: shares its bytes with the other union_* fields of the range; fn:SfxOpMoveCtx16ToObj:18 fn:SfxOpMoveCtx16ToObj:32 fn:SfxOpCtx16Advance:14</summary>
    [FieldOffset(13)] public byte union_13_1;
    /// <summary>uint8_t; medium; union member: overlaps union_14_2; reads 1, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:SfxOpCtx16Advance:17 fn:SfxOpCtx16BlendNearObj:19 fn:SfxOpCtx16BlendNearObjStep:60</summary>
    [FieldOffset(14)] public byte union_14_1;
    /// <summary>int16_t; medium; union member: overlaps union_14_1, union_15_1; reads 6, writes 4; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectEffectHandler:84 fn:SfxObjectEffectHandler:91 fn:SfxObjectEffectHandler:96</summary>
    [FieldOffset(14)] public short union_14_2;
    /// <summary>uint8_t; medium; union member: overlaps union_14_2; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxOpCtx16Advance:16 fn:SfxOpCtx16AdvanceStep:35</summary>
    [FieldOffset(15)] public byte union_15_1;
    /// <summary>uint8_t[8]; unknown; reads 80, writes 42; conflict: 1:uint8_t, 1:uint8_t@17, 1:uint8_t@18, 1:uint8_t@19, 1:uint8_t@20, 1:uint8_t@21, 1:uint8_t@22, 1:uint8_t@23, 2:int16_t, 2:int16_t@18, 2:uint16_t, 2:uint16_t@18, 2:uint16_t@22, 4:uint32_t@20, 8:uint16_t*, 8:uint64_t; possibly a union; fn:SfxOpMoveObjToObj:14 fn:SfxOpMoveObjToObj:16 fn:SfxOpMoveCtx16ToObj:20</summary>
    [FieldOffset(16)] public ByteArray8 raw_16;
    /// <summary>uint8_t; medium; union member: overlaps union_24_4; reads 2, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectStrikeSequence:132 fn:SfxObjectStrikeSequence:367 fn:SfxObjectStrikeSequence:369</summary>
    [FieldOffset(24)] public byte union_24_1;
    /// <summary>uint32_t; medium; union member: overlaps union_24_1, union_25_1, union_26_2; reads 9, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectEffectHandler:52 fn:SfxObjectEffectHandler:140 fn:SfxObjectEffectHandler:143</summary>
    [FieldOffset(24)] public uint union_24_4;
    /// <summary>uint8_t; medium; union member: overlaps union_24_4; reads 6, writes 4; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectStrikeSequence:155 fn:SfxObjectStrikeSequence:164 fn:SfxObjectStrikeSequence:169</summary>
    [FieldOffset(25)] public byte union_25_1;
    /// <summary>uint16_t; medium; union member: overlaps union_24_4; reads 4, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectStrikeSequence:72 fn:SfxObjectStrikeSequence:430 fn:SfxObjectStrikeSequence:446</summary>
    [FieldOffset(26)] public ushort union_26_2;
    /// <summary>int16_t; medium; union member: overlaps union_28_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectEffectHandler:115 fn:SfxObjectEffectHandler:249</summary>
    [FieldOffset(28)] public short union_28_2;
    /// <summary>int32_t; medium; union member: overlaps union_28_2, union_30_2; reads 2, writes 4; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectStrikeSequence:147 fn:SfxObjectStrikeSequence:363 fn:SfxObjectStrikeSequence:430</summary>
    [FieldOffset(28)] public int union_28_4;
    /// <summary>int16_t; medium; union member: overlaps union_28_4; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectEffectHandler:116 fn:SfxObjectEffectHandler:250</summary>
    [FieldOffset(30)] public short union_30_2;
    /// <summary>HostObject; medium; holders bound to HostObject point at +32</summary>
    [FieldOffset(32)] public HostObject hostObject;
    /// <summary>uint8_t; medium; union member: overlaps union_96_2, union_96_4; reads 8, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SeqStateUpdate:85 fn:SeqStateUpdate:311 fn:SfxObjectStrikeSequence:162</summary>
    [FieldOffset(96)] public byte union_96_1;
    /// <summary>int16_t; medium; union member: overlaps union_96_1, union_96_4; reads 4, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:SeqStateUpdate:71 fn:SeqStateUpdate:96 fn:SfxUpdateKeyframeTracks:411</summary>
    [FieldOffset(96)] public short union_96_2;
    /// <summary>uint32_t; medium; union member: overlaps union_96_1, union_96_2, union_98_2; reads 3, writes 2; union member: shares its bytes with the other union_* fields of the range; fn:RegisterSummonModel:98 fn:RegisterSummonModel:109 fn:SfxLoaderStep:609</summary>
    [FieldOffset(96)] public uint union_96_4;
    /// <summary>int16_t; medium; union member: overlaps union_96_4; reads 2, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxUpdateKeyframeTracks:414 fn:SfxUpdateKeyframeTracks:531</summary>
    [FieldOffset(98)] public short union_98_2;
    /// <summary>int16_t; medium; union member: overlaps union_100_4; reads 2, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxUpdateKeyframeTracks:412 fn:SfxUpdateKeyframeTracks:547</summary>
    [FieldOffset(100)] public short union_100_2;
    /// <summary>uint32_t; medium; union member: overlaps union_100_2, union_102_2; reads 1, writes 6; union member: shares its bytes with the other union_* fields of the range; fn:SfxTrailSequenceHandler:130 fn:SfxSpawnTrailEffect:43 fn:SfxSpawnTrailEffect:70</summary>
    [FieldOffset(100)] public uint union_100_4;
    /// <summary>int16_t; medium; union member: overlaps union_100_4; reads 2, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxUpdateKeyframeTracks:415 fn:SfxUpdateKeyframeTracks:544</summary>
    [FieldOffset(102)] public short union_102_2;
    /// <summary>int16_t; medium; union member: overlaps union_104_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxUpdateKeyframeTracks:545</summary>
    [FieldOffset(104)] public short union_104_2;
    /// <summary>uint32_t; medium; union member: overlaps union_104_2, union_106_2; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxLoaderStep:354 fn:SfxLoaderStep:626</summary>
    [FieldOffset(104)] public uint union_104_4;
    /// <summary>int16_t; medium; union member: overlaps union_104_4; reads 3, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxUpdateParticles:318 fn:SfxUpdateParticles:537 fn:SfxUpdateKeyframeTracks:546</summary>
    [FieldOffset(106)] public short union_106_2;
    /// <summary>int16_t; medium; union member: overlaps union_108_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxUpdateParticles:369</summary>
    [FieldOffset(108)] public short union_108_2;
    /// <summary>uint32_t; medium; union member: overlaps union_108_2, union_110_2; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxLoaderStep:334</summary>
    [FieldOffset(108)] public uint union_108_4;
    /// <summary>int16_t; medium; union member: overlaps union_108_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxUpdateParticles:399</summary>
    [FieldOffset(110)] public short union_110_2;
    /// <summary>uint16_t; medium; union member: overlaps union_112_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SeqStateUpdate:267</summary>
    [FieldOffset(112)] public ushort union_112_2;
    /// <summary>uint32_t; medium; union member: overlaps union_112_2, union_114_1, union_115_1; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxLoaderStep:356</summary>
    [FieldOffset(112)] public uint union_112_4;
    /// <summary>int8_t; medium; union member: overlaps union_112_4; reads 2, writes 4; union member: shares its bytes with the other union_* fields of the range; fn:SFX_SendIntData_0:68 fn:SFX_SendIntData_0:71 fn:SeqStateUpdate:259</summary>
    [FieldOffset(114)] public sbyte union_114_1;
    /// <summary>int8_t; medium; union member: overlaps union_112_4; reads 2, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SeqStateUpdate:262 fn:SfxOpStartTrackBlock:42</summary>
    [FieldOffset(115)] public sbyte union_115_1;
    [FieldOffset(116)] public fixed byte gap_116[2]; // unknown
    /// <summary>int16_t; medium; reads 1, writes 0; fn:SfxUpdateParticles:253</summary>
    [FieldOffset(118)] public short field_118;
    /// <summary>uint8_t[8]; unknown; reads 6, writes 3; conflict: 1:uint8_t@124, 2:int16_t, 2:int16_t@122, 2:uint16_t@124, 2:uint16_t@126, 4:uint32_t@124, 8:uint64_t[8]; possibly a union; fn:SeqStateUpdate:116 fn:SfxUpdateParticles:286 fn:SfxUpdateParticles:290</summary>
    [FieldOffset(120)] public ByteArray8 raw_120;
    /// <summary>uint16_t; medium; union member: overlaps union_128_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxObjectEffectHandler:233</summary>
    [FieldOffset(128)] public ushort union_128_2;
    /// <summary>uint32_t; medium; union member: overlaps union_128_2, union_130_2; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxEmitParticles:534</summary>
    [FieldOffset(128)] public uint union_128_4;
    /// <summary>int16_t; medium; union member: overlaps union_128_4; reads 5, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SfxUpdateParticles:512 fn:SfxUpdateParticles:527 fn:SfxUpdateParticles:534</summary>
    [FieldOffset(130)] public short union_130_2;
    /// <summary>uint32_t; medium; reads 2, writes 1; fn:SfxUpdateParticles:695 fn:SfxUpdateParticles:717 fn:SfxUpdateParticles:754</summary>
    [FieldOffset(132)] public uint field_132;
    /// <summary>uint8_t; medium; union member: overlaps union_136_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SeqStateUpdate:340</summary>
    [FieldOffset(136)] public byte union_136_1;
    /// <summary>uint32_t; medium; union member: overlaps union_136_1, union_137_1; reads 2, writes 1; union member: shares its bytes with the other union_* fields of the range; fn:SfxUpdateParticles:722 fn:SfxUpdateParticles:744 fn:SfxUpdateParticles:761</summary>
    [FieldOffset(136)] public uint union_136_4;
    /// <summary>uint8_t; medium; union member: overlaps union_136_4; reads 1, writes 0; union member: shares its bytes with the other union_* fields of the range; fn:SeqStateUpdate:302</summary>
    [FieldOffset(137)] public byte union_137_1;
    /// <summary>uint32_t; medium; reads 0, writes 1; fn:SfxSpawnTrailEffect:100</summary>
    [FieldOffset(140)] public uint field_140;
    [FieldOffset(144)] public fixed byte gap_144[40]; // unknown
    /// <summary>void*; medium; reads 1, writes 0; fn:PsxGetContextQword8Or0:35</summary>
    [FieldOffset(184)] public void* field_184;
    [FieldOffset(192)] public fixed byte gap_192[224]; // unknown
    /// <summary>uint32_t; medium; reads 3, writes 0; fn:RegisterSummonModel:42 fn:RegisterSummonModel:53 fn:SfxLoaderStep:629</summary>
    [FieldOffset(416)] public uint field_416;
}

/// <summary>S094: size unknown, 8 bytes = extent (last offset + width); inferred shape H77: 9 access(es) in 2 function(s); size unknown (extent 8); used by SfxWaitObjCmd09Handler (a2), SfxWaitObjCmd0C17Handler (a2), SfxWaitObjCmd0C19Handler (a2), SfxObjCmd21Handler (a1) +15 more.</summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
internal unsafe struct State3
{
    /// <summary>uint8_t; medium; reads 3, writes 1; fn:SfxOpMoveObjToAnchor:8 fn:SfxOpMoveObjToAnchorStep:26 fn:SfxOpMoveObjToAnchorStep:112</summary>
    [FieldOffset(0)] public byte field_0;
    /// <summary>uint8_t; medium; reads 1, writes 1; fn:SfxOpMoveObjToAnchor:9 fn:SfxOpMoveObjToAnchorStep:17</summary>
    [FieldOffset(1)] public byte field_1;
    [FieldOffset(2)] public fixed byte gap_2[2]; // unknown
    /// <summary>uint32_t; medium; reads 2, writes 1; fn:SfxOpMoveObjToAnchor:10 fn:SfxOpMoveObjToAnchorStep:110 fn:SfxOpMoveObjToAnchorStep:117</summary>
    [FieldOffset(4)] public uint field_4;
}

/// <summary>S273: size unknown, 83 bytes = extent (last offset + width); object returned by the address translator PsxAddrToPtr (local:Native.SfxSendIntDataSlot(int):((uint*)off_180066C68)[index]).</summary>
[StructLayout(LayoutKind.Explicit, Size = 83)]
internal unsafe struct State4
{
    /// <summary>uint16_t; medium; reads 2, writes 0; Rewritten/u0116_SFX_SendIntData_0.cs:95 Rewritten/u0116_SFX_SendIntData_0.cs:117</summary>
    [FieldOffset(0)] public ushort field_0;
    [FieldOffset(2)] public fixed byte gap_2[13]; // unknown
    /// <summary>uint8_t; medium; reads 0, writes 1; Rewritten/u0116_SFX_SendIntData_0.cs:106</summary>
    [FieldOffset(15)] public byte field_15;
    [FieldOffset(16)] public fixed byte gap_16[12]; // unknown
    /// <summary>uint32_t; medium; reads 2, writes 0; Rewritten/u0116_SFX_SendIntData_0.cs:96 Rewritten/u0116_SFX_SendIntData_0.cs:116</summary>
    [FieldOffset(28)] public uint field_28;
    [FieldOffset(32)] public fixed byte gap_32[3]; // unknown
    /// <summary>uint8_t; medium; reads 1, writes 0; Rewritten/u0116_SFX_SendIntData_0.cs:104</summary>
    [FieldOffset(35)] public byte field_35;
    [FieldOffset(36)] public fixed byte gap_36[46]; // unknown
    /// <summary>int8_t; medium; reads 1, writes 1; Rewritten/u0116_SFX_SendIntData_0.cs:99 Rewritten/u0116_SFX_SendIntData_0.cs:101</summary>
    [FieldOffset(82)] public sbyte field_82;
}

/// <summary>S017: 88 bytes; 88-byte summon slot; array at 0x180220830 (right after the 32 EffModelSlots), used by Hi_SetSummonMotion/Hi_SetSummonMotFrame.</summary>
[StructLayout(LayoutKind.Explicit, Size = 88)]
internal unsafe struct SummonSlot
{
    /// <summary>uint64_t; high; host pointer; model+16 holds the motion pointer, motion+2 the u16 frame count</summary>
    [FieldOffset(0)] public ulong model;
    [FieldOffset(8)] public fixed byte gap_8[72]; // unknown
    /// <summary>uint8_t; high</summary>
    [FieldOffset(80)] public byte inUse;
    /// <summary>uint8_t; high; a2 != 0 in Hi_RegisterSummonModel</summary>
    [FieldOffset(81)] public byte field_81;
    [FieldOffset(82)] public fixed byte gap_82[2]; // unknown
    /// <summary>uint16_t; high; current frame, reset to 0 by Hi_SetSummonMotion</summary>
    [FieldOffset(84)] public ushort motionFrame;
    [FieldOffset(86)] public fixed byte gap_86[2]; // unknown
}

/// <summary>S310: 16 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct TILE
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public short w;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(14)] public short h;
}

/// <summary>S313: 12 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 12)]
internal unsafe struct TILE_1
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
}

/// <summary>S311: 12 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 12)]
internal unsafe struct TILE_16
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
}

/// <summary>S312: 12 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 12)]
internal unsafe struct TILE_8
{
    /// <summary>uint32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public uint tag;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public byte r0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(5)] public byte g0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(6)] public byte b0;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(7)] public byte code;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public short x0;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(10)] public short y0;
}

/// <summary>S036: size unknown, 48 bytes = extent (last offset + width); inferred shape H16: 110 access(es) in 31 function(s); size unknown (extent 48); used by SfxVramInitUpload (return), PsxAddPrimWithMode (return), PsxAddPrimAtDepthWithMode (return), PsxAddScreenTiles (return) +1 more.</summary>
[StructLayout(LayoutKind.Explicit, Size = 48)]
internal unsafe struct Table
{
    [FieldOffset(0)] public fixed byte gap_0[12]; // unknown
    /// <summary>int32_t; medium; reads 4, writes 0; fn:SeqStateUpdate:95 fn:SfxLoaderStep:371 fn:PsxGetContextQword:40</summary>
    [FieldOffset(12)] public int field_12;
    [FieldOffset(16)] public fixed byte gap_16[4]; // unknown
    /// <summary>int32_t; medium; reads 43, writes 0; fn:GeoTransformPart:217 fn:GeoTransformPart:241 fn:GeoTransformPart:248</summary>
    [FieldOffset(20)] public int field_20;
    [FieldOffset(24)] public fixed byte gap_24[4]; // unknown
    /// <summary>int32_t; medium; reads 4, writes 0; fn:EmitScrollLayers:129 fn:EmitScrollLayers:140 fn:EmitScrollLayers:195</summary>
    [FieldOffset(28)] public int field_28;
    /// <summary>int32_t; medium; reads 24, writes 0; fn:PsgDrawMeshPart:373 fn:PsgDrawMeshPart:379 fn:GeoSfxRenderPart:360</summary>
    [FieldOffset(32)] public int field_32;
    /// <summary>int32_t; medium; reads 13, writes 4; fn:DrawMorphModelByBone:83 fn:DrawMorphModelByBone:86 fn:SfxRunFrame:129</summary>
    [FieldOffset(36)] public int field_36;
    /// <summary>int32_t; medium; reads 8, writes 2; fn:SfxRunFrame:24 fn:SfxUpdateParticles:1359 fn:EmitSpriteCellFrame:242</summary>
    [FieldOffset(40)] public int field_40;
    /// <summary>int32_t; medium; reads 8, writes 0; fn:SfxRunFrame:95 fn:SfxRunFrame:108 fn:DrawFacingRibbon:157</summary>
    [FieldOffset(44)] public int field_44;
}

/// <summary>S127: size unknown, 84 bytes = extent (last offset + width); inferred shape H115: 24 access(es) in 5 function(s); size unknown (extent 84).</summary>
[StructLayout(LayoutKind.Explicit, Size = 84)]
internal unsafe struct Table2
{
    [FieldOffset(0)] public fixed byte gap_0[4]; // unknown
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxSpawnTrailEffect:91</summary>
    [FieldOffset(4)] public int field_4;
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxSpawnTrailEffect:99</summary>
    [FieldOffset(8)] public int field_8;
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxTrailSequenceHandler:117</summary>
    [FieldOffset(12)] public int field_12;
    /// <summary>int32_t; medium; reads 2, writes 0; fn:SfxSpawnTrailEffect:36 fn:SfxObjectStrikeSequence:189</summary>
    [FieldOffset(16)] public int field_16;
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxSpawnTrailEffect:52</summary>
    [FieldOffset(20)] public int field_20;
    /// <summary>int32_t; medium; reads 5, writes 0; fn:SfxTrailSequenceHandler:129 fn:SfxSpawnTrailEffect:42 fn:SfxSpawnTrailEffect:69</summary>
    [FieldOffset(24)] public int field_24;
    /// <summary>int32_t; medium; reads 4, writes 0; fn:SfxTrailSequenceHandler:124 fn:SfxSpawnTrailEffect:40 fn:SfxSpawnTrailEffect:65</summary>
    [FieldOffset(28)] public int field_28;
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxSpawnTrailEffect:59</summary>
    [FieldOffset(32)] public int field_32;
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxStartIdleTrack:34</summary>
    [FieldOffset(36)] public int field_36;
    [FieldOffset(40)] public fixed byte gap_40[4]; // unknown
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxTriggerReaction:43</summary>
    [FieldOffset(44)] public int field_44;
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxTriggerReaction:45</summary>
    [FieldOffset(48)] public int field_48;
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxTriggerReaction:32</summary>
    [FieldOffset(52)] public int field_52;
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxTriggerReaction:34</summary>
    [FieldOffset(56)] public int field_56;
    [FieldOffset(60)] public fixed byte gap_60[8]; // unknown
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxStartIdleTrack:36</summary>
    [FieldOffset(68)] public int field_68;
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxTriggerReaction:25</summary>
    [FieldOffset(72)] public int field_72;
    [FieldOffset(76)] public fixed byte gap_76[4]; // unknown
    /// <summary>int32_t; medium; reads 1, writes 0; fn:SfxSpawnTrailEffect:95</summary>
    [FieldOffset(80)] public int field_80;
}

/// <summary>S255: size unknown, 4 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.LinkOtPacketAtDepth(uint, uint, int, int, byte):packet).</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct Target
{
    /// <summary>uint32_t; medium; union member: overlaps union_3_1; reads 1, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0087_sub_180029890.cs:147</summary>
    [FieldOffset(0)] public uint union_0_4;
    /// <summary>uint8_t; medium; union member: overlaps union_0_4; reads 0, writes 1; union member: shares its bytes with the other union_* fields of the range; Rewritten/u0087_sub_180029890.cs:145</summary>
    [FieldOffset(3)] public byte union_3_1;
}

/// <summary>S283: size unknown, 6 bytes = extent (last offset + width); object returned by the address translator Sub1800354E0Ptr (local:Native.Sub1800354E0Place(Sub1800354E0Frame*, long, short*):*(uint*)(e+44)).</summary>
[StructLayout(LayoutKind.Explicit, Size = 6)]
internal unsafe struct Target2
{
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:515</summary>
    [FieldOffset(0)] public short field_0;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:516</summary>
    [FieldOffset(2)] public short field_2;
    /// <summary>int16_t; medium; reads 1, writes 0; Rewritten/u0106g_sub_1800354E0.cs:517</summary>
    [FieldOffset(4)] public short field_4;
}

/// <summary>S229: size unknown, 32 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.DrawFacingRibbon(ulong, short*, int, short, short, short, short, int, int, int, byte*, byte*, int):cameraAddress).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Translation
{
    [FieldOffset(0)] public fixed byte gap_0[20]; // unknown
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0052_DrawFacingRibbon.cs:57</summary>
    [FieldOffset(20)] public uint field_20;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0052_DrawFacingRibbon.cs:58</summary>
    [FieldOffset(24)] public uint field_24;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0052_DrawFacingRibbon.cs:59</summary>
    [FieldOffset(28)] public uint field_28;
}

/// <summary>S232: size unknown, 32 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.DrawScreenAlignedRibbon(ulong, int, short, short, short, short, int, int, int, byte*, byte*, int):cameraAddress).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Translation2
{
    [FieldOffset(0)] public fixed byte gap_0[20]; // unknown
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0057_CreateRibbonEffectSlot.cs:152</summary>
    [FieldOffset(20)] public uint field_20;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0057_CreateRibbonEffectSlot.cs:153</summary>
    [FieldOffset(24)] public uint field_24;
    /// <summary>uint32_t; medium; reads 1, writes 0; Rewritten/u0057_CreateRibbonEffectSlot.cs:154</summary>
    [FieldOffset(28)] public uint field_28;
}

/// <summary>S237: size unknown, 32 bytes = extent (last offset + width); object returned by the address translator ResolveGlobalPsxAddress (local:Native.EmitDepthCuedRibbon(ulong, int, short, short, short, short, int, int, int, byte*, byte*, int):matrixAddress).</summary>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal unsafe struct Translation3
{
    [FieldOffset(0)] public fixed byte gap_0[20]; // unknown
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0055_EmitDepthCuedRibbon.cs:83</summary>
    [FieldOffset(20)] public int field_20;
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0055_EmitDepthCuedRibbon.cs:84</summary>
    [FieldOffset(24)] public int field_24;
    /// <summary>int32_t; medium; reads 1, writes 0; Rewritten/u0055_EmitDepthCuedRibbon.cs:85</summary>
    [FieldOffset(28)] public int field_28;
}

/// <summary>S308: 2 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 2)]
internal unsafe struct UV
{
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public byte u;
    /// <summary>uint8_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(1)] public byte v;
}

/// <summary>S330: 16 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal unsafe struct VECTOR
{
    /// <summary>int32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public int vx;
    /// <summary>int32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(4)] public int vy;
    /// <summary>int32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(8)] public int vz;
    /// <summary>int32_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(12)] public int pad;
}

/// <summary>S309: 4 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct WH
{
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public short w;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(2)] public short h;
}

/// <summary>S307: 4 bytes; imported from MemoriaSfx.h.</summary>
[StructLayout(LayoutKind.Explicit, Size = 4)]
internal unsafe struct XY
{
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(0)] public short x;
    /// <summary>int16_t; high; file:MemoriaSfx.h</summary>
    [FieldOffset(2)] public short y;
}

/// <summary>2 x byte: the bytes of a fixed byte[2] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(2)]
internal struct ByteArray2
{
    private byte _element0;
}

/// <summary>3 x byte: the bytes of a fixed byte[3] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(3)]
internal struct ByteArray3
{
    private byte _element0;
}

/// <summary>4 x byte: the bytes of a fixed byte[4] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(4)]
internal struct ByteArray4
{
    private byte _element0;
}

/// <summary>8 x byte: the bytes of a fixed byte[8] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(8)]
internal struct ByteArray8
{
    private byte _element0;
}

/// <summary>12 x byte: the bytes of a fixed byte[12] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(12)]
internal struct ByteArray12
{
    private byte _element0;
}

/// <summary>128 x byte: the bytes of a fixed byte[128] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(128)]
internal struct ByteArray128
{
    private byte _element0;
}

/// <summary>224 x byte: the bytes of a fixed byte[224] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(224)]
internal struct ByteArray224
{
    private byte _element0;
}

/// <summary>1024 x byte: the bytes of a fixed byte[1024] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(1024)]
internal struct ByteArray1024
{
    private byte _element0;
}

/// <summary>8128 x byte: the bytes of a fixed byte[8128] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(8128)]
internal struct ByteArray8128
{
    private byte _element0;
}

/// <summary>8192 x byte: the bytes of a fixed byte[8192] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(8192)]
internal struct ByteArray8192
{
    private byte _element0;
}

/// <summary>3 x int: the bytes of a fixed int[3] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(3)]
internal struct IntArray3
{
    private int _element0;
}

/// <summary>1 x short: the bytes of a fixed short[1] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(1)]
internal struct ShortArray1
{
    private short _element0;
}

/// <summary>2 x short: the bytes of a fixed short[2] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(2)]
internal struct ShortArray2
{
    private short _element0;
}

/// <summary>9 x short: the bytes of a fixed short[9] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(9)]
internal struct ShortArray9
{
    private short _element0;
}

/// <summary>16 x short: the bytes of a fixed short[16] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(16)]
internal struct ShortArray16
{
    private short _element0;
}

/// <summary>1 x uint: the bytes of a fixed uint[1] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(1)]
internal struct UIntArray1
{
    private uint _element0;
}

/// <summary>2 x uint: the bytes of a fixed uint[2] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(2)]
internal struct UIntArray2
{
    private uint _element0;
}

/// <summary>3 x uint: the bytes of a fixed uint[3] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(3)]
internal struct UIntArray3
{
    private uint _element0;
}

/// <summary>5 x uint: the bytes of a fixed uint[5] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(5)]
internal struct UIntArray5
{
    private uint _element0;
}

/// <summary>13 x uint: the bytes of a fixed uint[13] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(13)]
internal struct UIntArray13
{
    private uint _element0;
}

/// <summary>15 x uint: the bytes of a fixed uint[15] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(15)]
internal struct UIntArray15
{
    private uint _element0;
}

/// <summary>16 x uint: the bytes of a fixed uint[16] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(16)]
internal struct UIntArray16
{
    private uint _element0;
}

/// <summary>64 x uint: the bytes of a fixed uint[64] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(64)]
internal struct UIntArray64
{
    private uint _element0;
}

/// <summary>1 x ulong: the bytes of a fixed ulong[1] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(1)]
internal struct ULongArray1
{
    private ulong _element0;
}

/// <summary>2 x ulong: the bytes of a fixed ulong[2] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(2)]
internal struct ULongArray2
{
    private ulong _element0;
}

/// <summary>1 x ushort: the bytes of a fixed ushort[1] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(1)]
internal struct UShortArray1
{
    private ushort _element0;
}

/// <summary>8 x ushort: the bytes of a fixed ushort[8] buffer, indexed with bounds checks (struct gen --inline-arrays).</summary>
[System.Runtime.CompilerServices.InlineArray(8)]
internal struct UShortArray8
{
    private ushort _element0;
}
