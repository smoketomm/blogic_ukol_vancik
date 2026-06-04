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

            var novaSmlouva = new Smlouva
            {
                EvCislo = nahoda.Next(100, 1000),
                Instituce = data.Instituce,
                Klient = data.Klient,
                DatumUzavreni = DateTime.Now,
                DatumPlatnosti = data.DatumPlatnosti,
                DatumUkonceni = data.DatumUkonceni
            };



            _context.Smlouvy.Add(novaSmlouva);
            _context.SaveChanges();

            Console.WriteLine("Hotovo");
            Console.WriteLine("Pocet spravcu: " + data.Spravce[0]);

            return Ok(new { message = "uloženo úspěšně!" });
        }

        public IActionResult Create()
        {
            
            List<Klient> seznamKlientu = _context.Klienti.ToList();
            List<Spravce> seznamSpravcu = _context.Spravci.ToList();

            var seznamy = (SeznamKlientu: seznamKlientu, SeznamSpravcu: seznamSpravcu);

            return View(seznamy);
        }
    }
}
