using System.ComponentModel.DataAnnotations;

namespace Blogic_ukol_vancik.Models
{
    public class Uzivatel
    {
        [Key] public int ID { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string Email { get; set; }
        public int Telefon { get; set; }
        public int RodneCislo { get; set; }
        public int Vek {  get; set; }
        public bool JeSpravce { get; set; }
    }
}
