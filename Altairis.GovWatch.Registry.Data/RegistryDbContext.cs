using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Altairis.GovWatch.Registry.Data {
    public class RegistryDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int> {

        public RegistryDbContext(DbContextOptions options) : base(options) {
        }

        public DbSet<WebSite> WebSites { get; set; }

        public DbSet<Category> Categories { get; set; }

    }
}
