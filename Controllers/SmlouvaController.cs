using Blogic_ukol_vancik.Data;
using Blogic_ukol_vancik.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blogic_ukol_vancik.Controllers
{
    public class SmlouvaController : Controller
    {
        Random nahoda = new Random();
        private readonly MostDataDbContext _context;
        private readonly ILogger<SmlouvaController> _logger;
        public SmlouvaController(ILogger<SmlouvaController> logger, MostDataDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public IActionResult VytvoritSmlouvu([FromBody] SmlouvaDto data)
        {

            Smlouva novaSmlouva = new Smlouva
            {
                EvCislo = nahoda.Next(100, 1000),
                Instituce = data.Instituce,
                Klient = data.Klient,
                DatumUzavreni = DateTime.Now,
                DatumPlatnosti = DateTime.Parse(data.DatumPlatnosti?.ToString("yyyy-MM-dd")),
                DatumUkonceni = DateTime.Parse(data.DatumUkonceni?.ToString("yyyy-MM-dd"))
            };


            _context.Smlouvy.Add(novaSmlouva);
            _context.SaveChanges();

            _context.Entry(novaSmlouva).Reload();


            List<SmlouvaSpravce> seznamZaznamu = new List<SmlouvaSpravce>();

            for (int i = 0; i < data.Spravce.Count; i++)
            {
                SmlouvaSpravce novyDotaz = new SmlouvaSpravce
                {
                    SpravceID = int.Parse(data.Spravce[i]),
                    SmlouvaID = novaSmlouva.ID
                };

                seznamZaznamu.Add(novyDotaz);
            }

            _context.SmlouvySpravci.AddRange(seznamZaznamu);
            _context.SaveChanges();

            return Ok(new { message = "uloženo úspěšně!" });
        }

        public IActionResult Create()
        {

            List<Klient> seznamKlientu = _context.Klienti.ToList();
            List<Spravce> seznamSpravcu = _context.Spravci.ToList();

            var seznamy = (SeznamKlientu: seznamKlientu, SeznamSpravcu: seznamSpravcu);

            return View(seznamy);
        }

        public IActionResult Show(int id)
        {
            var smlouva = _context.Smlouvy.FirstOrDefault(s => s.ID == id);

            if (smlouva == null)
            {
                return NotFound();
            }

            var prirazeneVazby = _context.SmlouvySpravci.Where(v => v.SmlouvaID == id).ToList();
            var SpravciID = prirazeneVazby.Select(v => v.SpravceID).ToList();

            ViewBag.SpravciViewBag = _context.Spravci.Where(s => SpravciID.Contains(s.ID)).ToList();

            return View(smlouva);
        }

        public IActionResult Delete(int id)
        {

            var smlouva = _context.Smlouvy.FirstOrDefault(s => s.ID == id);

            if (smlouva == null)
            {
                return NotFound();
            }

            var vazbyKeSmazani = _context.SmlouvySpravci.Where(v => v.SmlouvaID == id).ToList();

            _context.SmlouvySpravci.RemoveRange(vazbyKeSmazani);
            _context.Smlouvy.Remove(smlouva);

            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Edit(int id)
        {
            var smlouva = _context.Smlouvy.FirstOrDefault(s => s.ID == id);

            if (smlouva == null)
            {
                return NotFound();
            }

            var prirazeneVazby = _context.SmlouvySpravci.Where(v => v.SmlouvaID == id).ToList();
            var SpravciID = prirazeneVazby.Select(v => v.SpravceID).ToList();

            ViewBag.SpravciViewBag = _context.Spravci.Where(s => SpravciID.Contains(s.ID)).ToList();

            List<Klient> seznamKlientu = _context.Klienti.ToList();
            List<Spravce> seznamSpravcu = _context.Spravci.ToList();
            var seznamy = (SeznamKlientu: seznamKlientu, SeznamSpravcu: seznamSpravcu, Smlouva: smlouva);
            return View(seznamy);
        }

        [HttpPost]
        public IActionResult Edit(Smlouva upravenaSmlouva, List<int> Spravci)
        {
            var staraSmlouva = _context.Smlouvy.FirstOrDefault(s => s.ID == upravenaSmlouva.ID);
            if (staraSmlouva == null) return NotFound();

            staraSmlouva.Instituce = upravenaSmlouva.Instituce;
            staraSmlouva.Klient = upravenaSmlouva.Klient;
            staraSmlouva.DatumPlatnosti = upravenaSmlouva.DatumPlatnosti;
            staraSmlouva.DatumUkonceni = upravenaSmlouva.DatumUkonceni;

            var stareVazby = _context.SmlouvySpravci.Where(v => v.SmlouvaID == upravenaSmlouva.ID).ToList();
            _context.SmlouvySpravci.RemoveRange(stareVazby);

            if (Spravci != null)
            {
                foreach (var spravceId in Spravci)
                {
                    _context.SmlouvySpravci.Add(new SmlouvaSpravce
                    {
                        SmlouvaID = upravenaSmlouva.ID,
                        SpravceID = spravceId
                    });
                }
            }
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
