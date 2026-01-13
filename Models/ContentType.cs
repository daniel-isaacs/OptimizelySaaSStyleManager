using System.Text.Json.Serialization;

namespace OptimizelySaaSStyleManager.Models;

public class ContentType
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("baseType")]
    public string? BaseType { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("sortOrder")]
    public int? SortOrder { get; set; }

    [JsonPropertyName("isAbstract")]
    public bool? IsAbstract { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }
}
