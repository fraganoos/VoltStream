namespace VoltStream.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltStream.Domain.Entities;

public class CustomerOperationConfiguration : IEntityTypeConfiguration<CustomerOperation>
{
    public void Configure(EntityTypeBuilder<CustomerOperation> builder)
    {
        builder.ToTable("CustomerOperations");
        builder.HasKey(co => co.Id);

        builder.HasOne(co => co.Account)
               .WithMany()
               .HasForeignKey(co => co.AccountId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(co => co.Sale)
               .WithOne(s => s.CustomerOperation)
               .HasForeignKey<Sale>(s => s.CustomerOperationId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(co => co.Payment)
               .WithOne(p => p.CustomerOperation)
               .HasForeignKey<Payment>(p => p.CustomerOperationId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);
    }
}