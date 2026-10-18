#include "Unity.h"

#include <Windows.h>
#include <cstring>
#include <cwchar>

// Unity resolves the Mono API through GetProcAddress during startup. Hooking that
// import lets us substitute only the two required Mono functions without relying
// on version-specific Unity.exe addresses or a general-purpose hooking library.

using fp_get_proc_address = FARPROC(WINAPI*)(HMODULE, LPCSTR);
using fp_mono_set_commandline_arguments = void(*)(int, const char*[], const char*);
using fp_mono_runtime_unhandled_exception_policy_set = void(*)(int);
using fp_mono_jit_parse_options = void(*)(int, char*[]);
using fp_mono_debug_init = void(*)(int);
#if defined(_WIN64)
using fp_queue_user_apc = DWORD(WINAPI*)(PAPCFUNC, HANDLE, ULONG_PTR);
#endif

static fp_get_proc_address real_get_proc_address = nullptr;
static fp_mono_set_commandline_arguments real_mono_set_commandline_arguments = nullptr;
static fp_mono_runtime_unhandled_exception_policy_set real_mono_runtime_unhandled_exception_policy_set = nullptr;
static fp_mono_jit_parse_options mono_jit_parse_options = nullptr;
static fp_mono_debug_init mono_debug_init = nullptr;
#if defined(_WIN64)
static fp_queue_user_apc real_queue_user_apc = nullptr;
static volatile LONG mono_thread_cleanup_hook_installed = 0;
#endif
static volatile LONG mono_debugger_initialized = 0;
static volatile LONG debugger_ready_message_shown = 0;

static bool PatchImportedFunction(HMODULE module, const char* procName, PVOID replacement, PVOID* original)
{
	if (!module || !procName || !replacement || !original)
		return false;

	auto base = reinterpret_cast<BYTE*>(module);
	auto dosHeader = reinterpret_cast<IMAGE_DOS_HEADER*>(base);
	if (dosHeader->e_magic != IMAGE_DOS_SIGNATURE || dosHeader->e_lfanew <= 0)
		return false;

	auto ntHeaders = reinterpret_cast<IMAGE_NT_HEADERS*>(base + dosHeader->e_lfanew);
	if (ntHeaders->Signature != IMAGE_NT_SIGNATURE)
		return false;

	const IMAGE_DATA_DIRECTORY& importDirectory = ntHeaders->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT];
	if (!importDirectory.VirtualAddress)
		return false;

	auto importDescriptor = reinterpret_cast<IMAGE_IMPORT_DESCRIPTOR*>(base + importDirectory.VirtualAddress);
	for (; importDescriptor->Name; ++importDescriptor)
	{
		if (!importDescriptor->OriginalFirstThunk)
			continue;

		auto nameThunk = reinterpret_cast<IMAGE_THUNK_DATA*>(base + importDescriptor->OriginalFirstThunk);
		auto addressThunk = reinterpret_cast<IMAGE_THUNK_DATA*>(base + importDescriptor->FirstThunk);
		for (; nameThunk->u1.AddressOfData; ++nameThunk, ++addressThunk)
		{
			if (IMAGE_SNAP_BY_ORDINAL(nameThunk->u1.Ordinal))
				continue;

			auto import = reinterpret_cast<IMAGE_IMPORT_BY_NAME*>(base + nameThunk->u1.AddressOfData);
			if (std::strcmp(reinterpret_cast<const char*>(import->Name), procName) != 0)
				continue;

			auto slot = reinterpret_cast<PVOID*>(&addressThunk->u1.Function);
			DWORD oldProtect;
			if (!VirtualProtect(slot, sizeof(*slot), PAGE_READWRITE, &oldProtect))
				return false;

			*original = InterlockedExchangePointer(reinterpret_cast<PVOID volatile*>(slot), replacement);

			DWORD ignored;
			VirtualProtect(slot, sizeof(*slot), oldProtect, &ignored);
			return *original != nullptr;
		}
	}

	return false;
}

#if defined(_WIN64)
// These offsets and instruction bytes belong to the x64 Mono build shipped with
// Unity 5.2. Validation at the call site prevents this workaround from modifying
// unrelated QueueUserAPC calls or incompatible Mono versions.
static constexpr SIZE_T notify_thread_call_length = 20;
static constexpr SIZE_T callback_displacement_offset = 13;
static constexpr SIZE_T callback_instruction_end_offset = 9;
static constexpr SIZE_T mono_thread_handle_offset = 0x18;
static constexpr SIZE_T mono_thread_id_offset = 0x58;
static constexpr SIZE_T debugger_tls_terminated_offset = 0x120;

static bool IsDebuggerThreadNotification(PAPCFUNC callback, HANDLE thread, void* tls, void* threadObject, const BYTE* returnAddress)
{
	if (!callback || !thread || !tls || !threadObject || !returnAddress)
		return false;

	__try
	{
		// Unity 5.2 debugger-agent's notify_thread():
		// mov rdx,[rdi+18h]; lea rcx,[notify_thread_apc]; xor r8d,r8d; call QueueUserAPC
		const BYTE expectedPrefix[] = { 0x48, 0x8B, 0x57, 0x18, 0x48, 0x8D, 0x0D };
		const BYTE expectedSuffix[] = { 0x45, 0x33, 0xC0, 0xFF, 0x15 };
		if (std::memcmp(returnAddress - notify_thread_call_length, expectedPrefix, sizeof(expectedPrefix)) != 0 ||
			std::memcmp(returnAddress - callback_instruction_end_offset, expectedSuffix, sizeof(expectedSuffix)) != 0)
			return false;

		INT32 callbackDisplacement;
		std::memcpy(&callbackDisplacement, returnAddress - callback_displacement_offset, sizeof(callbackDisplacement));
		const BYTE* expectedCallback = returnAddress - callback_instruction_end_offset + callbackDisplacement;
		if (reinterpret_cast<const BYTE*>(callback) != expectedCallback)
			return false;

		auto threadBytes = reinterpret_cast<const BYTE*>(threadObject);
		if (*reinterpret_cast<HANDLE const*>(threadBytes + mono_thread_handle_offset) != thread)
			return false;

		return *reinterpret_cast<const ULONG_PTR*>(threadBytes + mono_thread_id_offset) == GetThreadId(thread);
	}
	__except (EXCEPTION_EXECUTE_HANDLER)
	{
		return false;
	}
}

extern "C" DWORD WINAPI QueueUserApcHook(PAPCFUNC callback, HANDLE thread, ULONG_PTR data);

extern "C" DWORD WINAPI QueueUserApcHookImpl(PAPCFUNC callback, HANDLE thread, ULONG_PTR data,
	void* tls, void* threadObject, const BYTE* returnAddress)
{
	const DWORD result = real_queue_user_apc(callback, thread, data);

	if (!result && IsDebuggerThreadNotification(callback, thread, tls, threadObject, returnAddress) &&
		WaitForSingleObject(thread, 0) == WAIT_OBJECT_0)
	{
		// DebuggerTlsData::terminated. count_thread() excludes these records from wait_for_suspend().
		InterlockedExchange(reinterpret_cast<volatile LONG*>(reinterpret_cast<BYTE*>(tls) + debugger_tls_terminated_offset), TRUE);
	}

	return result;
}

static void InstallMonoThreadCleanupHook(HMODULE monoModule)
{
	if (InterlockedCompareExchange(&mono_thread_cleanup_hook_installed, 1, 0) != 0)
		return;

	if (PatchImportedFunction(monoModule, "QueueUserAPC",
		reinterpret_cast<PVOID>(QueueUserApcHook), reinterpret_cast<PVOID*>(&real_queue_user_apc)))
		return;

	InterlockedExchange(&mono_thread_cleanup_hook_installed, 0);
}
#else
static void InstallMonoThreadCleanupHook(HMODULE monoModule)
{
	UNREFERENCED_PARAMETER(monoModule);
}
#endif

static void mono_set_commandline_arguments_hook(int argc, const char* argv[], const char* baseDir)
{
	if (mono_jit_parse_options && mono_debug_init &&
		InterlockedCompareExchange(&mono_debugger_initialized, 1, 0) == 0)
	{
		char debuggerAgentOptions[] = "--debugger-agent=transport=dt_socket,embedding=1,server=y,defer=y";
		char* jitArguments[] = { debuggerAgentOptions };
		mono_jit_parse_options(ARRAYSIZE(jitArguments), jitArguments);
		mono_debug_init(1); // MONO_DEBUG_FORMAT_MONO
	}

	real_mono_set_commandline_arguments(argc, argv, baseDir);
}

static void mono_runtime_unhandled_exception_policy_set_hook(int policy)
{
	real_mono_runtime_unhandled_exception_policy_set(policy);

	if (InterlockedCompareExchange(&debugger_ready_message_shown, 1, 0) == 0)
		MessageBoxA(nullptr, "Mono debugger initialized.\nIt is safe to attach your debugger now.", "Memoria Injection", MB_OK | MB_ICONINFORMATION);
}

static bool IsMonoModule(HMODULE module)
{
	wchar_t path[MAX_PATH];
	const DWORD length = GetModuleFileNameW(module, path, ARRAYSIZE(path));
	if (length == 0 || length == ARRAYSIZE(path))
		return false;

	const wchar_t* fileName = wcsrchr(path, L'\\');
	fileName = fileName ? fileName + 1 : path;
	return _wcsicmp(fileName, L"mono.dll") == 0;
}

static FARPROC WINAPI get_proc_address_hook(HMODULE module, LPCSTR procName)
{
	const FARPROC original = real_get_proc_address(module, procName);
	if (!original || reinterpret_cast<ULONG_PTR>(procName) <= 0xFFFF)
		return original;

	const bool hookArguments = std::strcmp(procName, "mono_set_commandline_arguments") == 0;
	const bool hookPolicy = std::strcmp(procName, "mono_runtime_unhandled_exception_policy_set") == 0;
	if ((!hookArguments && !hookPolicy) || !IsMonoModule(module))
		return original;

	InstallMonoThreadCleanupHook(module);

	if (hookArguments)
	{
		real_mono_set_commandline_arguments = reinterpret_cast<fp_mono_set_commandline_arguments>(original);
		mono_jit_parse_options = reinterpret_cast<fp_mono_jit_parse_options>(real_get_proc_address(module, "mono_jit_parse_options"));
		mono_debug_init = reinterpret_cast<fp_mono_debug_init>(real_get_proc_address(module, "mono_debug_init"));
		return reinterpret_cast<FARPROC>(mono_set_commandline_arguments_hook);
	}

	if (hookPolicy)
	{
		real_mono_runtime_unhandled_exception_policy_set = reinterpret_cast<fp_mono_runtime_unhandled_exception_policy_set>(original);
		return reinterpret_cast<FARPROC>(mono_runtime_unhandled_exception_policy_set_hook);
	}

	return original;
}

void InstallMonoHooks()
{
	HMODULE mainModule = GetModuleHandleW(nullptr);
	if (!mainModule)
		return;

	PatchImportedFunction(mainModule, "GetProcAddress",
		reinterpret_cast<PVOID>(get_proc_address_hook), reinterpret_cast<PVOID*>(&real_get_proc_address));
}
