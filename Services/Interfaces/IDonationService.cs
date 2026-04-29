using HindawiFoundation.Web.Models;

namespace HindawiFoundation.Web.Services.Interfaces;

public interface IDonationService
{
    Task<string?> GetClientToken();

    Task<bool> Donate(DonationViewModel donationViewModel, string language);

    Task<bool> ValidateRecaptcha(string token);

    Task<List<CurrencyDto>> GetCurrencies();

    Task<string> GetDefaultCurrency(string countryCode);
}
