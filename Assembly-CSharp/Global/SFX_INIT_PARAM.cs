using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public struct SFX_INIT_PARAM
{
	public const Int32 S_REQ_TRGMAX = 8;

	public const Int32 S_REQ_RTRGMAX = 4;

	private unsafe fixed Byte pad0 [2];

	public Byte btl_scene_FixedCamera1;

	public Byte btl_scene_FixedCamera2;

	public Int16 btl_scene_patAddr_putX;

	public Int16 btl_scene_patAddr_putZ;

	public BTL_DATA_INIT btl_data_init0;

	public BTL_DATA_INIT btl_data_init1;

	public BTL_DATA_INIT btl_data_init2;

	public BTL_DATA_INIT btl_data_init3;

	public BTL_DATA_INIT btl_data_init4;

	public BTL_DATA_INIT btl_data_init5;

	public BTL_DATA_INIT btl_data_init6;

	public BTL_DATA_INIT btl_data_init7;

    public unsafe fixed Int16 camMat[9];

	public unsafe fixed Int32 camTrans[3];

	public Int16 camProj;

	private Int16 pad1;
}
