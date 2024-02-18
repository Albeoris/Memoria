using System;
using System.IO;
using System.Collections.Generic;
using Memoria.Prime;

public class SFXBinaryFile
{
	public UInt16 chunkCount;
	public List<UInt16> chunkIndex = new List<UInt16>();
	public List<UInt16> resourceCount = new List<UInt16>();
	public List<List<Byte>> resourceID = new List<List<Byte>>();
	public List<List<Byte>> resourceInfo = new List<List<Byte>>();
	public List<List<UInt16>> resourceSize = new List<List<UInt16>>();
	public List<List<UInt32>> resourceOffset = new List<List<UInt32>>();

	public List<UInt16> smallFileFullSize = new List<UInt16>();
	public List<UInt32> smallFileBaseOffset = new List<UInt32>();
	public List<UInt32> smallFileCount = new List<UInt32>();
	public List<List<Int32>> smallFileIndex = new List<List<Int32>>();
	public List<List<UInt16>> smallFileOffset = new List<List<UInt16>>();
	public List<List<UInt16>> smallFileFlags = new List<List<UInt16>>();
	public List<List<UInt32>> smallFileSize = new List<List<UInt32>>();
	public List<List<Byte[]>> smallFileRaw = new List<List<Byte[]>>();
	public List<UInt32> externalFileCount = new List<UInt32>();
	public List<List<Int32>> externalFileOffset = new List<List<Int32>>();

	public List<KeyValuePair<Int32, Int32>> cameraFileList = new List<KeyValuePair<Int32, Int32>>();
	public List<KeyValuePair<Int32, Int32>> akaoFileList = new List<KeyValuePair<Int32, Int32>>();

	public List<SequenceCode> sequence = new List<SequenceCode>();

	public void Load(Byte[] sfxBinary)
	{
		using BinaryReader reader = new BinaryReader(new MemoryStream(sfxBinary));
		chunkCount = reader.ReadUInt16();
		UInt32 resourcePosition = 0x800u;
		for (Int32 i = 0; i < chunkCount; i++)
		{
			chunkIndex.Add(reader.ReadUInt16());
			resourceCount.Add(reader.ReadUInt16());
			resourceID.Add(new List<Byte>());
			resourceInfo.Add(new List<Byte>());
			resourceSize.Add(new List<UInt16>());
			resourceOffset.Add(new List<UInt32>());
			smallFileIndex.Add(new List<Int32>());
			smallFileOffset.Add(new List<UInt16>());
			smallFileFlags.Add(new List<UInt16>());
			externalFileOffset.Add(new List<Int32>());
			smallFileSize.Add(new List<UInt32>());
			smallFileRaw.Add(new List<Byte[]>());
			smallFileFullSize.Add(0);
			smallFileBaseOffset.Add(0);
			smallFileCount.Add(0);
			externalFileCount.Add(0);
			for (Int32 j = 0; j < resourceCount[i]; j++)
			{
				resourceOffset[i].Add(resourcePosition);
				resourceID[i].Add(reader.ReadByte());
				resourceInfo[i].Add(reader.ReadByte());
				resourceSize[i].Add(reader.ReadUInt16());
				if (resourceID[i][j] == 2)
				{
					smallFileFullSize[i] = resourceSize[i][j];
					if (chunkIndex[i] == 0)
					{
						resourceSize[i][j] = reader.ReadUInt16();
						resourcePosition += resourceSize[i][j] * 0x800u;
					}
					smallFileBaseOffset[i] = resourcePosition;
					resourcePosition += smallFileFullSize[i] * 0x800u;
				}
				else
				{
					resourcePosition += resourceSize[i][j] * 0x800u;
				}
			}
		}
		for (Int32 i = 0; i < chunkCount; i++)
		{
			if (smallFileFullSize[i] == 0)
				continue;
			reader.BaseStream.Seek(smallFileBaseOffset[i], SeekOrigin.Begin);
			smallFileOffset[i].Add(reader.ReadUInt16());
			smallFileFlags[i].Add(reader.ReadUInt16());
			smallFileIndex[i].Add(0);
			smallFileCount[i] = (UInt32)smallFileOffset[i][0] / 4;
			externalFileCount[i] = 0;
			for (Int32 j = 1; j < smallFileCount[i]; j++)
			{
				UInt16 sfOffset = reader.ReadUInt16();
				UInt16 sfFlags = reader.ReadUInt16();
				if (sfFlags == 0xFFFF || smallFileIndex[i][j - 1] < 0)
				{
					smallFileIndex[i].Add(-1);
					externalFileOffset[i].Add((Int32)sfOffset | ((Int32)sfFlags << 16));
					externalFileCount[i]++;
				}
				else if (sfOffset == smallFileOffset[i][smallFileOffset[i].Count - 1])
				{
					smallFileIndex[i].Add(smallFileIndex[i][j - 1]);
				}
				else
				{
					smallFileIndex[i].Add(smallFileIndex[i][j - 1] + 1);
					smallFileOffset[i].Add(sfOffset);
					smallFileFlags[i].Add(sfFlags);
				}
			}
			smallFileCount[i] -= externalFileCount[i];
			for (Int32 j = 0; j < smallFileOffset[i].Count; j++)
			{
				if (j + 1 < smallFileOffset[i].Count)
					smallFileSize[i].Add((UInt32)smallFileOffset[i][j + 1] - smallFileOffset[i][j]);
				else
					smallFileSize[i].Add(smallFileFullSize[i] * 0x800u - smallFileOffset[i][j]);
				reader.BaseStream.Seek(smallFileBaseOffset[i] + smallFileOffset[i][j], SeekOrigin.Begin);
				smallFileRaw[i].Add(reader.ReadBytes((Int32)smallFileSize[i][j]));
			}
		}
		reader.BaseStream.Seek(0x400, SeekOrigin.Begin);
		Int32 currentChunk = 0;
		do
		{
			SequenceCode seqCode = new SequenceCode((SequenceCode.CODE)reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
			sequence.Add(seqCode);
			if (seqCode.code == SequenceCode.CODE.LOAD_CHUNK)
				currentChunk = seqCode.arg1;
			else if (seqCode.code == SequenceCode.CODE.SETUP_CAMERA && seqCode.arg1 != Byte.MaxValue)
				cameraFileList.Add(new KeyValuePair<Int32, Int32>(currentChunk, seqCode.arg1));
			else if (seqCode.code == SequenceCode.CODE.PLAY_CAMERA && seqCode.arg2 == 0)
				cameraFileList.Add(new KeyValuePair<Int32, Int32>(currentChunk, seqCode.arg1));
			else if (seqCode.code == SequenceCode.CODE.PLAY_SOUND)
				akaoFileList.Add(new KeyValuePair<Int32, Int32>(currentChunk, seqCode.arg1));
		} while (sequence[sequence.Count - 1].code != SequenceCode.CODE.END);


        // DEBUG
        /*if (false)
		{
			String[] EffectPointType = new String[] { "Effect", "Figure" };
			Dictionary<Int32, String> CharacterCode = new Dictionary<Int32, String>{
				{ 0, "FirstTarget" },
				{ 1, "SecondTarget" },
				{ 2, "ThirdTarget" },
				{ 3, "FourthTarget" },
				{ 4, "FifthTarget" },
				{ 5, "SixthTarget" },
				{ 6, "SeventhTarget" },
				{ 7, "EighthTarget" },
				{ 0x10, "Caster" },
				{ 0xFF, "AllTargets" }
			};
			Dictionary<Int32, String> ShowHideCode = new Dictionary<Int32, String>{
				{ 0, "Char=AllPlayers ; Enable=True" },
				{ 1, "Char=AllEnemies ; Enable=True" },
				{ 2, "Char=Everyone ; Enable=True" },
				{ 4, "Char=AllPlayers ; Enable=False" },
				{ 5, "Char=AllEnemies ; Enable=False" },
				{ 6, "Char=Everyone ; Enable=False" },
				{ 9, "Char=Caster ; Enable=True" },
				{ 13, "Char=Caster ; Enable=False" }
			};
			String sequenceString = "";
			List<Int32> randomCameras = new List<Int32>();
			for (Int32 i = 0; i < sequence.Count; i++)
			{
				if (sequence[i].code == SequenceCode.CODE.SETUP_CAMERA && sequence[i].arg1 == 0xFF)
					randomCameras.Clear();
				else if (sequence[i].code == SequenceCode.CODE.SETUP_CAMERA)
					randomCameras.Add(sequence[i].arg1);
				else if (sequence[i].code == SequenceCode.CODE.PLAY_CAMERA && sequence[i].arg2 == 0)
					sequenceString += $"PlayCamera: Camera={sequence[i].arg1}\n";
				else if (sequence[i].code == SequenceCode.CODE.PLAY_CAMERA && sequence[i].arg2 == 3)
					sequenceString += $"PlayCamera: Camera=({String.Join(", ", Array.ConvertAll(randomCameras.ToArray(), c => c.ToString()))}) ; IsAlternate=True\n";
				else if (sequence[i].code == SequenceCode.CODE.PLAY_SOUND && sequence[i].arg2 == 0)
					sequenceString += $"PlaySound: Sound={sequence[i].arg1}\n";
				else if (sequence[i].code == SequenceCode.CODE.WAIT && sequence[i].arg1 == 0)
					sequenceString += $"Wait: Time={sequence[i].arg2}\n";
				else if (sequence[i].code == SequenceCode.CODE.WAIT)
					sequenceString += $"Wait: Time={sequence[i].arg2} ; Type={sequence[i].arg1}\n";
				else if (sequence[i].code == SequenceCode.CODE.EFFECT_POINT)
					sequenceString += $"EffectPoint: Char={CharacterCode[sequence[i].arg1]} ; Type={EffectPointType[sequence[i].arg2]}\n";
				else if (sequence[i].code == SequenceCode.CODE.SET_BATTLE_SCENE_TRANSPARENCY)
					sequenceString += $"SetBackgroundIntensity: Intensity={sequence[i].arg1 / 255f} ; Time={sequence[i].arg1}\n";
				else if (sequence[i].code == SequenceCode.CODE.SHOW_HIDE_CHARACTERS)
					sequenceString += $"ShowMesh: {ShowHideCode[sequence[i].arg2]} ; Time={sequence[i].arg1}\n";
				else if (sequence[i].code == SequenceCode.CODE.PUT_BACK_IN_SLEEP_MODE)
					sequenceString += $"Return\n";
			}
			Directory.CreateDirectory("SpecialEffects/ef" + ((Int32)SFX.currentEffectID).ToString("D3"));
			File.WriteAllText("SpecialEffects/ef" + ((Int32)SFX.currentEffectID).ToString("D3") + "/SequenceBin" + UnifiedBattleSequencer.EXTENSION_SEQ, sequenceString);
			for (Int32 i = 0; i < chunkCount; i++)
			{
				for (Int32 j = 0; j < smallFileCount[i]; j++)
				{
					String typestr = "";
					foreach (KeyValuePair<Int32, Int32> p in cameraFileList)
						if (p.Key == i && p.Value == j)
							typestr += " (camera)";
					foreach (KeyValuePair<Int32, Int32> p in akaoFileList)
						if (p.Key == i && p.Value == j)
							typestr += " (AKAO)";
					String ress = "[SFXBinary] " + i + "-" + j + "-th file" + typestr + "\n";
					if (j > 0 && smallFileIndex[i][j] == smallFileIndex[i][j - 1])
					{
						ress += "Same as above";
					}
					else
					{
						for (Int32 k = 0; k < smallFileSize[i][smallFileIndex[i][j]]; k++)
							ress += smallFileRaw[i][smallFileIndex[i][j]][k].ToString("X2") + (k % 16 == 15 ? "\n" : " ");
					}
					Log.Message(ress);
				}
				for (Int32 j = 0; j < externalFileCount[i]; j++)
				{
					reader.BaseStream.Seek(smallFileBaseOffset[i] + (externalFileOffset[i][j] & 0xFFFF), SeekOrigin.Begin);
					String ress = "[SFXBinary] " + i + "-" + j + "-th external file\n";
					ress += smallFileBaseOffset[i].ToString("X8") + " + " + externalFileOffset[i][j].ToString("X8") + " = " + reader.BaseStream.Position.ToString("X8") + "\n";
					for (Int32 k = 0; k < 32; k++)
						ress += reader.ReadByte().ToString("X2") + (k % 16 == 15 ? "\n" : " ");
					Log.Message(ress);
				}
			}
		}*/
    }

    public class SequenceCode
	{
		public CODE code;
		public Byte arg1;
		public Byte arg2;

		public SequenceCode(CODE c, Byte a1, Byte a2)
		{
			code = c;
			arg1 = a1;
			arg2 = a2;
		}

		public enum CODE : Byte
		{
			END = 0x00,
			WAIT = 0x01,
			PLAY_CASTER_ANIMATION = 0x02,
			//??? = 0x03,
			//??? = 0x04,
			LOAD_CHUNK = 0x05,
			//??? = 0x06,
			//??? = 0x07,
			//??? = 0x08,
			//??? = 0x09,
			//??? = 0x0A,
			//??? = 0x0B,
			//??? = 0x0C,
			CHANGE_MODEL = 0x0D,
			//??? = 0x0E,
			//??? = 0x0F,
			SETUP_CAMERA = 0x23,
			SHOW_HIDE_CHARACTERS = 0x24,
			EFFECT_POINT = 0x25,
			CHANNEL_TOWARD_TARGET = 0x27,
			PUT_BACK_IN_SLEEP_MODE = 0x28,
			PLAY_CAMERA = 0x29,
			SET_BATTLE_SCENE_TRANSPARENCY = 0x2A,
			WAIT_FOR_ANIMATION = 0x2B,
			//??? = 0x2C,
			PLAY_SOUND = 0x2D,
			//??? = 0x2E,
			//??? = 0x50,
			SETUP_DESTINATION = 0x51,
			PLAY_MISS_SOUND = 0x55,
			//??? = 0x57,
			PLAY_CHARACTER_ANIMATION_LOOPING = 0x5A,
			PLAY_CHARACTER_ANIMATION = 0x5B,
			//??? = 0x5C,
			//??? = 0x5E,
			//??? = 0x5F,
			//??? = 0x61,
			FACE_BACK_TO_NORMAL = 0x62,
			PLAY_RAW_ANIMATION = 0x63,
			PLAY_FREYA_CASTING_ANIM = 0x64,
			//??? = 0x65,
			MOVE_BEFORE = 0x69,
			//??? = 0x6A,
			//??? = 0x6B,
			//??? = 0x6C,
			//??? = 0x6D,
			//??? = 0x6E,
			MOVE_CASTER = 0x6F,
			//??? = 0x70,
			//??? = 0x71,
			//??? = 0x72,
			//??? = 0x73,
			//??? = 0x79,
			PLAY_MODEL_ON_TARGET_V1 = 0x80,
			PLAY_MODEL_ON_TARGET_V2 = 0x81,
			PLAY_MODEL_ON_TARGET_V3 = 0x82,
			PLAY_MODEL_ON_TARGET_V4 = 0x83,
			//??? = 0x84,
			//??? = 0x85,
			//??? = 0x86,
			//??? = 0x87,
		};
	}
}
