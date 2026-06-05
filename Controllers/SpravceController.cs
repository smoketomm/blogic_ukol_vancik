using Blogic_ukol_vancik.Data;
using Blogic_ukol_vancik.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blogic_ukol_vancik.Controllers
{
    public class SpravceController : Controller
    {

        private readonly MostDataDbContext _context;
        private readonly ILogger<SpravceController> _logger;
        public SpravceController(ILogger<SpravceController> logger, MostDataDbContext context)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            Console.WriteLine($"Je uživatel reálně přihlášen? {User.Identity?.IsAuthenticated}");
            List<Spravce> seznamSpravcu = _context.Spravci.ToList();
            return View(seznamSpravcu);
        }

        [HttpPost]
        public async Task<IActionResult> PrihlasitSe(int id)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Přihlašování správce s ID: {id}");
            var spravce = await _context.Spravci.FindAsync(id);
            if (spravce == null)
            {
                return NotFound();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, spravce.ID.ToString()),
                new Claim(ClaimTypes.Name, spravce.Jmeno),
                new Claim("JeSpravce", spravce.JeSpravce.ToString())
            };

            var claimsID = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsID));


            return RedirectToAction("Index", "Spravce");
        }

        [HttpPost]
        public async Task<IActionResult> Odhlasit()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
