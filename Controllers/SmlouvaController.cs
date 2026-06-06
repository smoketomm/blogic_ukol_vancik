using Blogic_ukol_vancik.Data;
using Blogic_ukol_vancik.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public IActionResult VytvoritSmlouvu(Smlouva novaSmlouva, List<int> Spravci, int KlientID)
        {
            if (Spravci == null || Spravci.Count == 0 || novaSmlouva.DatumPlatnosti < DateTime.Now || novaSmlouva.DatumUkonceni < DateTime.Now || novaSmlouva.DatumUkonceni < novaSmlouva.DatumPlatnosti || novaSmlouva.Klient == "Vyberte klienta...")
            {
                return BadRequest(new { message = "Neplatne hodnoty" });
            }

            int EvCisloGen = nahoda.Next(100, 1000);


            while (_context.Smlouvy.Any(smlouva => smlouva.EvCislo == EvCisloGen))
            {
                EvCisloGen = nahoda.Next(100, 1000);
            }

            string[] casti = novaSmlouva.Klient.Split('|');

            int klientID = int.Parse(casti[0]);
            string Klient = casti[1];

            novaSmlouva = new Smlouva
            {
                EvCislo = EvCisloGen,
                Instituce = novaSmlouva.Instituce,
                Klient = Klient,
                DatumUzavreni = DateTime.Now,
                DatumPlatnosti = novaSmlouva.DatumPlatnosti,
                DatumUkonceni = novaSmlouva.DatumUkonceni
            };

            _context.Smlouvy.Add(novaSmlouva);
            _context.SaveChanges();

            _context.Entry(novaSmlouva).Reload();

            List<SmlouvaVazba> seznamZaznamu = new List<SmlouvaVazba>();

            foreach(int spravceId in Spravci)
            {
                SmlouvaVazba novyDotaz = new SmlouvaVazba
                {
                    SpravceID = spravceId,
                    SmlouvaID = novaSmlouva.ID,
                    KlientID = klientID
                };
                seznamZaznamu.Add(novyDotaz);
            }

            _context.SmlouvyVazby.AddRange(seznamZaznamu);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");

        }

        public IActionResult Create()
        {

            List<Klient> seznamKlientu = _context.Klienti.ToList();
            List<Spravce> seznamSpravcu = _context.Spravci.ToList();

            (List<Klient> SeznamKlientu, List<Spravce> SeznamSpravcu) seznamy = (SeznamKlientu: seznamKlientu, SeznamSpravcu: seznamSpravcu);

            return View(seznamy);
        }

        public IActionResult Show(int id)
        {
            Smlouva smlouva = _context.Smlouvy.FirstOrDefault(s => s.ID == id);

            if (smlouva == null)
            {
                return NotFound();
            }

            List<SmlouvaVazba> prirazeneVazby = _context.SmlouvyVazby.Where(v => v.SmlouvaID == id).ToList();
            List<int> SpravciID = prirazeneVazby.Select(v => v.SpravceID).ToList();
            int? KlientID = prirazeneVazby.Select(v => v.KlientID).FirstOrDefault();


            ViewBag.SpravciViewBag = _context.Spravci.Where(s => SpravciID.Contains(s.ID)).ToList();

            (Klient ContextKlient, Smlouva Smlouva) seznam = (ContextKlient: _context.Klienti.FirstOrDefault(k => k.ID == KlientID), Smlouva: smlouva);

            return View(seznam);
        }

        public IActionResult Delete(int id)
        {

            Smlouva smlouva = _context.Smlouvy.FirstOrDefault(s => s.ID == id);

            if (smlouva == null)
            {
                return NotFound();
            }

            List<SmlouvaVazba> vazbyKeSmazani = _context.SmlouvyVazby.Where(v => v.SmlouvaID == id).ToList();

            _context.SmlouvyVazby.RemoveRange(vazbyKeSmazani);
            _context.Smlouvy.Remove(smlouva);

            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Edit(int id)
        {
            Smlouva smlouva = _context.Smlouvy.FirstOrDefault(s => s.ID == id);

            if (smlouva == null)
            {
                return NotFound();
            }

            List<SmlouvaVazba> prirazeneVazby = _context.SmlouvyVazby.Where(v => v.SmlouvaID == id).ToList();
            List<int> SpravciID = prirazeneVazby.Select(v => v.SpravceID).ToList();

            ViewBag.SpravciViewBag = _context.Spravci.Where(s => SpravciID.Contains(s.ID)).ToList();

            List<Klient> seznamKlientu = _context.Klienti.ToList();
            List<Spravce> seznamSpravcu = _context.Spravci.ToList();
            (List<Klient> SeznamKlientu, List<Spravce> SeznamSpravcu, Smlouva Smlouva) seznamy = (SeznamKlientu: seznamKlientu, SeznamSpravcu: seznamSpravcu, Smlouva: smlouva);
            return View(seznamy);
        }

        [HttpPost]
        public IActionResult Edit(Smlouva upravenaSmlouva, List<int> Spravci)
        {
            Smlouva staraSmlouva = _context.Smlouvy.FirstOrDefault(s => s.ID == upravenaSmlouva.ID);
            if (staraSmlouva == null) return NotFound();

            if (Spravci == null || Spravci.Count == 0 || upravenaSmlouva.DatumPlatnosti < DateTime.Now  || upravenaSmlouva.DatumUkonceni < DateTime.Now || upravenaSmlouva.DatumUkonceni < upravenaSmlouva.DatumPlatnosti)
            {
                return BadRequest(new { message = "Neplatne hodnoty" });
            }

            string[] casti = upravenaSmlouva.Klient.Split("|");

            int klientID = int.Parse(casti[0]);
            string klient = casti[1];

            staraSmlouva.Instituce = upravenaSmlouva.Instituce;
            staraSmlouva.Klient = klient;
            staraSmlouva.DatumPlatnosti = upravenaSmlouva.DatumPlatnosti;
            staraSmlouva.DatumUkonceni = upravenaSmlouva.DatumUkonceni;

            List<SmlouvaVazba> stareVazby = _context.SmlouvyVazby.Where(v => v.SmlouvaID == upravenaSmlouva.ID).ToList();
            _context.SmlouvyVazby.RemoveRange(stareVazby);

            if (Spravci != null)
            {
                foreach (int spravceId in Spravci)
                {
                    _context.SmlouvyVazby.Add(new SmlouvaVazba
                    {
                        SmlouvaID = upravenaSmlouva.ID,
                        SpravceID = spravceId,
                        KlientID = klientID
                    });
                }
            }
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
