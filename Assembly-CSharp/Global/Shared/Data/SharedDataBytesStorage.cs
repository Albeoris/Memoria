using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Assets;
using Memoria.Prime;
using SimpleJSON;
using UnityEngine;

public class SharedDataBytesStorage : ISharedDataStorage
{
    static SharedDataBytesStorage()
    {
        SetPCPath();
    }

    private void Awake()
    {
        if (Application.isEditor)
            this.SetEditorPath();
        else
            SetPCPath();
        ISharedDataLog.Log("MetaData.FilePath: " + MetaData.FilePath);
    }

    public static void SetPCPath()
    {
        if (!String.IsNullOrEmpty(MetaData.FilePath))
            return;

        String fileName;
        String saveFolder = "/Steam/EncryptedSavedData";

        if (FF9StateSystem.PCEStorePlatform)
        {
            String myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            myDocuments = myDocuments.Replace('\\', '/');
            MetaData.DirPath = myDocuments + "/My Games/FINAL FANTASY IX/EncryptedSaveData";
        }
        else
        {
            MetaData.DirPath = AssetManagerUtil.GetPersistentDataPath() + saveFolder;
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

        MetaData.FilePath = Application.persistentDataPath + saveFolder + "/" + fileName;
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
        if (this.jsonParserInstance == null)
        {
            GameObject jsonGo = new GameObject("JsonParser");
            this.jsonParserInstance = jsonGo.AddComponent<JsonParser>();
            this.jsonParserInstance.transform.parent = base.transform;
        }
        return this.jsonParserInstance;
    }

    private JSONClass ParseDataListToJsonTree(List<JSONData> jsonList, JSONClass rootSchemaNode)
    {
        Stack<SharedDataBytesStorage.JSONNodeWithIndex> stack = new Stack<SharedDataBytesStorage.JSONNodeWithIndex>();
        stack.Push(new SharedDataBytesStorage.JSONNodeWithIndex(rootSchemaNode));
        ISharedDataLog.Log("dataList.Count: " + jsonList.Count);
        Int32 listIndex = 0;
        while (stack.Count > 0)
        {
            SharedDataBytesStorage.JSONNodeWithIndex nextNode = stack.Peek();
            if (nextNode.Node is JSONClass)
            {
                List<String> nodeFieldList = ((JSONClass)nextNode.Node).Dict.Keys.ToList();
                nodeFieldList.Sort();
                if (nextNode.Index < nextNode.Node.Count)
                {
                    String aKey = nodeFieldList[nextNode.Index];
                    JSONNode node = nextNode.Node[aKey];
                    stack.Push(new SharedDataBytesStorage.JSONNodeWithIndex(node));
                    nextNode.Index++;
                }
                else
                {
                    stack.Pop();
                }
            }
            else if (nextNode.Node is JSONArray)
            {
                if (nextNode.Index < nextNode.Node.Count)
                {
                    JSONNode node = nextNode.Node[nextNode.Index];
                    stack.Push(new SharedDataBytesStorage.JSONNodeWithIndex(node));
                    nextNode.Index++;
                }
                else
                {
                    stack.Pop();
                }
            }
            else if (nextNode.Node is JSONData)
            {
                ISharedDataLog.Log("data: " + jsonList[listIndex]);
                nextNode.Node.Value = jsonList[listIndex];
                listIndex++;
                stack.Pop();
            }
        }
        return rootSchemaNode;
    }

    private List<JSONData> ParseJsonTreeToDataList(JSONClass rootNode)
    {
        this.tmpParseCount = 0;
        List<JSONData> jsonList = new List<JSONData>();
        Stack<SharedDataBytesStorage.JSONNodeWithIndex> stack = new Stack<SharedDataBytesStorage.JSONNodeWithIndex>();
        stack.Push(new SharedDataBytesStorage.JSONNodeWithIndex(rootNode));
        while (stack.Count > 0)
        {
            SharedDataBytesStorage.JSONNodeWithIndex nextNode = stack.Peek();
            if (nextNode.Node is JSONClass)
            {
                List<String> nodeFieldList = ((JSONClass)nextNode.Node).Dict.Keys.ToList();
                nodeFieldList.Sort();
                if (nextNode.Index < nextNode.Node.Count)
                {
                    String aKey = nodeFieldList[nextNode.Index];
                    JSONNode jsonnode = nextNode.Node[aKey];
                    if (jsonnode == null)
                        ISharedDataLog.LogError("child is null");
                    else
                        stack.Push(new SharedDataBytesStorage.JSONNodeWithIndex(jsonnode));
                    nextNode.Index++;
                }
                else
                {
                    stack.Pop();
                }
            }
            else if (nextNode.Node is JSONArray)
            {
                if (nextNode.Index < nextNode.Node.Count)
                {
                    JSONNode jsonnode = nextNode.Node[nextNode.Index];
                    if (jsonnode == null)
                        ISharedDataLog.LogError("child is null");
                    else
                        stack.Push(new SharedDataBytesStorage.JSONNodeWithIndex(jsonnode));
                    nextNode.Index++;
                }
                else
                {
                    stack.Pop();
                }
            }
            else if (nextNode.Node is JSONData)
            {
                jsonList.Add((JSONData)nextNode.Node);
                stack.Pop();
            }
            else
            {
                ISharedDataLog.LogError("Parsing failed!");
                return jsonList;
            }
        }
        return jsonList;
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
            ISharedDataLog.LogError($"data.Count({data.Count}) != schema.Count({schema.Count})");
            return false;
        }
        return true;
    }

    private Int32 EstimateDataSize(List<JSONData> dataTypeList)
    {
        Int32 dataSize = 0;
        foreach (JSONData jsonNode in dataTypeList)
        {
            String typeName = jsonNode;
            if (String.Equals(typeName, SharedDataBytesStorage.TypeByte))
                dataSize++;
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeSByte))
                dataSize++;
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeInt16))
                dataSize += 2;
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeUInt16))
                dataSize += 2;
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeInt32))
                dataSize += 4;
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeUInt32))
                dataSize += 4;
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeInt64))
                dataSize += 8;
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeUInt64))
                dataSize += 8;
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeSingle))
                dataSize += 4;
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeDouble))
                dataSize += 8;
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeBoolean))
                dataSize++;
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeString128))
                dataSize += 128;
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeString4K))
                dataSize += 4096;
            else
                ISharedDataLog.LogError("Data type not found, AT type estimation, with type: " + jsonNode);
        }
        if (dataSize > MetaData.DataReservedSize)
        {
            ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
            ISharedDataLog.LogError($"Estimated saved data size({dataSize}) larger than Save reserved size({MetaData.DataReservedSize})!");
        }
        return dataSize;
    }

    private void ReadDataFromStream(List<JSONNode> dataList, List<JSONData> dataTypeList, Stream stream, BinaryReader reader)
    {
        for (Int32 i = 0; i < dataTypeList.Count; i++)
        {
            String typeName = dataTypeList[i];
            if (String.Equals(typeName, SharedDataBytesStorage.TypeByte))
                dataList.Add(reader.ReadByte().ToString());
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeSByte))
                dataList.Add(reader.ReadSByte().ToString());
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeInt16))
                dataList.Add(reader.ReadInt16().ToString());
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeUInt16))
                dataList.Add(reader.ReadUInt16().ToString());
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeInt32))
                dataList.Add(reader.ReadInt32().ToString());
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeUInt32))
                dataList.Add(reader.ReadUInt32().ToString());
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeInt64))
                dataList.Add(reader.ReadInt64().ToString());
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeUInt64))
                dataList.Add(reader.ReadUInt64().ToString());
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeSingle))
                dataList.Add(reader.ReadSingle().ToString());
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeDouble))
                dataList.Add(reader.ReadDouble().ToString());
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeBoolean))
                dataList.Add(reader.ReadBoolean().ToString());
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeString128))
            {
                String str = Encoding.UTF8.GetString(reader.ReadBytes(128), 0, 128);
                str = str.Trim(new Char[1]);
                dataList.Add(str);
            }
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeString4K))
            {
                String dataStr = Encoding.UTF8.GetString(reader.ReadBytes(4096), 0, 4096);
                dataStr = dataStr.Trim(new Char[1]);
                dataList.Add(dataStr);
            }
            else
            {
                ISharedDataLog.LogError("Data type not found, AT type parser, with type: " + typeName);
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
            String typeName = dataTypeList[i];
            JSONData jsonData = dataList[i];
            if (String.Equals(typeName, SharedDataBytesStorage.TypeByte))
                writer.Write((Byte)jsonData.AsInt);
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeSByte))
                writer.Write((SByte)jsonData.AsInt);
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeInt16))
                writer.Write((Int16)jsonData.AsInt);
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeUInt16))
                writer.Write((UInt16)jsonData.AsInt);
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeInt32))
                writer.Write(jsonData.AsInt);
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeUInt32))
                writer.Write(jsonData.AsUInt);
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeInt64))
                writer.Write(jsonData.AsLong);
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeUInt64))
                writer.Write(jsonData.AsULong);
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeSingle))
                writer.Write(jsonData.AsFloat);
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeDouble))
                writer.Write(jsonData.AsDouble);
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeBoolean))
                writer.Write(jsonData.AsBool);
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeString128))
            {
                Byte[] str = Encoding.UTF8.GetBytes(jsonData);
                Int32 strLen = Encoding.UTF8.GetByteCount(jsonData);
                Byte[] buffer = new Byte[128];
                Buffer.BlockCopy(str, 0, buffer, 0, Math.Min(strLen, 128));
                writer.Write(buffer, 0, 128);
            }
            else if (String.Equals(typeName, SharedDataBytesStorage.TypeString4K))
            {
                Byte[] dataStr = Encoding.UTF8.GetBytes(jsonData);
                Int32 dataStrLen = Encoding.UTF8.GetByteCount(jsonData);
                Byte[] buffer = new Byte[4096];
                Buffer.BlockCopy(dataStr, 0, buffer, 0, Math.Min(dataStrLen, 4096));
                writer.Write(buffer, 0, 4096);
            }
            else
            {
                ISharedDataLog.LogError("Data type not found, AT type parser, with type: " + typeName);
            }
        }
    }

    protected void CreateFileIfDoesNotExist(SharedDataBytesStorage.MetaData metaData)
    {
        if (!Directory.Exists(MetaData.DirPath))
            Directory.CreateDirectory(MetaData.DirPath);
        if (!File.Exists(MetaData.FilePath))
        {
            using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter fileWriter = new BinaryWriter(fileStream))
                {
                    // Write meta-data
                    metaData.WriteAllFields(fileStream, fileWriter, this.Encryption);
                    Int32 paddingSize = MetaData.MetaDataReservedSize - (Int32)fileStream.Position;
                    Byte[] padding = new Byte[paddingSize];
                    ISharedDataLog.Log("Write to: " + paddingSize);
                    fileWriter.Write(padding, 0, paddingSize);
                    // Write empty previews
                    Byte[] emptySaveRaw = new Byte[965];
                    using (MemoryStream emptySaveStream = new MemoryStream(emptySaveRaw))
                    {
                        using (BinaryWriter emptySaveWriter = new BinaryWriter(emptySaveStream))
                        {
                            emptySaveWriter.Write('N');
                            emptySaveWriter.Write('O');
                            emptySaveWriter.Write('N');
                            emptySaveWriter.Write('E');
                        }
                    }
                    Byte[] encryptedEmptySave = this.Encryption.Encrypt(emptySaveRaw);
                    paddingSize = MetaData.PreviewReservedSize - encryptedEmptySave.Length;
                    padding = new Byte[paddingSize];
                    for (Int32 i = 0; i < MetaData.TotalSaveCount; i++)
                    {
                        fileWriter.Write(encryptedEmptySave, 0, encryptedEmptySave.Length);
                        fileWriter.Write(padding, 0, padding.Length);
                    }
                    // Write empty saves
                    this.CreateDataSchema();
                    Int32 saveSize = metaData.DataSize;
                    emptySaveRaw = new Byte[saveSize + 4];
                    using (MemoryStream emptySaveStream = new MemoryStream(emptySaveRaw))
                    {
                        using (BinaryWriter emptySaveWriter = new BinaryWriter(emptySaveStream))
                        {
                            emptySaveWriter.Write('N');
                            emptySaveWriter.Write('O');
                            emptySaveWriter.Write('N');
                            emptySaveWriter.Write('E');
                        }
                    }
                    encryptedEmptySave = this.Encryption.Encrypt(emptySaveRaw);
                    paddingSize = MetaData.SaveBlockSize - encryptedEmptySave.Length;
                    padding = new Byte[paddingSize];
                    fileWriter.Write(encryptedEmptySave, 0, encryptedEmptySave.Length);
                    fileWriter.Write(padding, 0, padding.Length);
                    for (Int32 i = 0; i < MetaData.TotalSaveCount; i++)
                    {
                        fileWriter.Write(encryptedEmptySave, 0, encryptedEmptySave.Length);
                        fileWriter.Write(padding, 0, padding.Length);
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
        catch (Exception err)
        {
            ISharedDataLog.LogError(err);
            ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
            if (this.cStream != null)
                this.cStream.Close();
            if (this.cReader != null)
                this.cReader.Close();
            this.cStream = null;
            this.cReader = null;
            onFinishDelegate(-1, null);
            return;
        }
        SlotPreviewReadWriterUtil.Instance.ReadSlotPreview(this.metaData, slotID, this.cStream, this.cReader, this.Encryption, new ISharedDataStorage.OnLoadSlotFinish(this.LoadSlotPreviewDelegate));
    }

    private void LoadSlotPreviewDelegate(Int32 outSlotID, List<SharedDataPreviewSlot> slotList)
    {
        this.cReader.Close();
        this.cStream.Close();
        this.cStream = null;
        this.cReader = null;
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
            SharedDataPreviewCharacterInfo previewInfo = null;
            PLAYER player = FF9StateSystem.Common.FF9.party.member[i];
            if (player != null)
            {
                previewInfo = new SharedDataPreviewCharacterInfo();
                previewInfo.SerialID = (Int32)player.info.serial_no;
                previewInfo.Name = FF9TextTool.RemoveOpCode(player.Name);
                previewInfo.Level = player.level;
            }
            sharedDataPreviewSlot.CharacterInfoList.Add(previewInfo);
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
        catch (Exception err)
        {
            ISharedDataLog.LogError(err);
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
            Boolean saveIsValid = false;
            using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader encryptedReader = new BinaryReader(fileStream))
                {
                    this.metaData.Read(fileStream, encryptedReader, this.Encryption);
                    FF9StateSystem.latestSlot = this.metaData.LatestSlot;
                    FF9StateSystem.latestSave = this.metaData.LatestSave;
                    FF9StateSystem.LatestTimestamp = this.metaData.LatestTimestamp;
                    Int32 saveOffset = MetaData.BaseSaveBlockOffset;
                    if (!isAutoload)
                        saveOffset += MetaData.SaveBlockSize * (1 + slotID * MetaData.SaveCount + saveID);
                    ISharedDataLog.Log("Seek to: " + saveOffset);
                    fileStream.Seek(saveOffset, SeekOrigin.Begin);
                    Int32 cipherSize = this.Encryption.GetCipherSize(this.metaData.DataSize + 4);
                    Byte[] encryptedJson = new Byte[cipherSize];
                    encryptedReader.Read(encryptedJson, 0, encryptedJson.Length);
                    Byte[] jsonRaw = null;
                    try
                    {
                        jsonRaw = this.Encryption.Decrypt(encryptedJson);
                    }
                    catch (Exception err)
                    {
                        ISharedDataLog.LogError(err);
                        ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
                        return null;
                    }
                    if (jsonRaw.Length != this.metaData.DataSize + 4)
                    {
                        ISharedDataLog.LogError($"plainText.size({jsonRaw.Length}) and metaData.DataSize({this.metaData.DataSize}) is NOT equal, load the old data?");
                        ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
                        return null;
                    }
                    try
                    {
                        using (MemoryStream memoryStream = new MemoryStream(jsonRaw))
                        {
                            using (BinaryReader jsonReader = new BinaryReader(memoryStream))
                            {
                                String magic = new String(jsonReader.ReadChars(4));
                                if (magic == "SAVE")
                                {
                                    saveIsValid = true;
                                    this.ReadDataFromStream(this.dataNodeList, this.dataTypeList, memoryStream, jsonReader);
                                }
                                else
                                {
                                    if (magic != "NONE")
                                    {
                                        ISharedDataLog.LogError($"Save block starts with '{magic}' instead of 'SAVE' or 'NONE'");
                                        ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
                                        return null;
                                    }
                                    saveIsValid = false;
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        ISharedDataLog.LogError(err);
                        ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
                        return null;
                    }
                }
            }
            if (!saveIsValid)
                return null;
            List<JSONData> jsonList = new List<JSONData>();
            foreach (JSONNode jsonnode in this.dataNodeList)
                jsonList.Add((JSONData)jsonnode);
            if (!this.ValidateDataListWithSchemaDataList(jsonList, this.dataTypeList))
            {
                ISharedDataLog.LogError("Verification failed!");
                return null;
            }
            JSONClass fullTree = this.ParseDataListToJsonTree(jsonList, this.rootSchemaNode);
            String memoriaSavePath = MetaData.GetMemoriaExtraSaveFilePath(isAutoload, slotID, saveID);
            if (!String.IsNullOrEmpty(memoriaSavePath) && File.Exists(memoriaSavePath))
            {
                JSONNode memoriaNode = JSONNode.LoadFromFile(memoriaSavePath);
                if (memoriaNode != null)
                {
                    memoriaNode.Add("MetaDataSlotID", slotID.ToString());
                    memoriaNode.Add("MetaDataSaveID", saveID.ToString());
                    memoriaNode.Add("MetaDataFileName", Path.GetFileName(memoriaSavePath));
                    fullTree.Add("MemoriaExtraData", memoriaNode);
                }
            }
            return fullTree;
        }
        catch (Exception err)
        {
            ISharedDataLog.LogError(err);
            ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
        }
        return null;
    }

    protected Boolean Save(Boolean isAutosave, Int32 slotID, Int32 saveID, JSONClass rootNode)
    {
        Boolean result;
        try
        {
            JSONClass memoriaNode = rootNode.Remove("MemoriaExtraData")?.AsObject;
            this.CreateDataSchema();
            List<JSONData> jsonList = this.ParseJsonTreeToDataList(rootNode);
            if (!this.ValidateDataListWithSchemaDataList(jsonList, this.dataTypeList))
            {
                ISharedDataLog.LogError("Verification failed!");
                result = false;
            }
            else
            {
                ISharedDataLog.Log("Verification success, start to pack data");
                this.metaData.DataSize = this.EstimateDataSize(this.dataTypeList);
                FF9StateSystem.latestSlot = slotID;
                FF9StateSystem.latestSave = saveID;
                this.metaData.LatestSlot = slotID;
                this.metaData.LatestSave = saveID;
                Double totalSeconds = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                FF9StateSystem.LatestTimestamp = totalSeconds;
                this.metaData.LatestTimestamp = totalSeconds;
                ISharedDataLog.Log("DataSize: " + this.metaData.DataSize);
                ISharedDataLog.Log("TotalDataSize should be: " + MetaData.TotalSaveCount * this.metaData.DataSize);
                this.CreateFileIfDoesNotExist(this.metaData);
                using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    using (BinaryReader fileReader = new BinaryReader(fileStream))
                    {
                        using (BinaryWriter fileWriter = new BinaryWriter(fileStream))
                        {
                            this.metaData.WriteLatestSlotAndSaveAndLatestTimestamp(fileStream, fileWriter, fileReader, this.Encryption);
                            Byte[] jsonRaw;
                            using (MemoryStream jsonStream = new MemoryStream())
                            {
                                using (BinaryWriter jsonStreamWriter = new BinaryWriter(jsonStream))
                                {
                                    this.WriteDataToStream(jsonList, this.dataTypeList, jsonStream, jsonStreamWriter);
                                    jsonRaw = jsonStream.ToArray();
                                }
                            }
                            Int32 saveOffset = MetaData.BaseSaveBlockOffset;
                            if (!isAutosave)
                                saveOffset += MetaData.SaveBlockSize * (1 + slotID * MetaData.SaveCount + saveID);
                            ISharedDataLog.Log("Seek to: " + saveOffset);
                            fileStream.Seek(saveOffset, SeekOrigin.Begin);
                            Byte[] encryptedJson = this.Encryption.Encrypt(jsonRaw);
                            fileWriter.Write(encryptedJson, 0, encryptedJson.Length);
                        }
                    }
                }
                if (memoriaNode != null)
                    memoriaNode.SaveToFile(MetaData.GetMemoriaExtraSaveFilePath(isAutosave, slotID, saveID));
                result = true;
            }
        }
        catch (Exception err)
        {
            ISharedDataLog.LogError(err);
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
            using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                    {
                        this.metaData.WriteFinishFlag(fileStream, binaryWriter, binaryReader, this.Encryption);
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
            using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                    {
                        this.metaData.WriteSelectedLanguage(fileStream, binaryWriter, binaryReader, this.Encryption);
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
            using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                    {
                        this.metaData.WriteIsAutoLogin(fileStream, binaryWriter, binaryReader, this.Encryption);
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
            using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    this.metaData.Read(fileStream, binaryReader, this.Encryption);
                }
            }
            for (Int32 i = 0; i < systemAchievementStatuses.Length; i++)
            {
                this.metaData.SystemAchievementStatuses[i] = systemAchievementStatuses[i];
                global::Debug.Log($"SetSystemAchievementStatuses : copying data[{i}] = 0x{this.metaData.SystemAchievementStatuses[i]:X}");
            }
            using (FileStream fileStream = File.Open(MetaData.FilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                    {
                        this.metaData.WriteSystemAchievementStatuses(fileStream, binaryWriter, binaryReader, this.Encryption);
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
            global::Debug.Log($"ByteStorage.ReadSystemData lang = {this.metaData.SelectedLanguage}, screenrotation = {this.metaData.ScreenRotation}");
        }
        catch (Exception message)
        {
            ISharedDataLog.LogError(message);
            ISharedDataSerializer.LastErrno = DataSerializerErrorCode.FileCorruption;
            onFinishDelegate(null);
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
            Byte[] metaDataRaw = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(MetaData.HeaderChars);
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
                    Byte[] padding = new Byte[MetaData.MetaDataReservedBuffer];
                    binaryWriter.Write(padding, 0, padding.Length);
                    metaDataRaw = memoryStream.ToArray();
                }
            }
            Byte[] encryptedMetadata = encryption.Encrypt(metaDataRaw);
            writer.Write(encryptedMetadata, 0, encryptedMetadata.Length);
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
            Byte[] achievementStatuses = new Byte[this.SystemAchievementStatuses.Length];
            for (Int32 i = 0; i < this.SystemAchievementStatuses.Length; i++)
                achievementStatuses[i] = this.SystemAchievementStatuses[i];
            this.Read(stream, reader, encryption);
            stream.Seek(0L, SeekOrigin.Begin);
            for (Int32 i = 0; i < achievementStatuses.Length; i++)
            {
                this.SystemAchievementStatuses[i] = achievementStatuses[i];
                global::Debug.Log($"WriteSystemAchievementStatuses : copying data SystemAchievementStatuses[{i}] = 0x{this.SystemAchievementStatuses[i]:X}");
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
            Int32 cipherSize = encryption.GetCipherSize(MetaData.MetaDataFullSize);
            Byte[] encryptedMetadata = reader.ReadBytes(cipherSize);
            Byte[] metaDataRaw;
            try
            {
                metaDataRaw = encryption.Decrypt(encryptedMetadata);
            }
            catch (Exception message)
            {
                ISharedDataLog.LogError(message);
                ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
                return;
            }
            using (MemoryStream memoryStream = new MemoryStream(metaDataRaw))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    String magic = new String(binaryReader.ReadChars(4));
                    if (magic != MetaData.Header)
                    {
                        ISharedDataLog.LogError($"Savefile starts with '{magic}' instead of '{MetaData.Header}'");
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
                        binaryReader.Read(this.SystemAchievementStatuses, 0, this.SystemAchievementStatuses.Length);
                        this.ScreenRotation = binaryReader.ReadByte();
                        global::Debug.Log($"Meta.Read: SelectedLanguage = {this.SelectedLanguage}, SystemAchievementStatuses[0] = 0x{this.SystemAchievementStatuses[0]:X}, ScreenRotation = {this.ScreenRotation}");
                    }
                }
            }
        }

        private const Int32 MetaDataReservedBuffer = 249;
        private const Int32 MetaDataSize = 39;
        private const Int32 MetaDataFullSize = 288;
        public const Int32 SlotCount = 10;
        public const Int32 SaveCount = 15;
        public const Int32 TotalSaveCount = 150;
        public const Int32 MetaDataReservedSize = 320;
        public const Int32 PreviewReservedSize = 1024;
        public const Int32 SaveBlockSize = 18432;
        public const Int32 AutosaveReservedSize = 18432;
        public const Int32 DataReservedSize = 18432;
        public const Int32 BaseSaveBlockOffset = MetaDataReservedSize + TotalSaveCount * PreviewReservedSize;
        public const String Header = "SAVE";
        public static readonly Char[] HeaderChars = ['S', 'A', 'V', 'E'];

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
