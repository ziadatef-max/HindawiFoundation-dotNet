using HindawiFoundation.Web.Models;

namespace HindawiFoundation.Web.Services.Static;

public static class SupportedLanguages
{
    public static IReadOnlyList<LanguageDto> All { get; } =
    [
        new() { IsoCode = "en", Name = "English" },
        new() { IsoCode = "ar", Name = "العربية" }
    ];
}
