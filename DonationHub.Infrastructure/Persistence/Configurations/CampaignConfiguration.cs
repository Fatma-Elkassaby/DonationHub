using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder) 
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.TitleAr).IsRequired();
            builder.Property(c => c.TitleEn).IsRequired();
            builder.Property(c => c.DescriptionAr).IsRequired();
            builder.Property(c => c.DescriptionEn).IsRequired();

            builder.Property(c => c.Status)
                .HasConversion<string>();

            builder.Property(c => c.RowVersion)
                .IsRowVersion();

            builder.OwnsOne(c => c.GoalAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("GoalAmount");
                money.Property(m => m.Currency)
                    .HasMaxLength(3)
                    .HasColumnName("GoalCurrency");
            });

            builder.OwnsOne(c => c.RaisedAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("RaisedAmount");

                money.Property(m => m.Currency)
                    .HasMaxLength(3)
                    .HasColumnName("RaisedCurrency");
            });
            builder.Navigation(c => c.GoalAmount).IsRequired();
            builder.Navigation(c => c.RaisedAmount).IsRequired();
            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
