using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder) 
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.TokenHash)
                .IsRequired();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<RefreshToken>()
                .WithOne()
                .HasForeignKey<RefreshToken>(r => r.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
