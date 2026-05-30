using System;
using System.Net;
using System.Net.Http;

namespace Memoria.Launcher
{
    internal static class HttpClients
    {
        private const String DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:151.0) Gecko/20100101 Firefox/151.0";

        private static readonly Lazy<HttpClient> _shared = new Lazy<HttpClient>(CreateShared, isThreadSafe: true);

        public static HttpClient Shared => _shared.Value;

        public static HttpClient CreateDownloadClient()
        {
            HttpClientHandler handler = CreateDefaultHandler();
            HttpClient client = new HttpClient(handler, disposeHandler: true);
            ApplyDefaultHeaders(client);
            return client;
        }

        private static HttpClient CreateShared()
        {
            HttpClientHandler handler = CreateDefaultHandler();
            HttpClient client = new HttpClient(handler, disposeHandler: true);
            ApplyDefaultHeaders(client);
            return client;
        }

        private static HttpClientHandler CreateDefaultHandler()
        {
            return new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
        }

        private static void ApplyDefaultHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd(DefaultUserAgent);
        }
    }
}