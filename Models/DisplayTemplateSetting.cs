using System.Text.Json.Serialization;

namespace OptimizelySaaSStyleManager.Models;

public class DisplayTemplateSetting
{
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("editor")]
    public string Editor { get; set; } = "string";

    [JsonPropertyName("sortOrder")]
    public int SortOrder { get; set; }

    [JsonPropertyName("choices")]
    public Dictionary<string, SettingChoice>? Choices { get; set; }
}
