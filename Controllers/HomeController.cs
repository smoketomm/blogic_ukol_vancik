using Blogic_ukol_vancik.Data;
using Blogic_ukol_vancik.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Blogic_ukol_vancik.Controllers
{
    public class HomeController : Controller
    {
        private readonly MostDataDbContext _context;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, MostDataDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Smlouva> seznamSmluv = _context.Smlouvy.ToList();

            ViewBag.Vazby = _context.SmlouvyVazby.ToList();
            ViewBag.Spravci = _context.Spravci.ToList();

            return View(seznamSmluv);
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
