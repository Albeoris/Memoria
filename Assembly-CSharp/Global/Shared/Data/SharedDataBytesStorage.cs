using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Memoria.Assets;
using Memoria.Prime;
using SimpleJSON;
using UnityEngine;
using Object = System.Object;

public class SharedDataBytesStorage : ISharedDataStorage
{
	static SharedDataBytesStorage()
	{
		SetPCPath();
	}

	private void Awake()
	{
		if (Application.isEditor)
		{
			this.SetEditorPath();
		}
		else
		{
			SetPCPath();
		}
		ISharedDataLog.Log("MetaData.FilePath: " + MetaData.FilePath);
	}

	private static void SetPCPath()
	{
		if (!String.IsNullOrEmpty(MetaData.FilePath))
			return;

		String fileName;
		String str2 = "/Steam/EncryptedSavedData";

		if (FF9StateSystem.PCEStorePlatform)
		{
			String text2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			text2 = text2.Replace('\\', '/');
			MetaData.DirPath = text2 + "/My Games/FINAL FANTASY IX/EncryptedSaveData";
		}
		else
		{
			MetaData.DirPath = AssetManagerUtil.GetPersistentDataPath() + str2;
		}

		Directory.CreateDirectory(MetaData.DirPath);

		String[] saveDataFiles = Directory.GetFiles(MetaData.DirPath, "SavedData_??.dat");
		if (saveDataFiles.Length == 0)
		{
			if (Localization.CurrentLanguage == "Japanese")
				fileName = "SavedData_jp.dat";
			else
				fileName = "SavedData_ww.dat";

			Log.Message($"There is no SavedData_??.dat files. Choose by a language: {fileName}");
		}
		else if (saveDataFiles.Length == 1)
		{
			fileName = Path.GetFileName(saveDataFiles[0]);
		}
		else
		{
			String lastWritePath = null;
			DateTime lastWriteTime = DateTime.MinValue;
			foreach (String file in saveDataFiles)
			{
				DateTime time = File.GetLastWriteTimeUtc(file);

				if (time > lastWriteTime)
				{
					lastWritePath = file;
					lastWriteTime = time;
				}
			}

			fileName = Path.GetFileName(lastWritePath);
			Log.Message($"There is many SavedData_??.dat files. Choose by a last write time: {fileName}");
		}

		MetaData.FilePath = Application.persistentDataPath + str2 + "/" + fileName;
	}

	private void SetOtherPlatformPath()
	{
		MetaData.FilePath = AssetManagerUtil.GetPersistentDataPath() + "/EncryptedSavedData/SavedData.dat";
		MetaData.DirPath = AssetManagerUtil.GetPersistentDataPath() + "/EncryptedSavedData";
	}

	private void SetEditorPath()
	{
		MetaData.FilePath = Application.persistentDataPath + "/SavedData/SavedData.dat";
		MetaData.DirPath = Application.persistentDataPath + "/SavedData";
	}

	private JsonParser LazyCreateJsonParser()
	{
		if (this.jsonParserInstance == (UnityEngine.Object)null)
		{
			GameObject gameObject = new GameObject("JsonParser");
			this.jsonParserInstance = gameObject.AddComponent<JsonParser>();
			this.jsonParserInstance.transform.parent = base.transform;
		}
		return this.jsonParserInstance;
	}

	private JSONClass ParseDataListToJsonTree(List<JSONData> dataList, JSONClass rootSchemaNode)
	{
		Stack<SharedDataBytesStorage.JSONNodeWithIndex> stack = new Stack<SharedDataBytesStorage.JSONNodeWithIndex>();
		stack.Push(new SharedDataBytesStorage.JSONNodeWithIndex(rootSchemaNode));
		ISharedDataLog.Log("dataList.Count: " + dataList.Count);
		Int32 num = 0;
		while (stack.Count > 0)
		{
			SharedDataBytesStorage.JSONNodeWithIndex jsonnodeWithIndex = stack.Peek();
			if (jsonnodeWithIndex.Node is JSONClass)
			{
				List<String> list = ((JSONClass)jsonnodeWithIndex.Node).Dict.Keys.ToList<String>();
				list.Sort();
				if (jsonnodeWithIndex.Index < jsonnodeWithIndex.Node.Count)
				{
					String aKey = list[jsonnodeWithIndex.Index];
					JSONNode node = jsonnodeWithIndex.Node[aKey];
					stack.Push(new SharedDataBytesStorage.JSONNodeWithIndex(node));
					jsonnodeWithIndex.Index++;
				}
				else
				{
					stack.Pop();
				}
			}
			else if (jsonnodeWithIndex.Node is JSONArray)
			{
				if (jsonnodeWithIndex.Index < jsonnodeWithIndex.Node.Count)
				{
					JSONNode node2 = jsonnodeWithIndex.Node[jsonnodeWithIndex.Index];
					stack.Push(new SharedDataBytesStorage.JSONNodeWithIndex(node2));
					jsonnodeWithIndex.Index++;
				}
				else
				{
					stack.Pop();
				}
			}
			else if (jsonnodeWithIndex.Node is JSONData)
			{
				JSONData d = dataList[num];
				ISharedDataLog.Log("data: " + d);
				jsonnodeWithIndex.Node.Value = d;
				num++;
				stack.Pop();
			}
		}
		return rootSchemaNode;
	}

	private List<JSONData> ParseJsonTreeToDataList(JSONClass rootNode)
	{
		this.tmpParseCount = 0;
		List<JSONData> list = new List<JSONData>();
		Stack<SharedDataBytesStorage.JSONNodeWithIndex> stack = new Stack<SharedDataBytesStorage.JSONNodeWithIndex>();
		stack.Push(new SharedDataBytesStorage.JSONNodeWithIndex(rootNode));
		while (stack.Count > 0)
		{
			SharedDataBytesStorage.JSONNodeWithIndex jsonnodeWithIndex = stack.Peek();
			if (jsonnodeWithIndex.Node is JSONClass)
			{
				List<String> list2 = ((JSONClass)jsonnodeWithIndex.Node).Dict.Keys.ToList<String>();
				list2.Sort();
				if (jsonnodeWithIndex.Index < jsonnodeWithIndex.Node.Count)
				{
					String aKey = list2[jsonnodeWithIndex.Index];
					JSONNode jsonnode = jsonnodeWithIndex.Node[aKey];
					if (jsonnode == null)
					{
						ISharedDataLog.LogError("child is null");
					}
					else
					{
						stack.Push(new SharedDataBytesStorage.JSONNodeWithIndex(jsonnode));
					}
					jsonnodeWithIndex.Index++;
				}
				else
				{
					stack.Pop();
				}
			}
			else if (jsonnodeWithIndex.Node is JSONArray)
			{
				if (jsonnodeWithIndex.Index < jsonnodeWithIndex.Node.Count)
				{
					JSONNode jsonnode2 = jsonnodeWithIndex.Node[jsonnodeWithIndex.Index];
					if (jsonnode2 == null)
					{
						ISharedDataLog.LogError("child is null");
					}
					else
					{
						stack.Push(new SharedDataBytesStorage.JSONNodeWithIndex(jsonnode2));
					}
					jsonnodeWithIndex.Index++;
				}
				else
				{
					stack.Pop();
				}
			}
			else
			{
				if (!(jsonnodeWithIndex.Node is JSONData))
				{
					ISharedDataLog.LogError("Parsing failed!");
					return list;
				}
				list.Add((JSONData)jsonnodeWithIndex.Node);
				stack.Pop();
			}
		}
		return list;
	}

	private Boolean ValidateDataListWithSchemaDataList(List<JSONData> data, List<JSONData> schema)
	{
		if (data == null)
		{
			ISharedDataLog.LogError("data == null");
			return false;
		}
		if (schema == null)
		{
			ISharedDataLog.LogError("schema == null");
			return false;
		}
		if (data.Count != schema.Count)
		{
			ISharedDataLog.LogError(String.Concat(new Object[]
			{
				"data.Count(",
				data.Count,
				") != schema.Count(",
				schema.Count,
				")"
			}));
			return false;
		}
		return true;
	}

	private Int32 EstimateDataSize(List<JSONData> dataTypeList)
	{
		Int32 num = 0;
		foreach (JSONData d in dataTypeList)
		{
			if (String.Compare(d, SharedDataBytesStorage.TypeByte) == 0)
			{
				num++;
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeSByte) == 0)
			{
				num++;
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeInt16) == 0)
			{
				num += 2;
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeUInt16) == 0)
			{
				num += 2;
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeInt32) == 0)
			{
				num += 4;
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeUInt32) == 0)
			{
				num += 4;
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeInt64) == 0)
			{
				num += 8;
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeUInt64) == 0)
			{
				num += 8;
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeSingle) == 0)
			{
				num += 4;
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeDouble) == 0)
			{
				num += 8;
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeBoolean) == 0)
			{
				num++;
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeString128) == 0)
			{
				num += 128;
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeString4K) == 0)
			{
				num += 4096;
			}
			else
			{
				ISharedDataLog.LogError("Data type not found, AT type estimation, with type: " + d);
			}
		}
		if (num > 18432)
		{
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
			ISharedDataLog.LogError(String.Concat(new Object[]
			{
				"Estimated saved data size(",
				num,
				") larger than Save reserved size(",
				18432,
				")!"
			}));
		}
		return num;
	}

	private void ReadDataFromStream(List<JSONNode> dataList, List<JSONData> dataTypeList, Stream stream, BinaryReader reader)
	{
		for (Int32 i = 0; i < dataTypeList.Count; i++)
		{
			JSONData d = dataTypeList[i];
			if (String.Compare(d, SharedDataBytesStorage.TypeByte) == 0)
			{
				dataList.Add(reader.ReadByte().ToString());
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeSByte) == 0)
			{
				dataList.Add(reader.ReadSByte().ToString());
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeInt16) == 0)
			{
				dataList.Add(reader.ReadInt16().ToString());
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeUInt16) == 0)
			{
				dataList.Add(reader.ReadUInt16().ToString());
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeInt32) == 0)
			{
				dataList.Add(reader.ReadInt32().ToString());
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeUInt32) == 0)
			{
				dataList.Add(reader.ReadUInt32().ToString());
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeInt64) == 0)
			{
				dataList.Add(reader.ReadInt64().ToString());
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeUInt64) == 0)
			{
				dataList.Add(reader.ReadUInt64().ToString());
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeSingle) == 0)
			{
				dataList.Add(reader.ReadSingle().ToString());
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeDouble) == 0)
			{
				dataList.Add(reader.ReadDouble().ToString());
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeBoolean) == 0)
			{
				dataList.Add(reader.ReadBoolean().ToString());
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeString128) == 0)
			{
				String text = Encoding.UTF8.GetString(reader.ReadBytes(128), 0, 128);
				text = text.Trim(new Char[1]);
				dataList.Add(text);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeString4K) == 0)
			{
				String text2 = Encoding.UTF8.GetString(reader.ReadBytes(4096), 0, 4096);
				text2 = text2.Trim(new Char[1]);
				dataList.Add(text2);
			}
			else
			{
				ISharedDataLog.LogError("Data type not found, AT type parser, with type: " + d);
			}
		}
	}

	private void WriteDataToStream(List<JSONData> dataList, List<JSONData> dataTypeList, Stream stream, BinaryWriter writer)
	{
		writer.Write('S');
		writer.Write('A');
		writer.Write('V');
		writer.Write('E');
		for (Int32 i = 0; i < dataTypeList.Count; i++)
		{
			JSONData d = dataTypeList[i];
			JSONData jsondata = dataList[i];
			if (String.Compare(d, SharedDataBytesStorage.TypeByte) == 0)
			{
				writer.Write((Byte)jsondata.AsInt);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeSByte) == 0)
			{
				writer.Write((SByte)jsondata.AsInt);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeInt16) == 0)
			{
				writer.Write((Int16)jsondata.AsInt);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeUInt16) == 0)
			{
				writer.Write((UInt16)jsondata.AsInt);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeInt32) == 0)
			{
				writer.Write(jsondata.AsInt);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeUInt32) == 0)
			{
				writer.Write(jsondata.AsUInt);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeInt64) == 0)
			{
				writer.Write(jsondata.AsLong);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeUInt64) == 0)
			{
				writer.Write(jsondata.AsULong);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeSingle) == 0)
			{
				writer.Write(jsondata.AsFloat);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeDouble) == 0)
			{
				writer.Write(jsondata.AsDouble);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeBoolean) == 0)
			{
				writer.Write(jsondata.AsBool);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeString128) == 0)
			{
				Byte[] bytes = Encoding.UTF8.GetBytes(jsondata);
				Int32 byteCount = Encoding.UTF8.GetByteCount(jsondata);
				Byte[] array = new Byte[128];
				Buffer.BlockCopy(bytes, 0, array, 0, (Int32)((byteCount <= 128) ? byteCount : 128));
				writer.Write(array, 0, 128);
			}
			else if (String.Compare(d, SharedDataBytesStorage.TypeString4K) == 0)
			{
				Byte[] bytes2 = Encoding.UTF8.GetBytes(jsondata);
				Int32 byteCount2 = Encoding.UTF8.GetByteCount(jsondata);
				Byte[] array2 = new Byte[4096];
				Buffer.BlockCopy(bytes2, 0, array2, 0, (Int32)((byteCount2 <= 4096) ? byteCount2 : 4096));
				writer.Write(array2, 0, 4096);
			}
			else
			{
				ISharedDataLog.LogError("Data type not found, AT type parser, with type: " + d);
			}
		}
	}

	protected void CreateFileIfDoesNotExist(SharedDataBytesStorage.MetaData metaData)
	{
		if (!Directory.Exists(MetaData.DirPath))
		{
			Directory.CreateDirectory(MetaData.DirPath);
		}
		if (!File.Exists(MetaData.FilePath))
		{
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Create, FileAccess.Write))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					metaData.WriteAllFields(fileStream, binaryWriter, this.Encryption);
					Int32 num = 320 - (Int32)fileStream.Position;
					Byte[] buffer = new Byte[num];
					ISharedDataLog.Log("Write to: " + num);
					binaryWriter.Write(buffer, 0, num);
					Byte[] array = new Byte[965];
					using (MemoryStream memoryStream = new MemoryStream(array))
					{
						using (BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream))
						{
							binaryWriter2.Write('N');
							binaryWriter2.Write('O');
							binaryWriter2.Write('N');
							binaryWriter2.Write('E');
						}
					}
					Byte[] array2 = this.Encryption.Encrypt(array);
					Int32 num2 = 1024 - (Int32)array2.Length;
					Byte[] array3 = new Byte[num2];
					for (Int32 i = 0; i < 150; i++)
					{
						binaryWriter.Write(array2, 0, (Int32)array2.Length);
						binaryWriter.Write(array3, 0, (Int32)array3.Length);
					}
					this.CreateDataSchema();
					Int32 dataSize = metaData.DataSize;
					Byte[] array4 = new Byte[dataSize + 4];
					using (MemoryStream memoryStream2 = new MemoryStream(array4))
					{
						using (BinaryWriter binaryWriter3 = new BinaryWriter(memoryStream2))
						{
							binaryWriter3.Write('N');
							binaryWriter3.Write('O');
							binaryWriter3.Write('N');
							binaryWriter3.Write('E');
						}
					}
					Byte[] array5 = this.Encryption.Encrypt(array4);
					Int32 num3 = 18432 - (Int32)array5.Length;
					Byte[] array6 = new Byte[num3];
					Int32 num4 = 18432;
					buffer = new Byte[num4];
					binaryWriter.Write(array5, 0, (Int32)array5.Length);
					binaryWriter.Write(array6, 0, (Int32)array6.Length);
					for (Int32 j = 0; j < 150; j++)
					{
						Int32 num5 = (Int32)fileStream.Position;
						binaryWriter.Write(array5, 0, (Int32)array5.Length);
						binaryWriter.Write(array6, 0, (Int32)array6.Length);
					}
				}
			}
		}
	}

	public override void LoadSlotPreview(Int32 slotID, ISharedDataStorage.OnLoadSlotFinish onFinishDelegate)
	{
		this.CreateDataSchema();
		this.CreateFileIfDoesNotExist(this.metaData);
		try
		{
			this.cStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read);
			this.cReader = new BinaryReader(this.cStream);
			this.cOnFinishDelegate = onFinishDelegate;
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			if (this.cStream != null)
			{
				this.cStream.Close();
			}
			if (this.cReader != null)
			{
				this.cReader.Close();
			}
			this.cStream = (FileStream)null;
			this.cReader = (BinaryReader)null;
			onFinishDelegate(-1, null);
			return;
		}
		SlotPreviewReadWriterUtil.Instance.ReadSlotPreview(this.metaData, slotID, this.cStream, this.cReader, this.Encryption, new ISharedDataStorage.OnLoadSlotFinish(this.LoadSlotPreviewDelegate));
	}

	private void LoadSlotPreviewDelegate(Int32 outSlotID, List<SharedDataPreviewSlot> slotList)
	{
		this.cReader.Close();
		this.cStream.Close();
		this.cStream = (FileStream)null;
		this.cReader = (BinaryReader)null;
		this.cOnFinishDelegate(outSlotID, slotList);
	}

	public override void SaveSlotPreview(Int32 slotID, Int32 saveID, ISharedDataStorage.OnSaveSlotFinish onFinishDelegate)
	{
		SharedDataPreviewSlot sharedDataPreviewSlot = new SharedDataPreviewSlot();
		sharedDataPreviewSlot.CharacterInfoList = new List<SharedDataPreviewCharacterInfo>();
		sharedDataPreviewSlot.Gil = FF9StateSystem.Common.FF9.party.gil;
		sharedDataPreviewSlot.PlayDuration = (UInt64)FF9StateSystem.Settings.time;
		sharedDataPreviewSlot.Location = FF9StateSystem.Common.FF9.mapNameStr == null ? "Undefinded" : FF9StateSystem.Common.FF9.mapNameStr;
		sharedDataPreviewSlot.win_type = FF9StateSystem.Settings.cfg.win_type;
		for (Int32 i = 0; i < 4; i++)
		{
			SharedDataPreviewCharacterInfo sharedDataPreviewCharacterInfo = null;
			PLAYER player = FF9StateSystem.Common.FF9.party.member[i];
			if (player != null)
			{
				sharedDataPreviewCharacterInfo = new SharedDataPreviewCharacterInfo();
				sharedDataPreviewCharacterInfo.SerialID = (Int32)player.info.serial_no;
				sharedDataPreviewCharacterInfo.Name = player.Name;
				sharedDataPreviewCharacterInfo.Level = player.level;
			}
			sharedDataPreviewSlot.CharacterInfoList.Add(sharedDataPreviewCharacterInfo);
		}
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			List<SharedDataPreviewSlot> list = new List<SharedDataPreviewSlot>();
			Int32 cacheOutSlotID = -1;
			Int32 cacheOutSaveID = -1;
			SharedDataPreviewSlot cacheData = null;
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.ReadWrite))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
					{
						SlotPreviewReadWriterUtil.Instance.WriteSlotPreview(sharedDataPreviewSlot, this.metaData, saveID, slotID, fileStream, binaryReader, binaryWriter, this.Encryption, delegate (Int32 outSlotID, Int32 outSaveID, SharedDataPreviewSlot data)
						{
							cacheOutSlotID = outSlotID;
							cacheOutSaveID = outSaveID;
							cacheData = data;
						});
					}
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate(-1, -1, null);
			return;
		}
		onFinishDelegate(slotID, saveID, sharedDataPreviewSlot);
	}

	protected void CreateDataSchema()
	{
		this.jsonParser = this.LazyCreateJsonParser();
		JSONClass rootNode = new JSONClass();
		this.rootSchemaNode = new JSONClass();
		this.jsonParser.ParseFromFF9StateSystem(rootNode, this.rootSchemaNode);
		this.dataTypeList = this.ParseJsonTreeToDataList(this.rootSchemaNode);
		this.dataNodeList = new List<JSONNode>();
		Int32 dataSize = this.EstimateDataSize(this.dataTypeList);
		this.metaData.DataSize = dataSize;
	}

	protected JSONClass Load(Boolean isAutoload, Int32 slotID, Int32 saveID)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			Boolean flag = false;
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
					fileStream.Seek((Int64)(320 - (Int32)fileStream.Position), SeekOrigin.Current);
					FF9StateSystem.latestSlot = this.metaData.LatestSlot;
					FF9StateSystem.latestSave = this.metaData.LatestSave;
					FF9StateSystem.LatestTimestamp = this.metaData.LatestTimestamp;
					Int32 num = 153600;
					ISharedDataLog.Log("Seek to: " + num);
					fileStream.Seek((Int64)num, SeekOrigin.Current);
					if (!isAutoload)
					{
						Int32 num2 = 18432 + slotID * 15 * 18432 + saveID * 18432;
						ISharedDataLog.Log("Seek to: " + num2);
						fileStream.Seek((Int64)num2, SeekOrigin.Current);
					}
					Int32 cipherSize = this.Encryption.GetCipherSize(this.metaData.DataSize + 4);
					Byte[] array = new Byte[cipherSize];
					binaryReader.Read(array, 0, (Int32)array.Length);
					Byte[] array2 = null;
					try
					{
						array2 = this.Encryption.Decrypt(array);
					}
					catch (Exception message)
					{
						ISharedDataLog.LogError(message);
						ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
						return (JSONClass)null;
					}
					if ((Int32)array2.Length != this.metaData.DataSize + 4)
					{
						ISharedDataLog.LogError("plainText.size and metaData.DataSize is NOT equal, load the old data?");
						ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
						return (JSONClass)null;
					}
					try
					{
						using (MemoryStream memoryStream = new MemoryStream(array2))
						{
							using (BinaryReader binaryReader2 = new BinaryReader(memoryStream))
							{
								Char[] array3 = binaryReader2.ReadChars(4);
								if (array3[0] == 'S' && array3[1] == 'A' && array3[2] == 'V' && array3[3] == 'E')
								{
									flag = true;
									this.ReadDataFromStream(this.dataNodeList, this.dataTypeList, memoryStream, binaryReader2);
								}
								else
								{
									if (array3[0] != 'N' || array3[1] != 'O' || array3[2] != 'N' || array3[3] != 'E')
									{
										ISharedDataLog.LogError("Invalid data");
										ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
										return (JSONClass)null;
									}
									flag = false;
								}
							}
						}
					}
					catch (Exception message2)
					{
						ISharedDataLog.LogError(message2);
						ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
						return (JSONClass)null;
					}
				}
			}
			if (!flag)
			{
				return (JSONClass)null;
			}
			List<JSONData> list = new List<JSONData>();
			foreach (JSONNode jsonnode in this.dataNodeList)
			{
				list.Add((JSONData)jsonnode);
			}
			if (!this.ValidateDataListWithSchemaDataList(list, this.dataTypeList))
			{
				ISharedDataLog.LogError("Verification failed!");
				return (JSONClass)null;
			}
			JSONClass tree = this.ParseDataListToJsonTree(list, this.rootSchemaNode);
            String memoriaSavePath = MetaData.GetMemoriaExtraSaveFilePath(isAutoload, slotID, saveID);
            if (!String.IsNullOrEmpty(memoriaSavePath) && File.Exists(memoriaSavePath))
            {
                JSONNode memoriaNode = JSONNode.LoadFromFile(memoriaSavePath);
                if (memoriaNode != null)
                {
                    memoriaNode.Add("MetaDataSlotID", slotID.ToString());
                    memoriaNode.Add("MetaDataSaveID", saveID.ToString());
                    memoriaNode.Add("MetaDataFileName", Path.GetFileName(memoriaSavePath));
                    tree.Add("MemoriaExtraData", memoriaNode);
                }
            }
			return tree;
		}
		catch (Exception message3)
		{
			ISharedDataLog.LogError(message3);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
		}
		return (JSONClass)null;
	}

	protected Boolean Save(Boolean isAutosave, Int32 slotID, Int32 saveID, JSONClass rootNode)
	{
		Boolean result;
		try
		{
			JSONClass memoriaNode = rootNode.Remove("MemoriaExtraData")?.AsObject;
			this.CreateDataSchema();
			List<JSONData> list = this.ParseJsonTreeToDataList(rootNode);
			if (!this.ValidateDataListWithSchemaDataList(list, this.dataTypeList))
			{
				ISharedDataLog.LogError("Verification failed!");
				result = false;
			}
			else
			{
				ISharedDataLog.Log("Verification success, start to pack data");
				Int32 dataSize = this.EstimateDataSize(this.dataTypeList);
				Byte[] bytes = Encoding.UTF8.GetBytes(this.rootSchemaNode.ToString());
				Int32 num = (Int32)bytes.Length;
				this.metaData.DataSize = dataSize;
				FF9StateSystem.latestSlot = slotID;
				FF9StateSystem.latestSave = saveID;
				this.metaData.LatestSlot = slotID;
				this.metaData.LatestSave = saveID;
				Double totalSeconds = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
				FF9StateSystem.LatestTimestamp = totalSeconds;
				this.metaData.LatestTimestamp = totalSeconds;
				ISharedDataLog.Log("DataSize: " + this.metaData.DataSize);
				ISharedDataLog.Log("TotalDataSize should be: " + 150 * this.metaData.DataSize);
				this.CreateFileIfDoesNotExist(this.metaData);
				using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.ReadWrite))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream))
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
						{
							this.metaData.WriteLatestSlotAndSaveAndLatestTimestamp(fileStream, binaryWriter, binaryReader, this.Encryption);
							fileStream.Seek((Int64)(320 - (Int32)fileStream.Position), SeekOrigin.Current);
							Int32 num2 = 153600;
							ISharedDataLog.Log("Seek to: " + num2);
							fileStream.Seek((Int64)num2, SeekOrigin.Current);
							Byte[] bytes2 = null;
							using (MemoryStream memoryStream = new MemoryStream())
							{
								using (BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream))
								{
									this.WriteDataToStream(list, this.dataTypeList, memoryStream, binaryWriter2);
									bytes2 = memoryStream.ToArray();
								}
							}
							Byte[] array = this.Encryption.Encrypt(bytes2);
							if (!isAutosave)
							{
								Int32 num3 = 18432 + slotID * 15 * 18432 + saveID * 18432;
								ISharedDataLog.Log("Seek to: " + num3);
								fileStream.Seek((Int64)num3, SeekOrigin.Current);
							}
							binaryWriter.Write(array, 0, (Int32)array.Length);
						}
					}
				}
				if (memoriaNode != null)
					memoriaNode.SaveToFile(MetaData.GetMemoriaExtraSaveFilePath(isAutosave, slotID, saveID));
				result = true;
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			result = false;
		}
		return result;
	}

	public override void Load(Int32 slotID, Int32 saveID, ISharedDataStorage.OnLoadFinish onFinishDelegate)
	{
		JSONClass rootNode = this.Load(false, slotID, saveID);
		onFinishDelegate(slotID, saveID, rootNode);
	}

	public override void Save(Int32 slotID, Int32 saveID, JSONClass rootNode, ISharedDataStorage.OnSaveFinish onFinishDelegate)
	{
		Boolean isSuccess = this.Save(false, slotID, saveID, rootNode);
		onFinishDelegate(slotID, saveID, isSuccess);
	}

	public override void Autoload(ISharedDataStorage.OnAutoloadFinish onFinishDelegate)
	{
		JSONClass rootNode = this.Load(true, -1, -1);
		onFinishDelegate(rootNode);
	}

	public override void Autosave(JSONClass rootNode, ISharedDataStorage.OnAutosaveFinish onFinishDelegate)
	{
		Boolean isSuccess = this.Save(true, -1, -1, rootNode);
		onFinishDelegate(isSuccess);
	}

	public override void HasAutoload(ISharedDataStorage.OnHasAutoloadFinish onFinishDelegate)
	{
		JSONClass a = this.Load(true, -1, -1);
		onFinishDelegate(a != null);
	}

	public override void ClearAllData()
	{
		ISharedDataLog.Log("No implementation");
	}

	public override void LoadRawData(ISharedDataStorage.OnLoadRawDataFinish onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			if (!File.Exists(MetaData.FilePath))
			{
				onFinishDelegate(null);
			}
			Byte[] rawData = File.ReadAllBytes(MetaData.FilePath);
			onFinishDelegate(rawData);
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate(null);
		}
	}

	public override void SaveRawData(Byte[] rawData, ISharedDataStorage.OnSaveRawDataFinish onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			File.WriteAllBytes(MetaData.FilePath, rawData);
			onFinishDelegate(true);
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate(false);
		}
	}

	public override void GetLatestSlotAndSave(ISharedDataStorage.OnGetLatestSlotAndSaveFinish onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate(-1, -1);
			return;
		}
		onFinishDelegate(this.metaData.LatestSlot, this.metaData.LatestSave);
	}

	public override void GetLatestTimestamp(ISharedDataStorage.OnGetLatestSaveTimestamp onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate(false, 0.0);
			return;
		}
		onFinishDelegate(true, this.metaData.LatestTimestamp);
	}

	public override void GetGameFinishFlag(ISharedDataStorage.OnGetGameFinishFlag onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate(false, false);
			return;
		}
		onFinishDelegate(true, this.metaData.IsGameFinishFlag != 0);
	}

	public override void SetGameFinishFlagWithTrue(ISharedDataStorage.OnSetGameFinishFlag onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
			this.metaData.IsGameFinishFlag = 1;
			using (FileStream fileStream2 = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.ReadWrite))
			{
				using (BinaryReader binaryReader2 = new BinaryReader(fileStream2))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream2))
					{
						this.metaData.WriteFinishFlag(fileStream2, binaryWriter, binaryReader2, this.Encryption);
					}
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate(false);
			return;
		}
		onFinishDelegate(true);
	}

	public override void GetSelectedLanguage(ISharedDataStorage.OnGetSelectedLanguage onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate(-1);
			return;
		}
		onFinishDelegate(this.metaData.SelectedLanguage);
	}

	public override void SetSelectedLanguage(Int32 selectedLanguage, ISharedDataStorage.OnSetSelectedLanguage onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
			this.metaData.SelectedLanguage = selectedLanguage;
			using (FileStream fileStream2 = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.ReadWrite))
			{
				using (BinaryReader binaryReader2 = new BinaryReader(fileStream2))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream2))
					{
						this.metaData.WriteSelectedLanguage(fileStream2, binaryWriter, binaryReader2, this.Encryption);
					}
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate();
			return;
		}
		onFinishDelegate();
	}

	public override void GetIsAutoLogin(ISharedDataStorage.OnGetIsAutoLogin onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate(-1);
			return;
		}
		onFinishDelegate(this.metaData.IsAutoLogin);
	}

	public override void SetIsAutoLogin(SByte isAutoLogin, ISharedDataStorage.OnSetIsAutoLogin onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
			this.metaData.IsAutoLogin = isAutoLogin;
			using (FileStream fileStream2 = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.ReadWrite))
			{
				using (BinaryReader binaryReader2 = new BinaryReader(fileStream2))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream2))
					{
						this.metaData.WriteIsAutoLogin(fileStream2, binaryWriter, binaryReader2, this.Encryption);
					}
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate();
			return;
		}
		onFinishDelegate();
	}

	public override void GetSystemAchievementStatuses(ISharedDataStorage.OnGetSystemAchievementStatuses onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate(null);
			return;
		}
		onFinishDelegate(this.metaData.SystemAchievementStatuses);
	}

	public override void SetSystemAchievementStatuses(Byte[] systemAchievementStatuses, ISharedDataStorage.OnSetSystemAchievementStatuses onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			Byte[] array = new Byte[(Int32)systemAchievementStatuses.Length];
			for (Int32 i = 0; i < (Int32)systemAchievementStatuses.Length; i++)
			{
				array[i] = systemAchievementStatuses[i];
			}
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
			for (Int32 j = 0; j < (Int32)systemAchievementStatuses.Length; j++)
			{
				this.metaData.SystemAchievementStatuses[j] = array[j];
				global::Debug.Log(String.Concat(new Object[]
				{
					"SetSystemAchievementStatuses : copying data[",
					j,
					"] = 0x",
					this.metaData.SystemAchievementStatuses[j].ToString("X")
				}));
			}
			array = null;
			using (FileStream fileStream2 = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.ReadWrite))
			{
				using (BinaryReader binaryReader2 = new BinaryReader(fileStream2))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream2))
					{
						this.metaData.WriteSystemAchievementStatuses(fileStream2, binaryWriter, binaryReader2, this.Encryption);
					}
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate();
			return;
		}
		onFinishDelegate();
	}

	public override void GetScreenRotation(ISharedDataStorage.OnGetScreenRotation onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate(0);
			return;
		}
		onFinishDelegate(this.metaData.ScreenRotation);
	}

	public override void SetScreenRotation(Byte screenRotation, ISharedDataStorage.OnSetScreenRotation onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
			this.metaData.ScreenRotation = screenRotation;
			global::Debug.Log("ByteStorage: 1 SetScreenRotation = " + this.metaData.ScreenRotation);
			using (FileStream fileStream2 = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.ReadWrite))
			{
				using (BinaryReader binaryReader2 = new BinaryReader(fileStream2))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream2))
					{
						this.metaData.WriteScreenRotation(fileStream2, binaryWriter, binaryReader2, this.Encryption);
					}
				}
			}
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate();
			return;
		}
		global::Debug.Log("ByteStorage: 2 SetScreenRotation = " + this.metaData.ScreenRotation);
		onFinishDelegate();
	}

	public override void ReadSystemData(ISharedDataStorage.OnReadSystemData onFinishDelegate)
	{
		try
		{
			this.CreateDataSchema();
			this.CreateFileIfDoesNotExist(this.metaData);
			using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.metaData.Read(fileStream, binaryReader, this.Encryption);
				}
			}
			global::Debug.Log(String.Concat(new Object[]
			{
				"ByteStorage.ReadSystemData lang = ",
				this.metaData.SelectedLanguage,
				", screenrotation = ",
				this.metaData.ScreenRotation
			}));
		}
		catch (Exception message)
		{
			ISharedDataLog.LogError(message);
			ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
			onFinishDelegate((SharedDataBytesStorage.MetaData)null);
			return;
		}
		onFinishDelegate(this.metaData);
	}

	public override void GetDataSize(ISharedDataStorage.OnGetDataSizeFinish onFinishDelegate)
	{
		if (!Directory.Exists(MetaData.DirPath))
		{
			onFinishDelegate(false, 0);
			return;
		}
		if (!File.Exists(MetaData.FilePath))
		{
			onFinishDelegate(false, 0);
			return;
		}
		FileInfo fileInfo = new FileInfo(MetaData.FilePath);
		Int32 fileSize = (Int32)fileInfo.Length;
		Int32 expectedSize = 2937152;
		if (fileSize == expectedSize)
			onFinishDelegate(true, expectedSize);
		else
			onFinishDelegate(false, 0);
	}

	private const Int32 tmpMaxParseCount = 10000;

	public static readonly String TypeByte = typeof(Byte).FullName;

	public static readonly String TypeSByte = typeof(SByte).FullName;

	public static readonly String TypeInt16 = typeof(Int16).FullName;

	public static readonly String TypeUInt16 = typeof(UInt16).FullName;

	public static readonly String TypeInt32 = typeof(Int32).FullName;

	public static readonly String TypeUInt32 = typeof(UInt32).FullName;

	public static readonly String TypeInt64 = typeof(Int64).FullName;

	public static readonly String TypeUInt64 = typeof(UInt64).FullName;

	public static readonly String TypeSingle = typeof(Single).FullName;

	public static readonly String TypeDouble = typeof(Double).FullName;

	public static readonly String TypeBoolean = typeof(Boolean).FullName;

	public static readonly String TypeString128 = typeof(String).FullName;

	public static String TypeString4K = "String4K";

	protected JsonParser jsonParserInstance;

	private Int32 tmpParseCount;

	private FileStream cStream;

	private BinaryReader cReader;

	private ISharedDataStorage.OnLoadSlotFinish cOnFinishDelegate;

	protected JsonParser jsonParser;

	protected SharedDataBytesStorage.MetaData metaData = new SharedDataBytesStorage.MetaData();

	protected List<JSONData> dataTypeList;

	protected List<JSONNode> dataNodeList;

	protected JSONClass rootSchemaNode;

	public class MetaData
	{
		public void WriteAllFields(Stream stream, BinaryWriter writer, ISharedDataEncryption encryption)
		{
			Byte[] bytes = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(this.Header, 0, 4);
					binaryWriter.Write(this.SaveVersion);
					binaryWriter.Write(this.DataSize);
					binaryWriter.Write(this.LatestSlot);
					binaryWriter.Write(this.LatestSave);
					binaryWriter.Write(this.LatestTimestamp);
					binaryWriter.Write(this.IsGameFinishFlag);
					binaryWriter.Write(this.SelectedLanguage);
					binaryWriter.Write(this.IsAutoLogin);
					binaryWriter.Write(this.SystemAchievementStatuses);
					binaryWriter.Write(this.ScreenRotation);
					Byte[] array = new Byte[249];
					binaryWriter.Write(array, 0, (Int32)array.Length);
					bytes = memoryStream.ToArray();
				}
			}
			Byte[] array2 = encryption.Encrypt(bytes);
			writer.Write(array2, 0, (Int32)array2.Length);
		}

		public void WriteLatestSlotAndSaveAndLatestTimestamp(Stream stream, BinaryWriter writer, BinaryReader reader, ISharedDataEncryption encryption)
		{
			Int32 latestSlot = this.LatestSlot;
			Int32 latestSave = this.LatestSave;
			Double latestTimestamp = this.LatestTimestamp;
			this.Read(stream, reader, encryption);
			stream.Seek(0L, SeekOrigin.Begin);
			if (latestSlot != -1 && latestSave != -1)
			{
				this.LatestSlot = latestSlot;
				this.LatestSave = latestSave;
				this.LatestTimestamp = latestTimestamp;
			}
			this.WriteAllFields(stream, writer, encryption);
		}

		public void WriteFinishFlag(Stream stream, BinaryWriter writer, BinaryReader reader, ISharedDataEncryption encryption)
		{
			Int32 isGameFinishFlag = this.IsGameFinishFlag;
			this.Read(stream, reader, encryption);
			stream.Seek(0L, SeekOrigin.Begin);
			this.IsGameFinishFlag = isGameFinishFlag;
			this.WriteAllFields(stream, writer, encryption);
		}

		public void WriteSelectedLanguage(Stream stream, BinaryWriter writer, BinaryReader reader, ISharedDataEncryption encryption)
		{
			Int32 selectedLanguage = this.SelectedLanguage;
			this.Read(stream, reader, encryption);
			stream.Seek(0L, SeekOrigin.Begin);
			this.SelectedLanguage = selectedLanguage;
			this.WriteAllFields(stream, writer, encryption);
		}

		public void WriteIsAutoLogin(Stream stream, BinaryWriter writer, BinaryReader reader, ISharedDataEncryption encryption)
		{
			SByte isAutoLogin = this.IsAutoLogin;
			this.Read(stream, reader, encryption);
			stream.Seek(0L, SeekOrigin.Begin);
			this.IsAutoLogin = isAutoLogin;
			this.WriteAllFields(stream, writer, encryption);
		}

		public void WriteSystemAchievementStatuses(Stream stream, BinaryWriter writer, BinaryReader reader, ISharedDataEncryption encryption)
		{
			Byte[] array = new Byte[(Int32)this.SystemAchievementStatuses.Length];
			for (Int32 i = 0; i < (Int32)this.SystemAchievementStatuses.Length; i++)
			{
				array[i] = this.SystemAchievementStatuses[i];
			}
			this.Read(stream, reader, encryption);
			stream.Seek(0L, SeekOrigin.Begin);
			for (Int32 j = 0; j < (Int32)array.Length; j++)
			{
				this.SystemAchievementStatuses[j] = array[j];
				global::Debug.Log(String.Concat(new Object[]
				{
					"WriteSystemAchievementStatuses : copying data SystemAchievementStatuses[",
					j,
					"] = 0x",
					this.SystemAchievementStatuses[j].ToString("X")
				}));
			}
			this.WriteAllFields(stream, writer, encryption);
		}

		public void WriteScreenRotation(Stream stream, BinaryWriter writer, BinaryReader reader, ISharedDataEncryption encryption)
		{
			global::Debug.Log("WriteScreenRotation ScreenRotation = " + this.ScreenRotation);
			Byte screenRotation = this.ScreenRotation;
			this.Read(stream, reader, encryption);
			stream.Seek(0L, SeekOrigin.Begin);
			this.ScreenRotation = screenRotation;
			this.WriteAllFields(stream, writer, encryption);
		}

		public void Read(Stream stream, BinaryReader reader, ISharedDataEncryption encryption)
		{
			Int32 cipherSize = encryption.GetCipherSize(288);
			Byte[] bytes = reader.ReadBytes(cipherSize);
			Byte[] buffer = null;
			try
			{
				buffer = encryption.Decrypt(bytes);
			}
			catch (Exception message)
			{
				ISharedDataLog.LogError(message);
				ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
				return;
			}
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					Char[] array = binaryReader.ReadChars(4);
					if (array[0] != 'S' && array[1] != 'A' && array[2] != 'V' && array[3] != 'E')
					{
						ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
					}
					else
					{
						this.SaveVersion = binaryReader.ReadSingle();
						this.DataSize = binaryReader.ReadInt32();
						this.LatestSlot = binaryReader.ReadInt32();
						this.LatestSave = binaryReader.ReadInt32();
						this.LatestTimestamp = binaryReader.ReadDouble();
						this.IsGameFinishFlag = binaryReader.ReadInt32();
						this.SelectedLanguage = binaryReader.ReadInt32();
						this.IsAutoLogin = binaryReader.ReadSByte();
						Int32 num = binaryReader.Read(this.SystemAchievementStatuses, 0, (Int32)this.SystemAchievementStatuses.Length);
						this.ScreenRotation = binaryReader.ReadByte();
						global::Debug.Log(String.Concat(new Object[]
						{
							"Meta.Read: SelectedLanguage = ",
							this.SelectedLanguage,
							", SystemAchievementStatuses[0] = 0x",
							this.SystemAchievementStatuses[0].ToString("X"),
							", ScreenRotation = ",
							this.ScreenRotation
						}));
					}
				}
			}
		}

		private const Int32 metaPlainTextReservedBuffer = 249;

		private const Int32 metaDataSize = 39;

		private const Int32 metaPlainTextSize = 288;

		public const Int32 SlotCount = 10;

		public const Int32 SaveCount = 15;

		public const Int32 TotalSaveCount = 150;

		public const Int32 MetaDataReservedSize = 320;

		public const Int32 PreviewReservedSize = 1024;

		public const Int32 SaveBlockSize = 18432;

		public const Int32 AutosaveReservedSize = 18432;

		public const Int32 DataReservedSize = 18432;

		public Char[] Header = new Char[]
		{
			'S',
			'A',
			'V',
			'E'
		};

		public Single SaveVersion = 1f;

		public Int32 DataSize;

		public static Int32 SystemAchievementStatusesSize = 1;

		public Int32 LatestSlot = -1;

		public Int32 LatestSave = -1;

		public Double LatestTimestamp = -1.0;

		public Int32 IsGameFinishFlag;

		public Int32 SelectedLanguage = -1;

		public SByte IsAutoLogin;

		public Byte[] SystemAchievementStatuses = new Byte[SharedDataBytesStorage.MetaData.SystemAchievementStatusesSize];

		public Byte ScreenRotation;

		public static String FilePath = String.Empty;

		public static String DirPath = String.Empty;

		public static String GetMemoriaExtraSaveFilePath(Boolean isAutosave, Int32 slotID, Int32 saveID)
		{
			if (String.IsNullOrEmpty(MetaData.FilePath) || !MetaData.FilePath.EndsWith(".dat"))
				return String.Empty;
			String extraName = isAutosave ? "_Memoria_Autosave" : $"_Memoria_{slotID}_{saveID}";
			return MetaData.FilePath.Substring(0, MetaData.FilePath.Length - 4) + extraName + ".dat";
		}
	}

	private class JSONNodeWithIndex
	{
		public JSONNodeWithIndex(JSONNode node)
		{
			this.Node = node;
			this.Index = 0;
		}

		public JSONNode Node;

		public Int32 Index;
	}
}
