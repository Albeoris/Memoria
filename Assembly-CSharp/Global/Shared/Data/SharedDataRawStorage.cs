using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;

public class SharedDataRawStorage : SharedDataBytesStorage
{
	public override void LoadSlotPreview(Int32 slotID, ISharedDataStorage.OnLoadSlotFinish onFinishDelegate)
	{
		if (this.RawData == null)
		{
			ISharedDataLog.LogWarning("SharedDataRawStorage LoadSlotPreview is being called but RawData is null!");
			onFinishDelegate(slotID, null);
			return;
		}
		base.CreateDataSchema();
		List<SharedDataPreviewSlot> list = new List<SharedDataPreviewSlot>();
		this.cStream = new MemoryStream(this.RawData);
		this.cReader = new BinaryReader(this.cStream);
		this.cOnFinishDelegate = onFinishDelegate;
		SlotPreviewReadWriterUtil.Instance.ReadSlotPreview(this.metaData, slotID, this.cStream, this.cReader, this.Encryption, new ISharedDataStorage.OnLoadSlotFinish(this.LoadSlotPreviewDelegate));
	}

	private void LoadSlotPreviewDelegate(Int32 outSlotID, List<SharedDataPreviewSlot> slotList)
	{
		this.cReader.Close();
		this.cStream.Close();
		this.cOnFinishDelegate(outSlotID, slotList);
	}

	public override void GetLatestSlotAndSave(ISharedDataStorage.OnGetLatestSlotAndSaveFinish onFinishDelegate)
	{
		if (this.RawData == null)
		{
			ISharedDataLog.LogWarning("SharedDataRawStorage GetLatestSlotAndSave is being called but RawData is null!");
			onFinishDelegate(-1, -1);
			return;
		}
		base.CreateDataSchema();
		using (MemoryStream memoryStream = new MemoryStream(this.RawData))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				this.metaData.Read(memoryStream, binaryReader, this.Encryption);
			}
		}
		onFinishDelegate(this.metaData.LatestSlot, this.metaData.LatestSave);
	}

	public override void GetLatestTimestamp(ISharedDataStorage.OnGetLatestSaveTimestamp onFinishDelegate)
	{
		if (this.RawData == null)
		{
			ISharedDataLog.LogWarning("SharedDataRawStorage GetLatestTimestamp is being called but RawData is null!");
			onFinishDelegate(false, 0.0);
			return;
		}
		base.CreateDataSchema();
		using (MemoryStream memoryStream = new MemoryStream(this.RawData))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				this.metaData.Read(memoryStream, binaryReader, this.Encryption);
			}
		}
		onFinishDelegate(true, this.metaData.LatestTimestamp);
	}

	public override void GetDataSize(ISharedDataStorage.OnGetDataSizeFinish onFinishDelegate)
	{
		if (this.RawData == null)
		{
			onFinishDelegate(false, 0);
			return;
		}
		Int32 rawSize = this.RawData.Length;
		Int32 expectedSize = 2937152;
		if (rawSize == expectedSize)
			onFinishDelegate(true, expectedSize);
		else
			onFinishDelegate(false, 0);
	}

	public override void SaveSlotPreview(Int32 slotID, Int32 saveID, ISharedDataStorage.OnSaveSlotFinish onFinishDelegate)
	{
		ISharedDataLog.LogWarning("No need implementation");
	}

	public override void Load(Int32 slotID, Int32 saveID, ISharedDataStorage.OnLoadFinish onFinishDelegate)
	{
		ISharedDataLog.LogWarning("No need implementation");
	}

	public override void Save(Int32 slotID, Int32 saveID, JSONClass rootNode, ISharedDataStorage.OnSaveFinish onFinishDelegate)
	{
		ISharedDataLog.LogWarning("No need implementation");
	}

	public override void Autosave(JSONClass rootNode, ISharedDataStorage.OnAutosaveFinish onFinishDelegate)
	{
		ISharedDataLog.LogWarning("No need implementation");
	}

	public override void Autoload(ISharedDataStorage.OnAutoloadFinish onFinishDelegate)
	{
		ISharedDataLog.LogWarning("No need implementation");
	}

	public override void HasAutoload(ISharedDataStorage.OnHasAutoloadFinish onFinishDelegate)
	{
		ISharedDataLog.LogWarning("No need implementation");
	}

	public override void LoadRawData(ISharedDataStorage.OnLoadRawDataFinish onFinishDelegate)
	{
		ISharedDataLog.LogWarning("No need implementation");
	}

	public override void SaveRawData(Byte[] rawData, ISharedDataStorage.OnSaveRawDataFinish onFinishDelegate)
	{
		ISharedDataLog.LogWarning("No need implementation");
	}

	public override void ClearAllData()
	{
		ISharedDataLog.LogWarning("No need implementation");
	}

	public Byte[] RawData;

	private MemoryStream cStream;

	private BinaryReader cReader;

	private ISharedDataStorage.OnLoadSlotFinish cOnFinishDelegate;
}
