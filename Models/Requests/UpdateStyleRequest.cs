using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OptimizelySaaSStyleManager.Models.Requests;

public class UpdateStyleRequest
{
    [StringLength(200, ErrorMessage = "Display name must be less than 200 characters")]
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }

    [JsonPropertyName("_baseType")]
    public string? BaseType { get; set; }

    [JsonPropertyName("isDefault")]
    public bool? IsDefault { get; set; }

    [JsonPropertyName("settings")]
    public Dictionary<string, DisplayTemplateSetting>? Settings { get; set; }
}
