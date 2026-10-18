using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Memoria.Launcher.Utils.Downloads;
using Xunit;

namespace Memoria.Launcher.Tests.Downloads;

public sealed class DownloadResponseValidatorTests
{
    private static readonly Uri Source = new("https://www.dropbox.com/download/Mistwake-UI.zip");
    private static readonly Uri EffectiveSource = new("https://www.dropbox.com/removed-file-page");

    [Fact]
    public void ValidateHeaders_rejects_an_html_page_disguised_by_a_zip_url()
    {
        using HttpResponseMessage response = CreateResponse("text/html");

        DownloadException exception = Assert.Throws<DownloadException>(
            () => DownloadResponseValidator.ValidateHeaders(response, Source));

        Assert.Equal(DownloadFailureKind.UnexpectedContent, exception.Kind);
        Assert.Contains(Source.ToString(), exception.Message);
        Assert.Contains(EffectiveSource.ToString(), exception.Message);
        Assert.Contains("text/html", exception.Message);
    }

    [Fact]
    public void ValidatePayloadPrefix_rejects_html_when_the_server_claims_it_is_binary()
    {
        using HttpResponseMessage response = CreateResponse("application/octet-stream");
        Byte[] content = Encoding.UTF8.GetBytes("\uFEFF  \r\n<!DOCTYPE html><html><body>File removed</body></html>");

        DownloadException exception = Assert.Throws<DownloadException>(
            () => DownloadResponseValidator.ValidatePayloadPrefix(content, content.Length, response, Source));

        Assert.Equal(DownloadFailureKind.UnexpectedContent, exception.Kind);
    }

    [Fact]
    public void Validators_accept_a_zip_payload_with_a_generic_content_type()
    {
        using HttpResponseMessage response = CreateResponse("application/octet-stream");
        Byte[] content = [0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x00, 0x00];

        DownloadResponseValidator.ValidateHeaders(response, Source);
        DownloadResponseValidator.ValidatePayloadPrefix(content, content.Length, response, Source);
    }

    private static HttpResponseMessage CreateResponse(String mediaType)
    {
        ByteArrayContent content = new([]);
        content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = content,
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, EffectiveSource)
        };
    }
}
