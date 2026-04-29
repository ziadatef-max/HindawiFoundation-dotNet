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

    [HttpGet("")]
    public async Task<IActionResult> Index([FromRoute] string culture)
    {
        ViewData["Culture"] = culture;
        ViewData["ActivePage"] = "donate";

        var clientToken = await _donationService.GetClientToken();

        var model = new DonationViewModel
        {
            Frequency = "monthly",
            CurrencyCode = "EGP",
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
        ViewData["Culture"] = culture;
        ViewData["ActivePage"] = "donate";

        // Server-side validation: Frequency
        if (model.Frequency != "monthly" && model.Frequency != "one-time")
        {
            ModelState.AddModelError(nameof(model.Frequency), "Please select a valid donation frequency.");
        }

        // Server-side validation: Amount
        if (!model.Amount.HasValue || model.Amount.Value <= 0 || model.Amount.Value > 1000000)
        {
            ModelState.AddModelError(nameof(model.Amount), "Please enter a valid donation amount.");
        }

        // reCAPTCHA validation
        //var recaptchaToken = Request.Form["g-recaptcha-response"].ToString();
        //if (string.IsNullOrWhiteSpace(recaptchaToken))
        //{
        //    ModelState.AddModelError(string.Empty, "Please complete the reCAPTCHA verification.");
        //}
        //else
        //{
        //    var recaptchaValid = await _donationService.ValidateRecaptcha(recaptchaToken);
        //    if (!recaptchaValid)
        //    {
        //        ModelState.AddModelError(string.Empty, "reCAPTCHA verification failed. Please try again.");
        //    }
        //}

        if (string.IsNullOrWhiteSpace(model.PaymentMethodnonce))
        {
            ModelState.AddModelError(string.Empty, "Please enter valid card details before submitting your donation.");
        }
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
            ModelState.AddModelError(string.Empty, "We were unable to process your donation at this time. Please try again later.");
            return View("~/Views/Home/Donate.cshtml", model);
        }

        return View("~/Views/Donate/donation_success.cshtml");
    }

    [HttpGet("~/{culture}/donate-unsubscribe")]
    public IActionResult Unsubscribe([FromRoute] string culture)
    {
        ViewData["Culture"] = culture;
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
