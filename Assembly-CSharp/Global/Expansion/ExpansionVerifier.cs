using System;
using UnityEngine;
using Object = System.Object;

public class ExpansionVerifier : MonoBehaviour
{
	private static void Log(Object message)
	{
	}

	private static void LogError(Object message)
	{
	}

	private static void LogWarning(Object message)
	{
	}

	private void Start()
	{
		this.StartGame();
	}

	private void StartGame()
	{
		ExpansionVerifier.printLog("ExpantionVerifier : StartGame()");
		AssetManagerForObb.Initialize();
		ExpansionVerifier.printLog("ExpantionVerifier : Expansion verification done. Move to Bundle scene");
		if (this.isShowInfoUI)
		{
			global::Debug.Log("isShowInfoUI == true. Go to Bundle scene");
			Application.LoadLevel("Bundle");
		}
		else
		{
			global::Debug.Log("isShowInfoUI == false. Go to SplashScreen scene");
			Application.LoadLevel("SplashScreen");
		}
		ExpansionVerifier.printLog("ExpantionVerifier : Expansion verification done.");
	}

	public static void printLog(String mes)
	{
		global::Debug.Log(mes);
	}

	private const String OBB2_TMP_FILE_NAME = "OBB2.tmp";

	private const Int32 BUFFER_SIZE = 4194304;

	private Int64 currentExtractedFileSize;

	private Int64 lastFrameExtractedFileSize;

	private Int64 originalFileLength;

	private Single displayEstimateTime;

	private Int32 lastFrameProgress = -1;

	private Boolean isShowInfoUI;

	private Boolean isFirstTimeEstimate = true;

	private ExpansionVerifier.State currentState;

	private ExpansionVerifier.ErrorCode currentErrorCode;

	public AndroidExpansionUI UI;

	public enum State
	{
		None,
		ValidateDownloadOBB,
		ValidateDecompressOBB,
		DetermineAvailableSpace,
		DecompressOBB,
		ValidateDecompressedOBBSize,
		ReplaceCompressedOBBWithUncompressedOBB,
		DecompressSuccess,
		DecompressFailure
	}

	public enum ErrorCode
	{
		None,
		NotEnoughStorage,
		FileTypeNotSupport,
		DecompressionFailure,
		PatchOBBNotFound,
		MoveTempFileFailure,
		PatchOBBFileSizeNotCorrect,
		ValidateFileSizeFailure
	}
}
