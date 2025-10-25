namespace VoltStream.WebApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoltStream.Application.Features.Payments.Commands;
using VoltStream.Application.Features.Payments.DTOs;
using VoltStream.Application.Features.Payments.Queries;
using VoltStream.WebApi.Controllers.Common;
using VoltStream.WebApi.Models;

public class PaymentsController
    : CrudController<PaymentDto,
        GetAllPaymentsQuery,
        GetPaymentByIdQuery,
        CreatePaymentCommand,
        UpdatePaymentCommand,
        DeletePaymentCommand>
{

    [HttpPost("filter")]
    public async Task<IActionResult> GetFiltered(PaymentFilterQuery query)
        => Ok(new Response { Data = await Mediator.Send(query) });
}