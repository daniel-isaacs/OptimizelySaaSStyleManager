using OptimizelySaaSStyleManager.Models;

namespace OptimizelySaaSStyleManager.Services;

public interface IContentTypeService
{
    Task<ContentTypeListResponse> GetContentTypesAsync(int pageIndex = 0, int pageSize = 100);

    Task<List<ContentType>> GetAllContentTypesAsync();

    Task<List<ContentType>> SearchContentTypesAsync(string searchTerm);
}
