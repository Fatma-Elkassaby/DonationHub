using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DonationHub.Domain.Entities;

namespace DonationHub.Infrastructure.Persistence
{
    public class DonationHubDbContext : IdentityDbContext<ApplicationUser,IdentityRole<Guid>,Guid>
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Campaign> Campaigns => Set<Campaign>();
        public DbSet<Donation> Donations => Set<Donation>();
        public DbSet<PaymentAttempt> PaymentAttempts => Set<PaymentAttempt>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<ProcessedStripeEvent> ProcessedStripeEvents => Set<ProcessedStripeEvent>();
        public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
        public DonationHubDbContext(DbContextOptions<DonationHubDbContext> options): base(options)
        {}
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(
                typeof(DonationHubDbContext).Assembly);

            builder.Entity<Campaign>()
                .HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
