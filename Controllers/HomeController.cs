using Microsoft.AspNetCore.Mvc;
using NetflixClone.Models;
using System.Diagnostics;

namespace NetflixClone.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = true;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            ViewData["Page"] = "Home";
            ViewData["ShowSignIn"] = false;
            ViewData["BodyClass"] = "lighter-background";
            return View();
        }

        public IActionResult HomeNetflix()
        {
            ViewData["Page"] = "Movie";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
