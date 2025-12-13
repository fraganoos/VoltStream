namespace VoltStream.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltStream.Domain.Entities;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);

        builder.HasMany(c => c.Accounts)
               .WithOne(a => a.Customer)
               .HasForeignKey(a => a.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.CustomerOperations)
               .WithOne()
               .HasForeignKey(co => co.CustomerId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
