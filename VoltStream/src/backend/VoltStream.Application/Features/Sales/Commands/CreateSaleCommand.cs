namespace VoltStream.Application.Features.Sales.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Sales.DTOs;
using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

public record CreateSaleCommand(
    DateTimeOffset Date,
    long? CustomerId,
    long CurrencyId,
    int RollCount,
    decimal Length,
    decimal Amount,
    bool IsApplied,
    decimal Discount,
    string Description,
    List<SaleItemCommandDto> Items)
    : IRequest<long>;

public class CreateSaleCommandHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<CreateSaleCommand, long>
{
    public async Task<long> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        await context.BeginTransactionAsync(cancellationToken);

        try
        {
            var warehouse = await GetWarehouseAsync(cancellationToken);
            var customer = await GetCustomerAsync(request.CustomerId, cancellationToken);

            var descriptionBuilder = new StringBuilder();

            await ProcessSaleItemsAsync(request.Items, warehouse, descriptionBuilder, cancellationToken);
            var sale = mapper.Map<Sale>(request);


            if (customer is not null)
            {
                var account = customer.Accounts.FirstOrDefault(a => a.CurrencyId == request.CurrencyId);

                UpdateAccountBalance(account, sale.Amount, sale.Discount, request.IsApplied);
                sale.CustomerOperation = CreateCustomerOperation(sale, account, request.Description, descriptionBuilder);
                sale.DiscountOperation = CreateDiscountOperation(request, account, descriptionBuilder);
            }

            context.Sales.Add(sale);
            await context.CommitTransactionAsync(cancellationToken);

            return sale.Id;
        }
        catch
        {
            await context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<Warehouse> GetWarehouseAsync(CancellationToken cancellationToken)
    {
        return await context.Warehouses
            .Include(w => w.Stocks)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Warehouse));
    }

    private async Task<Customer?> GetCustomerAsync(long? customerId, CancellationToken cancellationToken)
    {
        if (customerId is null)
            return default;
        var customer = await context.Customers
            .Include(c => c.Accounts)
            .FirstOrDefaultAsync(a => a.Id == customerId, cancellationToken);

        return customer;
    }

    private async Task ProcessSaleItemsAsync(
        List<SaleItemCommandDto> saleItems,
        Warehouse warehouse,
        StringBuilder descriptionBuilder,
        CancellationToken cancellationToken)
    {
        foreach (var item in saleItems)
        {
            var residue = warehouse.Stocks
                .FirstOrDefault(r => r.ProductId == item.ProductId && r.LengthPerRoll == item.LengthPerRoll)
                ?? throw new NotFoundException(nameof(WarehouseStock), nameof(item.Id), item.Id);

            if (residue.TotalLength < item.TotalLength)
                throw new ConflictException($"Omborda faqat {residue.TotalLength} metr mahsulot bor");

            residue.RollCount -= item.RollCount;
            residue.TotalLength -= item.RollCount * item.LengthPerRoll;

            await HandleResidueAsync(item, warehouse, cancellationToken);

            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken)
                ?? throw new NotFoundException(nameof(Product), nameof(item.Id), item.ProductId);

            descriptionBuilder.Append($"{product.Name} - {item.TotalLength} metr; ");
        }
    }

    private async Task HandleResidueAsync(SaleItemCommandDto item, Warehouse warehouse, CancellationToken cancellationToken)
    {
        if (item.LengthPerRoll * item.RollCount != item.TotalLength)
        {
            var detail = item.LengthPerRoll * item.RollCount - item.TotalLength;
            var existStock = await context.WarehouseStocks
                .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.LengthPerRoll == detail, cancellationToken);

            if (existStock is null)
                context.WarehouseStocks.Add(new WarehouseStock
                {
                    RollCount = 1,
                    ProductId = item.ProductId,
                    LengthPerRoll = detail,
                    UnitPrice = item.UnitPrice,
                    TotalLength = detail,
                    Warehouse = warehouse
                });
            else
            {
                existStock.RollCount += 1;
                existStock.UnitPrice = item.UnitPrice;
                existStock.TotalLength += detail;
            }
        }
    }

    private static DiscountOperation CreateDiscountOperation(CreateSaleCommand request, Account account, StringBuilder description)
    {
        return new DiscountOperation
        {
            Date = request.Date.ToOffset(TimeSpan.Zero),
            Amount = request.Discount,
            IsApplied = request.IsApplied,
            CustomerId = account.CustomerId,
            Description = $"Chegirma savdo uchun: {description}",
            Account = account
        };
    }

    private static void UpdateAccountBalance(Account account, decimal amount, decimal discount, bool isApplied)
    {
        account.Balance -= amount;
        if (!isApplied)
            account.Discount += discount;
    }

    private static CustomerOperation CreateCustomerOperation(Sale sale, Account account, string description, StringBuilder descriptionBuilder)
    {
        return new CustomerOperation
        {
            Date = sale.Date,
            Amount = sale.Amount,
            Account = account,
            AccountId = account.Id,
            CustomerId = sale.CustomerId,
            OperationType = OperationType.Sale,
            Description = $"Savdo ID = {sale.Id}: {description}. {descriptionBuilder}".Trimmer(200)
        };
    }
}
