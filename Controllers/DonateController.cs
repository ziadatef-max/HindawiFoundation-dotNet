using HindawiFoundation.Web.Models;
using HindawiFoundation.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HindawiFoundation.Web.Controllers;

[Route("{culture}/donate")]
public class DonateController : Controller
{
    private readonly IDonationService _donationService;
    private readonly AppSettings _appSettings;
    private readonly ILogger<DonateController> _logger;

    public DonateController(
        IDonationService donationService,
        IOptions<AppSettings> appSettings,
        ILogger<DonateController> logger)
    {
        _donationService = donationService;
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    private static string NormalizeCulture(string? culture) =>
        string.Equals(culture, "ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en";

    private void SetCommonViewData(string culture)
    {
        ViewData["Culture"] = NormalizeCulture(culture);
        ViewData["ActivePage"] = "donate";
        ViewData["TitleKey"] = "donate_page_title";
    }

    private static readonly HashSet<string> SupportedCultures =
        new(StringComparer.OrdinalIgnoreCase) { "en", "ar" };

    private static bool IsSupportedCulture(string? culture) =>
        !string.IsNullOrEmpty(culture) && SupportedCultures.Contains(culture);

    [HttpGet("")]
    public async Task<IActionResult> Index([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();

        SetCommonViewData(culture);

        var clientToken = await _donationService.GetClientToken();

        var model = new DonationViewModel
        {
            Frequency = "monthly",
            CurrencyCode = "USD",
            ClientToken = clientToken,
            IsValid = !string.IsNullOrWhiteSpace(clientToken)
        };

        if (string.IsNullOrWhiteSpace(clientToken))
        {
            ModelState.AddModelError(string.Empty, "The donation form is temporarily unavailable. Please try again later.");
        }

        return View("~/Views/Home/Donate.cshtml", model);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromRoute] string culture, DonationViewModel model)
    {
        if (!IsSupportedCulture(culture)) return NotFound();

        SetCommonViewData(culture);

        if (!ModelState.IsValid)
        {
            model.ClientToken = await _donationService.GetClientToken();
            model.IsValid = false;
            return View("~/Views/Home/Donate.cshtml", model);
        }

        var success = await _donationService.Donate(model, culture);

        if (!success)
        {
            _logger.LogWarning("Donation submission failed for culture {Culture}.", culture);
            model.ClientToken = await _donationService.GetClientToken();
            model.IsValid = false;
            return View("~/Views/Home/Donate.cshtml", model);
        }

        return View("~/Views/Donate/donation_success.cshtml");
    }

    [HttpGet("~/{culture}/donate-unsubscribe")]
    public IActionResult Unsubscribe([FromRoute] string culture)
    {
        if (!IsSupportedCulture(culture)) return NotFound();
        ViewData["Culture"] = NormalizeCulture(culture);
        ViewData["ActivePage"] = "donate";
        return View("~/Views/Donate/donation_unsubscribe.cshtml");
    }

    [HttpGet("tiers")]
    public IActionResult Tiers([FromRoute] string culture, [FromQuery] string? currencyCode = "USD", [FromQuery] decimal? exchangeRate = 1)
    {
        var tiers = new List<object>
        {
            new { id = "translation-champion", label = "Translation Champion", amount = 100 },
            new { id = "translation-hero", label = "Translation Hero", amount = 500 },
            new { id = "literacy-leader", label = "Literacy Leader", amount = 1000 },
            new { id = "book-supporter", label = "Book Supporter", amount = 1000 },
            new { id = "series-superhero", label = "Series SuperHero", amount = 10000 },
            new { id = "library-builder", label = "Library Builder", amount = 2500 }
        };

        if (exchangeRate.HasValue && exchangeRate > 0)
        {
            tiers = tiers.Select(t => new
            {
                id = ((dynamic)t).id,
                label = ((dynamic)t).label,
                amount = Math.Round(((dynamic)t).amount * exchangeRate.Value, 2)
            }).Cast<object>().ToList();
        }

        return Json(new { tiers, currencyCode });
    }

    [HttpGet("currencies")]
    public async Task<IActionResult> Currencies()
    {
        var currencies = await _donationService.GetCurrencies();
        return Json(currencies);
    }

    [HttpGet("default-currency")]
    public async Task<IActionResult> DefaultCurrency([FromQuery] string? countryCode = "US")
    {
        var currency = await _donationService.GetDefaultCurrency(countryCode ?? "US");
        return Json(new { currencyCode = currency });
    }
}
