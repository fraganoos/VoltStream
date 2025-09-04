namespace VoltStream.Application.Commons.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
    DbSet<Account> Accounts { get; }
    DbSet<Supply> Supplies { get; }
    DbSet<WarehouseItem> WarehouseItems { get; }
    DbSet<Warehouse> Warehouses { get; }
    DbSet<DiscountOperation> DiscountsOperations { get; }
    DbSet<CashOperation> CashOperations { get; }

    Task<int> SaveAsync(CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
