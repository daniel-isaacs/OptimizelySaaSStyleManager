using OptimizelySaaSStyleManager.Models;

namespace OptimizelySaaSStyleManager.Services;

public interface IStyleExportService
{
    string ExportToJson(IEnumerable<DisplayTemplate> styles);

    string ExportToJson(DisplayTemplate style);

    List<DisplayTemplate> ImportFromJson(string json);

    DisplayTemplate? ImportSingleFromJson(string json);
}
