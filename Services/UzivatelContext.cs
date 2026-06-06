using Blogic_ukol_vancik.Models;
using System.Security.Claims;

namespace Blogic_ukol_vancik.Services
{
    public class UzivatelContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UzivatelContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public PrihlasenyUzivatel aktualniUzivatel
        {
            get
            {
                var uzivatel = _httpContextAccessor.HttpContext?.User;

                if (uzivatel == null ||!uzivatel.Identity.IsAuthenticated)
                {
                    return null;
                }

                return new PrihlasenyUzivatel
                {
                    ID = int.Parse(uzivatel.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                    Jmeno = uzivatel.Identity.Name,
                    Prijmeni = uzivatel.FindFirst("Prijmeni")?.Value,
                    JeSpravce = bool.Parse(uzivatel.FindFirst("JeSpravce")?.Value ?? "false")
                };
            }
        }
    }
}
