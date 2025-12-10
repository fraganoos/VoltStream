namespace VoltStream.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoltStream.Domain.Entities;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(a => a.Id);

        builder.HasMany<CustomerOperation>()
               .WithOne(co => co.Account)
               .HasForeignKey(co => co.AccountId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<DiscountOperation>()
               .WithOne(co => co.Account)
               .HasForeignKey(co => co.AccountId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Currency)
               .WithMany()
               .HasForeignKey(a => a.CurrencyId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}