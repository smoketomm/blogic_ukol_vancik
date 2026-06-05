using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blogic_ukol_vancik.Models
{
    [PrimaryKey(nameof(SmlouvaID), nameof(SpravceID), nameof(KlientID))]
    public class SmlouvaVazba
    {
        public int SmlouvaID { get; set; }
        public int SpravceID { get; set; }
        public int KlientID { get; set; }
    }
}
