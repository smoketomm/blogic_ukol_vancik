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
            Spravce spravce = await _context.Spravci.FindAsync(id);
            if (spravce == null)
            {
                return NotFound();
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, spravce.ID.ToString()),
                new Claim(ClaimTypes.Name, spravce.Jmeno),
                new Claim("Prijmeni", spravce.Prijmeni),
                new Claim("JeSpravce", spravce.JeSpravce.ToString())
            };

            ClaimsIdentity claimsID = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsID));


            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult VytvoritSpravce(Spravce novySpravce)
        {
            if (novySpravce == null || !int.TryParse(novySpravce.Telefon, out _) || !int.TryParse(novySpravce.RodneCislo, out _))
            {
                return BadRequest("Neplatné hodnoty.");
            }

            if (novySpravce.Vek <= 0 || novySpravce.Vek > 99 || novySpravce.RodneCislo.Length != 10 || novySpravce.Telefon.Length != 9 || int.Parse(novySpravce.RodneCislo) < 0 || int.Parse(novySpravce.Telefon) < 0)
            {
                return BadRequest("Neplatné hodnoty.");
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
            return RedirectToAction("Index");
        }

        public IActionResult Show(int id)
        {
            Spravce spravce = _context.Spravci.FirstOrDefault(s => s.ID == id);
            List<int> vazbySpravce = _context.SmlouvyVazby.Where(v => v.SpravceID == id).Select(v => v.SmlouvaID).ToList();
            List<Smlouva> smlouvy = _context.Smlouvy.Where(s => vazbySpravce.Contains(s.ID)).ToList();
            (Spravce Spravce, List<Smlouva> Smlouvy) info = (Spravce: spravce, Smlouvy: smlouvy);
            return View(info);
        }

        public async Task<IActionResult> Delete(int id)
        {
            Spravce spravce = _context.Spravci.Find(id);
            if (spravce == null)
            {
                return NotFound();
            }

            List<SmlouvaVazba> vazbyKeSmazani = _context.SmlouvyVazby.Where(v => v.SpravceID == id).ToList();
            List<int> smlouvuSpravce = vazbyKeSmazani.Select(v => v.SmlouvaID).ToList();

            List<int> samotneSmlouvyID = _context.SmlouvyVazby.Where(v => smlouvuSpravce.Contains(v.SmlouvaID)).GroupBy(v => v.SmlouvaID).Where(g => g.Count() == 1).Select(g => g.Key).ToList();


            if (samotneSmlouvyID.Any())
            {
                return BadRequest("Nelze smazat správce, protože je jediným správcem u některých smluv. Nejprve přiřaďte tyto smlouvy jinému správci.");
            }

            if (User.Identity.IsAuthenticated && User.FindFirst(ClaimTypes.NameIdentifier)?.Value == id.ToString() && User.FindFirst("JeSpravce")?.Value == "True")
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            _context.SmlouvyVazby.RemoveRange(vazbyKeSmazani);
            _context.Spravci.Remove(spravce);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult UpravitSpravce(Spravce upravenySpravce)
        {

            Spravce starySpravce = _context.Spravci.FirstOrDefault(s => s.ID == upravenySpravce.ID);

            if (starySpravce == null) return NotFound();

            if (upravenySpravce == null || !int.TryParse(upravenySpravce.Telefon, out _) || !int.TryParse(upravenySpravce.RodneCislo, out _))
            {
                return BadRequest("Neplatné hodnoty.");
            }

            if (upravenySpravce.Vek <= 0 || upravenySpravce.Vek > 99 || upravenySpravce.RodneCislo.Length != 10 || upravenySpravce.Telefon.Length != 9 || int.Parse(upravenySpravce.RodneCislo) < 0 || int.Parse(upravenySpravce.Telefon) < 0)
            {
                return BadRequest("Neplatné hodnoty.");
            }

            starySpravce.Jmeno = upravenySpravce.Jmeno;
            starySpravce.Prijmeni = upravenySpravce.Prijmeni;
            starySpravce.Telefon = upravenySpravce.Telefon;
            starySpravce.Email = upravenySpravce.Email;
            starySpravce.RodneCislo = upravenySpravce.RodneCislo;
            starySpravce.Vek = upravenySpravce.Vek;

            _context.SaveChanges();
            return RedirectToAction("Index", "Spravce");
        }

        public IActionResult Edit(int id)
        {
            Spravce spravce = _context.Spravci.FirstOrDefault(s => s.ID == id);
            if (spravce == null)
            {
                return NotFound();
            }

            return View(spravce);
        }
    }
}
