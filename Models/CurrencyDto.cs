namespace HindawiFoundation.Web.Models;

public class CurrencyDto
{
    public string CurrencyCode { get; set; } = string.Empty;

    public string CurrencyName { get; set; } = string.Empty;

    public decimal ExchangeRate { get; set; }
}
