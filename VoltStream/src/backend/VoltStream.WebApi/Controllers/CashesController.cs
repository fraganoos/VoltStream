namespace VoltStream.WebApi.Controllers;

using VoltStream.Application.Features.Cashes.Commands;
using VoltStream.Application.Features.Cashes.DTOs;
using VoltStream.Application.Features.Cashes.Queries;
using VoltStream.WebApi.Controllers.Common;

public class CashesController
    : CrudController<CashDto,
    GetAllCashesQuery,
    GetCashByIdQuery,
    CreateCashCommand,
    UpdateCashCommand,
    DeleteCashCommand>;