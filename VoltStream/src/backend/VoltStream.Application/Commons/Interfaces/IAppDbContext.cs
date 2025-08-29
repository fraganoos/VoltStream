namespace VoltStream.Application.Commons.Interfaces;

using Microsoft.EntityFrameworkCore;
using VoltStream.Domain.Entities;

public interface IAppDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Sale> Sales { get; }
    DbSet<SaleItem> SaleItems { get; }
    DbSet<Customer> Customers { get; }
    DbSet<CustomerOperation> CustomerOperations { get; }
    DbSet<Cash> Cashes { get; }
    DbSet<DebtKredit> DebtKredits { get; }
    DbSet<Supply> Supplies { get; }
    DbSet<Warehouse> Warehouses { get; }

    Task<int> SaveAsync(CancellationToken cancellationToken);
}
