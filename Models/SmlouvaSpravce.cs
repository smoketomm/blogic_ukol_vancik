using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blogic_ukol_vancik.Models
{
    [PrimaryKey(nameof(SmlouvaID), nameof(SpravceID))]
    public class SmlouvaSpravce
    {
        public int SmlouvaID { get; set; }
        public int SpravceID { get; set; }
    }
}
