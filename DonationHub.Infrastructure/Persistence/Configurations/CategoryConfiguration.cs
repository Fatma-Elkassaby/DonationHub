using Microsoft.EntityFrameworkCore;
using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category>builder) 
        {
            builder.HasKey(c =>c.Id);

            builder.Property(c => c.NameAr)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.NameEn)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => c.NameAr)
                .IsUnique();

            builder.HasIndex(c => c.NameEn)
                .IsUnique();
        }
    }
}
