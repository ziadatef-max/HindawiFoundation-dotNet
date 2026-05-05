using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using HindawiFoundation.Web.Models;
using HindawiFoundation.Web.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HindawiFoundation.Web.Services;

public class DonationService : IDonationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppSettings _appSettings;
    private readonly ILogger<DonationService> _logger;
    private readonly byte[] _iv = new byte[16];

    private static readonly JsonSerializerSettings InvariantJson = new()
    {
        Culture = CultureInfo.InvariantCulture,
        DateParseHandling = DateParseHandling.None
    };

    public DonationService(
        IHttpClientFactory httpClientFactory,
        IOptions<AppSettings> appSettings,
        ILogger<DonationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    public async Task<string?> GetClientToken()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = string.Format(_appSettings.DonationApi.GetClinetToken, _appSettings.DonationApi.BaseURL);

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to get client token. Status: {StatusCode}. Response: {Response}",
                    response.StatusCode,
                    content);

                return null;
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning("Client token API returned empty response.");
                return null;
            }

            return content.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client token");
            return null;
        }
    }

    public async Task<bool> Donate(DonationViewModel donationViewModel, string language)
    {
        try
        {
            _logger.LogInformation(
                "Processing donation. Frequency: {Frequency}, Amount: {Amount}, Currency: {CurrencyCode}, HasEmail: {HasEmail}, Language: {Language}.",
                donationViewModel.Frequency,
                donationViewModel.Amount,
                donationViewModel.CurrencyCode,
                !string.IsNullOrWhiteSpace(donationViewModel.Email),
                language);

            var vmDonation = new VMDonation
            {
                FirstName = donationViewModel.FirstName ?? string.Empty,
                LastName = donationViewModel.LastName ?? string.Empty,
                Mobile = donationViewModel.Phone ?? string.Empty,
                UserEmail = donationViewModel.Email,
                PeriodPay = donationViewModel.Frequency,
                Amount = donationViewModel.Amount,
                PaymentMethodnonce = donationViewModel.PaymentMethodnonce,
                ProjectId = _appSettings.DonationApi.ProjectId,
                CurrencyCode = donationViewModel.CurrencyCode,
                Language = language
            };

            var jsonPayload = JsonConvert.SerializeObject(vmDonation, InvariantJson);
            var encryptedData = EncryptPayload(jsonPayload);

            var client = _httpClientFactory.CreateClient();
            var url = string.Format(_appSettings.DonationApi.Donate, _appSettings.DonationApi.BaseURL);

            var jsonContent = JsonConvert.SerializeObject(encryptedData, InvariantJson);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Donation API returned non-success status {StatusCode} : {Body}.",
                    response.StatusCode, responseBody);

                return false;
            }

            _logger.LogInformation("Donation succeeded.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing donation");
            return false;
        }
    }

    public async Task<bool> ValidateRecaptcha(string token)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = string.Format(
                _appSettings.DonationApi.GoogleRecaptchaURL,
                _appSettings.DonationApi.GoogleRecaptchaSecret,
                token);

            var response = await client.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                dynamic? result = JsonConvert.DeserializeObject(content, InvariantJson);
                return result?.success == true;
            }

            _logger.LogWarning("reCAPTCHA validation failed. Status: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating reCAPTCHA");
            return false;
        }
    }

    public async Task<List<CurrencyDto>> GetCurrencies()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = string.Format(_appSettings.DonationApi.GetCurrencies, _appSettings.DonationApi.BaseURL);
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var currencies = JsonConvert.DeserializeObject<List<CurrencyDto>>(content, InvariantJson) ?? new List<CurrencyDto>();
                return currencies;
            }

            _logger.LogWarning("Failed to get currencies. Status: {StatusCode}", response.StatusCode);
            return new List<CurrencyDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting currencies");
            return new List<CurrencyDto>();
        }
    }

    public async Task<string> GetDefaultCurrency(string countryCode)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = string.Format(
                _appSettings.DonationApi.GetDefaultCurrency,
                _appSettings.DonationApi.BaseURL,
                countryCode);

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                dynamic? result = JsonConvert.DeserializeObject(content, InvariantJson);
                return result?.currencyCode?.ToString() ?? "USD";
            }

            _logger.LogWarning(
                "Default currency API returned non-success status {StatusCode} for country {CountryCode}. Falling back to USD.",
                response.StatusCode,
                countryCode);
            return "USD";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting default currency for country code: {CountryCode}", countryCode);
            return "USD";
        }
    }

    private string EncryptPayload(string data)
    {
        try
        {
            using var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(_appSettings.DonationApi.EncryptionKey);
            aes.IV = _iv;

            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(data);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to encrypt data", ex);
        }
    }
}
