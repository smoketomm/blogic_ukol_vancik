namespace Blogic_ukol_vancik.Models
{
    public class Spravci:Uzivatel
    {
        public override bool jeSpravce { set => base.jeSpravce = true; }
    }
}
