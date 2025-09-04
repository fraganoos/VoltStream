namespace VoltStream.Application.Features.Payments.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Payments.DTOs;
using VoltStream.Domain.Entities;

public record GetPaymentByIdQuery(long Id) : IRequest<PaymentDTO>;

public class GetPaymentByIdQueryHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<GetPaymentByIdQuery, PaymentDTO>
{
    public async Task<PaymentDTO> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        => mapper.Map<PaymentDTO>(await context.Payments
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken))
            ?? throw new NotFoundException(nameof(Payment), nameof(request.Id), request.Id);
}