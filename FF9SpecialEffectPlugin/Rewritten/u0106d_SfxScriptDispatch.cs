namespace Recompiled.FF9SpecialEffectPlugin;

/// <summary>
/// Native-call dispatcher of the PSX script interpreter: opcode a2 selects a handler, its o32 arguments come from the
/// current register bank of the script context a1 (a0..a3, then the words at sp + 16, sp + 20, ...; PsxGetCallArg /
/// PsxGetCallArgPtr read exactly the slots the original reads inline), and the handler's result goes to v0.
/// </summary>
internal static unsafe partial class Native
{
    /// <summary>
    /// Runs script native call <paramref name="a2"/> (0..215; anything else, including negative values and the holes
    /// 20, 27, 36, 41, 133, 194, 209 and 214, only clears v0) against the script context <paramref name="a1"/> and
    /// stores the 32-bit result in v0 of the register bank selected after the call. Handlers that return a host
    /// pointer report its PSX-style address (PsxAddrMapperRegister), 0 for null. Returns PsxSetGpr's result (2).
    /// <paramref name="a3"/> is not read.
    /// </summary>
    public static long SfxScriptDispatch(long a1, int a2, long a3)
    {
        Recompiler.Runtime.Tracer.Enter("SfxScriptDispatch(a1, a2, a3)", a1, a2, a3);
        int v0 = (uint)a2 switch
        {
            <= 63 => SfxScriptDispatchOps0To63(a1, a2),
            <= 127 => SfxScriptDispatchOps64To127(a1, a2),
            <= 191 => SfxScriptDispatchOps128To191(a1, a2),
            <= 215 => SfxScriptDispatchOps192To215(a1, a2),
            _ => 0,
        };

        // The bank is read again after the handler (the original reloads it), the handler may have switched it.
        var ctx = (PsxCallContext*)a1;
        return PsxSetGpr((long)((byte*)((uint*)&ctx->gprBanks) + ((long)ctx->bank << 7)), 2, v0);
    }

    /// <summary>The PSX-style address of host pointer <paramref name="host"/> in the context's mapper; 0 for null.</summary>
    private static int SfxScriptDispatchMap(long ctx, long host)
    {
        Recompiler.Runtime.Tracer.Enter("SfxScriptDispatchMap(ctx, host)", ctx, host);
        return host != 0 ? (int)PsxAddrMapperRegister((long)((PsxCallContext*)ctx)->memory, (ulong)host) : 0;
    }

    private static int SfxScriptDispatchOps0To63(long ctx, int op)
    {
        Recompiler.Runtime.Tracer.Enter("SfxScriptDispatchOps0To63(ctx, op)", ctx, op);
        int A(int i) => (int)PsxGetCallArg(ctx, i);
        long P(int i) => PsxGetCallArgPtr(ctx, i);

        switch (op)
        {
            case 0:
                return (int)HostSendRect64((short*)P(0), P(1));
            case 1:
                return (int)HostSendRect65((short*)P(0), P(1));
            case 2:
                ParticleStepMotion((ulong)P(0), (short)A(1), (ulong)P(2), (ulong)P(3));
                return 0;
            case 3:
                DrawScreenAlignedRibbon((ulong)P(0), A(1), (short)A(2), (short)A(3), (short)A(4), (short)A(5), A(6), A(7),
                    A(8), (byte*)P(9), (byte*)P(10), A(11));
                return 0;
            case 4:
                HostSetSoundFps((uint)A(0));
                return 0;
            case 5:
                return SfxScriptDispatchMap(ctx, RotMatrixYXZ((short*)P(0), P(1)));
            case 6:
                return (int)RegisterGouEffModel((ulong)P(0));
            case 7:
                SfxScriptDispatchWriteDrawArea(ctx);
                return 0;
            case 8:
                return SfxScriptDispatchMap(ctx, (long)ScaleMatrix((short*)P(0), (uint*)P(1)));
            case 9:
                return SfxScriptDispatchMap(ctx, CompMatrix(P(0), (short*)P(1), P(2)));
            case 10:
            {
                // 4 dword stores to 0x180211F40..4C, then the u16 at +16 zero-extended to dword_180211F50.
                byte* src = (byte*)P(0);
                *(uint*)xmmword_180211F40 = *(uint*)src;
                *(uint*)(xmmword_180211F40 + 4) = *(uint*)(src + 4);
                *(uint*)(xmmword_180211F40 + 8) = *(uint*)(src + 8);
                *(uint*)(xmmword_180211F40 + 12) = *(uint*)(src + 12);
                dword_180211F50 = *(ushort*)(src + 16);
                return 0;
            }
            case 11:
                StopSummonTexAnim(A(0), A(1));
                return 0;
            case 12:
                StartSummonTexAnim(A(0), A(1), (sbyte)(A(2) != 0 ? 1 : 0));
                return 0;
            case 13:
            {
                byte* src = (byte*)P(0);
                *(uint*)unk_180211F54 = *(uint*)(src + 20);
                *(uint*)unk_180211F58 = *(uint*)(src + 24);
                dword_180211F5C = *(int*)(src + 28);
                return 0;
            }
            case 14:
                return Recompiler.Runtime.X86.Cvttsd2si(Math.Cos((double)A(0) / 4096.8 * 2.0 * 3.141592653589793) * 4096.8);
            case 15:
                return Recompiler.Runtime.X86.Cvttsd2si(Math.Sin((double)A(0) / 4096.8 * 2.0 * 3.141592653589793) * 4096.8);
            case 16:
                return 1024 - Recompiler.Runtime.X86.Cvttsd2si(
                    -Math.Atan2((double)A(1), (double)A(0)) / 6.283185307179586 * -4096.0);
            case 17:
            case 44:
                return Recompiler.Runtime.X86.Cvttsd2si(Math.Sqrt((double)A(0)));
            case 18:
                GteLoadAverageShort12((short*)P(0), (short*)P(1), A(2), A(3), (ushort*)P(4));
                return 0;
            case 19:
                return CurrentEffectId != (int)SpecialEffectId.Sinkhole ? (int)RegisterTexListModel((ulong)P(0), P(1), P(2)) : 0;
            case 21:
                return (int)RegisterSolidEffModel((ulong)P(0));
            case 22:
                return (int)RegisterTexEffModel((ulong)P(0), (short)A(1), (short)A(2), (short)A(3), (short)A(4));
            case 23:
                // r8 is not set by the original; RegisterSummonModel does not read its third argument.
                return (int)RegisterSummonModel(P(0), P(1), 0);
            case 24:
                DrawEffModel(P(0), P(1), P(2), A(3));
                return 0;
            case 25:
                return (int)DrawSummonModel(P(0), P(1), P(2), A(3), (sbyte)A(4));
            case 26:
                SetSummonMotion(P(0), A(1));
                return 0;
            case 28:
                if (CurrentEffectId != (int)SpecialEffectId.Sinkhole)
                {
                    InitEffModelSlots(P(0), (uint)A(1));
                }

                return 0;
            case 29:
                InitModelTable88(P(0), (uint)A(1));
                return 0;
            case 30:
                if (CurrentEffectId != (int)SpecialEffectId.Sinkhole)
                {
                    ResetEffModelSlots();
                }

                return 0;
            case 31:
                dword_180220880 = 0;
                qword_180220830 = 0;
                return 0;
            case 32:
                HostSendCommand0EMode0((uint)A(0), (uint)A(1), P(2));
                return 0;
            case 33:
                HostSendCommand0EMode0FromBlock((uint)A(0), A(1), P(2));
                return 0;
            case 34:
                // The host pointer of argument 1 travels as its low 32 bits (the callee forwards edx).
                HostObjCmd0CU32((uint)A(0), (uint)P(1));
                return 0;
            case 35:
            {
                // The raw argument (not a resolved pointer) is sign-extended and registered with the mapper.
                int address = A(1);
                return address != 0
                    ? (int)PsxAddrMapperRegister((long)((PsxCallContext*)ctx)->memory, (ulong)(long)address)
                    : 0;
            }
            case 37:
                return -1;
            case 38:
                HostObjCmd0B((uint)A(0), (uint)A(1));
                return 0;
            case 39:
                return (int)HostObjCmd0A((uint)A(0));
            case 40:
                HostObjCmd0CIfFlag((uint)A(0), (byte)A(1));
                return 0;
            case 42:
                HostObjCmd0CReset((uint)A(0), (sbyte)(A(1) != 0 ? 1 : 0));
                return 0;
            case 43:
            {
                int value = A(0);
                int aligned = value & ~3;
                if ((value & 3) != 0)
                {
                    aligned += 4;
                }

                return aligned;
            }
            case 45:
                return (int)GteIntplScalar(A(0), A(1), A(2));
            case 46:
                return (int)LerpIntRatio(A(0), A(1), A(2), A(3));
            case 47:
                GteIntplVector((short*)P(0), (short*)P(1), A(2), (ushort*)P(3));
                return 0;
            case 48:
                dword_1803231DC = unchecked(1103515245 * dword_1803231DC + 12345);
                return (int)((uint)dword_1803231DC >> 16);
            case 49:
                return (int)PsxRandRange((uint)A(0), A(1));
            case 50:
            {
                int range = A(0);
                int divisor = range != 0 ? range : 1;
                dword_1803231DC = unchecked(1103515245 * dword_1803231DC + 12345);
                return (int)((uint)dword_1803231DC >> 16) % divisor - (range >> 1);
            }
            case 51:
                return (int)VecDistance3((short*)P(0), (short*)P(1));
            case 52:
                PsxRandScatterVec(P(0));
                return 0;
            case 53:
                LookAnglesBetween((short*)P(0), (short*)P(1), (ushort*)P(2));
                return 0;
            case 54:
                RotateDepthVector((short*)P(0), A(1), (ushort*)P(2));
                return 0;
            case 55:
                SfxRotTransPointZ((short*)P(0), (short*)P(1), A(2), (ushort*)P(3));
                return 0;
            case 56:
                return (int)SfxAngleDelta((short)A(0), (short)A(1), (int*)P(2));
            case 57:
                SfxTurnTowards((ushort*)P(0), (ushort*)P(1), A(2));
                return 0;
            case 58:
                SfxViewTransformTr((long*)P(0), (byte*)P(1));
                return 0;
            case 59:
                // The original also passes a3 in r9; SfxBuildRotScaleMatrix takes three arguments.
                SfxBuildRotScaleMatrix((short*)P(0), (uint*)P(1), (byte*)P(2));
                return 0;
            case 60:
                TransformPointByAngles((short*)P(0), (ushort*)P(1), A(2), (int*)P(3), A(4), (short)A(5));
                return 0;
            case 61:
                RotationToLookAngles((ushort*)P(0), (int*)P(1), (short)A(2));
                return 0;
            case 62:
                ComputeLocalYawDistance((ushort*)P(0), (ushort*)P(1), (ushort*)P(2), (uint*)P(3), (uint*)P(4), (uint*)P(5));
                return 0;
            case 63:
                return SfxScriptDispatchMap(ctx, (long)PsxAllocDrawMode(A(0), (sbyte)A(1)));
            default:
                return 0;
        }
    }

    /// <summary>
    /// Opcode 7: fills the 5-word packet at argument 0: byte 3 = 5, word 1 = 0xE7000000, word 2 = r[1] | r[0] &lt;&lt; 16
    /// (r = s16 x4 at argument 1; r[1] zero-extended), word 3 = r[3] | r[2] &lt;&lt; 16 (r[3] sign-extended, as the
    /// original ORs it), word 4 = (s16)a3 | (s16)a2 &lt;&lt; 16 (a3 sign-extended).
    /// </summary>
    private static void SfxScriptDispatchWriteDrawArea(long ctx)
    {
        Recompiler.Runtime.Tracer.Enter("SfxScriptDispatchWriteDrawArea(ctx)", ctx);
        short* rect = (short*)PsxGetCallArgPtr(ctx, 1);
        int low = (int)PsxGetCallArg(ctx, 3);
        int high = (int)PsxGetCallArg(ctx, 2);
        byte* packet = (byte*)PsxGetCallArgPtr(ctx, 0);
        Packet5* packet5 = (Packet5*)packet;
        packet[3] = 5;
        packet5->union_4_4 = 0xE7000000;
        packet5->field_8 = (ushort)rect[1] | (rect[0] << 16);
        packet5->field_12 = rect[3] | (rect[2] << 16);
        packet5->field_16 = (short)low | ((short)high << 16);
    }

    private static int SfxScriptDispatchOps64To127(long ctx, int op)
    {
        Recompiler.Runtime.Tracer.Enter("SfxScriptDispatchOps64To127(ctx, op)", ctx, op);
        int A(int i) => (int)PsxGetCallArg(ctx, i);
        long P(int i) => PsxGetCallArgPtr(ctx, i);

        switch (op)
        {
            case 64:
                PsxAddScreenTiles(P(0), A(1), (byte)A(2), (byte)A(3), (byte)A(4));
                return 0;
            case 65:
                ModifySummonModelRgb(A(0), (ushort)A(1), (ushort)A(2), (ushort)A(3));
                return 0;
            case 66:
                DrawFacingRibbon((ulong)P(0), (short*)P(1), A(2), (short)A(3), (short)A(4), (short)A(5), (short)A(6), A(7),
                    A(8), A(9), (byte*)P(10), (byte*)P(11), A(12));
                return 0;
            case 67:
                EmitDepthCuedRibbon((ulong)P(0), A(1), (short)A(2), (short)A(3), (short)A(4), (short)A(5), A(6), A(7), A(8),
                    (byte*)P(9), (byte*)P(10), A(11));
                return 0;
            case 68:
                DrawRibbonTrail((ushort*)P(0), P(1), (short*)P(2), A(3), A(4), A(5), A(6), (byte*)P(7), A(8));
                return 0;
            case 69:
                if (CurrentEffectId != (int)SpecialEffectId.Slow)
                {
                    EmitSpriteCellFrame((ushort*)P(0), (long*)P(1), (short*)P(2), A(3), A(4), A(5), A(6), A(7), A(8));
                }

                return 0;
            case 70:
                EmitWorldSpriteList((ushort*)P(0), (int*)P(1), A(2), A(3), A(4));
                return 0;
            case 71:
                EmitScreenSpriteList(A(0), (ushort*)P(1), A(2), A(3), A(4), A(5), A(6));
                return 0;
            case 72:
                EmitSpriteAnimFrame(P(0), (long*)P(1), (short*)P(2), A(3), A(4), A(5), A(6));
                return 0;
            case 73:
                EmitWorldSpriteFrame(P(0), (long*)P(1), (short*)P(2), A(3), A(4), A(5), (uint)A(6));
                return 0;
            case 74:
                DrawRingMeshEffect((byte*)P(0), (long*)P(1), (short*)P(2), A(3), A(4), A(5), A(6));
                return 0;
            case 75:
                EmitScaledSpriteFrame((ushort*)P(0), (long*)P(1), (short*)P(2), A(3), A(4), A(5), A(6));
                return 0;
            case 76:
                EmitSpriteFrame(P(0), (short)A(1), (short)A(2), A(3), A(4), A(5));
                return 0;
            case 77:
                EmitScrollLayers((ushort*)P(0), A(1));
                return 0;
            case 78:
                EmitInterpolatedSpriteTrail((ushort*)P(0), (long*)P(1), A(2), A(3));
                return 0;
            case 79:
                InitPool56(A(0), P(1), P(2));
                return 0;
            case 80:
                return SfxScriptDispatchMap(ctx, (long)SfxIndexedRecordPtr(P(0), P(1)));
            case 81:
                return (int)SfxUpdateParticles(P(0));
            case 82:
                InitPool108(A(0), P(1));
                return 0;
            case 83:
                return SfxScriptDispatchMap(ctx, CreateRibbonEffectSlot(P(0), P(1), (ulong)P(2), (ulong)P(3)));
            case 84:
                MoveObjectWithAttachments(P(0), P(1), (short*)P(2));
                return 0;
            case 85:
                Pool56ReleaseOwner(P(0), P(1));
                return 0;
            case 86:
                return (int)SfxUpdateKeyframeTracks(P(0), P(1));
            case 87:
                return SfxScriptDispatchMap(ctx, Pool56FindFree(P(0)));
            case 88:
                sub_1800354E0(P(0), P(1));
                return 0;
            case 89:
                SfxEmitParticles(P(0), P(1), (short*)P(2), (int*)P(3));
                return 0;
            case 90:
                ResetSfxWorkArea();
                return 0;
            case 91:
                return SfxScriptDispatchMap(ctx, (long)PsxObjectAlloc(P(0), A(1)));
            case 92:
                StartPsxTask(A(0), (short)A(1), A(2), A(3));
                return 0;
            case 93:
                return (int)SfxTickEffectTasks();
            case 94:
                return (int)ComputeArenaBytes(A(0), A(1));
            case 95:
                PsxInitEntryBlock((uint)A(0), P(1), A(2), A(3), A(4));
                return 0;
            case 96:
                ClearPoolSlotHeaders((uint)A(0));
                return 0;
            case 97:
                return SfxScriptDispatchMap(ctx, (long)SfxStartScriptSlot((uint)A(0)));
            case 98:
                return (int)SfxTickScriptSlots((uint)A(0));
            case 99:
                return (int)FindPairSlot(A(0));
            case 100:
                SetSummonMotFrame(A(0), A(1));
                return 0;
            case 101:
                PopRotTransMatrix();
                return 0;
            case 102:
                return SfxScriptDispatchMap(ctx, (long)PsxGetTableEntry(A(0), A(1)));
            case 103:
                return (int)PsxTableEntrySize(A(0), A(1));
            case 104:
                SfxClearWord1FC();
                return 0;
            case 105:
                return (int)PsxChannelReset(A(0));
            case 106:
                return (int)PsxChannelFlip();
            case 107:
                SfxLoaderStep();
                return 0;
            case 108:
                sub_1800313D0();
                return 0;
            case 109:
                return (int)SeqStateBegin((short)A(0));
            case 110:
                SeqStateUpdate();
                return 0;
            case 111:
                SfxArenaInit(P(0), A(1));
                return 0;
            case 112:
                return SfxScriptDispatchMap(ctx, (long)PsxArenaAlloc(P(0), A(1)));
            case 113:
                return (int)SfxArenaResetIfShort(P(0), A(1));
            case 114:
                SfxArenaFillBlock(P(0));
                return 0;
            case 115:
                SfxPoolsReset();
                return 0;
            case 116:
                SfxTickParticleSystem();
                return 0;
            case 117:
                return SfxScriptDispatchMap(ctx, CreateTrailEffect(P(0), P(1)));
            case 118:
                MoveSfxObject(P(0), P(1));
                return 0;
            case 119:
                DetachSfxObject(P(0));
                return 0;
            case 120:
                SfxRegisterTableSlot3((ulong)P(0));
                return 0;
            case 121:
                RestoreMatrixDepth();
                return 0;
            case 122:
                SetMatrixDepth(A(0));
                return 0;
            case 123:
                ClearActiveObjectAddr();
                return 0;
            case 124:
                return (int)ObjSelectDistanceById(A(0));
            case 125:
                return (int)ObjSizeById(A(0));
            case 126:
                HostGetObjectPositionById(A(0), P(1));
                return 0;
            case 127:
                HostQueryObjPositionById(A(0), P(1));
                return 0;
            default:
                return 0;
        }
    }

    private static int SfxScriptDispatchOps128To191(long ctx, int op)
    {
        Recompiler.Runtime.Tracer.Enter("SfxScriptDispatchOps128To191(ctx, op)", ctx, op);
        int A(int i) => (int)PsxGetCallArg(ctx, i);
        long P(int i) => PsxGetCallArgPtr(ctx, i);

        switch (op)
        {
            case 128:
                HostQueryObjAnchorPosById(A(0), A(1), P(2));
                return 0;
            case 129:
                HostSetObjectPositionById(A(0), P(1));
                return 0;
            case 130:
                return (int)HostObjQueryAccess(A(0), P(1), (uint*)P(2), (uint*)P(3));
            case 131:
                HostGetObjectRotationById(A(0), P(1));
                return 0;
            case 132:
                return (int)IsObjNotFlippedById(A(0));
            case 134:
                ObjAimPointById(A(0), A(1), (short*)P(2));
                return 0;
            case 135:
                return (int)ObjDepthOrderSignById(A(0), A(1));
            case 136:
                return (int)ObjDistanceSixthPlusById(A(0), A(1));
            case 137:
                return (int)SetMeshFadeForObjectList(A(0), A(1));
            case 138:
                HostExecuteVfxForObjects(A(0));
                return 0;
            case 139:
                PsxAddPrimAtDepthWithMode(A(0), A(1), (ulong)P(2), A(3));
                return 0;
            case 140:
                HostRequestBattleFiguresForObjects(A(0));
                return 0;
            case 141:
                PsxAddPrimAtDepth(A(0), A(1), (ulong)P(2));
                return 0;
            case 142:
                SampleColorRamp(P(0), (byte*)P(1), A(2));
                return 0;
            case 143:
                if (CurrentEffectId != (int)SpecialEffectId.Stop)
                {
                    PsxAddPrimWithMode(A(0), (uint*)P(1), (ulong)P(2), A(3));
                }

                return 0;
            case 144:
                EmitSplitE7Prims(A(0), (short)A(1), (short)A(2), (short)A(3), A(4), (short)A(5), (ushort)A(6));
                return 0;
            case 145:
                DrawMorphModelByBone((short*)P(0), A(1), A(2), (ushort)A(3), A(4), A(5));
                return 0;
            case 146:
                SfxDrawWarpGrid((uint)A(0), A(1), A(2), A(3), A(4), A(5), A(6), (short)A(7), (short)A(8), (short*)P(9),
                    (int*)P(10), P(11), A(12), A(13));
                return 0;
            case 147:
                ModifySummonModelAbr(A(0), A(1));
                return 0;
            case 148:
                PushRotTransMatrix((short*)P(0));
                return 0;
            case 149:
                GetSummonBonePos(A(0), A(1), (ushort*)P(2));
                return 0;
            case 150:
                return SfxScriptDispatchMap(ctx, (long)PsxBumpAlloc(A(0)));
            case 151:
                FreeEffModelSlot(A(0));
                return 0;
            case 152:
                HostObjCmd13((uint)A(0), (uint)A(1));
                return 0;
            case 153:
                PsxAddWaveGradient((uint*)P(0), (short)A(1), (uint)A(2), A(3), (short)A(4), A(5), P(6), A(7), A(8), A(9),
                    A(10));
                return 0;
            case 154:
                HiModifyEffModelAbr(A(0), A(1));
                return 0;
            case 155:
                ModifyEffModelRgb(A(0), (ushort)A(1), (ushort)A(2), (ushort)A(3));
                return 0;
            case 156:
                HostObjSetFacing((uint)A(0), A(1));
                return 0;
            case 157:
                ShowSummonModelMesh(A(0), (sbyte)A(1));
                return 0;
            case 158:
                HideSummonModelMesh(A(0), (sbyte)A(1));
                return 0;
            case 159:
                return SfxScriptDispatchMap(ctx, MulRotMatrix((short*)P(0)));
            case 160:
                HostObjCmd0D((uint)A(0), (uint)A(1));
                return 0;
            case 161:
                return (int)HostObjCmd0DOn((uint)A(0));
            case 162:
                DrawEffModelByBone((short*)P(0), A(1), A(2), A(3));
                return 0;
            case 163:
                DrawMorphEffModel(P(0), P(1), P(2), A(3), A(4), (ushort)A(5));
                return 0;
            case 164:
                GetSummonBoneMatrix(A(0), A(1), (void*)P(2));
                return 0;
            case 165:
                // The original reads arguments 0..3 (register slots, no memory access) for the empty host stub.
                return (int)sub_180002FD0();
            case 166:
                return (int)HostSendVramRect66((short*)P(0), (uint)A(1), (uint)A(2));
            case 167:
                HiDebugPsgData(A(0), (sbyte)A(1));
                return 0;
            case 168:
                HostQueryObjScaleById(A(0), (uint*)P(1));
                return 0;
            case 169:
                HostSetObjScaleById(A(0), (uint*)P(1));
                return 0;
            case 170:
                HostSetObjRotation(A(0), P(1));
                return 0;
            case 171:
                return (int)RegisterTexPtrModel((ulong)P(0), P(1), P(2));
            case 172:
                SfxMatrixPointZ(P(0), A(1), (ushort*)P(2));
                return 0;
            case 173:
                SfxMatrixAimAngles(P(0), (ushort*)P(1));
                return 0;
            case 174:
                GetObjectHostPosition(A(0), A(1), (ushort*)P(2));
                return 0;
            case 175:
                SetObjectMeshVisibilityById(A(0), A(1));
                return 0;
            case 176:
                HostControlEyeTextureAnimation(A(0), (uint)A(1), (uint)A(2));
                return 0;
            case 177:
                HostSendCommand0EMode2((uint)A(0), (uint)A(1), P(2));
                return 0;
            case 178:
                HostSendCommand0EMode2FromBlock((uint)A(0), A(1), P(2));
                return 0;
            case 179:
                StartPsxTask14((uint)A(0), (uint)A(1));
                return 0;
            case 180:
                SfxVoiceSetSource(A(0), A(1));
                return 0;
            case 181:
                SfxVoiceSetAllSources((uint)A(0));
                return 0;
            case 182:
                return (int)SfxVoicePlay(A(0), (uint)A(1), (uint)A(2));
            case 183:
                return (int)HostObjectCheckAction(A(0), A(1));
            case 184:
                return SfxScriptDispatchMap(ctx, sub_180002FD0());
            case 185:
                SetEffModelOffset(A(0), (short)A(1));
                return 0;
            case 186:
                HostFadeObjectMeshIfAllowed(A(0), (uint)A(1));
                return 0;
            case 187:
                HostObjCmd12((uint)A(0), A(1));
                return 0;
            case 188:
                SelectReg3ObjectFromList(A(0));
                return 0;
            case 189:
                // Argument 4 is passed as a resolved host pointer in a 64-bit stack slot; HostMessage78 ignores a5.
                HostCaptureBackgroundTexture(A(0), A(1), A(2), A(3), (int)P(4), A(5), A(6), A(7));
                return 0;
            case 190:
                return SfxScriptDispatchMap(ctx, PsxGetContextQword(A(0)));
            case 191:
                return (int)GetSplitMdlVertex(A(0), A(1));
            default:
                return 0;
        }
    }

    private static int SfxScriptDispatchOps192To215(long ctx, int op)
    {
        Recompiler.Runtime.Tracer.Enter("SfxScriptDispatchOps192To215(ctx, op)", ctx, op);
        int A(int i) => (int)PsxGetCallArg(ctx, i);
        long P(int i) => PsxGetCallArgPtr(ctx, i);

        switch (op)
        {
            case 192:
            case 195:
                return (int)sub_180002FD0();
            case 193:
                SplitMdlVertex(A(0), (uint)A(1), P(2));
                return 0;
            case 196:
                return (int)GetMdlVertexPtr(A(0), (ushort)A(1), (ulong)P(2), (uint*)P(3), A(4));
            case 197:
                return (int)HostObjCmd1A((uint)A(0));
            case 198:
                return (int)EffModelPartCount(A(0));
            case 199:
                if (CurrentEffectId != (int)SpecialEffectId.Sinkhole)
                {
                    DrawSliceEffModel(P(0), P(1), P(2), A(3));
                }

                return 0;
            case 200:
                if (CurrentEffectId != (int)SpecialEffectId.Sinkhole)
                {
                    SetEffModelSlice(A(0), (short)A(1));
                }

                return 0;
            case 201:
                PsxSelectTableRecord(A(0));
                return 0;
            case 202:
                return (int)HostObjCmd09((uint)A(0));
            case 203:
                return (int)HostObjQuery14IsZero((uint)A(0));
            case 204:
                return (int)SfxRangeFraction(A(0), A(1), A(2), A(3), A(4), (int*)P(5));
            case 205:
                StartPsxTask10(A(0), (uint)A(1));
                return 0;
            case 206:
                return (int)RegisterSoModel((ushort*)P(0), (sbyte)A(1));
            case 207:
                return (int)IsObjectStateEqual(A(0), A(1));
            case 208:
                RegisterSoSummonModel((ushort*)P(0), (int*)P(1), (uint)A(2), (ulong)P(3));
                return 0;
            case 210:
                return (int)SfxVoiceRestart(A(0), (uint)A(1), (uint)A(2), A(3));
            case 211:
                HostEnableLastBossFlag();
                return 0;
            case 212:
                SfxVoiceStop(A(0));
                return 0;
            case 213:
                HostResetObjectStatuses(A(0));
                return 0;
            case 215:
                // The original also passes argument 1 (pointer) and 2; FillIdTable435 reads only its first argument.
                FillIdTable435((ushort*)P(0));
                return 0;
            default:
                return 0;
        }
    }
}
