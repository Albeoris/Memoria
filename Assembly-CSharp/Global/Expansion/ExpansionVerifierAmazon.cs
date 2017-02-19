using System;
using UnityEngine;
using Object = System.Object;

public class ExpansionVerifierAmazon : MonoBehaviour
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
		ExpansionVerifierAmazon.printLog("ExpantionVerifier : StartGame()");
		AssetManager.UseBundles = true;
		ExpansionVerifierAmazon.printLog("ExpantionVerifier : Expansion verification done. Move to Bundle scene");
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
		ExpansionVerifierAmazon.printLog("ExpantionVerifier : Expansion verification done.");
	}

	public static void printLog(String mes)
	{
		global::Debug.Log("pl: " + mes);
	}

	private Double currentExtractedSumPercent;

	private Double lastCurrentExtractedSumPercent;

	private Double allSumPercent;

	private Double lastExtractedSumPercent;

	private Int64 numOfExtractedFiles;

	private Single timeSinceLastCalculation;

	private Single displayEstimateTime;

	private Int32 lastFrameProgress = -1;

	private Boolean isShowInfoUI;

	private Boolean isFirstTimeEstimate = true;

	private ExpansionVerifier.State currentState;

	private ExpansionVerifier.ErrorCode currentErrorCode;

	private String _baseUrl;

	private Double[] bundleSizeMB = new Double[]
	{
		45.8,
		26.3,
		37.4,
		34.9,
		31.3,
		34.5,
		31.4,
		32.6,
		31.3,
		56.6,
		16.6,
		74.1,
		82.5,
		163.3,
		176.9,
		124.9,
		9.1
	};

	private Int64 minimumSpaceForExtractingBundles = (Int64)(-1073741824);

	private ExpansionVerifierAmazon.OnDecompressBundlesFinish onDecompressBundlesFinishDelegate;

	public AndroidExpansionUI UI;

	private enum DecompressBundleResult
	{
		Success,
		Failure
	}

	private delegate void OnDecompressBundlesFinish(ExpansionVerifierAmazon.DecompressBundleResult res);
}
