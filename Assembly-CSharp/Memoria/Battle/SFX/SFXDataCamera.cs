using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Memoria;
using Memoria.Prime;
using UnityEngine;

public class SFXDataCamera
{
	// There are at least 3 bugs of the Steam version concerning cameras:
	// 1) For some reason, these btlseq cameras are not always taken into account
	// 2) Some cameras don't focus the correct enemy
	//  (eg.: Ragtime Mouse's opening camera sequence focus on Ragtime instead of True...
	//  Maybe it has to do with the EBin.event_code_binary.NEW3 of the battle's AI script that is ordered differently than enemy types)
	// 3) Camera is slightly off, which is obvious on some effects (Beatrix fights, Madeen's full animation...)
	// Surely, these are bugs of FF9SpecialEffectPlugin.dll, maybe they can't be fixed as long as that DLL is used

	// Using a custom camera system is difficult because FF9SpecialEffectPlugin.dll must be aware of the current camera in order to render SFX correctly

	public Flags flags;
	public Sequence sequence0 = null;
	public Sequence sequence1 = null;
	public Sequence sequence2 = null;
	public Byte[] unknown = null;
	public Int16[] position = null;

	public void Load(BinaryReader reader)
	{
		Int64 basePos = reader.BaseStream.Position;
		flags = (Flags)reader.ReadUInt16();
		UInt16 offOffset = 2;
		if ((flags & Flags.HAS_SEQUENCE_0) != 0)
		{
			reader.BaseStream.Seek(basePos + offOffset, SeekOrigin.Begin);
			UInt16 seqOffset = reader.ReadUInt16();
			reader.BaseStream.Seek(basePos + seqOffset, SeekOrigin.Begin);
			sequence0 = new Sequence();
			sequence0.Read(reader);
			offOffset += 2;
		}
		if ((flags & Flags.HAS_SEQUENCE_1) != 0)
		{
			reader.BaseStream.Seek(basePos + offOffset, SeekOrigin.Begin);
			UInt16 seqOffset = reader.ReadUInt16();
			reader.BaseStream.Seek(basePos + seqOffset, SeekOrigin.Begin);
			sequence1 = new Sequence();
			sequence1.Read(reader);
			offOffset += 2;
		}
		if ((flags & Flags.HAS_SEQUENCE_2) != 0)
		{
			reader.BaseStream.Seek(basePos + offOffset, SeekOrigin.Begin);
			UInt16 seqOffset = reader.ReadUInt16();
			reader.BaseStream.Seek(basePos + seqOffset, SeekOrigin.Begin);
			sequence2 = new Sequence();
			sequence2.Read(reader);
			offOffset += 2;
		}
		if ((flags & Flags.HAS_UNKNOWN) != 0)
		{
			reader.BaseStream.Seek(basePos + offOffset, SeekOrigin.Begin);
			UInt16 unkOffset = reader.ReadUInt16();
			reader.BaseStream.Seek(basePos + unkOffset, SeekOrigin.Begin);
			unknown = new Byte[4];
			unknown[0] = reader.ReadByte();
			unknown[1] = reader.ReadByte();
			unknown[2] = reader.ReadByte();
			unknown[3] = reader.ReadByte();
			offOffset += 2;
		}
		if ((flags & Flags.HAS_CUSTOM_POSITION) != 0)
		{
			reader.BaseStream.Seek(basePos + offOffset, SeekOrigin.Begin);
			UInt16 posOffset = reader.ReadUInt16();
			reader.BaseStream.Seek(basePos + posOffset, SeekOrigin.Begin);
			Int32 cpSize = 3; // There are not always 3 coordinates... sometimes more
			position = new Int16[cpSize];
			for (Int32 i = 0; i < cpSize; i++)
				position[i] = reader.ReadInt16();
		}
	}

	public static List<SFXDataCamera> LoadFromBSC(Byte[] bscBinary)
	{
		// PSX camera datas in BSC files (.raw17) all contain at least 9 camera movements:
		// cameraList[0] are the opening cameras
		// cameraList[1] are the default cameras, mostly empty because the basis for them is a snapshot of the camera taken during opening
		// cameraList[2] are the victory cameras
		// cameraList[3] to [8] seem to be cameras used for enemy attack sequences (many of them following the target setup with SeqExecRunCamera)
		// cameraList[9] and more, optional, are cutscene cameras, played directly with btl_scrp.SetBattleData(36, X) from AI scripts
		// Each camera may contain up to 3 camera sequences (mostly different openings)
		List<SFXDataCamera> cameraList = new List<SFXDataCamera>();
		using BinaryReader reader = new BinaryReader(new MemoryStream(bscBinary));
		reader.ReadInt16();
		Int16 cameraOffset = reader.ReadInt16();
		reader.BaseStream.Seek(cameraOffset, SeekOrigin.Begin);
		UInt16 firstSetOffset = reader.ReadUInt16();
		Int32 cameraCount = firstSetOffset / 2;
		for (Int32 i = 0; i < cameraCount; i++)
		{
			SFXDataCamera sfxCam = new SFXDataCamera();
			cameraList.Add(sfxCam);
			reader.BaseStream.Seek(cameraOffset + 2 * i, SeekOrigin.Begin);
			UInt16 setOffset = reader.ReadUInt16();
			reader.BaseStream.Seek(cameraOffset + setOffset, SeekOrigin.Begin);
			sfxCam.Load(reader);
		}
		return cameraList;
	}

	public static List<SFXDataCamera> LoadFromSFX(SFXBinaryFile sfxBinary)
	{
		List<SFXDataCamera> cameraList = new List<SFXDataCamera>();
		foreach (KeyValuePair<Int32, Int32> p in sfxBinary.cameraFileList)
		{
			if (sfxBinary.smallFileIndex[p.Key][p.Value] >= 0)
			{
				Log.Message($"[{nameof(SFXDataCamera)}] Load Camera {sfxBinary.smallFileIndex[p.Key][p.Value]}");
				SFXDataCamera sfxCam = new SFXDataCamera();
				cameraList.Add(sfxCam);
				sfxCam.Load(new BinaryReader(new MemoryStream(sfxBinary.smallFileRaw[p.Key][sfxBinary.smallFileIndex[p.Key][p.Value]])));
			}
			else
				Log.Message($"[{nameof(SFXDataCamera)}] Camera is an external file... ({p.Key}, {p.Value})");
		}
		return cameraList;
	}

	// May be used after btlseq.ReadBattleSequence and before SFX.SFX_StartPlungeCamera
	public static void UpdateBSC(List<SFXDataCamera> cameraList, btlseq.btlseqinstance inst)
	{
		Byte[] tmpArray = new Byte[10000];
		using BinaryWriter writer = new BinaryWriter(new MemoryStream(tmpArray));
		UInt16 setOffset = 0x12;
		for (Int32 i = 0; i < cameraList.Count; i++)
		{
			writer.BaseStream.Seek(2 * i, SeekOrigin.Begin);
			writer.Write(setOffset);
			writer.BaseStream.Seek(setOffset, SeekOrigin.Begin);
			writer.Write((UInt16)cameraList[i].flags);
			UInt16 offOffset = 2;
			UInt16 seqOffset = (UInt16)(((cameraList[i].flags & Flags.HAS_SEQUENCE_0) != 0 ? 4 : 2)
									+ ((cameraList[i].flags & Flags.HAS_SEQUENCE_1) != 0 ? 2 : 0)
									+ ((cameraList[i].flags & Flags.HAS_SEQUENCE_2) != 0 ? 2 : 0)
									+ ((cameraList[i].flags & Flags.HAS_UNKNOWN) != 0 ? 2 : 0)
									+ ((cameraList[i].flags & Flags.HAS_CUSTOM_POSITION) != 0 ? 2 : 0));
			if ((cameraList[i].flags & Flags.HAS_SEQUENCE_0) != 0)
			{
				writer.BaseStream.Seek(setOffset + offOffset, SeekOrigin.Begin);
				writer.Write(seqOffset);
				writer.BaseStream.Seek(setOffset + seqOffset, SeekOrigin.Begin);
				cameraList[i].sequence0.Write(writer);
				seqOffset = (UInt16)(writer.BaseStream.Position - setOffset);
				offOffset += 2;
			}
			if ((cameraList[i].flags & Flags.HAS_SEQUENCE_1) != 0)
			{
				writer.BaseStream.Seek(setOffset + offOffset, SeekOrigin.Begin);
				writer.Write(seqOffset);
				writer.BaseStream.Seek(setOffset + seqOffset, SeekOrigin.Begin);
				cameraList[i].sequence1.Write(writer);
				seqOffset = (UInt16)(writer.BaseStream.Position - setOffset);
				offOffset += 2;
			}
			if ((cameraList[i].flags & Flags.HAS_SEQUENCE_2) != 0)
			{
				writer.BaseStream.Seek(setOffset + offOffset, SeekOrigin.Begin);
				writer.Write(seqOffset);
				writer.BaseStream.Seek(setOffset + seqOffset, SeekOrigin.Begin);
				cameraList[i].sequence2.Write(writer);
				seqOffset = (UInt16)(writer.BaseStream.Position - setOffset);
				offOffset += 2;
			}
			if ((cameraList[i].flags & Flags.HAS_UNKNOWN) != 0)
			{
				writer.BaseStream.Seek(setOffset + offOffset, SeekOrigin.Begin);
				writer.Write(seqOffset);
				writer.BaseStream.Seek(setOffset + seqOffset, SeekOrigin.Begin);
				writer.Write(cameraList[i].unknown[0]);
				writer.Write(cameraList[i].unknown[1]);
				writer.Write(cameraList[i].unknown[2]);
				writer.Write(cameraList[i].unknown[3]);
				seqOffset = (UInt16)(writer.BaseStream.Position - setOffset);
				offOffset += 2;
			}
			if ((cameraList[i].flags & Flags.HAS_CUSTOM_POSITION) != 0)
			{
				writer.BaseStream.Seek(setOffset + offOffset, SeekOrigin.Begin);
				writer.Write(seqOffset);
				writer.BaseStream.Seek(setOffset + seqOffset, SeekOrigin.Begin);
				for (Int32 j = 0; j < cameraList[i].position.Length; j++)
					writer.Write(cameraList[i].position[j]);
				seqOffset = (UInt16)(writer.BaseStream.Position - setOffset);
				offOffset += 2;
			}
			setOffset = (UInt16)writer.BaseStream.Position;
		}
		Byte[] newData = new Byte[inst.camOffset + setOffset];
		Array.Copy(inst.data, 0, newData, 0, inst.camOffset);
		Array.Copy(tmpArray, 0, newData, inst.camOffset, setOffset);
		inst.data = newData;
	}

	public static SFXDataCamera LoadFromJSON(String path)
	{
		// TODO
		return null;
	}

	public class Sequence
	{
		public List<Code> code;

		public void Read(BinaryReader reader)
		{
			code = new List<Code>();
			while (true)
			{
				Code c = new Code();
				code.Add(c);
				c.frame = reader.ReadUInt16();
				if (c.frame == 0)
				{
					c.flags = 0;
					return;
				}
				c.flags = (CodeFlags)reader.ReadUInt16();
				if ((c.flags & CodeFlags.HAS_CAMERA_POSITION) != 0)
				{
					c.cameraPosition = new Code.Position();
					c.cameraPosition.code = reader.ReadByte();
					c.cameraPosition.flags = reader.ReadByte();
					c.cameraPosition.pitch = reader.ReadByte();
					c.cameraPosition.orientation = reader.ReadByte();
					c.cameraPosition.roll = reader.ReadByte();
					c.cameraPosition.distance = reader.ReadByte();
				}
				if ((c.flags & CodeFlags.HAS_CAMERA_MOVEMENT) != 0)
				{
					c.cameraMovement = new Code.Movement();
					c.cameraMovement.duration = reader.ReadUInt16();
					c.cameraMovement.type = reader.ReadByte();
					c.cameraMovement.unknown = reader.ReadByte();
				}
				if ((c.flags & CodeFlags.HAS_UNKNOWN_1) != 0)
				{
					Log.Message($"[{nameof(Code)}] Found the flag HAS_UNKNOWN_1, camera data reading aborted");
					return;
				}
				if ((c.flags & CodeFlags.HAS_TARGET_POSITION) != 0)
				{
					c.targetPosition = new Code.Position();
					c.targetPosition.code = reader.ReadByte();
					c.targetPosition.flags = reader.ReadByte();
					c.targetPosition.pitch = reader.ReadByte();
					c.targetPosition.orientation = reader.ReadByte();
					c.targetPosition.roll = reader.ReadByte();
					c.targetPosition.distance = reader.ReadByte();
				}
				if ((c.flags & CodeFlags.HAS_TARGET_MOVEMENT) != 0)
				{
					c.targetMovement = new Code.Movement();
					c.targetMovement.duration = reader.ReadUInt16();
					c.targetMovement.type = reader.ReadByte();
					c.targetMovement.unknown = reader.ReadByte();
				}
				if ((c.flags & CodeFlags.HAS_UNKNOWN_2) != 0)
				{
					Log.Message($"[{nameof(Code)}] Found the flag HAS_UNKNOWN_2, camera data reading aborted");
					return;
				}
				if ((c.flags & CodeFlags.HAS_SIGNING_VALUE) != 0)
				{
					c.signing = new Code.Signing();
					c.signing.value = reader.ReadByte();
					c.signing.unknown = reader.ReadByte();
				}
				if ((c.flags & CodeFlags.HAS_UNKNOWN_3) != 0)
				{
					c.unknown3 = reader.ReadUInt16();
				}
				if ((c.flags & CodeFlags.HAS_UNKNOWN_4) != 0)
				{
					c.unknown4 = reader.ReadUInt16();
				}
				if ((c.flags & CodeFlags.HAS_FOCAL_POINT) != 0)
				{
					c.focal = new Code.Focal();
					c.focal.duration = reader.ReadByte();
					c.focal.flags = reader.ReadByte();
					c.focal.distance = reader.ReadUInt16();
				}
				if ((c.flags & CodeFlags.HAS_UNKNOWN_5) != 0)
				{
					c.unknown5 = reader.ReadUInt32();
				}
				if ((c.flags & CodeFlags.HAS_SETTING_CHANGE) != 0)
				{
					c.variable = new Code.Variable();
					c.variable.type = reader.ReadByte();
					c.variable.unknown = reader.ReadByte();
				}
				if ((c.flags & CodeFlags.HAS_UNKNOWN_6) != 0)
				{
					c.unknown6 = reader.ReadUInt32();
				}
			}
		}

		public void Write(BinaryWriter writer)
		{
			for (Int32 i = 0; i < code.Count; i++)
			{
				Code c = code[i];
				if ((c.flags & CodeFlags.HAS_UNKNOWN_1) != 0 || (c.flags & CodeFlags.HAS_UNKNOWN_2) != 0)
				{
					Log.Message($"[{nameof(Code)}] Flag {c.flags} used... camera sequence aborted before that");
					c.frame = 0;
				}
				writer.Write(c.frame);
				if (c.frame == 0)
					return;
				writer.Write((UInt16)c.flags);
				if ((c.flags & CodeFlags.HAS_CAMERA_POSITION) != 0)
				{
					writer.Write(c.cameraPosition.code);
					writer.Write(c.cameraPosition.flags);
					writer.Write(c.cameraPosition.pitch);
					writer.Write(c.cameraPosition.orientation);
					writer.Write(c.cameraPosition.roll);
					writer.Write(c.cameraPosition.distance);
				}
				if ((c.flags & CodeFlags.HAS_CAMERA_MOVEMENT) != 0)
				{
					writer.Write(c.cameraMovement.duration);
					writer.Write(c.cameraMovement.type);
					writer.Write(c.cameraMovement.unknown);
				}
				if ((c.flags & CodeFlags.HAS_TARGET_POSITION) != 0)
				{
					writer.Write(c.targetPosition.code);
					writer.Write(c.targetPosition.flags);
					writer.Write(c.targetPosition.pitch);
					writer.Write(c.targetPosition.orientation);
					writer.Write(c.targetPosition.roll);
					writer.Write(c.targetPosition.distance);
				}
				if ((c.flags & CodeFlags.HAS_TARGET_MOVEMENT) != 0)
				{
					writer.Write(c.targetMovement.duration);
					writer.Write(c.targetMovement.type);
					writer.Write(c.targetMovement.unknown);
				}
				if ((c.flags & CodeFlags.HAS_SIGNING_VALUE) != 0)
				{
					writer.Write(c.signing.value);
					writer.Write(c.signing.unknown);
				}
				if ((c.flags & CodeFlags.HAS_UNKNOWN_3) != 0)
				{
					writer.Write(c.unknown3);
				}
				if ((c.flags & CodeFlags.HAS_UNKNOWN_4) != 0)
				{
					writer.Write(c.unknown4);
				}
				if ((c.flags & CodeFlags.HAS_FOCAL_POINT) != 0)
				{
					writer.Write(c.focal.duration);
					writer.Write(c.focal.flags);
					writer.Write(c.focal.distance);
				}
				if ((c.flags & CodeFlags.HAS_UNKNOWN_5) != 0)
				{
					writer.Write(c.unknown5);
				}
				if ((c.flags & CodeFlags.HAS_SETTING_CHANGE) != 0)
				{
					writer.Write(c.variable.type);
					writer.Write(c.variable.unknown);
				}
				if ((c.flags & CodeFlags.HAS_UNKNOWN_6) != 0)
				{
					writer.Write(c.unknown6);
				}
			}
		}

		public class Code
		{
			public UInt16 frame;
			public CodeFlags flags;
			public Position cameraPosition = null;
			public Movement cameraMovement = null;
			public Position targetPosition = null;
			public Movement targetMovement = null;
			public Signing signing = null;
			public Focal focal = null;
			public Variable variable = null;
			public UInt16 unknown3; // Possible values: 0, 0x0091, 0x00F1 (0 seems to always follow the others, like a re-init)
			public UInt16 unknown4; // Possible values: 0, 0x0013, 0x0022, 0x0023
			public UInt32 unknown5; // Possible values: 0x02021919, 0x020C1408
			public UInt32 unknown6; // Seems to be 0x00000021 almost everytime, 0x00020F61 is also possible

			public class Position
			{
				public Byte code;
				public Byte flags; // Surely, a movement following a target uses flags here
				public Byte pitch; // pitch angle: [0, 0x80] for [0 degree, 360 degrees]
				public Byte orientation; // orientation angle: [0, 0x40] for [0 degree, 360 degrees]
				public Byte roll; // roll angle: [0, 0x80] for [0 degree, 360 degrees]
				public Byte distance; // 1 unit here is about 63 units in the usual coordinate system
										// Considering the target position is 0 (distance = 0), the set doesn't use custom position (or the code ignores it, like 0x0B) and camera position is large enough:
										// angles = (0, 0, 0) => camera is placed behind party members, facing the enemies
										// angles = (0x20, 0, 0) => camera is watching from below
										// angles = (0x60, 0, 0) => camera is watching from above
										// angles = (0, 0x10, 0) => camera is watching from the left (wrt. the players)
										// angles = (0, 0, 0x20) => camera is placed behind party members, rolled 90 degrees to the left, making the 1st player character be at top of screen
										// Target position has the same kind of spherical coordinates but the rolling angle is less important or ignored
			}

			public class Movement
			{
				public UInt16 duration;
				public Byte type; // 0: Linear, 1: SinusIn (start slowly), 2: SinusOut (ends slowly)
				public Byte unknown;
			}

			public class Signing // unknown, but seems to have some effect on movement (un)signed operations
			{
				public Byte value;
				public Byte unknown;
			}

			public class Focal
			{
				public Byte duration;
				public Byte flags; // 2: apply
				public UInt16 distance;
			}

			public class Variable
			{
				public Byte type; // 1: same as SFX.SetCameraPhase(1), 2: call a btl_seq++ code (SFX.COMMAND.COMMAND_INCREMENT_BATTLE_SEQ)
				public Byte unknown;
			}
		}
	}

	[Flags]
	public enum Flags : UInt16
	{
		HAS_SEQUENCE_0 = 1,
		HAS_SEQUENCE_1 = 2,
		HAS_SEQUENCE_2 = 4,
		HAS_UNKNOWN = 8,
		HAS_CUSTOM_POSITION_A = 0x10,
		HAS_CUSTOM_POSITION_B = 0x20,
		HAS_CUSTOM_POSITION_C = 0x40,
		HAS_CUSTOM_POSITION_D = 0x80,
		UNKNOWN_FLAG_1 = 0x100,
		UNKNOWN_FLAG_2 = 0x200,
		UNKNOWN_FLAG_3 = 0x400,
		UNKNOWN_FLAG_4 = 0x800,
		UNKNOWN_FLAG_5 = 0x1000,
		UNKNOWN_FLAG_6 = 0x2000,
		UNKNOWN_FLAG_7 = 0x4000,
		UNKNOWN_FLAG_8 = 0x8000,
		HAS_CUSTOM_POSITION = HAS_CUSTOM_POSITION_A | HAS_CUSTOM_POSITION_B | HAS_CUSTOM_POSITION_C | HAS_CUSTOM_POSITION_D
	};

	[Flags]
	public enum CodeFlags : UInt16
	{
		HAS_CAMERA_POSITION_BIT = 1,
		HAS_CAMERA_MOVEMENT = 2, // Automatically turn on HAS_CAMERA_POSITION but makes start of movement somehow different if used without it
		HAS_CAMERA_POSITION = HAS_CAMERA_POSITION_BIT | HAS_CAMERA_MOVEMENT,
		HAS_UNKNOWN_1 = 4, // Unused but enables a block of data of unknown size
		HAS_TARGET_POSITION_BIT = 8,
		HAS_TARGET_MOVEMENT = 0x10, // Same as HAS_CAMERA_MOVEMENT
		HAS_TARGET_POSITION = HAS_TARGET_POSITION_BIT | HAS_TARGET_MOVEMENT,
		HAS_UNKNOWN_2 = 0x20, // Same as HAS_UNKNOWN_1
		HAS_SIGNING_VALUE = 0x40,
		SAVE_FOR_FIXED = 0x80, // Save the current camera/target positions for using it as a basis of the battle normal camera
		UNKNOWN_FLAG_1 = 0x100,
		HAS_UNKNOWN_3 = 0x200, // 2 bytes
		HAS_UNKNOWN_4 = 0x400, // 2 bytes
		HAS_FOCAL_POINT = 0x800,
		HAS_UNKNOWN_5 = 0x1000, // 4 bytes
		UNKNOWN_FLAG_2 = 0x2000,
		HAS_SETTING_CHANGE = 0x4000,
		HAS_UNKNOWN_6 = 0x8000 // 4 bytes
	};

	public enum CameraEngine
	{
		NONE,
		SFX_PLUGIN,
		SFX_DATA_CAMERA,
		SFX_PLUGIN_SAVE
	};

	public static List<SFXDataCamera> currentCamera = new List<SFXDataCamera>();
	public static Queue<Single[]> pluginSaveCamera = new Queue<Single[]>();
	private static CameraEngine _currentCameraEngine;
	public static CameraEngine currentCameraEngine
	{
		get => _currentCameraEngine;
		set
		{
			if (!Configuration.Battle.SFXRework)
				value = CameraEngine.SFX_PLUGIN;
			_currentCameraEngine = value;
			if (_currentCameraEngine != CameraEngine.SFX_PLUGIN_SAVE)
				pluginSaveCamera.Clear();
		}
	}

	public static void RunCamera(SFXDataCamera cam)
	{
		if (cam == null)
			return;
		SFXDataCamera.currentCamera.Add(cam);
		SFXDataCamera.currentCameraEngine = CameraEngine.SFX_DATA_CAMERA;
	}

	public static void SavePluginCamera()
	{
		if (SFX.isSystemRun && !FF9StateSystem.Battle.isTutorial && !SFX.isDebugCam)
		{
			Int32 isDebug = (!SFX.isDebugMode) ? 0 : 1;
			IntPtr source = SFX.SFX_UpdateCamera(isDebug);
			Single[] array = new Single[13];
			Marshal.Copy(source, array, 0, 13);
			SFXDataCamera.pluginSaveCamera.Enqueue(array);
		}
	}

	public static void UpdateCamera()
	{
		if (SFXDataCamera.currentCameraEngine == CameraEngine.NONE)
			return;
		if (SFXDataCamera.currentCameraEngine == CameraEngine.SFX_PLUGIN)
		{
			SFX.UpdateCamera();
		}
		else if (SFXDataCamera.currentCameraEngine == CameraEngine.SFX_DATA_CAMERA)
		{
			// TODO
			SFXDataCamera.currentCamera.Clear();
			SFXDataCamera.currentCameraEngine = CameraEngine.NONE;
		}
		else if (SFXDataCamera.currentCameraEngine == CameraEngine.SFX_PLUGIN_SAVE)
		{
			if (SFXDataCamera.pluginSaveCamera.Count == 0)
			{
				SFXDataCamera.currentCameraEngine = CameraEngine.NONE;
				return;
			}
			Single[] array = SFXDataCamera.pluginSaveCamera.Dequeue();
			Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
			SFX.fxNearZ = array[12];
			SFX.fxFarZ = 65535f;
			camera.nearClipPlane = SFX.fxNearZ;
			camera.farClipPlane = SFX.fxFarZ;
			camera.worldToCameraMatrix = PsxCamera.PsxMatrix2UnityMatrix(array, SFX.cameraOffset);
			camera.projectionMatrix = PsxCamera.PsxProj2UnityProj(SFX.fxNearZ, SFX.fxFarZ);
		}
	}
}
