using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using powerr.Api.Models.Entities.User;
using powerr.Models.Entities.Meter;
using powerr.Models.Entities.MeterToken;
using powerr.Models.Entities.Wallet;
using powerr.repository.Configuration;

namespace powerr.Api.repository
{
    public class RepositoryContext : IdentityDbContext<AppUser>
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options): base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            //These populates the database on every migration
            builder.ApplyConfiguration(new RoleConfiguration());
            base.OnModelCreating(builder);
        }

        public DbSet<MeterRequest> meterRequests { get; set; }
        public DbSet<Meter> meters { get; set; }

        public DbSet<RechargeToken> rechargeTokens { get; set; }

        public DbSet<Wallet> wallets { get; set; }

    }
}
