using System.ComponentModel.DataAnnotations;

namespace Blogic_ukol_vancik.Models
{
    public class Smlouva
    {
        [Key] public int ID { get; set; }
        public int EvCislo { get; set; }
        public string Instituce { get; set; }
        public string Klient { get; set; }
        public DateTime? DatumUzavreni;
        public DateTime? DatumPlatnosti {  get; set; }
        public DateTime? DatumUkonceni { get; set; }
    }
}
