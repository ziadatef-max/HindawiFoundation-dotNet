using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HindawiFoundation.Web.Localization;

public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IDistributedCache _cache;
    private readonly ILoggerFactory _loggerFactory;
    private readonly string _resourcesPath;

    public JsonStringLocalizerFactory(
        IDistributedCache cache,
        ILoggerFactory loggerFactory,
        IWebHostEnvironment env)
    {
        _cache = cache;
        _loggerFactory = loggerFactory;
        _resourcesPath = Path.Combine(env.ContentRootPath, "Resources");
    }

    public IStringLocalizer Create(Type resourceSource) =>
        new JsonStringLocalizer(_cache, _loggerFactory.CreateLogger<JsonStringLocalizer>(), _resourcesPath);

    public IStringLocalizer Create(string baseName, string location) =>
        new JsonStringLocalizer(_cache, _loggerFactory.CreateLogger<JsonStringLocalizer>(), _resourcesPath);
}
