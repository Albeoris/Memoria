using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ae.Dns.Client;
using Ae.Dns.Protocol;

namespace Memoria.Launcher
{
    internal static class HttpClients
    {
        private const String DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:151.0) Gecko/20100101 Firefox/151.0";
        private static readonly Uri _cloudflareDohEndpoint = new Uri("https://cloudflare-dns.com/");
        private static readonly Uri _googleDohEndpoint = new Uri("https://dns.google/");

        private static readonly Lazy<HttpClient> _shared = new Lazy<HttpClient>(CreateShared, isThreadSafe: true);
        private static readonly Lazy<HttpClient> _sharedDohFallback = new Lazy<HttpClient>(CreateDohFallbackShared, isThreadSafe: true);

        public static HttpClient Shared => _shared.Value;
        public static HttpClient SharedDohFallback => _sharedDohFallback.Value;

        public static HttpClient CreateDownloadClient()
        {
            HttpClientHandler handler = CreateDefaultHandler();
            HttpClient client = new HttpClient(handler, disposeHandler: true);
            ApplyDefaultHeaders(client);
            return client;
        }

        public static HttpClient CreateDownloadDohFallbackClient()
        {
            HttpClientHandler httpHandler = CreateDefaultHandler();

            HttpClient cloudflareTransport = new HttpClient { BaseAddress = _cloudflareDohEndpoint };
            HttpClient googleTransport = new HttpClient { BaseAddress = _googleDohEndpoint };

            IDnsClient dnsClient = new DnsRacerClient(
                new DnsHttpClient(cloudflareTransport),
                new DnsHttpClient(googleTransport));

            DnsDelegatingHandler dnsHandler = new DnsDelegatingHandler(dnsClient, internetProtocolV4: true)
            {
                InnerHandler = httpHandler
            };

            HttpClient client = new HttpClient(dnsHandler, disposeHandler: true);
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

        private static HttpClient CreateDohFallbackShared()
        {
            return CreateDownloadDohFallbackClient();
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

        public static bool ShouldRetryWithDoh(Exception exception, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return false;

            return !(exception is OperationCanceledException);
        }

        public static async Task<HttpResponseMessage> GetWithDohFallbackAsync(HttpClient primaryClient, HttpClient fallbackClient, Uri uri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            try
            {
                return await primaryClient.GetAsync(uri, completionOption, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ShouldRetryWithDoh(ex, cancellationToken))
            {
                return await fallbackClient.GetAsync(uri, completionOption, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}