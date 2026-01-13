using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using OptimizelySaaSStyleManager.Configuration;

namespace OptimizelySaaSStyleManager.Services;

public class TokenService : ITokenService
{
    private readonly HttpClient _httpClient;
    private readonly OptimizelyApiSettings _settings;
    private readonly ILogger<TokenService> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    // Buffer before actual expiry to ensure we don't use an about-to-expire token
    private static readonly TimeSpan ExpiryBuffer = TimeSpan.FromSeconds(30);

    public TokenService(
        HttpClient httpClient,
        IOptions<OptimizelyApiSettings> settings,
        ILogger<TokenService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        // Check if we have a valid cached token
        if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
        {
            return _cachedToken;
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check after acquiring lock
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
            {
                return _cachedToken;
            }

            _logger.LogInformation("Requesting new OAuth token from {TokenUrl}", _settings.TokenUrl);

            // Use form-encoded data (standard OAuth format)
            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync(_settings.TokenUrl, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to obtain OAuth token. Status: {StatusCode}, Response: {Response}",
                    response.StatusCode, errorContent);
                throw new InvalidOperationException($"Failed to obtain OAuth token: {response.StatusCode}");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("Invalid token response received");
            }

            _cachedToken = tokenResponse.AccessToken;
            // Token expires in 300 seconds (5 minutes), subtract buffer
            _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn) - ExpiryBuffer;

            _logger.LogInformation("Successfully obtained OAuth token, expires at {Expiry}", _tokenExpiry);

            return _cachedToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
