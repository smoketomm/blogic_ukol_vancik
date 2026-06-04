namespace Blogic_ukol_vancik.Models
{
    public class SmlouvaViewModel
    {
        public int evCislo { get; set; }
        public string instituce { get; set; }
        public string klient { get; set; }
        public required List<string> SpravciID { get; set; }
        public DateTime datumUzavreni { get; set; }
        public DateTime datumPlatnosti {  get; set; }
        public DateTime datumUkonceni { get; set; }

    }
}
