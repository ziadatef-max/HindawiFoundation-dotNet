using System.ComponentModel.DataAnnotations;

namespace HindawiFoundation.Web.Models;

public class DonationViewModel
{
    [Required(ErrorMessage = "Frequency is required")]
    public string Frequency { get; set; } = "monthly";

    [Required(ErrorMessage = "Amount is required")]
    public decimal? Amount { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    public string PaymentMethodnonce { get; set; } = string.Empty;

    public string CurrencyCode { get; set; } = "USD";

    public bool IsValid { get; set; }
    public bool ShowDonationError { get; set; } = false;
    public bool ShowRecaptchaError { get; set; }

    public string? ClientToken { get; set; }
}
