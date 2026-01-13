using System.Text.Json.Serialization;

namespace OptimizelySaaSStyleManager.Models;

public class DisplayTemplate
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }

    [JsonPropertyName("_baseType")]
    public string? BaseType { get; set; }

    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; set; }

    [JsonPropertyName("settings")]
    public Dictionary<string, DisplayTemplateSetting> Settings { get; set; } = new();
}
