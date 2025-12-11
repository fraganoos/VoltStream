namespace VoltStream.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Payments.Commands;
using VoltStream.Application.Features.Payments.DTOs;
using VoltStream.Application.Features.Payments.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class PaymentsController
    : ReadOnlyController<PaymentDto,
        GetAllPaymentsQuery,
        GetPaymentByIdQuery>
{
    [HttpPost]
    public async Task<IActionResult> Create(CreatePaymentCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpPut]
    public async Task<IActionResult> Update(UpdatePaymentCommand command)
        => Ok(new Response { Data = await Mediator.Send(command) });

    [HttpPost("filter")]
    public async Task<IActionResult> GetFiltered(PaymentFilterQuery query)
        => Ok(new Response { Data = await Mediator.Send(query) });
}