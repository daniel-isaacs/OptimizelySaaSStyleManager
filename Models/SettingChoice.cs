using System.Text.Json.Serialization;

namespace OptimizelySaaSStyleManager.Models;

public class SettingChoice
{
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("sortOrder")]
    public int SortOrder { get; set; }
}
