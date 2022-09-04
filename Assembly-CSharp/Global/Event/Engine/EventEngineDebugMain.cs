using System;
using Memoria;
using UnityEngine;

public class EventEngineDebugMain : MonoBehaviour
{
	private void Start()
	{
		this.ee = PersistenSingleton<EventEngine>.Instance;
		this.ee.InitEvents();
		this.eBin = this.ee.eBin;
		this.eTb = this.ee.eTb;
		this.ee.StartEventsByEBFileName("CommonAsset/EventEngine/EventBinary/US/EVT_ALEX1_TS_CARGO_0.eb");
	}

	private void Update()
	{
		for (Int32 updateCount = 0; updateCount < FPSManager.MainLoopUpdateCount; updateCount++)
			this.ee.ProcessEvents();
	}

	private void OnDestroy()
	{
	}

	private EventEngine ee;

	private EBin eBin;

	private ETb eTb;
}
