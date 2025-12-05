namespace VoltStream.Application.Features.CustomerOperations.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Enums;

public record DeleteCustomerOperation(long Id) : IRequest<bool>;
public class DeleteCustomerOperationHandler(IAppDbContext context) 
    : IRequestHandler<DeleteCustomerOperation, bool>
{
    public async Task<bool> Handle(DeleteCustomerOperation request, CancellationToken cancellationToken)
    {
        var operationId = request.Id;

        // CustomerOperation ni OperationType bilan birga yuklaymiz
        var customerOp = await context.CustomerOperations
            .FirstOrDefaultAsync(co => co.Id == operationId, cancellationToken);

        if (customerOp == null)
            return false;

        using var transaction = await context.BeginTransactionAsync(cancellationToken);

        try
        {
            // OperationType ga qarab to‘g‘ri jadvaldan o‘chiramiz
            switch (customerOp.OperationType)
            {
                case OperationType.Sale:
                    // Sale ni topib o‘chiramiz
                    var sale = await context.Sales
                        .Include(s => s.Items) // agar SaleItem lar bo‘lsa
                        .FirstOrDefaultAsync(s => s.CustomerOperationId == operationId, cancellationToken);

                    if (sale != null)
                    {
                        context.SaleItems.RemoveRange(sale.Items);
                        context.Sales.Remove(sale);
                    }
                    break;

                case OperationType.Payment:
                    var payment = await context.Payments
                        .FirstOrDefaultAsync(p => p.CustomerOperationId == operationId, cancellationToken);

                    if (payment != null)
                        context.Payments.Remove(payment);
                    break;

                case OperationType.DiscountApplied:
                    var discount = await context.DiscountOperations
                        .FirstOrDefaultAsync(d => d.AccountId == operationId, cancellationToken);

                    if (discount != null)
                        context.DiscountOperations.Remove(discount);
                    break;

                // Agar yangi tur qo‘shilsa — shu yerga qo‘shiladi
                default:
                    // Hech narsa topilmasa — faqat CustomerOperation o‘chadi
                    break;
            }

            // Har doim CustomerOperation o‘chadi
            context.CustomerOperations.Remove(customerOp);

            await context.SaveAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }
    }
}
