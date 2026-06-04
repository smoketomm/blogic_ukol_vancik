using Blogic_ukol_vancik.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography.Xml;

namespace Blogic_ukol_vancik.Controllers
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
            List<SmlouvaViewModel> smlouvy = new List<SmlouvaViewModel>();
            smlouvy.Add(new SmlouvaViewModel { evCislo = 1111, instituce = "CSOB", klient = "Mavek", SpravciID = new List<string>(){"Tomáš"}, datumUzavreni = DateTime.Now, datumPlatnosti = DateTime.Now, datumUkonceni = DateTime.Now });
            smlouvy.Add(new SmlouvaViewModel { evCislo = 555, instituce = "Alianz", klient = "Kuba", SpravciID = new List<string>() { "Picus" }, datumUzavreni = DateTime.Now, datumPlatnosti = DateTime.Now, datumUkonceni = DateTime.Now });
            return View(smlouvy);
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
    }
}
