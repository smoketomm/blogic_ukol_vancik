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
            var klient = await _context.Klienti.FindAsync(id);
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

            var claimsID = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsID));

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

        public IActionResult Delete(int id)
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

            return RedirectToAction("Index");
        }
    }
}
