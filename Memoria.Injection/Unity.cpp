#include "Unity.h"
#include "LoadMonoDynamically.h"
#include <detours/detours.h>
#include <stdint.h>

typedef void(*fp_unity_bootstrap)();

#if defined(_WIN64)
static const uintptr_t UNITY_BOOTSTRAP_RVA = 0x20F430ULL;
#else
static const uintptr_t UNITY_BOOTSTRAP_RVA = 0x001CE990UL;
#endif

static fp_unity_bootstrap real_unity_bootstrap = nullptr;
static fp_mono_set_commandline_arguments real_mono_set_commandline_arguments = nullptr;
static fp_mono_runtime_unhandled_exception_policy_set real_mono_runtime_unhandled_exception_policy_set = nullptr;
static volatile LONG g_unity_init_done = 0;
static volatile LONG g_mono_hook_installed = 0;

static bool IsExecutableAddress(const void* address)
{
	MEMORY_BASIC_INFORMATION mbi = {};
	if (VirtualQuery(address, &mbi, sizeof(mbi)) == 0)
		return false;

	if (mbi.State != MEM_COMMIT)
		return false;

	const DWORD executableFlags = PAGE_EXECUTE | PAGE_EXECUTE_READ | PAGE_EXECUTE_READWRITE | PAGE_EXECUTE_WRITECOPY;
	return (mbi.Protect & executableFlags) != 0;
}

static void mono_set_commandline_arguments_hook(int argc, const char **argv, char *baseDir)
{
	const char *jitOptions = "--debugger-agent=transport=dt_socket,embedding=1,server=y,defer=y";
	char *jitArguments[1];
	jitArguments[0] = _strdup(jitOptions);
	mono_jit_parse_options(1, jitArguments);
	mono_debug_init(1);

	free(jitArguments[0]);

	real_mono_set_commandline_arguments(argc, argv, baseDir);
}

static void mono_runtime_unhandled_exception_policy_set_hook(int policy)
{
	real_mono_runtime_unhandled_exception_policy_set(policy);

	MessageBoxA(NULL, "Mono debugger initialized.\nIt is safe to attach your debugger now.", "Memoria Injection", MB_OK | MB_ICONINFORMATION);
}

static void unity_bootstrap_hook()
{
	real_unity_bootstrap();

	if (InterlockedCompareExchange(&g_unity_init_done, 1, 0) == 0)
	{
		UnityInit();
	}
}

void UnityInit()
{
	SetupMono();
	if (InterlockedCompareExchange(&g_mono_hook_installed, 1, 0) != 0)
		return;

	// Hook mono_set_commandline_arguments via Detours so the trampoline
	// correctly calls back into the original implementation.
	real_mono_set_commandline_arguments = mono_set_commandline_arguments;
	real_mono_runtime_unhandled_exception_policy_set = mono_runtime_unhandled_exception_policy_set;
	LONG status = DetourTransactionBegin();
	if (status != NO_ERROR)
		return;

	DetourUpdateThread(GetCurrentThread());
	status = DetourAttach(&(PVOID&)real_mono_set_commandline_arguments, (PVOID)mono_set_commandline_arguments_hook);
	if (status != NO_ERROR)
	{
		DetourTransactionAbort();
		return;
	}

	status = DetourAttach(&(PVOID&)real_mono_runtime_unhandled_exception_policy_set, (PVOID)mono_runtime_unhandled_exception_policy_set_hook);
	if (status != NO_ERROR)
	{
		DetourTransactionAbort();
		return;
	}

	status = DetourTransactionCommit();
	if (status != NO_ERROR)
	{
		InterlockedExchange(&g_mono_hook_installed, 0);
	}
}

void InstallUnityBootstrapHook()
{
	HMODULE mainModule = GetModuleHandle(NULL);
	if (!mainModule)
		return;

	real_unity_bootstrap = reinterpret_cast<fp_unity_bootstrap>(reinterpret_cast<uintptr_t>(mainModule) + UNITY_BOOTSTRAP_RVA);
	if (!IsExecutableAddress((const void*)real_unity_bootstrap))
		return;

	LONG status = DetourTransactionBegin();
	if (status != NO_ERROR)
		return;

	DetourUpdateThread(GetCurrentThread());
	status = DetourAttach(&(PVOID&)real_unity_bootstrap, (PVOID)unity_bootstrap_hook);
	if (status != NO_ERROR)
	{
		DetourTransactionAbort();
		return;
	}

	DetourTransactionCommit();
}
