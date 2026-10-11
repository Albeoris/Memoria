
// F030 BattleCallbackFn: int32_t (*)(int32_t code, int32_t arg0, int32_t arg1, int32_t arg2, int32_t arg3, void *p); manual; qword_1801C1DE8, SFX_InitSystem:a1 — Memoria SFX.Callback; ref:R001/Assembly-CSharp/Global/SFX/SFX.cs#L2537
global using unsafe BattleCallbackFn = delegate* unmanaged<int, int, int, int, int, void*, int>;

// F019 MipsDecodeAllCallbackFn: __int64 (__fastcall *)(__int64, int, int *); call in MipsDecodeAll
global using unsafe MipsDecodeAllCallbackFn = delegate* unmanaged<long, int, int*, long>;

// F020 MipsDecodeAllCallbackFn2: __int64 (__fastcall *)(__int64, int); call in MipsDecodeAll
global using unsafe MipsDecodeAllCallbackFn2 = delegate* unmanaged<long, int, long>;

// F023 MipsDecodeAllCallbackFn3: __int64 (__fastcall *)(__int64, _QWORD, int *); call in MipsDecodeAll
global using unsafe MipsDecodeAllCallbackFn3 = delegate* unmanaged<long, ulong, int*, long>;

// F015 PsxCallContextCtorCallbackFn: void (*)(void *); call in PsxCallContextCtor
global using unsafe PsxCallContextCtorCallbackFn = delegate* unmanaged<void*, void>;

// F025 SfxLoaderStepCallbackFn: void (__fastcall *)(_QWORD, int *, _QWORD, __int64); call in SfxLoaderStep
global using unsafe SfxLoaderStepCallbackFn = delegate* unmanaged<ulong, int*, ulong, long, void>;

// F018 SfxTickEffectTasksCallbackFn: unsigned int (__fastcall *)(__int64, unsigned __int8 *, int *, char *, _QWORD); call in SfxTickEffectTasks
global using unsafe SfxTickEffectTasksCallbackFn = delegate* unmanaged<long, byte*, int*, byte*, ulong, uint>;

// F026 SfxUpdateParticlesCallbackFn: __int64 (__fastcall *)(unsigned __int64, __int64, __int64 *, unsigned __int8 *, int, int); call in SfxUpdateParticles
global using unsafe SfxUpdateParticlesCallbackFn = delegate* unmanaged<ulong, long, long*, byte*, int, int, long>;

// F027 SfxUpdateParticlesCallbackFn2: void (__fastcall *)(unsigned __int64, __int64, __int64 *, __int64 *, int *, int *, int *, int, int); call in SfxUpdateParticles
global using unsafe SfxUpdateParticlesCallbackFn2 = delegate* unmanaged<ulong, long, long*, long*, int*, int*, int*, int, int, void>;

// F017 StartPsxTaskCallbackFn: __int64 (__fastcall *)(__int64, unsigned __int8 *, int *, char *, _DWORD *); call in StartPsxTask
global using unsafe StartPsxTaskCallbackFn = delegate* unmanaged<long, byte*, int*, byte*, uint*, long>;

// F029 StartPsxTaskCallbackFn2: unsigned int (__fastcall *)(_QWORD, _QWORD, int *, char *, __int64 *); call in StartPsxTask
global using unsafe StartPsxTaskCallbackFn2 = delegate* unmanaged<ulong, ulong, int*, byte*, long*, uint>;

// F016 StartPsxTaskFn: __int64 (__fastcall *)(int, int, int, int, __int64); call through qword_18004AB80
global using unsafe StartPsxTaskFn = delegate* unmanaged<int, int, int, int, long, long>;

// F007 Sub1800354E0CallbackFn: __int64 (__fastcall *)(__int64 *, __int64 *, _QWORD, _QWORD); call in sub_1800354E0
global using unsafe Sub1800354E0CallbackFn = delegate* unmanaged<long*, long*, ulong, ulong, long>;
