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

        [HttpPost]
        public IActionResult VytvoritSpravce(Spravce novySpravce)
        {
            if (novySpravce == null || novySpravce.Vek <= 0 || novySpravce.Vek > 99 || novySpravce.RodneCislo.ToString().Length != 10 || novySpravce.Telefon.ToString().Length != 9)
            {
                return BadRequest("Neplatný správce.");
            }

            novySpravce = new Spravce
            {
                Jmeno = novySpravce.Jmeno,
                Prijmeni = novySpravce.Prijmeni,
                Telefon = novySpravce.Telefon,
                Email = novySpravce.Email,
                RodneCislo = novySpravce.RodneCislo,
                Vek = novySpravce.Vek,
            };

            _context.Spravci.Add(novySpravce);
            _context.SaveChanges();
            return RedirectToAction("Index", "Spravce");
        }

        public IActionResult Show(int id)
        {
            var spravce = _context.Spravci.FirstOrDefault(s => s.ID == id);
            var vazbySpravce = _context.SmlouvyVazby.Where(v => v.SpravceID == id).Select(v => v.SmlouvaID).ToList();
            var smlouvy = _context.Smlouvy.Where(s => vazbySpravce.Contains(s.ID)).ToList();
            var info = (Spravce: spravce, Smlouvy: smlouvy);
            return View(info);
        }

        public IActionResult Delete(int id)
        {
            var spravce = _context.Spravci.Find(id);
            if (spravce == null)
            {
                return NotFound();
            }

            var vazbyKeSmazani = _context.SmlouvyVazby.Where(v => v.SpravceID == id).ToList();
            var smlouvuSpravce = vazbyKeSmazani.Select(v => v.SmlouvaID).ToList();

            var samotneSmlouvyID = _context.SmlouvyVazby.Where(v => smlouvuSpravce.Contains(v.SmlouvaID)).GroupBy(v => v.SmlouvaID).Where(g => g.Count() == 1).Select(g => g.Key).ToList();


            if (samotneSmlouvyID.Any())
            {
                return BadRequest("Nelze smazat správce, protože je jediným správcem u některých smluv. Nejprve přiřaďte tyto smlouvy jinému správci.");
            }

            _context.SmlouvyVazby.RemoveRange(vazbyKeSmazani);
            _context.Spravci.Remove(spravce);
            _context.SaveChanges();
            return RedirectToAction("Index", "Spravce");
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
