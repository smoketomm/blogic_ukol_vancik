using Blogic_ukol_vancik.Data;
using Blogic_ukol_vancik.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blogic_ukol_vancik.Controllers
{
    public class KlientController : Controller
    {
        private readonly MostDataDbContext _context;
        public KlientController(MostDataDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Klient> klienti = _context.Klienti.ToList();
            return View(klienti);
        }

        public async Task<IActionResult> PrihlasitSe(int id) 
        {
            Klient klient = await _context.Klienti.FindAsync(id);
            if (klient == null)
            {
                return NotFound();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, klient.ID.ToString()),
                new Claim(ClaimTypes.Name, klient.Jmeno),
                new Claim("Prijmeni", klient.Prijmeni),
                new Claim("JeSpravce", klient.JeSpravce.ToString())
            };

            ClaimsIdentity claimsID = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsID));

            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult VytvoritKlienta(Klient novyKlient)
        {
            if (novyKlient == null || !int.TryParse(novyKlient.Telefon, out _) || !int.TryParse(novyKlient.RodneCislo, out _))
            {
                return BadRequest("Neplatné hodnoty.");
            }

            if (novyKlient.Vek <= 0 || novyKlient.Vek > 99 || novyKlient.RodneCislo.Length != 10 || novyKlient.Telefon.Length != 9 || int.Parse(novyKlient.RodneCislo) < 0 || int.Parse(novyKlient.Telefon) < 0)
            {
                return BadRequest("Neplatné hodnoty.");
            }

            novyKlient = new Klient
            {
                Jmeno = novyKlient.Jmeno,
                Prijmeni = novyKlient.Prijmeni,
                Vek = novyKlient.Vek,
                RodneCislo = novyKlient.RodneCislo,
                Email = novyKlient.Email,
                Telefon = novyKlient.Telefon,
            };
            _context.Klienti.Add(novyKlient);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Klient klient = _context.Klienti.Find(id);
            if (klient == null)
            {
                return NotFound();
            }
            return View(klient);
        }

        public IActionResult UpravitKlienta(Klient upravenyKlient)
        {
            if (upravenyKlient == null || !int.TryParse(upravenyKlient.Telefon, out _) || !int.TryParse(upravenyKlient.RodneCislo, out _))
            {
                return BadRequest("Neplatné hodnoty.");
            }

            if (upravenyKlient.Vek <= 0 || upravenyKlient.Vek > 99 || upravenyKlient.RodneCislo.Length != 10 || upravenyKlient.Telefon.Length != 9 || int.Parse(upravenyKlient.RodneCislo) < 0 || int.Parse(upravenyKlient.Telefon) < 0)
            {
                return BadRequest("Neplatné hodnoty.");
            }


            Klient klient = _context.Klienti.Find(upravenyKlient.ID);
            if (klient == null)
            {
                return NotFound();
            }
            klient.Jmeno = upravenyKlient.Jmeno;
            klient.Prijmeni = upravenyKlient.Prijmeni;
            klient.Vek = upravenyKlient.Vek;
            klient.RodneCislo = upravenyKlient.RodneCislo;
            klient.Email = upravenyKlient.Email;
            klient.Telefon = upravenyKlient.Telefon;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Show(int id)
        {
            Klient klient = _context.Klienti.FirstOrDefault(s => s.ID == id);
            List<int> vazbySpravce = _context.SmlouvyVazby.Where(v => v.KlientID == id).Select(v => v.SmlouvaID).ToList();
            List<Smlouva> smlouvy = _context.Smlouvy.Where(s => vazbySpravce.Contains(s.ID)).ToList();
            (Klient Klient, List<Smlouva> Smlouvy) info = (Klient: klient, Smlouvy: smlouvy);
            return View(info);
        }

        public async Task<IActionResult> Delete(int id)
        {
            Klient klient = _context.Klienti.Find(id);
            if (klient == null)
            {
                return NotFound();
            }

            List<SmlouvaVazba> vazbyKeSmazani = _context.SmlouvyVazby.Where(v => v.KlientID == id).ToList();
            List<int> smlouvyKlienta = vazbyKeSmazani.Select(v => v.SmlouvaID).ToList();

            List<Smlouva> samotneSmlouvy = _context.Smlouvy.Where(s => smlouvyKlienta.Contains(s.ID)).ToList();

            _context.Klienti.Remove(klient);
            _context.SmlouvyVazby.RemoveRange(vazbyKeSmazani);
            _context.Smlouvy.RemoveRange(samotneSmlouvy);
            _context.SaveChanges();

            if (User.Identity.IsAuthenticated && User.FindFirst(ClaimTypes.NameIdentifier)?.Value == id.ToString() && User.FindFirst("JeSpravce")?.Value == "False")
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return RedirectToAction("Index");
        }
    }
}
