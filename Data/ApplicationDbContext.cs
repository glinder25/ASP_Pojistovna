using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PojistovnaApp.Models;

namespace PojistovnaApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pojistenec> Pojistenci { get; set; }
        public DbSet<Pojisteni> Pojisteni { get; set; }
        public DbSet<PojistnaUdalost> PojistneUdalosti { get; set; }

    }
}