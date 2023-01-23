using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace VaucherPracticeService.SharedTool
{
    [ExcludeFromCodeCoverage]
    public static class HttpClientFactoryCustom
    {
        private static readonly ConcurrentDictionary<string, HttpClient> _httpClientCache = new ConcurrentDictionary<string, HttpClient>();

        /// <summary>
        /// Gets an existing HttpClient for the specified URL if exists or creates a new HttpClient with the specified parameters
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="timeout"></param>
        /// <param name="socketConnectionTimeoutMinutes"></param>
        /// <param name="defaultAcceptHeader"></param>
        /// <param name="defaultAuthenticationHeader"></param>
        /// <param name="defaultHeaders"></param>
        /// <returns></returns>
        public static HttpClient GetOrCreateHttpClient(Uri uri, TimeSpan? timeout, int socketConnectionTimeoutMinutes = 1, MediaTypeWithQualityHeaderValue defaultAcceptHeader = null, AuthenticationHeaderValue defaultAuthenticationHeader = null, params (string headerName, string headerValue)[] defaultHeaders)
        {
            var key = $"{uri.Scheme}://{uri.DnsSafeHost}:{uri.Port}";

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            return _httpClientCache.GetOrAdd(key, k =>
            {
                var client = new HttpClient(clientHandler)
                {
                    BaseAddress = uri,
                    Timeout = timeout ?? Timeout.InfiniteTimeSpan
                };

                if (defaultAcceptHeader != null)
                    client.DefaultRequestHeaders.Accept.Add(defaultAcceptHeader);

                if (defaultAuthenticationHeader != null)
                    client.DefaultRequestHeaders.Authorization = defaultAuthenticationHeader;

                foreach (var (headerName, headerValue) in defaultHeaders)
                {
                    client.DefaultRequestHeaders.Add(headerName, headerValue);
                }

                ServicePointManager.FindServicePoint(uri).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(socketConnectionTimeoutMinutes).TotalMilliseconds;

                return client;
            });
        }

        /// <summary>
        /// Tries to get HttpClient associated with the Url.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool TryGetHttpClient(Uri uri, out HttpClient client)
        {
            var key = $"{uri.Scheme}://{uri.DnsSafeHost}:{uri.Port}";
            return _httpClientCache.TryGetValue(key, out client);
        }
    }
}
