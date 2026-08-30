using System.Net;
using System.Net.Http.Headers;
using Memoria.Launcher.Utils.Downloads;
using Xunit;

namespace Memoria.Launcher.Tests.Downloads;

public sealed class RemoteFileNameResolverTests
{
    [Fact]
    public void Resolve_prefers_content_disposition_from_the_redirected_response()
    {
        Uri source = new("https://drive.example/download?id=42");
        using HttpResponseMessage response = CreateResponse("https://cdn.example/content/42");
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileNameStar = "Freya Game+ v0.5.rar"
        };

        String fileName = RemoteFileNameResolver.Resolve(response, source);

        Assert.Equal("Freya Game+ v0.5.rar", fileName);
    }

    [Fact]
    public void Resolve_uses_the_original_file_url_when_the_redirect_target_has_no_extension()
    {
        Uri source = new("https://mods.example/releases/Freya.Game.Plus.rar");
        using HttpResponseMessage response = CreateResponse("https://cdn.example/download/12345");

        String fileName = RemoteFileNameResolver.Resolve(response, source);

        Assert.Equal("Freya.Game.Plus.rar", fileName);
    }

    [Fact]
    public void Resolve_reduces_a_legacy_content_disposition_path_to_a_safe_file_name()
    {
        Uri source = new("https://mods.example/download");
        using HttpResponseMessage response = CreateResponse("https://cdn.example/content");
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileName = "\"../../outside/archive.zip\""
        };

        String fileName = RemoteFileNameResolver.Resolve(response, source);

        Assert.Equal("archive.zip", fileName);
    }

    private static HttpResponseMessage CreateResponse(String effectiveUrl) => new(HttpStatusCode.OK)
    {
        Content = new ByteArrayContent([]),
        RequestMessage = new HttpRequestMessage(HttpMethod.Get, effectiveUrl)
    };
}
