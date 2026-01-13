using System.Text.Json.Serialization;

namespace OptimizelySaaSStyleManager.Models;

public class StyleListResponse
{
    [JsonPropertyName("items")]
    public List<DisplayTemplate> Items { get; set; } = new();

    [JsonPropertyName("pageIndex")]
    public int PageIndex { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
}
