using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sadegh_Identity_Sample.Models;

namespace Sadegh_Identity_Sample.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Policy = "Buyer")]
        public string JustBuyer()
        {
            return "??? ?????? ?????";
        }

        [Authorize(Policy = "BloodType")]
        public string Blood()
        {
            return "Ap and O";
        }

        [Authorize(Policy = "Credit")]
        public string Credit()
        {
            return "ok";
        }
    }
}
