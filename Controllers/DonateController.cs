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
        ViewData["TitleKey"] = "donate_button";
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

        return View("~/Views/Home/Donate.cshtml", model);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([FromRoute] string culture, DonationViewModel model)
    {
        if (!IsSupportedCulture(culture)) return NotFound();

        SetCommonViewData(culture);

        if (!string.IsNullOrWhiteSpace(_appSettings.DonationApi.GoogleRecaptchaKey))
        {
            var recaptchaToken = Request.Form["g-recaptcha-response"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(recaptchaToken))
            {
                model.ClientToken = await _donationService.GetClientToken();
                model.IsValid = false;
                model.ShowRecaptchaError = true;
                return View("~/Views/Home/Donate.cshtml", model);
            }

            var isRecaptchaValid = await _donationService.ValidateRecaptcha(recaptchaToken);
            if (!isRecaptchaValid)
            {
                _logger.LogWarning("reCAPTCHA validation failed for culture {Culture}.", culture);
                model.ClientToken = await _donationService.GetClientToken();
                model.IsValid = false;
                model.ShowRecaptchaError = true;
                return View("~/Views/Home/Donate.cshtml", model);
            }
        }

        if (!ModelState.IsValid)
        {
            model.ClientToken = await _donationService.GetClientToken();
            model.IsValid = false;
            model.ShowDonationError = true;
            return View("~/Views/Home/Donate.cshtml", model);
        }

        var success = await _donationService.Donate(model, culture);

        if (!success)
        {
            _logger.LogWarning("Donation submission failed for culture {Culture}.", culture);
            model.ClientToken = await _donationService.GetClientToken();
            model.ShowDonationError = true;
            return View("~/Views/Home/Donate.cshtml", model);
        }

        return View("~/Views/Donate/donation_success.cshtml");
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
