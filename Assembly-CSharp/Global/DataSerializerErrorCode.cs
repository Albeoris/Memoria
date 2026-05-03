using System;

public enum DataSerializerErrorCode
{
    Success,
    FileCorruption,
    DataCorruption,
    SaveFailed,
    LoadFailed,
    CloudDataCorruption,
    CloudConnectionTimeout,
    CloudFileNotFound,
    CloudDataUnknownError,
    CloudConnectionError
}
