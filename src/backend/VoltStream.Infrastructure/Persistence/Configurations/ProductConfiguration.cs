namespace VoltStream.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltStream.Domain.Entities;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        builder.HasMany(p => p.Stocks)
               .WithOne(ws => ws.Product)
               .HasForeignKey(ws => ws.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<SaleItem>()
               .WithOne(si => si.Product)
               .HasForeignKey(si => si.ProductId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
