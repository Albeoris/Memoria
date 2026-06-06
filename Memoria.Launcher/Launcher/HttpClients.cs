using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ae.Dns.Client;
using Ae.Dns.Protocol;

namespace Memoria.Launcher
{
    internal static class HttpClients
    {
        private static readonly NLog.Logger _log = AppLogger.GetLogger();
        private const String DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:151.0) Gecko/20100101 Firefox/151.0";
        private static readonly Uri _cloudflareDohEndpoint = new Uri("https://cloudflare-dns.com/");
        private static readonly Uri _googleDohEndpoint = new Uri("https://dns.google/");
        private const String ResolverSystem = "system-dns";
        private const String ResolverDohRacer = "doh-racer(cloudflare+google)";

        private static readonly Lazy<HttpClient> _shared = new Lazy<HttpClient>(CreateShared, isThreadSafe: true);
        private static readonly Lazy<HttpClient> _sharedDohFallback = new Lazy<HttpClient>(CreateDohFallbackShared, isThreadSafe: true);
        private static readonly ConditionalWeakTable<HttpClient, HttpClient> _fallbackClients = new ConditionalWeakTable<HttpClient, HttpClient>();

        public static HttpClient Shared => _shared.Value;

        public static HttpClient CreateDownloadClient()
        {
            HttpClientHandler handler = CreateDefaultHandler();
            HttpClient client = new HttpClient(handler, disposeHandler: true);
            ApplyDefaultHeaders(client);
            _fallbackClients.GetValue(client, _ => CreateDohFallbackClient());
            return client;
        }

        private static HttpClient CreateDohFallbackClient()
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
            return CreateDohFallbackClient();
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

        public static async Task<HttpResponseMessage> GetWithDohFallbackAsync(HttpClient primaryClient, Uri uri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            HttpClient fallbackClient = GetFallbackClient(primaryClient);
            _log.Info("HTTP GET {Uri} via {Resolver}", uri, ResolverSystem);
            try
            {
                HttpResponseMessage response = await primaryClient.GetAsync(uri, completionOption, cancellationToken).ConfigureAwait(false);
                LogResponse(response, uri, ResolverSystem);
                return response;
            }
            catch (Exception ex) when (ShouldRetryWithDoh(ex, cancellationToken))
            {
                _log.Warn(ex, "HTTP GET failed for {Uri} via {Resolver}; retrying via {FallbackResolver}", uri, ResolverSystem, ResolverDohRacer);
                try
                {
                    HttpResponseMessage response = await fallbackClient.GetAsync(uri, completionOption, cancellationToken).ConfigureAwait(false);
                    LogResponse(response, uri, ResolverDohRacer);
                    return response;
                }
                catch (Exception fallbackEx)
                {
                    _log.Error(fallbackEx, "HTTP GET failed for {Uri} via {FallbackResolver}", uri, ResolverDohRacer);
                    throw;
                }
            }
        }

        public static async Task<HttpResponseMessage> SendWithDohFallbackAsync(HttpClient primaryClient, HttpMethod method, Uri uri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            HttpClient fallbackClient = GetFallbackClient(primaryClient);
            _log.Info("HTTP {Method} {Uri} via {Resolver}", method, uri, ResolverSystem);
            try
            {
                HttpResponseMessage response = await SendAsync(primaryClient, method, uri, completionOption, cancellationToken).ConfigureAwait(false);
                LogResponse(response, uri, ResolverSystem);
                return response;
            }
            catch (Exception ex) when (ShouldRetryWithDoh(ex, cancellationToken))
            {
                _log.Warn(ex, "HTTP {Method} failed for {Uri} via {Resolver}; retrying via {FallbackResolver}", method, uri, ResolverSystem, ResolverDohRacer);
                try
                {
                    HttpResponseMessage response = await SendAsync(fallbackClient, method, uri, completionOption, cancellationToken).ConfigureAwait(false);
                    LogResponse(response, uri, ResolverDohRacer);
                    return response;
                }
                catch (Exception fallbackEx)
                {
                    _log.Error(fallbackEx, "HTTP {Method} failed for {Uri} via {FallbackResolver}", method, uri, ResolverDohRacer);
                    throw;
                }
            }
        }

        public static async Task<Stream> GetStreamWithDohFallbackAsync(HttpClient primaryClient, Uri uri, CancellationToken cancellationToken)
        {
            HttpClient fallbackClient = GetFallbackClient(primaryClient);
            _log.Info("HTTP GET(stream) {Uri} via {Resolver}", uri, ResolverSystem);
            try
            {
                Stream stream = await primaryClient.GetStreamAsync(uri).ConfigureAwait(false);
                _log.Info("HTTP GET(stream) opened for {Uri} via {Resolver}", uri, ResolverSystem);
                return stream;
            }
            catch (Exception ex) when (ShouldRetryWithDoh(ex, cancellationToken))
            {
                _log.Warn(ex, "HTTP GET(stream) failed for {Uri} via {Resolver}; retrying via {FallbackResolver}", uri, ResolverSystem, ResolverDohRacer);
                try
                {
                    Stream stream = await fallbackClient.GetStreamAsync(uri).ConfigureAwait(false);
                    _log.Info("HTTP GET(stream) opened for {Uri} via {Resolver}", uri, ResolverDohRacer);
                    return stream;
                }
                catch (Exception fallbackEx)
                {
                    _log.Error(fallbackEx, "HTTP GET(stream) failed for {Uri} via {FallbackResolver}", uri, ResolverDohRacer);
                    throw;
                }
            }
        }

        private static async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpMethod method, Uri uri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(method, uri))
                return await client.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);
        }

        private static HttpClient GetFallbackClient(HttpClient primaryClient)
        {
            if (ReferenceEquals(primaryClient, _shared.Value))
                return _sharedDohFallback.Value;

            return _fallbackClients.GetValue(primaryClient, _ => CreateDohFallbackClient());
        }

        private static void LogResponse(HttpResponseMessage response, Uri uri, String resolver)
        {
            Int32 status = (Int32)response.StatusCode;
            if (status >= 400)
            {
                _log.Warn("HTTP {StatusCode} ({StatusDescription}) for {Uri} via {Resolver} - Content-Type: {ContentType}, Content-Length: {ContentLength}",
                    status,
                    response.ReasonPhrase,
                    uri,
                    resolver,
                    response.Content.Headers.ContentType,
                    response.Content.Headers.ContentLength ?? -1);
            }
            else
            {
                _log.Info("HTTP {StatusCode} for {Uri} via {Resolver} - Content-Type: {ContentType}, Content-Length: {ContentLength}",
                    status,
                    uri,
                    resolver,
                    response.Content.Headers.ContentType,
                    response.Content.Headers.ContentLength ?? -1);
            }
        }
    }
}