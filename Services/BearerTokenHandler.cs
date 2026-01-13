using System.Net.Http.Headers;

namespace OptimizelySaaSStyleManager.Services;

public class BearerTokenHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public BearerTokenHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetAccessTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
