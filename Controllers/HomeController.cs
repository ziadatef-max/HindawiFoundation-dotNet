using HindawiFoundation.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HindawiFoundation.Web.Controllers;

[Route("{culture}")]
public class HomeController : Controller
{
    private static readonly HashSet<string> SupportedCultures =
        new(StringComparer.OrdinalIgnoreCase) { "en", "ar" };

    private static bool IsSupportedCulture(string? culture) =>
        !string.IsNullOrEmpty(culture) && SupportedCultures.Contains(culture);

    private static string NormalizeCulture(string? culture) =>
        string.Equals(culture, "ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en";

    private void SetCommonViewData(string culture, string activePage, string titleKey)
    {
        var safe = NormalizeCulture(culture);
        ViewData["Culture"] = safe;
        ViewData["ActivePage"] = activePage;
        ViewData["TitleKey"] = titleKey;
    }

    private IActionResult ViewForCulture(string baseName, string culture) =>
        View($"{baseName}_{NormalizeCulture(culture)}");

    // ── Home ─────────────────────────────────────────────────────────────────

    /// /en  or  /ar
    [HttpGet("")]
    public IActionResult Index([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        SetCommonViewData(culture, "home", "home_page_title");
        return ViewForCulture("Index", culture);
    }

    /// /en/home or /en/Home → permanent redirect to /en (or /ar)
    [HttpGet("home")]
    public IActionResult HomeRedirect([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        return RedirectPermanent($"/{NormalizeCulture(culture)}");
    }

    // ── About ────────────────────────────────────────────────────────────────

    [HttpGet("about")]
    public IActionResult About([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        SetCommonViewData(culture, "about", "about_page_title");
        return ViewForCulture("About", culture);
    }

    // ── Partners ─────────────────────────────────────────────────────────────

    [HttpGet("partners")]
    public IActionResult Partners([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        SetCommonViewData(culture, "partners", "partners_page_title");
        return ViewForCulture("Partners", culture);
    }

    // ── News listing ─────────────────────────────────────────────────────────

    [HttpGet("news")]
    public IActionResult News([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        SetCommonViewData(culture, "news", "news_page_title");
        return ViewForCulture("News", culture);
    }

    // ── News details  /en/news/{id}  ─────────────────────────────────────────

    [HttpGet("news/{id}")]
    public IActionResult NewsDetails([FromRoute] string culture, [FromRoute] string id)
    {
        if (!IsSupportedCulture(culture)) return NotFound();

        var resolvedId = NewsMap.Resolve(id);

        // Unknown ID → redirect to default article
        if (!string.Equals(id, resolvedId, StringComparison.Ordinal))
            return RedirectPermanent($"/{NormalizeCulture(culture)}/news/{resolvedId}");

        SetCommonViewData(culture, "news", "news_details_page_title");
        ViewData["NewsId"] = resolvedId;
        ViewData["NewsIndex"] = NewsMap.GetIndex(resolvedId);
        return ViewForCulture("NewsDetails", culture);
    }

    // ── Contact ──────────────────────────────────────────────────────────────

    [HttpGet("contact")]
    public IActionResult Contact([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        SetCommonViewData(culture, "contact", "contact_page_title");
        return ViewForCulture("Contact", culture);
    }

    // ── Error ────────────────────────────────────────────────────────────────

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
