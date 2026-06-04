
using Blogic_ukol_vancik.Models;
using Microsoft.EntityFrameworkCore;

namespace Blogic_ukol_vancik.Data
{
    public class MostDataDbContext : DbContext
    {
        public MostDataDbContext(DbContextOptions<MostDataDbContext> options) : base(options) { }

        public DbSet<Klient> Klienti {  get; set; }
        public DbSet<Spravce> Spravci {  get; set; }
        public DbSet<Smlouva> Smlouvy {  get; set; }
    }
}
