using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OptimizelySaaSStyleManager.Models.Requests;

public class CreateStyleRequest
{
    [Required(ErrorMessage = "Key is required")]
    [StringLength(100, ErrorMessage = "Key must be less than 100 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\-_]+$", ErrorMessage = "Key can only contain letters, numbers, hyphens, and underscores")]
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required")]
    [StringLength(200, ErrorMessage = "Display name must be less than 200 characters")]
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }

    [JsonPropertyName("_baseType")]
    public string? BaseType { get; set; }

    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; set; }

    [JsonPropertyName("settings")]
    public Dictionary<string, DisplayTemplateSetting>? Settings { get; set; }
}
