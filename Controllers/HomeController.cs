using Microsoft.AspNetCore.Mvc;

namespace HindawiFoundation.Web.Controllers;

public class HomeController : Controller
{
    // GET: /  or /Home/Index   (was home.html)
    public IActionResult Index() => View();

    // GET: /Home/About          (was about.html)
    public IActionResult About() => View();

    // GET: /Home/Partners       (was partners.html)
    public IActionResult Partners() => View();

    // GET: /Home/News           (was news.html)
    public IActionResult News() => View();

    // GET: /Home/NewsDetails    (was news-details.html)
    public IActionResult NewsDetails(int? id = null) => View();

    // GET: /Home/Contact        (was contact.html)
    public IActionResult Contact() => View();

    // GET: /Home/Donate         (was donate.html)
    public IActionResult Donate() => View();

    // Generic error page used by app.UseExceptionHandler("/Home/Error") in non-dev.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
