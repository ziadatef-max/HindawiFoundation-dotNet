using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HindawiFoundation.Web.Localization;

public class JsonStringLocalizer : IStringLocalizer
{
    private readonly IDistributedCache _cache;
    private readonly ILogger _logger;
    private readonly string _resourcesPath;
    private const string DefaultCulture = "en";

    public JsonStringLocalizer(IDistributedCache cache, ILogger logger, string resourcesPath)
    {
        _cache = cache;
        _logger = logger;
        _resourcesPath = resourcesPath;
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = GetString(name);
            var value = format == null ? name : string.Format(format, arguments);
            return new LocalizedString(name, value, resourceNotFound: format == null);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var culture = CultureInfo.CurrentUICulture.Name;
        var dict = LoadCulture(culture) ?? LoadCulture(DefaultCulture);
        if (dict == null) yield break;
        foreach (var kv in dict)
        {
            yield return new LocalizedString(kv.Key, kv.Value, resourceNotFound: false);
        }
    }

    private string? GetString(string key)
    {
        var culture = CultureInfo.CurrentUICulture.Name;
        var primary = LoadCulture(culture);
        if (primary != null && primary.TryGetValue(key, out var v) && !string.IsNullOrEmpty(v))
            return v;

        if (!string.Equals(culture, DefaultCulture, StringComparison.OrdinalIgnoreCase))
        {
            var fallback = LoadCulture(DefaultCulture);
            if (fallback != null && fallback.TryGetValue(key, out var fv) && !string.IsNullOrEmpty(fv))
                return fv;
        }

        _logger.LogWarning("Localization key not found: {Key} (culture: {Culture})", key, culture);
        return null;
    }

    private Dictionary<string, string>? LoadCulture(string culture)
    {
        var cacheKey = $"locale:{culture}";
        var cached = _cache.GetString(cacheKey);
        if (cached != null)
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, string>>(cached);
            }
            catch
            {
                // fall through and reload
            }
        }

        var path = Path.Combine(_resourcesPath, $"{culture}.json");
        if (!File.Exists(path)) return null;

        try
        {
            var json = File.ReadAllText(path);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (dict != null)
            {
                _cache.SetString(cacheKey, json, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }
            return dict;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load resource file for culture {Culture}", culture);
            return null;
        }
    }
}
