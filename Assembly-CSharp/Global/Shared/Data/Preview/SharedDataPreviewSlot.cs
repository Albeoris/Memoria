using System;
using System.Collections.Generic;

public class SharedDataPreviewSlot
{
    public const Int32 Size = 961;

    public const Int32 SaveCount = 15;

    public const Int32 RESERVED_DATA_COUNT = 64;

    public Boolean IsPreviewCorrupted;

    public Boolean HasData;

    public Int64 Gil;

    public UInt64 PlayDuration;

    public UInt64 win_type;

    public String Location;

    public List<SharedDataPreviewCharacterInfo> CharacterInfoList;

    public Double Timestamp;

    public Int32[] ReservedData = new Int32[64];
}
