using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public class DonationConfiguration : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation>builder) 
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Status)
                .HasConversion<string>();

            builder.OwnsOne(d => d.Amount, money =>
            {
                money.Property(m => m.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("DonationAmount");

                money.Property(m => m.Currency)
                .HasMaxLength(3)
                .HasColumnName("DonationCurrency");
            });
            builder.Navigation(d => d.Amount).IsRequired();

            builder.HasOne<Campaign>()
                .WithMany()
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(d => d.DonorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
