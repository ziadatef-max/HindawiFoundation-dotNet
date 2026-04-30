using System.Globalization;
using HindawiFoundation.Web.Localization;
using HindawiFoundation.Web.Models;
using HindawiFoundation.Web.Services;
using HindawiFoundation.Web.Services.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// MVC + view localization
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Force generated URLs to be lowercase (e.g. /en/about, not /en/About)
builder.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);

// Caching for the JSON localizer
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();

// Localization (JSON-based)
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

// Application services
builder.Services.AddHttpClient();
builder.Services.AddScoped<IDonationService, DonationService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Configure request localization (route-based culture)
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("ar")
};
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
localizationOptions.RequestCultureProviders.Clear();
localizationOptions.RequestCultureProviders.Add(new CustomRouteDataCultureProvider());
app.UseRequestLocalization(localizationOptions);

app.UseAuthorization();

// ── Redirects ────────────────────────────────────────────────────────────────

// Root → English home
app.MapGet("/", context =>
{
    context.Response.Redirect("/en", permanent: false);
    return Task.CompletedTask;
});

// /news/{id} without culture → /en/news/{id}
app.MapGet("/news/{id}", context =>
{
    var id = context.GetRouteValue("id") as string ?? NewsMap.GetId(0);
    context.Response.Redirect($"/en/news/{NewsMap.Resolve(id)}", permanent: false);
    return Task.CompletedTask;
});

app.MapControllers();

app.Run();
