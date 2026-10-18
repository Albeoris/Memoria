using System.Net;

namespace Memoria.Launcher.Tests.Downloads;

internal sealed record RemoteFileMetadata(
    HttpStatusCode StatusCode,
    String? ReasonPhrase,
    Uri EffectiveUri,
    String? FileName,
    String? Extension,
    String? MediaType,
    Int64? ContentLength);
