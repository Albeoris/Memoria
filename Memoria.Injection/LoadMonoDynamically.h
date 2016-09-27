#pragma once

// https://github.com/Unity-Technologies/mono/tree/unity-staging/unity/smalltestcases/asyncsocketshutdown
#include <Windows.h>

// define a ProcPtr type for each API
#define DO_API(r, n, p) typedef r(*fp_##n) p;
#include "MonoFunctions.h"

// declare storage for each API's function pointers
#define DO_API(r, n, p) fp_##n n = NULL;
#include "MonoFunctions.h"

HMODULE gMonoModule;

inline int SetupMono()
{
	gMonoModule = LoadLibraryW(L"Unity_Data\\Mono\\mono.dll");
	if (!gMonoModule) {
		return 1;
	}

	bool success = true;
#define DO_API(r, n, p)                                                                                                \
	n = (fp_##n)GetProcAddress(gMonoModule, #n);                                                                       \
	if (!n) {                                                                                                          \
		success = false;                                                                                               \
	}
#include "MonoFunctions.h"

	return success ? 0 : 2;
}