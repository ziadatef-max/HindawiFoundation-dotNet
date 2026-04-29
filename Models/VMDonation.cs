using System.ComponentModel.DataAnnotations;

namespace HindawiFoundation.Web.Models;

public class VMDonation
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    [Required]
    public string UserEmail { get; set; } = string.Empty;

    public string PeriodPay { get; set; } = "monthly";

    public decimal? Amount { get; set; }

    [Required]
    public string PaymentMethodnonce { get; set; } = string.Empty;

    public int ProjectId { get; set; } = 1;

    public string CurrencyCode { get; set; } = "USD";

    public string Language { get; set; } = "en";

    // Keep for API compatibility
    public string? CreditCardNumber { get; set; }

    public string? CreditCardCVV { get; set; }

    public string? CreditCardExpiryMonth { get; set; }

    public string? CreditCardExpiryYear { get; set; }
}
