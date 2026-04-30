using Microsoft.AspNetCore.Localization;

namespace HindawiFoundation.Web.Localization;

public class CustomRouteDataCultureProvider : RequestCultureProvider
{
    private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase) { "en", "ar" };
    private const string DefaultCulture = "en";

    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var path = httpContext.Request.Path.Value ?? string.Empty;
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        var uiCulture = DefaultCulture;
        if (segments.Length > 0 && Supported.Contains(segments[0]))
        {
            uiCulture = segments[0].ToLowerInvariant();
        }

        // Keep format culture ("en") for date/number parsing so Arabic locale
        // does not break DateTime.Parse or decimal.TryParse in the pipeline.
        // The UI culture drives string resource selection (JsonStringLocalizer
        // uses CurrentUICulture, not CurrentCulture).
        return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult("en", uiCulture));
    }
}
