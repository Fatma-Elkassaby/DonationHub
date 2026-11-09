using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
    {
        public void Configure(EntityTypeBuilder<PaymentAttempt> builder) 
        {
            builder.HasKey(p =>p.Id);

            builder.Property(p => p.StripeCheckoutSessionId)
                .IsRequired();

            builder.Property(p => p.Status)
                .HasConversion<string>();

            builder.OwnsOne(p => p.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("PaymentAmount");

                money.Property(m => m.Currency)
                    .HasMaxLength(3)
                    .HasColumnName("PaymentCurrency");
            });

            builder.Navigation(p => p.Amount)
                .IsRequired();

            builder.HasOne<Donation>()
                .WithMany()
                .HasForeignKey(p => p.DonationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
