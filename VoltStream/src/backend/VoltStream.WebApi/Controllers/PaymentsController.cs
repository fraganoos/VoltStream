namespace VoltStream.WebApi.Controllers;

using VoltStream.Application.Features.Payments.Commands;
using VoltStream.Application.Features.Payments.DTOs;
using VoltStream.Application.Features.Payments.Queries;
using VoltStream.WebApi.Controllers.Common;

public class PaymentsController
    : CrudController<PaymentDto,
        GetAllPaymentsQuery,
        GetPaymentByIdQuery,
        CreatePaymentCommand,
        UpdatePaymentCommand,
        DeletePaymentCommand>;