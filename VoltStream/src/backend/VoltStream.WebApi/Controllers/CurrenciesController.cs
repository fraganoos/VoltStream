namespace VoltStream.WebApi.Controllers;

using VoltStream.Application.Features.Currencies.Commands;
using VoltStream.Application.Features.Currencies.DTOs;
using VoltStream.Application.Features.Currencies.Queries;
using VoltStream.WebApi.Controllers.Common;

public class CurrenciesController
    : CrudController<CurrencyDto,
        GetAllCurrenciesQuery,
        GetCurrencyByIdQuery,
        CreateCurrencyCommand,
        UpdateCurrencyCommand,
        DeleteCurrencyCommand>;
