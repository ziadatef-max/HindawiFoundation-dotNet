namespace HindawiFoundation.Web.Models;

public class AppSettings
{
    public string PartnershipsTeamEmail { get; set; } = string.Empty;
    public DonationApi DonationApi { get; set; } = new();

    public DonationAccount DonationAccount { get; set; } = new();
}

public class DonationApi
{
    public string BaseURL { get; set; } = "https://dev-donation.api.booktime.org";

    public string Donate { get; set; } = "{0}/donation";
    public int ProjectId { get; set; } = 2;

    public string EncryptionKey { get; set; } = string.Empty;

    public string GoogleRecaptchaURL { get; set; } = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";

    public string GoogleRecaptchaKey { get; set; } = string.Empty;

    public string GoogleRecaptchaSecret { get; set; } = string.Empty;

    public string GetClinetToken { get; set; } = "{0}/donation/get-client-token";

    public string GetCurrencies { get; set; } = "{0}/donation/currencies";

    public string GetDefaultCurrency { get; set; } = "{0}/donation/default-currency?countryCode={1}";
}

public class DonationAccount
{
    public string BankNumber { get; set; } = string.Empty;

    public string AccountName { get; set; } = string.Empty;

    public string BankCode { get; set; } = string.Empty;

    public string SwiftCode { get; set; } = string.Empty;

    public string Iban { get; set; } = string.Empty;
}
