namespace VoltStream.Application.Features.Payments.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.Payments.DTOs;

public record GetAllPaymentsQuery : IRequest<List<PaymentDTO>>;

public class GetAllPaymentsQueryHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<GetAllPaymentsQuery, List<PaymentDTO>>
{
    public async Task<List<PaymentDTO>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
        => mapper.Map<List<PaymentDTO>>(await context.Payments.ToListAsync(cancellationToken));
}
