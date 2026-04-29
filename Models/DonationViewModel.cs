using System.ComponentModel.DataAnnotations;

namespace HindawiFoundation.Web.Models;

public class DonationViewModel
{
    [Required(ErrorMessage = "Frequency is required")]
    public string Frequency { get; set; } = "monthly";

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 1000000, ErrorMessage = "Amount must be between 0.01 and 1,000,000")]
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

    public string? ClientToken { get; set; }
}
