using Microsoft.AspNetCore.Mvc;

namespace Blogic_ukol_vancik.Controllers
{
    public class SmlouvaController : Controller
    {
        private readonly ILogger<SmlouvaController> _logger;
        public SmlouvaController(ILogger<SmlouvaController> logger)
        {
            _logger = logger;
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
