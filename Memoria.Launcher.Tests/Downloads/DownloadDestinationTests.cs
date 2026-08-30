using System.Net;
using System.Net.Http.Headers;
using Memoria.Launcher.Tests.Infrastructure;
using Memoria.Launcher.Utils.Downloads;
using Xunit;

namespace Memoria.Launcher.Tests.Downloads;

public sealed class DownloadDestinationTests
{
    [Fact]
    public void File_destination_always_resolves_to_its_initialized_exact_path()
    {
        using TemporaryDirectory directory = new TemporaryDirectory();
        String expectedPath = Path.Combine(directory.FullPath, "catalog.xml");
        DownloadDestination destination = DownloadDestination.ForFile(expectedPath);
        using HttpResponseMessage response = CreateResponse("downloaded-name.rar");

        String resolvedPath = destination.ResolveFilePath(response, new Uri("https://example.test/source"));

        Assert.Equal(expectedPath, destination.DisplayPath);
        Assert.Equal(directory.FullPath, destination.DirectoryPath);
        Assert.Equal(expectedPath, resolvedPath);
    }

    [Fact]
    public void Directory_destination_resolves_the_file_name_from_response_metadata()
    {
        using TemporaryDirectory directory = new TemporaryDirectory();
        DownloadDestination destination = DownloadDestination.InDirectory(directory.FullPath);
        using HttpResponseMessage response = CreateResponse("Freya Game+ v0.5.rar");

        String resolvedPath = destination.ResolveFilePath(response, new Uri("https://example.test/download?id=42"));

        Assert.Equal(Path.Combine(directory.FullPath, "Freya Game+ v0.5.rar"), resolvedPath);
        Assert.Equal(directory.FullPath, destination.DirectoryPath);
    }

    private static HttpResponseMessage CreateResponse(String fileName)
    {
        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent([]),
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://cdn.example.test/content")
        };
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileNameStar = fileName
        };
        return response;
    }
}
