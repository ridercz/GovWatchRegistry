using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Altairis.GovWatch.Registry.Data {
    public class RegistryDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int> {

        public RegistryDbContext(DbContextOptions options) : base(options) {
        }

        public DbSet<WebSite> WebSites { get; set; }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<WebSite>().HasOne<ApplicationUser>(x => x.CreatedBy).WithMany().HasForeignKey(x => x.CreatedById).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WebSite>().HasOne<ApplicationUser>(x => x.UpdatedBy).WithMany().HasForeignKey(x => x.UpdatedById).OnDelete(DeleteBehavior.Restrict);
        }

    }
}
