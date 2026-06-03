namespace Blogic_ukol_vancik.Models
{
    public class Uzivatel
    {
        public string jmeno { get; set; }
        public string prijmeni { get; set; }
        public string email { get; set; }
        public string telefon { get; set; }
        public int rodneCislo { get; set; }
        public int vek {  get; set; }
        public virtual bool jeSpravce { get; set; }
    }
}
