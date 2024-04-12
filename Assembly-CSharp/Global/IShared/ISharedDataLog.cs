using System;

public class ISharedDataLog
{
	public static void Log(Object message)
	{
	}

	public static void LogWarning(Object message)
	{
		Memoria.Prime.Log.Message($"[{nameof(ISharedDataLog)}] {message}");
	}

	public static void LogError(Object message)
	{
		Memoria.Prime.Log.Message($"[{nameof(ISharedDataLog)}] {message}");
	}
}
