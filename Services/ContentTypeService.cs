using System.Net.Http.Json;
using System.Text.Json;
using OptimizelySaaSStyleManager.Models;

namespace OptimizelySaaSStyleManager.Services;

public class ContentTypeService : IContentTypeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ContentTypeService> _logger;
    private List<ContentType>? _cachedContentTypes;
    private DateTime _cacheExpiry = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ContentTypeService(HttpClient httpClient, ILogger<ContentTypeService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ContentTypeListResponse> GetContentTypesAsync(int pageIndex = 0, int pageSize = 100)
    {
        var url = $"contenttypes?pageIndex={pageIndex}&pageSize={pageSize}";

        try
        {
            _logger.LogInformation("Fetching content types from {Url}", url);
            var response = await _httpClient.GetAsync(url);

            await EnsureSuccessWithDetailsAsync(response, "GET", url);

            var result = await response.Content.ReadFromJsonAsync<ContentTypeListResponse>(JsonOptions);
            _logger.LogInformation("Successfully fetched {Count} content types", result?.Items?.Count ?? 0);
            return result ?? new ContentTypeListResponse();
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching content types from API");
            throw new ApiException("GET", url, 0, "Failed to fetch content types from Optimizely API", ex.Message);
        }
    }

    public async Task<List<ContentType>> GetAllContentTypesAsync()
    {
        // Return cached data if valid
        if (_cachedContentTypes != null && DateTime.UtcNow < _cacheExpiry)
        {
            _logger.LogInformation("Returning cached content types ({Count} items)", _cachedContentTypes.Count);
            return _cachedContentTypes;
        }

        var allContentTypes = new List<ContentType>();
        var pageIndex = 0;
        const int pageSize = 100;

        while (true)
        {
            var response = await GetContentTypesAsync(pageIndex, pageSize);
            allContentTypes.AddRange(response.Items);

            if (response.Items.Count < pageSize || allContentTypes.Count >= response.TotalCount)
            {
                break;
            }

            pageIndex++;
        }

        // Cache the results
        _cachedContentTypes = allContentTypes;
        _cacheExpiry = DateTime.UtcNow.Add(CacheDuration);

        return allContentTypes;
    }

    public async Task<List<ContentType>> SearchContentTypesAsync(string searchTerm)
    {
        var allContentTypes = await GetAllContentTypesAsync();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return allContentTypes.OrderBy(ct => ct.DisplayName ?? ct.Key).ToList();
        }

        var lowerSearchTerm = searchTerm.ToLowerInvariant();

        return allContentTypes
            .Where(ct =>
                (ct.Key?.ToLowerInvariant().Contains(lowerSearchTerm) ?? false) ||
                (ct.DisplayName?.ToLowerInvariant().Contains(lowerSearchTerm) ?? false) ||
                (ct.Description?.ToLowerInvariant().Contains(lowerSearchTerm) ?? false))
            .OrderBy(ct => ct.DisplayName ?? ct.Key)
            .ToList();
    }

    private async Task EnsureSuccessWithDetailsAsync(HttpResponseMessage response, string method, string url)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var statusCode = (int)response.StatusCode;

        _logger.LogError(
            "API request failed. Method: {Method}, URL: {Url}, Status: {StatusCode} {ReasonPhrase}, Response: {ResponseBody}",
            method, url, statusCode, response.ReasonPhrase, responseBody);

        var errorMessage = response.StatusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => "Authentication failed. Check your API credentials.",
            System.Net.HttpStatusCode.Forbidden => "Access denied. Your API key may not have permission for this operation.",
            System.Net.HttpStatusCode.NotFound => "Resource not found.",
            System.Net.HttpStatusCode.BadRequest => "Invalid request.",
            _ => $"Request failed with status {statusCode}."
        };

        throw new ApiException(method, url, statusCode, errorMessage, responseBody);
    }
}
