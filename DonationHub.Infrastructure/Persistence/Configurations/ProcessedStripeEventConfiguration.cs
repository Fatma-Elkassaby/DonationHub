using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public class ProcessedStripeEventConfiguration : IEntityTypeConfiguration<ProcessedStripeEvent>
    {
        public void Configure(EntityTypeBuilder<ProcessedStripeEvent> builder)
        {
            builder.HasKey(p => p.EventId);

            builder.Property(p => p.Type)
                .IsRequired();

            builder.Property(p => p.ProcessedAt)
                .IsRequired();
        }
    }
}
