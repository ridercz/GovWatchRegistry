using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Altairis.GovWatch.Registry.Data {
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RegistryDbContext> {
        public RegistryDbContext CreateDbContext(string[] args) {
            var builder = new DbContextOptionsBuilder<RegistryDbContext>();
            builder.UseSqlServer("SERVER=.\\SqlExpress;TRUSTED_CONNECTION=yes;DATABASE=GovWatchRegistry_design");
            return new RegistryDbContext(builder.Options);
        }
    }
}
