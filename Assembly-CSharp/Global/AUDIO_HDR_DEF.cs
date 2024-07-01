using System;
using System.IO;

public struct AUDIO_HDR_DEF
{
    public void ExtractHeaderData(BinaryReader reader)
    {
        this.magic = reader.ReadUInt16();
        this.sectorType = reader.ReadUInt16();
        this.sectorNdx = reader.ReadUInt16();
        this.sectorsInFrame = reader.ReadUInt16();
        this.frameNdx = reader.ReadUInt32();
        this.frameSize = reader.ReadUInt32();
        this.audioDataPresent = reader.ReadUInt16();
        this.mbgType = reader.ReadUInt16();
        this.mbgCameraA = new MBG_CAM_DEF();
        this.mbgCameraA.ReadBinary(reader);
        this.mbgCameraB = new MBG_CAM_DEF();
        this.mbgCameraB.ReadBinary(reader);
        this.forceFeedbackBits = reader.ReadUInt32();
        this.frameCount = reader.ReadUInt32();
        this.vibrate = new UInt32[2];
        if ((this.forceFeedbackBits & 2147483648u) != 0u)
        {
            if ((this.forceFeedbackBits >> 8 & 255u) >= 128u)
            {
                this.vibrate[0] = 1u;
            }
            else
            {
                this.vibrate[0] = 0u;
            }
            this.vibrate[1] = (this.forceFeedbackBits & 255u);
        }
        else
        {
            this.vibrate[0] = 0u;
            this.vibrate[1] = 0u;
        }
    }

    private UInt16 magic;

    private UInt16 sectorType;

    private UInt16 sectorNdx;

    public UInt16 sectorsInFrame;

    public UInt32 frameNdx;

    private UInt32 frameSize;

    private UInt16 audioDataPresent;

    private UInt16 mbgType;

    public MBG_CAM_DEF mbgCameraA;

    public MBG_CAM_DEF mbgCameraB;

    public UInt32 forceFeedbackBits;

    private UInt32 frameCount;

    public UInt32[] vibrate;
}
