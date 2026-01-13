namespace OptimizelySaaSStyleManager.Services;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
