using System;

public static class FF9Define
{
	public const Int32 TRUE = 1;

	public const Int32 FALSE = 0;

	public const Int32 NONE = -1;

	public const Byte FF9_MODE_OPENING = 0;

	public const Byte FF9_MODE_FIELD = 1;

	public const Byte FF9_MODE_BATTLE = 2;

	public const Byte FF9_MODE_WORLD = 3;

	public const Byte FF9_MODE_ENDING = 4;

	public const Byte FF9_MODE_FIELDMAP = 5;

	public const Byte FF9_MODE_BATTLEDEBUG = 6;

	public const Byte FF9_MODE_GAMEOVER = 7;

	public const Byte FF9_MODE_BATTLERESULT = 8;

	public const Byte FF9_MODE_MINIGAME = 9;

	public const Byte FF9_MODE_COUNT = 10;

	public const UInt32 FF9_ATTR_SYSTEM_RESET = 1u;

	public const UInt32 FF9_MASK_SYSTEM_RESET = 1u;

	public const UInt32 FF9_ATTR_SYSTEM_EXITFIELD = 2u;

	public const UInt32 FF9_MASK_SYSTEM_EXITFIELD = 3u;

	public const UInt32 FF9_ATTR_SYSTEM_CHANGELOCATION = 4u;

	public const UInt32 FF9_MASK_SYSTEM_EXITLOCATION = 7u;

	public const UInt32 FF9_ATTR_SYSTEM_CHANGEMAP = 8u;

	public const UInt32 FF9_MASK_SYSTEM_EXITMAP = 15u;

	public const UInt32 FF9_ATTR_SYSTEM_EXITBATTLE = 256u;

	public const UInt32 FF9_MASK_SYSTEM_EXITBATTLE = 257u;

	public const UInt32 FF9_ATTR_SYSTEM_CHANGEBATTLEMAP = 512u;

	public const UInt32 FF9_MASK_SYSTEM_EXITBATTLEMAP = 769u;

	public const UInt32 FF9_ATTR_SYSTEM_EXITWORLD = 4096u;

	public const UInt32 FF9_MASK_SYSTEM_EXITWORLD = 4097u;

	public const UInt32 FF9_ATTR_SYSTEM_CHANGEWORLDMAP = 8192u;

	public const UInt32 FF9_MASK_SYSTEM_EXITWORLDMAP = 12289u;

	public const UInt32 FF9_ATTR_SYSTEM_INVALIDSECTORTALE = 1048576u;

	public const UInt32 FF9_ATTR_SYSTEM_DEBUGBATTLE = 2147483648u;

	public const UInt32 FF9_ATTR_FIELD_NOWINDOW = 1u;

	public const UInt32 FF9_ATTR_FIELD_NOBATTLEEFFECT = 4u;

	public const UInt32 FF9_ATTR_FIELD_NOBGI = 8u;

	public const UInt32 FF9_ATTR_FIELD_NOCONTROLLER = 16u;

	public const UInt32 FF9_ATTR_FIELD_NOPUTDISPENV = 32u;

	public const UInt32 FF9_ATTR_FIELD_NOPUTDRAWENV = 64u;

	public const UInt32 FF9_ATTR_FIELD_NOTIME = 128u;

	public const UInt32 FF9_ATTR_FIELD_NOFMV = 256u;

	public const UInt32 FF9_ATTR_FIELD_NOCLEAROTR = 512u;

	public const UInt32 FF9_ATTR_FIELD_NODRAWOTAG = 1024u;

	public const UInt32 FF9_ATTR_FIELD_NOWIPE = 2048u;

	public const UInt32 FF9_ATTR_FIELD_NOHINT = 4096u;

	public const UInt32 FF9_ATTR_FIELD_INVALIDSECTORTALE = 65536u;

	public const UInt32 FF9_ATTR_FIELD_DISCREQUEST = 1048576u;

	public const UInt32 FF9_ATTR_WORLD_INIT = 1u;

	public const UInt32 FF9_ATTR_WORLD_NOMAIN = 256u;

	public const UInt32 FF9_ATTR_WORLD_NOWIPE = 512u;

	public const UInt32 FF9_ATTR_WORLD_NOBATTLEEFFECT = 1024u;

	public const UInt32 FF9_ATTR_WORLD_NOTIME = 2048u;

	public const UInt32 FF9_ATTR_WORLD_NAVIGATIONMAP = 4096u;

	public const UInt32 FF9_ATTR_WORLDMAP_BATTLEEFFECTDONE = 1u;

	public const UInt32 FF9_ATTR_GLOBAL_NOMENU = 1u;

	public const UInt32 FF9_ATTR_GLOBAL_NOEVENT = 2u;

	public const UInt32 FF9_ATTR_GLOBAL_NOCHAR = 4u;

	public const UInt32 FF9_ATTR_GLOBAL_NOBG = 8u;

	public const UInt32 FF9_ATTR_GLOBAL_NOSPS = 16u;

	public const UInt32 FF9_ATTR_GLOBAL_NOIDTOGGLE = 32u;

	public const UInt32 FF9_ATTR_GLOBAL_NOEVENTGRAPHICS = 64u;

	public const UInt32 FF9_ATTR_GLOBAL_PAUSE = 256u;

	public const Int32 FLDINT_DEF_PLAYERID = 0;

	public const Int32 FLDINT_GEOTEXANIM_BLINK = 2;

	public const Int32 FF9_ATTR_CHAR_INITED = 1;

	public const Int32 FF9_ATTR_CHAR_BGI_UPRIGHT = 2;

	public const Int32 FF9_ATTR_CHAR_BGI_LADDER = 4;

	public const Int32 FF9_ATTR_CHAR_SHADOW_HIDE = 16;

	public const Int32 FF9_ATTR_CHAR_SHADOW_ROTFIXED = 32;

	public const Int32 FF9_ATTR_CHAR_SOUND_DISABLE = 64;

	public const Int32 FF9_ATTR_CHAR_LIGHT_INITED = 256;

	public const Int32 FF9_ATTR_CHAR_LIGHT_INPROGRESS = 512;

	public const Int32 FF9_ATTR_CHAR_LIGHT_DISABLE = 1024;

	public const Int32 FF9_ATTR_CHAR_LIGHT_SETDIRECT = 2048;

	public const Int32 FF9_ATTR_CHAR_TEXANIM_NOBLINK = 4096;

	public const Int32 FF9_ATTR_CHAR_ALPHA_ACTIVE = 65536;

	public const Int32 FF9_ATTR_CHAR_ALPHA_INPROGRESS = 131072;

	public const Int32 FF9_ATTR_CHAR_ALPHA_CHANGERATE = 262144;

	public const Int32 FF9_ATTR_CHAR_SLICE_ACTIVE = 1048576;

	public const Int32 FF9_ATTR_CHAR_MIRROR_ACTIVE = 16777216;

	public const Int32 FF9_ATTR_CHAR_MIRROR_COPY = 33554432;

	public const Int32 FF9CHOCO_EVENT_MAP_START = 187;

	public const Int32 FF9CHOCO_EVENT_GEM_START = 184;

	public const Int32 FF9CHOCO_EVENT_KIND_START = 191;

	public const Int32 FF9CHOCO_HINTMAP_MAX = 24;

	public const Int32 FF9CHOCO_HINTMAP_NONE = 0;

	public const Int32 FF9CHOCO_HINTMAP_START = 1;

	public const Int32 keventScriptOffsetSys = 64;

	public const Int32 keventScriptOffsetCho = 128;

	public const Int32 keventScriptOffsetBtl = 192;

	public const Int32 keventNaviLocF0 = 92;

	public const Int32 keventNaviLocF1 = 94;

	public const Int32 keventNaviLocF2 = 96;

	public const Int32 keventNaviLocF3 = 98;

	public const Int32 keventNaviModeNo = 100;

	public const Int32 keventGeneral = 101;

	public const Int32 keventScriptNo = 102;

	public const Int32 keventNaviLoc = 103;

	public const Int32 keventMusicVol = 104;

	public const Int32 keventRideNo = 190;

	public const Int32 keventBattleFlgLW = 194;

	public const Int32 keventBattleFlgHI = 198;

	public const Int32 kframeEventStartLoop = 4;

	public const Int32 keventGlobalScenario = 0;

	public const Int32 FLDSCRPT_EVTNO_ENDING = 16000;

	public const Int32 FF9_ATTR_FIELDMAP_BATTLEEFFECTDONE = 1;
}
