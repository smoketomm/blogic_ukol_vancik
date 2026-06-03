namespace Blogic_ukol_vancik.Models
{
    public class SpravceViewModel:Uzivatel
    {
        public override bool jeSpravce { set => base.jeSpravce = true; }
    }
}
