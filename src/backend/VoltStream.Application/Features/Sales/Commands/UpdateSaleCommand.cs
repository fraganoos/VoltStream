namespace VoltStream.Application.Features.Sales.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;

public record UpdateSaleCommand(
    long Id,
    DateTimeOffset Date,
    long CustomerId,
    long CurrencyId,
    int RollCount,
    decimal Length,
    decimal Discount,
    bool IsApplied,
    decimal Amount,
    string Description,
    List<SaleItem> SaleItems)
    : IRequest<bool>;

public class UpdateSaleCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<UpdateSaleCommand, bool>
{
    public async Task<bool> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        await context.BeginTransactionAsync(cancellationToken);

        try
        {
            var sale = await GetSaleAsync(request.Id, cancellationToken);
            var warehouse = await GetWarehouseAsync(cancellationToken);
            var customer = await GetCustomerWithAccountAsync(request.CustomerId, request.CurrencyId, cancellationToken);
            var account = customer.Accounts.First(a => a.CurrencyId == request.CurrencyId);

            // 1. Eski savdoni revert qilish
            await RevertOldSaleAsync(sale, warehouse, account, cancellationToken);

            // 2. Yangilangan savdoni apply qilish
            await ApplyUpdatedSaleAsync(request, sale, warehouse, account, mapper, cancellationToken);

            return true;
        }
        catch
        {
            await context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<Sale> GetSaleAsync(long saleId, CancellationToken cancellationToken)
    {
        return await context.Sales
            .Include(s => s.CustomerOperation)
            .Include(s => s.Customer)
                .ThenInclude(c => c!.Accounts)
            .Include(s => s.Discount)
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == saleId, cancellationToken)
            ?? throw new NotFoundException(nameof(Sale), nameof(saleId), saleId);
    }

    private async Task<Warehouse> GetWarehouseAsync(CancellationToken cancellationToken)
    {
        return await context.Warehouses
            .Include(w => w.Stocks)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Warehouse));
    }

    private async Task<Customer> GetCustomerWithAccountAsync(long customerId, long currencyId, CancellationToken cancellationToken)
    {
        var customer = await context.Customers
            .Include(c => c.Accounts)
            .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer));

        if (!customer.Accounts.Any(a => a.CurrencyId == currencyId))
            throw new NotFoundException(nameof(Account), "CurrencyId", currencyId);

        return customer;
    }

    private async Task RevertOldSaleAsync(Sale sale, Warehouse warehouse, Account account, CancellationToken cancellationToken)
    {
        foreach (var item in sale.Items)
        {
            var residue = warehouse.Stocks.FirstOrDefault(r => r.ProductId == item.ProductId && r.LengthPerRoll == item.LengthPerRoll)
                ?? throw new NotFoundException(nameof(WarehouseStock), nameof(item.Id), item.Id);

            var countRoll = (int)item.TotalLength / item.LengthPerRoll;
            var residueItem = item.TotalLength % item.LengthPerRoll;

            residue.RollCount += countRoll;
            residue.TotalLength += item.TotalLength - residueItem;

            if (residueItem > 0)
                await AddOrUpdateResidueItemAsync(item, residueItem, warehouse, cancellationToken);
        }

        // Accountni revert qilish faqat IsApplied = false bo'lsa
        if (!sale.DiscountOperation!.IsApplied)
        {
            account.Balance += sale.Amount;
            account.Discount -= sale.Discount;
        }
    }

    private async Task AddOrUpdateResidueItemAsync(SaleItem item, decimal residueItem, Warehouse warehouse, CancellationToken cancellationToken)
    {
        var existItem = await context.WarehouseStocks
            .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.LengthPerRoll == residueItem, cancellationToken);

        if (existItem is null)
            context.WarehouseStocks.Add(new WarehouseStock
            {
                RollCount = 1,
                ProductId = item.ProductId,
                LengthPerRoll = residueItem,
                DiscountRate = item.DiscountRate,
                UnitPrice = item.UnitPrice,
                TotalLength = residueItem,
                Warehouse = warehouse
            });
        else
        {
            existItem.RollCount += 1;
            existItem.DiscountRate = item.DiscountRate;
            existItem.UnitPrice = item.UnitPrice;
            existItem.TotalLength += residueItem;
        }
    }

    private async Task ApplyUpdatedSaleAsync(UpdateSaleCommand request, Sale sale, Warehouse warehouse, Account account, IMapper mapper, CancellationToken cancellationToken)
    {
        var descriptionBuilder = new StringBuilder();

        foreach (var item in request.SaleItems)
            await ProcessSaleItemAsync(item, warehouse, descriptionBuilder, cancellationToken);

        // Customer update faqat IsApplied = false bo'lsa
        if (!request.IsApplied)
        {
            account.Balance -= request.Amount;
            account.Discount += request.Discount;
        }

        // Sale map qilish
        mapper.Map(request, sale);

        // CustomerOperation
        var customerOperation = new CustomerOperation
        {
            Amount = request.Amount,
            Account = account,
            AccountId = account.Id,
            OperationType = OperationType.Sale,
            Description = $"Savdo ID = {sale.Id}: {request.Description}. {descriptionBuilder}".Trimmer(200)
        };
        sale.CustomerOperation = customerOperation;

        // DiscountOperation
        sale.DiscountOperation!.Date = request.Date;
        sale.DiscountOperation.Amount = request.Discount;
        sale.DiscountOperation.IsApplied = request.IsApplied;
        sale.DiscountOperation.Description = $"Chegirma savdo uchun: {descriptionBuilder}";

        await context.CommitTransactionAsync(cancellationToken);
    }

    private async Task ProcessSaleItemAsync(SaleItem item, Warehouse warehouse, StringBuilder descriptionBuilder, CancellationToken cancellationToken)
    {
        var residue = warehouse.Stocks.FirstOrDefault(r => r.ProductId == item.ProductId && r.LengthPerRoll == item.LengthPerRoll)
            ?? throw new NotFoundException(nameof(WarehouseStock), nameof(item.Id), item.Id);

        if (residue.TotalLength < item.TotalLength)
            throw new ConflictException($"Omborda faqat {residue.TotalLength} metr mahsulot bor");

        residue.RollCount -= item.RollCount;
        residue.TotalLength -= item.TotalLength;

        if (item.LengthPerRoll * item.RollCount != item.TotalLength)
        {
            var detail = item.LengthPerRoll * item.RollCount - item.TotalLength;
            var existItem = await context.WarehouseStocks
                .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.LengthPerRoll == detail, cancellationToken);

            if (existItem is null)
                context.WarehouseStocks.Add(new WarehouseStock
                {
                    RollCount = 1,
                    ProductId = item.ProductId,
                    LengthPerRoll = detail,
                    DiscountRate = item.DiscountRate,
                    UnitPrice = item.UnitPrice,
                    TotalLength = detail,
                    Warehouse = warehouse
                });
            else
            {
                existItem.RollCount += 1;
                existItem.DiscountRate = item.DiscountRate;
                existItem.UnitPrice = item.UnitPrice;
                existItem.TotalLength += detail;
            }
        }

        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), nameof(item.Id), item.ProductId);

        descriptionBuilder.Append($"{product.Name} - {item.TotalLength} metr; ");
    }
}
