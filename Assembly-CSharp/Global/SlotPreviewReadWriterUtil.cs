using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SlotPreviewReadWriterUtil : MonoBehaviour
{
    private void Awake()
    {
        if (SlotPreviewReadWriterUtil.Instance == (UnityEngine.Object)null)
        {
            SlotPreviewReadWriterUtil.Instance = this;
        }
    }

    public void ReadSlotPreview(SharedDataBytesStorage.MetaData metaData, Int32 slotID, Stream stream, BinaryReader reader, ISharedDataEncryption encryption, ISharedDataStorage.OnLoadSlotFinish onFinishDelegate)
    {
        base.StartCoroutine(this.ReadSlotPreviewInCoroutine(metaData, slotID, stream, reader, encryption, onFinishDelegate));
    }

    private IEnumerator ReadSlotPreviewInCoroutine(SharedDataBytesStorage.MetaData metaData, Int32 slotID, Stream stream, BinaryReader reader, ISharedDataEncryption encryption, ISharedDataStorage.OnLoadSlotFinish onFinishDelegate)
    {
        List<SharedDataPreviewSlot> list = new List<SharedDataPreviewSlot>();
        metaData.Read(stream, reader, encryption);
        stream.Seek((Int64)(320 - (Int32)stream.Position), SeekOrigin.Current);
        Int32 seekTo = slotID * 1024 * 15;
        ISharedDataLog.Log("Seek to: " + seekTo);
        stream.Seek((Int64)seekTo, SeekOrigin.Current);
        Int32 previewCipherSize = encryption.GetCipherSize(965);
        Int32 i = 0;
        while (i < 15)
        {
            yield return onFinishDelegate;
            Int32 prevPosition = (Int32)stream.Position;
            Int32 seekSize = 0;
            Byte[] cipherText = new Byte[previewCipherSize];
            reader.Read(cipherText, 0, (Int32)cipherText.Length);
            Byte[] plainText = null;
            try
            {
                plainText = encryption.Decrypt(cipherText);
            }
            catch (Exception ex2)
            {
                Exception ex = ex2;
                list.Add(new SharedDataPreviewSlot
                {
                    IsPreviewCorrupted = true
                });
                ISharedDataLog.LogError(ex);
                ISharedDataSerializer.LastErrno = DataSerializerErrorCode.DataCorruption;
                seekSize = 1024 - ((Int32)stream.Position - prevPosition);
                stream.Seek((Int64)seekSize, SeekOrigin.Current);
                goto IL_597;
            }
            goto IL_1CC;
        IL_597:
            i++;
            continue;
        IL_1CC:
            Boolean isValidPreview = false;
            SharedDataPreviewSlot preview = new SharedDataPreviewSlot();
            using (MemoryStream memStram = new MemoryStream(plainText))
            {
                using (BinaryReader memReader = new BinaryReader(memStram))
                {
                    Char[] chs = memReader.ReadChars(4);
                    if (chs[0] == 'N' && chs[1] == 'O' && chs[2] == 'N' && chs[3] == 'E')
                    {
                        list.Add((SharedDataPreviewSlot)null);
                        seekSize = 1024 - ((Int32)stream.Position - prevPosition);
                        stream.Seek((Int64)seekSize, SeekOrigin.Current);
                        goto IL_597;
                    }
                    if (chs[0] != 'P' || chs[1] != 'R' || chs[2] != 'E' || chs[3] != 'V')
                    {
                        list.Add(new SharedDataPreviewSlot
                        {
                            IsPreviewCorrupted = true
                        });
                        ISharedDataLog.LogError("Error! seek to NOT preview position");
                        seekSize = 1024 - ((Int32)stream.Position - prevPosition);
                        stream.Seek((Int64)seekSize, SeekOrigin.Current);
                        goto IL_597;
                    }
                    preview.CharacterInfoList = new List<SharedDataPreviewCharacterInfo>();
                    preview.HasData = memReader.ReadBoolean();
                    preview.Gil = memReader.ReadInt64();
                    preview.PlayDuration = memReader.ReadUInt64();
                    preview.win_type = memReader.ReadUInt64();
                    preview.Location = Encoding.UTF8.GetString(memReader.ReadBytes(128), 0, 128);
                    preview.Location = preview.Location.Trim(new Char[1]);
                    if (preview.HasData)
                    {
                        isValidPreview = true;
                    }
                    for (Int32 j = 0; j < 4; j++)
                    {
                        SharedDataPreviewCharacterInfo cInfo = new SharedDataPreviewCharacterInfo();
                        cInfo.SerialID = memReader.ReadInt32();
                        cInfo.Level = memReader.ReadInt32();
                        cInfo.Name = Encoding.UTF8.GetString(memReader.ReadBytes(128), 0, 128);
                        cInfo.Name = cInfo.Name.Trim(new Char[1]);
                        if (cInfo.SerialID == -1)
                        {
                            preview.CharacterInfoList.Add((SharedDataPreviewCharacterInfo)null);
                        }
                        else
                        {
                            preview.CharacterInfoList.Add(cInfo);
                        }
                    }
                    preview.Timestamp = memReader.ReadDouble();
                }
            }
            if (isValidPreview)
            {
                list.Add(preview);
            }
            else
            {
                list.Add((SharedDataPreviewSlot)null);
            }
            seekSize = 1024 - ((Int32)stream.Position - prevPosition);
            stream.Seek((Int64)seekSize, SeekOrigin.Current);
            goto IL_597;
        }
        onFinishDelegate(slotID, list);
        yield break;
    }

    public void WriteSlotPreview(SharedDataPreviewSlot previewSlot, SharedDataBytesStorage.MetaData metaData, Int32 saveID, Int32 slotID, Stream stream, BinaryReader reader, BinaryWriter writer, ISharedDataEncryption encryption, ISharedDataStorage.OnSaveSlotFinish onFinishDelegate)
    {
        stream.Seek((Int64)(320 - (Int32)stream.Position), SeekOrigin.Current);
        Int32 num = slotID * 1024 * 15 + saveID * 1024;
        ISharedDataLog.Log("Seek to: " + num);
        stream.Seek((Int64)num, SeekOrigin.Current);
        Int32 num2 = (Int32)stream.Position;
        Int32 cipherSize = encryption.GetCipherSize(965);
        Byte[] buffer = new Byte[cipherSize];
        Byte[] array = null;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write('P');
                binaryWriter.Write('R');
                binaryWriter.Write('E');
                binaryWriter.Write('V');
                previewSlot.HasData = true;
                binaryWriter.Write(previewSlot.HasData);
                binaryWriter.Write(previewSlot.Gil);
                binaryWriter.Write(previewSlot.PlayDuration);
                binaryWriter.Write(previewSlot.win_type);
                Byte[] array2 = new Byte[128];
                Byte[] bytes = Encoding.UTF8.GetBytes(previewSlot.Location);
                Buffer.BlockCopy(bytes, 0, array2, 0, (Int32)bytes.Length);
                binaryWriter.Write(array2, 0, (Int32)array2.Length);
                Byte[] array3 = new Byte[136];
                for (Int32 i = 0; i < 4; i++)
                {
                    if (previewSlot.CharacterInfoList[i] == null)
                    {
                        Int32 value = -1;
                        Int32 value2 = -1;
                        binaryWriter.Write(value);
                        binaryWriter.Write(value2);
                        Byte[] array4 = new Byte[128];
                        binaryWriter.Write(array4, 0, (Int32)array4.Length);
                    }
                    else
                    {
                        binaryWriter.Write(previewSlot.CharacterInfoList[i].SerialID);
                        binaryWriter.Write(previewSlot.CharacterInfoList[i].Level);
                        Byte[] array5 = new Byte[128];
                        Byte[] bytes2 = Encoding.UTF8.GetBytes(previewSlot.CharacterInfoList[i].Name);
                        Buffer.BlockCopy(bytes2, 0, array5, 0, (Int32)bytes2.Length);
                        binaryWriter.Write(array5, 0, (Int32)array5.Length);
                    }
                }
                Double totalSeconds = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                previewSlot.Timestamp = totalSeconds;
                binaryWriter.Write(previewSlot.Timestamp);
                Byte[] array6 = new Byte[256];
                binaryWriter.Write(array6, 0, (Int32)array6.Length);
                array = memoryStream.ToArray();
                if ((Int32)array.Length != 965)
                {
                    ISharedDataLog.LogError("previewPlainText has incorrect size: " + (Int32)array.Length);
                }
            }
        }
        buffer = encryption.Encrypt(array);
        stream.Seek((Int64)num2, SeekOrigin.Begin);
        writer.Write(buffer, 0, cipherSize);
        onFinishDelegate(slotID, saveID, previewSlot);
    }

    public static SlotPreviewReadWriterUtil Instance;
}
