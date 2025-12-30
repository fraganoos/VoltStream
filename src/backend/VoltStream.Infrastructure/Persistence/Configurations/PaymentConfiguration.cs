namespace VoltStream.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltStream.Domain.Entities;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.CustomerOperation)
               .WithOne(co => co.Payment)
               .HasForeignKey<Payment>(p => p.CustomerOperationId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Currency)
                .WithMany()
                .HasForeignKey(p => p.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Customer)
                .WithMany()
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}
