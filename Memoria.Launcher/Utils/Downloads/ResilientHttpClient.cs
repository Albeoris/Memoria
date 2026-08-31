using Ae.Dns.Client;
using Ae.Dns.Protocol;
using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Memoria.Launcher.Utils.Downloads
{
    internal static class ResilientHttpClient
    {
        private const String DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:151.0) Gecko/20100101 Firefox/151.0";
        private const String SystemResolver = "system-dns";
        private const String FallbackResolver = "doh-racer(cloudflare+google)";

        private static readonly Logger Log = AppLogger.GetLogger(nameof(ResilientHttpClient));
        private static readonly Uri CloudflareDohEndpoint = new Uri("https://cloudflare-dns.com/");
        private static readonly Uri GoogleDohEndpoint = new Uri("https://dns.google/");
        private static readonly Lazy<HttpClient> SharedClient = new Lazy<HttpClient>(CreatePrimaryClient, isThreadSafe: true);
        private static readonly Lazy<HttpClient> SharedFallbackClient = new Lazy<HttpClient>(CreateDohFallbackClient, isThreadSafe: true);
        private static readonly ConditionalWeakTable<HttpClient, HttpClient> FallbackClients = new ConditionalWeakTable<HttpClient, HttpClient>();

        public static HttpClient Shared
        {
            get
            {
                return SharedClient.Value;
            }
        }

        public static HttpClient CreateClient()
        {
            return CreatePrimaryClient();
        }

        public static void DisposeClient(HttpClient client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (SharedClient.IsValueCreated && ReferenceEquals(client, SharedClient.Value))
                throw new ArgumentException("The shared HTTP client cannot be disposed.", nameof(client));

            if (FallbackClients.TryGetValue(client, out HttpClient fallbackClient))
            {
                FallbackClients.Remove(client);
                fallbackClient.Dispose();
            }
            client.Dispose();
        }

        public static async Task<HttpResponseMessage> GetAsync(
            HttpClient primaryClient,
            Uri uri,
            HttpCompletionOption completionOption,
            CancellationToken cancellationToken)
        {
            if (primaryClient == null)
                throw new ArgumentNullException(nameof(primaryClient));
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            Log.Info("HTTP GET {Uri} via {Resolver}", uri, SystemResolver);
            try
            {
                HttpResponseMessage response = await primaryClient.GetAsync(uri, completionOption, cancellationToken).ConfigureAwait(false);
                LogResponse(response, uri, SystemResolver);
                return response;
            }
            catch (Exception exception) when (ShouldRetryWithDoh(exception, cancellationToken))
            {
                Log.Warn(exception, "HTTP GET failed for {Uri} via {Resolver}; retrying via {FallbackResolver}", uri, SystemResolver, FallbackResolver);
                HttpClient fallbackClient = GetFallbackClient(primaryClient);
                HttpResponseMessage response = await fallbackClient.GetAsync(uri, completionOption, cancellationToken).ConfigureAwait(false);
                LogResponse(response, uri, FallbackResolver);
                return response;
            }
        }

        public static async Task<HttpResponseMessage> SendAsync(
            HttpClient primaryClient,
            HttpMethod method,
            Uri uri,
            HttpCompletionOption completionOption,
            CancellationToken cancellationToken)
        {
            if (primaryClient == null)
                throw new ArgumentNullException(nameof(primaryClient));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            try
            {
                return await SendAndLogAsync(primaryClient, method, uri, completionOption, cancellationToken, SystemResolver).ConfigureAwait(false);
            }
            catch (Exception exception) when (ShouldRetryWithDoh(exception, cancellationToken))
            {
                Log.Warn(exception, "HTTP {Method} failed for {Uri} via {Resolver}; retrying via {FallbackResolver}", method, uri, SystemResolver, FallbackResolver);
                HttpClient fallbackClient = GetFallbackClient(primaryClient);
                return await SendAndLogAsync(fallbackClient, method, uri, completionOption, cancellationToken, FallbackResolver).ConfigureAwait(false);
            }
        }

        private static HttpClient CreatePrimaryClient()
        {
            HttpClient client = new HttpClient(CreateDefaultHandler(), disposeHandler: true);
            ApplyDefaultHeaders(client);
            return client;
        }

        private static HttpClient CreateDohFallbackClient()
        {
            HttpClient cloudflareTransport = new HttpClient { BaseAddress = CloudflareDohEndpoint };
            HttpClient googleTransport = new HttpClient { BaseAddress = GoogleDohEndpoint };
            IDnsClient dnsClient = new DnsRacerClient(new DnsHttpClient(cloudflareTransport), new DnsHttpClient(googleTransport));
            DnsDelegatingHandler dnsHandler = new DnsDelegatingHandler(dnsClient, internetProtocolV4: true)
            {
                InnerHandler = CreateDefaultHandler()
            };
            HttpClient client = new HttpClient(dnsHandler, disposeHandler: true);
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

        private static Boolean ShouldRetryWithDoh(Exception exception, CancellationToken cancellationToken)
        {
            return !cancellationToken.IsCancellationRequested && !(exception is OperationCanceledException);
        }

        private static HttpClient GetFallbackClient(HttpClient primaryClient)
        {
            if (ReferenceEquals(primaryClient, SharedClient.Value))
                return SharedFallbackClient.Value;

            return FallbackClients.GetValue(primaryClient, _ => CreateDohFallbackClient());
        }

        private static async Task<HttpResponseMessage> SendAndLogAsync(HttpClient client, HttpMethod method, Uri uri, HttpCompletionOption completionOption, CancellationToken cancellationToken, String resolver)
        {
            using HttpRequestMessage request = new HttpRequestMessage(method, uri);
            HttpResponseMessage response = await client.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);
            LogResponse(response, uri, resolver);
            return response;
        }

        private static void LogResponse(HttpResponseMessage response, Uri uri, String resolver)
        {
            Int32 statusCode = (Int32)response.StatusCode;
            if (statusCode >= 400)
            {
                Log.Warn("HTTP {StatusCode} ({Reason}) for {Uri} via {Resolver}", statusCode, response.ReasonPhrase, uri, resolver);
                return;
            }

            Log.Info("HTTP {StatusCode} for {Uri} via {Resolver}; Content-Length: {ContentLength}",
                statusCode,
                uri,
                resolver,
                response.Content.Headers.ContentLength ?? -1);
        }
    }
}
