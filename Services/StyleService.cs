using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using OptimizelySaaSStyleManager.Models;
using OptimizelySaaSStyleManager.Models.Requests;

namespace OptimizelySaaSStyleManager.Services;

public class StyleService : IStyleService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StyleService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public StyleService(HttpClient httpClient, ILogger<StyleService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<StyleListResponse> GetStylesAsync(int pageIndex = 0, int pageSize = 50)
    {
        var url = $"displaytemplates?pageIndex={pageIndex}&pageSize={pageSize}";

        try
        {
            _logger.LogInformation("Fetching styles from {Url}", url);
            var response = await _httpClient.GetAsync(url);

            await EnsureSuccessWithDetailsAsync(response, "GET", url);

            var result = await response.Content.ReadFromJsonAsync<StyleListResponse>(JsonOptions);
            _logger.LogInformation("Successfully fetched {Count} styles", result?.Items?.Count ?? 0);
            return result ?? new StyleListResponse();
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching styles from API");
            throw new ApiException("GET", url, 0, "Failed to fetch styles from Optimizely API", ex.Message);
        }
    }

    public async Task<List<DisplayTemplate>> GetAllStylesAsync()
    {
        var allStyles = new List<DisplayTemplate>();
        var pageIndex = 0;
        const int pageSize = 100;

        while (true)
        {
            var response = await GetStylesAsync(pageIndex, pageSize);
            allStyles.AddRange(response.Items);

            if (response.Items.Count < pageSize || allStyles.Count >= response.TotalCount)
            {
                break;
            }

            pageIndex++;
        }

        return allStyles;
    }

    public async Task<DisplayTemplate?> GetStyleAsync(string key)
    {
        var url = $"displaytemplates/{Uri.EscapeDataString(key)}";

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            await EnsureSuccessWithDetailsAsync(response, "GET", url);
            return await response.Content.ReadFromJsonAsync<DisplayTemplate>(JsonOptions);
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching style {Key} from API", key);
            throw new ApiException("GET", url, 0, $"Failed to fetch style '{key}'", ex.Message);
        }
    }

    public async Task<DisplayTemplate> CreateStyleAsync(CreateStyleRequest request)
    {
        var url = "displaytemplates";

        try
        {
            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            await EnsureSuccessWithDetailsAsync(response, "POST", url);

            var result = await response.Content.ReadFromJsonAsync<DisplayTemplate>(JsonOptions);
            return result ?? throw new InvalidOperationException("Failed to parse created style response");
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating style {Key}", request.Key);
            throw new ApiException("POST", url, 0, $"Failed to create style '{request.Key}'", ex.Message);
        }
    }

    public async Task<DisplayTemplate> UpdateStyleAsync(string key, UpdateStyleRequest request)
    {
        var url = $"displaytemplates/{Uri.EscapeDataString(key)}";

        try
        {
            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/merge-patch+json");

            var httpRequest = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(httpRequest);
            await EnsureSuccessWithDetailsAsync(response, "PATCH", url);

            var result = await response.Content.ReadFromJsonAsync<DisplayTemplate>(JsonOptions);
            return result ?? throw new InvalidOperationException("Failed to parse updated style response");
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating style {Key}", key);
            throw new ApiException("PATCH", url, 0, $"Failed to update style '{key}'", ex.Message);
        }
    }

    public async Task<DisplayTemplate> ReplaceStyleAsync(string key, CreateStyleRequest request)
    {
        var url = $"displaytemplates/{Uri.EscapeDataString(key)}";

        try
        {
            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            await EnsureSuccessWithDetailsAsync(response, "PUT", url);

            var result = await response.Content.ReadFromJsonAsync<DisplayTemplate>(JsonOptions);
            return result ?? throw new InvalidOperationException("Failed to parse replaced style response");
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replacing style {Key}", key);
            throw new ApiException("PUT", url, 0, $"Failed to replace style '{key}'", ex.Message);
        }
    }

    public async Task DeleteStyleAsync(string key)
    {
        var url = $"displaytemplates/{Uri.EscapeDataString(key)}";

        try
        {
            var response = await _httpClient.DeleteAsync(url);
            await EnsureSuccessWithDetailsAsync(response, "DELETE", url);
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting style {Key}", key);
            throw new ApiException("DELETE", url, 0, $"Failed to delete style '{key}'", ex.Message);
        }
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

public class ApiException : Exception
{
    public string Method { get; }
    public string Url { get; }
    public int StatusCode { get; }
    public string ResponseBody { get; }

    public ApiException(string method, string url, int statusCode, string message, string responseBody)
        : base(FormatMessage(method, url, statusCode, message, responseBody))
    {
        Method = method;
        Url = url;
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    private static string FormatMessage(string method, string url, int statusCode, string message, string responseBody)
    {
        var sb = new StringBuilder();
        sb.AppendLine(message);
        sb.AppendLine();
        sb.AppendLine($"Request: {method} {url}");

        if (statusCode > 0)
        {
            sb.AppendLine($"Status: {statusCode}");
        }

        if (!string.IsNullOrWhiteSpace(responseBody))
        {
            sb.AppendLine($"Response: {responseBody}");
        }

        return sb.ToString();
    }
}
