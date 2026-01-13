namespace OptimizelySaaSStyleManager.Configuration;

public class OptimizelyApiSettings
{
    public const string SectionName = "OptimizelyApi";

    /// <summary>
    /// Base URL for the Optimizely CMS API (e.g., https://api.cms.optimizely.com/preview3)
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.cms.optimizely.com/preview3";

    /// <summary>
    /// OAuth token endpoint URL
    /// </summary>
    public string TokenUrl { get; set; } = "https://api.cms.optimizely.com/oauth/token";

    /// <summary>
    /// OAuth Client ID from your API key settings in Optimizely
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// OAuth Client Secret from your API key settings in Optimizely
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}
