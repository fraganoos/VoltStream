namespace VoltStream.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;
using VoltStream.Infrastructure.Persistence.Interceptors;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IAppDbContext
{
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<AllowedClient> AllowedClients { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerOperation> CustomerOperations { get; set; }
    public DbSet<Cash> Cashes { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Supply> Supplies { get; set; }
    public DbSet<WarehouseStock> WarehouseStocks { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<DiscountOperation> DiscountOperations { get; set; }
    public DbSet<User> Users { get; set; }

    private IDbContextTransaction? currentTransaction;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditInterceptor());
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (currentTransaction is not null)
            return currentTransaction;

        currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
        return currentTransaction;
    }

    public async Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        bool isSuccess;
        try
        {
            await SaveChangesAsync(cancellationToken);
            if (currentTransaction is not null)
                await currentTransaction.CommitAsync(cancellationToken);
            isSuccess = true;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (currentTransaction is not null)
            {
                await currentTransaction.DisposeAsync();
                currentTransaction = null;
            }
        }

        return isSuccess;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (currentTransaction is not null)
        {
            await currentTransaction.RollbackAsync(cancellationToken);
            await currentTransaction.DisposeAsync();
            currentTransaction = null;
        }
    }

    public Task<int> SaveAsync(CancellationToken cancellationToken)
        => SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var staticSalt = new byte[]
    {
        207, 59, 204, 225, 8, 63, 123, 97, 231, 188, 115, 95, 151, 168,
        158, 111, 135, 209, 47, 185, 19, 65, 31, 171, 177, 241, 34, 1,
        136, 205, 249, 166, 77, 56, 219, 250, 241, 48, 162, 224, 60, 65,
        232, 2, 43, 232, 17, 83, 130, 85, 7, 41, 168, 205, 9, 254, 157,
        74, 111, 84, 173, 35, 146, 128, 125, 19, 4, 52, 5, 72, 57, 143,
        112, 91, 154, 44, 100, 192, 197, 33, 51, 61, 166, 200, 19, 114,
        95, 45, 77, 95, 151, 118, 218, 47, 4, 69, 97, 202, 40, 25, 160,
        30, 199, 131, 47, 186, 247, 246, 159, 118, 112, 158, 253, 154,
        201, 238, 44, 175, 168, 173, 234, 84, 20, 248, 48, 90, 16, 94
    };

        var staticHash = new byte[]
        {
        118, 62, 216, 239, 235, 45, 161, 97, 101, 27, 77, 239, 76, 120,
        192, 237, 102, 224, 166, 22, 78, 113, 126, 59, 2, 187, 182, 251,
        18, 12, 237, 95, 2, 90, 53, 5, 105, 3, 105, 243, 28, 188, 83, 24,
        133, 200, 170, 108, 124, 147, 109, 18, 85, 208, 89, 19, 16, 32,
        60, 26, 251, 4, 28, 82
        };

        builder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                NormalizedUsername = "ADMIN",
                PasswordHash = staticHash,
                PasswordSalt = staticSalt,
                CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
            });
    }
}
