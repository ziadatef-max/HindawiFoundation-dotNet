using Microsoft.AspNetCore.Mvc;

namespace HindawiFoundation.Web.Controllers;

[Route("{culture}")]
public class HomeController : Controller
{
    [HttpGet("")]
    [HttpGet("Home")]
    public IActionResult Index([FromRoute] string culture)
    {
        ViewData["Culture"] = culture;
        ViewData["ActivePage"] = "home";
        return View();
    }

    [HttpGet("About")]
    public IActionResult About([FromRoute] string culture)
    {
        ViewData["Culture"] = culture;
        ViewData["ActivePage"] = "about";
        return View();
    }

    [HttpGet("Partners")]
    public IActionResult Partners([FromRoute] string culture)
    {
        ViewData["Culture"] = culture;
        ViewData["ActivePage"] = "partners";
        return View();
    }

    [HttpGet("News")]
    public IActionResult News([FromRoute] string culture)
    {
        ViewData["Culture"] = culture;
        ViewData["ActivePage"] = "news";
        return View();
    }

    [HttpGet("NewsDetails")]
    [HttpGet("NewsDetails/{id?}")]
    public IActionResult NewsDetails([FromRoute] string culture, int? id = null)
    {
        ViewData["Culture"] = culture;
        ViewData["ActivePage"] = "news";
        return View();
    }

    [HttpGet("Contact")]
    public IActionResult Contact([FromRoute] string culture)
    {
        ViewData["Culture"] = culture;
        ViewData["ActivePage"] = "contact";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
