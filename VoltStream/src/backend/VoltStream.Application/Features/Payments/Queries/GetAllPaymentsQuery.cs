namespace VoltStream.Application.Features.Payments.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Payments.DTOs;

public record GetAllPaymentsQuery : IRequest<IReadOnlyCollection<PaymentDTO>>;

public class GetAllPaymentsQueryHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<GetAllPaymentsQuery, IReadOnlyCollection<PaymentDTO>>
{
    public async Task<IReadOnlyCollection<PaymentDTO>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
        => mapper.Map<IReadOnlyCollection<PaymentDTO>>(await context.Payments.ToListAsync(cancellationToken));
}
