
using Blogic_ukol_vancik.Models;
using Microsoft.EntityFrameworkCore;

namespace Blogic_ukol_vancik.Data
{
    public class MostDataDbContext : DbContext
    {
        public MostDataDbContext(DbContextOptions<MostDataDbContext> options) : base(options) { }

        public DbSet<Klienti> Klient {  get; set; }
    }
}
