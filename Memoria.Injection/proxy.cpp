#include "Windows.h"
#include <string>
#include "proxy.h"

// Ensure output is named "version.dll" and exports match the real version.dll names
#pragma comment(linker, "/OUT:version.dll")

#if defined(_M_IX86)
#pragma comment(linker, "/EXPORT:GetFileVersionInfoSizeA=_My_GetFileVersionInfoSizeA@8")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoSizeW=_My_GetFileVersionInfoSizeW@8")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoA=_My_GetFileVersionInfoA@16")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoW=_My_GetFileVersionInfoW@16")
#pragma comment(linker, "/EXPORT:VerQueryValueA=_My_VerQueryValueA@16")
#pragma comment(linker, "/EXPORT:VerQueryValueW=_My_VerQueryValueW@16")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoByHandle=_My_GetFileVersionInfoByHandle@16")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoExA=_My_GetFileVersionInfoExA@20")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoExW=_My_GetFileVersionInfoExW@20")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoSizeExA=_My_GetFileVersionInfoSizeExA@12")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoSizeExW=_My_GetFileVersionInfoSizeExW@12")
#pragma comment(linker, "/EXPORT:VerFindFileA=_My_VerFindFileA@32")
#pragma comment(linker, "/EXPORT:VerFindFileW=_My_VerFindFileW@32")
#pragma comment(linker, "/EXPORT:VerInstallFileA=_My_VerInstallFileA@32")
#pragma comment(linker, "/EXPORT:VerInstallFileW=_My_VerInstallFileW@32")
#pragma comment(linker, "/EXPORT:VerLanguageNameA=_My_VerLanguageNameA@12")
#pragma comment(linker, "/EXPORT:VerLanguageNameW=_My_VerLanguageNameW@12")
#else
#pragma comment(linker, "/EXPORT:GetFileVersionInfoSizeA=My_GetFileVersionInfoSizeA")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoSizeW=My_GetFileVersionInfoSizeW")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoA=My_GetFileVersionInfoA")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoW=My_GetFileVersionInfoW")
#pragma comment(linker, "/EXPORT:VerQueryValueA=My_VerQueryValueA")
#pragma comment(linker, "/EXPORT:VerQueryValueW=My_VerQueryValueW")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoByHandle=My_GetFileVersionInfoByHandle")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoExA=My_GetFileVersionInfoExA")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoExW=My_GetFileVersionInfoExW")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoSizeExA=My_GetFileVersionInfoSizeExA")
#pragma comment(linker, "/EXPORT:GetFileVersionInfoSizeExW=My_GetFileVersionInfoSizeExW")
#pragma comment(linker, "/EXPORT:VerFindFileA=My_VerFindFileA")
#pragma comment(linker, "/EXPORT:VerFindFileW=My_VerFindFileW")
#pragma comment(linker, "/EXPORT:VerInstallFileA=My_VerInstallFileA")
#pragma comment(linker, "/EXPORT:VerInstallFileW=My_VerInstallFileW")
#pragma comment(linker, "/EXPORT:VerLanguageNameA=My_VerLanguageNameA")
#pragma comment(linker, "/EXPORT:VerLanguageNameW=My_VerLanguageNameW")
#endif

// Proxy loader for system version.dll
static HMODULE g_realVersion = NULL;

bool LoadRealVersion()
{
    if (g_realVersion) return true;

    std::wstring systemDir(MAX_PATH + 1, '\0');
    UINT nChars = GetSystemDirectoryW(&systemDir[0], systemDir.length());
    if (nChars == 0) return false;
    systemDir.resize(nChars);

    std::wstring path = systemDir + L"\\version.dll";
    g_realVersion = LoadLibraryW(path.c_str());
    if (!g_realVersion)
    {
        // fallback to normal load
        g_realVersion = LoadLibraryW(L"version.dll");
        if (!g_realVersion) return false;
    }
    return true;
}

// Function pointer typedefs for commonly used exports
typedef BOOL (WINAPI *pGetFileVersionInfoA)(LPCSTR, DWORD, DWORD, LPVOID);
typedef BOOL (WINAPI *pGetFileVersionInfoW)(LPCWSTR, DWORD, DWORD, LPVOID);
typedef DWORD (WINAPI *pGetFileVersionInfoSizeA)(LPCSTR, LPDWORD);
typedef DWORD (WINAPI *pGetFileVersionInfoSizeW)(LPCWSTR, LPDWORD);
typedef BOOL (WINAPI *pVerQueryValueA)(LPCVOID, LPCSTR, LPVOID*, PUINT);
typedef BOOL (WINAPI *pVerQueryValueW)(LPCVOID, LPCWSTR, LPVOID*, PUINT);
typedef BOOL (WINAPI *pGetFileVersionInfoByHandle)(HANDLE, DWORD, DWORD, LPVOID);
typedef BOOL (WINAPI *pGetFileVersionInfoExA)(DWORD, LPCSTR, DWORD, DWORD, LPVOID);
typedef BOOL (WINAPI *pGetFileVersionInfoExW)(DWORD, LPCWSTR, DWORD, DWORD, LPVOID);
typedef DWORD (WINAPI *pGetFileVersionInfoSizeExA)(DWORD, LPCSTR, LPDWORD);
typedef DWORD (WINAPI *pGetFileVersionInfoSizeExW)(DWORD, LPCWSTR, LPDWORD);
typedef BOOL (WINAPI *pVerFindFileA)(UINT, LPCSTR, LPCSTR, LPCSTR, LPSTR, PUINT, LPSTR, PUINT);
typedef BOOL (WINAPI *pVerFindFileW)(UINT, LPCWSTR, LPCWSTR, LPCWSTR, LPWSTR, PUINT, LPWSTR, PUINT);
typedef BOOL (WINAPI *pVerInstallFileA)(UINT, LPCSTR, LPCSTR, LPCSTR, LPCSTR, LPCSTR, LPCSTR, PUINT);
typedef BOOL (WINAPI *pVerInstallFileW)(UINT, LPCWSTR, LPCWSTR, LPCWSTR, LPCWSTR, LPCWSTR, LPCWSTR, PUINT);
typedef UINT (WINAPI *pVerLanguageNameA)(DWORD, LPSTR, UINT);
typedef UINT (WINAPI *pVerLanguageNameW)(DWORD, LPWSTR, UINT);

static pGetFileVersionInfoA real_GetFileVersionInfoA = NULL;
static pGetFileVersionInfoW real_GetFileVersionInfoW = NULL;
static pGetFileVersionInfoSizeA real_GetFileVersionInfoSizeA = NULL;
static pGetFileVersionInfoSizeW real_GetFileVersionInfoSizeW = NULL;
static pVerQueryValueA real_VerQueryValueA = NULL;
static pVerQueryValueW real_VerQueryValueW = NULL;
static pGetFileVersionInfoByHandle real_GetFileVersionInfoByHandle = NULL;
static pGetFileVersionInfoExA real_GetFileVersionInfoExA = NULL;
static pGetFileVersionInfoExW real_GetFileVersionInfoExW = NULL;
static pGetFileVersionInfoSizeExA real_GetFileVersionInfoSizeExA = NULL;
static pGetFileVersionInfoSizeExW real_GetFileVersionInfoSizeExW = NULL;
static pVerFindFileA real_VerFindFileA = NULL;
static pVerFindFileW real_VerFindFileW = NULL;
static pVerInstallFileA real_VerInstallFileA = NULL;
static pVerInstallFileW real_VerInstallFileW = NULL;
static pVerLanguageNameA real_VerLanguageNameA = NULL;
static pVerLanguageNameW real_VerLanguageNameW = NULL;

void ResolveAll()
{
    if (!g_realVersion) return;
    real_GetFileVersionInfoA = (pGetFileVersionInfoA)GetProcAddress(g_realVersion, "GetFileVersionInfoA");
    real_GetFileVersionInfoW = (pGetFileVersionInfoW)GetProcAddress(g_realVersion, "GetFileVersionInfoW");
    real_GetFileVersionInfoSizeA = (pGetFileVersionInfoSizeA)GetProcAddress(g_realVersion, "GetFileVersionInfoSizeA");
    real_GetFileVersionInfoSizeW = (pGetFileVersionInfoSizeW)GetProcAddress(g_realVersion, "GetFileVersionInfoSizeW");
    real_VerQueryValueA = (pVerQueryValueA)GetProcAddress(g_realVersion, "VerQueryValueA");
    real_VerQueryValueW = (pVerQueryValueW)GetProcAddress(g_realVersion, "VerQueryValueW");
    real_GetFileVersionInfoByHandle = (pGetFileVersionInfoByHandle)GetProcAddress(g_realVersion, "GetFileVersionInfoByHandle");
    real_GetFileVersionInfoExA = (pGetFileVersionInfoExA)GetProcAddress(g_realVersion, "GetFileVersionInfoExA");
    real_GetFileVersionInfoExW = (pGetFileVersionInfoExW)GetProcAddress(g_realVersion, "GetFileVersionInfoExW");
    real_GetFileVersionInfoSizeExA = (pGetFileVersionInfoSizeExA)GetProcAddress(g_realVersion, "GetFileVersionInfoSizeExA");
    real_GetFileVersionInfoSizeExW = (pGetFileVersionInfoSizeExW)GetProcAddress(g_realVersion, "GetFileVersionInfoSizeExW");
    real_VerFindFileA = (pVerFindFileA)GetProcAddress(g_realVersion, "VerFindFileA");
    real_VerFindFileW = (pVerFindFileW)GetProcAddress(g_realVersion, "VerFindFileW");
    real_VerInstallFileA = (pVerInstallFileA)GetProcAddress(g_realVersion, "VerInstallFileA");
    real_VerInstallFileW = (pVerInstallFileW)GetProcAddress(g_realVersion, "VerInstallFileW");
    real_VerLanguageNameA = (pVerLanguageNameA)GetProcAddress(g_realVersion, "VerLanguageNameA");
    real_VerLanguageNameW = (pVerLanguageNameW)GetProcAddress(g_realVersion, "VerLanguageNameW");
}

// Exported wrapper functions that forward to the real version.dll
extern "C" DWORD WINAPI My_GetFileVersionInfoSizeA(LPCSTR lptstrFilename, LPDWORD lpdwHandle)
{
    if (!LoadRealVersion()) return 0;
    if (!real_GetFileVersionInfoSizeA) ResolveAll();
    if (real_GetFileVersionInfoSizeA) return real_GetFileVersionInfoSizeA(lptstrFilename, lpdwHandle);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return 0;
}

extern "C" DWORD WINAPI My_GetFileVersionInfoSizeW(LPCWSTR lptstrFilename, LPDWORD lpdwHandle)
{
    if (!LoadRealVersion()) return 0;
    if (!real_GetFileVersionInfoSizeW) ResolveAll();
    if (real_GetFileVersionInfoSizeW) return real_GetFileVersionInfoSizeW(lptstrFilename, lpdwHandle);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return 0;
}

extern "C" BOOL WINAPI My_GetFileVersionInfoA(LPCSTR lptstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    if (!LoadRealVersion()) return FALSE;
    if (!real_GetFileVersionInfoA) ResolveAll();
    if (real_GetFileVersionInfoA) return real_GetFileVersionInfoA(lptstrFilename, dwHandle, dwLen, lpData);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return FALSE;
}

extern "C" BOOL WINAPI My_GetFileVersionInfoW(LPCWSTR lptstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    if (!LoadRealVersion()) return FALSE;
    if (!real_GetFileVersionInfoW) ResolveAll();
    if (real_GetFileVersionInfoW) return real_GetFileVersionInfoW(lptstrFilename, dwHandle, dwLen, lpData);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return FALSE;
}

extern "C" BOOL WINAPI My_VerQueryValueA(LPCVOID pBlock, LPCSTR lpSubBlock, LPVOID *lplpBuffer, PUINT puLen)
{
    if (!LoadRealVersion()) return FALSE;
    if (!real_VerQueryValueA) ResolveAll();
    if (real_VerQueryValueA) return real_VerQueryValueA(pBlock, lpSubBlock, lplpBuffer, puLen);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return FALSE;
}

extern "C" BOOL WINAPI My_VerQueryValueW(LPCVOID pBlock, LPCWSTR lpSubBlock, LPVOID *lplpBuffer, PUINT puLen)
{
    if (!LoadRealVersion()) return FALSE;
    if (!real_VerQueryValueW) ResolveAll();
    if (real_VerQueryValueW) return real_VerQueryValueW(pBlock, lpSubBlock, lplpBuffer, puLen);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return FALSE;
}

extern "C" BOOL WINAPI My_GetFileVersionInfoByHandle(HANDLE hFile, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    if (!LoadRealVersion()) return FALSE;
    if (!real_GetFileVersionInfoByHandle) ResolveAll();
    if (real_GetFileVersionInfoByHandle) return real_GetFileVersionInfoByHandle(hFile, dwHandle, dwLen, lpData);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return FALSE;
}

extern "C" BOOL WINAPI My_GetFileVersionInfoExA(DWORD dwFlags, LPCSTR lpwstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    if (!LoadRealVersion()) return FALSE;
    if (!real_GetFileVersionInfoExA) ResolveAll();
    if (real_GetFileVersionInfoExA) return real_GetFileVersionInfoExA(dwFlags, lpwstrFilename, dwHandle, dwLen, lpData);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return FALSE;
}

extern "C" BOOL WINAPI My_GetFileVersionInfoExW(DWORD dwFlags, LPCWSTR lpwstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    if (!LoadRealVersion()) return FALSE;
    if (!real_GetFileVersionInfoExW) ResolveAll();
    if (real_GetFileVersionInfoExW) return real_GetFileVersionInfoExW(dwFlags, lpwstrFilename, dwHandle, dwLen, lpData);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return FALSE;
}

extern "C" DWORD WINAPI My_GetFileVersionInfoSizeExA(DWORD dwFlags, LPCSTR lpwstrFilename, LPDWORD lpdwHandle)
{
    if (!LoadRealVersion()) return 0;
    if (!real_GetFileVersionInfoSizeExA) ResolveAll();
    if (real_GetFileVersionInfoSizeExA) return real_GetFileVersionInfoSizeExA(dwFlags, lpwstrFilename, lpdwHandle);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return 0;
}

extern "C" DWORD WINAPI My_GetFileVersionInfoSizeExW(DWORD dwFlags, LPCWSTR lpwstrFilename, LPDWORD lpdwHandle)
{
    if (!LoadRealVersion()) return 0;
    if (!real_GetFileVersionInfoSizeExW) ResolveAll();
    if (real_GetFileVersionInfoSizeExW) return real_GetFileVersionInfoSizeExW(dwFlags, lpwstrFilename, lpdwHandle);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return 0;
}

extern "C" BOOL WINAPI My_VerFindFileA(UINT uFlags, LPCSTR szFileName, LPCSTR szWinDir, LPCSTR szAppDir, LPSTR szCurDir, PUINT lpuCurDirLen, LPSTR szDestDir, PUINT lpuDestDirLen)
{
    if (!LoadRealVersion()) return FALSE;
    if (!real_VerFindFileA) ResolveAll();
    if (real_VerFindFileA) return real_VerFindFileA(uFlags, szFileName, szWinDir, szAppDir, szCurDir, lpuCurDirLen, szDestDir, lpuDestDirLen);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return FALSE;
}

extern "C" BOOL WINAPI My_VerFindFileW(UINT uFlags, LPCWSTR szFileName, LPCWSTR szWinDir, LPCWSTR szAppDir, LPWSTR szCurDir, PUINT lpuCurDirLen, LPWSTR szDestDir, PUINT lpuDestDirLen)
{
    if (!LoadRealVersion()) return FALSE;
    if (!real_VerFindFileW) ResolveAll();
    if (real_VerFindFileW) return real_VerFindFileW(uFlags, szFileName, szWinDir, szAppDir, szCurDir, lpuCurDirLen, szDestDir, lpuDestDirLen);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return FALSE;
}

extern "C" BOOL WINAPI My_VerInstallFileA(UINT uFlags, LPCSTR szSrcFileName, LPCSTR szDestFileName, LPCSTR szSrcDir, LPCSTR szDestDir, LPCSTR szCurDir, LPCSTR szTmpFile, PUINT lpuTmpFileLen)
{
    if (!LoadRealVersion()) return FALSE;
    if (!real_VerInstallFileA) ResolveAll();
    if (real_VerInstallFileA) return real_VerInstallFileA(uFlags, szSrcFileName, szDestFileName, szSrcDir, szDestDir, szCurDir, szTmpFile, lpuTmpFileLen);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return FALSE;
}

extern "C" BOOL WINAPI My_VerInstallFileW(UINT uFlags, LPCWSTR szSrcFileName, LPCWSTR szDestFileName, LPCWSTR szSrcDir, LPCWSTR szDestDir, LPCWSTR szCurDir, LPCWSTR szTmpFile, PUINT lpuTmpFileLen)
{
    if (!LoadRealVersion()) return FALSE;
    if (!real_VerInstallFileW) ResolveAll();
    if (real_VerInstallFileW) return real_VerInstallFileW(uFlags, szSrcFileName, szDestFileName, szSrcDir, szDestDir, szCurDir, szTmpFile, lpuTmpFileLen);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return FALSE;
}

extern "C" UINT WINAPI My_VerLanguageNameA(DWORD wLang, LPSTR szLang, UINT nSize)
{
    if (!LoadRealVersion()) return 0;
    if (!real_VerLanguageNameA) ResolveAll();
    if (real_VerLanguageNameA) return real_VerLanguageNameA(wLang, szLang, nSize);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return 0;
}

extern "C" UINT WINAPI My_VerLanguageNameW(DWORD wLang, LPWSTR szLang, UINT nSize)
{
    if (!LoadRealVersion()) return 0;
    if (!real_VerLanguageNameW) ResolveAll();
    if (real_VerLanguageNameW) return real_VerLanguageNameW(wLang, szLang, nSize);
    SetLastError(ERROR_PROC_NOT_FOUND);
    return 0;
}
