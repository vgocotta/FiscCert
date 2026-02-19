using FiscCert.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiscCert.Infrastructure.Persistence.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificates");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.TenantId)
            .IsRequired();

        builder.Property(c => c.BlobPath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.OwnerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.FederalInscription)
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(c => c.SerialNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.ExpirationDate)
            .IsRequired();

        builder.Property(c => c.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.EconomicGroupId)
            .IsRequired(false);
    }
}
