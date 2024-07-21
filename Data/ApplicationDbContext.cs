using Microsoft.EntityFrameworkCore;
using MansorySupplyHub.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MansorySupplyHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationType> ApplicationTypes { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
