﻿namespace VoltStream.Application.Features.Payments.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Models;
using VoltStream.Application.Features.Payments.DTOs;

public record PaymentFilterQuery : FilteringRequest, IRequest<IReadOnlyCollection<PaymentDto>>;

public class PaymentFilterQueryHandler(
    IAppDbContext context,
    IPagingMetadataWriter writer,
    IMapper mapper) : IRequestHandler<PaymentFilterQuery, IReadOnlyCollection<PaymentDto>>
{
 
    public async Task<IReadOnlyCollection<PaymentDto>> Handle(PaymentFilterQuery request, CancellationToken cancellationToken)
    => mapper.Map<IReadOnlyCollection<PaymentDto>>(await context.Payments
            .ToPagedListAsync(request, writer, cancellationToken));
}
