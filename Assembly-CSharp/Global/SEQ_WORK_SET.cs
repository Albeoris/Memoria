using System;
using UnityEngine;

public class SEQ_WORK_SET
{
	public const Int32 SEQ_MAX = 4;

	public Int32[] AnmAddrList;

	public Byte[] AnmOfsList;

	public Int32[] SeqData = new Int32[18];

	public UInt32[] pad0 = new UInt32[6];

	public SEQ_WORK[] SeqWork = new SEQ_WORK[4];

	public BTL_DATA CamExe;

	public BTL_DATA CamTrg;

	public Vector3 CamTrgCPos;

	public Byte CameraNo;
}
