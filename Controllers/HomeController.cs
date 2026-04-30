using Microsoft.AspNetCore.Mvc;

namespace HindawiFoundation.Web.Controllers;

[Route("{culture}")]
public class HomeController : Controller
{
    private void SetCommonViewData(string culture, string activePage, string titleKey)
    {
        var safe = string.Equals(culture, "ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en";
        ViewData["Culture"] = safe;
        ViewData["ActivePage"] = activePage;
        ViewData["TitleKey"] = titleKey;
    }

    private static string NormalizeCulture(string? culture) =>
        string.Equals(culture, "ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en";

    private static readonly HashSet<string> SupportedCultures =
        new(StringComparer.OrdinalIgnoreCase) { "en", "ar" };

    private static bool IsSupportedCulture(string? culture) =>
        !string.IsNullOrEmpty(culture) && SupportedCultures.Contains(culture);

    private IActionResult ViewForCulture(string baseName, string culture) =>
        View($"{baseName}_{NormalizeCulture(culture)}");

    [HttpGet("")]
    [HttpGet("Home")]
    public IActionResult Index([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        SetCommonViewData(culture, "home", "home_page_title");
        return ViewForCulture("Index", culture);
    }

    [HttpGet("About")]
    public IActionResult About([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        SetCommonViewData(culture, "about", "about_page_title");
        return ViewForCulture("About", culture);
    }

    [HttpGet("Partners")]
    public IActionResult Partners([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        SetCommonViewData(culture, "partners", "partners_page_title");
        return ViewForCulture("Partners", culture);
    }

    [HttpGet("News")]
    public IActionResult News([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        SetCommonViewData(culture, "news", "news_page_title");
        return ViewForCulture("News", culture);
    }

    [HttpGet("News/Details")]
    [HttpGet("News/Details/{id?}")]
    [HttpGet("NewsDetails")]
    [HttpGet("NewsDetails/{id?}")]
    public IActionResult NewsDetails([FromRoute] string culture, int? id = null)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        SetCommonViewData(culture, "news", "news_details_page_title");
        return ViewForCulture("NewsDetails", culture);
    }

    [HttpGet("Contact")]
    public IActionResult Contact([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        SetCommonViewData(culture, "contact", "contact_page_title");
        return ViewForCulture("Contact", culture);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
