namespace Blogic_ukol_vancik.Models
{
    public class KlientViewModel:Uzivatel
    {
        public override bool jeSpravce {set => base.jeSpravce = false; }
    }
}
