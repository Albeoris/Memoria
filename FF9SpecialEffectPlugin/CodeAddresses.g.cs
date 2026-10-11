using Recompiler.Runtime;

namespace Recompiled.FF9SpecialEffectPlugin;

internal static unsafe partial class Native
{
    /// <summary>MipsJumpTargetOffset @ 0x18000d140 (RVA 0xd140): thunk.</summary>
    public static nint Code_MipsJumpTargetOffset => (nint)(MipsDecodeAllCallbackFn2)(&CodeThunk_MipsJumpTargetOffset);
    /// <summary>MipsDecodeImm16Rs @ 0x18000d180 (RVA 0xd180): thunk.</summary>
    public static nint Code_MipsDecodeImm16Rs => (nint)(MipsDecodeAllCallbackFn)(&CodeThunk_MipsDecodeImm16Rs);
    /// <summary>SfxBufferReset @ 0x18000d520 (RVA 0xd520): thunk.</summary>
    public static nint Code_SfxBufferReset => (nint)(delegate* unmanaged<ulong*, ulong*>)&CodeThunk_SfxBufferReset;
    /// <summary>SfxBufferFreeData @ 0x18000d530 (RVA 0xd530): thunk.</summary>
    public static nint Code_SfxBufferFreeData => (nint)(delegate* unmanaged<long, void>)&CodeThunk_SfxBufferFreeData;
    /// <summary>PsxTaskScriptHandler @ 0x180049170 (RVA 0x49170): thunk.</summary>
    public static nint Code_PsxTaskScriptHandler => (nint)(delegate* unmanaged<long, long, long, long, long, long>)&CodeThunk_PsxTaskScriptHandler;

    /// <summary>The managed address for a code RVA the original's data or the planned code points to; 0 when there is none yet.</summary>
    internal static nint CodeAt(uint rva) => rva switch
    {
        // 0x1000 sub_180001000: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        // 0x1010 sub_180001010: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        // 0x1020 sub_180001020: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        0x1030 => (nint)(delegate* unmanaged<long>)&CodeThunk_InitMapperTable, // InitMapperTable
        // 0x1060 sub_180001060: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        // 0x1160 sub_180001160: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        // 0x1190 unknown_libname_4: is excluded from the plan (library); until then only a DLL in shared mode can hand out the original's code address
        // 0x11a0 sub_1800011A0: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        // 0x11e0 sub_1800011E0: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        // 0x1200 sub_180001200: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        // 0x1210 sub_180001210: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        // 0x1280 sub_180001280: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        // 0x1290 sub_180001290: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        // 0x12e0 sub_1800012E0: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        // 0x12f0 sub_1800012F0: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        // 0x1360 sub_180001360: is excluded from the plan (unreachable-from-roots); until then only a DLL in shared mode can hand out the original's code address
        0x2cd0 => (nint)(delegate* unmanaged<short*, long, long>)&CodeThunk_HostSendRect64, // HostSendRect64
        0x2d20 => (nint)(delegate* unmanaged<short*, long, long>)&CodeThunk_HostSendRect65, // HostSendRect65
        0x2f70 => (nint)(delegate* unmanaged<long, short*, short, short, long>)&CodeThunk_InitE7Packet, // InitE7Packet
        0x2fd0 => (nint)(delegate* unmanaged<long>)&CodeThunk_sub_180002FD0, // sub_180002FD0
        0x2fe0 => (nint)(delegate* unmanaged<short*, uint, long, long>)&CodeThunk_HostSendVramRect66, // HostSendVramRect66
        0x30b0 => (nint)(delegate* unmanaged<short*, short*, int, int, ushort*, long>)&CodeThunk_GteLoadAverageShort12, // GteLoadAverageShort12
        0x3190 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_PsxRcos, // PsxRcos
        0x31d0 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_PsxRsin, // PsxRsin
        0x3210 => (nint)(delegate* unmanaged<int, int, long>)&CodeThunk_PsxRatan2, // PsxRatan2
        0x3250 => (nint)(delegate* unmanaged<long, long>)&CodeThunk_GteSetRotMatrix, // GteSetRotMatrix
        0x3280 => (nint)(delegate* unmanaged<uint*, long>)&CodeThunk_GteSetTransMatrix, // GteSetTransMatrix
        0x32a0 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_PsxSquareRoot0, // PsxSquareRoot0
        0x39d0 => (nint)(delegate* unmanaged<short*, long, long>)&CodeThunk_RotMatrixYXZ, // RotMatrixYXZ
        0x3a20 => (nint)(delegate* unmanaged<short*, uint*, short*>)&CodeThunk_ScaleMatrix, // ScaleMatrix
        0x3ab0 => (nint)(delegate* unmanaged<short*, long>)&CodeThunk_MulRotMatrix, // MulRotMatrix
        0x3cb0 => (nint)(delegate* unmanaged<long, short*, long, long>)&CodeThunk_CompMatrix, // CompMatrix
        0xd0d0 => (nint)(delegate* unmanaged<long, uint, long>)&CodeThunk_MipsDecodeRs, // MipsDecodeRs
        0xd0e0 => (nint)(delegate* unmanaged<long, uint, long>)&CodeThunk_MipsDecodeRt, // MipsDecodeRt
        0xd0f0 => (nint)(delegate* unmanaged<long, ushort, long>)&CodeThunk_MipsDecodeRd, // MipsDecodeRd
        0xd100 => (nint)(delegate* unmanaged<long, uint, long>)&CodeThunk_MipsDecodeShamt, // MipsDecodeShamt
        0xd110 => (nint)(delegate* unmanaged<long, uint, long>)&CodeThunk_MipsDecodeCode20, // MipsDecodeCode20
        0xd120 => (nint)(delegate* unmanaged<long, ushort, long>)&CodeThunk_MipsDecodeSimm16, // MipsDecodeSimm16
        0xd130 => (nint)(delegate* unmanaged<long, ushort, long>)&CodeThunk_MipsDecodeImm16, // MipsDecodeImm16
        0xd140 => (nint)(MipsDecodeAllCallbackFn2)(&CodeThunk_MipsJumpTargetOffset), // MipsJumpTargetOffset
        0xd150 => (nint)(MipsDecodeAllCallbackFn2)(&CodeThunk_MipsDecodeImm25), // MipsDecodeImm25
        0xd160 => (nint)(delegate* unmanaged<int, ushort, long>)&CodeThunk_MipsBranchTarget, // MipsBranchTarget
        0xd180 => (nint)(MipsDecodeAllCallbackFn)(&CodeThunk_MipsDecodeImm16Rs), // MipsDecodeImm16Rs
        0xd520 => (nint)(delegate* unmanaged<ulong*, ulong*>)&CodeThunk_SfxBufferReset, // SfxBufferReset
        0xd530 => (nint)(delegate* unmanaged<long, void>)&CodeThunk_SfxBufferFreeData, // SfxBufferFreeData
        0x15200 => (nint)(delegate* unmanaged<int, sbyte, long>)&CodeThunk_HiDebugPsgData, // HiDebugPsgData
        0x15940 => (nint)(delegate* unmanaged<long, long, ulong*>)&CodeThunk_InitEffModelSlots, // InitEffModelSlots
        0x159a0 => (nint)(delegate* unmanaged<int, void*>)&CodeThunk_FreeEffModelSlot, // FreeEffModelSlot
        0x159f0 => (nint)(delegate* unmanaged<ushort*>)&CodeThunk_ResetEffModelSlots, // ResetEffModelSlots
        0x15a20 => (nint)(delegate* unmanaged<long, long, long*>)&CodeThunk_InitModelTable88, // InitModelTable88
        0x15aa0 => (nint)(delegate* unmanaged<long>)&CodeThunk_ResetSummonSlot, // ResetSummonSlot
        0x15ac0 => (nint)(delegate* unmanaged<ulong, long>)&CodeThunk_RegisterSolidEffModel, // RegisterSolidEffModel
        0x15b70 => (nint)(delegate* unmanaged<ulong, long>)&CodeThunk_RegisterGouEffModel, // RegisterGouEffModel
        0x15c20 => (nint)(delegate* unmanaged<ulong, short, short, short, short, long>)&CodeThunk_RegisterTexEffModel, // RegisterTexEffModel
        0x15d30 => (nint)(delegate* unmanaged<ulong, long, long, long>)&CodeThunk_RegisterTexListModel, // RegisterTexListModel
        0x15e10 => (nint)(delegate* unmanaged<ulong, long, long, long>)&CodeThunk_RegisterTexPtrModel, // RegisterTexPtrModel
        0x15ee0 => (nint)(delegate* unmanaged<long, long, long, long>)&CodeThunk_RegisterSummonModel, // RegisterSummonModel
        0x16150 => (nint)(delegate* unmanaged<long, long, long, int, long>)&CodeThunk_DrawEffModel, // DrawEffModel
        0x16570 => (nint)(delegate* unmanaged<long, long, long, int, long>)&CodeThunk_DrawSliceEffModel, // DrawSliceEffModel
        0x167f0 => (nint)(delegate* unmanaged<short*, int, int, int, long>)&CodeThunk_DrawEffModelByBone, // DrawEffModelByBone
        0x16cc0 => (nint)(delegate* unmanaged<long, long, long, int, int, ushort, long>)&CodeThunk_DrawMorphEffModel, // DrawMorphEffModel
        0x17190 => (nint)(delegate* unmanaged<short*, int, int, ushort, int, int, long>)&CodeThunk_DrawMorphModelByBone, // DrawMorphModelByBone
        0x17710 => (nint)(delegate* unmanaged<long, long, long, int, sbyte, long>)&CodeThunk_DrawSummonModel, // DrawSummonModel
        0x17a10 => (nint)(MipsDecodeAllCallbackFn2)(&CodeThunk_SetSummonMotion), // SetSummonMotion
        0x17a70 => (nint)(delegate* unmanaged<int, int, long>)&CodeThunk_SetSummonMotFrame, // SetSummonMotFrame
        0x17ae0 => (nint)(delegate* unmanaged<int, long, long, long>)&CodeThunk_SplitMdlVertex, // SplitMdlVertex
        0x17b30 => (nint)(delegate* unmanaged<int, int, long>)&CodeThunk_GetSplitMdlVertex, // GetSplitMdlVertex
        0x18000 => (nint)(delegate* unmanaged<int, ushort, ulong, uint*, int, long>)&CodeThunk_GetMdlVertexPtr, // GetMdlVertexPtr
        0x182c0 => (nint)(delegate* unmanaged<void>)&CodeThunk_nullsub_1, // nullsub_1
        0x182d0 => (nint)(delegate* unmanaged<long, uint, long, long>)&CodeThunk_HostSendCommand0EMode0, // HostSendCommand0EMode0
        0x18340 => (nint)(delegate* unmanaged<long, int, long, long>)&CodeThunk_HostSendCommand0EMode0FromBlock, // HostSendCommand0EMode0FromBlock
        0x18440 => (nint)(delegate* unmanaged<long, uint, long, long>)&CodeThunk_HostSendCommand0EMode2, // HostSendCommand0EMode2
        0x184b0 => (nint)(delegate* unmanaged<long, int, long, long>)&CodeThunk_HostSendCommand0EMode2FromBlock, // HostSendCommand0EMode2FromBlock
        0x185b0 => (nint)(delegate* unmanaged<int, int, ushort*, long>)&CodeThunk_GetSummonBonePos, // GetSummonBonePos
        0x18630 => (nint)(delegate* unmanaged<int, int, void*, long>)&CodeThunk_GetSummonBoneMatrix, // GetSummonBoneMatrix
        0x187e0 => (nint)(delegate* unmanaged<int, sbyte, long>)&CodeThunk_ShowSummonModelMesh, // ShowSummonModelMesh
        0x18840 => (nint)(delegate* unmanaged<int, sbyte, long>)&CodeThunk_HideSummonModelMesh, // HideSummonModelMesh
        0x188a0 => (nint)(delegate* unmanaged<int, int, sbyte, long>)&CodeThunk_StartSummonTexAnim, // StartSummonTexAnim
        0x18930 => (nint)(delegate* unmanaged<int, int, long>)&CodeThunk_StopSummonTexAnim, // StopSummonTexAnim
        0x18990 => (nint)(delegate* unmanaged<int, int, void>)&CodeThunk_HiModifyEffModelAbr, // HiModifyEffModelAbr
        0x189f0 => (nint)(delegate* unmanaged<int, ushort, ushort, ushort, void>)&CodeThunk_ModifyEffModelRgb, // ModifyEffModelRgb
        0x18a40 => (nint)(delegate* unmanaged<int, short, void*>)&CodeThunk_SetEffModelOffset, // SetEffModelOffset
        0x18a90 => (nint)(delegate* unmanaged<int, short, void*>)&CodeThunk_SetEffModelSlice, // SetEffModelSlice
        0x18af0 => (nint)(delegate* unmanaged<int, int, void>)&CodeThunk_ModifySummonModelAbr, // ModifySummonModelAbr
        0x18b50 => (nint)(delegate* unmanaged<int, ushort, ushort, ushort, void>)&CodeThunk_ModifySummonModelRgb, // ModifySummonModelRgb
        0x18ba0 => (nint)(delegate* unmanaged<long, long>)&CodeThunk_HostObjCmd1A, // HostObjCmd1A
        0x18c00 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_EffModelPartCount, // EffModelPartCount
        0x18ce0 => (nint)(delegate* unmanaged<ushort*, long>)&CodeThunk_FillIdTable435, // FillIdTable435
        0x18fd0 => (nint)(delegate* unmanaged<int, long, long, long, byte*, long>)&CodeThunk_StoreBytePairA, // StoreBytePairA
        0x19000 => (nint)(delegate* unmanaged<int, long, long, long, byte*, long>)&CodeThunk_StoreBytePairB, // StoreBytePairB
        0x19030 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxObjCmd1EHandler, // SfxObjCmd1EHandler
        0x190a0 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxObjCmd1FHandler, // SfxObjCmd1FHandler
        0x19130 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxObjCmd1DHandler, // SfxObjCmd1DHandler
        0x19170 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxObjCmd0CHandler, // SfxObjCmd0CHandler
        0x191f0 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxWaitObjCmd09Handler, // SfxWaitObjCmd09Handler
        0x19230 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxWaitObjCmd09OffsetHandler, // SfxWaitObjCmd09OffsetHandler
        0x19280 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxWaitObjCmd0C17Handler, // SfxWaitObjCmd0C17Handler
        0x192c0 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxWaitObjCmd0C17OffsetHandler, // SfxWaitObjCmd0C17OffsetHandler
        0x19310 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxWaitObjCmd0C19Handler, // SfxWaitObjCmd0C19Handler
        0x19350 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxObjResetHandler, // SfxObjResetHandler
        0x193e0 => (nint)(delegate* unmanaged<long, byte*, long, long, byte*, long>)&CodeThunk_SfxObjCmd21Handler, // SfxObjCmd21Handler
        0x19430 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxOpCmd09Wait, // SfxOpCmd09Wait
        0x19470 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxOpHostCmd0D, // SfxOpHostCmd0D
        0x194f0 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxOpMoveObjToObj, // SfxOpMoveObjToObj
        0x195a0 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxOpMoveObjToQueryPos, // SfxOpMoveObjToQueryPos
        0x195e0 => (nint)(delegate* unmanaged<int, long, long, long, byte*, long>)&CodeThunk_SfxOpMoveCtx16ToObj, // SfxOpMoveCtx16ToObj
        0x196f0 => (nint)(delegate* unmanaged<int, long, long, long, byte*, long>)&CodeThunk_SfxOpCtx16Advance, // SfxOpCtx16Advance
        0x197a0 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxOpCtx16ToQueryPos, // SfxOpCtx16ToQueryPos
        0x19830 => (nint)(delegate* unmanaged<int, long, long, long, byte*, long>)&CodeThunk_SfxOpMoveObjToAnchor, // SfxOpMoveObjToAnchor
        0x19880 => (nint)(delegate* unmanaged<int, long, long, long, byte*, long>)&CodeThunk_SfxOpCtx16BlendNearObj, // SfxOpCtx16BlendNearObj
        0x19990 => (nint)(delegate* unmanaged<int, long, long, long, byte*, long>)&CodeThunk_SfxOpCtx16MoveNearObj, // SfxOpCtx16MoveNearObj
        0x19a90 => (nint)(delegate* unmanaged<int, long, long, long, byte*, long>)&CodeThunk_SfxObjPairTaskHandler, // SfxObjPairTaskHandler
        0x19b80 => (nint)(delegate* unmanaged<int, long, long, long, byte*, long>)&CodeThunk_SfxObjRotateHandler, // SfxObjRotateHandler
        0x19c20 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxObjSetFacingHandler, // SfxObjSetFacingHandler
        0x19c80 => (nint)(delegate* unmanaged<int, byte*, long, int*, byte*, long>)&CodeThunk_SfxObjAnimWaitHandler, // SfxObjAnimWaitHandler
        0x19d70 => (nint)(delegate* unmanaged<int, byte*, long, uint*, byte*, long>)&CodeThunk_SfxObjFlagsDifferWaitHandler, // SfxObjFlagsDifferWaitHandler
        0x19e50 => (nint)(delegate* unmanaged<int, byte*, long, uint*, byte*, long>)&CodeThunk_SfxObjFlagsMatchWaitHandler, // SfxObjFlagsMatchWaitHandler
        0x19f30 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxObjCallKindScriptHandler, // SfxObjCallKindScriptHandler
        0x19fc0 => (nint)(delegate* unmanaged<int, byte*, long, long, byte*, long>)&CodeThunk_SfxObjFlagsMatchHandler, // SfxObjFlagsMatchHandler
        0x1b240 => (nint)(delegate* unmanaged<long, byte, long>)&CodeThunk_HostObjCmd0CIfFlag, // HostObjCmd0CIfFlag
        0x1b310 => (nint)(delegate* unmanaged<long, sbyte, long>)&CodeThunk_HostObjCmd0CReset, // HostObjCmd0CReset
        0x1b380 => (nint)(delegate* unmanaged<long, uint, long>)&CodeThunk_HostObjCmd0CU32, // HostObjCmd0CU32
        0x1b4a0 => (nint)(MipsDecodeAllCallbackFn2)(&CodeThunk_ReturnArg2), // ReturnArg2
        0x1b4b0 => (nint)(delegate* unmanaged<long, uint, long>)&CodeThunk_HostObjCmd0B, // HostObjCmd0B
        0x1b500 => (nint)(delegate* unmanaged<long, long>)&CodeThunk_HostObjCmd09, // HostObjCmd09
        0x1b560 => (nint)(delegate* unmanaged<long, long>)&CodeThunk_HostObjCmd0A, // HostObjCmd0A
        0x1b5c0 => (nint)(MipsDecodeAllCallbackFn2)(&CodeThunk_HostObjSetFacing), // HostObjSetFacing
        0x1b870 => (nint)(delegate* unmanaged<long, long>)&CodeThunk_HostObjCmd0DOn, // HostObjCmd0DOn
        0x1b8d0 => (nint)(delegate* unmanaged<long, uint, long>)&CodeThunk_HostObjCmd0D, // HostObjCmd0D
        0x1b980 => (nint)(MipsDecodeAllCallbackFn2)(&CodeThunk_HostObjCmd12), // HostObjCmd12
        0x1b9e0 => (nint)(delegate* unmanaged<long, uint, long>)&CodeThunk_HostObjCmd13, // HostObjCmd13
        0x1ba30 => (nint)(delegate* unmanaged<long, long>)&CodeThunk_HostObjQuery14IsZero, // HostObjQuery14IsZero
        0x1bab0 => (nint)(delegate* unmanaged<long>)&CodeThunk_ReturnMinusOne, // ReturnMinusOne
        0x208d0 => (nint)(delegate* unmanaged<short*, short*, long>)&CodeThunk_VecDistance3, // VecDistance3
        0x20930 => (nint)(delegate* unmanaged<long>)&CodeThunk_PsxRand, // PsxRand
        0x20950 => (nint)(delegate* unmanaged<uint, int, long>)&CodeThunk_PsxRandRange, // PsxRandRange
        0x20980 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_PsxRandCentered, // PsxRandCentered
        0x209b0 => (nint)(delegate* unmanaged<long, long>)&CodeThunk_PsxRandScatterVec, // PsxRandScatterVec
        0x20a20 => (nint)(delegate* unmanaged<short*, short*, ushort*, long>)&CodeThunk_LookAnglesBetween, // LookAnglesBetween
        0x20bd0 => (nint)(delegate* unmanaged<int, int, int, long>)&CodeThunk_GteIntplScalar, // GteIntplScalar
        0x20c00 => (nint)(delegate* unmanaged<int, int, int, int, long>)&CodeThunk_LerpIntRatio, // LerpIntRatio
        0x20c50 => (nint)(delegate* unmanaged<short*, short*, int, ushort*, long>)&CodeThunk_GteIntplVector, // GteIntplVector
        0x2fc80 => (nint)(delegate* unmanaged<ulong, short, ulong, ulong, long>)&CodeThunk_ParticleStepMotion, // ParticleStepMotion
        0x30620 => (nint)(delegate* unmanaged<long>)&CodeThunk_SfxPoolsReset, // SfxPoolsReset
        0x306b0 => (nint)(delegate* unmanaged<long>)&CodeThunk_SfxTickParticleSystem, // SfxTickParticleSystem
        0x306f0 => (nint)(delegate* unmanaged<long, long, long>)&CodeThunk_CreateTrailEffect, // CreateTrailEffect
        0x30710 => (nint)(delegate* unmanaged<long, long, long>)&CodeThunk_MoveSfxObject, // MoveSfxObject
        0x30730 => (nint)(delegate* unmanaged<long, void>)&CodeThunk_DetachSfxObject, // DetachSfxObject
        0x30780 => (nint)(delegate* unmanaged<int, int, long>)&CodeThunk_ComputeArenaBytes, // ComputeArenaBytes
        0x307a0 => (nint)(delegate* unmanaged<uint, long, int, int, int, uint*>)&CodeThunk_PsxInitEntryBlock, // PsxInitEntryBlock
        0x30880 => (nint)(delegate* unmanaged<uint, void>)&CodeThunk_ClearPoolSlotHeaders, // ClearPoolSlotHeaders
        0x30930 => (nint)(delegate* unmanaged<uint, ushort*>)&CodeThunk_SfxStartScriptSlot, // SfxStartScriptSlot
        0x30a70 => (nint)(delegate* unmanaged<uint, long>)&CodeThunk_SfxTickScriptSlots, // SfxTickScriptSlots
        0x30bd0 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_FindPairSlot, // FindPairSlot
        0x313d0 => (nint)(delegate* unmanaged<long>)&CodeThunk_sub_1800313D0, // sub_1800313D0
        0x313f0 => (nint)(delegate* unmanaged<short, long>)&CodeThunk_SeqStateBegin, // SeqStateBegin
        0x314f0 => (nint)(delegate* unmanaged<int, byte*>)&CodeThunk_PsxSelectTableRecord, // PsxSelectTableRecord
        0x31520 => (nint)(delegate* unmanaged<void>)&CodeThunk_SeqStateUpdate, // SeqStateUpdate
        0x32260 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_SfxVoiceStop, // SfxVoiceStop
        0x322e0 => (nint)(delegate* unmanaged<int, int, void>)&CodeThunk_SfxVoiceSetSource, // SfxVoiceSetSource
        0x32310 => (nint)(delegate* unmanaged<uint, long>)&CodeThunk_SfxVoiceSetAllSources, // SfxVoiceSetAllSources
        0x323a0 => (nint)(delegate* unmanaged<int, uint, uint, long>)&CodeThunk_SfxVoicePlay, // SfxVoicePlay
        0x324b0 => (nint)(delegate* unmanaged<int, uint, uint, int, long>)&CodeThunk_SfxVoiceRestart, // SfxVoiceRestart
        0x324f0 => (nint)(delegate* unmanaged<int, long, long, byte*>)&CodeThunk_InitPool56, // InitPool56
        0x32540 => (nint)(delegate* unmanaged<long, long, byte*>)&CodeThunk_SfxIndexedRecordPtr, // SfxIndexedRecordPtr
        0x325f0 => (nint)(delegate* unmanaged<long, long>)&CodeThunk_SfxUpdateParticles, // SfxUpdateParticles
        0x34330 => (nint)(delegate* unmanaged<int, long, ushort*>)&CodeThunk_InitPool108, // InitPool108
        0x34380 => (nint)(delegate* unmanaged<long, long, ulong, ulong, long>)&CodeThunk_CreateRibbonEffectSlot, // CreateRibbonEffectSlot
        0x34710 => (nint)(delegate* unmanaged<long, long, short*, long>)&CodeThunk_MoveObjectWithAttachments, // MoveObjectWithAttachments
        0x34820 => (nint)(delegate* unmanaged<long, long, long>)&CodeThunk_Pool56ReleaseOwner, // Pool56ReleaseOwner
        0x34860 => (nint)(delegate* unmanaged<long, long, long>)&CodeThunk_SfxUpdateKeyframeTracks, // SfxUpdateKeyframeTracks
        0x35490 => (nint)(delegate* unmanaged<long, long>)&CodeThunk_Pool56FindFree, // Pool56FindFree
        0x354e0 => (nint)(delegate* unmanaged<long, long, void>)&CodeThunk_sub_1800354E0, // sub_1800354E0
        0x380c0 => (nint)(delegate* unmanaged<long, long, short*, int*, short>)&CodeThunk_SfxEmitParticles, // SfxEmitParticles
        0x39d60 => (nint)(delegate* unmanaged<int, long, long, long, byte*, long>)&CodeThunk_SfxOpSetParamBytes01, // SfxOpSetParamBytes01
        0x39d90 => (nint)(delegate* unmanaged<int, long, long, long, byte*, long>)&CodeThunk_SfxOpSetParamBytes23, // SfxOpSetParamBytes23
        0x39dc0 => (nint)(delegate* unmanaged<int, long, long, long, uint*, long>)&CodeThunk_SfxOpSetHostFrameRate, // SfxOpSetHostFrameRate
        0x39e10 => (nint)(delegate* unmanaged<int, int*, long, int*, uint*, long>)&CodeThunk_SfxTrailSequenceHandler, // SfxTrailSequenceHandler
        0x3a690 => (nint)(delegate* unmanaged<int, byte*, uint*, int*, byte*, long>)&CodeThunk_SfxObjectStrikeSequence, // SfxObjectStrikeSequence
        0x3b0d0 => (nint)(delegate* unmanaged<int, long, long, int*, uint*, long>)&CodeThunk_SfxOpMoveObjToPoint, // SfxOpMoveObjToPoint
        0x3b3e0 => (nint)(delegate* unmanaged<int, long, long, long, byte*, long>)&CodeThunk_SfxOpStoreArgBytes, // SfxOpStoreArgBytes
        0x3b410 => (nint)(delegate* unmanaged<int, long, long, long, int*, long>)&CodeThunk_SfxOpCopyVarToEntry, // SfxOpCopyVarToEntry
        0x3b480 => (nint)(delegate* unmanaged<int, long, long, long, int*, long>)&CodeThunk_SfxObjectEffectHandler, // SfxObjectEffectHandler
        0x3bab0 => (nint)(delegate* unmanaged<int, long, long, long, uint*, long>)&CodeThunk_SfxOpObjMessage, // SfxOpObjMessage
        0x3bb80 => (nint)(delegate* unmanaged<int, long, long, long, uint*, long>)&CodeThunk_SfxOpPushVariantId, // SfxOpPushVariantId
        0x3bbd0 => (nint)(delegate* unmanaged<int, long, long, long, uint*, long>)&CodeThunk_SfxOpStartTrackBlock, // SfxOpStartTrackBlock
        0x3bce0 => (nint)(delegate* unmanaged<int, uint, long>)&CodeThunk_StartPsxTask10, // StartPsxTask10
        0x3bd10 => (nint)(delegate* unmanaged<int, long, long, int*, short*, long>)&CodeThunk_SfxOpHostLevelFade, // SfxOpHostLevelFade
        0x3be40 => (nint)(delegate* unmanaged<int, long, long, int*, int*, long>)&CodeThunk_SfxOpMotionFade, // SfxOpMotionFade
        0x3bf00 => (nint)(delegate* unmanaged<int, long, long, long, int*, long>)&CodeThunk_SfxOpVoicePlay, // SfxOpVoicePlay
        0x3bf40 => (nint)(delegate* unmanaged<uint, uint, long>)&CodeThunk_StartPsxTask14, // StartPsxTask14
        0x3bf60 => (nint)(delegate* unmanaged<int, uint*, long, int*, int*, long>)&CodeThunk_SfxOpScreenTilesFade, // SfxOpScreenTilesFade
        0x3c0d0 => (nint)(delegate* unmanaged<int, int, int, int, int, int*, long>)&CodeThunk_SfxRangeFraction, // SfxRangeFraction
        0x3c110 => (nint)(delegate* unmanaged<short*, int, ushort*, long>)&CodeThunk_RotateDepthVector, // RotateDepthVector
        0x3c320 => (nint)(delegate* unmanaged<short*, short*, int, ushort*, long>)&CodeThunk_SfxRotTransPointZ, // SfxRotTransPointZ
        0x3c560 => (nint)(delegate* unmanaged<long, int, ushort*, long>)&CodeThunk_SfxMatrixPointZ, // SfxMatrixPointZ
        0x3c730 => (nint)(delegate* unmanaged<long, ushort*, long>)&CodeThunk_SfxMatrixAimAngles, // SfxMatrixAimAngles
        0x3c9b0 => (nint)(delegate* unmanaged<short, short, int*, long>)&CodeThunk_SfxAngleDelta, // SfxAngleDelta
        0x3ca00 => (nint)(delegate* unmanaged<ushort*, ushort*, int, long>)&CodeThunk_SfxTurnTowards, // SfxTurnTowards
        0x3cae0 => (nint)(delegate* unmanaged<long*, byte*, long>)&CodeThunk_SfxViewTransformTr, // SfxViewTransformTr
        0x3cce0 => (nint)(delegate* unmanaged<short*, uint*, byte*, long>)&CodeThunk_SfxBuildRotScaleMatrix, // SfxBuildRotScaleMatrix
        0x3cf30 => (nint)(delegate* unmanaged<short*, ushort*, int, int*, int, short, long>)&CodeThunk_TransformPointByAngles, // TransformPointByAngles
        0x3d170 => (nint)(delegate* unmanaged<ushort*, int*, short, long>)&CodeThunk_RotationToLookAngles, // RotationToLookAngles
        0x3d420 => (nint)(delegate* unmanaged<ushort*, ushort*, ushort*, uint*, uint*, uint*, long>)&CodeThunk_ComputeLocalYawDistance, // ComputeLocalYawDistance
        0x3d660 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_PsxAlignUp4, // PsxAlignUp4
        0x3d670 => (nint)(delegate* unmanaged<long, int, void>)&CodeThunk_SfxArenaInit, // SfxArenaInit
        0x3d6c0 => (nint)(delegate* unmanaged<long, int, int*>)&CodeThunk_PsxArenaAlloc, // PsxArenaAlloc
        0x3d7a0 => (nint)(MipsDecodeAllCallbackFn2)(&CodeThunk_SfxArenaResetIfShort), // SfxArenaResetIfShort
        0x3d7d0 => (nint)(delegate* unmanaged<long, long>)&CodeThunk_SfxArenaFillBlock, // SfxArenaFillBlock
        0x3d800 => (nint)(delegate* unmanaged<int, int, byte*>)&CodeThunk_PsxGetTableEntry, // PsxGetTableEntry
        0x3daa0 => (nint)(delegate* unmanaged<int, int, long>)&CodeThunk_PsxTableEntrySize, // PsxTableEntrySize
        0x3dae0 => (nint)(delegate* unmanaged<int, byte*>)&CodeThunk_PsxBumpAlloc, // PsxBumpAlloc
        0x3db80 => (nint)(delegate* unmanaged<long>)&CodeThunk_SfxClearWord1FC, // SfxClearWord1FC
        0x3db90 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_PsxChannelReset, // PsxChannelReset
        0x3dbf0 => (nint)(delegate* unmanaged<long>)&CodeThunk_PsxChannelFlip, // PsxChannelFlip
        0x3de20 => (nint)(delegate* unmanaged<short>)&CodeThunk_SfxLoaderStep, // SfxLoaderStep
        0x3edb0 => (nint)(delegate* unmanaged<int, uint*, ulong, int, long>)&CodeThunk_PsxAddPrimWithMode, // PsxAddPrimWithMode
        0x3eed0 => (nint)(delegate* unmanaged<int, int, ulong, int*>)&CodeThunk_PsxAddPrimAtDepth, // PsxAddPrimAtDepth
        0x3efc0 => (nint)(delegate* unmanaged<int, int, ulong, int, int*>)&CodeThunk_PsxAddPrimAtDepthWithMode, // PsxAddPrimAtDepthWithMode
        0x3f0c0 => (nint)(delegate* unmanaged<int, sbyte, byte*>)&CodeThunk_PsxAllocDrawMode, // PsxAllocDrawMode
        0x3f180 => (nint)(delegate* unmanaged<long, int, byte, byte, byte, long>)&CodeThunk_PsxAddScreenTiles, // PsxAddScreenTiles
        0x3f3a0 => (nint)(delegate* unmanaged<uint*, short, uint, int, short, int, long, int, int, int, int, ulong>)&CodeThunk_PsxAddWaveGradient, // PsxAddWaveGradient
        0x3f6f0 => (nint)(delegate* unmanaged<ulong, short*, int, short, short, short, short, int, int, int, byte*, byte*, int, long>)&CodeThunk_DrawFacingRibbon, // DrawFacingRibbon
        0x3fc70 => (nint)(delegate* unmanaged<ulong, int, short, short, short, short, int, int, int, byte*, byte*, int, sbyte>)&CodeThunk_EmitDepthCuedRibbon, // EmitDepthCuedRibbon
        0x40340 => (nint)(delegate* unmanaged<ulong, int, short, short, short, short, int, int, int, byte*, byte*, int, long>)&CodeThunk_DrawScreenAlignedRibbon, // DrawScreenAlignedRibbon
        0x40730 => (nint)(delegate* unmanaged<ushort*, long, short*, int, int, int, int, byte*, int, sbyte>)&CodeThunk_DrawRibbonTrail, // DrawRibbonTrail
        0x409d0 => (nint)(delegate* unmanaged<ushort*, long*, short*, int, int, int, int, int, int, long>)&CodeThunk_EmitSpriteCellFrame, // EmitSpriteCellFrame
        0x40f90 => (nint)(delegate* unmanaged<ushort*, int*, int, int, int, long>)&CodeThunk_EmitWorldSpriteList, // EmitWorldSpriteList
        0x413b0 => (nint)(delegate* unmanaged<int, ushort*, int, int, int, int, int, long>)&CodeThunk_EmitScreenSpriteList, // EmitScreenSpriteList
        0x41670 => (nint)(delegate* unmanaged<long, long*, short*, int, int, int, int, long>)&CodeThunk_EmitSpriteAnimFrame, // EmitSpriteAnimFrame
        0x41b80 => (nint)(delegate* unmanaged<long, long*, short*, int, int, int, uint, long>)&CodeThunk_EmitWorldSpriteFrame, // EmitWorldSpriteFrame
        0x42180 => (nint)(delegate* unmanaged<byte*, long*, short*, int, int, int, int, long>)&CodeThunk_DrawRingMeshEffect, // DrawRingMeshEffect
        0x42be0 => (nint)(delegate* unmanaged<ushort*, long*, short*, int, int, int, int, long>)&CodeThunk_EmitScaledSpriteFrame, // EmitScaledSpriteFrame
        0x434b0 => (nint)(delegate* unmanaged<long, short, short, int, int, int, int*>)&CodeThunk_EmitSpriteFrame, // EmitSpriteFrame
        0x43800 => (nint)(delegate* unmanaged<ushort*, int, int*>)&CodeThunk_EmitScrollLayers, // EmitScrollLayers
        0x44560 => (nint)(delegate* unmanaged<ushort*, long*, int, int, short>)&CodeThunk_EmitInterpolatedSpriteTrail, // EmitInterpolatedSpriteTrail
        0x44a30 => (nint)(delegate* unmanaged<ulong, long>)&CodeThunk_SfxRegisterTableSlot3, // SfxRegisterTableSlot3
        0x44a60 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_PsxGetContextQword, // PsxGetContextQword
        0x44d50 => (nint)(delegate* unmanaged<int*>)&CodeThunk_ClearActiveObjectAddr, // ClearActiveObjectAddr
        0x44d60 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_ObjSelectDistanceById, // ObjSelectDistanceById
        0x44da0 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_ObjSizeById, // ObjSizeById
        0x44dc0 => (nint)(delegate* unmanaged<int, long, long>)&CodeThunk_HostGetObjectPositionById, // HostGetObjectPositionById
        0x44f60 => (nint)(delegate* unmanaged<int, long, long>)&CodeThunk_HostQueryObjPositionById, // HostQueryObjPositionById
        0x450c0 => (nint)(delegate* unmanaged<int, int, long, sbyte>)&CodeThunk_HostQueryObjAnchorPosById, // HostQueryObjAnchorPosById
        0x450f0 => (nint)(delegate* unmanaged<int, long, long>)&CodeThunk_HostSetObjectPositionById, // HostSetObjectPositionById
        0x45350 => (nint)(delegate* unmanaged<int, long, long>)&CodeThunk_HostGetObjectRotationById, // HostGetObjectRotationById
        0x453c0 => (nint)(delegate* unmanaged<int, long, long>)&CodeThunk_HostSetObjRotation, // HostSetObjRotation
        0x45480 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_IsObjNotFlippedById, // IsObjNotFlippedById
        0x454a0 => (nint)(delegate* unmanaged<int, short*, ushort*, sbyte, short, int, int, long>)&CodeThunk_SfxObjTurnTowardsPoint, // SfxObjTurnTowardsPoint
        0x455f0 => (nint)(delegate* unmanaged<int, uint*, long>)&CodeThunk_HostQueryObjScaleById, // HostQueryObjScaleById
        0x456d0 => (nint)(delegate* unmanaged<int, uint*, long>)&CodeThunk_HostSetObjScaleById, // HostSetObjScaleById
        0x457c0 => (nint)(delegate* unmanaged<int, int, short*, long>)&CodeThunk_ObjAimPointById, // ObjAimPointById
        0x45970 => (nint)(delegate* unmanaged<int, int, long>)&CodeThunk_ObjDepthOrderSignById, // ObjDepthOrderSignById
        0x45a80 => (nint)(delegate* unmanaged<int, int, long>)&CodeThunk_ObjDistanceSixthPlusById, // ObjDistanceSixthPlusById
        0x45ab0 => (nint)(delegate* unmanaged<int, int, ushort*, long>)&CodeThunk_GetObjectHostPosition, // GetObjectHostPosition
        0x45f90 => (nint)(delegate* unmanaged<int, int, ushort*>)&CodeThunk_SetObjectMeshVisibilityById, // SetObjectMeshVisibilityById
        0x46020 => (nint)(delegate* unmanaged<int, uint, long>)&CodeThunk_HostFadeObjectMeshIfAllowed, // HostFadeObjectMeshIfAllowed
        0x460e0 => (nint)(delegate* unmanaged<int, int, long>)&CodeThunk_SetMeshFadeForObjectList, // SetMeshFadeForObjectList
        0x462a0 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_HostResetObjectStatuses, // HostResetObjectStatuses
        0x46320 => (nint)(delegate* unmanaged<int, long, uint*, uint*, long>)&CodeThunk_HostObjQueryAccess, // HostObjQueryAccess
        0x464f0 => (nint)(delegate* unmanaged<long>)&CodeThunk_HostEnableLastBossFlag, // HostEnableLastBossFlag
        0x46670 => (nint)(delegate* unmanaged<int, int, long>)&CodeThunk_HostObjectCheckAction, // HostObjectCheckAction
        0x46890 => (nint)(delegate* unmanaged<int, int, long>)&CodeThunk_IsObjectStateEqual, // IsObjectStateEqual
        0x468d0 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_SelectReg3ObjectFromList, // SelectReg3ObjectFromList
        0x46cb0 => (nint)(delegate* unmanaged<int, uint, uint, long>)&CodeThunk_HostControlEyeTextureAnimation, // HostControlEyeTextureAnimation
        0x46e80 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_HostExecuteVfxForObjects, // HostExecuteVfxForObjects
        0x47070 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_HostRequestBattleFiguresForObjects, // HostRequestBattleFiguresForObjects
        0x47260 => (nint)(delegate* unmanaged<uint, uint>)&CodeThunk_HostSetSoundFps, // HostSetSoundFps
        0x47290 => (nint)(delegate* unmanaged<ushort*, sbyte, long>)&CodeThunk_RegisterSoModel, // RegisterSoModel
        0x47330 => (nint)(delegate* unmanaged<ushort*, int*, uint, ulong, long>)&CodeThunk_RegisterSoSummonModel, // RegisterSoSummonModel
        0x47a00 => (nint)(delegate* unmanaged<long, byte*, int, long>)&CodeThunk_SampleColorRamp, // SampleColorRamp
        0x47b40 => (nint)(delegate* unmanaged<int, short, short, short, int, short, ushort, long>)&CodeThunk_EmitSplitE7Prims, // EmitSplitE7Prims
        0x47e30 => (nint)(delegate* unmanaged<uint, int, int, int, int, int, int, short, short, short*, int*, long, int, int, long>)&CodeThunk_SfxDrawWarpGrid, // SfxDrawWarpGrid
        0x484e0 => (nint)(delegate* unmanaged<short*, long>)&CodeThunk_PushRotTransMatrix, // PushRotTransMatrix
        0x486a0 => (nint)(delegate* unmanaged<long>)&CodeThunk_PopRotTransMatrix, // PopRotTransMatrix
        0x48740 => (nint)(delegate* unmanaged<int, long>)&CodeThunk_SetMatrixDepth, // SetMatrixDepth
        0x48760 => (nint)(delegate* unmanaged<long>)&CodeThunk_RestoreMatrixDepth, // RestoreMatrixDepth
        0x48770 => (nint)(delegate* unmanaged<int, int, int, int, int, int, int, int, long>)&CodeThunk_HostCaptureBackgroundTexture, // HostCaptureBackgroundTexture
        0x48800 => (nint)(delegate* unmanaged<uint*>)&CodeThunk_ResetSfxWorkArea, // ResetSfxWorkArea
        0x488c0 => (nint)(delegate* unmanaged<long, int, int*>)&CodeThunk_PsxObjectAlloc, // PsxObjectAlloc
        0x48b10 => (nint)(delegate* unmanaged<int, short, int, int, long>)&CodeThunk_StartPsxTask, // StartPsxTask
        0x48e70 => (nint)(delegate* unmanaged<long>)&CodeThunk_SfxTickEffectTasks, // SfxTickEffectTasks
        0x49170 => (nint)(delegate* unmanaged<long, long, long, long, long, long>)&CodeThunk_PsxTaskScriptHandler, // PsxTaskScriptHandler
        // 0x491d0 __atonexitinit: is excluded from the plan (library); until then only a DLL in shared mode can hand out the original's code address
        // 0x492e8 _purecall: is excluded from the plan (thunk); until then only a DLL in shared mode can hand out the original's code address
        // 0x492f4 ??_Etype_info@@UEAAPEAXI@Z: is excluded from the plan (library); until then only a DLL in shared mode can hand out the original's code address
        // 0x49594 pre_c_init: is excluded from the plan (library); until then only a DLL in shared mode can hand out the original's code address
        _ => 0,
    };
    /// <summary>Unmanaged entry of Native.MipsJumpTargetOffset (MipsJumpTargetOffset @ 0x18000d140) for CodeAt and Code_MipsJumpTargetOffset.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MipsJumpTargetOffset(long a0, int a1) => MipsJumpTargetOffset(a0, a1);
    /// <summary>Unmanaged entry of Native.MipsDecodeImm16Rs (MipsDecodeImm16Rs @ 0x18000d180) for CodeAt and Code_MipsDecodeImm16Rs.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MipsDecodeImm16Rs(long a0, int a1, int* a2) => MipsDecodeImm16Rs(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.SfxBufferReset (SfxBufferReset @ 0x18000d520) for CodeAt and Code_SfxBufferReset.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static ulong* CodeThunk_SfxBufferReset(ulong* a0) => SfxBufferReset(a0);
    /// <summary>Unmanaged entry of Native.SfxBufferFreeData (SfxBufferFreeData @ 0x18000d530) for CodeAt and Code_SfxBufferFreeData.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void CodeThunk_SfxBufferFreeData(long a0) => SfxBufferFreeData(a0);
    /// <summary>Unmanaged entry of Native.PsxTaskScriptHandler (PsxTaskScriptHandler @ 0x180049170) for CodeAt and Code_PsxTaskScriptHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxTaskScriptHandler(long a0, long a1, long a2, long a3, long a4) => PsxTaskScriptHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.InitMapperTable (InitMapperTable @ 0x180001030) for CodeAt and Code_InitMapperTable.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_InitMapperTable() => InitMapperTable();
    /// <summary>Unmanaged entry of Native.HostSendRect64 (HostSendRect64 @ 0x180002cd0) for CodeAt and Code_HostSendRect64.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostSendRect64(short* a0, long a1) => HostSendRect64(a0, a1);
    /// <summary>Unmanaged entry of Native.HostSendRect65 (HostSendRect65 @ 0x180002d20) for CodeAt and Code_HostSendRect65.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostSendRect65(short* a0, long a1) => HostSendRect65(a0, a1);
    /// <summary>Unmanaged entry of Native.InitE7Packet (InitE7Packet @ 0x180002f70) for CodeAt and Code_InitE7Packet.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_InitE7Packet(long a0, short* a1, short a2, short a3) => InitE7Packet(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.sub_180002FD0 (sub_180002FD0 @ 0x180002fd0) for CodeAt and Code_sub_180002FD0.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_sub_180002FD0() => sub_180002FD0();
    /// <summary>Unmanaged entry of Native.HostSendVramRect66 (HostSendVramRect66 @ 0x180002fe0) for CodeAt and Code_HostSendVramRect66.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostSendVramRect66(short* a0, uint a1, long a2) => HostSendVramRect66(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.GteLoadAverageShort12 (GteLoadAverageShort12 @ 0x1800030b0) for CodeAt and Code_GteLoadAverageShort12.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_GteLoadAverageShort12(short* a0, short* a1, int a2, int a3, ushort* a4) => GteLoadAverageShort12(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.PsxRcos (PsxRcos @ 0x180003190) for CodeAt and Code_PsxRcos.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxRcos(int a0) => PsxRcos(a0);
    /// <summary>Unmanaged entry of Native.PsxRsin (PsxRsin @ 0x1800031d0) for CodeAt and Code_PsxRsin.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxRsin(int a0) => PsxRsin(a0);
    /// <summary>Unmanaged entry of Native.PsxRatan2 (PsxRatan2 @ 0x180003210) for CodeAt and Code_PsxRatan2.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxRatan2(int a0, int a1) => PsxRatan2(a0, a1);
    /// <summary>Unmanaged entry of Native.GteSetRotMatrix (GteSetRotMatrix @ 0x180003250) for CodeAt and Code_GteSetRotMatrix.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_GteSetRotMatrix(long a0) => GteSetRotMatrix(a0);
    /// <summary>Unmanaged entry of Native.GteSetTransMatrix (GteSetTransMatrix @ 0x180003280) for CodeAt and Code_GteSetTransMatrix.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_GteSetTransMatrix(uint* a0) => GteSetTransMatrix(a0);
    /// <summary>Unmanaged entry of Native.PsxSquareRoot0 (PsxSquareRoot0 @ 0x1800032a0) for CodeAt and Code_PsxSquareRoot0.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxSquareRoot0(int a0) => PsxSquareRoot0(a0);
    /// <summary>Unmanaged entry of Native.RotMatrixYXZ (RotMatrixYXZ @ 0x1800039d0) for CodeAt and Code_RotMatrixYXZ.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_RotMatrixYXZ(short* a0, long a1) => RotMatrixYXZ(a0, a1);
    /// <summary>Unmanaged entry of Native.ScaleMatrix (ScaleMatrix @ 0x180003a20) for CodeAt and Code_ScaleMatrix.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static short* CodeThunk_ScaleMatrix(short* a0, uint* a1) => ScaleMatrix(a0, a1);
    /// <summary>Unmanaged entry of Native.MulRotMatrix (MulRotMatrix @ 0x180003ab0) for CodeAt and Code_MulRotMatrix.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MulRotMatrix(short* a0) => MulRotMatrix(a0);
    /// <summary>Unmanaged entry of Native.CompMatrix (CompMatrix @ 0x180003cb0) for CodeAt and Code_CompMatrix.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_CompMatrix(long a0, short* a1, long a2) => CompMatrix(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.MipsDecodeRs (MipsDecodeRs @ 0x18000d0d0) for CodeAt and Code_MipsDecodeRs.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MipsDecodeRs(long a0, uint a1) => MipsDecodeRs(a0, a1);
    /// <summary>Unmanaged entry of Native.MipsDecodeRt (MipsDecodeRt @ 0x18000d0e0) for CodeAt and Code_MipsDecodeRt.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MipsDecodeRt(long a0, uint a1) => MipsDecodeRt(a0, a1);
    /// <summary>Unmanaged entry of Native.MipsDecodeRd (MipsDecodeRd @ 0x18000d0f0) for CodeAt and Code_MipsDecodeRd.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MipsDecodeRd(long a0, ushort a1) => MipsDecodeRd(a0, a1);
    /// <summary>Unmanaged entry of Native.MipsDecodeShamt (MipsDecodeShamt @ 0x18000d100) for CodeAt and Code_MipsDecodeShamt.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MipsDecodeShamt(long a0, uint a1) => MipsDecodeShamt(a0, a1);
    /// <summary>Unmanaged entry of Native.MipsDecodeCode20 (MipsDecodeCode20 @ 0x18000d110) for CodeAt and Code_MipsDecodeCode20.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MipsDecodeCode20(long a0, uint a1) => MipsDecodeCode20(a0, a1);
    /// <summary>Unmanaged entry of Native.MipsDecodeSimm16 (MipsDecodeSimm16 @ 0x18000d120) for CodeAt and Code_MipsDecodeSimm16.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MipsDecodeSimm16(long a0, ushort a1) => MipsDecodeSimm16(a0, a1);
    /// <summary>Unmanaged entry of Native.MipsDecodeImm16 (MipsDecodeImm16 @ 0x18000d130) for CodeAt and Code_MipsDecodeImm16.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MipsDecodeImm16(long a0, ushort a1) => MipsDecodeImm16(a0, a1);
    /// <summary>Unmanaged entry of Native.MipsDecodeImm25 (MipsDecodeImm25 @ 0x18000d150) for CodeAt and Code_MipsDecodeImm25.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MipsDecodeImm25(long a0, int a1) => MipsDecodeImm25(a0, a1);
    /// <summary>Unmanaged entry of Native.MipsBranchTarget (MipsBranchTarget @ 0x18000d160) for CodeAt and Code_MipsBranchTarget.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MipsBranchTarget(int a0, ushort a1) => MipsBranchTarget(a0, a1);
    /// <summary>Unmanaged entry of Native.HiDebugPsgData (HiDebugPsgData @ 0x180015200) for CodeAt and Code_HiDebugPsgData.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HiDebugPsgData(int a0, sbyte a1) => HiDebugPsgData(a0, a1);
    /// <summary>Unmanaged entry of Native.InitEffModelSlots (InitEffModelSlots @ 0x180015940) for CodeAt and Code_InitEffModelSlots.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static ulong* CodeThunk_InitEffModelSlots(long a0, long a1) => InitEffModelSlots(a0, a1);
    /// <summary>Unmanaged entry of Native.FreeEffModelSlot (FreeEffModelSlot @ 0x1800159a0) for CodeAt and Code_FreeEffModelSlot.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void* CodeThunk_FreeEffModelSlot(int a0) => FreeEffModelSlot(a0);
    /// <summary>Unmanaged entry of Native.ResetEffModelSlots (ResetEffModelSlots @ 0x1800159f0) for CodeAt and Code_ResetEffModelSlots.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static ushort* CodeThunk_ResetEffModelSlots() => ResetEffModelSlots();
    /// <summary>Unmanaged entry of Native.InitModelTable88 (InitModelTable88 @ 0x180015a20) for CodeAt and Code_InitModelTable88.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long* CodeThunk_InitModelTable88(long a0, long a1) => InitModelTable88(a0, a1);
    /// <summary>Unmanaged entry of Native.ResetSummonSlot (ResetSummonSlot @ 0x180015aa0) for CodeAt and Code_ResetSummonSlot.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_ResetSummonSlot() => ResetSummonSlot();
    /// <summary>Unmanaged entry of Native.RegisterSolidEffModel (RegisterSolidEffModel @ 0x180015ac0) for CodeAt and Code_RegisterSolidEffModel.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_RegisterSolidEffModel(ulong a0) => RegisterSolidEffModel(a0);
    /// <summary>Unmanaged entry of Native.RegisterGouEffModel (RegisterGouEffModel @ 0x180015b70) for CodeAt and Code_RegisterGouEffModel.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_RegisterGouEffModel(ulong a0) => RegisterGouEffModel(a0);
    /// <summary>Unmanaged entry of Native.RegisterTexEffModel (RegisterTexEffModel @ 0x180015c20) for CodeAt and Code_RegisterTexEffModel.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_RegisterTexEffModel(ulong a0, short a1, short a2, short a3, short a4) => RegisterTexEffModel(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.RegisterTexListModel (RegisterTexListModel @ 0x180015d30) for CodeAt and Code_RegisterTexListModel.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_RegisterTexListModel(ulong a0, long a1, long a2) => RegisterTexListModel(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.RegisterTexPtrModel (RegisterTexPtrModel @ 0x180015e10) for CodeAt and Code_RegisterTexPtrModel.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_RegisterTexPtrModel(ulong a0, long a1, long a2) => RegisterTexPtrModel(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.RegisterSummonModel (RegisterSummonModel @ 0x180015ee0) for CodeAt and Code_RegisterSummonModel.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_RegisterSummonModel(long a0, long a1, long a2) => RegisterSummonModel(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.DrawEffModel (DrawEffModel @ 0x180016150) for CodeAt and Code_DrawEffModel.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_DrawEffModel(long a0, long a1, long a2, int a3) => DrawEffModel(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.DrawSliceEffModel (DrawSliceEffModel @ 0x180016570) for CodeAt and Code_DrawSliceEffModel.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_DrawSliceEffModel(long a0, long a1, long a2, int a3) => DrawSliceEffModel(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.DrawEffModelByBone (DrawEffModelByBone @ 0x1800167f0) for CodeAt and Code_DrawEffModelByBone.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_DrawEffModelByBone(short* a0, int a1, int a2, int a3) => DrawEffModelByBone(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.DrawMorphEffModel (DrawMorphEffModel @ 0x180016cc0) for CodeAt and Code_DrawMorphEffModel.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_DrawMorphEffModel(long a0, long a1, long a2, int a3, int a4, ushort a5) => DrawMorphEffModel(a0, a1, a2, a3, a4, a5);
    /// <summary>Unmanaged entry of Native.DrawMorphModelByBone (DrawMorphModelByBone @ 0x180017190) for CodeAt and Code_DrawMorphModelByBone.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_DrawMorphModelByBone(short* a0, int a1, int a2, ushort a3, int a4, int a5) => DrawMorphModelByBone(a0, a1, a2, a3, a4, a5);
    /// <summary>Unmanaged entry of Native.DrawSummonModel (DrawSummonModel @ 0x180017710) for CodeAt and Code_DrawSummonModel.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_DrawSummonModel(long a0, long a1, long a2, int a3, sbyte a4) => DrawSummonModel(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SetSummonMotion (SetSummonMotion @ 0x180017a10) for CodeAt and Code_SetSummonMotion.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SetSummonMotion(long a0, int a1) => SetSummonMotion(a0, a1);
    /// <summary>Unmanaged entry of Native.SetSummonMotFrame (SetSummonMotFrame @ 0x180017a70) for CodeAt and Code_SetSummonMotFrame.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SetSummonMotFrame(int a0, int a1) => SetSummonMotFrame(a0, a1);
    /// <summary>Unmanaged entry of Native.SplitMdlVertex (SplitMdlVertex @ 0x180017ae0) for CodeAt and Code_SplitMdlVertex.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SplitMdlVertex(int a0, long a1, long a2) => SplitMdlVertex(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.GetSplitMdlVertex (GetSplitMdlVertex @ 0x180017b30) for CodeAt and Code_GetSplitMdlVertex.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_GetSplitMdlVertex(int a0, int a1) => GetSplitMdlVertex(a0, a1);
    /// <summary>Unmanaged entry of Native.GetMdlVertexPtr (GetMdlVertexPtr @ 0x180018000) for CodeAt and Code_GetMdlVertexPtr.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_GetMdlVertexPtr(int a0, ushort a1, ulong a2, uint* a3, int a4) => GetMdlVertexPtr(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.nullsub_1 (nullsub_1 @ 0x1800182c0) for CodeAt and Code_nullsub_1.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void CodeThunk_nullsub_1() => nullsub_1();
    /// <summary>Unmanaged entry of Native.HostSendCommand0EMode0 (HostSendCommand0EMode0 @ 0x1800182d0) for CodeAt and Code_HostSendCommand0EMode0.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostSendCommand0EMode0(long a0, uint a1, long a2) => HostSendCommand0EMode0(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.HostSendCommand0EMode0FromBlock (HostSendCommand0EMode0FromBlock @ 0x180018340) for CodeAt and Code_HostSendCommand0EMode0FromBlock.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostSendCommand0EMode0FromBlock(long a0, int a1, long a2) => HostSendCommand0EMode0FromBlock(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.HostSendCommand0EMode2 (HostSendCommand0EMode2 @ 0x180018440) for CodeAt and Code_HostSendCommand0EMode2.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostSendCommand0EMode2(long a0, uint a1, long a2) => HostSendCommand0EMode2(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.HostSendCommand0EMode2FromBlock (HostSendCommand0EMode2FromBlock @ 0x1800184b0) for CodeAt and Code_HostSendCommand0EMode2FromBlock.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostSendCommand0EMode2FromBlock(long a0, int a1, long a2) => HostSendCommand0EMode2FromBlock(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.GetSummonBonePos (GetSummonBonePos @ 0x1800185b0) for CodeAt and Code_GetSummonBonePos.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_GetSummonBonePos(int a0, int a1, ushort* a2) => GetSummonBonePos(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.GetSummonBoneMatrix (GetSummonBoneMatrix @ 0x180018630) for CodeAt and Code_GetSummonBoneMatrix.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_GetSummonBoneMatrix(int a0, int a1, void* a2) => GetSummonBoneMatrix(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.ShowSummonModelMesh (ShowSummonModelMesh @ 0x1800187e0) for CodeAt and Code_ShowSummonModelMesh.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_ShowSummonModelMesh(int a0, sbyte a1) => ShowSummonModelMesh(a0, a1);
    /// <summary>Unmanaged entry of Native.HideSummonModelMesh (HideSummonModelMesh @ 0x180018840) for CodeAt and Code_HideSummonModelMesh.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HideSummonModelMesh(int a0, sbyte a1) => HideSummonModelMesh(a0, a1);
    /// <summary>Unmanaged entry of Native.StartSummonTexAnim (StartSummonTexAnim @ 0x1800188a0) for CodeAt and Code_StartSummonTexAnim.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_StartSummonTexAnim(int a0, int a1, sbyte a2) => StartSummonTexAnim(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.StopSummonTexAnim (StopSummonTexAnim @ 0x180018930) for CodeAt and Code_StopSummonTexAnim.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_StopSummonTexAnim(int a0, int a1) => StopSummonTexAnim(a0, a1);
    /// <summary>Unmanaged entry of Native.HiModifyEffModelAbr (HiModifyEffModelAbr @ 0x180018990) for CodeAt and Code_HiModifyEffModelAbr.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void CodeThunk_HiModifyEffModelAbr(int a0, int a1) => HiModifyEffModelAbr(a0, a1);
    /// <summary>Unmanaged entry of Native.ModifyEffModelRgb (ModifyEffModelRgb @ 0x1800189f0) for CodeAt and Code_ModifyEffModelRgb.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void CodeThunk_ModifyEffModelRgb(int a0, ushort a1, ushort a2, ushort a3) => ModifyEffModelRgb(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.SetEffModelOffset (SetEffModelOffset @ 0x180018a40) for CodeAt and Code_SetEffModelOffset.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void* CodeThunk_SetEffModelOffset(int a0, short a1) => SetEffModelOffset(a0, a1);
    /// <summary>Unmanaged entry of Native.SetEffModelSlice (SetEffModelSlice @ 0x180018a90) for CodeAt and Code_SetEffModelSlice.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void* CodeThunk_SetEffModelSlice(int a0, short a1) => SetEffModelSlice(a0, a1);
    /// <summary>Unmanaged entry of Native.ModifySummonModelAbr (ModifySummonModelAbr @ 0x180018af0) for CodeAt and Code_ModifySummonModelAbr.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void CodeThunk_ModifySummonModelAbr(int a0, int a1) => ModifySummonModelAbr(a0, a1);
    /// <summary>Unmanaged entry of Native.ModifySummonModelRgb (ModifySummonModelRgb @ 0x180018b50) for CodeAt and Code_ModifySummonModelRgb.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void CodeThunk_ModifySummonModelRgb(int a0, ushort a1, ushort a2, ushort a3) => ModifySummonModelRgb(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.HostObjCmd1A (HostObjCmd1A @ 0x180018ba0) for CodeAt and Code_HostObjCmd1A.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjCmd1A(long a0) => HostObjCmd1A(a0);
    /// <summary>Unmanaged entry of Native.EffModelPartCount (EffModelPartCount @ 0x180018c00) for CodeAt and Code_EffModelPartCount.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_EffModelPartCount(int a0) => EffModelPartCount(a0);
    /// <summary>Unmanaged entry of Native.FillIdTable435 (FillIdTable435 @ 0x180018ce0) for CodeAt and Code_FillIdTable435.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_FillIdTable435(ushort* a0) => FillIdTable435(a0);
    /// <summary>Unmanaged entry of Native.StoreBytePairA (StoreBytePairA @ 0x180018fd0) for CodeAt and Code_StoreBytePairA.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_StoreBytePairA(int a0, long a1, long a2, long a3, byte* a4) => StoreBytePairA(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.StoreBytePairB (StoreBytePairB @ 0x180019000) for CodeAt and Code_StoreBytePairB.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_StoreBytePairB(int a0, long a1, long a2, long a3, byte* a4) => StoreBytePairB(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjCmd1EHandler (SfxObjCmd1EHandler @ 0x180019030) for CodeAt and Code_SfxObjCmd1EHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjCmd1EHandler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxObjCmd1EHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjCmd1FHandler (SfxObjCmd1FHandler @ 0x1800190a0) for CodeAt and Code_SfxObjCmd1FHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjCmd1FHandler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxObjCmd1FHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjCmd1DHandler (SfxObjCmd1DHandler @ 0x180019130) for CodeAt and Code_SfxObjCmd1DHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjCmd1DHandler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxObjCmd1DHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjCmd0CHandler (SfxObjCmd0CHandler @ 0x180019170) for CodeAt and Code_SfxObjCmd0CHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjCmd0CHandler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxObjCmd0CHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxWaitObjCmd09Handler (SfxWaitObjCmd09Handler @ 0x1800191f0) for CodeAt and Code_SfxWaitObjCmd09Handler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxWaitObjCmd09Handler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxWaitObjCmd09Handler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxWaitObjCmd09OffsetHandler (SfxWaitObjCmd09OffsetHandler @ 0x180019230) for CodeAt and Code_SfxWaitObjCmd09OffsetHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxWaitObjCmd09OffsetHandler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxWaitObjCmd09OffsetHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxWaitObjCmd0C17Handler (SfxWaitObjCmd0C17Handler @ 0x180019280) for CodeAt and Code_SfxWaitObjCmd0C17Handler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxWaitObjCmd0C17Handler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxWaitObjCmd0C17Handler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxWaitObjCmd0C17OffsetHandler (SfxWaitObjCmd0C17OffsetHandler @ 0x1800192c0) for CodeAt and Code_SfxWaitObjCmd0C17OffsetHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxWaitObjCmd0C17OffsetHandler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxWaitObjCmd0C17OffsetHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxWaitObjCmd0C19Handler (SfxWaitObjCmd0C19Handler @ 0x180019310) for CodeAt and Code_SfxWaitObjCmd0C19Handler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxWaitObjCmd0C19Handler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxWaitObjCmd0C19Handler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjResetHandler (SfxObjResetHandler @ 0x180019350) for CodeAt and Code_SfxObjResetHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjResetHandler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxObjResetHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjCmd21Handler (SfxObjCmd21Handler @ 0x1800193e0) for CodeAt and Code_SfxObjCmd21Handler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjCmd21Handler(long a0, byte* a1, long a2, long a3, byte* a4) => SfxObjCmd21Handler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpCmd09Wait (SfxOpCmd09Wait @ 0x180019430) for CodeAt and Code_SfxOpCmd09Wait.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpCmd09Wait(int a0, byte* a1, long a2, long a3, byte* a4) => SfxOpCmd09Wait(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpHostCmd0D (SfxOpHostCmd0D @ 0x180019470) for CodeAt and Code_SfxOpHostCmd0D.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpHostCmd0D(int a0, byte* a1, long a2, long a3, byte* a4) => SfxOpHostCmd0D(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpMoveObjToObj (SfxOpMoveObjToObj @ 0x1800194f0) for CodeAt and Code_SfxOpMoveObjToObj.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpMoveObjToObj(int a0, byte* a1, long a2, long a3, byte* a4) => SfxOpMoveObjToObj(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpMoveObjToQueryPos (SfxOpMoveObjToQueryPos @ 0x1800195a0) for CodeAt and Code_SfxOpMoveObjToQueryPos.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpMoveObjToQueryPos(int a0, byte* a1, long a2, long a3, byte* a4) => SfxOpMoveObjToQueryPos(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpMoveCtx16ToObj (SfxOpMoveCtx16ToObj @ 0x1800195e0) for CodeAt and Code_SfxOpMoveCtx16ToObj.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpMoveCtx16ToObj(int a0, long a1, long a2, long a3, byte* a4) => SfxOpMoveCtx16ToObj(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpCtx16Advance (SfxOpCtx16Advance @ 0x1800196f0) for CodeAt and Code_SfxOpCtx16Advance.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpCtx16Advance(int a0, long a1, long a2, long a3, byte* a4) => SfxOpCtx16Advance(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpCtx16ToQueryPos (SfxOpCtx16ToQueryPos @ 0x1800197a0) for CodeAt and Code_SfxOpCtx16ToQueryPos.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpCtx16ToQueryPos(int a0, byte* a1, long a2, long a3, byte* a4) => SfxOpCtx16ToQueryPos(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpMoveObjToAnchor (SfxOpMoveObjToAnchor @ 0x180019830) for CodeAt and Code_SfxOpMoveObjToAnchor.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpMoveObjToAnchor(int a0, long a1, long a2, long a3, byte* a4) => SfxOpMoveObjToAnchor(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpCtx16BlendNearObj (SfxOpCtx16BlendNearObj @ 0x180019880) for CodeAt and Code_SfxOpCtx16BlendNearObj.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpCtx16BlendNearObj(int a0, long a1, long a2, long a3, byte* a4) => SfxOpCtx16BlendNearObj(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpCtx16MoveNearObj (SfxOpCtx16MoveNearObj @ 0x180019990) for CodeAt and Code_SfxOpCtx16MoveNearObj.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpCtx16MoveNearObj(int a0, long a1, long a2, long a3, byte* a4) => SfxOpCtx16MoveNearObj(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjPairTaskHandler (SfxObjPairTaskHandler @ 0x180019a90) for CodeAt and Code_SfxObjPairTaskHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjPairTaskHandler(int a0, long a1, long a2, long a3, byte* a4) => SfxObjPairTaskHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjRotateHandler (SfxObjRotateHandler @ 0x180019b80) for CodeAt and Code_SfxObjRotateHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjRotateHandler(int a0, long a1, long a2, long a3, byte* a4) => SfxObjRotateHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjSetFacingHandler (SfxObjSetFacingHandler @ 0x180019c20) for CodeAt and Code_SfxObjSetFacingHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjSetFacingHandler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxObjSetFacingHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjAnimWaitHandler (SfxObjAnimWaitHandler @ 0x180019c80) for CodeAt and Code_SfxObjAnimWaitHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjAnimWaitHandler(int a0, byte* a1, long a2, int* a3, byte* a4) => SfxObjAnimWaitHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjFlagsDifferWaitHandler (SfxObjFlagsDifferWaitHandler @ 0x180019d70) for CodeAt and Code_SfxObjFlagsDifferWaitHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjFlagsDifferWaitHandler(int a0, byte* a1, long a2, uint* a3, byte* a4) => SfxObjFlagsDifferWaitHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjFlagsMatchWaitHandler (SfxObjFlagsMatchWaitHandler @ 0x180019e50) for CodeAt and Code_SfxObjFlagsMatchWaitHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjFlagsMatchWaitHandler(int a0, byte* a1, long a2, uint* a3, byte* a4) => SfxObjFlagsMatchWaitHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjCallKindScriptHandler (SfxObjCallKindScriptHandler @ 0x180019f30) for CodeAt and Code_SfxObjCallKindScriptHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjCallKindScriptHandler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxObjCallKindScriptHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjFlagsMatchHandler (SfxObjFlagsMatchHandler @ 0x180019fc0) for CodeAt and Code_SfxObjFlagsMatchHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjFlagsMatchHandler(int a0, byte* a1, long a2, long a3, byte* a4) => SfxObjFlagsMatchHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.HostObjCmd0CIfFlag (HostObjCmd0CIfFlag @ 0x18001b240) for CodeAt and Code_HostObjCmd0CIfFlag.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjCmd0CIfFlag(long a0, byte a1) => HostObjCmd0CIfFlag(a0, a1);
    /// <summary>Unmanaged entry of Native.HostObjCmd0CReset (HostObjCmd0CReset @ 0x18001b310) for CodeAt and Code_HostObjCmd0CReset.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjCmd0CReset(long a0, sbyte a1) => HostObjCmd0CReset(a0, a1);
    /// <summary>Unmanaged entry of Native.HostObjCmd0CU32 (HostObjCmd0CU32 @ 0x18001b380) for CodeAt and Code_HostObjCmd0CU32.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjCmd0CU32(long a0, uint a1) => HostObjCmd0CU32(a0, a1);
    /// <summary>Unmanaged entry of Native.ReturnArg2 (ReturnArg2 @ 0x18001b4a0) for CodeAt and Code_ReturnArg2.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_ReturnArg2(long a0, int a1) => ReturnArg2(a0, a1);
    /// <summary>Unmanaged entry of Native.HostObjCmd0B (HostObjCmd0B @ 0x18001b4b0) for CodeAt and Code_HostObjCmd0B.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjCmd0B(long a0, uint a1) => HostObjCmd0B(a0, a1);
    /// <summary>Unmanaged entry of Native.HostObjCmd09 (HostObjCmd09 @ 0x18001b500) for CodeAt and Code_HostObjCmd09.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjCmd09(long a0) => HostObjCmd09(a0);
    /// <summary>Unmanaged entry of Native.HostObjCmd0A (HostObjCmd0A @ 0x18001b560) for CodeAt and Code_HostObjCmd0A.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjCmd0A(long a0) => HostObjCmd0A(a0);
    /// <summary>Unmanaged entry of Native.HostObjSetFacing (HostObjSetFacing @ 0x18001b5c0) for CodeAt and Code_HostObjSetFacing.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjSetFacing(long a0, int a1) => HostObjSetFacing(a0, a1);
    /// <summary>Unmanaged entry of Native.HostObjCmd0DOn (HostObjCmd0DOn @ 0x18001b870) for CodeAt and Code_HostObjCmd0DOn.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjCmd0DOn(long a0) => HostObjCmd0DOn(a0);
    /// <summary>Unmanaged entry of Native.HostObjCmd0D (HostObjCmd0D @ 0x18001b8d0) for CodeAt and Code_HostObjCmd0D.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjCmd0D(long a0, uint a1) => HostObjCmd0D(a0, a1);
    /// <summary>Unmanaged entry of Native.HostObjCmd12 (HostObjCmd12 @ 0x18001b980) for CodeAt and Code_HostObjCmd12.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjCmd12(long a0, int a1) => HostObjCmd12(a0, a1);
    /// <summary>Unmanaged entry of Native.HostObjCmd13 (HostObjCmd13 @ 0x18001b9e0) for CodeAt and Code_HostObjCmd13.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjCmd13(long a0, uint a1) => HostObjCmd13(a0, a1);
    /// <summary>Unmanaged entry of Native.HostObjQuery14IsZero (HostObjQuery14IsZero @ 0x18001ba30) for CodeAt and Code_HostObjQuery14IsZero.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjQuery14IsZero(long a0) => HostObjQuery14IsZero(a0);
    /// <summary>Unmanaged entry of Native.ReturnMinusOne (ReturnMinusOne @ 0x18001bab0) for CodeAt and Code_ReturnMinusOne.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_ReturnMinusOne() => ReturnMinusOne();
    /// <summary>Unmanaged entry of Native.VecDistance3 (VecDistance3 @ 0x1800208d0) for CodeAt and Code_VecDistance3.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_VecDistance3(short* a0, short* a1) => VecDistance3(a0, a1);
    /// <summary>Unmanaged entry of Native.PsxRand (PsxRand @ 0x180020930) for CodeAt and Code_PsxRand.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxRand() => PsxRand();
    /// <summary>Unmanaged entry of Native.PsxRandRange (PsxRandRange @ 0x180020950) for CodeAt and Code_PsxRandRange.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxRandRange(uint a0, int a1) => PsxRandRange(a0, a1);
    /// <summary>Unmanaged entry of Native.PsxRandCentered (PsxRandCentered @ 0x180020980) for CodeAt and Code_PsxRandCentered.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxRandCentered(int a0) => PsxRandCentered(a0);
    /// <summary>Unmanaged entry of Native.PsxRandScatterVec (PsxRandScatterVec @ 0x1800209b0) for CodeAt and Code_PsxRandScatterVec.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxRandScatterVec(long a0) => PsxRandScatterVec(a0);
    /// <summary>Unmanaged entry of Native.LookAnglesBetween (LookAnglesBetween @ 0x180020a20) for CodeAt and Code_LookAnglesBetween.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_LookAnglesBetween(short* a0, short* a1, ushort* a2) => LookAnglesBetween(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.GteIntplScalar (GteIntplScalar @ 0x180020bd0) for CodeAt and Code_GteIntplScalar.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_GteIntplScalar(int a0, int a1, int a2) => GteIntplScalar(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.LerpIntRatio (LerpIntRatio @ 0x180020c00) for CodeAt and Code_LerpIntRatio.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_LerpIntRatio(int a0, int a1, int a2, int a3) => LerpIntRatio(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.GteIntplVector (GteIntplVector @ 0x180020c50) for CodeAt and Code_GteIntplVector.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_GteIntplVector(short* a0, short* a1, int a2, ushort* a3) => GteIntplVector(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.ParticleStepMotion (ParticleStepMotion @ 0x18002fc80) for CodeAt and Code_ParticleStepMotion.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_ParticleStepMotion(ulong a0, short a1, ulong a2, ulong a3) => ParticleStepMotion(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.SfxPoolsReset (SfxPoolsReset @ 0x180030620) for CodeAt and Code_SfxPoolsReset.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxPoolsReset() => SfxPoolsReset();
    /// <summary>Unmanaged entry of Native.SfxTickParticleSystem (SfxTickParticleSystem @ 0x1800306b0) for CodeAt and Code_SfxTickParticleSystem.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxTickParticleSystem() => SfxTickParticleSystem();
    /// <summary>Unmanaged entry of Native.CreateTrailEffect (CreateTrailEffect @ 0x1800306f0) for CodeAt and Code_CreateTrailEffect.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_CreateTrailEffect(long a0, long a1) => CreateTrailEffect(a0, a1);
    /// <summary>Unmanaged entry of Native.MoveSfxObject (MoveSfxObject @ 0x180030710) for CodeAt and Code_MoveSfxObject.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MoveSfxObject(long a0, long a1) => MoveSfxObject(a0, a1);
    /// <summary>Unmanaged entry of Native.DetachSfxObject (DetachSfxObject @ 0x180030730) for CodeAt and Code_DetachSfxObject.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void CodeThunk_DetachSfxObject(long a0) => DetachSfxObject(a0);
    /// <summary>Unmanaged entry of Native.ComputeArenaBytes (ComputeArenaBytes @ 0x180030780) for CodeAt and Code_ComputeArenaBytes.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_ComputeArenaBytes(int a0, int a1) => ComputeArenaBytes(a0, a1);
    /// <summary>Unmanaged entry of Native.PsxInitEntryBlock (PsxInitEntryBlock @ 0x1800307a0) for CodeAt and Code_PsxInitEntryBlock.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static uint* CodeThunk_PsxInitEntryBlock(uint a0, long a1, int a2, int a3, int a4) => PsxInitEntryBlock(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.ClearPoolSlotHeaders (ClearPoolSlotHeaders @ 0x180030880) for CodeAt and Code_ClearPoolSlotHeaders.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void CodeThunk_ClearPoolSlotHeaders(uint a0) => ClearPoolSlotHeaders(a0);
    /// <summary>Unmanaged entry of Native.SfxStartScriptSlot (SfxStartScriptSlot @ 0x180030930) for CodeAt and Code_SfxStartScriptSlot.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static ushort* CodeThunk_SfxStartScriptSlot(uint a0) => SfxStartScriptSlot(a0);
    /// <summary>Unmanaged entry of Native.SfxTickScriptSlots (SfxTickScriptSlots @ 0x180030a70) for CodeAt and Code_SfxTickScriptSlots.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxTickScriptSlots(uint a0) => SfxTickScriptSlots(a0);
    /// <summary>Unmanaged entry of Native.FindPairSlot (FindPairSlot @ 0x180030bd0) for CodeAt and Code_FindPairSlot.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_FindPairSlot(int a0) => FindPairSlot(a0);
    /// <summary>Unmanaged entry of Native.sub_1800313D0 (sub_1800313D0 @ 0x1800313d0) for CodeAt and Code_sub_1800313D0.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_sub_1800313D0() => sub_1800313D0();
    /// <summary>Unmanaged entry of Native.SeqStateBegin (SeqStateBegin @ 0x1800313f0) for CodeAt and Code_SeqStateBegin.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SeqStateBegin(short a0) => SeqStateBegin(a0);
    /// <summary>Unmanaged entry of Native.PsxSelectTableRecord (PsxSelectTableRecord @ 0x1800314f0) for CodeAt and Code_PsxSelectTableRecord.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static byte* CodeThunk_PsxSelectTableRecord(int a0) => PsxSelectTableRecord(a0);
    /// <summary>Unmanaged entry of Native.SeqStateUpdate (SeqStateUpdate @ 0x180031520) for CodeAt and Code_SeqStateUpdate.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void CodeThunk_SeqStateUpdate() => SeqStateUpdate();
    /// <summary>Unmanaged entry of Native.SfxVoiceStop (SfxVoiceStop @ 0x180032260) for CodeAt and Code_SfxVoiceStop.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxVoiceStop(int a0) => SfxVoiceStop(a0);
    /// <summary>Unmanaged entry of Native.SfxVoiceSetSource (SfxVoiceSetSource @ 0x1800322e0) for CodeAt and Code_SfxVoiceSetSource.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void CodeThunk_SfxVoiceSetSource(int a0, int a1) => SfxVoiceSetSource(a0, a1);
    /// <summary>Unmanaged entry of Native.SfxVoiceSetAllSources (SfxVoiceSetAllSources @ 0x180032310) for CodeAt and Code_SfxVoiceSetAllSources.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxVoiceSetAllSources(uint a0) => SfxVoiceSetAllSources(a0);
    /// <summary>Unmanaged entry of Native.SfxVoicePlay (SfxVoicePlay @ 0x1800323a0) for CodeAt and Code_SfxVoicePlay.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxVoicePlay(int a0, uint a1, uint a2) => SfxVoicePlay(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.SfxVoiceRestart (SfxVoiceRestart @ 0x1800324b0) for CodeAt and Code_SfxVoiceRestart.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxVoiceRestart(int a0, uint a1, uint a2, int a3) => SfxVoiceRestart(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.InitPool56 (InitPool56 @ 0x1800324f0) for CodeAt and Code_InitPool56.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static byte* CodeThunk_InitPool56(int a0, long a1, long a2) => InitPool56(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.SfxIndexedRecordPtr (SfxIndexedRecordPtr @ 0x180032540) for CodeAt and Code_SfxIndexedRecordPtr.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static byte* CodeThunk_SfxIndexedRecordPtr(long a0, long a1) => SfxIndexedRecordPtr(a0, a1);
    /// <summary>Unmanaged entry of Native.SfxUpdateParticles (SfxUpdateParticles @ 0x1800325f0) for CodeAt and Code_SfxUpdateParticles.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxUpdateParticles(long a0) => SfxUpdateParticles(a0);
    /// <summary>Unmanaged entry of Native.InitPool108 (InitPool108 @ 0x180034330) for CodeAt and Code_InitPool108.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static ushort* CodeThunk_InitPool108(int a0, long a1) => InitPool108(a0, a1);
    /// <summary>Unmanaged entry of Native.CreateRibbonEffectSlot (CreateRibbonEffectSlot @ 0x180034380) for CodeAt and Code_CreateRibbonEffectSlot.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_CreateRibbonEffectSlot(long a0, long a1, ulong a2, ulong a3) => CreateRibbonEffectSlot(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.MoveObjectWithAttachments (MoveObjectWithAttachments @ 0x180034710) for CodeAt and Code_MoveObjectWithAttachments.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_MoveObjectWithAttachments(long a0, long a1, short* a2) => MoveObjectWithAttachments(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.Pool56ReleaseOwner (Pool56ReleaseOwner @ 0x180034820) for CodeAt and Code_Pool56ReleaseOwner.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_Pool56ReleaseOwner(long a0, long a1) => Pool56ReleaseOwner(a0, a1);
    /// <summary>Unmanaged entry of Native.SfxUpdateKeyframeTracks (SfxUpdateKeyframeTracks @ 0x180034860) for CodeAt and Code_SfxUpdateKeyframeTracks.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxUpdateKeyframeTracks(long a0, long a1) => SfxUpdateKeyframeTracks(a0, a1);
    /// <summary>Unmanaged entry of Native.Pool56FindFree (Pool56FindFree @ 0x180035490) for CodeAt and Code_Pool56FindFree.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_Pool56FindFree(long a0) => Pool56FindFree(a0);
    /// <summary>Unmanaged entry of Native.sub_1800354E0 (sub_1800354E0 @ 0x1800354e0) for CodeAt and Code_sub_1800354E0.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void CodeThunk_sub_1800354E0(long a0, long a1) => sub_1800354E0(a0, a1);
    /// <summary>Unmanaged entry of Native.SfxEmitParticles (SfxEmitParticles @ 0x1800380c0) for CodeAt and Code_SfxEmitParticles.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static short CodeThunk_SfxEmitParticles(long a0, long a1, short* a2, int* a3) => SfxEmitParticles(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.SfxOpSetParamBytes01 (SfxOpSetParamBytes01 @ 0x180039d60) for CodeAt and Code_SfxOpSetParamBytes01.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpSetParamBytes01(int a0, long a1, long a2, long a3, byte* a4) => SfxOpSetParamBytes01(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpSetParamBytes23 (SfxOpSetParamBytes23 @ 0x180039d90) for CodeAt and Code_SfxOpSetParamBytes23.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpSetParamBytes23(int a0, long a1, long a2, long a3, byte* a4) => SfxOpSetParamBytes23(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpSetHostFrameRate (SfxOpSetHostFrameRate @ 0x180039dc0) for CodeAt and Code_SfxOpSetHostFrameRate.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpSetHostFrameRate(int a0, long a1, long a2, long a3, uint* a4) => SfxOpSetHostFrameRate(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxTrailSequenceHandler (SfxTrailSequenceHandler @ 0x180039e10) for CodeAt and Code_SfxTrailSequenceHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxTrailSequenceHandler(int a0, int* a1, long a2, int* a3, uint* a4) => SfxTrailSequenceHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjectStrikeSequence (SfxObjectStrikeSequence @ 0x18003a690) for CodeAt and Code_SfxObjectStrikeSequence.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjectStrikeSequence(int a0, byte* a1, uint* a2, int* a3, byte* a4) => SfxObjectStrikeSequence(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpMoveObjToPoint (SfxOpMoveObjToPoint @ 0x18003b0d0) for CodeAt and Code_SfxOpMoveObjToPoint.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpMoveObjToPoint(int a0, long a1, long a2, int* a3, uint* a4) => SfxOpMoveObjToPoint(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpStoreArgBytes (SfxOpStoreArgBytes @ 0x18003b3e0) for CodeAt and Code_SfxOpStoreArgBytes.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpStoreArgBytes(int a0, long a1, long a2, long a3, byte* a4) => SfxOpStoreArgBytes(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpCopyVarToEntry (SfxOpCopyVarToEntry @ 0x18003b410) for CodeAt and Code_SfxOpCopyVarToEntry.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpCopyVarToEntry(int a0, long a1, long a2, long a3, int* a4) => SfxOpCopyVarToEntry(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxObjectEffectHandler (SfxObjectEffectHandler @ 0x18003b480) for CodeAt and Code_SfxObjectEffectHandler.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjectEffectHandler(int a0, long a1, long a2, long a3, int* a4) => SfxObjectEffectHandler(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpObjMessage (SfxOpObjMessage @ 0x18003bab0) for CodeAt and Code_SfxOpObjMessage.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpObjMessage(int a0, long a1, long a2, long a3, uint* a4) => SfxOpObjMessage(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpPushVariantId (SfxOpPushVariantId @ 0x18003bb80) for CodeAt and Code_SfxOpPushVariantId.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpPushVariantId(int a0, long a1, long a2, long a3, uint* a4) => SfxOpPushVariantId(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpStartTrackBlock (SfxOpStartTrackBlock @ 0x18003bbd0) for CodeAt and Code_SfxOpStartTrackBlock.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpStartTrackBlock(int a0, long a1, long a2, long a3, uint* a4) => SfxOpStartTrackBlock(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.StartPsxTask10 (StartPsxTask10 @ 0x18003bce0) for CodeAt and Code_StartPsxTask10.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_StartPsxTask10(int a0, uint a1) => StartPsxTask10(a0, a1);
    /// <summary>Unmanaged entry of Native.SfxOpHostLevelFade (SfxOpHostLevelFade @ 0x18003bd10) for CodeAt and Code_SfxOpHostLevelFade.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpHostLevelFade(int a0, long a1, long a2, int* a3, short* a4) => SfxOpHostLevelFade(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpMotionFade (SfxOpMotionFade @ 0x18003be40) for CodeAt and Code_SfxOpMotionFade.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpMotionFade(int a0, long a1, long a2, int* a3, int* a4) => SfxOpMotionFade(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxOpVoicePlay (SfxOpVoicePlay @ 0x18003bf00) for CodeAt and Code_SfxOpVoicePlay.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpVoicePlay(int a0, long a1, long a2, long a3, int* a4) => SfxOpVoicePlay(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.StartPsxTask14 (StartPsxTask14 @ 0x18003bf40) for CodeAt and Code_StartPsxTask14.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_StartPsxTask14(uint a0, uint a1) => StartPsxTask14(a0, a1);
    /// <summary>Unmanaged entry of Native.SfxOpScreenTilesFade (SfxOpScreenTilesFade @ 0x18003bf60) for CodeAt and Code_SfxOpScreenTilesFade.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxOpScreenTilesFade(int a0, uint* a1, long a2, int* a3, int* a4) => SfxOpScreenTilesFade(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.SfxRangeFraction (SfxRangeFraction @ 0x18003c0d0) for CodeAt and Code_SfxRangeFraction.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxRangeFraction(int a0, int a1, int a2, int a3, int a4, int* a5) => SfxRangeFraction(a0, a1, a2, a3, a4, a5);
    /// <summary>Unmanaged entry of Native.RotateDepthVector (RotateDepthVector @ 0x18003c110) for CodeAt and Code_RotateDepthVector.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_RotateDepthVector(short* a0, int a1, ushort* a2) => RotateDepthVector(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.SfxRotTransPointZ (SfxRotTransPointZ @ 0x18003c320) for CodeAt and Code_SfxRotTransPointZ.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxRotTransPointZ(short* a0, short* a1, int a2, ushort* a3) => SfxRotTransPointZ(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.SfxMatrixPointZ (SfxMatrixPointZ @ 0x18003c560) for CodeAt and Code_SfxMatrixPointZ.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxMatrixPointZ(long a0, int a1, ushort* a2) => SfxMatrixPointZ(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.SfxMatrixAimAngles (SfxMatrixAimAngles @ 0x18003c730) for CodeAt and Code_SfxMatrixAimAngles.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxMatrixAimAngles(long a0, ushort* a1) => SfxMatrixAimAngles(a0, a1);
    /// <summary>Unmanaged entry of Native.SfxAngleDelta (SfxAngleDelta @ 0x18003c9b0) for CodeAt and Code_SfxAngleDelta.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxAngleDelta(short a0, short a1, int* a2) => SfxAngleDelta(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.SfxTurnTowards (SfxTurnTowards @ 0x18003ca00) for CodeAt and Code_SfxTurnTowards.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxTurnTowards(ushort* a0, ushort* a1, int a2) => SfxTurnTowards(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.SfxViewTransformTr (SfxViewTransformTr @ 0x18003cae0) for CodeAt and Code_SfxViewTransformTr.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxViewTransformTr(long* a0, byte* a1) => SfxViewTransformTr(a0, a1);
    /// <summary>Unmanaged entry of Native.SfxBuildRotScaleMatrix (SfxBuildRotScaleMatrix @ 0x18003cce0) for CodeAt and Code_SfxBuildRotScaleMatrix.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxBuildRotScaleMatrix(short* a0, uint* a1, byte* a2) => SfxBuildRotScaleMatrix(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.TransformPointByAngles (TransformPointByAngles @ 0x18003cf30) for CodeAt and Code_TransformPointByAngles.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_TransformPointByAngles(short* a0, ushort* a1, int a2, int* a3, int a4, short a5) => TransformPointByAngles(a0, a1, a2, a3, a4, a5);
    /// <summary>Unmanaged entry of Native.RotationToLookAngles (RotationToLookAngles @ 0x18003d170) for CodeAt and Code_RotationToLookAngles.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_RotationToLookAngles(ushort* a0, int* a1, short a2) => RotationToLookAngles(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.ComputeLocalYawDistance (ComputeLocalYawDistance @ 0x18003d420) for CodeAt and Code_ComputeLocalYawDistance.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_ComputeLocalYawDistance(ushort* a0, ushort* a1, ushort* a2, uint* a3, uint* a4, uint* a5) => ComputeLocalYawDistance(a0, a1, a2, a3, a4, a5);
    /// <summary>Unmanaged entry of Native.PsxAlignUp4 (PsxAlignUp4 @ 0x18003d660) for CodeAt and Code_PsxAlignUp4.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxAlignUp4(int a0) => PsxAlignUp4(a0);
    /// <summary>Unmanaged entry of Native.SfxArenaInit (SfxArenaInit @ 0x18003d670) for CodeAt and Code_SfxArenaInit.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static void CodeThunk_SfxArenaInit(long a0, int a1) => SfxArenaInit(a0, a1);
    /// <summary>Unmanaged entry of Native.PsxArenaAlloc (PsxArenaAlloc @ 0x18003d6c0) for CodeAt and Code_PsxArenaAlloc.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int* CodeThunk_PsxArenaAlloc(long a0, int a1) => PsxArenaAlloc(a0, a1);
    /// <summary>Unmanaged entry of Native.SfxArenaResetIfShort (SfxArenaResetIfShort @ 0x18003d7a0) for CodeAt and Code_SfxArenaResetIfShort.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxArenaResetIfShort(long a0, int a1) => SfxArenaResetIfShort(a0, a1);
    /// <summary>Unmanaged entry of Native.SfxArenaFillBlock (SfxArenaFillBlock @ 0x18003d7d0) for CodeAt and Code_SfxArenaFillBlock.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxArenaFillBlock(long a0) => SfxArenaFillBlock(a0);
    /// <summary>Unmanaged entry of Native.PsxGetTableEntry (PsxGetTableEntry @ 0x18003d800) for CodeAt and Code_PsxGetTableEntry.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static byte* CodeThunk_PsxGetTableEntry(int a0, int a1) => PsxGetTableEntry(a0, a1);
    /// <summary>Unmanaged entry of Native.PsxTableEntrySize (PsxTableEntrySize @ 0x18003daa0) for CodeAt and Code_PsxTableEntrySize.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxTableEntrySize(int a0, int a1) => PsxTableEntrySize(a0, a1);
    /// <summary>Unmanaged entry of Native.PsxBumpAlloc (PsxBumpAlloc @ 0x18003dae0) for CodeAt and Code_PsxBumpAlloc.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static byte* CodeThunk_PsxBumpAlloc(int a0) => PsxBumpAlloc(a0);
    /// <summary>Unmanaged entry of Native.SfxClearWord1FC (SfxClearWord1FC @ 0x18003db80) for CodeAt and Code_SfxClearWord1FC.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxClearWord1FC() => SfxClearWord1FC();
    /// <summary>Unmanaged entry of Native.PsxChannelReset (PsxChannelReset @ 0x18003db90) for CodeAt and Code_PsxChannelReset.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxChannelReset(int a0) => PsxChannelReset(a0);
    /// <summary>Unmanaged entry of Native.PsxChannelFlip (PsxChannelFlip @ 0x18003dbf0) for CodeAt and Code_PsxChannelFlip.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxChannelFlip() => PsxChannelFlip();
    /// <summary>Unmanaged entry of Native.SfxLoaderStep (SfxLoaderStep @ 0x18003de20) for CodeAt and Code_SfxLoaderStep.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static short CodeThunk_SfxLoaderStep() => SfxLoaderStep();
    /// <summary>Unmanaged entry of Native.PsxAddPrimWithMode (PsxAddPrimWithMode @ 0x18003edb0) for CodeAt and Code_PsxAddPrimWithMode.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxAddPrimWithMode(int a0, uint* a1, ulong a2, int a3) => PsxAddPrimWithMode(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.PsxAddPrimAtDepth (PsxAddPrimAtDepth @ 0x18003eed0) for CodeAt and Code_PsxAddPrimAtDepth.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int* CodeThunk_PsxAddPrimAtDepth(int a0, int a1, ulong a2) => PsxAddPrimAtDepth(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.PsxAddPrimAtDepthWithMode (PsxAddPrimAtDepthWithMode @ 0x18003efc0) for CodeAt and Code_PsxAddPrimAtDepthWithMode.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int* CodeThunk_PsxAddPrimAtDepthWithMode(int a0, int a1, ulong a2, int a3) => PsxAddPrimAtDepthWithMode(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.PsxAllocDrawMode (PsxAllocDrawMode @ 0x18003f0c0) for CodeAt and Code_PsxAllocDrawMode.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static byte* CodeThunk_PsxAllocDrawMode(int a0, sbyte a1) => PsxAllocDrawMode(a0, a1);
    /// <summary>Unmanaged entry of Native.PsxAddScreenTiles (PsxAddScreenTiles @ 0x18003f180) for CodeAt and Code_PsxAddScreenTiles.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxAddScreenTiles(long a0, int a1, byte a2, byte a3, byte a4) => PsxAddScreenTiles(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.PsxAddWaveGradient (PsxAddWaveGradient @ 0x18003f3a0) for CodeAt and Code_PsxAddWaveGradient.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static ulong CodeThunk_PsxAddWaveGradient(uint* a0, short a1, uint a2, int a3, short a4, int a5, long a6, int a7, int a8, int a9, int a10) => PsxAddWaveGradient(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
    /// <summary>Unmanaged entry of Native.DrawFacingRibbon (DrawFacingRibbon @ 0x18003f6f0) for CodeAt and Code_DrawFacingRibbon.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_DrawFacingRibbon(ulong a0, short* a1, int a2, short a3, short a4, short a5, short a6, int a7, int a8, int a9, byte* a10, byte* a11, int a12) => DrawFacingRibbon(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
    /// <summary>Unmanaged entry of Native.EmitDepthCuedRibbon (EmitDepthCuedRibbon @ 0x18003fc70) for CodeAt and Code_EmitDepthCuedRibbon.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static sbyte CodeThunk_EmitDepthCuedRibbon(ulong a0, int a1, short a2, short a3, short a4, short a5, int a6, int a7, int a8, byte* a9, byte* a10, int a11) => EmitDepthCuedRibbon(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
    /// <summary>Unmanaged entry of Native.DrawScreenAlignedRibbon (DrawScreenAlignedRibbon @ 0x180040340) for CodeAt and Code_DrawScreenAlignedRibbon.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_DrawScreenAlignedRibbon(ulong a0, int a1, short a2, short a3, short a4, short a5, int a6, int a7, int a8, byte* a9, byte* a10, int a11) => DrawScreenAlignedRibbon(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
    /// <summary>Unmanaged entry of Native.DrawRibbonTrail (DrawRibbonTrail @ 0x180040730) for CodeAt and Code_DrawRibbonTrail.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static sbyte CodeThunk_DrawRibbonTrail(ushort* a0, long a1, short* a2, int a3, int a4, int a5, int a6, byte* a7, int a8) => DrawRibbonTrail(a0, a1, a2, a3, a4, a5, a6, a7, a8);
    /// <summary>Unmanaged entry of Native.EmitSpriteCellFrame (EmitSpriteCellFrame @ 0x1800409d0) for CodeAt and Code_EmitSpriteCellFrame.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_EmitSpriteCellFrame(ushort* a0, long* a1, short* a2, int a3, int a4, int a5, int a6, int a7, int a8) => EmitSpriteCellFrame(a0, a1, a2, a3, a4, a5, a6, a7, a8);
    /// <summary>Unmanaged entry of Native.EmitWorldSpriteList (EmitWorldSpriteList @ 0x180040f90) for CodeAt and Code_EmitWorldSpriteList.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_EmitWorldSpriteList(ushort* a0, int* a1, int a2, int a3, int a4) => EmitWorldSpriteList(a0, a1, a2, a3, a4);
    /// <summary>Unmanaged entry of Native.EmitScreenSpriteList (EmitScreenSpriteList @ 0x1800413b0) for CodeAt and Code_EmitScreenSpriteList.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_EmitScreenSpriteList(int a0, ushort* a1, int a2, int a3, int a4, int a5, int a6) => EmitScreenSpriteList(a0, a1, a2, a3, a4, a5, a6);
    /// <summary>Unmanaged entry of Native.EmitSpriteAnimFrame (EmitSpriteAnimFrame @ 0x180041670) for CodeAt and Code_EmitSpriteAnimFrame.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_EmitSpriteAnimFrame(long a0, long* a1, short* a2, int a3, int a4, int a5, int a6) => EmitSpriteAnimFrame(a0, a1, a2, a3, a4, a5, a6);
    /// <summary>Unmanaged entry of Native.EmitWorldSpriteFrame (EmitWorldSpriteFrame @ 0x180041b80) for CodeAt and Code_EmitWorldSpriteFrame.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_EmitWorldSpriteFrame(long a0, long* a1, short* a2, int a3, int a4, int a5, uint a6) => EmitWorldSpriteFrame(a0, a1, a2, a3, a4, a5, a6);
    /// <summary>Unmanaged entry of Native.DrawRingMeshEffect (DrawRingMeshEffect @ 0x180042180) for CodeAt and Code_DrawRingMeshEffect.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_DrawRingMeshEffect(byte* a0, long* a1, short* a2, int a3, int a4, int a5, int a6) => DrawRingMeshEffect(a0, a1, a2, a3, a4, a5, a6);
    /// <summary>Unmanaged entry of Native.EmitScaledSpriteFrame (EmitScaledSpriteFrame @ 0x180042be0) for CodeAt and Code_EmitScaledSpriteFrame.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_EmitScaledSpriteFrame(ushort* a0, long* a1, short* a2, int a3, int a4, int a5, int a6) => EmitScaledSpriteFrame(a0, a1, a2, a3, a4, a5, a6);
    /// <summary>Unmanaged entry of Native.EmitSpriteFrame (EmitSpriteFrame @ 0x1800434b0) for CodeAt and Code_EmitSpriteFrame.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int* CodeThunk_EmitSpriteFrame(long a0, short a1, short a2, int a3, int a4, int a5) => EmitSpriteFrame(a0, a1, a2, a3, a4, a5);
    /// <summary>Unmanaged entry of Native.EmitScrollLayers (EmitScrollLayers @ 0x180043800) for CodeAt and Code_EmitScrollLayers.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int* CodeThunk_EmitScrollLayers(ushort* a0, int a1) => EmitScrollLayers(a0, a1);
    /// <summary>Unmanaged entry of Native.EmitInterpolatedSpriteTrail (EmitInterpolatedSpriteTrail @ 0x180044560) for CodeAt and Code_EmitInterpolatedSpriteTrail.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static short CodeThunk_EmitInterpolatedSpriteTrail(ushort* a0, long* a1, int a2, int a3) => EmitInterpolatedSpriteTrail(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.SfxRegisterTableSlot3 (SfxRegisterTableSlot3 @ 0x180044a30) for CodeAt and Code_SfxRegisterTableSlot3.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxRegisterTableSlot3(ulong a0) => SfxRegisterTableSlot3(a0);
    /// <summary>Unmanaged entry of Native.PsxGetContextQword (PsxGetContextQword @ 0x180044a60) for CodeAt and Code_PsxGetContextQword.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PsxGetContextQword(int a0) => PsxGetContextQword(a0);
    /// <summary>Unmanaged entry of Native.ClearActiveObjectAddr (ClearActiveObjectAddr @ 0x180044d50) for CodeAt and Code_ClearActiveObjectAddr.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int* CodeThunk_ClearActiveObjectAddr() => ClearActiveObjectAddr();
    /// <summary>Unmanaged entry of Native.ObjSelectDistanceById (ObjSelectDistanceById @ 0x180044d60) for CodeAt and Code_ObjSelectDistanceById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_ObjSelectDistanceById(int a0) => ObjSelectDistanceById(a0);
    /// <summary>Unmanaged entry of Native.ObjSizeById (ObjSizeById @ 0x180044da0) for CodeAt and Code_ObjSizeById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_ObjSizeById(int a0) => ObjSizeById(a0);
    /// <summary>Unmanaged entry of Native.HostGetObjectPositionById (HostGetObjectPositionById @ 0x180044dc0) for CodeAt and Code_HostGetObjectPositionById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostGetObjectPositionById(int a0, long a1) => HostGetObjectPositionById(a0, a1);
    /// <summary>Unmanaged entry of Native.HostQueryObjPositionById (HostQueryObjPositionById @ 0x180044f60) for CodeAt and Code_HostQueryObjPositionById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostQueryObjPositionById(int a0, long a1) => HostQueryObjPositionById(a0, a1);
    /// <summary>Unmanaged entry of Native.HostQueryObjAnchorPosById (HostQueryObjAnchorPosById @ 0x1800450c0) for CodeAt and Code_HostQueryObjAnchorPosById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static sbyte CodeThunk_HostQueryObjAnchorPosById(int a0, int a1, long a2) => HostQueryObjAnchorPosById(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.HostSetObjectPositionById (HostSetObjectPositionById @ 0x1800450f0) for CodeAt and Code_HostSetObjectPositionById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostSetObjectPositionById(int a0, long a1) => HostSetObjectPositionById(a0, a1);
    /// <summary>Unmanaged entry of Native.HostGetObjectRotationById (HostGetObjectRotationById @ 0x180045350) for CodeAt and Code_HostGetObjectRotationById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostGetObjectRotationById(int a0, long a1) => HostGetObjectRotationById(a0, a1);
    /// <summary>Unmanaged entry of Native.HostSetObjRotation (HostSetObjRotation @ 0x1800453c0) for CodeAt and Code_HostSetObjRotation.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostSetObjRotation(int a0, long a1) => HostSetObjRotation(a0, a1);
    /// <summary>Unmanaged entry of Native.IsObjNotFlippedById (IsObjNotFlippedById @ 0x180045480) for CodeAt and Code_IsObjNotFlippedById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_IsObjNotFlippedById(int a0) => IsObjNotFlippedById(a0);
    /// <summary>Unmanaged entry of Native.SfxObjTurnTowardsPoint (SfxObjTurnTowardsPoint @ 0x1800454a0) for CodeAt and Code_SfxObjTurnTowardsPoint.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxObjTurnTowardsPoint(int a0, short* a1, ushort* a2, sbyte a3, short a4, int a5, int a6) => SfxObjTurnTowardsPoint(a0, a1, a2, a3, a4, a5, a6);
    /// <summary>Unmanaged entry of Native.HostQueryObjScaleById (HostQueryObjScaleById @ 0x1800455f0) for CodeAt and Code_HostQueryObjScaleById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostQueryObjScaleById(int a0, uint* a1) => HostQueryObjScaleById(a0, a1);
    /// <summary>Unmanaged entry of Native.HostSetObjScaleById (HostSetObjScaleById @ 0x1800456d0) for CodeAt and Code_HostSetObjScaleById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostSetObjScaleById(int a0, uint* a1) => HostSetObjScaleById(a0, a1);
    /// <summary>Unmanaged entry of Native.ObjAimPointById (ObjAimPointById @ 0x1800457c0) for CodeAt and Code_ObjAimPointById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_ObjAimPointById(int a0, int a1, short* a2) => ObjAimPointById(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.ObjDepthOrderSignById (ObjDepthOrderSignById @ 0x180045970) for CodeAt and Code_ObjDepthOrderSignById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_ObjDepthOrderSignById(int a0, int a1) => ObjDepthOrderSignById(a0, a1);
    /// <summary>Unmanaged entry of Native.ObjDistanceSixthPlusById (ObjDistanceSixthPlusById @ 0x180045a80) for CodeAt and Code_ObjDistanceSixthPlusById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_ObjDistanceSixthPlusById(int a0, int a1) => ObjDistanceSixthPlusById(a0, a1);
    /// <summary>Unmanaged entry of Native.GetObjectHostPosition (GetObjectHostPosition @ 0x180045ab0) for CodeAt and Code_GetObjectHostPosition.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_GetObjectHostPosition(int a0, int a1, ushort* a2) => GetObjectHostPosition(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.SetObjectMeshVisibilityById (SetObjectMeshVisibilityById @ 0x180045f90) for CodeAt and Code_SetObjectMeshVisibilityById.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static ushort* CodeThunk_SetObjectMeshVisibilityById(int a0, int a1) => SetObjectMeshVisibilityById(a0, a1);
    /// <summary>Unmanaged entry of Native.HostFadeObjectMeshIfAllowed (HostFadeObjectMeshIfAllowed @ 0x180046020) for CodeAt and Code_HostFadeObjectMeshIfAllowed.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostFadeObjectMeshIfAllowed(int a0, uint a1) => HostFadeObjectMeshIfAllowed(a0, a1);
    /// <summary>Unmanaged entry of Native.SetMeshFadeForObjectList (SetMeshFadeForObjectList @ 0x1800460e0) for CodeAt and Code_SetMeshFadeForObjectList.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SetMeshFadeForObjectList(int a0, int a1) => SetMeshFadeForObjectList(a0, a1);
    /// <summary>Unmanaged entry of Native.HostResetObjectStatuses (HostResetObjectStatuses @ 0x1800462a0) for CodeAt and Code_HostResetObjectStatuses.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostResetObjectStatuses(int a0) => HostResetObjectStatuses(a0);
    /// <summary>Unmanaged entry of Native.HostObjQueryAccess (HostObjQueryAccess @ 0x180046320) for CodeAt and Code_HostObjQueryAccess.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjQueryAccess(int a0, long a1, uint* a2, uint* a3) => HostObjQueryAccess(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.HostEnableLastBossFlag (HostEnableLastBossFlag @ 0x1800464f0) for CodeAt and Code_HostEnableLastBossFlag.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostEnableLastBossFlag() => HostEnableLastBossFlag();
    /// <summary>Unmanaged entry of Native.HostObjectCheckAction (HostObjectCheckAction @ 0x180046670) for CodeAt and Code_HostObjectCheckAction.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostObjectCheckAction(int a0, int a1) => HostObjectCheckAction(a0, a1);
    /// <summary>Unmanaged entry of Native.IsObjectStateEqual (IsObjectStateEqual @ 0x180046890) for CodeAt and Code_IsObjectStateEqual.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_IsObjectStateEqual(int a0, int a1) => IsObjectStateEqual(a0, a1);
    /// <summary>Unmanaged entry of Native.SelectReg3ObjectFromList (SelectReg3ObjectFromList @ 0x1800468d0) for CodeAt and Code_SelectReg3ObjectFromList.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SelectReg3ObjectFromList(int a0) => SelectReg3ObjectFromList(a0);
    /// <summary>Unmanaged entry of Native.HostControlEyeTextureAnimation (HostControlEyeTextureAnimation @ 0x180046cb0) for CodeAt and Code_HostControlEyeTextureAnimation.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostControlEyeTextureAnimation(int a0, uint a1, uint a2) => HostControlEyeTextureAnimation(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.HostExecuteVfxForObjects (HostExecuteVfxForObjects @ 0x180046e80) for CodeAt and Code_HostExecuteVfxForObjects.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostExecuteVfxForObjects(int a0) => HostExecuteVfxForObjects(a0);
    /// <summary>Unmanaged entry of Native.HostRequestBattleFiguresForObjects (HostRequestBattleFiguresForObjects @ 0x180047070) for CodeAt and Code_HostRequestBattleFiguresForObjects.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostRequestBattleFiguresForObjects(int a0) => HostRequestBattleFiguresForObjects(a0);
    /// <summary>Unmanaged entry of Native.HostSetSoundFps (HostSetSoundFps @ 0x180047260) for CodeAt and Code_HostSetSoundFps.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static uint CodeThunk_HostSetSoundFps(uint a0) => HostSetSoundFps(a0);
    /// <summary>Unmanaged entry of Native.RegisterSoModel (RegisterSoModel @ 0x180047290) for CodeAt and Code_RegisterSoModel.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_RegisterSoModel(ushort* a0, sbyte a1) => RegisterSoModel(a0, a1);
    /// <summary>Unmanaged entry of Native.RegisterSoSummonModel (RegisterSoSummonModel @ 0x180047330) for CodeAt and Code_RegisterSoSummonModel.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_RegisterSoSummonModel(ushort* a0, int* a1, uint a2, ulong a3) => RegisterSoSummonModel(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.SampleColorRamp (SampleColorRamp @ 0x180047a00) for CodeAt and Code_SampleColorRamp.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SampleColorRamp(long a0, byte* a1, int a2) => SampleColorRamp(a0, a1, a2);
    /// <summary>Unmanaged entry of Native.EmitSplitE7Prims (EmitSplitE7Prims @ 0x180047b40) for CodeAt and Code_EmitSplitE7Prims.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_EmitSplitE7Prims(int a0, short a1, short a2, short a3, int a4, short a5, ushort a6) => EmitSplitE7Prims(a0, a1, a2, a3, a4, a5, a6);
    /// <summary>Unmanaged entry of Native.SfxDrawWarpGrid (SfxDrawWarpGrid @ 0x180047e30) for CodeAt and Code_SfxDrawWarpGrid.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxDrawWarpGrid(uint a0, int a1, int a2, int a3, int a4, int a5, int a6, short a7, short a8, short* a9, int* a10, long a11, int a12, int a13) => SfxDrawWarpGrid(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
    /// <summary>Unmanaged entry of Native.PushRotTransMatrix (PushRotTransMatrix @ 0x1800484e0) for CodeAt and Code_PushRotTransMatrix.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PushRotTransMatrix(short* a0) => PushRotTransMatrix(a0);
    /// <summary>Unmanaged entry of Native.PopRotTransMatrix (PopRotTransMatrix @ 0x1800486a0) for CodeAt and Code_PopRotTransMatrix.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_PopRotTransMatrix() => PopRotTransMatrix();
    /// <summary>Unmanaged entry of Native.SetMatrixDepth (SetMatrixDepth @ 0x180048740) for CodeAt and Code_SetMatrixDepth.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SetMatrixDepth(int a0) => SetMatrixDepth(a0);
    /// <summary>Unmanaged entry of Native.RestoreMatrixDepth (RestoreMatrixDepth @ 0x180048760) for CodeAt and Code_RestoreMatrixDepth.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_RestoreMatrixDepth() => RestoreMatrixDepth();
    /// <summary>Unmanaged entry of Native.HostCaptureBackgroundTexture (HostCaptureBackgroundTexture @ 0x180048770) for CodeAt and Code_HostCaptureBackgroundTexture.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_HostCaptureBackgroundTexture(int a0, int a1, int a2, int a3, int a4, int a5, int a6, int a7) => HostCaptureBackgroundTexture(a0, a1, a2, a3, a4, a5, a6, a7);
    /// <summary>Unmanaged entry of Native.ResetSfxWorkArea (ResetSfxWorkArea @ 0x180048800) for CodeAt and Code_ResetSfxWorkArea.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static uint* CodeThunk_ResetSfxWorkArea() => ResetSfxWorkArea();
    /// <summary>Unmanaged entry of Native.PsxObjectAlloc (PsxObjectAlloc @ 0x1800488c0) for CodeAt and Code_PsxObjectAlloc.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int* CodeThunk_PsxObjectAlloc(long a0, int a1) => PsxObjectAlloc(a0, a1);
    /// <summary>Unmanaged entry of Native.StartPsxTask (StartPsxTask @ 0x180048b10) for CodeAt and Code_StartPsxTask.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_StartPsxTask(int a0, short a1, int a2, int a3) => StartPsxTask(a0, a1, a2, a3);
    /// <summary>Unmanaged entry of Native.SfxTickEffectTasks (SfxTickEffectTasks @ 0x180048e70) for CodeAt and Code_SfxTickEffectTasks.</summary>

    [global::System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static long CodeThunk_SfxTickEffectTasks() => SfxTickEffectTasks();
}
