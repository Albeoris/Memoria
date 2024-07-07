using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XInputDotNetPure;

public class vib
{
    public static VIB_TRACK_DEF VIB_TRACK_PTR(VIB_DEF vibPtr, Int32 ndx, BinaryReader reader = null)
    {
        if (reader != null)
        {
            reader.BaseStream.Seek((Int64)((Int32)vibPtr.trackOffset + ndx), SeekOrigin.Begin);
        }
        return vib.Parms.tracks[ndx];
    }

    public static void VIB_TRACK_HEAD(VIB_DEF vibPtr, Int32 ndx, BinaryReader reader = null)
    {
        vib.VIB_TRACK_PTR(vibPtr, 0, reader);
    }

    public static void LoadVibData(String vibrationFileName)
    {
        vib.Parms.vibPtr = null;
        if (vib.vibList.Contains(vibrationFileName))
        {
            Byte[] binAsset = AssetManager.LoadBytes("CommonAsset/VibrationData/" + vibrationFileName, true);
            MemoryStream memoryStream = new MemoryStream(binAsset);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            vib.VIB_init(binaryReader);
            binaryReader.Close();
            memoryStream.Close();
        }
    }

    public static void VIB_init(BinaryReader reader)
    {
        try
        {
            VIB_DEF value = default(VIB_DEF);
            value.magic = (UInt16)reader.ReadInt16();
            value.fps = (UInt16)reader.ReadInt16();
            value.trackCount = (UInt16)reader.ReadInt16();
            value.trackOffset = (UInt16)reader.ReadInt16();
            Int32 trackCount = (Int32)value.trackCount;
            VIB_TRACK_DEF[] array = new VIB_TRACK_DEF[trackCount];
            for (Int32 i = 0; i < trackCount; i++)
            {
                reader.BaseStream.Seek((Int64)((Int32)value.trackOffset + i * 12), SeekOrigin.Begin);
                array[i] = default(VIB_TRACK_DEF);
                array[i].sampleFlags = new UInt16[2];
                array[i].sampleCount = new UInt16[2];
                array[i].sampleOffset = new UInt16[2];
                array[i].sampleFlags[0] = (UInt16)reader.ReadInt16();
                array[i].sampleFlags[1] = (UInt16)reader.ReadInt16();
                array[i].sampleCount[0] = (UInt16)reader.ReadInt16();
                array[i].sampleCount[1] = (UInt16)reader.ReadInt16();
                array[i].sampleOffset[0] = (UInt16)reader.ReadInt16();
                array[i].sampleOffset[1] = (UInt16)reader.ReadInt16();
            }
            Byte[][][] array2 = new Byte[trackCount][][];
            for (Int32 j = 0; j < trackCount; j++)
            {
                VIB_TRACK_DEF vib_TRACK_DEF = array[j];
                array2[j] = new Byte[2][];
                Int32 count = (Int32)vib_TRACK_DEF.sampleCount[(Int32)vib.VIB_SAMPLE_LO];
                Int32 num = (Int32)vib_TRACK_DEF.sampleOffset[(Int32)vib.VIB_SAMPLE_LO];
                reader.BaseStream.Seek((Int64)num, SeekOrigin.Begin);
                array2[j][0] = reader.ReadBytes(count);
                count = (Int32)vib_TRACK_DEF.sampleCount[(Int32)vib.VIB_SAMPLE_HI];
                num = (Int32)vib_TRACK_DEF.sampleOffset[(Int32)vib.VIB_SAMPLE_HI];
                reader.BaseStream.Seek((Int64)num, SeekOrigin.Begin);
                array2[j][1] = reader.ReadBytes(count);
            }
            vib.Parms.vibPtr = new VIB_DEF?(value);
            vib.Parms.tracks = array;
            vib.Parms.frameData = array2;
        }
        catch (Exception message)
        {
            global::Debug.LogError(message);
        }
        finally
        {
            reader.Close();
        }
        vib.Parms.statusFlags = (UInt16)vib.VIB_STATUS_INIT;
        vib.Parms.frameRate = 256;
        vib.Parms.frameNdx = 0;
        vib.Parms.frameStart = 0;
        UInt32 num2 = vib.VIB_computeFrameCount();
        vib.Parms.frameStop = (Int16)(num2 - 1u);
        vib.VIB_initSamples();
    }

    private static void VIB_initSamples()
    {
        Int32 trackCount = (Int32)vib.Parms.vibPtr.Value.trackCount;
        for (Int32 i = 0; i < trackCount; i++)
        {
            vib.VIB_setTrackActive(i, 0, false);
            vib.VIB_setTrackActive(i, 1, false);
        }
        vib.VIB_setTrackActive(0, 0, true);
        vib.VIB_setTrackActive(0, 1, true);
    }

    private static UInt32 VIB_computeFrameCount()
    {
        UInt32 num = 0u;
        for (Int32 i = 0; i < (Int32)vib.Parms.vibPtr.Value.trackCount; i++)
        {
            VIB_TRACK_DEF vib_TRACK_DEF = vib.Parms.tracks[i];
            if ((UInt32)vib_TRACK_DEF.sampleCount[(Int32)vib.VIB_SAMPLE_LO] > num)
            {
                num = (UInt32)vib_TRACK_DEF.sampleCount[(Int32)vib.VIB_SAMPLE_LO];
            }
            if ((UInt32)vib_TRACK_DEF.sampleCount[(Int32)vib.VIB_SAMPLE_HI] > num)
            {
                num = (UInt32)vib_TRACK_DEF.sampleCount[(Int32)vib.VIB_SAMPLE_HI];
            }
        }
        return num;
    }

    public static void VIB_service()
    {
        if ((vib.Parms.statusFlags & (UInt16)vib.VIB_STATUS_INIT) == 0)
        {
            return;
        }
        if (PersistenSingleton<UIManager>.Instance.IsPause)
        {
            return;
        }
        VIB_DEF? vibPtr = vib.Parms.vibPtr;
        if (vibPtr == null)
        {
            return;
        }
        if ((vib.Parms.statusFlags & (UInt16)vib.VIB_STATUS_ACTIVE) != 0 && (vib.Parms.statusFlags & (UInt16)(vib.VIB_STATUS_LOOP | vib.VIB_STATUS_PLAY_SET)) != 0)
        {
            VIB_DEF value = vib.Parms.vibPtr.Value;
            Int32 num = 0;
            Int32 num2 = 0;
            Int32 num3 = vib.Parms.frameNdx >> 8;
            for (Int32 i = 0; i < (Int32)value.trackCount; i++)
            {
                VIB_TRACK_DEF vib_TRACK_DEF = vib.Parms.tracks[i];
                if ((vib_TRACK_DEF.sampleFlags[(Int32)vib.VIB_SAMPLE_LO] & (UInt16)vib.VIB_SAMPLE_ACTIVE) != 0 && num3 < (Int32)vib_TRACK_DEF.sampleCount[(Int32)vib.VIB_SAMPLE_LO])
                {
                    num += (Int32)vib.Parms.frameData[i][(Int32)vib.VIB_SAMPLE_LO][num3];
                }
                if ((vib_TRACK_DEF.sampleFlags[(Int32)vib.VIB_SAMPLE_HI] & (UInt16)vib.VIB_SAMPLE_ACTIVE) != 0 && num3 < (Int32)vib_TRACK_DEF.sampleCount[(Int32)vib.VIB_SAMPLE_HI])
                {
                    num2 += (Int32)vib.Parms.frameData[i][(Int32)vib.VIB_SAMPLE_HI][num3];
                }
            }
            if (num > 255)
            {
                num = 255;
            }
            if (num2 > 127)
            {
                num2 = 1;
            }
            if ((FF9StateSystem.Settings.cfg.vibe == (UInt64)FF9CFG.FF9CFG_VIBE_ON || PersistenSingleton<FF9StateSystem>.Instance.mode == 5) && (num > 0 || num2 > 0))
            {
                vib.VIB_actuatorSet(0, (Single)num2 / 255f, (Single)num / 255f);
                if (PersistenSingleton<FF9StateSystem>.Instance.mode == 2)
                {
                    vib.VIB_actuatorSet(1, (Single)num2 / 255f, (Single)num / 255f);
                }
            }
            vib.Parms.frameNdx = vib.Parms.frameNdx + (Int32)vib.Parms.frameRate;
            if (vib.Parms.frameNdx >> 8 > (Int32)vib.Parms.frameStop)
            {
                if ((vib.Parms.statusFlags & (UInt16)vib.VIB_STATUS_WRAP) != 0)
                {
                    vib.Parms.frameNdx = (Int32)vib.Parms.frameStop << 8;
                    vib.VIB_setFrameRate((Int16)(-vib.Parms.frameRate));
                }
                else
                {
                    vib.Parms.frameNdx = (Int32)vib.Parms.frameStart << 8;
                }
                vib.Parms.statusFlags = (UInt16)(vib.Parms.statusFlags & (UInt16)(~(UInt16)vib.VIB_STATUS_PLAY_SET));
                if ((vib.Parms.statusFlags & (UInt16)vib.VIB_STATUS_LOOP) == 0)
                {
                    vib.VIB_actuatorReset(0);
                    vib.VIB_actuatorReset(1);
                    vib.Parms.statusFlags = (UInt16)(vib.Parms.statusFlags & (UInt16)(~(UInt16)vib.VIB_STATUS_ACTIVE));
                }
            }
            if (vib.Parms.frameNdx >> 8 < (Int32)vib.Parms.frameStart)
            {
                if ((vib.Parms.statusFlags & (UInt16)vib.VIB_STATUS_WRAP) != 0)
                {
                    vib.Parms.frameNdx = (Int32)vib.Parms.frameStart << 8;
                    vib.VIB_setFrameRate((Int16)(-vib.Parms.frameRate));
                }
                else
                {
                    vib.Parms.frameNdx = (Int32)vib.Parms.frameStop << 8;
                }
                vib.Parms.statusFlags = (UInt16)(vib.Parms.statusFlags & (UInt16)(~(UInt16)vib.VIB_STATUS_PLAY_SET));
                if ((vib.Parms.statusFlags & (UInt16)vib.VIB_STATUS_LOOP) == 0)
                {
                    vib.VIB_actuatorReset(0);
                    vib.VIB_actuatorReset(1);
                    vib.Parms.statusFlags = (UInt16)(vib.Parms.statusFlags & (UInt16)(~(UInt16)vib.VIB_STATUS_ACTIVE));
                }
            }
        }
    }

    public static void VIB_purge()
    {
        vib.VIB_actuatorReset(0);
        vib.VIB_actuatorReset(1);
        vib.Parms.statusFlags = (UInt16)vib.VIB_STATUS_NONE;
        vib.Parms.frameStart = 0;
        vib.Parms.frameStop = 0;
        vib.Parms.vibPtr = null;
    }

    public static void VIB_vibrate(Int16 frameNdx)
    {
        vib.Parms.statusFlags = (UInt16)(vib.Parms.statusFlags | (UInt16)(vib.VIB_STATUS_ACTIVE | vib.VIB_STATUS_PLAY_SET));
        vib.Parms.frameStart = frameNdx;
        vib.Parms.frameNdx = (Int32)((Int16)(frameNdx << 8));
    }

    public static void VIB_setActive(Boolean isActive)
    {
        if (isActive)
        {
            vib.Parms.statusFlags = (UInt16)(vib.Parms.statusFlags | (UInt16)(vib.VIB_STATUS_ACTIVE | vib.VIB_STATUS_PLAY_SET));
            if (vib.Parms.frameNdx >> 8 < (Int32)vib.Parms.frameStart || vib.Parms.frameNdx >> 8 > (Int32)vib.Parms.frameStop)
            {
                vib.Parms.frameNdx = (Int32)vib.Parms.frameStart << 8;
            }
        }
        else
        {
            vib.VIB_actuatorReset(0);
            vib.VIB_actuatorReset(1);
            vib.Parms.statusFlags = (UInt16)(vib.Parms.statusFlags & (UInt16)(~(UInt16)(vib.VIB_STATUS_ACTIVE | vib.VIB_STATUS_PLAY_SET)));
        }
    }

    public static void VIB_setTrackActive(Int32 trackNdx, Int32 sampleNdx, Boolean isActive)
    {
        VIB_TRACK_DEF vib_TRACK_DEF;
        for (Int32 i = 0; i < (Int32)vib.Parms.vibPtr.Value.trackCount; i++)
        {
            vib_TRACK_DEF = vib.Parms.tracks[i];
            UInt16[] sampleFlags = vib_TRACK_DEF.sampleFlags;
            sampleFlags[sampleNdx] = (UInt16)(sampleFlags[sampleNdx] & (UInt16)(~(UInt16)vib.VIB_SAMPLE_ACTIVE));
        }
        vib_TRACK_DEF = vib.VIB_TRACK_PTR(vib.Parms.vibPtr.Value, trackNdx, (BinaryReader)null);
        if (isActive)
        {
            UInt16[] sampleFlags2 = vib_TRACK_DEF.sampleFlags;
            sampleFlags2[sampleNdx] = (UInt16)(sampleFlags2[sampleNdx] | (UInt16)vib.VIB_SAMPLE_ACTIVE);
            vib.VIB_actuatorReset(0);
            vib.VIB_actuatorReset(1);
            vib.Parms.statusFlags = (UInt16)(vib.Parms.statusFlags & (UInt16)(~(UInt16)(vib.VIB_STATUS_ACTIVE | vib.VIB_STATUS_PLAY_SET)));
            vib.Parms.frameNdx = (Int32)vib.Parms.frameStart << 8;
        }
        else
        {
            UInt16[] sampleFlags3 = vib_TRACK_DEF.sampleFlags;
            sampleFlags3[sampleNdx] = (UInt16)(sampleFlags3[sampleNdx] & (UInt16)(~(UInt16)vib.VIB_SAMPLE_ACTIVE));
        }
    }

    public static void VIB_setTrackToModulate(UInt32 trackNdx, UInt32 sampleNdx, Boolean isActive)
    {
        VIB_TRACK_DEF vib_TRACK_DEF = vib.VIB_TRACK_PTR(vib.Parms.vibPtr.Value, (Int32)trackNdx, (BinaryReader)null);
        if (isActive)
        {
            UInt16[] sampleFlags = vib_TRACK_DEF.sampleFlags;
            UIntPtr uintPtr = (UIntPtr)sampleNdx;
            sampleFlags[(Int32)uintPtr] = (UInt16)(sampleFlags[(Int32)uintPtr] | (UInt16)vib.VIB_SAMPLE_ACTIVE);
        }
        else
        {
            UInt16[] sampleFlags2 = vib_TRACK_DEF.sampleFlags;
            UIntPtr uintPtr2 = (UIntPtr)sampleNdx;
            sampleFlags2[(Int32)uintPtr2] = (UInt16)(sampleFlags2[(Int32)uintPtr2] & (UInt16)(~(UInt16)vib.VIB_SAMPLE_ACTIVE));
        }
    }

    public static void VIB_setFrameRate(Int16 frameRate)
    {
        vib.Parms.frameRate = frameRate;
    }

    public static void VIB_setFlags(Int16 flags)
    {
        vib.Parms.statusFlags = (UInt16)(vib.Parms.statusFlags & (UInt16)(~(UInt16)(vib.VIB_STATUS_LOOP | vib.VIB_STATUS_WRAP)));
        vib.Parms.statusFlags = (UInt16)(vib.Parms.statusFlags | (UInt16)(flags & (Int16)(vib.VIB_STATUS_LOOP | vib.VIB_STATUS_WRAP)));
    }

    public static void VIB_setPlayRange(Int16 frameStart, Int16 frameStop)
    {
        if (frameStart >= 0)
        {
            vib.Parms.frameStart = frameStart;
        }
        if (frameStop >= 0)
        {
            vib.Parms.frameStop = frameStop;
        }
    }

    public static UInt32 VIB_getStatusFlags()
    {
        return (UInt32)vib.Parms.statusFlags;
    }

    public static Boolean VIB_getTrackActive(Int32 trackNdx, Int32 sampleNdx)
    {
        return (vib.VIB_TRACK_PTR(vib.Parms.vibPtr.Value, trackNdx, (BinaryReader)null).sampleFlags[sampleNdx] & (UInt16)vib.VIB_SAMPLE_ACTIVE) != 0;
    }

    public static VIB_DEF VIB_getVIBPtr()
    {
        return vib.Parms.vibPtr.Value;
    }

    public static void VIB_getPlayRange(ref UInt32 frameStart, ref UInt32 frameStop)
    {
        frameStart = (UInt32)vib.Parms.frameStart;
        frameStop = (UInt32)vib.Parms.frameStop;
    }

    public static Int16 VIB_getFrameRate()
    {
        return vib.Parms.frameRate;
    }

    public static UInt32 VIB_getFrame()
    {
        return (UInt32)(vib.Parms.frameNdx >> 8);
    }

    public static void VIB_actuatorReset(Int32 index)
    {
        vib.VIB_actuatorSet(index, 0f, 0f);
    }

    public static void VIB_actuatorSet(Int32 index, Single left, Single right)
    {
        vib.CurrentVibrateLeft = left;
        vib.CurrentVibrateRight = right;
        if (global::GamePad.GetState((PlayerIndex)index).IsConnected)
        {
            global::GamePad.SetVibration((PlayerIndex)index, left, right);
        }
    }

    public static Byte VIB_STATUS_NONE = 0;

    public static Byte VIB_STATUS_INIT = 1;

    public static Byte VIB_STATUS_ACTIVE = 2;

    public static Byte VIB_STATUS_PLAY_SET = 4;

    public static Byte VIB_STATUS_LOOP = 8;

    public static Byte VIB_STATUS_WRAP = 16;

    public static Byte VIB_SAMPLE_LO = 0;

    public static Byte VIB_SAMPLE_HI = 1;

    public static Byte VIB_SAMPLE_ACTIVE = 1;

    public static Single CurrentVibrateLeft = 0f;

    public static Single CurrentVibrateRight = 0f;

    private static VIB_PARMS_DEF Parms;

    private static readonly List<String> vibList = new List<String>
    {
        "EVT_ALEX1_AC_H2F",
        "EVT_ALEX1_AT_ROOF",
        "EVT_ALEX1_AT_SENTOU",
        "EVT_ALEX1_AT_STREET_A",
        "EVT_ALEX1_TS_CARGO_0",
        "EVT_ALEX1_TS_COCKPIT",
        "EVT_ALEX1_TS_ENGIN",
        "EVT_ALEX1_TS_ORCHE",
        "EVT_ALEX2_AC_PRISON_A",
        "EVT_ALEX2_AC_QUEEN",
        "EVT_ALEX2_AC_R_HALL",
        "EVT_ALEX2_AC_S_ROOM",
        "EVT_ALEX3_AC_ENT_2F",
        "EVT_ALEX3_AT_PUB",
        "EVT_ALEX4_AC_HILDA2_A",
        "EVT_ALEX4_AC_SAI_E",
        "EVT_ALEX4_AC_SAI_F",
        "EVT_ALEX4_AC_SAI_G",
        "EVT_BAL_BB_FGT_0",
        "EVT_BAL_BB_SQR_0",
        "EVT_BURMECIA_HOUSE_1",
        "EVT_BURMECIA_PATIO_1",
        "EVT_CARGO_CA_COP_1",
        "EVT_CARGO_CA_DCK_0",
        "EVT_CARGO_CA_DCK_W",
        "EVT_CARGO_FN_DCK_0",
        "EVT_CLEYRA2_CATHE_2",
        "EVT_DALI_A_AP_AIR_0",
        "EVT_DALI_F_UF_BLK_1",
        "EVT_DOWNSHIP_BT_CCR_0",
        "EVT_EFOREST1_EF_DEP_0",
        "EVT_EVA1_IF_ENT_0",
        "EVT_EVA1_IF_MKJ_1",
        "EVT_EVA2_IU_SDV_0",
        "EVT_EVA2_IU_SDV_1",
        "EVT_FOSSIL_FR_DN3_0",
        "EVT_FOSSIL_FR_ENT_0",
        "EVT_FOSSIL_FR_TRP_0",
        "EVT_GARGAN_GR_GGT_0",
        "EVT_GARGAN_GR_GGT_1",
        "EVT_GATE_S_SG_ALX_4",
        "EVT_GATE_S_SG_TRN_0",
        "EVT_GATE_S_SG_TRN_2",
        "EVT_GIZ_BELL_1",
        "EVT_ICE_DL_VIW_0",
        "EVT_ICE_IC_STA_0",
        "EVT_INV_RR_BRG_1",
        "EVT_IPSEN_IP_ST1_0",
        "EVT_LIND1_CS_LB_CTM_0",
        "EVT_LIND1_CS_LB_EXB_0",
        "EVT_LIND1_CS_LB_LFT_0",
        "EVT_LIND1_CS_LB_LLP_0",
        "EVT_LIND1_CS_LB_MET_0",
        "EVT_LIND1_CS_LB_PLA_0",
        "EVT_LIND1_TN_LB_CAG_0",
        "EVT_LIND1_TN_LB_FTM_0",
        "EVT_LIND1_TN_LB_MTM_0",
        "EVT_LIND1_TN_LB_STN_0",
        "EVT_LIND2_CS_LB_CTM_0",
        "EVT_LIND2_CS_LB_EXB_0",
        "EVT_LIND2_CS_LB_LLP_0",
        "EVT_LIND2_CS_LB_PLA_0",
        "EVT_LIND2_TN_LB_MTM_0",
        "EVT_LIND2_TN_LB_STN_0",
        "EVT_LIND3_CS_LB_CTM_0",
        "EVT_LIND3_CS_LB_EXB_0",
        "EVT_LIND3_CS_LB_LLP_0",
        "EVT_LIND3_CS_LB_PLA_0",
        "EVT_LIND3_TN_LB_MTM_0",
        "EVT_LIND3_TN_LB_STN_0",
        "EVT_OEIL_UV_FDP_0",
        "EVT_PANDEMO_PD_AOS_0",
        "EVT_PANDEMO_PD_BWR_0",
        "EVT_PANDEMO_PD_RTB_0",
        "EVT_PANDEMO_PD_RTB_1",
        "EVT_PATA_M_CM_MP0_0",
        "EVT_PATA_M_CM_MP5_0",
        "EVT_SARI1_MS_MRR_2",
        "EVT_SARI1_MS_PMR_0",
        "EVT_SHRINE_ES_TRP_0",
        "EVT_SUNA_SP_KDR_0",
        "EVT_SUNA_SP_RFG_0",
        "EVT_SUNA_SP_RRO_0",
        "EVT_TRENO1_TR_KHM_0",
        "EVT_TRENO2_TR_KHM_0",
        "EVT_TRENO2_TR_KHM_1",
        "EVT_TRENO2_TR_KHM_2"
    };
}
