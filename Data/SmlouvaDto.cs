using Blogic_ukol_vancik.Models;
using System.Text.Json.Serialization;

namespace Blogic_ukol_vancik.Data
{
    public class SmlouvaDto: Smlouva
    {
        [JsonPropertyName("spravce")]
        public List<string>? Spravce { get; set; }
    }
}
