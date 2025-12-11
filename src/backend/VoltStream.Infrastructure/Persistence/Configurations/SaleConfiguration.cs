namespace VoltStream.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltStream.Domain.Entities;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        builder.HasKey(s => s.Id);

        builder.HasMany(s => s.Items)
               .WithOne(si => si.Sale)
               .HasForeignKey(si => si.SaleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.DiscountOperation)
               .WithOne(dope => dope.Sale)
               .HasForeignKey<Sale>(s => s.DiscountOperationId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.CustomerOperation)
               .WithOne(co => co.Sale)
               .HasForeignKey<Sale>(s => s.CustomerOperationId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Currency)
               .WithMany()
               .HasForeignKey(s => s.CurrencyId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Customer)
               .WithMany()
               .HasForeignKey(s => s.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}