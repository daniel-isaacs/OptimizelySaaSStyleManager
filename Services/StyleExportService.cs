using System.Text.Json;
using OptimizelySaaSStyleManager.Models;

namespace OptimizelySaaSStyleManager.Services;

public class StyleExportService : IStyleExportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public string ExportToJson(IEnumerable<DisplayTemplate> styles)
    {
        return JsonSerializer.Serialize(styles, JsonOptions);
    }

    public string ExportToJson(DisplayTemplate style)
    {
        return JsonSerializer.Serialize(style, JsonOptions);
    }

    public List<DisplayTemplate> ImportFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<DisplayTemplate>();
        }

        try
        {
            var trimmedJson = json.Trim();

            if (trimmedJson.StartsWith('['))
            {
                return JsonSerializer.Deserialize<List<DisplayTemplate>>(json, JsonOptions)
                    ?? new List<DisplayTemplate>();
            }

            var single = JsonSerializer.Deserialize<DisplayTemplate>(json, JsonOptions);
            return single != null ? new List<DisplayTemplate> { single } : new List<DisplayTemplate>();
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("Invalid JSON format. Expected a style object or array of styles.");
        }
    }

    public DisplayTemplate? ImportSingleFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<DisplayTemplate>(json, JsonOptions);
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("Invalid JSON format. Expected a single style object.");
        }
    }
}
