using OptimizelySaaSStyleManager.Models;
using OptimizelySaaSStyleManager.Models.Requests;

namespace OptimizelySaaSStyleManager.Services;

public interface IStyleService
{
    Task<StyleListResponse> GetStylesAsync(int pageIndex = 0, int pageSize = 50);

    Task<List<DisplayTemplate>> GetAllStylesAsync();

    Task<DisplayTemplate?> GetStyleAsync(string key);

    Task<DisplayTemplate> CreateStyleAsync(CreateStyleRequest request);

    Task<DisplayTemplate> UpdateStyleAsync(string key, UpdateStyleRequest request);

    Task<DisplayTemplate> ReplaceStyleAsync(string key, CreateStyleRequest request);

    Task DeleteStyleAsync(string key);
}
