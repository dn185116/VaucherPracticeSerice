using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Serilog;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using VaucherPracticeService.Services.VPS.Models;
using VaucherPracticeService.SharedTool;

namespace VaucherPracticeService.Services.VPS
{
    public class VPSApi : IVPSApi
    {
        private readonly ILogger<VPSApi> _logger;
        private readonly IConfiguration _configuration;

        private readonly string _baseUrl;
        private readonly string _username;
        private readonly string _password;
        private readonly int _apsRequestRetryAfterSeconds;
        private readonly int _apsRequestRetryCount;

        private readonly TimeSpan _apsRequestTimeout;
        private readonly TimeSpan _apsRequestTimeoutForCompleteTransaction;

        private static readonly int _socketConnectionTimeoutMinutes = 1;

        private readonly HttpClient _httpClient;

        public VPSApi(ILogger<VPSApi> logger, IConfiguration configuration)
        {
            _logger = logger;

            _configuration = configuration;
            _baseUrl = _configuration["AppSettings:ApsBaseUrl"];
            _username = _configuration["AppSettings:ApsUsername"];
            _password = Utility.Base64Decode(_configuration["AppSettings:ApsPassword"]);
            _apsRequestTimeout = TimeSpan.FromSeconds(int.Parse(_configuration["AppSettings:ApsRequestTimeoutSeconds"]));
            _apsRequestTimeoutForCompleteTransaction = TimeSpan.FromSeconds(int.Parse(_configuration["AppSettings:ApsRequestTimeoutSecondsForCompleteTransaction"]));
            _apsRequestRetryAfterSeconds = int.Parse(_configuration["AppSettings:ApsRequestRetryAfterSeconds"]);
            _apsRequestRetryCount = int.Parse(_configuration["AppSettings:ApsRequestRetryCount"]);

            Uri uri = new Uri(_baseUrl);
            if (!HttpClientFactoryCustom.TryGetHttpClient(uri, out _httpClient))
                _httpClient = HttpClientFactoryCustom.GetOrCreateHttpClient(uri, null, _socketConnectionTimeoutMinutes,
                new MediaTypeWithQualityHeaderValue("application/json"), new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"))));
        }

        public async Task<OutGetTransactionsTO> GetTransactions()
        {
            try
            {
                return await BaseRetryPolicy().ExecuteAsync(async () =>
                {
                    var urlBuilder = new StringBuilder(_baseUrl).Append("/ExternalAPI/PayAtPos/GetTransactions");

                    using var cts = new CancellationTokenSource();
                    cts.CancelAfter(_apsRequestTimeout);

                    using HttpResponseMessage response = await _httpClient.GetAsync(urlBuilder.ToString(), cts.Token);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.LogError("GetTransactions returned: " + (int)response.StatusCode + ", " + await response.Content?.ReadAsStringAsync());
                        return null;
                    }

                    return await JsonSerializer.DeserializeAsync<OutGetTransactionsTO>(await response.Content.ReadAsStreamAsync());
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<bool> CompleteTransaction(InCompleteTransactionTO model)
        {
            try
            {
                return await BaseRetryPolicy().ExecuteAsync(async () =>
                {
                    var urlBuilder = new StringBuilder(_baseUrl).Append("/ExternalAPI/PayAtPos/CompleteTransaction");

                    using var cts = new CancellationTokenSource();
                    cts.CancelAfter(_apsRequestTimeoutForCompleteTransaction);

                    using var content = new StringContent(JsonSerializer.Serialize(model));
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    using HttpResponseMessage response = await _httpClient.PostAsync(urlBuilder.ToString(), content, cts.Token);

                    bool success = response.StatusCode == HttpStatusCode.OK;

                    if (!success)
                        _logger.LogError("CompleteTransaction returned: " + (int)response.StatusCode + ", " + await response.Content?.ReadAsStringAsync());

                    return success;
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }

        public async Task<bool> DisplayMessage(InDisplayMessageTO model)
        {
            try
            {
                return await BaseRetryPolicy().ExecuteAsync(async () =>
                {
                    var urlBuilder = new StringBuilder(_baseUrl).Append("/ExternalAPI/PayAtPos/DisplayMessage");

                    using var cts = new CancellationTokenSource();
                    cts.CancelAfter(_apsRequestTimeout);

                    using var content = new StringContent(JsonSerializer.Serialize(model));
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    using HttpResponseMessage response = await _httpClient.PostAsync(urlBuilder.ToString(), content, cts.Token);

                    bool success = response.StatusCode == HttpStatusCode.OK;

                    if (!success)
                        _logger.LogError("DisplayMessage returned: " + (int)response.StatusCode + ", " + await response.Content?.ReadAsStringAsync());

                    return success;
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }

        public async Task<bool> DisplayConfirmation(InDisplayConfirmationTO model)
        {
            try
            {
                return await BaseRetryPolicy().ExecuteAsync(async () =>
                {
                    var urlBuilder = new StringBuilder(_baseUrl).Append("/ExternalAPI/PayAtPos/DisplayConfirmation");

                    using var cts = new CancellationTokenSource();
                    cts.CancelAfter(_apsRequestTimeout);

                    using var content = new StringContent(JsonSerializer.Serialize(model));
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    using HttpResponseMessage response = await _httpClient.PostAsync(urlBuilder.ToString(), content, cts.Token);

                    bool success = response.StatusCode == HttpStatusCode.OK;

                    if (!success)
                        _logger.LogError("DisplayConfirmation returned: " + (int)response.StatusCode + ", " + await response.Content?.ReadAsStringAsync());

                    return success;
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return false;
            }
        }

        public async Task<OutPromptCashierTO> PromptCashier(InPromptCashierTO model)
        {
            try
            {
                return await BaseRetryPolicy().ExecuteAsync(async () =>
                {
                    var urlBuilder = new StringBuilder(_baseUrl).Append("/ExternalAPI/PayAtPos/PromptCashier");

                    using var cts = new CancellationTokenSource();
                    cts.CancelAfter(_apsRequestTimeout);

                    using var content = new StringContent(JsonSerializer.Serialize(model));
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    using HttpResponseMessage response = await _httpClient.PostAsync(urlBuilder.ToString(), content, cts.Token);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.LogError("PromptCashier returned: " + (int)response.StatusCode + ", " + await response.Content?.ReadAsStringAsync());
                        return null;
                    }

                    return await JsonSerializer.DeserializeAsync<OutPromptCashierTO>(await response.Content.ReadAsStreamAsync());
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        public async Task<OutGetAlphaNumeric> GetAlphaNumeric(InGetAlphaNumericTO model)
        {
            try
            {
                return await BaseRetryPolicy().ExecuteAsync(async () =>
                {
                    var urlBuilder = new StringBuilder(_baseUrl).Append("/ExternalAPI/PayAtPos/GetAlphaNum");

                    using var cts = new CancellationTokenSource();
                    cts.CancelAfter(_apsRequestTimeout);

                    using var content = new StringContent(JsonSerializer.Serialize(model));
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    using HttpResponseMessage response = await _httpClient.PostAsync(urlBuilder.ToString(), content, cts.Token);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.LogError("GetAlphaNumeric returned: " + (int)response.StatusCode + ", " + await response.Content?.ReadAsStringAsync());
                        return null;
                    }

                    return await JsonSerializer.DeserializeAsync<OutGetAlphaNumeric>(await response.Content.ReadAsStreamAsync());
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return null;
            }
        }

        private AsyncRetryPolicy BaseRetryPolicy()
        {
            return Policy.Handle<OperationCanceledException>()
                .WaitAndRetryAsync(_apsRequestRetryCount,
                    sleepDurationProvider: _ => TimeSpan.FromSeconds(_apsRequestRetryAfterSeconds),
                    onRetry: (exception, sleepDuration, attemptNumber, context) =>
                    {
                        _logger.LogError($"Request timeout. Retrying in {sleepDuration}. {attemptNumber} / {_apsRequestRetryCount}. Exception message: {exception?.Message}");
                    });
        }
    }
}