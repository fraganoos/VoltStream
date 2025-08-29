namespace VoltStream.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public class AppDbContext(DbContextOptions<AppDbContext> options)
                                        : DbContext(options), IAppDbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerOperation> CustomerOperations { get; set; }
    public DbSet<Cash> Cashes { get; set; }
    public DbSet<DebtKredit> DebtKredits { get; set; }
    public DbSet<Supply> Supplies { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }

    public Task<int> SaveAsync(CancellationToken cancellationToken)
        => SaveChangesAsync(cancellationToken);
}
