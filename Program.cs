using HindawiFoundation.Web.Models;
using HindawiFoundation.Web.Services;
using HindawiFoundation.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Add MVC services (controllers + views).
builder.Services.AddControllersWithViews();

// Add services
builder.Services.AddHttpClient();
builder.Services.AddScoped<IDonationService, DonationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Redirect root to /en/Home
app.MapGet("/", context =>
{
    context.Response.Redirect("/en/Home", permanent: false);
    return Task.CompletedTask;
});

// Attribute routing for Home and Donate controllers
app.MapControllers();

app.Run();
